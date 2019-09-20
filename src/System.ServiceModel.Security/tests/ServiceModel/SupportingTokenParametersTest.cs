// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.ObjectModel;
using System.ServiceModel.Security.Tokens;
using Infrastructure.Common;
using Xunit;

public static class SupportingTokenParametersTest
{
    [WcfFact]
    public static void Property_Signed()
    {
        SupportingTokenParameters stp = new SupportingTokenParameters();
        Assert.NotNull(stp.Signed);
        Assert.Equal(new Collection<SecurityTokenParameters>(), stp.Signed);
    }
}
