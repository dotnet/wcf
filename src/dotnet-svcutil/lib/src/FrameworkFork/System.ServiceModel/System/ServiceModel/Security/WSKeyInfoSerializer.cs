// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using Microsoft.Xml;

namespace System.ServiceModel.Security
{
    internal class WSKeyInfoSerializer : KeyInfoSerializer
    {
        private static Func<KeyInfoSerializer, IEnumerable<SecurityTokenSerializer.SerializerEntries>> CreateAdditionalEntries(SecurityVersion securityVersion, SecureConversationVersion secureConversationVersion)
        {
            return (KeyInfoSerializer keyInfoSerializer) =>
            {
                List<SecurityTokenSerializer.SerializerEntries> serializerEntries = new List<SecurityTokenSerializer.SerializerEntries>();

                if (securityVersion == SecurityVersion.WSSecurity10)
                {
                    serializerEntries.Add(new System.IdentityModel.Tokens.WSSecurityJan2004(keyInfoSerializer));
                }
                else if (securityVersion == SecurityVersion.WSSecurity11)
                {
                    serializerEntries.Add(new System.IdentityModel.Tokens.WSSecurityXXX2005(keyInfoSerializer));
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("securityVersion", SRServiceModel.MessageSecurityVersionOutOfRange));
                }

                if (secureConversationVersion == SecureConversationVersion.WSSecureConversationFeb2005)
                {
                    serializerEntries.Add(new WSSecureConversationFeb2005(keyInfoSerializer));
                }
                else if (secureConversationVersion == SecureConversationVersion.WSSecureConversation13)
                {
                    serializerEntries.Add(new WSSecureConversationDec2005(keyInfoSerializer));
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
                }

                return serializerEntries;
            };
        }

        public WSKeyInfoSerializer(bool emitBspRequiredAttributes, DictionaryManager dictionaryManager, System.IdentityModel.TrustDictionary trustDictionary, SecurityTokenSerializer innerSecurityTokenSerializer, SecurityVersion securityVersion, SecureConversationVersion secureConversationVersion)
            : base(emitBspRequiredAttributes, dictionaryManager, trustDictionary, innerSecurityTokenSerializer, CreateAdditionalEntries(securityVersion, secureConversationVersion))
        {
        }

        #region WSSecureConversation classes
        internal abstract class WSSecureConversation : SecurityTokenSerializer.SerializerEntries
        {
            private KeyInfoSerializer _securityTokenSerializer;

            protected WSSecureConversation(KeyInfoSerializer securityTokenSerializer)
            {
                _securityTokenSerializer = securityTokenSerializer;
            }

            public KeyInfoSerializer SecurityTokenSerializer
            {
                get { return _securityTokenSerializer; }
            }

            public abstract System.IdentityModel.SecureConversationDictionary SerializerDictionary
            {
                get;
            }

            public virtual string DerivationAlgorithm
            {
                get { return SecurityAlgorithms.Psha1KeyDerivation; }
            }

            public override void PopulateTokenEntries(IList<TokenEntry> tokenEntryList)
            {
                if (tokenEntryList == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenEntryList");
                }
                tokenEntryList.Add(new DerivedKeyTokenEntry(this));
                tokenEntryList.Add(new SecurityContextTokenEntry(this));
            }

            internal protected abstract class SctStrEntry : StrEntry
            {
                private WSSecureConversation _parent;

                public SctStrEntry(WSSecureConversation parent)
                {
                    _parent = parent;
                }

                protected WSSecureConversation Parent
                {
                    get { return _parent; }
                }

                public override Type GetTokenType(SecurityKeyIdentifierClause clause)
                {
                    return null;
                }

                public override string GetTokenTypeUri()
                {
                    return null;
                }

