// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public static class UpnEndpointIdentityTest
{
#if FULLXUNIT_NOTSUPPORTED
    [Theory]
#endif
    [WcfTheory]
    [InlineData("")]
    [InlineData("test@wcf.example.com")]
    [ActiveIssue(1454)]
    public static void Ctor_UpnName(string upn)
    {
        UpnEndpointIdentity upnEndpointEntity = new UpnEndpointIdentity(upn);
    }

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
#endif
    [WcfFact]
    public static void Ctor_NullUpn()
    {
        string upnName = null;

        Assert.Throws<ArgumentNullException>("upnName", () =>
        {
            UpnEndpointIdentity upnEndpointEntity = new UpnEndpointIdentity(upnName);
        });
    }
}
