// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    enum TolerateFaultsMode
    {
        Never,
        IfNotSecuritySession,
        Always
    }

    [Flags]
    enum MaskingMode
    {
        None = 0x0,
        Handled = 0x1,
        Unhandled = 0x2,
        All = Handled | Unhandled
    }

    // Issue #31 in progress
//    abstract class ReliableChannelBinder<TChannel> : IReliableChannelBinder
//        where TChannel : class, IChannel
//    {
//        bool aborted = false;
//        TimeSpan defaultCloseTimeout;
//        MaskingMode defaultMaskingMode;
//        TimeSpan defaultSendTimeout;
//        AsyncCallback onCloseChannelComplete;
//        CommunicationState state = CommunicationState.Created;
//        ChannelSynchronizer synchronizer;
//        object thisLock = new object();

//        protected ReliableChannelBinder(TChannel channel, MaskingMode maskingMode,
//            TolerateFaultsMode faultMode, TimeSpan defaultCloseTimeout,
//            TimeSpan defaultSendTimeout)
//        {
//            if ((maskingMode != MaskingMode.None) && (maskingMode != MaskingMode.All))
//            {
//                throw Fx.AssertAndThrow("ReliableChannelBinder was implemented with only 2 default masking modes, None and All.");
//            }

//            defaultMaskingMode = maskingMode;
//            defaultCloseTimeout = defaultCloseTimeout;
//            defaultSendTimeout = defaultSendTimeout;

//            synchronizer = new ChannelSynchronizer(this, channel, faultMode);
//        }

//        protected abstract bool CanGetChannelForReceive
//        {
//            get;
//        }

//        public abstract bool CanSendAsynchronously
//        {
//            get;
//        }

//        public virtual ChannelParameterCollection ChannelParameters
//        {
//            get { return null; }
//        }

//        public IChannel Channel
//        {
//            get
//            {
//                return synchronizer.CurrentChannel;
//            }
//        }

//        public bool Connected
//        {
//            get
//            {
//                return synchronizer.Connected;
//            }
//        }

//        public MaskingMode DefaultMaskingMode
//        {
//            get
//            {
//                return defaultMaskingMode;
//            }
//        }

//        public TimeSpan DefaultSendTimeout
//        {
//            get
//            {
//                return defaultSendTimeout;
//            }
//        }

//        public abstract bool HasSession
//        {
//            get;
//        }

//        public abstract EndpointAddress LocalAddress
//        {
//            get;
//        }

//        protected abstract bool MustCloseChannel
//        {
//            get;
//        }

//        protected abstract bool MustOpenChannel
//        {
//            get;
//        }

//        public abstract EndpointAddress RemoteAddress
//        {
//            get;
//        }

//        public CommunicationState State
//        {
//            get
//            {
//                return state;
//            }
//        }

//        protected ChannelSynchronizer Synchronizer
//        {
//            get
//            {
//                return synchronizer;
//            }
//        }

//        protected object ThisLock
//        {
//            get
//            {
//                return thisLock;
//            }
//        }

//        bool TolerateFaults
//        {
//            get
//            {
//                return synchronizer.TolerateFaults;
//            }
//        }

//        public event EventHandler ConnectionLost;
//        public event BinderExceptionHandler Faulted;
//        public event BinderExceptionHandler OnException;


//        public void Abort()
//        {
//            TChannel channel;
//            lock (ThisLock)
//            {
//                aborted = true;

//                if (state == CommunicationState.Closed)
//                {
//                    return;
//                }

//                state = CommunicationState.Closing;
//                channel = synchronizer.StopSynchronizing(true);

//                if (!MustCloseChannel)
//                {
//                    channel = null;
//                }
//            }

//            synchronizer.UnblockWaiters();
//            OnShutdown();
//            OnAbort();

//            if (channel != null)
//            {
//                channel.Abort();
//            }

//            TransitionToClosed();
//        }

//        protected virtual void AddOutputHeaders(Message message)
//        {
//        }

//        public IAsyncResult BeginClose(TimeSpan timeout, AsyncCallback callback,
//            object state)
//        {
//            return BeginClose(timeout, defaultMaskingMode, callback, state);
//        }

//        public IAsyncResult BeginClose(TimeSpan timeout, MaskingMode maskingMode,
//            AsyncCallback callback, object state)
//        {
//            ThrowIfTimeoutNegative(timeout);
//            TChannel channel;

//            if (CloseCore(out channel))
//            {
//                return new CompletedAsyncResult(callback, state);
//            }
//            else
//            {
//                return new CloseAsyncResult(this, channel, timeout, maskingMode, callback, state);
//            }
//        }

//        protected virtual IAsyncResult BeginCloseChannel(TChannel channel, TimeSpan timeout,
//            AsyncCallback callback, object state)
//        {
//            return channel.BeginClose(timeout, callback, state);
//        }

//        public IAsyncResult BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
//        {
//            ThrowIfTimeoutNegative(timeout);

//            if (OnOpening(defaultMaskingMode))
//            {
//                try
//                {
//                    return OnBeginOpen(timeout, callback, state);
//                }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                    {
//                        throw;
//                    }

//                    Fault(null);

//                    if (defaultMaskingMode == MaskingMode.None)
//                    {
//                        throw;
//                    }
//                    else
//                    {
//                        RaiseOnException(e);
//                    }
//                }
//            }

//            return new BinderCompletedAsyncResult(callback, state);
//        }

//        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback,
//            object state)
//        {
//            return BeginSend(message, timeout, defaultMaskingMode, callback, state);
//        }

//        public IAsyncResult BeginSend(Message message, TimeSpan timeout, MaskingMode maskingMode,
//            AsyncCallback callback, object state)
//        {
//            SendAsyncResult result = new SendAsyncResult(this, callback, state);
//            result.Start(message, timeout, maskingMode);
//            return result;
//        }

//        // ChannelSynchronizer helper, cannot take a lock.
//        protected abstract IAsyncResult BeginTryGetChannel(TimeSpan timeout,
//            AsyncCallback callback, object state);

//        public virtual IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback,
//            object state)
//        {
//            return BeginTryReceive(timeout, defaultMaskingMode, callback, state);
//        }

//        public virtual IAsyncResult BeginTryReceive(TimeSpan timeout, MaskingMode maskingMode,
//            AsyncCallback callback, object state)
//        {
//            if (ValidateInputOperation(timeout))
//                return new TryReceiveAsyncResult(this, timeout, maskingMode, callback, state);
//            else
//                return new CompletedAsyncResult(callback, state);
//        }

//        internal IAsyncResult BeginWaitForPendingOperations(TimeSpan timeout,
//            AsyncCallback callback, object state)
//        {
//            return synchronizer.BeginWaitForPendingOperations(timeout, callback, state);
//        }

//        bool CloseCore(out TChannel channel)
//        {
//            channel = null;
//            bool abort = true;
//            bool abortChannel = false;

//            lock (ThisLock)
//            {
//                if ((state == CommunicationState.Closing)
//                    || (state == CommunicationState.Closed))
//                {
//                    return true;
//                }

//                if (state == CommunicationState.Opened)
//                {
//                    state = CommunicationState.Closing;
//                    channel = synchronizer.StopSynchronizing(true);
//                    abort = false;

//                    if (!MustCloseChannel)
//                    {
//                        channel = null;
//                    }

//                    if (channel != null)
//                    {
//                        CommunicationState channelState = channel.State;

//                        if ((channelState == CommunicationState.Created)
//                            || (channelState == CommunicationState.Opening)
//                            || (channelState == CommunicationState.Faulted))
//                        {
//                            abortChannel = true;
//                        }
//                        else if ((channelState == CommunicationState.Closing)
//                            || (channelState == CommunicationState.Closed))
//                        {
//                            channel = null;
//                        }
//                    }
//                }
//            }

//            synchronizer.UnblockWaiters();

//            if (abort)
//            {
//                Abort();
//                return true;
//            }
//            else
//            {
//                if (abortChannel)
//                {
//                    channel.Abort();
//                    channel = null;
//                }

//                return false;
//            }
//        }

//        public void Close(TimeSpan timeout)
//        {
//            Close(timeout, defaultMaskingMode);
//        }

//        public void Close(TimeSpan timeout, MaskingMode maskingMode)
//        {
//            ThrowIfTimeoutNegative(timeout);
//            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
//            TChannel channel;

//            if (CloseCore(out channel))
//            {
//                return;
//            }

//            try
//            {
//                OnShutdown();
//                OnClose(timeoutHelper.RemainingTime());

//                if (channel != null)
//                {
//                    CloseChannel(channel, timeoutHelper.RemainingTime());
//                }

//                TransitionToClosed();
//            }
//            catch (Exception e)
//            {
//                if (Fx.IsFatal(e))
//                {
//                    throw;
//                }

//                Abort();

//                if (!HandleException(e, maskingMode))
//                {
//                    throw;
//                }
//            }
//        }

//        // The ChannelSynchronizer calls this from an operation thread so this method must not
//        // block.
//        void CloseChannel(TChannel channel)
//        {
//            if (!MustCloseChannel)
//            {
//                throw Fx.AssertAndThrow("MustCloseChannel is false when there is no receive loop and this method is called when there is a receive loop.");
//            }

//            if (onCloseChannelComplete == null)
//            {
//                onCloseChannelComplete = Fx.ThunkCallback(new AsyncCallback(OnCloseChannelComplete));
//            }

//            try
//            {
//                IAsyncResult result = channel.BeginClose(onCloseChannelComplete, channel);

//                if (result.CompletedSynchronously)
//                {
//                    channel.EndClose(result);
//                }
//            }
//            catch (Exception e)
//            {
//                if (Fx.IsFatal(e))
//                {
//                    throw;
//                }

//                HandleException(e, MaskingMode.All);
//            }
//        }

//        protected virtual void CloseChannel(TChannel channel, TimeSpan timeout)
//        {
//            channel.Close(timeout);
//        }

//        public void EndClose(IAsyncResult result)
//        {
//            CloseAsyncResult closeResult = result as CloseAsyncResult;

//            if (closeResult != null)
//            {
//                closeResult.End();
//            }
//            else
//            {
//                CompletedAsyncResult.End(result);
//            }
//        }

//        protected virtual void EndCloseChannel(TChannel channel, IAsyncResult result)
//        {
//            channel.EndClose(result);
//        }

//        public void EndOpen(IAsyncResult result)
//        {
//            BinderCompletedAsyncResult completedResult = result as BinderCompletedAsyncResult;

//            if (completedResult != null)
//            {
//                completedResult.End();
//            }
//            else
//            {
//                try
//                {
//                    OnEndOpen(result);
//                }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                    {
//                        throw;
//                    }

//                    Fault(null);

//                    if (defaultMaskingMode == MaskingMode.None)
//                    {
//                        throw;
//                    }
//                    else
//                    {
//                        RaiseOnException(e);
//                        return;
//                    }
//                }

//                synchronizer.StartSynchronizing();
//                OnOpened();
//            }
//        }

//        public void EndSend(IAsyncResult result)
//        {
//            SendAsyncResult.End(result);
//        }

//        // ChannelSynchronizer helper, cannot take a lock.
//        protected abstract bool EndTryGetChannel(IAsyncResult result);

//        public virtual bool EndTryReceive(IAsyncResult result, out RequestContext requestContext)
//        {
//            TryReceiveAsyncResult tryReceiveResult = result as TryReceiveAsyncResult;

//            if (tryReceiveResult != null)
//            {
//                return tryReceiveResult.End(out requestContext);
//            }
//            else
//            {
//                CompletedAsyncResult.End(result);
//                requestContext = null;
//                return true;
//            }
//        }

//        public void EndWaitForPendingOperations(IAsyncResult result)
//        {
//            synchronizer.EndWaitForPendingOperations(result);
//        }

//        protected void Fault(Exception e)
//        {
//            lock (ThisLock)
//            {
//                if (state == CommunicationState.Created)
//                {
//                    throw Fx.AssertAndThrow("The binder should not detect the inner channel's faults until after the binder is opened.");
//                }

//                if ((state == CommunicationState.Faulted)
//                    || (state == CommunicationState.Closed))
//                {
//                    return;
//                }

//                state = CommunicationState.Faulted;
//                synchronizer.StopSynchronizing(false);
//            }

//            synchronizer.UnblockWaiters();

//            BinderExceptionHandler handler = Faulted;

//            if (handler != null)
//            {
//                handler(this, e);
//            }
//        }

//        // ChannelSynchronizer helper, cannot take a lock.
//        Exception GetClosedException(MaskingMode maskingMode)
//        {
//            if (ReliableChannelBinderHelper.MaskHandled(maskingMode))
//            {
//                return null;
//            }
//            else if (aborted)
//            {
//                return new CommunicationObjectAbortedException(SR.GetString(
//                    SR.CommunicationObjectAborted1, GetType().ToString()));
//            }
//            else
//            {
//                return new ObjectDisposedException(GetType().ToString());
//            }
//        }

