// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.Runtime;
using System.Threading.Tasks;
using System.ServiceModel.Diagnostics;
using System.Collections.Generic;
using System.Threading;

namespace System.ServiceModel.Channels
{
    public abstract class CommunicationObject : ICommunicationObject, IAsyncCommunicationObject
    {
        private bool _closeCalled;
        private ExceptionQueue _exceptionQueue;
        private bool _onClosingCalled;
        private bool _onClosedCalled;
        private bool _onOpeningCalled;
        private bool _onOpenedCalled;
        private bool _raisedClosed;
        private bool _raisedClosing;
        private bool _raisedFaulted;
        private bool _traceOpenAndClose;
        private CommunicationState _state;
        internal bool _isSynchronousOpen;
        internal bool _isSynchronousClose;
        private bool _supportsAsyncOpenClose;
        private bool _supportsAsyncOpenCloseSet;
        private AsyncLock _asyncLock;
        private object _thisLock;

        protected CommunicationObject()
            : this(new object())
        {
        }

        protected CommunicationObject(object mutex)
        {
            _thisLock = mutex;
            EventSender = this;
            _state = CommunicationState.Created;
        }

        // External CommunicationObjects cannot support IAsyncCommunicationObject,
        // but can appear to because they may derive from WCF types that do.
        // Attempting to call any IAsyncCommunicationObject method on those types
        // will go directly to the WCF base type and bypass the external type's
        // synchronous or asynchronous code paths.  We cannot distinguish between
        // this and a product CommunicationObject handling it and calling base.
        // This property detects if the current type is safe to use for
        // IAsyncCommunicationObject method calls.
        internal bool SupportsAsyncOpenClose
        {
            get
            {
                if (!_supportsAsyncOpenCloseSet)
                {
                    // We'll use the simple heuristic that namespace System.ServiceModel
                    // indicates a product type that is required to support async open/close.
                    // We don't expect exceptions here in NET Native because the product rd.xml
                    // grants the Reflection degree to all subtypes of ICommunicationObject.
                    // However, in the interests of being safe, catch that exception if it happens.
                    try
                    {
                        string ns = GetType().Namespace;
                        _supportsAsyncOpenClose = ns != null && ns.StartsWith("System.ServiceModel");
                    }
                    catch
                    {
                        // The most likely situation for this exception is the NET Native
                        // toolchain not recognizing this type is ICommunicationObject.
                        // But in that case, the best assumption is that this is a WCF
                        // product type, and therefore it supports async open/close.
                        _supportsAsyncOpenClose = true;
                    }

                    _supportsAsyncOpenCloseSet = true;
                }

                return _supportsAsyncOpenClose;
            }
            set
            {
                // It is permissible for types to set this value if they know
                // they can or cannot support async open/close.  Unsealed public
                // types must *not* set this to true because they cannot know if
                // an external derived type does support it.
                _supportsAsyncOpenClose = value;
                _supportsAsyncOpenCloseSet = true;
            }
        }

        internal bool Aborted { get; private set; }

        internal object EventSender { get; set; }

        protected bool IsDisposed
        {
            get { return _state == CommunicationState.Closed; }
        }

        public CommunicationState State
        {
            get { return _state; }
        }

        protected object ThisLock { 
            get
            {
#if DEBUG
                if (_asyncLock != null)
                {
                    Fx.Assert("Inconsistent usage of ThisLock and AsyncLock");
                }
#endif
                return _thisLock;
            }
        }

        internal AsyncLock ThisAsyncLock
        {
            get
            {
                if (_asyncLock != null)
                {
                    return _asyncLock;
                }

                _ = Interlocked.CompareExchange(ref _asyncLock, new AsyncLock(), null);
                return _asyncLock;
            }
        }

        protected abstract TimeSpan DefaultCloseTimeout { get; }
        protected abstract TimeSpan DefaultOpenTimeout { get; }

        internal TimeSpan InternalCloseTimeout
        {
            get { return DefaultCloseTimeout; }
        }

        internal TimeSpan InternalOpenTimeout
        {
            get { return DefaultOpenTimeout; }
        }

