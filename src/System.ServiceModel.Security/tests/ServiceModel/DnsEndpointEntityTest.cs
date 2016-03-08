// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using Xunit;

public static class DnsEndpointEntityTest
{
    //Can only verify the Ctor does not throw as Claim is not in contract
    [Fact]
    public static void Ctor_DnsName()
    {
        string dnsName = "MyDns";
        DnsEndpointIdentity dnsEndpointEntity = new DnsEndpointIdentity(dnsName);
    }

    [Fact]
    public static void Ctor_NullDnsName()
    {
        string dnsName = null;

        Assert.Throws<ArgumentNullException>("dnsName", () =>
        {
            DnsEndpointIdentity dnsEndpointEntity = new DnsEndpointIdentity(dnsName);
        });
    }
}
