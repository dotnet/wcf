// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Infrastructure.Common;
using Xunit;

// Regression coverage for dotnet/wcf#5803.
//
// When a server closes its session while the client is idle between calls,
// the client's duplex receive pump processes the EndRecord and the
// ServiceChannel's auto-close path must transition the outer channel from
// Opened to Closed. Prior to the fix, only the inner session was half-closed
// and the outer channel remained Opened indefinitely.
public class ServerInitiatedSessionShutdownTests : ConditionalWcfTest
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    private class NoOpCallback : IServerInitiatedShutdownCallback
    {
        public void OnShutdownNotification() { }
    }

    [WcfFact]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void ServerInitiatedShutdown_ClientChannelTransitionsToClosed()
    {
        DuplexChannelFactory<IServerInitiatedShutdownService> factory = null;
        IServerInitiatedShutdownService proxy = null;
        ICommunicationObject commObj = null;
        ManualResetEventSlim closingSignal = new ManualResetEventSlim(false);
        ManualResetEventSlim closedSignal = new ManualResetEventSlim(false);
        bool faulted = false;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            binding.CloseTimeout = TimeSpan.FromSeconds(10);
            binding.OpenTimeout = TimeSpan.FromSeconds(10);
            binding.SendTimeout = TimeSpan.FromSeconds(10);

            InstanceContext context = new InstanceContext(new NoOpCallback());
            factory = new DuplexChannelFactory<IServerInitiatedShutdownService>(
                context,
                binding,
                new EndpointAddress(Endpoints.Tcp_NoSecurity_ServerInitiatedShutdown_Address));

            proxy = factory.CreateChannel();
            commObj = (ICommunicationObject)proxy;
            commObj.Closing += (s, e) => closingSignal.Set();
            commObj.Closed += (s, e) => closedSignal.Set();
            commObj.Faulted += (s, e) => { faulted = true; closedSignal.Set(); };
            commObj.Open();

            // *** EXECUTE *** \\
            // Warm-up call so the duplex receive pump is active.
            string echoed = proxy.Echo("hello");
            Assert.Equal("hello", echoed);

            // Ask the service to close its session after replying.
            string shutdownReply = proxy.RequestServerShutdown();
            Assert.Equal("Server shutting down", shutdownReply);

            // *** VALIDATE *** \\
            // Wait up to 10s for the client to react to the server's EndRecord.
            bool signaled = closedSignal.Wait(binding.CloseTimeout + TimeSpan.FromSeconds(5));

            Assert.True(signaled,
                $"Client ServiceChannel did not transition out of {CommunicationState.Opened} after the " +
                $"server closed its session. Current state: {commObj.State}. " +
                "This indicates issue dotnet/wcf#5803 has regressed.");

            Assert.True(closingSignal.IsSet,
                "Client ServiceChannel did not raise Closing in response to a graceful server-initiated session shutdown.");

            Assert.False(faulted,
                "Client ServiceChannel transitioned to Faulted instead of Closed in response to a graceful " +
                "server-initiated session shutdown.");

            Assert.Equal(CommunicationState.Closed, commObj.State);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)proxy, factory);
        }
    }
}
