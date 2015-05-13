// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using Xunit;

public static class CallbackBehaviorAttributeTest
{
    [Fact]
    public static void Default_Ctor_Initializes_Correctly()
    {
        CallbackBehaviorAttribute cba = new CallbackBehaviorAttribute();

        Assert.True(cba.AutomaticSessionShutdown, "AutomaticSessionShutdown should have been true");
        Assert.True(cba.UseSynchronizationContext, "UseSynchronizationContext should have been true");
    }
}
