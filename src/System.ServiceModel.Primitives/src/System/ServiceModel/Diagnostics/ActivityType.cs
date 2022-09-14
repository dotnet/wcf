// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


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
