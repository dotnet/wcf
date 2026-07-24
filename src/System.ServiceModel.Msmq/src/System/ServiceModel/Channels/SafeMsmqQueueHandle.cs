// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace System.ServiceModel.Channels
{
    // SafeHandle wrapper around an MSMQ queue handle (QUEUEHANDLE).
    // Ensures mqrt!MQCloseQueue is called even if the owning channel
    // is finalized before close, and prevents the handle being passed
    // to native code while a release is in flight.
    [SupportedOSPlatform("windows")]
    internal sealed class SafeMsmqQueueHandle : SafeHandle
    {
        private SafeMsmqQueueHandle() : base(IntPtr.Zero, ownsHandle: true)
        {
        }

        public override bool IsInvalid => handle == IntPtr.Zero;

        protected override bool ReleaseHandle()
        {
            // mqrt!MQCloseQueue returns MQ_OK on success. We swallow
            // any failure here because the runtime calls us from the
            // finalizer queue or from Dispose and has no recovery path.
            return UnsafeNativeMethods.MQCloseQueue(handle) == UnsafeNativeMethods.S_OK;
        }

        // Opens a queue for send access. Throws MsmqException.Normalized
        // on failure, matching the rest of the send pipeline.
        //
        // `formatName` is the raw MSMQ format name as produced by the
        // MsmqUri translator (e.g. "DIRECT=OS:.\\private$\\queue"). Some
        // callers and string-handling layers in WCF prepend the
        // "FormatName:" prefix that System.Messaging public APIs accept;
        // mqrt!MQOpenQueue rejects that prefix as
        // MQ_ERROR_ILLEGAL_FORMATNAME, so we strip it here as a
        // convenience.
        internal static SafeMsmqQueueHandle OpenForSend(string formatName)
        {
            if (formatName == null)
            {
                throw new ArgumentNullException(nameof(formatName));
            }
            if (formatName.StartsWith("FormatName:", StringComparison.OrdinalIgnoreCase))
            {
                formatName = formatName.Substring("FormatName:".Length);
            }

            int hr = UnsafeNativeMethods.MQOpenQueue(
                formatName,
                UnsafeNativeMethods.MQ_SEND_ACCESS,
                UnsafeNativeMethods.MQ_DENY_NONE,
                out IntPtr rawHandle);

            // FAILED(hr): same COM contract as MsmqQueue.Send — only
            // treat the high bit being set as a real failure. Warnings
            // such as MQ_INFORMATION_FORMATNAME_BUFFER_TOO_SMALL
            // (returned by MQOpenQueue in some edge cases) should not
            // throw because the queue handle was still produced.
            if (hr < 0)
            {
                throw new MsmqException(SR.Format(SR.MsmqOpenQueueFailed, formatName), hr).Normalized;
            }

            var safe = new SafeMsmqQueueHandle();
            safe.SetHandle(rawHandle);
            return safe;
        }
    }
}
