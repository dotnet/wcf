// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Xunit;

public static class SecurityBindingElementTest
{
    [Fact]
    public static void Create_HttpBinding_SecurityMode_TransportWithMessageCredential()
    {
        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
        var bindingElements = binding.CreateBindingElements();

        var securityBindingElement = bindingElements.FirstOrDefault(x => x is SecurityBindingElement) as SecurityBindingElement;
        Assert.True(securityBindingElement != null, "securityBindingElement should not be null when BasicHttpSecurityMode.TransportWithMessageCredential is specified");
    }

    [Theory]
    [InlineData(BasicHttpSecurityMode.TransportCredentialOnly)]
    [InlineData(BasicHttpSecurityMode.Transport)]
    [InlineData(BasicHttpSecurityMode.None)]
    public static void Create_HttpBinding_SecurityMode_Without_SecurityBindingElement(BasicHttpSecurityMode securityMode)
    {
        BasicHttpBinding binding = new BasicHttpBinding(securityMode);
        var bindingElements = binding.CreateBindingElements();

        var securityBindingElement = bindingElements.FirstOrDefault(x => x is SecurityBindingElement) as SecurityBindingElement;
        Assert.True(securityBindingElement == null, "securityBindingElement should be null when BasicHttpSecurityMode.TransportCredentialOnly is specified");
    }

    [Theory]
    [InlineData(BasicHttpSecurityMode.Message)]
    // BasicHttpSecurityMode.Message is not supported
    public static void Create_HttpBinding_SecurityMode_Message_Throws_NotSupported(BasicHttpSecurityMode securityMode)
    {
        BasicHttpBinding binding = new BasicHttpBinding(securityMode);

        Assert.Throws<NotSupportedException>(() => {
            var bindingElements = binding.CreateBindingElements();
        });
    }

}
