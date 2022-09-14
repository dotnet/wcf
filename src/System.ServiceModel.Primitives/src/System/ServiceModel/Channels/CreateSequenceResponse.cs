// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal sealed class CreateSequenceResponse : BodyWriter
    {
        private AddressingVersion _addressingVersion;
        private ReliableMessagingVersion _reliableMessagingVersion;

        private CreateSequenceResponse()
            : base(true)
        {
        }

        public CreateSequenceResponse(AddressingVersion addressingVersion,
            ReliableMessagingVersion reliableMessagingVersion)
            : base(true)
        {
            _addressingVersion = addressingVersion;
            _reliableMessagingVersion = reliableMessagingVersion;
        }

        public EndpointAddress AcceptAcksTo { get; set; }

        public TimeSpan? Expires { get; set; }

        public UniqueId Identifier { get; set; }

        public bool Ordered { get; set; }

        public static CreateSequenceResponseInfo Create(AddressingVersion addressingVersion,
            ReliableMessagingVersion reliableMessagingVersion, XmlDictionaryReader reader)
        {
            if (reader == null)
            {
                Fx.Assert("Argument reader cannot be null.");
            }

            CreateSequenceResponseInfo createSequenceResponse = new CreateSequenceResponseInfo();
            WsrmFeb2005Dictionary wsrmFeb2005Dictionary = XD.WsrmFeb2005Dictionary;
            XmlDictionaryString wsrmNs = WsrmIndex.GetNamespace(reliableMessagingVersion);

            reader.ReadStartElement(wsrmFeb2005Dictionary.CreateSequenceResponse, wsrmNs);

            reader.ReadStartElement(wsrmFeb2005Dictionary.Identifier, wsrmNs);
            createSequenceResponse.Identifier = reader.ReadContentAsUniqueId();
            reader.ReadEndElement();

            if (reader.IsStartElement(wsrmFeb2005Dictionary.Expires, wsrmNs))
            {
                reader.ReadElementContentAsTimeSpan();
            }

            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                if (reader.IsStartElement(DXD.Wsrm11Dictionary.IncompleteSequenceBehavior, wsrmNs))
                {
                    string incompleteSequenceBehavior = reader.ReadElementContentAsString();

                    if ((incompleteSequenceBehavior != Wsrm11Strings.DiscardEntireSequence)
                        && (incompleteSequenceBehavior != Wsrm11Strings.DiscardFollowingFirstGap)
                        && (incompleteSequenceBehavior != Wsrm11Strings.NoDiscard))
                    {
                        string reason = SRP.CSResponseWithInvalidIncompleteSequenceBehavior;
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(reason));
                    }

                    // Otherwise ignore the value.
                }
            }

            if (reader.IsStartElement(wsrmFeb2005Dictionary.Accept, wsrmNs))
            {
                reader.ReadStartElement();
                createSequenceResponse.AcceptAcksTo = EndpointAddress.ReadFrom(addressingVersion, reader,
                    wsrmFeb2005Dictionary.AcksTo, wsrmNs);
                while (reader.IsStartElement())
                {
                    reader.Skip();
                }
                reader.ReadEndElement();
            }

            while (reader.IsStartElement())
            {
                reader.Skip();
            }

            reader.ReadEndElement();

            return createSequenceResponse;
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            WsrmFeb2005Dictionary wsrmFeb2005Dictionary = XD.WsrmFeb2005Dictionary;
            XmlDictionaryString wsrmNs = WsrmIndex.GetNamespace(_reliableMessagingVersion);
            writer.WriteStartElement(wsrmFeb2005Dictionary.CreateSequenceResponse, wsrmNs);

            writer.WriteStartElement(wsrmFeb2005Dictionary.Identifier, wsrmNs);
            writer.WriteValue(Identifier);
            writer.WriteEndElement();

            if (Expires.HasValue)
            {
                writer.WriteStartElement(wsrmFeb2005Dictionary.Expires, wsrmNs);
                writer.WriteValue(Expires.Value);
                writer.WriteEndElement();
            }

            if (_reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                Wsrm11Dictionary wsrm11Dictionary = DXD.Wsrm11Dictionary;
                writer.WriteStartElement(wsrm11Dictionary.IncompleteSequenceBehavior, wsrmNs);
                writer.WriteValue(
                    Ordered ? wsrm11Dictionary.DiscardFollowingFirstGap : wsrm11Dictionary.NoDiscard);
                writer.WriteEndElement();
            }

            if (AcceptAcksTo != null)
            {
                writer.WriteStartElement(wsrmFeb2005Dictionary.Accept, wsrmNs);
                AcceptAcksTo.WriteTo(_addressingVersion, writer, wsrmFeb2005Dictionary.AcksTo, wsrmNs);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
