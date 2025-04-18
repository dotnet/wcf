// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public partial class DataContractTests : ConditionalWcfTest
{
    // Verify that a callback contract correctly returns a complex type when this type is not part of the contract with the ServiceContract attribute.
    [WcfFact]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void NetTcpBinding_DuplexCallback_ReturnsDataContractComplexType()
    {
        DuplexChannelFactory<IWcfDuplexService_DataContract> factory = null;
        NetTcpBinding binding = null;
        WcfDuplexServiceCallback callbackService = null;
        InstanceContext context = null;
        IWcfDuplexService_DataContract serviceProxy = null;
        Guid guid = Guid.NewGuid();

        try
        {
            binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;

            callbackService = new WcfDuplexServiceCallback();
            context = new InstanceContext(callbackService);

            factory = new DuplexChannelFactory<IWcfDuplexService_DataContract>(context, binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_DataContractDuplexCallback_Address));
            serviceProxy = factory.CreateChannel();

            serviceProxy.Ping_DataContract(guid);
            ComplexCompositeTypeDuplexCallbackOnly returnedType = callbackService.DataContractCallbackGuid;

            // validate response
            Assert.True((guid == returnedType.GuidValue), String.Format("The Guid sent was not the same as the Guid returned.\nSent: {0}\nReturned: {1}", guid, returnedType.GuidValue));

            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    // Verify that a callback contract correctly returns a complex type using Xml instead of DataContract when this type is not part of the contract with the ServiceContract attribute.
    [WcfFact]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void NetTcpBinding_DuplexCallback_ReturnsXmlComplexType()
    {
        DuplexChannelFactory<IWcfDuplexService_Xml> factory = null;
        NetTcpBinding binding = null;
        WcfDuplexServiceCallback callbackService = null;
        InstanceContext context = null;
        IWcfDuplexService_Xml serviceProxy = null;
        Guid guid = Guid.NewGuid();

        try
        {
            binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;

            callbackService = new WcfDuplexServiceCallback();
            context = new InstanceContext(callbackService);

            factory = new DuplexChannelFactory<IWcfDuplexService_Xml>(context, binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_XmlDuplexCallback_Address));
            serviceProxy = factory.CreateChannel();

            serviceProxy.Ping_Xml(guid);
            XmlCompositeTypeDuplexCallbackOnly returnedType = callbackService.XmlCallbackGuid;

            // validate response
            Assert.True((guid.ToString() == returnedType.StringValue), String.Format("The Guid to string value sent was not the same as what was returned.\nSent: {0}\nReturned: {1}", guid.ToString(), returnedType.StringValue));

            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
