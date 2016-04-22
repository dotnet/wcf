// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

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
            return new CustomBinding(new CustomTextMessageBindingElement("ISO-8859-1"), new HttpTransportBindingElement
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