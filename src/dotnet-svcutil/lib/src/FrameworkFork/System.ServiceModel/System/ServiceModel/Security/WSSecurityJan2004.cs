// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.Runtime;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security.Tokens;
    using System.Text;
    using Microsoft.Xml;
    //using HexBinary = System.Runtime.Remoting.Metadata.W3cXsd2001.SoapHexBinary;
    using KeyIdentifierClauseEntry = WSSecurityTokenSerializer.KeyIdentifierClauseEntry;
    using StrEntry = WSSecurityTokenSerializer.StrEntry;
    using TokenEntry = WSSecurityTokenSerializer.TokenEntry;

    internal class WSSecurityJan2004 : WSSecurityTokenSerializer.SerializerEntries
    {
        private WSSecurityTokenSerializer _tokenSerializer;
        private SamlSerializer _samlSerializer;

        public WSSecurityJan2004(WSSecurityTokenSerializer tokenSerializer, SamlSerializer samlSerializer)
        {
            _tokenSerializer = tokenSerializer;
            _samlSerializer = samlSerializer;
        }

        public WSSecurityTokenSerializer WSSecurityTokenSerializer
        {
            get { return _tokenSerializer; }
        }

        public SamlSerializer SamlSerializer
        {
            get { return _samlSerializer; }
        }

        protected void PopulateJan2004TokenEntries(IList<TokenEntry> tokenEntryList)
        {
            tokenEntryList.Add(new GenericXmlTokenEntry());
            tokenEntryList.Add(new UserNamePasswordTokenEntry(_tokenSerializer));
            tokenEntryList.Add(new KerberosTokenEntry(_tokenSerializer));
            tokenEntryList.Add(new X509TokenEntry(_tokenSerializer));
        }

        public override void PopulateTokenEntries(IList<TokenEntry> tokenEntryList)
        {
            PopulateJan2004TokenEntries(tokenEntryList);
            tokenEntryList.Add(new SamlTokenEntry(_tokenSerializer, _samlSerializer));
            tokenEntryList.Add(new WrappedKeyTokenEntry(_tokenSerializer));
        }

        internal abstract class BinaryTokenEntry : TokenEntry
        {
            internal static readonly XmlDictionaryString ElementName = XD.SecurityJan2004Dictionary.BinarySecurityToken;
            internal static readonly XmlDictionaryString EncodingTypeAttribute = XD.SecurityJan2004Dictionary.EncodingType;
            internal const string EncodingTypeAttributeString = SecurityJan2004Strings.EncodingType;
            internal const string EncodingTypeValueBase64Binary = SecurityJan2004Strings.EncodingTypeValueBase64Binary;
            internal const string EncodingTypeValueHexBinary = SecurityJan2004Strings.EncodingTypeValueHexBinary;
            internal static readonly XmlDictionaryString ValueTypeAttribute = XD.SecurityJan2004Dictionary.ValueType;

            private WSSecurityTokenSerializer _tokenSerializer;
            private string[] _valueTypeUris = null;

            protected BinaryTokenEntry(WSSecurityTokenSerializer tokenSerializer, string valueTypeUri)
            {
                _tokenSerializer = tokenSerializer;
                _valueTypeUris = new string[1];
                _valueTypeUris[0] = valueTypeUri;
            }

            protected BinaryTokenEntry(WSSecurityTokenSerializer tokenSerializer, string[] valueTypeUris)
            {
                if (valueTypeUris == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("valueTypeUris");

                _tokenSerializer = tokenSerializer;
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

            public abstract SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromBinaryCore(byte[] rawData);

            public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXmlCore(XmlElement issuedTokenXml,
                SecurityTokenReferenceStyle tokenReferenceStyle)
            {
                throw new NotImplementedException();
            }

            public abstract SecurityToken ReadBinaryCore(string id, string valueTypeUri, byte[] rawData);

            public override SecurityToken ReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver)
            {
                throw new NotImplementedException();
            }

            public abstract void WriteBinaryCore(SecurityToken token, out string id, out byte[] rawData);

            public override void WriteTokenCore(XmlDictionaryWriter writer, SecurityToken token)
            {
                string id;
                byte[] rawData;

                WriteBinaryCore(token, out id, out rawData);

                if (rawData == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("rawData");
                }

                writer.WriteStartElement(XD.SecurityJan2004Dictionary.Prefix.Value, ElementName, XD.SecurityJan2004Dictionary.Namespace);
                if (id != null)
                {
                    writer.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, id);
                }
                if (_valueTypeUris != null)
                {
                    writer.WriteAttributeString(ValueTypeAttribute, null, _valueTypeUris[0]);
                }
                if (_tokenSerializer.EmitBspRequiredAttributes)
                {
                    writer.WriteAttributeString(EncodingTypeAttribute, null, EncodingTypeValueBase64Binary);
                }
                writer.WriteBase64(rawData, 0, rawData.Length);
                writer.WriteEndElement(); // BinarySecurityToken
            }
        }

        private class GenericXmlTokenEntry : TokenEntry
        {
            protected override XmlDictionaryString LocalName { get { return null; } }
            protected override XmlDictionaryString NamespaceUri { get { return null; } }
            protected override Type[] GetTokenTypesCore() { return new Type[] { typeof(GenericXmlSecurityToken) }; }
            public override string TokenTypeUri { get { return null; } }
            protected override string ValueTypeUri { get { return null; } }

            public GenericXmlTokenEntry()
            {
            }


            public override bool CanReadTokenCore(XmlElement element)
            {
                return false;
            }

            public override bool CanReadTokenCore(XmlDictionaryReader reader)
            {
                return false;
            }

            public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXmlCore(XmlElement issuedTokenXml,
                SecurityTokenReferenceStyle tokenReferenceStyle)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }

            public override SecurityToken ReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }

            public override void WriteTokenCore(XmlDictionaryWriter writer, SecurityToken token)
            {
                throw new NotImplementedException();
            }
        }

        private class KerberosTokenEntry : BinaryTokenEntry
        {
            public KerberosTokenEntry(WSSecurityTokenSerializer tokenSerializer)
                : base(tokenSerializer, new string[] { SecurityJan2004Strings.KerberosTokenTypeGSS, SecurityJan2004Strings.KerberosTokenType1510 })
            {
            }

            protected override Type[] GetTokenTypesCore()
            {
                throw new NotImplementedException();
            }

            public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromBinaryCore(byte[] rawData)
            {
                throw new NotImplementedException();
            }

            public override SecurityToken ReadBinaryCore(string id, string valueTypeUri, byte[] rawData)
            {
                throw new NotImplementedException();
            }

            public override void WriteBinaryCore(SecurityToken token, out string id, out byte[] rawData)
            {
                throw new NotImplementedException();
            }
        }
        protected class SamlTokenEntry : TokenEntry
        {
            private const string samlAssertionId = "AssertionID";
            private SamlSerializer _samlSerializer;
            private SecurityTokenSerializer _tokenSerializer;

            public SamlTokenEntry(SecurityTokenSerializer tokenSerializer, SamlSerializer samlSerializer)
            {
                _tokenSerializer = tokenSerializer;
                if (samlSerializer != null)
                {
                    _samlSerializer = samlSerializer;
                }
                else
                {
                    _samlSerializer = new SamlSerializer();
                }
                _samlSerializer.PopulateDictionary(BinaryMessageEncoderFactory.XmlDictionary);
            }

            protected override XmlDictionaryString LocalName { get { return XD.SecurityJan2004Dictionary.SamlAssertion; } }
            protected override XmlDictionaryString NamespaceUri { get { return XD.SecurityJan2004Dictionary.SamlUri; } }
            protected override Type[] GetTokenTypesCore()
            {
                throw new NotImplementedException();
            }
            public override string TokenTypeUri { get { return null; } }
            protected override string ValueTypeUri { get { return null; } }

            public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXmlCore(XmlElement issuedTokenXml,
                SecurityTokenReferenceStyle tokenReferenceStyle)
            {
                throw new NotImplementedException();
            }

            public override SecurityToken ReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver)
            {
                throw new NotImplementedException();
            }

            public override void WriteTokenCore(XmlDictionaryWriter writer, SecurityToken token)
            {
                throw new NotImplementedException();
            }
        }

        private class UserNamePasswordTokenEntry : TokenEntry
        {
            private WSSecurityTokenSerializer _tokenSerializer;

            public UserNamePasswordTokenEntry(WSSecurityTokenSerializer tokenSerializer)
            {
                _tokenSerializer = tokenSerializer;
            }

            protected override XmlDictionaryString LocalName { get { return XD.SecurityJan2004Dictionary.UserNameTokenElement; } }
            protected override XmlDictionaryString NamespaceUri { get { return XD.SecurityJan2004Dictionary.Namespace; } }
            protected override Type[] GetTokenTypesCore() { return new Type[] { typeof(UserNameSecurityToken) }; }
            public override string TokenTypeUri { get { return SecurityJan2004Strings.UPTokenType; } }
            protected override string ValueTypeUri { get { return null; } }

            public override IAsyncResult BeginReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver, AsyncCallback callback, object state)
            {
                string id;
                string userName;
                string password;

                ParseToken(reader, out id, out userName, out password);

                SecurityToken token = new UserNameSecurityToken(userName, password, id);
                return new CompletedAsyncResult<SecurityToken>(token, callback, state);
            }

            public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXmlCore(XmlElement issuedTokenXml,
                SecurityTokenReferenceStyle tokenReferenceStyle)
            {
                TokenReferenceStyleHelper.Validate(tokenReferenceStyle);

                switch (tokenReferenceStyle)
                {
                    case SecurityTokenReferenceStyle.Internal:
                        return CreateDirectReference(issuedTokenXml, UtilityStrings.IdAttribute, UtilityStrings.Namespace, typeof(UserNameSecurityToken));
                    case SecurityTokenReferenceStyle.External:
                        // UP tokens aren't referred to externally
                        return null;
                    default:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("tokenReferenceStyle"));
                }
            }

            public override SecurityToken EndReadTokenCore(IAsyncResult result)
            {
                return CompletedAsyncResult<SecurityToken>.End(result);
            }

            public override SecurityToken ReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver)
            {
                string id;
                string userName;
                string password;

                ParseToken(reader, out id, out userName, out password);

                if (id == null)
                    id = SecurityUniqueId.Create().Value;

                return new UserNameSecurityToken(userName, password, id);
            }

            public override void WriteTokenCore(XmlDictionaryWriter writer, SecurityToken token)
            {
                UserNameSecurityToken upToken = (UserNameSecurityToken)token;
                WriteUserNamePassword(writer, upToken.Id, upToken.UserName, upToken.Password);
            }

            private void WriteUserNamePassword(XmlDictionaryWriter writer, string id, string userName, string password)
            {
                writer.WriteStartElement(XD.SecurityJan2004Dictionary.Prefix.Value, XD.SecurityJan2004Dictionary.UserNameTokenElement,
                    XD.SecurityJan2004Dictionary.Namespace); // <wsse:UsernameToken
                writer.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute,
                    XD.UtilityDictionary.Namespace, id); // wsu:Id="..."
                writer.WriteElementString(XD.SecurityJan2004Dictionary.Prefix.Value, XD.SecurityJan2004Dictionary.UserNameElement,
                    XD.SecurityJan2004Dictionary.Namespace, userName); // ><wsse:Username>...</wsse:Username>
                if (password != null)
                {
                    writer.WriteStartElement(XD.SecurityJan2004Dictionary.Prefix.Value, XD.SecurityJan2004Dictionary.PasswordElement,
                        XD.SecurityJan2004Dictionary.Namespace);
                    if (_tokenSerializer.EmitBspRequiredAttributes)
                    {
                        writer.WriteAttributeString(XD.SecurityJan2004Dictionary.TypeAttribute, null, SecurityJan2004Strings.UPTokenPasswordTextValue);
                    }
                    writer.WriteString(password); // <wsse:Password>...</wsse:Password>
                    writer.WriteEndElement();
                }
                writer.WriteEndElement(); // </wsse:UsernameToken>
            }

            private static string ParsePassword(XmlDictionaryReader reader)
            {
                string type = reader.GetAttribute(XD.SecurityJan2004Dictionary.TypeAttribute, null);
                if (type != null && type.Length > 0 && type != SecurityJan2004Strings.UPTokenPasswordTextValue)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(string.Format(SRServiceModel.UnsupportedPasswordType, type)));
                }

                return reader.ReadElementString();
            }

            private static void ParseToken(XmlDictionaryReader reader, out string id, out string userName, out string password)
            {
                id = null;
                userName = null;
                password = null;

                reader.MoveToContent();
                id = reader.GetAttribute(XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace);

                reader.ReadStartElement(XD.SecurityJan2004Dictionary.UserNameTokenElement, XD.SecurityJan2004Dictionary.Namespace);
                while (reader.IsStartElement())
                {
                    if (reader.IsStartElement(XD.SecurityJan2004Dictionary.UserNameElement, XD.SecurityJan2004Dictionary.Namespace))
                    {
                        userName = reader.ReadElementString();
                    }
                    else if (reader.IsStartElement(XD.SecurityJan2004Dictionary.PasswordElement, XD.SecurityJan2004Dictionary.Namespace))
                    {
                        password = ParsePassword(reader);
                    }
                    else if (reader.IsStartElement(XD.SecurityJan2004Dictionary.NonceElement, XD.SecurityJan2004Dictionary.Namespace))
                    {
                        // Nonce can be safely ignored
                        reader.Skip();
                    }
                    else if (reader.IsStartElement(XD.UtilityDictionary.CreatedElement, XD.UtilityDictionary.Namespace))
                    {
                        // wsu:Created can be safely ignored
                        reader.Skip();
                    }
                    else
                    {
                        XmlHelper.OnUnexpectedChildNodeError(SecurityJan2004Strings.UserNameTokenElement, reader);
                    }
                }
                reader.ReadEndElement();

                if (userName == null)
                    XmlHelper.OnRequiredElementMissing(SecurityJan2004Strings.UserNameElement, SecurityJan2004Strings.Namespace);
            }
        }

        protected class WrappedKeyTokenEntry : TokenEntry
        {
            private WSSecurityTokenSerializer _tokenSerializer;

            public WrappedKeyTokenEntry(WSSecurityTokenSerializer tokenSerializer)
            {
                _tokenSerializer = tokenSerializer;
            }

            protected override XmlDictionaryString LocalName { get { return EncryptedKey.ElementName; } }
            protected override XmlDictionaryString NamespaceUri { get { return XD.XmlEncryptionDictionary.Namespace; } }
            protected override Type[] GetTokenTypesCore() { return new Type[] { typeof(WrappedKeySecurityToken) }; }
            public override string TokenTypeUri { get { return null; } }
            protected override string ValueTypeUri { get { return null; } }

            public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXmlCore(XmlElement issuedTokenXml,
                SecurityTokenReferenceStyle tokenReferenceStyle)
            {
                TokenReferenceStyleHelper.Validate(tokenReferenceStyle);

                switch (tokenReferenceStyle)
                {
                    case SecurityTokenReferenceStyle.Internal:
                        return CreateDirectReference(issuedTokenXml, XmlEncryptionStrings.Id, null, null);
                    case SecurityTokenReferenceStyle.External:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(string.Format(SRServiceModel.CantInferReferenceForToken, EncryptedKey.ElementName.Value)));
                    default:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("tokenReferenceStyle"));
                }
            }

            public override SecurityToken ReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver)
            {
                EncryptedKey encryptedKey = new EncryptedKey();
                encryptedKey.SecurityTokenSerializer = _tokenSerializer;
                encryptedKey.ReadFrom(reader);
                SecurityKeyIdentifier unwrappingTokenIdentifier = encryptedKey.KeyIdentifier;
                byte[] wrappedKey = encryptedKey.GetWrappedKey();
                WrappedKeySecurityToken wrappedKeyToken = CreateWrappedKeyToken(encryptedKey.Id, encryptedKey.EncryptionMethod,
                    encryptedKey.CarriedKeyName, unwrappingTokenIdentifier, wrappedKey, tokenResolver);
                wrappedKeyToken.EncryptedKey = encryptedKey;

                return wrappedKeyToken;
            }

            private WrappedKeySecurityToken CreateWrappedKeyToken(string id, string encryptionMethod, string carriedKeyName,
                SecurityKeyIdentifier unwrappingTokenIdentifier, byte[] wrappedKey, SecurityTokenResolver tokenResolver)
            {
                throw new NotImplementedException();
            }

            public override void WriteTokenCore(XmlDictionaryWriter writer, SecurityToken token)
            {
                throw new NotImplementedException();
            }
        }

        protected class X509TokenEntry : BinaryTokenEntry
        {
            internal const string ValueTypeAbsoluteUri = SecurityJan2004Strings.X509TokenType;

            public X509TokenEntry(WSSecurityTokenSerializer tokenSerializer)
                : base(tokenSerializer, ValueTypeAbsoluteUri)
            {
            }

            protected override Type[] GetTokenTypesCore() { return new Type[] { typeof(X509SecurityToken), typeof(X509WindowsSecurityToken) }; }

            public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromBinaryCore(byte[] rawData)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(string.Format(SRServiceModel.CantInferReferenceForToken, ValueTypeAbsoluteUri)));
            }

            public override SecurityToken ReadBinaryCore(string id, string valueTypeUri, byte[] rawData)
            {
                X509Certificate2 certificate;
                if (!SecurityUtils.TryCreateX509CertificateFromRawData(rawData, out certificate))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(string.Format(SRServiceModel.InvalidX509RawData)));
                }
                return new X509SecurityToken(certificate, id, false);
            }

            public override void WriteBinaryCore(SecurityToken token, out string id, out byte[] rawData)
            {
                throw new NotImplementedException();
            }
        }

        public class IdManager : SignatureTargetIdManager
        {
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");

                if (reader.IsStartElement(EncryptedData.ElementName, XD.XmlEncryptionDictionary.Namespace))
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");

                writer.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, id);
            }
        }
    }
}
