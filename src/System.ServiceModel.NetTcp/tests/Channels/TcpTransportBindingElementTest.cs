// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel.Channels;
using Xunit;

public static class TcpTransportBindingElementTest
{
    [Fact]
    public static void Ctor_Default_Properties()
    {
        // Validates new TcpTransportBindingElement() initializes correct default property values
        TcpTransportBindingElement element = new TcpTransportBindingElement();

        Assert.True(String.Equals(element.Scheme, "net.tcp"), String.Format("Scheme property expected '{0}' but actual was '{1}'", "net.tcp", element.Scheme));

        // Validate only a non-null TcpConnectionPoolSetting.
        // Its own default values are validated in that type's test methods
        Assert.True(element.ConnectionPoolSettings != null, "ConnectionPoolSettings should not be null.");
    }
}
