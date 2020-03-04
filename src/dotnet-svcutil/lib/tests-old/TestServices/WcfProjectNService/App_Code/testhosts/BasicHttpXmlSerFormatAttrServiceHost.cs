// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfProjectNService
{
    public class XmlSerializerICalculatorServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new XmlSerializerICalculatorServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class XmlSerializerICalculatorServiceHost : TestServiceHostBase<ICalculator>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public XmlSerializerICalculatorServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class XmlSerializerDualContractServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new XmlSerializerDualContractServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class XmlSerializerDualContractServiceHost : TestServiceHostBase<ICalculator, IHelloWorld>
    {
        protected static Binding binding;
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return binding = binding == null ? new BasicHttpBinding() : binding;
        }

        public XmlSerializerDualContractServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
