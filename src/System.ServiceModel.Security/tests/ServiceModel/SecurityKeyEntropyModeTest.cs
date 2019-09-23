// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Security;
using Infrastructure.Common;
using Xunit;

public static class SecurityKeyEntropyModeTest
{
    [WcfTheory]
    [InlineData(SecurityKeyEntropyMode.ClientEntropy)]
    [InlineData(SecurityKeyEntropyMode.ServerEntropy)]
    [InlineData(SecurityKeyEntropyMode.CombinedEntropy)]
    public static void Set_EnumMembers(SecurityKeyEntropyMode skem)
    {
        SecurityKeyEntropyMode entropyMode = skem;
        Assert.Equal(skem, entropyMode);
    }

    [WcfTheory]
    [InlineData(0, SecurityKeyEntropyMode.ClientEntropy)]
    [InlineData(1, SecurityKeyEntropyMode.ServerEntropy)]
    [InlineData(2, SecurityKeyEntropyMode.CombinedEntropy)]
    public static void TypeConvert_EnumToInt(int value, SecurityKeyEntropyMode sem)
    {
        Assert.Equal(value, (int)sem);
    }
}
