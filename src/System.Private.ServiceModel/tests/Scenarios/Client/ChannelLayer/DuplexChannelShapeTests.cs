// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

public static class DuplexChannelShapeTests
{
    // Creating a ChannelFactory using a binding's 'BuildChannelFactory' method and providing a channel shape...
    //       returns a concrete type determined by the channel shape requested and other binding related settings.
    // The tests in this file use the IDuplexChannel shape.

    private const string action = "http://tempuri.org/IWcfService/MessageRequestReply";
    private const string clientMessage = "[client] This is my request.";

    [Fact]
    [OuterLoop]
    public static void IDuplexSessionChannel_Tcp_NetTcpBinding()
    {
        IChannelFactory<IDuplexSessionChannel> factory = null;
        IDuplexSessionChannel channel = null;
        Message replyMessage = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            // Create the channel factory
            factory = binding.BuildChannelFactory<IDuplexSessionChannel>(new BindingParameterCollection());
            factory.Open();

            // Create the channel.
            channel = factory.CreateChannel(new EndpointAddress(Endpoints.Tcp_NoSecurity_Address));
            channel.Open();

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));
            requestMessage.Headers.MessageId = new UniqueId(Guid.NewGuid());

            // *** EXECUTE *** \\
            // Send the Message and receive the Response.
            channel.Send(requestMessage);
            replyMessage = channel.Receive(TimeSpan.FromSeconds(5));

            // *** VALIDATE *** \\
            // If the incoming Message did not contain the same UniqueId used for the MessageId of the outgoing Message we would have received a Fault from the Service
            Assert.Equal(requestMessage.Headers.MessageId.ToString(), replyMessage.Headers.RelatesTo.ToString());

            // Validate the Response
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
}