//        // Must be called within lock (ThisLock)
//        Exception GetClosedOrFaultedException(MaskingMode maskingMode)
//        {
//            if (state == CommunicationState.Faulted)
//            {
//                return GetFaultedException(maskingMode);
//            }
//            else if ((state == CommunicationState.Closing)
//               || (state == CommunicationState.Closed))
//            {
//                return GetClosedException(maskingMode);
//            }
//            else
//            {
//                throw Fx.AssertAndThrow("Caller is attempting to get a terminal exception in a non-terminal state.");
//            }
//        }

//        // ChannelSynchronizer helper, cannot take a lock.
//        Exception GetFaultedException(MaskingMode maskingMode)
//        {
//            if (ReliableChannelBinderHelper.MaskHandled(maskingMode))
//            {
//                return null;
//            }
//            else
//            {
//                return new CommunicationObjectFaultedException(SR.GetString(
//                    SR.CommunicationObjectFaulted1, GetType().ToString()));
//            }
//        }

//        public abstract ISession GetInnerSession();

//        public void HandleException(Exception e)
//        {
//            HandleException(e, MaskingMode.All);
//        }

//        protected bool HandleException(Exception e, MaskingMode maskingMode)
//        {
//            if (TolerateFaults && (e is CommunicationObjectFaultedException))
//            {
//                return true;
//            }

//            if (IsHandleable(e))
//            {
//                return ReliableChannelBinderHelper.MaskHandled(maskingMode);
//            }

//            bool maskUnhandled = ReliableChannelBinderHelper.MaskUnhandled(maskingMode);

//            if (maskUnhandled)
//            {
//                RaiseOnException(e);
//            }

//            return maskUnhandled;
//        }

//        protected bool HandleException(Exception e, MaskingMode maskingMode, bool autoAborted)
//        {
//            if (TolerateFaults && autoAborted && e is CommunicationObjectAbortedException)
//            {
//                return true;
//            }

//            return HandleException(e, maskingMode);
//        }

//        // ChannelSynchronizer helper, cannot take a lock.
//        protected abstract bool HasSecuritySession(TChannel channel);

//        public bool IsHandleable(Exception e)
//        {
//            if (e is ProtocolException)
//            {
//                return false;
//            }

//            return (e is CommunicationException)
//                || (e is TimeoutException);
//        }

//        protected abstract void OnAbort();
//        protected abstract IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback,
//            object state);
//        protected abstract IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback,
//            object state);

//        protected virtual IAsyncResult OnBeginSend(TChannel channel, Message message,
//            TimeSpan timeout, AsyncCallback callback, object state)
//        {
//            throw Fx.AssertAndThrow("The derived class does not support the BeginSend operation.");
//        }

//        protected virtual IAsyncResult OnBeginTryReceive(TChannel channel, TimeSpan timeout,
//            AsyncCallback callback, object state)
//        {
//            throw Fx.AssertAndThrow("The derived class does not support the BeginTryReceive operation.");
//        }

//        protected abstract void OnClose(TimeSpan timeout);

//        void OnCloseChannelComplete(IAsyncResult result)
//        {
//            if (result.CompletedSynchronously)
//            {
//                return;
//            }

//            TChannel channel = (TChannel)result.AsyncState;

//            try
//            {
//                channel.EndClose(result);
//            }
//            catch (Exception e)
//            {
//                if (Fx.IsFatal(e))
//                {
//                    throw;
//                }

//                HandleException(e, MaskingMode.All);
//            }
//        }

//        protected abstract void OnEndClose(IAsyncResult result);
//        protected abstract void OnEndOpen(IAsyncResult result);

//        protected virtual void OnEndSend(TChannel channel, IAsyncResult result)
//        {
//            throw Fx.AssertAndThrow("The derived class does not support the EndSend operation.");
//        }

//        protected virtual bool OnEndTryReceive(TChannel channel, IAsyncResult result,
//            out RequestContext requestContext)
//        {
//            throw Fx.AssertAndThrow("The derived class does not support the EndTryReceive operation.");
//        }

//        void OnInnerChannelFaulted()
//        {
//            if (!TolerateFaults)
//                return;

//            EventHandler handler = ConnectionLost;

//            if (handler != null)
//                handler(this, EventArgs.Empty);
//        }

//        protected abstract void OnOpen(TimeSpan timeout);

//        void OnOpened()
//        {
//            lock (ThisLock)
//            {
//                if (state == CommunicationState.Opening)
//                {
//                    state = CommunicationState.Opened;
//                }
//            }
//        }

//        bool OnOpening(MaskingMode maskingMode)
//        {
//            lock (ThisLock)
//            {
//                if (state != CommunicationState.Created)
//                {
//                    Exception e = null;

//                    if ((state == CommunicationState.Opening)
//                        || (state == CommunicationState.Opened))
//                    {
//                        if (!ReliableChannelBinderHelper.MaskUnhandled(maskingMode))
//                        {
//                            e = new InvalidOperationException(SR.GetString(
//                                SR.CommunicationObjectCannotBeModifiedInState,
//                                GetType().ToString(), state.ToString()));
//                        }
//                    }
//                    else
//                    {
//                        e = GetClosedOrFaultedException(maskingMode);
//                    }

//                    if (e != null)
//                    {
//                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(e);
//                    }

//                    return false;
//                }
//                else
//                {
//                    state = CommunicationState.Opening;
//                    return true;
//                }
//            }
//        }

//        protected virtual void OnShutdown()
//        {
//        }

//        protected virtual void OnSend(TChannel channel, Message message, TimeSpan timeout)
//        {
//            throw Fx.AssertAndThrow("The derived class does not support the Send operation.");
//        }

//        protected virtual bool OnTryReceive(TChannel channel, TimeSpan timeout,
//            out RequestContext requestContext)
//        {
//            throw Fx.AssertAndThrow("The derived class does not support the TryReceive operation.");
//        }

//        public void Open(TimeSpan timeout)
//        {
//            ThrowIfTimeoutNegative(timeout);

//            if (!OnOpening(defaultMaskingMode))
//            {
//                return;
//            }

//            try
//            {
//                OnOpen(timeout);
//            }
//            catch (Exception e)
//            {
//                if (Fx.IsFatal(e))
//                {
//                    throw;
//                }

//                Fault(null);

//                if (defaultMaskingMode == MaskingMode.None)
//                {
//                    throw;
//                }
//                else
//                {
//                    RaiseOnException(e);
//                    return;
//                }
//            }

//            synchronizer.StartSynchronizing();
//            OnOpened();
//        }

//        void RaiseOnException(Exception e)
//        {
//            BinderExceptionHandler handler = OnException;

//            if (handler != null)
//            {
//                handler(this, e);
//            }
//        }

//        public void Send(Message message, TimeSpan timeout)
//        {
//            Send(message, timeout, defaultMaskingMode);
//        }

//        public void Send(Message message, TimeSpan timeout, MaskingMode maskingMode)
//        {
//            if (!ValidateOutputOperation(message, timeout, maskingMode))
//            {
//                return;
//            }

//            bool autoAborted = false;

//            try
//            {
//                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
//                TChannel channel;

//                if (!synchronizer.TryGetChannelForOutput(timeoutHelper.RemainingTime(), maskingMode,
//                    out channel))
//                {
//                    if (!ReliableChannelBinderHelper.MaskHandled(maskingMode))
//                    {
//                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
//                            new TimeoutException(SR.GetString(SR.TimeoutOnSend, timeout)));
//                    }

//                    return;
//                }

//                if (channel == null)
//                {
//                    return;
//                }

//                AddOutputHeaders(message);

//                try
//                {
//                    OnSend(channel, message, timeoutHelper.RemainingTime());
//                }
//                finally
//                {
//                    autoAborted = Synchronizer.Aborting;
//                    synchronizer.ReturnChannel();
//                }
//            }
//            catch (Exception e)
//            {
//                if (Fx.IsFatal(e))
//                {
//                    throw;
//                }

//                if (!HandleException(e, maskingMode, autoAborted))
//                {
//                    throw;
//                }
//            }
//        }

//        public void SetMaskingMode(RequestContext context, MaskingMode maskingMode)
//        {
//            BinderRequestContext binderContext = (BinderRequestContext)context;
//            binderContext.SetMaskingMode(maskingMode);
//        }

//        // throwDisposed indicates whether to throw in the Faulted, Closing, and Closed states.
//        // returns true if in Opened state
//        bool ThrowIfNotOpenedAndNotMasking(MaskingMode maskingMode, bool throwDisposed)
//        {
//            lock (ThisLock)
//            {
//                if (State == CommunicationState.Created)
//                {
//                    throw Fx.AssertAndThrow("Messaging operations cannot be called when the binder is in the Created state.");
//                }

//                if (State == CommunicationState.Opening)
//                {
//                    throw Fx.AssertAndThrow("Messaging operations cannot be called when the binder is in the Opening state.");
//                }

//                if (State == CommunicationState.Opened)
//                {
//                    return true;
//                }

//                // state is Faulted, Closing, or Closed
//                if (throwDisposed)
//                {
//                    Exception e = GetClosedOrFaultedException(maskingMode);

//                    if (e != null)
//                    {
//                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(e);
//                    }
//                }

//                return false;
//            }
//        }

//        void ThrowIfTimeoutNegative(TimeSpan timeout)
//        {
//            if (timeout < TimeSpan.Zero)
//            {
//                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
//                    new ArgumentOutOfRangeException("timeout", timeout, SR.SFxTimeoutOutOfRange0));
//            }
//        }

//        void TransitionToClosed()
//        {
//            lock (ThisLock)
//            {
//                if ((state != CommunicationState.Closing)
//                    && (state != CommunicationState.Closed)
//                    && (state != CommunicationState.Faulted))
//                {
//                    throw Fx.AssertAndThrow("Caller cannot transition to the Closed state from a non-terminal state.");
//                }

//                state = CommunicationState.Closed;
//            }
//        }

//        // ChannelSynchronizer helper, cannot take a lock.
//        protected abstract bool TryGetChannel(TimeSpan timeout);

//        public virtual bool TryReceive(TimeSpan timeout, out RequestContext requestContext)
//        {
//            return TryReceive(timeout, out requestContext, defaultMaskingMode);
//        }

//        public virtual bool TryReceive(TimeSpan timeout, out RequestContext requestContext, MaskingMode maskingMode)
//        {
//            if (maskingMode != MaskingMode.None)
//            {
//                throw Fx.AssertAndThrow("This method was implemented only for the case where we do not mask exceptions.");
//            }

//            if (!ValidateInputOperation(timeout))
//            {
//                requestContext = null;
//                return true;
//            }

//            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

//            while (true)
//            {
//                bool autoAborted = false;

//                try
//                {
//                    TChannel channel;
//                    bool success = !synchronizer.TryGetChannelForInput(
//                        CanGetChannelForReceive, timeoutHelper.RemainingTime(), out channel);

//                    if (channel == null)
//                    {
//                        requestContext = null;
//                        return success;
//                    }

//                    try
//                    {
//                        success = OnTryReceive(channel, timeoutHelper.RemainingTime(),
//                            out requestContext);

//                        // timed out || got message, return immediately
//                        if (!success || (requestContext != null))
//                        {
//                            return success;
//                        }

//                        // the underlying channel closed or faulted, retry
//                        synchronizer.OnReadEof();
//                    }
//                    finally
//                    {
//                        autoAborted = Synchronizer.Aborting;
//                        synchronizer.ReturnChannel();
//                    }
//                }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                    {
//                        throw;
//                    }

//                    if (!HandleException(e, maskingMode, autoAborted))
//                    {
//                        throw;
//                    }
//                }
//            }
//        }

//        protected bool ValidateInputOperation(TimeSpan timeout)
//        {
//            if (timeout < TimeSpan.Zero)
//            {
//                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("timeout", timeout,
//                    SR.SFxTimeoutOutOfRange0));
//            }

//            return ThrowIfNotOpenedAndNotMasking(MaskingMode.All, false);
//        }

//        protected bool ValidateOutputOperation(Message message, TimeSpan timeout, MaskingMode maskingMode)
//        {
//            if (message == null)
//            {
//                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
//            }

//            if (timeout < TimeSpan.Zero)
//            {
//                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("timeout", timeout,
//                    SR.SFxTimeoutOutOfRange0));
//            }

//            return ThrowIfNotOpenedAndNotMasking(maskingMode, true);
//        }

//        internal void WaitForPendingOperations(TimeSpan timeout)
//        {
//            synchronizer.WaitForPendingOperations(timeout);
//        }

//        protected RequestContext WrapMessage(Message message)
//        {
//            if (message == null)
//            {
//                return null;
//            }

//            return new MessageRequestContext(this, message);
//        }

//        public RequestContext WrapRequestContext(RequestContext context)
//        {
//            if (context == null)
//            {
//                return null;
//            }

//            if (!TolerateFaults && defaultMaskingMode == MaskingMode.None)
//            {
//                return context;
//            }

