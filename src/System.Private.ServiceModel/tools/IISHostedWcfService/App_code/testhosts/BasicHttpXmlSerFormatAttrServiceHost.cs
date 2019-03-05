// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class XmlSerializerICalculatorRpcEncServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new XmlSerializerICalculatorRpcEncServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class XmlSerializerICalculatorRpcEncServiceHost : TestServiceHostBase<ICalculatorRpcEnc>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public XmlSerializerICalculatorRpcEncServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class XmlSerializerICalculatorRpcLitServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new XmlSerializerICalculatorRpcLitServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class XmlSerializerICalculatorRpcLitServiceHost : TestServiceHostBase<ICalculatorRpcLit>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public XmlSerializerICalculatorRpcLitServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class XmlSerializerICalculatorDocLitServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new XmlSerializerICalculatorDocLitServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class XmlSerializerICalculatorDocLitServiceHost : TestServiceHostBase<ICalculatorDocLit>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public XmlSerializerICalculatorDocLitServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class XmlSerializerDualContractRpcEncServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new XmlSerializerDualContractRpcEncServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class XmlSerializerDualContractRpcEncServiceHost : TestServiceHostBase<ICalculatorRpcEnc, IHelloWorldRpcEnc>
    {
        protected static Binding s_binding;
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return s_binding = s_binding == null ? new BasicHttpBinding() : s_binding;
        }

        public XmlSerializerDualContractRpcEncServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class XmlSerializerDualContractRpcLitServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new XmlSerializerDualContractRpcLitServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class XmlSerializerDualContractRpcLitServiceHost : TestServiceHostBase<ICalculatorRpcLit, IHelloWorldRpcLit>
    {
        protected static Binding s_binding;
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return s_binding = s_binding == null ? new BasicHttpBinding() : s_binding;
        }

        public XmlSerializerDualContractRpcLitServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class XmlSerializerDualContractDocLitServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new XmlSerializerDualContractDocLitServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class XmlSerializerDualContractDocLitServiceHost : TestServiceHostBase<ICalculatorDocLit, IHelloWorldDocLit>
    {
        protected static Binding s_binding;
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return s_binding = s_binding == null ? new BasicHttpBinding() : s_binding;
        }

        public XmlSerializerDualContractDocLitServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
