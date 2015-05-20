﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;

public static class TestHelpers
{
    private const string testString = "Hello";
    public static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(20);

    public static bool XmlDictionaryReaderQuotasAreEqual(XmlDictionaryReaderQuotas a, XmlDictionaryReaderQuotas b)
    {
        return a.MaxArrayLength == b.MaxArrayLength
            && a.MaxBytesPerRead == b.MaxBytesPerRead
            && a.MaxDepth == b.MaxDepth
            && a.MaxNameTableCharCount == b.MaxNameTableCharCount
            && a.MaxStringContentLength == b.MaxStringContentLength;
    }

    public static string GenerateStringValue(int length)
    {
        // There's no great reason why we use this set of characters - we just want to be able to generate a longish string
        uint firstCharacter = 0x41; // A

        StringBuilder builder = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            builder.Append((char)(firstCharacter + i % 25));
        }

        return builder.ToString();
    }
}

//Helper class used in this test to allow construction of ContractDescription
public class MyClientBase<T> : ClientBase<T> where T : class
{
    public MyClientBase(Binding binding, EndpointAddress endpointAddress)
        : base(binding, endpointAddress)
    {
    }
}

// This helper class is used for ClientBase<T> tests
public class MyClientBase : ClientBase<IWtfServiceGenerated>
{
    public MyClientBase(Binding binding, EndpointAddress endpointAddress)
        : base(binding, endpointAddress)
    {
    }
}

// This helper class is used by the ContractDescription tests to validate contracts.
public class ContractDescriptionTestHelper
{
    // Real service endpoint not required for this test because we never open the channel
    private const string address = "http://localhost/fakeservice.svc";

