// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


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
