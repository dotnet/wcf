// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public static class Contract_XmlSerializer_XmlSerializerFormatTests
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
}
