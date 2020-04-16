// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Net.Security;
using System.Security.Authentication;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using Microsoft.Xml;

namespace System.ServiceModel.Channels
{
    public class SslStreamSecurityBindingElement : StreamUpgradeBindingElement, ITransportTokenAssertionProvider
    {
        private IdentityVerifier _identityVerifier;
        private bool _requireClientCertificate;
        private SslProtocols _sslProtocols;

        public SslStreamSecurityBindingElement()
        {
            _requireClientCertificate = TransportDefaults.RequireClientCertificate;
            _sslProtocols = TransportDefaults.SslProtocols;
        }

        protected SslStreamSecurityBindingElement(SslStreamSecurityBindingElement elementToBeCloned)
            : base(elementToBeCloned)
        {
            _identityVerifier = elementToBeCloned._identityVerifier;
            _requireClientCertificate = elementToBeCloned._requireClientCertificate;
            _sslProtocols = elementToBeCloned._sslProtocols;
        }

        public IdentityVerifier IdentityVerifier
        {
            get
            {
                if (_identityVerifier == null)
                {
                    _identityVerifier = IdentityVerifier.CreateDefault();
                }

                return _identityVerifier;
            }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }

                _identityVerifier = value;
            }
        }

        [DefaultValue(TransportDefaults.RequireClientCertificate)]
        public bool RequireClientCertificate
        {
            get
            {
                return _requireClientCertificate;
            }
            set
            {
                _requireClientCertificate = value;
            }
        }

        [DefaultValue(TransportDefaults.SslProtocols)]
        public SslProtocols SslProtocols
        {
            get
            {
                return _sslProtocols;
            }
            set
            {
                SslProtocolsHelper.Validate(value);
                _sslProtocols = value;
            }
        }


        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            context.BindingParameters.Add(this);
            return context.BuildInnerChannelFactory<TChannel>();
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }

            context.BindingParameters.Add(this);
            return context.CanBuildInnerChannelFactory<TChannel>();
        }

        public override BindingElement Clone()
        {
            return new SslStreamSecurityBindingElement(this);
        }

        public override T GetProperty<T>(BindingContext context)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            }
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T)(object)new SecurityCapabilities(this.RequireClientCertificate, true, this.RequireClientCertificate,
                    ProtectionLevel.EncryptAndSign, ProtectionLevel.EncryptAndSign);
            }
            else if (typeof(T) == typeof(IdentityVerifier))
            {
                return (T)(object)this.IdentityVerifier;
            }

            else
            {
                return context.GetInnerProperty<T>();
            }
        }

        public override StreamUpgradeProvider BuildClientStreamUpgradeProvider(BindingContext context)
        {
            return SslStreamSecurityUpgradeProvider.CreateClientProvider(this, context);
        }

        internal static void ImportPolicy(MetadataImporter importer, PolicyConversionContext policyContext)
        {
            XmlElement assertion = PolicyConversionContext.FindAssertion(policyContext.GetBindingAssertions(),
                TransportPolicyConstants.SslTransportSecurityName, TransportPolicyConstants.DotNetFramingNamespace, true);

            if (assertion != null)
            {
                SslStreamSecurityBindingElement sslBindingElement = new SslStreamSecurityBindingElement();

                XmlReader reader = new XmlNodeReader(assertion);
                reader.ReadStartElement();
                sslBindingElement.RequireClientCertificate = reader.IsStartElement(
                    TransportPolicyConstants.RequireClientCertificateName,
                    TransportPolicyConstants.DotNetFramingNamespace);
                if (sslBindingElement.RequireClientCertificate)
                {
                    reader.ReadElementString();
                }

                policyContext.BindingElements.Add(sslBindingElement);
            }
        }

        #region ITransportTokenAssertionProvider Members

        public XmlElement GetTransportTokenAssertion()
        {
            XmlDocument document = new XmlDocument();
            XmlElement assertion =
                document.CreateElement(TransportPolicyConstants.DotNetFramingPrefix,
                TransportPolicyConstants.SslTransportSecurityName,
                TransportPolicyConstants.DotNetFramingNamespace);
            if (_requireClientCertificate)
            {
                assertion.AppendChild(document.CreateElement(TransportPolicyConstants.DotNetFramingPrefix,
                    TransportPolicyConstants.RequireClientCertificateName,
                    TransportPolicyConstants.DotNetFramingNamespace));
            }
            return assertion;
        }

        #endregion

        internal override bool IsMatch(BindingElement b)
        {
            if (b == null)
            {
                return false;
            }
            SslStreamSecurityBindingElement ssl = b as SslStreamSecurityBindingElement;
            if (ssl == null)
            {
                return false;
            }

            return _requireClientCertificate == ssl._requireClientCertificate && _sslProtocols == ssl._sslProtocols;
        }
    }
}
