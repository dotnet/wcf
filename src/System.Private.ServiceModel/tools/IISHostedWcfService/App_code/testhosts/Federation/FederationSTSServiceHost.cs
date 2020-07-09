﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IdentityModel.Configuration;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.Web.Hosting;

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTPS, BasePath = FederationSTSServiceHost.BasePath)]
    public class FederationSTSServiceHost : WSTrustServiceHost
    {
        internal const string BasePath = "LocalSTS.svc";
        internal const string RelativePath = "transport";

        public FederationSTSServiceHost(params Uri[] baseAddresses)
            : base(new SecurityTokenServiceConfiguration(), baseAddresses)
        {
            ConfigureService();
            AddServiceEndpoint(typeof(IWSTrust13SyncContract), GetBinding(), RelativePath);
        }

        private Binding GetBinding()
        {
            var binding = new WS2007HttpBinding(SecurityMode.Transport);
            if (HostingEnvironment.IsHosted)
            {
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.None;
                var customBinding = new CustomBinding(binding);
                customBinding.Elements.Insert(2, new BasicAuthenticationBindingElement());
                return customBinding;
            }
            else
            {
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                return binding;
            }
        }

        protected void ConfigureService()
        {
            base.ApplyConfiguration();
            var config = ServiceContract.SecurityTokenServiceConfiguration;
            config.SigningCredentials = new X509SigningCredentials(TestHost.CertificateFromFriendlyName(StoreName.My, StoreLocation.LocalMachine, "WCF Bridge - STSMetaData"));
            config.ServiceCertificate = TestHost.CertificateFromFriendlyName(StoreName.My, StoreLocation.LocalMachine, "WCF Bridge - Machine certificate generated by the CertificateManager");
            config.SecurityTokenService = typeof(SelfHostSecurityTokenService);
            config.TokenIssuerName = "SelfHostSTS";
            config.SecurityTokenHandlerCollectionManager[SecurityTokenHandlerCollectionManager.Usage.ActAs] = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
            config.SecurityTokenHandlerCollectionManager[SecurityTokenHandlerCollectionManager.Usage.OnBehalfOf] = SecurityTokenHandlerCollection.CreateDefaultSecurityTokenHandlerCollection();
            config.CertificateValidationMode = X509CertificateValidationMode.ChainTrust;
            config.IssuerNameRegistry = new ReturnX509SubjectNameOrRSAIssuerNameRegistry();
            config.SecurityTokenHandlers.AddOrReplace(new AcceptAnyUsernameSecurityTokenHandler());

            Credentials.ServiceCertificate.Certificate = config.ServiceCertificate;
            //Credentials.UseIdentityConfiguration = true;
            Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
            Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new AcceptAnyUsernamePasswordValidator();
        }
    }
}
