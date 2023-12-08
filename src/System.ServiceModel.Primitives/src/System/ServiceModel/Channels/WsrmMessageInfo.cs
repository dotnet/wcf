// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.Serialization;
using System.ServiceModel.Security;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal sealed class WsrmMessageInfo
    {
        private Exception _faultException;
        private Message _faultReply;

        public WsrmMessageInfo()
        {
        }

        public WsrmAcknowledgmentInfo AcknowledgementInfo { get; private set; }

        public WsrmAckRequestedInfo AckRequestedInfo { get; private set; }

        public string Action { get; private set; }

        public CloseSequenceInfo CloseSequenceInfo { get; private set; }

        public CloseSequenceResponseInfo CloseSequenceResponseInfo { get; private set; }

        public CreateSequenceInfo CreateSequenceInfo { get; private set; }

        public CreateSequenceResponseInfo CreateSequenceResponseInfo { get; private set; }

        public Exception FaultException
        {
            get
            {
                return _faultException;
            }
            set
            {
                if (_faultException != null)
                {
                    throw Fx.AssertAndThrow("FaultException can only be set once.");
                }

                _faultException = value;
            }
        }

        public MessageFault FaultInfo
        {
            get
            {
                return MessageFault;
            }
        }

        public Message FaultReply
        {
            get
            {
                return _faultReply;
            }
            set
            {
                if (_faultReply != null)
                {
                    throw Fx.AssertAndThrow("FaultReply can only be set once.");
                }

                _faultReply = value;
            }
        }

        public Message Message { get; private set; }

        public MessageFault MessageFault { get; private set; }

        public Exception ParsingException { get; private set; }

        public WsrmSequencedMessageInfo SequencedMessageInfo { get; private set; }

        public TerminateSequenceInfo TerminateSequenceInfo { get; private set; }

        public TerminateSequenceResponseInfo TerminateSequenceResponseInfo { get; private set; }

        public WsrmUsesSequenceSSLInfo UsesSequenceSSLInfo { get; private set; }

        public WsrmUsesSequenceSTRInfo UsesSequenceSTRInfo { get; private set; }

        public WsrmHeaderFault WsrmHeaderFault
        {
            get
            {
                return MessageFault as WsrmHeaderFault;
            }
        }

        public static Exception CreateInternalFaultException(Message faultReply, string message, Exception inner)
        {
            return new InternalFaultException(faultReply, SRP.Format(SRP.WsrmMessageProcessingError, message), inner);
        }

        private static Exception CreateWsrmRequiredException(MessageVersion messageVersion)
        {
            string exceptionReason = SRP.WsrmRequiredExceptionString;
            string faultReason = SRP.WsrmRequiredFaultString;
            Message faultReply = new WsrmRequiredFault(faultReason).CreateMessage(messageVersion,
                ReliableMessagingVersion.WSReliableMessaging11);
            return CreateInternalFaultException(faultReply, exceptionReason, new ProtocolException(exceptionReason));
        }

        // Caller should check these things:
        // FaultReply and FaultException, FaultInfo and FaultException or ParsingException
        public static WsrmMessageInfo Get(MessageVersion messageVersion,
            ReliableMessagingVersion reliableMessagingVersion, IChannel channel, ISession session, Message message)
        {
            return Get(messageVersion, reliableMessagingVersion, channel, session, message, false);
        }

        public static WsrmMessageInfo Get(MessageVersion messageVersion,
            ReliableMessagingVersion reliableMessagingVersion, IChannel channel, ISession session, Message message,
            bool csrOnly)
        {
            WsrmMessageInfo messageInfo = new WsrmMessageInfo();
            messageInfo.Message = message;
            bool isFault = true;

            try
            {
                isFault = message.IsFault;
                MessageHeaders headers = message.Headers;
                string action = headers.Action;
                messageInfo.Action = action;
                bool foundAction = false;
                bool wsrmFeb2005 = reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005;
                bool wsrm11 = reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11;
                bool csOnly = false;

                if (action == WsrmIndex.GetCreateSequenceResponseActionString(reliableMessagingVersion))
                {
                    messageInfo.CreateSequenceResponseInfo = CreateSequenceResponseInfo.ReadMessage(messageVersion,
                        reliableMessagingVersion, message, headers);
                    ValidateMustUnderstand(messageVersion, message);
                    return messageInfo;
                }

                if (csrOnly)
                    return messageInfo;

                if (action == WsrmIndex.GetTerminateSequenceActionString(reliableMessagingVersion))
                {
                    messageInfo.TerminateSequenceInfo = TerminateSequenceInfo.ReadMessage(messageVersion,
                        reliableMessagingVersion, message, headers);
                    foundAction = true;
                }
                else if (action == WsrmIndex.GetCreateSequenceActionString(reliableMessagingVersion))
                {
                    messageInfo.CreateSequenceInfo = CreateSequenceInfo.ReadMessage(messageVersion,
                        reliableMessagingVersion, session as ISecureConversationSession, message, headers);

                    if (wsrmFeb2005)
                    {
                        ValidateMustUnderstand(messageVersion, message);
                        return messageInfo;
                    }

                    csOnly = true;
                }
                else if (wsrm11)
                {
                    if (action == Wsrm11Strings.CloseSequenceAction)
                    {
                        messageInfo.CloseSequenceInfo = CloseSequenceInfo.ReadMessage(messageVersion, message,
                            headers);
                        foundAction = true;
                    }
                    else if (action == Wsrm11Strings.CloseSequenceResponseAction)
                    {
                        messageInfo.CloseSequenceResponseInfo = CloseSequenceResponseInfo.ReadMessage(messageVersion,
                             message, headers);
                        foundAction = true;
                    }
                    else if (action == WsrmIndex.GetTerminateSequenceResponseActionString(reliableMessagingVersion))
                    {
                        messageInfo.TerminateSequenceResponseInfo = TerminateSequenceResponseInfo.ReadMessage(messageVersion,
                             message, headers);
                        foundAction = true;
                    }
                }

                string wsrmNs = WsrmIndex.GetNamespaceString(reliableMessagingVersion);
                bool soap11 = messageVersion.Envelope == EnvelopeVersion.Soap11;
                bool foundHeader = false;
                int foundTooManyIndex = -1;
                int sequenceIndex = -1;
                int ackIndex = -1;
                int ackRequestedIndex = -1;
                int maxIndex = -1;
                int minIndex = -1;
                int sequenceFaultIndex = -1;
                int usesSequenceSSLIndex = -1;
                int usesSequenceSTRIndex = -1;

                for (int index = 0; index < headers.Count; index++)
                {
                    MessageHeaderInfo header = headers[index];

                    if (!messageVersion.Envelope.IsUltimateDestinationActor(header.Actor))
                        continue;

                    if (header.Namespace == wsrmNs)
                    {
                        bool setIndex = true;

                        if (csOnly)
                        {
                            if (wsrm11 && (header.Name == Wsrm11Strings.UsesSequenceSSL))
                            {
                                if (usesSequenceSSLIndex != -1)
                                {
                                    foundTooManyIndex = index;
                                    break;
                                }
                                usesSequenceSSLIndex = index;
                            }
                            else if (wsrm11 && (header.Name == Wsrm11Strings.UsesSequenceSTR))
                            {
                                if (usesSequenceSTRIndex != -1)
                                {
                                    foundTooManyIndex = index;
                                    break;
                                }
                                usesSequenceSTRIndex = index;
                            }
                            else
                            {
                                setIndex = false;
                            }
                        }
                        else
                        {
                            if (header.Name == WsrmFeb2005Strings.Sequence)
                            {
                                if (sequenceIndex != -1)
                                {
                                    foundTooManyIndex = index;
                                    break;
                                }
                                sequenceIndex = index;
                            }
                            else if (header.Name == WsrmFeb2005Strings.SequenceAcknowledgement)
                            {
                                if (ackIndex != -1)
                                {
                                    foundTooManyIndex = index;
                                    break;
                                }
                                ackIndex = index;
                            }
                            else if (header.Name == WsrmFeb2005Strings.AckRequested)
                            {
                                if (ackRequestedIndex != -1)
                                {
                                    foundTooManyIndex = index;
                                    break;
                                }
                                ackRequestedIndex = index;
                            }
                            else if (soap11 && (header.Name == WsrmFeb2005Strings.SequenceFault))
                            {
                                if (sequenceFaultIndex != -1)
                                {
                                    foundTooManyIndex = index;
                                    break;
                                }
                                sequenceFaultIndex = index;
                            }
                            else
                            {
                                setIndex = false;
                            }
                        }

                        if (setIndex)
                        {
                            if (index > maxIndex)
                                maxIndex = index;

                            if (minIndex == -1)
                                minIndex = index;
                        }
                    }
                }

                if (foundTooManyIndex != -1)
                {
                    Collection<MessageHeaderInfo> notUnderstoodHeaders = new Collection<MessageHeaderInfo>();
                    notUnderstoodHeaders.Add(headers[foundTooManyIndex]);
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new MustUnderstandSoapException(notUnderstoodHeaders, messageVersion.Envelope));
                }

                if (maxIndex > -1)
                {
                    BufferedMessage bufferedMessage = message as BufferedMessage;

                    if (bufferedMessage != null && bufferedMessage.Headers.ContainsOnlyBufferedMessageHeaders)
                    {
                        foundHeader = true;

                        using (XmlDictionaryReader reader = headers.GetReaderAtHeader(minIndex))
                        {
                            for (int index = minIndex; index <= maxIndex; index++)
                            {
                                MessageHeaderInfo header = headers[index];

                                if (csOnly)
                                {
                                    if (wsrm11 && (index == usesSequenceSSLIndex))
                                    {
                                        messageInfo.UsesSequenceSSLInfo = WsrmUsesSequenceSSLInfo.ReadHeader(
                                            reader, header);
                                        headers.UnderstoodHeaders.Add(header);
                                    }
                                    else if (wsrm11 && (index == usesSequenceSTRIndex))
                                    {
                                        messageInfo.UsesSequenceSTRInfo = WsrmUsesSequenceSTRInfo.ReadHeader(
                                            reader, header);
                                        headers.UnderstoodHeaders.Add(header);
                                    }
                                    else
                                    {
                                        reader.Skip();
                                    }
                                }
                                else
                                {
                                    if (index == sequenceIndex)
                                    {
                                        messageInfo.SequencedMessageInfo = WsrmSequencedMessageInfo.ReadHeader(
                                            reliableMessagingVersion, reader, header);
                                        headers.UnderstoodHeaders.Add(header);
                                    }
                                    else if (index == ackIndex)
                                    {
                                        messageInfo.AcknowledgementInfo = WsrmAcknowledgmentInfo.ReadHeader(
                                            reliableMessagingVersion, reader, header);
                                        headers.UnderstoodHeaders.Add(header);
                                    }
                                    else if (index == ackRequestedIndex)
                                    {
                                        messageInfo.AckRequestedInfo = WsrmAckRequestedInfo.ReadHeader(
                                            reliableMessagingVersion, reader, header);
                                        headers.UnderstoodHeaders.Add(header);
                                    }
                                    else
                                    {
                                        reader.Skip();
                                    }
                                }
                            }
                        }
                    }
                }

                if (maxIndex > -1 && !foundHeader)
                {
                    foundHeader = true;

                    if (csOnly)
                    {
                        if (usesSequenceSSLIndex != -1)
                        {
                            using (XmlDictionaryReader reader = headers.GetReaderAtHeader(usesSequenceSSLIndex))
                            {
                                MessageHeaderInfo header = headers[usesSequenceSSLIndex];
                                messageInfo.UsesSequenceSSLInfo = WsrmUsesSequenceSSLInfo.ReadHeader(
                                    reader, header);
                                headers.UnderstoodHeaders.Add(header);
                            }
                        }

                        if (usesSequenceSTRIndex != -1)
                        {
                            using (XmlDictionaryReader reader = headers.GetReaderAtHeader(usesSequenceSTRIndex))
                            {
                                MessageHeaderInfo header = headers[usesSequenceSTRIndex];
                                messageInfo.UsesSequenceSTRInfo = WsrmUsesSequenceSTRInfo.ReadHeader(
                                    reader, header);
                                headers.UnderstoodHeaders.Add(header);
                            }
                        }
                    }
                    else
                    {
                        if (sequenceIndex != -1)
                        {
                            using (XmlDictionaryReader reader = headers.GetReaderAtHeader(sequenceIndex))
                            {
                                MessageHeaderInfo header = headers[sequenceIndex];

                                messageInfo.SequencedMessageInfo = WsrmSequencedMessageInfo.ReadHeader(
                                    reliableMessagingVersion, reader, header);
                                headers.UnderstoodHeaders.Add(header);
                            }
                        }

                        if (ackIndex != -1)
                        {
                            using (XmlDictionaryReader reader = headers.GetReaderAtHeader(ackIndex))
                            {
                                MessageHeaderInfo header = headers[ackIndex];
                                messageInfo.AcknowledgementInfo = WsrmAcknowledgmentInfo.ReadHeader(
                                    reliableMessagingVersion, reader, header);
                                headers.UnderstoodHeaders.Add(header);
                            }
                        }

                        if (ackRequestedIndex != -1)
                        {
                            using (XmlDictionaryReader reader = headers.GetReaderAtHeader(ackRequestedIndex))
                            {
                                MessageHeaderInfo header = headers[ackRequestedIndex];
                                messageInfo.AckRequestedInfo = WsrmAckRequestedInfo.ReadHeader(reliableMessagingVersion,
                                    reader, header);
                                headers.UnderstoodHeaders.Add(header);
                            }
                        }
                    }
                }

                if (csOnly)
                {
                    CreateSequenceInfo.ValidateCreateSequenceHeaders(messageVersion,
                        session as ISecureConversationSession, messageInfo);
                    ValidateMustUnderstand(messageVersion, message);
                    return messageInfo;
                }

                if (messageInfo.SequencedMessageInfo == null && messageInfo.Action == null)
                {
                    if (wsrmFeb2005)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageHeaderException(SRP.NoActionNoSequenceHeaderReason, messageVersion.Addressing.Namespace, AddressingStrings.Action, false));
                    }
                    else
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            CreateWsrmRequiredException(messageVersion));
                    }
                }

                if (messageInfo.SequencedMessageInfo == null && message.IsFault)
                {
                    messageInfo.MessageFault = MessageFault.CreateFault(message, TransportDefaults.MaxRMFaultSize);
                    WsrmHeaderFault wsrmFault;

                    if (soap11)
                    {
                        if (WsrmHeaderFault.TryCreateFault11(reliableMessagingVersion, message, messageInfo.MessageFault, sequenceFaultIndex, out wsrmFault))
                        {
                            messageInfo.MessageFault = wsrmFault;
                            messageInfo._faultException = WsrmHeaderFault.CreateException(wsrmFault);
                        }
                    }
                    else
                    {
                        if (WsrmHeaderFault.TryCreateFault12(reliableMessagingVersion, message, messageInfo.MessageFault, out wsrmFault))
                        {
                            messageInfo.MessageFault = wsrmFault;
                            messageInfo._faultException = WsrmHeaderFault.CreateException(wsrmFault);
                        }
                    }

                    // Not a wsrm fault, maybe it is another fault we should understand (i.e. addressing or soap fault).
                    if (wsrmFault == null)
                    {
                        FaultConverter faultConverter = channel.GetProperty<FaultConverter>();

                        if (faultConverter == null)
                        {
                            faultConverter = FaultConverter.GetDefaultFaultConverter(messageVersion);
                        }

                        if (!faultConverter.TryCreateException(message, messageInfo.MessageFault, out messageInfo._faultException))
                        {
                            messageInfo._faultException = new ProtocolException(SRP.Format(SRP.UnrecognizedFaultReceived, messageInfo.MessageFault.Code.Namespace, messageInfo.MessageFault.Code.Name, System.ServiceModel.FaultException.GetSafeReasonText(messageInfo.MessageFault)));
                        }
                    }

                    foundAction = true;
                }

                if (!foundHeader && !foundAction)
                {
                    if (wsrmFeb2005)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new ActionNotSupportedException(SRP.Format(SRP.NonWsrmFeb2005ActionNotSupported, action)));
                    }
                    else
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            CreateWsrmRequiredException(messageVersion));
                    }
                }

                if (foundAction || WsrmUtilities.IsWsrmAction(reliableMessagingVersion, action))
                {
                    ValidateMustUnderstand(messageVersion, message);
                }
            }
            catch (InternalFaultException exception)
            {
                if (DiagnosticUtility.ShouldTraceInformation)
                    DiagnosticUtility.TraceHandledException(exception, TraceEventType.Information);

                messageInfo.FaultReply = exception.FaultReply;
                messageInfo._faultException = exception.InnerException;
            }
            catch (CommunicationException exception)
            {
                if (DiagnosticUtility.ShouldTraceInformation)
                    DiagnosticUtility.TraceHandledException(exception, TraceEventType.Information);

                if (isFault)
                {
                    messageInfo.ParsingException = exception;
                    return messageInfo;
                }

                FaultConverter faultConverter = channel.GetProperty<FaultConverter>();
                if (faultConverter == null)
                    faultConverter = FaultConverter.GetDefaultFaultConverter(messageVersion);

                if (faultConverter.TryCreateFaultMessage(exception, out messageInfo._faultReply))
                {
                    messageInfo._faultException = new ProtocolException(SRP.MessageExceptionOccurred, exception);
                }
                else
                {
                    messageInfo.ParsingException = new ProtocolException(SRP.MessageExceptionOccurred, exception);
                }
            }
            catch (XmlException exception)
            {
                if (DiagnosticUtility.ShouldTraceInformation)
                    DiagnosticUtility.TraceHandledException(exception, TraceEventType.Information);

                messageInfo.ParsingException = new ProtocolException(SRP.MessageExceptionOccurred, exception);
            }

            return messageInfo;
        }

        private static void ValidateMustUnderstand(MessageVersion version, Message message)
        {
            Collection<MessageHeaderInfo> notUnderstoodHeaders = message.Headers.GetHeadersNotUnderstood();
            if ((notUnderstoodHeaders != null) && (notUnderstoodHeaders.Count > 0))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new MustUnderstandSoapException(notUnderstoodHeaders, version.Envelope));
            }
        }

        [Serializable]
        private class InternalFaultException : ProtocolException
        {
            private Message faultReply;

            public InternalFaultException()
                : base()
            {
            }

            public InternalFaultException(Message faultReply, string message, Exception inner)
                : base(message, inner)
            {
                this.faultReply = faultReply;
            }

#pragma warning disable SYSLIB0051
            protected InternalFaultException(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
            }
#pragma warning restore SYSLIB0051

            public Message FaultReply
            {
                get
                {
                    return faultReply;
                }
            }
        }
    }

    internal sealed class CloseSequenceInfo : WsrmRequestInfo
    {
        public UniqueId Identifier { get; set; }

        public long LastMsgNumber { get; set; }

        public override string RequestName
        {
            get
            {
                return Wsrm11Strings.CloseSequence;
            }
        }

        public static CloseSequenceInfo ReadMessage(MessageVersion messageVersion, Message message,
            MessageHeaders headers)
        {
            if (message.IsEmpty)
            {
                string reason = SRP.Format(SRP.NonEmptyWsrmMessageIsEmpty, Wsrm11Strings.CloseSequenceAction);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(reason));
            }

            CloseSequenceInfo info;
            using (XmlDictionaryReader reader = message.GetReaderAtBodyContents())
            {
                info = CloseSequence.Create(reader);
                message.ReadFromBodyContentsToEnd(reader);
            }

            info.SetMessageId(messageVersion, headers);
            info.SetReplyTo(messageVersion, headers);

            return info;
        }
    }

    internal sealed class CloseSequenceResponseInfo
    {
        public UniqueId Identifier { get; set; }

        public UniqueId RelatesTo { get; set; }

        public static CloseSequenceResponseInfo ReadMessage(MessageVersion messageVersion, Message message,
            MessageHeaders headers)
        {
            if (headers.RelatesTo == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new MessageHeaderException(SRP.Format(SRP.MissingRelatesToOnWsrmResponseReason,
                    DXD.Wsrm11Dictionary.CloseSequenceResponse), messageVersion.Addressing.Namespace,
                    AddressingStrings.RelatesTo, false));
            }

            if (message.IsEmpty)
            {
                string reason = SRP.Format(SRP.NonEmptyWsrmMessageIsEmpty,
                    Wsrm11Strings.CloseSequenceResponseAction);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(reason));
            }

            CloseSequenceResponseInfo info;
            using (XmlDictionaryReader reader = message.GetReaderAtBodyContents())
            {
                info = CloseSequenceResponse.Create(reader);
                message.ReadFromBodyContentsToEnd(reader);
            }

            info.RelatesTo = headers.RelatesTo;
            return info;
        }
    }

    internal sealed class CreateSequenceInfo : WsrmRequestInfo
    {
        public EndpointAddress AcksTo { get; set; } = EndpointAddress.AnonymousAddress;

        public TimeSpan? Expires { get; set; }

        public TimeSpan? OfferExpires { get; set; }

        public UniqueId OfferIdentifier { get; set; }

        public override string RequestName
        {
            get
            {
                return WsrmFeb2005Strings.CreateSequence;
            }
        }

        public Uri To { get; private set; }

        public static CreateSequenceInfo ReadMessage(MessageVersion messageVersion,
            ReliableMessagingVersion reliableMessagingVersion, ISecureConversationSession securitySession,
            Message message, MessageHeaders headers)
        {
            if (message.IsEmpty)
            {
                string reason = SRP.Format(SRP.NonEmptyWsrmMessageIsEmpty,
                    WsrmIndex.GetCreateSequenceActionString(reliableMessagingVersion));
                Message faultReply = WsrmUtilities.CreateCSRefusedProtocolFault(messageVersion, reliableMessagingVersion, reason);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(WsrmMessageInfo.CreateInternalFaultException(faultReply, reason, new ProtocolException(reason)));
            }

            CreateSequenceInfo info;
            using (XmlDictionaryReader reader = message.GetReaderAtBodyContents())
            {
                info = CreateSequence.Create(messageVersion, reliableMessagingVersion, securitySession, reader);
                message.ReadFromBodyContentsToEnd(reader);
            }

            info.SetMessageId(messageVersion, headers);
            info.SetReplyTo(messageVersion, headers);

            if (info.AcksTo.Uri != info.ReplyTo.Uri)
            {
                string reason = SRP.CSRefusedAcksToMustEqualReplyTo;
                Message faultReply = WsrmUtilities.CreateCSRefusedProtocolFault(messageVersion, reliableMessagingVersion, reason);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(WsrmMessageInfo.CreateInternalFaultException(faultReply, reason, new ProtocolException(reason)));
            }

            info.To = message.Headers.To;
            if (info.To == null && messageVersion.Addressing == AddressingVersion.WSAddressing10)
                info.To = messageVersion.Addressing.AnonymousUri;

            return info;
        }

        public static void ValidateCreateSequenceHeaders(MessageVersion messageVersion,
            ISecureConversationSession securitySession, WsrmMessageInfo info)
        {
            string reason = null;

            if (info.UsesSequenceSSLInfo != null)
            {
                reason = SRP.CSRefusedSSLNotSupported;
            }
            else if ((info.UsesSequenceSTRInfo != null) && (securitySession == null))
            {
                reason = SRP.CSRefusedSTRNoWSSecurity;
            }
            else if ((info.UsesSequenceSTRInfo == null) && (securitySession != null))
            {
                reason = SRP.CSRefusedNoSTRWSSecurity;
            }

            if (reason != null)
            {
                Message faultReply = WsrmUtilities.CreateCSRefusedProtocolFault(messageVersion,
                    ReliableMessagingVersion.WSReliableMessaging11, reason);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(WsrmMessageInfo.CreateInternalFaultException(faultReply, reason, new ProtocolException(reason)));
            }
        }
    }

    internal sealed class CreateSequenceResponseInfo
    {
        public EndpointAddress AcceptAcksTo { get; set; }

        public UniqueId Identifier { get; set; }

        public UniqueId RelatesTo { get; set; }

        public static CreateSequenceResponseInfo ReadMessage(MessageVersion messageVersion,
            ReliableMessagingVersion reliableMessagingVersion, Message message, MessageHeaders headers)
        {
            if (message.IsEmpty)
            {
                string reason = SRP.Format(SRP.NonEmptyWsrmMessageIsEmpty,
                    WsrmIndex.GetCreateSequenceResponseActionString(reliableMessagingVersion));
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(reason));
            }

            if (headers.RelatesTo == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new MessageHeaderException(SRP.Format(SRP.MissingRelatesToOnWsrmResponseReason,
                    XD.WsrmFeb2005Dictionary.CreateSequenceResponse), messageVersion.Addressing.Namespace,
                    AddressingStrings.RelatesTo, false));
            }

            CreateSequenceResponseInfo info;
            using (XmlDictionaryReader reader = message.GetReaderAtBodyContents())
            {
                info = CreateSequenceResponse.Create(messageVersion.Addressing, reliableMessagingVersion, reader);
                message.ReadFromBodyContentsToEnd(reader);
            }

            info.RelatesTo = headers.RelatesTo;
            return info;
        }
    }

    internal sealed class TerminateSequenceInfo : WsrmRequestInfo
    {
        public UniqueId Identifier { get; set; }

        public long LastMsgNumber { get; set; }

        public override string RequestName
        {
            get
            {
                return WsrmFeb2005Strings.TerminateSequence;
            }
        }

        public static TerminateSequenceInfo ReadMessage(MessageVersion messageVersion,
            ReliableMessagingVersion reliableMessagingVersion, Message message, MessageHeaders headers)
        {
            if (message.IsEmpty)
            {
                string reason = SRP.Format(SRP.NonEmptyWsrmMessageIsEmpty,
                    WsrmIndex.GetTerminateSequenceActionString(reliableMessagingVersion));
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(reason));
            }

            TerminateSequenceInfo info;
            using (XmlDictionaryReader reader = message.GetReaderAtBodyContents())
            {
                info = TerminateSequence.Create(reliableMessagingVersion, reader);
                message.ReadFromBodyContentsToEnd(reader);
            }

            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                info.SetMessageId(messageVersion, headers);
                info.SetReplyTo(messageVersion, headers);
            }

            return info;
        }
    }

    internal sealed class TerminateSequenceResponseInfo
    {
        public UniqueId Identifier { get; set; }

        public UniqueId RelatesTo { get; set; }

        public static TerminateSequenceResponseInfo ReadMessage(MessageVersion messageVersion, Message message,
            MessageHeaders headers)
        {
            if (headers.RelatesTo == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new MessageHeaderException(SRP.Format(SRP.MissingRelatesToOnWsrmResponseReason,
                    DXD.Wsrm11Dictionary.TerminateSequenceResponse), messageVersion.Addressing.Namespace,
                    AddressingStrings.RelatesTo, false));
            }

            if (message.IsEmpty)
            {
                string reason = SRP.Format(SRP.NonEmptyWsrmMessageIsEmpty,
                    WsrmIndex.GetTerminateSequenceResponseActionString(ReliableMessagingVersion.WSReliableMessaging11));
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(reason));
            }

            TerminateSequenceResponseInfo info;
            using (XmlDictionaryReader reader = message.GetReaderAtBodyContents())
            {
                info = TerminateSequenceResponse.Create(reader);
                message.ReadFromBodyContentsToEnd(reader);
            }

            info.RelatesTo = headers.RelatesTo;
            return info;
        }
    }

    internal abstract class WsrmMessageHeader : DictionaryHeader, IMessageHeaderWithSharedNamespace
    {
        protected WsrmMessageHeader(ReliableMessagingVersion reliableMessagingVersion)
        {
            ReliableMessagingVersion = reliableMessagingVersion;
        }

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedPrefix
        {
            get { return XD.WsrmFeb2005Dictionary.Prefix; }
        }

        XmlDictionaryString IMessageHeaderWithSharedNamespace.SharedNamespace
        {
            get { return WsrmIndex.GetNamespace(ReliableMessagingVersion); }
        }

        public override XmlDictionaryString DictionaryNamespace
        {
            get { return WsrmIndex.GetNamespace(ReliableMessagingVersion); }
        }

        public override string Namespace
        {
            get { return WsrmIndex.GetNamespaceString(ReliableMessagingVersion); }
        }

        protected ReliableMessagingVersion ReliableMessagingVersion { get; }
    }

    internal abstract class WsrmHeaderInfo
    {
        protected WsrmHeaderInfo(MessageHeaderInfo messageHeader)
        {
            MessageHeader = messageHeader;
        }

        public MessageHeaderInfo MessageHeader { get; }
    }

    internal abstract class WsrmRequestInfo
    {
        protected WsrmRequestInfo()
        {
        }

        public UniqueId MessageId { get; private set; }

        public EndpointAddress ReplyTo { get; private set; }

        public abstract string RequestName { get; }

        protected void SetMessageId(MessageVersion messageVersion, MessageHeaders headers)
        {
            MessageId = headers.MessageId;

            if (MessageId == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageHeaderException(
                    SRP.Format(SRP.MissingMessageIdOnWsrmRequest, RequestName),
                    messageVersion.Addressing.Namespace,
                    AddressingStrings.MessageId,
                    false));
            }
        }

        protected void SetReplyTo(MessageVersion messageVersion, MessageHeaders headers)
        {
            ReplyTo = headers.ReplyTo;

            if (messageVersion.Addressing == AddressingVersion.WSAddressing10 && ReplyTo == null)
            {
                ReplyTo = EndpointAddress.AnonymousAddress;
            }

            if (ReplyTo == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageHeaderException(
                    SRP.Format(SRP.MissingReplyToOnWsrmRequest, RequestName),
                    messageVersion.Addressing.Namespace,
                    AddressingStrings.ReplyTo,
                    false));
            }
        }
    }

    internal sealed class WsrmSequencedMessageInfo : WsrmHeaderInfo
    {
        private WsrmSequencedMessageInfo(
            UniqueId sequenceID,
            long sequenceNumber,
            bool lastMessage,
            MessageHeaderInfo header)
            : base(header)
        {
            SequenceID = sequenceID;
            SequenceNumber = sequenceNumber;
            LastMessage = lastMessage;
        }

        public UniqueId SequenceID { get; }

        public long SequenceNumber { get; }

        public bool LastMessage { get; }

        public static WsrmSequencedMessageInfo ReadHeader(ReliableMessagingVersion reliableMessagingVersion,
            XmlDictionaryReader reader, MessageHeaderInfo header)
        {
            WsrmFeb2005Dictionary wsrmFeb2005Dictionary = XD.WsrmFeb2005Dictionary;
            XmlDictionaryString wsrmNs = WsrmIndex.GetNamespace(reliableMessagingVersion);

            reader.ReadStartElement();

            reader.ReadStartElement(wsrmFeb2005Dictionary.Identifier, wsrmNs);
            UniqueId sequenceID = reader.ReadContentAsUniqueId();
            reader.ReadEndElement();

            reader.ReadStartElement(wsrmFeb2005Dictionary.MessageNumber, wsrmNs);
            long sequenceNumber = WsrmUtilities.ReadSequenceNumber(reader);
            reader.ReadEndElement();

            bool lastMessage = false;

            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                if (reader.IsStartElement(wsrmFeb2005Dictionary.LastMessage, wsrmNs))
                {
                    WsrmUtilities.ReadEmptyElement(reader);
                    lastMessage = true;
                }
            }

            while (reader.IsStartElement())
            {
                reader.Skip();
            }

            reader.ReadEndElement();

            return new WsrmSequencedMessageInfo(sequenceID, sequenceNumber, lastMessage, header);
        }
    }

    internal sealed class WsrmSequencedMessageHeader : WsrmMessageHeader
    {
        private bool _lastMessage;
        private UniqueId _sequenceID;
        private long _sequenceNumber;

        public WsrmSequencedMessageHeader(
            ReliableMessagingVersion reliableMessagingVersion,
            UniqueId sequenceID,
            long sequenceNumber,
            bool lastMessage)
            : base(reliableMessagingVersion)
        {
            _sequenceID = sequenceID;
            _sequenceNumber = sequenceNumber;
            _lastMessage = lastMessage;
        }

        public override XmlDictionaryString DictionaryName
        {
            get { return XD.WsrmFeb2005Dictionary.Sequence; }
        }

        public override bool MustUnderstand
        {
            get { return true; }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            WsrmFeb2005Dictionary wsrmFeb2005Dictionary = XD.WsrmFeb2005Dictionary;
            XmlDictionaryString wsrmNs = DictionaryNamespace;

            writer.WriteStartElement(wsrmFeb2005Dictionary.Identifier, wsrmNs);
            writer.WriteValue(_sequenceID);
            writer.WriteEndElement();

            writer.WriteStartElement(wsrmFeb2005Dictionary.MessageNumber, wsrmNs);
            writer.WriteValue(_sequenceNumber);
            writer.WriteEndElement();

            if ((ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
                && _lastMessage)
            {
                writer.WriteStartElement(wsrmFeb2005Dictionary.LastMessage, wsrmNs);
                writer.WriteEndElement();
            }
        }
    }

    internal sealed class WsrmAcknowledgmentInfo : WsrmHeaderInfo
    {
        private WsrmAcknowledgmentInfo(
            UniqueId sequenceID,
            SequenceRangeCollection ranges,
            bool final,
            int bufferRemaining,
            MessageHeaderInfo header)
            : base(header)
        {
            SequenceID = sequenceID;
            Ranges = ranges;
            Final = final;
            BufferRemaining = bufferRemaining;
        }

        public int BufferRemaining { get; }

        public bool Final { get; }

        public SequenceRangeCollection Ranges { get; }

        public UniqueId SequenceID { get; }

        // February 2005 - Reads Identifier, AcknowledgementRange, Nack
        // 1.1 - Reads Identifier, AcknowledgementRange, None, Final, Nack
        internal static void ReadAck(ReliableMessagingVersion reliableMessagingVersion,
            XmlDictionaryReader reader, out UniqueId sequenceId, out SequenceRangeCollection rangeCollection,
            out bool final)
        {
            WsrmFeb2005Dictionary wsrmFeb2005Dictionary = XD.WsrmFeb2005Dictionary;
            XmlDictionaryString wsrmNs = WsrmIndex.GetNamespace(reliableMessagingVersion);

            reader.ReadStartElement(wsrmFeb2005Dictionary.SequenceAcknowledgement, wsrmNs);
            reader.ReadStartElement(wsrmFeb2005Dictionary.Identifier, wsrmNs);
            sequenceId = reader.ReadContentAsUniqueId();
            reader.ReadEndElement();
            bool allowZero = reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005;

            rangeCollection = SequenceRangeCollection.Empty;
            while (reader.IsStartElement(wsrmFeb2005Dictionary.AcknowledgementRange, wsrmNs))
            {
                reader.MoveToAttribute(WsrmFeb2005Strings.Lower);
                long lower = WsrmUtilities.ReadSequenceNumber(reader, allowZero);

                reader.MoveToAttribute(WsrmFeb2005Strings.Upper);
                long upper = WsrmUtilities.ReadSequenceNumber(reader, allowZero);

                if (lower < 0 || lower > upper
                    || ((reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005) && (lower == 0 && upper > 0))
                    || ((reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11) && (lower == 0)))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new XmlException(SRP.Format(SRP.InvalidSequenceRange, lower, upper)));
                }

                rangeCollection = rangeCollection.MergeWith(new SequenceRange(lower, upper));

                reader.MoveToElement();

                WsrmUtilities.ReadEmptyElement(reader);
            }

            bool validAck = rangeCollection.Count > 0;
            final = false;
            bool wsrm11 = reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11;

            if (wsrm11)
            {
                Wsrm11Dictionary wsrm11Dictionary = DXD.Wsrm11Dictionary;

                if (reader.IsStartElement(wsrm11Dictionary.None, wsrmNs))
                {
                    if (validAck)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(
                            SRP.Format(SRP.UnexpectedXmlChildNode, reader.Name, reader.NodeType,
                            wsrmFeb2005Dictionary.SequenceAcknowledgement)));
                    }

                    WsrmUtilities.ReadEmptyElement(reader);
                    validAck = true;
                }

                if (reader.IsStartElement(wsrm11Dictionary.Final, wsrmNs))
                {
                    if (!validAck)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(
                            SRP.Format(SRP.UnexpectedXmlChildNode, reader.Name, reader.NodeType,
                            wsrmFeb2005Dictionary.SequenceAcknowledgement)));
                    }

                    WsrmUtilities.ReadEmptyElement(reader);
                    final = true;
                }
            }

            bool foundNack = false;
            while (reader.IsStartElement(wsrmFeb2005Dictionary.Nack, wsrmNs))
            {
                if (validAck)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(
                        SRP.Format(SRP.UnexpectedXmlChildNode, reader.Name, reader.NodeType,
                        MessageStrings.Body)));
                }

                reader.ReadStartElement();
                WsrmUtilities.ReadSequenceNumber(reader, true);
                reader.ReadEndElement();
                foundNack = true;
            }

            if (!validAck && !foundNack)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(
                    SRP.Format(SRP.UnexpectedXmlChildNode, reader.Name, reader.NodeType,
                    MessageStrings.Body)));
            }
        }

        public static WsrmAcknowledgmentInfo ReadHeader(ReliableMessagingVersion reliableMessagingVersion,
            XmlDictionaryReader reader, MessageHeaderInfo header)
        {
            WsrmFeb2005Dictionary wsrmFeb2005Dictionary = XD.WsrmFeb2005Dictionary;
            XmlDictionaryString wsrmNs = WsrmIndex.GetNamespace(reliableMessagingVersion);

            UniqueId sequenceID;
            SequenceRangeCollection rangeCollection;
            bool final;
            ReadAck(reliableMessagingVersion, reader, out sequenceID, out rangeCollection, out final);

            int bufferRemaining = -1;

            // Parse the extensibility section.
            while (reader.IsStartElement())
            {
                if (reader.IsStartElement(wsrmFeb2005Dictionary.BufferRemaining,
                    XD.WsrmFeb2005Dictionary.NETNamespace))
                {
                    if (bufferRemaining != -1)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(
                            SRP.Format(SRP.UnexpectedXmlChildNode, reader.Name, reader.NodeType,
                            MessageStrings.Body)));
                    }

                    reader.ReadStartElement();
                    bufferRemaining = reader.ReadContentAsInt();
                    reader.ReadEndElement();

                    if (bufferRemaining < 0)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(
                            SRP.Format(SRP.InvalidBufferRemaining, bufferRemaining)));
                    }

                    // Found BufferRemaining, continue parsing.
                    continue;
                }

                if (reader.IsStartElement(wsrmFeb2005Dictionary.AcknowledgementRange, wsrmNs))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(
                        SRP.Format(SRP.UnexpectedXmlChildNode, reader.Name, reader.NodeType,
                        MessageStrings.Body)));
                }
                else if (reader.IsStartElement(wsrmFeb2005Dictionary.Nack, wsrmNs))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(
                        SRP.Format(SRP.UnexpectedXmlChildNode, reader.Name, reader.NodeType,
                        MessageStrings.Body)));
                }
                else if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
                {
                    Wsrm11Dictionary wsrm11Dictionary = DXD.Wsrm11Dictionary;

                    if (reader.IsStartElement(wsrm11Dictionary.None, wsrmNs)
                        || reader.IsStartElement(wsrm11Dictionary.Final, wsrmNs))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(
                            SRP.Format(SRP.UnexpectedXmlChildNode, reader.Name, reader.NodeType,
                            wsrmFeb2005Dictionary.SequenceAcknowledgement)));
                    }
                }

                // Advance the reader in all cases.
                reader.Skip();
            }

            reader.ReadEndElement();

            return new WsrmAcknowledgmentInfo(sequenceID, rangeCollection, final, bufferRemaining, header);
        }
    }

    internal sealed class WsrmAcknowledgmentHeader : WsrmMessageHeader
    {
        private int _bufferRemaining;
        private bool _final;
        private SequenceRangeCollection _ranges;
        private UniqueId _sequenceID;

        public WsrmAcknowledgmentHeader(
            ReliableMessagingVersion reliableMessagingVersion,
            UniqueId sequenceID,
            SequenceRangeCollection ranges,
            bool final,
            int bufferRemaining)
            : base(reliableMessagingVersion)
        {
            _sequenceID = sequenceID;
            _ranges = ranges;
            _final = final;
            _bufferRemaining = bufferRemaining;
        }

        public override XmlDictionaryString DictionaryName
        {
            get { return XD.WsrmFeb2005Dictionary.SequenceAcknowledgement; }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            WsrmFeb2005Dictionary wsrmFeb2005Dictionary = XD.WsrmFeb2005Dictionary;
            XmlDictionaryString wsrmNs = DictionaryNamespace;

            WriteAckRanges(writer, ReliableMessagingVersion, _sequenceID, _ranges);

            if ((ReliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11) && _final)
            {
                writer.WriteStartElement(DXD.Wsrm11Dictionary.Final, wsrmNs);
                writer.WriteEndElement();
            }

            if (_bufferRemaining != -1)
            {
                writer.WriteStartElement(WsrmFeb2005Strings.NETPrefix, wsrmFeb2005Dictionary.BufferRemaining,
                    XD.WsrmFeb2005Dictionary.NETNamespace);
                writer.WriteValue(_bufferRemaining);
                writer.WriteEndElement();
            }
        }

        // February 2005 - Writes Identifier, AcknowledgementRange
        // 1.1 - Writes Identifier, AcknowledgementRange | None
        internal static void WriteAckRanges(XmlDictionaryWriter writer,
            ReliableMessagingVersion reliableMessagingVersion, UniqueId sequenceId, SequenceRangeCollection ranges)
        {
            WsrmFeb2005Dictionary wsrmFeb2005Dictionary = XD.WsrmFeb2005Dictionary;
            XmlDictionaryString wsrmNs = WsrmIndex.GetNamespace(reliableMessagingVersion);

            writer.WriteStartElement(wsrmFeb2005Dictionary.Identifier, wsrmNs);
            writer.WriteValue(sequenceId);
            writer.WriteEndElement();

            if (ranges.Count == 0)
            {
                if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
                {
                    ranges = ranges.MergeWith(0);
                }
                else if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
                {
                    writer.WriteStartElement(DXD.Wsrm11Dictionary.None, wsrmNs);
                    writer.WriteEndElement();
                }
            }

            for (int index = 0; index < ranges.Count; index++)
            {
                writer.WriteStartElement(wsrmFeb2005Dictionary.AcknowledgementRange, wsrmNs);
                writer.WriteStartAttribute(wsrmFeb2005Dictionary.Lower, null);
                writer.WriteValue(ranges[index].Lower);
                writer.WriteEndAttribute();
                writer.WriteStartAttribute(wsrmFeb2005Dictionary.Upper, null);
                writer.WriteValue(ranges[index].Upper);
                writer.WriteEndAttribute();
                writer.WriteEndElement();
            }
        }
    }

    internal sealed class WsrmAckRequestedInfo : WsrmHeaderInfo
    {
        public WsrmAckRequestedInfo(UniqueId sequenceID, MessageHeaderInfo header)
            : base(header)
        {
            SequenceID = sequenceID;
        }

        public UniqueId SequenceID { get; }

        public static WsrmAckRequestedInfo ReadHeader(ReliableMessagingVersion reliableMessagingVersion,
            XmlDictionaryReader reader, MessageHeaderInfo header)
        {
            WsrmFeb2005Dictionary wsrmFeb2005Dictionary = XD.WsrmFeb2005Dictionary;
            XmlDictionaryString wsrmNs = WsrmIndex.GetNamespace(reliableMessagingVersion);

            reader.ReadStartElement();

            reader.ReadStartElement(wsrmFeb2005Dictionary.Identifier, wsrmNs);
            UniqueId sequenceID = reader.ReadContentAsUniqueId();
            reader.ReadEndElement();

            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessagingFebruary2005)
            {
                if (reader.IsStartElement(wsrmFeb2005Dictionary.MessageNumber, wsrmNs))
                {
                    reader.ReadStartElement();
                    WsrmUtilities.ReadSequenceNumber(reader, true);
                    reader.ReadEndElement();
                }
            }

            while (reader.IsStartElement())
            {
                reader.Skip();
            }

            reader.ReadEndElement();

            return new WsrmAckRequestedInfo(sequenceID, header);
        }
    }

    internal sealed class WsrmAckRequestedHeader : WsrmMessageHeader
    {
        private UniqueId sequenceID;

        public WsrmAckRequestedHeader(ReliableMessagingVersion reliableMessagingVersion, UniqueId sequenceID)
            : base(reliableMessagingVersion)
        {
            this.sequenceID = sequenceID;
        }

        public override XmlDictionaryString DictionaryName
        {
            get { return XD.WsrmFeb2005Dictionary.AckRequested; }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            WsrmFeb2005Dictionary wsrmFeb2005Dictionary = XD.WsrmFeb2005Dictionary;
            XmlDictionaryString wsrmNs = DictionaryNamespace;

            writer.WriteStartElement(wsrmFeb2005Dictionary.Identifier, wsrmNs);
            writer.WriteValue(sequenceID);
            writer.WriteEndElement();
        }
    }

    // We do not generate the UsesSequenceSSL header. Thus, there is an info, but no header.
    internal sealed class WsrmUsesSequenceSSLInfo : WsrmHeaderInfo
    {
        private WsrmUsesSequenceSSLInfo(MessageHeaderInfo header)
            : base(header)
        {
        }

        public static WsrmUsesSequenceSSLInfo ReadHeader(XmlDictionaryReader reader, MessageHeaderInfo header)
        {
            WsrmUtilities.ReadEmptyElement(reader);
            return new WsrmUsesSequenceSSLInfo(header);
        }
    }

    internal sealed class WsrmUsesSequenceSTRHeader : WsrmMessageHeader
    {
        public WsrmUsesSequenceSTRHeader()
            : base(ReliableMessagingVersion.WSReliableMessaging11)
        {
        }

        public override XmlDictionaryString DictionaryName
        {
            get { return DXD.Wsrm11Dictionary.UsesSequenceSTR; }
        }

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
        }

        public override bool MustUnderstand
        {
            get { return true; }
        }
    }

    internal sealed class WsrmUsesSequenceSTRInfo : WsrmHeaderInfo
    {
        private WsrmUsesSequenceSTRInfo(MessageHeaderInfo header)
            : base(header)
        {
        }

        public static WsrmUsesSequenceSTRInfo ReadHeader(XmlDictionaryReader reader, MessageHeaderInfo header)
        {
            WsrmUtilities.ReadEmptyElement(reader);
            return new WsrmUsesSequenceSTRInfo(header);
        }
    }
}