                public override bool CanReadClause(XmlDictionaryReader reader, string tokenType)
                {
                    if (tokenType != null && tokenType != _parent.SerializerDictionary.SecurityContextTokenType.Value)
                    {
                        return false;
                    }
                    if (reader.IsStartElement(
                        _parent.SecurityTokenSerializer.DictionaryManager.SecurityJan2004Dictionary.Reference,
                        _parent.SecurityTokenSerializer.DictionaryManager.SecurityJan2004Dictionary.Namespace))
                    {
                        string valueType = reader.GetAttribute(_parent.SecurityTokenSerializer.DictionaryManager.SecurityJan2004Dictionary.ValueType, null);
                        if (valueType != null && valueType != _parent.SerializerDictionary.SecurityContextTokenReferenceValueType.Value)
                        {
                            return false;
                        }
                        string uri = reader.GetAttribute(_parent.SecurityTokenSerializer.DictionaryManager.SecurityJan2004Dictionary.URI, null);
                        if (uri != null)
                        {
                            if (uri.Length > 0 && uri[0] != '#')
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }

                public override SecurityKeyIdentifierClause ReadClause(XmlDictionaryReader reader, byte[] derivationNonce, int derivationLength, string tokenType)
                {
                    Microsoft.Xml.UniqueId uri = XmlHelper.GetAttributeAsUniqueId(reader, XD.SecurityJan2004Dictionary.URI, null);
                    Microsoft.Xml.UniqueId generation = ReadGeneration(reader);

                    if (reader.IsEmptyElement)
                    {
                        reader.Read();
                    }
                    else
                    {
                        reader.ReadStartElement();
                        while (reader.IsStartElement())
                        {
                            reader.Skip();
                        }
                        reader.ReadEndElement();
                    }

                    return new SecurityContextKeyIdentifierClause(uri, generation, derivationNonce, derivationLength);
                }

                protected abstract Microsoft.Xml.UniqueId ReadGeneration(XmlDictionaryReader reader);

                public override bool SupportsCore(SecurityKeyIdentifierClause clause)
                {
                    return clause is SecurityContextKeyIdentifierClause;
                }

                public override void WriteContent(XmlDictionaryWriter writer, SecurityKeyIdentifierClause clause)
                {
                    SecurityContextKeyIdentifierClause sctClause = clause as SecurityContextKeyIdentifierClause;
                    writer.WriteStartElement(XD.SecurityJan2004Dictionary.Prefix.Value, XD.SecurityJan2004Dictionary.Reference, XD.SecurityJan2004Dictionary.Namespace);
                    XmlHelper.WriteAttributeStringAsUniqueId(writer, null, XD.SecurityJan2004Dictionary.URI, null, sctClause.ContextId);
                    WriteGeneration(writer, sctClause);
                    writer.WriteAttributeString(XD.SecurityJan2004Dictionary.ValueType, null, _parent.SerializerDictionary.SecurityContextTokenReferenceValueType.Value);
                    writer.WriteEndElement();
                }

                protected abstract void WriteGeneration(XmlDictionaryWriter writer, SecurityContextKeyIdentifierClause clause);
            }

            internal protected class SecurityContextTokenEntry : SecurityTokenSerializer.TokenEntry
            {
                private WSSecureConversation _parent;
                private Type[] _tokenTypes;

                public SecurityContextTokenEntry(WSSecureConversation parent)
                {
                    _parent = parent;
                }

                protected WSSecureConversation Parent
                {
                    get { return _parent; }
                }

                protected override XmlDictionaryString LocalName { get { return _parent.SerializerDictionary.SecurityContextToken; } }
                protected override XmlDictionaryString NamespaceUri { get { return _parent.SerializerDictionary.Namespace; } }
                protected override Type[] GetTokenTypesCore()
                {
                    if (_tokenTypes == null)
                        _tokenTypes = new Type[] { typeof(SecurityContextSecurityToken) };

                    return _tokenTypes;
                }
                public override string TokenTypeUri { get { return _parent.SerializerDictionary.SecurityContextTokenType.Value; } }
                protected override string ValueTypeUri { get { return null; } }
            }

            internal protected class DerivedKeyTokenEntry : SecurityTokenSerializer.TokenEntry
            {
                public const string DefaultLabel = "WS-SecureConversation";

                private WSSecureConversation _parent;
                private Type[] _tokenTypes;

                public DerivedKeyTokenEntry(WSSecureConversation parent)
                {
                    if (parent == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parent");
                    }
                    _parent = parent;
                }

                protected override XmlDictionaryString LocalName { get { return _parent.SerializerDictionary.DerivedKeyToken; } }
                protected override XmlDictionaryString NamespaceUri { get { return _parent.SerializerDictionary.Namespace; } }
                protected override Type[] GetTokenTypesCore()
                {
                    if (_tokenTypes == null)
                        _tokenTypes = new Type[] { typeof(DerivedKeySecurityToken) };

                    return _tokenTypes;
                }

                public override string TokenTypeUri { get { return _parent.SerializerDictionary.DerivedKeyTokenType.Value; } }
                protected override string ValueTypeUri { get { return null; } }
            }
        }

