// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;
using System.ServiceModel.Channels;
using WcfService.CertificateResources;
using WcfTestBridgeCommon;

namespace WcfService.TestResources
{
    internal class DuplexWebSocketResource : EndpointResource<WcfWebSocketService, IWcfDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeWebSocketPort;
        }

        protected override string Address { get { return "http-defaultduplexwebsockets"; } }

        protected override Binding GetBinding()
        {
            return new NetHttpBinding();
        }
    }

    internal class WebSocketTransportResource : EndpointResource<WcfWebSocketTransportUsageAlwaysService, IWcfDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeWebSocketPort;
        }

        protected override string Address { get { return "http-requestreplywebsockets-transportusagealways"; } }

        protected override Binding GetBinding()
        {
            NetHttpBinding binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            return binding;
        }
    }

    internal class WebSocketHttpDuplexBinaryStreamedResource : EndpointResource<WSDuplexService, IWSDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpDuplexBinaryStreamedResource"; } }

        protected override Binding GetBinding()
        {
            NetHttpBinding binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = TransferMode.Streamed;
            binding.MessageEncoding = NetHttpMessageEncoding.Binary;
            return binding;
        }
    }

    internal class WebSocketHttpRequestReplyBinaryStreamedResource : EndpointResource<WSRequestReplyService, IWSRequestReplyService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpRequestReplyBinaryStreamedResource"; } }

        protected override Binding GetBinding()
        {
            NetHttpBinding binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = TransferMode.Streamed;
            binding.MessageEncoding = NetHttpMessageEncoding.Binary;
            return binding;
        }
    }

    internal class WebSocketHttpsDuplexBinaryStreamedResource : EndpointResource<WSDuplexService, IWSDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Https; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeSecureWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpsDuplexBinaryStreamedResource"; } }

        protected override Binding GetBinding()
        {
            BinaryMessageEncodingBindingElement binaryMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
            HttpsTransportBindingElement httpsTransportBindingElement = new HttpsTransportBindingElement()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
                TransferMode = TransferMode.Streamed
            };
            httpsTransportBindingElement.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            CustomBinding binding = new CustomBinding(binaryMessageEncodingBindingElement, httpsTransportBindingElement);
            return binding;
        }

        protected override void ModifyHost(ServiceHost serviceHost, ResourceRequestContext context)
        {
            // Ensure the https certificate is installed before this endpoint resource is used
            CertificateResourceHelpers.EnsureSslPortCertificateInstalled(context.BridgeConfiguration, context.BridgeConfiguration.BridgeSecureWebSocketPort);

            base.ModifyHost(serviceHost, context);
        }
    }

    internal class WebSocketHttpsDuplexTextStreamedResource : EndpointResource<WSDuplexService, IWSDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Https; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeSecureWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpsDuplexTextStreamedResource"; } }

        protected override Binding GetBinding()
        {
            TextMessageEncodingBindingElement textMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
            HttpsTransportBindingElement httpsTransportBindingElement = new HttpsTransportBindingElement()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
                TransferMode = TransferMode.Streamed
            };
            httpsTransportBindingElement.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            CustomBinding binding = new CustomBinding(textMessageEncodingBindingElement, httpsTransportBindingElement);
            return binding;
        }

        protected override void ModifyHost(ServiceHost serviceHost, ResourceRequestContext context)
        {
            // Ensure the https certificate is installed before this endpoint resource is used
            CertificateResourceHelpers.EnsureSslPortCertificateInstalled(context.BridgeConfiguration, context.BridgeConfiguration.BridgeSecureWebSocketPort);

            base.ModifyHost(serviceHost, context);
        }
    }

    internal class WebSocketHttpRequestReplyTextStreamedResource : EndpointResource<WSRequestReplyService, IWSRequestReplyService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpRequestReplyTextStreamedResource"; } }

        protected override Binding GetBinding()
        {
            NetHttpBinding binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = TransferMode.Streamed;
            binding.MessageEncoding = NetHttpMessageEncoding.Text;
            return binding;
        }
    }

    internal class WebSocketHttpDuplexTextStreamedResource : EndpointResource<WSDuplexService, IWSDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpDuplexTextStreamedResource"; } }

        protected override Binding GetBinding()
        {
            NetHttpBinding binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = TransferMode.Streamed;
            binding.MessageEncoding = NetHttpMessageEncoding.Text;
            return binding;
        }
    }

    internal class WebSocketHttpRequestReplyTextBufferedResource : EndpointResource<WSRequestReplyService, IWSRequestReplyService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpRequestReplyTextBufferedResource"; } }

        protected override Binding GetBinding()
        {
            NetHttpBinding binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.MessageEncoding = NetHttpMessageEncoding.Text;
            return binding;
        }
    }

    internal class WebSocketHttpRequestReplyBinaryBufferedResource : EndpointResource<WSRequestReplyService, IWSRequestReplyService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpRequestReplyBinaryBufferedResource"; } }

        protected override Binding GetBinding()
        {
            NetHttpBinding binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.MessageEncoding = NetHttpMessageEncoding.Binary;
            return binding;
        }
    }

    internal class WebSocketHttpDuplexTextBufferedResource : EndpointResource<WSDuplexService, IWSDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpDuplexTextBufferedResource"; } }

        protected override Binding GetBinding()
        {
            NetHttpBinding binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.MessageEncoding = NetHttpMessageEncoding.Text;
            return binding;
        }
    }

    internal class WebSocketHttpDuplexBinaryBufferedResource : EndpointResource<WSDuplexService, IWSDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Http; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpDuplexBinaryBufferedResource"; } }

        protected override Binding GetBinding()
        {
            NetHttpBinding binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.MessageEncoding = NetHttpMessageEncoding.Binary;
            return binding;
        }
    }

    internal class WebSocketHttpsRequestReplyBinaryBufferedResource : EndpointResource<WSRequestReplyService, IWSRequestReplyService>
    {
        protected override string Protocol { get { return BaseAddressResource.Https; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeSecureWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpsRequestReplyBinaryBufferedResource"; } }

        protected override Binding GetBinding()
        {
            BinaryMessageEncodingBindingElement binaryMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
            HttpsTransportBindingElement httpsTransportBindingElement = new HttpsTransportBindingElement()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
            };
            httpsTransportBindingElement.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            CustomBinding binding = new CustomBinding(binaryMessageEncodingBindingElement, httpsTransportBindingElement);
            return binding;
        }

        protected override void ModifyHost(ServiceHost serviceHost, ResourceRequestContext context)
        {
            // Ensure the https certificate is installed before this endpoint resource is used
            CertificateResourceHelpers.EnsureSslPortCertificateInstalled(context.BridgeConfiguration, context.BridgeConfiguration.BridgeSecureWebSocketPort);

            base.ModifyHost(serviceHost, context);
        }
    }

    internal class WebSocketHttpsRequestReplyTextBufferedResource : EndpointResource<WSRequestReplyService, IWSRequestReplyService>
    {
        protected override string Protocol { get { return BaseAddressResource.Https; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeSecureWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpsRequestReplyTextBufferedResource"; } }

        protected override Binding GetBinding()
        {
            TextMessageEncodingBindingElement textMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
            HttpsTransportBindingElement httpsTransportBindingElement = new HttpsTransportBindingElement()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB
            };
            httpsTransportBindingElement.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            CustomBinding binding = new CustomBinding(textMessageEncodingBindingElement, httpsTransportBindingElement);
            return binding;
        }

        protected override void ModifyHost(ServiceHost serviceHost, ResourceRequestContext context)
        {
            // Ensure the https certificate is installed before this endpoint resource is used
            CertificateResourceHelpers.EnsureSslPortCertificateInstalled(context.BridgeConfiguration, context.BridgeConfiguration.BridgeSecureWebSocketPort);

            base.ModifyHost(serviceHost, context);
        }
    }

    internal class WebSocketHttpsDuplexBinaryBufferedResource : EndpointResource<WSDuplexService, IWSDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Https; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeSecureWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpsDuplexBinaryBufferedResource"; } }

        protected override Binding GetBinding()
        {
            BinaryMessageEncodingBindingElement binaryMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
            HttpsTransportBindingElement httpsTransportBindingElement = new HttpsTransportBindingElement()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
            };
            httpsTransportBindingElement.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            CustomBinding binding = new CustomBinding(binaryMessageEncodingBindingElement, httpsTransportBindingElement);
            return binding;
        }

        protected override void ModifyHost(ServiceHost serviceHost, ResourceRequestContext context)
        {
            // Ensure the https certificate is installed before this endpoint resource is used
            CertificateResourceHelpers.EnsureSslPortCertificateInstalled(context.BridgeConfiguration, context.BridgeConfiguration.BridgeSecureWebSocketPort);

            base.ModifyHost(serviceHost, context);
        }
    }

    internal class WebSocketHttpsDuplexTextBufferedResource : EndpointResource<WSDuplexService, IWSDuplexService>
    {
        protected override string Protocol { get { return BaseAddressResource.Https; } }

        protected override int GetPort(ResourceRequestContext context)
        {
            return context.BridgeConfiguration.BridgeSecureWebSocketPort;
        }

        protected override string Address { get { return "WebSocketHttpsDuplexTextBufferedResource"; } }

        protected override Binding GetBinding()
        {
            TextMessageEncodingBindingElement textMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
            HttpsTransportBindingElement httpsTransportBindingElement = new HttpsTransportBindingElement()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB
            };
            httpsTransportBindingElement.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            CustomBinding binding = new CustomBinding(textMessageEncodingBindingElement, httpsTransportBindingElement);
            return binding;
        }

        protected override void ModifyHost(ServiceHost serviceHost, ResourceRequestContext context)
        {
            // Ensure the https certificate is installed before this endpoint resource is used
            CertificateResourceHelpers.EnsureSslPortCertificateInstalled(context.BridgeConfiguration, context.BridgeConfiguration.BridgeSecureWebSocketPort);

            base.ModifyHost(serviceHost, context);
        }
    }
}
