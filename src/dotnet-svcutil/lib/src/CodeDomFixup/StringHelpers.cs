//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

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