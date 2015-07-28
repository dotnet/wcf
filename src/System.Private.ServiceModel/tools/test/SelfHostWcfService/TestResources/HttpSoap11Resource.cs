// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel.Channels;
using System.Text;

namespace WcfService.TestResources
{
    internal class HttpSoap11Resource : HttpResource
    {
        protected override string Address { get { return "http-soap11"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap11, Encoding.UTF8), new HttpTransportBindingElement());
        }
    }
}
