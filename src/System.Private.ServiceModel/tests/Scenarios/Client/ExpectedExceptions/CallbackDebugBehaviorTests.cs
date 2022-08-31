// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Infrastructure.Common;
using Xunit;

public partial class ExpectedExceptionTests : ConditionalWcfTest
{
    [WcfTheory]
    [InlineData(true)]
    [InlineData(false)]
    public static void DuplexCallbackDebugBehavior_IncludeExceptionDetailInFaults_True(bool includeExceptionDetailInFaults)
    {
        DuplexChannelFactory<IWcfDuplexService_CallbackDebugBehavior> factory;
        ChannelFactory<ICheckCallbackDbgBhvService> factory2 = null;
        EndpointAddress endpointAddress;
        NetTcpBinding binding;
        const string greeting = "hello";
        WcfDuplexService_CallbackDebugBehavior_Callback callbackService;
        InstanceContext context;
        IWcfDuplexService_CallbackDebugBehavior serviceProxy;
        ICheckCallbackDbgBhvService serviceProxy2 = null;

        // *** VALIDATE *** \\

        // *** SETUP *** \\
        binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.None;
        callbackService = new WcfDuplexService_CallbackDebugBehavior_Callback();
        context = new InstanceContext(callbackService);
        endpointAddress = new EndpointAddress(Endpoints.DuplexCallbackDebugBehavior_Address);
        factory = new DuplexChannelFactory<IWcfDuplexService_CallbackDebugBehavior>(context, binding, endpointAddress);

        System.Collections.ObjectModel.KeyedCollection<Type, IEndpointBehavior> endpointBehaviors = factory.Endpoint.EndpointBehaviors;
        if (endpointBehaviors.TryGetValue(typeof(CallbackDebugBehavior), out IEndpointBehavior ieb))
        {
            (ieb as CallbackDebugBehavior).IncludeExceptionDetailInFaults = includeExceptionDetailInFaults;
        }
        else
        {
            endpointBehaviors.Add(new CallbackDebugBehavior(includeExceptionDetailInFaults));
        }

        serviceProxy = factory.CreateChannel();
        
        // *** EXECUTE *** \\
        try
        {
            string response = serviceProxy.Hello(greeting, includeExceptionDetailInFaults);
            Assert.Null(response);
        }
        catch
        {
            var binding2 = new BasicHttpBinding();
            factory2 = new ChannelFactory<ICheckCallbackDbgBhvService>(binding2, new EndpointAddress(Endpoints.HttpBaseAddress_BasicCheckCallbackDbgBhvService));
            serviceProxy2 = factory2.CreateChannel();
            Assert.True(serviceProxy2.GetResult(includeExceptionDetailInFaults));
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy2, factory2);
        }
    }
}
