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
    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "TcpSoap11WSA10.svc")]
    public class TcpSoap11WSA10TestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "tcp-Soap11WSA10"; } }

        protected override Binding GetBinding()
        {
            return new CustomBinding(new TextMessageEncodingBindingElement(MessageVersion.Soap11WSAddressing10, Encoding.UTF8), new TcpTransportBindingElement());
        }

        public TcpSoap11WSA10TestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
