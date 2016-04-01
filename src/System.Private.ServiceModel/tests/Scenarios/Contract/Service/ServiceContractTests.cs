// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ScenarioTests.Common;
using Xunit;


public static class ServiceContractTests
{
    [Fact]
    [OuterLoop]
    public static void BasicHttp_DefaultSettings_Echo_RoundTrips_String_Buffered()
    {
        string testString = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.TransferMode = TransferMode.Buffered;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            serviceProxy = factory.CreateChannel();
            stream = StringToStream(testString);

            // *** EXECUTE *** \\
            var returnStream = serviceProxy.EchoStream(stream);
            var result = StreamToString(returnStream);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

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
    public static void BasicHttp_DefaultSettings_Echo_RoundTrips_String_StreamedRequest()
    {
        string testString = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.TransferMode = TransferMode.StreamedRequest;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            serviceProxy = factory.CreateChannel();
            stream = StringToStream(testString);

            // *** EXECUTE *** \\
            var result = serviceProxy.GetStringFromStream(stream);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

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
    public static void BasicHttp_DefaultSettings_Echo_RoundTrips_String_StreamedResponse()
    {
        string testString = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.TransferMode = TransferMode.StreamedResponse;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            var returnStream = serviceProxy.GetStreamFromString(testString);
            var result = StreamToString(returnStream);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

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
    public static void BasicHttp_DefaultSettings_Echo_RoundTrips_String_Streamed()
    {
        string testString = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.TransferMode = TransferMode.Streamed;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            serviceProxy = factory.CreateChannel();
            stream = StringToStream(testString);

            // *** EXECUTE *** \\
            var returnStream = serviceProxy.EchoStream(stream);
            var result = StreamToString(returnStream);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

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
    public static void BasicHttp_DefaultSettings_Echo_RoundTrips_String_Streamed_Async()
    {
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();
        BasicHttpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.TransferMode = TransferMode.Streamed;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            serviceProxy = factory.CreateChannel();
            stream = StringToStream(testString);

            // *** EXECUTE *** \\
            var returnStream = serviceProxy.EchoStreamAsync(stream).Result;
            var result = StreamToString(returnStream);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

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
    public static void BasicHttp_DefaultSettings_Echo_RoundTrips_String_Streamed_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => ServiceContractTests.BasicHttp_DefaultSettings_Echo_RoundTrips_String_Streamed(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: BasicHttp_DefaultSettings_Echo_RoundTrips_String_Streamed_WithSingleThreadedSyncContext timed-out.");
    }

    [Fact]
    [OuterLoop]
    public static void BasicHttp_DefaultSettings_Echo_RoundTrips_String_Streamed_Async_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => ServiceContractTests.BasicHttp_DefaultSettings_Echo_RoundTrips_String_Streamed_Async(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: BasicHttp_DefaultSettings_Echo_RoundTrips_String_Streamed_Async_WithSingleThreadedSyncContext timed-out.");
    }


    [Fact]
    [OuterLoop]
    public static void BasicHttp_Streamed_Async_Delayed_And_Aborted_Request_Throws_TimeoutException()
    {
        // This test is a regression test that verifies an issue discovered where exceeding the timeout
        // and aborting the channel before the client's Task completed led to incorrect error handling.
        BasicHttpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;
        int sendTimeoutMs = 3000;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.TransferMode = TransferMode.Streamed;
            binding.SendTimeout = TimeSpan.FromMilliseconds(sendTimeoutMs);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            serviceProxy = factory.CreateChannel();

            // Create a read stream that will both timeout and then abort the proxy channel when the
            // async read is called. We also intercept the synchronous read because that path can also
            // be executed during an async read.
            stream = new TestMockStream()
            {
                CopyToAsyncFunc = (Stream destination, int bufferSize, CancellationToken ct) =>
                {
                    // Abort to force the internal HttpClientChannelAsyncRequest.Cleanup()
                    // to clear its data structures before the client's Task completes.
                    Task.Delay(sendTimeoutMs * 2).Wait();
                    ((ICommunicationObject)serviceProxy).Abort();
                    return null;
                },

                ReadFunc = (byte[] buffer, int offset, int count) =>
                {
                    // Abort to force the internal HttpClientChannelAsyncRequest.Cleanup()
                    // to clear its data structures before the client's Task completes.
                    Task.Delay(sendTimeoutMs * 2).Wait();
                    ((ICommunicationObject)serviceProxy).Abort();
                    return -1;
                }
            };

            // *** EXECUTE *** \\
            Assert.Throws<TimeoutException>(() =>
            {
                var unused = serviceProxy.EchoStreamAsync(stream).GetAwaiter().GetResult();
            });

            // *** VALIDATE *** \\

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
    public static void BasicHttp_Streamed_Async_Delayed_Request_Throws_TimeoutException()
    {
        BasicHttpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;
        int sendTimeoutMs = 3000;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.TransferMode = TransferMode.Streamed;
            binding.SendTimeout = TimeSpan.FromMilliseconds(sendTimeoutMs);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
            serviceProxy = factory.CreateChannel();

            // Create a read stream that deliberately times out during the async read during the request.
            // We also intercept the synchronous read because that path can also be executed during
            // an async read.
            stream = new TestMockStream()
            {
                CopyToAsyncFunc = (Stream destination, int bufferSize, CancellationToken ct) =>
                {
                    Task.Delay(sendTimeoutMs * 2).Wait();
                    return null;
                },

                ReadFunc = (byte[] buffer, int offset, int count) =>
                {
                    Task.Delay(sendTimeoutMs * 2).Wait();
                    return -1;
                }
            };

            // *** EXECUTE *** \\
            Assert.Throws<TimeoutException>(() =>
            {
                var unused = serviceProxy.EchoStreamAsync(stream).GetAwaiter().GetResult();
            });

            // *** VALIDATE *** \\

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
    [ActiveIssue(951, PlatformID.OSX)]
    public static void NetTcp_NoSecurity_Buffered_RoundTrips_String()
    {
        string testString = "Hello";
        NetTcpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.None);
            binding.TransferMode = TransferMode.Buffered;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_Address));
            serviceProxy = factory.CreateChannel();
            stream = StringToStream(testString);

            // *** EXECUTE *** \\
            var returnStream = serviceProxy.EchoStream(stream);
            var result = StreamToString(returnStream);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

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
    public static void NetTcp_NoSecurity_StreamedRequest_RoundTrips_String()
    {
        string testString = "Hello";
        NetTcpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = binding = new NetTcpBinding(SecurityMode.None);
            binding.TransferMode = TransferMode.StreamedRequest;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_Streamed_NoSecurity_Address));
            serviceProxy = factory.CreateChannel();
            stream = StringToStream(testString);

            // *** EXECUTE *** \\
            var result = serviceProxy.GetStringFromStream(stream);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

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
    public static void NetTcp_NoSecurity_StreamedResponse_RoundTrips_String()
    {
        string testString = "Hello";
        NetTcpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.None);
            binding.TransferMode = TransferMode.StreamedResponse;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_Streamed_NoSecurity_Address));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            var returnStream = serviceProxy.GetStreamFromString(testString);
            var result = StreamToString(returnStream);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

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
    public static void NetTcp_NoSecurity_Streamed_RoundTrips_String()
    {
        string testString = "Hello";
        NetTcpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.None);
            binding.TransferMode = TransferMode.Streamed;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_Streamed_NoSecurity_Address));
            serviceProxy = factory.CreateChannel();
            stream = StringToStream(testString);

