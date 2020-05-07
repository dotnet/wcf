// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.ServiceModel
{
    public enum HostNameComparisonMode
    {
        StrongWildcard = 0, // +
        Exact = 1,
        WeakWildcard = 2,   // *
    }

    public static class HostNameComparisonModeHelper
    {
        public static bool IsDefined(HostNameComparisonMode value)
        {
            return
                value == HostNameComparisonMode.StrongWildcard
                || value == HostNameComparisonMode.Exact
                || value == HostNameComparisonMode.WeakWildcard;
        }

        public static void Validate(HostNameComparisonMode value)
        {
            if (!IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("value", (int)value,
                    typeof(HostNameComparisonMode)));
            }
        }
    }
}
