// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Net;
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

// Separate raw / application/octet-stream contract, used only by the
// self-contained loopback raw round-trip test. Kept local (not shared with the
// CoreWCF host contract above) because the raw path is exercised purely against
// an in-process HttpListener.
[ServiceContract]
public interface IWcfWebHttpRawService
{
    [OperationContract]
    [WebInvoke(Method = "POST", UriTemplate = "EchoStream",
               BodyStyle = WebMessageBodyStyle.Bare)]
    Stream EchoStream(Stream stream);
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

    // Pick a random dynamic port and retry transient collisions. Fail rather
    // than skip if no listener can be started within the retry budget.
    private const int MaxPortRetries = 10;

    private static (System.Net.HttpListener listener, string baseUrl) StartLoopbackHttpListener()
    {
        var random = new Random();
        var errors = new System.Collections.Generic.List<string>();
        for (int attempt = 1; attempt <= MaxPortRetries; attempt++)
        {
            int port = random.Next(49152, 65536);
            string url = "http://127.0.0.1:" + port + "/WebHttp.svc/";
            var listener = new System.Net.HttpListener();
            listener.Prefixes.Add(url);
            try
            {
                listener.Start();
                return (listener, url);
            }
            catch (System.Net.HttpListenerException ex)
            {
                errors.Add("attempt " + attempt + " port " + port +
                    ": " + ex.ErrorCode + "/" + ex.Message);
            }
        }
        Assert.Fail(
            "Unable to find a random port number after " + MaxPortRetries +
            " attempts. Errors: " + string.Join("; ", errors));
        return default; // unreachable
    }

