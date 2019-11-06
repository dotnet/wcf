// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.WS, BasePath = "DuplexWebSocket.svc")]
    public class DuplexWebSocketTestServiceHost : TestServiceHostBase<IWcfDuplexService>
    {
        protected override string Address { get { return "http-defaultduplexwebsockets"; } }

        protected override Binding GetBinding()
        {
            return new NetHttpBinding();
        }

        public DuplexWebSocketTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfWebSocketService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WS, BasePath = "WebSocketTransport.svc")]
    public class WebSocketTransportTestServiceHost : TestServiceHostBase<IWcfDuplexService>
    {
        protected override string Address { get { return "http-requestreplywebsockets-transportusagealways"; } }

        protected override Binding GetBinding()
        {
            NetHttpBinding binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            return binding;
        }

        public WebSocketTransportTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfWebSocketTransportUsageAlwaysService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WS, BasePath = "WebSocketHttpDuplexBinaryStreamed.svc")]
    public class WebSocketHttpDuplexBinaryStreamedTestServiceHost : TestServiceHostBase<IWSDuplexService>
    {
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

        public WebSocketHttpDuplexBinaryStreamedTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WSDuplexService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WS, BasePath = "WebSocketHttpRequestReplyBinaryStreamed.svc")]
    public class WebSocketHttpRequestReplyBinaryStreamedTestServiceHost : TestServiceHostBase<IWSRequestReplyService>
    {
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

        public WebSocketHttpRequestReplyBinaryStreamedTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WSRequestReplyService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WSS, BasePath = "WebSocketHttpsDuplexBinaryStreamed.svc")]
    public class WebSocketHttpsDuplexBinaryStreamedTestServiceHost : TestServiceHostBase<IWSDuplexService>
    {
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

        public WebSocketHttpsDuplexBinaryStreamedTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WSDuplexService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WSS, BasePath = "WebSocketHttpsDuplexTextStreamed.svc")]
    public class WebSocketHttpsDuplexTextStreamedTestServiceHost : TestServiceHostBase<IWSDuplexService>
    {
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

        public WebSocketHttpsDuplexTextStreamedTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WSDuplexService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WS, BasePath = "WebSocketHttpRequestReplyTextStreamed.svc")]
    public class WebSocketHttpRequestReplyTextStreamedTestServiceHost : TestServiceHostBase<IWSRequestReplyService>
    {
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

        public WebSocketHttpRequestReplyTextStreamedTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WSRequestReplyService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WS, BasePath = "WebSocketHttpDuplexTextStreamed.svc")]
    public class WebSocketHttpDuplexTextStreamedTestServiceHost : TestServiceHostBase<IWSDuplexService>
    {
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

        public WebSocketHttpDuplexTextStreamedTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WSDuplexService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WS, BasePath = "WebSocketHttpRequestReplyTextBuffered.svc")]
    public class WebSocketHttpRequestReplyTextBufferedTestServiceHost : TestServiceHostBase<IWSRequestReplyService>
    {
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

        public WebSocketHttpRequestReplyTextBufferedTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WSRequestReplyService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WS, BasePath = "WebSocketHttpRequestReplyBinaryBuffered.svc")]
    public class WebSocketHttpRequestReplyBinaryBufferedTestServiceHost : TestServiceHostBase<IWSRequestReplyService>
    {
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

        public WebSocketHttpRequestReplyBinaryBufferedTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WSRequestReplyService), baseAddresses)
        {
        }
    }


    [TestServiceDefinition(Schema = ServiceSchema.WS, BasePath = "WebSocketHttpDuplexTextBuffered.svc")]
    public class WebSocketHttpDuplexTextBufferedTestServiceHost : TestServiceHostBase<IWSDuplexService>
    {
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

        public WebSocketHttpDuplexTextBufferedTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WSDuplexService), baseAddresses)
        {
        }
    }


    [TestServiceDefinition(Schema = ServiceSchema.WS, BasePath = "WebSocketHttpDuplexBinaryBuffered.svc")]
    public class WebSocketHttpDuplexBinaryBufferedTestServiceHost : TestServiceHostBase<IWSDuplexService>
    {
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

        public WebSocketHttpDuplexBinaryBufferedTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WSDuplexService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WSS, BasePath = "WebSocketHttpsRequestReplyBinaryBuffered.svc")]
    public class WebSocketHttpsRequestReplyBinaryBufferedTestServiceHost : TestServiceHostBase<IWSRequestReplyService>
    {
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
        public WebSocketHttpsRequestReplyBinaryBufferedTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WSRequestReplyService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WSS, BasePath = "WebSocketHttpsRequestReplyTextBuffered.svc")]
    public class WebSocketHttpsRequestReplyTextBufferedTestServiceHost : TestServiceHostBase<IWSRequestReplyService>
    {
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

        public WebSocketHttpsRequestReplyTextBufferedTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WSRequestReplyService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WSS, BasePath = "WebSocketHttpsDuplexBinaryBuffered.svc")]
    public class WebSocketHttpsDuplexBinaryBufferedTestServiceHost : TestServiceHostBase<IWSDuplexService>
    {
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
        public WebSocketHttpsDuplexBinaryBufferedTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WSDuplexService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WSS, BasePath = "WebSocketHttpsDuplexTextBuffered.svc")]
    public class WebSocketHttpsDuplexTextBufferedTestServiceHost : TestServiceHostBase<IWSDuplexService>
    {
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

        public WebSocketHttpsDuplexTextBufferedTestServiceHost(params Uri[] baseAddresses)
           : base(typeof(WSDuplexService), baseAddresses)
        {
        }
    }


    [TestServiceDefinition(Schema = ServiceSchema.WS, BasePath = "WebSocketHttpVerifyWebSocketsUsed.svc")]
    public class WebSocketHttpVerifyWebSocketsUsedTestServiceHost : TestServiceHostBase<IVerifyWebSockets>
    {
        protected override string Address { get { return "WebSocketHttpVerifyWebSocketsUsed"; } }

        protected override Binding GetBinding()
        {
            NetHttpBinding binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            // This setting lights up the code path that calls the operation attributed with ConnectionOpenedAction
            binding.WebSocketSettings.CreateNotificationOnConnection = true;
            return binding;
        }

        public WebSocketHttpVerifyWebSocketsUsedTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(VerifyWebSockets), baseAddresses)
        {
        }
    }
}
