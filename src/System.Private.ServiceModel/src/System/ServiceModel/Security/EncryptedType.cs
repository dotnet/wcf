// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Xml;
using DictionaryManager = System.IdentityModel.DictionaryManager;
using ISecurityElement = System.IdentityModel.ISecurityElement;

namespace System.ServiceModel.Security
{
    abstract class EncryptedType : ISecurityElement
    {
        internal static readonly XmlDictionaryString s_NamespaceUri = System.IdentityModel.XD.XmlEncryptionDictionary.Namespace;
        internal static readonly XmlDictionaryString s_EncodingAttribute = System.IdentityModel.XD.XmlEncryptionDictionary.Encoding;
        internal static readonly XmlDictionaryString s_MimeTypeAttribute = System.IdentityModel.XD.XmlEncryptionDictionary.MimeType;
        internal static readonly XmlDictionaryString s_TypeAttribute = System.IdentityModel.XD.XmlEncryptionDictionary.Type;
        internal static readonly XmlDictionaryString s_CipherDataElementName = System.IdentityModel.XD.XmlEncryptionDictionary.CipherData;
        internal static readonly XmlDictionaryString s_CipherValueElementName = System.IdentityModel.XD.XmlEncryptionDictionary.CipherValue;

        private string _encoding;
        private EncryptionMethodElement _encryptionMethod;
        private string _id;
        private string _wsuId;
        private SecurityKeyIdentifier _keyIdentifier;
        private string _mimeType;
        private EncryptionState _state;
        private string _type;
        private SecurityTokenSerializer _tokenSerializer;
        private bool _shouldReadXmlReferenceKeyInfoClause;

        protected EncryptedType()
        {
            this._encryptionMethod.Init();
            this._state = EncryptionState.New;
            this._tokenSerializer = new KeyInfoSerializer(false);
        }

        public string Encoding
        {
            get
            {
                return this._encoding;
            }
            set
            {
                this._encoding = value;
            }
        }

        public string EncryptionMethod
        {
            get
            {
                return this._encryptionMethod.algorithm;
            }
            set
            {
                this._encryptionMethod.algorithm = value;
            }
        }

        public XmlDictionaryString EncryptionMethodDictionaryString
        {
            get
            {
                return this._encryptionMethod.algorithmDictionaryString;
            }
            set
            {
                this._encryptionMethod.algorithmDictionaryString = value;
            }
        }

        public bool HasId
        {
            get
            {
                return true;
            }
        }

        public string Id
        {
            get
            {
                return this._id;
            }
            set
            {
                this._id = value;
            }
        }

        // This is set to true on the client side. And this means that when this knob is set to true and the default serializers on the client side fail 
        // to read the KeyInfo clause from the incoming response message from a service; then the ckient should 
        // try to read the keyInfo clause as GenericXmlSecurityKeyIdentifierClause before throwing.
        public bool ShouldReadXmlReferenceKeyInfoClause
        {
            get
            {
                return this._shouldReadXmlReferenceKeyInfoClause;
            }
            set
            {
                this._shouldReadXmlReferenceKeyInfoClause = value;
            }
        }

        public string WsuId
        {
            get
            {
                return this._wsuId;
            }
            set
            {
                this._wsuId = value;
            }
        }

        public SecurityKeyIdentifier KeyIdentifier
        {
            get
            {
                return this._keyIdentifier;
            }
            set
            {
                this._keyIdentifier = value;
            }
        }

        public string MimeType
        {
            get
            {
                return this._mimeType;
            }
            set
            {
                this._mimeType = value;
            }
        }

        public string Type
        {
            get
            {
                return this._type;
            }
            set
            {
                this._type = value;
            }
        }

        protected abstract XmlDictionaryString OpeningElementName
        {
            get;
        }

        protected EncryptionState State
        {
            get
            {
                return this._state;
            }
            set
            {
                this._state = value;
            }
        }

        public SecurityTokenSerializer SecurityTokenSerializer
        {
            get
            {
                return this._tokenSerializer;
            }
            set
            {
                this._tokenSerializer = value ?? new KeyInfoSerializer(false);
            }
        }

        protected abstract void ForceEncryption();

        protected virtual void ReadAdditionalAttributes(XmlDictionaryReader reader)
        {
        }

        protected virtual void ReadAdditionalElements(XmlDictionaryReader reader)
        {
        }

        protected abstract void ReadCipherData(XmlDictionaryReader reader);
        protected abstract void ReadCipherData(XmlDictionaryReader reader, long maxBufferSize);

        public void ReadFrom(XmlReader reader)
        {
            ReadFrom(reader, 0);
        }

