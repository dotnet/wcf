// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Xunit;

public static class WebSocketTests
{
    [Fact]
    [OuterLoop]
    [ActiveIssue(468)]
    public static void WebSocket_Http_RequestReply_BinaryStreamed()
    {
        NetHttpBinding binding = null;
        ChannelFactory<IWSRequestReplyService> channelFactory = null;
        IWSRequestReplyService client = null;
        FlowControlledStream uploadStream = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.MaxReceivedMessageSize = ScenarioTestHelpers.DefaultMaxReceivedMessageSize;
            binding.MaxBufferSize = ScenarioTestHelpers.DefaultMaxReceivedMessageSize;
            binding.TransferMode = TransferMode.Streamed;
            binding.MessageEncoding = NetHttpMessageEncoding.Binary;

            channelFactory = new ChannelFactory<IWSRequestReplyService>(binding, new EndpointAddress(Endpoints.WebSocketHttpRequestReplyBinaryStreamed_Address));
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
                Assert.True(serverLogItem != ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure, ScenarioTestHelpers.RemoteEndpointMessagePropertyFailure);
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

    [Fact]
    [OuterLoop]
    [ActiveIssue(468)]
    public static void WebSocket_Http_Duplex_BinaryStreamed()
    {
        NetHttpBinding binding = null;
        ClientReceiver clientReceiver = null;
        InstanceContext context = null;
        DuplexChannelFactory<IWSDuplexService> channelFactory = null;
        IWSDuplexService client = null;
        FlowControlledStream uploadStream = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.MaxReceivedMessageSize = ScenarioTestHelpers.DefaultMaxReceivedMessageSize;
            binding.MaxBufferSize = ScenarioTestHelpers.DefaultMaxReceivedMessageSize;
            binding.TransferMode = TransferMode.Streamed;
            binding.MessageEncoding = NetHttpMessageEncoding.Binary;

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);

            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, Endpoints.WebSocketHttpDuplexBinaryStreamed_Address);
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

            clientReceiver.ReceiveStreamInvoked.WaitOne();
            clientReceiver.ReceiveStreamInvoked.Reset();

            // Upload the stream while we are downloading a different stream
            uploadStream = new FlowControlledStream();
            uploadStream.ReadThrottle = TimeSpan.FromMilliseconds(500);
            uploadStream.StreamDuration = TimeSpan.FromSeconds(1);
            client.UploadStream(uploadStream);

            client.StopPushingStream();
            // Waiting on ReceiveStreamCompleted from the ClientReceiver.
            clientReceiver.ReceiveStreamCompleted.WaitOne();
            clientReceiver.ReceiveStreamCompleted.Reset();

            // *** VALIDATE *** \\
            // Validation is based on no exceptions being thrown.

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

    [Fact]
    [OuterLoop]
    [ActiveIssue(470)]
    public static void WebSocket_Https_Duplex_BinaryStreamed()
    {
        BinaryMessageEncodingBindingElement binaryMessageEncodingBindingElement = null;
        HttpsTransportBindingElement httpsTransportBindingElement = null;
        CustomBinding binding = null;
        ClientReceiver clientReceiver = null;
        InstanceContext context = null;
        DuplexChannelFactory<IWSDuplexService> channelFactory = null;
        IWSDuplexService client = null;
        FlowControlledStream uploadStream = null;

        try
        {
            // *** SETUP *** \\
            binaryMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
            httpsTransportBindingElement = new HttpsTransportBindingElement();
            httpsTransportBindingElement.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            httpsTransportBindingElement.MaxReceivedMessageSize = ScenarioTestHelpers.DefaultMaxReceivedMessageSize;
            httpsTransportBindingElement.MaxBufferSize = ScenarioTestHelpers.DefaultMaxReceivedMessageSize;
            httpsTransportBindingElement.TransferMode = TransferMode.Streamed;
            binding = new CustomBinding(binaryMessageEncodingBindingElement, httpsTransportBindingElement);

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);

            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, Endpoints.WebSocketHttpsDuplexBinaryStreamed_Address);
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

            Assert.True(clientReceiver.ReceiveStreamInvoked.WaitOne(ScenarioTestHelpers.TestTimeout), "Test case timeout was reached while waiting for the stream response from the Service.");
            clientReceiver.ReceiveStreamInvoked.Reset();

            // Upload the stream while we are downloading a different stream
            uploadStream = new FlowControlledStream();
            uploadStream.ReadThrottle = TimeSpan.FromMilliseconds(500);
            uploadStream.StreamDuration = TimeSpan.FromSeconds(1);
            client.UploadStream(uploadStream);

            client.StopPushingStream();
            // Waiting on ReceiveStreamCompleted from the ClientReceiver.
            clientReceiver.ReceiveStreamCompleted.WaitOne();
            clientReceiver.ReceiveStreamCompleted.Reset();

            // *** VALIDATE *** \\
            // Validation is based on no exceptions being thrown.

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

    [Fact]
    [OuterLoop]
    [ActiveIssue(420, PlatformID.AnyUnix)]
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

    [Fact]
    [OuterLoop]
    [ActiveIssue(420, PlatformID.AnyUnix)]
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

    [Fact]
    [OuterLoop]
    [ActiveIssue(420, PlatformID.AnyUnix)]
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
}