            // *** EXECUTE *** \\
            var returnStream = serviceProxy.EchoStream(stream);
            var result = StreamToString(returnStream);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

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
    public static void NetTcp_NoSecurity_Streamed_Async_RoundTrips_String()
    {
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();
        NetTcpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.None);
            binding.TransferMode = TransferMode.Streamed;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_Streamed_NoSecurity_Address));
            serviceProxy = factory.CreateChannel();
            stream = StringToStream(testString);

            // *** EXECUTE *** \\
            var returnStream = serviceProxy.EchoStreamAsync(stream).Result;
            var result = StreamToString(returnStream);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

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
    public static void NetTcp_NoSecurity_StreamedRequest_Async_RoundTrips_String()
    {
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();
        NetTcpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.None);
            binding.TransferMode = TransferMode.StreamedRequest;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_Streamed_NoSecurity_Address));
            serviceProxy = factory.CreateChannel();
            stream = StringToStream(testString);

            // *** EXECUTE *** \\
            var returnStream = serviceProxy.EchoStreamAsync(stream).Result;
            var result = StreamToString(returnStream);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

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
    public static void NetTcp_NoSecurity_StreamedResponse_Async_RoundTrips_String()
    {
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();
        NetTcpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.None);
            binding.TransferMode = TransferMode.StreamedResponse;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_Streamed_NoSecurity_Address));
            serviceProxy = factory.CreateChannel();
            stream = StringToStream(testString);

            // *** EXECUTE *** \\
            var returnStream = serviceProxy.EchoStreamAsync(stream).Result;
            var result = StreamToString(returnStream);

            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

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
    public static void NetTcp_NoSecurity_Streamed_RoundTrips_String_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => ServiceContractTests.NetTcp_NoSecurity_Streamed_RoundTrips_String(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: NetTcp_NoSecurity_String_Streamed_RoundTrips_WithSingleThreadedSyncContext timed-out.");
    }

    [Fact]
    [OuterLoop]
    public static void NetTcp_NoSecurity_Streamed_Async_RoundTrips_String_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => ServiceContractTests.NetTcp_NoSecurity_Streamed_Async_RoundTrips_String(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: NetTcp_NoSecurity_Streamed_Async_RoundTrips_String_WithSingleThreadedSyncContext timed-out.");
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

            // *** VALIDATE *** \\
            Assert.True(string.Equals(result, "Hello " + echoString), String.Format("Expected response from Service: {0} Actual was: {1}", "Hello " + echoString, result));

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

    // End operation includes keyword "out" on an Int as an arg.
    [Fact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncEndOperation_IntOutArg()
    {
        string message = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IServiceContractIntOutService> factory = null;
        IServiceContractIntOutService serviceProxy = null;
        int number = 0;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            factory = new ChannelFactory<IServiceContractIntOutService>(binding, new EndpointAddress(Endpoints.ServiceContractAsyncIntOut_Address));
            serviceProxy = factory.CreateChannel();

            ManualResetEvent waitEvent = new ManualResetEvent(false);

            // *** EXECUTE *** \\
            // This delegate will execute when the call has completed, which is how we get the result of the call.
            AsyncCallback callback = (iar) =>
            {
                serviceProxy.EndRequest(out number, iar);
                waitEvent.Set();
            };

            IAsyncResult ar = serviceProxy.BeginRequest(message, callback, null);

            // *** VALIDATE *** \\
            Assert.True(waitEvent.WaitOne(ScenarioTestHelpers.TestTimeout), "AsyncCallback was not called.");
            Assert.True((number == message.Count<char>()), String.Format("The local int variable was not correctly set, expected {0} but got {1}", message.Count<char>(), number));

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

    // End operation includes keyword "out" on a unique type as an arg.
    // The unique type used must not appear anywhere else in any contracts in order
    // test the static analysis logic of the Net Native toolchain.
    [Fact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncEndOperation_UniqueTypeOutArg()
    {
        string message = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IServiceContractUniqueTypeOutService> factory = null;
        IServiceContractUniqueTypeOutService serviceProxy = null;
        UniqueType uniqueType = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            factory = new ChannelFactory<IServiceContractUniqueTypeOutService>(binding, new EndpointAddress(Endpoints.ServiceContractAsyncUniqueTypeOut_Address));
            serviceProxy = factory.CreateChannel();

            ManualResetEvent waitEvent = new ManualResetEvent(false);

            // *** EXECUTE *** \\
            // This delegate will execute when the call has completed, which is how we get the result of the call.
            AsyncCallback callback = (iar) =>
            {
                serviceProxy.EndRequest(out uniqueType, iar);
                waitEvent.Set();
            };

            IAsyncResult ar = serviceProxy.BeginRequest(message, callback, null);

            // *** VALIDATE *** \\
            Assert.True(waitEvent.WaitOne(ScenarioTestHelpers.TestTimeout), "AsyncCallback was not called.");
            Assert.True((uniqueType.stringValue == message),
                String.Format("The 'stringValue' field in the instance of 'UniqueType' was not as expected. expected {0} but got {1}", message, uniqueType.stringValue));

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

    // End & Begin operations include keyword "ref" on an Int as an arg.
    [Fact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncEndOperation_IntRefArg()
    {
        string message = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IServiceContractIntRefService> factory = null;
        IServiceContractIntRefService serviceProxy = null;
        int number = 0;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            factory = new ChannelFactory<IServiceContractIntRefService>(binding, new EndpointAddress(Endpoints.ServiceContractAsyncIntRef_Address));
            serviceProxy = factory.CreateChannel();

            ManualResetEvent waitEvent = new ManualResetEvent(false);

            // *** EXECUTE *** \\
            // This delegate will execute when the call has completed, which is how we get the result of the call.
            AsyncCallback callback = (iar) =>
            {
                serviceProxy.EndRequest(ref number, iar);
                waitEvent.Set();
            };

            IAsyncResult ar = serviceProxy.BeginRequest(message, ref number, callback, null);

            // *** VALIDATE *** \\
            Assert.True(waitEvent.WaitOne(ScenarioTestHelpers.TestTimeout), "AsyncCallback was not called.");
            Assert.True((number == message.Count<char>()),
                String.Format("The value of the integer sent by reference was not the expected value. expected {0} but got {1}", message.Count<char>(), number));

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

    // End & Begin operations include keyword "ref" on a unique type as an arg.
    // The unique type used must not appear anywhere else in any contracts in order
    // test the static analysis logic of the Net Native toolchain.
    [Fact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncEndOperation_UniqueTypeRefArg()
    {
        string message = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IServiceContractUniqueTypeRefService> factory = null;
        IServiceContractUniqueTypeRefService serviceProxy = null;
        UniqueType uniqueType = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            factory = new ChannelFactory<IServiceContractUniqueTypeRefService>(binding, new EndpointAddress(Endpoints.ServiceContractAsyncUniqueTypeRef_Address));
            serviceProxy = factory.CreateChannel();

            ManualResetEvent waitEvent = new ManualResetEvent(false);

            // *** EXECUTE *** \\
            // This delegate will execute when the call has completed, which is how we get the result of the call.
            AsyncCallback callback = (iar) =>
            {
                serviceProxy.EndRequest(ref uniqueType, iar);
                waitEvent.Set();
            };

            IAsyncResult ar = serviceProxy.BeginRequest(message, ref uniqueType, callback, null);

            // *** VALIDATE *** \\
            Assert.True(waitEvent.WaitOne(ScenarioTestHelpers.TestTimeout), "AsyncCallback was not called.");
            Assert.True((uniqueType.stringValue == message),
                String.Format("The 'stringValue' field in the instance of 'UniqueType' was not as expected. expected {0} but got {1}", message, uniqueType.stringValue));

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

    // Synchronous operation using the keyword "out" on a unique type as an arg.
    // The unique type used must not appear anywhere else in any contracts in order
    // test the static analysis logic of the Net Native toolchain.
    [Fact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_SyncOperation_UniqueTypeOutArg()
    {
        string message = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IServiceContractUniqueTypeOutSyncService> factory = null;
        IServiceContractUniqueTypeOutSyncService serviceProxy = null;
        UniqueType uniqueType = null;

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            factory = new ChannelFactory<IServiceContractUniqueTypeOutSyncService>(binding, new EndpointAddress(Endpoints.ServiceContractSyncUniqueTypeOut_Address));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            serviceProxy.Request(message, out uniqueType);

            // *** VALIDATE *** \\
            Assert.True((uniqueType.stringValue == message),
                String.Format("The value of the 'stringValue' field in the UniqueType instance was not as expected. expected {0} but got {1}", message, uniqueType.stringValue));

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

    // Synchronous operation using the keyword "ref" on a unique type as an arg.
    // The unique type used must not appear anywhere else in any contracts in order
    // test the static analysis logic of the Net Native toolchain.
    [Fact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_SyncOperation_UniqueTypeRefArg()
    {
        string message = "Hello";
        BasicHttpBinding binding = null;
        ChannelFactory<IServiceContractUniqueTypeRefSyncService> factory = null;
        IServiceContractUniqueTypeRefSyncService serviceProxy = null;
        UniqueType uniqueType = new UniqueType();

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            factory = new ChannelFactory<IServiceContractUniqueTypeRefSyncService>(binding, new EndpointAddress(Endpoints.ServiceContractSyncUniqueTypeRef_Address));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            serviceProxy.Request(message, ref uniqueType);

            // *** VALIDATE *** \\
            Assert.True((uniqueType.stringValue == message),
                String.Format("The value of the 'stringValue' field in the UniqueType instance was not as expected. expected {0} but got {1}", message, uniqueType.stringValue));

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
