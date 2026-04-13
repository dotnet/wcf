// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Channels
{
    internal static class TransactionFlowDefaults
    {
        internal const TransactionFlowOption IssuedTokens = TransactionFlowOption.NotAllowed;
        internal const bool Transactions = false;
        internal static TransactionProtocol TransactionProtocol => TransactionProtocol.OleTransactions;
    }
}
