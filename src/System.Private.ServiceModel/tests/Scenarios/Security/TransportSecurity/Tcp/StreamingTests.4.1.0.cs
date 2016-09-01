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
#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(832)] // Windows Stream Security is not supported in NET Native
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [Issue(832, Framework = FrameworkID.NetNative)] // Windows Stream Security is not supported in NET Native
    [OuterLoop]
    public static void NetTcp_TransportSecurity_StreamedRequest_RoundTrips_String()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        bool client_Certificate_Installed = Client_Certificate_Installed();
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool ambient_Credentials_Available = Ambient_Credentials_Available();

        if (!root_Certificate_Installed || 
            !client_Certificate_Installed || 
            !windows_Authentication_Available ||
            !ambient_Credentials_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);
            Console.WriteLine("Ambient_Credentials_Available evaluated as {0}", ambient_Credentials_Available);
            return;
        }
#endif
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(832)] // Windows Stream Security is not supported in NET Native
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [Issue(832, Framework = FrameworkID.NetNative)] // Windows Stream Security is not supported in NET Native
    [OuterLoop]
    public static void NetTcp_TransportSecurity_StreamedResponse_RoundTrips_String()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        bool client_Certificate_Installed = Client_Certificate_Installed();
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool ambient_Credentials_Available = Ambient_Credentials_Available();

        if (!root_Certificate_Installed ||
            !client_Certificate_Installed ||
            !windows_Authentication_Available ||
            !ambient_Credentials_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);
            Console.WriteLine("Ambient_Credentials_Available evaluated as {0}", ambient_Credentials_Available);
            return;
        }
#endif
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(832)] // Windows Stream Security is not supported in NET Native
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [Issue(832, Framework = FrameworkID.NetNative)] // Windows Stream Security is not supported in NET Native
    [OuterLoop]
    public static void NetTcp_TransportSecurity_Streamed_RoundTrips_String()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        bool client_Certificate_Installed = Client_Certificate_Installed();
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool ambient_Credentials_Available = Ambient_Credentials_Available();

        if (!root_Certificate_Installed ||
            !client_Certificate_Installed ||
            !windows_Authentication_Available ||
            !ambient_Credentials_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);
            Console.WriteLine("Ambient_Credentials_Available evaluated as {0}", ambient_Credentials_Available);
            return;
        }
#endif
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(832)] // Windows Stream Security is not supported in NET Native
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [Issue(832, Framework = FrameworkID.NetNative)] // Windows Stream Security is not supported in NET Native
    [OuterLoop]
    public static void NetTcp_TransportSecurity_Streamed_TimeOut_Long_Running_Operation()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        bool client_Certificate_Installed = Client_Certificate_Installed();
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool ambient_Credentials_Available = Ambient_Credentials_Available();

        if (!root_Certificate_Installed ||
            !client_Certificate_Installed ||
            !windows_Authentication_Available ||
            !ambient_Credentials_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);
            Console.WriteLine("Ambient_Credentials_Available evaluated as {0}", ambient_Credentials_Available);
            return;
        }
#endif
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(832)] // Windows Stream Security is not supported in NET Native
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [Issue(832, Framework = FrameworkID.NetNative)] // Windows Stream Security is not supported in NET Native
    [OuterLoop]
    public static void NetTcp_TransportSecurity_Streamed_Async_RoundTrips_String()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        bool client_Certificate_Installed = Client_Certificate_Installed();
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool ambient_Credentials_Available = Ambient_Credentials_Available();

        if (!root_Certificate_Installed ||
            !client_Certificate_Installed ||
            !windows_Authentication_Available ||
            !ambient_Credentials_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);
            Console.WriteLine("Ambient_Credentials_Available evaluated as {0}", ambient_Credentials_Available);
            return;
        }
