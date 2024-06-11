// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;

namespace WcfService
{
    //Start the crlUrl service as the client use it to ensure all services have been started
    [TestServiceDefinition(BasePath = "", Schema = ServiceSchema.HTTP)]
    public class TestHostServiceHost : TestServiceHostBase<ITestHost>
    {
        protected override string Address { get { return "TestHost.svc"; } }

        protected override Binding GetBinding()
        {
            return GetWebHttpBinding();
        }

        private Binding GetWebHttpBinding()
        {
            var binding = new WebHttpBinding();
            return binding;
        }

        public TestHostServiceHost(params Uri[] baseAddresses)
            : base(typeof(TestHost), baseAddresses)
        {
        }
    }
}
#endif
