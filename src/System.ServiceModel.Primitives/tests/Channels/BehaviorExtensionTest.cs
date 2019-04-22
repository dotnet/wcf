// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using Infrastructure.Common;
using Xunit;
using System.Threading.Tasks;

public static class BehaviorExtensionTest
{
    [WcfFact]
    public static void BehaviorExtension_OperationBehaviorAttribute()
    {
        string testMessageBody = "BehaviorExtension_OperationBehaviorAttribute";
        Message inputMessage = Message.CreateMessage(MessageVersion.Default, action: "Test", body: testMessageBody);

        // *** SETUP *** \\
        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<IBehaviorExtensionServerInterface>(binding, address);

        // We rely on the implicit open of the channel to be synchronous.
        // This is true for both the full framework and this NET Core version.
        IBehaviorExtensionServerInterface channel = factory.CreateChannel();

        // *** EXECUTE *** \\
        Message outputMessage = channel.Process(inputMessage);

        // The mock's default behavior is just to loopback what we sent.
        var result = outputMessage.GetBody<string>();

        ((IClientChannel)channel).Close();
        factory.Close();

        // *** VALIDATE *** \\
        Assert.True(String.Equals(testMessageBody, result),
                    String.Format("Expected body to be '{0}' but actual was '{1}'", testMessageBody, result));

        Assert.True(TestOperationBehaviorAttribute.operationBehaviorSet, "TestOperationBehavior attribute on IBehaviorExtensionServerInterface.Process operation should have triggered ApplyClientBehavior() call");
    }

    [WcfFact]
    public static void BehaviorExtension_ContractBehaviorAttribute()
    {
        string testMessageBody = "BehaviorExtension_ContractBehaviorAttribute";
        Message inputMessage = Message.CreateMessage(MessageVersion.Default, action: "Test", body: testMessageBody);

        // *** SETUP *** \\

        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<IBehaviorExtensionServerInterface2>(binding, address);

        // We rely on the implicit open of the channel to be synchronous.
        // This is true for both the full framework and this NET Core version.
        IBehaviorExtensionServerInterface2 channel = factory.CreateChannel();

        // *** EXECUTE *** \\
        Message outputMessage = channel.Process(inputMessage);

        // The mock's default behavior is just to loopback what we sent.
        var result = outputMessage.GetBody<string>();

        ((IClientChannel)channel).Close();
        factory.Close();


        // *** VALIDATE *** \\
        Assert.True(String.Equals(testMessageBody, result),
                    String.Format("Expected body to be '{0}' but actual was '{1}'", testMessageBody, result));

        Assert.True(TestContractBehaviorAttribute.contractBehaviorSet, "TestContractBehavior attribute on IBehaviorExtensionServerInterface2 constract should have triggered ApplyClientBehavior() call");

    }


    #region Helpers

    [ServiceContract]
    public interface IBehaviorExtensionServerInterface
    {
        [OperationContract(Action = "*", ReplyAction = "*")]
        [TestOperationBehavior]
        Message Process(Message input);
    }

    [ServiceContract]
    [TestContractBehavior]
    public interface IBehaviorExtensionServerInterface2
    {
        [OperationContract(Action = "*", ReplyAction = "*")]
        Message Process(Message input);
    }

    public class BehaviorExtensionServer : IBehaviorExtensionServerInterface
    {
        public Message Process(Message input) { return input; }

    }

    public class BehaviorExtensionServer2 : IBehaviorExtensionServerInterface2
    {
        public Message Process(Message input) { return input; }

    }

    public class TestOperationBehaviorAttribute : Attribute, IOperationBehavior
    {
        public static bool operationBehaviorSet = false;
        public void Validate(OperationDescription description) { }
        public void AddBindingParameters(OperationDescription description, BindingParameterCollection bindingParameters) { }
        public void ApplyDispatchBehavior(OperationDescription description, DispatchOperation dispatchOperationr) { }
        public void ApplyClientBehavior(OperationDescription description, ClientOperation clientOperation)
        {
            operationBehaviorSet = true;
        }
    }

    public class TestContractBehaviorAttribute : Attribute, IContractBehavior
    {
        public static bool contractBehaviorSet = false; 
        public void Validate(ContractDescription description, ServiceEndpoint endpoint) { }
        public void AddBindingParameters(ContractDescription description, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }
        public void ApplyDispatchBehavior(ContractDescription description, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime) { }
        public void ApplyClientBehavior(ContractDescription description, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            contractBehaviorSet = true;
        }
    }
    #endregion Helpers
}
