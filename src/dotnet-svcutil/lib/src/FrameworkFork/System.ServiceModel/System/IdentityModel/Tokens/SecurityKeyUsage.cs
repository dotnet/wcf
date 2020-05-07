// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime;

namespace System.IdentityModel.Tokens
{
    public enum SecurityKeyUsage
    {
        Exchange,
        Signature
    }

    internal static class SecurityKeyUsageHelper
    {
        internal static bool IsDefined(SecurityKeyUsage value)
        {
            return (value == SecurityKeyUsage.Exchange
                || value == SecurityKeyUsage.Signature);
        }

        internal static void Validate(SecurityKeyUsage value)
        {
            if (!IsDefined(value))
            {
                throw Fx.Exception.AsError(new InvalidEnumArgumentException("value", (int)value,
                    typeof(SecurityKeyUsage)));
            }
        }
    }
}
