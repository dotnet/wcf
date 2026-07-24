// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.Transactions;

namespace System.ServiceModel.Transactions
{
    internal class OleTxTransactionFormatter : TransactionFormatter
    {
        private static readonly OleTxTransactionHeader s_emptyTransactionHeader = new OleTxTransactionHeader(null);

        public override MessageHeader EmptyTransactionHeader => s_emptyTransactionHeader;

        public override void WriteTransaction(Transaction transaction, Message message)
        {
            byte[] propToken = TransactionInterop.GetTransmitterPropagationToken(transaction);
            OleTxTransactionHeader header = new OleTxTransactionHeader(propToken);
            message.Headers.Add(header);
        }

        public override TransactionInfo ReadTransaction(Message message)
        {
            OleTxTransactionHeader header = OleTxTransactionHeader.ReadFrom(message);
            if (header == null)
                return null;

            return new OleTxTransactionInfo(header);
        }
    }
}
