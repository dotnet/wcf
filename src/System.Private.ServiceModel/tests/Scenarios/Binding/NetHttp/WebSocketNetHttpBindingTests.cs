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

public static class Binding_NetHttp_WebSocketNetHttpBindingTests
{
    static TimeSpan maxTestWaitTime = TimeSpan.FromSeconds(10);

    [Fact]
    [OuterLoop]
    public static void TransportUsageAlways_WebSockets_Synchronous_Call()
    {
        DuplexClientBase<IWcfDuplexService> duplexService = null;
        Guid guid = Guid.NewGuid();

        try
        {
            NetHttpBinding binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;

            WcfDuplexServiceCallback callbackService = new WcfDuplexServiceCallback();
            InstanceContext context = new InstanceContext(callbackService);

            var uri = new Uri(Endpoints.HttpBaseAddress_NetHttpWebSocket);
            UriBuilder builder = new UriBuilder(Endpoints.HttpBaseAddress_NetHttpWebSocket);
            switch (uri.Scheme.ToLowerInvariant())
            {
                case "http":
                    builder.Scheme = "ws";
                    break;
                case "https":
                    builder.Scheme = "wss";
                    break;
            }

            duplexService = new MyDuplexClientBase<IWcfDuplexService>(context, binding, new EndpointAddress(builder.Uri));
            IWcfDuplexService proxy = duplexService.ChannelFactory.CreateChannel();

            // Ping on another thread.
            proxy.Ping(guid);
            //Task.Run(() => proxy.Ping(guid));
            Guid returnedGuid = callbackService.CallbackGuid;

            Assert.True(guid == returnedGuid,
                string.Format("The sent GUID does not match the returned GUID. Sent '{0}', Received: '{1}'", guid, returnedGuid));

            ((ICommunicationObject)duplexService).Close();
        }
        finally
        {
            if (duplexService != null && duplexService.State != CommunicationState.Closed)
            {
                duplexService.Abort();
            }
        }
    }

    public class WcfDuplexServiceCallback : IWcfDuplexServiceCallback
    {
        private TaskCompletionSource<Guid> _tcs;

        public WcfDuplexServiceCallback()
        {
            _tcs = new TaskCompletionSource<Guid>();
        }

        public Guid CallbackGuid
        {
            get
            {
                if (_tcs.Task.Wait(maxTestWaitTime))
                {
                    return _tcs.Task.Result;
                }
                throw new TimeoutException(string.Format("Not completed within the alloted time of {0}", maxTestWaitTime));
            }
        }

        public void OnPingCallback(Guid guid)
        {
            // Set the result in an async task with a 100ms delay to prevent a race condition
            // where the OnPingCallback hasn't sent the reply to the server before the channel is closed.
            Task.Run(async () =>
            {
                await Task.Delay(100);
                _tcs.SetResult(guid);
            });
        }
    }
}