﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Infrastructure.Common;

public class WebSocketTests : ConditionalWcfTest
{
    [Fact]
    [OuterLoop]
#if !FEATURE_NETNATIVE
    [ActiveIssue(420, PlatformID.AnyUnix)]
#else
    [ActiveIssue(526)]
#endif
    public static void WebSocket_Http_Duplex_Echo_BinaryStreamed()
    {
        NetHttpBinding binding = null;
        ClientReceiver clientReceiver = null;
        InstanceContext context = null;
        DuplexChannelFactory<IWSDuplexService> channelFactory = null;
        IWSDuplexService client = null;
        Stream sentStream = null;
        Stream receivedStream = null;
        string sentString = "test data";
        int timeoutMs = 5000;

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
            binding.MessageEncoding = NetHttpMessageEncoding.Binary;
            binding.SendTimeout = TimeSpan.FromMilliseconds(timeoutMs);

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);

            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, Endpoints.WebSocketHttpDuplexBinaryStreamed_Address);
            client = channelFactory.CreateChannel();

            sentStream = ScenarioTestHelpers.StringToStream(sentString);

            // *** EXECUTE *** \\
            receivedStream = client.EchoStream(sentStream);

            // *** VALIDATE *** \\ 
            Assert.True(receivedStream != null, "Expected non-null stream returned.");
            string receivedString = ScenarioTestHelpers.StreamToString(receivedStream);
            Assert.True(String.Equals(sentString, receivedString),
                        String.Format("Expected returned stream with '{0}' but actual was '{1}'", sentString, receivedString));

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            channelFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, channelFactory);
            clientReceiver.Dispose();
            if (sentStream != null)
            {
                sentStream.Dispose();
            }
            if (receivedStream != null)
            {
                receivedStream.Dispose();
            }
        }
    }


    [Fact]
    [OuterLoop]
#if !FEATURE_NETNATIVE
    [ActiveIssue(420, PlatformID.AnyUnix)]
#else
    [ActiveIssue(526)]
#endif
    public static void WebSocket_Http_Duplex_Echo_Async_BinaryStreamed()
    {
        NetHttpBinding binding = null;
        ClientReceiver clientReceiver = null;
        InstanceContext context = null;
        DuplexChannelFactory<IWSDuplexService> channelFactory = null;
        IWSDuplexService client = null;
        Stream sentStream = null;
        Stream receivedStream = null;
        string sentString = "test data";
        int timeoutMs = 5000;

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
            binding.MessageEncoding = NetHttpMessageEncoding.Binary;
            binding.SendTimeout = TimeSpan.FromMilliseconds(timeoutMs);

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);

            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, Endpoints.WebSocketHttpDuplexBinaryStreamed_Address);
            client = channelFactory.CreateChannel();

            sentStream = ScenarioTestHelpers.StringToStream(sentString);

            // *** EXECUTE *** \\
            //using (OperationContextScope scope = new OperationContextScope((IContextChannel)client))
            {
                Task<Stream> echoTask = client.EchoStreamAsync(sentStream);
                receivedStream = echoTask.GetAwaiter().GetResult();
            }

            // *** VALIDATE *** \\ 
            Assert.True(receivedStream != null, "Expected non-null stream returned.");
            string receivedString = ScenarioTestHelpers.StreamToString(receivedStream);
            Assert.True(String.Equals(sentString, receivedString),
                        String.Format("Expected returned stream with '{0}' but actual was '{1}'", sentString, receivedString));

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            channelFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, channelFactory);
            clientReceiver.Dispose();
            if (sentStream != null)
            {
                sentStream.Dispose();
            }
            if (receivedStream != null)
            {
                receivedStream.Dispose();
            }
        }
    }

    [Fact]
    [OuterLoop]
#if !FEATURE_NETNATIVE
    [ActiveIssue(420, PlatformID.AnyUnix)]
#else
    [ActiveIssue(526)]
