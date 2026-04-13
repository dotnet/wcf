// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.Transactions;

namespace System.ServiceModel.Transactions
{
    internal abstract class WsatTransactionFormatter : TransactionFormatter
    {
        private string _headerElement;
        private string _coordinationNamespace;
        private string _atomicTransactionNamespace;
        private AddressingVersion _addressingVersion;

        protected WsatTransactionFormatter(
            string headerElement,
            string coordinationNamespace,
            string atomicTransactionNamespace,
            AddressingVersion addressingVersion)
        {
            _headerElement = headerElement;
            _coordinationNamespace = coordinationNamespace;
            _atomicTransactionNamespace = atomicTransactionNamespace;
            _addressingVersion = addressingVersion;
        }

        public override void WriteTransaction(Transaction transaction, Message message)
        {
            // Force promotion to a distributed transaction
            byte[] propToken = TransactionInterop.GetTransmitterPropagationToken(transaction);

            Guid transactionId = transaction.TransactionInformation.DistributedIdentifier;

            // Use a default timeout; the actual transaction timeout is not easily accessible
            // from Transaction.Current in .NET. The server uses OleTx upgrade to unmarshal
            // the transaction directly from the propagation token, so the Expires value
            // is informational only.
            uint expires = 0;

            WsatTransactionHeader header = new WsatTransactionHeader(
                transactionId,
                expires,
                propToken,
                _headerElement,
                _coordinationNamespace,
                _atomicTransactionNamespace,
                _addressingVersion);

            message.Headers.Add(header);
        }

        public override TransactionInfo ReadTransaction(Message message)
        {
            WsatTransactionHeader header = WsatTransactionHeader.ReadFrom(message, _headerElement, _coordinationNamespace);
            if (header == null)
                return null;

            return new WsatTransactionInfo(header);
        }
    }

    internal class WsatTransactionFormatter10 : WsatTransactionFormatter
    {
        private static readonly WsatTransactionHeader s_emptyHeader =
            new WsatTransactionHeader(null, CoordinationExternalStrings.CoordinationContext, CoordinationExternal10Strings.Namespace);

        public WsatTransactionFormatter10()
            : base(CoordinationExternalStrings.CoordinationContext,
                   CoordinationExternal10Strings.Namespace,
                   AtomicTransactionExternal10Strings.Namespace,
                   AddressingVersion.WSAddressingAugust2004)
        {
        }

        public override MessageHeader EmptyTransactionHeader => s_emptyHeader;
    }

    internal class WsatTransactionFormatter11 : WsatTransactionFormatter
    {
        private static readonly WsatTransactionHeader s_emptyHeader =
            new WsatTransactionHeader(null, CoordinationExternalStrings.CoordinationContext, CoordinationExternal11Strings.Namespace);

        public WsatTransactionFormatter11()
            : base(CoordinationExternalStrings.CoordinationContext,
                   CoordinationExternal11Strings.Namespace,
                   AtomicTransactionExternal11Strings.Namespace,
                   AddressingVersion.WSAddressing10)
        {
        }

        public override MessageHeader EmptyTransactionHeader => s_emptyHeader;
    }
}
