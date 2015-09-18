// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using TestTypes;
using System.Text;
using Xunit;

public static class DataContractTests
{
    [Fact]
    [OuterLoop]
    public static void CustomBinding_DefaultSettings_Echo_RoundTrips_DataContract()
    {
        // Verifies a typed proxy can call a service operation echoing a DataContract object synchronously
        StringBuilder errorBuilder = new StringBuilder();
        try
        {
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            // Note the service interface used.  It was manually generated with svcutil.
            ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
            IWcfService serviceProxy = factory.CreateChannel();

            CompositeType request = new CompositeType() { StringValue = "myString", BoolValue = true };
            CompositeType response = serviceProxy.GetDataUsingDataContract(request);

            Assert.True(response != null, "GetDataUsingDataContract(request) returned null");

            string expectedStringValue = request.StringValue + "Suffix";
            if (!string.Equals(response.StringValue, expectedStringValue))
            {
                errorBuilder.AppendLine(string.Format("Expected CompositeType.StringValue \"{0}\", actual was \"{1}\"",
                                                        expectedStringValue, response.StringValue));
            }
            if (response.BoolValue != request.BoolValue)
            {
                errorBuilder.AppendLine(string.Format("Expected CompositeType.BoolValue \"{0}\", actual was \"{1}\"",
                                                        request.BoolValue, response.BoolValue));
            }

            factory.Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, errorBuilder.ToString());
    }

    // Verify that a callback contract correctly returns a complex type when this type is not part of the contract with the ServiceContract attribute.
    [Fact]
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
    [Fact]
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
