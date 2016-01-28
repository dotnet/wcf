// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Xunit;

public static class XmlSerializerFormatTests
{
    private static readonly string s_basicEndpointAddress = Endpoints.HttpBaseAddress_Basic;

    [Fact]
    [OuterLoop]
    public static void XmlSerializerFormat_RoundTrips_String()
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress(s_basicEndpointAddress);
        ChannelFactory<IWcfServiceXmlGenerated> factory = new ChannelFactory<IWcfServiceXmlGenerated>(binding, endpointAddress);
        IWcfServiceXmlGenerated serviceProxy = factory.CreateChannel();

        var response = serviceProxy.EchoXmlSerializerFormat("message");
        Assert.Equal("message", response);
    }

    [Fact]
    [OuterLoop]
    public static void XmlSerializerFormat_Using_SupportsFault_RoundTrips_String()
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress(s_basicEndpointAddress);
        ChannelFactory<IWcfServiceXmlGenerated> factory = new ChannelFactory<IWcfServiceXmlGenerated>(binding, endpointAddress);
        IWcfServiceXmlGenerated serviceProxy = factory.CreateChannel();

        var response = serviceProxy.EchoXmlSerializerFormatSupportFaults("message", false);
        Assert.Equal("message", response);
    }

    [Fact]
    [OuterLoop]
    public static void XmlSerializerFormat_Using_SupportsFault_Throws_FaultException()
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress(s_basicEndpointAddress);
        ChannelFactory<IWcfServiceXmlGenerated> factory = new ChannelFactory<IWcfServiceXmlGenerated>(binding, endpointAddress);
        IWcfServiceXmlGenerated serviceProxy = factory.CreateChannel();

        var errorMessage = "ErrorMessage";

        try
        {
            var response = serviceProxy.EchoXmlSerializerFormatSupportFaults(errorMessage, true);
        }
        catch (FaultException e)
        {
            Assert.Equal(errorMessage, e.Message);
            return;
        }

        // we shouldn't reach here.
        Assert.True(false);
    }

    [Fact]
    [OuterLoop]
    public static void XmlSerializerFormat_RoundTrips_Using_Rpc()
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress(s_basicEndpointAddress);
        ChannelFactory<IWcfServiceXmlGenerated> factory = new ChannelFactory<IWcfServiceXmlGenerated>(binding, endpointAddress);
        IWcfServiceXmlGenerated serviceProxy = factory.CreateChannel();

        var response = serviceProxy.EchoXmlSerializerFormatUsingRpc("message");
        Assert.Equal("message", response);
    }

    [Fact]
    [OuterLoop]
    public static void XmlSerializerFormat_RoundTrips_String_AsyncTask()
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress(s_basicEndpointAddress);
        ChannelFactory<IWcfServiceXmlGenerated> factory = new ChannelFactory<IWcfServiceXmlGenerated>(binding, endpointAddress);
        IWcfServiceXmlGenerated serviceProxy = factory.CreateChannel();


        Task<string> response = serviceProxy.EchoXmlSerializerFormatAsync("message");
        response.Wait();
        Assert.True(response != null);
        Assert.Equal("message", response.Result);
    }

    [Fact]
    [OuterLoop]
    public static void XmlSerializerFormat_RoundTrips_CompositeType()
    {
        BasicHttpBinding binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress(s_basicEndpointAddress);
        ChannelFactory<IWcfServiceXmlGenerated> factory = new ChannelFactory<IWcfServiceXmlGenerated>(binding, endpointAddress);
        IWcfServiceXmlGenerated serviceProxy = factory.CreateChannel();


        var input = new XmlCompositeType();
        input.StringValue = "message";
        input.BoolValue = false;
        var response = serviceProxy.GetDataUsingXmlSerializer(input);
        Assert.True(response != null);
        Assert.Equal("message", response.StringValue);
        Assert.True(!input.BoolValue);
    }

    [Fact]
    [OuterLoop]
    public static void XmlSerializerFormat_MessageContract_LoginService()
    {
        // *** SETUP *** \\
        BasicHttpBinding binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress(s_basicEndpointAddress);
        ChannelFactory<ILoginService> factory = new ChannelFactory<ILoginService>(binding, endpointAddress);
        ILoginService serviceProxy = factory.CreateChannel();
        
        var request = new LoginRequest();
        request.clientId = "1";
        request.user = "2";
        request.pwd = "3";

        try
        {
            // *** EXECUTE *** \\
            var response = serviceProxy.Login(request);

            // *** VALIDATE *** \\
            Assert.True(response != null);
            Assert.Equal("123", response.@return);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [Fact]
    [OuterLoop]
    // The test is for the case where a paramerter type contains a field 
    // never used.The test is to make sure the reflection info of the type 
    // of the unused field would be kept by Net Native toolchain.
    public static void XmlSerializerFormat_ComplexType_With_FieldType_Never_Used()
    {
        // *** SETUP *** \\
        BasicHttpBinding binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress(s_basicEndpointAddress);
        ChannelFactory<IWcfServiceXmlGenerated> factory = new ChannelFactory<IWcfServiceXmlGenerated>(binding, endpointAddress);
        IWcfServiceXmlGenerated serviceProxy = factory.CreateChannel();

        var complex = new XmlVeryComplexType();
        complex.Id = 1;

        try
        {
            // *** EXECUTE *** \\
            var response = serviceProxy.EchoXmlVeryComplexType(complex);

            // *** VALIDATE *** \\
            Assert.True(response != null);
            Assert.True(response.NonInstantiatedField == null);
            Assert.Equal(complex.Id, response.Id);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [Fact]
    [OuterLoop]
    public static void OperationContextScope_HttpRequestCustomMessageHeader_RoundTrip_Verify()
    {
        // *** SETUP *** \\
        BasicHttpBinding binding = new BasicHttpBinding();
        MyClientBase<IWcfServiceXmlGenerated> client = new MyClientBase<IWcfServiceXmlGenerated>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
        IWcfServiceXmlGenerated serviceProxy = client.ChannelFactory.CreateChannel();

        string customHeaderName = "TestSessionHeader";
        string customHeaderNS = "xmlns=urn:TestWebServices/MyWebService/";
        var customHeaderValue = new MesssageHeaderCreateHeaderWithXmlSerializerTestType { Message = "secret" };

        try
        {
            using (OperationContextScope scope = new OperationContextScope((IContextChannel)serviceProxy))
            {
                // *** EXECUTE *** \\
                MessageHeader header
                  = MessageHeader.CreateHeader(
                  customHeaderName,
                  customHeaderNS,
                  customHeaderValue
                  );
                OperationContext.Current.OutgoingMessageHeaders.Add(header);

                string result = serviceProxy.GetIncomingMessageHeadersMessage(customHeaderName, customHeaderNS);

                // *** VALIDATE *** \\
                Assert.Equal(customHeaderValue.Message, result);
            }

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, client);
        }
    }

    [Fact]
    [OuterLoop]
    public static void MessageHeader_ResponseTypeWithUsesMessageHeaderAttribute()
    {
        // *** SETUP *** \\
        var binding = new BasicHttpBinding();
        var endpointAddress = new EndpointAddress(Endpoints.HttpBaseAddress_Basic);
        var factory = new ChannelFactory<IXmlMessageContarctTestService>(binding, endpointAddress);
        IXmlMessageContarctTestService serviceProxy = factory.CreateChannel();
        var input = new XmlMessageContractTestRequest("1");

        try
        {
            // *** EXECUTE *** \\
            XmlMessageContractTestResponse response = serviceProxy.EchoMessageResponseWithMessageHeader(input);

            // *** VALIDATE *** \\
            Assert.NotNull(response);
            Assert.Equal(input.Message, response.Message);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [Fact]
    [OuterLoop]
    [ActiveIssue(702)]
    public static void MessageHeader_RequestTypeWithUsesMessageHeaderAttribute()
    {
        // *** SETUP *** \\
        var binding = new BasicHttpBinding();
        var endpointAddress = new EndpointAddress(Endpoints.HttpBaseAddress_Basic);
        var factory = new ChannelFactory<IXmlMessageContarctTestService>(binding, endpointAddress);
        IXmlMessageContarctTestService serviceProxy = factory.CreateChannel();
        var input = new XmlMessageContractTestRequestWithMessageHeader("1");

        try
        {
            // *** EXECUTE *** \\
            XmlMessageContractTestResponse response = serviceProxy.EchoMessageResquestWithMessageHeader(input);

            // *** VALIDATE *** \\
            Assert.NotNull(response);
            Assert.Equal(input.Message, response.Message);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
