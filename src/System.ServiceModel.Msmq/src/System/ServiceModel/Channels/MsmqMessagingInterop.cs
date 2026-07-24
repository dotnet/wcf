// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;
using System.ServiceModel.MsmqIntegration;
using System.Transactions;

namespace System.ServiceModel.Channels
{
    // Send-path facade. Walks an outgoing payload + optional
    // MsmqIntegrationMessageProperty bag into a NativeMsmqMessage and
    // dispatches it through MsmqQueue.Send. All MSMQ-specific marshaling
    // and DTC enlistment live in the dedicated native layer
    // (UnsafeNativeMethods / NativeMsmqMessage / MsmqQueue /
    // DtcTransactionBridge). This file is intentionally small — it owns
    // only the "fold caller intent into native primitives" decisions.
    [SupportedOSPlatform("windows")]
    internal static class MsmqMessagingInterop
    {
        internal static void Send(
            string formatName,
            byte[] body,
            int offset,
            int count,
            bool exactlyOnce,
            TimeSpan timeToLive,
            TimeSpan sendTimeout)
        {
            using var message = new NativeMsmqMessage();
            PopulateBaseProperties(message, body, offset, count, timeToLive);
            SendCore(formatName, message, exactlyOnce, sendTimeout);
        }

        internal static void Send(
            string formatName,
            byte[] body,
            int offset,
            int count,
            MsmqIntegrationMessageProperty property,
            bool exactlyOnce,
            TimeSpan timeToLive,
            TimeSpan sendTimeout)
        {
            using var message = new NativeMsmqMessage();
            PopulateBaseProperties(message, body, offset, count, timeToLive);
            property?.ApplyTo(message);
            SendCore(formatName, message, exactlyOnce, sendTimeout);
        }

        private static void PopulateBaseProperties(NativeMsmqMessage message, byte[] body, int offset, int count, TimeSpan timeToLive)
        {
            message.SetBody(body, offset, count);
            if (timeToLive != default)
            {
                message.SetTimeToBeReceived(timeToLive);
            }
        }

        // Picks the MSMQ transaction mode that matches the binding's
        // ExactlyOnce contract and the ambient System.Transactions
        // transaction:
        //
        //   ExactlyOnce  Transaction.Current   MSMQ mode
        //   -----------  -------------------   -----------
        //   true         non-null              Automatic   (enlist in ambient DTC tx)
        //   true         null                  Single      (start a one-shot MSMQ tx)
        //   false        any                   None        (non-transactional send)
        //
        // MsmqTransactionMode.Automatic delegates enlistment to mqrt.dll
        // which uses the native MSMQ DTC integration — the MSMQ send
        // commits or aborts atomically with any other resource managers
        // participating in Transaction.Current.
        internal static MsmqTransactionMode GetTransactionMode(bool exactlyOnce, Transaction ambient)
        {
            if (!exactlyOnce)
            {
                return MsmqTransactionMode.None;
            }
            return ambient != null
                ? MsmqTransactionMode.Automatic
                : MsmqTransactionMode.Single;
        }

        private static void SendCore(
            string formatName,
            NativeMsmqMessage message,
            bool exactlyOnce,
            TimeSpan sendTimeout)
        {
            _ = sendTimeout; // mqrt!MQSendMessage has no per-call timeout.
            using MsmqQueue queue = MsmqQueue.OpenForSend(formatName);
            MsmqTransactionMode mode = GetTransactionMode(exactlyOnce, Transaction.Current);
            queue.Send(message, mode);
        }
    }
}

