// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Dispatcher;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using Microsoft.Xml;
    using System.Runtime;
    using System.Security.Cryptography;
    using System.IdentityModel.Claims;
    using System.IdentityModel.Policy;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel.Security.Tokens;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.Runtime.Serialization;

    using KeyIdentifierEntry = WSSecurityTokenSerializer.KeyIdentifierEntry;
    using KeyIdentifierClauseEntry = WSSecurityTokenSerializer.KeyIdentifierClauseEntry;
    using TokenEntry = WSSecurityTokenSerializer.TokenEntry;
    using StrEntry = WSSecurityTokenSerializer.StrEntry;

    internal abstract class WSTrust : WSSecurityTokenSerializer.SerializerEntries
    {
        private WSSecurityTokenSerializer _tokenSerializer;

        public WSTrust(WSSecurityTokenSerializer tokenSerializer)
        {
            _tokenSerializer = tokenSerializer;
        }

        public WSSecurityTokenSerializer WSSecurityTokenSerializer
        {
            get { return _tokenSerializer; }
        }

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
                    _otherDictionary = _parent.SerializerDictionary;
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

                return element.LocalName == LocalName.Value && (element.NamespaceURI == NamespaceUri.Value || element.NamespaceURI == _otherDictionary.Namespace.Value) && valueTypeUri == this.ValueTypeUri;
            }

            public override bool CanReadTokenCore(XmlDictionaryReader reader)
            {
                return (reader.IsStartElement(this.LocalName, this.NamespaceUri) || reader.IsStartElement(this.LocalName, _otherDictionary.Namespace)) &&
                       reader.GetAttribute(XD.SecurityJan2004Dictionary.ValueType, null) == this.ValueTypeUri;
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
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("tokenReferenceStyle"));
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
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(string.Format(SRServiceModel.UnexpectedBinarySecretType, _parent.SerializerDictionary.SymmetricKeyBinarySecret.Value, secretType)));
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }

            public override RequestSecurityTokenResponse CreateRequestSecurityTokenResponse(XmlReader xmlReader)
            {
                throw new NotImplementedException();
            }

            public override RequestSecurityTokenResponseCollection CreateRequestSecurityTokenResponseCollection(XmlReader xmlReader)
            {
                XmlDictionaryReader reader = XmlDictionaryReader.CreateDictionaryReader(xmlReader);
                List<RequestSecurityTokenResponse> rstrCollection = new List<RequestSecurityTokenResponse>(2);
                string rootName = reader.Name;
                reader.ReadStartElement(DriverDictionary.RequestSecurityTokenResponseCollection, DriverDictionary.Namespace);
                while (reader.IsStartElement(DriverDictionary.RequestSecurityTokenResponse.Value, DriverDictionary.Namespace.Value))
                {
                    RequestSecurityTokenResponse rstr = this.CreateRequestSecurityTokenResponse(reader);
                    rstrCollection.Add(rstr);
                }
                reader.ReadEndElement();
                if (rstrCollection.Count == 0)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SRServiceModel.NoRequestSecurityTokenResponseElements));
                return new RequestSecurityTokenResponseCollection(rstrCollection.AsReadOnly(), this.StandardsManager);
            }

            private XmlElement GetAppliesToElement(XmlElement rootElement)
            {
                if (rootElement == null)
                {
                    return null;
                }
                for (int i = 0; i < rootElement.ChildNodes.Count; ++i)
                {
                    XmlElement elem = (rootElement.ChildNodes[i] as XmlElement);
                    if (elem != null)
                    {
                        if (elem.LocalName == DriverDictionary.AppliesTo.Value && elem.NamespaceURI == Namespaces.WSPolicy)
                        {
                            return elem;
                        }
                    }
                }
                return null;
            }

            private T GetAppliesTo<T>(XmlElement rootXml, XmlObjectSerializer serializer)
            {
                XmlElement appliesToElement = GetAppliesToElement(rootXml);
                if (appliesToElement != null)
                {
                    using (XmlReader reader = new XmlNodeReader(appliesToElement))
                    {
                        reader.ReadStartElement();
                        lock (serializer)
                        {
                            return (T)serializer.ReadObject(reader);
                        }
                    }
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.NoAppliesToPresent));
                }
            }

            public override T GetAppliesTo<T>(RequestSecurityToken rst, XmlObjectSerializer serializer)
            {
                if (rst == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rst");

                return GetAppliesTo<T>(rst.RequestSecurityTokenXml, serializer);
            }

            public override T GetAppliesTo<T>(RequestSecurityTokenResponse rstr, XmlObjectSerializer serializer)
            {
                if (rstr == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rstr");

                return GetAppliesTo<T>(rstr.RequestSecurityTokenResponseXml, serializer);
            }

            public override bool IsAppliesTo(string localName, string namespaceUri)
            {
                return (localName == DriverDictionary.AppliesTo.Value && namespaceUri == Namespaces.WSPolicy);
            }

            private void GetAppliesToQName(XmlElement rootElement, out string localName, out string namespaceUri)
            {
                localName = namespaceUri = null;
                XmlElement appliesToElement = GetAppliesToElement(rootElement);
                if (appliesToElement != null)
                {
                    using (XmlReader reader = new XmlNodeReader(appliesToElement))
                    {
                        reader.ReadStartElement();
                        reader.MoveToContent();
                        localName = reader.LocalName;
                        namespaceUri = reader.NamespaceURI;
                    }
                }
            }

            public override void GetAppliesToQName(RequestSecurityToken rst, out string localName, out string namespaceUri)
            {
                if (rst == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rst");

                GetAppliesToQName(rst.RequestSecurityTokenXml, out localName, out namespaceUri);
            }

            public override void GetAppliesToQName(RequestSecurityTokenResponse rstr, out string localName, out string namespaceUri)
            {
                if (rstr == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rstr");

                GetAppliesToQName(rstr.RequestSecurityTokenResponseXml, out localName, out namespaceUri);
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rstr");

                return GetBinaryNegotiation(rstr.RequestSecurityTokenResponseXml);
            }

            public override BinaryNegotiation GetBinaryNegotiation(RequestSecurityToken rst)
            {
                if (rst == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rst");

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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rst");

                return GetEntropy(rst.RequestSecurityTokenXml, resolver);
            }

            public override SecurityToken GetEntropy(RequestSecurityTokenResponse rstr, SecurityTokenResolver resolver)
            {
                if (rstr == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rstr");

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
                                valueTypeUri = null;
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
                                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.RstrHasMultipleIssuedTokens));
                                }
                                issuedTokenXml = XmlHelper.GetChildElement(elem);
                            }
                            else if (elem.LocalName == DriverDictionary.RequestedProofToken.Value && elem.NamespaceURI == DriverDictionary.Namespace.Value)
                            {
                                if (proofTokenXml != null)
                                {
                                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.RstrHasMultipleProofTokens));
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
                throw new NotImplementedException();
            }

            public override GenericXmlSecurityToken GetIssuedToken(RequestSecurityTokenResponse rstr, string expectedTokenType,
                ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies, RSA clientKey)
            {
                throw new NotImplementedException();
            }

            public override bool IsAtRequestSecurityTokenResponse(XmlReader reader)
            {
                if (reader == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");

                return reader.IsStartElement(DriverDictionary.RequestSecurityTokenResponse.Value, DriverDictionary.Namespace.Value);
            }

            public override bool IsAtRequestSecurityTokenResponseCollection(XmlReader reader)
            {
                if (reader == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");

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
                throw new NotImplementedException();
            }

            // Note in Apr2004, internal & external references aren't supported - 
            // our strategy is to see if there's a token reference (and use it for external ref) and backup is to scan the token xml to compute reference
            protected virtual void ReadReferences(XmlElement rstrXml, out SecurityKeyIdentifierClause requestedAttachedReference,
                    out SecurityKeyIdentifierClause requestedUnattachedReference)
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }

            internal SecurityKeyIdentifierClause GetKeyIdentifierXmlReferenceClause(XmlElement keyIdentifierReferenceXmlElement)
            {
                SecurityKeyIdentifierClause keyIdentifierClause = null;
                XmlNodeReader reader = new XmlNodeReader(keyIdentifierReferenceXmlElement);
                if (!this.TryReadKeyIdentifierClause(reader, out keyIdentifierClause))
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

            public override void OnRSTRorRSTRCMissingException()
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(string.Format(SRServiceModel.ExpectedOneOfTwoElementsFromNamespace,
                    DriverDictionary.RequestSecurityTokenResponse, DriverDictionary.RequestSecurityTokenResponseCollection,
                    DriverDictionary.Namespace)));
            }

            public override void WriteRequestSecurityToken(RequestSecurityToken rst, XmlWriter xmlWriter)
            {
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }

            public override void WriteRequestSecurityTokenResponseCollection(RequestSecurityTokenResponseCollection rstrCollection, XmlWriter xmlWriter)
            {
                if (rstrCollection == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rstrCollection");

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

            public override bool TryParseKeySizeElement(XmlElement element, out int keySize)
            {
                if (element == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");

                if (element.LocalName == this.DriverDictionary.KeySize.Value
                    && element.NamespaceURI == this.DriverDictionary.Namespace.Value)
                {
                    keySize = Int32.Parse(XmlHelper.ReadTextElementAsTrimmedString(element), NumberFormatInfo.InvariantInfo);
                    return true;
                }

                keySize = 0;
                return false;
            }

            public override XmlElement CreateKeySizeElement(int keySize)
            {
                if (keySize < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("keySize", SRServiceModel.ValueMustBeNonNegative));
                }
                XmlDocument doc = new XmlDocument();
                XmlElement result = doc.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.KeySize.Value,
                    this.DriverDictionary.Namespace.Value);
                result.AppendChild(doc.CreateTextNode(keySize.ToString(System.Globalization.CultureInfo.InvariantCulture.NumberFormat)));
                return result;
            }

            public override XmlElement CreateKeyTypeElement(SecurityKeyType keyType)
            {
                if (keyType == SecurityKeyType.SymmetricKey)
                    return CreateSymmetricKeyTypeElement();
                else if (keyType == SecurityKeyType.AsymmetricKey)
                    return CreatePublicKeyTypeElement();
                else
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.UnableToCreateKeyTypeElementForUnknownKeyType, keyType.ToString())));
            }

            public override bool TryParseKeyTypeElement(XmlElement element, out SecurityKeyType keyType)
            {
                if (element == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");

                if (TryParseSymmetricKeyElement(element))
                {
                    keyType = SecurityKeyType.SymmetricKey;
                    return true;
                }
                else if (TryParsePublicKeyElement(element))
                {
                    keyType = SecurityKeyType.AsymmetricKey;
                    return true;
                }

                keyType = SecurityKeyType.SymmetricKey;
                return false;
            }

            public bool TryParseSymmetricKeyElement(XmlElement element)
            {
                if (element == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");

                return element.LocalName == this.DriverDictionary.KeyType.Value
                    && element.NamespaceURI == this.DriverDictionary.Namespace.Value
                    && element.InnerText == this.DriverDictionary.SymmetricKeyType.Value;
            }

            private XmlElement CreateSymmetricKeyTypeElement()
            {
                XmlDocument doc = new XmlDocument();
                XmlElement result = doc.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.KeyType.Value,
                    this.DriverDictionary.Namespace.Value);
                result.AppendChild(doc.CreateTextNode(this.DriverDictionary.SymmetricKeyType.Value));
                return result;
            }

            private bool TryParsePublicKeyElement(XmlElement element)
            {
                if (element == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");

                return element.LocalName == this.DriverDictionary.KeyType.Value
                    && element.NamespaceURI == this.DriverDictionary.Namespace.Value
                    && element.InnerText == this.DriverDictionary.PublicKeyType.Value;
            }

            private XmlElement CreatePublicKeyTypeElement()
            {
                XmlDocument doc = new XmlDocument();
                XmlElement result = doc.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.KeyType.Value,
                    this.DriverDictionary.Namespace.Value);
                result.AppendChild(doc.CreateTextNode(this.DriverDictionary.PublicKeyType.Value));
                return result;
            }

            public override bool TryParseTokenTypeElement(XmlElement element, out string tokenType)
            {
                if (element == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");

                if (element.LocalName == this.DriverDictionary.TokenType.Value
                    && element.NamespaceURI == this.DriverDictionary.Namespace.Value)
                {
                    tokenType = element.InnerText;
                    return true;
                }

                tokenType = null;
                return false;
            }

            public override XmlElement CreateTokenTypeElement(string tokenTypeUri)
            {
                if (tokenTypeUri == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenTypeUri");
                }
                XmlDocument doc = new XmlDocument();
                XmlElement result = doc.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.TokenType.Value,
                    this.DriverDictionary.Namespace.Value);
                result.AppendChild(doc.CreateTextNode(tokenTypeUri));
                return result;
            }

            public override XmlElement CreateUseKeyElement(SecurityKeyIdentifier keyIdentifier, SecurityStandardsManager standardsManager)
            {
                if (keyIdentifier == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifier");
                }
                if (standardsManager == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("standardsManager");
                }
                XmlDocument doc = new XmlDocument();
                XmlElement result = doc.CreateElement(this.DriverDictionary.UseKey.Value, this.DriverDictionary.Namespace.Value);
                MemoryStream stream = new MemoryStream();
                using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateDictionaryWriter(new XmlTextWriter(stream, Encoding.UTF8)))
                {
#pragma warning disable 56506 // standardsManager.SecurityTokenSerializer can never be null.
                    standardsManager.SecurityTokenSerializer.WriteKeyIdentifier(writer, keyIdentifier);
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);
                    XmlNode skiNode;
                    using (XmlDictionaryReader reader = XmlDictionaryReader.CreateDictionaryReader(new XmlTextReader(stream)))
                    {
                        reader.MoveToContent();
                        skiNode = doc.ReadNode(reader);
                    }
                    result.AppendChild(skiNode);
                }
                return result;
            }

            public override XmlElement CreateSignWithElement(string signatureAlgorithm)
            {
                if (signatureAlgorithm == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("signatureAlgorithm");
                }
                XmlDocument doc = new XmlDocument();
                XmlElement result = doc.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.SignWith.Value,
                    this.DriverDictionary.Namespace.Value);
                result.AppendChild(doc.CreateTextNode(signatureAlgorithm));
                return result;
            }

            internal override bool IsSignWithElement(XmlElement element, out string signatureAlgorithm)
            {
                return CheckElement(element, this.DriverDictionary.SignWith.Value, this.DriverDictionary.Namespace.Value, out signatureAlgorithm);
            }

            public override XmlElement CreateEncryptWithElement(string encryptionAlgorithm)
            {
                if (encryptionAlgorithm == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("encryptionAlgorithm");
                }
                XmlDocument doc = new XmlDocument();
                XmlElement result = doc.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.EncryptWith.Value,
                    this.DriverDictionary.Namespace.Value);
                result.AppendChild(doc.CreateTextNode(encryptionAlgorithm));
                return result;
            }

            public override XmlElement CreateEncryptionAlgorithmElement(string encryptionAlgorithm)
            {
                if (encryptionAlgorithm == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("encryptionAlgorithm");
                }
                XmlDocument doc = new XmlDocument();
                XmlElement result = doc.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.EncryptionAlgorithm.Value,
                    this.DriverDictionary.Namespace.Value);
                result.AppendChild(doc.CreateTextNode(encryptionAlgorithm));
                return result;
            }

            internal override bool IsEncryptWithElement(XmlElement element, out string encryptWithAlgorithm)
            {
                return CheckElement(element, this.DriverDictionary.EncryptWith.Value, this.DriverDictionary.Namespace.Value, out encryptWithAlgorithm);
            }

            internal override bool IsEncryptionAlgorithmElement(XmlElement element, out string encryptionAlgorithm)
            {
                return CheckElement(element, this.DriverDictionary.EncryptionAlgorithm.Value, this.DriverDictionary.Namespace.Value, out encryptionAlgorithm);
            }

            public override XmlElement CreateComputedKeyAlgorithmElement(string algorithm)
            {
                if (algorithm == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("algorithm");
                }
                XmlDocument doc = new XmlDocument();
                XmlElement result = doc.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.ComputedKeyAlgorithm.Value,
                    this.DriverDictionary.Namespace.Value);
                result.AppendChild(doc.CreateTextNode(algorithm));
                return result;
            }

            public override XmlElement CreateCanonicalizationAlgorithmElement(string algorithm)
            {
                if (algorithm == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("algorithm");
                }
                XmlDocument doc = new XmlDocument();
                XmlElement result = doc.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.CanonicalizationAlgorithm.Value,
                    this.DriverDictionary.Namespace.Value);
                result.AppendChild(doc.CreateTextNode(algorithm));
                return result;
            }

            internal override bool IsCanonicalizationAlgorithmElement(XmlElement element, out string canonicalizationAlgorithm)
            {
                return CheckElement(element, this.DriverDictionary.CanonicalizationAlgorithm.Value, this.DriverDictionary.Namespace.Value, out canonicalizationAlgorithm);
            }

            public override bool TryParseRequiredClaimsElement(XmlElement element, out System.Collections.ObjectModel.Collection<XmlElement> requiredClaims)
            {
                if (element == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");

                if (element.LocalName == this.DriverDictionary.Claims.Value
                    && element.NamespaceURI == this.DriverDictionary.Namespace.Value)
                {
                    requiredClaims = new System.Collections.ObjectModel.Collection<XmlElement>();
                    foreach (XmlNode node in element.ChildNodes)
                        if (node is XmlElement)
                        {
                            // PreSharp Bug: Parameter 'requiredClaims' to this public method must be validated: A null-dereference can occur here.
#pragma warning disable 56506
                            requiredClaims.Add((XmlElement)node);
                        }
                    return true;
                }

                requiredClaims = null;
                return false;
            }

            public override XmlElement CreateRequiredClaimsElement(IEnumerable<XmlElement> claimsList)
            {
                if (claimsList == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("claimsList");
                }
                XmlDocument doc = new XmlDocument();
                XmlElement result = doc.CreateElement(this.DriverDictionary.Prefix.Value, this.DriverDictionary.Claims.Value,
                    this.DriverDictionary.Namespace.Value);
                foreach (XmlElement claimElement in claimsList)
                {
                    XmlElement element = (XmlElement)doc.ImportNode(claimElement, true);
                    result.AppendChild(element);
                }
                return result;
            }
        }

        protected static bool CheckElement(XmlElement element, string name, string ns, out string value)
        {
            value = null;
            if (element.LocalName != name || element.NamespaceURI != ns)
                return false;
            if (element.FirstChild is XmlText)
            {
                value = ((XmlText)element.FirstChild).Value;
                return true;
            }
            return false;
        }
    }
}
