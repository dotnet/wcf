// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel
{
    public enum SecurityMode
    {
        None,
        Transport,
        Message,
        TransportWithMessageCredential
    }

    public static class SecurityModeHelper
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
