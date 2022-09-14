// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Globalization;
using System.Runtime;

namespace System.ServiceModel
{
    public enum BasicHttpsSecurityMode
    {
        Transport,
        TransportWithMessageCredential
    }

    internal static class BasicHttpsSecurityModeHelper
    {
        internal static bool IsDefined(BasicHttpsSecurityMode value)
        {
            return value == BasicHttpsSecurityMode.Transport ||
                value == BasicHttpsSecurityMode.TransportWithMessageCredential;
        }

        internal static BasicHttpsSecurityMode ToSecurityMode(UnifiedSecurityMode value)
        {
            switch (value)
            {
                case UnifiedSecurityMode.Transport:
                    return BasicHttpsSecurityMode.Transport;
                case UnifiedSecurityMode.TransportWithMessageCredential:
                    return BasicHttpsSecurityMode.TransportWithMessageCredential;
                default:
                    return (BasicHttpsSecurityMode)value;
            }
        }

        internal static BasicHttpsSecurityMode ToBasicHttpsSecurityMode(BasicHttpSecurityMode mode)
        {
            Fx.Assert(mode == BasicHttpSecurityMode.Transport || mode == BasicHttpSecurityMode.TransportWithMessageCredential, string.Format(CultureInfo.InvariantCulture, "Invalid BasicHttpSecurityMode value: {0}.", mode.ToString()));
            BasicHttpsSecurityMode basicHttpsSecurityMode = (mode == BasicHttpSecurityMode.Transport) ? BasicHttpsSecurityMode.Transport : BasicHttpsSecurityMode.TransportWithMessageCredential;

            return basicHttpsSecurityMode;
        }

        internal static BasicHttpSecurityMode ToBasicHttpSecurityMode(BasicHttpsSecurityMode mode)
        {
            if (!BasicHttpsSecurityModeHelper.IsDefined(mode))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(mode)));
            }

            BasicHttpSecurityMode basicHttpSecurityMode = (mode == BasicHttpsSecurityMode.Transport) ? BasicHttpSecurityMode.Transport : BasicHttpSecurityMode.TransportWithMessageCredential;

            return basicHttpSecurityMode;
        }
    }
}
