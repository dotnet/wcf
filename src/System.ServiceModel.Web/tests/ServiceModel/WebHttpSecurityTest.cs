// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using Xunit;

public static class WebHttpSecurityTest
{
    [WcfFact]
    public static void Defaults_To_No_Security()
    {
        WebHttpSecurity security = new WebHttpSecurity();

        Assert.Equal(WebHttpSecurityMode.None, security.Mode);
        Assert.NotNull(security.Transport);
        Assert.Equal(HttpClientCredentialType.None, security.Transport.ClientCredentialType);
        Assert.Equal(HttpProxyCredentialType.None, security.Transport.ProxyCredentialType);
    }

    [WcfFact]
    public static void Mode_RoundTrips_All_Defined_Values()
    {
        WebHttpSecurityMode[] modes =
        {
            WebHttpSecurityMode.None,
            WebHttpSecurityMode.Transport,
            WebHttpSecurityMode.TransportCredentialOnly
        };

        foreach (WebHttpSecurityMode mode in modes)
        {
            WebHttpSecurity security = new WebHttpSecurity { Mode = mode };
            Assert.Equal(mode, security.Mode);
        }
    }

    [WcfFact]
    public static void Mode_Rejects_Undefined_Values()
    {
        WebHttpSecurity security = new WebHttpSecurity();

        Assert.Throws<ArgumentOutOfRangeException>(() => security.Mode = (WebHttpSecurityMode)99);
        Assert.Throws<ArgumentOutOfRangeException>(() => security.Mode = (WebHttpSecurityMode)(-1));
    }

    [WcfFact]
    public static void Transport_Settings_RoundTrip()
    {
        WebHttpSecurity security = new WebHttpSecurity();
        security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
        security.Transport.ProxyCredentialType = HttpProxyCredentialType.Ntlm;

        Assert.Equal(HttpClientCredentialType.Basic, security.Transport.ClientCredentialType);
        Assert.Equal(HttpProxyCredentialType.Ntlm, security.Transport.ProxyCredentialType);
    }

    [WcfFact]
    public static void Security_Mode_None_Yields_Plain_Http_Transport_With_Anonymous_Auth()
    {
        WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.None);
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

        HttpTransportBindingElement transport = FindTransport(binding);

        Assert.IsType<HttpTransportBindingElement>(transport);
        Assert.Equal(AuthenticationSchemes.Anonymous, transport.AuthenticationScheme);
        Assert.Equal(AuthenticationSchemes.Anonymous, transport.ProxyAuthenticationScheme);
    }

    [WcfFact]
    public static void TransportCredentialOnly_Yields_Plain_Http_Transport_With_Credentials()
    {
        WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

        HttpTransportBindingElement transport = FindTransport(binding);

        Assert.IsType<HttpTransportBindingElement>(transport);
        Assert.Equal(AuthenticationSchemes.Basic, transport.AuthenticationScheme);
        Assert.Equal(AuthenticationSchemes.Anonymous, transport.ProxyAuthenticationScheme);
        Assert.Equal("http", binding.Scheme);
    }

    [WcfFact]
    public static void Transport_Mode_Yields_Https_Transport_With_Credentials()
    {
        WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.Transport);
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;

        HttpTransportBindingElement transport = FindTransport(binding);

        Assert.IsType<HttpsTransportBindingElement>(transport);
        Assert.Equal(AuthenticationSchemes.Basic, transport.AuthenticationScheme);
        Assert.Equal(AuthenticationSchemes.Anonymous, transport.ProxyAuthenticationScheme);
        Assert.Equal("https", binding.Scheme);
    }

    [WcfFact]
    public static void Proxy_Credential_Type_Flows_To_Transport()
    {
        WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
        binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Ntlm;

        HttpTransportBindingElement transport = FindTransport(binding);

        Assert.Equal(AuthenticationSchemes.Ntlm, transport.ProxyAuthenticationScheme);
    }

    [WcfFact]
    public static void Changing_Mode_To_None_Resets_Proxy_Authentication_And_Preserves_Extended_Protection()
    {
        WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
        ExtendedProtectionPolicy policy = new ExtendedProtectionPolicy(PolicyEnforcement.Never);
        binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Ntlm;
        binding.Security.Transport.ExtendedProtectionPolicy = policy;

        HttpTransportBindingElement authenticatedTransport = FindTransport(binding);
        Assert.Equal(AuthenticationSchemes.Ntlm, authenticatedTransport.ProxyAuthenticationScheme);

        binding.Security.Mode = WebHttpSecurityMode.None;
        HttpTransportBindingElement anonymousTransport = FindTransport(binding);

        Assert.Equal(AuthenticationSchemes.Anonymous, anonymousTransport.AuthenticationScheme);
        Assert.Equal(AuthenticationSchemes.Anonymous, anonymousTransport.ProxyAuthenticationScheme);
        Assert.Same(policy, anonymousTransport.ExtendedProtectionPolicy);
    }

    private static HttpTransportBindingElement FindTransport(WebHttpBinding binding)
    {
        foreach (BindingElement element in binding.CreateBindingElements())
        {
            if (element is HttpTransportBindingElement transport)
            {
                return transport;
            }
        }

        return null;
    }
}
