// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Selectors;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using Infrastructure.Common;
using Xunit;

public static class SecurityCredentialsManagerTest
{
    [WcfFact]
    public static void Derivable()
    {
        var credentials = new ClientCredentials();
        Assert.NotNull(credentials);
        Assert.IsAssignableFrom<SecurityCredentialsManager>(credentials);

        SecurityTokenManager tokenManager = credentials.CreateSecurityTokenManager();
        Assert.NotNull(tokenManager);
    }
}
