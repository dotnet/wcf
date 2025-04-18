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
    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "BasicHttp_4_4_0.svc")]
    public class BasicHttpTestServiceHost_4_4_0 : TestServiceHostBase<IWcfService_4_4_0>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public BasicHttpTestServiceHost_4_4_0(params Uri[] baseAddresses)
            : base(typeof(WcfService_4_4_0), baseAddresses)
        {
        }
    }
}
