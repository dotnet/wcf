// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.IO;
using System.Runtime.Versioning;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Infrastructure.Common;
using MSMQ.Messaging;
using Xunit;

using WcfMessage = System.ServiceModel.Channels.Message;
using MsmqRawMessage = MSMQ.Messaging.Message;

namespace Binding.Msmq.IntegrationTests
{
    // End-to-end scenario tests for NetMsmqBinding's send path. Each
    // test provisions a fresh private queue, sends through our client
    // channel, then verifies the message arrived by reading the queue
    // back with MSMQ.Messaging directly. No service host is required;
    // we exercise the wire path against the local queue manager.
    [SupportedOSPlatform("windows")]
    public class NetMsmqClientScenarioTests : ConditionalWcfTest, IDisposable
    {
        private readonly string _shortName;
        private readonly string _machineQueuePath;
        private readonly Uri _queueUri;

        public NetMsmqClientScenarioTests()
        {
            _shortName = "wcf-test-" + Guid.NewGuid().ToString("N");
            _machineQueuePath = ".\\private$\\" + _shortName;
            _queueUri = new Uri("net.msmq://localhost/private/" + _shortName);
        }

        public void Dispose()
        {
            try
            {
                if (MessageQueue.Exists(_machineQueuePath))
                {
                    MessageQueue.Delete(_machineQueuePath);
                }
            }
            catch (MessageQueueException)
            {
                // Queue teardown is best effort: the queue may already be gone,
                // or the worker may lack delete rights. Narrow the catch to MSMQ
                // failures so an unrelated bug in cleanup is not buried.
            }
        }

        // Durable=true must reach PROPID_M_DELIVERY as MQMSG_DELIVERY_RECOVERABLE.
        // While it went unwritten MSMQ applied its EXPRESS default, and every
        // message the caller believed was durable was lost on a queue manager
        // restart.
        [WcfFact]
        [Condition(nameof(MsmqInstalled))]
        public void Send_Durable_MessageIsRecoverable()
        {
            MessageQueue.Create(_machineQueuePath, transactional: false);

            SendOne("durable-payload");

            using var receiver = new MessageQueue("FormatName:DIRECT=OS:" + _machineQueuePath);
            using MsmqRawMessage received = receiver.Receive(TimeSpan.FromSeconds(10));
            Assert.True(received.Recoverable);
        }

        // The asynchronous send path used to dispatch through Task.Run and read
        // the thread-static Transaction.Current on the worker thread, so an
        // ambient transaction was never observed. This covers the APM overloads
        // reaching the queue at all; the transactional behaviour itself is
        // covered by NetMsmqTransactionalScenarioTests.
        [WcfFact]
        [Condition(nameof(MsmqInstalled))]
        public void BeginSend_MessageArrivesInQueue()
        {
            MessageQueue.Create(_machineQueuePath, transactional: false);

            var binding = new NetMsmqBinding(NetMsmqSecurityMode.None)
            {
                Durable = false,
                ExactlyOnce = false,
            };
            IChannelFactory<IOutputChannel> factory = binding.BuildChannelFactory<IOutputChannel>();
            try
            {
                factory.Open();
                IOutputChannel channel = factory.CreateChannel(new EndpointAddress(_queueUri));
                channel.Open();
                try
                {
                    WcfMessage msg = WcfMessage.CreateMessage(MessageVersion.Soap12WSAddressing10, "urn:test/echo", "async");
                    IAsyncResult result = channel.BeginSend(msg, null, null);
                    channel.EndSend(result);
                }
                finally
                {
                    channel.Close();
                }
            }
            finally
            {
                factory.Close();
            }

            using var receiver = new MessageQueue("FormatName:DIRECT=OS:" + _machineQueuePath);
            using MsmqRawMessage received = receiver.Receive(TimeSpan.FromSeconds(10));
            Assert.NotNull(received.BodyStream);
        }

        // The send timeout used to be discarded outright.
        [WcfFact]
        [Condition(nameof(MsmqInstalled))]
        public void Send_WithExpiredTimeout_ThrowsTimeoutException()
        {
            MessageQueue.Create(_machineQueuePath, transactional: false);

            var binding = new NetMsmqBinding(NetMsmqSecurityMode.None)
            {
                Durable = false,
                ExactlyOnce = false,
            };
            IChannelFactory<IOutputChannel> factory = binding.BuildChannelFactory<IOutputChannel>();
            try
            {
                factory.Open();
                IOutputChannel channel = factory.CreateChannel(new EndpointAddress(_queueUri));
                channel.Open();
                try
                {
                    WcfMessage msg = WcfMessage.CreateMessage(MessageVersion.Soap12WSAddressing10, "urn:test/echo", "x");
                    Assert.Throws<TimeoutException>(() => channel.Send(msg, TimeSpan.Zero));
                }
                finally
                {
                    channel.Abort();
                }
            }
            finally
            {
                factory.Close();
            }
        }

