// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfService
{
    public class HttpsSoap12TestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            HttpsSoap12TestServiceHost serviceHost = new HttpsSoap12TestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class HttpsSoap12TestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "https-soap12"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8), new HttpsTransportBindingElement());
        }

        public HttpsSoap12TestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
