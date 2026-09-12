// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Transactions;

namespace System.ServiceModel.Channels
{
    // Shared send-side behaviour for every MSMQ output channel
    // (NetMsmqBinding's datagram and session channels, and
    // MsmqIntegrationBinding's raw-payload channel).
    //
    // Keeping the send pipeline in one place matters for correctness, not
    // just tidiness: the transport properties written for each message, the
    // capture of the ambient transaction, the message-size quota and the
    // deferral of address translation to Open are all easy to fix in one
    // channel and forget in the other two.
    [SupportedOSPlatform("windows")]
    internal abstract class MsmqOutputChannelBase : ChannelBase
    {
        private readonly EndpointAddress _remoteAddress;
        private readonly Uri _via;
        private string _formatName;

        protected MsmqOutputChannelBase(ChannelManagerBase channelManager, EndpointAddress remoteAddress, Uri via)
            : base(channelManager)
        {
            _remoteAddress = remoteAddress ?? throw new ArgumentNullException(nameof(remoteAddress));
            _via = via ?? throw new ArgumentNullException(nameof(via));
        }

        public EndpointAddress RemoteAddress => _remoteAddress;

        public Uri Via => _via;

        // Resolved during Open rather than in the constructor. CreateChannel
        // must hand back a channel object even for an address this transport
        // cannot reach; surfacing the failure from Open is what the
        // CommunicationObject contract promises and what callers that wrap
        // Open in a try/catch expect.
        protected string FormatName => _formatName;

        protected abstract MsmqBindingElementBase BindingElement { get; }

        protected abstract MsmqUri.IAddressTranslator AddressTranslator { get; }

        // Quota handed to the message encoder. Falls back to int.MaxValue when
        // the binding asks for an unbounded size.
        protected int MaxMessageSize =>
            (int)Math.Min(BindingElement.MaxReceivedMessageSize, int.MaxValue);

        public void Send(Message message) => Send(message, DefaultSendTimeout);

        public void Send(Message message, TimeSpan timeout)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            ThrowIfDisposedOrNotOpen();
            OnSend(message, timeout, Transaction.Current);
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
            => BeginSend(message, DefaultSendTimeout, callback, state);

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            ThrowIfDisposedOrNotOpen();

            // Transaction.Current is [ThreadStatic] and does not flow onto a
            // thread-pool thread. Capturing it here, on the caller's thread, is
            // what keeps an asynchronous send inside the caller's ambient
            // transaction; reading it inside the Task body would always observe
            // null and silently commit the send outside the transaction.
            Transaction ambientTransaction = Transaction.Current;
            return Task.Run(() => OnSend(message, timeout, ambientTransaction)).ToApm(callback, state);
        }

        public void EndSend(IAsyncResult result) => result.ToApmEnd();

        // Encodes and dispatches one message. Overridden by the integration
        // channel, which serializes a raw MSMQ payload instead of a SOAP
        // envelope.
        protected abstract void OnSend(Message message, TimeSpan timeout, Transaction ambientTransaction);

        protected override void OnOpen(TimeSpan timeout) => ResolveFormatName();

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            ResolveFormatName();
            return Task.CompletedTask.ToApm(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result) => result.ToApmEnd();

        protected override Task OnOpenAsync(TimeSpan timeout)
        {
            ResolveFormatName();
            return Task.CompletedTask;
        }

        protected override void OnAbort()
        {
        }

        protected override void OnClose(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            => Task.CompletedTask.ToApm(callback, state);

        protected override void OnEndClose(IAsyncResult result) => result.ToApmEnd();

        protected override Task OnCloseAsync(TimeSpan timeout) => Task.CompletedTask;

        private void ResolveFormatName()
        {
            _formatName = AddressTranslator.UriToFormatName(_via);
        }
    }
}
