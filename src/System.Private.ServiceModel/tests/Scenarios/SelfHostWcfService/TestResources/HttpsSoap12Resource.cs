// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel.Channels;
using System.Text;

namespace WcfService.TestResources
{
    internal class HttpsSoap12Resource : HttpsResource
    {
        protected override string Address { get { return "https-soap12"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8), new HttpsTransportBindingElement());
        }
    }
}
