// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        StringBuilder errorBuilder = new StringBuilder();

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
        catch (System.ServiceModel.CommunicationException e)
        {
            errorBuilder.AppendLine(string.Format("Unexpected exception thrown: '{0}'", e.ToString()));
            PrintInnerExceptionsHresult(e, errorBuilder);
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: DefaultSettings_Echo_RoundTrips_String_Streamed FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String_Streamed_Async()
    {
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();

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
        catch (System.ServiceModel.CommunicationException e)
        {
            errorBuilder.AppendLine(string.Format("Unexpected exception thrown: '{0}'", e.ToString()));
            PrintInnerExceptionsHresult(e, errorBuilder);
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: DefaultSettings_Echo_RoundTrips_String_Streamed FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String_Streamed_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => ServiceContractTests.DefaultSettings_Echo_RoundTrips_String_Streamed(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: DefaultSettings_Echo_RoundTrips_String_Streamed_WithSingleThreadedSyncContext timed-out.");
    }

    [Fact]
    [OuterLoop]
    public static void DefaultSettings_Echo_RoundTrips_String_Streamed_Async_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => ServiceContractTests.DefaultSettings_Echo_RoundTrips_String_Streamed_Async(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: DefaultSettings_Echo_RoundTrips_String_Streamed_Async_WithSingleThreadedSyncContext timed-out.");
    }

    [Fact]
    [OuterLoop]
    public static void ServiceContract_Call_Operation_With_MessageParameterAttribute()
    {
        // This test verifies the scenario where MessageParameter attribute is used in the contract
        StringBuilder errorBuilder = new StringBuilder();
        ChannelFactory<IWcfServiceGenerated> factory = null;
        IWcfServiceGenerated serviceProxy = null;
        try
        {
            // *** SETUP *** \\
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            // Note the service interface used.  It was manually generated with svcutil.
            factory = new ChannelFactory<IWcfServiceGenerated>(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string echoString = "you";
            string result = serviceProxy.EchoMessageParameter(echoString);
            if (!string.Equals(result, "Hello " + echoString))
            {
                errorBuilder.AppendLine(String.Format("Expected response from Service: {0} Actual was: {1}", "Hello " + echoString, result));
            }

            // *** CLEANUP *** \\
            factory.Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: ServiceContract_Call_Operation_With_MessageParameterAttribute FAILED with the following errors: {0}", errorBuilder));
    }

    private static void PrintInnerExceptionsHresult(Exception e, StringBuilder errorBuilder)
    {
        if (e.InnerException != null)
        {
            errorBuilder.AppendLine(string.Format("\r\n InnerException type: '{0}', Hresult:'{1}'", e.InnerException, e.InnerException.HResult));
            PrintInnerExceptionsHresult(e.InnerException, errorBuilder);
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
