// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.ServiceModel.Security.Tokens;
using System.Xml;
using HexBinary = System.Runtime.Remoting.Metadata.W3cXsd2001.SoapHexBinary;
using IdentityModelXD = System.IdentityModel.XD;
using TokenEntry = System.ServiceModel.Security.WSSecurityTokenSerializer.TokenEntry;

namespace System.ServiceModel.Security
{
    internal class WSSecurityJan2004 : WSSecurityTokenSerializer.SerializerEntries
    {
        private SamlSerializer _samlSerializer;

        public WSSecurityJan2004(WSSecurityTokenSerializer tokenSerializer, SamlSerializer samlSerializer)
        {
            WSSecurityTokenSerializer = tokenSerializer;
            _samlSerializer = samlSerializer;
        }

        public WSSecurityTokenSerializer WSSecurityTokenSerializer { get; }

        public SamlSerializer SamlSerializer
        {
            get { return _samlSerializer; }
        }

        protected void PopulateJan2004TokenEntries(IList<TokenEntry> tokenEntryList)
        {
            tokenEntryList.Add(new GenericXmlTokenEntry());
            tokenEntryList.Add(new UserNamePasswordTokenEntry(WSSecurityTokenSerializer));
            //tokenEntryList.Add(new KerberosTokenEntry(WSSecurityTokenSerializer));
            tokenEntryList.Add(new X509TokenEntry(WSSecurityTokenSerializer));
        }

        public override void PopulateTokenEntries(IList<TokenEntry> tokenEntryList)
        {
            PopulateJan2004TokenEntries(tokenEntryList);
            //tokenEntryList.Add(new SamlTokenEntry(WSSecurityTokenSerializer, SamlSerializer));
            tokenEntryList.Add(new WrappedKeyTokenEntry(WSSecurityTokenSerializer));
        }

        internal abstract class BinaryTokenEntry : TokenEntry
        {
            internal const string EncodingTypeAttributeString = SecurityJan2004Strings.EncodingType;
            internal const string EncodingTypeValueBase64Binary = SecurityJan2004Strings.EncodingTypeValueBase64Binary;
            internal const string EncodingTypeValueHexBinary = SecurityJan2004Strings.EncodingTypeValueHexBinary;

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
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(valueTypeUris));
                }

