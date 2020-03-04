// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
