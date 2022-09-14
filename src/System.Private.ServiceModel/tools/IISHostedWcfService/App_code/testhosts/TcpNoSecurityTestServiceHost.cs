// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.NETTCP | ServiceSchema.NETPIPE, BasePath = "TcpNoSecurity.svc")]
    public class TcpNoSecurityTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override IList<Binding> GetBindings()
        {
            return new List<Binding> { GetNetTcpBinding(), GetNetNamedPipeBinding() };
        }

        private Binding GetNetTcpBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false , Name = "tcp-nosecurity" };
        }

        private Binding GetNetNamedPipeBinding()
        {
            return new NetNamedPipeBinding(NetNamedPipeSecurityMode.None) { Name = "namedpipe-nosecurity" };
        }

        public TcpNoSecurityTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
