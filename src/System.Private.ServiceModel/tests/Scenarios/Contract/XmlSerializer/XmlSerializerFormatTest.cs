// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;
using Infrastructure.Common;
using Xunit;
using System;
using System.Reflection;
using System.Xml.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;


public class InspectorBehavior : IEndpointBehavior
{
    public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
    {

    }

    public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    {
        clientRuntime.ClientMessageInspectors.Add(new MyMessageInspector());
    }

    public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
    {
        
    }

    public void Validate(ServiceEndpoint endpoint)
    {
        
    }
}
public class MyMessageInspector : IClientMessageInspector
{
    public void AfterReceiveReply(ref Message reply, object correlationState)
    {
    }

    public object BeforeSendRequest(ref Message request, IClientChannel channel)
    {
        string str = request.ToString();
        Assert.Contains("http://schemas.xmlsoap.org/soap/encoding/", str);
        return null;
    }
}

public static partial class XmlSerializerFormatTests
{
#if SVCUTILTESTS
    private static readonly string s_serializationModeSetterName = "set_Mode";

    static XmlSerializerFormatTests()
    {
        MethodInfo method = typeof(XmlSerializer).GetMethod(s_serializationModeSetterName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        Assert.True(method != null, $"No method named {s_serializationModeSetterName}");
        method.Invoke(null, new object[] { 3 });
    }
#endif

    [WcfFact]
    [OuterLoop]
    public static void XmlSerializerFormatAttribute_SupportFaults()
    {
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IXmlSFAttribute> factory = null;
        IXmlSFAttribute serviceProxy = null;

        // *** SETUP *** \\
        binding = new BasicHttpBinding();
        endpointAddress = new EndpointAddress(Endpoints.XmlSFAttribute_Address);
        factory = new ChannelFactory<IXmlSFAttribute>(binding, endpointAddress);
        serviceProxy = factory.CreateChannel();

        // *** EXECUTE 1st Variation *** \\
        try
        {
            // Calling the Operation Contract overload with "SupportFaults" not set, default is "false"
            serviceProxy.TestXmlSerializerSupportsFaults_False();
        }
        catch (FaultException<FaultDetailWithXmlSerializerFormatAttribute> fException)
        {
            // In this variation the Fault message should have been returned using the Data Contract Serializer.
            Assert.True(fException.Detail.UsedDataContractSerializer, "The returning Fault Detail should have used the Data Contract Serializer.");
            Assert.True(fException.Detail.UsedXmlSerializer == false, "The returning Fault Detail should NOT have used the Xml Serializer.");
        }
        catch (Exception exception)
        {
            Assert.Fail($"Test Failed, caught unexpected exception.\nException: {exception.ToString()}\nException Message: {exception.Message}");
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy);
        }

        // *** EXECUTE 2nd Variation *** \\
        try
        {
            serviceProxy = factory.CreateChannel();
            serviceProxy.TestXmlSerializerSupportsFaults_True();
        }
        catch (FaultException<FaultDetailWithXmlSerializerFormatAttribute> fException)
        {
            // In this variation the Fault message should have been returned using the Xml Serializer.
            Assert.True(fException.Detail.UsedDataContractSerializer == false, "The returning Fault Detail should NOT have used the Data Contract Serializer.");
            Assert.True(fException.Detail.UsedXmlSerializer, "The returning Fault Detail should have used the Xml Serializer.");
        }
        catch (Exception exception)
        {
            Assert.Fail($"Test Failed, caught unexpected exception.\nException: {exception.ToString()}\nException Message: {exception.Message}");
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void XmlSerializerFormatEncodedAttributeTest()
    {
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<ICalculatorRpcEnc> factory1 = null;
        ICalculatorRpcEnc serviceProxy1 = null;

        // *** SETUP *** \\
        binding = new BasicHttpBinding();
        endpointAddress = new EndpointAddress(Endpoints.BasicHttpRpcEncSingleNs_Address);
        factory1 = new ChannelFactory<ICalculatorRpcEnc>(binding, endpointAddress);
        factory1.Endpoint.EndpointBehaviors.Add(new InspectorBehavior());
        serviceProxy1 = factory1.CreateChannel();

        // *** EXECUTE Variation *** \\
        try
        {
            Assert.Equal(3, serviceProxy1.Sum2(1, 2));
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy1);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void XmlSFAttributeRpcEncSingleNsTest()
    {
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<ICalculatorRpcEnc> factory1 = null;
        ICalculatorRpcEnc serviceProxy1 = null;

        // *** SETUP *** \\
        binding = new BasicHttpBinding();
        endpointAddress = new EndpointAddress(Endpoints.BasicHttpRpcEncSingleNs_Address);
        factory1 = new ChannelFactory<ICalculatorRpcEnc>(binding, endpointAddress);
        serviceProxy1 = factory1.CreateChannel();

        // *** EXECUTE Variation *** \\
        try
        {
            var dateTime = DateTime.Now;
            var intParams = new IntParams() { P1 = 5, P2 = 10 };
            var floatParams = new FloatParams() { P1 = 5.0f, P2 = 10.0f };
            var byteParams = new ByteParams() { P1 = 5, P2 = 10 };

            Assert.Equal(3, serviceProxy1.Sum2(1, 2));
            Assert.Equal(intParams.P1 + intParams.P2, serviceProxy1.Sum(intParams));
            Assert.Equal(string.Format("{0}{1}", intParams.P1, intParams.P2), serviceProxy1.Concatenate(intParams));
            Assert.Equal((float)(floatParams.P1 / floatParams.P2), serviceProxy1.Divide(floatParams));
            Assert.Equal((new byte[] { byteParams.P1, byteParams.P2 }), serviceProxy1.CreateSet(byteParams));
            Assert.Equal(dateTime, serviceProxy1.ReturnInputDateTime(dateTime));

            Guid guid = Guid.NewGuid();
            serviceProxy1.AddIntParams(guid, intParams);
            IntParams outputIntParams = serviceProxy1.GetAndRemoveIntParams(guid);
            Assert.NotNull(outputIntParams);
            Assert.Equal(intParams.P1, outputIntParams.P1);
            Assert.Equal(intParams.P2, outputIntParams.P2);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy1);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void XmlSFAttributeRpcLitSingleNsTest()
    {
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<ICalculatorRpcLit> factory1 = null;
        ICalculatorRpcLit serviceProxy1 = null;

        // *** SETUP *** \\
        binding = new BasicHttpBinding();
        endpointAddress = new EndpointAddress(Endpoints.BasicHttpRpcLitSingleNs_Address);
        factory1 = new ChannelFactory<ICalculatorRpcLit>(binding, endpointAddress);
        serviceProxy1 = factory1.CreateChannel();

        // *** EXECUTE Variation *** \\
        try
        {
            var dateTime = DateTime.Now;
            var intParams = new IntParams() { P1 = 5, P2 = 10 };
            var floatParams = new FloatParams() { P1 = 5.0f, P2 = 10.0f };
            var byteParams = new ByteParams() { P1 = 5, P2 = 10 };

            Assert.Equal(3, serviceProxy1.Sum2(1, 2));
            Assert.Equal(intParams.P1 + intParams.P2, serviceProxy1.Sum(intParams));
            Assert.Equal(string.Format("{0}{1}", intParams.P1, intParams.P2), serviceProxy1.Concatenate(intParams));
            Assert.Equal((float)(floatParams.P1 / floatParams.P2), serviceProxy1.Divide(floatParams));
            Assert.Equal((new byte[] { byteParams.P1, byteParams.P2 }), serviceProxy1.CreateSet(byteParams));
            Assert.Equal(dateTime, serviceProxy1.ReturnInputDateTime(dateTime));

            Guid guid = Guid.NewGuid();
            serviceProxy1.AddIntParams(guid, intParams);
            IntParams outputIntParams = serviceProxy1.GetAndRemoveIntParams(guid);
            Assert.NotNull(outputIntParams);
            Assert.Equal(intParams.P1, outputIntParams.P1);
            Assert.Equal(intParams.P2, outputIntParams.P2);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy1);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void XmlSFAttributeDocLitSingleNsTest()
    {
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<ICalculatorDocLit> factory1 = null;
        ICalculatorDocLit serviceProxy1 = null;

        // *** SETUP *** \\
        binding = new BasicHttpBinding();
        endpointAddress = new EndpointAddress(Endpoints.BasicHttpDocLitSingleNs_Address);
        factory1 = new ChannelFactory<ICalculatorDocLit>(binding, endpointAddress);
        serviceProxy1 = factory1.CreateChannel();

        // *** EXECUTE Variation *** \\
        try
        {
            var dateTime = DateTime.Now;
            var intParams = new IntParams() { P1 = 5, P2 = 10 };
            var floatParams = new FloatParams() { P1 = 5.0f, P2 = 10.0f };
            var byteParams = new ByteParams() { P1 = 5, P2 = 10 };

            Assert.Equal(3, serviceProxy1.Sum2(1, 2));
            Assert.Equal(intParams.P1 + intParams.P2, serviceProxy1.Sum(intParams));
            Assert.Equal(string.Format("{0}{1}", intParams.P1, intParams.P2), serviceProxy1.Concatenate(intParams));
            Assert.Equal((float)(floatParams.P1 / floatParams.P2), serviceProxy1.Divide(floatParams));
            Assert.Equal((new byte[] { byteParams.P1, byteParams.P2 }), serviceProxy1.CreateSet(byteParams));
            Assert.Equal(dateTime, serviceProxy1.ReturnInputDateTime(dateTime));

            Guid guid = Guid.NewGuid();
            serviceProxy1.AddIntParams(guid, intParams);
            IntParams outputIntParams = serviceProxy1.GetAndRemoveIntParams(guid);
            Assert.NotNull(outputIntParams);
            Assert.Equal(intParams.P1, outputIntParams.P1);
            Assert.Equal(intParams.P2, outputIntParams.P2);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy1);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void XmlSFAttributeRpcEncDualNsTest()
    {
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<ICalculatorRpcEnc> factory1 = null;
        ChannelFactory<IHelloWorldRpcEnc> factory2 = null;
        ICalculatorRpcEnc serviceProxy1 = null;
        IHelloWorldRpcEnc serviceProxy2 = null;

        // *** SETUP *** \\
        binding = new BasicHttpBinding();
        endpointAddress = new EndpointAddress(Endpoints.BasicHttpRpcEncDualNs_Address);
        factory1 = new ChannelFactory<ICalculatorRpcEnc>(binding, endpointAddress);
        serviceProxy1 = factory1.CreateChannel();
        factory2 = new ChannelFactory<IHelloWorldRpcEnc>(binding, endpointAddress);
        serviceProxy2 = factory2.CreateChannel();
        
        // *** EXECUTE Variation *** \\
        try
        {
            var dateTime = DateTime.Now;
            string testStr = "test string";
            var intParams = new IntParams() { P1 = 5, P2 = 10 };
            var floatParams = new FloatParams() { P1 = 5.0f, P2 = 10.0f };
            var byteParams = new ByteParams() { P1 = 5, P2 = 10 };

            Assert.Equal(3, serviceProxy1.Sum2(1, 2));
            Assert.Equal(intParams.P1 + intParams.P2, serviceProxy1.Sum(intParams));
            Assert.Equal(string.Format("{0}{1}", intParams.P1, intParams.P2), serviceProxy1.Concatenate(intParams));
            Assert.Equal((float)(floatParams.P1 / floatParams.P2), serviceProxy1.Divide(floatParams));
            Assert.Equal((new byte[] { byteParams.P1, byteParams.P2 }), serviceProxy1.CreateSet(byteParams));
            Assert.Equal(dateTime, serviceProxy1.ReturnInputDateTime(dateTime));

            Guid guid = Guid.NewGuid();
            serviceProxy1.AddIntParams(guid, intParams);
            IntParams outputIntParams = serviceProxy1.GetAndRemoveIntParams(guid);
            Assert.NotNull(outputIntParams);
            Assert.Equal(intParams.P1, outputIntParams.P1);
            Assert.Equal(intParams.P2, outputIntParams.P2);
            Guid guid2 = Guid.NewGuid();
            serviceProxy2.AddString(guid2, testStr);
            Assert.Equal(testStr, serviceProxy2.GetAndRemoveString(guid2));
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy1);
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy2);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void XmlSFAttributeRpcLitDualNsTest()
    {
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<ICalculatorRpcLit> factory1 = null;
        ChannelFactory<IHelloWorldRpcLit> factory2 = null;
        ICalculatorRpcLit serviceProxy1 = null;
        IHelloWorldRpcLit serviceProxy2 = null;

        // *** SETUP *** \\
        binding = new BasicHttpBinding();
        endpointAddress = new EndpointAddress(Endpoints.BasicHttpRpcLitDualNs_Address);
        factory1 = new ChannelFactory<ICalculatorRpcLit>(binding, endpointAddress);
        serviceProxy1 = factory1.CreateChannel();
        factory2 = new ChannelFactory<IHelloWorldRpcLit>(binding, endpointAddress);
        serviceProxy2 = factory2.CreateChannel();

        // *** EXECUTE Variation *** \\
        try
        {
            var dateTime = DateTime.Now;
            string testStr = "test string";
            var intParams = new IntParams() { P1 = 5, P2 = 10 };
            var floatParams = new FloatParams() { P1 = 5.0f, P2 = 10.0f };
            var byteParams = new ByteParams() { P1 = 5, P2 = 10 };

            Assert.Equal(3, serviceProxy1.Sum2(1, 2));
            Assert.Equal(intParams.P1 + intParams.P2, serviceProxy1.Sum(intParams));
            Assert.Equal(string.Format("{0}{1}", intParams.P1, intParams.P2), serviceProxy1.Concatenate(intParams));
            Assert.Equal((float)(floatParams.P1 / floatParams.P2), serviceProxy1.Divide(floatParams));
            Assert.Equal((new byte[] { byteParams.P1, byteParams.P2 }), serviceProxy1.CreateSet(byteParams));
            Assert.Equal(dateTime, serviceProxy1.ReturnInputDateTime(dateTime));

            Guid guid = Guid.NewGuid();
            serviceProxy1.AddIntParams(guid, intParams);
            IntParams outputIntParams = serviceProxy1.GetAndRemoveIntParams(guid);
            Assert.NotNull(outputIntParams);
            Assert.Equal(intParams.P1, outputIntParams.P1);
            Assert.Equal(intParams.P2, outputIntParams.P2);
            Guid guid2 = Guid.NewGuid();
            serviceProxy2.AddString(guid2, testStr);
            Assert.Equal(testStr, serviceProxy2.GetAndRemoveString(guid2));
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy1);
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy2);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void XmlSFAttributeDocLitDualNsTest()
    {
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<ICalculatorDocLit> factory1 = null;
        ChannelFactory<IHelloWorldDocLit> factory2 = null;
        ICalculatorDocLit serviceProxy1 = null;
        IHelloWorldDocLit serviceProxy2 = null;

        // *** SETUP *** \\
        binding = new BasicHttpBinding();
        endpointAddress = new EndpointAddress(Endpoints.BasicHttpDocLitDualNs_Address);
        factory1 = new ChannelFactory<ICalculatorDocLit>(binding, endpointAddress);
        serviceProxy1 = factory1.CreateChannel();
        factory2 = new ChannelFactory<IHelloWorldDocLit>(binding, endpointAddress);
        serviceProxy2 = factory2.CreateChannel();

        // *** EXECUTE Variation *** \\
        try
        {
            var dateTime = DateTime.Now;
            string testStr = "test string";
            var intParams = new IntParams() { P1 = 5, P2 = 10 };
            var floatParams = new FloatParams() { P1 = 5.0f, P2 = 10.0f };
            var byteParams = new ByteParams() { P1 = 5, P2 = 10 };

            Assert.Equal(3, serviceProxy1.Sum2(1, 2));
            Assert.Equal(intParams.P1 + intParams.P2, serviceProxy1.Sum(intParams));
            Assert.Equal(string.Format("{0}{1}", intParams.P1, intParams.P2), serviceProxy1.Concatenate(intParams));
            Assert.Equal((float)(floatParams.P1 / floatParams.P2), serviceProxy1.Divide(floatParams));
            Assert.Equal((new byte[] { byteParams.P1, byteParams.P2 }), serviceProxy1.CreateSet(byteParams));
            Assert.Equal(dateTime, serviceProxy1.ReturnInputDateTime(dateTime));

            Guid guid = Guid.NewGuid();
            serviceProxy1.AddIntParams(guid, intParams);
            IntParams outputIntParams = serviceProxy1.GetAndRemoveIntParams(guid);
            Assert.NotNull(outputIntParams);
            Assert.Equal(intParams.P1, outputIntParams.P1);
            Assert.Equal(intParams.P2, outputIntParams.P2);
            Guid guid2 = Guid.NewGuid();
            serviceProxy2.AddString(guid2, testStr);
            Assert.Equal(testStr, serviceProxy2.GetAndRemoveString(guid2));
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy1);
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy2);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void XmlSFAttributeRpcEncMessageHeaderTest()
    {
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IEchoRpcEncWithHeadersService> factory1 = null;
        IEchoRpcEncWithHeadersService serviceProxy1 = null;
        string echoText = "Hello";
        string headerText = "WCF is Cool!";
        string expectedHeaderText = headerText + headerText;

        // *** SETUP *** \\
        binding = new BasicHttpBinding();
        endpointAddress = new EndpointAddress(Endpoints.BasicHttpRpcEncWithHeaders_Address);
        factory1 = new ChannelFactory<IEchoRpcEncWithHeadersService>(binding, endpointAddress);
        serviceProxy1 = factory1.CreateChannel();

        // *** EXECUTE Variation *** \\
        try
        {
            var response = serviceProxy1.Echo(new EchoRequest { message = "Hello", StringHeader = new StringHeader { HeaderValue = "WCF is Cool!" } });

            Assert.Equal(echoText, response.EchoResult);
            Assert.Equal(expectedHeaderText, response.StringHeader.HeaderValue);
        }
        catch (Exception ex)
        {
            Assert.Fail(ex.Message);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy1);
        }
    }
}
