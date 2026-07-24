// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;
using System.Transactions;

namespace System.ServiceModel.Channels
{
    // Send-side facade over the native MSMQ queue. Owns the
    // SafeMsmqQueueHandle for its lifetime and serializes calls
    // through MQSendMessage with the appropriate transaction handle.
    //
    // Stateless w.r.t. property layout — the caller passes a fully
    // populated NativeMsmqMessage, and MsmqQueue is responsible only
    // for opening the queue, calling Send, mapping the HRESULT, and
    // managing DTC enlistment.
    [SupportedOSPlatform("windows")]
    internal sealed class MsmqQueue : IDisposable
    {
        private readonly SafeMsmqQueueHandle _handle;
        private bool _disposed;

        private MsmqQueue(SafeMsmqQueueHandle handle)
        {
            _handle = handle;
        }

        internal static MsmqQueue OpenForSend(string formatName)
        {
            SafeMsmqQueueHandle handle = SafeMsmqQueueHandle.OpenForSend(formatName);
            return new MsmqQueue(handle);
        }

        // Sends a single message. txMode selects between non-transactional,
        // MSMQ-single-message, and DTC-flowed.
        internal void Send(NativeMsmqMessage message, MsmqTransactionMode txMode)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MsmqQueue));
            }
            IntPtr propsPtr = message.Freeze();

            int hr;
            switch (txMode)
            {
                case MsmqTransactionMode.None:
                    hr = UnsafeNativeMethods.MQSendMessage(_handle, propsPtr, UnsafeNativeMethods.MQ_NO_TRANSACTION);
                    break;
                case MsmqTransactionMode.Single:
                    hr = UnsafeNativeMethods.MQSendMessage(_handle, propsPtr, UnsafeNativeMethods.MQ_SINGLE_MESSAGE);
                    break;
                case MsmqTransactionMode.Automatic:
                    IntPtr dtcPtr = DtcTransactionBridge.AcquireITransactionPointer(Transaction.Current);
                    try
                    {
                        hr = UnsafeNativeMethods.MQSendMessage(_handle, propsPtr, dtcPtr);
                    }
                    finally
                    {
                        DtcTransactionBridge.ReleaseITransactionPointer(dtcPtr);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(txMode));
            }

            // FAILED(hr): the high bit being set indicates a true
            // failure. Warnings such as MQ_INFORMATION_PROPERTY
            // (0x400E0001) mean the call still completed and the
            // message was enqueued — they should not surface as
            // exceptions. This matches COM's SUCCEEDED/FAILED contract
            // and the .NET Framework MSMQ reference source.
            if (hr < 0)
            {
                throw new MsmqException(SR.Format(SR.MsmqSendFailed, hr.ToString("x8")), hr).Normalized;
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            _handle?.Dispose();
        }
    }
}
