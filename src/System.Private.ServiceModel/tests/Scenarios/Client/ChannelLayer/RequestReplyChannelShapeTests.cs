// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Xunit;

public static class Client_ChannelLayer_RequestReplyChannelShapeTests
{
    private const string action = "http://tempuri.org/IWcfService/MessageRequestReply";
    private const string clientMessage = "[client] This is my request.";

    [Fact]
    [OuterLoop]
    public static void IChannelFactoryOfIRequestChannel()
    {
        try
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

            // Create the channel factory
            IChannelFactory<IRequestChannel> factory =
            binding.BuildChannelFactory<IRequestChannel>(
                            new BindingParameterCollection());
            factory.Open();

            // Create the channel.
            IRequestChannel channel = factory.CreateChannel(
               new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            channel.Open();

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));

            // Send the Message and receive the Response.
            Message replyMessage = channel.Request(requestMessage);

            // BasicHttpBinding uses SOAP1.1 which doesn't return the Headers.Action property in the Response
            // Therefore not validating this property as we do in the test "InvokeIRequestChannelCreatedViaBinding"
            var replyReader = replyMessage.GetReaderAtBodyContents();
            string actualResponse = replyReader.ReadElementContentAsString();
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";
            if (!string.Equals(actualResponse, expectedResponse))
            {
                Assert.True(false, String.Format("Actual MessageBodyContent from service did not match the expected MessageBodyContent, expected: {0} actual: {1}", expectedResponse, actualResponse));
            }

