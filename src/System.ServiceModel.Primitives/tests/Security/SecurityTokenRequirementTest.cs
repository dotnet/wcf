// Licensed to the .NET Foundation under one or more agreements. 
// The .NET Foundation licenses this file to you under the MIT license. 
// See the LICENSE file in the project root for more information. 


using System;
using System.IdentityModel.Selectors;
using Infrastructure.Common;
using Xunit;

public static class SecurityTokenRequirementTest
{
    [WcfFact]
    public static void TokenType_DefaultValueIsSameAsNetFramework()
    {
        string NetValue = "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/TokenType";

        SecurityTokenRequirement tokenrequirement = new SecurityTokenRequirement();
        tokenrequirement.TokenType = SecurityTokenRequirement.TokenTypeProperty;
        Assert.Equal(NetValue, tokenrequirement.TokenType);   
    }

    [WcfTheory]
    [InlineData("http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement/example")]
    public static void TokenType_Property_IsSettable(string value)
    {
        SecurityTokenRequirement tokenrequirement = new SecurityTokenRequirement();
        tokenrequirement.TokenType = SecurityTokenRequirement.TokenTypeProperty;
        tokenrequirement.TokenType = value;
        Assert.Equal(value, tokenrequirement.TokenType);
    }

    [WcfFact]
    public static void Method_TryGetProperty_Return_TrueOrFalse()
    {
        SecurityTokenRequirement tokenrequirement = new SecurityTokenRequirement();
        tokenrequirement.TokenType = SecurityTokenRequirement.TokenTypeProperty;
        Assert.True(tokenrequirement.TryGetProperty(tokenrequirement.TokenType, out string valueIsTrue));
        Assert.False(tokenrequirement.TryGetProperty("invalidproperty", out string valueIsFalse));
    }

    [WcfFact]
    public static void Method_TryGetProperty_Invalid_Value_Throws()
    {
        SecurityTokenRequirement tokenrequirement = new SecurityTokenRequirement();
        tokenrequirement.TokenType = SecurityTokenRequirement.TokenTypeProperty;
        Assert.Throws<ArgumentException>(() => tokenrequirement.TryGetProperty(tokenrequirement.TokenType, out int Tvalue));
    }
}
