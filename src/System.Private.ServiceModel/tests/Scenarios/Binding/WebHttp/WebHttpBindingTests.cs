// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
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
}
