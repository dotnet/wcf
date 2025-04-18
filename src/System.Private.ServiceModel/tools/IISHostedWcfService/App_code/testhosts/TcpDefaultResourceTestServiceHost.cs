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
using System.Web.Hosting;
#endif

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.NETTCP | ServiceSchema.NETPIPE, BasePath = "TcpDefault.svc")]
    public class TcpDefaultResourceTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override IList<Binding> GetBindings()
        {
            var bindings = new List<Binding>();
            bindings.Add(GetNetTcpBinding());

#if !NET
            if (!HostingEnvironment.IsHosted)
            {
                bindings.Add(GetNetNamedPipeBinding());
            }
#endif
            return bindings;
        }

        private Binding GetNetTcpBinding()
        {
            return new NetTcpBinding() { Name = "tcp-default" };
        }

#if !NET
        private Binding GetNetNamedPipeBinding()
        {
            return new NetNamedPipeBinding() { Name = "namedpipe-default" };
        }
#endif

        public TcpDefaultResourceTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
