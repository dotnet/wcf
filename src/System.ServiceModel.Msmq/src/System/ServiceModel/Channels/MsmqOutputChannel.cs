// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;
using System.Transactions;

namespace System.ServiceModel.Channels
{
    // Send-only IOutputChannel for NetMsmqBinding. Serializes the WCF Message
    // through the binding's MessageEncoder and hands the bytes to the native
    // MSMQ send path (mqrt.dll via MsmqMessagingInterop). Transactional sends
    // flow the caller's ambient System.Transactions transaction when the
    // channel is built from an ExactlyOnce binding.
    [SupportedOSPlatform("windows")]
    internal sealed class MsmqOutputChannel : MsmqOutputChannelBase, IOutputChannel
    {
        private readonly MsmqOutputChannelFactory _factory;

        internal MsmqOutputChannel(MsmqOutputChannelFactory factory, EndpointAddress remoteAddress, Uri via)
            : base(factory, remoteAddress, via)
        {
            _factory = factory;
        }

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
    }
}
