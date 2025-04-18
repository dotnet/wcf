// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public class StreamingTests : ConditionalWcfTest
{
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [OuterLoop]
    public static void NetTcp_TransportSecurity_StreamedRequest_RoundTrips_String()
    {
        string testString = "Hello";
        NetTcpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = binding = new NetTcpBinding(SecurityMode.Transport);
            binding.TransferMode = TransferMode.StreamedRequest;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_Transport_Security_Streamed_Address));
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
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
               
    [OuterLoop]
    public static void NetTcp_TransportSecurity_StreamedResponse_RoundTrips_String()
    {
        string testString = "Hello";
        NetTcpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.Transport);
            binding.TransferMode = TransferMode.StreamedResponse;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_Transport_Security_Streamed_Address));
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
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [OuterLoop]
    public static void NetTcp_TransportSecurity_Streamed_RoundTrips_String()
    {
        string testString = "Hello";
        NetTcpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.Transport);
            binding.TransferMode = TransferMode.Streamed;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_Transport_Security_Streamed_Address));
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
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [OuterLoop]
    public static void NetTcp_TransportSecurity_Streamed_MultipleReads()
    {
        string testString = ScenarioTestHelpers.CreateInterestingString(20001);
        NetTcpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.Transport);
            binding.TransferMode = TransferMode.Streamed;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_Transport_Security_Streamed_Address));
            serviceProxy = factory.CreateChannel();
            stream = StringToStream(testString);

            // *** EXECUTE *** \\
            var returnStream = serviceProxy.EchoStream(stream);
            var ms = new MemoryStream((int)stream.Length);
            var buffer = new byte[10];
            int bytesRead = 0;
            while ((bytesRead = returnStream.ReadAsync(buffer, 0, buffer.Length).Result) != 0)
            {
                ms.Write(buffer, 0, bytesRead);
            }

            ms.Position = 0;
            var result = StreamToString(ms);

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
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [Issue(1888)]
    [OuterLoop]
    public static void NetTcp_TransportSecurity_Streamed_TimeOut_Long_Running_Operation()
    {
        string testString = "Hello";
        NetTcpBinding binding = null;
        TimeSpan serviceOperationTimeout = TimeSpan.FromMilliseconds(10000);
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.Transport);
            binding.TransferMode = TransferMode.Streamed;
            binding.SendTimeout = TimeSpan.FromMilliseconds(5000);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_Transport_Security_Streamed_Address));
            serviceProxy = factory.CreateChannel();
            ((ICommunicationObject)serviceProxy).Open();
            Stopwatch watch = new Stopwatch();
            watch.Start();

            // *** EXECUTE *** \\
            try
            {
                Assert.Throws<TimeoutException>(() =>
                {
                    string returnString = serviceProxy.EchoWithTimeout(testString, serviceOperationTimeout);
                });
            }
            finally
            {
                watch.Stop();

                // *** CLEANUP *** \\
                ((ICommunicationObject)serviceProxy).Close();
                factory.Close();
            }

            // *** VALIDATE *** \\
            // want to assert that this completed in > 5 s as an upper bound since the SendTimeout is 5 sec
            // (usual case is around 5001-5005 ms) 
            Assert.True(watch.ElapsedMilliseconds >= 4985 && watch.ElapsedMilliseconds < 6000,
                        String.Format("Expected timeout was {0}ms but actual was {1}ms",
                                      serviceOperationTimeout.TotalMilliseconds,
                                      watch.ElapsedMilliseconds));
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [OuterLoop]
    public static void NetTcp_TransportSecurity_Streamed_Async_RoundTrips_String()
    {
        string testString = "Hello";
        NetTcpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.Transport);
            binding.TransferMode = TransferMode.Streamed;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_Transport_Security_Streamed_Address));
            serviceProxy = factory.CreateChannel();
            stream = StringToStream(testString);

            // *** EXECUTE *** \\
            var returnStream = serviceProxy.EchoStreamAsync(stream).Result;
            var result = StreamToString(returnStream);

            // *** VALIDATE *** \\
            Assert.True(String.Equals(testString, result), String.Format("Error: Expected test string: '{0}' but got '{1}'", testString, result));

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
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [OuterLoop]
    public static void NetTcp_TransportSecurity_StreamedRequest_Async_RoundTrips_String()
    {
        string testString = "Hello";
        NetTcpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.Transport);
            binding.TransferMode = TransferMode.StreamedRequest;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_Transport_Security_Streamed_Address));
            serviceProxy = factory.CreateChannel();
            stream = StringToStream(testString);

            // *** EXECUTE *** \\
            var returnStream = serviceProxy.EchoStreamAsync(stream).Result;
            var result = StreamToString(returnStream);

            // *** VALIDATE *** \\
            Assert.True(String.Equals(testString, result), String.Format("Error: Expected test string: '{0}' but got '{1}'", testString, result));

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
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [OuterLoop]
    public static void NetTcp_TransportSecurity_StreamedResponse_Async_RoundTrips_String()
    {
        string testString = "Hello";
        NetTcpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.Transport);
            binding.TransferMode = TransferMode.StreamedResponse;
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.Tcp_Transport_Security_Streamed_Address));
            serviceProxy = factory.CreateChannel();
            stream = StringToStream(testString);

            // *** EXECUTE *** \\
            var returnStream = serviceProxy.EchoStreamAsync(stream).Result;
            var result = StreamToString(returnStream);

            // *** VALIDATE *** \\
            Assert.True(String.Equals(testString, result), String.Format("Error: Expected test string: '{0}' but got '{1}'", testString, result));

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
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [OuterLoop]
    public static void NetTcp_TransportSecurity_Streamed_RoundTrips_String_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => StreamingTests.NetTcp_TransportSecurity_Streamed_RoundTrips_String(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: NetTcp_TransportSecurity_String_Streamed_RoundTrips_WithSingleThreadedSyncContext timed-out.");
    }

    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [OuterLoop]
    public static void NetTcp_TransportSecurity_Streamed_Async_RoundTrips_String_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => StreamingTests.NetTcp_TransportSecurity_Streamed_Async_RoundTrips_String(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: NetTcp_TransportSecurity_Streamed_Async_RoundTrips_String_WithSingleThreadedSyncContext timed-out.");
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