//            return new RequestRequestContext(this, context, context.RequestMessage);
//        }

//        sealed class BinderCompletedAsyncResult : CompletedAsyncResult
//        {
//            public BinderCompletedAsyncResult(AsyncCallback callback, object state)
//                : base(callback, state)
//            {
//            }

//            public void End()
//            {
//                CompletedAsyncResult.End(this);
//            }
//        }

//        abstract class BinderRequestContext : RequestContextBase
//        {
//            ReliableChannelBinder<TChannel> binder;
//            MaskingMode maskingMode;

//            public BinderRequestContext(ReliableChannelBinder<TChannel> binder, Message message)
//                : base(message, binder.defaultCloseTimeout, binder.defaultSendTimeout)
//            {
//                if (binder == null)
//                {
//                    Fx.Assert("Argument binder cannot be null.");
//                }

//                binder = binder;
//                maskingMode = binder.defaultMaskingMode;
//            }

//            protected ReliableChannelBinder<TChannel> Binder
//            {
//                get
//                {
//                    return binder;
//                }
//            }

//            protected MaskingMode MaskingMode
//            {
//                get
//                {
//                    return maskingMode;
//                }
//            }

//            public void SetMaskingMode(MaskingMode maskingMode)
//            {
//                if (binder.defaultMaskingMode != MaskingMode.All)
//                {
//                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
//                }

//                maskingMode = maskingMode;
//            }
//        }

//        protected class ChannelSynchronizer
//        {
//            bool aborting; // Indicates the current channel is being aborted, not the synchronizer.
//            ReliableChannelBinder<TChannel> binder;
//            int count = 0;
//            TChannel currentChannel;
//            InterruptibleWaitObject drainEvent;
//            static Action<object> asyncGetChannelCallback = new Action<object>(AsyncGetChannelCallback);
//            TolerateFaultsMode faultMode;
//            Queue<IWaiter> getChannelQueue;
//            bool innerChannelFaulted;
//            EventHandler onChannelFaulted;
//            State state = State.Created;
//            bool tolerateFaults = true;
//            object thisLock = new object();
//            Queue<IWaiter> waitQueue;

//            public ChannelSynchronizer(ReliableChannelBinder<TChannel> binder, TChannel channel,
//                TolerateFaultsMode faultMode)
//            {
//                binder = binder;
//                currentChannel = channel;
//                faultMode = faultMode;
//            }

//            public bool Aborting
//            {
//                get
//                {
//                    return aborting;
//                }
//            }

//            public bool Connected
//            {
//                get
//                {
//                    return (state == State.ChannelOpened ||
//                        state == State.ChannelOpening);
//                }
//            }

//            public TChannel CurrentChannel
//            {
//                get
//                {
//                    return currentChannel;
//                }
//            }

//            object ThisLock
//            {
//                get
//                {
//                    return thisLock;
//                }
//            }

//            public bool TolerateFaults
//            {
//                get
//                {
//                    return tolerateFaults;
//                }
//            }

//            // Server only API.
//            public TChannel AbortCurentChannel()
//            {
//                lock (ThisLock)
//                {
//                    if (!tolerateFaults)
//                    {
//                        throw Fx.AssertAndThrow("It is only valid to abort the current channel when masking faults");
//                    }

//                    if (state == State.ChannelOpening)
//                    {
//                        aborting = true;
//                    }
//                    else if (state == State.ChannelOpened)
//                    {
//                        if (count == 0)
//                        {
//                            state = State.NoChannel;
//                        }
//                        else
//                        {
//                            aborting = true;
//                            state = State.ChannelClosing;
//                        }
//                    }
//                    else
//                    {
//                        return null;
//                    }

//                    return currentChannel;
//                }
//            }

//            static void AsyncGetChannelCallback(object state)
//            {
//                AsyncWaiter waiter = (AsyncWaiter)state;
//                waiter.GetChannel(false);
//            }

//            public IAsyncResult BeginTryGetChannelForInput(bool canGetChannel, TimeSpan timeout,
//                AsyncCallback callback, object state)
//            {
//                return BeginTryGetChannel(canGetChannel, false, timeout, MaskingMode.All,
//                    callback, state);
//            }

//            public IAsyncResult BeginTryGetChannelForOutput(TimeSpan timeout,
//                MaskingMode maskingMode, AsyncCallback callback, object state)
//            {
//                return BeginTryGetChannel(true, true, timeout, maskingMode,
//                    callback, state);
//            }

//            IAsyncResult BeginTryGetChannel(bool canGetChannel, bool canCauseFault,
//                TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
//            {
//                TChannel channel = null;
//                AsyncWaiter waiter = null;
//                bool getChannel = false;
//                bool faulted = false;

//                lock (ThisLock)
//                {
//                    if (!ThrowIfNecessary(maskingMode))
//                    {
//                        channel = null;
//                    }
//                    else if (state == State.ChannelOpened)
//                    {
//                        if (currentChannel == null)
//                        {
//                            throw Fx.AssertAndThrow("Field currentChannel cannot be null in the ChannelOpened state.");
//                        }

//                        count++;
//                        channel = currentChannel;
//                    }
//                    else if (!tolerateFaults
//                        && ((state == State.NoChannel)
//                        || (state == State.ChannelClosing)))
//                    {
//                        if (canCauseFault)
//                        {
//                            faulted = true;
//                        }

//                        channel = null;
//                    }
//                    else if (!canGetChannel
//                        || (state == State.ChannelOpening)
//                        || (state == State.ChannelClosing))
//                    {
//                        waiter = new AsyncWaiter(this, canGetChannel, null, timeout, maskingMode,
//                            binder.ChannelParameters,
//                            callback, state);
//                        GetQueue(canGetChannel).Enqueue(waiter);
//                    }
//                    else
//                    {
//                        if (state != State.NoChannel)
//                        {
//                            throw Fx.AssertAndThrow("The state must be NoChannel.");
//                        }

//                        waiter = new AsyncWaiter(this, canGetChannel,
//                            GetCurrentChannelIfCreated(), timeout, maskingMode,
//                            binder.ChannelParameters,
//                            callback, state);

//                        state = State.ChannelOpening;
//                        getChannel = true;
//                    }
//                }

//                if (faulted)
//                {
//                    binder.Fault(null);
//                }

//                if (waiter == null)
//                {
//                    return new CompletedAsyncResult<TChannel>(channel, callback, state);
//                }

//                if (getChannel)
//                {
//                    waiter.GetChannel(true);
//                }
//                else
//                {
//                    waiter.Wait();
//                }

//                return waiter;
//            }

//            public IAsyncResult BeginWaitForPendingOperations(TimeSpan timeout,
//                AsyncCallback callback, object state)
//            {
//                lock (ThisLock)
//                {
//                    if (drainEvent != null)
//                    {
//                        throw Fx.AssertAndThrow("The WaitForPendingOperations operation may only be invoked once.");
//                    }

//                    if (count > 0)
//                    {
//                        drainEvent = new InterruptibleWaitObject(false, false);
//                    }
//                }

//                if (drainEvent != null)
//                {
//                    return drainEvent.BeginWait(timeout, callback, state);
//                }
//                else
//                {
//                    return new SynchronizerCompletedAsyncResult(callback, state);
//                }
//            }

//            bool CompleteSetChannel(IWaiter waiter, out TChannel channel)
//            {
//                if (waiter == null)
//                {
//                    throw Fx.AssertAndThrow("Argument waiter cannot be null.");
//                }

//                bool close = false;

//                lock (ThisLock)
//                {
//                    if (ValidateOpened())
//                    {
//                        channel = currentChannel;
//                        return true;
//                    }
//                    else
//                    {
//                        channel = null;
//                        close = state == State.Closed;
//                    }
//                }

//                if (close)
//                {
//                    waiter.Close();
//                }
//                else
//                {
//                    waiter.Fault();
//                }

//                return false;
//            }

//            public bool EndTryGetChannel(IAsyncResult result, out TChannel channel)
//            {
//                AsyncWaiter waiter = result as AsyncWaiter;

//                if (waiter != null)
//                {
//                    return waiter.End(out channel);
//                }
//                else
//                {
//                    channel = CompletedAsyncResult<TChannel>.End(result);
//                    return true;
//                }
//            }

//            public void EndWaitForPendingOperations(IAsyncResult result)
//            {
//                SynchronizerCompletedAsyncResult completedResult =
//                    result as SynchronizerCompletedAsyncResult;

//                if (completedResult != null)
//                {
//                    completedResult.End();
//                }
//                else
//                {
//                    drainEvent.EndWait(result);
//                }
//            }

//            // Client API only.
//            public bool EnsureChannel()
//            {
//                bool fault = false;

//                lock (ThisLock)
//                {
//                    if (ValidateOpened())
//                    {
//                        // This is called only during the RM CS phase. In this phase, there are 2
//                        // valid states between Request calls, ChannelOpened and NoChannel.
//                        if (state == State.ChannelOpened)
//                        {
//                            return true;
//                        }

//                        if (state != State.NoChannel)
//                        {
//                            throw Fx.AssertAndThrow("The caller may only invoke this EnsureChannel during the CreateSequence negotiation. ChannelOpening and ChannelClosing are invalid states during this phase of the negotiation.");
//                        }

//                        if (!tolerateFaults)
//                        {
//                            fault = true;
//                        }
//                        else
//                        {
//                            if (GetCurrentChannelIfCreated() != null)
//                            {
//                                return true;
//                            }

//                            if (binder.TryGetChannel(TimeSpan.Zero))
//                            {
//                                if (currentChannel == null)
//                                {
//                                    return false;
//                                }

//                                return true;
//                            }
//                        }
//                    }
//                }

//                if (fault)
//                {
//                    binder.Fault(null);
//                }

//                return false;
//            }

//            IWaiter GetChannelWaiter()
//            {
//                if ((getChannelQueue == null) || (getChannelQueue.Count == 0))
//                {
//                    return null;
//                }

//                return getChannelQueue.Dequeue();
//            }

//            // Must be called within lock (ThisLock)
//            TChannel GetCurrentChannelIfCreated()
//            {
//                if (state != State.NoChannel)
//                {
//                    throw Fx.AssertAndThrow("This method may only be called in the NoChannel state.");
//                }

//                if ((currentChannel != null)
//                    && (currentChannel.State == CommunicationState.Created))
//                {
//                    return currentChannel;
//                }
//                else
//                {
//                    return null;
//                }
//            }

//            Queue<IWaiter> GetQueue(bool canGetChannel)
//            {
//                if (canGetChannel)
//                {
//                    if (getChannelQueue == null)
//                    {
//                        getChannelQueue = new Queue<IWaiter>();
//                    }

//                    return getChannelQueue;
//                }
//                else
//                {
//                    if (waitQueue == null)
//                    {
//                        waitQueue = new Queue<IWaiter>();
//                    }

//                    return waitQueue;
//                }
//            }

//            void OnChannelFaulted(object sender, EventArgs e)
//            {
//                TChannel faultedChannel = (TChannel)sender;
//                bool faultBinder = false;
//                bool raiseInnerChannelFaulted = false;

//                lock (ThisLock)
//                {
//                    if (currentChannel != faultedChannel)
//                    {
//                        return;
//                    }

//                    // The synchronizer is already closed or aborted.
//                    if (!ValidateOpened())
//                    {
//                        return;
//                    }

//                    if (state == State.ChannelOpened)
//                    {
//                        if (count == 0)
//                        {
//                            faultedChannel.Faulted -= onChannelFaulted;
//                        }

//                        faultBinder = !tolerateFaults;
//                        state = State.ChannelClosing;
//                        innerChannelFaulted = true;

//                        if (!faultBinder && count == 0)
//                        {
//                            state = State.NoChannel;
//                            aborting = false;
//                            raiseInnerChannelFaulted = true;
//                            innerChannelFaulted = false;
//                        }
//                    }
//                }

//                if (faultBinder)
//                {
//                    binder.Fault(null);
//                }

//                faultedChannel.Abort();

//                if (raiseInnerChannelFaulted)
//                {
//                    binder.OnInnerChannelFaulted();
//                }
//            }

//            bool OnChannelOpened(IWaiter waiter)
//            {
//                if (waiter == null)
//                {
//                    throw Fx.AssertAndThrow("Argument waiter cannot be null.");
//                }

//                bool close = false;
//                bool fault = false;

//                Queue<IWaiter> temp1 = null;
//                Queue<IWaiter> temp2 = null;
//                TChannel channel = null;

//                lock (ThisLock)
//                {
//                    if (currentChannel == null)
//                    {
//                        throw Fx.AssertAndThrow("Caller must ensure that field currentChannel is set before opening the channel.");
//                    }

//                    if (ValidateOpened())
//                    {
//                        if (state != State.ChannelOpening)
//                        {
//                            throw Fx.AssertAndThrow("This method may only be called in the ChannelOpening state.");
//                        }

//                        state = State.ChannelOpened;
//                        SetTolerateFaults();

