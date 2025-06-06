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

    [TestServiceDefinition(Schema = ServiceSchema.HTTP, BasePath = "UnderstoodHeaders.svc")]
    public class UnderstoodHeadersServiceHost : TestServiceHostBase<IUnderstoodHeaders>
    {
        protected override string Address { get { return "UnderstoodHeaders"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public UnderstoodHeadersServiceHost(params Uri[] baseAddresses)
            : base(typeof(UnderstoodHeaders), baseAddresses)
        {
        }
    }
}
