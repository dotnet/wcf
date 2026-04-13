// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !NET
using System.ServiceModel;
using System.Transactions;

namespace WcfService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class WcfTransactionService : IWcfTransactionService
    {
        [OperationBehavior(TransactionScopeRequired = true)]
        public bool IsTransactionFlowed()
        {
            return Transaction.Current != null;
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public void ThrowDuringTransaction()
        {
            throw new FaultException("Intentional service fault during transaction");
        }
    }
}
#endif
