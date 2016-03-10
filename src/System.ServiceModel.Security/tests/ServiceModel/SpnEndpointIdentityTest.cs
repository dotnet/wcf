// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using Xunit;

public static class SpnEndpointIdentityTest
{
    [Theory]
    [InlineData("")]
    [InlineData("host/wcf")]
    [InlineData("host/wcf.example.com")]
    public static void Ctor_SpnName(string spn)
    {
        SpnEndpointIdentity spnEndpointEntity = new SpnEndpointIdentity(spn);
    }

    [Fact]
    public static void Ctor_NullSpn()
    {
        string spnName = null;

        Assert.Throws<ArgumentNullException>("spnName", () =>
        {
            SpnEndpointIdentity spnEndpointEntity = new SpnEndpointIdentity(spnName);
        });
    }
}
