// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.ServiceModel.Security.Tokens;
using Infrastructure.Common;
using Xunit;

public static class IssuedSecurityTokenParametersTest
{
    [WcfFact]
    public static void Ctor_Default()
    {
        IssuedSecurityTokenParameters tokenParameters = new IssuedSecurityTokenParameters();
        Assert.NotNull(tokenParameters);
    }

    [WcfFact]
    public static void Properties_Settable()
    {
        var edpa = new EndpointAddress("https://localhost");
        var binding = new BasicHttpsBinding();
        string tokenType = "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/TokenType";

        IssuedSecurityTokenParameters tokenParameters = new IssuedSecurityTokenParameters()
        {
            DefaultMessageSecurityVersion = MessageSecurityVersion.Default,
            IssuerAddress = edpa,
            IssuerBinding = binding,
            KeyType = SecurityKeyType.AsymmetricKey,
            TokenType = tokenType
        };

        Assert.Equal(MessageSecurityVersion.Default, tokenParameters.DefaultMessageSecurityVersion);
        Assert.Equal(edpa, tokenParameters.IssuerAddress);
        Assert.Equal(binding, tokenParameters.IssuerBinding);
        Assert.Equal(SecurityKeyType.AsymmetricKey, tokenParameters.KeyType);
        Assert.Equal(tokenType, tokenParameters.TokenType);
    }
}
