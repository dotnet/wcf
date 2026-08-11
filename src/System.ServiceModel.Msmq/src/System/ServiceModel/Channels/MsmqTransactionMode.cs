// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.Channels
{
    // Selects the pTransaction argument passed to mqrt!MQSendMessage.
    // Determined by GetTransactionMode in MsmqMessagingInterop based
    // on the binding's ExactlyOnce flag and the ambient
    // System.Transactions.Transaction.Current.
    internal enum MsmqTransactionMode
    {
        // Non-transactional send (MQ_NO_TRANSACTION / NULL).
        None = 0,

        // Single-message MSMQ transaction (MQ_SINGLE_MESSAGE / (ITransaction*)3).
        Single = 1,

        // DTC-flowed transaction — we marshal Transaction.Current as
        // an IDtcTransaction COM pointer.
        Automatic = 2,
    }
}
