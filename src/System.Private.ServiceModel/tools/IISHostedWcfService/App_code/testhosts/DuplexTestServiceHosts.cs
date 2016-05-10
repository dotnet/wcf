// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;

namespace WcfService
{
    public class DuplexTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            DuplexTestServiceHost serviceHost = new DuplexTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class DuplexTestServiceHost : TestServiceHostBase<IWcfDuplexService>
    {
        protected override string Address { get { return "tcp-nosecurity-callback"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }

        public DuplexTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class DuplexCallbackTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            DuplexCallbackTestServiceHost serviceHost = new DuplexCallbackTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class DuplexCallbackTestServiceHost : TestServiceHostBase<IDuplexChannelService>
    {
        protected override string Address { get { return "tcp-nosecurity-typedproxy-duplexcallback"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }

        public DuplexCallbackTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class DuplexChannelCallbackReturnTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            DuplexChannelCallbackReturnTestServiceHost serviceHost = new DuplexChannelCallbackReturnTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class DuplexChannelCallbackReturnTestServiceHost : TestServiceHostBase<IWcfDuplexTaskReturnService>
    {
        protected override string Address { get { return "tcp-nosecurity-taskreturn"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }

        public DuplexChannelCallbackReturnTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class DuplexCallbackDataContractComplexTypeTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            DuplexCallbackDataContractComplexTypeTestServiceHost serviceHost = new DuplexCallbackDataContractComplexTypeTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class DuplexCallbackDataContractComplexTypeTestServiceHost : TestServiceHostBase<IWcfDuplexService_DataContract>
    {
        protected override string Address { get { return "tcp-nosecurity-callback"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }

        public DuplexCallbackDataContractComplexTypeTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }

    public class DuplexCallbackXmlComplexTypeTestServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            DuplexCallbackXmlComplexTypeTestServiceHost serviceHost = new DuplexCallbackXmlComplexTypeTestServiceHost(serviceType, baseAddresses);
            return serviceHost;
        }
    }

    public class DuplexCallbackXmlComplexTypeTestServiceHost : TestServiceHostBase<IWcfDuplexService_Xml>
    {
        protected override string Address { get { return "tcp-nosecurity-callback"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }

        public DuplexCallbackXmlComplexTypeTestServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        {
        }
    }
}
