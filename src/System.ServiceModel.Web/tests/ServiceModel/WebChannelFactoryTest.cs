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

public static class WebChannelFactoryTest
{
    [WcfFact]
    public static void Ctor_With_Http_Uri_Uses_Default_WebHttpBinding()
    {
        using (WebChannelFactory<IWebHttpBindingTestService> factory =
            new WebChannelFactory<IWebHttpBindingTestService>(new Uri("http://localhost:8080/svc")))
        {
            WebHttpBinding binding = Assert.IsType<WebHttpBinding>(factory.Endpoint.Binding);
            Assert.Equal(WebHttpSecurityMode.None, binding.Security.Mode);
            Assert.Equal("http://localhost:8080/svc", factory.Endpoint.Address.Uri.AbsoluteUri);
        }
    }

    [WcfFact]
    public static void Ctor_With_Https_Uri_Enables_Transport_Security()
    {
        using (WebChannelFactory<IWebHttpBindingTestService> factory =
            new WebChannelFactory<IWebHttpBindingTestService>(new Uri("https://localhost:8443/svc")))
        {
            WebHttpBinding binding = Assert.IsType<WebHttpBinding>(factory.Endpoint.Binding);
            Assert.Equal(WebHttpSecurityMode.Transport, binding.Security.Mode);
            Assert.Equal(HttpClientCredentialType.None, binding.Security.Transport.ClientCredentialType);
        }
    }

    [WcfFact]
    public static void Ctor_With_Non_Http_Uri_Has_No_Default_Binding()
    {
        // Only http/https get a default WebHttpBinding; anything else must be configured explicitly.
        Assert.Throws<ArgumentNullException>(
            () => new WebChannelFactory<IWebHttpBindingTestService>(new Uri("net.tcp://localhost:8080/svc")));
    }

    [WcfFact]
    public static void Ctor_With_Binding_And_Uri_Preserves_Both()
    {
        WebHttpBinding binding = new WebHttpBinding { Name = "custom" };

        using (WebChannelFactory<IWebHttpBindingTestService> factory =
            new WebChannelFactory<IWebHttpBindingTestService>(binding, new Uri("http://localhost:8080/svc")))
        {
            Assert.Same(binding, factory.Endpoint.Binding);
            Assert.Equal("http://localhost:8080/svc", factory.Endpoint.Address.Uri.AbsoluteUri);
        }
    }

    [WcfFact]
    public static void Ctor_With_ServiceEndpoint_Preserves_Endpoint()
    {
        ContractDescription contract = ContractDescription.GetContract(typeof(IWebHttpBindingTestService));
        ServiceEndpoint endpoint = new ServiceEndpoint(contract, new WebHttpBinding(),
            new EndpointAddress("http://localhost:8080/svc"));

        using (WebChannelFactory<IWebHttpBindingTestService> factory =
            new WebChannelFactory<IWebHttpBindingTestService>(endpoint))
        {
            Assert.Same(endpoint, factory.Endpoint);
        }
    }

    [WcfFact]
    public static void Opening_Adds_WebHttpBehavior_Automatically()
    {
        using (WebChannelFactory<IWebHttpBindingTestService> factory =
            new WebChannelFactory<IWebHttpBindingTestService>(new Uri("http://localhost:8080/svc")))
        {
            Assert.Null(factory.Endpoint.Behaviors.Find<WebHttpBehavior>());

            factory.Open();

            Assert.NotNull(factory.Endpoint.Behaviors.Find<WebHttpBehavior>());
        }
    }

    [WcfFact]
    public static void Opening_Preserves_An_Explicitly_Configured_WebHttpBehavior()
    {
        using (WebChannelFactory<IWebHttpBindingTestService> factory =
            new WebChannelFactory<IWebHttpBindingTestService>(new Uri("http://localhost:8080/svc")))
        {
            WebHttpBehavior behavior = new WebHttpBehavior
            {
                DefaultOutgoingRequestFormat = WebMessageFormat.Json
            };
            factory.Endpoint.Behaviors.Add(behavior);

            factory.Open();

            Assert.Same(behavior, factory.Endpoint.Behaviors.Find<WebHttpBehavior>());
        }
    }

    [WcfFact]
    public static void Opening_Stream_Contract_Installs_Raw_ContentTypeMapper()
    {
        using (WebChannelFactory<IWebHttpRawTestService> factory =
            new WebChannelFactory<IWebHttpRawTestService>(
                new WebHttpBinding(), new Uri("http://localhost:8080/svc")))
        {
            Assert.Null(factory.Endpoint.Binding.CreateBindingElements()
                .Find<WebMessageEncodingBindingElement>().ContentTypeMapper);

            factory.Open();

            WebContentTypeMapper mapper = factory.Endpoint.Binding.CreateBindingElements()
                .Find<WebMessageEncodingBindingElement>().ContentTypeMapper;
            Assert.NotNull(mapper);
            Assert.Equal(WebContentFormat.Raw, mapper.GetMessageFormatForContentType("application/json"));
            Assert.Equal(WebContentFormat.Raw, mapper.GetMessageFormatForContentType("text/plain"));
        }
    }

    [WcfFact]
    public static void CreateChannel_Produces_A_Usable_Proxy()
    {
        using (WebChannelFactory<IWebHttpBindingTestService> factory =
            new WebChannelFactory<IWebHttpBindingTestService>(new Uri("http://localhost:8080/svc")))
        {
            IWebHttpBindingTestService channel = factory.CreateChannel();

            Assert.NotNull(channel);
            Assert.IsAssignableFrom<IClientChannel>(channel);
            ((IClientChannel)channel).Abort();
        }
    }
}
