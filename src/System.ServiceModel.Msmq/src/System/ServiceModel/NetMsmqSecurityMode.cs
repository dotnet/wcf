// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel
{
    public enum NetMsmqSecurityMode
    {
        None,
        Transport,
        Message,
        Both
    }

    internal static class NetMsmqSecurityModeHelper
    {
        internal static bool IsDefined(NetMsmqSecurityMode value)
        {
            return (value == NetMsmqSecurityMode.Transport
                || value == NetMsmqSecurityMode.Message
                || value == NetMsmqSecurityMode.Both
                || value == NetMsmqSecurityMode.None);
        }
    }
}
