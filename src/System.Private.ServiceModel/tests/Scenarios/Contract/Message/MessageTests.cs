// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;
using System.ServiceModel.Channels;
using Xunit;

public static class MessageTests
{
    private const string action = "http://tempuri.org/IWcfService/MessageRequestReply";
    private const string clientMessage = "Test Custom_Message_RoundTrips.";

    [Fact]
    [OuterLoop]
    public static void Custom_Message_RoundTrips()
    {
        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

        // Create the channel factory
        IChannelFactory<IRequestChannel> factory = binding.BuildChannelFactory<IRequestChannel>(new BindingParameterCollection());
        factory.Open();

        // Create the channel.
        IRequestChannel channel = factory.CreateChannel(new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
        channel.Open();

        // Create the Message object to send to the service.
        using (Message requestMessage = Message.CreateMessage(
            binding.MessageVersion,
            action,
            new CustomBodyWriter(clientMessage)))
        {
            // Send the Message and receive the Response.
            using (Message replyMessage = channel.Request(requestMessage))
            {
                Assert.False(replyMessage.IsFault);
                Assert.False(replyMessage.IsEmpty);
                Assert.Equal(MessageState.Created, replyMessage.State);
                Assert.Equal(MessageVersion.Soap11, replyMessage.Version);

                var replyReader = replyMessage.GetReaderAtBodyContents();
                string actualResponse = replyReader.ReadElementContentAsString();
                string expectedResponse = "Test Custom_Message_RoundTrips.[service] Request received, this is my Reply.";

                Assert.True(string.Equals(actualResponse, expectedResponse),
                    string.Format("Actual MessageBodyContent from service did not match the expected MessageBodyContent, expected: {0} actual: {1}", expectedResponse, actualResponse));
            }
        }

        channel.Close();
        factory.Close();
    }
}
