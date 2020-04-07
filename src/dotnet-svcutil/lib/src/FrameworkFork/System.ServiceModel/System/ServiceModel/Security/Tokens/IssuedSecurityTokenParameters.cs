// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.


namespace System.ServiceModel.Security.Tokens
{
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.Runtime;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.Text;
    using Microsoft.Xml;

    public class IssuedSecurityTokenParameters : SecurityTokenParameters
    {
        const string wsidPrefix = "wsid";
        const string wsidNamespace = "http://schemas.xmlsoap.org/ws/2005/05/identity";
        static readonly string wsidPPIClaim = String.Format(CultureInfo.InvariantCulture, "{0}/claims/privatepersonalidentifier", wsidNamespace);
        internal const SecurityKeyType defaultKeyType = SecurityKeyType.SymmetricKey;
        internal const bool defaultUseStrTransform = false;

        internal struct AlternativeIssuerEndpoint
        {
            public EndpointAddress IssuerAddress;
            public EndpointAddress IssuerMetadataAddress;
            public Binding IssuerBinding;
        }

        Collection<XmlElement> additionalRequestParameters = new Collection<XmlElement>();
        Collection<AlternativeIssuerEndpoint> alternativeIssuerEndpoints = new Collection<AlternativeIssuerEndpoint>();
        MessageSecurityVersion defaultMessageSecurityVersion;
        EndpointAddress issuerAddress;
        EndpointAddress issuerMetadataAddress;
        Binding issuerBinding;
        int keySize;
        SecurityKeyType keyType = defaultKeyType;
        Collection<ClaimTypeRequirement> claimTypeRequirements = new Collection<ClaimTypeRequirement>();
        bool useStrTransform = defaultUseStrTransform;
        string tokenType;

        protected IssuedSecurityTokenParameters(IssuedSecurityTokenParameters other)
            : base(other)
        {
            this.defaultMessageSecurityVersion = other.defaultMessageSecurityVersion;
            this.issuerAddress = other.issuerAddress;
            this.keyType = other.keyType;
            this.tokenType = other.tokenType;
            this.keySize = other.keySize;
            this.useStrTransform = other.useStrTransform;

            foreach (XmlElement parameter in other.additionalRequestParameters)
            {
                this.additionalRequestParameters.Add((XmlElement)parameter.Clone());
            }
            foreach (ClaimTypeRequirement c in other.claimTypeRequirements)
            {
                this.claimTypeRequirements.Add(c);
            }
            if (other.issuerBinding != null)
            {
                this.issuerBinding = new CustomBinding(other.issuerBinding);
            }
            this.issuerMetadataAddress = other.issuerMetadataAddress;
        }

        public IssuedSecurityTokenParameters()
            : this(null, null, null)
        {
            // empty
        }

        public IssuedSecurityTokenParameters(string tokenType)
            : this(tokenType, null, null)
        {
            // empty
        }

        public IssuedSecurityTokenParameters(string tokenType, EndpointAddress issuerAddress)
            : this(tokenType, issuerAddress, null)
        {
            // empty
        }

        public IssuedSecurityTokenParameters(string tokenType, EndpointAddress issuerAddress, Binding issuerBinding)
            : base()
        {
            this.tokenType = tokenType;
            this.issuerAddress = issuerAddress;
            this.issuerBinding = issuerBinding;
        }

        internal protected override bool HasAsymmetricKey { get { return this.KeyType == SecurityKeyType.AsymmetricKey; } }

        public Collection<XmlElement> AdditionalRequestParameters
        {
            get
            {
                return this.additionalRequestParameters;
            }
        }

        public MessageSecurityVersion DefaultMessageSecurityVersion
        {
            get
            {
                return this.defaultMessageSecurityVersion;
            }

            set
            {
                defaultMessageSecurityVersion = value;
            }
        }

        internal Collection<AlternativeIssuerEndpoint> AlternativeIssuerEndpoints
        {
            get
            {
                return this.alternativeIssuerEndpoints;
            }
        }

