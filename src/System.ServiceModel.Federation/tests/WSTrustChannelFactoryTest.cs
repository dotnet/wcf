// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Infrastructure.Common;

namespace System.ServiceModel.Federation.Tests
{
    public static class WSTrustChannelFactoryTest
    {
        [WcfFact]
        public static void DefaultWSTrustChannelFactory()
        {
            WSTrustChannelFactory trustChannelFactory = new WSTrustChannelFactory((string)null, null);
        }
    }
}
