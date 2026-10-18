// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel.MsmqIntegration
{
    // Mirrors System.Messaging.Acknowledgment. Values correspond to the
    // MQMSG_CLASS_* classifications set by MSMQ on incoming admin
    // messages to indicate the disposition of a previously sent
    // message. Never set by user code; surfaced via
    // MsmqMessage<T>.Acknowledgment.
    public enum Acknowledgment
    {
        None = 0,
        ReachQueue = 0x0002,
        Receive = 0x4000,
        QueuePurged = 0x4001,
        ReceiveTimeout = 0x4002,
        ReachQueueTimeout = 0x4003,
        ReceiveRejected = 0x4005,
        BadDestinationQueue = 0x8000,
        Purged = 0x8001,
        QueueExceedMaximumSize = 0x8002,
        AccessDenied = 0x8004,
        HopCountExceeded = 0x8005,
        BadSignature = 0x8006,
        BadEncryption = 0x8007,
        CouldNotEncrypt = 0x8008,
        NotTransactionalQueue = 0x8009,
        NotTransactionalMessage = 0x800A,
    }
}