//                        count += 1;
//                        count += (getChannelQueue == null) ? 0 : getChannelQueue.Count;
//                        count += (waitQueue == null) ? 0 : waitQueue.Count;

//                        temp1 = getChannelQueue;
//                        temp2 = waitQueue;
//                        channel = currentChannel;

//                        getChannelQueue = null;
//                        waitQueue = null;
//                    }
//                    else
//                    {
//                        close = state == State.Closed;
//                        fault = state == State.Faulted;
//                    }
//                }

//                if (close)
//                {
//                    waiter.Close();
//                    return false;
//                }
//                else if (fault)
//                {
//                    waiter.Fault();
//                    return false;
//                }

//                SetWaiters(temp1, channel);
//                SetWaiters(temp2, channel);
//                return true;
//            }

//            void OnGetChannelFailed()
//            {
//                IWaiter waiter = null;

//                lock (ThisLock)
//                {
//                    if (!ValidateOpened())
//                    {
//                        return;
//                    }

//                    if (state != State.ChannelOpening)
//                    {
//                        throw Fx.AssertAndThrow("The state must be set to ChannelOpening before the caller attempts to open the channel.");
//                    }

//                    waiter = GetChannelWaiter();

//                    if (waiter == null)
//                    {
//                        state = State.NoChannel;
//                        return;
//                    }
//                }

//                if (waiter is SyncWaiter)
//                {
//                    waiter.GetChannel(false);
//                }
//                else
//                {
//                    ActionItem.Schedule(asyncGetChannelCallback, waiter);
//                }
//            }

//            public void OnReadEof()
//            {
//                lock (ThisLock)
//                {
//                    if (count <= 0)
//                    {
//                        throw Fx.AssertAndThrow("Caller must ensure that OnReadEof is called before ReturnChannel.");
//                    }

//                    if (ValidateOpened())
//                    {
//                        if ((state != State.ChannelOpened) && (state != State.ChannelClosing))
//                        {
//                            throw Fx.AssertAndThrow("Since count is positive, the only valid states are ChannelOpened and ChannelClosing.");
//                        }

//                        if (currentChannel.State != CommunicationState.Faulted)
//                        {
//                            state = State.ChannelClosing;
//                        }
//                    }
//                }
//            }

//            bool RemoveWaiter(IWaiter waiter)
//            {
//                Queue<IWaiter> waiters = waiter.CanGetChannel ? getChannelQueue : waitQueue;
//                bool removed = false;

//                lock (ThisLock)
//                {
//                    if (!ValidateOpened())
//                    {
//                        return false;
//                    }

//                    for (int i = waiters.Count; i > 0; i--)
//                    {
//                        IWaiter temp = waiters.Dequeue();

//                        if (object.ReferenceEquals(waiter, temp))
//                        {
//                            removed = true;
//                        }
//                        else
//                        {
//                            waiters.Enqueue(temp);
//                        }
//                    }
//                }

//                return removed;
//            }

//            public void ReturnChannel()
//            {
//                TChannel channel = null;
//                IWaiter waiter = null;
//                bool faultBinder = false;
//                bool drained;
//                bool raiseInnerChannelFaulted = false;

//                lock (ThisLock)
//                {
//                    if (count <= 0)
//                    {
//                        throw Fx.AssertAndThrow("Method ReturnChannel() can only be called after TryGetChannel or EndTryGetChannel returns a channel.");
//                    }

//                    count--;
//                    drained = (count == 0) && (drainEvent != null);

//                    if (ValidateOpened())
//                    {
//                        if ((state != State.ChannelOpened) && (state != State.ChannelClosing))
//                        {
//                            throw Fx.AssertAndThrow("ChannelOpened and ChannelClosing are the only 2 valid states when count is positive.");
//                        }

//                        if (currentChannel.State == CommunicationState.Faulted)
//                        {
//                            faultBinder = !tolerateFaults;
//                            innerChannelFaulted = true;
//                            state = State.ChannelClosing;
//                        }

//                        if (!faultBinder && (state == State.ChannelClosing) && (count == 0))
//                        {
//                            channel = currentChannel;
//                            raiseInnerChannelFaulted = innerChannelFaulted;
//                            innerChannelFaulted = false;

//                            state = State.NoChannel;
//                            aborting = false;

//                            waiter = GetChannelWaiter();

//                            if (waiter != null)
//                            {
//                                state = State.ChannelOpening;
//                            }
//                        }
//                    }
//                }

//                if (faultBinder)
//                {
//                    binder.Fault(null);
//                }

//                if (drained)
//                {
//                    drainEvent.Set();
//                }

//                if (channel != null)
//                {
//                    channel.Faulted -= onChannelFaulted;

//                    if (channel.State == CommunicationState.Opened)
//                    {
//                        binder.CloseChannel(channel);
//                    }
//                    else
//                    {
//                        channel.Abort();
//                    }

//                    if (waiter != null)
//                    {
//                        waiter.GetChannel(false);
//                    }
//                }

//                if (raiseInnerChannelFaulted)
//                {
//                    binder.OnInnerChannelFaulted();
//                }
//            }

//            public bool SetChannel(TChannel channel)
//            {
//                lock (ThisLock)
//                {
//                    if (state != State.ChannelOpening && state != State.NoChannel)
//                    {
//                        throw Fx.AssertAndThrow("SetChannel is only valid in the NoChannel and ChannelOpening states");
//                    }

//                    if (!tolerateFaults)
//                    {
//                        throw Fx.AssertAndThrow("SetChannel is only valid when masking faults");
//                    }

//                    if (ValidateOpened())
//                    {
//                        currentChannel = channel;
//                        return true;
//                    }
//                    else
//                    {
//                        return false;
//                    }
//                }
//            }

//            void SetTolerateFaults()
//            {
//                if (faultMode == TolerateFaultsMode.Never)
//                {
//                    tolerateFaults = false;
//                }
//                else if (faultMode == TolerateFaultsMode.IfNotSecuritySession)
//                {
//                    tolerateFaults = !binder.HasSecuritySession(currentChannel);
//                }

//                if (onChannelFaulted == null)
//                {
//                    onChannelFaulted = new EventHandler(OnChannelFaulted);
//                }

//                currentChannel.Faulted += onChannelFaulted;
//            }

//            void SetWaiters(Queue<IWaiter> waiters, TChannel channel)
//            {
//                if ((waiters != null) && (waiters.Count > 0))
//                {
//                    foreach (IWaiter waiter in waiters)
//                    {
//                        waiter.Set(channel);
//                    }
//                }
//            }

//            public void StartSynchronizing()
//            {
//                lock (ThisLock)
//                {
//                    if (state == State.Created)
//                    {
//                        state = State.NoChannel;
//                    }
//                    else
//                    {
//                        if (state != State.Closed)
//                        {
//                            throw Fx.AssertAndThrow("Abort is the only operation that can race with Open.");
//                        }

//                        return;
//                    }

//                    if (currentChannel == null)
//                    {
//                        if (!binder.TryGetChannel(TimeSpan.Zero))
//                        {
//                            return;
//                        }
//                    }

//                    if (currentChannel == null)
//                    {
//                        return;
//                    }

//                    if (!binder.MustOpenChannel)
//                    {
//                        // Channel is already opened.
//                        state = State.ChannelOpened;
//                        SetTolerateFaults();
//                    }
//                }
//            }

//            public TChannel StopSynchronizing(bool close)
//            {
//                lock (ThisLock)
//                {
//                    if ((state != State.Faulted) && (state != State.Closed))
//                    {
//                        state = close ? State.Closed : State.Faulted;

//                        if ((currentChannel != null) && (onChannelFaulted != null))
//                        {
//                            currentChannel.Faulted -= onChannelFaulted;
//                        }
//                    }

//                    return currentChannel;
//                }
//            }

//            // Must be called under a lock.
//            bool ThrowIfNecessary(MaskingMode maskingMode)
//            {
//                if (ValidateOpened())
//                {
//                    return true;
//                }

//                // state is Closed or Faulted.
//                Exception e;

//                if (state == State.Closed)
//                {
//                    e = binder.GetClosedException(maskingMode);
//                }
//                else
//                {
//                    e = binder.GetFaultedException(maskingMode);
//                }

//                if (e != null)
//                {
//                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(e);
//                }

//                return false;
//            }

//            public bool TryGetChannelForInput(bool canGetChannel, TimeSpan timeout,
//                out TChannel channel)
//            {
//                return TryGetChannel(canGetChannel, false, timeout, MaskingMode.All,
//                    out channel);
//            }

//            public bool TryGetChannelForOutput(TimeSpan timeout, MaskingMode maskingMode,
//                out TChannel channel)
//            {
//                return TryGetChannel(true, true, timeout, maskingMode, out channel);
//            }

//            bool TryGetChannel(bool canGetChannel, bool canCauseFault, TimeSpan timeout,
//                MaskingMode maskingMode, out TChannel channel)
//            {
//                SyncWaiter waiter = null;
//                bool faulted = false;
//                bool getChannel = false;

//                lock (ThisLock)
//                {
//                    if (!ThrowIfNecessary(maskingMode))
//                    {
//                        channel = null;
//                        return true;
//                    }

//                    if (state == State.ChannelOpened)
//                    {
//                        if (currentChannel == null)
//                        {
//                            throw Fx.AssertAndThrow("Field currentChannel cannot be null in the ChannelOpened state.");
//                        }

//                        count++;
//                        channel = currentChannel;
//                        return true;
//                    }

//                    if (!tolerateFaults
//                        && ((state == State.ChannelClosing)
//                        || (state == State.NoChannel)))
//                    {
//                        if (!canCauseFault)
//                        {
//                            channel = null;
//                            return true;
//                        }

//                        faulted = true;
//                    }
//                    else if (!canGetChannel
//                        || (state == State.ChannelOpening)
//                        || (state == State.ChannelClosing))
//                    {
//                        waiter = new SyncWaiter(this, canGetChannel, null, timeout, maskingMode, binder.ChannelParameters);
//                        GetQueue(canGetChannel).Enqueue(waiter);
//                    }
//                    else
//                    {
//                        if (state != State.NoChannel)
//                        {
//                            throw Fx.AssertAndThrow("The state must be NoChannel.");
//                        }

//                        waiter = new SyncWaiter(this, canGetChannel,
//                            GetCurrentChannelIfCreated(), timeout, maskingMode,
//                            binder.ChannelParameters);

//                        state = State.ChannelOpening;
//                        getChannel = true;
//                    }
//                }

//                if (faulted)
//                {
//                    binder.Fault(null);
//                    channel = null;
//                    return true;
//                }

//                if (getChannel)
//                {
//                    waiter.GetChannel(true);
//                }

//                return waiter.TryWait(out channel);
//            }

//            public void UnblockWaiters()
//            {
//                Queue<IWaiter> temp1;
//                Queue<IWaiter> temp2;

//                lock (ThisLock)
//                {
//                    temp1 = getChannelQueue;
//                    temp2 = waitQueue;

//                    getChannelQueue = null;
//                    waitQueue = null;
//                }

//                bool close = state == State.Closed;
//                UnblockWaiters(temp1, close);
//                UnblockWaiters(temp2, close);
//            }

//            void UnblockWaiters(Queue<IWaiter> waiters, bool close)
//            {
//                if ((waiters != null) && (waiters.Count > 0))
//                {
//                    foreach (IWaiter waiter in waiters)
//                    {
//                        if (close)
//                        {
//                            waiter.Close();
//                        }
//                        else
//                        {
//                            waiter.Fault();
//                        }
//                    }
//                }
//            }

//            bool ValidateOpened()
//            {
//                if (state == State.Created)
//                {
//                    throw Fx.AssertAndThrow("This operation expects that the synchronizer has been opened.");
//                }

//                return (state != State.Closed) && (state != State.Faulted);
//            }

//            public void WaitForPendingOperations(TimeSpan timeout)
//            {
//                lock (ThisLock)
//                {
//                    if (drainEvent != null)
//                    {
//                        throw Fx.AssertAndThrow("The WaitForPendingOperations operation may only be invoked once.");
//                    }

//                    if (count > 0)
//                    {
//                        drainEvent = new InterruptibleWaitObject(false, false);
//                    }
//                }

//                if (drainEvent != null)
//                {
//                    drainEvent.Wait(timeout);
//                }
//            }

//            enum State
//            {
//                Created,
//                NoChannel,
//                ChannelOpening,
//                ChannelOpened,
//                ChannelClosing,
//                Faulted,
//                Closed
//            }

//            public interface IWaiter
//            {
//                bool CanGetChannel { get; }

//                void Close();
//                void Fault();
//                void GetChannel(bool onUserThread);
//                void Set(TChannel channel);
//            }

