// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Xunit;

public static class Binding_NetHttp_WebSocketTests
{
    // Duplex over WebSocket occurs automatically when the service contract is declared with a callback contract.
    [Fact]
    [OuterLoop]
    [ActiveIssue(420, PlatformID.AnyUnix)]
    public static void DuplexOverWebSocket_Echo_RoundTrips_Guid()
    {
        DuplexChannelFactory<IWcfDuplexService> factory = null;
        IWcfDuplexService duplexProxy = null;
        Guid guid = Guid.NewGuid();

        try
        {
            // *** SETUP *** \\
            NetHttpBinding binding = new NetHttpBinding();

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
            factory.Close();
            ((ICommunicationObject)duplexProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)duplexProxy, factory);
        }
    }

    // Force the binding to use WebSocket by setting the WebSocket transport usage to Always.
    [Fact]
    [OuterLoop]
    [ActiveIssue(420, PlatformID.AnyUnix)]
    public static void RequestResponseOverWebSocketManually_Echo_RoundTrips_Guid()
    {
        DuplexChannelFactory<IWcfDuplexService> factory = null;
        IWcfDuplexService duplexProxy = null;
        Guid guid = Guid.NewGuid();

        try
        {
            // *** SETUP *** \\
            NetHttpBinding binding = new NetHttpBinding();
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
    public static void TransportUsageAlways_WebSockets_Synchronous_Call()
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
