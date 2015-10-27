// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;
using System.ServiceModel.Channels;
using WcfService.CertificateResources;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal abstract class WebSocketHttpsDuplexBinaryStreamed : EndpointResource<WSDuplexService, IWSDuplexService>
    {
        protected override string Address { get { return "WebSocketHttpsDuplexBinaryStreamed"; } }

        protected override string Protocol { get { return BaseAddressResource.Https; } }

        protected override Binding GetBinding()
        {
            int DefaultMaxReceivedMessageSize = 64 * 1024 * 1024;

            BinaryMessageEncodingBindingElement binaryMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();

            HttpsTransportBindingElement httpsTransportBindingElement = new HttpsTransportBindingElement();
            httpsTransportBindingElement.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            httpsTransportBindingElement.MaxReceivedMessageSize = DefaultMaxReceivedMessageSize;
            httpsTransportBindingElement.MaxBufferSize = DefaultMaxReceivedMessageSize;
            httpsTransportBindingElement.TransferMode = TransferMode.Streamed;

            CustomBinding binding = new CustomBinding(binaryMessageEncodingBindingElement, httpsTransportBindingElement);

            return binding;
        }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeHttpsPort;
        }

        protected override void ModifyHost(ServiceHost serviceHost, ResourceRequestContext context)
        {
            // Ensure the https certificate is installed before this endpoint resource is used
            CertificateResourceHelpers.EnsureSslPortCertificateInstalled(context.BridgeConfiguration);

            base.ModifyHost(serviceHost, context);
        }
    }
}
