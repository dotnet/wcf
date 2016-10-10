// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public static class TcpTransportSecurityTest
{
    [WcfFact]
    public static void Ctor_Default_Properties()
    {
        // new TcpTransportSecurity() initializes correct defaults
        TcpTransportSecurity transport = new TcpTransportSecurity();

        Assert.True(transport.ClientCredentialType == TcpClientCredentialType.Windows,
                    String.Format("ClientCredentialType should have been '{0}' but was '{1}'", TcpClientCredentialType.Windows, transport.ClientCredentialType));
    }

    [WcfTheory]
    [InlineData(TcpClientCredentialType.None)]
    [InlineData(TcpClientCredentialType.Windows)]
    [InlineData(TcpClientCredentialType.Certificate)]
    public static void ClientCredentialType_Property_Sets(TcpClientCredentialType credentialType)
    {
        TcpTransportSecurity transport = new TcpTransportSecurity();
        transport.ClientCredentialType = credentialType;
        Assert.Equal<TcpClientCredentialType>(credentialType, transport.ClientCredentialType);
    }

    [WcfFact]
    public static void ClientCredentialType_Property_Set_Invalid_Value_Throws()
    {
        TcpTransportSecurity transport = new TcpTransportSecurity();
        Assert.Throws<ArgumentOutOfRangeException>(() => transport.ClientCredentialType = (TcpClientCredentialType)999);
    }
}
