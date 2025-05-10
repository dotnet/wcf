// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Xunit;
using Infrastructure.Common;

public class WebSocketTests : ConditionalWcfTest
{
    [WcfTheory]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [InlineData(NetHttpMessageEncoding.Binary)]
    [InlineData(NetHttpMessageEncoding.Text)]
    [InlineData(NetHttpMessageEncoding.Mtom)]
    [OuterLoop]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    public static void WebSocket_Http_Duplex_Streamed(NetHttpMessageEncoding messageEncoding)
    {
        string endpointAddress;
        NetHttpBinding binding = null;
        ClientReceiver clientReceiver = null;
        InstanceContext context = null;
        DuplexChannelFactory<IWSDuplexService> channelFactory = null;
        IWSDuplexService client = null;
        FlowControlledStream uploadStream = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = TransferMode.Streamed;
            binding.MessageEncoding = messageEncoding;

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);

            endpointAddress = Endpoints.WebSocketHttpDuplexStreamed_Address + Enum.GetName(typeof(NetHttpMessageEncoding), messageEncoding);
            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, endpointAddress);
            client = channelFactory.CreateChannel();

            // *** EXECUTE *** \\
            using (Stream stream = client.DownloadStream())
            {
                int readResult;
                // Read from the stream, 1000 bytes at a time.
                byte[] buffer = new byte[1000];
                do
                {
                    readResult = stream.Read(buffer, 0, buffer.Length);
                }
                while (readResult != 0);
            }

            uploadStream = new FlowControlledStream();
            uploadStream.ReadThrottle = TimeSpan.FromMilliseconds(500);
            uploadStream.StreamDuration = TimeSpan.FromSeconds(1);

            client.UploadStream(uploadStream);
            client.StartPushingStream();
            // Wait for the callback to get invoked before telling the service to stop streaming.
            // This ensures we can read from the stream on the callback while the NCL layer at the service
            // is still writing the bytes from the stream to the wire.
            // This will deadlock if the transfer mode is buffered because the callback will wait for the
            // stream, and the NCL layer will continue to buffer the stream until it reaches the end.

            Assert.True(clientReceiver.ReceiveStreamInvoked.WaitOne(ScenarioTestHelpers.TestTimeout),
                String.Format("Test case timeout was reached while waiting for the stream response from the Service. Timeout was: {0}", ScenarioTestHelpers.TestTimeout));
            clientReceiver.ReceiveStreamInvoked.Reset();

            // Upload the stream while we are downloading a different stream
            uploadStream = new FlowControlledStream();
            uploadStream.ReadThrottle = TimeSpan.FromMilliseconds(500);
            uploadStream.StreamDuration = TimeSpan.FromSeconds(1);
            client.UploadStream(uploadStream);

            client.StopPushingStream();
            // Waiting on ReceiveStreamCompleted from the ClientReceiver.
            Assert.True(clientReceiver.ReceiveStreamCompleted.WaitOne(ScenarioTestHelpers.TestTimeout),
                String.Format("Test case timeout was reached while waiting for the stream response from the Service to be completed. Timeout was: {0}", ScenarioTestHelpers.TestTimeout));
            clientReceiver.ReceiveStreamCompleted.Reset();

            // Getting results from server via callback.
            client.GetLog();
            Assert.True(clientReceiver.LogReceived.WaitOne(ScenarioTestHelpers.TestTimeout),
                String.Format("Test case timeout was reached while waiting for the Logging from the Service to be received. Timeout was: {0}", ScenarioTestHelpers.TestTimeout));

            // *** VALIDATE *** \\
            Assert.True(clientReceiver.ServerLog.Count > 0,
                "The logging done by the Server was not returned via the Callback.");

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            channelFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, channelFactory);
            clientReceiver.Dispose();
        }
    }

    [WcfTheory]
    [InlineData(NetHttpMessageEncoding.Binary)]
    [InlineData(NetHttpMessageEncoding.Text)]
    [InlineData(NetHttpMessageEncoding.Mtom)]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    [OuterLoop]
    public static void WebSocket_Http_Duplex_Buffered(NetHttpMessageEncoding messageEncoding)
    {
        EndpointAddress endpointAddress;
        NetHttpBinding binding = null;
        ClientReceiver clientReceiver = null;
        InstanceContext context = null;
        DuplexChannelFactory<IWSDuplexService> channelFactory = null;
        IWSDuplexService client = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.MessageEncoding = messageEncoding;

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);
            endpointAddress = new EndpointAddress(Endpoints.WebSocketHttpDuplexBuffered_Address + Enum.GetName(typeof(NetHttpMessageEncoding), messageEncoding));
            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, endpointAddress);
            client = channelFactory.CreateChannel();

            // *** EXECUTE *** \\
            // Invoking UploadData
            client.UploadData(ScenarioTestHelpers.CreateInterestingString(123));

            // Invoking StartPushingData
            client.StartPushingData();
            Assert.True(clientReceiver.ReceiveDataInvoked.WaitOne(ScenarioTestHelpers.TestTimeout),
                String.Format("Test case timeout was reached while waiting for the buffered response from the Service. Timeout was: {0}", ScenarioTestHelpers.TestTimeout));
            clientReceiver.ReceiveDataInvoked.Reset();
            // Invoking StopPushingData
            client.StopPushingData();
            Assert.True(clientReceiver.ReceiveDataCompleted.WaitOne(ScenarioTestHelpers.TestTimeout),
                String.Format("Test case timeout was reached while waiting for the buffered response from the Service to be completed. Timeout was: {0}", ScenarioTestHelpers.TestTimeout));
            clientReceiver.ReceiveDataCompleted.Reset();

            // Getting results from server via callback.
            client.GetLog();
            Assert.True(clientReceiver.LogReceived.WaitOne(ScenarioTestHelpers.TestTimeout),
                String.Format("Test case timeout was reached while waiting for the Logging from the Service to be received. Timeout was: {0}", ScenarioTestHelpers.TestTimeout));

            // *** VALIDATE *** \\
            Assert.True(clientReceiver.ServerLog.Count > 0,
                "The logging done by the Server was not returned via the Callback.");

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            channelFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, channelFactory);
        }
    }

    [WcfTheory]
    [InlineData(NetHttpMessageEncoding.Binary)]
    [InlineData(NetHttpMessageEncoding.Text)]
    [InlineData(NetHttpMessageEncoding.Mtom)]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    [OuterLoop]
    public static void WebSocket_Http_Duplex_Buffered_KeepAlive(NetHttpMessageEncoding messageEncoding)
    {
        EndpointAddress endpointAddress;
        NetHttpBinding binding = null;
        ClientReceiver clientReceiver = null;
        InstanceContext context = null;
        DuplexChannelFactory<IWSDuplexService> channelFactory = null;
        IWSDuplexService client = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.MessageEncoding = messageEncoding;
            binding.WebSocketSettings.KeepAliveInterval = TimeSpan.FromSeconds(2);

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);
            endpointAddress = new EndpointAddress(Endpoints.WebSocketHttpDuplexBuffered_Address + Enum.GetName(typeof(NetHttpMessageEncoding), messageEncoding));
            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, endpointAddress);
            client = channelFactory.CreateChannel();

            // *** EXECUTE *** \\
            // Invoking UploadData
            client.UploadData(ScenarioTestHelpers.CreateInterestingString(123));

            // Invoking StartPushingData
            client.StartPushingData();
            Assert.True(clientReceiver.ReceiveDataInvoked.WaitOne(ScenarioTestHelpers.TestTimeout),
                String.Format("Test case timeout was reached while waiting for the buffered response from the Service. Timeout was: {0}", ScenarioTestHelpers.TestTimeout));
            clientReceiver.ReceiveDataInvoked.Reset();
            // Invoking StopPushingData
            client.StopPushingData();
            Assert.True(clientReceiver.ReceiveDataCompleted.WaitOne(ScenarioTestHelpers.TestTimeout),
                String.Format("Test case timeout was reached while waiting for the buffered response from the Service to be completed. Timeout was: {0}", ScenarioTestHelpers.TestTimeout));
            clientReceiver.ReceiveDataCompleted.Reset();

            // Getting results from server via callback.
            client.GetLog();
            Assert.True(clientReceiver.LogReceived.WaitOne(ScenarioTestHelpers.TestTimeout),
                String.Format("Test case timeout was reached while waiting for the Logging from the Service to be received. Timeout was: {0}", ScenarioTestHelpers.TestTimeout));

            // *** VALIDATE *** \\
            Assert.True(clientReceiver.ServerLog.Count > 0,
                "The logging done by the Server was not returned via the Callback.");

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            channelFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\  
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, channelFactory);
        }
    }

    [WcfTheory]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [InlineData(NetHttpMessageEncoding.Binary)]
    [InlineData(NetHttpMessageEncoding.Text)]
    [InlineData(NetHttpMessageEncoding.Mtom)]
    [Issue(3572, OS = OSID.OSX)]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    [OuterLoop]
    public static void WebSocket_Https_Duplex_Streamed(NetHttpMessageEncoding messageEncoding)
    {
        string endpointAddress;
        NetHttpsBinding binding = null;
        ClientReceiver clientReceiver = null;
        InstanceContext context = null;
        DuplexChannelFactory<IWSDuplexService> channelFactory = null;
        IWSDuplexService client = null;
        FlowControlledStream uploadStream = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetHttpsBinding()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = TransferMode.Streamed;
            binding.MessageEncoding = messageEncoding;

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);
            endpointAddress = Endpoints.WebSocketHttpsDuplexStreamed_Address + Enum.GetName(typeof(NetHttpMessageEncoding), messageEncoding);
            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, endpointAddress);
            client = channelFactory.CreateChannel();

            // *** EXECUTE *** \\
            using (Stream stream = client.DownloadStream())
            {
                int readResult;
                // Read from the stream, 1000 bytes at a time.
                byte[] buffer = new byte[1000];
                do
                {
                    readResult = stream.Read(buffer, 0, buffer.Length);
                }
                while (readResult != 0);
            }

            uploadStream = new FlowControlledStream();
            uploadStream.ReadThrottle = TimeSpan.FromMilliseconds(500);
            uploadStream.StreamDuration = TimeSpan.FromSeconds(1);

            client.UploadStream(uploadStream);
            client.StartPushingStream();
            // Wait for the callback to get invoked before telling the service to stop streaming.
            // This ensures we can read from the stream on the callback while the NCL layer at the service
            // is still writing the bytes from the stream to the wire.
            // This will deadlock if the transfer mode is buffered because the callback will wait for the
            // stream, and the NCL layer will continue to buffer the stream until it reaches the end.

            Assert.True(clientReceiver.ReceiveStreamInvoked.WaitOne(ScenarioTestHelpers.TestTimeout),
                String.Format("Test case timeout was reached while waiting for the stream response from the Service. Timeout was: {0}", ScenarioTestHelpers.TestTimeout));
            clientReceiver.ReceiveStreamInvoked.Reset();

            // Upload the stream while we are downloading a different stream
            uploadStream = new FlowControlledStream();
            uploadStream.ReadThrottle = TimeSpan.FromMilliseconds(500);
            uploadStream.StreamDuration = TimeSpan.FromSeconds(1);
            client.UploadStream(uploadStream);

            client.StopPushingStream();
            // Waiting on ReceiveStreamCompleted from the ClientReceiver.
            Assert.True(clientReceiver.ReceiveStreamCompleted.WaitOne(ScenarioTestHelpers.TestTimeout),
                String.Format("Test case timeout was reached while waiting for the stream response from the Service to be completed. Timeout was: {0}", ScenarioTestHelpers.TestTimeout));
            clientReceiver.ReceiveStreamCompleted.Reset();

            // Getting results from server via callback.
            client.GetLog();
            Assert.True(clientReceiver.LogReceived.WaitOne(ScenarioTestHelpers.TestTimeout),
                String.Format("Test case timeout was reached while waiting for the Logging from the Service to be received. Timeout was: {0}", ScenarioTestHelpers.TestTimeout));

            // *** VALIDATE *** \\
            Assert.True(clientReceiver.ServerLog.Count > 0,
                "The logging done by the Server was not returned via the Callback.");

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            channelFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, channelFactory);
            clientReceiver.Dispose();
        }
    }

    [WcfTheory]
    [InlineData(NetHttpMessageEncoding.Binary)]
    [InlineData(NetHttpMessageEncoding.Text)]
    [InlineData(NetHttpMessageEncoding.Mtom)]
    [Condition(nameof(Root_Certificate_Installed),
        nameof(Skip_CoreWCFService_FailedTest))]
    [Issue(3572, OS = OSID.OSX)]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    [OuterLoop]
    public static void WebSocket_Https_Duplex_Buffered(NetHttpMessageEncoding messageEncoding)
    {
        EndpointAddress endpointAddress;
        NetHttpsBinding binding = null;
        ClientReceiver clientReceiver = null;
        InstanceContext context = null;
        DuplexChannelFactory<IWSDuplexService> channelFactory = null;
        IWSDuplexService client = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetHttpsBinding()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = TransferMode.Buffered;
            binding.MessageEncoding = messageEncoding;

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);
            endpointAddress = new EndpointAddress(Endpoints.WebSocketHttpsDuplexBuffered_Address + Enum.GetName(typeof(NetHttpMessageEncoding), messageEncoding));
            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, endpointAddress);
            client = channelFactory.CreateChannel();

            // *** EXECUTE *** \\
            // Invoking UploadData
            client.UploadData(ScenarioTestHelpers.CreateInterestingString(123));

            // Invoking StartPushingData
            client.StartPushingData();
            Assert.True(clientReceiver.ReceiveDataInvoked.WaitOne(ScenarioTestHelpers.TestTimeout),
                String.Format("Test case timeout was reached while waiting for the buffered response from the Service. Timeout was: {0}", ScenarioTestHelpers.TestTimeout));
            clientReceiver.ReceiveDataInvoked.Reset();
            // Invoking StopPushingData
            client.StopPushingData();
            Assert.True(clientReceiver.ReceiveDataCompleted.WaitOne(ScenarioTestHelpers.TestTimeout),
                String.Format("Test case timeout was reached while waiting for the buffered response from the Service to be completed. Timeout was: {0}", ScenarioTestHelpers.TestTimeout));
            clientReceiver.ReceiveDataCompleted.Reset();

            // Getting results from server via callback.
            client.GetLog();
            Assert.True(clientReceiver.LogReceived.WaitOne(ScenarioTestHelpers.TestTimeout),
                String.Format("Test case timeout was reached while waiting for the Logging from the Service to be received. Timeout was: {0}", ScenarioTestHelpers.TestTimeout));

            // *** VALIDATE *** \\
            Assert.True(clientReceiver.ServerLog.Count > 0,
                "The logging done by the Server was not returned via the Callback.");

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            channelFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, channelFactory);
        }
    }

    [WcfTheory]
    [InlineData(NetHttpMessageEncoding.Binary)]
    [InlineData(NetHttpMessageEncoding.Text)]
    [InlineData(NetHttpMessageEncoding.Mtom)]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    [OuterLoop]
    public static void WebSocket_Http_RequestReply_Streamed(NetHttpMessageEncoding messageEncoding)
    {
        EndpointAddress endpointAddress;
        NetHttpBinding binding = null;
        ChannelFactory<IWSRequestReplyService> channelFactory = null;
        IWSRequestReplyService client = null;
        FlowControlledStream uploadStream = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = TransferMode.Streamed;
            binding.MessageEncoding = messageEncoding;
            endpointAddress = new EndpointAddress(Endpoints.WebSocketHttpRequestReplyStreamed_Address + Enum.GetName(typeof(NetHttpMessageEncoding), messageEncoding));
            channelFactory = new ChannelFactory<IWSRequestReplyService>(binding, endpointAddress);
            client = channelFactory.CreateChannel();

            // *** EXECUTE *** \\
            using (Stream stream = client.DownloadStream())
            {
                int readResult;
                // Read from the stream, 1000 bytes at a time.
                byte[] buffer = new byte[1000];

                do
                {
                    readResult = stream.Read(buffer, 0, buffer.Length);
                }
                while (readResult != 0);
            }

            uploadStream = new FlowControlledStream();
            uploadStream.ReadThrottle = TimeSpan.FromMilliseconds(500);
            uploadStream.StreamDuration = TimeSpan.FromSeconds(1);
            client.UploadStream(uploadStream);

            // *** VALIDATE *** \\
            foreach (string serverLogItem in client.GetLog())
            {
                //Assert.True(serverLogItem != ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure, ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure);
                Assert.True(!ScenarioTestHelpers.IsLocalHost() || !serverLogItem.Contains(ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure), serverLogItem);
            }

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            channelFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, channelFactory);
        }
    }

    [WcfTheory]
    [InlineData(NetHttpMessageEncoding.Binary)]
    [InlineData(NetHttpMessageEncoding.Text)]
    [InlineData(NetHttpMessageEncoding.Mtom)]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void WebSocket_Http_RequestReply_Buffered(NetHttpMessageEncoding messageEncoding)
    {
        EndpointAddress endpointAddress;
        NetHttpBinding binding = null;
        ChannelFactory<IWSRequestReplyService> channelFactory = null;
        IWSRequestReplyService client = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = TransferMode.Buffered;
            binding.MessageEncoding = messageEncoding;
            endpointAddress = new EndpointAddress(Endpoints.WebSocketHttpRequestReplyBuffered_Address + Enum.GetName(typeof(NetHttpMessageEncoding), messageEncoding));
            channelFactory = new ChannelFactory<IWSRequestReplyService>(binding, endpointAddress);

            client = channelFactory.CreateChannel();

            // *** EXECUTE *** \\
            // Invoking DownloadData
            string result = client.DownloadData();

            // Invoking UploadData
            client.UploadData(ScenarioTestHelpers.CreateInterestingString(123));

            // *** VALIDATE *** \\
            foreach (string serverLogItem in client.GetLog())
            {
                //Assert.True(serverLogItem != ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure, ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure);
                Assert.True(!ScenarioTestHelpers.IsLocalHost() || !serverLogItem.Contains(ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure), serverLogItem);
            }

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            channelFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, channelFactory);
        }
    }

    [WcfTheory]
    [InlineData(NetHttpMessageEncoding.Binary)]
    [InlineData(NetHttpMessageEncoding.Text)]
    [InlineData(NetHttpMessageEncoding.Mtom)]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    [OuterLoop]
    public static void WebSocket_Http_RequestReply_Buffered_KeepAlive(NetHttpMessageEncoding messageEncoding)
    {
        EndpointAddress endpointAddress;
        NetHttpBinding binding = null;
        ChannelFactory<IWSRequestReplyService> channelFactory = null;
        IWSRequestReplyService client = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.MessageEncoding = messageEncoding;
            binding.WebSocketSettings.KeepAliveInterval = TimeSpan.FromSeconds(2);
            endpointAddress = new EndpointAddress(Endpoints.WebSocketHttpRequestReplyBuffered_Address + Enum.GetName(typeof(NetHttpMessageEncoding), messageEncoding));
            channelFactory = new ChannelFactory<IWSRequestReplyService>(binding, endpointAddress);

            client = channelFactory.CreateChannel();

            // *** EXECUTE *** \\
            // Invoking DownloadData
            string result = client.DownloadData();

            // Invoking UploadData
            client.UploadData(ScenarioTestHelpers.CreateInterestingString(123));

            // *** VALIDATE *** \\
            foreach (string serverLogItem in client.GetLog())
            {
                //Assert.True(serverLogItem != ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure, ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure);
                Assert.True(!ScenarioTestHelpers.IsLocalHost() || !serverLogItem.Contains(ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure), serverLogItem);
            }

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            channelFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\  
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, channelFactory);
        }
    }

    [WcfTheory]
    [InlineData(NetHttpMessageEncoding.Binary)]
    [InlineData(NetHttpMessageEncoding.Text)]
    [InlineData(NetHttpMessageEncoding.Mtom)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Skip_CoreWCFService_FailedTest))]
    [Issue(3572, OS = OSID.OSX)]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    [OuterLoop]
    public static void WebSocket_Https_RequestReply_Buffered(NetHttpMessageEncoding messageEncoding)
    {
        EndpointAddress endpointAddress;
        NetHttpsBinding binding = null;
        ChannelFactory<IWSRequestReplyService> channelFactory = null;
        IWSRequestReplyService client = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetHttpsBinding()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = TransferMode.Buffered;
            binding.MessageEncoding = messageEncoding;
            endpointAddress = new EndpointAddress(Endpoints.WebSocketHttpsRequestReplyBuffered_Address + Enum.GetName(typeof(NetHttpMessageEncoding), messageEncoding));
            channelFactory = new ChannelFactory<IWSRequestReplyService>(binding, endpointAddress);
            client = channelFactory.CreateChannel();

            // *** EXECUTE *** \\
            // Invoking DownloadData
            string result = client.DownloadData();

            // Invoking UploadData
            client.UploadData(ScenarioTestHelpers.CreateInterestingString(123));

            // *** VALIDATE *** \\
            foreach (string serverLogItem in client.GetLog())
            {
                //Assert.True(serverLogItem != ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure, ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure);
                Assert.True(!ScenarioTestHelpers.IsLocalHost() || !serverLogItem.Contains(ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure), serverLogItem);
            }

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            channelFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, channelFactory);
        }
    }

    [WcfTheory]
    [InlineData(NetHttpMessageEncoding.Binary)]
    [InlineData(NetHttpMessageEncoding.Text)]
    [InlineData(NetHttpMessageEncoding.Mtom)]
    [Condition(nameof(Root_Certificate_Installed),
               nameof(Skip_CoreWCFService_FailedTest))]
    [Issue(3572, OS = OSID.OSX)]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    [OuterLoop]
    public static void WebSocket_Https_RequestReply_Buffered_KeepAlive(NetHttpMessageEncoding messageEncoding)
    {
        EndpointAddress endpointAddress;
        NetHttpsBinding binding = null;
        ChannelFactory<IWSRequestReplyService> channelFactory = null;
        IWSRequestReplyService client = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetHttpsBinding()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.WebSocketSettings.KeepAliveInterval = TimeSpan.FromSeconds(2);
            binding.TransferMode = TransferMode.Buffered;
            binding.MessageEncoding = messageEncoding;
            endpointAddress = new EndpointAddress(Endpoints.WebSocketHttpsRequestReplyBuffered_Address + Enum.GetName(typeof(NetHttpMessageEncoding), messageEncoding));
            channelFactory = new ChannelFactory<IWSRequestReplyService>(binding, endpointAddress);

            client = channelFactory.CreateChannel();

            // *** EXECUTE *** \\
            // Invoking DownloadData
            string result = client.DownloadData();

            // Invoking UploadData
            client.UploadData(ScenarioTestHelpers.CreateInterestingString(123));

            // *** VALIDATE *** \\
            foreach (string serverLogItem in client.GetLog())
            {
                //Assert.True(serverLogItem != ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure, ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure);
                Assert.True(!ScenarioTestHelpers.IsLocalHost() || !serverLogItem.Contains(ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure), serverLogItem);
            }

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            channelFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, channelFactory);
        }
    }

    [WcfFact]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    [OuterLoop]
    public static void WebSocket_Http_WSTransportUsageDefault_DuplexCallback_GuidRoundtrip()
    {
        DuplexChannelFactory<IWcfDuplexService> factory = null;
        IWcfDuplexService duplexProxy = null;
        Guid guid = Guid.NewGuid();

        try
        {
            // *** SETUP *** \\  
            NetHttpBinding binding = new NetHttpBinding();

            // NetHttpBinding default value of WebSocketTransportSettings.WebSocketTransportUsage is "WhenDuplex"  
            // Therefore using a Duplex Contract will trigger the use of the WCF implementation of WebSockets.  
            WcfDuplexServiceCallback callbackService = new WcfDuplexServiceCallback();
            InstanceContext context = new InstanceContext(callbackService);

            factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, new EndpointAddress(Endpoints.NetHttpDuplexWebSocket_Address));
            duplexProxy = factory.CreateChannel();

            // *** EXECUTE *** \\  
            Task.Run(() => duplexProxy.Ping(guid));
            Guid returnedGuid = callbackService.CallbackGuid;

            // *** VALIDATE *** \\  
            Assert.True(guid == returnedGuid, string.Format("The sent GUID does not match the returned GUID. Sent '{0}', Received: '{1}'", guid, returnedGuid));

            // *** CLEANUP *** \\  
            ((ICommunicationObject)duplexProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\  
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)duplexProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    [OuterLoop]
    public static void WebSocket_Http_WSTransportUsageAlways_DuplexCallback_GuidRoundtrip()
    {
        DuplexChannelFactory<IWcfDuplexService> factory = null;
        IWcfDuplexService duplexProxy = null;
        Guid guid = Guid.NewGuid();

        try
        {
            // *** SETUP *** \\  
            NetHttpBinding binding = new NetHttpBinding();
            // Verifying the scenario works when explicitly setting the transport to use WebSockets.  
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;

            WcfDuplexServiceCallback callbackService = new WcfDuplexServiceCallback();
            InstanceContext context = new InstanceContext(callbackService);

            factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, new EndpointAddress(Endpoints.NetHttpWebSocketTransport_Address));
            duplexProxy = factory.CreateChannel();

            // *** EXECUTE *** \\  
            Task.Run(() => duplexProxy.Ping(guid));
            Guid returnedGuid = callbackService.CallbackGuid;

            // *** VALIDATE *** \\  
            Assert.True(guid == returnedGuid, string.Format("The sent GUID does not match the returned GUID. Sent '{0}', Received: '{1}'", guid, returnedGuid));

            // *** CLEANUP *** \\  
            factory.Close();
            ((ICommunicationObject)duplexProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\  
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)duplexProxy, factory);
        }
    }

    [WcfFact]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    [OuterLoop]
    public static void WebSocket_WSScheme_WSTransportUsageAlways_DuplexCallback_GuidRoundtrip()
    {
        DuplexChannelFactory<IWcfDuplexService> factory = null;
        IWcfDuplexService proxy = null;
        Guid guid = Guid.NewGuid();

        try
        {
            // *** SETUP *** \\  
            NetHttpBinding binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;

            WcfDuplexServiceCallback callbackService = new WcfDuplexServiceCallback();
            InstanceContext context = new InstanceContext(callbackService);

            UriBuilder builder = new UriBuilder(Endpoints.NetHttpWebSocketTransport_Address);
            // Replacing "http" with "ws" as the uri scheme.  
            builder.Scheme = "ws";

            factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, new EndpointAddress(Endpoints.NetHttpWebSocketTransport_Address));
            proxy = factory.CreateChannel();

            // *** EXECUTE *** \\  
            Task.Run(() => proxy.Ping(guid));
            Guid returnedGuid = callbackService.CallbackGuid;

            // *** VALIDATE *** \\  
            Assert.True(guid == returnedGuid,
                string.Format("The sent GUID does not match the returned GUID. Sent '{0}', Received: '{1}'", guid, returnedGuid));

            // *** CLEANUP *** \\  
            factory.Close();
            ((ICommunicationObject)proxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\  
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)proxy, factory);
        }
    }

    // WCF detects when a callback is used in the channel construction process and switches to use WebSockets.
    // When not using a callback you can still force WCF to use WebSockets.
    // This test verifies that it actually uses WebSockets when not using a callback.
    [WcfFact]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [Issue(1438, OS = OSID.Windows_7)]  // not supported on Win7
    [OuterLoop]
    public static void WebSocket_Http_VerifyWebSocketsUsed()
    {
        NetHttpBinding binding = null;
        ChannelFactory<IVerifyWebSockets> channelFactory = null;
        IVerifyWebSockets client = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;

            channelFactory = new ChannelFactory<IVerifyWebSockets>(binding, new EndpointAddress(Endpoints.WebSocketHttpVerifyWebSocketsUsed_Address));
            client = channelFactory.CreateChannel();

            // *** EXECUTE *** \\
            ((ICommunicationObject)client).Open();

            // *** VALIDATE *** \\
            bool responseFromService = client.ValidateWebSocketsUsed();
            Assert.True(responseFromService, String.Format("Response from the service was not expected. Expected: 'True' but got {0}", responseFromService));

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            channelFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, channelFactory);
        }
    }
}
