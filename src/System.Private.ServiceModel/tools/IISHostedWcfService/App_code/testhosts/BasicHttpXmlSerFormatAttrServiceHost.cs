// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class RpcEncSingleNsServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new RpcEncSingleNsServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class RpcEncSingleNsServiceHost : TestServiceHostBase<ICalculator>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public RpcEncSingleNsServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
            //multiple binding namespaces test: need to figure out if to add new servicehost for it, code would be like below.
            //Binding binding = GetBinding();
            //binding.Namespace = "http://some.Namespace.org/";
            //ServiceEndpoint endpoint2 = this.AddServiceEndpoint(typeof(ICalculator), binding, Address+"/extra");
        }
    }

    public class RpcLitSingleNsServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new RpcLitSingleNsServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class RpcLitSingleNsServiceHost : TestServiceHostBase<ICalculator>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public RpcLitSingleNsServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class DocLitSingleNsServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new DocLitSingleNsServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class DocLitSingleNsServiceHost : TestServiceHostBase<ICalculator>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public DocLitSingleNsServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class RpcEncMultiNsServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new RpcEncMultiNsServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class RpcEncMultiNsServiceHost : TestServiceHostBase<ICalculator, IHelloWorld>
    {
        protected static Binding binding;
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return binding = binding == null ? new BasicHttpBinding() : binding;
        }

        public RpcEncMultiNsServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class RpcLitMultiNsServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new RpcLitMultiNsServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class RpcLitMultiNsServiceHost : TestServiceHostBase<ICalculator, IHelloWorld>
    {
        protected static Binding binding;
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return binding = binding == null ? new BasicHttpBinding() : binding;
        }

        public RpcLitMultiNsServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class DocLitMultiNsServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new DocLitMultiNsServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class DocLitMultiNsServiceHost : TestServiceHostBase<ICalculator, IHelloWorld>
    {
        protected static Binding binding;
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return binding = binding == null ? new BasicHttpBinding() : binding;
        }

        public DocLitMultiNsServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
