// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.ServiceModel.Channels;
using System.Transactions;

namespace System.ServiceModel.Transactions
{
    internal abstract class TransactionFormatter
    {
        // Factory pattern: choose Windows vs Unsupported implementation at static init time.
        // This prevents JIT from loading Windows-only types on non-Windows platforms.
        private static readonly ITransactionFormatterFactory s_factory =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new WindowsTransactionFormatterFactory()
                : new UnsupportedTransactionFormatterFactory();

        public static TransactionFormatter OleTxFormatter => s_factory.CreateOleTxFormatter();

        public static TransactionFormatter WsatFormatter10 => s_factory.CreateWsatFormatter10();

        public static TransactionFormatter WsatFormatter11 => s_factory.CreateWsatFormatter11();

        public abstract MessageHeader EmptyTransactionHeader { get; }

        public abstract void WriteTransaction(Transaction transaction, Message message);

        public abstract TransactionInfo ReadTransaction(Message message);
    }

    internal abstract class TransactionInfo
    {
        public abstract Transaction UnmarshalTransaction();
    }

    internal interface ITransactionFormatterFactory
    {
        TransactionFormatter CreateOleTxFormatter();
        TransactionFormatter CreateWsatFormatter10();
        TransactionFormatter CreateWsatFormatter11();
    }

    internal class UnsupportedTransactionFormatterFactory : ITransactionFormatterFactory
    {
        public TransactionFormatter CreateOleTxFormatter() => new UnsupportedTransactionFormatter();
        public TransactionFormatter CreateWsatFormatter10() => new UnsupportedTransactionFormatter();
        public TransactionFormatter CreateWsatFormatter11() => new UnsupportedTransactionFormatter();
    }

    internal class UnsupportedTransactionFormatter : TransactionFormatter
    {
        public override MessageHeader EmptyTransactionHeader =>
            throw new PlatformNotSupportedException(SRP.TransactionsNotSupported);

        public override void WriteTransaction(Transaction transaction, Message message)
        {
            throw new PlatformNotSupportedException(SRP.TransactionsNotSupported);
        }

        public override TransactionInfo ReadTransaction(Message message)
        {
            throw new PlatformNotSupportedException(SRP.TransactionsNotSupported);
        }
    }

    internal class WindowsTransactionFormatterFactory : ITransactionFormatterFactory
    {
        private static readonly Lazy<TransactionFormatter> s_oleTxFormatter =
            new Lazy<TransactionFormatter>(() => new OleTxTransactionFormatter());

        private static readonly Lazy<TransactionFormatter> s_wsatFormatter10 =
            new Lazy<TransactionFormatter>(() => new WsatTransactionFormatter10());

        private static readonly Lazy<TransactionFormatter> s_wsatFormatter11 =
            new Lazy<TransactionFormatter>(() => new WsatTransactionFormatter11());

        public TransactionFormatter CreateOleTxFormatter() => s_oleTxFormatter.Value;
        public TransactionFormatter CreateWsatFormatter10() => s_wsatFormatter10.Value;
        public TransactionFormatter CreateWsatFormatter11() => s_wsatFormatter11.Value;
    }
}
