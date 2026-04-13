// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.Transactions;
using System.Xml;

namespace System.ServiceModel.Transactions
{
    internal class OleTxTransactionHeader : MessageHeader
    {
        private const string OleTxHeaderElement = OleTxTransactionExternalStrings.OleTxTransaction;
        private const string OleTxNamespace = OleTxTransactionExternalStrings.Namespace;
        private const string PropagationTokenElement = OleTxTransactionExternalStrings.PropagationToken;

        private byte[] _propagationToken;

        public OleTxTransactionHeader(byte[] propagationToken)
        {
            _propagationToken = propagationToken;
        }

        public override bool MustUnderstand => true;

        public override string Name => OleTxHeaderElement;

        public override string Namespace => OleTxNamespace;

        public byte[] PropagationToken => _propagationToken;

        protected override void OnWriteHeaderContents(XmlDictionaryWriter writer, MessageVersion messageVersion)
        {
            writer.WriteStartElement(PropagationTokenElement, OleTxNamespace);
            writer.WriteBase64(_propagationToken, 0, _propagationToken.Length);
            writer.WriteEndElement();
        }

        public static OleTxTransactionHeader ReadFrom(Message message)
        {
            int index;
            try
            {
                index = message.Headers.FindHeader(OleTxHeaderElement, OleTxNamespace);
            }
            catch (MessageHeaderException e)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new TransactionException(SRP.OleTxHeaderCorrupt, e));
            }

            if (index < 0)
                return null;

            OleTxTransactionHeader oleTxHeader;
            XmlDictionaryReader reader = message.Headers.GetReaderAtHeader(index);
            using (reader)
            {
                try
                {
                    oleTxHeader = ReadFrom(reader);
                }
                catch (XmlException xe)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new TransactionException(SRP.OleTxHeaderCorrupt, xe));
                }
            }

            MessageHeaderInfo header = message.Headers[index];
            if (!message.Headers.UnderstoodHeaders.Contains(header))
            {
                message.Headers.UnderstoodHeaders.Add(header);
            }

            return oleTxHeader;
        }

        private static OleTxTransactionHeader ReadFrom(XmlDictionaryReader reader)
        {
            reader.ReadFullStartElement(OleTxHeaderElement, OleTxNamespace);

            byte[] propagationToken = ReadPropagationTokenElement(reader);

            // Skip extensibility elements
            while (reader.IsStartElement())
            {
                reader.Skip();
            }
            reader.ReadEndElement();

            return new OleTxTransactionHeader(propagationToken);
        }

        public static bool IsStartPropagationTokenElement(XmlDictionaryReader reader)
        {
            return reader.IsStartElement(PropagationTokenElement, OleTxNamespace);
        }

        public static byte[] ReadPropagationTokenElement(XmlDictionaryReader reader)
        {
            reader.ReadFullStartElement(PropagationTokenElement, OleTxNamespace);

            byte[] propagationToken = reader.ReadContentAsBase64();
            if (propagationToken.Length == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new XmlException(SRP.InvalidPropagationToken));
            }

            reader.ReadEndElement();

            return propagationToken;
        }
    }
}
