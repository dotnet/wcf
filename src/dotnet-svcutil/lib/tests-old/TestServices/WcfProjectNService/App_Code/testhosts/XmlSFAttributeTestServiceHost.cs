// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfProjectNService
{
    public class XmlSFAttributeTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            XmlSFAttributeTestServiceHost serviceHost = new XmlSFAttributeTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class XmlSFAttributeTestServiceHost : TestServiceHostBase<IXmlSFAttribute>
    {
        protected override string Address { get { return "XmlSFAttribute"; } }

        protected override Binding GetBinding()
        {
            var binding = new BasicHttpBinding();
            return binding;
        }

        public XmlSFAttributeTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