        public event EventHandler Closed;
        public event EventHandler Closing;
        public event EventHandler Faulted;
        public event EventHandler Opened;
        public event EventHandler Opening;

        public void Abort()
        {
            lock (_thisLock)
            {
                if (Aborted || _state == CommunicationState.Closed)
                {
                    return;
                }

                Aborted = true;

                _state = CommunicationState.Closing;
            }



            try
            {
                OnClosing();
                if (!_onClosingCalled)
                {
                    throw TraceUtility.ThrowHelperError(CreateBaseClassMethodNotCalledException("OnClosing"), Guid.Empty, this);
                }

                OnAbort();

                OnClosed();
                if (!_onClosedCalled)
                {
                    throw TraceUtility.ThrowHelperError(CreateBaseClassMethodNotCalledException("OnClosed"), Guid.Empty, this);
                }
            }
            finally
            {
            }
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return BeginClose(DefaultCloseTimeout, callback, state);
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return CloseAsyncInternal(timeout).ToApm(callback, state);
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return BeginOpen(DefaultOpenTimeout, callback, state);
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OpenAsyncInternal(timeout).ToApm(callback, state);
        }

        public void Close()
        {
            Close(DefaultCloseTimeout);
        }

        public void Close(TimeSpan timeout)
        {
            _isSynchronousClose = true;
            CloseAsyncInternal(timeout).WaitForCompletion();
        }

        private async Task CloseAsyncInternal(TimeSpan timeout)
        {
            await TaskHelpers.EnsureDefaultTaskScheduler();
            await ((IAsyncCommunicationObject)this).CloseAsync(timeout);
        }

        async Task IAsyncCommunicationObject.CloseAsync(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException(nameof(timeout), SRP.SFxTimeoutOutOfRange0));
            }

            CommunicationState originalState;
            lock (_thisLock)
            {
                originalState = _state;
                if (originalState != CommunicationState.Closed)
                {
                    _state = CommunicationState.Closing;
                }

                _closeCalled = true;
            }

