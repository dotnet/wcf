// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal abstract class ReliableDuplexSessionChannel : DuplexChannel, IDuplexSessionChannel, IAsyncDuplexSessionChannel
    {
        private bool _acknowledgementScheduled = false;
        private IOThreadTimer _acknowledgementTimer;
        private ulong _ackVersion = 1;
        private bool _advertisedZero = false;
        private InterruptibleWaitObject _closeOutputWaitObject;
        private SendWaitReliableRequestor _closeRequestor;
        private DeliveryStrategy<Message> _deliveryStrategy;
        private Guard _guard = new Guard(int.MaxValue);
        private ReliableInputConnection _inputConnection;
        private Exception _maxRetryCountException = null;
        private int _pendingAcknowledgements = 0;
        private SendWaitReliableRequestor _terminateRequestor;
        private static Action<object> s_processMessageStatic = new Action<object>(ProcessMessageStatic);
        protected static Func<object, Task> s_startReceivingAsyncStatic = new Func<object, Task>(StartReceivingAsyncStatic);

        protected ReliableDuplexSessionChannel(ChannelManagerBase manager, IReliableFactorySettings settings, IReliableChannelBinder binder)
            : base(manager, binder.LocalAddress)
        {
            Binder = binder;
            Settings = settings;
            _acknowledgementTimer = new IOThreadTimer(new Func<object, Task>(OnAcknowledgementTimeoutElapsedAsync), null, true);
            Binder.Faulted += OnBinderFaulted;
            Binder.OnException += OnBinderException;
        }

        public IReliableChannelBinder Binder { get; }

        public override EndpointAddress LocalAddress => Binder.LocalAddress;

        protected ReliableOutputConnection OutputConnection { get; private set; }

        protected UniqueId OutputID => ReliableSession.OutputID;

        protected ChannelReliableSession ReliableSession { get; private set; }

        public override EndpointAddress RemoteAddress => Binder.RemoteAddress;

        protected IReliableFactorySettings Settings { get; }

        public override Uri Via => RemoteAddress.Uri;

        public IDuplexSession Session => (IDuplexSession)ReliableSession;

        IAsyncDuplexSession ISessionChannel<IAsyncDuplexSession>.Session => (IAsyncDuplexSession)ReliableSession;

        private void AddPendingAcknowledgements(Message message)
        {
            using(ThisAsyncLock.TakeLock())
            {
                if (_pendingAcknowledgements > 0)
                {
                    _acknowledgementTimer.Cancel();
                    _acknowledgementScheduled = false;
                    _pendingAcknowledgements = 0;
                    _ackVersion++;

                    int bufferRemaining = GetBufferRemaining();

                    WsrmUtilities.AddAcknowledgementHeader(
                        Settings.ReliableMessagingVersion,
                        message,
                        ReliableSession.InputID,
                        _inputConnection.Ranges,
                        _inputConnection.IsLastKnown,
                        bufferRemaining);
                }
            }
        }

        private Task CloseSequenceAsync(TimeSpan timeout)
        {
            CreateCloseRequestor();
            return _closeRequestor.RequestAsync(timeout);
            // reply came from receive loop, receive loop owns verified message so nothing more to do.
        }

        private void ConfigureRequestor(ReliableRequestor requestor)
        {
            requestor.MessageVersion = Settings.MessageVersion;
            requestor.Binder = Binder;
            requestor.SetRequestResponsePattern();
        }

        private Message CreateAcknowledgmentMessage()
        {
            using(ThisAsyncLock.TakeLock())
                _ackVersion++;

            int bufferRemaining = GetBufferRemaining();

            Message message = WsrmUtilities.CreateAcknowledgmentMessage(Settings.MessageVersion,
                Settings.ReliableMessagingVersion, ReliableSession.InputID, _inputConnection.Ranges,
                _inputConnection.IsLastKnown, bufferRemaining);

            if (WcfEventSource.Instance.SequenceAcknowledgementSentIsEnabled())
            {
                WcfEventSource.Instance.SequenceAcknowledgementSent(ReliableSession.Id);
            }

            return message;
        }

        private void CreateCloseRequestor()
        {
            SendWaitReliableRequestor temp = new SendWaitReliableRequestor();

            ConfigureRequestor(temp);
            temp.TimeoutString1Index = SRP.TimeoutOnClose;
            temp.MessageAction = WsrmIndex.GetCloseSequenceActionHeader(
                Settings.MessageVersion.Addressing);
            temp.MessageBody = new CloseSequence(ReliableSession.OutputID, OutputConnection.Last);

            using(ThisAsyncLock.TakeLock())
            {
                ThrowIfClosed();
                _closeRequestor = temp;
            }
        }

        private void CreateTerminateRequestor()
        {
            SendWaitReliableRequestor temp = new SendWaitReliableRequestor();

            ConfigureRequestor(temp);
            ReliableMessagingVersion reliableMessagingVersion = Settings.ReliableMessagingVersion;
            temp.MessageAction = WsrmIndex.GetTerminateSequenceActionHeader(
                Settings.MessageVersion.Addressing, reliableMessagingVersion);
            temp.MessageBody = new TerminateSequence(reliableMessagingVersion, ReliableSession.OutputID,
                OutputConnection.Last);

            using(ThisAsyncLock.TakeLock())
            {
                ThrowIfClosed();
                _terminateRequestor = temp;

                if (_inputConnection.IsLastKnown)
                {
                    ReliableSession.CloseSession();
                }
            }
        }

        private int GetBufferRemaining()
        {
            int bufferRemaining = -1;

            if (Settings.FlowControlEnabled)
            {
                bufferRemaining = Settings.MaxTransferWindowSize - _deliveryStrategy.EnqueuedCount;
                _advertisedZero = (bufferRemaining == 0);
            }

            return bufferRemaining;
        }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(IDuplexSessionChannel))
            {
                return (T)(object)this;
            }

            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            T innerProperty = Binder.Channel.GetProperty<T>();
            if ((innerProperty == null) && (typeof(T) == typeof(FaultConverter)))
            {
                return (T)(object)FaultConverter.GetDefaultFaultConverter(Settings.MessageVersion);
            }
            else
            {
                return innerProperty;
            }
        }

        private async Task InternalCloseOutputSessionAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await OutputConnection.CloseAsync(timeoutHelper.RemainingTime());

            if (Settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                await CloseSequenceAsync(timeoutHelper.RemainingTime());
            }

            await TerminateSequenceAsync(timeoutHelper.RemainingTime());
        }

        protected virtual void OnRemoteActivity()
        {
            ReliableSession.OnRemoteActivity(false);
        }

        private WsrmFault ProcessCloseOrTerminateSequenceResponse(bool close, WsrmMessageInfo info)
        {
            SendWaitReliableRequestor requestor = close ? _closeRequestor : _terminateRequestor;

            if (requestor != null)
            {
                WsrmFault fault = close
                    ? WsrmUtilities.ValidateCloseSequenceResponse(ReliableSession, _closeRequestor.MessageId, info,
                    OutputConnection.Last)
                    : WsrmUtilities.ValidateTerminateSequenceResponse(ReliableSession, _terminateRequestor.MessageId,
                    info, OutputConnection.Last);

                if (fault != null)
                {
                    return fault;
                }

                requestor.SetInfo(info);
                return null;
            }

            string request = close ? Wsrm11Strings.CloseSequence : WsrmFeb2005Strings.TerminateSequence;
            string faultString = SRP.Format(SRP.ReceivedResponseBeforeRequestFaultString, request);
            string exceptionString = SRP.Format(SRP.ReceivedResponseBeforeRequestExceptionString, request);
            return SequenceTerminatedFault.CreateProtocolFault(ReliableSession.OutputID, faultString, exceptionString);
        }

        protected async Task ProcessDuplexMessageAsync(WsrmMessageInfo info)
        {
            bool closeMessage = true;

            try
            {
                bool wsrmFeb2005 = Settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005;
                bool wsrm11 = Settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11;
                bool final = false;

                if (OutputConnection != null && info.AcknowledgementInfo != null)
                {
                    final = wsrm11 && info.AcknowledgementInfo.Final;

                    int bufferRemaining = -1;

                    if (Settings.FlowControlEnabled)
                        bufferRemaining = info.AcknowledgementInfo.BufferRemaining;

                    OutputConnection.ProcessTransferred(info.AcknowledgementInfo.Ranges, bufferRemaining);
                }

                OnRemoteActivity();

                bool tryAckNow = (info.AckRequestedInfo != null);
                bool forceAck = false;
                bool terminate = false;
                bool scheduleShutdown = false;
                ulong oldAckVersion = 0;
                WsrmFault fault = null;
                Message message = null;
                Exception remoteFaultException = null;

                if (info.SequencedMessageInfo != null)
                {
                    bool needDispatch = false;

                    using (ThisAsyncLock.TakeLock())
                    {
                        if (Aborted || State == CommunicationState.Faulted)
                        {
                            return;
                        }

                        long sequenceNumber = info.SequencedMessageInfo.SequenceNumber;
                        bool isLast = wsrmFeb2005 && info.SequencedMessageInfo.LastMessage;

                        if (!_inputConnection.IsValid(sequenceNumber, isLast))
                        {
                            if (wsrmFeb2005)
                            {
                                fault = new LastMessageNumberExceededFault(ReliableSession.InputID);
                            }
                            else
                            {
                                message = new SequenceClosedFault(ReliableSession.InputID).CreateMessage(
                                    Settings.MessageVersion, Settings.ReliableMessagingVersion);
                                forceAck = true;

                                OnMessageDropped();
                            }
                        }
                        else if (_inputConnection.Ranges.Contains(sequenceNumber))
                        {
                            OnMessageDropped();
                            tryAckNow = true;
                        }
                        else if (wsrmFeb2005 && info.Action == WsrmFeb2005Strings.LastMessageAction)
                        {
                            _inputConnection.Merge(sequenceNumber, isLast);

                            if (_inputConnection.AllAdded)
                            {
                                scheduleShutdown = true;

                                if (OutputConnection.CheckForTermination())
                                {
                                    ReliableSession.CloseSession();
                                }
                            }
                        }
                        else if (State == CommunicationState.Closing)
                        {
                            if (wsrmFeb2005)
                            {
                                fault = SequenceTerminatedFault.CreateProtocolFault(ReliableSession.InputID,
                                    SRP.SequenceTerminatedSessionClosedBeforeDone,
                                    SRP.SessionClosedBeforeDone);
                            }
                            else
                            {
                                message = new SequenceClosedFault(ReliableSession.InputID).CreateMessage(
                                    Settings.MessageVersion, Settings.ReliableMessagingVersion);
                                forceAck = true;

                                OnMessageDropped();
                            }
                        }
                        // In the unordered case we accept no more than MaxSequenceRanges ranges to limit the
                        // serialized ack size and the amount of memory taken by the ack ranges. In the
                        // ordered case, the delivery strategy MaxTransferWindowSize quota mitigates this
                        // threat.
                        else if (_deliveryStrategy.CanEnqueue(sequenceNumber)
                            && (Settings.Ordered || _inputConnection.CanMerge(sequenceNumber)))
                        {
                            _inputConnection.Merge(sequenceNumber, isLast);
                            needDispatch = _deliveryStrategy.Enqueue(info.Message, sequenceNumber);
                            closeMessage = false;
                            oldAckVersion = _ackVersion;
                            _pendingAcknowledgements++;

                            if (_inputConnection.AllAdded)
                            {
                                scheduleShutdown = true;

                                if (OutputConnection.CheckForTermination())
                                {
                                    ReliableSession.CloseSession();
                                }
                            }
                        }
                        else
                        {
                            OnMessageDropped();
                        }

                        // if (ack now && we enqueued && an ack has been sent since we enqueued (and thus
                        // carries the sequence number of the message we just processed)) then we don't
                        // need to ack again.
                        if (_inputConnection.IsLastKnown || _pendingAcknowledgements == Settings.MaxTransferWindowSize)
                            tryAckNow = true;

                        bool startTimer = tryAckNow || (_pendingAcknowledgements > 0 && fault == null);
                        if (startTimer && !_acknowledgementScheduled)
                        {
                            _acknowledgementScheduled = true;
                            _acknowledgementTimer.Set(Settings.AcknowledgementInterval);
                        }
                    }

                    if (needDispatch)
                    {
                        Dispatch();
                    }
                }
                else if (wsrmFeb2005 && info.TerminateSequenceInfo != null)
                {
                    bool isTerminateEarly;

                    using (ThisAsyncLock.TakeLock())
                    {
                        isTerminateEarly = !_inputConnection.Terminate();
                    }

                    if (isTerminateEarly)
                    {
                        fault = SequenceTerminatedFault.CreateProtocolFault(ReliableSession.InputID,
                            SRP.SequenceTerminatedEarlyTerminateSequence,
                            SRP.EarlyTerminateSequence);
                    }
                }
                else if (wsrm11)
                {
                    if (((info.TerminateSequenceInfo != null) && (info.TerminateSequenceInfo.Identifier == ReliableSession.InputID))
                        || (info.CloseSequenceInfo != null))
                    {
                        bool isTerminate = info.TerminateSequenceInfo != null;
                        WsrmRequestInfo requestInfo = isTerminate
                            ? (WsrmRequestInfo)info.TerminateSequenceInfo
                            : (WsrmRequestInfo)info.CloseSequenceInfo;
                        long last = isTerminate ? info.TerminateSequenceInfo.LastMsgNumber : info.CloseSequenceInfo.LastMsgNumber;

                        if (!WsrmUtilities.ValidateWsrmRequest(ReliableSession, requestInfo, Binder, null))
                        {
                            return;
                        }

                        bool isLastLargeEnough = true;
                        bool isLastConsistent = true;

                        using (ThisAsyncLock.TakeLock())
                        {
                            if (!_inputConnection.IsLastKnown)
                            {
                                if (isTerminate)
                                {
                                    if (_inputConnection.SetTerminateSequenceLast(last, out isLastLargeEnough))
                                    {
                                        scheduleShutdown = true;
                                    }
                                    else if (isLastLargeEnough)
                                    {
                                        remoteFaultException = new ProtocolException(SRP.EarlyTerminateSequence);
                                    }
                                }
                                else
                                {
                                    scheduleShutdown = _inputConnection.SetCloseSequenceLast(last);
                                    isLastLargeEnough = scheduleShutdown;
                                }

                                if (scheduleShutdown)
                                {
                                    ReliableSession.SetFinalAck(_inputConnection.Ranges);
                                    if (_terminateRequestor != null)
                                    {
                                        ReliableSession.CloseSession();
                                    }

                                    _deliveryStrategy.Dispose();
                                }
                            }
                            else
                            {
                                isLastConsistent = (last == _inputConnection.Last);

                                // Have seen CloseSequence already, TerminateSequence means cleanup.
                                if (isTerminate && isLastConsistent && _inputConnection.IsSequenceClosed)
                                {
                                    terminate = true;
                                }
                            }
                        }

                        if (!isLastLargeEnough)
                        {
                            string faultString = SRP.SequenceTerminatedSmallLastMsgNumber;
                            string exceptionString = SRP.SmallLastMsgNumberExceptionString;
                            fault = SequenceTerminatedFault.CreateProtocolFault(ReliableSession.InputID, faultString, exceptionString);
                        }
                        else if (!isLastConsistent)
                        {
                            string faultString = SRP.SequenceTerminatedInconsistentLastMsgNumber;
                            string exceptionString = SRP.InconsistentLastMsgNumberExceptionString;
                            fault = SequenceTerminatedFault.CreateProtocolFault(ReliableSession.InputID, faultString, exceptionString);
                        }
                        else
                        {
                            message = isTerminate
                                ? WsrmUtilities.CreateTerminateResponseMessage(Settings.MessageVersion,
                                requestInfo.MessageId, ReliableSession.InputID)
                                : WsrmUtilities.CreateCloseSequenceResponse(Settings.MessageVersion,
                                requestInfo.MessageId, ReliableSession.InputID);
                            forceAck = true;
                        }
                    }
                    else if (info.TerminateSequenceInfo != null)    // Identifier == OutputID
                    {
                        fault = SequenceTerminatedFault.CreateProtocolFault(ReliableSession.InputID,
                            SRP.SequenceTerminatedUnsupportedTerminateSequence,
                            SRP.UnsupportedTerminateSequenceExceptionString);
                    }
                    else if (info.TerminateSequenceResponseInfo != null)
                    {
                        fault = ProcessCloseOrTerminateSequenceResponse(false, info);
                    }
                    else if (info.CloseSequenceResponseInfo != null)
                    {
                        fault = ProcessCloseOrTerminateSequenceResponse(true, info);
                    }
                    else if (final)
                    {
                        if (_closeRequestor == null)
                        {
                            string exceptionString = SRP.UnsupportedCloseExceptionString;
                            string faultString = SRP.SequenceTerminatedUnsupportedClose;

                            fault = SequenceTerminatedFault.CreateProtocolFault(ReliableSession.OutputID, faultString,
                                exceptionString);
                        }
                        else
                        {
                            fault = WsrmUtilities.ValidateFinalAck(ReliableSession, info, OutputConnection.Last);

                            if (fault == null)
                            {
                                _closeRequestor.SetInfo(info);
                            }
                        }
                    }
                    else if (info.WsrmHeaderFault != null)
                    {
                        if (!(info.WsrmHeaderFault is UnknownSequenceFault))
                        {
                            throw Fx.AssertAndThrow("Fault must be UnknownSequence fault.");
                        }

                        if (_terminateRequestor == null)
                        {
                            throw Fx.AssertAndThrow("In wsrm11, if we start getting UnknownSequence, terminateRequestor cannot be null.");
                        }

                        _terminateRequestor.SetInfo(info);
                    }
                }

                if (fault != null)
                {
                    ReliableSession.OnLocalFault(fault.CreateException(), fault, null);
                    return;
                }

                if (scheduleShutdown)
                {
                    ActionItem.Schedule(ShutdownCallback, null);
                }

                if (message != null)
                {
                    if (forceAck)
                    {
                        WsrmUtilities.AddAcknowledgementHeader(Settings.ReliableMessagingVersion, message,
                            ReliableSession.InputID, _inputConnection.Ranges, true, GetBufferRemaining());
                    }
                    else if (tryAckNow)
                    {
                        AddPendingAcknowledgements(message);
                    }
                }
                else if (tryAckNow)
                {
                    using (ThisAsyncLock.TakeLock())
                    {
                        if (oldAckVersion != 0 && oldAckVersion != _ackVersion)
                            return;

                        if (_acknowledgementScheduled)
                        {
                            _acknowledgementTimer.Cancel();
                            _acknowledgementScheduled = false;
                        }
                        _pendingAcknowledgements = 0;
                    }

                    message = CreateAcknowledgmentMessage();
                }

                if (message != null)
                {
                    using (message)
                    {
                        if (_guard.Enter())
                        {
                            try
                            {
                                await Binder.SendAsync(message, DefaultSendTimeout);
                            }
                            finally
                            {
                                _guard.Exit();
                            }
                        }
                    }
                }

                if (terminate)
                {
                    using (ThisAsyncLock.TakeLock())
                    {
                        _inputConnection.Terminate();
                    }
                }

                if (remoteFaultException != null)
                {
                    ReliableSession.OnRemoteFault(remoteFaultException);
                }
            }
            finally
            {
                if (closeMessage)
                {
                    info.Message.Close();
                }
            }
        }

        private static void ProcessMessageStatic(object state)
        {
            (ReliableDuplexSessionChannel channel, WsrmMessageInfo info) = (ValueTuple<ReliableDuplexSessionChannel, WsrmMessageInfo>)state;
            _ = channel.ProcessMessageAsync(info);
        }

        protected abstract Task ProcessMessageAsync(WsrmMessageInfo info);

        protected override void OnAbort()
        {
            if (OutputConnection != null)
                OutputConnection.Abort(this);

            if (_inputConnection != null)
                _inputConnection.Abort(this);

            _guard.Abort();

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

            ReliableSession.Abort();
        }

        private async Task OnAcknowledgementTimeoutElapsedAsync(object state)
        {
            await using (await ThisAsyncLock.TakeLockAsync())
            {
                _acknowledgementScheduled = false;
                _pendingAcknowledgements = 0;

                if (State == CommunicationState.Closing
                    || State == CommunicationState.Closed
                    || State == CommunicationState.Faulted)
                    return;
            }

            if (_guard.Enter())
            {
                try
                {
                    using (Message message = CreateAcknowledgmentMessage())
                    {
                        await Binder.SendAsync(message, DefaultSendTimeout);
                    }
                }
                finally
                {
                    _guard.Exit();
                }
            }
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnCloseAsync(timeout).ToApm(callback, state);
        }

        private void OnBinderException(IReliableChannelBinder sender, Exception exception)
        {
            if (exception is QuotaExceededException)
            {
                if (State == CommunicationState.Opening ||
                    State == CommunicationState.Opened ||
                    State == CommunicationState.Closing)
                {
                    ReliableSession.OnLocalFault(exception, SequenceTerminatedFault.CreateQuotaExceededFault(ReliableSession.OutputID), null);
                }
            }
            else
            {
                EnqueueAndDispatch(exception, null, false);
            }
        }

        private void OnBinderFaulted(IReliableChannelBinder sender, Exception exception)
        {
            Binder.Abort();

            if (State == CommunicationState.Opening ||
                State == CommunicationState.Opened ||
                State == CommunicationState.Closing)
            {
                exception = new CommunicationException(SRP.EarlySecurityFaulted, exception);
                ReliableSession.OnLocalFault(exception, (Message)null, null);
            }
        }

        // CloseOutputSession && Close: CloseOutputSession only closes the ReliableOutputConnection
        // from the Opened state, if it does, it must create the closeOutputWaitObject so that
        // close may properly synchronize. If no closeOutputWaitObject is present, Close may close
        // the ReliableOutputConnection safely since it is in the Closing state.
        protected internal override async Task OnCloseAsync(TimeSpan timeout)
        {
            ThrowIfCloseInvalid();
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            if (OutputConnection != null)
            {
                if (_closeOutputWaitObject != null)
                {
                    await _closeOutputWaitObject.WaitAsync(timeoutHelper.RemainingTime());
                }
                else
                {
                    await InternalCloseOutputSessionAsync(timeoutHelper.RemainingTime());
                }

                await _inputConnection.CloseAsync(timeoutHelper.RemainingTime());
            }

            await _guard.CloseAsync(timeoutHelper.RemainingTime());
            await ReliableSession.CloseAsync(timeoutHelper.RemainingTime());
            await Binder.CloseAsync(timeoutHelper.RemainingTime(), MaskingMode.Handled);
            await base.OnCloseAsync(timeoutHelper.RemainingTime());
        }


        // CloseOutputSession && Close: CloseOutputSession only closes the ReliableOutputConnection
        // from the Opened state, if it does, it must create the closeOutputWaitObject so that
        // close may properly synchronize. If no closeOutputWaitObject is present, Close may close
        // the ReliableOutputConnection safely since it is in the Closing state.
        protected override void OnClose(TimeSpan timeout)
        {
            TaskHelpers.WaitForCompletionNoSpin(OnCloseAsync(timeout));
        }

        protected async Task OnCloseOutputSessionAsync(TimeSpan timeout)
        {
            using (ThisAsyncLock.TakeLock())
            {
                ThrowIfNotOpened();
                ThrowIfFaulted();

                if ((State != CommunicationState.Opened)
                    || (_closeOutputWaitObject != null))
                {
                    return;
                }

                _closeOutputWaitObject = new InterruptibleWaitObject(false, true);
            }

            bool throwing = true;

            try
            {
                await InternalCloseOutputSessionAsync(timeout);
                throwing = false;
            }
            finally
            {
                if (throwing)
                {
                    ReliableSession.OnLocalFault(null, SequenceTerminatedFault.CreateCommunicationFault(ReliableSession.OutputID, SRP.CloseOutputSessionErrorReason, null), null);
                    _closeOutputWaitObject.Fault(this);
                }
                else
                {
                    _closeOutputWaitObject.Set();
                }
            }
        }

        protected override void OnClosed()
        {
            base.OnClosed();

            Binder.Faulted -= OnBinderFaulted;
            if (_deliveryStrategy != null)
                _deliveryStrategy.Dispose();
        }

        protected override void OnClosing()
        {
            base.OnClosing();
            _acknowledgementTimer.Cancel();
        }

        private void OnComponentFaulted(Exception faultException, WsrmFault fault)
        {
            ReliableSession.OnLocalFault(faultException, fault, null);
        }

        private void OnComponentException(Exception exception)
        {
            ReliableSession.OnUnknownException(exception);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected override void OnFaulted()
        {
            ReliableSession.OnFaulted();
            UnblockClose();
            base.OnFaulted();
        }

        protected override async Task OnSendAsync(Message message, TimeSpan timeout)
        {
            if (!await OutputConnection.AddMessageAsync(message, timeout, null))
                ThrowInvalidAddException();
        }

        private async Task OnSendAsyncHandler(MessageAttemptInfo attemptInfo, TimeSpan timeout, bool maskUnhandledException)
        {
            using (attemptInfo.Message)
            {
                if (attemptInfo.RetryCount > Settings.MaxRetryCount)
                {
                    ReliableSession.OnLocalFault(new CommunicationException(SRP.MaximumRetryCountExceeded, _maxRetryCountException),
                        SequenceTerminatedFault.CreateMaxRetryCountExceededFault(ReliableSession.OutputID), null);
                }
                else
                {
                    ReliableSession.OnLocalActivity();
                    AddPendingAcknowledgements(attemptInfo.Message);

                    MaskingMode maskingMode = maskUnhandledException ? MaskingMode.Unhandled : MaskingMode.None;

                    if (attemptInfo.RetryCount < Settings.MaxRetryCount)
                    {
                        maskingMode |= MaskingMode.Handled;
                        await Binder.SendAsync(attemptInfo.Message, timeout, maskingMode);
                    }
                    else
                    {
                        try
                        {
                            await Binder.SendAsync(attemptInfo.Message, timeout, maskingMode);
                        }
                        catch (Exception e)
                        {
                            if (Fx.IsFatal(e))
                                throw;

                            if (Binder.IsHandleable(e))
                            {
                                _maxRetryCountException = e;
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
            }
        }

        private async Task OnSendAckRequestedAsyncHandler(TimeSpan timeout)
        {
            ReliableSession.OnLocalActivity();
            using (Message message = WsrmUtilities.CreateAckRequestedMessage(Settings.MessageVersion,
                Settings.ReliableMessagingVersion, ReliableSession.OutputID))
            {
                await Binder.SendAsync(message, timeout, MaskingMode.Handled);
            }
        }

        protected virtual void OnMessageDropped()
        {
        }

        protected void SetConnections()
        {
            OutputConnection = new ReliableOutputConnection(ReliableSession.OutputID,
            Settings.MaxTransferWindowSize, Settings.MessageVersion,
            Settings.ReliableMessagingVersion, ReliableSession.InitiationTime, true, DefaultSendTimeout);
            OutputConnection.Faulted += OnComponentFaulted;
            OutputConnection.OnException += OnComponentException;
            OutputConnection.SendAsyncHandler = OnSendAsyncHandler;
            OutputConnection.SendAckRequestedAsyncHandler = OnSendAckRequestedAsyncHandler;

            _inputConnection = new ReliableInputConnection();
            _inputConnection.ReliableMessagingVersion = Settings.ReliableMessagingVersion;

            if (Settings.Ordered)
                _deliveryStrategy = new OrderedDeliveryStrategy<Message>(this, Settings.MaxTransferWindowSize, false);
            else
                _deliveryStrategy = new UnorderedDeliveryStrategy<Message>(this, Settings.MaxTransferWindowSize);

            _deliveryStrategy.DequeueCallback = OnDeliveryStrategyItemDequeued;
        }

        protected void SetSession(ChannelReliableSession session)
        {
            session.UnblockChannelCloseCallback = UnblockClose;
            ReliableSession = session;
        }

        private void OnDeliveryStrategyItemDequeued()
        {
            if (_advertisedZero)
                _ = OnAcknowledgementTimeoutElapsedAsync(null);
        }

        private static Task StartReceivingAsyncStatic(object state)
        {
            var thisPtr = (ReliableDuplexSessionChannel)state;
            return thisPtr.StartReceivingAsync();
        }

        protected async Task StartReceivingAsync()
        {
            try
            {
                while (true)
                {
                    (bool success, RequestContext context) = await Binder.TryReceiveAsync(TimeSpan.MaxValue);
                    if (success)
                    {
                        if (context == null)
                        {
                            bool terminated = false;

                            using (ThisAsyncLock.TakeLock())
                            {
                                terminated = _inputConnection.Terminate();
                            }

                            if (!terminated && (Binder.State == CommunicationState.Opened))
                            {
                                Exception e = new CommunicationException(SRP.EarlySecurityClose);
                                ReliableSession.OnLocalFault(e, (Message)null, null);
                            }

                            break; // End receive loop
                        }

                        Message message = context.RequestMessage;
                        context.Close();

                        WsrmMessageInfo info = WsrmMessageInfo.Get(Settings.MessageVersion,
                            Settings.ReliableMessagingVersion, Binder.Channel, Binder.GetInnerSession(),
                            message);

                        ActionItem.Schedule(s_processMessageStatic, (this, info));
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

        private void ShutdownCallback(object state)
        {
            Shutdown();
        }

        private async Task TerminateSequenceAsync(TimeSpan timeout)
        {
            ReliableMessagingVersion reliableMessagingVersion = Settings.ReliableMessagingVersion;

            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                if (OutputConnection.CheckForTermination())
                {
                    ReliableSession.CloseSession();
                }

                Message message = WsrmUtilities.CreateTerminateMessage(Settings.MessageVersion,
                    reliableMessagingVersion, ReliableSession.OutputID);
                await Binder.SendAsync(message, timeout, MaskingMode.Handled);
            }
            else if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                CreateTerminateRequestor();
                await _terminateRequestor.RequestAsync(timeout);
                // reply came from receive loop, receive loop owns verified message so nothing more to do.
            }
            else
            {
                throw Fx.AssertAndThrow("Reliable messaging version not supported.");
            }
        }

        private void ThrowIfCloseInvalid()
        {
            bool shouldFault = false;

            if (Settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                if (_deliveryStrategy.EnqueuedCount > 0 || _inputConnection.Ranges.Count > 1)
                {
                    shouldFault = true;
                }
            }
            else if (Settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                if (_deliveryStrategy.EnqueuedCount > 0)
                {
                    shouldFault = true;
                }
            }

            if (shouldFault)
            {
                WsrmFault fault = SequenceTerminatedFault.CreateProtocolFault(ReliableSession.InputID,
                    SRP.SequenceTerminatedSessionClosedBeforeDone, SRP.SessionClosedBeforeDone);
                ReliableSession.OnLocalFault(null, fault, null);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(fault.CreateException());
            }
        }

        private void ThrowInvalidAddException()
        {
            if (State == CommunicationState.Opened)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SendCannotBeCalledAfterCloseOutputSession));
            else if (State == CommunicationState.Faulted)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(GetTerminalException());
            else
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateClosedException());
        }

        private void UnblockClose()
        {
            if (OutputConnection != null)
            {
                OutputConnection.Fault(this);
            }

            if (_inputConnection != null)
            {
                _inputConnection.Fault(this);
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

    internal class ClientReliableDuplexSessionChannel : ReliableDuplexSessionChannel
    {
        private ChannelParameterCollection _channelParameters;
        private DuplexClientReliableSession _clientSession;
        private TimeoutHelper _closeTimeoutHelper;
        private bool _closing;
        private static Func<object, Task> s_onReconnectTimerElapsed = new Func<object, Task>(OnReconnectTimerElapsed);

        public ClientReliableDuplexSessionChannel(ChannelManagerBase factory, IReliableFactorySettings settings,
            IReliableChannelBinder binder, FaultHelper faultHelper,
            LateBoundChannelParameterCollection channelParameters, UniqueId inputID)
            : base(factory, settings, binder)
        {
            _clientSession = new DuplexClientReliableSession(this, settings, faultHelper, inputID);
            _clientSession.PollingCallback = PollingAsyncCallback;
            SetSession(_clientSession);

            _channelParameters = channelParameters;
            channelParameters.SetChannel(this);
            ((IClientReliableChannelBinder)binder).ConnectionLost += OnConnectionLost;
        }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(ChannelParameterCollection))
            {
                return (T)(object)_channelParameters;
            }

            return base.GetProperty<T>();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnCloseAsync(timeout).ToApm(callback, state);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnOpenAsync(timeout).ToApm(callback, state);
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            _closeTimeoutHelper = new TimeoutHelper(timeout);
            _closing = true;
            return base.OnCloseAsync(timeout);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TaskHelpers.WaitForCompletion(OnCloseAsync(timeout));
        }

        private async void OnConnectionLost(object sender, EventArgs args)
        {
            await using (await ThisAsyncLock.TakeLockAsync())
            {
                if ((State == CommunicationState.Opened || State == CommunicationState.Closing) &&
                    !Binder.Connected && _clientSession.StopPolling())
                {

                    if (WcfEventSource.Instance.ClientReliableSessionReconnectIsEnabled())
                    {
                        WcfEventSource.Instance.ClientReliableSessionReconnect(_clientSession.Id);
                    }

                    await ReconnectAsync();
                }
            }
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected internal override async Task OnOpenAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            bool throwing = true;

            try
            {
                await Binder.OpenAsync(timeoutHelper.RemainingTime());
                await ReliableSession.OpenAsync(timeoutHelper.RemainingTime());
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

        protected override void OnOpen(TimeSpan timeout)
        {
            TaskHelpers.WaitForCompletion(OnOpenAsync(timeout));
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            SetConnections();
            ActionItem.Schedule(s_startReceivingAsyncStatic, this);
        }

        private static async Task OnReconnectTimerElapsed(object state)
        {
            ClientReliableDuplexSessionChannel channel = (ClientReliableDuplexSessionChannel)state;
            await using (await channel.ThisAsyncLock.TakeLockAsync())
            {
                if ((channel.State == CommunicationState.Opened || channel.State == CommunicationState.Closing) &&
                    !channel.Binder.Connected)
                {
                    await channel.ReconnectAsync();
                }
                else
                {
                    channel._clientSession.ResumePolling(channel.OutputConnection.Strategy.QuotaRemaining == 0);
                }
            }
        }

        protected override void OnRemoteActivity()
        {
            ReliableSession.OnRemoteActivity(OutputConnection.Strategy.QuotaRemaining == 0);
        }

        private async Task PollingAsyncCallback()
        {
            using (Message message = WsrmUtilities.CreateAckRequestedMessage(Settings.MessageVersion,
                Settings.ReliableMessagingVersion, ReliableSession.OutputID))
            {
                await Binder.SendAsync(message, DefaultSendTimeout);
            }
        }

        protected override Task ProcessMessageAsync(WsrmMessageInfo info)
        {
            if (!ReliableSession.ProcessInfo(info, null))
                return Task.CompletedTask;

            if (!ReliableSession.VerifyDuplexProtocolElements(info, null))
                return Task.CompletedTask;

            return ProcessDuplexMessageAsync(info);
        }

        private async Task ReconnectAsync()
        {
            bool handleException = true;

            try
            {
                Message message = WsrmUtilities.CreateAckRequestedMessage(Settings.MessageVersion,
                    Settings.ReliableMessagingVersion, ReliableSession.OutputID);
                TimeSpan timeout = _closing ? _closeTimeoutHelper.RemainingTime() : DefaultCloseTimeout;
                await Binder.SendAsync(message, timeout);
                handleException = false;

                using (ThisAsyncLock.TakeLock())
                {
                    if (Binder.Connected)
                        _clientSession.ResumePolling(OutputConnection.Strategy.QuotaRemaining == 0);
                    else
                        WaitForReconnect();
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                    throw;

                if (handleException)
                    WaitForReconnect();
                else
                    throw;
            }
        }

        // If anything throws out of this method, we'll consider it fatal.
        private void WaitForReconnect()
        {
            // It might be worth considering replacing this with Task.Delay and awaiting the task
            // but leaving for now to minimize code churn in porting.
            TimeSpan timeout;

            if (_closing)
                timeout = TimeoutHelper.Divide(_closeTimeoutHelper.RemainingTime(), 2);
            else
                timeout = TimeoutHelper.Divide(DefaultSendTimeout, 2);

            IOThreadTimer timer = new IOThreadTimer(s_onReconnectTimerElapsed, this, false);
            timer.Set(timeout);
        }

        private class DuplexClientReliableSession : ClientReliableSession, IDuplexSession
        {
            private ClientReliableDuplexSessionChannel channel;

            public DuplexClientReliableSession(ClientReliableDuplexSessionChannel channel,
                IReliableFactorySettings settings, FaultHelper helper, UniqueId inputID)
                : base(channel, settings, (IClientReliableChannelBinder)channel.Binder, helper, inputID)
            {
                this.channel = channel;
            }

            public IAsyncResult BeginCloseOutputSession(AsyncCallback callback, object state)
            {
                return BeginCloseOutputSession(channel.DefaultCloseTimeout, callback, state);
            }

            public IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return channel.OnCloseOutputSessionAsync(timeout).ToApm(callback, state);
            }

            public void EndCloseOutputSession(IAsyncResult result)
            {
                result.ToApmEnd();
            }

            public void CloseOutputSession()
            {
                CloseOutputSession(channel.DefaultCloseTimeout);
            }

            public void CloseOutputSession(TimeSpan timeout)
            {
                TaskHelpers.WaitForCompletionNoSpin(channel.OnCloseOutputSessionAsync(timeout));
            }
        }
    }
}
