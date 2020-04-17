// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime;
using System.Threading.Tasks;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Channels
{
    public abstract class CommunicationObject : ICommunicationObject, IAsyncCommunicationObject
    {
        private bool _aborted;
        private bool _closeCalled;
        private object _mutex;
        private bool _onClosingCalled;
        private bool _onClosedCalled;
        private bool _onOpeningCalled;
        private bool _onOpenedCalled;
        private bool _raisedClosed;
        private bool _raisedClosing;
        private bool _raisedFaulted;
        private bool _traceOpenAndClose;
        private object _eventSender;
        private CommunicationState _state;

        protected CommunicationObject()
            : this(new object())
        {
        }

        protected CommunicationObject(object mutex)
        {
            _mutex = mutex;
            _eventSender = this;
            _state = CommunicationState.Created;
        }

        internal bool Aborted
        {
            get { return _aborted; }
        }

        internal object EventSender
        {
            get { return _eventSender; }
            set { _eventSender = value; }
        }

        protected bool IsDisposed
        {
            get { return _state == CommunicationState.Closed; }
        }

        public CommunicationState State
        {
            get { return _state; }
        }

        protected object ThisLock
        {
            get { return _mutex; }
        }

        protected abstract TimeSpan DefaultCloseTimeout { get; }
        protected abstract TimeSpan DefaultOpenTimeout { get; }

        internal TimeSpan InternalCloseTimeout
        {
            get { return this.DefaultCloseTimeout; }
        }

        internal TimeSpan InternalOpenTimeout
        {
            get { return this.DefaultOpenTimeout; }
        }

        public event EventHandler Closed;
        public event EventHandler Closing;
        public event EventHandler Faulted;
        public event EventHandler Opened;
        public event EventHandler Opening;

        public void Abort()
        {
            lock (ThisLock)
            {
                if (_aborted || _state == CommunicationState.Closed)
                    return;
                _aborted = true;

                _state = CommunicationState.Closing;
            }



            try
            {
                OnClosing();
                if (!_onClosingCalled)
                    throw TraceUtility.ThrowHelperError(this.CreateBaseClassMethodNotCalledException("OnClosing"), Guid.Empty, this);

                OnAbort();

                OnClosed();
                if (!_onClosedCalled)
                    throw TraceUtility.ThrowHelperError(this.CreateBaseClassMethodNotCalledException("OnClosed"), Guid.Empty, this);
            }
            finally
            {
            }
        }

        public IAsyncResult BeginClose(AsyncCallback callback, object state)
        {
            return this.BeginClose(this.DefaultCloseTimeout, callback, state);
        }

        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return CloseAsyncInternal(timeout).ToApm(callback, state);
        }

        public IAsyncResult BeginOpen(AsyncCallback callback, object state)
        {
            return this.BeginOpen(this.DefaultOpenTimeout, callback, state);
        }

        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OpenAsyncInternal(timeout).ToApm(callback, state);
        }

        public void Close()
        {
            this.Close(this.DefaultCloseTimeout);
        }

        public void Close(TimeSpan timeout)
        {
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException("timeout", SRServiceModel.SFxTimeoutOutOfRange0));


            CommunicationState originalState;
            lock (ThisLock)
            {
                originalState = _state;
                if (originalState != CommunicationState.Closed)
                    _state = CommunicationState.Closing;

                _closeCalled = true;
            }

            switch (originalState)
            {
                case CommunicationState.Created:
                case CommunicationState.Opening:
                case CommunicationState.Faulted:
                    this.Abort();
                    if (originalState == CommunicationState.Faulted)
                    {
                        throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);
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
                                throw TraceUtility.ThrowHelperError(this.CreateBaseClassMethodNotCalledException("OnClosing"), Guid.Empty, this);

                            await OnCloseAsync(actualTimeout.RemainingTime());

                            OnClosed();
                            if (!_onClosedCalled)
                                throw TraceUtility.ThrowHelperError(this.CreateBaseClassMethodNotCalledException("OnClosed"), Guid.Empty, this);

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

        private Exception CreateNotOpenException()
        {
            return new InvalidOperationException(string.Format(SRServiceModel.CommunicationObjectCannotBeUsed, this.GetCommunicationObjectType().ToString(), _state.ToString()));
        }

        private Exception CreateImmutableException()
        {
            return new InvalidOperationException(string.Format(SRServiceModel.CommunicationObjectCannotBeModifiedInState, this.GetCommunicationObjectType().ToString(), _state.ToString()));
        }

        private Exception CreateBaseClassMethodNotCalledException(string method)
        {
            return new InvalidOperationException(string.Format(SRServiceModel.CommunicationObjectBaseClassMethodNotCalled, this.GetCommunicationObjectType().ToString(), method));
        }

        internal Exception CreateClosedException()
        {
            if (!_closeCalled)
            {
                return CreateAbortedException();
            }
            else
            {
                return new ObjectDisposedException(this.GetCommunicationObjectType().ToString());
            }
        }

        internal Exception CreateFaultedException()
        {
            string message = string.Format(SRServiceModel.CommunicationObjectFaulted1, this.GetCommunicationObjectType().ToString());
            return new CommunicationObjectFaultedException(message);
        }

        internal Exception CreateAbortedException()
        {
            return new CommunicationObjectAbortedException(string.Format(SRServiceModel.CommunicationObjectAborted1, this.GetCommunicationObjectType().ToString()));
        }

        internal bool DoneReceivingInCurrentState()
        {
            this.ThrowPending();

            switch (_state)
            {
                case CommunicationState.Created:
                    throw TraceUtility.ThrowHelperError(this.CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opening:
                    throw TraceUtility.ThrowHelperError(this.CreateNotOpenException(), Guid.Empty, this);

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
            lock (ThisLock)
            {
                if (_state == CommunicationState.Closed || _state == CommunicationState.Closing)
                    return;

                if (_state == CommunicationState.Faulted)
                    return;
                _state = CommunicationState.Faulted;
            }

            OnFaulted();
        }

        public void Open()
        {
            this.Open(this.DefaultOpenTimeout);
        }

        public void Open(TimeSpan timeout)
        {
            OpenAsyncInternal(timeout).WaitForCompletion();
        }

        private async Task OpenAsyncInternal(TimeSpan timeout)
        {
            await TaskHelpers.EnsureDefaultTaskScheduler();
            await ((IAsyncCommunicationObject)this).OpenAsync(timeout);
        }

        async Task IAsyncCommunicationObject.OpenAsync(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException("timeout", SRServiceModel.SFxTimeoutOutOfRange0));

            lock (ThisLock)
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
                    throw TraceUtility.ThrowHelperError(this.CreateBaseClassMethodNotCalledException("OnOpening"), Guid.Empty, this);

                TimeSpan remainingTime = actualTimeout.RemainingTime();
                await OnOpenAsync(remainingTime);

                OnOpened();
                if (!_onOpenedCalled)
                    throw TraceUtility.ThrowHelperError(this.CreateBaseClassMethodNotCalledException("OnOpened"), Guid.Empty, this);

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

        protected virtual void OnClosed()
        {
            _onClosedCalled = true;

            lock (ThisLock)
            {
                if (_raisedClosed)
                    return;
                _raisedClosed = true;
                _state = CommunicationState.Closed;
            }


            EventHandler handler = Closed;
            if (handler != null)
            {
                try
                {
                    handler(_eventSender, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    if (Fx.IsFatal(exception))
                        throw;

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                }
            }
        }

        protected virtual void OnClosing()
        {
            _onClosingCalled = true;

            lock (ThisLock)
            {
                if (_raisedClosing)
                    return;
                _raisedClosing = true;
            }


            EventHandler handler = Closing;
            if (handler != null)
            {
                try
                {
                    handler(_eventSender, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    if (Fx.IsFatal(exception))
                        throw;

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                }
            }
        }

        protected virtual void OnFaulted()
        {
            lock (ThisLock)
            {
                if (_raisedFaulted)
                    return;
                _raisedFaulted = true;
            }

            EventHandler handler = Faulted;
            if (handler != null)
            {
                try
                {
                    handler(_eventSender, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    if (Fx.IsFatal(exception))
                        throw;

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                }
            }
        }

        protected virtual void OnOpened()
        {
            _onOpenedCalled = true;

            lock (ThisLock)
            {
                if (_aborted || _state != CommunicationState.Opening)
                    return;
                _state = CommunicationState.Opened;
            }


            EventHandler handler = Opened;
            if (handler != null)
            {
                try
                {
                    handler(_eventSender, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    if (Fx.IsFatal(exception))
                        throw;

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
                    handler(_eventSender, EventArgs.Empty);
                }
                catch (Exception exception)
                {
                    if (Fx.IsFatal(exception))
                        throw;

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
                }
            }
        }

        internal void ThrowIfFaulted()
        {
            this.ThrowPending();

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
                    throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);

                default:
                    throw Fx.AssertAndThrow("ThrowIfFaulted: Unknown CommunicationObject.state");
            }
        }

        internal void ThrowIfAborted()
        {
            if (_aborted && !_closeCalled)
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
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);

                default:
                    throw Fx.AssertAndThrow("ThrowIfClosed: Unknown CommunicationObject.state");
            }
        }

        protected virtual Type GetCommunicationObjectType()
        {
            return this.GetType();
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
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);

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
                    throw TraceUtility.ThrowHelperError(this.CreateImmutableException(), Guid.Empty, this);

                case CommunicationState.Closing:
                    throw TraceUtility.ThrowHelperError(this.CreateImmutableException(), Guid.Empty, this);

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);

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
                    throw TraceUtility.ThrowHelperError(this.CreateImmutableException(), Guid.Empty, this);

                case CommunicationState.Opened:
                    throw TraceUtility.ThrowHelperError(this.CreateImmutableException(), Guid.Empty, this);

                case CommunicationState.Closing:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);

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
                    throw TraceUtility.ThrowHelperError(this.CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opening:
                    throw TraceUtility.ThrowHelperError(this.CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opened:
                    break;

                case CommunicationState.Closing:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);

                default:
                    throw Fx.AssertAndThrow("ThrowIfDisposedOrNotOpen: Unknown CommunicationObject.state");
            }
        }

        internal void ThrowIfNotOpened()
        {
            if (_state == CommunicationState.Created || _state == CommunicationState.Opening)
                throw TraceUtility.ThrowHelperError(this.CreateNotOpenException(), Guid.Empty, this);
        }

        internal void ThrowIfClosedOrNotOpen()
        {
            ThrowPending();

            switch (_state)
            {
                case CommunicationState.Created:
                    throw TraceUtility.ThrowHelperError(this.CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opening:
                    throw TraceUtility.ThrowHelperError(this.CreateNotOpenException(), Guid.Empty, this);

                case CommunicationState.Opened:
                    break;

                case CommunicationState.Closing:
                    break;

                case CommunicationState.Closed:
                    throw TraceUtility.ThrowHelperError(this.CreateClosedException(), Guid.Empty, this);

                case CommunicationState.Faulted:
                    throw TraceUtility.ThrowHelperError(this.CreateFaultedException(), Guid.Empty, this);

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
            Contract.Requires(false, "OnCloseAsync needs to be implemented on derived classes");
            return TaskHelpers.CompletedTask();
        }

        internal protected virtual Task OnOpenAsync(TimeSpan timeout)
        {
            Contract.Requires(false, "OnOpenAsync needs to be implemented on derived classes");
            return TaskHelpers.CompletedTask();
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
