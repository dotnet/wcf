// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.ServiceModel.Security
{
    public enum SecurityKeyEntropyMode
    {
        ClientEntropy,
        ServerEntropy,
        CombinedEntropy
    }

    internal sealed class SecurityKeyEntropyModeHelper
    {
        internal static bool IsDefined(SecurityKeyEntropyMode value)
        {
            return (value == SecurityKeyEntropyMode.ClientEntropy
                || value == SecurityKeyEntropyMode.ServerEntropy
                || value == SecurityKeyEntropyMode.CombinedEntropy);
        }

        internal static void Validate(SecurityKeyEntropyMode value)
        {
            if (!IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("value", (int)value,
                    typeof(SecurityKeyEntropyMode)));
            }
        }
    }
}
