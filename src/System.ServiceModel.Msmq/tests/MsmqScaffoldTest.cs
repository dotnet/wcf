// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Reflection;
using Infrastructure.Common;
using Xunit;

public static class MsmqScaffoldTest
{
    [WcfFact]
    public static void Assembly_Loads()
    {
        Assembly asm = Assembly.Load("System.ServiceModel.Msmq");
        Assert.NotNull(asm);
        Assert.Equal("System.ServiceModel.Msmq", asm.GetName().Name);
    }
}
