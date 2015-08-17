// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestTypes;
using Xunit;

public static class Binding_NetHttp_WebSocketTests
{
    // Duplex over WebSocket occurs automatically when the service contract is declared with a callback contract.
    [Fact]
    [OuterLoop]
    [ActiveIssue(275)]
    public static void DuplexOverWebSocket_Echo_RoundTrips_Guid()
    {
        DuplexChannelFactory<IWcfDuplexService> factory = null;
        Guid guid = Guid.NewGuid();

        NetHttpBinding binding = new NetHttpBinding();

        WcfDuplexServiceCallback callbackService = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callbackService);

        try
        {
            factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, new EndpointAddress(Endpoints.NetHttpDuplexWebSocket_Address));
            IWcfDuplexService duplexProxy = factory.CreateChannel();

            Task.Run(() => duplexProxy.Ping(guid));
            Guid returnedGuid = callbackService.CallbackGuid;

            Assert.True(guid == returnedGuid, string.Format("The sent GUID does not match the returned GUID. Sent '{0}', Received: '{1}'", guid, returnedGuid));
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }

    // Force the binding to use WebSocket by setting the WebSocket transport usage to Always.
    [Fact]
    [OuterLoop]
    [ActiveIssue(275)]
    public static void RequestResponseOverWebSocketManually_Echo_RoundTrips_Guid()
    {
        DuplexChannelFactory<IWcfDuplexService> factory = null;
        Guid guid = Guid.NewGuid();

        NetHttpBinding binding = new NetHttpBinding();
        binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;

        WcfDuplexServiceCallback callbackService = new WcfDuplexServiceCallback();
        InstanceContext context = new InstanceContext(callbackService);

        try
        {
            factory = new DuplexChannelFactory<IWcfDuplexService>(context, binding, new EndpointAddress(Endpoints.NetHttpWebSocketTransport_Address));
            IWcfDuplexService duplexProxy = factory.CreateChannel();

            Task.Run(() => duplexProxy.Ping(guid));
            Guid returnedGuid = callbackService.CallbackGuid;

            Assert.True(guid == returnedGuid, string.Format("The sent GUID does not match the returned GUID. Sent '{0}', Received: '{1}'", guid, returnedGuid));
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }
}