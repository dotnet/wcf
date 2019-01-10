// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Net.Security;
using Infrastructure.Common;
using Xunit;

public static class ISecurityCapabilitiesTest
{
    [WcfFact]
    public static void SecurityCapabilities_HttpsTransportBindingElement()
    {
        ISecurityCapabilities capability = CreateISecurityCapabilities(new HttpsTransportBindingElement());

        Assert.Equal(ProtectionLevel.EncryptAndSign, capability.SupportedRequestProtectionLevel);
        Assert.Equal(ProtectionLevel.EncryptAndSign, capability.SupportedResponseProtectionLevel);
        Assert.False(capability.SupportsClientAuthentication);
        Assert.False(capability.SupportsClientWindowsIdentity);
        Assert.True(capability.SupportsServerAuthentication);
    }

    [WcfFact]
    public static void SecurityCapabilities_HttpTransportBindingElement()
    {
        ISecurityCapabilities capability = CreateISecurityCapabilities(new HttpTransportBindingElement());

        Assert.Equal(ProtectionLevel.None, capability.SupportedRequestProtectionLevel);
        Assert.Equal(ProtectionLevel.None, capability.SupportedResponseProtectionLevel);
        Assert.False(capability.SupportsClientAuthentication);
        Assert.False(capability.SupportsClientWindowsIdentity);
        Assert.False(capability.SupportsServerAuthentication);
    }

    [WcfFact]
    public static void SecurityCapabilities_TransportSecurityBindingElement()
    {
        ISecurityCapabilities capability = CreateISecurityCapabilities(new TransportSecurityBindingElement());

        Assert.Equal(ProtectionLevel.None, capability.SupportedRequestProtectionLevel);
        Assert.Equal(ProtectionLevel.None, capability.SupportedResponseProtectionLevel);
        Assert.False(capability.SupportsClientAuthentication);
        Assert.False(capability.SupportsClientWindowsIdentity);
        Assert.False(capability.SupportsServerAuthentication);
    }

    [WcfFact]
    public static void SecurityCapabilities_SslStreamSecurityBindingElement()
    {
        ISecurityCapabilities capability = CreateISecurityCapabilities(new SslStreamSecurityBindingElement());

        Assert.Equal(ProtectionLevel.EncryptAndSign, capability.SupportedRequestProtectionLevel);
        Assert.Equal(ProtectionLevel.EncryptAndSign, capability.SupportedResponseProtectionLevel);
        Assert.False(capability.SupportsClientAuthentication);
        Assert.False(capability.SupportsClientWindowsIdentity);
        Assert.True(capability.SupportsServerAuthentication);
    }

    [WcfFact]
    public static void SecurityCapabilities_WindowsStreamSecurityBindingElement()
    {
        ISecurityCapabilities capability = CreateISecurityCapabilities(new WindowsStreamSecurityBindingElement());

        Assert.Equal(ProtectionLevel.EncryptAndSign, capability.SupportedRequestProtectionLevel);
        Assert.Equal(ProtectionLevel.EncryptAndSign, capability.SupportedResponseProtectionLevel);
        Assert.True(capability.SupportsClientAuthentication);
        Assert.True(capability.SupportsClientWindowsIdentity);
        Assert.True(capability.SupportsServerAuthentication);
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
