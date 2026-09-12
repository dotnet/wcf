// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public static class WebHttpBindingTest
{
    [WcfFact]
    public static void Default_Ctor_Initializes_Properties()
    {
        WebHttpBinding binding = new WebHttpBinding();

        Assert.Equal("http", binding.Scheme);
        Assert.Equal(WebHttpSecurityMode.None, binding.Security.Mode);
        Assert.Equal(EnvelopeVersion.None, binding.EnvelopeVersion);
        Assert.Equal(MessageVersion.None, binding.MessageVersion);
        Assert.False(binding.AllowCookies);
        Assert.False(binding.BypassProxyOnLocal);
        Assert.True(binding.UseDefaultWebProxy);
        Assert.Null(binding.ProxyAddress);
        Assert.Null(binding.ContentTypeMapper);
        Assert.False(binding.CrossDomainScriptAccessEnabled);
        Assert.Equal(TransferMode.Buffered, binding.TransferMode);
        Assert.Equal(524288, binding.MaxBufferPoolSize);
        Assert.Equal(65536, binding.MaxBufferSize);
        Assert.Equal(65536, binding.MaxReceivedMessageSize);
        Assert.Equal(Encoding.UTF8.WebName, binding.WriteEncoding.WebName);
        Assert.Equal(TimeSpan.FromMinutes(1), binding.OpenTimeout);
        Assert.Equal(TimeSpan.FromMinutes(1), binding.CloseTimeout);
        Assert.Equal(TimeSpan.FromMinutes(1), binding.SendTimeout);
        Assert.Equal(TimeSpan.FromMinutes(10), binding.ReceiveTimeout);
        Assert.NotNull(binding.ReaderQuotas);
    }

    [WcfFact]
    public static void WebHttpBinding_CanBeConstructed()
    {
        WebHttpBinding binding = new WebHttpBinding();
        Assert.NotNull(binding);
        Assert.Equal("http", binding.Scheme);
        Assert.Equal(WebHttpSecurityMode.None, binding.Security.Mode);
    }

    [WcfFact]
    public static void WebHttpBinding_TransportMode_UsesHttps()
    {
        WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.Transport);
        Assert.Equal("https", binding.Scheme);
    }

    [WcfTheory]
    [InlineData(WebHttpSecurityMode.None, "http")]
    [InlineData(WebHttpSecurityMode.TransportCredentialOnly, "http")]
    [InlineData(WebHttpSecurityMode.Transport, "https")]
    public static void Scheme_Follows_SecurityMode(WebHttpSecurityMode mode, string expectedScheme)
    {
        WebHttpBinding binding = new WebHttpBinding(mode);
        Assert.Equal(expectedScheme, binding.Scheme);
    }

    // The binding must produce exactly the encoder + transport pair, in that
    // order. Order matters: the encoding element has to sit above the transport
    // so the transport can pick up the message encoder factory.
    [WcfTheory]
    [InlineData(WebHttpSecurityMode.None)]
    [InlineData(WebHttpSecurityMode.TransportCredentialOnly)]
    public static void CreateBindingElements_Yields_Encoder_Then_HttpTransport(WebHttpSecurityMode mode)
    {
        WebHttpBinding binding = new WebHttpBinding(mode);
        BindingElementCollection elements = binding.CreateBindingElements();

        Assert.Equal(2, elements.Count);
        Assert.IsType<WebMessageEncodingBindingElement>(elements[0]);
        Assert.IsType<HttpTransportBindingElement>(elements[1]);
    }

    [WcfFact]
    public static void CreateBindingElements_TransportMode_Yields_HttpsTransport()
    {
        WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.Transport);
        BindingElementCollection elements = binding.CreateBindingElements();

        Assert.Equal(2, elements.Count);
        Assert.IsType<WebMessageEncodingBindingElement>(elements[0]);
        Assert.IsType<HttpsTransportBindingElement>(elements[1]);
    }

    [WcfFact]
    public static void CreateBindingElements_Returns_Independent_Collections()
    {
        WebHttpBinding binding = new WebHttpBinding();
        BindingElementCollection first = binding.CreateBindingElements();
        BindingElementCollection second = binding.CreateBindingElements();

        Assert.NotSame(first, second);
    }

    [WcfFact]
    public static void Properties_RoundTrip()
    {
        Uri proxy = new Uri("http://proxy.contoso.example:8080/");
        WebHttpBinding binding = new WebHttpBinding
        {
            AllowCookies = true,
            BypassProxyOnLocal = true,
            UseDefaultWebProxy = false,
            ProxyAddress = proxy,
            MaxBufferPoolSize = 1024,
            MaxBufferSize = 2048,
            MaxReceivedMessageSize = 4096,
            TransferMode = TransferMode.Streamed,
            WriteEncoding = Encoding.Unicode,
            CrossDomainScriptAccessEnabled = true
        };

        Assert.True(binding.AllowCookies);
        Assert.True(binding.BypassProxyOnLocal);
        Assert.False(binding.UseDefaultWebProxy);
        Assert.Equal(proxy, binding.ProxyAddress);
        Assert.Equal(1024, binding.MaxBufferPoolSize);
        Assert.Equal(2048, binding.MaxBufferSize);
        Assert.Equal(4096, binding.MaxReceivedMessageSize);
        Assert.Equal(TransferMode.Streamed, binding.TransferMode);
        Assert.Equal(Encoding.Unicode.WebName, binding.WriteEncoding.WebName);
        Assert.True(binding.CrossDomainScriptAccessEnabled);
    }

    [WcfFact]
    public static void ReaderQuotas_Null_Throws()
    {
        WebHttpBinding binding = new WebHttpBinding();
        Assert.Throws<ArgumentNullException>(() => binding.ReaderQuotas = null);
    }

    [WcfFact]
    public static void ReaderQuotas_Are_Copied_Not_Aliased()
    {
        WebHttpBinding binding = new WebHttpBinding();
        XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas { MaxStringContentLength = 1234 };
        binding.ReaderQuotas = quotas;

        Assert.Equal(1234, binding.ReaderQuotas.MaxStringContentLength);
        Assert.NotSame(quotas, binding.ReaderQuotas);

        // Mutating the original must not affect the binding.
        quotas.MaxStringContentLength = 4321;
        Assert.Equal(1234, binding.ReaderQuotas.MaxStringContentLength);
    }

    [WcfFact]
    public static void ReaderQuotas_Flow_To_EncodingBindingElement()
    {
        WebHttpBinding binding = new WebHttpBinding();
        binding.ReaderQuotas = new XmlDictionaryReaderQuotas { MaxArrayLength = 777 };

        WebMessageEncodingBindingElement encoding =
            binding.CreateBindingElements().Find<WebMessageEncodingBindingElement>();

        Assert.NotNull(encoding);
        Assert.Equal(777, encoding.ReaderQuotas.MaxArrayLength);
    }

    [WcfFact]
    public static void WriteEncoding_Flows_To_EncodingBindingElement()
    {
        WebHttpBinding binding = new WebHttpBinding { WriteEncoding = Encoding.Unicode };

        WebMessageEncodingBindingElement encoding =
            binding.CreateBindingElements().Find<WebMessageEncodingBindingElement>();

        Assert.Equal(Encoding.Unicode.WebName, encoding.WriteEncoding.WebName);
    }

    [WcfFact]
    public static void ContentTypeMapper_Flows_To_EncodingBindingElement()
    {
        WebContentTypeMapper mapper = new FixedWebContentTypeMapper(WebContentFormat.Json);
        WebHttpBinding binding = new WebHttpBinding { ContentTypeMapper = mapper };

        WebMessageEncodingBindingElement encoding =
            binding.CreateBindingElements().Find<WebMessageEncodingBindingElement>();

        Assert.Same(mapper, encoding.ContentTypeMapper);
    }

    [WcfFact]
    public static void Security_Null_Throws()
    {
        WebHttpBinding binding = new WebHttpBinding();
        Assert.Throws<ArgumentNullException>(() => binding.Security = null);
    }

    [WcfFact]
    public static void WebChannelFactory_Endpoint_HasWebHttpBinding()
    {
        WebHttpBinding binding = new WebHttpBinding();
        using WebChannelFactory<IWebHttpBindingTestService> factory = new WebChannelFactory<IWebHttpBindingTestService>(
            binding, new Uri("http://localhost/dummy/"));
        ServiceEndpoint endpoint = factory.Endpoint;
        Assert.NotNull(endpoint);
        Assert.IsType<WebHttpBinding>(endpoint.Binding);
    }

    // Verify AllowCookies flows to BOTH the HTTP and HTTPS transport binding
    // elements (WebHttpBinding wraps both). This is how WSHttpBinding /
    // HttpBindingBase already model shared HTTP settings.
    [WcfFact]
    public static void WebHttpBinding_AllowCookies_PropagatesToBothTransports()
    {
        WebHttpBinding binding = new WebHttpBinding();
        Assert.False(binding.AllowCookies);

        binding.AllowCookies = true;
        AssertTransportPropertyFlows(binding, (http, https) =>
        {
            Assert.True(http.AllowCookies);
            Assert.True(https.AllowCookies);
        });
    }

    [WcfFact]
    public static void WebHttpBinding_ProxyAddress_PropagatesToBothTransports()
    {
        WebHttpBinding binding = new WebHttpBinding();
        Assert.Null(binding.ProxyAddress);

        Uri proxy = new Uri("http://proxy.contoso.example:8080/");
        binding.ProxyAddress = proxy;
        AssertTransportPropertyFlows(binding, (http, https) =>
        {
            Assert.Equal(proxy, http.ProxyAddress);
            Assert.Equal(proxy, https.ProxyAddress);
        });
    }

    [WcfFact]
    public static void WebHttpBinding_UseDefaultWebProxy_And_BypassProxyOnLocal_PropagateToBothTransports()
    {
        WebHttpBinding binding = new WebHttpBinding();
        Assert.True(binding.UseDefaultWebProxy);
        Assert.False(binding.BypassProxyOnLocal);

        binding.UseDefaultWebProxy = false;
        binding.BypassProxyOnLocal = true;
        AssertTransportPropertyFlows(binding, (http, https) =>
        {
            Assert.False(http.UseDefaultWebProxy);
            Assert.False(https.UseDefaultWebProxy);
            Assert.True(http.BypassProxyOnLocal);
            Assert.True(https.BypassProxyOnLocal);
        });
    }

    [WcfFact]
    public static void WebHttpBinding_TransferMode_PropagatesToBothTransports()
    {
        WebHttpBinding binding = new WebHttpBinding { TransferMode = TransferMode.Streamed };
        AssertTransportPropertyFlows(binding, (http, https) =>
        {
            Assert.Equal(TransferMode.Streamed, http.TransferMode);
            Assert.Equal(TransferMode.Streamed, https.TransferMode);
        });
    }

    // Verify Security.Transport.ProxyCredentialType is mapped onto the
    // transport's ProxyAuthenticationScheme so HttpChannelFactory can
    // authenticate against an explicit proxy. Uses TransportCredentialOnly
    // because that's the mode where WebHttpSecurity actually runs
    // HttpTransportHelpers.ConfigureAuthentication against the HTTP
    // transport element.
    [WcfTheory]
    [InlineData(HttpProxyCredentialType.None, AuthenticationSchemes.Anonymous)]
    [InlineData(HttpProxyCredentialType.Basic, AuthenticationSchemes.Basic)]
    [InlineData(HttpProxyCredentialType.Digest, AuthenticationSchemes.Digest)]
    [InlineData(HttpProxyCredentialType.Ntlm, AuthenticationSchemes.Ntlm)]
    [InlineData(HttpProxyCredentialType.Windows, AuthenticationSchemes.Negotiate)]
    public static void WebHttpBinding_ProxyCredentialType_FlowsToTransportProxyAuthenticationScheme(
        HttpProxyCredentialType proxyCredential, AuthenticationSchemes expectedScheme)
    {
        WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
        binding.Security.Transport.ProxyCredentialType = proxyCredential;

        HttpTransportBindingElement httpBe = null;
        foreach (BindingElement be in binding.CreateBindingElements())
        {
            if (be is HttpTransportBindingElement http && !(be is HttpsTransportBindingElement))
            {
                httpBe = http;
            }
        }

        Assert.NotNull(httpBe);
        Assert.Equal(expectedScheme, httpBe.ProxyAuthenticationScheme);
    }

    // ClientCredentialType.InheritedFromHost is only valid on server hosts.
    // Constructing a client channel factory with it should throw
    // InvalidOperationException up front (matching .NET Framework), not fail
    // deeper inside HttpTransportBindingElement with ArgumentException.
    // We exercise the guard by calling binding.BuildChannelFactory<T> directly
    // — the same path ChannelFactory.CreateFactory() takes during Open().
    [WcfTheory]
    [InlineData(WebHttpSecurityMode.Transport)]
    [InlineData(WebHttpSecurityMode.TransportCredentialOnly)]
    public static void WebHttpBinding_InheritedFromHost_ThrowsInvalidOperationExceptionAtFactoryCreation(
        WebHttpSecurityMode mode)
    {
        WebHttpBinding binding = new WebHttpBinding(mode);
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.InheritedFromHost;

        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() =>
            binding.BuildChannelFactory<IRequestChannel>());
        Assert.Contains("InheritedFromHost", ex.Message);
    }

    // In WebHttpSecurityMode.None, InheritedFromHost is not applicable so
    // the guard must NOT fire.
    [WcfFact]
    public static void WebHttpBinding_InheritedFromHost_DoesNotThrow_WhenSecurityModeIsNone()
    {
        WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.None);
        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.InheritedFromHost;

        // The guard doesn't apply in None mode, so BuildChannelFactory itself
        // should succeed (although downstream HttpTransportBindingElement may
        // still reject the resulting AuthenticationSchemes.None). We only
        // assert that our guard doesn't fire.
        try
        {
            IChannelFactory<IRequestChannel> factory = binding.BuildChannelFactory<IRequestChannel>();
            factory.Close();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("InheritedFromHost"))
        {
            Assert.Fail("Guard should not fire in WebHttpSecurityMode.None");
        }
        catch
        {
            // Any other exception is unrelated to our guard and acceptable.
        }
    }

    // Walk both the HTTP and HTTPS transport elements the binding owns and
    // invoke assertions against them. We CreateBindingElements() for both
    // security modes (None -> yields HTTP, Transport -> yields HTTPS) so
    // both underlying elements are observed.
    private static void AssertTransportPropertyFlows(
        WebHttpBinding binding, Action<HttpTransportBindingElement, HttpsTransportBindingElement> assertions)
    {
        HttpTransportBindingElement httpBe = null;
        HttpsTransportBindingElement httpsBe = null;

        binding.Security.Mode = WebHttpSecurityMode.None;
        foreach (BindingElement be in binding.CreateBindingElements())
        {
            if (be is HttpsTransportBindingElement https)
            {
                httpsBe = https;
            }
            else if (be is HttpTransportBindingElement http)
            {
                httpBe = http;
            }
        }

        binding.Security.Mode = WebHttpSecurityMode.Transport;
        foreach (BindingElement be in binding.CreateBindingElements())
        {
            if (be is HttpsTransportBindingElement https)
            {
                httpsBe = https;
            }
            else if (be is HttpTransportBindingElement http)
            {
                httpBe = http;
            }
        }

        Assert.NotNull(httpBe);
        Assert.NotNull(httpsBe);
        assertions(httpBe, httpsBe);
    }

    private sealed class FixedWebContentTypeMapper : WebContentTypeMapper
    {
        private readonly WebContentFormat _format;

        public FixedWebContentTypeMapper(WebContentFormat format) => _format = format;

        public override WebContentFormat GetMessageFormatForContentType(string contentType) => _format;
    }
}
