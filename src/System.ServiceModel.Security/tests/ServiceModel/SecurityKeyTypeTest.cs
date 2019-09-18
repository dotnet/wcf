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
        SecurityKeyType skt0 = SecurityKeyType.SymmetricKey;
        Assert.Equal(SecurityKeyType.SymmetricKey, skt0);

        SecurityKeyType skt1 = SecurityKeyType.AsymmetricKey;
        Assert.Equal(SecurityKeyType.AsymmetricKey, skt1);

        SecurityKeyType skt2 = SecurityKeyType.BearerKey;
        Assert.Equal(SecurityKeyType.BearerKey, skt2);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public static void TypeConvert_EnumToInt_Test(int value)
    {
        switch (value)
        {
            case 0:
                Assert.Equal((int)SecurityKeyType.SymmetricKey, value);
                break;
            case 1:
                Assert.Equal((int)SecurityKeyType.AsymmetricKey, value);
                break;
            default:
                Assert.Equal((int)SecurityKeyType.BearerKey, value);
                break;
        }
    }
}
