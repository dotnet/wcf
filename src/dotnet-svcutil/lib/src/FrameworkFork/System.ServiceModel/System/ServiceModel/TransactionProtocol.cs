// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    using System.ComponentModel;

    // TODO: [TypeConverter(typeof(TransactionProtocolConverter))]
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

        internal abstract string Name
        {
            get;
        }

        internal static bool IsDefined(TransactionProtocol transactionProtocol)
        {
            return transactionProtocol == TransactionProtocol.OleTransactions ||
                   transactionProtocol == TransactionProtocol.WSAtomicTransactionOctober2004 ||
                   transactionProtocol == TransactionProtocol.WSAtomicTransaction11;
        }
    }

    internal class OleTransactionsProtocol : TransactionProtocol
    {
        private static TransactionProtocol s_instance = new OleTransactionsProtocol();

        internal static TransactionProtocol Instance
        {
            get { return s_instance; }
        }

        internal override string Name
        {
            get { return "OleTransactions"; } // TODO: ConfigurationStrings.OleTransactions; }
        }
    }

    internal class WSAtomicTransactionOctober2004Protocol : TransactionProtocol
    {
        private static TransactionProtocol s_instance = new WSAtomicTransactionOctober2004Protocol();

        internal static TransactionProtocol Instance
        {
            get { return s_instance; }
        }

        internal override string Name
        {
            get { return "WSAtomicTransactionOctober2004"; } // TODO: ConfigurationStrings.WSAtomicTransactionOctober2004; }
        }
    }

    internal class WSAtomicTransaction11Protocol : TransactionProtocol
    {
        private static TransactionProtocol s_instance = new WSAtomicTransaction11Protocol();

        internal static TransactionProtocol Instance
        {
            get { return s_instance; }
        }

        internal override string Name
        {
            get { return "WSAtomicTransaction11"; } //TODO: ConfigurationStrings.WSAtomicTransaction11; }
        }
    }
}
