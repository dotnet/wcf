// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Selectors;
using System.Runtime;
using System.Runtime.Diagnostics;

namespace System.ServiceModel.Diagnostics
{
    internal static class SecurityTraceRecordHelper
    {
        internal static void TraceTokenProviderOpened(EventTraceActivity eventTraceActivity, SecurityTokenProvider provider)
        {
            if (WcfEventSource.Instance.SecurityTokenProviderOpenedIsEnabled())
            {
                WcfEventSource.Instance.SecurityTokenProviderOpened(eventTraceActivity);
            }
        }
    }
}
