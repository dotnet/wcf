// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;
using System.Transactions;

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
    internal sealed class MsmqOutputSessionChannel : MsmqOutputChannelBase, IOutputSessionChannel
    {
        private readonly MsmqOutputSessionChannelFactory _factory;
        private readonly OutputSession _session = new OutputSession();

        internal MsmqOutputSessionChannel(MsmqOutputSessionChannelFactory factory, EndpointAddress remoteAddress, Uri via)
            : base(factory, remoteAddress, via)
        {
            _factory = factory;
        }

        public IOutputSession Session => _session;

        protected override MsmqBindingElementBase BindingElement => _factory.BindingElement;

        protected override MsmqUri.IAddressTranslator AddressTranslator => _factory.BindingElement.AddressTranslator;

        protected override void OnSend(Message message, TimeSpan timeout, Transaction ambientTransaction)
        {
            ArraySegment<byte> encoded = _factory.MessageEncoder.WriteMessage(
                message, MaxMessageSize, _factory.BufferManager);
            try
            {
                MsmqMessagingInterop.Send(
                    FormatName,
                    encoded.Array,
                    encoded.Offset,
                    encoded.Count,
                    property: null,
                    _factory.BindingElement,
                    timeout,
                    ambientTransaction);
            }
            finally
            {
                _factory.BufferManager.ReturnBuffer(encoded.Array);
            }
        }

        private sealed class OutputSession : IOutputSession
        {
            public string Id { get; } = "uuid:" + Guid.NewGuid().ToString("D");
        }
    }
}
