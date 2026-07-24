// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Transactions;

namespace System.ServiceModel.Transactions
{
    internal class OleTxTransactionInfo : TransactionInfo
    {
        private OleTxTransactionHeader _header;

        public OleTxTransactionInfo(OleTxTransactionHeader header)
        {
            _header = header;
        }

        public override Transaction UnmarshalTransaction()
        {
            return TransactionInterop.GetTransactionFromTransmitterPropagationToken(_header.PropagationToken);
        }
    }
}
