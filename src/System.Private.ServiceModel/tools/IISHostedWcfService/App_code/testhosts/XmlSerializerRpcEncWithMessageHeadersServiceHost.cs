// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
#endif

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "BasicHttpRpcEncWithHeaders.svc")]
    public class XmlSerializerRpcEncWithMessageHeadersServiceHost : TestServiceHostBase<IEchoRpcEncWithHeadersService>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public XmlSerializerRpcEncWithMessageHeadersServiceHost(params Uri[] baseAddresses)
            : base(typeof(EchoRpcEncWithHeadersService), baseAddresses)
        {
        }
    }
}
