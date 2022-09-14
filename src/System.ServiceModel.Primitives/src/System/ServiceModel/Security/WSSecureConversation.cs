// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel.Security.Tokens;
using System.Xml;
using TokenEntry = System.ServiceModel.Security.WSSecurityTokenSerializer.TokenEntry;

namespace System.ServiceModel.Security
{
    internal abstract class WSSecureConversation : WSSecurityTokenSerializer.SerializerEntries
    {
        private DerivedKeyTokenEntry _derivedKeyEntry;

        protected WSSecureConversation(WSSecurityTokenSerializer tokenSerializer, int maxKeyDerivationOffset, int maxKeyDerivationLabelLength, int maxKeyDerivationNonceLength)
        {
            WSSecurityTokenSerializer = tokenSerializer ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenSerializer));
            _derivedKeyEntry = new DerivedKeyTokenEntry(this, maxKeyDerivationOffset, maxKeyDerivationLabelLength, maxKeyDerivationNonceLength);
        }

        public abstract SecureConversationDictionary SerializerDictionary
        {
            get;
        }

        public WSSecurityTokenSerializer WSSecurityTokenSerializer { get; }

        public override void PopulateTokenEntries(IList<TokenEntry> tokenEntryList)
        {
            if (tokenEntryList == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenEntryList));
            }
            tokenEntryList.Add(_derivedKeyEntry);
        }

        public virtual bool IsAtDerivedKeyToken(XmlDictionaryReader reader)
        {
            return _derivedKeyEntry.CanReadTokenCore(reader);
        }

        public virtual void ReadDerivedKeyTokenParameters(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver, out string id, out string derivationAlgorithm, out string label, out int length, out byte[] nonce, out int offset, out int generation, out SecurityKeyIdentifierClause tokenToDeriveIdentifier, out SecurityToken tokenToDerive)
        {
            _derivedKeyEntry.ReadDerivedKeyTokenParameters(reader, tokenResolver, out id, out derivationAlgorithm, out label,
                out length, out nonce, out offset, out generation, out tokenToDeriveIdentifier, out tokenToDerive);
        }

        public virtual SecurityToken CreateDerivedKeyToken(string id, string derivationAlgorithm, string label, int length, byte[] nonce, int offset, int generation, SecurityKeyIdentifierClause tokenToDeriveIdentifier, SecurityToken tokenToDerive)
        {
            return _derivedKeyEntry.CreateDerivedKeyToken(id, derivationAlgorithm, label, length, nonce, offset, generation,
                tokenToDeriveIdentifier, tokenToDerive);
        }

        public virtual string DerivationAlgorithm
        {
            get { return SecurityAlgorithms.Psha1KeyDerivation; }
        }

        protected class DerivedKeyTokenEntry : TokenEntry
        {
            public const string DefaultLabel = "WS-SecureConversation";

            private WSSecureConversation _parent;
            private int _maxKeyDerivationOffset;
            private int _maxKeyDerivationLabelLength;
            private int _maxKeyDerivationNonceLength;

            public DerivedKeyTokenEntry(WSSecureConversation parent, int maxKeyDerivationOffset, int maxKeyDerivationLabelLength, int maxKeyDerivationNonceLength)
            {
                _parent = parent ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(parent));
                _maxKeyDerivationOffset = maxKeyDerivationOffset;
                _maxKeyDerivationLabelLength = maxKeyDerivationLabelLength;
                _maxKeyDerivationNonceLength = maxKeyDerivationNonceLength;
            }

            protected override XmlDictionaryString LocalName { get { return _parent.SerializerDictionary.DerivedKeyToken; } }
            protected override XmlDictionaryString NamespaceUri { get { return _parent.SerializerDictionary.Namespace; } }
            protected override Type[] GetTokenTypesCore() { return new Type[] { typeof(DerivedKeySecurityToken) }; }
            public override string TokenTypeUri { get { return _parent.SerializerDictionary.DerivedKeyTokenType.Value; } }
            protected override string ValueTypeUri { get { return null; } }

            public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXmlCore(XmlElement issuedTokenXml,
                SecurityTokenReferenceStyle tokenReferenceStyle)
            {
                TokenReferenceStyleHelper.Validate(tokenReferenceStyle);

                switch (tokenReferenceStyle)
                {
                    case SecurityTokenReferenceStyle.Internal:
                        return CreateDirectReference(issuedTokenXml, UtilityStrings.IdAttribute, UtilityStrings.Namespace, typeof(DerivedKeySecurityToken));
                    case SecurityTokenReferenceStyle.External:
                        // DerivedKeys aren't referred to externally
                        return null;
                    default:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(tokenReferenceStyle)));
                }
            }

            // xml format
            //<DerivedKeyToken wsu:Id="..." wsse:Algorithm="..."> id required, alg optional (curr disallowed)
            //  <SecurityTokenReference>...</SecurityTokenReference> - required
            //  <Properties>...</Properties> - disallowed (optional in spec, but we disallow it)
            // choice begin - (schema requires a choice - we allow neither on read - we always write one)
            //  <Generation>...</Generation> - optional
            //  <Offset>...</Offset> - optional
            // choice end
            //  <Length>...</Length> - optional - default 32 on read (default specified in spec, not in schema - we always write it)
            //  <Label>...</Label> - optional
            //  <Nonce>...</Nonce> - required (optional in spec, but we require it)
            //</DerivedKeyToken>
            public virtual void ReadDerivedKeyTokenParameters(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver, out string id, out string derivationAlgorithm, out string label, out int length, out byte[] nonce, out int offset, out int generation, out SecurityKeyIdentifierClause tokenToDeriveIdentifier, out SecurityToken tokenToDerive)
            {
                if (tokenResolver == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenResolver));
                }

                id = reader.GetAttribute(XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace);

                derivationAlgorithm = reader.GetAttribute(XD.XmlSignatureDictionary.Algorithm, null);
                if (derivationAlgorithm == null)
                {
                    derivationAlgorithm = _parent.DerivationAlgorithm;
                }

                reader.ReadStartElement();

                tokenToDeriveIdentifier = null;
                tokenToDerive = null;

                if (reader.IsStartElement(XD.SecurityJan2004Dictionary.SecurityTokenReference, XD.SecurityJan2004Dictionary.Namespace))
                {
                    tokenToDeriveIdentifier = _parent.WSSecurityTokenSerializer.ReadKeyIdentifierClause(reader);
                    tokenResolver.TryResolveToken(tokenToDeriveIdentifier, out tokenToDerive);
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SRP.DerivedKeyTokenRequiresTokenReference));
                }

                // no support for properties

                generation = -1;
                if (reader.IsStartElement(_parent.SerializerDictionary.Generation, _parent.SerializerDictionary.Namespace))
                {
                    reader.ReadStartElement();
                    generation = reader.ReadContentAsInt();
                    reader.ReadEndElement();
                    if (generation < 0)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SRP.Format(SRP.DerivedKeyInvalidGenerationSpecified, generation)));
                    }
                }

                offset = -1;
                if (reader.IsStartElement(_parent.SerializerDictionary.Offset, _parent.SerializerDictionary.Namespace))
                {
                    reader.ReadStartElement();
                    offset = reader.ReadContentAsInt();
                    reader.ReadEndElement();
                    if (offset < 0)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SRP.Format(SRP.DerivedKeyInvalidOffsetSpecified, offset)));
                    }
                }

                length = DerivedKeySecurityToken.DefaultDerivedKeyLength;
                if (reader.IsStartElement(_parent.SerializerDictionary.Length, _parent.SerializerDictionary.Namespace))
                {
                    reader.ReadStartElement();
                    length = reader.ReadContentAsInt();
                    reader.ReadEndElement();
                }

                if ((offset == -1) && (generation == -1))
                {
                    offset = 0;
                }

                // verify that the offset is not larger than the max allowed
                DerivedKeySecurityToken.EnsureAcceptableOffset(offset, generation, length, _maxKeyDerivationOffset);

                label = null;
                if (reader.IsStartElement(_parent.SerializerDictionary.Label, _parent.SerializerDictionary.Namespace))
                {
                    reader.ReadStartElement();
                    label = reader.ReadString();
                    reader.ReadEndElement();
                }
                if (label != null && label.Length > _maxKeyDerivationLabelLength)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SRP.Format(SRP.DerivedKeyTokenLabelTooLong, label.Length, _maxKeyDerivationLabelLength)));
                }

                nonce = null;
                reader.ReadStartElement(_parent.SerializerDictionary.Nonce, _parent.SerializerDictionary.Namespace);
                nonce = reader.ReadContentAsBase64();
                reader.ReadEndElement();

                if (nonce != null && nonce.Length > _maxKeyDerivationNonceLength)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SRP.Format(SRP.DerivedKeyTokenNonceTooLong, nonce.Length, _maxKeyDerivationNonceLength)));
                }

                reader.ReadEndElement();
            }

            public virtual SecurityToken CreateDerivedKeyToken(string id, string derivationAlgorithm, string label, int length, byte[] nonce, int offset, int generation, SecurityKeyIdentifierClause tokenToDeriveIdentifier, SecurityToken tokenToDerive)
            {
                if (tokenToDerive == null)
                {
                    return new DerivedKeySecurityTokenStub(generation, offset, length,
                        label, nonce, tokenToDeriveIdentifier, derivationAlgorithm, id);
                }
                else
                {
                    return new DerivedKeySecurityToken(generation, offset, length,
                        label, nonce, tokenToDerive, tokenToDeriveIdentifier, derivationAlgorithm, id);
                }
            }

            public override SecurityToken ReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver)
            {
                string id;
                string derivationAlgorithm;
                string label;
                int length;
                byte[] nonce;
                int offset;
                int generation;
                SecurityKeyIdentifierClause tokenToDeriveIdentifier;
                SecurityToken tokenToDerive;
                ReadDerivedKeyTokenParameters(reader, tokenResolver, out id, out derivationAlgorithm, out label, out length,
                    out nonce, out offset, out generation, out tokenToDeriveIdentifier, out tokenToDerive);

                return CreateDerivedKeyToken(id, derivationAlgorithm, label, length, nonce, offset, generation,
                    tokenToDeriveIdentifier, tokenToDerive);
            }

            public override void WriteTokenCore(XmlDictionaryWriter writer, SecurityToken token)
            {
                DerivedKeySecurityToken derivedKeyToken = token as DerivedKeySecurityToken;
                string serializerPrefix = _parent.SerializerDictionary.Prefix.Value;

                writer.WriteStartElement(serializerPrefix, _parent.SerializerDictionary.DerivedKeyToken, _parent.SerializerDictionary.Namespace);
                if (derivedKeyToken.Id != null)
                {
                    writer.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, derivedKeyToken.Id);
                }
                if (derivedKeyToken.KeyDerivationAlgorithm != _parent.DerivationAlgorithm)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.Format(SRP.UnsupportedKeyDerivationAlgorithm, derivedKeyToken.KeyDerivationAlgorithm)));
                }
                _parent.WSSecurityTokenSerializer.WriteKeyIdentifierClause(writer, derivedKeyToken.TokenToDeriveIdentifier);

                // Don't support Properties element
                if (derivedKeyToken.Generation > 0 || derivedKeyToken.Offset > 0 || derivedKeyToken.Length != 32)
                {
                    // this means they're both specified (offset must be gen * length) - we'll write generation
                    if (derivedKeyToken.Generation >= 0 && derivedKeyToken.Offset >= 0)
                    {
                        writer.WriteStartElement(serializerPrefix, _parent.SerializerDictionary.Generation, _parent.SerializerDictionary.Namespace);
                        writer.WriteValue(derivedKeyToken.Generation);
                        writer.WriteEndElement();
                    }
                    else if (derivedKeyToken.Generation != -1)
                    {
                        writer.WriteStartElement(serializerPrefix, _parent.SerializerDictionary.Generation, _parent.SerializerDictionary.Namespace);
                        writer.WriteValue(derivedKeyToken.Generation);
                        writer.WriteEndElement();
                    }
                    else if (derivedKeyToken.Offset != -1)
                    {
                        writer.WriteStartElement(serializerPrefix, _parent.SerializerDictionary.Offset, _parent.SerializerDictionary.Namespace);
                        writer.WriteValue(derivedKeyToken.Offset);
                        writer.WriteEndElement();
                    }

                    if (derivedKeyToken.Length != 32)
                    {
                        writer.WriteStartElement(serializerPrefix, _parent.SerializerDictionary.Length, _parent.SerializerDictionary.Namespace);
                        writer.WriteValue(derivedKeyToken.Length);
                        writer.WriteEndElement();
                    }
                }

                if (derivedKeyToken.Label != null)
                {
                    writer.WriteStartElement(serializerPrefix, _parent.SerializerDictionary.Generation, _parent.SerializerDictionary.Namespace);
                    writer.WriteString(derivedKeyToken.Label);
                    writer.WriteEndElement();
                }
                writer.WriteStartElement(serializerPrefix, _parent.SerializerDictionary.Nonce, _parent.SerializerDictionary.Namespace);
                writer.WriteBase64(derivedKeyToken.Nonce, 0, derivedKeyToken.Nonce.Length);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        protected abstract class SecurityContextTokenEntry : TokenEntry
        {
            public SecurityContextTokenEntry(WSSecureConversation parent, SecurityStateEncoder securityStateEncoder, IList<Type> knownClaimTypes)
            {
                Parent = parent;
            }

            protected WSSecureConversation Parent { get; }

            protected override XmlDictionaryString LocalName { get { return Parent.SerializerDictionary.SecurityContextToken; } }
            protected override XmlDictionaryString NamespaceUri { get { return Parent.SerializerDictionary.Namespace; } }
            protected override Type[] GetTokenTypesCore() { return new Type[] { typeof(SecurityContextSecurityToken) }; }
            public override string TokenTypeUri { get { return Parent.SerializerDictionary.SecurityContextTokenType.Value; } }
            protected override string ValueTypeUri { get { return null; } }

            public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXmlCore(XmlElement issuedTokenXml,
                SecurityTokenReferenceStyle tokenReferenceStyle)
            {

                TokenReferenceStyleHelper.Validate(tokenReferenceStyle);

                switch (tokenReferenceStyle)
                {
                    case SecurityTokenReferenceStyle.Internal:
                        return CreateDirectReference(issuedTokenXml, UtilityStrings.IdAttribute, UtilityStrings.Namespace, typeof(SecurityContextSecurityToken));
                    case SecurityTokenReferenceStyle.External:
                        UniqueId contextId = null;
                        UniqueId generation = null;
                        foreach (XmlNode node in issuedTokenXml.ChildNodes)
                        {
                            XmlElement element = node as XmlElement;
                            if (element != null)
                            {
                                if (element.LocalName == Parent.SerializerDictionary.Identifier.Value && element.NamespaceURI == Parent.SerializerDictionary.Namespace.Value)
                                {
                                    contextId = XmlHelper.ReadTextElementAsUniqueId(element);
                                }
                                else if (CanReadGeneration(element))
                                {
                                    generation = ReadGeneration(element);
                                }
                            }
                        }
                        return new SecurityContextKeyIdentifierClause(contextId, generation);
                    default:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("tokenReferenceStyle"));
                }
            }

            protected abstract bool CanReadGeneration(XmlDictionaryReader reader);
            protected abstract bool CanReadGeneration(XmlElement element);
            protected abstract UniqueId ReadGeneration(XmlDictionaryReader reader);
            protected abstract UniqueId ReadGeneration(XmlElement element);

            SecurityContextSecurityToken TryResolveSecurityContextToken(UniqueId contextId, UniqueId generation, string id, SecurityTokenResolver tokenResolver, out ISecurityContextSecurityTokenCache sctCache)
            {
                SecurityContextSecurityToken cachedSct = null;
                sctCache = null;
                if (tokenResolver is ISecurityContextSecurityTokenCache)
                {
                    sctCache = ((ISecurityContextSecurityTokenCache)tokenResolver);
                    cachedSct = sctCache.GetContext(contextId, generation);
                }
                else if (tokenResolver is AggregateSecurityHeaderTokenResolver)
                {
                    // We will see if we have a ISecurityContextSecurityTokenCache in the 
                    // AggregateTokenResolver. We will hold the reference to the first sctCache
                    // we find.
                    AggregateSecurityHeaderTokenResolver aggregateTokenResolve = tokenResolver as AggregateSecurityHeaderTokenResolver;
                    for (int i = 0; i < aggregateTokenResolve.TokenResolvers.Count; ++i)
                    {
                        ISecurityContextSecurityTokenCache oobTokenResolver = aggregateTokenResolve.TokenResolvers[i] as ISecurityContextSecurityTokenCache;
                        if (oobTokenResolver == null)
                        {
                            continue;
                        }
                        if (sctCache == null)
                        {
                            sctCache = oobTokenResolver;
                        }
                        cachedSct = oobTokenResolver.GetContext(contextId, generation);
                        if (cachedSct != null)
                        {
                            break;
                        }
                    }
                }
                if (cachedSct == null)
                {
                    return null;
                }
                else if (cachedSct.Id == id)
                {
                    return cachedSct;
                }
                else
                {
                    return new SecurityContextSecurityToken(cachedSct, id);
                }
            }

            public override SecurityToken ReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver)
            {
                UniqueId contextId = null;
                byte[] encodedCookie = null;
                UniqueId generation = null;
                bool isCookieMode = false;

                Fx.Assert(reader.NodeType == XmlNodeType.Element, "");

                // check if there is an id
                string id = reader.GetAttribute(XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace);

                SecurityContextSecurityToken sct = null;

                // There needs to be at least a contextId in here.
                reader.ReadFullStartElement();
                reader.MoveToStartElement(Parent.SerializerDictionary.Identifier, Parent.SerializerDictionary.Namespace);
                contextId = reader.ReadElementContentAsUniqueId();
                if (CanReadGeneration(reader))
                {
                    generation = ReadGeneration(reader);
                }
                if (reader.IsStartElement(Parent.SerializerDictionary.Cookie, XD.DotNetSecurityDictionary.Namespace))
                {
                    isCookieMode = true;
                    ISecurityContextSecurityTokenCache sctCache;
                    sct = TryResolveSecurityContextToken(contextId, generation, id, tokenResolver, out sctCache);
                    if (sct == null)
                    {
                        encodedCookie = reader.ReadElementContentAsBase64();
                        if (encodedCookie != null)
                        {
                            throw new PlatformNotSupportedException();
                            // Ultimately depends on ProtectedData which isn't available cross platform
                            // I believe this is server side only code but too complicated to reason about
                            // to know for sure.
                        }
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
                reader.ReadEndElement();

                if (contextId == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.NoSecurityContextIdentifier));
                }

                if (sct == null && !isCookieMode)
                {
                    ISecurityContextSecurityTokenCache sctCache;
                    sct = TryResolveSecurityContextToken(contextId, generation, id, tokenResolver, out sctCache);
                }
                if (sct == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new SecurityContextTokenValidationException(SRP.Format(SRP.SecurityContextNotRegistered, contextId, generation)));
                }
                return sct;
            }

            protected virtual void WriteGeneration(XmlDictionaryWriter writer, SecurityContextSecurityToken sct)
            {
            }

            public override void WriteTokenCore(XmlDictionaryWriter writer, SecurityToken token)
            {
                SecurityContextSecurityToken sct = (token as SecurityContextSecurityToken);

                // serialize the name and any wsu:Id attribute
                writer.WriteStartElement(Parent.SerializerDictionary.Prefix.Value, Parent.SerializerDictionary.SecurityContextToken, Parent.SerializerDictionary.Namespace);
                if (sct.Id != null)
                {
                    writer.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, sct.Id);
                }

                // serialize the context id
                writer.WriteStartElement(Parent.SerializerDictionary.Prefix.Value, Parent.SerializerDictionary.Identifier, Parent.SerializerDictionary.Namespace);
                XmlHelper.WriteStringAsUniqueId(writer, sct.ContextId);
                writer.WriteEndElement();

                WriteGeneration(writer, sct);

                // if cookie-mode, then it must have a cookie
                if (sct.IsCookieMode)
                {
                    if (sct.CookieBlob == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.NoCookieInSct));
                    }

                    // if the token has a cookie, write it out
                    writer.WriteStartElement(XD.DotNetSecurityDictionary.Prefix.Value, Parent.SerializerDictionary.Cookie, XD.DotNetSecurityDictionary.Namespace);
                    writer.WriteBase64(sct.CookieBlob, 0, sct.CookieBlob.Length);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        public abstract class Driver : SecureConversationDriver
        {
            public Driver()
            {
            }

            protected abstract SecureConversationDictionary DriverDictionary
            {
                get;
            }

            public override XmlDictionaryString IssueAction
            {
                get
                {
                    return DriverDictionary.RequestSecurityContextIssuance;
                }
            }

            public override XmlDictionaryString IssueResponseAction
            {
                get
                {
                    return DriverDictionary.RequestSecurityContextIssuanceResponse;
                }
            }

            public override XmlDictionaryString RenewNeededFaultCode
            {
                get { return DriverDictionary.RenewNeededFaultCode; }
            }

            public override XmlDictionaryString BadContextTokenFaultCode
            {
                get { return DriverDictionary.BadContextTokenFaultCode; }
            }

            public override UniqueId GetSecurityContextTokenId(XmlDictionaryReader reader)
            {
                if (reader == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(reader));
                }

                reader.ReadStartElement(DriverDictionary.SecurityContextToken, DriverDictionary.Namespace);
                UniqueId contextId = XmlHelper.ReadElementStringAsUniqueId(reader, DriverDictionary.Identifier, DriverDictionary.Namespace);
                while (reader.IsStartElement())
                {
                    reader.Skip();
                }
                reader.ReadEndElement();
                return contextId;
            }

            public override bool IsAtSecurityContextToken(XmlDictionaryReader reader)
            {
                if (reader == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(reader));
                }

                return reader.IsStartElement(DriverDictionary.SecurityContextToken, DriverDictionary.Namespace);
            }
        }
    }
}
