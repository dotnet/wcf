// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;
using Infrastructure.Common;
using Xunit;
using System;
using System.Reflection;
using System.Xml.Serialization;

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
            Assert.True(false, $"Test Failed, caught unexpected exception.\nException: {exception.ToString()}\nException Message: {exception.Message}");
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
            Assert.True(false, $"Test Failed, caught unexpected exception.\nException: {exception.ToString()}\nException Message: {exception.Message}");
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void XmlSFAttributeRpcEncSingleNsTest()
    {
        RunVariation(Endpoints.BasciHttpRpcEncSingleNs_Address);
    }

    [WcfFact]
    [OuterLoop]
    public static void XmlSFAttributeRpcLitSingleNsTest()
    {
        RunVariation(Endpoints.BasicHttpRpcLitSingleNs_Address);
    }

    [WcfFact]
    [OuterLoop]
    public static void XmlSFAttributeDocLitSingleNsTest()
    {
        RunVariation(Endpoints.BasicHttpDocLitSingleNs_Address);
    }

    [WcfFact]
    [OuterLoop]
    public static void XmlSFAttributeRpcEncDualNsTest()
    {
        RunVariation(Endpoints.BasicHttpRpcEncDualNs_Address, true);
    }

    [WcfFact]
    [OuterLoop]
    public static void XmlSFAttributeRpcLitDualNsTest()
    {
        RunVariation(Endpoints.BasicHttpRpcLitDualNs_Address, true);
    }

    [WcfFact]
    [OuterLoop]
    public static void XmlSFAttributeDocLitDualNsTest()
    {
        RunVariation(Endpoints.BasicHttpDocLitDualNs_Address, true);
    }

    private static void RunVariation(string serviceAddress, bool isMultiNs = false)
    {
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<ICalculator> factory1 = null;
        ChannelFactory<IHelloWorld> factory2 = null;
        ICalculator serviceProxy1 = null;
        IHelloWorld serviceProxy2 = null;

        // *** SETUP *** \\
        binding = new BasicHttpBinding();
        endpointAddress = new EndpointAddress(serviceAddress);
        factory1 = new ChannelFactory<ICalculator>(binding, endpointAddress);
        serviceProxy1 = factory1.CreateChannel();
        if (isMultiNs)
        {
            factory2 = new ChannelFactory<IHelloWorld>(binding, endpointAddress);
            serviceProxy2 = factory2.CreateChannel();
        }

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

            if (isMultiNs)
            {
                Guid guid2 = Guid.NewGuid();
                serviceProxy2.AddString(guid2, testStr);
                Assert.Equal(testStr, serviceProxy2.GetAndRemoveString(guid2));
            }
        }
        catch (Exception ex)
        {
            Assert.True(false, ex.Message);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy1);
            if (isMultiNs)
            {
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy2);
            }
        }
    }
}
