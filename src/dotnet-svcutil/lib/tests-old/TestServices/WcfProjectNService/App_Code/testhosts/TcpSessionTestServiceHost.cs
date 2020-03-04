// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfProjectNService
{
    public class TcpSessionTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            TcpSessionTestServiceHost serviceHost = new TcpSessionTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class TcpSessionTestServiceHost : TestServiceHostBase<ISessionTestsDefaultService>
    {
        protected override string Address { get { return "tcp-sessions"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }

        public TcpSessionTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class TcpSessionShortTimeoutTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            TcpSessionShortTimeoutTestServiceHost serviceHost = new TcpSessionShortTimeoutTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class TcpSessionShortTimeoutTestServiceHost : TestServiceHostBase<ISessionTestsShortTimeoutService>
    {
        protected override string Address { get { return ""; } }
        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false, ReceiveTimeout = TimeSpan.FromSeconds(5)};
        }

        public TcpSessionShortTimeoutTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class TcpSessionDuplexTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            TcpSessionDuplexTestServiceHost serviceHost = new TcpSessionDuplexTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }
    public class TcpSessionDuplexTestServiceHost : TestServiceHostBase<ISessionTestsDuplexService>
    {
        protected override string Address { get { return ""; } }
        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false};
        }

        public TcpSessionDuplexTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}