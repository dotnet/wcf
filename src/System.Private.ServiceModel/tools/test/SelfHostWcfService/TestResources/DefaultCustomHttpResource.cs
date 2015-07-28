// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel.Channels;

namespace WcfService.TestResources
{
    internal class DefaultCustomHttpResource : HttpResource
    {
        protected override string Address { get { return "default-custom-http"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(), new HttpTransportBindingElement());
        }
    }
}
