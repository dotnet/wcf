// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class DuplexWebSocketTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            DuplexWebSocketTestServiceHost serviceHost = new DuplexWebSocketTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class DuplexWebSocketTestServiceHost : TestServiceHostBase<IWcfDuplexService>
    {
        protected override string Address { get { return "http-defaultduplexwebsockets"; } }

        protected override Binding GetBinding()
        {
            return new NetHttpBinding();
        }

        public DuplexWebSocketTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketTransportTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketTransportTestServiceHost serviceHost = new WebSocketTransportTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class WebSocketTransportTestServiceHost : TestServiceHostBase<IWcfDuplexService>
    {
        protected override string Address { get { return "http-requestreplywebsockets-transportusagealways"; } }

        protected override Binding GetBinding()
        {
            NetHttpBinding binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            return binding;
        }

        public WebSocketTransportTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketHttpDuplexBinaryStreamedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketHttpDuplexBinaryStreamedTestServiceHost serviceHost = new WebSocketHttpDuplexBinaryStreamedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
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

        public WebSocketHttpDuplexBinaryStreamedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketHttpRequestReplyBinaryStreamedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketHttpRequestReplyBinaryStreamedTestServiceHost serviceHost = new WebSocketHttpRequestReplyBinaryStreamedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
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

        public WebSocketHttpRequestReplyBinaryStreamedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketHttpsDuplexBinaryStreamedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketHttpsDuplexBinaryStreamedTestServiceHost serviceHost = new WebSocketHttpsDuplexBinaryStreamedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
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

        public WebSocketHttpsDuplexBinaryStreamedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketHttpsDuplexTextStreamedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketHttpsDuplexTextStreamedTestServiceHost serviceHost = new WebSocketHttpsDuplexTextStreamedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
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

        public WebSocketHttpsDuplexTextStreamedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketHttpRequestReplyTextStreamedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketHttpRequestReplyTextStreamedTestServiceHost serviceHost = new WebSocketHttpRequestReplyTextStreamedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
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

        public WebSocketHttpRequestReplyTextStreamedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketHttpDuplexTextStreamedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketHttpDuplexTextStreamedTestServiceHost serviceHost = new WebSocketHttpDuplexTextStreamedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
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

        public WebSocketHttpDuplexTextStreamedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketHttpRequestReplyTextBufferedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketHttpRequestReplyTextBufferedTestServiceHost serviceHost = new WebSocketHttpRequestReplyTextBufferedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
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

        public WebSocketHttpRequestReplyTextBufferedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketHttpRequestReplyBinaryBufferedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketHttpRequestReplyBinaryBufferedTestServiceHost serviceHost = new WebSocketHttpRequestReplyBinaryBufferedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
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

        public WebSocketHttpRequestReplyBinaryBufferedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketHttpDuplexTextBufferedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketHttpDuplexTextBufferedTestServiceHost serviceHost = new WebSocketHttpDuplexTextBufferedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

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

        public WebSocketHttpDuplexTextBufferedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketHttpDuplexBinaryBufferedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketHttpDuplexBinaryBufferedTestServiceHost serviceHost = new WebSocketHttpDuplexBinaryBufferedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

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

        public WebSocketHttpDuplexBinaryBufferedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketHttpsRequestReplyBinaryBufferedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketHttpsRequestReplyBinaryBufferedTestServiceHost serviceHost = new WebSocketHttpsRequestReplyBinaryBufferedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
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
        public WebSocketHttpsRequestReplyBinaryBufferedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketHttpsRequestReplyTextBufferedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketHttpsRequestReplyTextBufferedTestServiceHost serviceHost = new WebSocketHttpsRequestReplyTextBufferedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
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

        public WebSocketHttpsRequestReplyTextBufferedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketHttpsDuplexBinaryBufferedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketHttpsDuplexBinaryBufferedTestServiceHost serviceHost = new WebSocketHttpsDuplexBinaryBufferedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
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
        public WebSocketHttpsDuplexBinaryBufferedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketHttpsDuplexTextBufferedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketHttpsDuplexTextBufferedTestServiceHost serviceHost = new WebSocketHttpsDuplexTextBufferedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
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

        public WebSocketHttpsDuplexTextBufferedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
           : base(serviceType, baseAddresses)
        {
        }
    }

    public class WebSocketHttpVerifyWebSocketsUsedTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            WebSocketHttpVerifyWebSocketsUsedTestServiceHost serviceHost = new WebSocketHttpVerifyWebSocketsUsedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

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

        public WebSocketHttpVerifyWebSocketsUsedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