            replyMessage.Close();
            channel.Close();
            factory.Close();
        }

        catch (Exception ex)
        {
            Assert.True(false, String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }
    }

    [Fact]
    [OuterLoop]
    public static void InvokeIRequestChannelCreatedViaBinding()
    {
        try
        {
            BindingElement[] bindingElements = new BindingElement[2];
            bindingElements[0] = new TextMessageEncodingBindingElement();
            bindingElements[1] = new HttpTransportBindingElement();
            CustomBinding binding = new CustomBinding(bindingElements);

            // Create the channel factory for the request-reply message exchange pattern.
            IChannelFactory<IRequestChannel> factory =
            binding.BuildChannelFactory<IRequestChannel>(
                             new BindingParameterCollection());
            factory.Open();

            // Create the channel.
            IRequestChannel channel = factory.CreateChannel(
               new EndpointAddress(BaseAddress.HttpBaseAddress));
            channel.Open();

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));

            // Send the Message and receive the Response.
            Message replyMessage = channel.Request(requestMessage);
            string replyMessageAction = replyMessage.Headers.Action;

            if (!string.Equals(replyMessageAction, action + "Response"))
            {
                Assert.True(false, String.Format("A response was received from the Service but it was not the expected Action, expected: {0} actual: {1}", action + "Response", replyMessageAction));
            }


            var replyReader = replyMessage.GetReaderAtBodyContents();
            string actualResponse = replyReader.ReadElementContentAsString();
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";
            if (!string.Equals(actualResponse, expectedResponse))
            {
                Assert.True(false, String.Format("Actual MessageBodyContent from service did not match the expected MessageBodyContent, expected: {0} actual: {1}", expectedResponse, actualResponse));
            }

            replyMessage.Close();
            channel.Close();
            factory.Close();
        }

        catch (Exception ex)
        {
            Assert.True(false, String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }
    }

    [Fact]
    [OuterLoop]
    public static void InvokeIRequestChannelViaProxy()
    {
        string address = BaseAddress.HttpBaseAddress;

        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding binding = new CustomBinding(new BindingElement[] {
                new TextMessageEncodingBindingElement(MessageVersion.Default, Encoding.UTF8),
                new HttpTransportBindingElement() });

            EndpointAddress endpointAddress = new EndpointAddress(address);

            // Create the channel factory for the request-reply message exchange pattern.
            var factory = new ChannelFactory<IRequestChannel>(binding, endpointAddress);

            // Create the channel.
            IRequestChannel channel = factory.CreateChannel();
            channel.Open();

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));

            // Send the Message and receive the Response.
            Message replyMessage = channel.Request(requestMessage);

            string replyMessageAction = replyMessage.Headers.Action;

            if (!string.Equals(replyMessageAction, action + "Response"))
            {
                errorBuilder.AppendLine(String.Format("A response was received from the Service but it was not the expected Action, expected: {0} actual: {1}", action + "Response", replyMessageAction));
            }

            var replyReader = replyMessage.GetReaderAtBodyContents();
            string actualResponse = replyReader.ReadElementContentAsString();
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";
            if (!string.Equals(actualResponse, expectedResponse))
            {
                errorBuilder.AppendLine(String.Format("Actual MessageBodyContent from service did not match the expected MessageBodyContent, expected: {0} actual: {1}", expectedResponse, actualResponse));
            }

            replyMessage.Close();
            channel.Close();
            factory.Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: InvokeRequestChannelViaProxy FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void InvokeIRequestChannelViaProxyTimeout()
    {
        string address = BaseAddress.HttpBaseAddress;
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding binding = new CustomBinding(new BindingElement[] {
                new TextMessageEncodingBindingElement(MessageVersion.Default, Encoding.UTF8),
                new HttpTransportBindingElement() });

            EndpointAddress endpointAddress = new EndpointAddress(address);

            // Create the channel factory for the request-reply message exchange pattern.
            var factory = new ChannelFactory<IRequestChannel>(binding, endpointAddress);

            // Create the channel.
            IRequestChannel channel = factory.CreateChannel();
            channel.Open();

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));

            // Send the Message and receive the Response.
            Message replyMessage = channel.Request(requestMessage, TimeSpan.FromSeconds(60));

            string replyMessageAction = replyMessage.Headers.Action;

            if (!string.Equals(replyMessageAction, action + "Response"))
            {
                errorBuilder.AppendLine(String.Format("A response was received from the Service but it was not the expected Action, expected: {0} actual: {1}", action + "Response", replyMessageAction));
            }


            var replyReader = replyMessage.GetReaderAtBodyContents();
            string actualResponse = replyReader.ReadElementContentAsString();
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";
            if (!string.Equals(actualResponse, expectedResponse))
            {
                errorBuilder.AppendLine(String.Format("Actual MessageBodyContent from service did not match the expected MessageBodyContent, expected: {0} actual: {1}", expectedResponse, actualResponse));
            }

            replyMessage.Close();
            channel.Close();
            factory.Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: InvokeIRequestChannelViaProxyTimeout FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void InvokeIRequestChannelViaProxyAsync()
    {
        string address = BaseAddress.HttpBaseAddress;
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding binding = new CustomBinding(new BindingElement[] {
                new TextMessageEncodingBindingElement(MessageVersion.Default, Encoding.UTF8),
                new HttpTransportBindingElement() });

            EndpointAddress endpointAddress = new EndpointAddress(address);

            // Create the channel factory for the request-reply message exchange pattern.
            var factory = new ChannelFactory<IRequestChannel>(binding, endpointAddress);

            // Create the channel.
            IRequestChannel channel = factory.CreateChannel();
            channel.Open();

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));

            // Send the Message and receive the Response.
            IAsyncResult ar = channel.BeginRequest(requestMessage, null, null);
            Message replyMessage = channel.EndRequest(ar);

            string replyMessageAction = replyMessage.Headers.Action;

            if (!string.Equals(replyMessageAction, action + "Response"))
            {
                errorBuilder.AppendLine(String.Format("A response was received from the Service but it was not the expected Action, expected: {0} actual: {1}", action + "Response", replyMessageAction));
            }

            var replyReader = replyMessage.GetReaderAtBodyContents();
            string actualResponse = replyReader.ReadElementContentAsString();
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";
            if (!string.Equals(actualResponse, expectedResponse))
            {
                errorBuilder.AppendLine(String.Format("Actual MessageBodyContent from service did not match the expected MessageBodyContent, expected: {0} actual: {1}", expectedResponse, actualResponse));
            }

            replyMessage.Close();
            channel.Close();
            factory.Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: InvokeIRequestChannelViaProxyAsync FAILED with the following errors: {0}", errorBuilder));
    }
}
