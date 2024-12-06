// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "BasicHttpSoap.svc")]
    public class BasicHttpSoapTestServiceHost : TestServiceHostBase<IWcfSoapService>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public BasicHttpSoapTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfSoapService), baseAddresses)
        {
        }
    }
}
