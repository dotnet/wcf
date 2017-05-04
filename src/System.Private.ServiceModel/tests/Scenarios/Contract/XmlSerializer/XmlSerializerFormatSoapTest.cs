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
    private static readonly string s_basicSoapEndpointAddress = Endpoints.HttpBaseAddress_Basic_Soap;

    [WcfFact]
    [OuterLoop]
    public static void CombineString_XmlSerializerFormat_Soap()
    {
        var binding = new BasicHttpBinding();
        var endpointAddress = new EndpointAddress(s_basicSoapEndpointAddress);
        var factory = new ChannelFactory<IWcfSoapService>(binding, endpointAddress);
        IWcfSoapService serviceProxy = factory.CreateChannel();

        string message1 = "hello";
        string message2 = "world";
        var response = serviceProxy.CombineStringXmlSerializerFormatSoap(message1, message2);
        Assert.Equal(message1 + message2, response);
    }

    [WcfFact]
    [OuterLoop]
    [ActiveIssue(1884)]
    public static void EchoComositeType_XmlSerializerFormat_Soap()
    {
        var binding = new BasicHttpBinding();
        var endpointAddress = new EndpointAddress(s_basicSoapEndpointAddress);
        var factory = new ChannelFactory<IWcfSoapService>(binding, endpointAddress);
        IWcfSoapService serviceProxy = factory.CreateChannel();

        var value = new SoapComplexType() { BoolValue = true, StringValue = "hello" };
        SoapComplexType response = serviceProxy.EchoComositeTypeXmlSerializerFormatSoap(value);
        Assert.NotNull(response);
        Assert.Equal(value.BoolValue, response.BoolValue);
        Assert.Equal(value.StringValue, response.StringValue);
    }

    [WcfFact]
    [OuterLoop]
    public static void ProcessCustomerData_XmlSerializerFormat_Soap()
    {
        var binding = new BasicHttpBinding();
        var endpointAddress = new EndpointAddress(s_basicSoapEndpointAddress);
        var factory = new ChannelFactory<IWcfSoapService>(binding, endpointAddress);
        IWcfSoapService serviceProxy = factory.CreateChannel();

        CustomerObject value = new CustomerObject() { Name = "MyName", Data = new AdditionalData() { Field = "Foo" } };
        string response = serviceProxy.ProcessCustomerData(value);

        Assert.Equal("MyNameFoo", response);
    }
}
