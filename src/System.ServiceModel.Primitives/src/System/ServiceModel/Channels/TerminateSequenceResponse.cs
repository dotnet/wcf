// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel;
using System.Xml;

namespace System.ServiceModel.Channels
{
    internal sealed class TerminateSequenceResponse : BodyWriter
    {
        public TerminateSequenceResponse() : base(true)
        {
        }

        public TerminateSequenceResponse(UniqueId identifier)
            : base(true)
        {
            this.Identifier = identifier;
        }

        public UniqueId Identifier { get; set; }

        public static TerminateSequenceResponseInfo Create(XmlDictionaryReader reader)
        {
            if (reader == null)
            {
                Fx.Assert("Argument reader cannot be null.");
            }

            TerminateSequenceResponseInfo terminateSequenceInfo = new TerminateSequenceResponseInfo();
            XmlDictionaryString wsrmNs = WsrmIndex.GetNamespace(ReliableMessagingVersion.WSReliableMessaging11);

            reader.ReadStartElement(DXD.Wsrm11Dictionary.TerminateSequenceResponse, wsrmNs);

            reader.ReadStartElement(XD.WsrmFeb2005Dictionary.Identifier, wsrmNs);
            terminateSequenceInfo.Identifier = reader.ReadContentAsUniqueId();
            reader.ReadEndElement();

            while (reader.IsStartElement())
            {
                reader.Skip();
            }

            reader.ReadEndElement();

            return terminateSequenceInfo;
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            XmlDictionaryString wsrmNs = WsrmIndex.GetNamespace(ReliableMessagingVersion.WSReliableMessaging11);
            writer.WriteStartElement(DXD.Wsrm11Dictionary.TerminateSequenceResponse, wsrmNs);
            writer.WriteStartElement(XD.WsrmFeb2005Dictionary.Identifier, wsrmNs);
            writer.WriteValue(this.Identifier);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
