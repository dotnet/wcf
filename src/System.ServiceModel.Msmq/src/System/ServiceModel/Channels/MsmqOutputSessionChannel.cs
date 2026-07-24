// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    // Send-only IOutputSessionChannel.
    //
    // NOTE: this implementation sends one MSMQ message per Send() call,
    // exposing a per-channel Guid session id via IOutputSession. It does
    // NOT yet produce the .NET Framework "session-gram" wire format (a
    // single concatenated MSMQ message carrying the session preamble +
    // every buffered application message, emitted on Close). That format
    // is required for full interoperability with netfx WCF endpoints
    // hosted with SessionMode.Required. Tracked as a follow-up; see
    // plan.md, slice 4b decisions.
    [SupportedOSPlatform("windows")]
    internal sealed class MsmqOutputSessionChannel : ChannelBase, IOutputSessionChannel
    {
        private readonly MsmqOutputSessionChannelFactory _factory;
        private readonly EndpointAddress _remoteAddress;
        private readonly Uri _via;
        private readonly string _formatName;
        private readonly OutputSession _session;

        internal MsmqOutputSessionChannel(MsmqOutputSessionChannelFactory factory, EndpointAddress remoteAddress, Uri via)
            : base(factory)
        {
            _factory = factory;
            _remoteAddress = remoteAddress;
            _via = via;
            _formatName = MsmqUri.UriToFormatNameByScheme(via);
            _session = new OutputSession();
        }

        public EndpointAddress RemoteAddress => _remoteAddress;

        public Uri Via => _via;

        public IOutputSession Session => _session;

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
                MsmqMessagingInterop.Send(
                    _formatName,
                    encoded.Array,
                    encoded.Offset,
                    encoded.Count,
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

        private sealed class OutputSession : IOutputSession
        {
            private readonly string _id = "uuid:" + Guid.NewGuid().ToString("D");
            public string Id => _id;
        }
    }
}