//            public sealed class AsyncWaiter : AsyncResult, IWaiter
//            {
//                bool canGetChannel;
//                TChannel channel;
//                ChannelParameterCollection channelParameters;
//                bool isSynchronous = true;
//                MaskingMode maskingMode;
//                static AsyncCallback onOpenComplete = Fx.ThunkCallback(new AsyncCallback(OnOpenComplete));
//                static Action<object> onTimeoutElapsed = new Action<object>(OnTimeoutElapsed);
//                static AsyncCallback onTryGetChannelComplete = Fx.ThunkCallback(new AsyncCallback(OnTryGetChannelComplete));
//                bool timedOut = false;
//                ChannelSynchronizer synchronizer;
//                TimeoutHelper timeoutHelper;
//                IOThreadTimer timer;
//                bool timerCancelled = false;

//                public AsyncWaiter(ChannelSynchronizer synchronizer, bool canGetChannel,
//                    TChannel channel, TimeSpan timeout, MaskingMode maskingMode,
//                    ChannelParameterCollection channelParameters,
//                    AsyncCallback callback, object state)
//                    : base(callback, state)
//                {
//                    if (!canGetChannel)
//                    {
//                        if (channel != null)
//                        {
//                            throw Fx.AssertAndThrow("This waiter must wait for a channel thus argument channel must be null.");
//                        }
//                    }

//                    synchronizer = synchronizer;
//                    canGetChannel = canGetChannel;
//                    channel = channel;
//                    timeoutHelper = new TimeoutHelper(timeout);
//                    maskingMode = maskingMode;
//                    channelParameters = channelParameters;
//                }

//                public bool CanGetChannel
//                {
//                    get
//                    {
//                        return canGetChannel;
//                    }
//                }

//                object ThisLock
//                {
//                    get
//                    {
//                        return this;
//                    }
//                }

//                void CancelTimer()
//                {
//                    lock (ThisLock)
//                    {
//                        if (!timerCancelled)
//                        {
//                            if (timer != null)
//                            {
//                                timer.Cancel();
//                            }

//                            timerCancelled = true;
//                        }
//                    }
//                }

//                public void Close()
//                {
//                    CancelTimer();
//                    channel = null;
//                    Complete(false,
//                        synchronizer.binder.GetClosedException(maskingMode));
//                }

//                bool CompleteOpen(IAsyncResult result)
//                {
//                    channel.EndOpen(result);
//                    return OnChannelOpened();
//                }

//                bool CompleteTryGetChannel(IAsyncResult result)
//                {
//                    if (!synchronizer.binder.EndTryGetChannel(result))
//                    {
//                        timedOut = true;
//                        OnGetChannelFailed();
//                        return true;
//                    }

//                    if (!synchronizer.CompleteSetChannel(this, out channel))
//                    {
//                        if (!IsCompleted)
//                        {
//                            throw Fx.AssertAndThrow("CompleteSetChannel must complete the IWaiter if it returns false.");
//                        }

//                        return false;
//                    }

//                    return OpenChannel();
//                }

//                public bool End(out TChannel channel)
//                {
//                    AsyncResult.End<AsyncWaiter>(this);
//                    channel = channel;
//                    return !timedOut;
//                }

//                public void Fault()
//                {
//                    CancelTimer();
//                    channel = null;
//                    Complete(false,
//                        synchronizer.binder.GetFaultedException(maskingMode));
//                }

//                bool GetChannel()
//                {
//                    if (channel != null)
//                    {
//                        return OpenChannel();
//                    }
//                    else
//                    {
//                        IAsyncResult result = synchronizer.binder.BeginTryGetChannel(
//                            timeoutHelper.RemainingTime(), onTryGetChannelComplete, this);

//                        if (result.CompletedSynchronously)
//                        {
//                            return CompleteTryGetChannel(result);
//                        }
//                    }

//                    return false;
//                }

//                public void GetChannel(bool onUserThread)
//                {
//                    if (!CanGetChannel)
//                    {
//                        throw Fx.AssertAndThrow("This waiter must wait for a channel thus the caller cannot attempt to get a channel.");
//                    }

//                    isSynchronous = onUserThread;

//                    if (onUserThread)
//                    {
//                        bool throwing = true;

//                        try
//                        {
//                            if (GetChannel())
//                            {
//                                Complete(true);
//                            }

//                            throwing = false;
//                        }
//                        finally
//                        {
//                            if (throwing)
//                            {
//                                OnGetChannelFailed();
//                            }
//                        }
//                    }
//                    else
//                    {
//                        bool complete = false;
//                        Exception completeException = null;

//                        try
//                        {
//                            CancelTimer();
//                            complete = GetChannel();
//                        }
//                        catch (Exception e)
//                        {
//                            if (Fx.IsFatal(e))
//                            {
//                                throw;
//                            }

//                            OnGetChannelFailed();
//                            completeException = e;
//                        }

//                        if (complete || completeException != null)
//                        {
//                            Complete(false, completeException);
//                        }
//                    }
//                }

//                bool OnChannelOpened()
//                {
//                    if (synchronizer.OnChannelOpened(this))
//                    {
//                        return true;
//                    }
//                    else
//                    {
//                        if (!IsCompleted)
//                        {
//                            throw Fx.AssertAndThrow("OnChannelOpened must complete the IWaiter if it returns false.");
//                        }

//                        return false;
//                    }
//                }

//                void OnGetChannelFailed()
//                {
//                    if (channel != null)
//                    {
//                        channel.Abort();
//                    }

//                    synchronizer.OnGetChannelFailed();
//                }

//                static void OnOpenComplete(IAsyncResult result)
//                {
//                    if (!result.CompletedSynchronously)
//                    {
//                        AsyncWaiter waiter = (AsyncWaiter)result.AsyncState;
//                        bool complete = false;
//                        Exception completeException = null;

//                        waiter.isSynchronous = false;

//                        try
//                        {
//                            complete = waiter.CompleteOpen(result);
//                        }
//                        catch (Exception e)
//                        {
//                            if (Fx.IsFatal(e))
//                            {
//                                throw;
//                            }

//                            completeException = e;
//                        }

//                        if (complete)
//                        {
//                            waiter.Complete(false);
//                        }
//                        else if (completeException != null)
//                        {
//                            waiter.OnGetChannelFailed();
//                            waiter.Complete(false, completeException);
//                        }
//                    }
//                }

//                void OnTimeoutElapsed()
//                {
//                    if (synchronizer.RemoveWaiter(this))
//                    {
//                        timedOut = true;
//                        Complete(isSynchronous, null);
//                    }
//                }

//                static void OnTimeoutElapsed(object state)
//                {
//                    AsyncWaiter waiter = (AsyncWaiter)state;
//                    waiter.isSynchronous = false;
//                    waiter.OnTimeoutElapsed();
//                }

//                static void OnTryGetChannelComplete(IAsyncResult result)
//                {
//                    if (!result.CompletedSynchronously)
//                    {
//                        AsyncWaiter waiter = (AsyncWaiter)result.AsyncState;
//                        waiter.isSynchronous = false;
//                        bool complete = false;
//                        Exception completeException = null;

//                        try
//                        {
//                            complete = waiter.CompleteTryGetChannel(result);
//                        }
//                        catch (Exception e)
//                        {
//                            if (Fx.IsFatal(e))
//                            {
//                                throw;
//                            }

//                            completeException = e;
//                        }

//                        if (complete || completeException != null)
//                        {
//                            if (completeException != null)
//                                waiter.OnGetChannelFailed();
//                            waiter.Complete(waiter.isSynchronous, completeException);
//                        }
//                    }
//                }

//                bool OpenChannel()
//                {
//                    if (synchronizer.binder.MustOpenChannel)
//                    {
//                        if (channelParameters != null)
//                        {
//                            channelParameters.PropagateChannelParameters(channel);
//                        }

//                        IAsyncResult result = channel.BeginOpen(
//                            timeoutHelper.RemainingTime(), onOpenComplete, this);

//                        if (result.CompletedSynchronously)
//                        {
//                            return CompleteOpen(result);
//                        }

//                        return false;
//                    }
//                    else
//                    {
//                        return OnChannelOpened();
//                    }
//                }

//                public void Set(TChannel channel)
//                {
//                    CancelTimer();
//                    channel = channel;
//                    Complete(false);
//                }

//                // Always called from the user's thread.
//                public void Wait()
//                {
//                    lock (ThisLock)
//                    {
//                        if (timerCancelled)
//                        {
//                            return;
//                        }

//                        TimeSpan timeout = timeoutHelper.RemainingTime();

//                        if (timeout > TimeSpan.Zero)
//                        {
//                            timer = new IOThreadTimer(onTimeoutElapsed, this, true);
//                            timer.Set(timeoutHelper.RemainingTime());
//                            return;
//                        }
//                    }

//                    OnTimeoutElapsed();
//                }
//            }

//            sealed class SynchronizerCompletedAsyncResult : CompletedAsyncResult
//            {
//                public SynchronizerCompletedAsyncResult(AsyncCallback callback, object state)
//                    : base(callback, state)
//                {
//                }

//                public void End()
//                {
//                    CompletedAsyncResult.End(this);
//                }
//            }

//            sealed class SyncWaiter : IWaiter
//            {
//                bool canGetChannel;
//                TChannel channel;
//                ChannelParameterCollection channelParameters;
//                AutoResetEvent completeEvent = new AutoResetEvent(false);
//                Exception exception;
//                bool getChannel = false;
//                MaskingMode maskingMode;
//                ChannelSynchronizer synchronizer;
//                TimeoutHelper timeoutHelper;

//                public SyncWaiter(ChannelSynchronizer synchronizer, bool canGetChannel,
//                    TChannel channel, TimeSpan timeout, MaskingMode maskingMode,
//                    ChannelParameterCollection channelParameters)
//                {
//                    if (!canGetChannel)
//                    {
//                        if (channel != null)
//                        {
//                            throw Fx.AssertAndThrow("This waiter must wait for a channel thus argument channel must be null.");
//                        }
//                    }

//                    synchronizer = synchronizer;
//                    canGetChannel = canGetChannel;
//                    channel = channel;
//                    timeoutHelper = new TimeoutHelper(timeout);
//                    maskingMode = maskingMode;
//                    channelParameters = channelParameters;
//                }

//                public bool CanGetChannel
//                {
//                    get
//                    {
//                        return canGetChannel;
//                    }
//                }

//                public void Close()
//                {
//                    exception = synchronizer.binder.GetClosedException(maskingMode);
//                    completeEvent.Set();
//                }

//                public void Fault()
//                {
//                    exception = synchronizer.binder.GetFaultedException(maskingMode);
//                    completeEvent.Set();
//                }

//                public void GetChannel(bool onUserThread)
//                {
//                    if (!CanGetChannel)
//                    {
//                        throw Fx.AssertAndThrow("This waiter must wait for a channel thus the caller cannot attempt to get a channel.");
//                    }

//                    getChannel = true;
//                    completeEvent.Set();
//                }

//                public void Set(TChannel channel)
//                {
//                    if (channel == null)
//                    {
//                        throw Fx.AssertAndThrow("Argument channel cannot be null. Caller must call Fault or Close instead.");
//                    }

//                    channel = channel;
//                    completeEvent.Set();
//                }

//                bool TryGetChannel()
//                {
//                    TChannel channel;

//                    if (channel != null)
//                    {
//                        channel = channel;
//                    }
//                    else if (synchronizer.binder.TryGetChannel(
//                        timeoutHelper.RemainingTime()))
//                    {
//                        if (!synchronizer.CompleteSetChannel(this, out channel))
//                        {
//                            return true;
//                        }
//                    }
//                    else
//                    {
//                        synchronizer.OnGetChannelFailed();
//                        return false;
//                    }

//                    if (synchronizer.binder.MustOpenChannel)
//                    {
//                        bool throwing = true;

//                        if (channelParameters != null)
//                        {
//                            channelParameters.PropagateChannelParameters(channel);
//                        }

//                        try
//                        {
//                            channel.Open(timeoutHelper.RemainingTime());
//                            throwing = false;
//                        }
//                        finally
//                        {
//                            if (throwing)
//                            {
//                                channel.Abort();
//                                synchronizer.OnGetChannelFailed();
//                            }
//                        }
//                    }

//                    if (synchronizer.OnChannelOpened(this))
//                    {
//                        Set(channel);
//                    }

//                    return true;
//                }

//                public bool TryWait(out TChannel channel)
//                {
//                    if (!Wait())
//                    {
//                        channel = null;
//                        return false;
//                    }
//                    else if (getChannel && !TryGetChannel())
//                    {
//                        channel = null;
//                        return false;
//                    }

//                    completeEvent.Close();

//                    if (exception != null)
//                    {
//                        if (channel != null)
//                        {
//                            throw Fx.AssertAndThrow("User of IWaiter called both Set and Fault or Close.");
//                        }

//                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
//                    }

//                    channel = channel;
//                    return true;
//                }

//                bool Wait()
//                {
//                    if (!TimeoutHelper.WaitOne(completeEvent, timeoutHelper.RemainingTime()))
//                    {
//                        if (synchronizer.RemoveWaiter(this))
//                        {
//                            return false;
//                        }
//                        else
//                        {
//                            TimeoutHelper.WaitOne(completeEvent, TimeSpan.MaxValue);
//                        }
//                    }

