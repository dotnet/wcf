// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF.Channels;
#else
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif

namespace WcfService
{
    //public class BasicAuthTestServiceHostFactory : ServiceHostFactory
    //{
    //    protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
    //    {
    //        BasicAuthTestServiceHost serviceHost = new BasicAuthTestServiceHost(baseAddresses);
    //        return serviceHost;
    //    }
    //}

    [TestServiceDefinition(BasePath = "BasicAuth.svc", Schema = ServiceSchema.HTTPS)]
    public class BasicAuthTestServiceHost : TestServiceHostBase<IWcfCustomUserNameService>
    {
        protected override string Address { get { return "https-basic"; } }


        protected override Binding GetBinding()
        {
            var binding = new BasicHttpsBinding(BasicHttpsSecurityMode.Transport);
            return binding;
        }

        public BasicAuthTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfUserNameService), baseAddresses)
        {
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
            AuthenticationResourceHelper.ConfigureServiceHostUseBasicAuth(this);
        }
    }
}
