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
    [TestServiceDefinition(Schema = ServiceSchema.HTTPS, BasePath = "BasicHttps.svc")]
    public class BasicHttpsTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override IList<Binding> GetBindings()
        {
            return new List<Binding> { GetBasicHttpsBinding(WSMessageEncoding.Text), GetBasicHttpsBinding(WSMessageEncoding.Mtom) };
        }

        private Binding GetBasicHttpsBinding(WSMessageEncoding messageEncoding)
        {
            var binding = new BasicHttpsBinding();
            binding.MessageEncoding = messageEncoding;
            binding.Name = Enum.GetName(typeof(WSMessageEncoding), messageEncoding);
            return binding;
        }

        public BasicHttpsTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
