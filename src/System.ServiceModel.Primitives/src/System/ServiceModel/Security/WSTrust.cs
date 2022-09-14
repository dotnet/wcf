// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Runtime;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using HexBinary = System.Runtime.Remoting.Metadata.W3cXsd2001.SoapHexBinary;
using System.Runtime.Serialization;
using TokenEntry = System.ServiceModel.Security.WSSecurityTokenSerializer.TokenEntry;
//using Psha1DerivedKeyGenerator = System.IdentityModel.Psha1DerivedKeyGenerator;

namespace System.ServiceModel.Security
{
    internal abstract class WSTrust : WSSecurityTokenSerializer.SerializerEntries
    {
        public WSTrust(WSSecurityTokenSerializer tokenSerializer)
        {
            WSSecurityTokenSerializer = tokenSerializer;
        }

        public WSSecurityTokenSerializer WSSecurityTokenSerializer { get; }

        public abstract TrustDictionary SerializerDictionary
        {
            get;
        }

        public override void PopulateTokenEntries(IList<TokenEntry> tokenEntryList)
        {
            tokenEntryList.Add(new BinarySecretTokenEntry(this));
        }

        private class BinarySecretTokenEntry : TokenEntry
        {
            private WSTrust _parent;
            private TrustDictionary _otherDictionary;

            public BinarySecretTokenEntry(WSTrust parent)
            {
                _parent = parent;
                _otherDictionary = null;

                if (parent.SerializerDictionary is TrustDec2005Dictionary)
                {
                    _otherDictionary = XD.TrustFeb2005Dictionary;
                }

                if (parent.SerializerDictionary is TrustFeb2005Dictionary)
                {
                    _otherDictionary = DXD.TrustDec2005Dictionary;
                }

                // always set it, so we don't have to worry about null
                if (_otherDictionary == null)
                {
                    _otherDictionary = _parent.SerializerDictionary;
                }
            }

            protected override XmlDictionaryString LocalName { get { return _parent.SerializerDictionary.BinarySecret; } }
            protected override XmlDictionaryString NamespaceUri { get { return _parent.SerializerDictionary.Namespace; } }
            protected override Type[] GetTokenTypesCore() { return new Type[] { typeof(BinarySecretSecurityToken) }; }
            public override string TokenTypeUri { get { return null; } }
            protected override string ValueTypeUri { get { return null; } }

            public override bool CanReadTokenCore(XmlElement element)
            {
                string valueTypeUri = null;

                if (element.HasAttribute(SecurityJan2004Strings.ValueType, null))
                {
                    valueTypeUri = element.GetAttribute(SecurityJan2004Strings.ValueType, null);
                }

                return element.LocalName == LocalName.Value && (element.NamespaceURI == NamespaceUri.Value || element.NamespaceURI == _otherDictionary.Namespace.Value) && valueTypeUri == ValueTypeUri;
            }

            public override bool CanReadTokenCore(XmlDictionaryReader reader)
            {
                return (reader.IsStartElement(LocalName, NamespaceUri) || reader.IsStartElement(LocalName, _otherDictionary.Namespace)) &&
                       reader.GetAttribute(XD.SecurityJan2004Dictionary.ValueType, null) == ValueTypeUri;
            }


            public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXmlCore(XmlElement issuedTokenXml,
                SecurityTokenReferenceStyle tokenReferenceStyle)
            {
                TokenReferenceStyleHelper.Validate(tokenReferenceStyle);

                switch (tokenReferenceStyle)
                {
                    case SecurityTokenReferenceStyle.Internal:
                        return CreateDirectReference(issuedTokenXml, UtilityStrings.IdAttribute, UtilityStrings.Namespace, typeof(GenericXmlSecurityToken));
                    case SecurityTokenReferenceStyle.External:
                        // Binary Secret tokens aren't referred to externally
                        return null;
                    default:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(tokenReferenceStyle)));
                }
            }

