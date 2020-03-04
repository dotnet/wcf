// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfProjectNService
{
    public class HttpSoap12WSA2004TestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            HttpSoap12WSA2004TestServiceHost serviceHost = new HttpSoap12WSA2004TestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class HttpSoap12WSA2004TestServiceHost : TestServiceHostBase<IWcfProjectNService>
    {
        protected override string Address { get { return "http-soap12WSA2004"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressingAugust2004, Encoding.UTF8), new HttpTransportBindingElement());
        }

        public HttpSoap12WSA2004TestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
