// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Diagnostics;
using System.Net.Security;
using System.Runtime.Versioning;
using System.ServiceModel.MsmqIntegration;
using System.Transactions;

namespace System.ServiceModel.Channels
{
    // Send-path facade. Folds the binding configuration plus an optional
    // MsmqIntegrationMessageProperty bag into a NativeMsmqMessage and
    // dispatches it through MsmqQueue.Send. All MSMQ-specific marshaling
    // and DTC enlistment live in the dedicated native layer
    // (UnsafeNativeMethods / NativeMsmqMessage / MsmqQueue /
    // DtcTransactionBridge). This file owns only the "fold caller intent
    // into native primitives" decisions.
    [SupportedOSPlatform("windows")]
    internal static class MsmqMessagingInterop
    {
        // The ambient transaction is passed in explicitly rather than read
        // from Transaction.Current here: Transaction.Current is thread-static
        // and the asynchronous send path runs this method on a thread-pool
        // thread, where the calling thread's ambient transaction is not
        // visible.
        internal static void Send(
            string formatName,
            byte[] body,
            int offset,
            int count,
            MsmqIntegrationMessageProperty property,
            MsmqBindingElementBase bindingElement,
            TimeSpan sendTimeout,
            Transaction ambientTransaction)
        {
            if (bindingElement == null)
            {
                throw new ArgumentNullException(nameof(bindingElement));
            }

            using NativeMsmqMessage message = new NativeMsmqMessage();
            PopulateBaseProperties(message, body, offset, count, bindingElement);
            property?.ApplyTo(message);
            SendCore(formatName, message, bindingElement.ExactlyOnce, sendTimeout, ambientTransaction);
        }

        // Writes the transport-level properties the binding controls. Every one
        // of these is settable on NetMsmqBinding, so leaving any of them
        // unwritten silently downgrades the message relative to what the caller
        // configured.
        private static void PopulateBaseProperties(
            NativeMsmqMessage message,
            byte[] body,
            int offset,
            int count,
            MsmqBindingElementBase bindingElement)
        {
            message.SetBody(body, offset, count);

            // PROPID_M_DELIVERY. Without this MSMQ defaults to EXPRESS, which
            // keeps the message in volatile memory only — it is lost if the
            // queue manager restarts. ExactlyOnce implies a transactional send,
            // and MSMQ requires recoverable delivery for those.
            message.SetByte(
                UnsafeNativeMethods.PROPID_M_DELIVERY,
                bindingElement.Durable || bindingElement.ExactlyOnce
                    ? UnsafeNativeMethods.MQMSG_DELIVERY_RECOVERABLE
                    : UnsafeNativeMethods.MQMSG_DELIVERY_EXPRESS);

            // PROPID_M_JOURNAL is a bit field: MQMSG_JOURNAL keeps a copy of
            // every successfully sent message in the source machine journal,
            // MQMSG_DEADLETTER routes undeliverable messages to a dead-letter
            // queue instead of discarding them.
            int journal = UnsafeNativeMethods.MQMSG_JOURNAL_NONE;
            if (bindingElement.UseSourceJournal)
            {
                journal |= UnsafeNativeMethods.MQMSG_JOURNAL;
            }
            if (bindingElement.DeadLetterQueue != DeadLetterQueue.None)
            {
                journal |= UnsafeNativeMethods.MQMSG_DEADLETTER;
            }
            message.SetByte(UnsafeNativeMethods.PROPID_M_JOURNAL, (byte)journal);

            if (bindingElement.DeadLetterQueue == DeadLetterQueue.Custom
                && bindingElement.CustomDeadLetterQueue != null)
            {
                message.SetWideString(
                    UnsafeNativeMethods.PROPID_M_DEADLETTER_QUEUE,
                    MsmqUri.UriToFormatNameByScheme(bindingElement.CustomDeadLetterQueue),
                    UnsafeNativeMethods.PROPID_M_DEADLETTER_QUEUE_LEN);
            }

            if (bindingElement.UseMsmqTracing)
            {
                message.SetByte(
                    UnsafeNativeMethods.PROPID_M_TRACE,
                    UnsafeNativeMethods.MQMSG_SEND_ROUTE_TO_REPORT_QUEUE);
            }

            if (bindingElement.TimeToLive != default)
            {
                message.SetTimeToBeReceived(bindingElement.TimeToLive);
            }

            ApplyTransportSecurity(message, bindingElement.MsmqTransportSecurity);
        }

