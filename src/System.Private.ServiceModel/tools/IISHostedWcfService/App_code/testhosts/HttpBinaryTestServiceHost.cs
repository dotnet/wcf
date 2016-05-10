// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class HttpBinaryTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            HttpBinaryTestServiceHost serviceHost = new HttpBinaryTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class HttpBinaryTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "http-binary"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new BinaryMessageEncodingBindingElement(), new HttpTransportBindingElement());
        }

        public HttpBinaryTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
