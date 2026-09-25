// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Transactions;

namespace System.ServiceModel.Channels
{
    // Bridges a System.Transactions.Transaction into the
    // ITransaction COM interface that mqrt!MQSendMessage expects.
    //
    // TransactionInterop.GetDtcTransaction returns an IDtcTransaction
    // whose IID is exactly the OLE ITransaction IID, so the COM
    // pointer we marshal here can be passed straight through to
    // MQSendMessage without an extra QueryInterface.
    //
    // The COM ref-counting contract is:
    //   * GetComInterfaceForObject returns a pointer with AddRef.
    //   * MQSendMessage either holds onto it for the duration of the
    //     two-phase commit or copies whatever it needs and returns.
    //   * We Release in a finally, balancing the AddRef.
    //
    // On .NET 8+, the runtime no longer promotes a local transaction
    // to a distributed one implicitly; callers wanting to flow
    // Transaction.Current through MSMQ must opt in via
    //   "System.Transactions.EnableImplicitDistributedTransactions": "true"
    // in their runtimeconfig.json. Without it, GetDtcTransaction itself
    // throws NotSupportedException long before we get to the COM
    // marshaling.
    [SupportedOSPlatform("windows")]
    internal static class DtcTransactionBridge
    {
        internal static IntPtr AcquireITransactionPointer(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }
            IDtcTransaction dtcTransaction = TransactionInterop.GetDtcTransaction(transaction);
            return Marshal.GetComInterfaceForObject(dtcTransaction, typeof(IDtcTransaction));
        }

        internal static void ReleaseITransactionPointer(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                Marshal.Release(ptr);
            }
        }
    }
}
