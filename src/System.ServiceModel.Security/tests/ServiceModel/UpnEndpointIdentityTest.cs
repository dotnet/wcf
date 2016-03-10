// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using Xunit;

public static class UpnEndpointIdentityTest
{
    [Theory]
    [InlineData("")]
    [InlineData("test@wcf.example.com")]
    public static void Ctor_UpnName(string upn)
    {
        UpnEndpointIdentity upnEndpointEntity = new UpnEndpointIdentity(upn);
    }

    [Fact]
    public static void Ctor_NullUpn()
    {
        string upnName = null;

        Assert.Throws<ArgumentNullException>("upnName", () =>
        {
            UpnEndpointIdentity upnEndpointEntity = new UpnEndpointIdentity(upnName);
        });
    }
}
