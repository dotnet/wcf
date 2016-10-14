// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

using Infrastructure.Common;
using Xunit;

public static partial class FaultExceptionTests
{
    [WcfFact]
    [OuterLoop]
    public static void TestAccessFaultContractInfo()
    {
        MyClientBase<IWcfService> client = null;
        IWcfService serviceProxy = null;
        string action = @"http://tempuri.org/IWcfService/TestFaultFaultDetailFault";
        Type detail = typeof(FaultDetail);

        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            client = new MyClientBase<IWcfService>(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
            client.Endpoint.EndpointBehaviors.Add(new TestFaultContractInfosBehavior());
            serviceProxy = client.ChannelFactory.CreateChannel();

            // *** EXECUTE *** \\
            var input = new object[] { new FaultDetail(), new KnownTypeA() };
            // Use exiting service as we only need to verify FaultContractInfo can be accessed from ClientOperation
            serviceProxy.TestFaultWithKnownType(null, input);

            //Verify
            Assert.True(TestFaultContractInfosBehavior.testFaultWithKnownTypeClientOp != null, "Expected testFaultWithKnownTypeClientOp is NOT null");
            Assert.True(TestFaultContractInfosBehavior.testFaultWithKnownTypeClientOp.FaultContractInfos.Count == 1, string.Format("expected FaultContractInfos count is 1, actual: {0}", TestFaultContractInfosBehavior.testFaultWithKnownTypeClientOp.FaultContractInfos.Count));
            Assert.True(TestFaultContractInfosBehavior.testFaultWithKnownTypeClientOp.FaultContractInfos[0].Action == action, string.Format("expected FaultContractInfo Action is {0}, actual: {1}", action, TestFaultContractInfosBehavior.testFaultWithKnownTypeClientOp.FaultContractInfos[0].Action));
            Assert.True(TestFaultContractInfosBehavior.testFaultWithKnownTypeClientOp.FaultContractInfos[0].Detail == detail, string.Format("expected FaultContractInfo Detail is {0}, actual: {1}", detail.ToString(), TestFaultContractInfosBehavior.testFaultWithKnownTypeClientOp.FaultContractInfos[0].Detail.ToString()));

        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, (ICommunicationObject)client);
        }
    }
}

public class TestFaultContractInfosBehavior : IEndpointBehavior
{
    public static ClientOperation testFaultWithKnownTypeClientOp;
    public TestFaultContractInfosBehavior()
    {
    }

    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {
    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        foreach (ClientOperation clientOperation in clientRuntime.ClientOperations)
        {
            if (clientOperation.Name == "TestFaultWithKnownType")
            {
                testFaultWithKnownTypeClientOp = clientOperation;
                return;
            }
        }

        throw new Exception("Expected TestFaultWithKnownType in the ClientOperations, Actual:  TestFaultWithKnownType NOT Found");
    }
    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {
    }

    public void Validate(ServiceEndpoint endpoint)
    {
    }
}
