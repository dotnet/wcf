// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
