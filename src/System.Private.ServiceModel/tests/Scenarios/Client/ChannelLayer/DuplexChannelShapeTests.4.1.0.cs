// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public partial class DuplexChannelShapeTests : ConditionalWcfTest
{
    // Creating a ChannelFactory using a binding's 'BuildChannelFactory' method and providing a channel shape...
    //       returns a concrete type determined by the channel shape requested and other binding related settings.
    // The tests in this file use the IDuplexChannel shape.
#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(1157)]    
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed))]
    [OuterLoop]
    [Issue(1157)]
    public static void IDuplexSessionChannel_Https_NetHttpsBinding()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        if (!root_Certificate_Installed)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            return;
        }
#endif
        IChannelFactory<IDuplexSessionChannel> factory = null;
        IDuplexSessionChannel channel = null;
        Message replyMessage = null;

        try
        {
            // *** SETUP *** \\
            NetHttpsBinding binding = new NetHttpsBinding(BasicHttpsSecurityMode.Transport);

            // Create the channel factory
            factory = binding.BuildChannelFactory<IDuplexSessionChannel>(new BindingParameterCollection());
            factory.Open();

            // Create the channel.
            channel = factory.CreateChannel(new EndpointAddress(Endpoints.HttpBaseAddress_NetHttps));
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
            string expectedMessageID = requestMessage.Headers.MessageId.ToString();
            string actualMessageID = replyMessage.Headers.RelatesTo.ToString();
            Assert.True(String.Equals(expectedMessageID, actualMessageID), String.Format("Expected Message ID was {0}. Actual was {1}", expectedMessageID, actualMessageID));

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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(1157)]
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed))]
    [OuterLoop]
    [Issue(1157)]
    public static void IDuplexSessionChannel_Http_BasicHttpBinding()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        if (!root_Certificate_Installed)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            return;
        }
#endif
        IChannelFactory<IDuplexSessionChannel> factory = null;
        IDuplexSessionChannel channel = null;
        Message replyMessage = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpBinding binding = new BasicHttpBinding();

            // Create the channel factory
            factory = binding.BuildChannelFactory<IDuplexSessionChannel>(new BindingParameterCollection());
            factory.Open();

            // Create the channel.
            channel = factory.CreateChannel(new EndpointAddress(Endpoints.HttpBaseAddress_NetHttps));
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
