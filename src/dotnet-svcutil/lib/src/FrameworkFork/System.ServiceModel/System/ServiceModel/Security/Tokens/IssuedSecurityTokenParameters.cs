// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


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
        private const string wsidPrefix = "wsid";
        private const string wsidNamespace = "http://schemas.xmlsoap.org/ws/2005/05/identity";
        private static readonly string s_wsidPPIClaim = String.Format(CultureInfo.InvariantCulture, "{0}/claims/privatepersonalidentifier", wsidNamespace);
        internal const SecurityKeyType defaultKeyType = SecurityKeyType.SymmetricKey;
        internal const bool defaultUseStrTransform = false;

        internal struct AlternativeIssuerEndpoint
        {
            public EndpointAddress IssuerAddress;
            public EndpointAddress IssuerMetadataAddress;
            public Binding IssuerBinding;
        }

        private Collection<XmlElement> _additionalRequestParameters = new Collection<XmlElement>();
        private Collection<AlternativeIssuerEndpoint> _alternativeIssuerEndpoints = new Collection<AlternativeIssuerEndpoint>();
        private MessageSecurityVersion _defaultMessageSecurityVersion;
        private EndpointAddress _issuerAddress;
        private EndpointAddress _issuerMetadataAddress;
        private Binding _issuerBinding;
        private int _keySize;
        private SecurityKeyType _keyType = defaultKeyType;
        private Collection<ClaimTypeRequirement> _claimTypeRequirements = new Collection<ClaimTypeRequirement>();
        private bool _useStrTransform = defaultUseStrTransform;
        private string _tokenType;

        protected IssuedSecurityTokenParameters(IssuedSecurityTokenParameters other)
            : base(other)
        {
            _defaultMessageSecurityVersion = other._defaultMessageSecurityVersion;
            _issuerAddress = other._issuerAddress;
            _keyType = other._keyType;
            _tokenType = other._tokenType;
            _keySize = other._keySize;
            _useStrTransform = other._useStrTransform;

            foreach (XmlElement parameter in other._additionalRequestParameters)
            {
                _additionalRequestParameters.Add((XmlElement)parameter.Clone());
            }
            foreach (ClaimTypeRequirement c in other._claimTypeRequirements)
            {
                _claimTypeRequirements.Add(c);
            }
            if (other._issuerBinding != null)
            {
                _issuerBinding = new CustomBinding(other._issuerBinding);
            }
            _issuerMetadataAddress = other._issuerMetadataAddress;
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
            _tokenType = tokenType;
            _issuerAddress = issuerAddress;
            _issuerBinding = issuerBinding;
        }

        internal protected override bool HasAsymmetricKey { get { return this.KeyType == SecurityKeyType.AsymmetricKey; } }

        public Collection<XmlElement> AdditionalRequestParameters
        {
            get
            {
                return _additionalRequestParameters;
            }
        }

        public MessageSecurityVersion DefaultMessageSecurityVersion
        {
            get
            {
                return _defaultMessageSecurityVersion;
            }

            set
            {
                _defaultMessageSecurityVersion = value;
            }
        }

        internal Collection<AlternativeIssuerEndpoint> AlternativeIssuerEndpoints
        {
            get
            {
                return _alternativeIssuerEndpoints;
            }
        }

        public EndpointAddress IssuerAddress
        {
            get
            {
                return _issuerAddress;
            }
            set
            {
                _issuerAddress = value;
            }
        }

        public EndpointAddress IssuerMetadataAddress
        {
            get
            {
                return _issuerMetadataAddress;
            }
            set
            {
                _issuerMetadataAddress = value;
            }
        }

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

        public SecurityKeyType KeyType
        {
            get
            {
                return _keyType;
            }
            set
            {
                SecurityKeyTypeHelper.Validate(value);
                _keyType = value;
            }
        }

        public int KeySize
        {
            get
            {
                return _keySize;
            }
            set
            {
                if (value < 0)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", SRServiceModel.ValueMustBeNonNegative));
                _keySize = value;
            }
        }

        public bool UseStrTransform
        {
            get
            {
                return _useStrTransform;
            }
            set
            {
                _useStrTransform = value;
            }
        }

        public Collection<ClaimTypeRequirement> ClaimTypeRequirements
        {
            get
            {
                return _claimTypeRequirements;
            }
        }

        public string TokenType
        {
            get
            {
                return _tokenType;
            }
            set
            {
                _tokenType = value;
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
                    _keySize = keySize;
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

                                    _claimTypeRequirements.Add(claimTypeRequirement);
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

            if (_tokenType != null)
            {
                result.Add(driver.CreateTokenTypeElement(_tokenType));
            }

            result.Add(driver.CreateKeyTypeElement(_keyType));

            if (_keySize != 0)
            {
                result.Add(driver.CreateKeySizeElement(_keySize));
            }
            if (_claimTypeRequirements.Count > 0)
            {
                Collection<XmlElement> claimsElements = new Collection<XmlElement>();
                XmlDocument doc = new XmlDocument();
                foreach (ClaimTypeRequirement claimType in _claimTypeRequirements)
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

            if (_additionalRequestParameters.Count > 0)
            {
                Collection<XmlElement> trustNormalizedParameters = NormalizeAdditionalParameters(_additionalRequestParameters,
                                                                                                 driver,
                                                                                                 (_claimTypeRequirements.Count > 0));

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
            this._additionalRequestParameters.Insert(0, standardsManager.TrustDriver.CreateEncryptionAlgorithmElement(algorithmSuite.DefaultEncryptionAlgorithm));
            this._additionalRequestParameters.Insert(0, standardsManager.TrustDriver.CreateCanonicalizationAlgorithmElement(algorithmSuite.DefaultCanonicalizationAlgorithm));

            if (this._keyType == SecurityKeyType.BearerKey)
            {
                // As the client does not have a proof token in the Bearer case
                // we don't have any specific algorithms to request for.
                return;
            }

            string signWithAlgorithm = (this._keyType == SecurityKeyType.SymmetricKey) ? algorithmSuite.DefaultSymmetricSignatureAlgorithm : algorithmSuite.DefaultAsymmetricSignatureAlgorithm;
            this._additionalRequestParameters.Insert(0, standardsManager.TrustDriver.CreateSignWithElement(signWithAlgorithm));
            string encryptWithAlgorithm;
            if (issuedKeyType == SecurityKeyType.SymmetricKey)
            {
                encryptWithAlgorithm = algorithmSuite.DefaultEncryptionAlgorithm;
            }
            else
            {
                encryptWithAlgorithm = algorithmSuite.DefaultAsymmetricKeyWrapAlgorithm;
            }
            this._additionalRequestParameters.Insert(0, standardsManager.TrustDriver.CreateEncryptWithElement(encryptWithAlgorithm));

            if (standardsManager.TrustVersion != TrustVersion.WSTrustFeb2005)
            {
                this._additionalRequestParameters.Insert(0, ((WSTrustDec2005.DriverDec2005)standardsManager.TrustDriver).CreateKeyWrapAlgorithmElement(algorithmSuite.DefaultAsymmetricKeyWrapAlgorithm));
            }

            return;
        }

        internal bool DoAlgorithmsMatch(SecurityAlgorithmSuite algorithmSuite, SecurityStandardsManager standardsManager, out Collection<XmlElement> otherRequestParameters)
        {
            bool doesSignWithAlgorithmMatch = false;
            bool doesEncryptWithAlgorithmMatch = false;
            bool doesEncryptionAlgorithmMatch = false;
            bool doesCanonicalizationAlgorithmMatch = false;
            bool doesKeyWrapAlgorithmMatch = false;
            otherRequestParameters = new Collection<XmlElement>();
            bool trustNormalizationPerformed = false;

            Collection<XmlElement> trustVersionNormalizedParameterCollection;

            // For Trust 1.3 we move all the additional parameters into the secondaryParameters
            // element. So the list contains just one element called SecondaryParameters that 
            // contains all the other elements as child elements.
            if ((standardsManager.TrustVersion == TrustVersion.WSTrust13) &&
                (this.AdditionalRequestParameters.Count == 1) &&
                (((WSTrustDec2005.DriverDec2005)standardsManager.TrustDriver).IsSecondaryParametersElement(this.AdditionalRequestParameters[0])))
            {
                trustNormalizationPerformed = true;
                trustVersionNormalizedParameterCollection = new Collection<XmlElement>();
                foreach (XmlElement innerElement in this.AdditionalRequestParameters[0])
                {
                    trustVersionNormalizedParameterCollection.Add(innerElement);
                }
            }
            else
            {
                trustVersionNormalizedParameterCollection = this.AdditionalRequestParameters;
            }

            for (int i = 0; i < trustVersionNormalizedParameterCollection.Count; i++)
            {
                string algorithm;
                XmlElement element = trustVersionNormalizedParameterCollection[i];
                if (standardsManager.TrustDriver.IsCanonicalizationAlgorithmElement(element, out algorithm))
                {
                    if (algorithmSuite.DefaultCanonicalizationAlgorithm != algorithm)
                    {
                        return false;
                    }
                    doesCanonicalizationAlgorithmMatch = true;
                }
                else if (standardsManager.TrustDriver.IsSignWithElement(element, out algorithm))
                {
                    if ((this._keyType == SecurityKeyType.SymmetricKey && algorithm != algorithmSuite.DefaultSymmetricSignatureAlgorithm)
                        || (this._keyType == SecurityKeyType.AsymmetricKey && algorithm != algorithmSuite.DefaultAsymmetricSignatureAlgorithm))
                    {
                        return false;
                    }
                    doesSignWithAlgorithmMatch = true;
                }
                else if (standardsManager.TrustDriver.IsEncryptWithElement(element, out algorithm))
                {
                    if ((this._keyType == SecurityKeyType.SymmetricKey && algorithm != algorithmSuite.DefaultEncryptionAlgorithm)
                        || (this._keyType == SecurityKeyType.AsymmetricKey && algorithm != algorithmSuite.DefaultAsymmetricKeyWrapAlgorithm))
                    {
                        return false;
                    }
                    doesEncryptWithAlgorithmMatch = true;
                }
                else if (standardsManager.TrustDriver.IsEncryptionAlgorithmElement(element, out algorithm))
                {
                    if (algorithm != algorithmSuite.DefaultEncryptionAlgorithm)
                    {
                        return false;
                    }
                    doesEncryptionAlgorithmMatch = true;
                }
                else if (standardsManager.TrustDriver.IsKeyWrapAlgorithmElement(element, out algorithm))
                {
                    if (algorithm != algorithmSuite.DefaultAsymmetricKeyWrapAlgorithm)
                    {
                        return false;
                    }
                    doesKeyWrapAlgorithmMatch = true;
                }
                else
                {
                    otherRequestParameters.Add(element);
                }
            }

            // Undo normalization if performed
            // move all back into secondaryParameters
            if (trustNormalizationPerformed)
            {
                otherRequestParameters = this.AdditionalRequestParameters;
            }

            if (this._keyType == SecurityKeyType.BearerKey)
            {
                // As the client does not have a proof token in the Bearer case
                // we don't have any specific algorithms to request for.
                return true;
            }
            if (standardsManager.TrustVersion == TrustVersion.WSTrustFeb2005)
            {
                // For V1 compatibility check all algorithms
                return (doesSignWithAlgorithmMatch && doesCanonicalizationAlgorithmMatch && doesEncryptionAlgorithmMatch && doesEncryptWithAlgorithmMatch);
            }
            else
            {
                return (doesSignWithAlgorithmMatch && doesCanonicalizationAlgorithmMatch && doesEncryptionAlgorithmMatch && doesEncryptWithAlgorithmMatch && doesKeyWrapAlgorithmMatch);
            }
        }

        internal static IssuedSecurityTokenParameters CreateInfoCardParameters(SecurityStandardsManager standardsManager, SecurityAlgorithmSuite algorithm)
        {
            IssuedSecurityTokenParameters result = new IssuedSecurityTokenParameters(SecurityXXX2005Strings.SamlTokenType);
            result.KeyType = SecurityKeyType.AsymmetricKey;
            result.ClaimTypeRequirements.Add(new ClaimTypeRequirement(s_wsidPPIClaim));
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
                if (claimTypeRequirement.ClaimType != s_wsidPPIClaim)
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
                        if (claimType != null && claimType.Value == s_wsidPPIClaim)
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

            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "TokenType: {0}", _tokenType == null ? "null" : _tokenType));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "KeyType: {0}", _keyType.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "KeySize: {0}", _keySize.ToString(CultureInfo.InvariantCulture)));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "IssuerAddress: {0}", _issuerAddress == null ? "null" : _issuerAddress.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "IssuerMetadataAddress: {0}", _issuerMetadataAddress == null ? "null" : _issuerMetadataAddress.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "DefaultMessgeSecurityVersion: {0}", _defaultMessageSecurityVersion == null ? "null" : _defaultMessageSecurityVersion.ToString()));
            sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "UseStrTransform: {0}", _useStrTransform.ToString()));

            if (_issuerBinding == null)
            {
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "IssuerBinding: null"));
            }
            else
            {
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "IssuerBinding:"));
                BindingElementCollection bindingElements = _issuerBinding.CreateBindingElements();
                for (int i = 0; i < bindingElements.Count; i++)
                {
                    sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "  BindingElement[{0}]:", i.ToString(CultureInfo.InvariantCulture)));
                    sb.AppendLine("    " + bindingElements[i].ToString().Trim().Replace("\n", "\n    "));
                }
            }

            if (_claimTypeRequirements.Count == 0)
            {
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "ClaimTypeRequirements: none"));
            }
            else
            {
                sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "ClaimTypeRequirements:"));
                for (int i = 0; i < _claimTypeRequirements.Count; i++)
                {
                    sb.AppendLine(String.Format(CultureInfo.InvariantCulture, "  {0}, optional={1}", _claimTypeRequirements[i].ClaimType, _claimTypeRequirements[i].IsOptional));
                }
            }

            return sb.ToString().Trim();
        }
    }
}
