// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif

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

    [TestServiceDefinition(Schema = ServiceSchema.WSS, BasePath = "WebSocketHttpsDuplex.svc")]
    [TestServiceDefinition(Schema = ServiceSchema.WS, BasePath = "WebSocketHttpDuplex.svc")]
    public class WebSocketHttpDuplexTestServiceHost : TestServiceHostBase<IWSDuplexService>
    {
        protected override IList<Binding> GetBindings()
        {
            return new List<Binding> {
                GetNetHttpBinding(NetHttpMessageEncoding.Text, TransferMode.Buffered),
                GetNetHttpBinding(NetHttpMessageEncoding.Binary, TransferMode.Buffered),
                GetNetHttpBinding(NetHttpMessageEncoding.Mtom, TransferMode.Buffered),
                GetNetHttpBinding(NetHttpMessageEncoding.Text, TransferMode.Streamed),
                GetNetHttpBinding(NetHttpMessageEncoding.Binary, TransferMode.Streamed),
                GetNetHttpBinding(NetHttpMessageEncoding.Mtom, TransferMode.Streamed),
                GetNetHttpsBinding(NetHttpMessageEncoding.Text, TransferMode.Buffered),
                GetNetHttpsBinding(NetHttpMessageEncoding.Binary, TransferMode.Buffered),
                GetNetHttpsBinding(NetHttpMessageEncoding.Mtom, TransferMode.Buffered),
                GetNetHttpsBinding(NetHttpMessageEncoding.Text, TransferMode.Streamed),
                GetNetHttpsBinding(NetHttpMessageEncoding.Binary, TransferMode.Streamed),
                GetNetHttpsBinding(NetHttpMessageEncoding.Mtom, TransferMode.Streamed)
            };
        }

        private Binding GetNetHttpBinding(NetHttpMessageEncoding messageEncoding, TransferMode transferMode)
        {
            NetHttpBinding binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = transferMode;
            binding.MessageEncoding = messageEncoding;
            binding.Name = Enum.GetName(typeof(TransferMode), transferMode) +
                           Enum.GetName(typeof(NetHttpMessageEncoding), messageEncoding);
            return binding;
        }

        private Binding GetNetHttpsBinding(NetHttpMessageEncoding messageEncoding, TransferMode transferMode)
        {
            NetHttpsBinding binding = new NetHttpsBinding()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = transferMode;
            binding.MessageEncoding = messageEncoding;
            binding.Name = Enum.GetName(typeof(TransferMode), transferMode) +
                           Enum.GetName(typeof(NetHttpMessageEncoding), messageEncoding);
            return binding;
        }

        public WebSocketHttpDuplexTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WSDuplexService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.WSS, BasePath = "WebSocketHttpsRequestReply.svc")]
    [TestServiceDefinition(Schema = ServiceSchema.WS, BasePath = "WebSocketHttpRequestReply.svc")]
    public class WebSocketHttpRequestReplyTestServiceHost : TestServiceHostBase<IWSRequestReplyService>
    {
        protected override IList<Binding> GetBindings()
        {
            return new List<Binding> {
                GetNetHttpBinding(NetHttpMessageEncoding.Text, TransferMode.Buffered),
                GetNetHttpBinding(NetHttpMessageEncoding.Binary, TransferMode.Buffered),
                GetNetHttpBinding(NetHttpMessageEncoding.Mtom, TransferMode.Buffered),
                GetNetHttpBinding(NetHttpMessageEncoding.Text, TransferMode.Streamed),
                GetNetHttpBinding(NetHttpMessageEncoding.Binary, TransferMode.Streamed),
                GetNetHttpBinding(NetHttpMessageEncoding.Mtom, TransferMode.Streamed),
                GetNetHttpsBinding(NetHttpMessageEncoding.Text, TransferMode.Buffered),
                GetNetHttpsBinding(NetHttpMessageEncoding.Binary, TransferMode.Buffered),
                GetNetHttpsBinding(NetHttpMessageEncoding.Mtom, TransferMode.Buffered),
                GetNetHttpsBinding(NetHttpMessageEncoding.Text, TransferMode.Streamed),
                GetNetHttpsBinding(NetHttpMessageEncoding.Binary, TransferMode.Streamed),
                GetNetHttpsBinding(NetHttpMessageEncoding.Mtom, TransferMode.Streamed)
            };
        }

        private Binding GetNetHttpBinding(NetHttpMessageEncoding messageEncoding, TransferMode transferMode)
        {
            NetHttpBinding binding = new NetHttpBinding()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = transferMode;
            binding.MessageEncoding = messageEncoding;
            binding.Name = Enum.GetName(typeof(TransferMode), transferMode) +
                           Enum.GetName(typeof(NetHttpMessageEncoding), messageEncoding);
            return binding;
        }

        private Binding GetNetHttpsBinding(NetHttpMessageEncoding messageEncoding, TransferMode transferMode)
        {
            NetHttpsBinding binding = new NetHttpsBinding()
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB,
            };
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            binding.TransferMode = transferMode;
            binding.MessageEncoding = messageEncoding;
            binding.Name = Enum.GetName(typeof(TransferMode), transferMode) +
                           Enum.GetName(typeof(NetHttpMessageEncoding), messageEncoding);
            return binding;
        }

        public WebSocketHttpRequestReplyTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WSRequestReplyService), baseAddresses)
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
