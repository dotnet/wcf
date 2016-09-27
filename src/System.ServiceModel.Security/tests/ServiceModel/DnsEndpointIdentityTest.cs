// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public static class DnsEndpointIdentityTest
{
    [WcfTheory]
    [InlineData("")]
    [InlineData("wcf")]
    [InlineData("wcf.example.com")]
    public static void Ctor_DnsName(string dnsName)
    {
        DnsEndpointIdentity dnsEndpointEntity = new DnsEndpointIdentity(dnsName);
    }

    [WcfFact]
    public static void Ctor_NullDnsName()
    {
        string dnsName = null;

        Assert.Throws<ArgumentNullException>("dnsName", () =>
        {
            DnsEndpointIdentity dnsEndpointEntity = new DnsEndpointIdentity(dnsName);
        });
    }
}
