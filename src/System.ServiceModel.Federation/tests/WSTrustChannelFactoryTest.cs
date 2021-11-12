// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Infrastructure.Common;
using Xunit;

namespace System.ServiceModel.Federation.Tests
{
    public static class WSTrustChannelFactoryTest
    {
        [WcfFact]
        public static void DefaultWSTrustChannelFactory()
        {
            try
            {
                WSTrustChannelFactory trustChannelFactory = new WSTrustChannelFactory(null, null);
            }
            catch(Exception ex)
            {
                Assert.Equal(typeof(ArgumentNullException), ex.GetType());
            }
        }
    }
}
