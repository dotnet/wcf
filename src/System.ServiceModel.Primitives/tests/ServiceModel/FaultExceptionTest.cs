// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public static class FaultExceptionTest
{
    [WcfFact]
    public static void Ctor_TDetail_FaultReason()
    {
        var detail = new FaultDetail("Fault Message");
        var reason = new FaultReason("Fault reason");
        var exception = new FaultException<FaultDetail>(detail, reason);
        Assert.NotNull(exception);
        Assert.NotNull(exception.Detail);
        Assert.NotNull(exception.Reason);
        Assert.NotNull(exception.Code);
        Assert.Equal(detail, exception.Detail);
        Assert.Equal(reason, exception.Reason);
        
        FaultDetail nullDetail = null;
        FaultReason nullReason = null;
        var exception2 = new FaultException<FaultDetail>(nullDetail, nullReason);
        Assert.NotNull(exception2);
        Assert.NotNull(exception2.Code);
        Assert.NotNull(exception2.Reason);
        Assert.Null(exception2.Detail);        
    }
}
