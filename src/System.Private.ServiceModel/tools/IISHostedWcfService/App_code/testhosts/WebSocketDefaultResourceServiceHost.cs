// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class WebSocketDefaultResourceServiceHostFactory : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "WebSocket"; } }

        protected override Binding GetBinding()
        {
            var binding = new NetHttpBinding();
            binding.WebSocketSettings.TransportUsage = WebSocketTransportUsage.Always;
            return binding;
        }

        public WebSocketDefaultResourceServiceHostFactory(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
