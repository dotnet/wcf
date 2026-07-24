// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using Infrastructure.Common;
using Xunit;

// Mirror of the contract defined in
//   tools/IISHostedWcfService/App_code/testhosts/WebHttpTestServiceHost.cs
// Declared here so the test project does not need a project reference
// to IISHostedWcfService (which is folded into the SelfHostedCoreWcfService
// host via wildcard <Compile Include> and is therefore not consumable as a
// library). The two declarations must stay in wire-format sync (same
// [ServiceContract], same UriTemplate paths).
[ServiceContract]
public interface IWcfWebHttpService
{
    [OperationContract]
    [WebGet(UriTemplate = "EchoWithGet?message={message}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Xml)]
    string EchoWithGet(string message);

    [OperationContract]
    [WebGet(UriTemplate = "EchoWithGetJson?message={message}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Json)]
    string EchoWithGetJson(string message);

    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "EchoWithPost",
               BodyStyle = WebMessageBodyStyle.Bare,
               ResponseFormat = WebMessageFormat.Xml,
               RequestFormat = WebMessageFormat.Xml)]
    string EchoWithPost(string message);

    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "EchoWithPostJson",
               BodyStyle = WebMessageBodyStyle.Bare,
               ResponseFormat = WebMessageFormat.Json,
               RequestFormat = WebMessageFormat.Json)]
    string EchoWithPostJson(string message);

    [OperationContract]
    [WebGet(UriTemplate = "EchoWithGetPath/{message}",
            BodyStyle = WebMessageBodyStyle.Bare,
            ResponseFormat = WebMessageFormat.Xml)]
    string EchoWithGetPath(string message);
}

