// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Runtime;
using System.ServiceModel.Security;
using System.Xml;
using TokenEntry = System.ServiceModel.Security.WSSecurityTokenSerializer.TokenEntry;
using DiagnosticUtility = System.ServiceModel.DiagnosticUtility;

namespace System.IdentityModel.Tokens
{
    /// <summary>
    /// Abstract class for SecurityKeyIdentifierClause Serializer.
    /// </summary>
    internal class KeyInfoSerializer : SecurityTokenSerializer
    {
        private readonly List<SecurityTokenSerializer.KeyIdentifierEntry> _keyIdentifierEntries;
        private readonly List<SecurityTokenSerializer.KeyIdentifierClauseEntry> _keyIdentifierClauseEntries;
        private readonly List<SecurityTokenSerializer.SerializerEntries> _serializerEntries;
        private readonly List<TokenEntry> _tokenEntries;

        private DictionaryManager _dictionaryManager;
        private bool _emitBspRequiredAttributes;
        private SecurityTokenSerializer _innerSecurityTokenSerializer;

        /// <summary>
        /// Creates an instance of <see cref="SecurityKeyIdentifierClauseSerializer"/>
        /// </summary>
        public KeyInfoSerializer(bool emitBspRequiredAttributes)
            : this(emitBspRequiredAttributes, new DictionaryManager(), XD.TrustDec2005Dictionary, null)
        {
        }

        public KeyInfoSerializer(
            bool emitBspRequiredAttributes,
            DictionaryManager dictionaryManager,
            TrustDictionary trustDictionary,
            SecurityTokenSerializer innerSecurityTokenSerializer ) :
            this( emitBspRequiredAttributes, dictionaryManager, trustDictionary, innerSecurityTokenSerializer, null )
        {
        }

        public KeyInfoSerializer(
            bool emitBspRequiredAttributes,
            DictionaryManager dictionaryManager,
            TrustDictionary trustDictionary,
            SecurityTokenSerializer innerSecurityTokenSerializer,
            Func<KeyInfoSerializer, IEnumerable<SerializerEntries>> additionalEntries)
        {
            _dictionaryManager = dictionaryManager;
            _emitBspRequiredAttributes = emitBspRequiredAttributes;
            _innerSecurityTokenSerializer = innerSecurityTokenSerializer;

            _serializerEntries = new List<SecurityTokenSerializer.SerializerEntries>();

            _serializerEntries.Add(new XmlDsigSep2000(this));
            _serializerEntries.Add(new XmlEncApr2001(this));

            // Issue #31 in progress (WSTrust is abstract in ServiceModel)
            //_serializerEntries.Add(new WSTrust(this, trustDictionary));

            if (additionalEntries != null)
            {
                foreach (SerializerEntries entries in additionalEntries(this))
                {
                    _serializerEntries.Add(entries);
                }
            }

            bool wsSecuritySerializerFound = false;
            foreach (SerializerEntries entry in _serializerEntries)
            {
                if ((entry is WSSecurityXXX2005) || (entry is WSSecurityJan2004))
                {
                    wsSecuritySerializerFound = true;
                    break;
                }
            }

            if (!wsSecuritySerializerFound)
            {
                _serializerEntries.Add(new WSSecurityXXX2005((WSSecurityTokenSerializer) innerSecurityTokenSerializer));
            }

            _tokenEntries = new List<TokenEntry>();
            _keyIdentifierEntries = new List<SecurityTokenSerializer.KeyIdentifierEntry>();
            _keyIdentifierClauseEntries = new List<SecurityTokenSerializer.KeyIdentifierClauseEntry>();

            for (int i = 0; i < _serializerEntries.Count; ++i)
            {
                SecurityTokenSerializer.SerializerEntries serializerEntry = _serializerEntries[i];
                serializerEntry.PopulateTokenEntries(_tokenEntries);
                serializerEntry.PopulateKeyIdentifierEntries(_keyIdentifierEntries);
                serializerEntry.PopulateKeyIdentifierClauseEntries(_keyIdentifierClauseEntries);
            }
        }

