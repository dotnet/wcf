// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public static class NetTcpSecurityTest
{
    [WcfFact]
    public static void Ctor_Default_Initializes_Properties()
    {
        // new NetTcpSecurity() initializes correct defaults
        NetTcpSecurity security = new NetTcpSecurity();
        Assert.Equal<SecurityMode>(SecurityMode.Transport, security.Mode);
    }

    [WcfTheory]
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

    [WcfFact]
    public static void Mode_Property_Set_Invalid_Value_Throws()
    {
        NetTcpSecurity security = new NetTcpSecurity();
        Assert.Throws<ArgumentOutOfRangeException>(() => security.Mode = (SecurityMode)999);
    }

    [WcfFact]
    public static void Transport_Property_Sets()
    {
        NetTcpSecurity security = new NetTcpSecurity();

        TcpTransportSecurity newSecurity = new TcpTransportSecurity();
        security.Transport = newSecurity;
        Assert.Equal<TcpTransportSecurity>(newSecurity, security.Transport);
    }
}