        internal class WSSecureConversationFeb2005 : WSSecureConversation
        {
            public WSSecureConversationFeb2005(KeyInfoSerializer securityTokenSerializer)
                : base(securityTokenSerializer)
            {
            }

            public override System.IdentityModel.SecureConversationDictionary SerializerDictionary
            {
                get { return this.SecurityTokenSerializer.DictionaryManager.SecureConversationFeb2005Dictionary; }
            }

            public override void PopulateStrEntries(IList<StrEntry> strEntries)
            {
                strEntries.Add(new SctStrEntryFeb2005(this));
            }

            internal class SctStrEntryFeb2005 : SctStrEntry
            {
                public SctStrEntryFeb2005(WSSecureConversationFeb2005 parent)
                    : base(parent)
                {
                }

                protected override Microsoft.Xml.UniqueId ReadGeneration(XmlDictionaryReader reader)
                {
                    return XmlHelper.GetAttributeAsUniqueId(
                        reader,
                        this.Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary.Instance,
                        this.Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationFeb2005Dictionary.Namespace);
                }

                protected override void WriteGeneration(XmlDictionaryWriter writer, SecurityContextKeyIdentifierClause clause)
                {
                    // serialize the generation
                    if (clause.Generation != null)
                    {
                        XmlHelper.WriteAttributeStringAsUniqueId(
                            writer,
                            this.Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationFeb2005Dictionary.Prefix.Value,
                            this.Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary.Instance,
                            this.Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationFeb2005Dictionary.Namespace,
                            clause.Generation);
                    }
                }
            }
        }

        internal class WSSecureConversationDec2005 : WSSecureConversation
        {
            public WSSecureConversationDec2005(KeyInfoSerializer securityTokenSerializer)
                : base(securityTokenSerializer)
            {
            }

            public override System.IdentityModel.SecureConversationDictionary SerializerDictionary
            {
                get { return this.SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary; }
            }

            public override void PopulateStrEntries(IList<StrEntry> strEntries)
            {
                strEntries.Add(new SctStrEntryDec2005(this));
            }

            public override string DerivationAlgorithm
            {
                get
                {
                    return SecurityAlgorithms.Psha1KeyDerivationDec2005;
                }
            }

            internal class SctStrEntryDec2005 : SctStrEntry
            {
                public SctStrEntryDec2005(WSSecureConversationDec2005 parent)
                    : base(parent)
                {
                }

                protected override Microsoft.Xml.UniqueId ReadGeneration(XmlDictionaryReader reader)
                {
                    return XmlHelper.GetAttributeAsUniqueId(reader, this.Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary.Instance,
                        this.Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary.Namespace);
                }

                protected override void WriteGeneration(XmlDictionaryWriter writer, SecurityContextKeyIdentifierClause clause)
                {
                    // serialize the generation
                    if (clause.Generation != null)
                    {
                        XmlHelper.WriteAttributeStringAsUniqueId(
                            writer,
                            this.Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary.Prefix.Value,
                            this.Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary.Instance,
                            this.Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary.Namespace,
                            clause.Generation);
                    }
                }
            }
        }
        #endregion
    }
}
