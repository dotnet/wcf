// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Xunit;

public static class RequestReplyChannelShapeTests
{
    // Creating a ChannelFactory using a binding's 'BuildChannelFactory' method and providing a channel shape...
    //       returns a concrete type determined by the channel shape requested and other binding related settings.
    // The tests in this file use the IRequestChannel shape.

    private const string action = "http://tempuri.org/IWcfService/MessageRequestReply";
    private const string clientMessage = "[client] This is my request.";

    [Fact]
    [OuterLoop]
    public static void IRequestChannel_Http_BasicHttpBinding()
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
    public static void IRequestChannel_Http_CustomBinding()
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
               new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
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
}
