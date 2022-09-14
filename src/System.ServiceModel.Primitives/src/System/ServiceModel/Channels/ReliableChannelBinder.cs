// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal enum TolerateFaultsMode
    {
        Never,
        IfNotSecuritySession,
        Always
    }

    [Flags]
    internal enum MaskingMode
    {
        None = 0x0,
        Handled = 0x1,
        Unhandled = 0x2,
        All = Handled | Unhandled
    }

    internal abstract class ReliableChannelBinder<TChannel> : IReliableChannelBinder
        where TChannel : class, IChannel
    {
        private bool _aborted = false;
        private TimeSpan _defaultCloseTimeout;
        private AsyncCallback _onCloseChannelComplete;
        private object _thisLock = new object();

        protected ReliableChannelBinder(TChannel channel, MaskingMode maskingMode,
            TolerateFaultsMode faultMode, TimeSpan defaultCloseTimeout,
            TimeSpan defaultSendTimeout)
        {
            if ((maskingMode != MaskingMode.None) && (maskingMode != MaskingMode.All))
            {
                throw Fx.AssertAndThrow("ReliableChannelBinder was implemented with only 2 default masking modes, None and All.");
            }

            DefaultMaskingMode = maskingMode;
            _defaultCloseTimeout = defaultCloseTimeout;
            DefaultSendTimeout = defaultSendTimeout;

            Synchronizer = new ChannelSynchronizer(this, channel, faultMode);
        }

        protected abstract bool CanGetChannelForReceive
        {
            get;
        }

        public abstract bool CanSendAsynchronously
        {
            get;
        }

        public virtual ChannelParameterCollection ChannelParameters
        {
            get { return null; }
        }

        public IChannel Channel
        {
            get
            {
                return Synchronizer.CurrentChannel;
            }
        }

        public bool Connected
        {
            get
            {
                return Synchronizer.Connected;
            }
        }

        public MaskingMode DefaultMaskingMode { get; }

        public TimeSpan DefaultSendTimeout { get; }

        public abstract bool HasSession
        {
            get;
        }

        public abstract EndpointAddress LocalAddress
        {
            get;
        }

        protected abstract bool MustCloseChannel
        {
            get;
        }

        protected abstract bool MustOpenChannel
        {
            get;
        }

        public abstract EndpointAddress RemoteAddress
        {
            get;
        }

        public CommunicationState State { get; private set; } = CommunicationState.Created;

        protected ChannelSynchronizer Synchronizer { get; }

        protected object ThisLock
        {
            get
            {
                return _thisLock;
            }
        }

        private bool TolerateFaults
        {
            get
            {
                return Synchronizer.TolerateFaults;
            }
        }

        public event EventHandler ConnectionLost;
        public event BinderExceptionHandler Faulted;
        public event BinderExceptionHandler OnException;


        public void Abort()
        {
            TChannel channel;
            lock (ThisLock)
            {
                _aborted = true;

                if (State == CommunicationState.Closed)
                {
                    return;
                }

                State = CommunicationState.Closing;
                channel = Synchronizer.StopSynchronizing(true);

                if (!MustCloseChannel)
                {
                    channel = null;
                }
            }

            Synchronizer.UnblockWaiters();
            OnShutdown();
            OnAbort();

            if (channel != null)
            {
                channel.Abort();
            }

            TransitionToClosed();
        }

        protected virtual void AddOutputHeaders(Message message)
        {
        }

        private bool CloseCore(out TChannel channel)
        {
            channel = null;
            bool abort = true;
            bool abortChannel = false;

            lock (ThisLock)
            {
                if ((State == CommunicationState.Closing)
                    || (State == CommunicationState.Closed))
                {
                    return true;
                }

                if (State == CommunicationState.Opened)
                {
                    State = CommunicationState.Closing;
                    channel = Synchronizer.StopSynchronizing(true);
                    abort = false;

                    if (!MustCloseChannel)
                    {
                        channel = null;
                    }

                    if (channel != null)
                    {
                        CommunicationState channelState = channel.State;

                        if ((channelState == CommunicationState.Created)
                            || (channelState == CommunicationState.Opening)
                            || (channelState == CommunicationState.Faulted))
                        {
                            abortChannel = true;
                        }
                        else if ((channelState == CommunicationState.Closing)
                            || (channelState == CommunicationState.Closed))
                        {
                            channel = null;
                        }
                    }
                }
            }

            Synchronizer.UnblockWaiters();

            if (abort)
            {
                Abort();
                return true;
            }
            else
            {
                if (abortChannel)
                {
                    channel.Abort();
                    channel = null;
                }

                return false;
            }
        }

        public Task CloseAsync(TimeSpan timeout)
        {
            return CloseAsync(timeout, DefaultMaskingMode);
        }

        public async Task CloseAsync(TimeSpan timeout, MaskingMode maskingMode)
        {
            ThrowIfTimeoutNegative(timeout);
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            TChannel channel;

            if (CloseCore(out channel))
            {
                return;
            }

            try
            {
                OnShutdown();
                await OnCloseAsync(timeoutHelper.RemainingTime());

                if (channel != null)
                {
                    await CloseChannelAsync(channel, timeoutHelper.RemainingTime());
                }

                TransitionToClosed();
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                Abort();

                if (!HandleException(e, maskingMode))
                {
                    throw;
                }
            }
        }

        // The ChannelSynchronizer calls this from an operation thread so this method must not
        // block.
        private void CloseChannel(TChannel channel)
        {
            if (!MustCloseChannel)
            {
                throw Fx.AssertAndThrow("MustCloseChannel is false when there is no receive loop and this method is called when there is a receive loop.");
            }

            if (_onCloseChannelComplete == null)
            {
                _onCloseChannelComplete = Fx.ThunkCallback(new AsyncCallback(OnCloseChannelComplete));
            }

            try
            {
                IAsyncResult result = channel.BeginClose(_onCloseChannelComplete, channel);

                if (result.CompletedSynchronously)
                {
                    channel.EndClose(result);
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                HandleException(e, MaskingMode.All);
            }
        }

        protected virtual Task CloseChannelAsync(TChannel channel, TimeSpan timeout)
        {
            return channel.CloseHelperAsync(timeout);
        }

        protected void Fault(Exception e)
        {
            lock (ThisLock)
            {
                if (State == CommunicationState.Created)
                {
                    throw Fx.AssertAndThrow("The binder should not detect the inner channel's faults until after the binder is opened.");
                }

                if ((State == CommunicationState.Faulted)
                    || (State == CommunicationState.Closed))
                {
                    return;
                }

                State = CommunicationState.Faulted;
                Synchronizer.StopSynchronizing(false);
            }

            Synchronizer.UnblockWaiters();

            BinderExceptionHandler handler = Faulted;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        // ChannelSynchronizer helper, cannot take a lock.
        private Exception GetClosedException(MaskingMode maskingMode)
        {
            if (ReliableChannelBinderHelper.MaskHandled(maskingMode))
            {
                return null;
            }
            else if (_aborted)
            {
                return new CommunicationObjectAbortedException(SRP.Format(
                    SRP.CommunicationObjectAborted1, GetType().ToString()));
            }
            else
            {
                return new ObjectDisposedException(GetType().ToString());
            }
        }

        // Must be called within lock(ThisLock)
        private Exception GetClosedOrFaultedException(MaskingMode maskingMode)
        {
            if (State == CommunicationState.Faulted)
            {
                return GetFaultedException(maskingMode);
            }
            else if ((State == CommunicationState.Closing)
               || (State == CommunicationState.Closed))
            {
                return GetClosedException(maskingMode);
            }
            else
            {
                throw Fx.AssertAndThrow("Caller is attempting to get a terminal exception in a non-terminal state.");
            }
        }

        // ChannelSynchronizer helper, cannot take a lock.
        private Exception GetFaultedException(MaskingMode maskingMode)
        {
            if (ReliableChannelBinderHelper.MaskHandled(maskingMode))
            {
                return null;
            }
            else
            {
                return new CommunicationObjectFaultedException(SRP.Format(
                    SRP.CommunicationObjectFaulted1, GetType().ToString()));
            }
        }

        public abstract ISession GetInnerSession();

        public void HandleException(Exception e)
        {
            HandleException(e, MaskingMode.All);
        }

        protected bool HandleException(Exception e, MaskingMode maskingMode)
        {
            if (TolerateFaults && (e is CommunicationObjectFaultedException))
            {
                return true;
            }

            if (IsHandleable(e))
            {
                return ReliableChannelBinderHelper.MaskHandled(maskingMode);
            }

            bool maskUnhandled = ReliableChannelBinderHelper.MaskUnhandled(maskingMode);

            if (maskUnhandled)
            {
                RaiseOnException(e);
            }

            return maskUnhandled;
        }

        protected bool HandleException(Exception e, MaskingMode maskingMode, bool autoAborted)
        {
            if (TolerateFaults && autoAborted && e is CommunicationObjectAbortedException)
            {
                return true;
            }

            return HandleException(e, maskingMode);
        }

        // ChannelSynchronizer helper, cannot take a lock.
        protected abstract bool HasSecuritySession(TChannel channel);

        public bool IsHandleable(Exception e)
        {
            if (e is ProtocolException)
            {
                return false;
            }

            return (e is CommunicationException)
                || (e is TimeoutException);
        }

        protected abstract void OnAbort();

        protected abstract Task OnCloseAsync(TimeSpan timeout);

        private void OnCloseChannelComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            TChannel channel = (TChannel)result.AsyncState;

            try
            {
                channel.EndClose(result);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                HandleException(e, MaskingMode.All);
            }
        }

        private void OnInnerChannelFaulted()
        {
            if (!TolerateFaults)
            {
                return;
            }

            ConnectionLost?.Invoke(this, EventArgs.Empty);
        }

        protected abstract Task OnOpenAsync(TimeSpan timeout);

        private void OnOpened()
        {
            lock (ThisLock)
            {
                if (State == CommunicationState.Opening)
                {
                    State = CommunicationState.Opened;
                }
            }
        }

        private bool OnOpening(MaskingMode maskingMode)
        {
            lock (ThisLock)
            {
                if (State != CommunicationState.Created)
                {
                    Exception e = null;

                    if ((State == CommunicationState.Opening)
                        || (State == CommunicationState.Opened))
                    {
                        if (!ReliableChannelBinderHelper.MaskUnhandled(maskingMode))
                        {
                            e = new InvalidOperationException(SRP.Format(
                                SRP.CommunicationObjectCannotBeModifiedInState,
                                GetType().ToString(), State.ToString()));
                        }
                    }
                    else
                    {
                        e = GetClosedOrFaultedException(maskingMode);
                    }

                    if (e != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(e);
                    }

                    return false;
                }
                else
                {
                    State = CommunicationState.Opening;
                    return true;
                }
            }
        }

        protected virtual void OnShutdown()
        {
        }

        protected virtual Task OnSendAsync(TChannel channel, Message message, TimeSpan timeout)
        {
            throw Fx.AssertAndThrow("The derived class does not support the Send operation.");
        }

        protected virtual Task<(bool success, RequestContext requestContext)> OnTryReceiveAsync(TChannel channel, TimeSpan timeout)
        {
            throw Fx.AssertAndThrow("The derived class does not support the TryReceive operation.");
        }

        public async Task OpenAsync(TimeSpan timeout)
        {
            ThrowIfTimeoutNegative(timeout);

            if (!OnOpening(DefaultMaskingMode))
            {
                return;
            }

            try
            {
                await OnOpenAsync(timeout);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                Fault(null);

                if (DefaultMaskingMode == MaskingMode.None)
                {
                    throw;
                }
                else
                {
                    RaiseOnException(e);
                    return;
                }
            }

            await Synchronizer.StartSynchronizingAsync();
            OnOpened();
        }

        private void RaiseOnException(Exception e)
        {
            BinderExceptionHandler handler = OnException;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public Task SendAsync(Message message, TimeSpan timeout)
        {
            return SendAsync(message, timeout, DefaultMaskingMode);
        }

        public async Task SendAsync(Message message, TimeSpan timeout, MaskingMode maskingMode)
        {
            if (!ValidateOutputOperation(message, timeout, maskingMode))
            {
                return;
            }

            bool autoAborted = false;

            try
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                (bool success, TChannel channel) = await Synchronizer.TryGetChannelForOutputAsync(timeoutHelper.RemainingTime(), maskingMode);
                if (!success)
                {
                    if (!ReliableChannelBinderHelper.MaskHandled(maskingMode))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new TimeoutException(SRP.Format(SRP.TimeoutOnSend, timeout)));
                    }

                    return;
                }

                if (channel == null)
                {
                    return;
                }

                AddOutputHeaders(message);

                try
                {
                    await OnSendAsync(channel, message, timeoutHelper.RemainingTime());
                }
                finally
                {
                    autoAborted = Synchronizer.Aborting;
                    Synchronizer.ReturnChannel();
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                if (!HandleException(e, maskingMode, autoAborted))
                {
                    throw;
                }
            }
        }

        public void SetMaskingMode(RequestContext context, MaskingMode maskingMode)
        {
            BinderRequestContext binderContext = (BinderRequestContext)context;
            binderContext.SetMaskingMode(maskingMode);
        }

        // throwDisposed indicates whether to throw in the Faulted, Closing, and Closed states.
        // returns true if in Opened state
        private bool ThrowIfNotOpenedAndNotMasking(MaskingMode maskingMode, bool throwDisposed)
        {
            lock (ThisLock)
            {
                if (State == CommunicationState.Created)
                {
                    throw Fx.AssertAndThrow("Messaging operations cannot be called when the binder is in the Created state.");
                }

                if (State == CommunicationState.Opening)
                {
                    throw Fx.AssertAndThrow("Messaging operations cannot be called when the binder is in the Opening state.");
                }

                if (State == CommunicationState.Opened)
                {
                    return true;
                }

                // state is Faulted, Closing, or Closed
                if (throwDisposed)
                {
                    Exception e = GetClosedOrFaultedException(maskingMode);

                    if (e != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(e);
                    }
                }

                return false;
            }
        }

        private void ThrowIfTimeoutNegative(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException(nameof(timeout), timeout, SRP.SFxTimeoutOutOfRange0));
            }
        }

        private void TransitionToClosed()
        {
            lock (ThisLock)
            {
                if ((State != CommunicationState.Closing)
                    && (State != CommunicationState.Closed)
                    && (State != CommunicationState.Faulted))
                {
                    throw Fx.AssertAndThrow("Caller cannot transition to the Closed state from a non-terminal state.");
                }

                State = CommunicationState.Closed;
            }
        }

        // ChannelSynchronizer helper, cannot take a lock.
        protected abstract Task<bool> TryGetChannelAsync(TimeSpan timeout);

        public virtual Task<(bool, RequestContext)> TryReceiveAsync(TimeSpan timeout)
        {
            return TryReceiveAsync(timeout, DefaultMaskingMode);
        }

        public virtual async Task<(bool, RequestContext)> TryReceiveAsync(TimeSpan timeout, MaskingMode maskingMode)
        {
            if (!ValidateInputOperation(timeout))
            {
                return (true, null);
            }

            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            while (true)
            {
                bool autoAborted = false;

                try
                {
                    (bool success, TChannel channel) = await Synchronizer.TryGetChannelForInputAsync(
                        CanGetChannelForReceive, timeoutHelper.RemainingTime());
                    success = !success;

                    // the synchronizer is faulted and not reestablishing or closed, or the call timed
                    // out, complete and don't retry.
                    if (channel == null)
                    {
                        return (success, null);
                    }

                    try
                    {
                        RequestContext requestContext;
                        (success, requestContext) = await OnTryReceiveAsync(channel, timeoutHelper.RemainingTime());

                        // timed out || got message, return immediately
                        if (!success || (requestContext != null))
                        {
                            return (success, requestContext);
                        }

                        // the underlying channel closed or faulted, retry
                        Synchronizer.OnReadEof();
                    }
                    finally
                    {
                        autoAborted = Synchronizer.Aborting;
                        Synchronizer.ReturnChannel();
                    }
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    if (!HandleException(e, maskingMode, autoAborted))
                    {
                        throw;
                    }
                }
            }
        }

        protected bool ValidateInputOperation(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(timeout), timeout,
                    SRP.SFxTimeoutOutOfRange0));
            }

            return ThrowIfNotOpenedAndNotMasking(MaskingMode.All, false);
        }

        protected bool ValidateOutputOperation(Message message, TimeSpan timeout, MaskingMode maskingMode)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));
            }

            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(timeout), timeout,
                    SRP.SFxTimeoutOutOfRange0));
            }

            return ThrowIfNotOpenedAndNotMasking(maskingMode, true);
        }

        internal Task WaitForPendingOperationsAsync(TimeSpan timeout)
        {
            return Synchronizer.WaitForPendingOperationsAsync(timeout);
        }

        protected RequestContext WrapMessage(Message message)
        {
            if (message == null)
            {
                return null;
            }

            return new MessageRequestContext(this, message);
        }

        public RequestContext WrapRequestContext(RequestContext context)
        {
            if (context == null)
            {
                return null;
            }

            if (!TolerateFaults && DefaultMaskingMode == MaskingMode.None)
            {
                return context;
            }

            return new RequestRequestContext(this, context, context.RequestMessage);
        }

        private abstract class BinderRequestContext : RequestContextBase
        {
            private MaskingMode _maskingMode;

            public BinderRequestContext(ReliableChannelBinder<TChannel> binder, Message message)
                : base(message, binder._defaultCloseTimeout, binder.DefaultSendTimeout)
            {
                if (binder == null)
                {
                    Fx.Assert("Argument binder cannot be null.");
                }

                Binder = binder;
                _maskingMode = binder.DefaultMaskingMode;
            }

            protected ReliableChannelBinder<TChannel> Binder { get; }

            protected MaskingMode MaskingMode
            {
                get
                {
                    return _maskingMode;
                }
            }

            public void SetMaskingMode(MaskingMode maskingMode)
            {
                if (Binder.DefaultMaskingMode != MaskingMode.All)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
                }

                _maskingMode = maskingMode;
            }
        }

        protected class ChannelSynchronizer
        {
            private ReliableChannelBinder<TChannel> _binder;
            private int _count = 0;
            private InterruptibleWaitObject _drainEvent;
            private TolerateFaultsMode _faultMode;
            private Queue<IWaiter> _getChannelQueue;
            private bool _innerChannelFaulted;
            private EventHandler _onChannelFaulted;
            private State _state = State.Created;
            private Queue<IWaiter> _waitQueue;

            public ChannelSynchronizer(ReliableChannelBinder<TChannel> binder, TChannel channel,
                TolerateFaultsMode faultMode)
            {
                _binder = binder;
                CurrentChannel = channel;
                _faultMode = faultMode;
            }

            public bool Aborting { get; private set; }

            public bool Connected
            {
                get
                {
                    return (_state == State.ChannelOpened ||
                        _state == State.ChannelOpening);
                }
            }

            public TChannel CurrentChannel { get; private set; }

            private AsyncLock ThisLock { get; } = new AsyncLock();

            public bool TolerateFaults { get; private set; } = true;

            // Server only API.
            public TChannel AbortCurentChannel()
            {
                using (ThisLock.TakeLock())
                {
                    if (!TolerateFaults)
                    {
                        throw Fx.AssertAndThrow("It is only valid to abort the current channel when masking faults");
                    }

                    if (_state == State.ChannelOpening)
                    {
                        Aborting = true;
                    }
                    else if (_state == State.ChannelOpened)
                    {
                        if (_count == 0)
                        {
                            _state = State.NoChannel;
                        }
                        else
                        {
                            Aborting = true;
                            _state = State.ChannelClosing;
                        }
                    }
                    else
                    {
                        return null;
                    }

                    return CurrentChannel;
                }
            }

            private bool CompleteSetChannel(IWaiter waiter, out TChannel channel)
            {
                if (waiter == null)
                {
                    throw Fx.AssertAndThrow("Argument waiter cannot be null.");
                }

                bool close = false;

                using (ThisLock.TakeLock())
                {
                    if (ValidateOpened())
                    {
                        channel = CurrentChannel;
                        return true;
                    }
                    else
                    {
                        channel = null;
                        close = _state == State.Closed;
                    }
                }

                if (close)
                {
                    waiter.Close();
                }
                else
                {
                    waiter.Fault();
                }

                return false;
            }

            // Client API only.
            public async Task<bool> EnsureChannelAsync()
            {
                bool fault = false;

                await using (await ThisLock.TakeLockAsync())
                {
                    if (ValidateOpened())
                    {
                        // This is called only during the RM CS phase. In this phase, there are 2
                        // valid states between Request calls, ChannelOpened and NoChannel.
                        if (_state == State.ChannelOpened)
                        {
                            return true;
                        }

                        if (_state != State.NoChannel)
                        {
                            throw Fx.AssertAndThrow("The caller may only invoke this EnsureChannel during the CreateSequence negotiation. ChannelOpening and ChannelClosing are invalid states during this phase of the negotiation.");
                        }

                        if (!TolerateFaults)
                        {
                            fault = true;
                        }
                        else
                        {
                            if (GetCurrentChannelIfCreated() != null)
                            {
                                return true;
                            }

                            if (await _binder.TryGetChannelAsync(TimeSpan.Zero))
                            {
                                if (CurrentChannel == null)
                                {
                                    return false;
                                }

                                return true;
                            }
                        }
                    }
                }

                if (fault)
                {
                    _binder.Fault(null);
                }

                return false;
            }

            private IWaiter GetChannelWaiter()
            {
                if ((_getChannelQueue == null) || (_getChannelQueue.Count == 0))
                {
                    return null;
                }

                return _getChannelQueue.Dequeue();
            }

            // Must be called within using(await ThisLock.TakeLockAsync())
            private TChannel GetCurrentChannelIfCreated()
            {
                if (_state != State.NoChannel)
                {
                    throw Fx.AssertAndThrow("This method may only be called in the NoChannel state.");
                }

                if ((CurrentChannel != null)
                    && (CurrentChannel.State == CommunicationState.Created))
                {
                    return CurrentChannel;
                }
                else
                {
                    return null;
                }
            }

            private Queue<IWaiter> GetQueue(bool canGetChannel)
            {
                if (canGetChannel)
                {
                    if (_getChannelQueue == null)
                    {
                        _getChannelQueue = new Queue<IWaiter>();
                    }

                    return _getChannelQueue;
                }
                else
                {
                    if (_waitQueue == null)
                    {
                        _waitQueue = new Queue<IWaiter>();
                    }

                    return _waitQueue;
                }
            }

            private void OnChannelFaulted(object sender, EventArgs e)
            {
                TChannel faultedChannel = (TChannel)sender;
                bool faultBinder = false;
                bool raiseInnerChannelFaulted = false;

                using (ThisLock.TakeLock())
                {
                    if (CurrentChannel != faultedChannel)
                    {
                        return;
                    }

                    // The synchronizer is already closed or aborted.
                    if (!ValidateOpened())
                    {
                        return;
                    }

                    if (_state == State.ChannelOpened)
                    {
                        if (_count == 0)
                        {
                            faultedChannel.Faulted -= _onChannelFaulted;
                        }

                        faultBinder = !TolerateFaults;
                        _state = State.ChannelClosing;
                        _innerChannelFaulted = true;

                        if (!faultBinder && _count == 0)
                        {
                            _state = State.NoChannel;
                            Aborting = false;
                            raiseInnerChannelFaulted = true;
                            _innerChannelFaulted = false;
                        }
                    }
                }

                if (faultBinder)
                {
                    _binder.Fault(null);
                }

                faultedChannel.Abort();

                if (raiseInnerChannelFaulted)
                {
                    _binder.OnInnerChannelFaulted();
                }
            }

            private bool OnChannelOpened(IWaiter waiter)
            {
                if (waiter == null)
                {
                    throw Fx.AssertAndThrow("Argument waiter cannot be null.");
                }

                bool close = false;
                bool fault = false;

                Queue<IWaiter> temp1 = null;
                Queue<IWaiter> temp2 = null;
                TChannel channel = null;

                using (ThisLock.TakeLock())
                {
                    if (CurrentChannel == null)
                    {
                        throw Fx.AssertAndThrow("Caller must ensure that field currentChannel is set before opening the channel.");
                    }

                    if (ValidateOpened())
                    {
                        if (_state != State.ChannelOpening)
                        {
                            throw Fx.AssertAndThrow("This method may only be called in the ChannelOpening state.");
                        }

                        _state = State.ChannelOpened;
                        SetTolerateFaults();

                        _count += 1;
                        _count += (_getChannelQueue == null) ? 0 : _getChannelQueue.Count;
                        _count += (_waitQueue == null) ? 0 : _waitQueue.Count;

                        temp1 = _getChannelQueue;
                        temp2 = _waitQueue;
                        channel = CurrentChannel;

                        _getChannelQueue = null;
                        _waitQueue = null;
                    }
                    else
                    {
                        close = _state == State.Closed;
                        fault = _state == State.Faulted;
                    }
                }

                if (close)
                {
                    waiter.Close();
                    return false;
                }
                else if (fault)
                {
                    waiter.Fault();
                    return false;
                }

                SetWaiters(temp1, channel);
                SetWaiters(temp2, channel);
                return true;
            }

            private void OnGetChannelFailed()
            {
                IWaiter waiter = null;

                using (ThisLock.TakeLock())
                {
                    if (!ValidateOpened())
                    {
                        return;
                    }

                    if (_state != State.ChannelOpening)
                    {
                        throw Fx.AssertAndThrow("The state must be set to ChannelOpening before the caller attempts to open the channel.");
                    }

                    waiter = GetChannelWaiter();

                    if (waiter == null)
                    {
                        _state = State.NoChannel;
                        return;
                    }
                }

                waiter.GetChannel(false);
            }

            public void OnReadEof()
            {
                using (ThisLock.TakeLock())
                {
                    if (_count <= 0)
                    {
                        throw Fx.AssertAndThrow("Caller must ensure that OnReadEof is called before ReturnChannel.");
                    }

                    if (ValidateOpened())
                    {
                        if ((_state != State.ChannelOpened) && (_state != State.ChannelClosing))
                        {
                            throw Fx.AssertAndThrow("Since count is positive, the only valid states are ChannelOpened and ChannelClosing.");
                        }

                        if (CurrentChannel.State != CommunicationState.Faulted)
                        {
                            _state = State.ChannelClosing;
                        }
                    }
                }
            }

            private bool RemoveWaiter(IWaiter waiter)
            {
                Queue<IWaiter> waiters = waiter.CanGetChannel ? _getChannelQueue : _waitQueue;

                if (waiters == null)
                {
                    return false;
                }

                bool removed = false;

                using (ThisLock.TakeLock())
                {
                    if (!ValidateOpened())
                    {
                        return false;
                    }

                    for (int i = waiters.Count; i > 0; i--)
                    {
                        IWaiter temp = waiters.Dequeue();

                        if (object.ReferenceEquals(waiter, temp))
                        {
                            removed = true;
                        }
                        else
                        {
                            waiters.Enqueue(temp);
                        }
                    }
                }

                return removed;
            }

            public void ReturnChannel()
            {
                TChannel channel = null;
                IWaiter waiter = null;
                bool faultBinder = false;
                bool drained;
                bool raiseInnerChannelFaulted = false;

                using (ThisLock.TakeLock())
                {
                    if (_count <= 0)
                    {
                        throw Fx.AssertAndThrow("Method ReturnChannel() can only be called after TryGetChannel or EndTryGetChannel returns a channel.");
                    }

                    _count--;
                    drained = (_count == 0) && (_drainEvent != null);

                    if (ValidateOpened())
                    {
                        if ((_state != State.ChannelOpened) && (_state != State.ChannelClosing))
                        {
                            throw Fx.AssertAndThrow("ChannelOpened and ChannelClosing are the only 2 valid states when count is positive.");
                        }

                        if (CurrentChannel.State == CommunicationState.Faulted)
                        {
                            faultBinder = !TolerateFaults;
                            _innerChannelFaulted = true;
                            _state = State.ChannelClosing;
                        }

                        if (!faultBinder && (_state == State.ChannelClosing) && (_count == 0))
                        {
                            channel = CurrentChannel;
                            raiseInnerChannelFaulted = _innerChannelFaulted;
                            _innerChannelFaulted = false;

                            _state = State.NoChannel;
                            Aborting = false;

                            waiter = GetChannelWaiter();

                            if (waiter != null)
                            {
                                _state = State.ChannelOpening;
                            }
                        }
                    }
                }

                if (faultBinder)
                {
                    _binder.Fault(null);
                }

                if (drained)
                {
                    _drainEvent.Set();
                }

                if (channel != null)
                {
                    channel.Faulted -= _onChannelFaulted;

                    if (channel.State == CommunicationState.Opened)
                    {
                        _binder.CloseChannel(channel);
                    }
                    else
                    {
                        channel.Abort();
                    }

                    if (waiter != null)
                    {
                        waiter.GetChannel(false);
                    }
                }

                if (raiseInnerChannelFaulted)
                {
                    _binder.OnInnerChannelFaulted();
                }
            }

            public bool SetChannel(TChannel channel)
            {
                using (ThisLock.TakeLock())
                {
                    if (_state != State.ChannelOpening && _state != State.NoChannel)
                    {
                        throw Fx.AssertAndThrow("SetChannel is only valid in the NoChannel and ChannelOpening states");
                    }

                    if (!TolerateFaults)
                    {
                        throw Fx.AssertAndThrow("SetChannel is only valid when masking faults");
                    }

                    if (ValidateOpened())
                    {
                        CurrentChannel = channel;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            private void SetTolerateFaults()
            {
                if (_faultMode == TolerateFaultsMode.Never)
                {
                    TolerateFaults = false;
                }
                else if (_faultMode == TolerateFaultsMode.IfNotSecuritySession)
                {
                    TolerateFaults = !_binder.HasSecuritySession(CurrentChannel);
                }

                if (_onChannelFaulted == null)
                {
                    _onChannelFaulted = new EventHandler(OnChannelFaulted);
                }

                CurrentChannel.Faulted += _onChannelFaulted;
            }

            private void SetWaiters(Queue<IWaiter> waiters, TChannel channel)
            {
                if ((waiters != null) && (waiters.Count > 0))
                {
                    foreach (IWaiter waiter in waiters)
                    {
                        waiter.Set(channel);
                    }
                }
            }

            public async Task StartSynchronizingAsync()
            {
                await using (await ThisLock.TakeLockAsync())
                {
                    if (_state == State.Created)
                    {
                        _state = State.NoChannel;
                    }
                    else
                    {
                        if (_state != State.Closed)
                        {
                            throw Fx.AssertAndThrow("Abort is the only operation that can race with Open.");
                        }

                        return;
                    }

                    if (CurrentChannel == null)
                    {
                        if (!await _binder.TryGetChannelAsync(TimeSpan.Zero))
                        {
                            return;
                        }
                    }

                    if (CurrentChannel == null)
                    {
                        return;
                    }

                    if (!_binder.MustOpenChannel)
                    {
                        // Channel is already opened.
                        _state = State.ChannelOpened;
                        SetTolerateFaults();
                    }
                }
            }

            public TChannel StopSynchronizing(bool close)
            {
                using (ThisLock.TakeLock())
                {
                    if ((_state != State.Faulted) && (_state != State.Closed))
                    {
                        _state = close ? State.Closed : State.Faulted;

                        if ((CurrentChannel != null) && (_onChannelFaulted != null))
                        {
                            CurrentChannel.Faulted -= _onChannelFaulted;
                        }
                    }

                    return CurrentChannel;
                }
            }

            // Must be called under a lock.
            private bool ThrowIfNecessary(MaskingMode maskingMode)
            {
                if (ValidateOpened())
                {
                    return true;
                }

                // state is Closed or Faulted.
                Exception e;

                if (_state == State.Closed)
                {
                    e = _binder.GetClosedException(maskingMode);
                }
                else
                {
                    e = _binder.GetFaultedException(maskingMode);
                }

                if (e != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(e);
                }

                return false;
            }

            public Task<(bool success, TChannel channel)> TryGetChannelForInputAsync(bool canGetChannel, TimeSpan timeout)
            {
                return TryGetChannelAsync(canGetChannel, false, timeout, MaskingMode.All);
            }

            public Task<(bool success, TChannel channel)> TryGetChannelForOutputAsync(TimeSpan timeout, MaskingMode maskingMode)
            {
                return TryGetChannelAsync(true, true, timeout, maskingMode);
            }

            private async Task<(bool success, TChannel channel)> TryGetChannelAsync(bool canGetChannel, bool canCauseFault, TimeSpan timeout,
                MaskingMode maskingMode)
            {
                TaskWaiter waiter = null;
                bool faulted = false;
                bool getChannel = false;

                await using (await ThisLock.TakeLockAsync())
                {
                    if (!ThrowIfNecessary(maskingMode))
                    {
                        return (true, (TChannel)null);
                    }

                    if (_state == State.ChannelOpened)
                    {
                        if (CurrentChannel == null)
                        {
                            throw Fx.AssertAndThrow("Field currentChannel cannot be null in the ChannelOpened state.");
                        }

                        _count++;
                        return (true, CurrentChannel);
                    }

                    if (!TolerateFaults
                        && ((_state == State.ChannelClosing)
                        || (_state == State.NoChannel)))
                    {
                        if (!canCauseFault)
                        {
                            return (true, (TChannel)null);
                        }

                        faulted = true;
                    }
                    else if (!canGetChannel
                        || (_state == State.ChannelOpening)
                        || (_state == State.ChannelClosing))
                    {
                        waiter = new TaskWaiter(this, canGetChannel, null, timeout, maskingMode, _binder.ChannelParameters);
                        GetQueue(canGetChannel).Enqueue(waiter);
                    }
                    else
                    {
                        if (_state != State.NoChannel)
                        {
                            throw Fx.AssertAndThrow("The state must be NoChannel.");
                        }

                        waiter = new TaskWaiter(this, canGetChannel,
                            GetCurrentChannelIfCreated(), timeout, maskingMode,
                            _binder.ChannelParameters);

                        _state = State.ChannelOpening;
                        getChannel = true;
                    }
                }

                if (faulted)
                {
                    _binder.Fault(null);
                    return (true, (TChannel)null);
                }

                if (getChannel)
                {
                    waiter.GetChannel(true);
                }

                return await waiter.TryWaitAsync();
            }

            public void UnblockWaiters()
            {
                Queue<IWaiter> temp1;
                Queue<IWaiter> temp2;

                using (ThisLock.TakeLock())
                {
                    temp1 = _getChannelQueue;
                    temp2 = _waitQueue;

                    _getChannelQueue = null;
                    _waitQueue = null;
                }

                bool close = _state == State.Closed;
                UnblockWaiters(temp1, close);
                UnblockWaiters(temp2, close);
            }

            private void UnblockWaiters(Queue<IWaiter> waiters, bool close)
            {
                if ((waiters != null) && (waiters.Count > 0))
                {
                    foreach (IWaiter waiter in waiters)
                    {
                        if (close)
                        {
                            waiter.Close();
                        }
                        else
                        {
                            waiter.Fault();
                        }
                    }
                }
            }

            private bool ValidateOpened()
            {
                if (_state == State.Created)
                {
                    throw Fx.AssertAndThrow("This operation expects that the synchronizer has been opened.");
                }

                return (_state != State.Closed) && (_state != State.Faulted);
            }

            public async Task WaitForPendingOperationsAsync(TimeSpan timeout)
            {
                await using (await ThisLock.TakeLockAsync())
                {
                    if (_drainEvent != null)
                    {
                        throw Fx.AssertAndThrow("The WaitForPendingOperations operation may only be invoked once.");
                    }

                    if (_count > 0)
                    {
                        _drainEvent = new InterruptibleWaitObject(false, false);
                    }
                }

                if (_drainEvent != null)
                {
                    await _drainEvent.WaitAsync(timeout);
                }
            }

            private enum State
            {
                Created,
                NoChannel,
                ChannelOpening,
                ChannelOpened,
                ChannelClosing,
                Faulted,
                Closed
            }

            public interface IWaiter
            {
                bool CanGetChannel { get; }

                void Close();
                void Fault();
                void GetChannel(bool onUserThread);
                void Set(TChannel channel);
            }

            private sealed class TaskWaiter : IWaiter
            {
                private TChannel _channel;
                private ChannelParameterCollection _channelParameters;
                private bool _getChannel = false;
                private MaskingMode _maskingMode;
                private ChannelSynchronizer _synchronizer;
                private TimeoutHelper _timeoutHelper;
                private TaskCompletionSource<object> _tcs;

                public TaskWaiter(ChannelSynchronizer synchronizer, bool canGetChannel,
                    TChannel channel, TimeSpan timeout, MaskingMode maskingMode,
                    ChannelParameterCollection channelParameters)
                {
                    if (!canGetChannel)
                    {
                        if (channel != null)
                        {
                            throw Fx.AssertAndThrow("This waiter must wait for a channel thus argument channel must be null.");
                        }
                    }

                    _synchronizer = synchronizer;
                    CanGetChannel = canGetChannel;
                    _channel = channel;
                    _timeoutHelper = new TimeoutHelper(timeout);
                    _maskingMode = maskingMode;
                    _channelParameters = channelParameters;
                    _tcs = new TaskCompletionSource<object>();
                }

                public bool CanGetChannel { get; }

                public void Close()
                {
                    var exception = _synchronizer._binder.GetClosedException(_maskingMode);
                    if (exception == null)
                    {
                        _tcs.TrySetResult(null);
                    }
                    else
                    {
                        _tcs.TrySetException(DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception));
                    }

                }

                public void Fault()
                {
                    var exception = _synchronizer._binder.GetFaultedException(_maskingMode);
                    _tcs.TrySetException(DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception));
                }

                public void GetChannel(bool onUserThread)
                {
                    if (!CanGetChannel)
                    {
                        throw Fx.AssertAndThrow("This waiter must wait for a channel thus the caller cannot attempt to get a channel.");
                    }

                    _getChannel = true;
                    _tcs.TrySetResult(null);
                }

                public void Set(TChannel channel)
                {
                    _channel = channel ?? throw Fx.AssertAndThrow("Argument channel cannot be null. Caller must call Fault or Close instead.");
                    _tcs.TrySetResult(null);
                }

                private async Task<bool> TryGetChannelAsync()
                {
                    TChannel channel;

                    if (_channel != null)
                    {
                        channel = _channel;
                    }
                    else if (await _synchronizer._binder.TryGetChannelAsync(
                        _timeoutHelper.RemainingTime()))
                    {
                        if (!_synchronizer.CompleteSetChannel(this, out channel))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        _synchronizer.OnGetChannelFailed();
                        return false;
                    }

                    if (_synchronizer._binder.MustOpenChannel)
                    {
                        bool throwing = true;

                        if (_channelParameters != null)
                        {
                            _channelParameters.PropagateChannelParameters(channel);
                        }

                        try
                        {
                            await channel.OpenHelperAsync(_timeoutHelper.RemainingTime());
                            throwing = false;
                        }
                        finally
                        {
                            if (throwing)
                            {
                                channel.Abort();
                                _synchronizer.OnGetChannelFailed();
                            }
                        }
                    }

                    if (_synchronizer.OnChannelOpened(this))
                    {
                        Set(channel);
                    }

                    return true;
                }

                public async Task<(bool success, TChannel channel)> TryWaitAsync()
                {
                    if (!await WaitAsync())
                    {
                        return (false, null);
                    }
                    else if (_getChannel && !await TryGetChannelAsync())
                    {
                        return (false, null);
                    }

                    if (_tcs.Task.IsFaulted)
                    {
                        if (_channel != null)
                        {
                            throw Fx.AssertAndThrow("User of IWaiter called both Set and Fault or Close.");
                        }

                        await _tcs.Task;
                    }

                    return (true, _channel);
                }

                private async Task<bool> WaitAsync()
                {
                    if (!await _tcs.Task.AwaitWithTimeout(_timeoutHelper.RemainingTime()))
                    {
                        if (_synchronizer.RemoveWaiter(this))
                        {
                            return false;
                        }
                        else
                        {
                            await _tcs.Task;
                        }
                    }

                    return true;
                }
            }
        }

        private sealed class MessageRequestContext : BinderRequestContext
        {
            public MessageRequestContext(ReliableChannelBinder<TChannel> binder, Message message)
                : base(binder, message)
            {
            }

            protected override void OnAbort()
            {
            }

            protected override void OnClose(TimeSpan timeout)
            {
            }

            protected override IAsyncResult OnBeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            {
                if (message != null)
                {
                    return Binder.SendAsync(message, timeout, MaskingMode).ToApm(callback, state);
                }
                return Task.CompletedTask.ToApm(callback, state);
            }

            protected override void OnEndReply(IAsyncResult result)
            {
                result.ToApmEnd();
            }

            protected override void OnReply(Message message, TimeSpan timeout)
            {
                if (message != null)
                {
                    Binder.SendAsync(message, timeout, MaskingMode).GetAwaiter().GetResult();
                }
            }
        }

        private sealed class RequestRequestContext : BinderRequestContext
        {
            private RequestContext _innerContext;

            public RequestRequestContext(ReliableChannelBinder<TChannel> binder,
                RequestContext innerContext, Message message)
                : base(binder, message)
            {
                if ((binder.DefaultMaskingMode != MaskingMode.All) && !binder.TolerateFaults)
                {
                    throw Fx.AssertAndThrow("This request context is designed to catch exceptions. Thus it cannot be used if the caller expects no exception handling.");
                }

                _innerContext = innerContext ?? throw Fx.AssertAndThrow("Argument innerContext cannot be null.");
            }

            protected override void OnAbort()
            {
                _innerContext.Abort();
            }

            protected override IAsyncResult OnBeginReply(Message message, TimeSpan timeout,
                AsyncCallback callback, object state)
            {
                return OnReplyAsync(message, timeout).ToApm(callback, state);
            }

            protected override void OnClose(TimeSpan timeout)
            {
                try
                {
                    _innerContext.Close(timeout);
                }
                catch (ObjectDisposedException) { }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    if (!Binder.HandleException(e, MaskingMode))
                    {
                        throw;
                    }

                    _innerContext.Abort();
                }
            }

            protected override void OnEndReply(IAsyncResult result)
            {
                result.ToApmEnd();
            }

            private async Task OnReplyAsync(Message message, TimeSpan timeout)
            {
                try
                {
                    if (message != null)
                    {
                        Binder.AddOutputHeaders(message);
                    }

                    await Task.Factory.FromAsync(_innerContext.BeginReply, _innerContext.EndReply, message, timeout, null);
                }
                catch (ObjectDisposedException) { }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    if (!Binder.HandleException(e, MaskingMode))
                    {
                        throw;
                    }

                    _innerContext.Abort();
                }
            }

            protected override void OnReply(Message message, TimeSpan timeout)
            {
                OnReplyAsync(message, timeout).GetAwaiter().GetResult();
            }
        }
    }

    internal static class ReliableChannelBinderHelper
    {
        internal static async Task CloseDuplexSessionChannelAsync(
    ReliableChannelBinder<IDuplexSessionChannel> binder, IDuplexSessionChannel channel, TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            await ((ISessionChannel<IAsyncDuplexSession>)channel).Session.CloseOutputSessionAsync(timeoutHelper.RemainingTime());
            await binder.WaitForPendingOperationsAsync(timeoutHelper.RemainingTime());

            TimeSpan iterationTimeout = timeoutHelper.RemainingTime();
            bool lastIteration = (iterationTimeout == TimeSpan.Zero);

            while (true)
            {
                Message message = null;
                bool receiveThrowing = true;

                try
                {
                    bool success;
                    if (channel is IAsyncDuplexSessionChannel)
                    {
                        (success, message) = await ((IAsyncDuplexSessionChannel)channel).TryReceiveAsync(timeout);
                    }
                    else
                    {
                        (success, message) = await TaskHelpers.FromAsync<TimeSpan, bool, Message>(channel.BeginTryReceive, channel.EndTryReceive, timeout, null);
                    }

                    receiveThrowing = false;
                    if (success && message == null)
                    {
                        await channel.CloseHelperAsync(timeoutHelper.RemainingTime());
                        return;
                    }
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    if (receiveThrowing)
                    {
                        if (!MaskHandled(binder.DefaultMaskingMode) || !binder.IsHandleable(e))
                        {
                            throw;
                        }

                        receiveThrowing = false;
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (message != null)
                    {
                        message.Close();
                    }

                    if (receiveThrowing)
                    {
                        channel.Abort();
                    }
                }

                if (lastIteration || channel.State != CommunicationState.Opened)
                {
                    break;
                }

                iterationTimeout = timeoutHelper.RemainingTime();
                lastIteration = (iterationTimeout == TimeSpan.Zero);
            }

            channel.Abort();
        }

        internal static async Task CloseReplySessionChannelAsync(
            ReliableChannelBinder<IReplySessionChannel> binder, IReplySessionChannel channel,
            TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            await binder.WaitForPendingOperationsAsync(timeoutHelper.RemainingTime());

            TimeSpan iterationTimeout = timeoutHelper.RemainingTime();
            bool lastIteration = (iterationTimeout == TimeSpan.Zero);

            while (true)
            {
                RequestContext context = null;
                bool receiveThrowing = true;

                try
                {
                    bool success;
                    (success, context) = await TaskHelpers.FromAsync<TimeSpan, bool, RequestContext>(channel.BeginTryReceiveRequest, channel.EndTryReceiveRequest, iterationTimeout, null);

                    receiveThrowing = false;
                    if (success && context == null)
                    {
                        await channel.CloseHelperAsync(timeoutHelper.RemainingTime());
                        return;
                    }
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    if (receiveThrowing)
                    {
                        if (!MaskHandled(binder.DefaultMaskingMode) || !binder.IsHandleable(e))
                        {
                            throw;
                        }

                        receiveThrowing = false;
                    }
                    else
                    {
                        throw;
                    }
                }
                finally
                {
                    if (context != null)
                    {
                        context.RequestMessage.Close();
                        context.Close();
                    }

                    if (receiveThrowing)
                    {
                        channel.Abort();
                    }
                }

                if (lastIteration || channel.State != CommunicationState.Opened)
                {
                    break;
                }

                iterationTimeout = timeoutHelper.RemainingTime();
                lastIteration = (iterationTimeout == TimeSpan.Zero);
            }

            channel.Abort();
        }

        internal static bool MaskHandled(MaskingMode maskingMode)
        {
            return (maskingMode & MaskingMode.Handled) == MaskingMode.Handled;
        }

        internal static bool MaskUnhandled(MaskingMode maskingMode)
        {
            return (maskingMode & MaskingMode.Unhandled) == MaskingMode.Unhandled;
        }
    }
}
