// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
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

        bool establishSecurityContext;
        bool negotiateServiceCredential;
        SecurityAlgorithmSuite algorithmSuite;
        EndpointAddress issuerAddress;
        EndpointAddress issuerMetadataAddress;
        Binding issuerBinding;
        Collection<ClaimTypeRequirement> claimTypeRequirements;
        string issuedTokenType;
        SecurityKeyType issuedKeyType;
        Collection<XmlElement> tokenRequestParameters;

        public FederatedMessageSecurityOverHttp()
        {
            negotiateServiceCredential = DefaultNegotiateServiceCredential;
            algorithmSuite = SecurityAlgorithmSuite.Default;
            issuedKeyType = DefaultIssuedKeyType;
            claimTypeRequirements = new Collection<ClaimTypeRequirement>();
            tokenRequestParameters = new Collection<XmlElement>();
            establishSecurityContext = DefaultEstablishSecurityContext;
        }

        public bool NegotiateServiceCredential
        {
            get { return this.negotiateServiceCredential; }
            set { this.negotiateServiceCredential = value; }
        }

        public SecurityAlgorithmSuite AlgorithmSuite
        {
            get { return this.algorithmSuite; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                this.algorithmSuite = value;
            }
        }

        public bool EstablishSecurityContext
        {
            get
            {
                return this.establishSecurityContext;
            }
            set
            {
                this.establishSecurityContext = value;
            }
        }

        [DefaultValue(null)]
        public EndpointAddress IssuerAddress
        {
            get { return this.issuerAddress; }
            set { this.issuerAddress = value; }
        }

        [DefaultValue(null)]
        public EndpointAddress IssuerMetadataAddress
        {
            get { return this.issuerMetadataAddress; }
            set { this.issuerMetadataAddress = value; }
        }

        [DefaultValue(null)]
        public Binding IssuerBinding
        {
            get
            {
                return this.issuerBinding;
            }
            set
            {
                this.issuerBinding = value;
            }
        }

        [DefaultValue(null)]
        public string IssuedTokenType
        {
            get { return this.issuedTokenType; }
            set { this.issuedTokenType = value; }
        }

        public SecurityKeyType IssuedKeyType
        {
            get { return this.issuedKeyType; }
            set
            {
                if (!SecurityKeyTypeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.issuedKeyType = value;
            }
        }

        public Collection<ClaimTypeRequirement> ClaimTypeRequirements
        {
            get { return this.claimTypeRequirements; }
        }

        public Collection<XmlElement> TokenRequestParameters
        {
            get { return this.tokenRequestParameters; }
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
