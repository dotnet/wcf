// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.ServiceModel
{
    public enum MsmqAuthenticationMode
    {
        None,
        WindowsDomain,
        Certificate,
    }

    internal static class MsmqAuthenticationModeHelper
    {
        public static bool IsDefined(MsmqAuthenticationMode mode)
        {
            return mode >= MsmqAuthenticationMode.None && mode <= MsmqAuthenticationMode.Certificate;
        }
    }
}
