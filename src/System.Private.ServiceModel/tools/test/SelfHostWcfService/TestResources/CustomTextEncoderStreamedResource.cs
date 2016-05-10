// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel;
using System.ServiceModel.Channels;
using WcfService.TestResources.CustomBindings;

namespace WcfService.TestResources
{
    internal class CustomTextEncoderStreamedResource : HttpResource
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
    }
}