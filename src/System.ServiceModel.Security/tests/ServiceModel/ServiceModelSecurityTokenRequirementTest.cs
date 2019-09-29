// Licensed to the .NET Foundation under one or more agreements. 
// The .NET Foundation licenses this file to you under the MIT license. 
// See the LICENSE file in the project root for more information. 


using System.ServiceModel.Security.Tokens;
using Infrastructure.Common;
using Xunit;

public static class ServiceModelSecurityTokenRequirementTest
{
    [WcfFact]
    public static void ChannelParametersCollectionProperty_DefaultValueIsSameAsNetFramework()
    {
        string cpcpValue = "http://schemas.microsoft.com/ws/2006/05/servicemodel/securitytokenrequirement/ChannelParametersCollection";

        Assert.NotNull(ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty);
        Assert.Equal(cpcpValue, ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty);
    }
}


