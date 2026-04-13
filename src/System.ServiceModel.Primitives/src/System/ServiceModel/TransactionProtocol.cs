// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    public abstract class TransactionProtocol
    {
        public static TransactionProtocol Default
        {
            get { return OleTransactions; }
        }

        public static TransactionProtocol OleTransactions
        {
            get { return OleTransactionsProtocol.Instance; }
        }

        public static TransactionProtocol WSAtomicTransactionOctober2004
        {
            get { return WSAtomicTransactionOctober2004Protocol.Instance; }
        }

        public static TransactionProtocol WSAtomicTransaction11
        {
            get { return WSAtomicTransaction11Protocol.Instance; }
        }

        internal abstract string Name { get; }

        internal static bool IsDefined(TransactionProtocol transactionProtocol)
        {
            return transactionProtocol == OleTransactions ||
                   transactionProtocol == WSAtomicTransactionOctober2004 ||
                   transactionProtocol == WSAtomicTransaction11;
        }
    }

    internal class OleTransactionsProtocol : TransactionProtocol
    {
        internal static TransactionProtocol Instance { get; } = new OleTransactionsProtocol();

        internal override string Name => "OleTransactions";
    }

    internal class WSAtomicTransactionOctober2004Protocol : TransactionProtocol
    {
        internal static TransactionProtocol Instance { get; } = new WSAtomicTransactionOctober2004Protocol();

        internal override string Name => "WSAtomicTransactionOctober2004";
    }

    internal class WSAtomicTransaction11Protocol : TransactionProtocol
    {
        internal static TransactionProtocol Instance { get; } = new WSAtomicTransaction11Protocol();

        internal override string Name => "WSAtomicTransaction11";
    }
}
