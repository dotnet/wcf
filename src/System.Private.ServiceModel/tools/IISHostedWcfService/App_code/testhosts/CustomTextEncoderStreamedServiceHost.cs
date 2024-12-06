// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif
using System.Text;

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "CustomTextEncoderStreamed.svc")]
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

        public CustomTextEncoderStreamedTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
