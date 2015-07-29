// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using Xunit;

public static class BasicHttpSecurityTest
{
    [Fact]
    public static void Default_Ctor_Initializes_Properties()
    {
        BasicHttpSecurity security = new BasicHttpSecurity();

        // These properties are not in a public contract
        BasicHttpSecurityMode expectedMode = BasicHttpSecurityMode.None;
        BasicHttpSecurityMode actualMode = security.Mode;
        Assert.True(expectedMode == actualMode, String.Format("Mode expected: {0}, actual: {1}", expectedMode, actualMode));

        HttpTransportSecurity transportSecurity = security.Transport;
        Assert.True(transportSecurity != null, "Transport property should have been non-null");

        BasicHttpMessageSecurity httpSecurity = security.Message;
        Assert.True(httpSecurity != null, "Message property should have been non-null");
    }
}
