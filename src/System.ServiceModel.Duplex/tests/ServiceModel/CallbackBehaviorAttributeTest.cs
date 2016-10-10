// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public static class CallbackBehaviorAttributeTest
{
    [WcfFact]
    public static void Default_Ctor_Initializes_Correctly()
    {
        CallbackBehaviorAttribute cba = new CallbackBehaviorAttribute();

        Assert.True(cba.AutomaticSessionShutdown, "AutomaticSessionShutdown should have been true");
        Assert.True(cba.UseSynchronizationContext, "UseSynchronizationContext should have been true");
    }

    [WcfTheory]
    [InlineData(false)]
    [InlineData(true)]
    public static void AutomaticSessionShutdown_Property_Is_Settable(bool value)
    {
        CallbackBehaviorAttribute cba = new CallbackBehaviorAttribute();
        cba.AutomaticSessionShutdown = value;
        Assert.Equal(value, cba.AutomaticSessionShutdown);
    }

    [WcfTheory]
    [InlineData(false)]
    [InlineData(true)]
    public static void UseSynchronizationContext_Property_Is_Settable(bool value)
    {
        CallbackBehaviorAttribute cba = new CallbackBehaviorAttribute();
        cba.UseSynchronizationContext = value;
        Assert.Equal(value, cba.UseSynchronizationContext);
    }
}
