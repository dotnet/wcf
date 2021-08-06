// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Serialization;
using System.ServiceModel;
using Infrastructure.Common;
using Xunit;

public static class ServiceKnownTypeTests
{
    public delegate object[] EchoItemsMethod(object[] items);

    [WcfFact]
    [OuterLoop]
    public static void ServiceKnownType_DataContract_AttrOnMethod_Test()
    {
        // *** SETUP *** \\
        ChannelFactory<IServiceKnownTypeTest_AttrOnMethod> factory = GetChannelFactory<IServiceKnownTypeTest_AttrOnMethod>();
        IServiceKnownTypeTest_AttrOnMethod serviceProxy = factory.CreateChannel();

        // *** EXECUTE *** \\
        // *** VALIDATE *** \\
        // *** CLEANUP *** \\
        RunTestMethodAndCleanup(factory, serviceProxy, serviceProxy.EchoItems, new Widget0());
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceKnownType_DataContract_AttrOnType_Test()
    {
        // *** SETUP *** \\
        ChannelFactory<IServiceKnownTypeTest_AttrOnType> factory = GetChannelFactory<IServiceKnownTypeTest_AttrOnType>();
        IServiceKnownTypeTest_AttrOnType serviceProxy = factory.CreateChannel();

        // *** EXECUTE *** \\
        // *** VALIDATE *** \\
        // *** CLEANUP *** \\
        RunTestMethodAndCleanup(factory, serviceProxy, serviceProxy.EchoItems, new Widget1());
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceKnownType_XmlSerializerFormat_AttrOnMethod_Test()
    {
        // *** SETUP *** \\
        ChannelFactory<IServiceKnownTypeTest_AttrOnMethod_Xml> factory = GetChannelFactory<IServiceKnownTypeTest_AttrOnMethod_Xml>();
        IServiceKnownTypeTest_AttrOnMethod_Xml serviceProxy = factory.CreateChannel();

        // *** EXECUTE *** \\
        // *** VALIDATE *** \\
        // *** CLEANUP *** \\
        RunTestMethodAndCleanup(factory, serviceProxy, serviceProxy.EchoItems_Xml, new Widget2());
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceKnownType_XmlSerializerFormat_AttrOnType_Test()
    {
        // *** SETUP *** \\
        ChannelFactory<IServiceKnownTypeTest_AttrOnType_Xml> factory = GetChannelFactory<IServiceKnownTypeTest_AttrOnType_Xml>();
        IServiceKnownTypeTest_AttrOnType_Xml serviceProxy = factory.CreateChannel();

        // *** EXECUTE *** \\
        // *** VALIDATE *** \\
        // *** CLEANUP *** \\
        RunTestMethodAndCleanup(factory, serviceProxy, serviceProxy.EchoItems_Xml, new Widget3());
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceKnownType_XmlSerializerFormat_TwoOperationsShareKnownTypes_Test()
    {
        // In XmlSerializerOperationBehavior.cs we made a performance improvement change for the scenario
        // where multiple operations of one service contract share the same ServiceKnownTypeAttribute(s).
        // The fix was to skip parsing duplicate ServiceKnownTypeAttribute(s). This test was to verify that
        // scenario still works after the fix.

        // *** SETUP *** \\
        ChannelFactory<IServiceKnownTypeTest_AttrOnType_Xml> factory = GetChannelFactory<IServiceKnownTypeTest_AttrOnType_Xml>();
        IServiceKnownTypeTest_AttrOnType_Xml serviceProxy = factory.CreateChannel();

        // *** EXECUTE *** \\
        // *** VALIDATE *** \\
        // *** CLEANUP *** \\
        RunTestMethodAndCleanup(factory, serviceProxy, serviceProxy.EchoItems_Xml1, new Widget3());
    }

    private static ChannelFactory<ServiceContractType> GetChannelFactory<ServiceContractType>()
    {
        var binding = new BasicHttpBinding();
        var endpointAddress = new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text);
        var factory = new ChannelFactory<ServiceContractType>(binding, endpointAddress);
        return factory;
    }

    private static void RunTestMethodAndCleanup<ServiceContractType>(
        ChannelFactory<ServiceContractType> factory,
        ServiceContractType serviceProxy,
        EchoItemsMethod echoItems,
        Widget widget)
    {
        try
        {
            // *** SETUP *** \\
            widget.Id = "1";
            widget.Catalog = "widget";
            var input = new object[] { widget };

            // *** EXECUTE *** \\
            var response = echoItems(input);

            // *** VALIDATE *** \\
            var expectedId = widget.Id;
            var expectedCatalog = widget.Catalog;
            Assert.Equal(expectedId, ((Widget)response[0]).Id);
            Assert.Equal(expectedCatalog, ((Widget)response[0]).Catalog);

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
}

// Net Native Can only Find ServiceKnownType marked on ServiceContract
// For DataContract serializer
[ServiceContract()]
public interface IServiceKnownTypeTest_AttrOnMethod
{
    [OperationContract(Action = "http://tempuri.org/IWcfService/EchoItems", ReplyAction = "http://tempuri.org/IWcfService/EchoItemsResponse")]
    [ServiceKnownType(typeof(Widget0))]
    object[] EchoItems(object[] objects);
}

[ServiceContract()]
[ServiceKnownType(typeof(Widget1))]
public interface IServiceKnownTypeTest_AttrOnType
{
    [OperationContract(Action = "http://tempuri.org/IWcfService/EchoItems", ReplyAction = "http://tempuri.org/IWcfService/EchoItemsResponse")]
    object[] EchoItems(object[] objects);
}

[ServiceContract()]
public interface IServiceKnownTypeTest_AttrOnMethod_Xml
{
    [OperationContract(Action = "http://tempuri.org/IWcfService/EchoItemsXml", ReplyAction = "http://tempuri.org/IWcfService/EchoItemsXmlResponse")]
    [ServiceKnownType(typeof(Widget2))]
    [XmlSerializerFormat]
    object[] EchoItems_Xml(object[] objects);
}

[ServiceContract()]
[ServiceKnownType(typeof(Widget3))]
public interface IServiceKnownTypeTest_AttrOnType_Xml
{
    [OperationContract(Action = "http://tempuri.org/IWcfService/EchoItemsXml", ReplyAction = "http://tempuri.org/IWcfService/EchoItemsXmlResponse")]
    [XmlSerializerFormat]
    object[] EchoItems_Xml(object[] objects);

    [OperationContract(Action = "http://tempuri.org/IWcfService/EchoItemsXml1", ReplyAction = "http://tempuri.org/IWcfService/EchoItemsXml1Response")]
    [XmlSerializerFormat]
    object[] EchoItems_Xml1(object[] objects);
}

[DataContract()]
public class Widget
{
    [DataMember]
    public string Id;
    [DataMember]
    public string Catalog;
}

[DataContract()]
public class Widget0 : Widget
{
}

[DataContract()]
public class Widget1 : Widget
{
}

[DataContract()]
public class Widget2 : Widget
{
}

[DataContract()]
public class Widget3 : Widget
{
}
