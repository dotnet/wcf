// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class CustomTextEncoderStreamedTestServiceFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            CustomTextEncoderStreamedTestServiceHost serviceHost = new CustomTextEncoderStreamedTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class CustomTextEncoderStreamedTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "http-customtextencoder-streamed"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new CustomTextMessageBindingElement("ISO-8859-1"),
                new HttpTransportBindingElement
                {
                    MaxReceivedMessageSize = SixtyFourMB,
                    MaxBufferSize = SixtyFourMB,
                    TransferMode = TransferMode.Streamed
                });
        }

        public CustomTextEncoderStreamedTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}