#endif
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(832)] // Windows Stream Security is not supported in NET Native
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [Issue(832, Framework = FrameworkID.NetNative)] // Windows Stream Security is not supported in NET Native
    [OuterLoop]
    public static void NetTcp_TransportSecurity_StreamedRequest_Async_RoundTrips_String()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        bool client_Certificate_Installed = Client_Certificate_Installed();
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool ambient_Credentials_Available = Ambient_Credentials_Available();

        if (!root_Certificate_Installed ||
            !client_Certificate_Installed ||
            !windows_Authentication_Available ||
            !ambient_Credentials_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);
            Console.WriteLine("Ambient_Credentials_Available evaluated as {0}", ambient_Credentials_Available);
            return;
        }
#endif
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(832)] // Windows Stream Security is not supported in NET Native
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [Issue(832, Framework = FrameworkID.NetNative)] // Windows Stream Security is not supported in NET Native
    [OuterLoop]
    public static void NetTcp_TransportSecurity_StreamedResponse_Async_RoundTrips_String()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        bool client_Certificate_Installed = Client_Certificate_Installed();
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool ambient_Credentials_Available = Ambient_Credentials_Available();

        if (!root_Certificate_Installed ||
            !client_Certificate_Installed ||
            !windows_Authentication_Available ||
            !ambient_Credentials_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);
            Console.WriteLine("Ambient_Credentials_Available evaluated as {0}", ambient_Credentials_Available);
            return;
        }
#endif
        string testString = "Hello";
        StringBuilder errorBuilder = new StringBuilder();
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

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(832)] // Windows Stream Security is not supported in NET Native
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [Issue(832, Framework = FrameworkID.NetNative)] // Windows Stream Security is not supported in NET Native
    [OuterLoop]
    public static void NetTcp_TransportSecurity_Streamed_RoundTrips_String_WithSingleThreadedSyncContext()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        bool client_Certificate_Installed = Client_Certificate_Installed();
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool ambient_Credentials_Available = Ambient_Credentials_Available();

        if (!root_Certificate_Installed ||
            !client_Certificate_Installed ||
            !windows_Authentication_Available ||
            !ambient_Credentials_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);
            Console.WriteLine("Ambient_Credentials_Available evaluated as {0}", ambient_Credentials_Available);
            return;
        }
#endif
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => StreamingTests.NetTcp_TransportSecurity_Streamed_RoundTrips_String(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: NetTcp_TransportSecurity_String_Streamed_RoundTrips_WithSingleThreadedSyncContext timed-out.");
    }

#if FULLXUNIT_NOTSUPPORTED
    [Fact]
    [ActiveIssue(832)] // Windows Stream Security is not supported in NET Native
#endif
    [WcfFact]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Client_Certificate_Installed),
               nameof(Windows_Authentication_Available),
               nameof(Ambient_Credentials_Available))]
    [Issue(832, Framework = FrameworkID.NetNative)] // Windows Stream Security is not supported in NET Native
    [OuterLoop]
    public static void NetTcp_TransportSecurity_Streamed_Async_RoundTrips_String_WithSingleThreadedSyncContext()
    {
#if FULLXUNIT_NOTSUPPORTED
        bool root_Certificate_Installed = Root_Certificate_Installed();
        bool client_Certificate_Installed = Client_Certificate_Installed();
        bool windows_Authentication_Available = Windows_Authentication_Available();
        bool ambient_Credentials_Available = Ambient_Credentials_Available();

        if (!root_Certificate_Installed ||
            !client_Certificate_Installed ||
            !windows_Authentication_Available ||
            !ambient_Credentials_Available)
        {
            Console.WriteLine("---- Test SKIPPED --------------");
            Console.WriteLine("Attempting to run the test in ToF, a ConditionalFact evaluated as FALSE.");
            Console.WriteLine("Root_Certificate_Installed evaluated as {0}", root_Certificate_Installed);
            Console.WriteLine("Client_Certificate_Installed evaluated as {0}", client_Certificate_Installed);
            Console.WriteLine("Windows_Authentication_Available evaluated as {0}", windows_Authentication_Available);
            Console.WriteLine("Ambient_Credentials_Available evaluated as {0}", ambient_Credentials_Available);
            return;
        }
#endif
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => StreamingTests.NetTcp_TransportSecurity_Streamed_Async_RoundTrips_String(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: NetTcp_TransportSecurity_Streamed_Async_RoundTrips_String_WithSingleThreadedSyncContext timed-out.");
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
