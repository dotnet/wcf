﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public static class Binding_Http_BasicHttpBindingTests
{
    [WcfFact]
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String()
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        string testString = "Hello";
        Binding binding = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.True(result == testString, String.Format("Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void HttpKeepAliveDisabled_Echo_RoundTrips_True()
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Binding binding = null;
        CustomBinding customBinding = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            customBinding = new CustomBinding(binding);
            var httpElement = customBinding.Elements.Find<HttpTransportBindingElement>();
            httpElement.KeepAliveEnabled = false;

            factory = new ChannelFactory<IWcfService>(customBinding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            bool result = serviceProxy.IsHttpKeepAliveDisabled();

            // *** VALIDATE *** \\
            Assert.True(result, "Error: expected response from service: 'true' Actual was: 'false'");

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void HttpMessageHandlerFactory_Success()
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        string testString = "Hello";
        Binding binding = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            var handlerFactoryBehavior = new HttpMessageHandlerBehavior();
            bool handlerCalled = false;
            handlerFactoryBehavior.OnSendingAsync = (message, token) =>
            {
                handlerCalled = true;
                return Task.FromResult((HttpResponseMessage)null);
            };
            factory.Endpoint.Behaviors.Add(handlerFactoryBehavior);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo("Hello");

            // *** VALIDATE *** \\
            Assert.True(handlerCalled, "Error: expected client to call intercepting handler");
            Assert.True(result == testString, String.Format("Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void HttpMessageHandlerFactory_ModifyContent_Success()
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        string testString = "Hello";
        string substituteString = "World";
        Binding binding = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            var handlerFactoryBehavior = new HttpMessageHandlerBehavior();
            handlerFactoryBehavior.OnSendingAsync = (message, token) =>
            {
                var oldContent = message.Content;
                string requestMessageBody = oldContent.ReadAsStringAsync().Result;
                requestMessageBody = requestMessageBody.Replace(testString, substituteString);
                message.Content = new StringContent(requestMessageBody);
                foreach (var header in oldContent.Headers)
                {
                    if (!header.Key.Equals("Content-Length") && message.Content.Headers.Contains(header.Key))
                    {
                        message.Content.Headers.Remove(header.Key);
                    }

                    message.Content.Headers.Add(header.Key, header.Value);
                }

                return Task.FromResult((HttpResponseMessage)null);
            };
            factory.Endpoint.Behaviors.Add(handlerFactoryBehavior);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo("Hello");

            // *** VALIDATE *** \\
            Assert.True(result == substituteString, String.Format("Error: expected response from service: '{0}' Actual was: '{1}'", testString, result));

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void HttpExpect100Continue_AnonymousAuth_False()
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        BasicHttpBinding binding = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            Dictionary<string, string> requestHeaders = serviceProxy.GetRequestHttpHeaders();

            // *** VALIDATE *** \\
            string expectHeader = null;
            bool expectHeaderSent = requestHeaders.TryGetValue("Expect", out expectHeader);
            Assert.False(expectHeaderSent, "Expect header should not have been sent. Header value:" + expectHeader);

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void MultiValue_UserAgent_Success()
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        BasicHttpBinding binding = null;
        string userAgent = "Mozilla/4.0 (compatible; MSIE 6.0; .Net Core WCF Scenario Test Client 1.2.3.4)";
        string userAgentHeaderName = "User-Agent";

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            Dictionary<string, string> requestHeaders;
            using (new OperationContextScope((IContextChannel)serviceProxy))
            {
                HttpRequestMessageProperty httpReqMsgProp = new HttpRequestMessageProperty();
                httpReqMsgProp.Headers[userAgentHeaderName] = userAgent;
                OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, httpReqMsgProp);
                requestHeaders = serviceProxy.GetRequestHttpHeaders();
            }

            // *** VALIDATE *** \\
            bool userAgentHeaderSent = requestHeaders.TryGetValue(userAgentHeaderName, out string userAgentHeader);
            Assert.True(userAgentHeaderSent, "User-Agent header should have been sent.");
            Assert.Equal(userAgent, userAgentHeader);

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void Invalid_UserAgent_Failure()
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        BasicHttpBinding binding = null;
        string userAgent = "(";
        string userAgentHeaderName = "User-Agent";

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            Dictionary<string, string> requestHeaders;
            using (new OperationContextScope((IContextChannel)serviceProxy))
            {
                HttpRequestMessageProperty httpReqMsgProp = new HttpRequestMessageProperty();
                httpReqMsgProp.Headers[userAgentHeaderName] = userAgent;
                OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, httpReqMsgProp);
                Assert.Throws<FormatException>(() =>
                {
                    requestHeaders = serviceProxy.GetRequestHttpHeaders();
                });
            }

            // *** VALIDATE *** \\

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