    // Helper method to validate that service contract T is correct.
    // This helper uses ClientBase<T> to construct the ContractDescription.
    // The 'expectedOperations' describes the operations we expect to find in that contract.
    public static string ValidateContractDescription<T>(ContractDescriptionData expectedContract) where T : class
    {
        OperationDescriptionData[] expectedOperations = expectedContract.Operations;

        StringBuilder errorBuilder = new StringBuilder();
        try
        {
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            MyClientBase<T> client = new MyClientBase<T>(customBinding, new EndpointAddress(address));
            ContractDescription contract = client.Endpoint.Contract;

            string results = ValidateContractDescription(contract, typeof(T), expectedContract);
            if (results != null)
            {
                errorBuilder.AppendLine(results);
            }
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        return errorBuilder.Length == 0 ? null : errorBuilder.ToString();
    }

    // Helper method to validate the given ContractDescription has the expected type and operations.
    public static string ValidateContractDescription(ContractDescription contract, Type expectedType, ContractDescriptionData expectedContract)
    {
        OperationDescriptionData[] expectedOperations = expectedContract.Operations;

#if DEBUGGING
        DumpContractDescription(contract);
#endif
        StringBuilder errorBuilder = new StringBuilder();
        string prefix = String.Format("    Contract type: {0}", expectedType);
        try
        {
            // ContractType must match expected type
            Type contractType = contract.ContractType;
            if (!expectedType.Equals(contractType))
            {
                errorBuilder.AppendLine(String.Format("{0} expected Type = {0}, actual = {1}", prefix, expectedType, contractType));
            }

            // Must have exactly the expected number of OperationDescriptions
            OperationDescriptionCollection ops = contract.Operations;
            if (ops.Count != expectedOperations.Length)
            {
                errorBuilder.AppendLine(String.Format("{0} operations.Count: expected={1}, actual = {2}", prefix, expectedOperations.Length, ops.Count));
            }

            foreach (OperationDescriptionData expectedOp in expectedOperations)
            {
                // Must have each operation by name
                OperationDescription op = ops.Find(expectedOp.Name);
                if (op == null)
                {
                    errorBuilder.AppendLine(String.Format("{0} operations: could not find operation {1}", prefix, expectedOp.Name));
                }
                else
                {
                    // Has expected operation name
                    if (!op.Name.Equals(expectedOp.Name))
                    {
                        errorBuilder.AppendLine(String.Format("{0} expected operation Name = {1}, actual = {2}",
                                                              prefix, expectedOp.Name, op.Name));
                    }

                    // Has expected one-way setting
                    if (op.IsOneWay != expectedOp.IsOneWay)
                    {
                        errorBuilder.AppendLine(String.Format("{0} expected operation {1}.IsOneWay = {2}, actual = {3}",
                                                              prefix, expectedOp.Name, expectedOp.IsOneWay, op.IsOneWay));
                    }

                    // If contains XxxAsync operation, op.TaskMethod will be non-null.  Verify it as expected.
                    bool hasTask = op.TaskMethod != null;
                    if (hasTask != expectedOp.HasTask)
                    {
                        errorBuilder.AppendLine(String.Format("{0} expected operation {1}.HasTask = {2}, actual = {3}",
                                                              prefix, expectedOp.Name, expectedOp.HasTask, hasTask));
                    }

                    // Validate each MessageDescription for each OperationDescription
                    MessageDescriptionCollection messages = op.Messages;
                    foreach (MessageDescriptionData messageData in expectedOp.Messages)
                    {
                        // Must find each expected MessageDescription by Action name
                        MessageDescription messageDesc = messages.FirstOrDefault(m => m.Action.Equals(messageData.Action));
                        if (messageDesc == null)
                        {
                            errorBuilder.AppendLine(String.Format("{0} could not find expected message action {1} in operation {2}",
                                                                  prefix, messageData.Action, op.Name));
                        }
                        else
                        {
                            // Must have expected direction
                            if (messageDesc.Direction != messageData.Direction)
                            {
                                errorBuilder.AppendLine(String.Format("{0} message action {1} expected Direction = {2}, actual = {3}",
                                                                      prefix, messageData.Action, messageData.Direction, messageDesc.Direction));
                            }

                            // MessageType is non-null for operations containing MessageContract types.
                            // Verify we were able to build a "typed message" from the MessageContract.
                            if (messageData.MessageType != null && !messageData.MessageType.Equals(messageDesc.MessageType))
                            {
                                errorBuilder.AppendLine(String.Format("{0} message action {1} expected MessageType = {2}, actual = {3}",
                                                                      prefix, messageData.Action, messageData.MessageType, messageDesc.MessageType));
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("{0} unexpected exception was caught: {1}",
                                                  prefix, ex.ToString()));
        }

        return errorBuilder.Length == 0 ? null : errorBuilder.ToString();
    }

#if DEBUGGING
    private static void DumpContractDescription(ContractDescription contract)
    {
        Console.WriteLine(String.Format("Contract type: {0}", contract.ContractType));
        foreach (OperationDescription op in contract.Operations)
        {
            Console.WriteLine(String.Format("  operation: {0}", op.Name));
            foreach (MessageDescription md in op.Messages)
            {
                Console.WriteLine("     message: Action = {0}, Direction = {1}, MessageType = {2}", md.Action, md.Direction, md.MessageType);
            }
        }
    }
#endif
}

// This helper class contains the description of what a ContractDescription should contain.
// It is used as the "expected" part of the test to compare against the "actual" ContractDescription.
public class ContractDescriptionData
{
    public OperationDescriptionData[] Operations { get; set; }
}

// This helper class contains the data we expect to find in an OperationDescription.
// It is used as the expected value in test comparisons.
public class OperationDescriptionData
{
    // Name of operation we expect
    public string Name { get; set; }

    // True if message is one-way
    public bool IsOneWay { get; set; }

    // True if we expect this to have a Task-based operation too
    public bool HasTask { get; set; }

    // The list of MessageDescriptionData we expect to compare against OperationDescription.Messages
    public MessageDescriptionData[] Messages { get; set; }
}

// This helper class contains the data we expect to find in a MessageDescription.
public class MessageDescriptionData
{
    // The name of the Action for the message
    public string Action { get; set; }

    // The direction of the message
    public MessageDirection Direction { get; set; }

    // If using MessageContract, the type of the "typed message"
    public Type MessageType { get; set; }
}

// This class is used to wire up the ClientMessageInspector on the endpoint
public class ClientMessageInspectorBehavior : IEndpointBehavior
{
    private ClientMessageInspector _inspector;

    public ClientMessageInspectorBehavior(ClientMessageInspectorData data)
    {
        _inspector = new ClientMessageInspector(data);
    }

    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        clientRuntime.ClientMessageInspectors.Add(_inspector);
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {
    }

    public void Validate(ServiceEndpoint endpoint)
    {
    }
}

// This class is ClientMessageInspector that captures its state for testing
public class ClientMessageInspector : IClientMessageInspector
{
    private ClientMessageInspectorData _data;
    public ClientMessageInspector(ClientMessageInspectorData data)
    {
        _data = data;
    }

    public void AfterReceiveReply(ref Message reply, object correlationState)
    {
        _data.AfterReceiveReplyCalled = true;
        _data.Reply = reply;
    }

    public object BeforeSendRequest(ref Message request, IClientChannel channel)
    {
        _data.BeforeSendRequestCalled = true;
        _data.Request = request;
        _data.Channel = channel;
        return null;
    }
}

// This class is used to hold test data set by ClientMessageInspector
public class ClientMessageInspectorData
{
    public bool BeforeSendRequestCalled { get; set; }
    public bool AfterReceiveReplyCalled { get; set; }
    public Message Request { get; set; }
    public Message Reply { get; set; }
    public IClientChannel Channel { get; set; }
}

public class CustomBodyWriter : BodyWriter
{
    private string _bodyContent;

    public CustomBodyWriter()
        : base(true)
    { }

    public CustomBodyWriter(string message)
        : base(true)
    {
        _bodyContent = message;
    }

    protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
    {
        writer.WriteString(_bodyContent);
    }
}
