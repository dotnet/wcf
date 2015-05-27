// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

public static class Tcp_TransportChannel_ChannelLayer_DuplexChannelShapeTests
{
    private const string action = "http://tempuri.org/IWcfService/MessageRequestReply";
    private const string clientMessage = "[client] This is my request.";

    [Fact]
    [OuterLoop]
    public static void IChannelFactory_IRequestChannel_NetTcp_SecurityModeNone()
    {
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            // Create the channel factory
            IChannelFactory<IDuplexSessionChannel> factory =
            binding.BuildChannelFactory<IDuplexSessionChannel>(
                            new BindingParameterCollection());
            factory.Open();

            // Create the channel.
            IDuplexSessionChannel channel = factory.CreateChannel(
               new EndpointAddress(Endpoints.Tcp_NoSecurity_Address));
            channel.Open();

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));
            requestMessage.Headers.MessageId = new UniqueId(Guid.NewGuid());

            // Send the Message and receive the Response.
            channel.Send(requestMessage);
            Message replyMessage = channel.Receive(TimeSpan.FromSeconds(5));

            // If the incoming Message did not contain the same UniqueId used for the MessageId of the outgoing Message we would have received a Fault from the Service
            if (!String.Equals(replyMessage.Headers.RelatesTo.ToString(), requestMessage.Headers.MessageId.ToString()))
            {
                errorBuilder.AppendLine(String.Format("The MessageId of the incoming Message does not match the MessageId of the outgoing Message, expected: {0} but got: {1}", requestMessage.Headers.MessageId, replyMessage.Headers.RelatesTo));
            }

            // Validate the Response
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

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: CustomBindingTest FAILED with the following errors: {0}", errorBuilder));
    }
}
