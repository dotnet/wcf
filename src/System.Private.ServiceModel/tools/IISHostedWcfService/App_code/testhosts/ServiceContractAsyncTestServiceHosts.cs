// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class ServiceContractAsyncIntOutTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceContractAsyncIntOutTestServiceHost serviceHost = new ServiceContractAsyncIntOutTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class ServiceContractAsyncIntOutTestServiceHost : TestServiceHostBase<IServiceContractIntOutService>
    {
        protected override string Address { get { return "ServiceContractIntOut"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public ServiceContractAsyncIntOutTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class ServiceContractAsyncUniqueTypeOutTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceContractAsyncUniqueTypeOutTestServiceHost serviceHost = new ServiceContractAsyncUniqueTypeOutTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class ServiceContractAsyncUniqueTypeOutTestServiceHost : TestServiceHostBase<IServiceContractUniqueTypeOutService>
    {
        protected override string Address { get { return "ServiceContractUniqueTypeOut"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public ServiceContractAsyncUniqueTypeOutTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class ServiceContractAsyncIntRefTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceContractAsyncIntRefTestServiceHost serviceHost = new ServiceContractAsyncIntRefTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class ServiceContractAsyncIntRefTestServiceHost : TestServiceHostBase<IServiceContractIntRefService>
    {
        protected override string Address { get { return "ServiceContractIntRef"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public ServiceContractAsyncIntRefTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class ServiceContractAsyncUniqueTypeRefTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceContractAsyncUniqueTypeRefTestServiceHost serviceHost = new ServiceContractAsyncUniqueTypeRefTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class ServiceContractAsyncUniqueTypeRefTestServiceHost : TestServiceHostBase<IServiceContractUniqueTypeRefService>
    {
        protected override string Address { get { return "ServiceContractAsyncUniqueTypeRef"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public ServiceContractAsyncUniqueTypeRefTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class ServiceContractSyncUniqueTypeOutTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceContractSyncUniqueTypeOutTestServiceHost serviceHost = new ServiceContractSyncUniqueTypeOutTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class ServiceContractSyncUniqueTypeOutTestServiceHost : TestServiceHostBase<IServiceContractUniqueTypeOutSyncService>
    {
        protected override string Address { get { return "ServiceContractUniqueTypeOutSync"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }
        public ServiceContractSyncUniqueTypeOutTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class ServiceContractSyncUniqueTypeRefTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            ServiceContractSyncUniqueTypeRefTestServiceHost serviceHost = new ServiceContractSyncUniqueTypeRefTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class ServiceContractSyncUniqueTypeRefTestServiceHost : TestServiceHostBase<IServiceContractUniqueTypeRefSyncService>
    {
        protected override string Address { get { return "ServiceContractUniqueTypeRefSync"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public ServiceContractSyncUniqueTypeRefTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
