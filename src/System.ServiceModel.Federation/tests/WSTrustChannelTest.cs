// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Infrastructure.Common;
using Microsoft.IdentityModel.Protocols.WsTrust;

namespace System.ServiceModel.Federation.Tests
{
    public static class WSTrustChannelTest
    {
        [WcfFact]
        public static void DefaultWSTrustChannel()
        {
            WSTrustChannel trustChannel = new WSTrustChannel(null);
        }
    }
}
