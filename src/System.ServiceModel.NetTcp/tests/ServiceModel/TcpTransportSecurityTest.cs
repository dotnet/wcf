// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using Xunit;
using TestTypes;

public static class TcpTransportSecurityTest
{
    [Fact]
    public static void Ctor_Default_Properties()
    {
        // new TcpTransportSecurity() initializes correct defaults
        TcpTransportSecurity transport = new TcpTransportSecurity();

        Assert.True(transport.ClientCredentialType == TcpClientCredentialType.Windows,
                    String.Format("ClientCredentialType should have been '{0}' but was '{1}'", TcpClientCredentialType.Windows, transport.ClientCredentialType));
    }

    [Theory]
    [InlineData(TcpClientCredentialType.None)]
    [InlineData(TcpClientCredentialType.Windows)]
    [InlineData(TcpClientCredentialType.Certificate)]
    public static void ClientCredentialType_Property_Sets(TcpClientCredentialType credentialType)
    {
        TcpTransportSecurity transport = new TcpTransportSecurity();
        transport.ClientCredentialType = credentialType;
        Assert.Equal<TcpClientCredentialType>(credentialType, transport.ClientCredentialType);
    }

    [Fact]
    public static void ClientCredentialType_Property_Set_Invalid_Value_Throws()
    {
        TcpTransportSecurity transport = new TcpTransportSecurity();
        Assert.Throws<ArgumentOutOfRangeException>(() => transport.ClientCredentialType = (TcpClientCredentialType)999);
    }
}
