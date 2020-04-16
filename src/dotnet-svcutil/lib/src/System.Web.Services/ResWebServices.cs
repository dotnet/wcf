// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Web.Services
{
    internal partial class ResWebServices
    {
        internal static string GetString(string format, params object[] values)
        {
            return string.Format(format, values);
        }
    }
}
