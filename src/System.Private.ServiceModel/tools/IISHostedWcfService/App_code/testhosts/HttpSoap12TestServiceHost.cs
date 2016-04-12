// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfService
{
    public class HttpSoap12TestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            HttpSoap12TestServiceHost serviceHost = new HttpSoap12TestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class HttpSoap12TestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "http-soap12"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8), new HttpTransportBindingElement());
        }

        public HttpSoap12TestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
