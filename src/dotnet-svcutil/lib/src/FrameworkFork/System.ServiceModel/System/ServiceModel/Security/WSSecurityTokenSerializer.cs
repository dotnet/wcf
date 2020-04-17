// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel;
using System.ServiceModel.Security.Tokens;
using Microsoft.Xml;
using System.ServiceModel.Diagnostics;
using System.Diagnostics;

namespace System.ServiceModel.Security
{
    public class WSSecurityTokenSerializer : SecurityTokenSerializer
    {
        private const int DefaultMaximumKeyDerivationOffset = 64; // bytes 
        private const int DefaultMaximumKeyDerivationLabelLength = 128; // bytes
        private const int DefaultMaximumKeyDerivationNonceLength = 128; // bytes

        private static WSSecurityTokenSerializer s_instance;
        private readonly bool _emitBspRequiredAttributes;
        private readonly SecurityVersion _securityVersion;
        private readonly List<SerializerEntries> _serializerEntries;
        private WSSecureConversation _secureConversation;
        private readonly List<TokenEntry> _tokenEntries;
        private int _maximumKeyDerivationOffset;
        private int _maximumKeyDerivationLabelLength;
        private int _maximumKeyDerivationNonceLength;

        private KeyInfoSerializer _keyInfoSerializer;

        public WSSecurityTokenSerializer()
            : this(SecurityVersion.WSSecurity11)
        {
        }

        public WSSecurityTokenSerializer(bool emitBspRequiredAttributes)
            : this(SecurityVersion.WSSecurity11, emitBspRequiredAttributes)
        {
        }

        public WSSecurityTokenSerializer(SecurityVersion securityVersion)
            : this(securityVersion, false)
        {
        }

        public WSSecurityTokenSerializer(SecurityVersion securityVersion, bool emitBspRequiredAttributes)
            : this(securityVersion, emitBspRequiredAttributes, null)
        {
        }

        public WSSecurityTokenSerializer(SecurityVersion securityVersion, bool emitBspRequiredAttributes, SamlSerializer samlSerializer)
            : this(securityVersion, emitBspRequiredAttributes, samlSerializer, null, null)
        {
        }

        public WSSecurityTokenSerializer(SecurityVersion securityVersion, bool emitBspRequiredAttributes, SamlSerializer samlSerializer, SecurityStateEncoder securityStateEncoder, IEnumerable<Type> knownTypes)
            : this(securityVersion, emitBspRequiredAttributes, samlSerializer, securityStateEncoder, knownTypes, DefaultMaximumKeyDerivationOffset, DefaultMaximumKeyDerivationLabelLength, DefaultMaximumKeyDerivationNonceLength)
        {
        }

        public WSSecurityTokenSerializer(SecurityVersion securityVersion, TrustVersion trustVersion, SecureConversationVersion secureConversationVersion, bool emitBspRequiredAttributes, SamlSerializer samlSerializer, SecurityStateEncoder securityStateEncoder, IEnumerable<Type> knownTypes)
            : this(securityVersion, trustVersion, secureConversationVersion, emitBspRequiredAttributes, samlSerializer, securityStateEncoder, knownTypes, DefaultMaximumKeyDerivationOffset, DefaultMaximumKeyDerivationLabelLength, DefaultMaximumKeyDerivationNonceLength)
        {
        }

        public WSSecurityTokenSerializer(SecurityVersion securityVersion, bool emitBspRequiredAttributes, SamlSerializer samlSerializer, SecurityStateEncoder securityStateEncoder, IEnumerable<Type> knownTypes,
            int maximumKeyDerivationOffset, int maximumKeyDerivationLabelLength, int maximumKeyDerivationNonceLength)
            : this(securityVersion, TrustVersion.Default, SecureConversationVersion.Default, emitBspRequiredAttributes, samlSerializer, securityStateEncoder, knownTypes, maximumKeyDerivationOffset, maximumKeyDerivationLabelLength, maximumKeyDerivationNonceLength)
        {
        }