        public EndpointAddress IssuerAddress
        {
            get
            {
                return this.issuerAddress;
            }
            set
            {
                this.issuerAddress = value;
            }
        }

        public EndpointAddress IssuerMetadataAddress
        {
            get
            {
                return this.issuerMetadataAddress;
            }
            set
            {
                this.issuerMetadataAddress = value;
            }
        }

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

        public SecurityKeyType KeyType
        {
            get
            {
                return this.keyType;
            }
            set
            {
                SecurityKeyTypeHelper.Validate(value);
                this.keyType = value;
            }
        }

        public int KeySize
        {
            get
            {
                return this.keySize;
            }
            set
            {
                if (value < 0)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", SRServiceModel.ValueMustBeNonNegative));
                this.keySize = value;
            }
        }

        public bool UseStrTransform
        {
            get
            {
                return this.useStrTransform;
            }
            set
            {
                this.useStrTransform = value;
            }
        }

        public Collection<ClaimTypeRequirement> ClaimTypeRequirements
        {
            get
            {
                return this.claimTypeRequirements;
            }
        }

        public string TokenType
        {
            get
            {
                return this.tokenType;
            }
            set
            {
                this.tokenType = value;
            }
        }

        internal protected override bool SupportsClientAuthentication { get { return true; } }
        internal protected override bool SupportsServerAuthentication { get { return true; } }
        internal protected override bool SupportsClientWindowsIdentity { get { return false; } }