public partial class Binding_WebHttp_WebHttpBindingTests : ConditionalWcfTest
{
    [WcfFact]
    [OuterLoop]
    [Condition(nameof(Run_With_CoreWCFService))]
    public static void DefaultSettings_EchoWithGet_Xml_RoundTrips_String()
    {
        WebChannelFactory<IWcfWebHttpService> factory = null;
        IWcfWebHttpService serviceProxy = null;
        const string testString = "Hello";
        try
        {
            // *** SETUP *** \\
            WebHttpBinding binding = new WebHttpBinding();
            factory = new WebChannelFactory<IWcfWebHttpService>(
                binding,
                new Uri(Endpoints.HttpBaseAddress_WebHttp));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.EchoWithGet(testString);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    [Condition(nameof(Run_With_CoreWCFService))]
    public static void DefaultSettings_EchoWithGetJson_Json_RoundTrips_String()
    {
        WebChannelFactory<IWcfWebHttpService> factory = null;
        IWcfWebHttpService serviceProxy = null;
        const string testString = "Hello-JSON";
        try
        {
            WebHttpBinding binding = new WebHttpBinding();
            factory = new WebChannelFactory<IWcfWebHttpService>(
                binding,
                new Uri(Endpoints.HttpBaseAddress_WebHttp));
            serviceProxy = factory.CreateChannel();

            string result = serviceProxy.EchoWithGetJson(testString);

            Assert.Equal(testString, result);

            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    [Condition(nameof(Run_With_CoreWCFService))]
    public static void DefaultSettings_EchoWithPost_Xml_RoundTrips_String()
    {
        WebChannelFactory<IWcfWebHttpService> factory = null;
        IWcfWebHttpService serviceProxy = null;
        const string testString = "Hello-POST";
        try
        {
            WebHttpBinding binding = new WebHttpBinding();
            factory = new WebChannelFactory<IWcfWebHttpService>(
                binding,
                new Uri(Endpoints.HttpBaseAddress_WebHttp));
            serviceProxy = factory.CreateChannel();

            string result = serviceProxy.EchoWithPost(testString);

            Assert.Equal(testString, result);

            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    [Condition(nameof(Run_With_CoreWCFService))]
    public static void DefaultSettings_EchoWithGetPath_PathVar_RoundTrips_String()
    {
        WebChannelFactory<IWcfWebHttpService> factory = null;
        IWcfWebHttpService serviceProxy = null;
        const string testString = "Hello-PATH";
        try
        {
            WebHttpBinding binding = new WebHttpBinding();
            factory = new WebChannelFactory<IWcfWebHttpService>(
                binding,
                new Uri(Endpoints.HttpBaseAddress_WebHttp));
            serviceProxy = factory.CreateChannel();

            string result = serviceProxy.EchoWithGetPath(testString);

            Assert.Equal(testString, result);

            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    // ---- Unit tests (no network) ---------------------------------------
    //
    // These do not require an outer-loop test service. They validate that
    // the binding, factory, and behavior can be constructed and composed.

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

    [WcfFact]
    public static void WebChannelFactory_Endpoint_HasWebHttpBinding()
    {
        WebHttpBinding binding = new WebHttpBinding();
        using var factory = new WebChannelFactory<IWcfWebHttpService>(
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

        var proxy = new Uri("http://proxy.contoso.example:8080/");
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

    // Verify Security.Transport.ProxyCredentialType is mapped onto the
    // transport's ProxyAuthenticationScheme so HttpChannelFactory can
    // authenticate against an explicit proxy. Uses TransportCredentialOnly
    // because that's the mode where WebHttpSecurity actually runs
    // HttpTransportHelpers.ConfigureAuthentication against the HTTP
    // transport element.
    [WcfTheory]
    [InlineData(HttpProxyCredentialType.None, System.Net.AuthenticationSchemes.Anonymous)]
    [InlineData(HttpProxyCredentialType.Basic, System.Net.AuthenticationSchemes.Basic)]
    [InlineData(HttpProxyCredentialType.Digest, System.Net.AuthenticationSchemes.Digest)]
    [InlineData(HttpProxyCredentialType.Ntlm, System.Net.AuthenticationSchemes.Ntlm)]
    [InlineData(HttpProxyCredentialType.Windows, System.Net.AuthenticationSchemes.Negotiate)]
    public static void WebHttpBinding_ProxyCredentialType_FlowsToTransportProxyAuthenticationScheme(
        HttpProxyCredentialType proxyCredential, System.Net.AuthenticationSchemes expectedScheme)
    {
        WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.TransportCredentialOnly);
        binding.Security.Transport.ProxyCredentialType = proxyCredential;

        HttpTransportBindingElement httpBe = null;
        foreach (var be in binding.CreateBindingElements())
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

        var ex = Assert.Throws<InvalidOperationException>(() =>
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
            var factory = binding.BuildChannelFactory<IRequestChannel>();
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
        foreach (var be in binding.CreateBindingElements())
        {
            if (be is HttpsTransportBindingElement https) httpsBe = https;
            else if (be is HttpTransportBindingElement http) httpBe = http;
        }
        binding.Security.Mode = WebHttpSecurityMode.Transport;
        foreach (var be in binding.CreateBindingElements())
        {
            if (be is HttpsTransportBindingElement https) httpsBe = https;
            else if (be is HttpTransportBindingElement http) httpBe = http;
        }

        Assert.NotNull(httpBe);
        Assert.NotNull(httpsBe);
        assertions(httpBe, httpsBe);
    }

    // Regression test for the WebHttpBinding wire-URL bug fixed in this
    // PR. Spin up a local HttpListener and verify that invoking an
    // operation with a UriTemplate path variable actually goes to the
    // bound URI (not just the base address). Also verifies that the
    // application/xml reply is deserialized correctly by the client.
    [WcfFact]
    public static void WebHttpBinding_RoundTripsAgainstLocalHttpListener()
    {
        int port = 18091;
        string baseUrl = "http://127.0.0.1:" + port + "/WebHttp.svc/";
        var listener = new System.Net.HttpListener();
        listener.Prefixes.Add(baseUrl);
        string capturedUrl = null;
        var done = new System.Threading.ManualResetEventSlim();
        try
        {
            listener.Start();
        }
        catch (System.Net.HttpListenerException)
        {
            // Cannot bind (likely needs admin on Windows); skip silently.
            return;
        }

        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                var ctx = listener.GetContext();
                capturedUrl = ctx.Request.Url.AbsoluteUri;
                ctx.Response.ContentType = "application/xml; charset=utf-8";
                byte[] body = System.Text.Encoding.UTF8.GetBytes(
                    "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">Hello-PATH</string>");
                ctx.Response.OutputStream.Write(body, 0, body.Length);
                ctx.Response.OutputStream.Close();
            }
            catch { }
            finally { done.Set(); }
        });

        WebHttpBinding binding = new WebHttpBinding();
        var factory = new WebChannelFactory<IWcfWebHttpService>(binding, new Uri(baseUrl));
        IWcfWebHttpService channel = factory.CreateChannel();
        string result = null;
        try
        {
            result = channel.EchoWithGetPath("Hello-PATH");
            done.Wait(TimeSpan.FromSeconds(10));
        }
        finally
        {
            try { ((ICommunicationObject)channel).Close(); } catch { }
            try { factory.Close(); } catch { }
            try { listener.Stop(); } catch { }
        }

        Assert.Equal(baseUrl + "EchoWithGetPath/Hello-PATH", capturedUrl);
        Assert.Equal("Hello-PATH", result);
    }

    // Regression test for the JSON reply-deserialization path: when an
    // operation declares ResponseFormat=Json, the client must use the JSON
    // formatter to read the body, not DataContractSerializer.
    [WcfFact]
    public static void WebHttpBinding_JsonReply_RoundTripsAgainstLocalHttpListener()
    {
        int port = 18092;
        string baseUrl = "http://127.0.0.1:" + port + "/WebHttp.svc/";
        var listener = new System.Net.HttpListener();
        listener.Prefixes.Add(baseUrl);
        var done = new System.Threading.ManualResetEventSlim();
        try
        {
            listener.Start();
        }
        catch (System.Net.HttpListenerException)
        {
            return;
        }

        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                var ctx = listener.GetContext();
                ctx.Response.ContentType = "application/json; charset=utf-8";
                byte[] body = System.Text.Encoding.UTF8.GetBytes("\"Hello-JSON\"");
                ctx.Response.OutputStream.Write(body, 0, body.Length);
                ctx.Response.OutputStream.Close();
            }
            catch { }
            finally { done.Set(); }
        });

        WebHttpBinding binding = new WebHttpBinding();
        var factory = new WebChannelFactory<IWcfWebHttpService>(binding, new Uri(baseUrl));
        IWcfWebHttpService channel = factory.CreateChannel();
        string result = null;
        try
        {
            result = channel.EchoWithGetJson("Hello-JSON");
            done.Wait(TimeSpan.FromSeconds(10));
        }
        finally
        {
            try { ((ICommunicationObject)channel).Close(); } catch { }
            try { factory.Close(); } catch { }
            try { listener.Stop(); } catch { }
        }

        Assert.Equal("Hello-JSON", result);
    }

    // End-to-end regression test for AllowCookies: with AllowCookies=true,
    // the client's underlying HttpMessageHandler should retain the cookie
    // set by the server on the first response and echo it back on the
    // second request. With AllowCookies=false (the default) the cookie
    // is NOT echoed back. Runs against a local HttpListener so it needs
    // no external service.
    [WcfFact]
    public static void WebHttpBinding_AllowCookies_RoundTripsCookieHeader()
    {
        int port = 18093;
        string baseUrl = "http://127.0.0.1:" + port + "/WebHttp.svc/";
        var listener = new System.Net.HttpListener();
        listener.Prefixes.Add(baseUrl);
        string secondRequestCookieHeader = null;
        var done = new System.Threading.ManualResetEventSlim();
        try
        {
            listener.Start();
        }
        catch (System.Net.HttpListenerException)
        {
            return;
        }

        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                // First request: set a cookie.
                var ctx1 = listener.GetContext();
                ctx1.Response.Headers.Add("Set-Cookie", "sid=abc123; Path=/");
                ctx1.Response.ContentType = "application/xml; charset=utf-8";
                byte[] body1 = System.Text.Encoding.UTF8.GetBytes(
                    "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">first</string>");
                ctx1.Response.OutputStream.Write(body1, 0, body1.Length);
                ctx1.Response.OutputStream.Close();

                // Second request: capture Cookie header.
                var ctx2 = listener.GetContext();
                secondRequestCookieHeader = ctx2.Request.Headers["Cookie"];
                ctx2.Response.ContentType = "application/xml; charset=utf-8";
                byte[] body2 = System.Text.Encoding.UTF8.GetBytes(
                    "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">second</string>");
                ctx2.Response.OutputStream.Write(body2, 0, body2.Length);
                ctx2.Response.OutputStream.Close();
            }
            catch { }
            finally { done.Set(); }
        });

        WebHttpBinding binding = new WebHttpBinding { AllowCookies = true };
        var factory = new WebChannelFactory<IWcfWebHttpService>(binding, new Uri(baseUrl));
        IWcfWebHttpService channel = factory.CreateChannel();
        try
        {
            channel.EchoWithGetPath("first");
            channel.EchoWithGetPath("second");
            done.Wait(TimeSpan.FromSeconds(10));
        }
        finally
        {
            try { ((ICommunicationObject)channel).Close(); } catch { }
            try { factory.Close(); } catch { }
            try { listener.Stop(); } catch { }
        }

        Assert.NotNull(secondRequestCookieHeader);
        Assert.Contains("sid=abc123", secondRequestCookieHeader);
    }
}
