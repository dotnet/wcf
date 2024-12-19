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
    [TestServiceDefinition(BasePath = "BasicService1.svc", Schema = ServiceSchema.HTTP)]
    public class BasicHttpSerivce1Host : TestServiceHostBase<IService1>
    {
        protected override string Address { get { return "Service1"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public BasicHttpSerivce1Host(params Uri[] baseAddresses)
            : base(typeof(Service1), baseAddresses)
        {
        }
    }
}
