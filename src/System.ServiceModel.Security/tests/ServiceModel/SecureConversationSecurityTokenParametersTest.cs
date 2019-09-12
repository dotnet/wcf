using System;
using System.Collections.Generic;
using System.Text;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using Infrastructure.Common;
using Xunit;


public static class SecureConversationSecurityTokenParametersTest
{
    [WcfTheory]
    [InlineData(false)]
    [InlineData(true)]
    public static void RequireCancellation_Property_Is_Settable(bool value)
    {
        SecureConversationSecurityTokenParameters scstr = new SecureConversationSecurityTokenParameters();
        scstr.RequireCancellation = value;
        Assert.Equal(value, scstr.RequireCancellation);
    }
}
