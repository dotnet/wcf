// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Tokens;
using Infrastructure.Common;
using Xunit;

public static class SecurityKeyTypeTest
{
    [WcfFact]
    public static void Get_EnumMembers_Test()
    {
        SecurityKeyType sk = SecurityKeyType.SymmetricKey;
        Assert.Equal(SecurityKeyType.SymmetricKey, sk);

        SecurityKeyType ak = SecurityKeyType.AsymmetricKey;
        Assert.Equal(SecurityKeyType.AsymmetricKey, ak);

        SecurityKeyType bk = SecurityKeyType.BearerKey;
        Assert.Equal(SecurityKeyType.BearerKey, bk);
    }

    [WcfTheory]
    [InlineData(SecurityKeyType.SymmetricKey, 0)]
    [InlineData(SecurityKeyType.AsymmetricKey, 1)]
    [InlineData(SecurityKeyType.BearerKey, 2)]
    public static void TypeConvert_EnumToInt_Test(SecurityKeyType key, int value)
    {
        Assert.Equal((int)key, value);
    }
}
