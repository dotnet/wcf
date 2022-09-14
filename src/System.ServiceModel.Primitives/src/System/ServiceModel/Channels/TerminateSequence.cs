// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal sealed class TerminateSequence : BodyWriter
    {
        private UniqueId _identifier;
        private long _lastMsgNumber;
        private ReliableMessagingVersion _reliableMessagingVersion;

        public TerminateSequence()
            : base(true)
        {
        }

        public TerminateSequence(ReliableMessagingVersion reliableMessagingVersion, UniqueId identifier, long last)
            : base(true)
        {
            _reliableMessagingVersion = reliableMessagingVersion;
            _identifier = identifier;
            _lastMsgNumber = last;
        }

        public static TerminateSequenceInfo Create(ReliableMessagingVersion reliableMessagingVersion,
            XmlDictionaryReader reader)
        {
            if (reader == null)
            {
                Fx.Assert("Argument reader cannot be null.");
            }

            TerminateSequenceInfo terminateSequenceInfo = new TerminateSequenceInfo();
            WsrmFeb2005Dictionary wsrmFeb2005Dictionary = XD.WsrmFeb2005Dictionary;
            XmlDictionaryString wsrmNs = WsrmIndex.GetNamespace(reliableMessagingVersion);

            reader.ReadStartElement(wsrmFeb2005Dictionary.TerminateSequence, wsrmNs);

            reader.ReadStartElement(wsrmFeb2005Dictionary.Identifier, wsrmNs);
            terminateSequenceInfo.Identifier = reader.ReadContentAsUniqueId();
            reader.ReadEndElement();

            if (reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                if (reader.IsStartElement(DXD.Wsrm11Dictionary.LastMsgNumber, wsrmNs))
                {
                    reader.ReadStartElement();
                    terminateSequenceInfo.LastMsgNumber = WsrmUtilities.ReadSequenceNumber(reader, false);
                    reader.ReadEndElement();
                }
            }

            while (reader.IsStartElement())
            {
                reader.Skip();
            }

            reader.ReadEndElement();

            return terminateSequenceInfo;
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            WsrmFeb2005Dictionary wsrmFeb2005Dictionary = XD.WsrmFeb2005Dictionary;
            XmlDictionaryString wsrmNs = WsrmIndex.GetNamespace(_reliableMessagingVersion);
            writer.WriteStartElement(wsrmFeb2005Dictionary.TerminateSequence, wsrmNs);
            writer.WriteStartElement(wsrmFeb2005Dictionary.Identifier, wsrmNs);
            writer.WriteValue(_identifier);
            writer.WriteEndElement();

            if (_reliableMessagingVersion == ReliableMessagingVersion.WSReliableMessaging11)
            {
                if (_lastMsgNumber > 0)
                {
                    writer.WriteStartElement(DXD.Wsrm11Dictionary.LastMsgNumber, wsrmNs);
                    writer.WriteValue(_lastMsgNumber);
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement();
        }
    }
}
