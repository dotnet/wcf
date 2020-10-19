﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using System.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Diagnostics;

namespace WcfService
{
    [TestServiceDefinition(Schema = ServiceSchema.HTTPS, BasePath = Saml2IssuedTokenTestServiceHost.BasePath)]
    public class Saml2IssuedTokenTestServiceHost : TestServiceHostBase<IWcfService>
    {
        private const string BasePath = "Saml2IssuedToken.svc";
        private const string Saml20TokenType = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV2.0";

        protected override string Address { get { return "issued-token-using-tls"; } }

        protected override IList<Binding> GetBindings()
        {
            var bindings = new List<Binding>();
            // Add binding to receive requests using tokens issued from each of the STS endpoints
            foreach (var tuple in FederationSTSServiceHost.EndpointList)
            {
                // Accept clients not using SecureConversation
                var binding = GetBinding(tuple.Item1, tuple.Item2, tuple.Item3, useSC: false);
                binding.Name = Address + "/" + tuple.Item3;
                bindings.Add(binding);
                // Accept clients using SecureConversation
                binding = GetBinding(tuple.Item1, tuple.Item2, tuple.Item3 + "/sc", useSC: true);
                binding.Name = Address + "/" + tuple.Item3 + "/sc";
                bindings.Add(binding);
            }

            return bindings;
        }

        private Binding GetBinding(Type contractType, Binding issuerBinding, string issuerRelativePath, bool useSC)
        {
            var serviceBinding = GetFederatedBindingFromIssuerContract(contractType);
            serviceBinding.Security.Message.EstablishSecurityContext = useSC;
            serviceBinding.Security.Message.IssuedTokenType = Saml20TokenType;
            var serverBasePathUri = BaseAddresses[0];
            var issuerUri = new Uri(serverBasePathUri, FederationSTSServiceHost.BasePath + "/" + issuerRelativePath);
            serviceBinding.Security.Message.IssuerAddress = new EndpointAddress(issuerUri);
            serviceBinding.Security.Message.IssuerBinding = issuerBinding;
            return serviceBinding;
        }

        private WSFederationHttpBinding GetFederatedBindingFromIssuerContract(Type contractType)
        {
            WSFederationHttpBinding binding;
            // If STS endpoint is using WSTrustFeb2005, then need to use WSFederationHttpBinding and symmetric issued tokens
            if (contractType == typeof(IWSTrustFeb2005SyncContract))
            {
                binding = new WSFederationHttpBinding(WSFederationHttpSecurityMode.TransportWithMessageCredential);
                binding.Security.Message.IssuedKeyType = SecurityKeyType.SymmetricKey;
            }
            // If STS endpoint is using WSTrust1.3, then need to use WS2007FederationHttpBinding and bearer issued tokens
            else if (contractType == typeof(IWSTrust13SyncContract))
            {
                binding = new WS2007FederationHttpBinding(WSFederationHttpSecurityMode.TransportWithMessageCredential);
                binding.Security.Message.IssuedKeyType = SecurityKeyType.BearerKey;
            }
            else
            {
                throw new ArgumentException("Unknown contract type", "contractType");
            }

            return binding;
        }

        protected override void ApplyConfiguration()
        {
            base.ApplyConfiguration();
            Credentials.ServiceCertificate.Certificate = TestHost.CertificateFromFriendlyName(StoreName.My, StoreLocation.LocalMachine, "WCF Bridge - Machine certificate generated by the CertificateManager");
            Credentials.UseIdentityConfiguration = true;
            Uri serverBasePathUri = BaseAddresses[0]; // Just in case an address isn't found for https, prevents a NRE
            foreach(var baseAddress in BaseAddresses)
            {
                if(baseAddress.Scheme == "https")
                {
                    serverBasePathUri = baseAddress;
                    break;
                }
            }

            foreach (var tuple in FederationSTSServiceHost.EndpointList)
            {
                AddAllowedAudienceUri(serverBasePathUri, tuple.Item3);
                AddAllowedAudienceUri(serverBasePathUri, tuple.Item3 + "/sc");
            }
            Credentials.IdentityConfiguration.CertificateValidationMode = X509CertificateValidationMode.None;
            Credentials.IdentityConfiguration.IssuerNameRegistry = new CustomIssuerNameRegistry();
        }

        private void AddAllowedAudienceUri(Uri basepath, string relativePath)
        {
            var audienceUriBuilder = new UriBuilder(basepath);
            audienceUriBuilder.Path = audienceUriBuilder.Path + "/" + Address + "/" + relativePath; // localhost
            audienceUriBuilder.Host = "localhost"; // When IIS hosted Host was the fqdn, self hosted it's localhost
            Credentials.IdentityConfiguration.AudienceRestriction.AllowedAudienceUris.Add(audienceUriBuilder.Uri);

            audienceUriBuilder.Host = System.Net.Dns.GetHostEntry("127.0.0.1").HostName; // fqdn
            Credentials.IdentityConfiguration.AudienceRestriction.AllowedAudienceUris.Add(audienceUriBuilder.Uri);

            audienceUriBuilder.Host = Environment.MachineName; // netbios name
            Credentials.IdentityConfiguration.AudienceRestriction.AllowedAudienceUris.Add(audienceUriBuilder.Uri);
        }

        public Saml2IssuedTokenTestServiceHost(params Uri[] baseAddresses)
            : base(typeof(WcfService), baseAddresses)
        {
        }

        class CustomIssuerNameRegistry : IssuerNameRegistry
        {
            string _issuer;
            public CustomIssuerNameRegistry()
            {
                var issuerCert = TestHost.CertificateFromFriendlyName(StoreName.My, StoreLocation.LocalMachine, "WCF Bridge - STSMetaData");
                _issuer = issuerCert.SubjectName.Name;
            }

            public override string GetIssuerName(SecurityToken securityToken)
            {
                return _issuer;
            }
        }
    }
}
