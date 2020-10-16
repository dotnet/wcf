// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    using System.Collections.ObjectModel;
    using System.IdentityModel.Tokens;
    using System.Runtime;
    using System.Runtime.CompilerServices;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.ServiceModel.Security.Tokens;
    using Microsoft.Xml;
    using System.ComponentModel;

    public sealed class FederatedMessageSecurityOverHttp
    {
        internal const bool DefaultNegotiateServiceCredential = true;
        internal const SecurityKeyType DefaultIssuedKeyType = SecurityKeyType.SymmetricKey;
        internal const bool DefaultEstablishSecurityContext = true;

        private bool _establishSecurityContext;
        private bool _negotiateServiceCredential;
        private SecurityAlgorithmSuite _algorithmSuite;
        private EndpointAddress _issuerAddress;
        private EndpointAddress _issuerMetadataAddress;
        private Binding _issuerBinding;
        private Collection<ClaimTypeRequirement> _claimTypeRequirements;
        private string _issuedTokenType;
        private SecurityKeyType _issuedKeyType;
        private Collection<XmlElement> _tokenRequestParameters;

        public FederatedMessageSecurityOverHttp()
        {
            _negotiateServiceCredential = DefaultNegotiateServiceCredential;
            _algorithmSuite = SecurityAlgorithmSuite.Default;
            _issuedKeyType = DefaultIssuedKeyType;
            _claimTypeRequirements = new Collection<ClaimTypeRequirement>();
            _tokenRequestParameters = new Collection<XmlElement>();
            _establishSecurityContext = DefaultEstablishSecurityContext;
        }

        public bool NegotiateServiceCredential
        {
            get { return _negotiateServiceCredential; }
            set { _negotiateServiceCredential = value; }
        }

        public SecurityAlgorithmSuite AlgorithmSuite
        {
            get { return _algorithmSuite; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                _algorithmSuite = value;
            }
        }

        public bool EstablishSecurityContext
        {
            get
            {
                return _establishSecurityContext;
            }
            set
            {
                _establishSecurityContext = value;
            }
        }

        [DefaultValue(null)]
        public EndpointAddress IssuerAddress
        {
            get { return _issuerAddress; }
            set { _issuerAddress = value; }
        }

        [DefaultValue(null)]
        public EndpointAddress IssuerMetadataAddress
        {
            get { return _issuerMetadataAddress; }
            set { _issuerMetadataAddress = value; }
        }

        [DefaultValue(null)]
        public Binding IssuerBinding
        {
            get
            {
                return _issuerBinding;
            }
            set
            {
                _issuerBinding = value;
            }
        }

        [DefaultValue(null)]
        public string IssuedTokenType
        {
            get { return _issuedTokenType; }
            set { _issuedTokenType = value; }
        }

        public SecurityKeyType IssuedKeyType
        {
            get { return _issuedKeyType; }
            set
            {
                if (!SecurityKeyTypeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                _issuedKeyType = value;
            }
        }

        public Collection<ClaimTypeRequirement> ClaimTypeRequirements
        {
            get { return _claimTypeRequirements; }
        }

        public Collection<XmlElement> TokenRequestParameters
        {
            get { return _tokenRequestParameters; }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal SecurityBindingElement CreateSecurityBindingElement(bool isSecureTransportMode,
                                                                     bool isReliableSession,
                                                                     MessageSecurityVersion version)
        {
            throw new NotImplementedException();
        }

        internal static bool TryCreate(SecurityBindingElement sbe, bool isSecureTransportMode, bool isReliableSession, MessageSecurityVersion version, out FederatedMessageSecurityOverHttp messageSecurity)
        {
            throw new NotImplementedException();
        }

        internal bool InternalShouldSerialize()
        {
            return (this.ShouldSerializeAlgorithmSuite()
                || this.ShouldSerializeClaimTypeRequirements()
                || this.ShouldSerializeNegotiateServiceCredential()
                || this.ShouldSerializeEstablishSecurityContext()
                || this.ShouldSerializeIssuedKeyType()
                || this.ShouldSerializeTokenRequestParameters());
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeAlgorithmSuite()
        {
            return (this.AlgorithmSuite != SecurityAlgorithmSuite.Default);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeClaimTypeRequirements()
        {
            return (this.ClaimTypeRequirements.Count > 0);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeNegotiateServiceCredential()
        {
            return (this.NegotiateServiceCredential != DefaultNegotiateServiceCredential);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeEstablishSecurityContext()
        {
            return (this.EstablishSecurityContext != DefaultEstablishSecurityContext);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeIssuedKeyType()
        {
            return (this.IssuedKeyType != DefaultIssuedKeyType);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeTokenRequestParameters()
        {
            return (this.TokenRequestParameters.Count > 0);
        }
    }
}