//                    return true;
//                }
//            }
//        }

//        sealed class CloseAsyncResult : AsyncResult
//        {
//            ReliableChannelBinder<TChannel> binder;
//            TChannel channel;
//            MaskingMode maskingMode;
//            static AsyncCallback onBinderCloseComplete = Fx.ThunkCallback(new AsyncCallback(OnBinderCloseComplete));
//            static AsyncCallback onChannelCloseComplete = Fx.ThunkCallback(new AsyncCallback(OnChannelCloseComplete));
//            TimeoutHelper timeoutHelper;

//            public CloseAsyncResult(ReliableChannelBinder<TChannel> binder, TChannel channel,
//                TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
//                : base(callback, state)
//            {
//                binder = binder;
//                channel = channel;
//                timeoutHelper = new TimeoutHelper(timeout);
//                maskingMode = maskingMode;
//                bool complete = false;

//                try
//                {
//                    binder.OnShutdown();
//                    IAsyncResult result = binder.OnBeginClose(timeout, onBinderCloseComplete, this);

//                    if (result.CompletedSynchronously)
//                    {
//                        complete = CompleteBinderClose(true, result);
//                    }
//                }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                    {
//                        throw;
//                    }

//                    binder.Abort();

//                    if (!binder.HandleException(e, maskingMode))
//                    {
//                        throw;
//                    }
//                    else
//                    {
//                        complete = true;
//                    }
//                }

//                if (complete)
//                {
//                    Complete(true);
//                }
//            }

//            bool CompleteBinderClose(bool synchronous, IAsyncResult result)
//            {
//                binder.OnEndClose(result);

//                if (channel != null)
//                {
//                    result = binder.BeginCloseChannel(channel,
//                        timeoutHelper.RemainingTime(), onChannelCloseComplete, this);

//                    if (result.CompletedSynchronously)
//                    {
//                        return CompleteChannelClose(synchronous, result);
//                    }
//                    else
//                    {
//                        return false;
//                    }
//                }
//                else
//                {
//                    binder.TransitionToClosed();
//                    return true;
//                }
//            }

//            bool CompleteChannelClose(bool synchronous, IAsyncResult result)
//            {
//                binder.EndCloseChannel(channel, result);
//                binder.TransitionToClosed();
//                return true;
//            }

//            public void End()
//            {
//                AsyncResult.End<CloseAsyncResult>(this);
//            }

//            Exception HandleAsyncException(Exception e)
//            {
//                binder.Abort();

//                if (binder.HandleException(e, maskingMode))
//                {
//                    return null;
//                }
//                else
//                {
//                    return e;
//                }
//            }

//            static void OnBinderCloseComplete(IAsyncResult result)
//            {
//                if (!result.CompletedSynchronously)
//                {
//                    CloseAsyncResult closeResult = (CloseAsyncResult)result.AsyncState;
//                    bool complete;
//                    Exception completeException;

//                    try
//                    {
//                        complete = closeResult.CompleteBinderClose(false, result);
//                        completeException = null;
//                    }
//                    catch (Exception e)
//                    {
//                        if (Fx.IsFatal(e))
//                        {
//                            throw;
//                        }

//                        complete = true;
//                        completeException = e;
//                    }

//                    if (complete)
//                    {
//                        if (completeException != null)
//                        {
//                            completeException = closeResult.HandleAsyncException(completeException);
//                        }

//                        closeResult.Complete(false, completeException);
//                    }
//                }
//            }

//            static void OnChannelCloseComplete(IAsyncResult result)
//            {
//                if (!result.CompletedSynchronously)
//                {
//                    CloseAsyncResult closeResult = (CloseAsyncResult)result.AsyncState;
//                    bool complete;
//                    Exception completeException;

//                    try
//                    {
//                        complete = closeResult.CompleteChannelClose(false, result);
//                        completeException = null;
//                    }
//                    catch (Exception e)
//                    {
//                        if (Fx.IsFatal(e))
//                        {
//                            throw;
//                        }

//                        complete = true;
//                        completeException = e;
//                    }

//                    if (complete)
//                    {
//                        if (completeException != null)
//                        {
//                            completeException = closeResult.HandleAsyncException(completeException);
//                        }

//                        closeResult.Complete(false, completeException);
//                    }
//                }
//            }
//        }

//        protected abstract class InputAsyncResult<TBinder> : AsyncResult
//            where TBinder : ReliableChannelBinder<TChannel>
//        {
//            bool autoAborted;
//            TBinder binder;
//            bool canGetChannel;
//            TChannel channel;
//            bool isSynchronous = true;
//            MaskingMode maskingMode;
//            static AsyncCallback onInputComplete = Fx.ThunkCallback(new AsyncCallback(OnInputCompleteStatic));
//            static AsyncCallback onTryGetChannelComplete = Fx.ThunkCallback(new AsyncCallback(OnTryGetChannelCompleteStatic));
//            bool success;
//            TimeoutHelper timeoutHelper;

//            public InputAsyncResult(TBinder binder, bool canGetChannel, TimeSpan timeout,
//                MaskingMode maskingMode, AsyncCallback callback, object state)
//                : base(callback, state)
//            {
//                binder = binder;
//                canGetChannel = canGetChannel;
//                timeoutHelper = new TimeoutHelper(timeout);
//                maskingMode = maskingMode;
//            }

//            protected abstract IAsyncResult BeginInput(TBinder binder, TChannel channel,
//                TimeSpan timeout, AsyncCallback callback, object state);

//            // returns true if the caller should retry
//            bool CompleteInput(IAsyncResult result)
//            {
//                bool complete;

//                try
//                {
//                    success = EndInput(binder, channel, result, out complete);
//                }
//                finally
//                {
//                    autoAborted = binder.Synchronizer.Aborting;
//                    binder.synchronizer.ReturnChannel();
//                }

//                return !complete;
//            }

//            // returns true if the caller should retry
//            bool CompleteTryGetChannel(IAsyncResult result, out bool complete)
//            {
//                complete = false;
//                success = binder.synchronizer.EndTryGetChannel(result, out channel);

//                // the synchronizer is faulted and not reestablishing or closed, or the call timed
//                // out, complete and don't retry.
//                if (channel == null)
//                {
//                    complete = true;
//                    return false;
//                }

//                bool throwing = true;
//                IAsyncResult inputResult = null;

//                try
//                {
//                    inputResult = BeginInput(binder, channel,
//                        timeoutHelper.RemainingTime(), onInputComplete, this);
//                    throwing = false;
//                }
//                finally
//                {
//                    if (throwing)
//                    {
//                        autoAborted = binder.Synchronizer.Aborting;
//                        binder.synchronizer.ReturnChannel();
//                    }
//                }

//                if (inputResult.CompletedSynchronously)
//                {
//                    if (CompleteInput(inputResult))
//                    {
//                        complete = false;
//                        return true;
//                    }
//                    else
//                    {
//                        complete = true;
//                        return false;
//                    }
//                }
//                else
//                {
//                    complete = false;
//                    return false;
//                }
//            }

//            public bool End()
//            {
//                AsyncResult.End<InputAsyncResult<TBinder>>(this);
//                return success;
//            }

//            protected abstract bool EndInput(TBinder binder, TChannel channel,
//                IAsyncResult result, out bool complete);

//            void OnInputComplete(IAsyncResult result)
//            {
//                isSynchronous = false;
//                bool retry;
//                Exception completeException = null;

//                try
//                {
//                    retry = CompleteInput(result);
//                }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                    {
//                        throw;
//                    }

//                    if (!binder.HandleException(e, maskingMode, autoAborted))
//                    {
//                        completeException = e;
//                        retry = false;
//                    }
//                    else
//                    {
//                        retry = true;
//                    }
//                }

//                if (retry)
//                {
//                    StartOnNonUserThread();
//                }
//                else
//                {
//                    Complete(isSynchronous, completeException);
//                }
//            }

//            static void OnInputCompleteStatic(IAsyncResult result)
//            {
//                if (!result.CompletedSynchronously)
//                {
//                    InputAsyncResult<TBinder> inputResult =
//                        (InputAsyncResult<TBinder>)result.AsyncState;
//                    inputResult.OnInputComplete(result);
//                }
//            }

//            void OnTryGetChannelComplete(IAsyncResult result)
//            {
//                isSynchronous = false;
//                bool retry = false;
//                bool complete = false;
//                Exception completeException = null;

//                try
//                {
//                    retry = CompleteTryGetChannel(result, out complete);
//                }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                    {
//                        throw;
//                    }

//                    if (!binder.HandleException(e, maskingMode, autoAborted))
//                    {
//                        completeException = e;
//                        retry = false;
//                    }
//                    else
//                    {
//                        retry = true;
//                    }
//                }

//                // Can't complete AND retry.
//                if (complete && retry)
//                {
//                    throw Fx.AssertAndThrow("The derived class' implementation of CompleteTryGetChannel() cannot indicate that the asynchronous operation should complete and retry.");
//                }

//                if (retry)
//                {
//                    StartOnNonUserThread();
//                }
//                else if (complete || completeException != null)
//                {
//                    Complete(isSynchronous, completeException);
//                }
//            }

//            static void OnTryGetChannelCompleteStatic(IAsyncResult result)
//            {
//                if (!result.CompletedSynchronously)
//                {
//                    InputAsyncResult<TBinder> inputResult =
//                        (InputAsyncResult<TBinder>)result.AsyncState;
//                    inputResult.OnTryGetChannelComplete(result);
//                }
//            }

//            protected bool Start()
//            {
//                while (true)
//                {
//                    bool retry = false;
//                    bool complete = false;

//                    autoAborted = false;

//                    try
//                    {
//                        IAsyncResult result = binder.synchronizer.BeginTryGetChannelForInput(
//                            canGetChannel, timeoutHelper.RemainingTime(),
//                            onTryGetChannelComplete, this);

//                        if (result.CompletedSynchronously)
//                        {
//                            retry = CompleteTryGetChannel(result, out complete);
//                        }
//                    }
//                    catch (Exception e)
//                    {
//                        if (Fx.IsFatal(e))
//                        {
//                            throw;
//                        }

//                        if (!binder.HandleException(e, maskingMode, autoAborted))
//                        {
//                            throw;
//                        }
//                        else
//                        {
//                            retry = true;
//                        }
//                    }

//                    // Can't complete AND retry.
//                    if (complete && retry)
//                    {
//                        throw Fx.AssertAndThrow("The derived class' implementation of CompleteTryGetChannel() cannot indicate that the asynchronous operation should complete and retry.");
//                    }

//                    if (!retry)
//                    {
//                        return complete;
//                    }
//                }
//            }

//            void StartOnNonUserThread()
//            {
//                bool complete = false;
//                Exception completeException = null;

//                try
//                {
//                    complete = Start();
//                }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                        throw;

//                    completeException = e;
//                }

//                if (complete || completeException != null)
//                    Complete(false, completeException);
//            }
//        }

//        sealed class MessageRequestContext : BinderRequestContext
//        {
//            public MessageRequestContext(ReliableChannelBinder<TChannel> binder, Message message)
//                : base(binder, message)
//            {
//            }

//            protected override void OnAbort()
//            {
//            }

//            protected override void OnClose(TimeSpan timeout)
//            {
//            }

//            protected override IAsyncResult OnBeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
//            {
//                return new ReplyAsyncResult(this, message, timeout, callback, state);
//            }

//            protected override void OnEndReply(IAsyncResult result)
//            {
//                ReplyAsyncResult.End(result);
//            }

//            protected override void OnReply(Message message, TimeSpan timeout)
//            {
//                if (message != null)
//                {
//                    Binder.Send(message, timeout, MaskingMode);
//                }
//            }

//            class ReplyAsyncResult : AsyncResult
//            {
//                static AsyncCallback onSend;
//                MessageRequestContext context;

//                public ReplyAsyncResult(MessageRequestContext context, Message message, TimeSpan timeout, AsyncCallback callback, object state)
//                    : base(callback, state)
//                {
//                    if (message != null)
//                    {
//                        if (onSend == null)
//                        {
//                            onSend = Fx.ThunkCallback(new AsyncCallback(OnSend));
//                        }
//                        context = context;
//                        IAsyncResult result = context.Binder.BeginSend(message, timeout, context.MaskingMode, onSend, this);
//                        if (!result.CompletedSynchronously)
//                        {
//                            return;
//                        }
//                        context.Binder.EndSend(result);
//                    }

//                    base.Complete(true);
//                }

//                public static void End(IAsyncResult result)
//                {
//                    AsyncResult.End<ReplyAsyncResult>(result);
//                }

//                static void OnSend(IAsyncResult result)
//                {
//                    if (result.CompletedSynchronously)
//                    {
//                        return;
//                    }