        // Maps MsmqTransportSecurity onto the native authentication and privacy
        // propids. MSMQ enforces these itself: MQMSG_AUTH_LEVEL_ALWAYS makes the
        // queue manager attach and verify a digital signature, and a non-zero
        // privacy level makes it encrypt the body.
        internal static void ApplyTransportSecurity(NativeMsmqMessage message, MsmqTransportSecurity security)
        {
            if (security == null || !security.Enabled)
            {
                message.SetUInt32(UnsafeNativeMethods.PROPID_M_AUTH_LEVEL, UnsafeNativeMethods.MQMSG_AUTH_LEVEL_NONE);
                message.SetUInt32(UnsafeNativeMethods.PROPID_M_PRIV_LEVEL, UnsafeNativeMethods.MQMSG_PRIV_LEVEL_NONE);
                return;
            }

            message.SetUInt32(UnsafeNativeMethods.PROPID_M_AUTH_LEVEL, UnsafeNativeMethods.MQMSG_AUTH_LEVEL_ALWAYS);
            message.SetUInt32(
                UnsafeNativeMethods.PROPID_M_HASH_ALG,
                ToHashAlgorithmId(security.MsmqSecureHashAlgorithm));

            if (security.MsmqProtectionLevel == ProtectionLevel.EncryptAndSign)
            {
                message.SetUInt32(
                    UnsafeNativeMethods.PROPID_M_PRIV_LEVEL,
                    ToPrivacyLevel(security.MsmqEncryptionAlgorithm));
                message.SetUInt32(
                    UnsafeNativeMethods.PROPID_M_ENCRYPTION_ALG,
                    ToEncryptionAlgorithmId(security.MsmqEncryptionAlgorithm));
            }
            else
            {
                message.SetUInt32(UnsafeNativeMethods.PROPID_M_PRIV_LEVEL, UnsafeNativeMethods.MQMSG_PRIV_LEVEL_NONE);
            }
        }

        internal static uint ToHashAlgorithmId(MsmqSecureHashAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case MsmqSecureHashAlgorithm.MD5:
                    return UnsafeNativeMethods.CALG_MD5;
                case MsmqSecureHashAlgorithm.Sha1:
                    return UnsafeNativeMethods.CALG_SHA1;
                case MsmqSecureHashAlgorithm.Sha512:
                    return UnsafeNativeMethods.CALG_SHA_512;
                default:
                    return UnsafeNativeMethods.CALG_SHA_256;
            }
        }

        internal static uint ToEncryptionAlgorithmId(MsmqEncryptionAlgorithm algorithm)
        {
            return algorithm == MsmqEncryptionAlgorithm.Aes
                ? UnsafeNativeMethods.CALG_AES
                : UnsafeNativeMethods.CALG_RC4;
        }

        // MSMQ selects the body-encryption strength from PROPID_M_PRIV_LEVEL
        // rather than from the algorithm id alone; AES requires the dedicated
        // MQMSG_PRIV_LEVEL_BODY_AES level.
        internal static uint ToPrivacyLevel(MsmqEncryptionAlgorithm algorithm)
        {
            return algorithm == MsmqEncryptionAlgorithm.Aes
                ? UnsafeNativeMethods.MQMSG_PRIV_LEVEL_BODY_AES
                : UnsafeNativeMethods.MQMSG_PRIV_LEVEL_BODY_ENHANCED;
        }

        // Picks the MSMQ transaction mode that matches the binding's
        // ExactlyOnce contract and the ambient System.Transactions
        // transaction:
        //
        //   ExactlyOnce  ambient transaction   MSMQ mode
        //   -----------  -------------------   -----------
        //   true         non-null              Automatic   (enlist in ambient DTC tx)
        //   true         null                  Single      (start a one-shot MSMQ tx)
        //   false        any                   None        (non-transactional send)
        //
        // MsmqTransactionMode.Automatic delegates enlistment to mqrt.dll
        // which uses the native MSMQ DTC integration — the MSMQ send
        // commits or aborts atomically with any other resource managers
        // participating in the ambient transaction.
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
            TimeSpan sendTimeout,
            Transaction ambientTransaction)
        {
            // mqrt!MQSendMessage takes no per-call timeout: the send is an
            // enqueue against the local queue manager, and there is no way to
            // cancel the blocking call once it is in flight. The caller's
            // timeout is therefore enforced as a budget around the whole
            // open + send operation rather than being pushed into MSMQ. It is
            // deliberately NOT mapped onto PROPID_M_TIME_TO_REACH_QUEUE: that
            // property controls how long MSMQ keeps trying to deliver the
            // message, which is a different contract, is separately settable
            // through MsmqIntegrationMessageProperty.TimeToReachQueue, and
            // would cause messages to be silently discarded.
            if (sendTimeout <= TimeSpan.Zero)
            {
                throw new TimeoutException(SR.Format(SR.MsmqSendTimedOut, sendTimeout));
            }

            long startTimestamp = Stopwatch.GetTimestamp();
            using (MsmqQueue queue = MsmqQueue.OpenForSend(formatName))
            {
                MsmqTransactionMode mode = GetTransactionMode(exactlyOnce, ambientTransaction);
                queue.Send(message, mode, ambientTransaction);
            }

            if (sendTimeout != TimeSpan.MaxValue
                && Stopwatch.GetElapsedTime(startTimestamp) > sendTimeout)
            {
                throw new TimeoutException(SR.Format(SR.MsmqSendTimedOut, sendTimeout));
            }
        }
    }
}