#endif
    public static void WebSocket_Http_Duplex_Echo_Async_Recursive_BinaryStreamed()
    {
        // This test is like the other async test above but deliberately exploits
        // the duplex callback's OperationContext to trigger a 2nd async operation
        // on the Thread using ServiceModel's own OperationContext.  This forces
        // ServiceChannel.PrepareCall to detect it is in a potential deadlock state
        // and throw an exception.
        NetHttpBinding binding = null;
        ClientReceiver clientReceiver = null;
        InstanceContext context = null;
        DuplexChannelFactory<IWSDuplexService> channelFactory = null;
        IWSDuplexService client = null;
        Stream sentStream = null;
        Stream receivedStream = null;
        string sentString = "test data";
        int timeoutMs = 5000;
        InvalidOperationException callbackException = null;

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
            binding.MessageEncoding = NetHttpMessageEncoding.Binary;
            binding.SendTimeout = TimeSpan.FromMilliseconds(timeoutMs);

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);

            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, Endpoints.WebSocketHttpDuplexBinaryStreamed_Address);
            client = channelFactory.CreateChannel();

            sentStream = ScenarioTestHelpers.StringToStream(sentString);

            // Arrange for a 2nd call to echo the stream when the duplex callback
            // is invoked.  The callback happens on the Thread used by ServiceModel
            // containing the OperationContext it uses internally.  This 2nd call
            // will trigger ServiceChannel.PrepareCall to detect a deadlock situation
            // and throw 
            EventHandler callbackAction = (sender, args) =>
            {
                try
                {
                    clientReceiver.OperationCompletedAction = null;
                    Task<Stream> echoTask = client.EchoStreamAsync(sentStream);
                    echoTask.GetAwaiter().GetResult();
                }
                catch (InvalidOperationException ex)
                {
                    callbackException = ex;
                }
            };

            clientReceiver.OperationCompletedAction = callbackAction;

            // *** EXECUTE *** \\
            //using (OperationContextScope scope = new OperationContextScope((IContextChannel)client))
            {
                Task<Stream> echoTask = client.EchoStreamAsync(sentStream);
                receivedStream = echoTask.GetAwaiter().GetResult();
            }

            // *** VALIDATE *** \\ 
            Assert.True(callbackException != null, "Expected exception from recursive EchoStream.");
            Assert.True(receivedStream != null, "Expected non-null stream returned.");
            string receivedString = ScenarioTestHelpers.StreamToString(receivedStream);
            Assert.True(String.Equals(sentString, receivedString),
                        String.Format("Expected returned stream with '{0}' but actual was '{1}'", sentString, receivedString));

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            channelFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, channelFactory);
            clientReceiver.Dispose();
            if (sentStream != null)
            {
                sentStream.Dispose();
            }
            if (receivedStream != null)
            {
                receivedStream.Dispose();
            }
        }
    }

    [Fact]
    [OuterLoop]
    [ActiveIssue(420, PlatformID.AnyUnix)]
    public static void WebSocket_Http_RequestReply_BinaryStreamed()
    {
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

    [Fact]
    [OuterLoop]
#if !FEATURE_NETNATIVE
    [ActiveIssue(420, PlatformID.AnyUnix)]
#else
    [ActiveIssue(526)]
#endif
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
            binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
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

    [ConditionalFact(nameof(Root_Certificate_Installed))]
    [OuterLoop]
#if !FEATURE_NETNATIVE
    [ActiveIssue(420, PlatformID.AnyUnix)]
#else
    [ActiveIssue(526)]
#endif
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
            httpsTransportBindingElement = new HttpsTransportBindingElement()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
                TransferMode = TransferMode.Streamed
            };
            httpsTransportBindingElement.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
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

    [Fact]
    [OuterLoop]
#if !FEATURE_NETNATIVE
    [ActiveIssue(4429)] // In #CoreFX
    [ActiveIssue(470)]
    [ActiveIssue(420, PlatformID.AnyUnix)]
#else
    [ActiveIssue(526)]
