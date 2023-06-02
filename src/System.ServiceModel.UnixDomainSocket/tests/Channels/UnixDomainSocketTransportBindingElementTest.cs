// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static class UnixDomainSocketTransportBindingElementTest
{
    [WcfFact]
    public static void Ctor_Default_Properties()
    {
        // Validates new UnixDomainSocketTransportBindingElement() initializes correct default property values
        UnixDomainSocketTransportBindingElement element = new UnixDomainSocketTransportBindingElement();

        Assert.True(String.Equals(element.Scheme, "net.uds"), String.Format("Scheme property expected '{0}' but actual was '{1}'", "net.uds", element.Scheme));

        // Validate only a non-null ConnectionPoolSetting.
        // Its own default values are validated in that type's test methods
        Assert.True(element.ConnectionPoolSettings != null, "ConnectionPoolSettings should not be null.");
    }
}