        public WSSecurityTokenSerializer(SecurityVersion securityVersion, TrustVersion trustVersion, SecureConversationVersion secureConversationVersion, bool emitBspRequiredAttributes, SamlSerializer samlSerializer, SecurityStateEncoder securityStateEncoder, IEnumerable<Type> knownTypes,
            int maximumKeyDerivationOffset, int maximumKeyDerivationLabelLength, int maximumKeyDerivationNonceLength)
        {
            if (securityVersion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("securityVersion"));

            if (maximumKeyDerivationOffset < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("maximumKeyDerivationOffset", SRServiceModel.ValueMustBeNonNegative));
            }
            if (maximumKeyDerivationLabelLength < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("maximumKeyDerivationLabelLength", SRServiceModel.ValueMustBeNonNegative));
            }
            if (maximumKeyDerivationNonceLength <= 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("maximumKeyDerivationNonceLength", SRServiceModel.ValueMustBeGreaterThanZero));
            }

            _securityVersion = securityVersion;
            _emitBspRequiredAttributes = emitBspRequiredAttributes;
            _maximumKeyDerivationOffset = maximumKeyDerivationOffset;
            _maximumKeyDerivationNonceLength = maximumKeyDerivationNonceLength;
            _maximumKeyDerivationLabelLength = maximumKeyDerivationLabelLength;

            _serializerEntries = new List<SerializerEntries>();

            if (secureConversationVersion == SecureConversationVersion.WSSecureConversationFeb2005)
            {
                _secureConversation = new WSSecureConversationFeb2005(this, securityStateEncoder, knownTypes, maximumKeyDerivationOffset, maximumKeyDerivationLabelLength, maximumKeyDerivationNonceLength);
            }
            else if (secureConversationVersion == SecureConversationVersion.WSSecureConversation13)
            {
                _secureConversation = new WSSecureConversationDec2005(this, securityStateEncoder, knownTypes, maximumKeyDerivationOffset, maximumKeyDerivationLabelLength, maximumKeyDerivationNonceLength);
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }

            if (securityVersion == SecurityVersion.WSSecurity10)
            {
                _serializerEntries.Add(new WSSecurityJan2004(this, samlSerializer));
            }
            else if (securityVersion == SecurityVersion.WSSecurity11)
            {
                _serializerEntries.Add(new WSSecurityXXX2005(this, samlSerializer));
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("securityVersion", SRServiceModel.MessageSecurityVersionOutOfRange));
            }
            _serializerEntries.Add(_secureConversation);
            IdentityModel.TrustDictionary trustDictionary;
            if (trustVersion == TrustVersion.WSTrustFeb2005)
            {
                _serializerEntries.Add(new WSTrustFeb2005(this));
                trustDictionary = new IdentityModel.TrustFeb2005Dictionary(new CollectionDictionary(DXD.TrustDec2005Dictionary.Feb2005DictionaryStrings));
            }
            else if (trustVersion == TrustVersion.WSTrust13)
            {
                _serializerEntries.Add(new WSTrustDec2005(this));
                trustDictionary = new IdentityModel.TrustDec2005Dictionary(new CollectionDictionary(DXD.TrustDec2005Dictionary.Dec2005DictionaryString));
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }

            _tokenEntries = new List<TokenEntry>();

            for (int i = 0; i < _serializerEntries.Count; ++i)
            {
                SerializerEntries serializerEntry = _serializerEntries[i];
                serializerEntry.PopulateTokenEntries(_tokenEntries);
            }

            IdentityModel.DictionaryManager dictionaryManager = new IdentityModel.DictionaryManager(ServiceModelDictionary.CurrentVersion);
            dictionaryManager.SecureConversationDec2005Dictionary = new IdentityModel.SecureConversationDec2005Dictionary(new CollectionDictionary(DXD.SecureConversationDec2005Dictionary.SecureConversationDictionaryStrings));
            dictionaryManager.SecurityAlgorithmDec2005Dictionary = new IdentityModel.SecurityAlgorithmDec2005Dictionary(new CollectionDictionary(DXD.SecurityAlgorithmDec2005Dictionary.SecurityAlgorithmDictionaryStrings));

            _keyInfoSerializer = new WSKeyInfoSerializer(_emitBspRequiredAttributes, dictionaryManager, trustDictionary, this, securityVersion, secureConversationVersion);
        }

        public static WSSecurityTokenSerializer DefaultInstance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new WSSecurityTokenSerializer();
                return s_instance;
            }
        }

        public bool EmitBspRequiredAttributes
        {
            get { return _emitBspRequiredAttributes; }
        }

        public SecurityVersion SecurityVersion
        {
            get { return _securityVersion; }
        }

        public int MaximumKeyDerivationOffset
        {
            get { return _maximumKeyDerivationOffset; }
        }

        public int MaximumKeyDerivationLabelLength
        {
            get { return _maximumKeyDerivationLabelLength; }
        }

        public int MaximumKeyDerivationNonceLength
        {
            get { return _maximumKeyDerivationNonceLength; }
        }

        internal WSSecureConversation SecureConversation
        {
            get { return _secureConversation; }
        }

        private bool ShouldWrapException(Exception e)
        {
            if (Fx.IsFatal(e))
            {
                return false;
            }
            return ((e is ArgumentException) || (e is FormatException) || (e is InvalidOperationException));
        }

        protected override bool CanReadTokenCore(XmlReader reader)
        {
            XmlDictionaryReader localReader = XmlDictionaryReader.CreateDictionaryReader(reader);
            for (int i = 0; i < _tokenEntries.Count; i++)
            {
                TokenEntry tokenEntry = _tokenEntries[i];
                if (tokenEntry.CanReadTokenCore(localReader))
                    return true;
            }
            return false;
        }

        protected override SecurityToken ReadTokenCore(XmlReader reader, SecurityTokenResolver tokenResolver)
        {
            XmlDictionaryReader localReader = XmlDictionaryReader.CreateDictionaryReader(reader);
            for (int i = 0; i < _tokenEntries.Count; i++)
            {
                TokenEntry tokenEntry = _tokenEntries[i];
                if (tokenEntry.CanReadTokenCore(localReader))
                {
                    try
                    {
                        return tokenEntry.ReadTokenCore(localReader, tokenResolver);
                    }
#pragma warning disable 56500 // covered by FxCOP
                    catch (Exception e)
                    {
                        if (!ShouldWrapException(e))
                        {
                            throw;
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SRServiceModel.ErrorDeserializingTokenXml, e));
                    }
                }
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(string.Format(SRServiceModel.CannotReadToken, reader.LocalName, reader.NamespaceURI, localReader.GetAttribute(XD.SecurityJan2004Dictionary.ValueType, null))));
        }

        protected override bool CanWriteTokenCore(SecurityToken token)
        {
            for (int i = 0; i < _tokenEntries.Count; i++)
            {
                TokenEntry tokenEntry = _tokenEntries[i];
                if (tokenEntry.SupportsCore(token.GetType()))
                    return true;
            }
            return false;
        }

        protected override void WriteTokenCore(XmlWriter writer, SecurityToken token)
        {
            throw new NotImplementedException();
        }

        protected override bool CanReadKeyIdentifierCore(XmlReader reader)
        {
            try
            {
                return _keyInfoSerializer.CanReadKeyIdentifier(reader);
            }
            catch (System.IdentityModel.SecurityMessageSerializationException ex)
            {
                throw FxTrace.Exception.AsError(new MessageSecurityException(ex.Message));
            }
        }

        protected override SecurityKeyIdentifier ReadKeyIdentifierCore(XmlReader reader)
        {
            try
            {
                return _keyInfoSerializer.ReadKeyIdentifier(reader);
            }
            catch (System.IdentityModel.SecurityMessageSerializationException ex)
            {
                throw FxTrace.Exception.AsError(new MessageSecurityException(ex.Message));
            }
        }

        protected override bool CanWriteKeyIdentifierCore(SecurityKeyIdentifier keyIdentifier)
        {
            try
            {
                return _keyInfoSerializer.CanWriteKeyIdentifier(keyIdentifier);
            }
            catch (System.IdentityModel.SecurityMessageSerializationException ex)
            {
                throw FxTrace.Exception.AsError(new MessageSecurityException(ex.Message));
            }
        }

        protected override void WriteKeyIdentifierCore(XmlWriter writer, SecurityKeyIdentifier keyIdentifier)
        {
            try
            {
                _keyInfoSerializer.WriteKeyIdentifier(writer, keyIdentifier);
            }
            catch (System.IdentityModel.SecurityMessageSerializationException ex)
            {
                throw FxTrace.Exception.AsError(new MessageSecurityException(ex.Message));
            }
        }

        protected override bool CanReadKeyIdentifierClauseCore(XmlReader reader)
        {
            try
            {
                return _keyInfoSerializer.CanReadKeyIdentifierClause(reader);
            }
            catch (System.IdentityModel.SecurityMessageSerializationException ex)
            {
                throw FxTrace.Exception.AsError(new MessageSecurityException(ex.Message));
            }
        }

        protected override SecurityKeyIdentifierClause ReadKeyIdentifierClauseCore(XmlReader reader)
        {
            try
            {
                return _keyInfoSerializer.ReadKeyIdentifierClause(reader);
            }
            catch (System.IdentityModel.SecurityMessageSerializationException ex)
            {
                throw FxTrace.Exception.AsError(new MessageSecurityException(ex.Message));
            }
        }

        protected override bool CanWriteKeyIdentifierClauseCore(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            try
            {
                return _keyInfoSerializer.CanWriteKeyIdentifierClause(keyIdentifierClause);
            }
            catch (System.IdentityModel.SecurityMessageSerializationException ex)
            {
                throw FxTrace.Exception.AsError(new MessageSecurityException(ex.Message));
            }
        }

        protected override void WriteKeyIdentifierClauseCore(XmlWriter writer, SecurityKeyIdentifierClause keyIdentifierClause)
        {
            try
            {
                _keyInfoSerializer.WriteKeyIdentifierClause(writer, keyIdentifierClause);
            }
            catch (System.IdentityModel.SecurityMessageSerializationException ex)
            {
                throw FxTrace.Exception.AsError(new MessageSecurityException(ex.Message));
            }
        }

        internal Type[] GetTokenTypes(string tokenTypeUri)
        {
            if (tokenTypeUri != null)
            {
                for (int i = 0; i < _tokenEntries.Count; i++)
                {
                    TokenEntry tokenEntry = _tokenEntries[i];

                    if (tokenEntry.SupportsTokenTypeUri(tokenTypeUri))
                    {
                        return tokenEntry.GetTokenTypes();
                    }
                }
            }
            return null;
        }

        protected internal virtual string GetTokenTypeUri(Type tokenType)
        {
            if (tokenType != null)
            {
                for (int i = 0; i < _tokenEntries.Count; i++)
                {
                    TokenEntry tokenEntry = _tokenEntries[i];

                    if (tokenEntry.SupportsCore(tokenType))
                    {
                        return tokenEntry.TokenTypeUri;
                    }
                }
            }
            return null;
        }

        public virtual bool TryCreateKeyIdentifierClauseFromTokenXml(XmlElement element, SecurityTokenReferenceStyle tokenReferenceStyle, out SecurityKeyIdentifierClause securityKeyIdentifierClause)
        {
            if (element == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");

            securityKeyIdentifierClause = null;

            try
            {
                securityKeyIdentifierClause = CreateKeyIdentifierClauseFromTokenXml(element, tokenReferenceStyle);
            }
            catch (XmlException e)
            {
                return false;
            }

            return true;
        }

        public virtual SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXml(XmlElement element, SecurityTokenReferenceStyle tokenReferenceStyle)
        {
            if (element == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");

            for (int i = 0; i < _tokenEntries.Count; i++)
            {
                TokenEntry tokenEntry = _tokenEntries[i];
                if (tokenEntry.CanReadTokenCore(element))
                {
                    try
                    {
                        return tokenEntry.CreateKeyIdentifierClauseFromTokenXmlCore(element, tokenReferenceStyle);
                    }
#pragma warning disable 56500 // covered by FxCOP
                    catch (Exception e)
                    {
                        if (!ShouldWrapException(e))
                        {
                            throw;
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(string.Format(SRServiceModel.ErrorDeserializingKeyIdentifierClauseFromTokenXml), e));
                    }
                }
            }

            // PreSharp Bug: Parameter 'element' to this public method must be validated: A null-dereference can occur here.
#pragma warning disable 56506
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(string.Format(SRServiceModel.CannotReadToken, element.LocalName, element.NamespaceURI, element.GetAttribute(SecurityJan2004Strings.ValueType, null))));
        }

        internal abstract new class TokenEntry
        {
            private Type[] _tokenTypes = null;
            public virtual IAsyncResult BeginReadTokenCore(XmlDictionaryReader reader,
                SecurityTokenResolver tokenResolver, AsyncCallback callback, object state)
            {
                SecurityToken result = this.ReadTokenCore(reader, tokenResolver);
                return new CompletedAsyncResult<SecurityToken>(result, callback, state);
            }

            protected abstract XmlDictionaryString LocalName { get; }
            protected abstract XmlDictionaryString NamespaceUri { get; }
            public Type TokenType { get { return GetTokenTypes()[0]; } }
            public abstract string TokenTypeUri { get; }
            protected abstract string ValueTypeUri { get; }

            protected abstract Type[] GetTokenTypesCore();

            public Type[] GetTokenTypes()
            {
                if (_tokenTypes == null)
                    _tokenTypes = GetTokenTypesCore();
                return _tokenTypes;
            }

            public bool SupportsCore(Type tokenType)
            {
                Type[] tokenTypes = GetTokenTypes();
                for (int i = 0; i < tokenTypes.Length; ++i)
                {
                    if (tokenTypes[i].IsAssignableFrom(tokenType))
                        return true;
                }
                return false;
            }

            public virtual bool SupportsTokenTypeUri(string tokenTypeUri)
            {
                return (this.TokenTypeUri == tokenTypeUri);
            }

            protected static SecurityKeyIdentifierClause CreateDirectReference(XmlElement issuedTokenXml, string idAttributeLocalName, string idAttributeNamespace, Type tokenType)
            {
                string id = issuedTokenXml.GetAttribute(idAttributeLocalName, idAttributeNamespace);
                if (id == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(string.Format(SRServiceModel.RequiredAttributeMissing, idAttributeLocalName, issuedTokenXml.LocalName)));
                }
                return new LocalIdKeyIdentifierClause(id, tokenType);
            }

            public virtual bool CanReadTokenCore(XmlElement element)
            {
                string valueTypeUri = null;

                if (element.HasAttribute(SecurityJan2004Strings.ValueType, null))
                {
                    valueTypeUri = element.GetAttribute(SecurityJan2004Strings.ValueType, null);
                }

                return element.LocalName == LocalName.Value && element.NamespaceURI == NamespaceUri.Value && valueTypeUri == this.ValueTypeUri;
            }

            public virtual bool CanReadTokenCore(XmlDictionaryReader reader)
            {
                return reader.IsStartElement(this.LocalName, this.NamespaceUri) &&
                       reader.GetAttribute(XD.SecurityJan2004Dictionary.ValueType, null) == this.ValueTypeUri;
            }

            public virtual SecurityToken EndReadTokenCore(IAsyncResult result)
            {
                return CompletedAsyncResult<SecurityToken>.End(result);
            }

            public abstract SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXmlCore(XmlElement issuedTokenXml, SecurityTokenReferenceStyle tokenReferenceStyle);

            public abstract SecurityToken ReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver);

            public abstract void WriteTokenCore(XmlDictionaryWriter writer, SecurityToken token);
        }

        internal abstract new class SerializerEntries
        {
            public virtual void PopulateTokenEntries(IList<TokenEntry> tokenEntries) { }
        }

        internal class CollectionDictionary : IXmlDictionary
        {
            private List<XmlDictionaryString> _dictionaryStrings;

            public CollectionDictionary(List<XmlDictionaryString> dictionaryStrings)
            {
                if (dictionaryStrings == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("dictionaryStrings"));

                _dictionaryStrings = dictionaryStrings;
            }

            public bool TryLookup(string value, out XmlDictionaryString result)
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));

                for (int i = 0; i < _dictionaryStrings.Count; ++i)
                {
                    if (_dictionaryStrings[i].Value.Equals(value))
                    {
                        result = _dictionaryStrings[i];
                        return true;
                    }
                }
                result = null;
                return false;
            }

            public bool TryLookup(int key, out XmlDictionaryString result)
            {
                for (int i = 0; i < _dictionaryStrings.Count; ++i)
                {
                    if (_dictionaryStrings[i].Key == key)
                    {
                        result = _dictionaryStrings[i];
                        return true;
                    }
                }
                result = null;
                return false;
            }

            public bool TryLookup(XmlDictionaryString value, out XmlDictionaryString result)
            {
                if (value == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));

                for (int i = 0; i < _dictionaryStrings.Count; ++i)
                {
                    if ((_dictionaryStrings[i].Key == value.Key) &&
                        (_dictionaryStrings[i].Value.Equals(value.Value)))
                    {
                        result = _dictionaryStrings[i];
                        return true;
                    }
                }
                result = null;
                return false;
            }
        }
    }
}
