// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "TcpDefault.svc")]
    public class TcpDefaultResourceTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "tcp-default"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding() { PortSharingEnabled = false };
        }

        public TcpDefaultResourceTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
