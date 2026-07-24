// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace System.ServiceModel.MsmqIntegration
{
    // Send-side channel for MsmqIntegrationBinding. Targets classic MSMQ
    // applications that exchange raw payloads (no SOAP envelope). The
    // channel writes the encoded WCF message bytes directly to MSMQ as
    // the message body. Full MsmqMessage<T> / MsmqIntegrationMessageProperty
    // support (priority, label, ack types, etc.) lands in a follow-up
    // slice that ports the netfx integration message-property layer.
    [SupportedOSPlatform("windows")]
    internal sealed class MsmqIntegrationOutputChannel : ChannelBase, IOutputChannel
    {
        private readonly MsmqIntegrationOutputChannelFactory _factory;
        private readonly EndpointAddress _remoteAddress;
        private readonly Uri _via;
        private readonly string _formatName;

        internal MsmqIntegrationOutputChannel(MsmqIntegrationOutputChannelFactory factory, EndpointAddress remoteAddress, Uri via)
            : base(factory)
        {
            _factory = factory;
            _remoteAddress = remoteAddress;
            _via = via;
            _formatName = MsmqUri.UriToFormatNameByScheme(via);
        }

        public EndpointAddress RemoteAddress => _remoteAddress;

        public Uri Via => _via;

        public void Send(Message message) => Send(message, DefaultSendTimeout);

        public void Send(Message message, TimeSpan timeout)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            ThrowIfDisposedOrNotOpen();

            ArraySegment<byte> encoded = _factory.MessageEncoder.WriteMessage(
                message, int.MaxValue, _factory.BufferManager);
            try
            {
                MsmqIntegrationMessageProperty property = MsmqIntegrationMessageProperty.Get(message);
                MsmqMessagingInterop.Send(
                    _formatName,
                    encoded.Array,
                    encoded.Offset,
                    encoded.Count,
                    property,
                    _factory.BindingElement.ExactlyOnce,
                    _factory.BindingElement.TimeToLive,
                    timeout);
            }
            finally
            {
                _factory.BufferManager.ReturnBuffer(encoded.Array);
            }
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
            => BeginSend(message, DefaultSendTimeout, callback, state);

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            => Task.Run(() => Send(message, timeout)).ToApm(callback, state);

        public void EndSend(IAsyncResult result) => result.ToApmEnd();

        protected override void OnAbort() { }

        protected override void OnClose(TimeSpan timeout) { }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            => Task.CompletedTask.ToApm(callback, state);

        protected override void OnEndClose(IAsyncResult result) => result.ToApmEnd();

        protected override void OnOpen(TimeSpan timeout) { }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            => Task.CompletedTask.ToApm(callback, state);

        protected override void OnEndOpen(IAsyncResult result) => result.ToApmEnd();

        protected override Task OnOpenAsync(TimeSpan timeout) => Task.CompletedTask;

        protected override Task OnCloseAsync(TimeSpan timeout) => Task.CompletedTask;
    }
}
