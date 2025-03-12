// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "ServiceContractAsyncIntOut.svc")]
    public class ServiceContractAsyncIntOutTestServiceHost : TestServiceHostBase<IServiceContractIntOutService>
    {
        protected override string Address { get { return "ServiceContractIntOut"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public ServiceContractAsyncIntOutTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(ServiceContractIntOutService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "ServiceContractAsyncUniqueTypeOut.svc")]
    public class ServiceContractAsyncUniqueTypeOutTestServiceHost : TestServiceHostBase<IServiceContractUniqueTypeOutService>
    {
        protected override string Address { get { return "ServiceContractUniqueTypeOut"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public ServiceContractAsyncUniqueTypeOutTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(ServiceContractUniqueTypeOutService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "ServiceContractAsyncIntRef.svc")]
    public class ServiceContractAsyncIntRefTestServiceHost : TestServiceHostBase<IServiceContractIntRefService>
    {
        protected override string Address { get { return "ServiceContractIntRef"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public ServiceContractAsyncIntRefTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(ServiceContractIntRefService), baseAddresses)
        {
        }
    }


    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "ServiceContractAsyncUniqueTypeRef.svc")]
    public class ServiceContractAsyncUniqueTypeRefTestServiceHost : TestServiceHostBase<IServiceContractUniqueTypeRefService>
    {
        protected override string Address { get { return "ServiceContractAsyncUniqueTypeRef"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public ServiceContractAsyncUniqueTypeRefTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(ServiceContractUniqueTypeRefService), baseAddresses)
        {
        }
    }


    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "ServiceContractSyncUniqueTypeOut.svc")]
    public class ServiceContractSyncUniqueTypeOutTestServiceHost : TestServiceHostBase<IServiceContractUniqueTypeOutSyncService>
    {
        protected override string Address { get { return "ServiceContractUniqueTypeOutSync"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }
        public ServiceContractSyncUniqueTypeOutTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(ServiceContractUniqueTypeOutSyncService), baseAddresses)
        {
        }
    }


    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "ServiceContractSyncUniqueTypeRef.svc")]
    public class ServiceContractSyncUniqueTypeRefTestServiceHost : TestServiceHostBase<IServiceContractUniqueTypeRefSyncService>
    {
        protected override string Address { get { return "ServiceContractUniqueTypeRefSync"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public ServiceContractSyncUniqueTypeRefTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(ServiceContractUniqueTypeRefSyncService), baseAddresses)
        {
        }
    }
}
