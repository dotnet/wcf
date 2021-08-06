// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common;
using ScenarioTests.Common;
using Xunit;

public static partial class ServiceContractTests
{
    [WcfFact]
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
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
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

    [WcfFact]
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
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
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

    [WcfFact]
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
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
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

    [WcfFact]
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
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
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

    [WcfFact]
    [OuterLoop]
    public static void BasicHttp_DefaultSettings_Echo_RoundTrips_String_Streamed_Async()
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
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
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

    [WcfFact]
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

    [WcfFact]
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

    [WcfFact]
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
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
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

    [WcfFact]
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
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
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

    [WcfFact]
    [OuterLoop]
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

    [WcfFact]
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

    [WcfFact]
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

    [WcfFact]
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

    [WcfFact]
    [OuterLoop]
    public static void NetTcp_NoSecurity_Streamed_Async_RoundTrips_String()
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

    [WcfFact]
    [OuterLoop]
    public static void NetTcp_NoSecurity_StreamedRequest_Async_RoundTrips_String()
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

    [WcfFact]
    [OuterLoop]
    public static void NetTcp_NoSecurity_StreamedResponse_Async_RoundTrips_String()
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

    [WcfFact]
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

    [WcfFact]
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

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_Call_Operation_With_MessageParameterAttribute()
    {
        // This test verifies the scenario where MessageParameter attribute is used in the contract
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

    [WcfFact]
    [OuterLoop]
    public static void BasicHttp_Abort_ChannelFactory_Operations_Active()
    {
        // Test creates 2 channels from a single channel factory and
        // aborts the channel factory while both channels are executing
        // operations.  This verifies the operations are cancelled and
        // the channel factory is in the correct state.
        BasicHttpBinding binding = null;
        TimeSpan delayOperation = TimeSpan.FromSeconds(3);
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy1 = null;
        IWcfService serviceProxy2 = null;
        string expectedEcho1 = "first";
        string expectedEcho2 = "second";

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.CloseTimeout = ScenarioTestHelpers.TestTimeout;
            binding.SendTimeout = ScenarioTestHelpers.TestTimeout;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            serviceProxy1 = factory.CreateChannel();
            serviceProxy2 = factory.CreateChannel();

            // *** EXECUTE *** \\
            Task<string> t1 = serviceProxy1.EchoWithTimeoutAsync("first", delayOperation);
            Task<string> t2 = serviceProxy2.EchoWithTimeoutAsync("second", delayOperation);
            factory.Abort();

            // *** VALIDATE *** \\
            Assert.True(factory.State == CommunicationState.Closed,
                        String.Format("Expected factory state 'Closed', actual was '{0}'", factory.State));

            Exception exception1 = null;
            Exception exception2 = null;
            string actualEcho1 = null;
            string actualEcho2 = null;

            // Verification is slightly more complex for the close with active operations because
            // we don't know which might have completed first and whether the channel factory
            // was able to close and dispose either channel before it completed.  So we just
            // ensure the Tasks complete with an exception or a successful return and have
            // been closed by the factory.
            try
            {
                actualEcho1 = t1.GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                exception1 = e;
            }

            try
            {
                actualEcho2 = t2.GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                exception2 = e;
            }

            Assert.True(exception1 != null || actualEcho1 != null, "First operation should have thrown Exception or returned an echo");
            Assert.True(exception2 != null || actualEcho2 != null, "Second operation should have thrown Exception or returned an echo");

            Assert.True(actualEcho1 == null || String.Equals(expectedEcho1, actualEcho1),
                        String.Format("First operation returned '{0}' but expected '{1}'.", expectedEcho1, actualEcho1));

            Assert.True(actualEcho2 == null || String.Equals(expectedEcho2, actualEcho2),
            String.Format("Second operation returned '{0}' but expected '{1}'.", expectedEcho2, actualEcho2));

            Assert.True(((ICommunicationObject)serviceProxy1).State == CommunicationState.Closed,
                            String.Format("Expected channel 1 state 'Closed', actual was '{0}'", ((ICommunicationObject)serviceProxy1).State));
            Assert.True(((ICommunicationObject)serviceProxy2).State == CommunicationState.Closed,
                String.Format("Expected channel 2 state 'Closed', actual was '{0}'", ((ICommunicationObject)serviceProxy2).State));

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy1).Abort();
            ((ICommunicationObject)serviceProxy2).Abort();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy1,
                                                          (ICommunicationObject)serviceProxy2, 
                                                          factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void BasicHttp_Close_ChannelFactory_Operations_Active()
    {
        // Test creates 2 channels from a single channel factory and
        // closes the channel factory while both channels are executing
        // operations.  This verifies the operations are cancelled and
        // the channel factory is in the correct state.
        BasicHttpBinding binding = null;
        TimeSpan delayOperation = TimeSpan.FromSeconds(3);
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy1 = null;
        IWcfService serviceProxy2 = null;
        string expectedEcho1 = "first";
        string expectedEcho2 = "second";

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.CloseTimeout = ScenarioTestHelpers.TestTimeout;
            binding.SendTimeout = ScenarioTestHelpers.TestTimeout;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            serviceProxy1 = factory.CreateChannel();
            serviceProxy2 = factory.CreateChannel();

            // *** EXECUTE *** \\
            Task<string> t1 = serviceProxy1.EchoWithTimeoutAsync(expectedEcho1, delayOperation);
            Task<string> t2 = serviceProxy2.EchoWithTimeoutAsync(expectedEcho2, delayOperation);
            factory.Close();

            // *** VALIDATE *** \\
            Assert.True(factory.State == CommunicationState.Closed,
                        String.Format("Expected factory state 'Closed', actual was '{0}'", factory.State));

            Exception exception1 = null;
            Exception exception2 = null;
            string actualEcho1 = null;
            string actualEcho2 = null;

            // Verification is slightly more complex for the close with active operations because
            // we don't know which might have completed first and whether the channel factory
            // was able to close and dispose either channel before it completed.  So we just
            // ensure the Tasks complete with an exception or a successful return and have
            // been closed by the factory.
            try
            {
                actualEcho1 = t1.GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                exception1 = e;
            }

            try
            {
                actualEcho2 = t2.GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                exception2 = e;
            }

            Assert.True(exception1 != null || actualEcho1 != null, "First operation should have thrown Exception or returned an echo");
            Assert.True(exception2 != null || actualEcho2!= null, "Second operation should have thrown Exception or returned an echo");

            Assert.True(actualEcho1 == null || String.Equals(expectedEcho1, actualEcho1),
                        String.Format("First operation returned '{0}' but expected '{1}'.", expectedEcho1, actualEcho1));

            Assert.True(actualEcho2 == null || String.Equals(expectedEcho2, actualEcho2),
            String.Format("Second operation returned '{0}' but expected '{1}'.", expectedEcho2, actualEcho2));

            Assert.True(((ICommunicationObject)serviceProxy1).State == CommunicationState.Closed,
                            String.Format("Expected channel 1 state 'Closed', actual was '{0}'", ((ICommunicationObject)serviceProxy1).State));
            Assert.True(((ICommunicationObject)serviceProxy2).State == CommunicationState.Closed,
                String.Format("Expected channel 2 state 'Closed', actual was '{0}'", ((ICommunicationObject)serviceProxy2).State));

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy1).Abort();
            ((ICommunicationObject)serviceProxy2).Abort();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy1,
                                                          (ICommunicationObject)serviceProxy2,
                                                          factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void BasicHttp_Async_Close_ChannelFactory_Operations_Active()
    {
        // Test creates 2 channels from a single channel factory and
        // asynchronously closes the channel factory while both channels are
        // executing operations.  This verifies the operations are cancelled and
        // the channel factory is in the correct state.
        BasicHttpBinding binding = null;
        TimeSpan delayOperation = TimeSpan.FromSeconds(3);
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy1 = null;
        IWcfService serviceProxy2 = null;
        string expectedEcho1 = "first";
        string expectedEcho2 = "second";

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.CloseTimeout = ScenarioTestHelpers.TestTimeout;
            binding.SendTimeout = ScenarioTestHelpers.TestTimeout;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            serviceProxy1 = factory.CreateChannel();
            serviceProxy2 = factory.CreateChannel();

            // *** EXECUTE *** \\
            Task<string> t1 = serviceProxy1.EchoWithTimeoutAsync(expectedEcho1, delayOperation);
            Task<string> t2 = serviceProxy2.EchoWithTimeoutAsync(expectedEcho2, delayOperation);
            Task factoryTask = Task.Factory.FromAsync(factory.BeginClose, factory.EndClose, TaskCreationOptions.None);

            // *** VALIDATE *** \\
            factoryTask.GetAwaiter().GetResult();
            Assert.True(factory.State == CommunicationState.Closed,
                        String.Format("Expected factory state 'Closed', actual was '{0}'", factory.State));

            Exception exception1 = null;
            Exception exception2 = null;
            string actualEcho1 = null;
            string actualEcho2 = null;

            // Verification is slightly more complex for the close with active operations because
            // we don't know which might have completed first and whether the channel factory
            // was able to close and dispose either channel before it completed.  So we just
            // ensure the Tasks complete with an exception or a successful return and have
            // been closed by the factory.
            try
            {
                actualEcho1 = t1.GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                exception1 = e;
            }

            try
            {
                actualEcho2 = t2.GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                exception2 = e;
            }

            Assert.True(exception1 != null || actualEcho1 != null, "First operation should have thrown Exception or returned an echo");
            Assert.True(exception2 != null || actualEcho2 != null, "Second operation should have thrown Exception or returned an echo");

            Assert.True(actualEcho1 == null || String.Equals(expectedEcho1, actualEcho1),
                        String.Format("First operation returned '{0}' but expected '{1}'.", expectedEcho1, actualEcho1));

            Assert.True(actualEcho2 == null || String.Equals(expectedEcho2, actualEcho2),
            String.Format("Second operation returned '{0}' but expected '{1}'.", expectedEcho2, actualEcho2));

            Assert.True(((ICommunicationObject)serviceProxy1).State == CommunicationState.Closed,
                            String.Format("Expected channel 1 state 'Closed', actual was '{0}'", ((ICommunicationObject)serviceProxy1).State));
            Assert.True(((ICommunicationObject)serviceProxy2).State == CommunicationState.Closed,
                String.Format("Expected channel 2 state 'Closed', actual was '{0}'", ((ICommunicationObject)serviceProxy2).State));

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy1).Abort();
            ((ICommunicationObject)serviceProxy2).Abort();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy1,
                                                          (ICommunicationObject)serviceProxy2,
                                                          factory);
        }
    }
}
