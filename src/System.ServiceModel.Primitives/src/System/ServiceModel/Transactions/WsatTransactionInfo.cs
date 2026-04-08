// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Transactions;

namespace System.ServiceModel.Transactions
{
    internal class WsatTransactionInfo : TransactionInfo
    {
        private WsatTransactionHeader _header;

        public WsatTransactionInfo(WsatTransactionHeader header)
        {
            _header = header;
        }

        public override Transaction UnmarshalTransaction()
        {
            if (_header.PropagationToken != null)
            {
                return TransactionInterop.GetTransactionFromTransmitterPropagationToken(_header.PropagationToken);
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                new TransactionException(SRP.UnmarshalTransactionFaulted));
        }
    }
}
