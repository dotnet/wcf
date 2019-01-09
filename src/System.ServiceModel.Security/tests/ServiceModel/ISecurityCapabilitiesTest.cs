// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static class ISecurityCapabilitiesTest
{
    [WcfFact]
    public static void SecurityCapabilities_HttpsTransportBindingElement()
    {
        ISecurityCapabilities capability = CreateISecurityCapabilities(new HttpsTransportBindingElement());

        Assert.Equal("EncryptAndSign", capability.SupportedRequestProtectionLevel.ToString());
        Assert.Equal("EncryptAndSign", capability.SupportedResponseProtectionLevel.ToString());
        Assert.Equal("False", capability.SupportsClientAuthentication.ToString());
        Assert.Equal("False", capability.SupportsClientWindowsIdentity.ToString());
        Assert.Equal("True", capability.SupportsServerAuthentication.ToString());
    }

    [WcfFact]
    public static void SecurityCapabilities_HttpTransportBindingElement()
    {
        ISecurityCapabilities capability = CreateISecurityCapabilities(new HttpTransportBindingElement());

        Assert.Equal("None", capability.SupportedRequestProtectionLevel.ToString());
        Assert.Equal("None", capability.SupportedResponseProtectionLevel.ToString());
        Assert.Equal("False", capability.SupportsClientAuthentication.ToString());
        Assert.Equal("False", capability.SupportsClientWindowsIdentity.ToString());
        Assert.Equal("False", capability.SupportsServerAuthentication.ToString());
    }

    [WcfFact]
    public static void SecurityCapabilities_TransportSecurityBindingElement()
    {
        ISecurityCapabilities capability = CreateISecurityCapabilities(new TransportSecurityBindingElement());

        Assert.Equal("None", capability.SupportedRequestProtectionLevel.ToString());
        Assert.Equal("None", capability.SupportedResponseProtectionLevel.ToString());
        Assert.Equal("False", capability.SupportsClientAuthentication.ToString());
        Assert.Equal("False", capability.SupportsClientWindowsIdentity.ToString());
        Assert.Equal("False", capability.SupportsServerAuthentication.ToString());
    }

    [WcfFact]
    public static void SecurityCapabilities_SslStreamSecurityBindingElement()
    {
        ISecurityCapabilities capability = CreateISecurityCapabilities(new SslStreamSecurityBindingElement());

        Assert.Equal("EncryptAndSign", capability.SupportedRequestProtectionLevel.ToString());
        Assert.Equal("EncryptAndSign", capability.SupportedResponseProtectionLevel.ToString());
        Assert.Equal("False", capability.SupportsClientAuthentication.ToString());
        Assert.Equal("False", capability.SupportsClientWindowsIdentity.ToString());
        Assert.Equal("True", capability.SupportsServerAuthentication.ToString());
    }

    [WcfFact]
    public static void SecurityCapabilities_WindowsStreamSecurityBindingElement()
    {
        ISecurityCapabilities capability = CreateISecurityCapabilities(new WindowsStreamSecurityBindingElement());

        Assert.Equal("EncryptAndSign", capability.SupportedRequestProtectionLevel.ToString());
        Assert.Equal("EncryptAndSign", capability.SupportedResponseProtectionLevel.ToString());
        Assert.Equal("True", capability.SupportsClientAuthentication.ToString());
        Assert.Equal("True", capability.SupportsClientWindowsIdentity.ToString());
        Assert.Equal("True", capability.SupportsServerAuthentication.ToString());
    }

    public static ISecurityCapabilities CreateISecurityCapabilities(BindingElement bindingElement)
    {
        CustomBinding binding = new CustomBinding(bindingElement);
        BindingParameterCollection collection = new BindingParameterCollection();
        BindingContext context = new BindingContext(binding, collection);
        ISecurityCapabilities capability = binding.GetProperty<ISecurityCapabilities>(collection);
        return capability;
    }
}
