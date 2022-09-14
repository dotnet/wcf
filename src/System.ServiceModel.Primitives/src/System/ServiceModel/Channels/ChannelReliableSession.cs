// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal abstract class ChannelReliableSession : ISession
    {
        private IReliableChannelBinder _binder;
        private bool _canSendFault = true;
        private SessionFaultState _faulted = SessionFaultState.NotFaulted;
        private SequenceRangeCollection _finalRanges;
        private InterruptibleTimer _inactivityTimer;
        private bool _isSessionClosed = false;
        private RequestContext _replyFaultContext;
        private Message _terminatingFault;
        private UnblockChannelCloseHandler _unblockChannelCloseCallback;

        protected ChannelReliableSession(ChannelBase channel, IReliableFactorySettings settings, IReliableChannelBinder binder, FaultHelper faultHelper)
        {
            Channel = channel;
            Settings = settings;
            _binder = binder;
            FaultHelper = faultHelper;
            _inactivityTimer = new InterruptibleTimer(Settings.InactivityTimeout, new WaitCallback(OnInactivityElapsed), null);
            InitiationTime = ReliableMessagingConstants.UnknownInitiationTime;
        }

        protected ChannelBase Channel { get; }

        protected Guard Guard { get; } = new Guard(int.MaxValue);

        public string Id
        {
            get
            {
                UniqueId sequenceId = SequenceID;
                if (sequenceId == null)
                    return null;
                else
                    return sequenceId.ToString();
            }
        }

        public TimeSpan InitiationTime { get; protected set; }

        public UniqueId InputID { get; protected set; }

        protected FaultHelper FaultHelper { get; }

        public UniqueId OutputID { get; protected set; }

        public abstract UniqueId SequenceID { get; }

        public IReliableFactorySettings Settings { get; }

        protected object ThisLock { get; } = new object();

        public UnblockChannelCloseHandler UnblockChannelCloseCallback { set => _unblockChannelCloseCallback = value; }

        public virtual void Abort()
        {
            Guard.Abort();
            _inactivityTimer.Abort();

            // Try to send a fault.
            bool sendFault;
            lock (ThisLock)
            {
                // Faulted thread already cleaned up. No need to to anything more.
                if (_faulted == SessionFaultState.CleanedUp)
                    return;

                // Can only send a fault if the other side did not send one already.
                sendFault = _canSendFault && (_faulted != SessionFaultState.RemotelyFaulted);    // NotFaulted || LocallyFaulted
                _faulted = SessionFaultState.CleanedUp;
            }

            if (sendFault)
            {
                if ((_binder.State == CommunicationState.Opened)
                    && _binder.Connected)
                {
                    if (_terminatingFault == null)
                    {
                        UniqueId sequenceId = InputID ?? OutputID;
                        if (sequenceId != null)
                        {
                            WsrmFault fault = SequenceTerminatedFault.CreateCommunicationFault(sequenceId, SRP.SequenceTerminatedOnAbort, null);
                            _terminatingFault = fault.CreateMessage(Settings.MessageVersion,
                                Settings.ReliableMessagingVersion);
                        }
                    }

                    if (_terminatingFault != null)
                    {
                        AddFinalRanges();
                        FaultHelper.SendFaultAsync(_binder, _replyFaultContext, _terminatingFault);
                        return;
                    }
                }
            }

            // Got here so the session did not actually send a fault, must clean up resources.
            if (_terminatingFault != null)
                _terminatingFault.Close();
            if (_replyFaultContext != null)
                _replyFaultContext.Abort();
            _binder.Abort();
        }

        private void AddFinalRanges()
        {
            // This relies on the assumption that acknowledgements can be piggybacked on sequence
            // faults for the converse sequence.
            if (_finalRanges != null)
            {
                WsrmUtilities.AddAcknowledgementHeader(Settings.ReliableMessagingVersion,
                    _terminatingFault, InputID, _finalRanges, true);
            }
        }

        public abstract Task OpenAsync(TimeSpan timeout);

        public virtual async Task CloseAsync(TimeSpan timeout)
        {
            await Guard.CloseAsync(timeout);
            _inactivityTimer.Abort();
        }

        // Corresponds to the state where the other side could have gone away already.
        public void CloseSession()
        {
            _isSessionClosed = true;
        }

        protected virtual void FaultCore()
        {

            if (WcfEventSource.Instance.ReliableSessionChannelFaultedIsEnabled())
            {
                WcfEventSource.Instance.ReliableSessionChannelFaulted(Id);
            }

            _inactivityTimer.Abort();
        }

        public void OnLocalFault(Exception e, WsrmFault fault, RequestContext context)
        {
            Message faultMessage = (fault == null) ? null : fault.CreateMessage(Settings.MessageVersion,
                Settings.ReliableMessagingVersion);
            OnLocalFault(e, faultMessage, context);
        }

        public void OnLocalFault(Exception e, Message faultMessage, RequestContext context)
        {
            if (Channel.Aborted ||
                Channel.State == CommunicationState.Faulted ||
                Channel.State == CommunicationState.Closed)
            {
                if (faultMessage != null)
                    faultMessage.Close();
                if (context != null)
                    context.Abort();
                return;
            }

            lock (ThisLock)
            {
                if (_faulted != SessionFaultState.NotFaulted)
                    return;
                _faulted = SessionFaultState.LocallyFaulted;
                _terminatingFault = faultMessage;
                _replyFaultContext = context;
            }

            FaultCore();
            Channel.Fault(e);
            UnblockChannelIfNecessary();
        }

        public void OnRemoteFault(WsrmFault fault)
        {
            OnRemoteFault(WsrmFault.CreateException(fault));
        }

        public void OnRemoteFault(Exception e)
        {
            if (Channel.Aborted ||
                Channel.State == CommunicationState.Faulted ||
                Channel.State == CommunicationState.Closed)
            {
                return;
            }

            lock (ThisLock)
            {
                if (_faulted != SessionFaultState.NotFaulted)
                    return;
                _faulted = SessionFaultState.RemotelyFaulted;
            }

            FaultCore();
            Channel.Fault(e);
            UnblockChannelIfNecessary();
        }

        public virtual void OnFaulted()
        {
            FaultCore();

            // Try to send a fault.
            bool sendFault;
            lock (ThisLock)
            {
                // Channel was faulted without the session being told first (e.g. open throws).
                // The session does not know what fault to send so let abort send it if it can.
                if (_faulted == SessionFaultState.NotFaulted)
                    return;

                // Abort thread decided to clean up.
                if (_faulted == SessionFaultState.CleanedUp)
                    return;

                // Can only send a fault if the other side did not send one already.
                sendFault = _canSendFault && (_faulted != SessionFaultState.RemotelyFaulted);  // LocallyFaulted
                _faulted = SessionFaultState.CleanedUp;
            }

            if (sendFault)
            {
                if ((_binder.State == CommunicationState.Opened)
                    && _binder.Connected
                    && (_terminatingFault != null))
                {
                    AddFinalRanges();
                    FaultHelper.SendFaultAsync(_binder, _replyFaultContext, _terminatingFault);
                    return;
                }
            }

            // Got here so the session did not actually send a fault, must clean up resources.
            if (_terminatingFault != null)
                _terminatingFault.Close();
            if (_replyFaultContext != null)
                _replyFaultContext.Abort();
            _binder.Abort();
        }

        private void OnInactivityElapsed(object state)
        {
            WsrmFault fault;
            Exception e;
            string exceptionMessage = SRP.Format(SRP.SequenceTerminatedInactivityTimeoutExceeded, Settings.InactivityTimeout);

            if (WcfEventSource.Instance.InactivityTimeoutIsEnabled())
            {
                WcfEventSource.Instance.InactivityTimeout(exceptionMessage);
            }

            if (SequenceID != null)
            {
                string faultReason = SRP.Format(SRP.SequenceTerminatedInactivityTimeoutExceeded, Settings.InactivityTimeout);
                fault = SequenceTerminatedFault.CreateCommunicationFault(SequenceID, faultReason, exceptionMessage);
                e = fault.CreateException();
            }
            else
            {
                fault = null;
                e = new CommunicationException(exceptionMessage);
            }

            OnLocalFault(e, fault, null);
        }

        public abstract void OnLocalActivity();

        public void OnUnknownException(Exception e)
        {
            _canSendFault = false;
            OnLocalFault(e, (Message)null, null);
        }

        public virtual void OnRemoteActivity(bool fastPolling)
        {
            _inactivityTimer.Set();
        }

        // returns true if the info does not fault the session.
        public bool ProcessInfo(WsrmMessageInfo info, RequestContext context)
        {
            return ProcessInfo(info, context, false);
        }

        public bool ProcessInfo(WsrmMessageInfo info, RequestContext context, bool throwException)
        {
            Exception e;
            if (info.ParsingException != null)
            {
                WsrmFault fault;

                if (SequenceID != null)
                {
                    string reason = SRP.Format(SRP.CouldNotParseWithAction, info.Action);
                    fault = SequenceTerminatedFault.CreateProtocolFault(SequenceID, reason, null);
                }
                else
                {
                    fault = null;
                }

                e = new ProtocolException(SRP.MessageExceptionOccurred, info.ParsingException);
                OnLocalFault(throwException ? null : e, fault, context);
            }
            else if (info.FaultReply != null)
            {
                e = info.FaultException;
                OnLocalFault(throwException ? null : e, info.FaultReply, context);
            }
            else if ((info.WsrmHeaderFault != null) && (info.WsrmHeaderFault.SequenceID != InputID)
                && (info.WsrmHeaderFault.SequenceID != OutputID))
            {
                e = new ProtocolException(SRP.Format(SRP.WrongIdentifierFault, FaultException.GetSafeReasonText(info.WsrmHeaderFault.Reason)));
                OnLocalFault(throwException ? null : e, (Message)null, context);
            }
            else if (info.FaultInfo != null)
            {
                if (_isSessionClosed)
                {
                    UnknownSequenceFault unknownSequenceFault = info.FaultInfo as UnknownSequenceFault;

                    if (unknownSequenceFault != null)
                    {
                        UniqueId sequenceId = unknownSequenceFault.SequenceID;

                        if (((OutputID != null) && (OutputID == sequenceId))
                            || ((InputID != null) && (InputID == sequenceId)))
                        {
                            if (Settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
                            {
                                info.Message.Close();
                                return false;
                            }
                            else if (Settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
                            {
                                return true;
                            }
                            else
                            {
                                throw Fx.AssertAndThrow("Unknown version.");
                            }
                        }
                    }
                }

                e = info.FaultException;
                if (context != null)
                    context.Close();
                OnRemoteFault(throwException ? null : e);
            }
            else
            {
                return true;
            }

            info.Message.Close();
            if (throwException)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(e);
            else
                return false;
        }

        public void SetFinalAck(SequenceRangeCollection finalRanges)
        {
            _finalRanges = finalRanges;
        }

        public virtual void StartInactivityTimer()
        {
            _inactivityTimer.Set();
        }

        // RM channels fault out of band. During the Closing and Closed states CommunicationObjects
        // do not fault. In all other states the RM channel can and must unblock various methods
        // from the OnFaulted method. This method will ensure that anything that needs to unblock
        // in the Closing state will unblock if a fault occurs.
        private void UnblockChannelIfNecessary()
        {
            lock (ThisLock)
            {
                if (_faulted == SessionFaultState.NotFaulted)
                {
                    throw Fx.AssertAndThrow("This method must be called from a fault thread.");
                }
                // Successfully faulted or aborted.
                else if (_faulted == SessionFaultState.CleanedUp)
                {
                    return;
                }
            }

            // Make sure the fault is sent then unblock the channel.
            OnFaulted();
            _unblockChannelCloseCallback();
        }

        public bool VerifyDuplexProtocolElements(WsrmMessageInfo info, RequestContext context)
        {
            return VerifyDuplexProtocolElements(info, context, false);
        }

        public bool VerifyDuplexProtocolElements(WsrmMessageInfo info, RequestContext context, bool throwException)
        {
            WsrmFault fault = VerifyDuplexProtocolElements(info);

            if (fault == null)
            {
                return true;
            }

            if (throwException)
            {
                Exception e = fault.CreateException();
                OnLocalFault(null, fault, context);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(e);
            }
            else
            {
                OnLocalFault(fault.CreateException(), fault, context);
                return false;
            }
        }

        protected virtual WsrmFault VerifyDuplexProtocolElements(WsrmMessageInfo info)
        {
            if (info.AcknowledgementInfo != null && info.AcknowledgementInfo.SequenceID != OutputID)
                return new UnknownSequenceFault(info.AcknowledgementInfo.SequenceID);
            else if (info.AckRequestedInfo != null && info.AckRequestedInfo.SequenceID != InputID)
                return new UnknownSequenceFault(info.AckRequestedInfo.SequenceID);
            else if (info.SequencedMessageInfo != null && info.SequencedMessageInfo.SequenceID != InputID)
                return new UnknownSequenceFault(info.SequencedMessageInfo.SequenceID);
            else if (info.TerminateSequenceInfo != null && info.TerminateSequenceInfo.Identifier != InputID)
            {
                if (Settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
                    return SequenceTerminatedFault.CreateProtocolFault(OutputID, SRP.SequenceTerminatedUnexpectedTerminateSequence, SRP.UnexpectedTerminateSequence);
                else if (info.TerminateSequenceInfo.Identifier == OutputID)
                    return null;
                else
                    return new UnknownSequenceFault(info.TerminateSequenceInfo.Identifier);
            }
            else if (info.TerminateSequenceResponseInfo != null)
            {
                WsrmUtilities.AssertWsrm11(Settings.ReliableMessagingVersion);

                if (info.TerminateSequenceResponseInfo.Identifier == OutputID)
                    return null;
                else
                    return new UnknownSequenceFault(info.TerminateSequenceResponseInfo.Identifier);
            }
            else if (info.CloseSequenceInfo != null)
            {
                WsrmUtilities.AssertWsrm11(Settings.ReliableMessagingVersion);

                if (info.CloseSequenceInfo.Identifier == InputID)
                    return null;
                else if (info.CloseSequenceInfo.Identifier == OutputID)
                    // Spec allows RM-Destination close, but we do not.
                    return SequenceTerminatedFault.CreateProtocolFault(OutputID, SRP.SequenceTerminatedUnsupportedClose, SRP.UnsupportedCloseExceptionString);
                else
                    return new UnknownSequenceFault(info.CloseSequenceInfo.Identifier);
            }
            else if (info.CloseSequenceResponseInfo != null)
            {
                WsrmUtilities.AssertWsrm11(Settings.ReliableMessagingVersion);

                if (info.CloseSequenceResponseInfo.Identifier == OutputID)
                    return null;
                else if (info.CloseSequenceResponseInfo.Identifier == InputID)
                    return SequenceTerminatedFault.CreateProtocolFault(InputID, SRP.SequenceTerminatedUnexpectedCloseSequenceResponse, SRP.UnexpectedCloseSequenceResponse);
                else
                    return new UnknownSequenceFault(info.CloseSequenceResponseInfo.Identifier);
            }
            else
                return null;
        }

        public bool VerifySimplexProtocolElements(WsrmMessageInfo info, RequestContext context)
        {
            return VerifySimplexProtocolElements(info, context, false);
        }

        public bool VerifySimplexProtocolElements(WsrmMessageInfo info, RequestContext context, bool throwException)
        {
            WsrmFault fault = VerifySimplexProtocolElements(info);

            if (fault == null)
            {
                return true;
            }

            info.Message.Close();

            if (throwException)
            {
                Exception e = fault.CreateException();
                OnLocalFault(null, fault, context);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(e);
            }
            else
            {
                OnLocalFault(fault.CreateException(), fault, context);
                return false;
            }
        }

        protected abstract WsrmFault VerifySimplexProtocolElements(WsrmMessageInfo info);

        private enum SessionFaultState
        {
            NotFaulted,
            LocallyFaulted,
            RemotelyFaulted,
            CleanedUp
        }

        public delegate void UnblockChannelCloseHandler();
    }

    internal class ClientReliableSession : ChannelReliableSession, IOutputSession
    {
        private IClientReliableChannelBinder _binder;
        private PollingMode _oldPollingMode;
        private PollingHandler _pollingHandler;
        private PollingMode _pollingMode;
        private InterruptibleTimer _pollingTimer;
        private ReliableRequestor _requestor;

        public delegate Task PollingHandler();

        public ClientReliableSession(ChannelBase channel, IReliableFactorySettings factory, IClientReliableChannelBinder binder, FaultHelper faultHelper, UniqueId inputID) :
            base(channel, factory, binder, faultHelper)
        {
            _binder = binder;
            InputID = inputID;
            _pollingTimer = new InterruptibleTimer(GetPollingInterval(), OnPollingTimerElapsed, null);

            if (_binder.Channel is IRequestChannel)
            {
                _requestor = new RequestReliableRequestor();
            }
            else if (_binder.Channel is IDuplexChannel)
            {
                SendReceiveReliableRequestor sendReceiveRequestor = new SendReceiveReliableRequestor();
                sendReceiveRequestor.TimeoutIsSafe = !ChannelSupportsOneCreateSequenceAttempt();
                _requestor = sendReceiveRequestor;
            }
            else
            {
                Fx.Assert("This channel type is not supported");
            }

            MessageVersion messageVersion = Settings.MessageVersion;
            ReliableMessagingVersion reliableMessagingVersion = Settings.ReliableMessagingVersion;
            _requestor.MessageVersion = messageVersion;
            _requestor.Binder = _binder;
            _requestor.IsCreateSequence = true;
            _requestor.TimeoutString1Index = SRP.TimeoutOnOpen;
            _requestor.MessageAction = WsrmIndex.GetCreateSequenceActionHeader(messageVersion.Addressing,
                reliableMessagingVersion);
            if ((reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
                && (_binder.GetInnerSession() is ISecureConversationSession))
            {
                _requestor.MessageHeader = new WsrmUsesSequenceSTRHeader();
            }
            _requestor.MessageBody = new CreateSequence(Settings.MessageVersion.Addressing,
                reliableMessagingVersion, Settings.Ordered, _binder, InputID);
            _requestor.SetRequestResponsePattern();
        }

        public PollingHandler PollingCallback { set => _pollingHandler = value; }

        public override UniqueId SequenceID => OutputID;

        public override void Abort()
        {
            ReliableRequestor temp = _requestor;

            if (temp != null)
                temp.Abort(Channel);
            _pollingTimer.Abort();
            base.Abort();
        }

        public override async Task OpenAsync(TimeSpan timeout)
        {
            if (_pollingHandler == null)
            {
                throw Fx.AssertAndThrow("The client reliable channel must set the polling handler prior to opening the client reliable session.");
            }

            var start = DateTime.UtcNow;
            Message response = await _requestor.RequestAsync(timeout);
            ProcessCreateSequenceResponse(response, start);
            _requestor = null;
        }

        private bool ChannelSupportsOneCreateSequenceAttempt()
        {
            IDuplexSessionChannel channel = _binder.Channel as IDuplexSessionChannel;

            if (channel == null)
                return false;

            return (channel.Session is ISecuritySession && !(channel.Session is ISecureConversationSession));
        }

        public override async Task CloseAsync(TimeSpan timeout)
        {
            await base.CloseAsync(timeout);
            _pollingTimer.Abort();
        }

        protected override void FaultCore()
        {
            _pollingTimer.Abort();
            base.FaultCore();
        }

        private TimeSpan GetPollingInterval()
        {
            switch (_pollingMode)
            {
                case PollingMode.Idle:
                    return Ticks.ToTimeSpan(Ticks.FromTimeSpan(Settings.InactivityTimeout) / 2);

                case PollingMode.KeepAlive:
                    return WsrmUtilities.CalculateKeepAliveInterval(Settings.InactivityTimeout, Settings.MaxRetryCount);

                case PollingMode.NotPolling:
                    return TimeSpan.MaxValue;

                case PollingMode.FastPolling:
                    TimeSpan keepAliveInterval = WsrmUtilities.CalculateKeepAliveInterval(Settings.InactivityTimeout, Settings.MaxRetryCount);
                    TimeSpan fastPollingInterval = Ticks.ToTimeSpan(Ticks.FromTimeSpan(_binder.DefaultSendTimeout) / 2);

                    if (fastPollingInterval < keepAliveInterval)
                        return fastPollingInterval;
                    else
                        return keepAliveInterval;

                default:
                    throw Fx.AssertAndThrow("Unknown polling mode.");
            }
        }

        public override void OnFaulted()
        {
            base.OnFaulted();

            ReliableRequestor temp = _requestor;

            if (temp != null)
                _requestor.Fault(Channel);
        }

        private async Task OnPollingTimerElapsed(object state)
        {
            if (Guard.Enter())
            {
                try
                {
                    lock (ThisLock)
                    {
                        if (_pollingMode == PollingMode.NotPolling)
                            return;

                        if (_pollingMode == PollingMode.Idle)
                            _pollingMode = PollingMode.KeepAlive;
                    }

                    await _pollingHandler();
                    _pollingTimer.Set(GetPollingInterval());
                }
                finally
                {
                    Guard.Exit();
                }
            }
        }

        public override void OnLocalActivity()
        {
            lock (ThisLock)
            {
                if (_pollingMode == PollingMode.NotPolling)
                    return;

                _pollingTimer.Set(GetPollingInterval());
            }
        }

        public override void OnRemoteActivity(bool fastPolling)
        {
            base.OnRemoteActivity(fastPolling);
            lock (ThisLock)
            {
                if (_pollingMode == PollingMode.NotPolling)
                    return;

                if (fastPolling)
                    _pollingMode = PollingMode.FastPolling;
                else
                    _pollingMode = PollingMode.Idle;

                _pollingTimer.Set(GetPollingInterval());
            }
        }

        private void ProcessCreateSequenceResponse(Message response, DateTime start)
        {
            CreateSequenceResponseInfo createResponse = null;

            using (response)
            {
                if (response.IsFault)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(WsrmUtilities.CreateCSFaultException(
                        Settings.MessageVersion, Settings.ReliableMessagingVersion, response,
                        _binder.Channel));
                }
                else
                {
                    WsrmMessageInfo info = WsrmMessageInfo.Get(Settings.MessageVersion,
                        Settings.ReliableMessagingVersion, _binder.Channel, _binder.GetInnerSession(),
                        response, true);

                    if (info.ParsingException != null)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ProtocolException(SRP.UnparsableCSResponse, info.ParsingException));

                    // this throws and sends a fault if something is wrong with the info
                    ProcessInfo(info, null, true);
                    createResponse = info.CreateSequenceResponseInfo;

                    string exceptionReason = null;
                    string faultReason = null;

                    if (createResponse == null)
                    {
                        exceptionReason = SRP.Format(SRP.InvalidWsrmResponseChannelNotOpened,
                            WsrmFeb2005Strings.CreateSequence, info.Action,
                            WsrmIndex.GetCreateSequenceResponseActionString(Settings.ReliableMessagingVersion));
                    }
                    else if (!object.Equals(createResponse.RelatesTo, _requestor.MessageId))
                    {
                        exceptionReason = SRP.Format(SRP.WsrmMessageWithWrongRelatesToExceptionString, WsrmFeb2005Strings.CreateSequence);
                        faultReason = SRP.Format(SRP.WsrmMessageWithWrongRelatesToFaultString, WsrmFeb2005Strings.CreateSequence);
                    }
                    else if ((createResponse.AcceptAcksTo == null) && (InputID != null))
                    {
                        if (Settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
                        {
                            exceptionReason = SRP.CSResponseWithoutOffer;
                            faultReason = SRP.CSResponseWithoutOfferReason;
                        }
                        else if (Settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
                        {
                            exceptionReason = SRP.CSResponseOfferRejected;
                            faultReason = SRP.CSResponseOfferRejectedReason;
                        }
                        else
                        {
                            throw Fx.AssertAndThrow("Reliable messaging version not supported.");
                        }
                    }
                    else if ((createResponse.AcceptAcksTo != null) && (InputID == null))
                    {
                        exceptionReason = SRP.CSResponseWithOffer;
                        faultReason = SRP.CSResponseWithOfferReason;
                    }
                    else if (createResponse.AcceptAcksTo != null && (createResponse.AcceptAcksTo.Uri != _binder.RemoteAddress.Uri))
                    {
                        exceptionReason = SRP.AcksToMustBeSameAsRemoteAddress;
                        faultReason = SRP.AcksToMustBeSameAsRemoteAddressReason;
                    }

                    if ((faultReason != null) && (createResponse != null))
                    {
                        UniqueId sequenceId = createResponse.Identifier;
                        WsrmFault fault = SequenceTerminatedFault.CreateProtocolFault(sequenceId, faultReason, null);
                        OnLocalFault(null, fault, null);
                    }

                    if (exceptionReason != null)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ProtocolException(exceptionReason));
                }
            }

            InitiationTime = DateTime.UtcNow - start;
            OutputID = createResponse.Identifier;
            _pollingTimer.Set(GetPollingInterval());
            base.StartInactivityTimer();
        }

        public void ResumePolling(bool fastPolling)
        {
            lock (ThisLock)
            {
                if (_pollingMode != PollingMode.NotPolling)
                {
                    throw Fx.AssertAndThrow("Can't resume polling if pollingMode != PollingMode.NotPolling");
                }

                if (fastPolling)
                {
                    _pollingMode = PollingMode.FastPolling;
                }
                else
                {
                    if (_oldPollingMode == PollingMode.FastPolling)
                        _pollingMode = PollingMode.Idle;
                    else
                        _pollingMode = _oldPollingMode;
                }

                Guard.Exit();
                _pollingTimer.Set(GetPollingInterval());
            }
        }

        // Returns true if caller should resume polling
        public bool StopPolling()
        {
            lock (ThisLock)
            {
                if (_pollingMode == PollingMode.NotPolling)
                    return false;

                _oldPollingMode = _pollingMode;
                _pollingMode = PollingMode.NotPolling;
                _pollingTimer.Cancel();
                return Guard.Enter();
            }
        }

        protected override WsrmFault VerifyDuplexProtocolElements(WsrmMessageInfo info)
        {
            WsrmFault fault = base.VerifyDuplexProtocolElements(info);

            if (fault != null)
                return fault;
            else if (info.CreateSequenceInfo != null)
                return SequenceTerminatedFault.CreateProtocolFault(OutputID, SRP.SequenceTerminatedUnexpectedCS, SRP.UnexpectedCS);
            else if (info.CreateSequenceResponseInfo != null && info.CreateSequenceResponseInfo.Identifier != OutputID)
                return SequenceTerminatedFault.CreateProtocolFault(OutputID, SRP.SequenceTerminatedUnexpectedCSROfferId, SRP.UnexpectedCSROfferId);
            else
                return null;
        }

        protected override WsrmFault VerifySimplexProtocolElements(WsrmMessageInfo info)
        {
            if (info.AcknowledgementInfo != null && info.AcknowledgementInfo.SequenceID != OutputID)
                return new UnknownSequenceFault(info.AcknowledgementInfo.SequenceID);
            else if (info.AckRequestedInfo != null)
                return SequenceTerminatedFault.CreateProtocolFault(OutputID, SRP.SequenceTerminatedUnexpectedAckRequested, SRP.UnexpectedAckRequested);
            else if (info.CreateSequenceInfo != null)
                return SequenceTerminatedFault.CreateProtocolFault(OutputID, SRP.SequenceTerminatedUnexpectedCS, SRP.UnexpectedCS);
            else if (info.SequencedMessageInfo != null)
                return new UnknownSequenceFault(info.SequencedMessageInfo.SequenceID);
            else if (info.TerminateSequenceInfo != null)
            {
                if (Settings.ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
                    return SequenceTerminatedFault.CreateProtocolFault(OutputID, SRP.SequenceTerminatedUnexpectedTerminateSequence, SRP.UnexpectedTerminateSequence);
                else if (info.TerminateSequenceInfo.Identifier == OutputID)
                    return null;
                else
                    return new UnknownSequenceFault(info.TerminateSequenceInfo.Identifier);
            }
            else if (info.TerminateSequenceResponseInfo != null)
            {
                WsrmUtilities.AssertWsrm11(Settings.ReliableMessagingVersion);

                if (info.TerminateSequenceResponseInfo.Identifier == OutputID)
                    return null;
                else
                    return new UnknownSequenceFault(info.TerminateSequenceResponseInfo.Identifier);
            }
            else if (info.CloseSequenceInfo != null)
            {
                WsrmUtilities.AssertWsrm11(Settings.ReliableMessagingVersion);

                if (info.CloseSequenceInfo.Identifier == OutputID)
                    return SequenceTerminatedFault.CreateProtocolFault(OutputID, SRP.SequenceTerminatedUnsupportedClose, SRP.UnsupportedCloseExceptionString);
                else
                    return new UnknownSequenceFault(info.CloseSequenceInfo.Identifier);
            }
            else if (info.CloseSequenceResponseInfo != null)
            {
                WsrmUtilities.AssertWsrm11(Settings.ReliableMessagingVersion);

                if (info.CloseSequenceResponseInfo.Identifier == OutputID)
                    return null;
                else
                    return new UnknownSequenceFault(info.CloseSequenceResponseInfo.Identifier);
            }
            else
                return null;
        }

        private enum PollingMode
        {
            Idle,
            KeepAlive,
            FastPolling,
            NotPolling
        }
    }
}