            switch (originalState)
            {
                case CommunicationState.Created:
                case CommunicationState.Opening:
                case CommunicationState.Faulted:
                    Abort();
                    if (originalState == CommunicationState.Faulted)
                    {
                        throw TraceUtility.ThrowHelperError(CreateFaultedException(), Guid.Empty, this);
                    }
                    break;

                case CommunicationState.Opened:
                    {
                        bool throwing = true;
                        try
                        {
                            TimeoutHelper actualTimeout = new TimeoutHelper(timeout);

                            OnClosing();
                            if (!_onClosingCalled)
                            {
                                throw TraceUtility.ThrowHelperError(CreateBaseClassMethodNotCalledException("OnClosing"), Guid.Empty, this);
                            }

                            await OnCloseAsyncInternal(actualTimeout.RemainingTime());

                            OnClosed();
                            if (!_onClosedCalled)
                            {
                                throw TraceUtility.ThrowHelperError(CreateBaseClassMethodNotCalledException("OnClosed"), Guid.Empty, this);
                            }

                            throwing = false;
                        }
                        finally
                        {
                            if (throwing)
                            {
                                Abort();
                            }
                        }
                        break;
                    }

                case CommunicationState.Closing:
                case CommunicationState.Closed:
                    break;

                default:
                    throw Fx.AssertAndThrow("CommunicationObject.BeginClose: Unknown CommunicationState");
            }
        }

        // Internal helper to call the right form of the OnClose() method
        // asynchronously.  It depends on whether the type can support the
        // async API's and how the current Communication object is being closed.
        private async Task OnCloseAsyncInternal(TimeSpan timeout)
        {
            // If this type is capable of overriding OnCloseAsync,
            // then use it for both async and sync closes
            if (SupportsAsyncOpenClose)
            {
                // The class supports OnCloseAsync(), so use it
                await OnCloseAsync(timeout);
            }
            else
            {
                // This type is an external type that cannot override OnCloseAsync.
                // If this is a synchronous close, invoke the synchronous OnClose.
                if (_isSynchronousClose)
                {
                    await TaskHelpers.CallActionAsync(OnClose, timeout);
                }
                else
                {
                    // The class does not support OnCloseAsync, and this is an asynchronous
                    // close, so use the Begin/End pattern
                    await Task.Factory.FromAsync(OnBeginClose, OnEndClose, timeout, TaskCreationOptions.RunContinuationsAsynchronously);
                }
            }
        }

        private Exception CreateNotOpenException()
        {
            return new InvalidOperationException(SRP.Format(SRP.CommunicationObjectCannotBeUsed, GetCommunicationObjectType().ToString(), _state.ToString()));
        }

        private Exception CreateImmutableException()
        {
            return new InvalidOperationException(SRP.Format(SRP.CommunicationObjectCannotBeModifiedInState, GetCommunicationObjectType().ToString(), _state.ToString()));
        }

        private Exception CreateBaseClassMethodNotCalledException(string method)
        {
            return new InvalidOperationException(SRP.Format(SRP.CommunicationObjectBaseClassMethodNotCalled, GetCommunicationObjectType().ToString(), method));
        }

        internal Exception CreateClosedException()
        {
            if (!_closeCalled)
            {
                return CreateAbortedException();
            }
            else
            {
                return new ObjectDisposedException(GetCommunicationObjectType().ToString());
            }
        }

        internal Exception CreateFaultedException()
        {
            string message = SRP.Format(SRP.CommunicationObjectFaulted1, GetCommunicationObjectType().ToString());
            return new CommunicationObjectFaultedException(message);
        }

        internal Exception CreateAbortedException()
        {
            return new CommunicationObjectAbortedException(SRP.Format(SRP.CommunicationObjectAborted1, GetCommunicationObjectType().ToString()));
        }

        internal bool DoneReceivingInCurrentState()
        {
            ThrowPending();

            switch (_state)
            {
                case CommunicationState.Created:
                    throw TraceUtility.ThrowHelperError(CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opening:
                    throw TraceUtility.ThrowHelperError(CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opened:
                    return false;

                case CommunicationState.Closing:
                    return true;

                case CommunicationState.Closed:
                    return true;

                case CommunicationState.Faulted:
                    return true;

                default:
                    throw Fx.AssertAndThrow("DoneReceivingInCurrentState: Unknown CommunicationObject.state");
            }
        }

        public void EndClose(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        public void EndOpen(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected void Fault()
        {
            lock (_thisLock)
            {
                if (_state == CommunicationState.Closed || _state == CommunicationState.Closing)
                {
                    return;
                }

                if (_state == CommunicationState.Faulted)
                {
                    return;
                }

                _state = CommunicationState.Faulted;
            }

            OnFaulted();
        }

        internal void Fault(Exception exception)
        {
            AddPendingException(exception);
            Fault();
        }

        internal void AddPendingException(Exception exception)
        {
            lock (_thisLock)
            {
                if (_exceptionQueue == null)
                {
                    _exceptionQueue = new ExceptionQueue(_thisLock);
                }
            }

            _exceptionQueue.AddException(exception);
        }

        internal Exception GetPendingException()
        {
            CommunicationState currentState = _state;

            Fx.Assert(currentState == CommunicationState.Closing || currentState == CommunicationState.Closed || currentState == CommunicationState.Faulted,
                "CommunicationObject.GetPendingException(currentState == CommunicationState.Closing || currentState == CommunicationState.Closed || currentState == CommunicationState.Faulted)");

            ExceptionQueue queue = _exceptionQueue;
            if (queue != null)
            {
                return queue.GetException();
            }
            else
            {
                return null;
            }
        }

        // Terminal is loosely defined as an interruption to close or a fault.
        internal Exception GetTerminalException()
        {
            Exception exception = GetPendingException();

            if (exception != null)
            {
                return exception;
            }

            switch (_state)
            {
                case CommunicationState.Closing:
                case CommunicationState.Closed:
                    return new CommunicationException(SRP.Format(SRP.CommunicationObjectCloseInterrupted1, GetCommunicationObjectType().ToString()));

                case CommunicationState.Faulted:
                    return CreateFaultedException();

                default:
                    throw Fx.AssertAndThrow("GetTerminalException: Invalid CommunicationObject.state");
            }
        }

        public void Open()
        {
            Open(DefaultOpenTimeout);
        }

        public void Open(TimeSpan timeout)
        {
            _isSynchronousOpen = true;
            OpenAsyncInternal(timeout).WaitForCompletion();
        }

        private Task OpenAsyncInternal(TimeSpan timeout)
        {
            return ((IAsyncCommunicationObject)this).OpenAsync(timeout);
        }

        async Task IAsyncCommunicationObject.OpenAsync(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException(nameof(timeout), SRP.SFxTimeoutOutOfRange0));
            }

            lock (_thisLock)
            {
                ThrowIfDisposedOrImmutable();
                _state = CommunicationState.Opening;
            }

            bool throwing = true;
            try
            {
                TimeoutHelper actualTimeout = new TimeoutHelper(timeout);

                OnOpening();
                if (!_onOpeningCalled)
                {
                    throw TraceUtility.ThrowHelperError(CreateBaseClassMethodNotCalledException(nameof(OnOpening)), Guid.Empty, this);
                }

                // EnsureDefaultTaskScheduler must be called after OnOpening to ensure that any SynchronizationContext
                // is captured by the DispatchRuntime
                await TaskHelpers.EnsureDefaultTaskScheduler();
                await OnOpenAsyncInternal(actualTimeout.RemainingTime());

                OnOpened();
                if (!_onOpenedCalled)
                {
                    throw TraceUtility.ThrowHelperError(CreateBaseClassMethodNotCalledException(nameof(OnOpened)), Guid.Empty, this);
                }

                throwing = false;
            }
            finally
            {
                if (throwing)
                {
                    Fault();
                }
            }
        }

        // Internal helper to call the right form of the OnOpen() method
        // asynchronously.  It depends on whether the type can support the
        // async API's and how the current Communication object is being opened.
        private async Task OnOpenAsyncInternal(TimeSpan timeout)
        {
            // If this type is capable of overriding OnOpenAsync,
            // then use it for both async and sync opens
            if (SupportsAsyncOpenClose)
            {
                // The class supports OnOpenAsync(), so use it
                await OnOpenAsync(timeout);
            }
            else
            {
                // This type is an external type that cannot override OnOpenAsync.
                // If this is a synchronous open, invoke the synchronous OnOpen
                if (_isSynchronousOpen)
                {
                    await TaskHelpers.CallActionAsync(OnOpen, timeout);
                }
                else
                {
                    // The class does not support OnOpenAsync, so use the Begin/End pattern
                    await Task.Factory.FromAsync(OnBeginOpen, OnEndOpen, timeout, TaskCreationOptions.RunContinuationsAsynchronously);
                }
            }
        }

        protected virtual void OnClosed()
        {
            _onClosedCalled = true;

            lock (_thisLock)
            {
                if (_raisedClosed)
                {
                    return;
                }

                _raisedClosed = true;
                _state = CommunicationState.Closed;
            }


            EventHandler handler = Closed;
            if (handler != null)
            {
                try
                {
                    handler(EventSender, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    if (Fx.IsFatal(exception))
                    {
                        throw;
                    }

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                }
            }
        }

        protected virtual void OnClosing()
        {
            _onClosingCalled = true;

            lock (_thisLock)
            {
                if (_raisedClosing)
                {
                    return;
                }

                _raisedClosing = true;
            }


            EventHandler handler = Closing;
            if (handler != null)
            {
                try
                {
                    handler(EventSender, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    if (Fx.IsFatal(exception))
                    {
                        throw;
                    }

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                }
            }
        }

        protected virtual void OnFaulted()
        {
            lock (_thisLock)
            {
                if (_raisedFaulted)
                {
                    return;
                }

                _raisedFaulted = true;
            }

            EventHandler handler = Faulted;
            if (handler != null)
            {
                try
                {
                    handler(EventSender, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    if (Fx.IsFatal(exception))
                    {
                        throw;
                    }

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                }
            }
        }

        protected virtual void OnOpened()
        {
            _onOpenedCalled = true;

            lock (_thisLock)
            {
                if (Aborted || _state != CommunicationState.Opening)
                {
                    return;
                }

                _state = CommunicationState.Opened;
            }


            EventHandler handler = Opened;
            if (handler != null)
            {
                try
                {
                    handler(EventSender, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    if (Fx.IsFatal(exception))
                    {
                        throw;
                    }

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                }
            }
        }

        protected virtual void OnOpening()
        {
            _onOpeningCalled = true;


            EventHandler handler = Opening;
            if (handler != null)
            {
                try
                {
                    handler(EventSender, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    if (Fx.IsFatal(exception))
                    {
                        throw;
                    }

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                }
            }
        }

        internal void ThrowIfFaulted()
        {
            ThrowPending();

            switch (_state)
            {
                case CommunicationState.Created:
                    break;

                case CommunicationState.Opening:
                    break;

                case CommunicationState.Opened:
                    break;

                case CommunicationState.Closing:
                    break;

                case CommunicationState.Closed:
                    break;

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(CreateFaultedException(), Guid.Empty, this);

                default:
                    throw Fx.AssertAndThrow("ThrowIfFaulted: Unknown CommunicationObject.state");
            }
        }

        internal void ThrowIfAborted()
        {
            if (Aborted && !_closeCalled)
            {
                throw TraceUtility.ThrowHelperError(CreateAbortedException(), Guid.Empty, this);
            }
        }

        internal bool TraceOpenAndClose
        {
            get
            {
                return _traceOpenAndClose;
            }
            set
            {
                _traceOpenAndClose = value && DiagnosticUtility.ShouldUseActivity;
            }
        }

        internal void ThrowIfClosed()
        {
            ThrowPending();

            switch (_state)
            {
                case CommunicationState.Created:
                    break;

                case CommunicationState.Opening:
                    break;

                case CommunicationState.Opened:
                    break;

                case CommunicationState.Closing:
                    break;

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(CreateFaultedException(), Guid.Empty, this);

                default:
                    throw Fx.AssertAndThrow("ThrowIfClosed: Unknown CommunicationObject.state");
            }
        }

        protected virtual Type GetCommunicationObjectType()
        {
            return GetType();
        }

        protected internal void ThrowIfDisposed()
        {
            ThrowPending();

            switch (_state)
            {
                case CommunicationState.Created:
                    break;

                case CommunicationState.Opening:
                    break;

                case CommunicationState.Opened:
                    break;

                case CommunicationState.Closing:
                    throw TraceUtility.ThrowHelperError(CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(CreateFaultedException(), Guid.Empty, this);

                default:
                    throw Fx.AssertAndThrow("ThrowIfDisposed: Unknown CommunicationObject.state");
            }
        }

        internal void ThrowIfClosedOrOpened()
        {
            ThrowPending();

            switch (_state)
            {
                case CommunicationState.Created:
                    break;

                case CommunicationState.Opening:
                    break;

                case CommunicationState.Opened:
                    throw TraceUtility.ThrowHelperError(CreateImmutableException(), Guid.Empty, this);

                case CommunicationState.Closing:
                    throw TraceUtility.ThrowHelperError(CreateImmutableException(), Guid.Empty, this);

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(CreateFaultedException(), Guid.Empty, this);

                default:
                    throw Fx.AssertAndThrow("ThrowIfClosedOrOpened: Unknown CommunicationObject.state");
            }
        }

        protected internal void ThrowIfDisposedOrImmutable()
        {
            ThrowPending();

            switch (_state)
            {
                case CommunicationState.Created:
                    break;

                case CommunicationState.Opening:
                    throw TraceUtility.ThrowHelperError(CreateImmutableException(), Guid.Empty, this);

                case CommunicationState.Opened:
                    throw TraceUtility.ThrowHelperError(CreateImmutableException(), Guid.Empty, this);

                case CommunicationState.Closing:
                    throw TraceUtility.ThrowHelperError(CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(CreateFaultedException(), Guid.Empty, this);

                default:
                    throw Fx.AssertAndThrow("ThrowIfDisposedOrImmutable: Unknown CommunicationObject.state");
            }
        }

        protected internal void ThrowIfDisposedOrNotOpen()
        {
            ThrowPending();

            switch (_state)
            {
                case CommunicationState.Created:
                    throw TraceUtility.ThrowHelperError(CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opening:
                    throw TraceUtility.ThrowHelperError(CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opened:
                    break;

                case CommunicationState.Closing:
                    throw TraceUtility.ThrowHelperError(CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(CreateFaultedException(), Guid.Empty, this);

                default:
                    throw Fx.AssertAndThrow("ThrowIfDisposedOrNotOpen: Unknown CommunicationObject.state");
            }
        }

        internal void ThrowIfNotOpened()
        {
            if (_state == CommunicationState.Created || _state == CommunicationState.Opening)
            {
                throw TraceUtility.ThrowHelperError(CreateNotOpenException(), Guid.Empty, this);
            }
        }

        internal void ThrowIfClosedOrNotOpen()
        {
            ThrowPending();

            switch (_state)
            {
                case CommunicationState.Created:
                    throw TraceUtility.ThrowHelperError(CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opening:
                    throw TraceUtility.ThrowHelperError(CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opened:
                    break;

                case CommunicationState.Closing:
                    break;

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(CreateFaultedException(), Guid.Empty, this);

                default:
                    throw Fx.AssertAndThrow("ThrowIfClosedOrNotOpen: Unknown CommunicationObject.state");
            }
        }

        internal void ThrowPending()
        {
        }

        //
        // State callbacks
        //

        protected abstract void OnAbort();

        protected abstract void OnClose(TimeSpan timeout);

        protected abstract IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state);

        protected abstract void OnEndClose(IAsyncResult result);

        protected abstract void OnOpen(TimeSpan timeout);

        protected abstract IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state);

        protected abstract void OnEndOpen(IAsyncResult result);

        internal protected virtual Task OnCloseAsync(TimeSpan timeout)
        {
            // All WCF product types are required to override this method and not call base.
            // No external types will be able to reach here because it is not public.
            Contract.Assert(false, String.Format("Type '{0}' is required to override OnCloseAsync", GetType()));

            return TaskHelpers.CompletedTask();
        }

        internal protected virtual Task OnOpenAsync(TimeSpan timeout)
        {
            // All WCF product types are required to override this method and not call base.
            // No external types will be able to reach here because it is not public.
            Contract.Assert(false, String.Format("Type '{0}' is required to override OnOpenAsync", GetType()));

            return TaskHelpers.CompletedTask();
        }

        // Helper used to open another CommunicationObject "owned" by the current one.
        // It is used to propagate the use of either the synchronous or asynchronous methods
        internal Task OpenOtherAsync(ICommunicationObject other, TimeSpan timeout)
        {
            // If the other object supports async open/close, we know it's an
            // internal implementation so we can continue the open asynchronously. We need
            // to propagate _isSynchronousOpen in case the next inner channel isn't an internal
            // implementation.
            if (other is CommunicationObject communicationObject && communicationObject.SupportsAsyncOpenClose)
            {
                communicationObject._isSynchronousOpen = _isSynchronousOpen;
                return ((IAsyncCommunicationObject)communicationObject).OpenAsync(timeout);
            }
            
            // Other object isn't an internal implementation so we need to match calling the
            // sync/async pattern of the Open call that was initially made.
            // If the current object is being opened synchronously, use the synchronous
            // open path for the other object.
            if (_isSynchronousOpen)
            {
                // We need to make sure the synchronous open which might block a thread happens on the
                // thread pool so as not to block the IOThreadScheduler thread.
                return TaskHelpers.CallActionAsync(other.Open, timeout);
            }
            else
            {
                return Task.Factory.FromAsync(other.BeginOpen, other.EndOpen, timeout, null);
            }
        }

        // Helper used to close another CommunicationObject "owned" by the current one.
        // It is used to propagate the use of either the synchronous or asynchronous methods
        internal Task CloseOtherAsync(ICommunicationObject other, TimeSpan timeout)
        {
            // If the other object supports async open/close, we know it's an
            // internal implementation so we can continue the close asynchronously. We need
            // to propagate _isSynchronousClose in case the next inner channel isn't an internal
            // implementation.
            if (other is CommunicationObject communicationObject && communicationObject.SupportsAsyncOpenClose)
            {
                communicationObject._isSynchronousClose = _isSynchronousClose;
                return ((IAsyncCommunicationObject)communicationObject).CloseAsync(timeout);
            }

            // Other object isn't an internal implementation so we need to match calling the
            // sync/async pattern of the Close call that was initially made.
            // If the current object is being closed synchronously, use the synchronous
            // close path for the other object.
            if (_isSynchronousClose)
            {
                // We need to make sure the synchronous close which might block a thread happens on the
                // thread pool so as not to block the IOThreadScheduler thread.
                return TaskHelpers.CallActionAsync(other.Close, timeout);
            }
            else
            {
                return Task.Factory.FromAsync(other.BeginClose, other.EndClose, timeout, null);
            }
        }

        private class ExceptionQueue
        {
            private Queue<Exception> _exceptions = new Queue<Exception>();

            internal ExceptionQueue(object thisLock)
            {
                _thisLock = thisLock;
            }

            private object _thisLock { get; }

            public void AddException(Exception exception)
            {
                if (exception == null)
                {
                    return;
                }

                lock (_thisLock)
                {
                    _exceptions.Enqueue(exception);
                }
            }

            public Exception GetException()
            {
                lock (_thisLock)
                {
                    if (_exceptions.Count > 0)
                    {
                        return _exceptions.Dequeue();
                    }
                }

                return null;
            }
        }
    }

    // This helper class exists to expose non-contract API's to other ServiceModel projects
    public static class CommunicationObjectInternal
    {
        public static void ThrowIfClosed(CommunicationObject communicationObject)
        {
            Contract.Assert(communicationObject != null);
            communicationObject.ThrowIfClosed();
        }

        public static void ThrowIfClosedOrOpened(CommunicationObject communicationObject)
        {
            Contract.Assert(communicationObject != null);
            communicationObject.ThrowIfClosedOrOpened();
        }

        public static void ThrowIfDisposedOrNotOpen(CommunicationObject communicationObject)
        {
            Contract.Assert(communicationObject != null);
            communicationObject.ThrowIfDisposedOrNotOpen();
        }

        public static void ThrowIfDisposed(CommunicationObject communicationObject)
        {
            Contract.Assert(communicationObject != null);
            communicationObject.ThrowIfDisposed();
        }

        public static TimeSpan GetInternalCloseTimeout(this CommunicationObject communicationObject)
        {
            return communicationObject.InternalCloseTimeout;
        }

        public static void OnClose(CommunicationObject communicationObject, TimeSpan timeout)
        {
            OnCloseAsyncInternal(communicationObject, timeout).WaitForCompletion();
        }

        public static IAsyncResult OnBeginClose(CommunicationObject communicationObject, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return communicationObject.OnCloseAsync(timeout).ToApm(callback, state);
        }

        public static void OnEnd(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        public static void OnOpen(CommunicationObject communicationObject, TimeSpan timeout)
        {
            OnOpenAsyncInternal(communicationObject, timeout).WaitForCompletion();
        }

        public static IAsyncResult OnBeginOpen(CommunicationObject communicationObject, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return communicationObject.OnOpenAsync(timeout).ToApm(callback, state);
        }

        public static async Task OnCloseAsyncInternal(CommunicationObject communicationObject, TimeSpan timeout)
        {
            await TaskHelpers.EnsureDefaultTaskScheduler();
            await communicationObject.OnCloseAsync(timeout);
        }

        public static async Task OnOpenAsyncInternal(CommunicationObject communicationObject, TimeSpan timeout)
        {
            await TaskHelpers.EnsureDefaultTaskScheduler();
            await communicationObject.OnOpenAsync(timeout);
        }
    }
}
