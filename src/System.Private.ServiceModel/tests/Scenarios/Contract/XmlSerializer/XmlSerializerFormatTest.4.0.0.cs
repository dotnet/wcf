// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public static partial class XmlSerializerFormatTests
{
    private static readonly string s_basicEndpointAddress = Endpoints.HttpBaseAddress_Basic_Text;

    [WcfFact]
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

    [WcfFact]
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

    [WcfFact]
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

    [WcfFact]
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

    [WcfFact]
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

    [WcfFact]
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

    [WcfFact]
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

    [WcfFact]
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

    [WcfFact]
    [OuterLoop]
    public static void XmlSerializerFormat_SameNamespace_SameOperation()
    {
        // This test covers the scenariow where two service contracts share
        // the same namespace and have the same method.

        // *** SETUP *** \\
        BasicHttpBinding binding = new BasicHttpBinding();
        EndpointAddress endpointAddress = new EndpointAddress(s_basicEndpointAddress);
        ChannelFactory<ISameNamespaceWithIWcfServiceXmlGenerated> factory = new ChannelFactory<ISameNamespaceWithIWcfServiceXmlGenerated>(binding, endpointAddress);
        ISameNamespaceWithIWcfServiceXmlGenerated serviceProxy = factory.CreateChannel();

        try
        {
            // *** EXECUTE *** \\
            string response = serviceProxy.EchoXmlSerializerFormat("message");

            // *** VALIDATE *** \\
            Assert.Equal("message", response);

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