        protected override SecurityTokenParameters CloneCore()
        {
            return new IssuedSecurityTokenParameters(this);
        }
        internal void SetRequestParameters(Collection<XmlElement> requestParameters, TrustDriver trustDriver)
        {
            if (requestParameters == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("requestParameters");

            if (trustDriver == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("trustDriver");

            Collection<XmlElement> unknownRequestParameters = new Collection<XmlElement>();

            foreach (XmlElement element in requestParameters)
            {
                int keySize;
                string tokenType;
                SecurityKeyType keyType;
                Collection<XmlElement> requiredClaims;
                if (trustDriver.TryParseKeySizeElement(element, out keySize))
                    this.keySize = keySize;
                else if (trustDriver.TryParseKeyTypeElement(element, out keyType))
                    this.KeyType = keyType;
                else if (trustDriver.TryParseTokenTypeElement(element, out tokenType))
                    this.TokenType = tokenType;
                // Only copy RP policy to client policy for TrustFeb2005
                else if (trustDriver.StandardsManager.TrustVersion == TrustVersion.WSTrustFeb2005)
                {
                    if (trustDriver.TryParseRequiredClaimsElement(element, out requiredClaims))
                    {
                        Collection<XmlElement> unrecognizedRequiredClaims = new Collection<XmlElement>();
                        foreach (XmlElement claimRequirement in requiredClaims)
                        {
                            if (claimRequirement.LocalName == "ClaimType" && claimRequirement.NamespaceURI == wsidNamespace)
                            {
                                string claimValue = claimRequirement.GetAttribute("Uri", string.Empty);
                                if (!string.IsNullOrEmpty(claimValue))
                                {
                                    ClaimTypeRequirement claimTypeRequirement;
                                    string optional = claimRequirement.GetAttribute("Optional", string.Empty);
                                    if (String.IsNullOrEmpty(optional))
                                    {
                                        claimTypeRequirement = new ClaimTypeRequirement(claimValue);
                                    }
                                    else
                                    {
                                        claimTypeRequirement = new ClaimTypeRequirement(claimValue, XmlConvert.ToBoolean(optional));
                                    }

                                    this.claimTypeRequirements.Add(claimTypeRequirement);
                                }
                            }
                            else
                            {
                                unrecognizedRequiredClaims.Add(claimRequirement);
                            }
                        }
                        if (unrecognizedRequiredClaims.Count > 0)
                            unknownRequestParameters.Add(trustDriver.CreateRequiredClaimsElement(unrecognizedRequiredClaims));
                    }
                    else
                    {
                        unknownRequestParameters.Add(element);
                    }
                }
            }

            unknownRequestParameters = trustDriver.ProcessUnknownRequestParameters(unknownRequestParameters, requestParameters);
            if (unknownRequestParameters.Count > 0)
            {
                for (int i = 0; i < unknownRequestParameters.Count; ++i)
                    this.AdditionalRequestParameters.Add(unknownRequestParameters[i]);
            }
        }

        public Collection<XmlElement> CreateRequestParameters(MessageSecurityVersion messageSecurityVersion, SecurityTokenSerializer securityTokenSerializer)
        {
            return CreateRequestParameters(SecurityUtils.CreateSecurityStandardsManager(messageSecurityVersion, securityTokenSerializer).TrustDriver);
        }

        internal Collection<XmlElement> CreateRequestParameters(TrustDriver driver)
        {
            if (driver == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("driver");

            Collection<XmlElement> result = new Collection<XmlElement>();

            if (this.tokenType != null)
            {
                result.Add(driver.CreateTokenTypeElement(tokenType));
            }

            result.Add(driver.CreateKeyTypeElement(this.keyType));

            if (this.keySize != 0)
            {
                result.Add(driver.CreateKeySizeElement(keySize));
            }
            if (this.claimTypeRequirements.Count > 0)
            {
                Collection<XmlElement> claimsElements = new Collection<XmlElement>();
                XmlDocument doc = new XmlDocument();
                foreach (ClaimTypeRequirement claimType in this.claimTypeRequirements)
                {
                    XmlElement element = doc.CreateElement(wsidPrefix, "ClaimType", wsidNamespace);
                    XmlAttribute attr = doc.CreateAttribute("Uri");
                    attr.Value = claimType.ClaimType;
                    element.Attributes.Append(attr);
                    if (claimType.IsOptional != ClaimTypeRequirement.DefaultIsOptional)
                    {
                        attr = doc.CreateAttribute("Optional");
                        attr.Value = XmlConvert.ToString(claimType.IsOptional);
                        element.Attributes.Append(attr);
                    }
                    claimsElements.Add(element);
                }
                result.Add(driver.CreateRequiredClaimsElement(claimsElements));
            }

            if (this.additionalRequestParameters.Count > 0)
            {
                Collection<XmlElement> trustNormalizedParameters = NormalizeAdditionalParameters(this.additionalRequestParameters,
                                                                                                 driver,
                                                                                                 (this.claimTypeRequirements.Count > 0));

                foreach (XmlElement parameter in trustNormalizedParameters)
                {
                    result.Add(parameter);
                }
            }

            return result;
        }

        private Collection<XmlElement> NormalizeAdditionalParameters(Collection<XmlElement> additionalParameters,
                                                                     TrustDriver driver,
                                                                     bool clientSideClaimTypeRequirementsSpecified)
        {
            throw new NotImplementedException();
        }

        private bool CollectionContainsElementsWithTrustNamespace(Collection<XmlElement> collection, string trustNamespace)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if ((collection[i] != null) && (collection[i].NamespaceURI == trustNamespace))
                {
                    return true;
                }
            }
            return false;
        }

        internal void AddAlgorithmParameters(SecurityAlgorithmSuite algorithmSuite, SecurityStandardsManager standardsManager, SecurityKeyType issuedKeyType)
        {
            throw new NotImplementedException();
        }

        internal static IssuedSecurityTokenParameters CreateInfoCardParameters(SecurityStandardsManager standardsManager, SecurityAlgorithmSuite algorithm)
        {
            IssuedSecurityTokenParameters result = new IssuedSecurityTokenParameters(SecurityXXX2005Strings.SamlTokenType);
            result.KeyType = SecurityKeyType.AsymmetricKey;
            result.ClaimTypeRequirements.Add(new ClaimTypeRequirement(wsidPPIClaim));
            result.IssuerAddress = null;
            result.AddAlgorithmParameters(algorithm, standardsManager, result.KeyType);
            return result;
        }

        internal static bool IsInfoCardParameters(IssuedSecurityTokenParameters parameters, SecurityStandardsManager standardsManager)
        {
            if (parameters == null)
                return false;
            if (parameters.TokenType != SecurityXXX2005Strings.SamlTokenType)
                return false;
            if (parameters.KeyType != SecurityKeyType.AsymmetricKey)
                return false;

            if (parameters.ClaimTypeRequirements.Count == 1)
            {
                ClaimTypeRequirement claimTypeRequirement = parameters.ClaimTypeRequirements[0] as ClaimTypeRequirement;
                if (claimTypeRequirement == null)
                    return false;
                if (claimTypeRequirement.ClaimType != wsidPPIClaim)
                    return false;
            }
            else if ((parameters.AdditionalRequestParameters != null) && (parameters.AdditionalRequestParameters.Count > 0))
            {
                // Check the AdditionalRequest Parameters to see if ClaimTypeRequirements got imported there.
                bool claimTypeRequirementMatched = false;
                XmlElement claimTypeRequirement = GetClaimTypeRequirement(parameters.AdditionalRequestParameters, standardsManager);
                if (claimTypeRequirement != null && claimTypeRequirement.ChildNodes.Count == 1)
                {
                    XmlElement claimTypeElement = claimTypeRequirement.ChildNodes[0] as XmlElement;
                    if (claimTypeElement != null)
                    {
                        XmlNode claimType = claimTypeElement.Attributes.GetNamedItem("Uri");
                        if (claimType != null && claimType.Value == wsidPPIClaim)
                        {
                            claimTypeRequirementMatched = true;
                        }
                    }
                }

                if (!claimTypeRequirementMatched)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            if (parameters.IssuerAddress != null)
                return false;
            if (parameters.AlternativeIssuerEndpoints != null && parameters.AlternativeIssuerEndpoints.Count > 0)
            {
                return false;
            }
            return true;
        }

        // The method walks through the entire set of AdditionalRequestParameters and return the Claims Type requirement alone.
        internal static XmlElement GetClaimTypeRequirement(Collection<XmlElement> additionalRequestParameters, SecurityStandardsManager standardsManager)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(base.ToString());

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "TokenType: {0}", this.tokenType == null ? "null" : this.tokenType));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "KeyType: {0}", this.keyType.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "KeySize: {0}", this.keySize.ToString(CultureInfo.InvariantCulture)));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "IssuerAddress: {0}", this.issuerAddress == null ? "null" : this.issuerAddress.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "IssuerMetadataAddress: {0}", this.issuerMetadataAddress == null ? "null" : this.issuerMetadataAddress.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "DefaultMessgeSecurityVersion: {0}", this.defaultMessageSecurityVersion == null ? "null" : this.defaultMessageSecurityVersion.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "UseStrTransform: {0}", this.useStrTransform.ToString()));

            if (this.issuerBinding == null)
            {
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "IssuerBinding: null"));
            }
            else
            {
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "IssuerBinding:"));
                BindingElementCollection bindingElements = this.issuerBinding.CreateBindingElements();
                for (int i = 0; i < bindingElements.Count; i++)
                {
                    sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "  BindingElement[{0}]:", i.ToString(CultureInfo.InvariantCulture)));
                    sb.AppendLine("    " + bindingElements[i].ToString().Trim().Replace("\n", "\n    "));
                }
            }

            if (this.claimTypeRequirements.Count == 0)
            {
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "ClaimTypeRequirements: none"));
            }
            else
            {
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "ClaimTypeRequirements:"));
                for (int i = 0; i < this.claimTypeRequirements.Count; i++)
                {
                    sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "  {0}, optional={1}", this.claimTypeRequirements[i].ClaimType, this.claimTypeRequirements[i].IsOptional));
                }
            }

            return sb.ToString().Trim();
        }
    }
}