        public void ReadFrom(XmlDictionaryReader reader)
        {
            ReadFrom(reader, 0);
        }

        public void ReadFrom(XmlReader reader, long maxBufferSize)
        {
            ReadFrom(XmlDictionaryReader.CreateDictionaryReader(reader), maxBufferSize);
        }

        public void ReadFrom(XmlDictionaryReader reader, long maxBufferSize)
        {
            ValidateReadState();
            reader.MoveToStartElement(OpeningElementName, s_NamespaceUri);
            this._encoding = reader.GetAttribute(s_EncodingAttribute, null);
            this._id = reader.GetAttribute(System.IdentityModel.XD.XmlEncryptionDictionary.Id, null) ?? SecurityUniqueId.Create().Value;
            this._wsuId = reader.GetAttribute(System.IdentityModel.XD.XmlEncryptionDictionary.Id, XD.UtilityDictionary.Namespace) ?? SecurityUniqueId.Create().Value;
            this._mimeType = reader.GetAttribute(s_MimeTypeAttribute, null);
            this._type = reader.GetAttribute(s_TypeAttribute, null);
            ReadAdditionalAttributes(reader);
            reader.Read();

            if (reader.IsStartElement(EncryptionMethodElement.ElementName, s_NamespaceUri))
            {
                this._encryptionMethod.ReadFrom(reader);
            }

            if (this._tokenSerializer.CanReadKeyIdentifier(reader))
            {
                // XmlElement xml = null;
                XmlDictionaryReader localReader;

                if (this.ShouldReadXmlReferenceKeyInfoClause)
                {
                    // We create the dom only when needed to not affect perf.
                    throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                    //XmlDocument doc = new XmlDocument();
                    //xml = (doc.ReadNode(reader) as XmlElement);
                    //localReader = XmlDictionaryReader.CreateDictionaryReader(new XmlNodeReader(xml));
                }
                else
                {
                    localReader = reader;
                }

                try
                {
                    this.KeyIdentifier = this._tokenSerializer.ReadKeyIdentifier(localReader);
                }
                catch (Exception e)
                {
                    // In case when the issued token ( custom token) is used as an initiator token; we will fail 
                    // to read the keyIdentifierClause using the plugged in default serializer. So We need to try to read it as an XmlReferencekeyIdentifierClause 
                    // if it is the client side.

                    if (Fx.IsFatal(e) || !this.ShouldReadXmlReferenceKeyInfoClause)
                    {
                        throw;
                    }

                    throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                    //this.keyIdentifier = ReadGenericXmlSecurityKeyIdentifier( XmlDictionaryReader.CreateDictionaryReader( new XmlNodeReader(xml)), e);
                }
            }

            reader.ReadStartElement(s_CipherDataElementName, EncryptedType.s_NamespaceUri);
            reader.ReadStartElement(s_CipherValueElementName, EncryptedType.s_NamespaceUri);
            if (maxBufferSize == 0)
                ReadCipherData(reader);
            else
                ReadCipherData(reader, maxBufferSize);
            reader.ReadEndElement(); // CipherValue
            reader.ReadEndElement(); // CipherData

            ReadAdditionalElements(reader);
            reader.ReadEndElement(); // OpeningElementName
            this.State = EncryptionState.Read;
        }

        private SecurityKeyIdentifier ReadGenericXmlSecurityKeyIdentifier(XmlDictionaryReader localReader, Exception previousException)
        {
            if (!localReader.IsStartElement(XD.XmlSignatureDictionary.KeyInfo, XD.XmlSignatureDictionary.Namespace))
            {
                return null;
            }

            localReader.ReadStartElement(XD.XmlSignatureDictionary.KeyInfo, XD.XmlSignatureDictionary.Namespace);
            SecurityKeyIdentifier keyIdentifier = new SecurityKeyIdentifier();
          
            if (localReader.IsStartElement())
            {
                SecurityKeyIdentifierClause clause = null;
                string strId = localReader.GetAttribute(XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace);
                XmlDocument doc = new XmlDocument();
                XmlElement keyIdentifierReferenceXml = (doc.ReadNode(localReader) as XmlElement);
                clause = new GenericXmlSecurityKeyIdentifierClause(keyIdentifierReferenceXml);
                if (!string.IsNullOrEmpty(strId))
                    clause.Id = strId;
                keyIdentifier.Add(clause);
            }

            if (keyIdentifier.Count == 0)
                throw previousException;

            localReader.ReadEndElement();
            return keyIdentifier;
        }

        protected virtual void WriteAdditionalAttributes(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
        }