                _tokenSerializer = tokenSerializer;
                _valueTypeUris = new string[valueTypeUris.GetLength(0)];
                for (int i = 0; i < _valueTypeUris.GetLength(0); ++i)
                {
                    _valueTypeUris[i] = valueTypeUris[i];
                }
            }

            protected override XmlDictionaryString LocalName => XD.SecurityJan2004Dictionary.BinarySecurityToken;
            protected override XmlDictionaryString NamespaceUri { get { return XD.SecurityJan2004Dictionary.Namespace; } }
            public override string TokenTypeUri { get { return _valueTypeUris[0]; } }
            protected override string ValueTypeUri { get { return _valueTypeUris[0]; } }
            public override bool SupportsTokenTypeUri(string tokenTypeUri)
            {
                for (int i = 0; i < _valueTypeUris.GetLength(0); ++i)
                {
                    if (_valueTypeUris[i] == tokenTypeUri)
                    {
                        return true;
                    }
                }

                return false;
            }

            public abstract SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromBinaryCore(byte[] rawData);

            public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXmlCore(XmlElement issuedTokenXml,
                SecurityTokenReferenceStyle tokenReferenceStyle)
            {
                TokenReferenceStyleHelper.Validate(tokenReferenceStyle);

                switch (tokenReferenceStyle)
                {
                    case SecurityTokenReferenceStyle.Internal:
                        return CreateDirectReference(issuedTokenXml, UtilityStrings.IdAttribute, UtilityStrings.Namespace, TokenType);
                    case SecurityTokenReferenceStyle.External:
                        string encoding = issuedTokenXml.GetAttribute(EncodingTypeAttributeString, null);
                        string encodedData = issuedTokenXml.InnerText;

                        byte[] binaryData;
                        if (encoding == null || encoding == EncodingTypeValueBase64Binary)
                        {
                            binaryData = Convert.FromBase64String(encodedData);
                        }
                        else if (encoding == EncodingTypeValueHexBinary)
                        {
                            binaryData = HexBinary.Parse(encodedData).Value;
                        }
                        else
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.UnknownEncodingInBinarySecurityToken));
                        }

                        return CreateKeyIdentifierClauseFromBinaryCore(binaryData);
                    default:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(tokenReferenceStyle)));
                }
            }

            public abstract SecurityToken ReadBinaryCore(string id, string valueTypeUri, byte[] rawData);

            public override SecurityToken ReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver)
            {
                string wsuId = reader.GetAttribute(XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace);
                string valueTypeUri = reader.GetAttribute(XD.SecurityJan2004Dictionary.ValueType, null);
                string encoding = reader.GetAttribute(XD.SecurityJan2004Dictionary.EncodingType, null);

                byte[] binaryData;
                if (encoding == null || encoding == EncodingTypeValueBase64Binary)
                {
                    binaryData = reader.ReadElementContentAsBase64();
                }
                else if (encoding == EncodingTypeValueHexBinary)
                {
                    binaryData = HexBinary.Parse(reader.ReadElementContentAsString()).Value;
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.UnknownEncodingInBinarySecurityToken));
                }

                return ReadBinaryCore(wsuId, valueTypeUri, binaryData);
            }

            public abstract void WriteBinaryCore(SecurityToken token, out string id, out byte[] rawData);

            public override void WriteTokenCore(XmlDictionaryWriter writer, SecurityToken token)
            {
                string id;
                byte[] rawData;

                WriteBinaryCore(token, out id, out rawData);

                if (rawData == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(rawData));
                }

                writer.WriteStartElement(XD.SecurityJan2004Dictionary.Prefix.Value, LocalName, XD.SecurityJan2004Dictionary.Namespace);
                if (id != null)
                {
                    writer.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, id);
                }
                if (_valueTypeUris != null)
                {
                    writer.WriteAttributeString(XD.SecurityJan2004Dictionary.ValueType, null, _valueTypeUris[0]);
                }
                if (_tokenSerializer.EmitBspRequiredAttributes)
                {
                    writer.WriteAttributeString(XD.SecurityJan2004Dictionary.EncodingType, null, EncodingTypeValueBase64Binary);
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
                BufferedGenericXmlSecurityToken bufferedXmlToken = token as BufferedGenericXmlSecurityToken;
                if (bufferedXmlToken != null && bufferedXmlToken.TokenXmlBuffer != null)
                {
                    using (XmlDictionaryReader reader = bufferedXmlToken.TokenXmlBuffer.GetReader(0))
                    {
                        writer.WriteNode(reader, false);
                    }
                }
                else
                {
                    GenericXmlSecurityToken xmlToken = (GenericXmlSecurityToken)token;
                    xmlToken.TokenXml.WriteTo(writer);
                }
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
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(tokenReferenceStyle)));
                }
            }

            public override SecurityToken ReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver)
            {
                string id;
                string userName;
                string password;

                ParseToken(reader, out id, out userName, out password);

                if (id == null)
                {
                    id = SecurityUniqueId.Create().Value;
                }

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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.UnsupportedPasswordType, type)));
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
                {
                    XmlHelper.OnRequiredElementMissing(SecurityJan2004Strings.UserNameElement, SecurityJan2004Strings.Namespace);
                }
            }
        }

        protected class WrappedKeyTokenEntry : TokenEntry
        {
            private WSSecurityTokenSerializer _tokenSerializer;

            public WrappedKeyTokenEntry(WSSecurityTokenSerializer tokenSerializer)
            {
                _tokenSerializer = tokenSerializer;
            }

            protected override XmlDictionaryString LocalName => IdentityModelXD.XmlEncryptionDictionary.EncryptedKey;
            protected override XmlDictionaryString NamespaceUri => IdentityModelXD.XmlEncryptionDictionary.Namespace;
            protected override Type[] GetTokenTypesCore() => [typeof(WrappedKeySecurityToken)];
            public override string TokenTypeUri => null;
            protected override string ValueTypeUri => null;

            public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromTokenXmlCore(XmlElement issuedTokenXml,
                SecurityTokenReferenceStyle tokenReferenceStyle)
            {
                TokenReferenceStyleHelper.Validate(tokenReferenceStyle);

                switch (tokenReferenceStyle)
                {
                    case SecurityTokenReferenceStyle.Internal:
                        return CreateDirectReference(issuedTokenXml, XmlEncryptionStrings.Id, null, null);
                    case SecurityTokenReferenceStyle.External:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SRP.Format(SRP.CantInferReferenceForToken, IdentityModelXD.XmlEncryptionDictionary.EncryptedKey.Value)));
                    default:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(tokenReferenceStyle)));
                }
            }

            public override SecurityToken ReadTokenCore(XmlDictionaryReader reader, SecurityTokenResolver tokenResolver)
            {
                // On .NET Framework, this code uses an internal implementation of EncryptedKey build on top of XmlReader/XmlWriter
                // To avoid owning implementations of security primitives such as EncryptedKey, we switched to using the
                // System.Security.Cryptography.Xml implementation. There's an impedence mismatch in the api surface, so some of the
                // code here doesn't exist on .NET Framework as it is only used to bridge the gap between the XmlReader and the
                // EncryptedKey implementation.
                // The main difference is that EncryptedKey exposes the SecurityKeyIdentifier as a KeyInfo object. If we want to
                // access the SecurityKeyIdentifiedClause instances, we need to read the KeyInfo as XML and use the token serializer
                // to parse it, which is what happens in CreateWrappedKeyToken.
                // We don't currently have a supported scenario for EncryptedKey which has a SecurityKeyIdentifier provided as this
                // class was ported to support Spnego tokens, which don't use the SecurityKeyIdentifier functionality of EncryptedKey.
                // The code here should be correct, but it's possible there might be some unknown issues with converting between the
                // KeyInfo and SecurityKeyIdentifier representations, so when porting any new cenarios depending on this, we should
                // be on the lookout for any issues that might arise from this.

                EncryptedKey encryptedKey = new EncryptedKey();
                var doc = new XmlDocument();
                var localReader = reader.ReadSubtree();
                XmlNode node = doc.ReadNode(localReader);
                localReader.Close();
                encryptedKey.LoadXml(node as XmlElement);
                if (encryptedKey.Id == null)
                {
                    encryptedKey.Id = SecurityUniqueId.Create().Value;
                }

                byte[] wrappedKey = encryptedKey.CipherData.CipherValue;
                WrappedKeySecurityToken wrappedKeyToken = CreateWrappedKeyToken(encryptedKey.Id, encryptedKey.EncryptionMethod.KeyAlgorithm,
                    encryptedKey.CarriedKeyName, encryptedKey.KeyInfo, wrappedKey, tokenResolver);
                wrappedKeyToken.EncryptedKey = encryptedKey;

                return wrappedKeyToken;
            }

            private WrappedKeySecurityToken CreateWrappedKeyToken(string id, string encryptionMethod, string carriedKeyName,
                KeyInfo keyInfoIdentifier, byte[] wrappedKey, SecurityTokenResolver tokenResolver)
            {
                ISspiNegotiationInfo sspiResolver = tokenResolver as ISspiNegotiationInfo;
                if (sspiResolver != null)
                {
                    ISspiNegotiation unwrappingSspiContext = sspiResolver.SspiNegotiation;
                    // ensure that the encryption algorithm is compatible
                    if (encryptionMethod != unwrappingSspiContext.KeyEncryptionAlgorithm)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.Format(SRP.BadKeyEncryptionAlgorithm, encryptionMethod)));
                    }
                    byte[] unwrappedKey = unwrappingSspiContext.Decrypt(wrappedKey);
                    return new WrappedKeySecurityToken(id, unwrappedKey, encryptionMethod, unwrappingSspiContext, unwrappedKey);
                }
                else
                {
                    if (tokenResolver == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(tokenResolver)));
                    }
                    if (keyInfoIdentifier == null || keyInfoIdentifier.Count == 0)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.MissingKeyInfoInEncryptedKey));
                    }

                    XmlReader keyInfoReader = new XmlNodeReader(keyInfoIdentifier.GetXml());
                    SecurityKeyIdentifier unwrappingTokenIdentifier = null;
                    if (_tokenSerializer.CanReadKeyIdentifier(keyInfoReader))
                    {
                        unwrappingTokenIdentifier = _tokenSerializer.ReadKeyIdentifier(keyInfoReader);
                    }
                    else
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.MissingKeyInfoInEncryptedKey));
                    }

                    SecurityToken unwrappingToken;
                    SecurityHeaderTokenResolver resolver = tokenResolver as SecurityHeaderTokenResolver;
                    if (resolver != null)
                    {
                        unwrappingToken = resolver.ExpectedWrapper;
                        if (unwrappingToken != null)
                        {
                            if (!resolver.CheckExternalWrapperMatch(unwrappingTokenIdentifier))
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(
                                    SRP.Format(SRP.EncryptedKeyWasNotEncryptedWithTheRequiredEncryptingToken, unwrappingToken)));
                            }
                        }
                        else
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(
                                SRP.Format(SRP.UnableToResolveKeyInfoForUnwrappingToken, unwrappingTokenIdentifier, resolver)));
                        }
                    }
                    else
                    {
                        try
                        {
                            unwrappingToken = tokenResolver.ResolveToken(unwrappingTokenIdentifier);
                        }
                        catch (Exception exception)
                        {
                            if (exception is MessageSecurityException)
                                throw;

                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(
                                SRP.Format(SRP.UnableToResolveKeyInfoForUnwrappingToken, unwrappingTokenIdentifier, tokenResolver), exception));
                        }
                    }
                    SecurityKey unwrappingSecurityKey;
                    byte[] unwrappedKey = SecurityUtils.DecryptKey(unwrappingToken, encryptionMethod, wrappedKey, out unwrappingSecurityKey);
                    return new WrappedKeySecurityToken(id, unwrappedKey, encryptionMethod, unwrappingToken, keyInfoIdentifier, wrappedKey, unwrappingSecurityKey);
                }
            }

            public override void WriteTokenCore(XmlDictionaryWriter writer, SecurityToken token)
            {
                WrappedKeySecurityToken wrappedKeyToken = token as WrappedKeySecurityToken;
                wrappedKeyToken.EnsureEncryptedKeySetUp();
                wrappedKeyToken.EncryptedKey.GetXml().WriteTo(writer);
            }
        }


        protected class X509TokenEntry : BinaryTokenEntry
        {
            internal const string ValueTypeAbsoluteUri = SecurityJan2004Strings.X509TokenType;

            public X509TokenEntry(WSSecurityTokenSerializer tokenSerializer)
                : base(tokenSerializer, ValueTypeAbsoluteUri)
            {
            }

            protected override Type[] GetTokenTypesCore() { return new Type[] { typeof(X509SecurityToken) }; }

            public override SecurityKeyIdentifierClause CreateKeyIdentifierClauseFromBinaryCore(byte[] rawData)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SRP.Format(SRP.CantInferReferenceForToken, ValueTypeAbsoluteUri)));
            }

            public override SecurityToken ReadBinaryCore(string id, string valueTypeUri, byte[] rawData)
            {
                X509Certificate2 certificate;
                if (!SecurityUtils.TryCreateX509CertificateFromRawData(rawData, out certificate))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.InvalidX509RawData));
                }
                return new X509SecurityToken(certificate, id, false);
            }

            public override void WriteBinaryCore(SecurityToken token, out string id, out byte[] rawData)
            {
                id = token.Id;
                X509SecurityToken x509Token = token as X509SecurityToken;
                if (x509Token != null)
                {
                    rawData = x509Token.Certificate.GetRawCertData();
                }
                else
                {
                    throw ExceptionHelper.PlatformNotSupported();
                }
            }
        }

        public class IdManager : SignatureTargetIdManager
        {
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

            internal static IdManager Instance { get; } = new IdManager();

            public override string ExtractId(XmlDictionaryReader reader)
            {
                if (reader == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(reader));
                }

                return reader.GetAttribute(XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace);
            }

            public override void WriteIdAttribute(XmlDictionaryWriter writer, string id)
            {
                if (writer == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(writer));
                }

                writer.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, id);
            }
        }
    }
}
