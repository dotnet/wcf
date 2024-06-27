// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public partial class CallbackErrorHandlerTests : ConditionalWcfTest
{
    [WcfFact]
    public static void DuplexCallbackErrorHandlerTest()
    {
        bool result;
        InstanceContext context;
        string testString="";
        NetTcpBinding binding;
        EndpointAddress endpointAddress;
        DuplexChannelFactory<IWcfDuplexService_CallbackErrorHandler> factory;
        IWcfDuplexService_CallbackErrorHandler serviceProxy;
        WcfDuplexService_CallbackErrorHandler_Callback callbackService;

        // *** SETUP *** \\
        binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.None;
        callbackService = new WcfDuplexService_CallbackErrorHandler_Callback();
        context = new InstanceContext(callbackService);
        endpointAddress = new EndpointAddress(Endpoints.DuplexCallbackErrorHandler_Address);
        factory = new DuplexChannelFactory<IWcfDuplexService_CallbackErrorHandler>(context, binding, endpointAddress);
        serviceProxy = factory.CreateChannel();       

        try
        {
            // *** EXECUTE *** \\
            result = serviceProxy.Hello(testString);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }

        // *** ADDITIONAL VALIDATION *** \\
        Assert.True(result);
    }
}
