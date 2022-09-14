// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel
{
    public enum SecurityMode
    {
        None,
        Transport,
        Message,
        TransportWithMessageCredential
    }

    internal static class SecurityModeHelper
    {
        public static bool IsDefined(SecurityMode value)
        {
            return (value == SecurityMode.None ||
                value == SecurityMode.Transport ||
                value == SecurityMode.Message ||
                value == SecurityMode.TransportWithMessageCredential);
        }

        public static SecurityMode ToSecurityMode(UnifiedSecurityMode value)
        {
            switch (value)
            {
                case UnifiedSecurityMode.None:
                    return SecurityMode.None;
                case UnifiedSecurityMode.Transport:
                    return SecurityMode.Transport;
                case UnifiedSecurityMode.Message:
                    return SecurityMode.Message;
                case UnifiedSecurityMode.TransportWithMessageCredential:
                    return SecurityMode.TransportWithMessageCredential;
                default:
                    return (SecurityMode)value;
            }
        }
    }
}
