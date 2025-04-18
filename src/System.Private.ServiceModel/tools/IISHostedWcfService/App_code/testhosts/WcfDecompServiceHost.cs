// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
#endif

namespace WcfService
{
    [TestServiceDefinition(BasePath = "BasicHttpWcfDecomp.svc", Schema = ServiceSchema.HTTP)]
    public class WcfDecompServiceHost : TestServiceHostBase<IWcfDecompService>
    {
        public WcfDecompServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfDecompService), baseAddresses)
        {
        }

        protected override string Address { get { return "TestDecompressionEnabled"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }
    }
}
