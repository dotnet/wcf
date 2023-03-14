// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace System.ServiceModel.Channels
{
    delegate void OverlappedIOCompleteCallback(bool haveResult, int error, int bytesRead);

    unsafe class OverlappedContext
    {
        const int HandleOffsetFromOverlapped32 = -4;
        const int HandleOffsetFromOverlapped64 = -3;

        static IOCompletionCallback s_completeCallback;
        static WaitOrTimerCallback s_eventCallback;
        static WaitOrTimerCallback s_cleanupCallback;
        static byte[] s_dummyBuffer = new byte[0];

        object[] _bufferHolder;
        byte* _bufferPtr;
        NativeOverlapped* _nativeOverlapped;
        GCHandle _pinnedHandle;
        object _pinnedTarget;
        Overlapped _overlapped;
        RootedHolder _rootedHolder;
        OverlappedIOCompleteCallback _pendingCallback;  // Null when no async I/O is pending.
        bool _deferredFree;
        bool _syncOperationPending;
        ManualResetEvent _completionEvent;
        IntPtr _eventHandle;

        // Only used by unbound I/O.
        RegisteredWaitHandle _registration;

#if DEBUG_EXPENSIVE
        StackTrace freeStack;
#endif

        [SecuritySafeCritical]
        public OverlappedContext()
        {
            if (OverlappedContext.s_completeCallback == null)
            {
                OverlappedContext.s_completeCallback = Fx.ThunkCallback(new IOCompletionCallback(CompleteCallback));
            }
            if (OverlappedContext.s_eventCallback == null)
            {
                OverlappedContext.s_eventCallback = Fx.ThunkCallback(new WaitOrTimerCallback(EventCallback));
            }
            if (OverlappedContext.s_cleanupCallback == null)
            {
                OverlappedContext.s_cleanupCallback = Fx.ThunkCallback(new WaitOrTimerCallback(CleanupCallback));
            }

            this._bufferHolder = new object[] { OverlappedContext.s_dummyBuffer };
            this._overlapped = new Overlapped();
            this._nativeOverlapped = this._overlapped.UnsafePack(OverlappedContext.s_completeCallback, this._bufferHolder);

            // When replacing the buffer, we need to provoke the CLR to fix up the handle of the pin.
            try
            {
                this._pinnedHandle = GCHandle.FromIntPtr(*((IntPtr*)_nativeOverlapped +
                (IntPtr.Size == 4 ? HandleOffsetFromOverlapped32 : HandleOffsetFromOverlapped64)));
                //this._pinnedTarget = this._pinnedHandle.Target;
            }
            catch (InvalidOperationException) //GCHandle.FromIntPtr intermittently throws: Handle is not initialized.
            {
            }
            
            // Create the permanently rooted holder and put it in the Overlapped.
            this._rootedHolder = new RootedHolder();
            this._overlapped.AsyncResult = _rootedHolder;
        }

        [SecuritySafeCritical]
        ~OverlappedContext()
        {
            if (this._nativeOverlapped != null && !AppDomain.CurrentDomain.IsFinalizingForUnload() && !Environment.HasShutdownStarted)
            {
                if (this._syncOperationPending)
                {
                    Fx.Assert(this._rootedHolder != null, "rootedHolder null in Finalize.");
                    Fx.Assert(this._rootedHolder.EventHolder != null, "rootedHolder.EventHolder null in Finalize.");
                    Fx.Assert(OverlappedContext.s_cleanupCallback != null, "cleanupCallback null in Finalize.");

                    // Can't free the overlapped.  Register a callback to deal with this.
                    // This will ressurect the OverlappedContext.
                    // The completionEvent will still be alive (not finalized) since it's rooted by the pending Overlapped in the holder.
                    // We own it now and will close it in the callback.
                    ThreadPool.UnsafeRegisterWaitForSingleObject(this._rootedHolder.EventHolder, OverlappedContext.s_cleanupCallback, this, Timeout.Infinite, true);
                }
                else
                {
                    Overlapped.Free(this._nativeOverlapped);
                }
            }
        }

        // None of the OverlappedContext methods are threadsafe.
        // Free or FreeOrDefer can only be called once.  FreeIfDeferred can be called any number of times, as long as it's only
        // called once after FreeOrDefer.
        [SecuritySafeCritical]
        public void Free()
        {
            if (this._pendingCallback != null)
            {
                throw Fx.AssertAndThrow("OverlappedContext.Free called while async operation is pending.");
            }
            if (this._syncOperationPending)
            {
                throw Fx.AssertAndThrow("OverlappedContext.Free called while sync operation is pending.");
            }
            if (this._nativeOverlapped == null)
            {
                throw Fx.AssertAndThrow("OverlappedContext.Free called multiple times.");
            }

#if DEBUG_EXPENSIVE
            this.freeStack = new StackTrace();
#endif

            // The OverlappedData is cached and reused.  It looks weird if there's still a reference to it form here.
            this._pinnedTarget = null;

            NativeOverlapped* nativePtr = this._nativeOverlapped;
            this._nativeOverlapped = null;
            Overlapped.Free(nativePtr);

            if (this._completionEvent != null)
            {
                this._completionEvent.Close();
            }

            GC.SuppressFinalize(this);
        }

        [SecuritySafeCritical]
        public bool FreeOrDefer()
        {
            if (this._pendingCallback != null || this._syncOperationPending)
            {
                this._deferredFree = true;
                return false;
            }

            Free();
            return true;
        }

        [SecuritySafeCritical]
        public bool FreeIfDeferred()
        {
            if (this._deferredFree)
            {
                return FreeOrDefer();
            }

            return false;
        }

        [SecuritySafeCritical]
        public void StartAsyncOperation(byte[] buffer, OverlappedIOCompleteCallback callback, bool bound)
        {
            if (callback == null)
            {
                throw Fx.AssertAndThrow("StartAsyncOperation called with null callback.");
            }
            if (this._pendingCallback != null)
            {
                throw Fx.AssertAndThrow("StartAsyncOperation called while another is in progress.");
            }
            if (this._syncOperationPending)
            {
                throw Fx.AssertAndThrow("StartAsyncOperation called while a sync operation was already pending.");
            }
            if (this._nativeOverlapped == null)
            {
                throw Fx.AssertAndThrow("StartAsyncOperation called on freed OverlappedContext.");
            }

            this._pendingCallback = callback;

            if (buffer != null)
            {
                Fx.Assert(object.ReferenceEquals(this._bufferHolder[0], OverlappedContext.s_dummyBuffer), "StartAsyncOperation: buffer holder corrupted.");
                this._bufferHolder[0] = buffer;
                //this._pinnedHandle.Target = this._pinnedTarget;
                this._bufferPtr = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            }

            if (bound)
            {
                this._overlapped.EventHandleIntPtr = IntPtr.Zero;

                // For completion ports, the back-reference is this member.
                this._rootedHolder.ThisHolder = this;
            }
            else
            {
                // Need to do this since we register the wait before posting the I/O.
                if (this._completionEvent != null)
                {
                    this._completionEvent.Reset();
                }

                this._overlapped.EventHandleIntPtr = EventHandle;

                // For unbound, the back-reference is this registration.
                this._registration = ThreadPool.UnsafeRegisterWaitForSingleObject(this._completionEvent, OverlappedContext.s_eventCallback, this, Timeout.Infinite, true);
            }
        }

        [SecuritySafeCritical]
        public void CancelAsyncOperation()
        {
            this._rootedHolder.ThisHolder = null;
            if (this._registration != null)
            {
                this._registration.Unregister(null);
                this._registration = null;
            }
            this._bufferPtr = null;
            this._bufferHolder[0] = OverlappedContext.s_dummyBuffer;
            this._pendingCallback = null;
        }       
        
        // The only holder allowed is Holder[0].  It can be passed in as a ref to prevent repeated expensive array lookups.
        [SecuritySafeCritical]
        public void StartSyncOperation(byte[] buffer, ref object holder)
        {
            if (this._syncOperationPending)
            {
                throw Fx.AssertAndThrow("StartSyncOperation called while an operation was already pending.");
            }
            if (this._pendingCallback != null)
            {
                throw Fx.AssertAndThrow("StartSyncOperation called while an async operation was already pending.");
            }
            if (this._nativeOverlapped == null)
            {
                throw Fx.AssertAndThrow("StartSyncOperation called on freed OverlappedContext.");
            }

            this._overlapped.EventHandleIntPtr = EventHandle;

            // Sync operations do NOT root this object.  If it gets finalized, we need to know not to free the buffer.
            // We do root the event.
            this._rootedHolder.EventHolder = this._completionEvent;
            this._syncOperationPending = true;

            if (buffer != null)
            {
                Fx.Assert(object.ReferenceEquals(holder, OverlappedContext.s_dummyBuffer), "StartSyncOperation: buffer holder corrupted.");
                holder = buffer;
                //this._pinnedHandle.Target = this._pinnedTarget;
                this._bufferPtr = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            }
        }

        // If this returns false, the OverlappedContext is no longer usable.  It shouldn't be freed or anything.
        [SecuritySafeCritical]
        public bool WaitForSyncOperation(TimeSpan timeout)
        {
            return WaitForSyncOperation(timeout, ref this._bufferHolder[0]);
        }

        // The only holder allowed is Holder[0].  It can be passed in as a ref to prevent repeated expensive array lookups.
        [SecurityCritical]
        public bool WaitForSyncOperation(TimeSpan timeout, ref object holder)
        {
            if (!this._syncOperationPending)
            {
                throw Fx.AssertAndThrow("WaitForSyncOperation called while no operation was pending.");
            }

            if (!UnsafeNativeMethods.HasOverlappedIoCompleted(this._nativeOverlapped))
            {
                if (!TimeoutHelper.WaitOne(this._completionEvent, timeout))
                {
                    // We can't free ourselves until the operation is done.  The only way to do that is register a callback.
                    // This will root the object.  No longer any need for the finalizer.  This instance is unusable after this.
                    GC.SuppressFinalize(this);
                    ThreadPool.UnsafeRegisterWaitForSingleObject(this._completionEvent, OverlappedContext.s_cleanupCallback, this, Timeout.Infinite, true);
                    return false;
                }
            }

            Fx.Assert(this._bufferPtr == null || this._bufferPtr == (byte*)Marshal.UnsafeAddrOfPinnedArrayElement((byte[])holder, 0),
                "The buffer moved during a sync call!");

            CancelSyncOperation(ref holder);
            return true;
        }

        // The only holder allowed is Holder[0].  It can be passed in as a ref to prevent repeated expensive array lookups.
        [SecuritySafeCritical]
        public void CancelSyncOperation(ref object holder)
        {
            this._bufferPtr = null;
            holder = OverlappedContext.s_dummyBuffer;
            Fx.Assert(object.ReferenceEquals(this._bufferHolder[0], OverlappedContext.s_dummyBuffer), "Bad holder passed to CancelSyncOperation.");

            this._syncOperationPending = false;
            this._rootedHolder.EventHolder = null;
        }

        // This should ONLY be used to make a 'ref object' parameter to the zeroth element, to prevent repeated expensive array lookups.
        public object[] Holder
        {
            [SecuritySafeCritical]
            get
            {
                return this._bufferHolder;
            }
        }

        public byte* BufferPtr
        {
            [SecuritySafeCritical]
            get
            {
                byte* ptr = this._bufferPtr;
                if (ptr == null)
                {
#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
#pragma warning suppress 56503 // justinbr, not a publicly accessible API
                    throw Fx.AssertAndThrow("Pointer requested while no operation pending or no buffer provided.");
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning
                }
                return ptr;
            }
        }

        public NativeOverlapped* NativeOverlapped
        {
            [SecuritySafeCritical]
            get
            {
                NativeOverlapped* ptr = this._nativeOverlapped;
                if (ptr == null)
                {
#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
#pragma warning suppress 56503 // justinbr, not a publicly accessible API
                    throw Fx.AssertAndThrow("NativeOverlapped pointer requested after it was freed.");
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning
                }
                return ptr;
            }
        }

        IntPtr EventHandle
        {
            get
            {
                if (this._completionEvent == null)
                {
                    this._completionEvent = new ManualResetEvent(false);
                    this._eventHandle = (IntPtr)(1 | (long)this._completionEvent.SafeWaitHandle.DangerousGetHandle());
                }
                return this._eventHandle;
            }
        }

        [SecuritySafeCritical]
        static void CompleteCallback(uint error, uint numBytes, NativeOverlapped* nativeOverlapped)
        {
            // Empty out the AsyncResult ASAP to close the leak window.
            Overlapped overlapped = Overlapped.Unpack(nativeOverlapped);
            OverlappedContext pThis = ((RootedHolder)overlapped.AsyncResult).ThisHolder;
            Fx.Assert(pThis != null, "Overlapped.AsyncResult not set. I/O completed multiple times, or cancelled I/O completed.");
            Fx.Assert(object.ReferenceEquals(pThis._overlapped, overlapped), "CompleteCallback completed with corrupt OverlappedContext.overlapped.");
            Fx.Assert(object.ReferenceEquals(pThis._rootedHolder, overlapped.AsyncResult), "CompleteCallback completed with corrupt OverlappedContext.rootedHolder.");
            pThis._rootedHolder.ThisHolder = null;

            Fx.Assert(pThis._bufferPtr == null || pThis._bufferPtr == (byte*)Marshal.UnsafeAddrOfPinnedArrayElement((byte[])pThis._bufferHolder[0], 0),
                "Buffer moved during bound async operation!");

            // Release the pin.
            pThis._bufferPtr = null;
            pThis._bufferHolder[0] = OverlappedContext.s_dummyBuffer;

            OverlappedIOCompleteCallback callback = pThis._pendingCallback;
            pThis._pendingCallback = null;
            Fx.Assert(callback != null, "PendingCallback not set. I/O completed multiple times, or cancelled I/O completed.");

            callback(true, (int)error, checked((int)numBytes));
        }

        [SecuritySafeCritical]
        static void EventCallback(object state, bool timedOut)
        {
            OverlappedContext pThis = state as OverlappedContext;
            Fx.Assert(pThis != null, "OverlappedContext.EventCallback registered wait doesn't have an OverlappedContext as state.");

            if (timedOut)
            {
                Fx.Assert("OverlappedContext.EventCallback registered wait timed out.");

                // Turn this into a leak.  Don't let ourselves get cleaned up - could scratch the heap.
                if (pThis == null || pThis._rootedHolder == null)
                {
                    // We're doomed to do a wild write and corrupt the process.
                    //DiagnosticUtility.FailFast("Can't prevent heap corruption.");
                }
                pThis._rootedHolder.ThisHolder = pThis;
                return;
            }

            pThis._registration = null;

            Fx.Assert(pThis._bufferPtr == null || pThis._bufferPtr == (byte*)Marshal.UnsafeAddrOfPinnedArrayElement((byte[])pThis._bufferHolder[0], 0),
                "Buffer moved during unbound async operation!");

            // Release the pin.
            pThis._bufferPtr = null;
            pThis._bufferHolder[0] = OverlappedContext.s_dummyBuffer;

            OverlappedIOCompleteCallback callback = pThis._pendingCallback;
            pThis._pendingCallback = null;
            Fx.Assert(callback != null, "PendingCallback not set. I/O completed multiple times, or cancelled I/O completed.");

            callback(false, 0, 0);
        }

        [SecuritySafeCritical]
        static void CleanupCallback(object state, bool timedOut)
        {
            OverlappedContext pThis = state as OverlappedContext;
            Fx.Assert(pThis != null, "OverlappedContext.CleanupCallback registered wait doesn't have an OverlappedContext as state.");

            if (timedOut)
            {
                Fx.Assert("OverlappedContext.CleanupCallback registered wait timed out.");

                // Turn this into a leak.
                return;
            }

            Fx.Assert(pThis._bufferPtr == null || pThis._bufferPtr == (byte*)Marshal.UnsafeAddrOfPinnedArrayElement((byte[])pThis._bufferHolder[0], 0),
                "Buffer moved during synchronous deferred cleanup!");

            Fx.Assert(pThis._syncOperationPending, "OverlappedContext.CleanupCallback called with no sync operation pending.");
            pThis._pinnedTarget = null;
            pThis._rootedHolder.EventHolder.Close();
            Overlapped.Free(pThis._nativeOverlapped);
        }

        // This class is always held onto (rooted) by the packed Overlapped.  The OverlappedContext instance moves itself in and out of
        // this object to root itself.  It's also used to root the ManualResetEvent during sync operations.
        // It needs to be an IAsyncResult since that's what Overlapped takes.
        class RootedHolder : IAsyncResult
        {
            OverlappedContext _overlappedBuffer;
            ManualResetEvent _eventHolder;


            public OverlappedContext ThisHolder
            {
                get
                {
                    return this._overlappedBuffer;
                }

                set
                {
                    this._overlappedBuffer = value;
                }
            }

            public ManualResetEvent EventHolder
            {
                get
                {
                    return this._eventHolder;
                }

                set
                {
                    this._eventHolder = value;
                }
            }


            // Unused IAsyncResult implementation.
#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
            object IAsyncResult.AsyncState =>
#pragma warning suppress 56503 // justinbr, not a publicly accessible API
                    throw Fx.AssertAndThrow("RootedHolder.AsyncState called.");
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning

#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
            WaitHandle IAsyncResult.AsyncWaitHandle =>
#pragma warning suppress 56503 // justinbr, not a publicly accessible API
                    throw Fx.AssertAndThrow("RootedHolder.AsyncWaitHandle called.");
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning

#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
            bool IAsyncResult.CompletedSynchronously =>
#pragma warning suppress 56503 // justinbr, not a publicly accessible API
                    throw Fx.AssertAndThrow("RootedHolder.CompletedSynchronously called.");
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning

#pragma warning disable CS1634 // Expected 'disable' or 'restore' after #pragma warning
            bool IAsyncResult.IsCompleted =>
#pragma warning suppress 56503 // justinbr, not a publicly accessible API
                    throw Fx.AssertAndThrow("RootedHolder.IsCompleted called.");
#pragma warning restore CS1634 // Expected 'disable' or 'restore' after #pragma warning
        }
    }
}
