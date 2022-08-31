// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace WcfService
{
    [TestServiceDefinition(BasePath = "BasicCheckCallbackDbgBhvService.svc", Schema = ServiceSchema.HTTP)]
    public class BasicCheckCallbackDbgBhvServiceHost : TestServiceHostBase<ICheckCallbackDbgBhvService>
    {
        protected override string Address { get { return "CheckCallbackDbgBhvService"; } }

        protected override Binding GetBinding()
        {
            return new BasicHttpBinding();
        }

        public BasicCheckCallbackDbgBhvServiceHost(params Uri[] baseAddresses)
            : base(typeof(CheckCallbackDbgBhvService), baseAddresses)
        {
        }
    }
}
