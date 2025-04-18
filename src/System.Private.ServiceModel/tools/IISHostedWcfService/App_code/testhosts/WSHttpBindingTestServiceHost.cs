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
    [TestServiceDefinition(BasePath = "WSHttp.svc", Schema = ServiceSchema.HTTP)]
    public class WSHttpBindingTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override IList<Binding> GetBindings()
        {
            return new List<Binding> { GetWSHttpBinding(WSMessageEncoding.Text), GetWSHttpBinding(WSMessageEncoding.Mtom) };
        }

        private Binding GetWSHttpBinding(WSMessageEncoding messageEncoding)
        {
            var binding = new WSHttpBinding(SecurityMode.None);
            binding.MessageEncoding = messageEncoding;
            binding.Name = Enum.GetName(typeof(WSMessageEncoding), messageEncoding);
            return binding;
        }

        public WSHttpBindingTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