        [WcfFact]
        [Condition(nameof(MsmqInstalled))]
        public void Send_NonTransactional_MessageArrivesInQueue()
        {
            MessageQueue.Create(_machineQueuePath, transactional: false);

            SendOne("hello");

            using var receiver = new MessageQueue("FormatName:DIRECT=OS:" + _machineQueuePath);
            using MsmqRawMessage received = receiver.Receive(TimeSpan.FromSeconds(10));
            Assert.NotNull(received);
            Assert.NotNull(received.BodyStream);
            Assert.True(received.BodyStream.Length > 0);
        }

        [WcfFact]
        [Condition(nameof(MsmqInstalled))]
        public void Send_ToNonExistentQueue_ThrowsEndpointNotFoundException()
        {
            var binding = new NetMsmqBinding(NetMsmqSecurityMode.None)
            {
                Durable = false,
                ExactlyOnce = false,
            };
            IChannelFactory<IOutputChannel> factory = binding.BuildChannelFactory<IOutputChannel>();
            try
            {
                factory.Open();
                Uri missing = new Uri("net.msmq://localhost/private/does-not-exist-" + Guid.NewGuid().ToString("N"));
                IOutputChannel channel = factory.CreateChannel(new EndpointAddress(missing));
                channel.Open();
                try
                {
                    WcfMessage msg = WcfMessage.CreateMessage(MessageVersion.Soap12WSAddressing10, "urn:test/echo", "x");
                    EndpointNotFoundException ex = Assert.Throws<EndpointNotFoundException>(() => channel.Send(msg));
                    Assert.IsType<MsmqException>(ex.InnerException);
                }
                finally
                {
                    channel.Abort();
                }
            }
            finally
            {
                factory.Close();
            }
        }

        [WcfFact]
        [Condition(nameof(MsmqInstalled))]
        public void Send_NonTransactional_BodyBytesRoundTripBinaryEncoder()
        {
            MessageQueue.Create(_machineQueuePath, transactional: false);

            byte[] sentEncoded = SendOne("payload-42", returnEncodedBody: true);

            using var receiver = new MessageQueue("FormatName:DIRECT=OS:" + _machineQueuePath);
            using MsmqRawMessage received = receiver.Receive(TimeSpan.FromSeconds(10));
            using var ms = new MemoryStream();
            received.BodyStream.CopyTo(ms);
            Assert.Equal(sentEncoded, ms.ToArray());
        }

        private byte[] SendOne(string bodyText, bool returnEncodedBody = false)
        {
            var binding = new NetMsmqBinding(NetMsmqSecurityMode.None)
            {
                Durable = true,
                ExactlyOnce = false,
            };
            IChannelFactory<IOutputChannel> factory = binding.BuildChannelFactory<IOutputChannel>();
            try
            {
                factory.Open();
                IOutputChannel channel = factory.CreateChannel(new EndpointAddress(_queueUri));
                channel.Open();
                try
                {
                    WcfMessage msg = WcfMessage.CreateMessage(MessageVersion.Soap12WSAddressing10, "urn:test/echo", bodyText);
                    byte[] encoded = null;
                    if (returnEncodedBody)
                    {
                        using MessageBuffer buffer = msg.CreateBufferedCopy(int.MaxValue);
                        WcfMessage previewMsg = buffer.CreateMessage();
                        MessageEncoder encoder = new BinaryMessageEncodingBindingElement().CreateMessageEncoderFactory().Encoder;
                        var preview = encoder.WriteMessage(previewMsg, int.MaxValue, BufferManager.CreateBufferManager(64 * 1024, 64 * 1024));
                        encoded = new byte[preview.Count];
                        Buffer.BlockCopy(preview.Array, preview.Offset, encoded, 0, preview.Count);
                        msg = buffer.CreateMessage();
                    }
                    channel.Send(msg);
                    return encoded;
                }
                finally
                {
                    channel.Close();
                }
            }
            finally
            {
                factory.Close();
            }
        }
    }
}
