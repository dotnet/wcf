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
            return new CustomBinding(new CustomTextMessageBindingElement(Encoding.UTF8.WebName),
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