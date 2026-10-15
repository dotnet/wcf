// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using Infrastructure.Common;
using Xunit;

public static class WebOperationContextTest
{
    private static OperationContext CreateOperationContext()
    {
        WebHttpBinding binding = new WebHttpBinding();
        ChannelFactory<IRequestChannel> factory = new ChannelFactory<IRequestChannel>(
            binding, new EndpointAddress("http://localhost:8080/svc"));
        IRequestChannel channel = factory.CreateChannel();

        return new OperationContext((IContextChannel)channel);
    }

    [WcfFact]
    public static void Ctor_Null_OperationContext_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new WebOperationContext(null));
    }

    [WcfFact]
    public static void Current_Is_Null_Without_An_Ambient_OperationContext()
    {
        Assert.Null(OperationContext.Current);
        Assert.Null(WebOperationContext.Current);
    }

    [WcfFact]
    public static void Current_Creates_And_Caches_A_Context_Inside_An_OperationContextScope()
    {
        using (new OperationContextScope(CreateOperationContext()))
        {
            WebOperationContext context = WebOperationContext.Current;

            Assert.NotNull(context);
            Assert.Same(context, WebOperationContext.Current);
        }

        Assert.Null(WebOperationContext.Current);
    }

    [WcfFact]
    public static void Current_Is_Registered_As_An_OperationContext_Extension()
    {
        using (new OperationContextScope(CreateOperationContext()))
        {
            WebOperationContext context = WebOperationContext.Current;

            Assert.Same(context, OperationContext.Current.Extensions.Find<WebOperationContext>());
        }
    }

    [WcfFact]
    public static void OutgoingRequest_Defaults_Are_Empty()
    {
        using (new OperationContextScope(CreateOperationContext()))
        {
            OutgoingWebRequestContext request = WebOperationContext.Current.OutgoingRequest;

            Assert.NotNull(request);
            Assert.NotNull(request.Headers);
            Assert.Equal("POST", request.Method);
            Assert.False(request.SuppressEntityBody);
        }
    }

    [WcfFact]
    public static void OutgoingRequest_Properties_RoundTrip_Through_The_Message_Property()
    {
        using (new OperationContextScope(CreateOperationContext()))
        {
            OutgoingWebRequestContext request = WebOperationContext.Current.OutgoingRequest;
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.UserAgent = "wcf-tests";
            request.IfMatch = "\"etag\"";
            request.IfNoneMatch = "\"other\"";
            request.SuppressEntityBody = true;

            // Each property getter reads back through HttpRequestMessageProperty, and OutgoingRequest
            // returns a fresh wrapper each time, so a new wrapper must observe the same state.
            OutgoingWebRequestContext reread = WebOperationContext.Current.OutgoingRequest;

            Assert.Equal("PUT", reread.Method);
            Assert.Equal("application/json", reread.ContentType);
            Assert.Equal("application/json", reread.Accept);
            Assert.Equal("wcf-tests", reread.UserAgent);
            Assert.Equal("\"etag\"", reread.IfMatch);
            Assert.Equal("\"other\"", reread.IfNoneMatch);
            Assert.True(reread.SuppressEntityBody);
        }
    }

    [WcfFact]
    public static void OutgoingRequest_Headers_Are_Shared_With_The_Message_Property()
    {
        using (new OperationContextScope(CreateOperationContext()))
        {
            WebOperationContext.Current.OutgoingRequest.Headers["X-Custom"] = "value";

            Assert.Equal("value", WebOperationContext.Current.OutgoingRequest.Headers["X-Custom"]);
        }
    }

    [WcfFact]
    public static void IncomingResponse_Throws_When_No_Response_Has_Arrived()
    {
        using (new OperationContextScope(CreateOperationContext()))
        {
            IncomingWebResponseContext response = WebOperationContext.Current.IncomingResponse;

            Assert.NotNull(response);

            // Without an incoming reply there is no HttpResponseMessageProperty to read from, and
            // every accessor surfaces that as a clear InvalidOperationException.
            Assert.Throws<InvalidOperationException>(() => response.Headers);
            Assert.Throws<InvalidOperationException>(() => response.StatusCode);
            Assert.Throws<InvalidOperationException>(() => response.ContentType);
        }
    }
    [WcfFact]
    public static void Attach_And_Detach_Do_Not_Throw()
    {
        OperationContext operationContext = CreateOperationContext();
        WebOperationContext context = new WebOperationContext(operationContext);

        context.Attach(operationContext);
        context.Detach(operationContext);
    }
}