        public DictionaryManager DictionaryManager
        {
            get { return _dictionaryManager; }
        }

        /// <summary>
        /// Gets or sets a value indicating if BSP required attributes should be written out.
        /// </summary>
        public bool EmitBspRequiredAttributes
        {
            get
            {
                return _emitBspRequiredAttributes;
            }
        }

        public SecurityTokenSerializer InnerSecurityTokenSerializer
        {
            get
            {
                return _innerSecurityTokenSerializer == null ? this : _innerSecurityTokenSerializer;
            }
           set
            {
                _innerSecurityTokenSerializer = value;
            }
        }

        protected override bool CanReadTokenCore(XmlReader reader)
        {
            return false;
        }

        protected override SecurityToken ReadTokenCore(XmlReader reader, SecurityTokenResolver tokenResolver)
        {
            XmlDictionaryReader localReader = XmlDictionaryReader.CreateDictionaryReader( reader ); 
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError( new XmlException( SR.Format( SR.CannotReadToken, reader.LocalName, reader.NamespaceURI, localReader.GetAttribute( XD.SecurityJan2004Dictionary.ValueType, null ) ) ) );
        }

        protected override bool CanWriteTokenCore(SecurityToken token)
        {
            return false;
        }

        protected override void WriteTokenCore(XmlWriter writer, SecurityToken token)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.StandardsManagerCannotWriteObject, token.GetType())));
        }

        protected override bool CanReadKeyIdentifierCore(XmlReader reader)
        {
            XmlDictionaryReader localReader = XmlDictionaryReader.CreateDictionaryReader(reader);
            for (int i = 0; i < _keyIdentifierEntries.Count; i++)
            {
                KeyIdentifierEntry keyIdentifierEntry = _keyIdentifierEntries[i];
                if (keyIdentifierEntry.CanReadKeyIdentifierCore(localReader))
                    return true;
            }
            return false;
        }

        protected override SecurityKeyIdentifier ReadKeyIdentifierCore(XmlReader reader)
        {
            XmlDictionaryReader localReader = XmlDictionaryReader.CreateDictionaryReader(reader);
            localReader.ReadStartElement(XD.XmlSignatureDictionary.KeyInfo, XD.XmlSignatureDictionary.Namespace);
            SecurityKeyIdentifier keyIdentifier = new SecurityKeyIdentifier();
            while (localReader.IsStartElement())
            {
                SecurityKeyIdentifierClause clause = InnerSecurityTokenSerializer.ReadKeyIdentifierClause(localReader);
                if (clause == null)
                {
                    localReader.Skip();
                }
                else
                {
                    keyIdentifier.Add(clause);
                }
            }
            if (keyIdentifier.Count == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.Format(SR.ErrorDeserializingKeyIdentifierClause)));
            }
            localReader.ReadEndElement();

            return keyIdentifier;
        }

        protected override bool CanWriteKeyIdentifierCore(SecurityKeyIdentifier keyIdentifier)
        {
            for (int i = 0; i < _keyIdentifierEntries.Count; ++i)
            {
                KeyIdentifierEntry keyIdentifierEntry = _keyIdentifierEntries[i];
                if (keyIdentifierEntry.SupportsCore(keyIdentifier))
                    return true;
            }
            return false;
        }

        protected override void WriteKeyIdentifierCore(XmlWriter writer, SecurityKeyIdentifier keyIdentifier)
        {
            bool wroteKeyIdentifier = false;
            XmlDictionaryWriter localWriter = XmlDictionaryWriter.CreateDictionaryWriter(writer);
            for (int i = 0; i < _keyIdentifierEntries.Count; ++i)
            {
                KeyIdentifierEntry keyIdentifierEntry = _keyIdentifierEntries[i];
                if (keyIdentifierEntry.SupportsCore(keyIdentifier))
                {
                    try
                    {
                        keyIdentifierEntry.WriteKeyIdentifierCore(localWriter, keyIdentifier);
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                            throw;
                        
                        if (!ShouldWrapException(e))
                        {
                            throw;
                        }

                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.Format(SR.ErrorSerializingKeyIdentifier), e));
                    }
                    wroteKeyIdentifier = true;
                    break;
                }
            }

            if (!wroteKeyIdentifier)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.StandardsManagerCannotWriteObject, keyIdentifier.GetType())));

            localWriter.Flush();
        }

        protected override bool CanReadKeyIdentifierClauseCore(XmlReader reader)
        {
            XmlDictionaryReader localReader = XmlDictionaryReader.CreateDictionaryReader(reader);
            for (int i = 0; i < _keyIdentifierClauseEntries.Count; i++)
            {
                KeyIdentifierClauseEntry keyIdentifierClauseEntry = _keyIdentifierClauseEntries[i];
                if (keyIdentifierClauseEntry.CanReadKeyIdentifierClauseCore(localReader))
                    return true;
            }
            return false;
        }

        protected override SecurityKeyIdentifierClause ReadKeyIdentifierClauseCore(XmlReader reader)
        {
            XmlDictionaryReader localReader = XmlDictionaryReader.CreateDictionaryReader(reader);
            for (int i = 0; i < _keyIdentifierClauseEntries.Count; i++)
            {
                KeyIdentifierClauseEntry keyIdentifierClauseEntry = _keyIdentifierClauseEntries[i];
                if (keyIdentifierClauseEntry.CanReadKeyIdentifierClauseCore(localReader))
                {
                    try
                    {
                        return keyIdentifierClauseEntry.ReadKeyIdentifierClauseCore(localReader);
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                            throw;

                        if (!ShouldWrapException(e))
                        {
                            throw;
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.Format(SR.ErrorDeserializingKeyIdentifierClause), e));
                    }
                }
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.Format(SR.CannotReadKeyIdentifierClause, reader.LocalName, reader.NamespaceURI)));
        }

        protected override bool CanWriteKeyIdentifierClauseCore(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            for (int i = 0; i < _keyIdentifierClauseEntries.Count; ++i)
            {
                KeyIdentifierClauseEntry keyIdentifierClauseEntry = _keyIdentifierClauseEntries[i];
                if (keyIdentifierClauseEntry.SupportsCore(keyIdentifierClause))
                    return true;
            }
            return false;
        }

        protected override void WriteKeyIdentifierClauseCore(XmlWriter writer, SecurityKeyIdentifierClause keyIdentifierClause)
        {
            bool wroteKeyIdentifierClause = false;
            XmlDictionaryWriter localWriter = XmlDictionaryWriter.CreateDictionaryWriter(writer);
            for (int i = 0; i < _keyIdentifierClauseEntries.Count; ++i)
            {
                KeyIdentifierClauseEntry keyIdentifierClauseEntry = _keyIdentifierClauseEntries[i];
                if (keyIdentifierClauseEntry.SupportsCore(keyIdentifierClause))
                {
                    try
                    {
                        keyIdentifierClauseEntry.WriteKeyIdentifierClauseCore(localWriter, keyIdentifierClause);
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                            throw;

                        if (!ShouldWrapException(e))
                        {
                            throw;
                        }
                        
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.Format(SR.ErrorSerializingKeyIdentifierClause), e));
                    }
                    wroteKeyIdentifierClause = true;
                    break;
                }
            }

            if (!wroteKeyIdentifierClause)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.StandardsManagerCannotWriteObject, keyIdentifierClause.GetType())));

            localWriter.Flush();
        }

        internal void PopulateStrEntries(IList<StrEntry> strEntries)
        {
            foreach (SerializerEntries serializerEntry in _serializerEntries)
            {
                serializerEntry.PopulateStrEntries(strEntries);
            }
        }

        bool ShouldWrapException(Exception e)
        {
            return ((e is ArgumentException) || (e is FormatException) || (e is InvalidOperationException));
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

    }

}
