// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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