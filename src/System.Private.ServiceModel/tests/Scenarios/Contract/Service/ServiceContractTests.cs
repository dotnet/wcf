// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IO;
using System.ServiceModel;
using System.Text;
using Xunit;

public static class ServiceContractTests
{
    [Fact]
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String_Buffered()
    {
        string testString = "Hello";

        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
        binding.TransferMode = TransferMode.Buffered;
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
        IWcfService serviceProxy = factory.CreateChannel();

        try
        {
            Stream stream = StringToStream(testString);
            var returnStream = serviceProxy.EchoStream(stream);
            var result = StreamToString(returnStream);
            Assert.Equal(testString, result);
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }

    [Fact]
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String_StreamedRequest()
    {
        string testString = "Hello";

        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
        binding.TransferMode = TransferMode.StreamedRequest;
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
        IWcfService serviceProxy = factory.CreateChannel();

        try
        {
            Stream stream = StringToStream(testString);
            var result = serviceProxy.GetStringFromStream(stream);
            Assert.Equal(testString, result);
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }

    [Fact]
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String_StreamedResponse()
    {
        string testString = "Hello";

        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
        binding.TransferMode = TransferMode.StreamedResponse;
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
        IWcfService serviceProxy = factory.CreateChannel();

        try
        {
            var returnStream = serviceProxy.GetStreamFromString(testString);
            var result = StreamToString(returnStream);
            Assert.Equal(testString, result);
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }

    [Fact]
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String_Streamed()
    {
        string testString = "Hello";

        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
        binding.TransferMode = TransferMode.Streamed;
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
        IWcfService serviceProxy = factory.CreateChannel();

        try
        {
            Stream stream = StringToStream(testString);
            var returnStream = serviceProxy.EchoStream(stream);
            var result = StreamToString(returnStream);
            Assert.Equal(testString, result);
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }

    [Fact]
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String_Streamed_Async()
    {
        string testString = "Hello";

        BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
        binding.TransferMode = TransferMode.Streamed;
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
        IWcfService serviceProxy = factory.CreateChannel();

        try
        {
            Stream stream = StringToStream(testString);
            var returnStream = serviceProxy.EchoStreamAsync(stream).Result;
            var result = StreamToString(returnStream);
            Assert.Equal(testString, result);
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }

    private static string StreamToString(Stream stream)
    {
        var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }

    private static Stream StringToStream(string str)
    {
        var ms = new MemoryStream();
        var sw = new StreamWriter(ms, Encoding.UTF8);
        sw.Write(str);
        sw.Flush();
        ms.Position = 0;
        return ms;
    }
}