//                    Exception completionException = null;
//                    ReplyAsyncResult thisPtr = (ReplyAsyncResult)result.AsyncState;
//                    try
//                    {
//                        thisPtr.context.Binder.EndSend(result);
//                    }
//                    catch (Exception exception)
//                    {
//                        if (Fx.IsFatal(exception))
//                        {
//                            throw;
//                        }
//                        completionException = exception;
//                    }

//                    thisPtr.Complete(false, completionException);
//                }
//            }
//        }

//        protected abstract class OutputAsyncResult<TBinder> : AsyncResult
//            where TBinder : ReliableChannelBinder<TChannel>
//        {
//            bool autoAborted;
//            TBinder binder;
//            TChannel channel;
//            bool hasChannel = false;
//            MaskingMode maskingMode;
//            Message message;
//            static AsyncCallback onTryGetChannelComplete = Fx.ThunkCallback(new AsyncCallback(OnTryGetChannelCompleteStatic));
//            static AsyncCallback onOutputComplete = Fx.ThunkCallback(new AsyncCallback(OnOutputCompleteStatic));
//            TimeSpan timeout;
//            TimeoutHelper timeoutHelper;

//            public OutputAsyncResult(TBinder binder, AsyncCallback callback, object state)
//                : base(callback, state)
//            {
//                binder = binder;
//            }

//            public MaskingMode MaskingMode
//            {
//                get
//                {
//                    return maskingMode;
//                }
//            }

//            protected abstract IAsyncResult BeginOutput(TBinder binder, TChannel channel,
//                Message message, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback,
//                object state);

//            void Cleanup()
//            {
//                if (hasChannel)
//                {
//                    autoAborted = binder.Synchronizer.Aborting;
//                    binder.synchronizer.ReturnChannel();
//                }
//            }

//            bool CompleteOutput(IAsyncResult result)
//            {
//                EndOutput(binder, channel, maskingMode, result);
//                Cleanup();
//                return true;
//            }

//            bool CompleteTryGetChannel(IAsyncResult result)
//            {
//                bool timedOut = !binder.synchronizer.EndTryGetChannel(result,
//                    out channel);

//                if (timedOut || (channel == null))
//                {
//                    Cleanup();

//                    if (timedOut && !ReliableChannelBinderHelper.MaskHandled(maskingMode))
//                    {
//                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(GetTimeoutString(timeout)));
//                    }

//                    return true;
//                }

//                hasChannel = true;

//                result = BeginOutput(binder, channel, message,
//                    timeoutHelper.RemainingTime(), maskingMode, onOutputComplete,
//                    this);

//                if (result.CompletedSynchronously)
//                {
//                    return CompleteOutput(result);
//                }
//                else
//                {
//                    return false;
//                }
//            }

//            protected abstract void EndOutput(TBinder binder, TChannel channel,
//                MaskingMode maskingMode, IAsyncResult result);

//            protected abstract string GetTimeoutString(TimeSpan timeout);

//            void OnOutputComplete(IAsyncResult result)
//            {
//                if (!result.CompletedSynchronously)
//                {
//                    bool complete = false;
//                    Exception completeException = null;

//                    try
//                    {
//                        complete = CompleteOutput(result);
//                    }
//                    catch (Exception e)
//                    {
//                        if (Fx.IsFatal(e))
//                        {
//                            throw;
//                        }

//                        Cleanup();
//                        complete = true;
//                        if (!binder.HandleException(e, maskingMode, autoAborted))
//                        {
//                            completeException = e;
//                        }
//                    }

//                    if (complete)
//                    {
//                        Complete(false, completeException);
//                    }
//                }
//            }

//            static void OnOutputCompleteStatic(IAsyncResult result)
//            {
//                OutputAsyncResult<TBinder> outputResult =
//                    (OutputAsyncResult<TBinder>)result.AsyncState;

//                outputResult.OnOutputComplete(result);
//            }

//            void OnTryGetChannelComplete(IAsyncResult result)
//            {
//                if (!result.CompletedSynchronously)
//                {
//                    bool complete = false;
//                    Exception completeException = null;

//                    try
//                    {
//                        complete = CompleteTryGetChannel(result);
//                    }
//                    catch (Exception e)
//                    {
//                        if (Fx.IsFatal(e))
//                        {
//                            throw;
//                        }

//                        Cleanup();
//                        complete = true;
//                        if (!binder.HandleException(e, maskingMode, autoAborted))
//                        {
//                            completeException = e;
//                        }
//                    }

//                    if (complete)
//                    {
//                        Complete(false, completeException);
//                    }
//                }
//            }

//            static void OnTryGetChannelCompleteStatic(IAsyncResult result)
//            {
//                OutputAsyncResult<TBinder> outputResult =
//                    (OutputAsyncResult<TBinder>)result.AsyncState;

//                outputResult.OnTryGetChannelComplete(result);
//            }

//            public void Start(Message message, TimeSpan timeout, MaskingMode maskingMode)
//            {
//                if (!binder.ValidateOutputOperation(message, timeout, maskingMode))
//                {
//                    Complete(true);
//                    return;
//                }

//                message = message;
//                timeout = timeout;
//                timeoutHelper = new TimeoutHelper(timeout);
//                maskingMode = maskingMode;

//                bool complete = false;

//                try
//                {
//                    IAsyncResult result = binder.synchronizer.BeginTryGetChannelForOutput(
//                        timeoutHelper.RemainingTime(), maskingMode, onTryGetChannelComplete, this);

//                    if (result.CompletedSynchronously)
//                    {
//                        complete = CompleteTryGetChannel(result);
//                    }
//                }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                    {
//                        throw;
//                    }

//                    Cleanup();
//                    if (binder.HandleException(e, maskingMode, autoAborted))
//                    {
//                        complete = true;
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }

//                if (complete)
//                {
//                    Complete(true);
//                }
//            }
//        }

//        sealed class RequestRequestContext : BinderRequestContext
//        {
//            RequestContext innerContext;

//            public RequestRequestContext(ReliableChannelBinder<TChannel> binder,
//                RequestContext innerContext, Message message)
//                : base(binder, message)
//            {
//                if ((binder.defaultMaskingMode != MaskingMode.All) && !binder.TolerateFaults)
//                {
//                    throw Fx.AssertAndThrow("This request context is designed to catch exceptions. Thus it cannot be used if the caller expects no exception handling.");
//                }

//                if (innerContext == null)
//                {
//                    throw Fx.AssertAndThrow("Argument innerContext cannot be null.");
//                }

//                innerContext = innerContext;
//            }

//            protected override void OnAbort()
//            {
//                innerContext.Abort();
//            }

//            protected override IAsyncResult OnBeginReply(Message message, TimeSpan timeout,
//                AsyncCallback callback, object state)
//            {
//                try
//                {
//                    if (message != null)
//                        Binder.AddOutputHeaders(message);
//                    return innerContext.BeginReply(message, timeout, callback, state);
//                }
//                catch (ObjectDisposedException) { }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                    {
//                        throw;
//                    }

//                    if (!Binder.HandleException(e, MaskingMode))
//                    {
//                        throw;
//                    }

//                    innerContext.Abort();
//                }

//                return new BinderCompletedAsyncResult(callback, state);
//            }

//            protected override void OnClose(TimeSpan timeout)
//            {
//                try
//                {
//                    innerContext.Close(timeout);
//                }
//                catch (ObjectDisposedException) { }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                    {
//                        throw;
//                    }

//                    if (!Binder.HandleException(e, MaskingMode))
//                    {
//                        throw;
//                    }

//                    innerContext.Abort();
//                }
//            }

//            protected override void OnEndReply(IAsyncResult result)
//            {
//                BinderCompletedAsyncResult completedResult = result as BinderCompletedAsyncResult;
//                if (completedResult != null)
//                {
//                    completedResult.End();
//                    return;
//                }

//                try
//                {
//                    innerContext.EndReply(result);
//                }
//                catch (ObjectDisposedException) { }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                    {
//                        throw;
//                    }

//                    if (!Binder.HandleException(e, MaskingMode))
//                    {
//                        throw;
//                    }

//                    innerContext.Abort();
//                }
//            }

//            protected override void OnReply(Message message, TimeSpan timeout)
//            {
//                try
//                {
//                    if (message != null)
//                        Binder.AddOutputHeaders(message);
//                    innerContext.Reply(message, timeout);
//                }
//                catch (ObjectDisposedException) { }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                    {
//                        throw;
//                    }

//                    if (!Binder.HandleException(e, MaskingMode))
//                    {
//                        throw;
//                    }

//                    innerContext.Abort();
//                }
//            }
//        }

//        sealed class SendAsyncResult : OutputAsyncResult<ReliableChannelBinder<TChannel>>
//        {
//            public SendAsyncResult(ReliableChannelBinder<TChannel> binder, AsyncCallback callback,
//                object state)
//                : base(binder, callback, state)
//            {
//            }

//            protected override IAsyncResult BeginOutput(ReliableChannelBinder<TChannel> binder,
//                TChannel channel, Message message, TimeSpan timeout, MaskingMode maskingMode,
//                AsyncCallback callback, object state)
//            {
//                binder.AddOutputHeaders(message);
//                return binder.OnBeginSend(channel, message, timeout, callback, state);
//            }

//            public static void End(IAsyncResult result)
//            {
//                AsyncResult.End<SendAsyncResult>(result);
//            }

//            protected override void EndOutput(ReliableChannelBinder<TChannel> binder,
//                TChannel channel, MaskingMode maskingMode, IAsyncResult result)
//            {
//                binder.OnEndSend(channel, result);
//            }

//            protected override string GetTimeoutString(TimeSpan timeout)
//            {
//                return SR.GetString(SR.TimeoutOnSend, timeout);
//            }
//        }

//        sealed class TryReceiveAsyncResult : InputAsyncResult<ReliableChannelBinder<TChannel>>
//        {
//            RequestContext requestContext;

//            public TryReceiveAsyncResult(ReliableChannelBinder<TChannel> binder, TimeSpan timeout,
//                MaskingMode maskingMode, AsyncCallback callback, object state)
//                : base(binder, binder.CanGetChannelForReceive, timeout, maskingMode, callback, state)
//            {
//                if (Start())
//                    Complete(true);
//            }

//            protected override IAsyncResult BeginInput(ReliableChannelBinder<TChannel> binder,
//                TChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
//            {
//                return binder.OnBeginTryReceive(channel, timeout, callback, state);
//            }

//            public bool End(out RequestContext requestContext)
//            {
//                requestContext = requestContext;
//                return End();
//            }

//            protected override bool EndInput(ReliableChannelBinder<TChannel> binder,
//                TChannel channel, IAsyncResult result, out bool complete)
//            {
//                bool success = binder.OnEndTryReceive(channel, result, out requestContext);

//                // timed out || got message, complete immediately
//                complete = !success || (requestContext != null);

//                if (!complete)
//                {
//                    // the underlying channel closed or faulted
//                    binder.synchronizer.OnReadEof();
//                }

//                return success;
//            }
//        }
//    }

//    static class ReliableChannelBinderHelper
//    {
//        internal static IAsyncResult BeginCloseDuplexSessionChannel(
//            ReliableChannelBinder<IDuplexSessionChannel> binder, IDuplexSessionChannel channel,
//            TimeSpan timeout, AsyncCallback callback, object state)
//        {
//            return new CloseDuplexSessionChannelAsyncResult(binder, channel, timeout, callback,
//                state);
//        }

//        internal static IAsyncResult BeginCloseReplySessionChannel(
//            ReliableChannelBinder<IReplySessionChannel> binder, IReplySessionChannel channel,
//            TimeSpan timeout, AsyncCallback callback, object state)
//        {
//            return new CloseReplySessionChannelAsyncResult(binder, channel, timeout, callback,
//                state);
//        }

//        internal static void CloseDuplexSessionChannel(
//            ReliableChannelBinder<IDuplexSessionChannel> binder, IDuplexSessionChannel channel,
//            TimeSpan timeout)
//        {
//            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

//            channel.Session.CloseOutputSession(timeoutHelper.RemainingTime());
//            binder.WaitForPendingOperations(timeoutHelper.RemainingTime());

//            TimeSpan iterationTimeout = timeoutHelper.RemainingTime();
//            bool lastIteration = (iterationTimeout == TimeSpan.Zero);

//            while (true)
//            {
//                Message message = null;
//                bool receiveThrowing = true;

//                try
//                {
//                    bool success = channel.TryReceive(iterationTimeout, out message);

//                    receiveThrowing = false;
//                    if (success && message == null)
//                    {
//                        channel.Close(timeoutHelper.RemainingTime());
//                        return;
//                    }
//                }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                        throw;

//                    if (receiveThrowing)
//                    {
//                        if (!MaskHandled(binder.DefaultMaskingMode) || !binder.IsHandleable(e))
//                            throw;

//                        receiveThrowing = false;
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                finally
//                {
//                    if (message != null)
//                        message.Close();

//                    if (receiveThrowing)
//                        channel.Abort();
//                }

