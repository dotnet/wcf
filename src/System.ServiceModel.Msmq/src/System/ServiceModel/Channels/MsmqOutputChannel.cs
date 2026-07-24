// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    // Send-only IOutputChannel implementation. Serializes the WCF Message
    // through the binding's MessageEncoder and dispatches the bytes to MSMQ
    // via the MSMQ.Messaging.MessageQueue high-level API. Transactional
    // sends are flowed through System.Transactions.Transaction.Current when
    // the channel is built from an ExactlyOnce binding.
    [SupportedOSPlatform("windows")]
    internal sealed class MsmqOutputChannel : ChannelBase, IOutputChannel
    {
        private readonly MsmqOutputChannelFactory _factory;
        private readonly EndpointAddress _remoteAddress;
        private readonly Uri _via;
        private readonly string _formatName;

        internal MsmqOutputChannel(MsmqOutputChannelFactory factory, EndpointAddress remoteAddress, Uri via)
            : base(factory)
        {
            _factory = factory;
            _remoteAddress = remoteAddress;
            _via = via;
            _formatName = MsmqUri.UriToFormatNameByScheme(via);
        }

        public EndpointAddress RemoteAddress => _remoteAddress;

        public Uri Via => _via;

        public void Send(Message message)
        {
            Send(message, DefaultSendTimeout);
        }

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
                SendToQueue(encoded, timeout);
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

        private void SendToQueue(ArraySegment<byte> encoded, TimeSpan timeout)
        {
            // MSMQ.Messaging is loaded reflectively so the rest of the package
            // can be exercised by unit tests on non-Windows hosts that lack
            // the dependency. On Windows, the call delegates to
            // MSMQ.Messaging.MessageQueue.Send(message[, transaction]).
            MsmqMessagingInterop.Send(
                _formatName,
                encoded.Array,
                encoded.Offset,
                encoded.Count,
                _factory.BindingElement.ExactlyOnce,
                _factory.BindingElement.TimeToLive,
                timeout);
        }

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
