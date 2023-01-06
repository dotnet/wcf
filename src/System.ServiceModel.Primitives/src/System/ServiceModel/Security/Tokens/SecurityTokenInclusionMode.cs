// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;

namespace System.ServiceModel.Security.Tokens
{
    public enum SecurityTokenInclusionMode
    {
        AlwaysToRecipient = 0,
        Never = 1,
        Once = 2,
        AlwaysToInitiator = 3
    }

    internal static class SecurityTokenInclusionModeHelper
    {
        public static bool IsDefined(SecurityTokenInclusionMode value)
        {
            return (value == SecurityTokenInclusionMode.AlwaysToInitiator
            || value == SecurityTokenInclusionMode.AlwaysToRecipient
            || value == SecurityTokenInclusionMode.Never
            || value == SecurityTokenInclusionMode.Once);
        }

        public static void Validate(SecurityTokenInclusionMode value)
        {
            if (!IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("value", (int)value,
                    typeof(SecurityTokenInclusionMode)));
            }
        }
    }
}