//                if (lastIteration || channel.State != CommunicationState.Opened)
//                    break;

//                iterationTimeout = timeoutHelper.RemainingTime();
//                lastIteration = (iterationTimeout == TimeSpan.Zero);
//            }

//            channel.Abort();
//        }

//        internal static void CloseReplySessionChannel(
//            ReliableChannelBinder<IReplySessionChannel> binder, IReplySessionChannel channel,
//            TimeSpan timeout)
//        {
//            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

//            binder.WaitForPendingOperations(timeoutHelper.RemainingTime());

//            TimeSpan iterationTimeout = timeoutHelper.RemainingTime();
//            bool lastIteration = (iterationTimeout == TimeSpan.Zero);

//            while (true)
//            {
//                RequestContext context = null;
//                bool receiveThrowing = true;

//                try
//                {
//                    bool success = channel.TryReceiveRequest(iterationTimeout, out context);

//                    receiveThrowing = false;
//                    if (success && context == null)
//                    {
//                        channel.Close(timeoutHelper.RemainingTime());
//                        return;
//                    }
//                }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                        throw;

//                    if (receiveThrowing)
//                    {
//                        if (!MaskHandled(binder.DefaultMaskingMode) || !binder.IsHandleable(e))
//                            throw;

//                        receiveThrowing = false;
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                finally
//                {
//                    if (context != null)
//                    {
//                        context.RequestMessage.Close();
//                        context.Close();
//                    }

//                    if (receiveThrowing)
//                        channel.Abort();
//                }

//                if (lastIteration || channel.State != CommunicationState.Opened)
//                    break;

//                iterationTimeout = timeoutHelper.RemainingTime();
//                lastIteration = (iterationTimeout == TimeSpan.Zero);
//            }

//            channel.Abort();
//        }

//        internal static void EndCloseDuplexSessionChannel(IDuplexSessionChannel channel,
//                IAsyncResult result)
//        {
//            CloseDuplexSessionChannelAsyncResult.End(result);
//        }

//        internal static void EndCloseReplySessionChannel(IReplySessionChannel channel,
//                IAsyncResult result)
//        {
//            CloseReplySessionChannelAsyncResult.End(result);
//        }

//        internal static bool MaskHandled(MaskingMode maskingMode)
//        {
//            return (maskingMode & MaskingMode.Handled) == MaskingMode.Handled;
//        }

//        internal static bool MaskUnhandled(MaskingMode maskingMode)
//        {
//            return (maskingMode & MaskingMode.Unhandled) == MaskingMode.Unhandled;
//        }

//        abstract class CloseInputSessionChannelAsyncResult<TChannel, TItem> : AsyncResult
//            where TChannel : class, IChannel
//            where TItem : class
//        {
//            static AsyncCallback onChannelCloseCompleteStatic =
//                Fx.ThunkCallback(
//                new AsyncCallback(OnChannelCloseCompleteStatic));
//            static AsyncCallback onInputCompleteStatic =
//                Fx.ThunkCallback(new AsyncCallback(OnInputCompleteStatic));
//            static AsyncCallback onWaitForPendingOperationsCompleteStatic =
//                Fx.ThunkCallback(
//                new AsyncCallback(OnWaitForPendingOperationsCompleteStatic));
//            ReliableChannelBinder<TChannel> binder;
//            TChannel channel;
//            bool lastReceive;
//            TimeoutHelper timeoutHelper;

//            protected CloseInputSessionChannelAsyncResult(
//                ReliableChannelBinder<TChannel> binder, TChannel channel,
//                TimeSpan timeout, AsyncCallback callback, object state)
//                : base(callback, state)
//            {
//                binder = binder;
//                channel = channel;
//                timeoutHelper = new TimeoutHelper(timeout);
//            }

//            protected TChannel Channel
//            {
//                get
//                {
//                    return channel;
//                }
//            }

//            protected TimeSpan RemainingTime
//            {
//                get
//                {
//                    return timeoutHelper.RemainingTime();
//                }
//            }

//            protected bool Begin()
//            {
//                bool complete = false;
//                IAsyncResult result = binder.BeginWaitForPendingOperations(
//                    RemainingTime, onWaitForPendingOperationsCompleteStatic,
//                    this);

//                if (result.CompletedSynchronously)
//                    complete = HandleWaitForPendingOperationsComplete(result);

//                return complete;
//            }

//            protected abstract IAsyncResult BeginTryInput(TimeSpan timeout, AsyncCallback callback,
//                object state);

//            protected abstract void DisposeItem(TItem item);

//            protected abstract bool EndTryInput(IAsyncResult result, out TItem item);

//            void HandleChannelCloseComplete(IAsyncResult result)
//            {
//                channel.EndClose(result);
//            }

//            bool HandleInputComplete(IAsyncResult result, out bool gotEof)
//            {
//                TItem item = null;
//                bool endThrowing = true;

//                gotEof = false;

//                try
//                {
//                    bool success = false;

//                    success = EndTryInput(result, out item);
//                    endThrowing = false;

//                    if (!success || item != null)
//                    {
//                        if (lastReceive || channel.State != CommunicationState.Opened)
//                        {
//                            channel.Abort();
//                            return true;
//                        }
//                        else
//                        {
//                            return false;
//                        }
//                    }

//                    gotEof = true;

//                    result = channel.BeginClose(RemainingTime,
//                        onChannelCloseCompleteStatic, this);
//                    if (result.CompletedSynchronously)
//                    {
//                        HandleChannelCloseComplete(result);
//                        return true;
//                    }
//                    else
//                    {
//                        return false;
//                    }
//                }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                        throw;

//                    if (endThrowing)
//                    {
//                        if (!MaskHandled(binder.DefaultMaskingMode) || !binder.IsHandleable(e))
//                            throw;

//                        if (lastReceive || channel.State != CommunicationState.Opened)
//                        {
//                            channel.Abort();
//                            return true;
//                        }
//                        else
//                        {
//                            return false;
//                        }
//                    }

//                    throw;
//                }
//                finally
//                {
//                    if (item != null)
//                        DisposeItem(item);

//                    if (endThrowing)
//                        channel.Abort();
//                }
//            }

//            bool HandleWaitForPendingOperationsComplete(IAsyncResult result)
//            {
//                binder.EndWaitForPendingOperations(result);
//                return WaitForEof();
//            }

//            static void OnChannelCloseCompleteStatic(IAsyncResult result)
//            {
//                if (result.CompletedSynchronously)
//                    return;

//                CloseInputSessionChannelAsyncResult<TChannel, TItem> closeResult =
//                    (CloseInputSessionChannelAsyncResult<TChannel, TItem>)result.AsyncState;

//                Exception completeException = null;

//                try
//                {
//                    closeResult.HandleChannelCloseComplete(result);
//                }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                        throw;

//                    completeException = e;
//                }

//                closeResult.Complete(false, completeException);
//            }

//            static void OnInputCompleteStatic(IAsyncResult result)
//            {
//                if (result.CompletedSynchronously)
//                    return;

//                CloseInputSessionChannelAsyncResult<TChannel, TItem> closeResult =
//                    (CloseInputSessionChannelAsyncResult<TChannel, TItem>)result.AsyncState;

//                bool complete = false;
//                Exception completeException = null;

//                try
//                {
//                    bool gotEof;

//                    complete = closeResult.HandleInputComplete(result, out gotEof);
//                    if (!complete && !gotEof)
//                        complete = closeResult.WaitForEof();
//                }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                        throw;

//                    completeException = e;
//                }

//                if (complete || completeException != null)
//                    closeResult.Complete(false, completeException);
//            }

//            static void OnWaitForPendingOperationsCompleteStatic(IAsyncResult result)
//            {
//                if (result.CompletedSynchronously)
//                    return;

//                CloseInputSessionChannelAsyncResult<TChannel, TItem> closeResult =
//                    (CloseInputSessionChannelAsyncResult<TChannel, TItem>)result.AsyncState;

//                bool complete = false;
//                Exception completeException = null;

//                try
//                {
//                    complete = closeResult.HandleWaitForPendingOperationsComplete(result);
//                }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                        throw;

//                    completeException = e;
//                }

//                if (complete || completeException != null)
//                    closeResult.Complete(false, completeException);
//            }

//            bool WaitForEof()
//            {
//                TimeSpan iterationTimeout = RemainingTime;
//                lastReceive = (iterationTimeout == TimeSpan.Zero);

//                while (true)
//                {
//                    IAsyncResult result = null;

//                    try
//                    {
//                        result = BeginTryInput(iterationTimeout, onInputCompleteStatic, this);
//                    }
//                    catch (Exception e)
//                    {
//                        if (Fx.IsFatal(e))
//                            throw;

//                        if (!MaskHandled(binder.DefaultMaskingMode) || !binder.IsHandleable(e))
//                            throw;
//                    }

//                    if (result != null)
//                    {
//                        if (result.CompletedSynchronously)
//                        {
//                            bool gotEof;
//                            bool complete = HandleInputComplete(result, out gotEof);

//                            if (complete || gotEof)
//                                return complete;
//                        }
//                        else
//                            return false;
//                    }

//                    if (lastReceive || channel.State != CommunicationState.Opened)
//                    {
//                        channel.Abort();
//                        break;
//                    }

//                    iterationTimeout = RemainingTime;
//                    lastReceive = (iterationTimeout == TimeSpan.Zero);
//                }

//                return true;
//            }
//        }

//        sealed class CloseDuplexSessionChannelAsyncResult :
//            CloseInputSessionChannelAsyncResult<IDuplexSessionChannel, Message>
//        {
//            static AsyncCallback onCloseOutputSessionCompleteStatic =
//                Fx.ThunkCallback(
//                new AsyncCallback(OnCloseOutputSessionCompleteStatic));

//            public CloseDuplexSessionChannelAsyncResult(
//                ReliableChannelBinder<IDuplexSessionChannel> binder, IDuplexSessionChannel channel,
//                TimeSpan timeout, AsyncCallback callback, object state)
//                : base(binder, channel, timeout, callback, state)
//            {
//                bool complete = false;

//                IAsyncResult result = Channel.Session.BeginCloseOutputSession(
//                    RemainingTime, onCloseOutputSessionCompleteStatic, this);

//                if (result.CompletedSynchronously)
//                    complete = HandleCloseOutputSessionComplete(result);

//                if (complete)
//                    Complete(true);
//            }

//            protected override IAsyncResult BeginTryInput(TimeSpan timeout, AsyncCallback callback, object state)
//            {
//                return Channel.BeginTryReceive(timeout, callback, state);
//            }

//            protected override void DisposeItem(Message item)
//            {
//                item.Close();
//            }

//            public static void End(IAsyncResult result)
//            {
//                AsyncResult.End<CloseDuplexSessionChannelAsyncResult>(result);
//            }

//            protected override bool EndTryInput(IAsyncResult result, out Message item)
//            {
//                return Channel.EndTryReceive(result, out item);
//            }

//            bool HandleCloseOutputSessionComplete(IAsyncResult result)
//            {
//                Channel.Session.EndCloseOutputSession(result);
//                return Begin();
//            }

//            static void OnCloseOutputSessionCompleteStatic(IAsyncResult result)
//            {
//                if (result.CompletedSynchronously)
//                    return;

//                CloseDuplexSessionChannelAsyncResult closeResult =
//                    (CloseDuplexSessionChannelAsyncResult)result.AsyncState;

//                bool complete = false;
//                Exception completeException = null;

//                try
//                {
//                    complete = closeResult.HandleCloseOutputSessionComplete(result);
//                }
//                catch (Exception e)
//                {
//                    if (Fx.IsFatal(e))
//                        throw;

//                    completeException = e;
//                }

//                if (complete || completeException != null)
//                    closeResult.Complete(false, completeException);
//            }
//        }

//        sealed class CloseReplySessionChannelAsyncResult :
//            CloseInputSessionChannelAsyncResult<IReplySessionChannel, RequestContext>
//        {
//            public CloseReplySessionChannelAsyncResult(
//                ReliableChannelBinder<IReplySessionChannel> binder, IReplySessionChannel channel,
//                TimeSpan timeout, AsyncCallback callback, object state)
//                : base(binder, channel, timeout, callback, state)
//            {
//                if (Begin())
//                    Complete(true);
//            }

//            protected override IAsyncResult BeginTryInput(TimeSpan timeout, AsyncCallback callback, object state)
//            {
//                return Channel.BeginTryReceiveRequest(timeout, callback, state);
//            }

//            protected override void DisposeItem(RequestContext item)
//            {
//                item.RequestMessage.Close();
//                item.Close();
//            }

//            public static void End(IAsyncResult result)
//            {
//                AsyncResult.End<CloseReplySessionChannelAsyncResult>(result);
//            }

//            protected override bool EndTryInput(IAsyncResult result, out RequestContext item)
//            {
//                return Channel.EndTryReceiveRequest(result, out item);
//            }
//        }
//    }
}

