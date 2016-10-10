// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static class TcpTransportBindingElementTest
{
    [WcfFact]
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
