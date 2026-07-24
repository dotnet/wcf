// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
#else
using System.ServiceModel;
#endif

namespace WcfService
{
    [ServiceContract]
    public interface IWcfTransactionService
    {
        [OperationContract]
#if !NET
        [TransactionFlow(TransactionFlowOption.Allowed)]
#endif
        bool IsTransactionFlowed();

        [OperationContract]
#if !NET
        [TransactionFlow(TransactionFlowOption.Allowed)]
#endif
        void ThrowDuringTransaction();
    }
}
