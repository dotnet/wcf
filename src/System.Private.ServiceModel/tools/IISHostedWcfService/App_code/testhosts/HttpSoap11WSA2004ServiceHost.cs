// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF.Channels;
#else
using System;
using System.ServiceModel.Channels;
#endif
using System.Text;

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "HttpSoap11WSA2004.svc")]
    public class HttpSoap11WSA2004TestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "http-soap11WSA2004"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap11WSAddressingAugust2004, Encoding.UTF8), new HttpTransportBindingElement());
        }

        public HttpSoap11WSA2004TestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
