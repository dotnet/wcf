// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel
{
    public enum QueueTransferProtocol
    {
        Native,
        Srmp,
        SrmpSecure
    }

    internal static class QueueTransferProtocolHelper
    {
        internal static bool IsDefined(QueueTransferProtocol value)
        {
            return value == QueueTransferProtocol.Native
                || value == QueueTransferProtocol.Srmp
                || value == QueueTransferProtocol.SrmpSecure;
        }
    }
}