#endif
    public static void WebSocket_Https_Duplex_TextStreamed()
    {
        TextMessageEncodingBindingElement textMessageEncodingBindingElement = null;
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
            textMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
            httpsTransportBindingElement = new HttpsTransportBindingElement()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
                TransferMode = TransferMode.Streamed
            };
            httpsTransportBindingElement.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding = new CustomBinding(textMessageEncodingBindingElement, httpsTransportBindingElement);

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);

            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, Endpoints.WebSocketHttpsDuplexTextStreamed_Address);
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

    [Fact]
    [OuterLoop]
#if !FEATURE_NETNATIVE
    [ActiveIssue(4429)] // In #CoreFX
    [ActiveIssue(470)]
    [ActiveIssue(420, PlatformID.AnyUnix)]
#else
    [ActiveIssue(526)]
#endif
    public static void WebSocket_Http_Duplex_TextStreamed()
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
            binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = TransferMode.Streamed;
            binding.MessageEncoding = NetHttpMessageEncoding.Text;

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);

            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, Endpoints.WebSocketHttpDuplexTextStreamed_Address);
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

    [Fact]
    [OuterLoop]
    [ActiveIssue(420, PlatformID.AnyUnix)]
    public static void WebSocket_Http_RequestReply_TextStreamed()
    {
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
            binding.MessageEncoding = NetHttpMessageEncoding.Text;

            channelFactory = new ChannelFactory<IWSRequestReplyService>(binding, new EndpointAddress(Endpoints.WebSocketHttpRequestReplyTextStreamed_Address));
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

    [ConditionalFact(nameof(Root_Certificate_Installed))]
    [OuterLoop]
    [ActiveIssue(625, PlatformID.AnyUnix)]
    public static void WebSocket_Https_RequestReply_BinaryBuffered()
    {
        BinaryMessageEncodingBindingElement binaryMessageEncodingBindingElement = null;
        HttpsTransportBindingElement httpsTransportBindingElement = null;
        CustomBinding binding = null;
        ChannelFactory<IWSRequestReplyService> channelFactory = null;
        IWSRequestReplyService client = null;

        try
        {
            // *** SETUP *** \\
            binaryMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
            httpsTransportBindingElement = new HttpsTransportBindingElement()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            httpsTransportBindingElement.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding = new CustomBinding(binaryMessageEncodingBindingElement, httpsTransportBindingElement);

            channelFactory = new ChannelFactory<IWSRequestReplyService>(binding, new EndpointAddress(Endpoints.WebSocketHttpsRequestReplyBinaryBuffered_Address));
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

    [ConditionalFact(nameof(Root_Certificate_Installed))]
    [OuterLoop]
    [ActiveIssue(625, PlatformID.AnyUnix)]
    public static void WebSocket_Https_RequestReply_TextBuffered_KeepAlive()
    {
        TextMessageEncodingBindingElement textMessageEncodingBindingElement = null;
        HttpsTransportBindingElement httpsTransportBindingElement = null;
        CustomBinding binding = null;
        ChannelFactory<IWSRequestReplyService> channelFactory = null;
        IWSRequestReplyService client = null;

        try
        {
            // *** SETUP *** \\
            textMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
            httpsTransportBindingElement = new HttpsTransportBindingElement()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB
            };
            httpsTransportBindingElement.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            httpsTransportBindingElement.WebSocketSettings.KeepAliveInterval = TimeSpan.FromSeconds(2);
            binding = new CustomBinding(textMessageEncodingBindingElement, httpsTransportBindingElement);

            channelFactory = new ChannelFactory<IWSRequestReplyService>(binding, new EndpointAddress(Endpoints.WebSocketHttpsRequestReplyTextBuffered_Address));

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

    [ConditionalFact(nameof(Root_Certificate_Installed))]
    [OuterLoop]
    [ActiveIssue(625, PlatformID.AnyUnix)]
    public static void WebSocket_Https_Duplex_BinaryBuffered()
    {
        BinaryMessageEncodingBindingElement binaryMessageEncodingBindingElement = null;
        HttpsTransportBindingElement httpsTransportBindingElement = null;
        CustomBinding binding = null;
        ClientReceiver clientReceiver = null;
        InstanceContext context = null;
        DuplexChannelFactory<IWSDuplexService> channelFactory = null;
        IWSDuplexService client = null;

        try
        {
            // *** SETUP *** \\
            binaryMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
            httpsTransportBindingElement = new HttpsTransportBindingElement()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
            };
            httpsTransportBindingElement.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding = new CustomBinding(binaryMessageEncodingBindingElement, httpsTransportBindingElement);

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);

            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, new EndpointAddress(Endpoints.WebSocketHttpsDuplexBinaryBuffered_Address));
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

    [ConditionalFact(nameof(Root_Certificate_Installed))]
    [OuterLoop]
    [ActiveIssue(625, PlatformID.AnyUnix)]
    public static void WebSocket_Https_Duplex_TextBuffered_KeepAlive()
    {
        TextMessageEncodingBindingElement textMessageEncodingBindingElement = null;
        HttpsTransportBindingElement httpsTransportBindingElement = null;
        CustomBinding binding = null;
        ClientReceiver clientReceiver = null;
        InstanceContext context = null;
        DuplexChannelFactory<IWSDuplexService> channelFactory = null;
        IWSDuplexService client = null;

        try
        {
            // *** SETUP *** \\
            textMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
            httpsTransportBindingElement = new HttpsTransportBindingElement()
            {
                MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                MaxBufferSize = ScenarioTestHelpers.SixtyFourMB
            };
            httpsTransportBindingElement.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            httpsTransportBindingElement.WebSocketSettings.KeepAliveInterval = TimeSpan.FromSeconds(2);
            binding = new CustomBinding(textMessageEncodingBindingElement, httpsTransportBindingElement);

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);
            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, new EndpointAddress(Endpoints.WebSocketHttpsDuplexTextBuffered_Address));
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

    [Fact]
    [OuterLoop]
    [ActiveIssue(625, PlatformID.AnyUnix)]
    public static void WebSocket_Http_RequestReply_TextBuffered()
    {
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
            binding.MessageEncoding = NetHttpMessageEncoding.Text;

            channelFactory = new ChannelFactory<IWSRequestReplyService>(binding, new EndpointAddress(Endpoints.WebSocketHttpRequestReplyTextBuffered_Address));

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

    [Fact]
    [OuterLoop]
    [ActiveIssue(625, PlatformID.AnyUnix)]
    public static void WebSocket_Http_RequestReply_BinaryBuffered_KeepAlive()
    {
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
            binding.MessageEncoding = NetHttpMessageEncoding.Binary;
            binding.WebSocketSettings.KeepAliveInterval = TimeSpan.FromSeconds(2);

            channelFactory = new ChannelFactory<IWSRequestReplyService>(binding, new EndpointAddress(Endpoints.WebSocketHttpRequestReplyBinaryBuffered_Address));

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

    [Fact]
    [OuterLoop]
    [ActiveIssue(625, PlatformID.AnyUnix)]
    public static void WebSocket_Http_Duplex_TextBuffered_KeepAlive()
    {
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
            binding.MessageEncoding = NetHttpMessageEncoding.Text;
            binding.WebSocketSettings.KeepAliveInterval = TimeSpan.FromSeconds(2);

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);
            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, new EndpointAddress(Endpoints.WebSocketHttpDuplexTextBuffered_Address));
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

    [Fact]
    [OuterLoop]
    [ActiveIssue(625, PlatformID.AnyUnix)]
    public static void WebSocket_Http_Duplex_BinaryBuffered()
    {
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
            binding.MessageEncoding = NetHttpMessageEncoding.Binary;

            clientReceiver = new ClientReceiver();
            context = new InstanceContext(clientReceiver);
            channelFactory = new DuplexChannelFactory<IWSDuplexService>(context, binding, new EndpointAddress(Endpoints.WebSocketHttpDuplexBinaryBuffered_Address));
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
}