            public override SecurityToken ReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver)
            {
                string secretType = reader.GetAttribute(XD.SecurityJan2004Dictionary.TypeAttribute, null);
                string id = reader.GetAttribute(XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace);
                bool isNonce = false;

                if (secretType != null && secretType.Length > 0)
                {
                    if (secretType == _parent.SerializerDictionary.NonceBinarySecret.Value || secretType == _otherDictionary.NonceBinarySecret.Value)
                    {
                        isNonce = true;
                    }
                    else if (secretType != _parent.SerializerDictionary.SymmetricKeyBinarySecret.Value && secretType != _otherDictionary.SymmetricKeyBinarySecret.Value)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.Format(SRP.UnexpectedBinarySecretType, _parent.SerializerDictionary.SymmetricKeyBinarySecret.Value, secretType)));
                    }
                }

                byte[] secret = reader.ReadElementContentAsBase64();
                if (isNonce)
                {
                    return new NonceToken(id, secret);
                }
                else
                {
                    return new BinarySecretSecurityToken(id, secret);
                }
            }

            public override void WriteTokenCore(XmlDictionaryWriter writer, SecurityToken token)
            {
                BinarySecretSecurityToken simpleToken = token as BinarySecretSecurityToken;
                byte[] secret = simpleToken.GetKeyBytes();
                writer.WriteStartElement(_parent.SerializerDictionary.Prefix.Value, _parent.SerializerDictionary.BinarySecret, _parent.SerializerDictionary.Namespace);
                if (simpleToken.Id != null)
                {
                    writer.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, simpleToken.Id);
                }
                if (token is NonceToken)
                {
                    writer.WriteAttributeString(XD.SecurityJan2004Dictionary.TypeAttribute, null, _parent.SerializerDictionary.NonceBinarySecret.Value);
                }
                writer.WriteBase64(secret, 0, secret.Length);
                writer.WriteEndElement();
            }
        }

        public abstract class Driver : TrustDriver
        {
            private static readonly string s_base64Uri = SecurityJan2004Strings.EncodingTypeValueBase64Binary;
            private static readonly string s_hexBinaryUri = SecurityJan2004Strings.EncodingTypeValueHexBinary;


            private SecurityStandardsManager _standardsManager;
            private List<SecurityTokenAuthenticator> _entropyAuthenticators;

            public Driver(SecurityStandardsManager standardsManager)
            {
                _standardsManager = standardsManager;
                _entropyAuthenticators = new List<SecurityTokenAuthenticator>(2);
            }

            public abstract TrustDictionary DriverDictionary
            {
                get;
            }

            public override XmlDictionaryString RequestSecurityTokenAction
            {
                get
                {
                    return DriverDictionary.RequestSecurityTokenIssuance;
                }
            }

            public override XmlDictionaryString RequestSecurityTokenResponseAction
            {
                get
                {
                    return DriverDictionary.RequestSecurityTokenIssuanceResponse;
                }
            }

            public override string RequestTypeIssue
            {
                get
                {
                    return DriverDictionary.RequestTypeIssue.Value;
                }
            }

            public override string ComputedKeyAlgorithm
            {
                get { return DriverDictionary.Psha1ComputedKeyUri.Value; }
            }

            public override SecurityStandardsManager StandardsManager
            {
                get
                {
                    return _standardsManager;
                }
            }

            public override XmlDictionaryString Namespace
            {
                get { return DriverDictionary.Namespace; }
            }

            public override RequestSecurityToken CreateRequestSecurityToken(XmlReader xmlReader)
            {
                XmlDictionaryReader reader = XmlDictionaryReader.CreateDictionaryReader(xmlReader);
                reader.MoveToStartElement(DriverDictionary.RequestSecurityToken, DriverDictionary.Namespace);
                string context = null;
                string tokenTypeUri = null;
                string requestType = null;
                int keySize = 0;
                XmlDocument doc = new XmlDocument();
                XmlElement rstXml = (doc.ReadNode(reader) as XmlElement);
                SecurityKeyIdentifierClause renewTarget = null;
                SecurityKeyIdentifierClause closeTarget = null;
                for (int i = 0; i < rstXml.Attributes.Count; ++i)
                {
                    XmlAttribute attr = rstXml.Attributes[i];
                    if (attr.LocalName == DriverDictionary.Context.Value)
                    {
                        context = attr.Value;
                    }
                }
                for (int i = 0; i < rstXml.ChildNodes.Count; ++i)
                {
                    XmlElement child = (rstXml.ChildNodes[i] as XmlElement);
                    if (child != null)
                    {
                        if (child.LocalName == DriverDictionary.TokenType.Value && child.NamespaceURI == DriverDictionary.Namespace.Value)
                        {
                            tokenTypeUri = XmlHelper.ReadTextElementAsTrimmedString(child);
                        }
                        else if (child.LocalName == DriverDictionary.RequestType.Value && child.NamespaceURI == DriverDictionary.Namespace.Value)
                        {
                            requestType = XmlHelper.ReadTextElementAsTrimmedString(child);
                        }
                        else if (child.LocalName == DriverDictionary.KeySize.Value && child.NamespaceURI == DriverDictionary.Namespace.Value)
                        {
                            keySize = Int32.Parse(XmlHelper.ReadTextElementAsTrimmedString(child), NumberFormatInfo.InvariantInfo);
                        }
                    }
                }

                ReadTargets(rstXml, out renewTarget, out closeTarget);

                RequestSecurityToken rst = new RequestSecurityToken(_standardsManager, rstXml, context, tokenTypeUri, requestType, keySize, renewTarget, closeTarget);
                return rst;
            }

            private XmlBuffer GetIssuedTokenBuffer(XmlBuffer rstrBuffer)
            {
                XmlBuffer issuedTokenBuffer = null;
                using (XmlDictionaryReader reader = rstrBuffer.GetReader(0))
                {
                    reader.ReadFullStartElement();
                    while (reader.IsStartElement())
                    {
                        if (reader.IsStartElement(DriverDictionary.RequestedSecurityToken, DriverDictionary.Namespace))
                        {
                            reader.ReadStartElement();
                            reader.MoveToContent();
                            issuedTokenBuffer = new XmlBuffer(Int32.MaxValue);
                            using (XmlDictionaryWriter writer = issuedTokenBuffer.OpenSection(reader.Quotas))
                            {
                                writer.WriteNode(reader, false);
                                issuedTokenBuffer.CloseSection();
                                issuedTokenBuffer.Close();
                            }
                            reader.ReadEndElement();
                            break;
                        }
                        else
                        {
                            reader.Skip();
                        }
                    }
                }
                return issuedTokenBuffer;
            }

            public override RequestSecurityTokenResponse CreateRequestSecurityTokenResponse(XmlReader xmlReader)
            {
                if (xmlReader == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(xmlReader));
                }
                XmlDictionaryReader reader = XmlDictionaryReader.CreateDictionaryReader(xmlReader);
                if (reader.IsStartElement(DriverDictionary.RequestSecurityTokenResponse, DriverDictionary.Namespace) == false)
                {
                    XmlHelper.OnRequiredElementMissing(DriverDictionary.RequestSecurityTokenResponse.Value, DriverDictionary.Namespace.Value);
                }

                XmlBuffer rstrBuffer = new XmlBuffer(int.MaxValue);
                using (XmlDictionaryWriter writer = rstrBuffer.OpenSection(reader.Quotas))
                {
                    writer.WriteNode(reader, false);
                    rstrBuffer.CloseSection();
                    rstrBuffer.Close();
                }
                XmlDocument doc = new XmlDocument();
                XmlElement rstrXml;
                using (XmlReader reader2 = rstrBuffer.GetReader(0))
                {
                    rstrXml = (doc.ReadNode(reader2) as XmlElement);
                }

                XmlBuffer issuedTokenBuffer = GetIssuedTokenBuffer(rstrBuffer);
                string context = null;
                string tokenTypeUri = null;
                int keySize = 0;
                SecurityKeyIdentifierClause requestedAttachedReference = null;
                SecurityKeyIdentifierClause requestedUnattachedReference = null;
                bool computeKey = false;
                DateTime created = DateTime.UtcNow;
                DateTime expires = SecurityUtils.MaxUtcDateTime;
                bool isRequestedTokenClosed = false;
                for (int i = 0; i < rstrXml.Attributes.Count; ++i)
                {
                    XmlAttribute attr = rstrXml.Attributes[i];
                    if (attr.LocalName == DriverDictionary.Context.Value)
                    {
                        context = attr.Value;
                    }
                }

                for (int i = 0; i < rstrXml.ChildNodes.Count; ++i)
                {
                    XmlElement child = (rstrXml.ChildNodes[i] as XmlElement);
                    if (child != null)
                    {
                        if (child.LocalName == DriverDictionary.TokenType.Value && child.NamespaceURI == DriverDictionary.Namespace.Value)
                        {
                            tokenTypeUri = XmlHelper.ReadTextElementAsTrimmedString(child);
                        }
                        else if (child.LocalName == DriverDictionary.KeySize.Value && child.NamespaceURI == DriverDictionary.Namespace.Value)
                        {
                            keySize = Int32.Parse(XmlHelper.ReadTextElementAsTrimmedString(child), NumberFormatInfo.InvariantInfo);
                        }
                        else if (child.LocalName == DriverDictionary.RequestedProofToken.Value && child.NamespaceURI == DriverDictionary.Namespace.Value)
                        {
                            XmlElement proofXml = XmlHelper.GetChildElement(child);
                            if (proofXml.LocalName == DriverDictionary.ComputedKey.Value && proofXml.NamespaceURI == DriverDictionary.Namespace.Value)
                            {
                                string computedKeyAlgorithm = XmlHelper.ReadTextElementAsTrimmedString(proofXml);
                                if (computedKeyAlgorithm != DriverDictionary.Psha1ComputedKeyUri.Value)
                                {
                                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new SecurityNegotiationException(SRP.Format(SRP.UnknownComputedKeyAlgorithm, computedKeyAlgorithm)));
                                }
                                computeKey = true;
                            }
                        }
                        else if (child.LocalName == DriverDictionary.Lifetime.Value && child.NamespaceURI == DriverDictionary.Namespace.Value)
                        {
                            XmlElement createdXml = XmlHelper.GetChildElement(child, UtilityStrings.CreatedElement, UtilityStrings.Namespace);
                            if (createdXml != null)
                            {
                                created = DateTime.ParseExact(XmlHelper.ReadTextElementAsTrimmedString(createdXml),
                                    WSUtilitySpecificationVersion.AcceptedDateTimeFormats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None).ToUniversalTime();
                            }
                            XmlElement expiresXml = XmlHelper.GetChildElement(child, UtilityStrings.ExpiresElement, UtilityStrings.Namespace);
                            if (expiresXml != null)
                            {
                                expires = DateTime.ParseExact(XmlHelper.ReadTextElementAsTrimmedString(expiresXml),
                                    WSUtilitySpecificationVersion.AcceptedDateTimeFormats, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None).ToUniversalTime();
                            }
                        }
                    }
                }

                isRequestedTokenClosed = ReadRequestedTokenClosed(rstrXml);
                ReadReferences(rstrXml, out requestedAttachedReference, out requestedUnattachedReference);

                return new RequestSecurityTokenResponse(_standardsManager, rstrXml, context, tokenTypeUri, keySize, requestedAttachedReference, requestedUnattachedReference,
                                                        computeKey, created, expires, isRequestedTokenClosed, issuedTokenBuffer);
            }

            public override RequestSecurityTokenResponseCollection CreateRequestSecurityTokenResponseCollection(XmlReader xmlReader)
            {
                XmlDictionaryReader reader = XmlDictionaryReader.CreateDictionaryReader(xmlReader);
                List<RequestSecurityTokenResponse> rstrCollection = new List<RequestSecurityTokenResponse>(2);
                string rootName = reader.Name;
                reader.ReadStartElement(DriverDictionary.RequestSecurityTokenResponseCollection, DriverDictionary.Namespace);
                while (reader.IsStartElement(DriverDictionary.RequestSecurityTokenResponse.Value, DriverDictionary.Namespace.Value))
                {
                    RequestSecurityTokenResponse rstr = CreateRequestSecurityTokenResponse(reader);
                    rstrCollection.Add(rstr);
                }
                reader.ReadEndElement();
                if (rstrCollection.Count == 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SRP.NoRequestSecurityTokenResponseElements));
                }

                return new RequestSecurityTokenResponseCollection(rstrCollection.AsReadOnly(), StandardsManager);
            }

            private T GetAppliesTo<T>(XmlElement rootXml, XmlObjectSerializer serializer)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            public override T GetAppliesTo<T>(RequestSecurityToken rst, XmlObjectSerializer serializer)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            public override T GetAppliesTo<T>(RequestSecurityTokenResponse rstr, XmlObjectSerializer serializer)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            public override bool IsAppliesTo(string localName, string namespaceUri)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            public override void GetAppliesToQName(RequestSecurityToken rst, out string localName, out string namespaceUri)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            public override void GetAppliesToQName(RequestSecurityTokenResponse rstr, out string localName, out string namespaceUri)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            public override byte[] GetAuthenticator(RequestSecurityTokenResponse rstr)
            {
                if (rstr != null && rstr.RequestSecurityTokenResponseXml != null && rstr.RequestSecurityTokenResponseXml.ChildNodes != null)
                {
                    for (int i = 0; i < rstr.RequestSecurityTokenResponseXml.ChildNodes.Count; ++i)
                    {
                        XmlElement element = rstr.RequestSecurityTokenResponseXml.ChildNodes[i] as XmlElement;
                        if (element != null)
                        {
                            if (element.LocalName == DriverDictionary.Authenticator.Value && element.NamespaceURI == DriverDictionary.Namespace.Value)
                            {
                                XmlElement combinedHashElement = XmlHelper.GetChildElement(element);
                                if (combinedHashElement.LocalName == DriverDictionary.CombinedHash.Value && combinedHashElement.NamespaceURI == DriverDictionary.Namespace.Value)
                                {
                                    string authenticatorString = XmlHelper.ReadTextElementAsTrimmedString(combinedHashElement);
                                    return Convert.FromBase64String(authenticatorString);
                                }
                            }
                        }
                    }
                }
                return null;
            }

            public override BinaryNegotiation GetBinaryNegotiation(RequestSecurityTokenResponse rstr)
            {
                if (rstr == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(rstr));
                }

                return GetBinaryNegotiation(rstr.RequestSecurityTokenResponseXml);
            }

            public override BinaryNegotiation GetBinaryNegotiation(RequestSecurityToken rst)
            {
                if (rst == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(rst));
                }

                return GetBinaryNegotiation(rst.RequestSecurityTokenXml);
            }

            private BinaryNegotiation GetBinaryNegotiation(XmlElement rootElement)
            {
                if (rootElement == null)
                {
                    return null;
                }
                for (int i = 0; i < rootElement.ChildNodes.Count; ++i)
                {
                    XmlElement elem = rootElement.ChildNodes[i] as XmlElement;
                    if (elem != null)
                    {
                        if (elem.LocalName == DriverDictionary.BinaryExchange.Value && elem.NamespaceURI == DriverDictionary.Namespace.Value)
                        {
                            return ReadBinaryNegotiation(elem);
                        }
                    }
                }
                return null;
            }

            public override SecurityToken GetEntropy(RequestSecurityToken rst, SecurityTokenResolver resolver)
            {
                if (rst == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(rst));
                }

                return GetEntropy(rst.RequestSecurityTokenXml, resolver);
            }

            public override SecurityToken GetEntropy(RequestSecurityTokenResponse rstr, SecurityTokenResolver resolver)
            {
                if (rstr == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(rstr));
                }

                return GetEntropy(rstr.RequestSecurityTokenResponseXml, resolver);
            }

            private SecurityToken GetEntropy(XmlElement rootElement, SecurityTokenResolver resolver)
            {
                if (rootElement == null || rootElement.ChildNodes == null)
                {
                    return null;
                }
                for (int i = 0; i < rootElement.ChildNodes.Count; ++i)
                {
                    XmlElement element = rootElement.ChildNodes[i] as XmlElement;
                    if (element != null)
                    {
                        if (element.LocalName == DriverDictionary.Entropy.Value && element.NamespaceURI == DriverDictionary.Namespace.Value)
                        {
                            XmlElement tokenXml = XmlHelper.GetChildElement(element);
                            string valueTypeUri = element.GetAttribute(SecurityJan2004Strings.ValueType);
                            if (valueTypeUri.Length == 0)
                            {
                                valueTypeUri = null;
                            }

                            return _standardsManager.SecurityTokenSerializer.ReadToken(new XmlNodeReader(tokenXml), resolver);
                        }
                    }
                }
                return null;
            }

            private void GetIssuedAndProofXml(RequestSecurityTokenResponse rstr, out XmlElement issuedTokenXml, out XmlElement proofTokenXml)
            {
                issuedTokenXml = null;
                proofTokenXml = null;
                if ((rstr.RequestSecurityTokenResponseXml != null) && (rstr.RequestSecurityTokenResponseXml.ChildNodes != null))
                {
                    for (int i = 0; i < rstr.RequestSecurityTokenResponseXml.ChildNodes.Count; ++i)
                    {
                        XmlElement elem = rstr.RequestSecurityTokenResponseXml.ChildNodes[i] as XmlElement;
                        if (elem != null)
                        {
                            if (elem.LocalName == DriverDictionary.RequestedSecurityToken.Value && elem.NamespaceURI == DriverDictionary.Namespace.Value)
                            {
                                if (issuedTokenXml != null)
                                {
                                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.RstrHasMultipleIssuedTokens));
                                }
                                issuedTokenXml = XmlHelper.GetChildElement(elem);
                            }
                            else if (elem.LocalName == DriverDictionary.RequestedProofToken.Value && elem.NamespaceURI == DriverDictionary.Namespace.Value)
                            {
                                if (proofTokenXml != null)
                                {
                                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.RstrHasMultipleProofTokens));
                                }
                                proofTokenXml = XmlHelper.GetChildElement(elem);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// The algorithm for computing the key is:
            /// 1. If there is requestorEntropy:
            ///    a. If there is no <RequestedProofToken> use the requestorEntropy as the key
            ///    b. If there is a <RequestedProofToken> with a ComputedKeyUri, combine the client and server entropies
            ///    c. Anything else, throw
            /// 2. If there is no requestorEntropy:
            ///    a. THere has to be a <RequestedProofToken> that contains the proof key
            /// </summary>
            public override GenericXmlSecurityToken GetIssuedToken(RequestSecurityTokenResponse rstr, SecurityTokenResolver resolver, IList<SecurityTokenAuthenticator> allowedAuthenticators, SecurityKeyEntropyMode keyEntropyMode, byte[] requestorEntropy, string expectedTokenType,
                ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies, int defaultKeySize, bool isBearerKeyType)
            {
                SecurityKeyEntropyModeHelper.Validate(keyEntropyMode);

                if (defaultKeySize < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(defaultKeySize), SRP.ValueMustBeNonNegative));
                }

                if (rstr == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(rstr));
                }

                string tokenType;
                if (rstr.TokenType != null)
                {
                    if (expectedTokenType != null && expectedTokenType != rstr.TokenType)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.BadIssuedTokenType, rstr.TokenType, expectedTokenType)));
                    }
                    tokenType = rstr.TokenType;
                }
                else
                {
                    tokenType = expectedTokenType;
                }

                // search the response elements for licenseXml, proofXml, and lifetime
                DateTime created = rstr.ValidFrom;
                DateTime expires = rstr.ValidTo;
                XmlElement proofXml;
                XmlElement issuedTokenXml;
                GetIssuedAndProofXml(rstr, out issuedTokenXml, out proofXml);

                if (issuedTokenXml == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.NoLicenseXml));
                }

                if (isBearerKeyType)
                {
                    if (proofXml != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.BearerKeyTypeCannotHaveProofKey));
                    }

                    return new GenericXmlSecurityToken(issuedTokenXml, null, created, expires, rstr.RequestedAttachedReference, rstr.RequestedUnattachedReference, authorizationPolicies);
                }

                SecurityToken proofToken;
                SecurityToken entropyToken = GetEntropy(rstr, resolver);
                if (keyEntropyMode == SecurityKeyEntropyMode.ClientEntropy)
                {
                    if (requestorEntropy == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.EntropyModeRequiresRequestorEntropy, keyEntropyMode)));
                    }
                    // enforce that there is no entropy or proof token in the RSTR
                    if (proofXml != null || entropyToken != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.EntropyModeCannotHaveProofTokenOrIssuerEntropy, keyEntropyMode)));
                    }
                    proofToken = new BinarySecretSecurityToken(requestorEntropy);
                }
                else if (keyEntropyMode == SecurityKeyEntropyMode.ServerEntropy)
                {
                    if (requestorEntropy != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.EntropyModeCannotHaveRequestorEntropy, keyEntropyMode)));
                    }
                    if (rstr.ComputeKey || entropyToken != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.EntropyModeCannotHaveComputedKey, keyEntropyMode)));
                    }
                    if (proofXml == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.EntropyModeRequiresProofToken, keyEntropyMode)));
                    }
                    string valueTypeUri = proofXml.GetAttribute(SecurityJan2004Strings.ValueType);
                    if (valueTypeUri.Length == 0)
                    {
                        valueTypeUri = null;
                    }

                    proofToken = _standardsManager.SecurityTokenSerializer.ReadToken(new XmlNodeReader(proofXml), resolver);
                }
                else
                {
                    if (!rstr.ComputeKey)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.EntropyModeRequiresComputedKey, keyEntropyMode)));
                    }
                    if (entropyToken == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.EntropyModeRequiresIssuerEntropy, keyEntropyMode)));
                    }
                    if (requestorEntropy == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.EntropyModeRequiresRequestorEntropy, keyEntropyMode)));
                    }
                    if (rstr.KeySize == 0 && defaultKeySize == 0)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.RstrKeySizeNotProvided));
                    }
                    int issuedKeySize = (rstr.KeySize != 0) ? rstr.KeySize : defaultKeySize;
                    byte[] issuerEntropy;
                    if (entropyToken is BinarySecretSecurityToken)
                    {
                        issuerEntropy = ((BinarySecretSecurityToken)entropyToken).GetKeyBytes();
                    }
                    else
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.UnsupportedIssuerEntropyType));
                    }

                    // compute the PSHA1 derived key
                    byte[] issuedKey = RequestSecurityTokenResponse.ComputeCombinedKey(requestorEntropy, issuerEntropy, issuedKeySize);
                    proofToken = new BinarySecretSecurityToken(issuedKey);
                }

                SecurityKeyIdentifierClause internalReference = rstr.RequestedAttachedReference;
                SecurityKeyIdentifierClause externalReference = rstr.RequestedUnattachedReference;

                return new BufferedGenericXmlSecurityToken(issuedTokenXml, proofToken, created, expires, internalReference, externalReference, authorizationPolicies, rstr.IssuedTokenBuffer);
            }

            public override bool IsAtRequestSecurityTokenResponse(XmlReader reader)
            {
                if (reader == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(reader));
                }

                return reader.IsStartElement(DriverDictionary.RequestSecurityTokenResponse.Value, DriverDictionary.Namespace.Value);
            }

            public override bool IsAtRequestSecurityTokenResponseCollection(XmlReader reader)
            {
                if (reader == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(reader));
                }

                return reader.IsStartElement(DriverDictionary.RequestSecurityTokenResponseCollection.Value, DriverDictionary.Namespace.Value);
            }

            public override bool IsRequestedSecurityTokenElement(string name, string nameSpace)
            {
                return (name == DriverDictionary.RequestedSecurityToken.Value && nameSpace == DriverDictionary.Namespace.Value);
            }

            public override bool IsRequestedProofTokenElement(string name, string nameSpace)
            {
                return (name == DriverDictionary.RequestedProofToken.Value && nameSpace == DriverDictionary.Namespace.Value);
            }

            public static BinaryNegotiation ReadBinaryNegotiation(XmlElement elem)
            {
                if (elem == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(elem));
                }

                // get the encoding and valueType attributes
                string encodingUri = null;
                string valueTypeUri = null;
                byte[] negotiationData = null;
                if (elem.Attributes != null)
                {
                    for (int i = 0; i < elem.Attributes.Count; ++i)
                    {
                        XmlAttribute attr = elem.Attributes[i];
                        if (attr.LocalName == SecurityJan2004Strings.EncodingType && attr.NamespaceURI.Length == 0)
                        {
                            encodingUri = attr.Value;
                            if (encodingUri != s_base64Uri && encodingUri != s_hexBinaryUri)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SRP.Format(SRP.UnsupportedBinaryEncoding, encodingUri)));
                            }
                        }
                        else if (attr.LocalName == SecurityJan2004Strings.ValueType && attr.NamespaceURI.Length == 0)
                        {
                            valueTypeUri = attr.Value;
                        }
                        // ignore all other attributes
                    }
                }
                if (encodingUri == null)
                {
                    XmlHelper.OnRequiredAttributeMissing("EncodingType", elem.Name);
                }
                if (valueTypeUri == null)
                {
                    XmlHelper.OnRequiredAttributeMissing("ValueType", elem.Name);
                }
                string encodedBlob = XmlHelper.ReadTextElementAsTrimmedString(elem);
                if (encodingUri == s_base64Uri)
                {
                    negotiationData = Convert.FromBase64String(encodedBlob);
                }
                else
                {
                    negotiationData = HexBinary.Parse(encodedBlob).Value;
                }
                return new BinaryNegotiation(valueTypeUri, negotiationData);
            }

            // Note in Apr2004, internal & external references aren't supported - 
            // our strategy is to see if there's a token reference (and use it for external ref) and backup is to scan the token xml to compute reference
            protected virtual void ReadReferences(XmlElement rstrXml, out SecurityKeyIdentifierClause requestedAttachedReference,
                    out SecurityKeyIdentifierClause requestedUnattachedReference)
            {
                XmlElement issuedTokenXml = null;
                requestedAttachedReference = null;
                requestedUnattachedReference = null;
                for (int i = 0; i < rstrXml.ChildNodes.Count; ++i)
                {
                    XmlElement child = rstrXml.ChildNodes[i] as XmlElement;
                    if (child != null)
                    {
                        if (child.LocalName == DriverDictionary.RequestedSecurityToken.Value && child.NamespaceURI == DriverDictionary.Namespace.Value)
                        {
                            issuedTokenXml = XmlHelper.GetChildElement(child);
                        }
                        else if (child.LocalName == DriverDictionary.RequestedTokenReference.Value && child.NamespaceURI == DriverDictionary.Namespace.Value)
                        {
                            requestedUnattachedReference = GetKeyIdentifierXmlReferenceClause(XmlHelper.GetChildElement(child));
                        }
                    }
                }

                if (issuedTokenXml != null)
                {
                    requestedAttachedReference = _standardsManager.CreateKeyIdentifierClauseFromTokenXml(issuedTokenXml, SecurityTokenReferenceStyle.Internal);
                    if (requestedUnattachedReference == null)
                    {
                        try
                        {
                            requestedUnattachedReference = _standardsManager.CreateKeyIdentifierClauseFromTokenXml(issuedTokenXml, SecurityTokenReferenceStyle.External);
                        }
                        catch (XmlException)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SRP.Format(SRP.TrustDriverIsUnableToCreatedNecessaryAttachedOrUnattachedReferences, issuedTokenXml.ToString())));
                        }
                    }
                }
            }

            internal bool TryReadKeyIdentifierClause(XmlNodeReader reader, out SecurityKeyIdentifierClause keyIdentifierClause)
            {
                keyIdentifierClause = null;

                try
                {
                    keyIdentifierClause = _standardsManager.SecurityTokenSerializer.ReadKeyIdentifierClause(reader);
                }
                catch (XmlException e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    keyIdentifierClause = null;
                    return false;
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    keyIdentifierClause = null;
                    return false;
                }

                return true;
            }

            internal SecurityKeyIdentifierClause CreateGenericXmlSecurityKeyIdentifierClause(XmlNodeReader reader, XmlElement keyIdentifierReferenceXmlElement)
            {
                SecurityKeyIdentifierClause keyIdentifierClause = null;
                XmlDictionaryReader localReader = XmlDictionaryReader.CreateDictionaryReader(reader);
                string strId = localReader.GetAttribute(XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace);
                keyIdentifierClause = new GenericXmlSecurityKeyIdentifierClause(keyIdentifierReferenceXmlElement);
                if (!string.IsNullOrEmpty(strId))
                {
                    keyIdentifierClause.Id = strId;
                }

                return keyIdentifierClause;
            }

            internal SecurityKeyIdentifierClause GetKeyIdentifierXmlReferenceClause(XmlElement keyIdentifierReferenceXmlElement)
            {
                SecurityKeyIdentifierClause keyIdentifierClause = null;
                XmlNodeReader reader = new XmlNodeReader(keyIdentifierReferenceXmlElement);
                if (!TryReadKeyIdentifierClause(reader, out keyIdentifierClause))
                {
                    keyIdentifierClause = CreateGenericXmlSecurityKeyIdentifierClause(new XmlNodeReader(keyIdentifierReferenceXmlElement), keyIdentifierReferenceXmlElement);
                }

                return keyIdentifierClause;
            }

            protected virtual bool ReadRequestedTokenClosed(XmlElement rstrXml)
            {
                return false;
            }

            protected virtual void ReadTargets(XmlElement rstXml, out SecurityKeyIdentifierClause renewTarget, out SecurityKeyIdentifierClause closeTarget)
            {
                renewTarget = null;
                closeTarget = null;
            }

            private void WriteAppliesTo(object appliesTo, Type appliesToType, XmlObjectSerializer serializer, XmlWriter xmlWriter)
            {
                XmlDictionaryWriter writer = XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter);
                writer.WriteStartElement(Namespaces.WSPolicyPrefix, DriverDictionary.AppliesTo.Value, Namespaces.WSPolicy);
                lock (serializer)
                {
                    serializer.WriteObject(writer, appliesTo);
                }
                writer.WriteEndElement();
            }

            public void WriteBinaryNegotiation(BinaryNegotiation negotiation, XmlWriter xmlWriter)
            {
                if (negotiation == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(negotiation));
                }

                XmlDictionaryWriter writer = XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter);
                negotiation.WriteTo(writer, DriverDictionary.Prefix.Value,
                                            DriverDictionary.BinaryExchange, DriverDictionary.Namespace,
                                            XD.SecurityJan2004Dictionary.ValueType, null);
            }

            public override void WriteRequestSecurityToken(RequestSecurityToken rst, XmlWriter xmlWriter)
            {
                if (rst == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(rst));
                }
                if (xmlWriter == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(xmlWriter));
                }
                XmlDictionaryWriter writer = XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter);
                if (rst.IsReceiver)
                {
                    rst.WriteTo(writer);
                    return;
                }
                writer.WriteStartElement(DriverDictionary.Prefix.Value, DriverDictionary.RequestSecurityToken, DriverDictionary.Namespace);
                XmlHelper.AddNamespaceDeclaration(writer, DriverDictionary.Prefix.Value, DriverDictionary.Namespace);
                if (rst.Context != null)
                {
                    writer.WriteAttributeString(DriverDictionary.Context, null, rst.Context);
                }

                rst.OnWriteCustomAttributes(writer);
                if (rst.TokenType != null)
                {
                    writer.WriteStartElement(DriverDictionary.Prefix.Value, DriverDictionary.TokenType, DriverDictionary.Namespace);
                    writer.WriteString(rst.TokenType);
                    writer.WriteEndElement();
                }
                if (rst.RequestType != null)
                {
                    writer.WriteStartElement(DriverDictionary.Prefix.Value, DriverDictionary.RequestType, DriverDictionary.Namespace);
                    writer.WriteString(rst.RequestType);
                    writer.WriteEndElement();
                }

                if (rst.AppliesTo != null)
                {
                    WriteAppliesTo(rst.AppliesTo, rst.AppliesToType, rst.AppliesToSerializer, writer);
                }

                SecurityToken entropyToken = rst.GetRequestorEntropy();
                if (entropyToken != null)
                {
                    writer.WriteStartElement(DriverDictionary.Prefix.Value, DriverDictionary.Entropy, DriverDictionary.Namespace);
                    _standardsManager.SecurityTokenSerializer.WriteToken(writer, entropyToken);
                    writer.WriteEndElement();
                }

                if (rst.KeySize != 0)
                {
                    writer.WriteStartElement(DriverDictionary.Prefix.Value, DriverDictionary.KeySize, DriverDictionary.Namespace);
                    writer.WriteValue(rst.KeySize);
                    writer.WriteEndElement();
                }

                BinaryNegotiation negotiationData = rst.GetBinaryNegotiation();
                if (negotiationData != null)
                {
                    WriteBinaryNegotiation(negotiationData, writer);
                }

                WriteTargets(rst, writer);

                if (rst.RequestProperties != null)
                {
                    foreach (XmlElement property in rst.RequestProperties)
                    {
                        property.WriteTo(writer);
                    }
                }

                rst.OnWriteCustomElements(writer);
                writer.WriteEndElement();
            }

            protected virtual void WriteTargets(RequestSecurityToken rst, XmlDictionaryWriter writer)
            {
            }

            // Note in Apr2004, internal & external references aren't supported - our strategy is to generate the external ref as the TokenReference.
            protected virtual void WriteReferences(RequestSecurityTokenResponse rstr, XmlDictionaryWriter writer)
            {
                if (rstr.RequestedUnattachedReference != null)
                {
                    writer.WriteStartElement(DriverDictionary.Prefix.Value, DriverDictionary.RequestedTokenReference, DriverDictionary.Namespace);
                    _standardsManager.SecurityTokenSerializer.WriteKeyIdentifierClause(writer, rstr.RequestedUnattachedReference);
                    writer.WriteEndElement();
                }
            }

            protected virtual void WriteRequestedTokenClosed(RequestSecurityTokenResponse rstr, XmlDictionaryWriter writer)
            {
            }

            public override void WriteRequestSecurityTokenResponse(RequestSecurityTokenResponse rstr, XmlWriter xmlWriter)
            {
                if (rstr == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(rstr));
                }

                if (xmlWriter == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(xmlWriter));
                }

                XmlDictionaryWriter writer = XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter);
                if (rstr.IsReceiver)
                {
                    rstr.WriteTo(writer);
                    return;
                }
                writer.WriteStartElement(DriverDictionary.Prefix.Value, DriverDictionary.RequestSecurityTokenResponse, DriverDictionary.Namespace);
                if (rstr.Context != null)
                {
                    writer.WriteAttributeString(DriverDictionary.Context, null, rstr.Context);
                }
                // define WSUtility at the top level to avoid multiple definitions below
                XmlHelper.AddNamespaceDeclaration(writer, UtilityStrings.Prefix, XD.UtilityDictionary.Namespace);
                rstr.OnWriteCustomAttributes(writer);

                if (rstr.TokenType != null)
                {
                    writer.WriteElementString(DriverDictionary.Prefix.Value, DriverDictionary.TokenType, DriverDictionary.Namespace, rstr.TokenType);
                }

                if (rstr.RequestedSecurityToken != null)
                {
                    writer.WriteStartElement(DriverDictionary.Prefix.Value, DriverDictionary.RequestedSecurityToken, DriverDictionary.Namespace);
                    _standardsManager.SecurityTokenSerializer.WriteToken(writer, rstr.RequestedSecurityToken);
                    writer.WriteEndElement();
                }

                if (rstr.AppliesTo != null)
                {
                    WriteAppliesTo(rstr.AppliesTo, rstr.AppliesToType, rstr.AppliesToSerializer, writer);
                }

                WriteReferences(rstr, writer);

                if (rstr.ComputeKey || rstr.RequestedProofToken != null)
                {
                    writer.WriteStartElement(DriverDictionary.Prefix.Value, DriverDictionary.RequestedProofToken, DriverDictionary.Namespace);
                    if (rstr.ComputeKey)
                    {
                        writer.WriteElementString(DriverDictionary.Prefix.Value, DriverDictionary.ComputedKey, DriverDictionary.Namespace, DriverDictionary.Psha1ComputedKeyUri.Value);
                    }
                    else
                    {
                        _standardsManager.SecurityTokenSerializer.WriteToken(writer, rstr.RequestedProofToken);
                    }
                    writer.WriteEndElement();
                }

                SecurityToken entropyToken = rstr.GetIssuerEntropy();
                if (entropyToken != null)
                {
                    writer.WriteStartElement(DriverDictionary.Prefix.Value, DriverDictionary.Entropy, DriverDictionary.Namespace);
                    _standardsManager.SecurityTokenSerializer.WriteToken(writer, entropyToken);
                    writer.WriteEndElement();
                }

                // To write out the lifetime, the following algorithm is used
                //   1. If the lifetime is explicitly set, write it out.
                //   2. Else, if a token/tokenbuilder has been set, use the lifetime in that.
                //   3. Else do not serialize lifetime
                if (rstr.IsLifetimeSet || rstr.RequestedSecurityToken != null)
                {
                    DateTime effectiveTime = SecurityUtils.MinUtcDateTime;
                    DateTime expirationTime = SecurityUtils.MaxUtcDateTime;

                    if (rstr.IsLifetimeSet)
                    {
                        effectiveTime = rstr.ValidFrom.ToUniversalTime();
                        expirationTime = rstr.ValidTo.ToUniversalTime();
                    }
                    else if (rstr.RequestedSecurityToken != null)
                    {
                        effectiveTime = rstr.RequestedSecurityToken.ValidFrom.ToUniversalTime();
                        expirationTime = rstr.RequestedSecurityToken.ValidTo.ToUniversalTime();
                    }

                    // write out the lifetime
                    writer.WriteStartElement(DriverDictionary.Prefix.Value, DriverDictionary.Lifetime, DriverDictionary.Namespace);
                    // write out Created
                    writer.WriteStartElement(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.CreatedElement, XD.UtilityDictionary.Namespace);
                    writer.WriteString(effectiveTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture.DateTimeFormat));
                    writer.WriteEndElement(); // wsu:Created
                    // write out Expires
                    writer.WriteStartElement(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.ExpiresElement, XD.UtilityDictionary.Namespace);
                    writer.WriteString(expirationTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture.DateTimeFormat));
                    writer.WriteEndElement(); // wsu:Expires
                    writer.WriteEndElement(); // wsse:Lifetime
                }

                byte[] authenticator = rstr.GetAuthenticator();
                if (authenticator != null)
                {
                    writer.WriteStartElement(DriverDictionary.Prefix.Value, DriverDictionary.Authenticator, DriverDictionary.Namespace);
                    writer.WriteStartElement(DriverDictionary.Prefix.Value, DriverDictionary.CombinedHash, DriverDictionary.Namespace);
                    writer.WriteBase64(authenticator, 0, authenticator.Length);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }

                if (rstr.KeySize > 0)
                {
                    writer.WriteStartElement(DriverDictionary.Prefix.Value, DriverDictionary.KeySize, DriverDictionary.Namespace);
                    writer.WriteValue(rstr.KeySize);
                    writer.WriteEndElement();
                }

                WriteRequestedTokenClosed(rstr, writer);

                BinaryNegotiation negotiationData = rstr.GetBinaryNegotiation();
                if (negotiationData != null)
                {
                    WriteBinaryNegotiation(negotiationData, writer);
                }

                rstr.OnWriteCustomElements(writer);
                writer.WriteEndElement();
            }

            public override void WriteRequestSecurityTokenResponseCollection(RequestSecurityTokenResponseCollection rstrCollection, XmlWriter xmlWriter)
            {
                if (rstrCollection == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(rstrCollection));
                }

                XmlDictionaryWriter writer = XmlDictionaryWriter.CreateDictionaryWriter(xmlWriter);
                writer.WriteStartElement(DriverDictionary.Prefix.Value, DriverDictionary.RequestSecurityTokenResponseCollection, DriverDictionary.Namespace);
                foreach (RequestSecurityTokenResponse rstr in rstrCollection.RstrCollection)
                {
                    rstr.WriteTo(writer);
                }
                writer.WriteEndElement();
            }

            protected void SetProtectionLevelForFederation(OperationDescriptionCollection operations)
            {
                foreach (OperationDescription operation in operations)
                {
                    foreach (MessageDescription message in operation.Messages)
                    {
                        if (message.Body.Parts.Count > 0)
                        {
                            foreach (MessagePartDescription part in message.Body.Parts)
                            {
                                part.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                            }
                        }
                        if (OperationFormatter.IsValidReturnValue(message.Body.ReturnValue))
                        {
                            message.Body.ReturnValue.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                        }
                    }
                }
            }

            public bool TryParseSymmetricKeyElement(XmlElement element)
            {
                if (element == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(element));
                }

                return element.LocalName == DriverDictionary.KeyType.Value
                    && element.NamespaceURI == DriverDictionary.Namespace.Value
                    && element.InnerText == DriverDictionary.SymmetricKeyType.Value;
            }

            private XmlElement CreateSymmetricKeyTypeElement()
            {
                XmlDocument doc = new XmlDocument();
                XmlElement result = doc.CreateElement(DriverDictionary.Prefix.Value, DriverDictionary.KeyType.Value,
                    DriverDictionary.Namespace.Value);
                result.AppendChild(doc.CreateTextNode(DriverDictionary.SymmetricKeyType.Value));
                return result;
            }

            private bool TryParsePublicKeyElement(XmlElement element)
            {
                if (element == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(element));
                }

                return element.LocalName == DriverDictionary.KeyType.Value
                    && element.NamespaceURI == DriverDictionary.Namespace.Value
                    && element.InnerText == DriverDictionary.PublicKeyType.Value;
            }

            private XmlElement CreatePublicKeyTypeElement()
            {
                XmlDocument doc = new XmlDocument();
                XmlElement result = doc.CreateElement(DriverDictionary.Prefix.Value, DriverDictionary.KeyType.Value,
                    DriverDictionary.Namespace.Value);
                result.AppendChild(doc.CreateTextNode(DriverDictionary.PublicKeyType.Value));
                return result;
            }

            internal static void ValidateRequestedKeySize(int keySize, SecurityAlgorithmSuite algorithmSuite)
            {
                if ((keySize % 8 == 0) && algorithmSuite.IsSymmetricKeyLengthSupported(keySize))
                {
                    return;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new SecurityNegotiationException(SRP.Format(SRP.InvalidKeyLengthRequested, keySize)));
                }
            }

            private static void ValidateRequestorEntropy(SecurityToken entropy, SecurityKeyEntropyMode mode)
            {
                if ((mode == SecurityKeyEntropyMode.ClientEntropy || mode == SecurityKeyEntropyMode.CombinedEntropy)
                    && (entropy == null))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SRP.Format(SRP.EntropyModeRequiresRequestorEntropy, mode)));
                }
                if (mode == SecurityKeyEntropyMode.ServerEntropy && entropy != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SRP.Format(SRP.EntropyModeCannotHaveRequestorEntropy, mode)));
                }
            }
        }
    }
}
