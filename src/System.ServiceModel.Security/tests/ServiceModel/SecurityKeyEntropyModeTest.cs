// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Security;
using Infrastructure.Common;
using Xunit;

public static class SecurityKeyEntropyModeTest
{
    [WcfFact]
    public static void Get_EnumMembers()
    {
        SecurityKeyEntropyMode clientEn = SecurityKeyEntropyMode.ClientEntropy;
        Assert.Equal(SecurityKeyEntropyMode.ClientEntropy, clientEn);

        SecurityKeyEntropyMode serverEn = SecurityKeyEntropyMode.ServerEntropy;
        Assert.Equal(SecurityKeyEntropyMode.ServerEntropy, serverEn);

        SecurityKeyEntropyMode combinedEn = SecurityKeyEntropyMode.CombinedEntropy;
        Assert.Equal(SecurityKeyEntropyMode.CombinedEntropy, combinedEn);
    }

    [WcfTheory]
    [InlineData(SecurityKeyEntropyMode.ClientEntropy, 0)]
    [InlineData(SecurityKeyEntropyMode.ServerEntropy, 1)]
    [InlineData(SecurityKeyEntropyMode.CombinedEntropy, 2)]
    public static void TypeConvert_EnumToInt(SecurityKeyEntropyMode sem, int value)
    {
        Assert.Equal((int)sem, value);
    }
}
