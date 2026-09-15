// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.Reflection;
using System.ServiceModel.Security;
using Infrastructure.Common;
using Xunit;

public class SecurityUtilsTest : ConditionalWcfTest
{
    [WcfFact]
    public static void FixNetworkCredential_AppContext_EnableLegacyUpnUsernameFix()
    {
        Type t = Assembly.GetAssembly(typeof(WindowsClientCredential))
                            .GetType(typeof(WindowsClientCredential).Namespace + ".SecurityUtils");

        MethodInfo method = t.GetMethod("FixNetworkCredential", BindingFlags.NonPublic | BindingFlags.Static,
            null, new[] { typeof(NetworkCredential).MakeByRefType() }, null);

        FieldInfo f = t.GetField("s_enableLegacyUpnUsernameFix", BindingFlags.Static | BindingFlags.NonPublic);

        //default
        var credential = new NetworkCredential("user@domain.com", "password");
        var parameters = new object[] { credential };
        method.Invoke(null, parameters);
        credential = (NetworkCredential)parameters[0];
        Assert.NotNull(credential);
        Assert.Equal("user@domain.com", credential.UserName);
        Assert.Equal("password", credential.Password);
        Assert.Equal(string.Empty, credential.Domain);

        //switch on
        f.SetValue(t, true);
        credential = new NetworkCredential("user@domain.com", "password");
        parameters = new object[] { credential };
        method.Invoke(null, parameters);
        credential = (NetworkCredential)parameters[0];
        Assert.NotNull(credential);
        Assert.Equal("user", credential.UserName);
        Assert.Equal("password", credential.Password);
        Assert.Equal("domain.com", credential.Domain);

        //switch off
        f.SetValue(t, false);
        credential = new NetworkCredential("user@domain.com", "password");
        parameters = new object[] { credential };
        method.Invoke(null, parameters);
        credential = (NetworkCredential)parameters[0];
        Assert.NotNull(credential);
        Assert.Equal("user@domain.com", credential.UserName);
        Assert.Equal("password", credential.Password);
        Assert.Equal(string.Empty, credential.Domain);
    }
}
