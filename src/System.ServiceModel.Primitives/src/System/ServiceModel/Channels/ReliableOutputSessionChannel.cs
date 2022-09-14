// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal abstract class ReliableOutputSessionChannel : OutputChannel, IOutputSessionChannel, IAsyncOutputSessionChannel
    {
        private IClientReliableChannelBinder _binder;
        private ChannelParameterCollection _channelParameters;
        private ReliableRequestor _closeRequestor;
        private Exception _maxRetryCountException = null;
        private ClientReliableSession _session;
        private ReliableRequestor _terminateRequestor;

        protected ReliableOutputSessionChannel(
            ChannelManagerBase factory,
            IReliableFactorySettings settings,
            IClientReliableChannelBinder binder,
            FaultHelper faultHelper,
            LateBoundChannelParameterCollection channelParameters)
            : base(factory)
        {
            Settings = settings;
            _binder = binder;
            _session = new ClientReliableSession(this, settings, binder, faultHelper, null);
            _session.PollingCallback = PollingAsyncCallback;
            _session.UnblockChannelCloseCallback = UnblockClose;
            _binder.Faulted += OnBinderFaulted;
            _binder.OnException += OnBinderException;

            _channelParameters = channelParameters;
            channelParameters.SetChannel(this);
        }

        protected IReliableChannelBinder Binder
        {
            get
            {
                return _binder;
            }
        }

        protected ReliableOutputConnection Connection { get; private set; }

        protected Exception MaxRetryCountException
        {
            set
            {
                _maxRetryCountException = value;
            }
        }

        protected ChannelReliableSession ReliableSession
        {
            get
            {
                return _session;
            }
        }

        public override EndpointAddress RemoteAddress
        {
            get
            {
                return _binder.RemoteAddress;
            }
        }

        protected abstract bool RequestAcks
        {
            get;
        }

        public IOutputSession Session
        {
            get
            {
                return _session;
            }
        }

        public override Uri Via
        {
            get
            {
                return _binder.Via;
            }
        }

        protected IReliableFactorySettings Settings { get; }

        private async Task CloseSequenceAsync(TimeSpan timeout)
        {
            CreateCloseRequestor();
            Message closeReply = await _closeRequestor.RequestAsync(timeout);
            ProcessCloseOrTerminateReply(true, closeReply);
        }

        private void ConfigureRequestor(ReliableRequestor requestor)
        {
            requestor.MessageVersion = Settings.MessageVersion;
            requestor.Binder = _binder;
            requestor.SetRequestResponsePattern();
        }

        private void CreateCloseRequestor()
        {
            ReliableRequestor temp = CreateRequestor();
            ConfigureRequestor(temp);
            temp.TimeoutString1Index = SRP.TimeoutOnClose;
            temp.MessageAction = WsrmIndex.GetCloseSequenceActionHeader(
                Settings.MessageVersion.Addressing);
            temp.MessageBody = new CloseSequence(_session.OutputID, Connection.Last);

            lock (ThisLock)
            {
                ThrowIfClosed();
                _closeRequestor = temp;
            }
        }

        protected abstract ReliableRequestor CreateRequestor();

        private void CreateTerminateRequestor()
        {
            ReliableRequestor temp = CreateRequestor();
            ConfigureRequestor(temp);
            ReliableMessagingVersion reliableMessagingVersion = Settings.ReliableMessagingVersion;
            temp.MessageAction = WsrmIndex.GetTerminateSequenceActionHeader(
                Settings.MessageVersion.Addressing, reliableMessagingVersion);
            temp.MessageBody = new TerminateSequence(reliableMessagingVersion, _session.OutputID,
                Connection.Last);

            lock (ThisLock)
            {
                ThrowIfClosed();
                _terminateRequestor = temp;
                _session.CloseSession();
            }
        }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(IOutputSessionChannel))
            {
                return (T)(object)this;
            }

            if (typeof(T) == typeof(ChannelParameterCollection))
            {
                return (T)(object)_channelParameters;
            }

            T baseProperty = base.GetProperty<T>();

            if (baseProperty != null)
            {
                return baseProperty;
            }

            T innerProperty = _binder.Channel.GetProperty<T>();
            if ((innerProperty == null) && (typeof(T) == typeof(FaultConverter)))
            {
                return (T)(object)FaultConverter.GetDefaultFaultConverter(Settings.MessageVersion);
            }
            else
            {
                return innerProperty;
            }
        }

        protected override void OnAbort()
        {
            if (Connection != null)
            {
                Connection.Abort(this);
            }

            ReliableRequestor tempRequestor = _closeRequestor;
            if (tempRequestor != null)
            {
                tempRequestor.Abort(this);
            }

            tempRequestor = _terminateRequestor;
            if (tempRequestor != null)
            {
                tempRequestor.Abort(this);
            }

            _session.Abort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return CommunicationObjectInternal.OnBeginClose(this, timeout, callback, state);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return CommunicationObjectInternal.OnBeginOpen(this, timeout, callback, state);
        }

        private void OnBinderException(IReliableChannelBinder sender, Exception exception)
        {
            if (exception is QuotaExceededException)
            {
                if (State == CommunicationState.Opening ||
                    State == CommunicationState.Opened ||
                    State == CommunicationState.Closing)
                {
                    _session.OnLocalFault(exception, SequenceTerminatedFault.CreateQuotaExceededFault(_session.OutputID), null);
                }
            }
            else
            {
                AddPendingException(exception);
            }
        }

        private void OnBinderFaulted(IReliableChannelBinder sender, Exception exception)
        {
            _binder.Abort();

            if (State == CommunicationState.Opening ||
                State == CommunicationState.Opened ||
                State == CommunicationState.Closing)
            {
                exception = new CommunicationException(SRP.EarlySecurityFaulted, exception);
                _session.OnLocalFault(exception, (Message)null, null);
            }
        }

        protected override void OnClose(TimeSpan timeout)
        {
            CommunicationObjectInternal.OnClose(this, timeout);
        }

        protected internal override async Task OnCloseAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await Connection.CloseAsync(timeoutHelper.RemainingTime());

            if (Settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                await CloseSequenceAsync(timeoutHelper.RemainingTime());
            }

            await TerminateSequenceAsync(timeoutHelper.RemainingTime());
            await _session.CloseAsync(timeoutHelper.RemainingTime());
            await _binder.CloseAsync(timeoutHelper.RemainingTime(), MaskingMode.Handled);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            _binder.Faulted -= OnBinderFaulted;
        }

        protected abstract Task OnConnectionSendAsync(Message message, TimeSpan timeout, bool saveHandledException, bool maskUnhandledException);

        private async Task OnConnectionSendAckRequestedAsyncHandler(TimeSpan timeout)
        {
            _session.OnLocalActivity();
            using (Message message = WsrmUtilities.CreateAckRequestedMessage(Settings.MessageVersion,
                Settings.ReliableMessagingVersion, ReliableSession.OutputID))
            {
                await OnConnectionSendAsync(message, timeout, false, true);
            }
        }

        private async Task OnConnectionSendAsyncHandler(MessageAttemptInfo attemptInfo, TimeSpan timeout, bool maskUnhandledException)
        {
            using (attemptInfo.Message)
            {
                if (attemptInfo.RetryCount > Settings.MaxRetryCount)
                {
                    if (WcfEventSource.Instance.MaxRetryCyclesExceededIsEnabled())
                    {
                        WcfEventSource.Instance.MaxRetryCyclesExceeded(SRP.MaximumRetryCountExceeded);
                    }
                    _session.OnLocalFault(new CommunicationException(SRP.MaximumRetryCountExceeded, _maxRetryCountException),
                        SequenceTerminatedFault.CreateMaxRetryCountExceededFault(_session.OutputID), null);
                }
                else
                {
                    _session.OnLocalActivity();
                    await OnConnectionSendAsync(attemptInfo.Message, timeout,
                        (attemptInfo.RetryCount == Settings.MaxRetryCount), maskUnhandledException);
                }
            }
        }

        protected abstract Task OnConnectionSendMessageAsync(Message message, TimeSpan timeout, MaskingMode maskingMode);

        private void OnComponentFaulted(Exception faultException, WsrmFault fault)
        {
            _session.OnLocalFault(faultException, fault, null);
        }

        private void OnComponentException(Exception exception)
        {
            ReliableSession.OnUnknownException(exception);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CommunicationObjectInternal.OnEnd(result);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CommunicationObjectInternal.OnEnd(result);
        }

        protected override void OnFaulted()
        {
            _session.OnFaulted();
            UnblockClose();
            base.OnFaulted();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            CommunicationObjectInternal.OnOpen(this, timeout);
        }

        protected internal override async Task OnOpenAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            bool throwing = true;

            try
            {
                await _binder.OpenAsync(timeoutHelper.RemainingTime());
                await _session.OpenAsync(timeoutHelper.RemainingTime());
                throwing = false;
            }
            finally
            {
                if (throwing)
                {
                    await Binder.CloseAsync(timeoutHelper.RemainingTime());
                }
            }
        }

        protected override async Task OnSendAsync(Message message, TimeSpan timeout)
        {
            if (!await Connection.AddMessageAsync(message, timeout, null))
                ThrowInvalidAddException();
        }

        protected override void OnSend(Message message, TimeSpan timeout)
        {
            TaskHelpers.WaitForCompletionNoSpin(OnSendAsync(message, timeout));
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            Connection = new ReliableOutputConnection(_session.OutputID, Settings.MaxTransferWindowSize,
                Settings.MessageVersion, Settings.ReliableMessagingVersion, _session.InitiationTime,
                RequestAcks, DefaultSendTimeout);
            Connection.Faulted += OnComponentFaulted;
            Connection.OnException += OnComponentException;
            Connection.SendAsyncHandler = OnConnectionSendAsyncHandler;
            Connection.SendAckRequestedAsyncHandler = OnConnectionSendAckRequestedAsyncHandler;
        }

        private async Task PollingAsyncCallback()
        {
            using (Message request = WsrmUtilities.CreateAckRequestedMessage(Settings.MessageVersion,
                Settings.ReliableMessagingVersion, ReliableSession.OutputID))
            {
                await OnConnectionSendMessageAsync(request, DefaultSendTimeout, MaskingMode.All);
            }
        }

        private void ProcessCloseOrTerminateReply(bool close, Message reply)
        {
            if (reply == null)
            {
                // In the close case, the requestor is configured to throw TimeoutException instead of returning null.
                // In the terminate case, this value can be null, but the caller should not call this method.
                throw Fx.AssertAndThrow("Argument reply cannot be null.");
            }

            ReliableRequestor requestor = close ? _closeRequestor : _terminateRequestor;
            WsrmMessageInfo info = requestor.GetInfo();

            // Some other thread has verified and cleaned up the reply, no more work to do.
            if (info != null)
            {
                return;
            }

            try
            {
                info = WsrmMessageInfo.Get(Settings.MessageVersion, Settings.ReliableMessagingVersion,
                    _binder.Channel, _binder.GetInnerSession(), reply);
                ReliableSession.ProcessInfo(info, null, true);
                ReliableSession.VerifyDuplexProtocolElements(info, null, true);

                WsrmFault fault = close
                    ? WsrmUtilities.ValidateCloseSequenceResponse(_session, requestor.MessageId, info,
                    Connection.Last)
                    : WsrmUtilities.ValidateTerminateSequenceResponse(_session, requestor.MessageId, info,
                    Connection.Last);

                if (fault != null)
                {
                    ReliableSession.OnLocalFault(null, fault, null);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(fault.CreateException());
                }
            }
            finally
            {
                reply.Close();
            }
        }

        protected async Task ProcessMessageAsync(Message message)
        {
            bool closeMessage = true;
            WsrmMessageInfo messageInfo = WsrmMessageInfo.Get(Settings.MessageVersion,
                Settings.ReliableMessagingVersion, _binder.Channel, _binder.GetInnerSession(), message);
            bool wsrm11 = Settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11;

            try
            {
                if (!_session.ProcessInfo(messageInfo, null))
                {
                    closeMessage = false;
                    return;
                }

                if (!ReliableSession.VerifySimplexProtocolElements(messageInfo, null))
                {
                    closeMessage = false;
                    return;
                }

                bool final = false;

                if (messageInfo.AcknowledgementInfo != null)
                {
                    final = wsrm11 && messageInfo.AcknowledgementInfo.Final;
                    int bufferRemaining = -1;

                    if (Settings.FlowControlEnabled)
                        bufferRemaining = messageInfo.AcknowledgementInfo.BufferRemaining;

                    Connection.ProcessTransferred(messageInfo.AcknowledgementInfo.Ranges, bufferRemaining);
                }

                if (wsrm11)
                {
                    WsrmFault fault = null;

                    if (messageInfo.TerminateSequenceResponseInfo != null)
                    {
                        fault = WsrmUtilities.ValidateTerminateSequenceResponse(_session,
                            _terminateRequestor.MessageId, messageInfo, Connection.Last);

                        if (fault == null)
                        {
                            fault = ProcessRequestorResponse(_terminateRequestor, WsrmFeb2005Strings.TerminateSequence, messageInfo);
                        }
                    }
                    else if (messageInfo.CloseSequenceResponseInfo != null)
                    {
                        fault = WsrmUtilities.ValidateCloseSequenceResponse(_session,
                            _closeRequestor.MessageId, messageInfo, Connection.Last);

                        if (fault == null)
                        {
                            fault = ProcessRequestorResponse(_closeRequestor, Wsrm11Strings.CloseSequence, messageInfo);
                        }
                    }
                    else if (messageInfo.TerminateSequenceInfo != null)
                    {
                        if (!WsrmUtilities.ValidateWsrmRequest(_session, messageInfo.TerminateSequenceInfo, _binder, null))
                        {
                            return;
                        }

                        WsrmAcknowledgmentInfo ackInfo = messageInfo.AcknowledgementInfo;
                        fault = WsrmUtilities.ValidateFinalAckExists(_session, ackInfo);

                        if ((fault == null) && !Connection.IsFinalAckConsistent(ackInfo.Ranges))
                        {
                            fault = new InvalidAcknowledgementFault(_session.OutputID, ackInfo.Ranges);
                        }

                        if (fault == null)
                        {
                            Message response = WsrmUtilities.CreateTerminateResponseMessage(
                                Settings.MessageVersion,
                                messageInfo.TerminateSequenceInfo.MessageId,
                                _session.OutputID);

                            try
                            {
                                await OnConnectionSendAsync(response, DefaultSendTimeout, false, true);
                            }
                            finally
                            {
                                response.Close();
                            }

                            _session.OnRemoteFault(new ProtocolException(SRP.UnsupportedTerminateSequenceExceptionString));
                            return;
                        }
                    }
                    else if (final)
                    {
                        if (_closeRequestor == null)
                        {
                            string exceptionString = SRP.UnsupportedCloseExceptionString;
                            string faultString = SRP.SequenceTerminatedUnsupportedClose;

                            fault = SequenceTerminatedFault.CreateProtocolFault(_session.OutputID, faultString,
                                exceptionString);
                        }
                        else
                        {
                            fault = WsrmUtilities.ValidateFinalAck(_session, messageInfo, Connection.Last);

                            if (fault == null)
                            {
                                _closeRequestor.SetInfo(messageInfo);
                            }
                        }
                    }
                    else if (messageInfo.WsrmHeaderFault != null)
                    {
                        if (!(messageInfo.WsrmHeaderFault is UnknownSequenceFault))
                        {
                            throw Fx.AssertAndThrow("Fault must be UnknownSequence fault.");
                        }

                        if (_terminateRequestor == null)
                        {
                            throw Fx.AssertAndThrow("In wsrm11, if we start getting UnknownSequence, terminateRequestor cannot be null.");
                        }

                        _terminateRequestor.SetInfo(messageInfo);
                    }

                    if (fault != null)
                    {
                        _session.OnLocalFault(fault.CreateException(), fault, null);
                        return;
                    }
                }

                _session.OnRemoteActivity(Connection.Strategy.QuotaRemaining == 0);
            }
            finally
            {
                if (closeMessage)
                    messageInfo.Message.Close();
            }
        }

        protected abstract WsrmFault ProcessRequestorResponse(ReliableRequestor requestor, string requestName, WsrmMessageInfo info);

        private async Task TerminateSequenceAsync(TimeSpan timeout)
        {
            ReliableMessagingVersion reliableMessagingVersion = Settings.ReliableMessagingVersion;

            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                _session.CloseSession();
                Message message = WsrmUtilities.CreateTerminateMessage(Settings.MessageVersion,
                    reliableMessagingVersion, _session.OutputID);
                await OnConnectionSendMessageAsync(message, timeout, MaskingMode.Handled);
            }
            else if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                CreateTerminateRequestor();
                Message terminateReply = await _terminateRequestor.RequestAsync(timeout);

                if (terminateReply != null)
                {
                    ProcessCloseOrTerminateReply(false, terminateReply);
                }
            }
            else
            {
                throw Fx.AssertAndThrow("Reliable messaging version not supported.");
            }
        }

        private void ThrowInvalidAddException()
        {
            if (State == CommunicationState.Faulted)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(GetTerminalException());
            else
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateClosedException());
        }

        private void UnblockClose()
        {
            if (Connection != null)
            {
                Connection.Fault(this);
            }

            ReliableRequestor tempRequestor = _closeRequestor;
            if (tempRequestor != null)
            {
                tempRequestor.Fault(this);
            }

            tempRequestor = _terminateRequestor;
            if (tempRequestor != null)
            {
                tempRequestor.Fault(this);
            }
        }
    }

    internal sealed class ReliableOutputSessionChannelOverRequest : ReliableOutputSessionChannel
    {
        private IClientReliableChannelBinder binder;

        public ReliableOutputSessionChannelOverRequest(ChannelManagerBase factory, IReliableFactorySettings settings,
            IClientReliableChannelBinder binder, FaultHelper faultHelper,
            LateBoundChannelParameterCollection channelParameters)
            : base(factory, settings, binder, faultHelper, channelParameters)
        {
            this.binder = binder;
        }

        protected override bool RequestAcks
        {
            get
            {
                return false;
            }
        }

        protected override ReliableRequestor CreateRequestor()
        {
            return new RequestReliableRequestor();
        }

        protected override async Task OnConnectionSendAsync(Message message, TimeSpan timeout,
            bool saveHandledException, bool maskUnhandledException)
        {
            MaskingMode maskingMode = maskUnhandledException ? MaskingMode.Unhandled : MaskingMode.None;
            Message reply = null;

            if (saveHandledException)
            {
                try
                {
                    reply = await binder.RequestAsync(message, timeout, maskingMode);
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                        throw;

                    if (Binder.IsHandleable(e))
                    {
                        MaxRetryCountException = e;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                maskingMode |= MaskingMode.Handled;
                reply = await binder.RequestAsync(message, timeout, maskingMode);

                if (reply != null)
                {
                    await ProcessMessageAsync(reply);
                }
            }
        }

        protected override async Task OnConnectionSendMessageAsync(Message message, TimeSpan timeout, MaskingMode maskingMode)
        {
            Message reply = await binder.RequestAsync(message, timeout, maskingMode);

            if (reply != null)
            {
                await ProcessMessageAsync(reply);
            }
        }

        protected override WsrmFault ProcessRequestorResponse(ReliableRequestor requestor, string requestName, WsrmMessageInfo info)
        {
            string faultString = SRP.Format(SRP.ReceivedResponseBeforeRequestFaultString, requestName);
            string exceptionString = SRP.Format(SRP.ReceivedResponseBeforeRequestExceptionString, requestName);
            return SequenceTerminatedFault.CreateProtocolFault(ReliableSession.OutputID, faultString, exceptionString);
        }
    }

    internal sealed class ReliableOutputSessionChannelOverDuplex : ReliableOutputSessionChannel
    {
        public ReliableOutputSessionChannelOverDuplex(ChannelManagerBase factory, IReliableFactorySettings settings,
            IClientReliableChannelBinder binder, FaultHelper faultHelper,
            LateBoundChannelParameterCollection channelParameters)
            : base(factory, settings, binder, faultHelper, channelParameters)
        {
        }

        protected override bool RequestAcks
        {
            get
            {
                return true;
            }
        }

        protected override ReliableRequestor CreateRequestor()
        {
            return new SendWaitReliableRequestor();
        }

        protected override async Task OnConnectionSendAsync(Message message, TimeSpan timeout, bool saveHandledException, bool maskUnhandledException)
        {
            MaskingMode maskingMode = maskUnhandledException ? MaskingMode.Unhandled : MaskingMode.None;

            if (saveHandledException)
            {
                try
                {
                    await Binder.SendAsync(message, timeout, maskingMode);
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                        throw;

                    if (Binder.IsHandleable(e))
                    {
                        MaxRetryCountException = e;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                maskingMode |= MaskingMode.Handled;
                await Binder.SendAsync(message, timeout, maskingMode);
            }
        }

        protected override Task OnConnectionSendMessageAsync(Message message, TimeSpan timeout, MaskingMode maskingMode)
        {
            return Binder.SendAsync(message, timeout, maskingMode);
        }

        protected override void OnOpened()
        {
            base.OnOpened();

            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                try
                {
                    _ = StartReceivingAsync();
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                        throw;

                    ReliableSession.OnUnknownException(e);
                }
            }
            else
            {
                ActionItem.Schedule(new Func<object, Task>(StartReceivingAsync), this);
            }
        }

        protected override WsrmFault ProcessRequestorResponse(ReliableRequestor requestor, string requestName, WsrmMessageInfo info)
        {
            if (requestor != null)
            {
                requestor.SetInfo(info);
                return null;
            }
            else
            {
                string faultString = SRP.Format(SRP.ReceivedResponseBeforeRequestFaultString, requestName);
                string exceptionString = SRP.Format(SRP.ReceivedResponseBeforeRequestExceptionString, requestName);
                return SequenceTerminatedFault.CreateProtocolFault(ReliableSession.OutputID, faultString, exceptionString);
            }
        }

        private async Task StartReceivingAsync()
        {
            try
            {
                while (true)
                {
                    (bool success, RequestContext context) = await Binder.TryReceiveAsync(TimeSpan.MaxValue);
                    if (success)
                    {
                        if (context != null)
                        {
                            using (context)
                            {
                                Message requestMessage = context.RequestMessage;
                                await ProcessMessageAsync(requestMessage);
                                context.Close(DefaultCloseTimeout);
                            }
                        }
                        else
                        {
                            if (!Connection.Closed && (Binder.State == CommunicationState.Opened))
                            {
                                Exception e = new CommunicationException(SRP.EarlySecurityClose);
                                ReliableSession.OnLocalFault(e, (Message)null, null);
                            }

                            // Null context means channel is closing
                            break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                    throw;

                ReliableSession.OnUnknownException(e);
            }
        }

        private static Task StartReceivingAsync(object state)
        {
            ReliableOutputSessionChannelOverDuplex channel =
                (ReliableOutputSessionChannelOverDuplex)state;
            return channel.StartReceivingAsync();
        }
    }
}
