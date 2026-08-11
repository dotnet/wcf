// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Versioning;
using System.ServiceModel.Channels;
using System.Transactions;

namespace System.ServiceModel.MsmqIntegration
{
    // Send-side channel for MsmqIntegrationBinding. Targets classic MSMQ
    // applications that exchange raw payloads rather than SOAP envelopes, so
    // the body is produced by MsmqIntegrationSerializer from
    // MsmqIntegrationMessageProperty.Body according to the binding's
    // SerializationFormat — no message encoder is involved.
    [SupportedOSPlatform("windows")]
    internal sealed class MsmqIntegrationOutputChannel : MsmqOutputChannelBase, IOutputChannel
    {
        private readonly MsmqIntegrationOutputChannelFactory _factory;

        internal MsmqIntegrationOutputChannel(MsmqIntegrationOutputChannelFactory factory, EndpointAddress remoteAddress, Uri via)
            : base(factory, remoteAddress, via)
        {
            _factory = factory;
        }

        protected override MsmqBindingElementBase BindingElement => _factory.BindingElement;

        protected override MsmqUri.IAddressTranslator AddressTranslator => _factory.BindingElement.AddressTranslator;

        protected override void OnSend(Message message, TimeSpan timeout, Transaction ambientTransaction)
        {
            MsmqIntegrationMessageProperty property = MsmqIntegrationMessageProperty.Get(message);
            if (property == null)
            {
                throw new CommunicationException(SR.MsmqMessageDoesntHaveIntegrationProperty);
            }

            byte[] body = _factory.Serializer.Serialize(property);
            MsmqMessagingInterop.Send(
                FormatName,
                body,
                0,
                body.Length,
                property,
                _factory.BindingElement,
                timeout,
                ambientTransaction);
        }
    }
}
