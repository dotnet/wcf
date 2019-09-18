// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using Infrastructure.Common;
using Xunit;

public static class SecutityBindingElementTest
{
    [WcfFact]
    public static void Method_CreateUserNameOverTransportBindingElement()
    {
        TransportSecurityBindingElement securityBindingElement = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
        Assert.True(securityBindingElement != null, "The created TransportSecurityBindingElement should not be null.");
        Assert.True(securityBindingElement.IncludeTimestamp);
        Assert.False(securityBindingElement.LocalClientSettings.DetectReplays);
        Assert.True(securityBindingElement.EndpointSupportingTokenParameters.SignedEncrypted[0] is UserNameSecurityTokenParameters);

        securityBindingElement.IncludeTimestamp = false;
        securityBindingElement.SecurityHeaderLayout = SecurityHeaderLayout.Lax;
        var binding = new CustomBinding();
        binding.Elements.Add(securityBindingElement);
        SecurityBindingElement element = binding.Elements.Find<SecurityBindingElement>();
        Assert.True(element != null, "SecurityBindingElement added to binding elements should not be null.");
        Assert.Equal(element, securityBindingElement);
    }

    [WcfFact]
    public static void Property_DefaultAlgorithmSuite()
    {
        TransportSecurityBindingElement securityBindingElement = new TransportSecurityBindingElement();
        Assert.Equal(securityBindingElement.DefaultAlgorithmSuite, SecurityAlgorithmSuite.Basic256);
    }

    [WcfFact]
    public static void Property_KeyEntropyMode()
    {
        TransportSecurityBindingElement securityBindingElement = new TransportSecurityBindingElement();
        Assert.Equal(securityBindingElement.KeyEntropyMode, SecurityKeyEntropyMode.CombinedEntropy);
    }
}
