// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;
using System;

public static partial class XmlSerializerFormatTests
{
    [WcfFact]
    [OuterLoop]
    public static void CombineString_XmlSerializerFormat_Soap()
    {
        RunWcfSoapServiceTest((serviceProxy) =>
        {
            // *** EXECUTE *** \\
            string message1 = "hello";
            string message2 = "world";
            var response = serviceProxy.CombineStringXmlSerializerFormatSoap(message1, message2);

            // *** VALIDATE *** \\
            Assert.Equal(message1 + message2, response);
        });
    }

    [WcfFact]
    [OuterLoop]
    public static void EchoComositeType_XmlSerializerFormat_Soap()
    {
        RunWcfSoapServiceTest((serviceProxy) =>
        {
            // *** EXECUTE *** \\
            var value = new SoapComplexType() { BoolValue = true, StringValue = "hello" };
            SoapComplexType response = serviceProxy.EchoComositeTypeXmlSerializerFormatSoap(value);

            // *** VALIDATE *** \\
            Assert.NotNull(response);
            Assert.Equal(value.BoolValue, response.BoolValue);
            Assert.Equal(value.StringValue, response.StringValue);
        });
    }

    [WcfFact]
    [OuterLoop]
    public static void ProcessCustomerData_XmlSerializerFormat_Soap()
    {
        RunWcfSoapServiceTest((serviceProxy) =>
        {
            // *** EXECUTE *** \\
            CustomerObject value = new CustomerObject() { Name = "MyName", Data = new AdditionalData() { Field = "Foo" } };
            string response = serviceProxy.ProcessCustomerData(value);

            // *** VALIDATE *** \\
            Assert.Equal("MyNameFoo", response);
        });
    }

    [WcfFact]
    [OuterLoop]
    public static void TestCreateChannel()
    {
        RunWcfSoapServiceTest((serviceProxy) =>
        {
            // *** EXECUTE *** \\
            int intValue = 11;
            string intString = intValue.ToString();
            var request = new PingEncodedRequest(intString);
            PingEncodedResponse response = serviceProxy.Ping(request);

            // *** VALIDATE *** \\
            Assert.NotNull(response);
            Assert.Equal(intValue, response.@Return);
        });
    }

    private static void RunWcfSoapServiceTest(Action<IWcfSoapService> testMethod)
    {
        BasicHttpBinding binding;
        EndpointAddress endpointAddress;
        ChannelFactory<IWcfSoapService> factory;
        IWcfSoapService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            endpointAddress = new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Soap);
            factory = new ChannelFactory<IWcfSoapService>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();
            testMethod(serviceProxy);

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy);
        }
    }
}
