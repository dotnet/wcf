// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using Xunit;
using TestTypes;

public static class NetTcpSecurityTest
{
    [Fact]
    public static void Ctor_Default_Initializes_Properties()
    {
        // new NetTcpSecurity() initializes correct defaults
        NetTcpSecurity security = new NetTcpSecurity();
        Assert.Equal<SecurityMode>(SecurityMode.Transport, security.Mode);
    }

    [Theory]
    [InlineData(SecurityMode.Message)]
    [InlineData(SecurityMode.None)]
    [InlineData(SecurityMode.Transport)]
    [InlineData(SecurityMode.TransportWithMessageCredential)]
    public static void Mode_Property_Sets(SecurityMode mode)
    {
        NetTcpSecurity security = new NetTcpSecurity();
        security.Mode = mode;
        Assert.Equal<SecurityMode>(mode, security.Mode);
    }

    [Fact]
    public static void Mode_Property_Set_Invalid_Value_Throws()
    {
        NetTcpSecurity security = new NetTcpSecurity();
        Assert.Throws<ArgumentOutOfRangeException>(() => security.Mode = (SecurityMode)999);
    }

    [Fact]
    public static void Transport_Property_Sets()
    {
        NetTcpSecurity security = new NetTcpSecurity();

        TcpTransportSecurity newSecurity = new TcpTransportSecurity();
        security.Transport = newSecurity;
        Assert.Equal<TcpTransportSecurity>(newSecurity, security.Transport);
    }
}
