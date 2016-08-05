// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using Xunit;

public static class UpnEndpointIdentityTest
{
    [Theory]
    [InlineData("")]
    [InlineData("test@wcf.example.com")]
    [ActiveIssue(1454)]
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
