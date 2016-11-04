// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

public static class TestHelpers
{
    private const string testString = "Hello";
    public static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(20);
    public static ManualResetEvent mre;

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
public class MyClientBase : ClientBase<IWcfServiceGenerated>
{
    public MyClientBase(Binding binding, EndpointAddress endpointAddress)
        : base(binding, endpointAddress)
    {
    }
}

// This helper class is used for DuplexClientBase<T> tests
public class MyDuplexClientBase<T> : DuplexClientBase<T> where T : class
{
    public MyDuplexClientBase(InstanceContext callbackInstance, Binding binding, EndpointAddress endpointAddress)
        : base(callbackInstance, binding, endpointAddress)
    {
    }
}

// This helper class is used by the ContractDescription tests to validate contracts.
public class ContractDescriptionTestHelper
{
    // Helper method to validate that service contract T is correct.
    // This helper uses ClientBase<T> to construct the ContractDescription.
    // The 'expectedOperations' describes the operations we expect to find in that contract.
    public static string ValidateContractDescription<T>(ContractDescriptionData expectedContract) where T : class
    {
        OperationDescriptionData[] expectedOperations = expectedContract.Operations;

        StringBuilder errorBuilder = new StringBuilder();
        try
        {
            // Arrange
            CustomBinding binding = new CustomBinding();
            binding.Elements.Add(new TextMessageEncodingBindingElement());
            binding.Elements.Add(new HttpTransportBindingElement());
            EndpointAddress address = new EndpointAddress(FakeAddress.HttpAddress);

            // Act
            ChannelFactory<T> factory = new ChannelFactory<T>(binding, address);
            ContractDescription contract = factory.Endpoint.Contract;

            // Assert
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

                        // Validate the Body parts of the MessageDescription
                        ValidatePartDescriptionCollection(messageData.Action, "Body", errorBuilder, messageDesc.Body.Parts.ToArray(), messageData.Body);

                        // Validate the Header parts of the MessageDescription
                        ValidatePartDescriptionCollection(messageData.Action, "Header", errorBuilder, messageDesc.Headers.ToArray(), messageData.Headers);

                        // Validate the Property parts of the MessageDescription
                        ValidatePartDescriptionCollection(messageData.Action, "Properties", errorBuilder, messageDesc.Properties.ToArray(), messageData.Properties);
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

    private static void ValidatePartDescriptionCollection(string action, string section, StringBuilder errorBuilder, MessagePartDescription[] desc, PartDescriptionData[] data)
    {
        if (data != null)
        {
            if (desc.Length != data.Length)
            {
                errorBuilder.AppendLine(String.Format("action {0}, section {1}, expected part count = {2}, actual = {3}",
                                        action, section, data.Length, desc.Length));
            }

            // MessagePartDescriptions are keyed collections, so their order is unpredictable
            foreach (PartDescriptionData dataPart in data)
            {
                MessagePartDescription descPart = desc.SingleOrDefault((d) => String.Equals(dataPart.Name, d.Name));
                ValidatePartDescription(action, section, errorBuilder, descPart, dataPart);
            }
        }
    }

    private static void ValidatePartDescription(string action, string section, StringBuilder errorBuilder, MessagePartDescription desc, PartDescriptionData data)
    {
        if (desc == null)
        {
            errorBuilder.AppendLine(String.Format("action {0}, section {1}, expected part Name = {2} but did not find it.",
                                                  action, section, data.Name));
            return;
        }

        if (!String.Equals(desc.Name, data.Name))
        {
            errorBuilder.AppendLine(String.Format("action {0}, section {1}, expected part Name = {2}, actual = {3}",
                                                    action, section, data.Name, desc.Name));
        }

        if (desc.Type != data.Type)
        {
            errorBuilder.AppendLine(String.Format("action {0}, section {1}, name {2}, expected Type = {3}, actual = {4}",
                                                  action, section, desc.Name, data.Type, desc.Type));
        }

        if (desc.Multiple != data.Multiple)
        {
            errorBuilder.AppendLine(String.Format("action {0}, section {1}, name {2}, expected Multiple = {3}, actual = {4}",
                                                  action, section, desc.Name, data.Multiple, desc.Multiple));
        }
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

    public PartDescriptionData[] Body { get; set; }

    public PartDescriptionData[] Headers { get; set; }

    public PartDescriptionData[] Properties { get; set; }
}

public class PartDescriptionData
{
    public string Name { get; set; }
    public Type Type { get; set; }
    public bool Multiple { get; set; }
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

public class WcfDuplexServiceCallback : IWcfDuplexServiceCallback
{
    public Task<Guid> OnPingCallback(Guid guid)
    {
        return Task.FromResult<Guid>(guid);
    }

    void IWcfDuplexServiceCallback.OnPingCallback(Guid guid)
    {
    }
}

public class MyOperationBehavior : Attribute, IOperationBehavior
{
    public static StringBuilder errorBuilder = new StringBuilder();
    public static TaskCompletionSource<bool> validateMethodTcs = new TaskCompletionSource<bool>();
    public static TaskCompletionSource<bool> addBindingParametersMethodTcs = new TaskCompletionSource<bool>();
    public static TaskCompletionSource<bool> applyClientBehaviorMethodTcs = new TaskCompletionSource<bool>();
    public string errorMessage = "method was called out-of-order, the correct order is: Validate, AddBindingParameters, ApplyClientBehavior.";

    // Pass data at runtime to bindings to support custom behavior
    public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
    {
        if ((!validateMethodTcs.Task.IsCompleted) || (applyClientBehaviorMethodTcs.Task.IsCompleted))
        {
            errorBuilder.AppendLine(String.Format("AddBindingParameters {1}", errorMessage));
        }
        if (operationDescription == null)
        {
            errorBuilder.AppendLine(String.Format("A parameter passed into the AddBindingParameters method was null/nThe null parameter is: {0}", typeof(OperationDescription).ToString()));
        }
        if (bindingParameters == null)
        {
            errorBuilder.AppendLine(String.Format("A parameter passed into the AddBindingParameters method was null/nThe null parameter is: {0}", typeof(BindingParameterCollection).ToString()));
        }

        addBindingParametersMethodTcs.TrySetResult(true);
    }

    // Implements a modification or extension of the client across an operation
    public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
    {
        if ((!validateMethodTcs.Task.IsCompleted) || (!addBindingParametersMethodTcs.Task.IsCompleted))
        {
            errorBuilder.AppendLine(String.Format("ApplyClientBehavior {1}", errorMessage));
        }
        if (operationDescription == null)
        {
            errorBuilder.AppendLine(String.Format("A parameter passed into the ApplyClientBehavior method was null/nThe null parameter is: {0}", typeof(OperationDescription).ToString()));
        }
        if (clientOperation == null)
        {
            errorBuilder.AppendLine(String.Format("A parameter passed into the ApplyClientBehavior method was null/nThe null parameter is: {0}", typeof(ClientOperation).ToString()));
        }

        applyClientBehaviorMethodTcs.TrySetResult(true);
    }

    // Implements a modification or extension of the service across an operation
    public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
    {
        // This method does not get called client side.
    }

    // Implement to confirm that the operation meets some intended criteria
    public void Validate(OperationDescription operationDescription)
    {
        if (addBindingParametersMethodTcs.Task.IsCompleted || applyClientBehaviorMethodTcs.Task.IsCompleted)
        {
            errorBuilder.AppendLine(String.Format("Validate {1}", errorMessage));
        }

        if (operationDescription == null)
        {
            errorBuilder.AppendLine(String.Format("The parameter passed into the Validate method was null/nThe null parameter is: {0}", typeof(OperationDescription).ToString()));
        }

        validateMethodTcs.TrySetResult(true);
    }
}