        protected virtual void WriteAdditionalElements(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
        }

        protected abstract void WriteCipherData(XmlDictionaryWriter writer);

        public void WriteTo(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            if (writer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
            }
            ValidateWriteState();
            writer.WriteStartElement(XmlEncryptionStrings.Prefix, this.OpeningElementName, s_NamespaceUri);
            if (this._id != null && this._id.Length != 0)
            {
                writer.WriteAttributeString(System.IdentityModel.XD.XmlEncryptionDictionary.Id, null, this.Id);
            }
            if (this._type != null)
            {
                writer.WriteAttributeString(s_TypeAttribute, null, this.Type);
            }
            if (this._mimeType != null)
            {
                writer.WriteAttributeString(s_MimeTypeAttribute, null, this.MimeType);
            }
            if (this._encoding != null)
            {
                writer.WriteAttributeString(s_EncodingAttribute, null, this.Encoding);
            }
            WriteAdditionalAttributes(writer, dictionaryManager);
            if (this._encryptionMethod.algorithm != null)
            {
                this._encryptionMethod.WriteTo(writer);
            }
            if (this.KeyIdentifier != null)
            {
                this._tokenSerializer.WriteKeyIdentifier(writer, this.KeyIdentifier);
            }

            writer.WriteStartElement(s_CipherDataElementName, s_NamespaceUri);
            writer.WriteStartElement(s_CipherValueElementName, s_NamespaceUri);
            WriteCipherData(writer);
            writer.WriteEndElement(); // CipherValue
            writer.WriteEndElement(); // CipherData

            WriteAdditionalElements(writer, dictionaryManager);
            writer.WriteEndElement(); // OpeningElementName
        }

        void ValidateReadState()
        {
            if (this.State != EncryptionState.New)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityMessageSerializationException(SR.Format(SR.BadEncryptionState)));
            }
        }

        void ValidateWriteState()
        {
            if (this.State == EncryptionState.EncryptionSetup)
            {
                ForceEncryption();
            }
            else if (this.State == EncryptionState.New)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityMessageSerializationException(SR.Format(SR.BadEncryptionState)));
            }
        }

        protected enum EncryptionState
        {
            New,
            Read,
            DecryptionSetup,
            Decrypted,
            EncryptionSetup,
            Encrypted
        }
        
        struct EncryptionMethodElement
        {
            internal string algorithm;
            internal XmlDictionaryString algorithmDictionaryString;
            internal static readonly XmlDictionaryString ElementName = System.IdentityModel.XD.XmlEncryptionDictionary.EncryptionMethod;

            public void Init()
            {
                this.algorithm = null;
            }

            public void ReadFrom(XmlDictionaryReader reader)
            {
                reader.MoveToStartElement(ElementName, System.IdentityModel.XD.XmlEncryptionDictionary.Namespace);
                bool isEmptyElement = reader.IsEmptyElement;
                this.algorithm = reader.GetAttribute(XD.XmlSignatureDictionary.Algorithm, null);
                if (this.algorithm == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityMessageSerializationException(
                        SR.Format(SR.RequiredAttributeMissing, XD.XmlSignatureDictionary.Algorithm.Value, ElementName.Value)));
                }
                reader.Read();
                if (!isEmptyElement)
                {
                    while (reader.IsStartElement())
                    {
                        reader.Skip();
                    }
                    reader.ReadEndElement();
                }
            }

            public void WriteTo(XmlDictionaryWriter writer)
            {
                writer.WriteStartElement(XmlEncryptionStrings.Prefix, ElementName, System.IdentityModel.XD.XmlEncryptionDictionary.Namespace);
                if (this.algorithmDictionaryString != null)
                {
                    writer.WriteStartAttribute(XD.XmlSignatureDictionary.Algorithm, null);
                    writer.WriteString(this.algorithmDictionaryString);
                    writer.WriteEndAttribute();
                }
                else
                {
                    writer.WriteAttributeString(XD.XmlSignatureDictionary.Algorithm, null, this.algorithm);
                }
                if (this.algorithm == XD.SecurityAlgorithmDictionary.RsaOaepKeyWrap.Value)
                {
                    writer.WriteStartElement(XmlSignatureStrings.Prefix, XD.XmlSignatureDictionary.DigestMethod, XD.XmlSignatureDictionary.Namespace);
                    writer.WriteStartAttribute(XD.XmlSignatureDictionary.Algorithm, null);
                    writer.WriteString(XD.SecurityAlgorithmDictionary.Sha1Digest);
                    writer.WriteEndAttribute();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement(); // EncryptionMethod
            }
        }
    }
}

