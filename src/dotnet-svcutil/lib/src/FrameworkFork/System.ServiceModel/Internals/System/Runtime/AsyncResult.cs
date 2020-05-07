// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace System.Runtime
{
    // AsyncResult starts acquired; Complete releases.
    [Fx.Tag.SynchronizationPrimitive(Fx.Tag.BlocksUsing.ManualResetEvent, SupportsAsync = true, ReleaseMethod = "Complete")]
    internal abstract class AsyncResult : IAsyncResult
    {
        private static AsyncCallback s_asyncCompletionWrapperCallback;
        private AsyncCallback _callback;
        private bool _completedSynchronously;
        private bool _endCalled;
        private Exception _exception;
        private bool _isCompleted;
        private AsyncCompletion _nextAsyncCompletion;
        private object _state;
        private Action _beforePrepareAsyncCompletionAction;
        private Func<IAsyncResult, bool> _checkSyncValidationFunc;
        [Fx.Tag.SynchronizationObject]

        private ManualResetEvent _manualResetEvent;
        [Fx.Tag.SynchronizationObject(Blocking = false)]

        private object _thisLock;


        protected AsyncResult(AsyncCallback callback, object state)
        {
            _callback = callback;
            _state = state;
            _thisLock = new object();
        }

        public object AsyncState
        {
            get
            {
                return _state;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (_manualResetEvent != null)
                {
                    return _manualResetEvent;
                }

                lock (ThisLock)
                {
                    if (_manualResetEvent == null)
                    {
                        _manualResetEvent = new ManualResetEvent(_isCompleted);
                    }
                }

                return _manualResetEvent;
            }
        }

        public bool CompletedSynchronously
        {
            get
            {
                return _completedSynchronously;
            }
        }

        public bool HasCallback
        {
            get
            {
                return _callback != null;
            }
        }

        public bool IsCompleted
        {
            get
            {
                return _isCompleted;
            }
        }

        // used in conjunction with PrepareAsyncCompletion to allow for finally blocks
        protected Action<AsyncResult, Exception> OnCompleting { get; set; }

        private object ThisLock
        {
            get
            {
                return _thisLock;
            }
        }

        // subclasses like TraceAsyncResult can use this to wrap the callback functionality in a scope
        protected Action<AsyncCallback, IAsyncResult> VirtualCallback
        {
            get;
            set;
        }

        protected void Complete(bool completedSynchronously)
        {
            if (_isCompleted)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(InternalSR.AsyncResultCompletedTwice(GetType())));
            }



            _completedSynchronously = completedSynchronously;
            if (OnCompleting != null)
            {
                // Allow exception replacement, like a catch/throw pattern.
                try
                {
                    OnCompleting(this, _exception);
                }
                catch (Exception exception)
                {
                    if (Fx.IsFatal(exception))
                    {
                        throw;
                    }
                    _exception = exception;
                }
            }

            if (completedSynchronously)
            {
                // If we completedSynchronously, then there's no chance that the manualResetEvent was created so
                // we don't need to worry about a race condition.
                Fx.Assert(_manualResetEvent == null, "No ManualResetEvent should be created for a synchronous AsyncResult.");
                _isCompleted = true;
            }
            else
            {
                lock (ThisLock)
                {
                    _isCompleted = true;
                    if (_manualResetEvent != null)
                    {
                        _manualResetEvent.Set();
                    }
                }
            }

            if (_callback != null)
            {
                try
                {
                    if (VirtualCallback != null)
                    {
                        VirtualCallback(_callback, this);
                    }
                    else
                    {
                        _callback(this);
                    }
                }
#pragma warning disable 1634
#pragma warning suppress 56500 // transferring exception to another thread
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    throw Fx.Exception.AsError(new CallbackException(InternalSR.AsyncCallbackThrewException, e));
                }
#pragma warning restore 1634
            }
        }

        protected void Complete(bool completedSynchronously, Exception exception)
        {
            _exception = exception;
            Complete(completedSynchronously);
        }

        private static void AsyncCompletionWrapperCallback(IAsyncResult result)
        {
            if (result == null)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(InternalSR.InvalidNullAsyncResult));
            }
            if (result.CompletedSynchronously)
            {
                return;
            }

            AsyncResult thisPtr = (AsyncResult)result.AsyncState;
            if (!thisPtr.OnContinueAsyncCompletion(result))
            {
                return;
            }

            AsyncCompletion callback = thisPtr.GetNextCompletion();
            if (callback == null)
            {
                ThrowInvalidAsyncResult(result);
            }

            bool completeSelf = false;
            Exception completionException = null;
            try
            {
                completeSelf = callback(result);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                completeSelf = true;
                completionException = e;
            }

            if (completeSelf)
            {
                thisPtr.Complete(false, completionException);
            }
        }

        // Note: this should be only derived by the TransactedAsyncResult
        protected virtual bool OnContinueAsyncCompletion(IAsyncResult result)
        {
            return true;
        }

        // Note: this should be used only by the TransactedAsyncResult
        protected void SetBeforePrepareAsyncCompletionAction(Action beforePrepareAsyncCompletionAction)
        {
            _beforePrepareAsyncCompletionAction = beforePrepareAsyncCompletionAction;
        }

        // Note: this should be used only by the TransactedAsyncResult
        protected void SetCheckSyncValidationFunc(Func<IAsyncResult, bool> checkSyncValidationFunc)
        {
            _checkSyncValidationFunc = checkSyncValidationFunc;
        }

        protected AsyncCallback PrepareAsyncCompletion(AsyncCompletion callback)
        {
            if (_beforePrepareAsyncCompletionAction != null)
            {
                _beforePrepareAsyncCompletionAction();
            }

            _nextAsyncCompletion = callback;
            if (AsyncResult.s_asyncCompletionWrapperCallback == null)
            {
                AsyncResult.s_asyncCompletionWrapperCallback = Fx.ThunkCallback(new AsyncCallback(AsyncCompletionWrapperCallback));
            }
            return AsyncResult.s_asyncCompletionWrapperCallback;
        }

        protected bool CheckSyncContinue(IAsyncResult result)
        {
            AsyncCompletion dummy;
            return TryContinueHelper(result, out dummy);
        }

        protected bool SyncContinue(IAsyncResult result)
        {
            AsyncCompletion callback;
            if (TryContinueHelper(result, out callback))
            {
                return callback(result);
            }
            else
            {
                return false;
            }
        }

        private bool TryContinueHelper(IAsyncResult result, out AsyncCompletion callback)
        {
            if (result == null)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(InternalSR.InvalidNullAsyncResult));
            }

            callback = null;
            if (_checkSyncValidationFunc != null)
            {
                if (!_checkSyncValidationFunc(result))
                {
                    return false;
                }
            }
            else if (!result.CompletedSynchronously)
            {
                return false;
            }

            callback = GetNextCompletion();
            if (callback == null)
            {
                ThrowInvalidAsyncResult("Only call Check/SyncContinue once per async operation (once per PrepareAsyncCompletion).");
            }
            return true;
        }

        private AsyncCompletion GetNextCompletion()
        {
            AsyncCompletion result = _nextAsyncCompletion;
            _nextAsyncCompletion = null;
            return result;
        }

        protected static void ThrowInvalidAsyncResult(IAsyncResult result)
        {
            throw Fx.Exception.AsError(new InvalidOperationException(InternalSR.InvalidAsyncResultImplementation(result.GetType())));
        }

        protected static void ThrowInvalidAsyncResult(string debugText)
        {
            string message = InternalSR.InvalidAsyncResultImplementationGeneric;
            if (debugText != null)
            {
#if DEBUG
                message += " " + debugText;
#endif
            }
            throw Fx.Exception.AsError(new InvalidOperationException(message));
        }

        [Fx.Tag.Blocking(Conditional = "!asyncResult.isCompleted")]
        protected static TAsyncResult End<TAsyncResult>(IAsyncResult result)
            where TAsyncResult : AsyncResult
        {
            if (result == null)
            {
                throw Fx.Exception.ArgumentNull("result");
            }

            TAsyncResult asyncResult = result as TAsyncResult;

            if (asyncResult == null)
            {
                throw Fx.Exception.Argument("result", InternalSR.InvalidAsyncResult);
            }

            if (asyncResult._endCalled)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(InternalSR.AsyncResultAlreadyEnded));
            }


            asyncResult._endCalled = true;

            if (!asyncResult._isCompleted)
            {
                asyncResult.AsyncWaitHandle.WaitOne();
            }

            if (asyncResult._manualResetEvent != null)
            {
                asyncResult._manualResetEvent.Dispose();
            }

            if (asyncResult._exception != null)
            {
                throw Fx.Exception.AsError(asyncResult._exception);
            }

            return asyncResult;
        }

        // can be utilized by subclasses to write core completion code for both the sync and async paths
        // in one location, signalling chainable synchronous completion with the boolean result,
        // and leveraging PrepareAsyncCompletion for conversion to an AsyncCallback.
        // NOTE: requires that "this" is passed in as the state object to the asynchronous sub-call being used with a completion routine.
        protected delegate bool AsyncCompletion(IAsyncResult result);
    }
}
