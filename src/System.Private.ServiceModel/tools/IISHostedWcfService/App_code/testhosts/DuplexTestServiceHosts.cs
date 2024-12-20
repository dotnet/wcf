﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Security;
#else
using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
#endif
using System.Security.Cryptography.X509Certificates;

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "Duplex.svc")]
    public class DuplexTestServiceHost : TestServiceHostBase<IWcfDuplexService>
    {
        protected override string Address { get { return "tcp-nosecurity-callback"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None);
        }

        public DuplexTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfDuplexService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "DuplexCallback.svc")]
    public class DuplexCallbackTestServiceHost : TestServiceHostBase<IDuplexChannelService>
    {
        protected override string Address { get { return "tcp-nosecurity-typedproxy-duplexcallback"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None);
        }

        public DuplexCallbackTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(DuplexCallbackService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "DuplexChannelCallbackReturn.svc")]
    public class DuplexChannelCallbackReturnTestServiceHost : TestServiceHostBase<IWcfDuplexTaskReturnService>
    {
        protected override string Address { get { return "tcp-nosecurity-taskreturn"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None);
        }

        public DuplexChannelCallbackReturnTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(DuplexChannelCallbackReturnService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "DuplexCallbackDataContractComplexType.svc")]
    public class DuplexCallbackDataContractComplexTypeTestServiceHost : TestServiceHostBase<IWcfDuplexService_DataContract>
    {
        protected override string Address { get { return "tcp-nosecurity-callback"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None);
        }

        public DuplexCallbackDataContractComplexTypeTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfDuplexService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "DuplexCallbackXmlComplexType.svc")]
    public class DuplexCallbackXmlComplexTypeTestServiceHost : TestServiceHostBase<IWcfDuplexService_Xml>
    {
        protected override string Address { get { return "tcp-nosecurity-callback"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None);
        }

        public DuplexCallbackXmlComplexTypeTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfDuplexService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "DuplexCallbackTcpCertificateCredential.svc")]
    public class DuplexCallbackTcpCertificateCredentialTestServiceHost : TestServiceHostBase<IWcfDuplexService>
    {
        protected override string Address { get { return "tcp-certificate-callback"; } }

        protected override Binding GetBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;
            return binding;
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
            // Ensure the https certificate is installed before this endpoint resource is used
            string thumbprint = TestHost.CertificateFromFriendlyName(StoreName.My, StoreLocation.LocalMachine, "WCF Bridge - Machine certificate generated by the CertificateManager").Thumbprint;

            this.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.None;
            this.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine,
                                                      StoreName.My,
                                                      X509FindType.FindByThumbprint,
                                                      thumbprint);
        }

        public DuplexCallbackTcpCertificateCredentialTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfDuplexService), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "DuplexCallbackConcurrencyMode.svc")]
    public class DuplexCallbackConcurrencyModeServiceHost : TestServiceHostBase<IWcfDuplexService_CallbackConcurrencyMode>
    {
        protected override string Address { get { return "tcp"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None);
        }

        public DuplexCallbackConcurrencyModeServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfDuplexService_CallbackConcurrenyMode), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "DuplexCallbackDebugBehavior.svc")]
    public class DuplexCallbackDebugBahaviorServiceHost : TestServiceHostBase<IWcfDuplexService_CallbackDebugBehavior>
    {
        protected override string Address { get { return "tcp"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None);
        }

        public DuplexCallbackDebugBahaviorServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfDuplexService_CallbackDebugBehavior), baseAddresses)
        {
        }
    }

    [TestServiceDefinition(Schema = ServiceSchema.NETTCP, BasePath = "DuplexCallbackErrorHandler.svc")]
    public class DuplexCallbackErrorHandlerServiceHost : TestServiceHostBase<IWcfDuplexService_CallbackErrorHandler>
    {
        protected override string Address { get { return "tcp"; } }

        protected override Binding GetBinding()
        {
            return new NetTcpBinding(SecurityMode.None) { PortSharingEnabled = false };
        }

        public DuplexCallbackErrorHandlerServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfDuplexService_CallbackErrorHandler), baseAddresses)
        {
        }
    }
}
