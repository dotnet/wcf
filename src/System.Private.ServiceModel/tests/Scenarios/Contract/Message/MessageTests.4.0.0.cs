// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using Infrastructure.Common;
using Xunit;

public static class MessageTests
{
    private const string action = "http://tempuri.org/IWcfService/MessageRequestReply";
    private const string clientMessage = "Test Custom_Message_RoundTrips.";

    [WcfFact]
    [OuterLoop]
    public static void Custom_Message_RoundTrips()
    {
        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);

        // Create the channel factory
        IChannelFactory<IRequestChannel> factory = binding.BuildChannelFactory<IRequestChannel>(new BindingParameterCollection());
        factory.Open();

        // Create the channel.
        IRequestChannel channel = factory.CreateChannel(new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
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

    [WcfFact]
    [OuterLoop]
    public static void Echo_With_CustomClientMessageFormatter_RoundTrips_String()
    {
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        string testString = "Hello";
        Binding binding = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));

            string suffix = "_suffix";
            var customBehavior = new AppendSuffixToStringBehavior(suffix);

            // Apply the custom behavior to each operation of the contract.
            foreach (OperationDescription operation in factory.Endpoint.Contract.Operations)
            {
                operation.OperationBehaviors.Add(customBehavior);
            }

            // AppendSuffixToStringBehavior appends the suffix to each string parameter
            // when creating request message, so we expect the server would echo testString + suffix
            string expectedResult = testString + suffix;

            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.Echo(testString);

            // *** VALIDATE *** \\
            Assert.True(result == expectedResult, string.Format("Error: expected response from service: '{0}' Actual was: '{1}'", expectedResult, result));

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

    // AppendSuffixToStringBehavior appends the suffix to each string parameter
    // when creating request message
    public class AppendSuffixToStringBehavior : IOperationBehavior
    {
        private string _suffix;

        public AppendSuffixToStringBehavior(string suffix)
        {
            _suffix = suffix;
        }

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            clientOperation.Formatter = new AppendSuffixToStringFormatter(clientOperation.Formatter, _suffix);
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
        }

        public void Validate(OperationDescription operationDescription)
        {
        }
    }

    // AppendSuffixToStringFormatter appends the suffix to each string parameter
    // when creating request message
    public class AppendSuffixToStringFormatter : IClientMessageFormatter
    {
        private IClientMessageFormatter _inner;
        private string _suffix;

        public AppendSuffixToStringFormatter(IClientMessageFormatter inner, string suffix)
        {
            _inner = inner;
            _suffix = suffix;
        }

        public object DeserializeReply(Message message, object[] parameters)
        {
            return _inner.DeserializeReply(message, parameters);
        }

        // The method appends a suffix to each string parameter and serializes
        // the parameters using the passed in inner formatter.
        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            object[] newParams = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] is string)
                {
                    newParams[i] = parameters[i] + _suffix;
                }
                else
                {
                    newParams[i] = parameters[i];
                }
            }

            return _inner.SerializeRequest(messageVersion, newParams);
        }
    }
}
