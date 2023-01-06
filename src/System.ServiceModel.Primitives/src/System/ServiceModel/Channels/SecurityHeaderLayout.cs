// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;

namespace System.ServiceModel.Channels
{
    public enum SecurityHeaderLayout
    {
        Strict = 0,
        Lax = 1,
        LaxTimestampFirst = 2,
        LaxTimestampLast = 3
    }

    internal static class SecurityHeaderLayoutHelper
    {
        public static bool IsDefined(SecurityHeaderLayout value)
        {
            return (value == SecurityHeaderLayout.Lax
            || value == SecurityHeaderLayout.LaxTimestampFirst
            || value == SecurityHeaderLayout.LaxTimestampLast
            || value == SecurityHeaderLayout.Strict);
        }

        public static void Validate(SecurityHeaderLayout value)
        {
            if (!IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("value", (int)value,
                    typeof(SecurityHeaderLayout)));
            }
        }
    }
}
