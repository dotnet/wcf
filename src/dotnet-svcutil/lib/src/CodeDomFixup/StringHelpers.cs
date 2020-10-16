// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    using System;

    internal static class StringHelpers
    {
        public static string StripPrefix(string s, string prefix)
        {
            return s.StartsWith(prefix, StringComparison.Ordinal) ? s.Substring(prefix.Length) : s;
        }
    }
}
