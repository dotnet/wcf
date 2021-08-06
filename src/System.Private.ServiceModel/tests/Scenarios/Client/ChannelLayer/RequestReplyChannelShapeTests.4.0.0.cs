// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public partial class RequestReplyChannelShapeTests
{
    // Creating a ChannelFactory using a binding's 'BuildChannelFactory' method and providing a channel shape...
    //       returns a concrete type determined by the channel shape requested and other binding related settings.
    // The tests in this file use the IRequestChannel shape.

    private const string action = "http://tempuri.org/IWcfService/MessageRequestReply";
    private const string clientMessage = "[client] This is my request.";

    [WcfFact]
    [OuterLoop]
    public static void IRequestChannel_Http_BasicHttpBinding()
    {
        IChannelFactory<IRequestChannel> factory = null;
        IRequestChannel channel = null;
        Message replyMessage = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

            // Create the channel factory
            factory = binding.BuildChannelFactory<IRequestChannel>(new BindingParameterCollection());
            factory.Open();

            // Create the channel.
            channel = factory.CreateChannel(new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            channel.Open();

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));

            // *** EXECUTE *** \\
            // Send the Message and receive the Response.
            replyMessage = channel.Request(requestMessage);

            // *** VALIDATE *** \\
            // BasicHttpBinding uses SOAP1.1 which doesn't return the Headers.Action property in the Response
            // Therefore not validating this property as we do in the test "InvokeIRequestChannelCreatedViaBinding"
            var replyReader = replyMessage.GetReaderAtBodyContents();
            string actualResponse = replyReader.ReadElementContentAsString();
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";
            Assert.Equal(expectedResponse, actualResponse);

            // *** CLEANUP *** \\
            replyMessage.Close();
            channel.Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects(channel, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void IRequestChannel_Http_CustomBinding()
    {
        IChannelFactory<IRequestChannel> factory = null;
        IRequestChannel channel = null;
        Message replyMessage = null;

        try
        {
            // *** SETUP *** \\
            BindingElement[] bindingElements = new BindingElement[2];
            bindingElements[0] = new TextMessageEncodingBindingElement();
            bindingElements[1] = new HttpTransportBindingElement();
            CustomBinding binding = new CustomBinding(bindingElements);

            // Create the channel factory for the request-reply message exchange pattern.
            factory = binding.BuildChannelFactory<IRequestChannel>(new BindingParameterCollection());
            factory.Open();

            // Create the channel.
            channel = factory.CreateChannel(new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
            channel.Open();

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));

            // *** EXECUTE *** \\
            // Send the Message and receive the Response.
            replyMessage = channel.Request(requestMessage);

            // *** VALIDATE *** \\
            string replyMessageAction = replyMessage.Headers.Action;
            Assert.Equal(action + "Response", replyMessageAction);

            var replyReader = replyMessage.GetReaderAtBodyContents();
            string actualResponse = replyReader.ReadElementContentAsString();
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";
            Assert.Equal(expectedResponse, actualResponse);

            // *** CLEANUP *** \\
            replyMessage.Close();
            channel.Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects(channel, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void IRequestChannel_Async_Http_BasicHttpBinding()
    {
        IChannelFactory<IRequestChannel> factory = null;
        IRequestChannel channel = null;
        Message replyMessage = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

            // Create the channel factory
            factory = binding.BuildChannelFactory<IRequestChannel>(new BindingParameterCollection());
            Task.Factory.FromAsync(factory.BeginOpen, factory.EndOpen, TaskCreationOptions.None).GetAwaiter().GetResult(); 

            // Create the channel.
            channel = factory.CreateChannel(new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            Task.Factory.FromAsync(channel.BeginOpen, channel.EndOpen, TaskCreationOptions.None).GetAwaiter().GetResult(); 

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));

            // *** EXECUTE *** \\
            // Send the Message and receive the Response.
            replyMessage = Task.Factory.FromAsync((asyncCallback, o) => channel.BeginRequest(requestMessage, asyncCallback, o), 
                channel.EndRequest, 
                TaskCreationOptions.None).GetAwaiter().GetResult();

            // *** VALIDATE *** \\
            // BasicHttpBinding uses SOAP1.1 which doesn't return the Headers.Action property in the Response
            // Therefore not validating this property as we do in the test "InvokeIRequestChannelCreatedViaBinding"
            var replyReader = replyMessage.GetReaderAtBodyContents();
            string actualResponse = replyReader.ReadElementContentAsString();
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";
            Assert.Equal(expectedResponse, actualResponse);

            // *** CLEANUP *** \\
            replyMessage.Close();
            Task.Factory.FromAsync(channel.BeginClose, channel.EndClose, TaskCreationOptions.None).GetAwaiter().GetResult();
            Task.Factory.FromAsync(factory.BeginClose, factory.EndClose, TaskCreationOptions.None).GetAwaiter().GetResult();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects(channel, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void IRequestChannel_Async_Http_CustomBinding()
    {
        IChannelFactory<IRequestChannel> factory = null;
        IRequestChannel channel = null;
        Message replyMessage = null;

        try
        {
            // *** SETUP *** \\
            BindingElement[] bindingElements = new BindingElement[2];
            bindingElements[0] = new TextMessageEncodingBindingElement();
            bindingElements[1] = new HttpTransportBindingElement();
            CustomBinding binding = new CustomBinding(bindingElements);

            // Create the channel factory for the request-reply message exchange pattern.
            factory = binding.BuildChannelFactory<IRequestChannel>(new BindingParameterCollection());
            Task.Factory.FromAsync(factory.BeginOpen, factory.EndOpen, TaskCreationOptions.None).GetAwaiter().GetResult(); 

            // Create the channel.
            channel = factory.CreateChannel(new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
            Task.Factory.FromAsync(channel.BeginOpen, channel.EndOpen, TaskCreationOptions.None).GetAwaiter().GetResult(); 

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));

            // *** EXECUTE *** \\
            // Send the Message and receive the Response.
            replyMessage = Task.Factory.FromAsync((asyncCallback, o) => channel.BeginRequest(requestMessage, asyncCallback, o), 
                channel.EndRequest, 
                TaskCreationOptions.None).GetAwaiter().GetResult();

            // *** VALIDATE *** \\
            string replyMessageAction = replyMessage.Headers.Action;
            Assert.Equal(action + "Response", replyMessageAction);

            var replyReader = replyMessage.GetReaderAtBodyContents();
            string actualResponse = replyReader.ReadElementContentAsString();
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";
            Assert.Equal(expectedResponse, actualResponse);

            // *** CLEANUP *** \\
            replyMessage.Close();
            Task.Factory.FromAsync(channel.BeginClose, channel.EndClose, TaskCreationOptions.None).GetAwaiter().GetResult();
            Task.Factory.FromAsync(factory.BeginClose, factory.EndClose, TaskCreationOptions.None).GetAwaiter().GetResult();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects(channel, factory);
        }
    }
}
