// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Transactions;

namespace System.ServiceModel.Channels
{
    // Native MSMQ P/Invoke surface used by the send path. Only the
    // subset of mqrt.dll required by the client-side IOutputChannel
    // is declared here; the receive side lives in CoreWCF.
    //
    // Layout notes:
    //   * MQMSGPROPS is a struct of pointers — we marshal it manually
    //     through Marshal.AllocHGlobal'd buffers in NativeMsmqMessage,
    //     and the entry point takes IntPtr so we can pass it ref-free.
    //   * MQPROPVARIANT is a tagged union whose vector-member offsets
    //     depend on pointer size. We write each slot byte-by-byte via
    //     Marshal.Write* using NativeMsmqMessage.PropVariantSize and
    //     VectorElementsOffset, so the same code works on x86 and x64.
    //   * MQOpenQueue requires the queue name as a wide string; the
    //     CharSet is Unicode on every entry point.
    //
    // Transaction handle conventions, copied straight from mqmsg.h:
    //   MQ_NO_TRANSACTION   = NULL (IntPtr.Zero)
    //   MQ_MTS_TRANSACTION  = (ITransaction*)1
    //   MQ_XA_TRANSACTION   = (ITransaction*)2
    //   MQ_SINGLE_MESSAGE   = (ITransaction*)3
    // For DTC enlistment we pass an IDtcTransaction COM pointer (same
    // IID as the OLE ITransaction interface) obtained from
    // TransactionInterop.GetDtcTransaction.
    [SupportedOSPlatform("windows")]
    internal static class UnsafeNativeMethods
    {
        internal const string MqrtDll = "mqrt.dll";

        // -- MQOpenQueue dwAccess flags --
        internal const int MQ_RECEIVE_ACCESS = 0x00000001;
        internal const int MQ_SEND_ACCESS    = 0x00000002;
        internal const int MQ_PEEK_ACCESS    = 0x00000020;

        // -- MQOpenQueue dwShareMode flags --
        internal const int MQ_DENY_NONE             = 0x00000000;
        internal const int MQ_DENY_RECEIVE_SHARE    = 0x00000001;

        // -- VARTYPE values used by MSMQ properties --
        internal const ushort VT_NULL    = 1;
        internal const ushort VT_I2      = 2;
        internal const ushort VT_I4      = 3;
        internal const ushort VT_UI1     = 17;
        internal const ushort VT_UI2     = 18;
        internal const ushort VT_UI4     = 19;
        internal const ushort VT_LPWSTR  = 31;
        internal const ushort VT_VECTOR  = 0x1000;

        // -- PROPID_M_* property identifiers (subset used on the send path) --
        internal const uint PROPID_M_CLASS                = 1;
        internal const uint PROPID_M_MSGID                = 2;
        internal const uint PROPID_M_CORRELATIONID        = 3;
        internal const uint PROPID_M_PRIORITY             = 4;
        internal const uint PROPID_M_DELIVERY             = 5;
        internal const uint PROPID_M_ACKNOWLEDGE          = 6;
        internal const uint PROPID_M_JOURNAL              = 7;
        internal const uint PROPID_M_APPSPECIFIC          = 8;
        internal const uint PROPID_M_BODY                 = 9;
        internal const uint PROPID_M_BODY_SIZE            = 10;
        internal const uint PROPID_M_LABEL                = 11;
        internal const uint PROPID_M_LABEL_LEN            = 12;
        internal const uint PROPID_M_TIME_TO_REACH_QUEUE  = 13;
        internal const uint PROPID_M_TIME_TO_BE_RECEIVED  = 14;
        internal const uint PROPID_M_RESP_QUEUE           = 15;
        internal const uint PROPID_M_RESP_QUEUE_LEN       = 16;
        internal const uint PROPID_M_ADMIN_QUEUE          = 17;
        internal const uint PROPID_M_ADMIN_QUEUE_LEN      = 18;
        internal const uint PROPID_M_EXTENSION            = 24;
        internal const uint PROPID_M_EXTENSION_LEN        = 25;
        internal const uint PROPID_M_BODY_TYPE            = 36;

        // -- Transaction handle sentinels (passed in the pTransaction slot) --
        internal static readonly IntPtr MQ_NO_TRANSACTION = IntPtr.Zero;
        internal static readonly IntPtr MQ_SINGLE_MESSAGE = new IntPtr(3);

        // S_OK
        internal const int S_OK = 0;

        // mqrt.dll exports `MQOpenQueue` (unsuffixed) — it does *not*
        // expose A/W variants. CharSet=Unicode + ExactSpelling=true on
        // the plain symbol is the right binding for the wide-string
        // overload that MSMQ provides.
        //
        // Note on the lpwcsFormatName argument: this is the raw MSMQ
        // format name (e.g. "DIRECT=OS:.\\private$\\foo"), NOT the
        // "FormatName:..." prefix used by System.Messaging public APIs
        // such as MessageQueue.Path. SafeMsmqQueueHandle.OpenForSend
        // is responsible for stripping that prefix if a caller hands it
        // through.
        [DllImport(MqrtDll, EntryPoint = "MQOpenQueue", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
        internal static extern int MQOpenQueue(
            string lpwcsFormatName,
            int dwAccess,
            int dwShareMode,
            out IntPtr phQueue);

        [DllImport(MqrtDll, EntryPoint = "MQCloseQueue", ExactSpelling = true, SetLastError = false)]
        internal static extern int MQCloseQueue(IntPtr hQueue);

        // Non-transactional / Single / MQ_SINGLE_MESSAGE / explicit ITransaction* via IntPtr.
        [DllImport(MqrtDll, EntryPoint = "MQSendMessage", ExactSpelling = true, SetLastError = false)]
        internal static extern int MQSendMessage(
            SafeMsmqQueueHandle hDestinationQueue,
            IntPtr pMessageProps,
            IntPtr pTransaction);

        // DTC enlistment overload — pTransaction is the IDtcTransaction
        // COM pointer obtained from TransactionInterop.
        [DllImport(MqrtDll, EntryPoint = "MQSendMessage", ExactSpelling = true, SetLastError = false)]
        internal static extern int MQSendMessage(
            SafeMsmqQueueHandle hDestinationQueue,
            IntPtr pMessageProps,
            [MarshalAs(UnmanagedType.Interface)] IDtcTransaction pTransaction);
    }
}
