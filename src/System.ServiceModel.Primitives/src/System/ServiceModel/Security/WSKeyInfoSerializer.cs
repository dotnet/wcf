// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
    internal class WSKeyInfoSerializer : KeyInfoSerializer
    {
        private static Func<KeyInfoSerializer, IEnumerable<SerializerEntries>> CreateAdditionalEntries(SecurityVersion securityVersion, SecureConversationVersion secureConversationVersion)
        {
            return (KeyInfoSerializer keyInfoSerializer) =>
            {
                List<SerializerEntries> serializerEntries = new List<SerializerEntries>();

                if (securityVersion == SecurityVersion.WSSecurity10)
                {
                    serializerEntries.Add(new IdentityModel.Tokens.WSSecurityJan2004(keyInfoSerializer));
                }
                else if (securityVersion == SecurityVersion.WSSecurity11)
                {
                    serializerEntries.Add(new IdentityModel.Tokens.WSSecurityXXX2005(keyInfoSerializer));
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(securityVersion), SRP.MessageSecurityVersionOutOfRange));
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

        public WSKeyInfoSerializer(bool emitBspRequiredAttributes, DictionaryManager dictionaryManager, IdentityModel.TrustDictionary trustDictionary, SecurityTokenSerializer innerSecurityTokenSerializer, SecurityVersion securityVersion, SecureConversationVersion secureConversationVersion)
            : base(emitBspRequiredAttributes, dictionaryManager, trustDictionary, innerSecurityTokenSerializer, CreateAdditionalEntries(securityVersion, secureConversationVersion))
        {
        }

        #region WSSecureConversation classes

        private abstract class WSSecureConversation : SerializerEntries
        {
            protected WSSecureConversation(KeyInfoSerializer securityTokenSerializer)
            {
                SecurityTokenSerializer = securityTokenSerializer;
            }

            public KeyInfoSerializer SecurityTokenSerializer { get; }

            public abstract IdentityModel.SecureConversationDictionary SerializerDictionary
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenEntryList));
                }
                tokenEntryList.Add(new DerivedKeyTokenEntry(this));
                tokenEntryList.Add(new SecurityContextTokenEntry(this));
            }

            protected abstract class SctStrEntry : StrEntry
            {
                public SctStrEntry(WSSecureConversation parent)
                {
                    Parent = parent;
                }

                protected WSSecureConversation Parent { get; }

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
                    if (tokenType != null && tokenType != Parent.SerializerDictionary.SecurityContextTokenType.Value)
                    {
                        return false;
                    }

                    if (reader.IsStartElement(
                        Parent.SecurityTokenSerializer.DictionaryManager.SecurityJan2004Dictionary.Reference,
                        Parent.SecurityTokenSerializer.DictionaryManager.SecurityJan2004Dictionary.Namespace))
                    {
                        string valueType = reader.GetAttribute(Parent.SecurityTokenSerializer.DictionaryManager.SecurityJan2004Dictionary.ValueType, null);
                        if (valueType != null && valueType != Parent.SerializerDictionary.SecurityContextTokenReferenceValueType.Value)
                        {
                            return false;
                        }

                        string uri = reader.GetAttribute(Parent.SecurityTokenSerializer.DictionaryManager.SecurityJan2004Dictionary.URI, null);
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
                    UniqueId uri = XmlHelper.GetAttributeAsUniqueId(reader, XD.SecurityJan2004Dictionary.URI, null);
                    UniqueId generation = ReadGeneration(reader);

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

                protected abstract UniqueId ReadGeneration(XmlDictionaryReader reader);

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
                    writer.WriteAttributeString(XD.SecurityJan2004Dictionary.ValueType, null, Parent.SerializerDictionary.SecurityContextTokenReferenceValueType.Value);
                    writer.WriteEndElement();
                }

                protected abstract void WriteGeneration(XmlDictionaryWriter writer, SecurityContextKeyIdentifierClause clause);
            }

            protected class SecurityContextTokenEntry : TokenEntry
            {
                private Type[] _tokenTypes;

                public SecurityContextTokenEntry(WSSecureConversation parent)
                {
                    Parent = parent;
                }

                protected WSSecureConversation Parent { get; }

                protected override XmlDictionaryString LocalName { get { return Parent.SerializerDictionary.SecurityContextToken; } }
                protected override XmlDictionaryString NamespaceUri { get { return Parent.SerializerDictionary.Namespace; } }
                protected override Type[] GetTokenTypesCore()
                {
                    if (_tokenTypes == null)
                    {
                        _tokenTypes = new Type[] { typeof(SecurityContextSecurityToken) };
                    }

                    return _tokenTypes;
                }
                public override string TokenTypeUri { get { return Parent.SerializerDictionary.SecurityContextTokenType.Value; } }
                protected override string ValueTypeUri { get { return null; } }
            }

            protected class DerivedKeyTokenEntry : TokenEntry
            {
                public const string DefaultLabel = "WS-SecureConversation";

                private WSSecureConversation _parent;
                private Type[] _tokenTypes;

                public DerivedKeyTokenEntry(WSSecureConversation parent)
                {
                    _parent = parent ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(parent));
                }

                protected override XmlDictionaryString LocalName { get { return _parent.SerializerDictionary.DerivedKeyToken; } }
                protected override XmlDictionaryString NamespaceUri { get { return _parent.SerializerDictionary.Namespace; } }
                protected override Type[] GetTokenTypesCore()
                {
                    if (_tokenTypes == null)
                    {
                        _tokenTypes = new Type[] { typeof(DerivedKeySecurityToken) };
                    }

                    return _tokenTypes;
                }

                public override string TokenTypeUri { get { return _parent.SerializerDictionary.DerivedKeyTokenType.Value; } }
                protected override string ValueTypeUri { get { return null; } }
            }
        }

        private class WSSecureConversationFeb2005 : WSSecureConversation
        {
            public WSSecureConversationFeb2005(KeyInfoSerializer securityTokenSerializer)
                : base(securityTokenSerializer)
            {
            }

            public override IdentityModel.SecureConversationDictionary SerializerDictionary
            {
                get { return SecurityTokenSerializer.DictionaryManager.SecureConversationFeb2005Dictionary; }
            }

            public override void PopulateStrEntries(IList<StrEntry> strEntries)
            {
                strEntries.Add(new SctStrEntryFeb2005(this));
            }

            private class SctStrEntryFeb2005 : SctStrEntry
            {
                public SctStrEntryFeb2005(WSSecureConversationFeb2005 parent)
                    : base(parent)
                {
                }

                protected override UniqueId ReadGeneration(XmlDictionaryReader reader)
                {
                    return XmlHelper.GetAttributeAsUniqueId(
                        reader,
                        Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary.Instance,
                        Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationFeb2005Dictionary.Namespace);
                }

                protected override void WriteGeneration(XmlDictionaryWriter writer, SecurityContextKeyIdentifierClause clause)
                {
                    // serialize the generation
                    if (clause.Generation != null)
                    {
                        XmlHelper.WriteAttributeStringAsUniqueId(
                            writer,
                            Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationFeb2005Dictionary.Prefix.Value,
                            Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary.Instance,
                            Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationFeb2005Dictionary.Namespace,
                            clause.Generation);
                    }
                }
            }
        }

        private class WSSecureConversationDec2005 : WSSecureConversation
        {
            public WSSecureConversationDec2005(KeyInfoSerializer securityTokenSerializer)
                : base(securityTokenSerializer)
            {
            }

            public override IdentityModel.SecureConversationDictionary SerializerDictionary
            {
                get { return SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary; }
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

            private class SctStrEntryDec2005 : SctStrEntry
            {
                public SctStrEntryDec2005(WSSecureConversationDec2005 parent)
                    : base(parent)
                {
                }

                protected override UniqueId ReadGeneration(XmlDictionaryReader reader)
                {
                    return XmlHelper.GetAttributeAsUniqueId(reader, Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary.Instance,
                        Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary.Namespace);
                }

                protected override void WriteGeneration(XmlDictionaryWriter writer, SecurityContextKeyIdentifierClause clause)
                {
                    // serialize the generation
                    if (clause.Generation != null)
                    {
                        XmlHelper.WriteAttributeStringAsUniqueId(
                            writer,
                            Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary.Prefix.Value,
                            Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary.Instance,
                            Parent.SecurityTokenSerializer.DictionaryManager.SecureConversationDec2005Dictionary.Namespace,
                            clause.Generation);
                    }
                }
            }
        }

        #endregion
    }
}
