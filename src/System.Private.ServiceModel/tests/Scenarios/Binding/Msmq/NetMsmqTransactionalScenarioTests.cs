// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Runtime.Versioning;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Transactions;
using Infrastructure.Common;
using MSMQ.Messaging;
using Xunit;

namespace Binding.Msmq.IntegrationTests
{
    // Verifies that the MSMQ output channel properly participates in an
    // ambient System.Transactions.Transaction: commit means the message
    // is on the queue; rollback means it is not. Both tests target a
    // transactional MSMQ queue (the only kind that honors transactional
    // sends).
    [SupportedOSPlatform("windows")]
    public class NetMsmqTransactionalScenarioTests : ConditionalWcfTest, IDisposable
    {
        private readonly string _shortName;
        private readonly string _machineQueuePath;
        private readonly Uri _queueUri;

        public NetMsmqTransactionalScenarioTests()
        {
            _shortName = "wcf-tx-test-" + Guid.NewGuid().ToString("N");
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
            catch { /* swallow on cleanup */ }
        }

        [WcfFact]
        [Condition(nameof(MsmqInstalled))]
        [Condition(nameof(ImplicitDtcEnabled))]
        public void Send_InCommittedTransactionScope_MessageArrivesInQueue()
        {
            MessageQueue.Create(_machineQueuePath, transactional: true);

            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                SendOne("committed");
                scope.Complete();
            }

            using var receiver = new MessageQueue("FormatName:DIRECT=OS:" + _machineQueuePath);
            using MSMQ.Messaging.Message received = receiver.Receive(MessageQueueTransactionType.Single);
            Assert.NotNull(received);
            Assert.True(received.BodyStream.Length > 0);
        }

        [WcfFact]
        [Condition(nameof(MsmqInstalled))]
        [Condition(nameof(ImplicitDtcEnabled))]
        public void Send_InRolledBackTransactionScope_MessageDoesNotArrive()
        {
            MessageQueue.Create(_machineQueuePath, transactional: true);

            using (var scope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                SendOne("rolled-back");
                // scope.Complete() not called -> Dispose rolls back.
            }

            using var receiver = new MessageQueue("FormatName:DIRECT=OS:" + _machineQueuePath);
            // Expect no message; use a short timeout so the test fails fast on regression.
            MSMQ.Messaging.MessageQueueException ex = Assert.Throws<MessageQueueException>(() =>
                receiver.Receive(TimeSpan.FromSeconds(2), MessageQueueTransactionType.Single));
            Assert.Equal(MessageQueueErrorCode.IOTimeout, ex.MessageQueueErrorCode);
        }

        // Note on transactional scenario tests: the .NET 8+ runtime
        // disables implicit DTC promotion by default and the property
        // must be flipped before any code in the process touches
        // System.Transactions — which is not reliable from inside an
        // xunit test host because the discovery layer initialises DTC
        // first. These two tests are therefore gated behind
        // [Condition(ImplicitDtcEnabled)], which keys off the
        // WCF_MSMQ_ENABLE_DTC_TESTS=true env var. Treat them as a
        // manual / dev-machine check unless and until we get a
        // dedicated host process. Product correctness has been verified
        // out-of-process via a standalone harness that sets
        // TransactionManager.ImplicitDistributedTransactions=true as
        // its very first statement.

        private void SendOne(string bodyText)
        {
            var binding = new NetMsmqBinding(NetMsmqSecurityMode.None)
            {
                Durable = true,
                ExactlyOnce = true,
            };
            IChannelFactory<IOutputChannel> factory = binding.BuildChannelFactory<IOutputChannel>();
            try
            {
                factory.Open();
                IOutputChannel channel = factory.CreateChannel(new EndpointAddress(_queueUri));
                channel.Open();
                try
                {
                    var msg = System.ServiceModel.Channels.Message.CreateMessage(
                        MessageVersion.Soap12WSAddressing10, "urn:test/echo", bodyText);
                    channel.Send(msg);
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
