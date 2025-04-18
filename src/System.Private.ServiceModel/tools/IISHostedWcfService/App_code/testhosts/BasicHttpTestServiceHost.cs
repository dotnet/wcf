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
    [TestServiceDefinition(BasePath = "BasicHttp.svc", Schema = ServiceSchema.HTTP)]
    public class BasicHttpTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override IList<Binding> GetBindings()
        {
            return new List<Binding> { GetBasicHttpBinding(WSMessageEncoding.Text), GetBasicHttpBinding(WSMessageEncoding.Mtom) };
        }

        private Binding GetBasicHttpBinding(WSMessageEncoding messageEncoding)
        {
            var binding = new BasicHttpBinding();
            binding.MessageEncoding = messageEncoding;
            binding.Name = Enum.GetName(typeof(WSMessageEncoding), messageEncoding);
            return binding;
        }

        public BasicHttpTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
