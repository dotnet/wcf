// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public static class UnixDomainSocketTransportSecurityTest
{
    [WcfFact]
    public static void Ctor_Default_Properties()
    {
        // new UnixDomainSocketTransportSecurity() initializes correct defaults
        UnixDomainSocketTransportSecurity transport = new UnixDomainSocketTransportSecurity();

        Assert.True(transport.ClientCredentialType == UnixDomainSocketClientCredentialType.Default,
                    String.Format("ClientCredentialType should have been '{0}' but was '{1}'", UnixDomainSocketClientCredentialType.Default, transport.ClientCredentialType));
    }

    [WcfTheory]
    [InlineData(UnixDomainSocketClientCredentialType.None)]
    [InlineData(UnixDomainSocketClientCredentialType.Windows)]
    [InlineData(UnixDomainSocketClientCredentialType.Certificate)]
    [InlineData(UnixDomainSocketClientCredentialType.Default)]
    [InlineData(UnixDomainSocketClientCredentialType.PosixIdentity)]
    public static void ClientCredentialType_Property_Sets(UnixDomainSocketClientCredentialType credentialType)
    {
        UnixDomainSocketTransportSecurity transport = new UnixDomainSocketTransportSecurity();
        transport.ClientCredentialType = credentialType;
        Assert.Equal<UnixDomainSocketClientCredentialType>(credentialType, transport.ClientCredentialType);
    }

    [WcfFact]
    public static void ClientCredentialType_Property_Set_Invalid_Value_Throws()
    {
        UnixDomainSocketTransportSecurity transport = new UnixDomainSocketTransportSecurity();
        Assert.Throws<ArgumentOutOfRangeException>(() => transport.ClientCredentialType = (UnixDomainSocketClientCredentialType)999);
    }
}