    private static string InvokeLoopbackReply(
        string contentType,
        string responseBody,
        Func<IWcfWebHttpService, string> invoke)
    {
        (HttpListener listener, string baseUrl) = StartLoopbackHttpListener();
        Exception listenerException = null;
        var done = new System.Threading.ManualResetEventSlim();

        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                HttpListenerContext context = listener.GetContext();
                context.Response.ContentType = contentType;
                byte[] body = System.Text.Encoding.UTF8.GetBytes(responseBody);
                context.Response.OutputStream.Write(body, 0, body.Length);
                context.Response.OutputStream.Close();
            }
            catch (Exception exception)
            {
                listenerException = exception;
            }
            finally
            {
                done.Set();
            }
        });

        var factory = new WebChannelFactory<IWcfWebHttpService>(new WebHttpBinding(), new Uri(baseUrl));
        IWcfWebHttpService channel = factory.CreateChannel();
        try
        {
            string result = invoke(channel);
            Assert.True(done.Wait(TimeSpan.FromSeconds(10)),
                "The loopback HttpListener did not complete within the timeout.");
            Assert.Null(listenerException);
            return result;
        }
        finally
        {
            try { ((ICommunicationObject)channel).Close(); } catch { }
            try { factory.Close(); } catch { }
            try { listener.Stop(); } catch { }
        }
    }

    [WcfFact]
    public static void WebHttpBinding_RoundTripsAgainstLocalHttpListener()
    {
        (HttpListener listener, string baseUrl) = StartLoopbackHttpListener();
        string capturedUrl = null;
        var done = new System.Threading.ManualResetEventSlim();

        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                HttpListenerContext ctx = listener.GetContext();
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
            Assert.True(done.Wait(TimeSpan.FromSeconds(10)),
                "The loopback HttpListener did not complete within the timeout.");
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

    // Raw / application/octet-stream pass-through round-trip. Exercises the
    // ByteStream encoder wired into WebMessageEncoder (RawMessageEncoder) plus
    // HttpStreamFormatter on both the request (Stream -> body) and reply
    // (body -> Stream) sides. This is the client-side raw path enabled by
    // porting ByteStreamMessageEncodingBindingElement into Primitives.
    [WcfFact]
    public static void WebHttpBinding_RawStream_RoundTripsAgainstLocalHttpListener()
    {
        (HttpListener listener, string baseUrl) = StartLoopbackHttpListener();
        byte[] captured = null;
        var done = new System.Threading.ManualResetEventSlim();

        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                HttpListenerContext ctx = listener.GetContext();
                using (var ms = new MemoryStream())
                {
                    ctx.Request.InputStream.CopyTo(ms);
                    captured = ms.ToArray();
                }
                // Stream-returning operations must treat the response as Raw even when
                // the server supplies a content type normally recognized as JSON.
                ctx.Response.ContentType = "application/json";
                ctx.Response.OutputStream.Write(captured, 0, captured.Length);
                ctx.Response.OutputStream.Close();
            }
            catch { }
            finally { done.Set(); }
        });

        byte[] payload = System.Text.Encoding.UTF8.GetBytes("raw-bytes-\u00e9\u00f1-42");
        WebHttpBinding binding = new WebHttpBinding();
        var factory = new WebChannelFactory<IWcfWebHttpRawService>(binding, new Uri(baseUrl));
        IWcfWebHttpRawService channel = factory.CreateChannel();
        byte[] roundTripped = null;
        try
        {
            using (Stream reply = channel.EchoStream(new MemoryStream(payload)))
            using (var ms = new MemoryStream())
            {
                reply.CopyTo(ms);
                roundTripped = ms.ToArray();
            }
            Assert.True(done.Wait(TimeSpan.FromSeconds(10)),
                "The loopback HttpListener did not complete within the timeout.");
        }
        finally
        {
            try { ((ICommunicationObject)channel).Close(); } catch { }
            try { factory.Close(); } catch { }
            try { listener.Stop(); } catch { }
        }

        Assert.Equal(payload, captured);
        Assert.Equal(payload, roundTripped);
    }

    [WcfFact]
    public static void WebHttpBinding_OperationContextScope_PreservesJsonContentType()
    {
        (HttpListener listener, string baseUrl) = StartLoopbackHttpListener();
        string capturedContentType = null;
        string capturedBody = null;
        Exception listenerException = null;
        var done = new System.Threading.ManualResetEventSlim();

        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                HttpListenerContext context = listener.GetContext();
                capturedContentType = context.Request.ContentType;
                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    capturedBody = reader.ReadToEnd();
                }

                context.Response.ContentType = "application/json; charset=utf-8";
                byte[] body = System.Text.Encoding.UTF8.GetBytes("\"Hello-JSON\"");
                context.Response.OutputStream.Write(body, 0, body.Length);
                context.Response.OutputStream.Close();
            }
            catch (Exception exception)
            {
                listenerException = exception;
            }
            finally
            {
                done.Set();
            }
        });

        WebHttpBinding binding = new WebHttpBinding();
        var factory = new WebChannelFactory<IWcfWebHttpService>(binding, new Uri(baseUrl));
        IWcfWebHttpService channel = factory.CreateChannel();
        string result = null;
        try
        {
            using (new OperationContextScope((IClientChannel)channel))
            {
                var custom = new HttpRequestMessageProperty();
                custom.Headers["X-Custom-Header"] = "value";
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = custom;

                result = channel.EchoWithPostJson("Hello-JSON");
            }

            Assert.True(done.Wait(TimeSpan.FromSeconds(10)),
                "The loopback HttpListener did not complete within the timeout.");
        }
        finally
        {
            try { ((ICommunicationObject)channel).Close(); } catch { }
            try { factory.Close(); } catch { }
            try { listener.Stop(); } catch { }
        }

        Assert.Null(listenerException);
        Assert.Equal("application/json; charset=utf-8", capturedContentType);
        Assert.Equal("\"Hello-JSON\"", capturedBody);
        Assert.Equal("Hello-JSON", result);
    }

    // Regression test for the JSON reply-deserialization path: when an
    // operation declares ResponseFormat=Json, the client must use the JSON
    // formatter to read the body, not DataContractSerializer.
    [WcfFact]
    public static void WebHttpBinding_JsonReply_RoundTripsAgainstLocalHttpListener()
    {
        (HttpListener listener, string baseUrl) = StartLoopbackHttpListener();
        var done = new System.Threading.ManualResetEventSlim();

        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                HttpListenerContext ctx = listener.GetContext();
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
            Assert.True(done.Wait(TimeSpan.FromSeconds(10)),
                "The loopback HttpListener did not complete within the timeout.");
        }
        finally
        {
            try { ((ICommunicationObject)channel).Close(); } catch { }
            try { factory.Close(); } catch { }
            try { listener.Stop(); } catch { }
        }

        Assert.Equal("Hello-JSON", result);
    }

    [WcfFact]
    public static void WebHttpBinding_XmlDeclaredReply_UsesJsonContentType()
    {
        string result = InvokeLoopbackReply(
            "application/json; charset=utf-8",
            "\"Hello-JSON\"",
            channel => channel.EchoWithGet("ignored"));

        Assert.Equal("Hello-JSON", result);
    }

    [WcfFact]
    public static void WebHttpBinding_JsonDeclaredReply_UsesXmlContentType()
    {
        string result = InvokeLoopbackReply(
            "application/xml; charset=utf-8",
            "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">Hello-XML</string>",
            channel => channel.EchoWithGetJson("ignored"));

        Assert.Equal("Hello-XML", result);
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
        (HttpListener listener, string baseUrl) = StartLoopbackHttpListener();
        string secondRequestCookieHeader = null;
        var done = new System.Threading.ManualResetEventSlim();

        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                // First request: set a cookie.
                HttpListenerContext ctx1 = listener.GetContext();
                ctx1.Response.Headers.Add("Set-Cookie", "sid=abc123; Path=/");
                ctx1.Response.ContentType = "application/xml; charset=utf-8";
                byte[] body1 = System.Text.Encoding.UTF8.GetBytes(
                    "<string xmlns=\"http://schemas.microsoft.com/2003/10/Serialization/\">first</string>");
                ctx1.Response.OutputStream.Write(body1, 0, body1.Length);
                ctx1.Response.OutputStream.Close();

                // Second request: capture Cookie header.
                HttpListenerContext ctx2 = listener.GetContext();
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
            Assert.True(done.Wait(TimeSpan.FromSeconds(10)),
                "The loopback HttpListener did not complete within the timeout.");
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

    // Regression test: when the caller opens an OperationContextScope and puts a
    // custom HttpRequestMessageProperty on OutgoingMessageProperties (a common way
    // to add per-call headers), the WebGet operation's HTTP method / SuppressEntityBody
    // must survive. Before WebOperationContext was reshaped to expose OutgoingRequest,
    // UriTemplateClientFormatter wrote Method/SuppressEntityBody onto the message
    // property, which ServiceChannel then overwrote with the ambient property -
    // reverting GET to the default POST. The formatter now routes through
    // WebOperationContext.Current.OutgoingRequest so the ambient property carries them.
    [WcfFact]
    public static void WebHttpBinding_OperationContextScope_PreservesGetMethod()
    {
        (HttpListener listener, string baseUrl) = StartLoopbackHttpListener();
        string capturedMethod = null;
        string capturedUrl = null;
        var done = new System.Threading.ManualResetEventSlim();

        System.Threading.Tasks.Task.Run(() =>
        {
            try
            {
                HttpListenerContext ctx = listener.GetContext();
                capturedMethod = ctx.Request.HttpMethod;
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
        try
        {
            using (new OperationContextScope((IClientChannel)channel))
            {
                // Simulate the caller adding a per-call custom header via an ambient
                // HttpRequestMessageProperty on the OperationContext.
                var custom = new HttpRequestMessageProperty();
                custom.Headers["X-Custom-Header"] = "value";
                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = custom;

                channel.EchoWithGetPath("Hello-PATH");
            }
            Assert.True(done.Wait(TimeSpan.FromSeconds(10)),
                "The loopback HttpListener did not complete within the timeout.");
        }
        finally
        {
            try { ((ICommunicationObject)channel).Close(); } catch { }
            try { factory.Close(); } catch { }
            try { listener.Stop(); } catch { }
        }

        // Would be "POST" (the HttpRequestMessageProperty default) if the ambient
        // property had clobbered the formatter's method.
        Assert.Equal("GET", capturedMethod);
        Assert.Equal(baseUrl + "EchoWithGetPath/Hello-PATH", capturedUrl);
    }
}
