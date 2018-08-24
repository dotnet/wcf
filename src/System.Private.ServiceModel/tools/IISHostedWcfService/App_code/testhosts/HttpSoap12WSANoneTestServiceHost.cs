// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Text;

namespace WcfService
{
    public class HttpSoap12WSANoneTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            HttpSoap12WSANoneTestServiceHost serviceHost = new HttpSoap12WSANoneTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class HttpSoap12WSANoneTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "http-Soap12WSANone"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12, Encoding.UTF8), new HttpTransportBindingElement());
        }

        public HttpSoap12WSANoneTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
