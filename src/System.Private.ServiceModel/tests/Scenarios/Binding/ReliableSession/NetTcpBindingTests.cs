// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public class Binding_ReliableSession_NetTcpBindingTests : ConditionalWcfTest
{
    [WcfTheory]
    [MemberData(nameof(GetTestVariations))]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static async Task EchoCall(ReliableMessagingVersion rmVersion, bool ordered, string endpointSuffix)
    {
        string testString = "Hello";
        ChannelFactory<IWcfReliableService> factory = null;
        IWcfReliableService serviceProxy = null;
        NetTcpBinding binding = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.None, true);
            binding.ReliableSession.Ordered = ordered;
            var customBinding = new CustomBinding(binding);
            var reliableSessionBindingElement = customBinding.Elements.Find<ReliableSessionBindingElement>();
            reliableSessionBindingElement.ReliableMessagingVersion = rmVersion;
            factory = new ChannelFactory<IWcfReliableService>(customBinding, new EndpointAddress(Endpoints.ReliableSession_NetTcp + endpointSuffix));
            serviceProxy = factory.CreateChannel();
            // *** EXECUTE *** \\
            ((IClientChannel)serviceProxy).Open(); // This will establish a reliable session
            var result = await serviceProxy.EchoAsync(testString);
            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            ((IClientChannel)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfTheory]
    [MemberData(nameof(GetTestVariations))]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static async Task OneWayCall(ReliableMessagingVersion rmVersion, bool ordered, string endpointSuffix)
    {
        string testString = "Hello";
        ChannelFactory<IOneWayWcfReliableService> factory = null;
        IOneWayWcfReliableService serviceProxy = null;
        NetTcpBinding binding = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.None, true);
            binding.ReliableSession.Ordered = ordered;
            var customBinding = new CustomBinding(binding);
            var reliableSessionBindingElement = customBinding.Elements.Find<ReliableSessionBindingElement>();
            reliableSessionBindingElement.ReliableMessagingVersion = rmVersion;
            factory = new ChannelFactory<IOneWayWcfReliableService>(customBinding, new EndpointAddress(Endpoints.ReliableOneWaySession_NetTcp + endpointSuffix));
            serviceProxy = factory.CreateChannel();
            // *** EXECUTE *** \\
            ((IClientChannel)serviceProxy).Open(); // This will establish a reliable session
            await serviceProxy.OneWayAsync(testString);
            // *** VALIDATE *** \\

            // *** CLEANUP *** \\
            ((IClientChannel)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfTheory]
    [MemberData(nameof(GetTestVariations))]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static async Task DuplexEchoCall(ReliableMessagingVersion rmVersion, bool ordered, string endpointSuffix)
    {
        string testString = "Hello";
        DuplexChannelFactory<IWcfReliableDuplexService> factory = null;
        IWcfReliableDuplexService serviceProxy = null;
        NetTcpBinding binding = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding(SecurityMode.None, true);
            binding.ReliableSession.Ordered = ordered;
            var customBinding = new CustomBinding(binding);
            var reliableSessionBindingElement = customBinding.Elements.Find<ReliableSessionBindingElement>();
            reliableSessionBindingElement.ReliableMessagingVersion = rmVersion;
            var callbackService = new ReliableDuplexCallbackService();
            var instanceContext = new InstanceContext(callbackService);
            factory = new DuplexChannelFactory<IWcfReliableDuplexService>(instanceContext, customBinding, new EndpointAddress(Endpoints.ReliableDuplexSession_NetTcp + endpointSuffix));
            serviceProxy = factory.CreateChannel();
            // *** EXECUTE *** \\
            ((IClientChannel)serviceProxy).Open(); // This will establish a reliable session
            var result = await serviceProxy.DuplexEchoAsync(testString);
            // *** VALIDATE *** \\
            Assert.Equal(testString, result);
            Assert.Equal(testString, callbackService.LastReceivedEcho);

            // *** CLEANUP *** \\
            ((IClientChannel)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    public class ReliableDuplexCallbackService : IWcfReliableDuplexService
    {
        public string LastReceivedEcho { get; private set; } = null;

        public Task<string> DuplexEchoAsync(string echo)
        {
            LastReceivedEcho = echo;
            return Task.FromResult(echo);
        }
    }

    public static IEnumerable<object[]> GetTestVariations()
    {
        yield return new object[] { ReliableMessagingVersion.WSReliableMessaging11, true, "Ordered_" + ReliableMessagingVersion.WSReliableMessaging11.ToString() };
        yield return new object[] { ReliableMessagingVersion.WSReliableMessaging11, false, "Unordered_" + ReliableMessagingVersion.WSReliableMessaging11.ToString() };
        yield return new object[] { ReliableMessagingVersion.WSReliableMessagingFebruary2005, true, "Ordered_" + ReliableMessagingVersion.WSReliableMessagingFebruary2005.ToString() };
        yield return new object[] { ReliableMessagingVersion.WSReliableMessagingFebruary2005, false, "Unordered_" + ReliableMessagingVersion.WSReliableMessagingFebruary2005.ToString() };
    }
}
