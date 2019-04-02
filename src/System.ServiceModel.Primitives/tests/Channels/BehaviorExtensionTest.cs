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

        // Create a mock interactive channel initializer to observe whether
        // its methods are invoked.
        bool beginInitializeCalled = false;
        TestOperationBehaviorAttribute testOperationBehavior = new TestOperationBehaviorAttribute();
        testOperationBehavior.ApplyClientBehaviorOverride = (description, clientOperation) =>
        {
            beginInitializeCalled = true;
            testOperationBehavior.DefaultApplyClientBehavior(description, clientOperation);
        };

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

        // Explicitly close the channel factory synchronously.
        // One of the important aspects of this test is that a synchronous
        // close of the factory also synchronously closes the channel.
        factory.Close();

        // *** VALIDATE *** \\
        Assert.True(String.Equals(testMessageBody, result),
                    String.Format("Expected body to be '{0}' but actual was '{1}'", testMessageBody, result));

        Assert.False(beginInitializeCalled, "BeginDisplayInitializationUI should not have been called.");
    }

    [WcfFact]
    public static void BehaviorExtension_ContractBehaviorAttribute()
    {
        string testMessageBody = "BehaviorExtension_ContractBehaviorAttribute";
        Message inputMessage = Message.CreateMessage(MessageVersion.Default, action: "Test", body: testMessageBody);

        // *** SETUP *** \\

        // Create a mock interactive channel initializer to observe whether
        // its methods are invoked.
        bool beginInitializeCalled = false;
        TestContractBehaviorAttribute testContractBehavior = new TestContractBehaviorAttribute();
        testContractBehavior.ApplyClientBehaviorOverride = (description, endpoint, clientRuntime) =>
        {
            beginInitializeCalled = true;
            testContractBehavior.DefaultApplyClientBehavior(description, endpoint, clientRuntime);
        };

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

        // Explicitly close the channel factory synchronously.
        // One of the important aspects of this test is that a synchronous
        // close of the factory also synchronously closes the channel.
        factory.Close();

        // *** VALIDATE *** \\
        Assert.True(String.Equals(testMessageBody, result),
                    String.Format("Expected body to be '{0}' but actual was '{1}'", testMessageBody, result));

        Assert.False(beginInitializeCalled, "BeginDisplayInitializationUI should not have been called.");

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

    public class TestOperationBehaviorAttribute: Attribute, IOperationBehavior
    {
        public TestOperationBehaviorAttribute()
        {
            ApplyClientBehaviorOverride = DefaultApplyClientBehavior;
        }
        public Action<OperationDescription, ClientOperation> ApplyClientBehaviorOverride { get; set; }
        public void Validate(OperationDescription description) { }
  
        public void AddBindingParameters(OperationDescription description, BindingParameterCollection bindingParameters) { }
        public void ApplyDispatchBehavior(OperationDescription description, DispatchOperation dispatchOperationr) { }

        public void ApplyClientBehavior(OperationDescription description, ClientOperation clientOperation)
        {
            ApplyClientBehaviorOverride(description, clientOperation);
        }

        public void DefaultApplyClientBehavior(OperationDescription description, ClientOperation clientOperation) { }
    }

    public class TestContractBehaviorAttribute : Attribute, IContractBehavior
    {
        public TestContractBehaviorAttribute()
        {
            ApplyClientBehaviorOverride = DefaultApplyClientBehavior;
        }
        public Action<ContractDescription, ServiceEndpoint, ClientRuntime> ApplyClientBehaviorOverride { get; set; }
        public void Validate(ContractDescription description, ServiceEndpoint endpoint) { }

        public void AddBindingParameters(ContractDescription description, ServiceEndpoint endpoint, BindingParameterCollection bindingParameters) { }

        public void ApplyDispatchBehavior(ContractDescription description, ServiceEndpoint endpoint, DispatchRuntime dispatchRuntime) { }

        public void ApplyClientBehavior(ContractDescription description, ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            ApplyClientBehaviorOverride(description, endpoint, clientRuntime);
        }

        public void DefaultApplyClientBehavior(ContractDescription description, ServiceEndpoint endpoint, ClientRuntime clientRuntime) { }
    }
    #endregion Helpers
}
