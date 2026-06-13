// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
namespace System.ServiceModel
{
    internal static class WebHttpSecurityModeHelper
    {
        internal static bool IsDefined(WebHttpSecurityMode value)
        {
            return (value == WebHttpSecurityMode.None ||
                value == WebHttpSecurityMode.Transport ||
                value == WebHttpSecurityMode.TransportCredentialOnly);
        }
    }
}
