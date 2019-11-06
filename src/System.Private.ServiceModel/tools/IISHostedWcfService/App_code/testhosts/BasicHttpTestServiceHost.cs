// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService
{
    [TestServiceDefinition(BasePath = "BasicHttp.svc", Schema = ServiceSchema.HTTP)]
    public class BasicHttpTestServiceHost : TestServiceHostBase<IWcfService>
    {
        protected override string Address { get { return "Basic"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public BasicHttpTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }
    }
}
