// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.Transactions;
using System.Xml;

namespace System.ServiceModel.Transactions
{
    internal class WsatTransactionHeader : MessageHeader
    {
        private string _wsatHeaderElement;
        private string _coordinationNamespace;
        private byte[] _propagationToken;

        // Fields for writing a full WS-Coordination context
        private string _atomicTransactionNamespace;
        private AddressingVersion _addressingVersion;
        private Guid _transactionId;
        private uint _expires;

        /// <summary>
        /// Constructor for an empty (sentinel) header or for reading.
        /// </summary>
        public WsatTransactionHeader(byte[] propagationToken, string headerElement, string coordinationNamespace)
        {
            _propagationToken = propagationToken;
            _wsatHeaderElement = headerElement;
            _coordinationNamespace = coordinationNamespace;
        }

        /// <summary>
        /// Constructor for writing a full WS-Coordination context with OleTx upgrade token.
        /// </summary>
        public WsatTransactionHeader(
            Guid transactionId,
            uint expires,
            byte[] propagationToken,
            string headerElement,
            string coordinationNamespace,
            string atomicTransactionNamespace,
            AddressingVersion addressingVersion)
        {
            _transactionId = transactionId;
            _expires = expires;
            _propagationToken = propagationToken;
            _wsatHeaderElement = headerElement;
            _coordinationNamespace = coordinationNamespace;
            _atomicTransactionNamespace = atomicTransactionNamespace;
            _addressingVersion = addressingVersion;
        }

        public override bool MustUnderstand => true;

        public override string Name => _wsatHeaderElement;

        public override string Namespace => _coordinationNamespace;

        public byte[] PropagationToken => _propagationToken;

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            if (_atomicTransactionNamespace != null)
            {
                WriteCoordinationContext(writer);
            }
        }

        private void WriteCoordinationContext(XmlDictionaryWriter writer)
        {
            // <wscoor:Identifier>
            writer.WriteStartElement(CoordinationExternalStrings.Prefix,
                CoordinationExternalStrings.Identifier, _coordinationNamespace);
            writer.WriteString("urn:uuid:" + _transactionId.ToString("D"));
            writer.WriteEndElement();

            // <wscoor:Expires>
            writer.WriteStartElement(CoordinationExternalStrings.Prefix,
                CoordinationExternalStrings.Expires, _coordinationNamespace);
            writer.WriteValue(_expires);
            writer.WriteEndElement();

            // <wscoor:CoordinationType>
            writer.WriteStartElement(CoordinationExternalStrings.Prefix,
                CoordinationExternalStrings.CoordinationType, _coordinationNamespace);
            writer.WriteString(_atomicTransactionNamespace);
            writer.WriteEndElement();

            // <wscoor:RegistrationService> - minimal EPR required by WS-Coordination schema.
            // The server uses the OleTx propagation token (below) to unmarshal the transaction
            // directly, so this EPR is not contacted.
            var dummyEpr = new EndpointAddress("https://localhost/wsat/registration");
            dummyEpr.WriteTo(_addressingVersion, writer,
                CoordinationExternalStrings.RegistrationService, _coordinationNamespace);

            // <oletx:PropagationToken> - OleTx upgrade token.
            // The server detects this and uses OleTx to unmarshal the transaction directly,
            // bypassing WS-AT coordination protocol.
            WritePropagationTokenElement(writer, _propagationToken);
        }

        internal static void WritePropagationTokenElement(XmlDictionaryWriter writer, byte[] propagationToken)
        {
            writer.WriteStartElement(OleTxTransactionExternalStrings.Prefix,
                OleTxTransactionExternalStrings.PropagationToken,
                OleTxTransactionExternalStrings.Namespace);
            writer.WriteBase64(propagationToken, 0, propagationToken.Length);
            writer.WriteEndElement();
        }

        public static WsatTransactionHeader ReadFrom(Message message, string headerElement, string ns)
        {
            int index;
            try
            {
                index = message.Headers.FindHeader(headerElement, ns);
            }
            catch (MessageHeaderException e)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new TransactionException(SRP.WsatHeaderCorrupt, e));
            }

            if (index < 0)
                return null;

            WsatTransactionHeader wsatHeader;
            XmlDictionaryReader reader = message.Headers.GetReaderAtHeader(index);
            using (reader)
            {
                try
                {
                    wsatHeader = ReadFrom(reader, headerElement, ns);
                }
                catch (XmlException xe)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new TransactionException(SRP.WsatHeaderCorrupt, xe));
                }
            }

            MessageHeaderInfo header = message.Headers[index];
            if (!message.Headers.UnderstoodHeaders.Contains(header))
            {
                message.Headers.UnderstoodHeaders.Add(header);
            }

            return wsatHeader;
        }

        private static WsatTransactionHeader ReadFrom(XmlDictionaryReader reader, string headerElement, string ns)
        {
            reader.ReadStartElement(headerElement, ns);

            // A WS-Coordination context contains: Identifier, Expires, CoordinationType,
            // RegistrationService, then optional extensions (including OleTx PropagationToken).
            // We scan through all child elements looking for the OleTx propagation token.
            byte[] propagationToken = null;
            while (reader.IsStartElement())
            {
                if (OleTxTransactionHeader.IsStartPropagationTokenElement(reader))
                {
                    propagationToken = OleTxTransactionHeader.ReadPropagationTokenElement(reader);
                }
                else
                {
                    reader.Skip();
                }
            }
            reader.ReadEndElement();

            return new WsatTransactionHeader(propagationToken, headerElement, ns);
        }
    }
}
