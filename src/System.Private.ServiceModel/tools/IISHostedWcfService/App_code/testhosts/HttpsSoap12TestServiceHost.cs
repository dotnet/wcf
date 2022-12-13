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
    [TestServiceDefinition(Schema = ServiceSchema.HTTPS, BasePath = "HttpsSoap12.svc")]
    public class HttpsSoap12TestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "https-soap12"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8), new HttpsTransportBindingElement());
        }

        public HttpsSoap12TestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
