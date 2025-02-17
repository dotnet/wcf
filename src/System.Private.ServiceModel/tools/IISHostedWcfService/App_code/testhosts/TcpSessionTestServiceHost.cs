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

    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "SessionTestsDefaultService.svc")]
    public class TcpSessionTestServiceHost : TestServiceHostBase<ISessionTestsDefaultService>
    {
        protected override string Address { get { return "tcp-sessions"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None);
        }

        public TcpSessionTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(SessionTestsDefaultService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "SessionTestsShortTimeoutService.svc")]
    public class TcpSessionShortTimeoutTestServiceHost : TestServiceHostBase<ISessionTestsShortTimeoutService>
    {
        protected override string Address { get { return ""; } }
        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { ReceiveTimeout = TimeSpan.FromSeconds(5) };
        }

        public TcpSessionShortTimeoutTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(SessionTestsShortTimeoutService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "SessionTestsDuplexService.svc")]
    public class TcpSessionDuplexTestServiceHost : TestServiceHostBase<ISessionTestsDuplexService>
    {
        protected override string Address { get { return ""; } }
        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None);
        }

        public TcpSessionDuplexTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(SessionTestsDuplexService), baseAddresses)
        {
        }
    }
}
