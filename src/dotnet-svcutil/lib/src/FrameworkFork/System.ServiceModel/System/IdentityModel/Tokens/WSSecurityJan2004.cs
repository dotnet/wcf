// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IdentityModel.Tokens
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IdentityModel;
    using System.IdentityModel.Selectors;
    using System.ServiceModel.Security;
    using System.ServiceModel.Security.Tokens;
    using System.Text;
    using Microsoft.Xml;
    //using HexBinary = System.Runtime.Remoting.Metadata.W3cXsd2001.SoapHexBinary;
    using KeyIdentifierClauseEntry = System.IdentityModel.Selectors.SecurityTokenSerializer.KeyIdentifierClauseEntry;
    using StrEntry = System.IdentityModel.Selectors.SecurityTokenSerializer.StrEntry;
    using TokenEntry = System.IdentityModel.Selectors.SecurityTokenSerializer.TokenEntry;

    internal class WSSecurityJan2004 : SecurityTokenSerializer.SerializerEntries
    {
        private KeyInfoSerializer _securityTokenSerializer;

        public WSSecurityJan2004(KeyInfoSerializer securityTokenSerializer)
        {
            _securityTokenSerializer = securityTokenSerializer;
        }

        public KeyInfoSerializer SecurityTokenSerializer
        {
            get { return _securityTokenSerializer; }
        }

        public override void PopulateKeyIdentifierClauseEntries(IList<KeyIdentifierClauseEntry> clauseEntries)
        {
            List<StrEntry> strEntries = new List<StrEntry>();
            _securityTokenSerializer.PopulateStrEntries(strEntries);
            SecurityTokenReferenceJan2004ClauseEntry strClause = new SecurityTokenReferenceJan2004ClauseEntry(_securityTokenSerializer.EmitBspRequiredAttributes, strEntries);
            clauseEntries.Add(strClause);
        }

        protected void PopulateJan2004StrEntries(IList<StrEntry> strEntries)
        {
            strEntries.Add(new LocalReferenceStrEntry(_securityTokenSerializer.EmitBspRequiredAttributes, _securityTokenSerializer));
            strEntries.Add(new KerberosHashStrEntry(_securityTokenSerializer.EmitBspRequiredAttributes));
            strEntries.Add(new X509SkiStrEntry(_securityTokenSerializer.EmitBspRequiredAttributes));
            strEntries.Add(new X509IssuerSerialStrEntry());
            strEntries.Add(new RelDirectStrEntry());
            strEntries.Add(new SamlJan2004KeyIdentifierStrEntry());
            strEntries.Add(new Saml2Jan2004KeyIdentifierStrEntry());
        }


        public override void PopulateStrEntries(IList<StrEntry> strEntries)
        {
            PopulateJan2004StrEntries(strEntries);
        }

        protected void PopulateJan2004TokenEntries(IList<TokenEntry> tokenEntryList)
        {
            tokenEntryList.Add(new GenericXmlTokenEntry());
            tokenEntryList.Add(new UserNamePasswordTokenEntry());
            tokenEntryList.Add(new KerberosTokenEntry());
            tokenEntryList.Add(new X509TokenEntry());
        }

        public override void PopulateTokenEntries(IList<TokenEntry> tokenEntryList)
        {
            PopulateJan2004TokenEntries(tokenEntryList);
            tokenEntryList.Add(new SamlTokenEntry());
            tokenEntryList.Add(new WrappedKeyTokenEntry());
        }

        internal abstract class BinaryTokenEntry : TokenEntry
        {
            internal static readonly XmlDictionaryString ElementName = XD.SecurityJan2004Dictionary.BinarySecurityToken;
            internal static readonly XmlDictionaryString EncodingTypeAttribute = XD.SecurityJan2004Dictionary.EncodingType;
            internal const string EncodingTypeAttributeString = SecurityJan2004Strings.EncodingType;
            internal const string EncodingTypeValueBase64Binary = SecurityJan2004Strings.EncodingTypeValueBase64Binary;
            internal const string EncodingTypeValueHexBinary = SecurityJan2004Strings.EncodingTypeValueHexBinary;
            internal static readonly XmlDictionaryString ValueTypeAttribute = XD.SecurityJan2004Dictionary.ValueType;

            private string[] _valueTypeUris = null;

            protected BinaryTokenEntry(string valueTypeUri)
            {
                _valueTypeUris = new string[1];
                _valueTypeUris[0] = valueTypeUri;
            }

            protected BinaryTokenEntry(string[] valueTypeUris)
            {
                if (valueTypeUris == null)
                    throw new ArgumentNullException("valueTypeUris"); // TODO:  DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("valueTypeUris");

                _valueTypeUris = new string[valueTypeUris.GetLength(0)];
                for (int i = 0; i < _valueTypeUris.GetLength(0); ++i)
                    _valueTypeUris[i] = valueTypeUris[i];
            }

            protected override XmlDictionaryString LocalName { get { return ElementName; } }
            protected override XmlDictionaryString NamespaceUri { get { return XD.SecurityJan2004Dictionary.Namespace; } }
            public override string TokenTypeUri { get { return _valueTypeUris[0]; } }
            protected override string ValueTypeUri { get { return _valueTypeUris[0]; } }
            public override bool SupportsTokenTypeUri(string tokenTypeUri)
            {
                for (int i = 0; i < _valueTypeUris.GetLength(0); ++i)
                {
                    if (_valueTypeUris[i] == tokenTypeUri)
                        return true;
                }

                return false;
            }
        }

        private class GenericXmlTokenEntry : TokenEntry
        {
            protected override XmlDictionaryString LocalName { get { return null; } }
            protected override XmlDictionaryString NamespaceUri { get { return null; } }
            protected override Type[] GetTokenTypesCore() { return new Type[] { typeof(GenericXmlSecurityToken) }; }
            public override string TokenTypeUri { get { return null; } }
            protected override string ValueTypeUri { get { return null; } }
        }

        private class KerberosTokenEntry : BinaryTokenEntry
        {
            public KerberosTokenEntry()
                : base(new string[] { SecurityJan2004Strings.KerberosTokenTypeGSS, SecurityJan2004Strings.KerberosTokenType1510 })
            {
            }

            protected override Type[] GetTokenTypesCore()
            {
                throw new NotImplementedException();
            }
        }

        protected class SamlTokenEntry : TokenEntry
        {
            protected override XmlDictionaryString LocalName { get { return XD.SecurityJan2004Dictionary.SamlAssertion; } }
            protected override XmlDictionaryString NamespaceUri { get { return XD.SecurityJan2004Dictionary.SamlUri; } }
            protected override Type[] GetTokenTypesCore() { return new Type[] { typeof(SamlSecurityToken) }; }
            public override string TokenTypeUri { get { return null; } }
            protected override string ValueTypeUri { get { return null; } }
        }

        private class UserNamePasswordTokenEntry : TokenEntry
        {
            protected override XmlDictionaryString LocalName { get { return XD.SecurityJan2004Dictionary.UserNameTokenElement; } }
            protected override XmlDictionaryString NamespaceUri { get { return XD.SecurityJan2004Dictionary.Namespace; } }
            protected override Type[] GetTokenTypesCore() { return new Type[] { typeof(UserNameSecurityToken) }; }
            public override string TokenTypeUri { get { return SecurityJan2004Strings.UPTokenType; } }
            protected override string ValueTypeUri { get { return null; } }
        }

        protected class WrappedKeyTokenEntry : TokenEntry
        {
            protected override XmlDictionaryString LocalName { get { return EncryptedKey.ElementName; } }
            protected override XmlDictionaryString NamespaceUri { get { return XD.XmlEncryptionDictionary.Namespace; } }
            protected override Type[] GetTokenTypesCore() { return new Type[] { typeof(WrappedKeySecurityToken) }; }
            public override string TokenTypeUri { get { return null; } }
            protected override string ValueTypeUri { get { return null; } }
        }

        protected class X509TokenEntry : BinaryTokenEntry
        {
            internal const string ValueTypeAbsoluteUri = SecurityJan2004Strings.X509TokenType;

            public X509TokenEntry()
                : base(ValueTypeAbsoluteUri)
            {
            }

            protected override Type[] GetTokenTypesCore() { return new Type[] { typeof(X509SecurityToken), typeof(X509WindowsSecurityToken) }; }
        }

        protected class SecurityTokenReferenceJan2004ClauseEntry : KeyIdentifierClauseEntry
        {
            private const int DefaultDerivedKeyLength = 32;

            private bool _emitBspRequiredAttributes;
            private IList<StrEntry> _strEntries;

            public SecurityTokenReferenceJan2004ClauseEntry(bool emitBspRequiredAttributes, IList<StrEntry> strEntries)
            {
                _emitBspRequiredAttributes = emitBspRequiredAttributes;
                _strEntries = strEntries;
            }
            protected bool EmitBspRequiredAttributes
            {
                get
                {
                    return _emitBspRequiredAttributes;
                }
            }
            protected IList<StrEntry> StrEntries
            {
                get
                {
                    return _strEntries;
                }
            }

            protected override XmlDictionaryString LocalName
            {
                get
                {
                    return XD.SecurityJan2004Dictionary.SecurityTokenReference;
                }
            }

            protected override XmlDictionaryString NamespaceUri
            {
                get
                {
                    return XD.SecurityJan2004Dictionary.Namespace;
                }
            }

            protected virtual string ReadTokenType(XmlDictionaryReader reader)
            {
                return null;
            }

            public override SecurityKeyIdentifierClause ReadKeyIdentifierClauseCore(XmlDictionaryReader reader)
            {
                throw new NotImplementedException();
            }

            public override bool SupportsCore(SecurityKeyIdentifierClause keyIdentifierClause)
            {
                for (int i = 0; i < _strEntries.Count; ++i)
                {
                    if (_strEntries[i].SupportsCore(keyIdentifierClause))
                    {
                        return true;
                    }
                }
                return false;
            }

            public override void WriteKeyIdentifierClauseCore(XmlDictionaryWriter writer, SecurityKeyIdentifierClause keyIdentifierClause)
            {
                throw new NotImplementedException();
            }
        }

        protected abstract class KeyIdentifierStrEntry : StrEntry
        {
            private bool _emitBspRequiredAttributes;

            protected const string EncodingTypeValueBase64Binary = SecurityJan2004Strings.EncodingTypeValueBase64Binary;
            protected const string EncodingTypeValueHexBinary = SecurityJan2004Strings.EncodingTypeValueHexBinary;
            protected const string EncodingTypeValueText = SecurityJan2004Strings.EncodingTypeValueText;

            protected abstract Type ClauseType { get; }
            protected virtual string DefaultEncodingType { get { return EncodingTypeValueBase64Binary; } }
            public abstract Type TokenType { get; }
            protected abstract string ValueTypeUri { get; }
            protected bool EmitBspRequiredAttributes { get { return _emitBspRequiredAttributes; } }

            protected KeyIdentifierStrEntry(bool emitBspRequiredAttributes)
            {
                _emitBspRequiredAttributes = emitBspRequiredAttributes;
            }

            public override bool CanReadClause(XmlDictionaryReader reader, string tokenType)
            {
                if (reader.IsStartElement(XD.SecurityJan2004Dictionary.KeyIdentifier, XD.SecurityJan2004Dictionary.Namespace))
                {
                    string valueType = reader.GetAttribute(XD.SecurityJan2004Dictionary.ValueType, null);
                    return (ValueTypeUri == valueType);
                }
                return false;
            }

            protected abstract SecurityKeyIdentifierClause CreateClause(byte[] bytes, byte[] derivationNonce, int derivationLength);

            public override Type GetTokenType(SecurityKeyIdentifierClause clause)
            {
                return this.TokenType;
            }

            public override SecurityKeyIdentifierClause ReadClause(XmlDictionaryReader reader, byte[] derivationNonce, int derivationLength, string tokenType)
            {
                throw new NotImplementedException();
            }

            public override bool SupportsCore(SecurityKeyIdentifierClause clause)
            {
                throw new NotImplementedException();
            }

            public override void WriteContent(XmlDictionaryWriter writer, SecurityKeyIdentifierClause clause)
            {
                writer.WriteStartElement(XD.SecurityJan2004Dictionary.Prefix.Value, XD.SecurityJan2004Dictionary.KeyIdentifier, XD.SecurityJan2004Dictionary.Namespace);
                writer.WriteAttributeString(XD.SecurityJan2004Dictionary.ValueType, null, ValueTypeUri);
                if (_emitBspRequiredAttributes)
                {
                    // Emit the encodingType attribute.
                    writer.WriteAttributeString(XD.SecurityJan2004Dictionary.EncodingType, null, DefaultEncodingType);
                }
                string encoding = DefaultEncodingType;
                BinaryKeyIdentifierClause binaryClause = clause as BinaryKeyIdentifierClause;

                byte[] keyIdentifier = binaryClause.GetBuffer();
                if (encoding == EncodingTypeValueBase64Binary)
                {
                    writer.WriteBase64(keyIdentifier, 0, keyIdentifier.Length);
                }
                else if (encoding == EncodingTypeValueHexBinary)
                {
                    writer.WriteBinHex(keyIdentifier, 0, keyIdentifier.Length);
                }
                else if (encoding == EncodingTypeValueText)
                {
                    writer.WriteString(new UTF8Encoding().GetString(keyIdentifier, 0, keyIdentifier.Length));
                }
                writer.WriteEndElement();
            }
        }

        protected class KerberosHashStrEntry : KeyIdentifierStrEntry
        {
            protected override Type ClauseType { get { throw new NotImplementedException(); } }

            public override Type TokenType { get { return typeof(KerberosRequestorSecurityToken); } }
            protected override string ValueTypeUri { get { return SecurityJan2004Strings.KerberosHashValueType; } }

            public KerberosHashStrEntry(bool emitBspRequiredAttributes)
                : base(emitBspRequiredAttributes)
            {
            }

            protected override SecurityKeyIdentifierClause CreateClause(byte[] bytes, byte[] derivationNonce, int derivationLength)
            {
                throw new NotImplementedException();
            }

            public override string GetTokenTypeUri()
            {
                return XD.SecurityJan2004Dictionary.KerberosTokenTypeGSS.Value;
            }
            public override void WriteContent(XmlDictionaryWriter writer, SecurityKeyIdentifierClause clause)
            {
                throw new NotImplementedException();
            }
        }

        protected class X509SkiStrEntry : KeyIdentifierStrEntry
        {
            protected override Type ClauseType { get { throw new NotImplementedException(); } }

            public override Type TokenType { get { return typeof(X509SecurityToken); } }
            protected override string ValueTypeUri { get { return SecurityJan2004Strings.X509SKIValueType; } }

            public X509SkiStrEntry(bool emitBspRequiredAttributes)
                : base(emitBspRequiredAttributes)
            {
            }

            protected override SecurityKeyIdentifierClause CreateClause(byte[] bytes, byte[] derivationNonce, int derivationLength)
            {
                throw new NotImplementedException();
            }
            public override string GetTokenTypeUri()
            {
                return SecurityJan2004Strings.X509TokenType;
            }
        }

        protected class LocalReferenceStrEntry : StrEntry
        {
            private bool _emitBspRequiredAttributes;
            private KeyInfoSerializer _tokenSerializer;

            public LocalReferenceStrEntry(bool emitBspRequiredAttributes, KeyInfoSerializer tokenSerializer)
            {
                _emitBspRequiredAttributes = emitBspRequiredAttributes;
                _tokenSerializer = tokenSerializer;
            }

            public override Type GetTokenType(SecurityKeyIdentifierClause clause)
            {
                LocalIdKeyIdentifierClause localClause = clause as LocalIdKeyIdentifierClause;
                return localClause.OwnerType;
            }

            public string GetLocalTokenTypeUri(SecurityKeyIdentifierClause clause)
            {
                Type tokenType = GetTokenType(clause);
                return _tokenSerializer.GetTokenTypeUri(tokenType);
            }
            public override string GetTokenTypeUri()
            {
                return null;
            }

            public override bool CanReadClause(XmlDictionaryReader reader, string tokenType)
            {
                if (reader.IsStartElement(XD.SecurityJan2004Dictionary.Reference, XD.SecurityJan2004Dictionary.Namespace))
                {
                    string uri = reader.GetAttribute(XD.SecurityJan2004Dictionary.URI, null);
                    if (uri != null && uri.Length > 0 && uri[0] == '#')
                    {
                        return true;
                    }
                }
                return false;
            }

            public override SecurityKeyIdentifierClause ReadClause(XmlDictionaryReader reader, byte[] derivationNonce, int derivationLength, string tokenType)
            {
                string uri = reader.GetAttribute(XD.SecurityJan2004Dictionary.URI, null);
                string tokenTypeUri = reader.GetAttribute(XD.SecurityJan2004Dictionary.ValueType, null);
                Type[] tokenTypes = null;
                if (tokenTypeUri != null)
                {
                    tokenTypes = _tokenSerializer.GetTokenTypes(tokenTypeUri);
                }
                SecurityKeyIdentifierClause clause = new LocalIdKeyIdentifierClause(uri.Substring(1), derivationNonce, derivationLength, tokenTypes);
                if (reader.IsEmptyElement)
                {
                    reader.Read();
                }
                else
                {
                    reader.ReadStartElement();
                    reader.ReadEndElement();
                }
                return clause;
            }

            public override bool SupportsCore(SecurityKeyIdentifierClause clause)
            {
                return clause is LocalIdKeyIdentifierClause;
            }

            public override void WriteContent(XmlDictionaryWriter writer, SecurityKeyIdentifierClause clause)
            {
                LocalIdKeyIdentifierClause localIdClause = clause as LocalIdKeyIdentifierClause;
                writer.WriteStartElement(XD.SecurityJan2004Dictionary.Prefix.Value, XD.SecurityJan2004Dictionary.Reference, XD.SecurityJan2004Dictionary.Namespace);
                if (_emitBspRequiredAttributes)
                {
                    string tokenTypeUri = GetLocalTokenTypeUri(localIdClause);
                    if (tokenTypeUri != null)
                    {
                        writer.WriteAttributeString(XD.SecurityJan2004Dictionary.ValueType, null, tokenTypeUri);
                    }
                }
                writer.WriteAttributeString(XD.SecurityJan2004Dictionary.URI, null, "#" + localIdClause.LocalId);
                writer.WriteEndElement();
            }
        }

        protected class SamlJan2004KeyIdentifierStrEntry : StrEntry
        {
            protected virtual bool IsMatchingValueType(string valueType)
            {
                return (valueType == SecurityJan2004Strings.SamlAssertionIdValueType);
            }

            public override bool CanReadClause(XmlDictionaryReader reader, string tokenType)
            {
                if (reader.IsStartElement(XD.SamlDictionary.AuthorityBinding, XD.SecurityJan2004Dictionary.SamlUri))
                {
                    return true;
                }
                else if (reader.IsStartElement(XD.SecurityJan2004Dictionary.KeyIdentifier, XD.SecurityJan2004Dictionary.Namespace))
                {
                    string valueType = reader.GetAttribute(XD.SecurityJan2004Dictionary.ValueType, null);
                    return IsMatchingValueType(valueType);
                }
                return false;
            }

            public override Type GetTokenType(SecurityKeyIdentifierClause clause)
            {
                return typeof(SamlSecurityToken);
            }

            public override string GetTokenTypeUri()
            {
                return XD.SecurityXXX2005Dictionary.SamlTokenType.Value;
            }

            public override SecurityKeyIdentifierClause ReadClause(XmlDictionaryReader reader, byte[] derivationNone, int derivationLength, string tokenType)
            {
                throw new NotImplementedException();
            }

            public override bool SupportsCore(SecurityKeyIdentifierClause clause)
            {
                throw new NotImplementedException();
            }

            public override void WriteContent(XmlDictionaryWriter writer, SecurityKeyIdentifierClause clause)
            {
                throw new NotImplementedException();
            }
        }

        private class Saml2Jan2004KeyIdentifierStrEntry : SamlJan2004KeyIdentifierStrEntry
        {
            // handles SAML2.0
            protected override bool IsMatchingValueType(string valueType)
            {
                return (valueType == XD.SecurityXXX2005Dictionary.Saml11AssertionValueType.Value);
            }

            public override string GetTokenTypeUri()
            {
                return XD.SecurityXXX2005Dictionary.Saml20TokenType.Value;
            }
        }

        protected class RelDirectStrEntry : StrEntry
        {
            public override bool CanReadClause(XmlDictionaryReader reader, string tokenType)
            {
                if (reader.IsStartElement(XD.SecurityJan2004Dictionary.Reference, XD.SecurityJan2004Dictionary.Namespace))
                {
                    string valueType = reader.GetAttribute(XD.SecurityJan2004Dictionary.ValueType, null);
                    return (valueType == SecurityJan2004Strings.RelAssertionValueType);
                }
                return false;
            }

            public override Type GetTokenType(SecurityKeyIdentifierClause clause)
            {
                return null;
            }
            public override string GetTokenTypeUri()
            {
                return XD.SecurityJan2004Dictionary.RelAssertionValueType.Value;
            }

            public override SecurityKeyIdentifierClause ReadClause(XmlDictionaryReader reader, byte[] derivationNone, int derivationLength, string tokenType)
            {
                throw new NotImplementedException();
            }

            public override bool SupportsCore(SecurityKeyIdentifierClause clause)
            {
                throw new NotImplementedException();
            }

            public override void WriteContent(XmlDictionaryWriter writer, SecurityKeyIdentifierClause clause)
            {
                throw new NotImplementedException();
            }
        }

        protected class X509IssuerSerialStrEntry : StrEntry
        {
            public override Type GetTokenType(SecurityKeyIdentifierClause clause)
            {
                return typeof(X509SecurityToken);
            }

            public override bool CanReadClause(XmlDictionaryReader reader, string tokenType)
            {
                return reader.IsStartElement(XD.XmlSignatureDictionary.X509Data, XD.XmlSignatureDictionary.Namespace);
            }
            public override string GetTokenTypeUri()
            {
                return SecurityJan2004Strings.X509TokenType;
            }

            public override SecurityKeyIdentifierClause ReadClause(XmlDictionaryReader reader, byte[] derivationNonce, int derivationLength, string tokenType)
            {
                throw new NotImplementedException();
            }

            public override bool SupportsCore(SecurityKeyIdentifierClause clause)
            {
                throw new NotImplementedException();
            }

            public override void WriteContent(XmlDictionaryWriter writer, SecurityKeyIdentifierClause clause)
            {
                throw new NotImplementedException();
            }
        }

        public class IdManager : SignatureTargetIdManager
        {
            internal static readonly XmlDictionaryString ElementName = XD.XmlEncryptionDictionary.EncryptedData;

            private static readonly IdManager s_instance = new IdManager();

            private IdManager()
            {
            }

            public override string DefaultIdNamespacePrefix
            {
                get { return UtilityStrings.Prefix; }
            }

            public override string DefaultIdNamespaceUri
            {
                get { return UtilityStrings.Namespace; }
            }

            internal static IdManager Instance
            {
                get { return s_instance; }
            }

            public override string ExtractId(XmlDictionaryReader reader)
            {
                if (reader == null)
                    throw new ArgumentNullException("reader");

                if (reader.IsStartElement(ElementName, XD.XmlEncryptionDictionary.Namespace))
                {
                    return reader.GetAttribute(XD.XmlEncryptionDictionary.Id, null);
                }
                else
                {
                    return reader.GetAttribute(XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace);
                }
            }

            public override void WriteIdAttribute(XmlDictionaryWriter writer, string id)
            {
                if (writer == null)
                    throw new ArgumentNullException("writer");


                writer.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, id);
            }
        }
    }
}
