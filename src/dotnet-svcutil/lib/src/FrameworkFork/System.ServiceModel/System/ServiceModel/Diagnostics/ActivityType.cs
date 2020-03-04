// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Diagnostics
{
    internal enum ActivityType
    {
        Unknown,
        Close,
        Construct,
        ExecuteUserCode,
        ListenAt,
        Open,
        OpenClient,
        ProcessMessage,
        ProcessAction,
        ReceiveBytes,
        SecuritySetup,
        TransferToComPlus,
        WmiGetObject,
        WmiPutInstance,
        NumItems, // leave this item at the end of the list.
    }
}
