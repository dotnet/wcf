// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal sealed class ReliableRequestSessionChannel : RequestChannel, IRequestSessionChannel, IAsyncRequestChannel
    {
        private IClientReliableChannelBinder binder;
        private ChannelParameterCollection channelParameters;
        private ReliableRequestor closeRequestor;
        private ReliableOutputConnection connection;
        private bool isLastKnown = false;
        private Exception maxRetryCountException = null;
        private SequenceRangeCollection ranges = SequenceRangeCollection.Empty;
        private Guard replyAckConsistencyGuard;
        private ClientReliableSession session;
        private IReliableFactorySettings settings;
        private InterruptibleWaitObject shutdownHandle;
        private ReliableRequestor terminateRequestor;

        public ReliableRequestSessionChannel(
            ChannelManagerBase factory,
            IReliableFactorySettings settings,
            IClientReliableChannelBinder binder,
            FaultHelper faultHelper,
            LateBoundChannelParameterCollection channelParameters,
            UniqueId inputID)
            : base(factory, binder.RemoteAddress, binder.Via, true)
        {
            this.settings = settings;
            this.binder = binder;
            session = new ClientReliableSession(this, settings, binder, faultHelper, inputID);
            session.PollingCallback = PollingCallback;
            session.UnblockChannelCloseCallback = UnblockClose;

            if (this.settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                shutdownHandle = new InterruptibleWaitObject(false);
            }
            else
            {
                replyAckConsistencyGuard = new Guard(int.MaxValue);
            }

            this.binder.Faulted += OnBinderFaulted;
            this.binder.OnException += OnBinderException;

            this.channelParameters = channelParameters;
            channelParameters.SetChannel(this);
        }

        public IOutputSession Session
        {
            get
            {
                return session;
            }
        }

        private void AddAcknowledgementHeader(Message message, bool force)
        {
            if (ranges.Count == 0)
            {
                return;
            }

            WsrmUtilities.AddAcknowledgementHeader(settings.ReliableMessagingVersion, message,
                session.InputID, ranges, isLastKnown);
        }

        private async Task CloseSequenceAsync(TimeSpan timeout)
        {
            CreateCloseRequestor();
            Message closeReply = await closeRequestor.RequestAsync(timeout);
            ProcessCloseOrTerminateReply(true, closeReply);
        }

        private void ConfigureRequestor(ReliableRequestor requestor)
        {
            ReliableMessagingVersion reliableMessagingVersion = settings.ReliableMessagingVersion;
            requestor.MessageVersion = settings.MessageVersion;
            requestor.Binder = binder;
            requestor.SetRequestResponsePattern();
            requestor.MessageHeader = new WsrmAcknowledgmentHeader(reliableMessagingVersion, session.InputID,
                ranges, true, -1);
        }

        private Message CreateAckRequestedMessage()
        {
            Message request = WsrmUtilities.CreateAckRequestedMessage(settings.MessageVersion,
                settings.ReliableMessagingVersion, session.OutputID);
            AddAcknowledgementHeader(request, true);
            return request;
        }

        protected override IAsyncRequest CreateAsyncRequest(Message message)
        {
            return new AsyncRequest(this);
        }

        private void CreateCloseRequestor()
        {
            RequestReliableRequestor temp = new RequestReliableRequestor();

            ConfigureRequestor(temp);
            temp.TimeoutString1Index = SRP.TimeoutOnClose;
            temp.MessageAction = WsrmIndex.GetCloseSequenceActionHeader(
                settings.MessageVersion.Addressing);
            temp.MessageBody = new CloseSequence(session.OutputID, connection.Last);

            lock (ThisLock)
            {
                ThrowIfClosed();
                closeRequestor = temp;
            }
        }

        private void CreateTerminateRequestor()
        {
            RequestReliableRequestor temp = new RequestReliableRequestor();

            ConfigureRequestor(temp);
            temp.MessageAction = WsrmIndex.GetTerminateSequenceActionHeader(
                settings.MessageVersion.Addressing, settings.ReliableMessagingVersion);
            temp.MessageBody = new TerminateSequence(settings.ReliableMessagingVersion,
                session.OutputID, connection.Last);

            lock (ThisLock)
            {
                ThrowIfClosed();
                terminateRequestor = temp;
                session.CloseSession();
            }
        }

        private Exception GetInvalidAddException()
        {
            if (State == CommunicationState.Faulted)
                return GetTerminalException();
            else
                return CreateClosedException();
        }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(IRequestSessionChannel))
            {
                return (T)(object)this;
            }

            if (typeof(T) == typeof(ChannelParameterCollection))
            {
                return (T)(object)channelParameters;
            }

            T baseProperty = base.GetProperty<T>();

            if (baseProperty != null)
            {
                return baseProperty;
            }

            T innerProperty = binder.Channel.GetProperty<T>();
            if ((innerProperty == null) && (typeof(T) == typeof(FaultConverter)))
            {
                return (T)(object)FaultConverter.GetDefaultFaultConverter(settings.MessageVersion);
            }
            else
            {
                return innerProperty;
            }
        }

        protected override void OnAbort()
        {
            if (connection != null)
            {
                connection.Abort(this);
            }

            if (shutdownHandle != null)
            {
                shutdownHandle.Abort(this);
            }

            ReliableRequestor tempRequestor = closeRequestor;
            if (tempRequestor != null)
            {
                tempRequestor.Abort(this);
            }

            tempRequestor = terminateRequestor;
            if (tempRequestor != null)
            {
                tempRequestor.Abort(this);
            }

            session.Abort();
            base.OnAbort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnCloseAsync(timeout).ToApm(callback, state);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnOpenAsync(timeout).ToApm(callback, state);
        }

        private void OnBinderException(IReliableChannelBinder sender, Exception exception)
        {
            if (exception is QuotaExceededException)
            {
                if (State == CommunicationState.Opening ||
                    State == CommunicationState.Opened ||
                    State == CommunicationState.Closing)
                {
                    session.OnLocalFault(exception, SequenceTerminatedFault.CreateQuotaExceededFault(session.OutputID), null);
                }
            }
            else
            {
                AddPendingException(exception);
            }
        }

        private void OnBinderFaulted(IReliableChannelBinder sender, Exception exception)
        {
            binder.Abort();

            if (State == CommunicationState.Opening ||
                State == CommunicationState.Opened ||
                State == CommunicationState.Closing)
            {
                exception = new CommunicationException(SRP.EarlySecurityFaulted, exception);
                session.OnLocalFault(exception, (Message)null, null);
            }
        }

        protected internal override async Task OnCloseAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await connection.CloseAsync(timeoutHelper.RemainingTime());
            await WaitForShutdownAsync(timeoutHelper.RemainingTime());

            if (settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                await CloseSequenceAsync(timeoutHelper.RemainingTime());
            }

            await TerminateSequenceAsync(timeoutHelper.RemainingTime());
            await session.CloseAsync(timeoutHelper.RemainingTime());
            await binder.CloseAsync(timeoutHelper.RemainingTime(), MaskingMode.Handled);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TaskHelpers.WaitForCompletionNoSpin(OnCloseAsync(timeout));
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            binder.Faulted -= OnBinderFaulted;
        }

        private async Task OnConnectionSendAsync(MessageAttemptInfo attemptInfo, TimeSpan timeout, bool maskUnhandledException)
        {
            using (attemptInfo.Message)
            {
                if (attemptInfo.RetryCount > settings.MaxRetryCount)
                {
                    if (WcfEventSource.Instance.MaxRetryCyclesExceededIsEnabled())
                    {
                        WcfEventSource.Instance.MaxRetryCyclesExceeded(SRP.MaximumRetryCountExceeded);
                    }
                    session.OnLocalFault(new CommunicationException(SRP.MaximumRetryCountExceeded, maxRetryCountException),
                        SequenceTerminatedFault.CreateMaxRetryCountExceededFault(session.OutputID), null);
                    return;
                }

                AddAcknowledgementHeader(attemptInfo.Message, false);
                session.OnLocalActivity();

                Message reply = null;
                MaskingMode maskingMode = maskUnhandledException ? MaskingMode.Unhandled : MaskingMode.None;

                if (attemptInfo.RetryCount < settings.MaxRetryCount)
                {
                    maskingMode |= MaskingMode.Handled;
                    reply = await binder.RequestAsync(attemptInfo.Message, timeout, maskingMode);
                }
                else
                {
                    try
                    {
                        reply = await binder.RequestAsync(attemptInfo.Message, timeout, maskingMode);
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                            throw;

                        if (binder.IsHandleable(e))
                        {
                            maxRetryCountException = e;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                if (reply != null)
                    ProcessReply(reply, (IReliableRequest)attemptInfo.State, attemptInfo.GetSequenceNumber());
            }
        }

        private Task OnConnectionSendAckAsyncRequested(TimeSpan timeout)
        {
            return Task.CompletedTask;
            // do nothing, only replies to sequence messages alter the state of the reliable output connection
        }

        private void OnComponentFaulted(Exception faultException, WsrmFault fault)
        {
            session.OnLocalFault(faultException, fault, null);
        }

        private void OnComponentException(Exception exception)
        {
            session.OnUnknownException(exception);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected override void OnFaulted()
        {
            session.OnFaulted();
            UnblockClose();
            base.OnFaulted();
        }

        protected internal override async Task OnOpenAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            bool throwing = true;

            try
            {
                await binder.OpenAsync(timeoutHelper.RemainingTime());
                await session.OpenAsync(timeoutHelper.RemainingTime());
                throwing = false;
            }
            finally
            {
                if (throwing)
                {
                    await binder.CloseAsync(timeoutHelper.RemainingTime());
                }
            }
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            TaskHelpers.WaitForCompletionNoSpin(OnOpenAsync(timeout));
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            connection = new ReliableOutputConnection(session.OutputID, settings.MaxTransferWindowSize,
                settings.MessageVersion, settings.ReliableMessagingVersion, session.InitiationTime,
                false, DefaultSendTimeout);
            connection.Faulted += OnComponentFaulted;
            connection.OnException += OnComponentException;
            connection.SendAsyncHandler = OnConnectionSendAsync;
            connection.SendAckRequestedAsyncHandler = OnConnectionSendAckAsyncRequested;
        }

        private async Task PollingCallback()
        {
            var message = CreateAckRequestedMessage();
            var reply = await binder.RequestAsync(message, DefaultSendTimeout, MaskingMode.All);
            if (reply != null)
            {
                ProcessReply(reply, null, 0);
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

            ReliableMessagingVersion reliableMessagingVersion = settings.ReliableMessagingVersion;

            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                if (close)
                {
                    throw Fx.AssertAndThrow("Close does not exist in Feb2005.");
                }

                reply.Close();
            }
            else if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                WsrmMessageInfo info = closeRequestor.GetInfo();

                // Close - Final ack made it.
                // Terminate - UnknownSequence.
                // Either way, message has been verified and does not belong to this thread.
                if (info != null)
                {
                    return;
                }

                try
                {
                    info = WsrmMessageInfo.Get(settings.MessageVersion, reliableMessagingVersion,
                        binder.Channel, binder.GetInnerSession(), reply);
                    session.ProcessInfo(info, null, true);
                    session.VerifyDuplexProtocolElements(info, null, true);

                    WsrmFault fault = close
                        ? WsrmUtilities.ValidateCloseSequenceResponse(session, closeRequestor.MessageId, info,
                        connection.Last)
                        : WsrmUtilities.ValidateTerminateSequenceResponse(session, terminateRequestor.MessageId,
                        info, connection.Last);

                    if (fault != null)
                    {
                        session.OnLocalFault(null, fault, null);
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(fault.CreateException());
                    }
                }
                finally
                {
                    reply.Close();
                }
            }
            else
            {
                throw Fx.AssertAndThrow("Reliable messaging version not supported.");
            }
        }

        private void ProcessReply(Message reply, IReliableRequest request, long requestSequenceNumber)
        {
            WsrmMessageInfo messageInfo = WsrmMessageInfo.Get(settings.MessageVersion,
                settings.ReliableMessagingVersion, binder.Channel, binder.GetInnerSession(), reply);

            if (!session.ProcessInfo(messageInfo, null))
                return;

            if (!session.VerifyDuplexProtocolElements(messageInfo, null))
                return;

            bool wsrm11 = settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11;

            if (messageInfo.WsrmHeaderFault != null)
            {
                // Close message now, going to stop processing in all cases.
                messageInfo.Message.Close();

                if (!(messageInfo.WsrmHeaderFault is UnknownSequenceFault))
                {
                    throw Fx.AssertAndThrow("Fault must be UnknownSequence fault.");
                }

                if (terminateRequestor == null)
                {
                    throw Fx.AssertAndThrow("If we start getting UnknownSequence, terminateRequestor cannot be null.");
                }

                terminateRequestor.SetInfo(messageInfo);

                return;
            }

            if (messageInfo.AcknowledgementInfo == null)
            {
                WsrmFault fault = SequenceTerminatedFault.CreateProtocolFault(session.InputID,
                    SRP.SequenceTerminatedReplyMissingAcknowledgement,
                    SRP.ReplyMissingAcknowledgement);
                messageInfo.Message.Close();
                session.OnLocalFault(fault.CreateException(), fault, null);
                return;
            }

            if (wsrm11 && (messageInfo.TerminateSequenceInfo != null))
            {
                UniqueId faultId = (messageInfo.TerminateSequenceInfo.Identifier == session.OutputID)
                    ? session.InputID
                    : session.OutputID;

                WsrmFault fault = SequenceTerminatedFault.CreateProtocolFault(faultId,
                    SRP.SequenceTerminatedUnsupportedTerminateSequence,
                    SRP.UnsupportedTerminateSequenceExceptionString);
                messageInfo.Message.Close();
                session.OnLocalFault(fault.CreateException(), fault, null);
                return;
            }
            else if (wsrm11 && messageInfo.AcknowledgementInfo.Final)
            {
                // Close message now, going to stop processing in all cases.
                messageInfo.Message.Close();

                if (closeRequestor == null)
                {
                    // Remote endpoint signaled Close, this is not allowed so we fault.
                    string exceptionString = SRP.UnsupportedCloseExceptionString;
                    string faultString = SRP.SequenceTerminatedUnsupportedClose;

                    WsrmFault fault = SequenceTerminatedFault.CreateProtocolFault(session.OutputID, faultString,
                        exceptionString);
                    session.OnLocalFault(fault.CreateException(), fault, null);
                }
                else
                {
                    WsrmFault fault = WsrmUtilities.ValidateFinalAck(session, messageInfo, connection.Last);

                    if (fault == null)
                    {
                        // Received valid final ack after sending Close, inform the close thread.
                        closeRequestor.SetInfo(messageInfo);
                    }
                    else
                    {
                        // Received invalid final ack after sending Close, fault.
                        session.OnLocalFault(fault.CreateException(), fault, null);
                    }
                }

                return;
            }

            int bufferRemaining = -1;

            if (settings.FlowControlEnabled)
                bufferRemaining = messageInfo.AcknowledgementInfo.BufferRemaining;

            // We accept no more than MaxSequenceRanges ranges to limit the serialized ack size and
            // the amount of memory taken up by the ack ranges. Since request reply uses the presence of
            // a reply as an acknowledgement we cannot call ProcessTransferred (which stops retrying the
            // request) if we intend to drop the message. This means the limit is not strict since we do
            // not check for the limit and merge the ranges atomically. The limit + the number of
            // concurrent threads is a sufficient mitigation.
            if ((messageInfo.SequencedMessageInfo != null) &&
                !ReliableInputConnection.CanMerge(messageInfo.SequencedMessageInfo.SequenceNumber, ranges))
            {
                messageInfo.Message.Close();
                return;
            }

            bool exitGuard = replyAckConsistencyGuard != null ? replyAckConsistencyGuard.Enter() : false;

            try
            {
                connection.ProcessTransferred(requestSequenceNumber,
                    messageInfo.AcknowledgementInfo.Ranges, bufferRemaining);

                session.OnRemoteActivity(connection.Strategy.QuotaRemaining == 0);

                if (messageInfo.SequencedMessageInfo != null)
                {
                    lock (ThisLock)
                    {
                        ranges = ranges.MergeWith(messageInfo.SequencedMessageInfo.SequenceNumber);
                    }
                }
            }
            finally
            {
                if (exitGuard)
                {
                    replyAckConsistencyGuard.Exit();
                }
            }

            if (request != null)
            {
                if (WsrmUtilities.IsWsrmAction(settings.ReliableMessagingVersion, messageInfo.Action))
                {
                    messageInfo.Message.Close();
                    request.Set(null);
                }
                else
                {
                    request.Set(messageInfo.Message);
                }
            }

            // The termination mechanism in the TerminateSequence fails with RequestReply.
            // Since the ack ranges are updated after ProcessTransferred is called and
            // ProcessTransferred potentially signals the Termination process, this channel 
            // winds up sending a message with the ack for last message missing.
            // Thus we send the termination after we update the ranges.

            if ((shutdownHandle != null) && connection.CheckForTermination())
            {
                shutdownHandle.Set();
            }

            if (request != null)
                request.Complete();
        }

        private async Task TerminateSequenceAsync(TimeSpan timeout)
        {
            CreateTerminateRequestor();
            Message terminateReply = await terminateRequestor.RequestAsync(timeout);

            if (terminateReply != null)
            {
                ProcessCloseOrTerminateReply(false, terminateReply);
            }
        }

        private void UnblockClose()
        {
            FaultPendingRequests();

            if (connection != null)
            {
                connection.Fault(this);
            }

            if (shutdownHandle != null)
            {
                shutdownHandle.Fault(this);
            }

            ReliableRequestor tempRequestor = closeRequestor;
            if (tempRequestor != null)
            {
                tempRequestor.Fault(this);
            }

            tempRequestor = terminateRequestor;
            if (tempRequestor != null)
            {
                tempRequestor.Fault(this);
            }
        }

        private Task WaitForShutdownAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            if (settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                return shutdownHandle.WaitAsync(timeoutHelper.RemainingTime());
            }
            else
            {
                isLastKnown = true;

                // We already closed the connection so we know everything was acknowledged.
                // Make sure the reply acknowledgement ranges are current.
                return replyAckConsistencyGuard.CloseAsync(timeoutHelper.RemainingTime());
            }
        }

        private interface IReliableRequest : IRequestBase
        {
            void Set(Message reply);
            void Complete();
        }

        private class AsyncRequest : IReliableRequest, IAsyncRequest
        {
            private bool _aborted = false;
            private bool _completed = false;
            private TaskCompletionSource<object> _tcs;
            private bool _faulted = false;
            private TimeSpan _originalTimeout;
            private Message _reply;
            private ReliableRequestSessionChannel _parent;

            public AsyncRequest(ReliableRequestSessionChannel parent)
            {
                _parent = parent;
            }

            private object ThisLock { get; } = new object();

            public void Abort(RequestChannel channel)
            {
                lock (ThisLock)
                {
                    if (!_completed)
                    {
                        _aborted = true;
                        _completed = true;
                        Fx.Assert(_tcs?.Task == null || !_tcs.Task.IsCompleted, "Task should be null or not already be completed");
                        _tcs?.SetResult(null);
                    }
                }
            }

            public void Fault(RequestChannel channel)
            {
                lock (ThisLock)
                {
                    if (!_completed)
                    {
                        _faulted = true;
                        _completed = true;
                        Fx.Assert(_tcs?.Task == null || !_tcs.Task.IsCompleted, "Task should be null or not already be completed");
                        _tcs?.SetResult(null);
                    }
                }
            }

            public void Complete()
            {
            }

            public async Task SendRequestAsync(Message message, TimeoutHelper timeoutHelper)
            {
                _originalTimeout = timeoutHelper.OriginalTimeout;
                if (!await _parent.connection.AddMessageAsync(message, timeoutHelper.RemainingTime(), this))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(_parent.GetInvalidAddException());
            }

            public void Set(Message reply)
            {
                lock (ThisLock)
                {
                    if (!_completed)
                    {
                        _reply = reply;
                        _completed = true;
                        Fx.Assert(_tcs?.Task == null || !_tcs.Task.IsCompleted, "Task should be null or not already be completed");
                        _tcs?.SetResult(null);
                        return;
                    }
                }
                if (reply != null)
                {
                    reply.Close();
                }
            }

            public async Task<Message> ReceiveReplyAsync(TimeoutHelper timeoutHelper)
            {
                bool throwing = true;

                try
                {
                    bool expired = false;

                    if (!_completed)
                    {
                        bool wait = false;

                        lock (ThisLock)
                        {
                            if (!_completed)
                            {
                                wait = true;
                                _tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
                            }
                        }

                        if (wait)
                        {
                            expired = !await TaskHelpers.AwaitWithTimeout(_tcs.Task, timeoutHelper.RemainingTime());

                            lock (ThisLock)
                            {
                                if (!_completed)
                                {
                                    _completed = true;
                                }
                                else
                                {
                                    expired = false;
                                }
                            }
                        }
                    }

                    if (_aborted)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(_parent.CreateClosedException());
                    }
                    else if (_faulted)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(_parent.GetTerminalException());
                    }
                    else if (expired)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(SRP.Format(SRP.TimeoutOnRequest, _originalTimeout)));
                    }
                    else
                    {
                        throwing = false;
                        return _reply;
                    }
                }
                finally
                {
                    if (throwing)
                    {
                        WsrmFault fault = SequenceTerminatedFault.CreateCommunicationFault(_parent.session.InputID,
                            SRP.SequenceTerminatedReliableRequestThrew, null);
                        _parent.session.OnLocalFault(null, fault, null);
                    }
                }
            }

            public void OnReleaseRequest()
            {
            }
        }
    }
}
