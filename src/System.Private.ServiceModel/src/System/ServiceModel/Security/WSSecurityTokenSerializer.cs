// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel;
using System.ServiceModel.Security.Tokens;
using System.Xml;
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
        private readonly List<TokenEntry> _tokenEntries = new List<TokenEntry>();

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
            throw ExceptionHelper.PlatformNotSupported();
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
            get { return false; }
        }

        public SecurityVersion SecurityVersion
        {
            get { return null; }
        }

        public int MaximumKeyDerivationOffset
        {
            get { return 0; }
        }

        public int MaximumKeyDerivationLabelLength
        {
            get { return 0; }
        }

        public int MaximumKeyDerivationNonceLength
        {
            get { return 0; }
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
#pragma warning suppress 56500 // covered by FxCOP
                    catch (Exception e)
                    {
                        if (!ShouldWrapException(e))
                        {
                            throw;
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.ErrorDeserializingTokenXml, e));
                    }
                }
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.Format(SR.CannotReadToken, reader.LocalName, reader.NamespaceURI, localReader.GetAttribute(XD.SecurityJan2004Dictionary.ValueType, null))));
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
            throw ExceptionHelper.PlatformNotSupported();
        }

        protected override bool CanReadKeyIdentifierCore(XmlReader reader)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        protected override SecurityKeyIdentifier ReadKeyIdentifierCore(XmlReader reader)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        protected override bool CanWriteKeyIdentifierCore(SecurityKeyIdentifier keyIdentifier)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        protected override void WriteKeyIdentifierCore(XmlWriter writer, SecurityKeyIdentifier keyIdentifier)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        protected override bool CanReadKeyIdentifierClauseCore(XmlReader reader)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        protected override SecurityKeyIdentifierClause ReadKeyIdentifierClauseCore(XmlReader reader)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        protected override bool CanWriteKeyIdentifierClauseCore(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        protected override void WriteKeyIdentifierClauseCore(XmlWriter writer, SecurityKeyIdentifierClause keyIdentifierClause)
        {
            throw ExceptionHelper.PlatformNotSupported();
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
            catch (XmlException)
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
#pragma warning suppress 56500 // covered by FxCOP
                    catch (Exception e)
                    {
                        if (!ShouldWrapException(e))
                        {
                            throw;
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.ErrorDeserializingKeyIdentifierClauseFromTokenXml, e));
                    }
                }
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.Format(SR.CannotReadToken, element.LocalName, element.NamespaceURI, element.GetAttribute(SecurityJan2004Strings.ValueType, null))));
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.Format(SR.RequiredAttributeMissing, idAttributeLocalName, issuedTokenXml.LocalName)));
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
