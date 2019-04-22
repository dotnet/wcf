// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Tests.Common;
using System.Text;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public static class BasicHttpSecurityTest
{
    [WcfTheory]
    [InlineData(BasicHttpSecurityMode.Message)]
    public static void BasicHttpSecurity_MessageSecurityMode_ThrowsPNSE(BasicHttpSecurityMode value)
    {
        var security = new BasicHttpSecurity();
        Assert.Throws<PlatformNotSupportedException>(() => security.Mode = value);
    }
}
