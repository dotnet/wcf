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
    public class CustomTextEncoderBufferedTestServiceFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            CustomTextEncoderBufferedTestServiceHost serviceHost = new CustomTextEncoderBufferedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class CustomTextEncoderBufferedTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "http-customtextencoder"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new CustomTextMessageBindingElement(Encoding.UTF8.WebName), new HttpTransportBindingElement
            {
                MaxReceivedMessageSize = SixtyFourMB,
                MaxBufferSize = SixtyFourMB
            });
        }

        public CustomTextEncoderBufferedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}