// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    public enum NetNamedPipeSecurityMode
    {
        None,
        Transport
    }

    static class NetNamedPipeSecurityModeHelper
    {
        internal static bool IsDefined(NetNamedPipeSecurityMode value)
        {
            return
                value == NetNamedPipeSecurityMode.Transport ||
                value == NetNamedPipeSecurityMode.None;
        }
    }
}
