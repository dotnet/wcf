// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using System.Security.Cryptography;
using System.Xml;

using DictionaryManager = System.IdentityModel.DictionaryManager;
using ISecurityElement = System.IdentityModel.ISecurityElement;

namespace System.ServiceModel.Security
{
    sealed class EncryptedHeaderXml
    {
        internal static readonly XmlDictionaryString s_ElementName = XD.SecurityXXX2005Dictionary.EncryptedHeader;
        internal static readonly XmlDictionaryString s_NamespaceUri = XD.SecurityXXX2005Dictionary.Namespace;
        const string Prefix = SecurityXXX2005Strings.Prefix;

        private string _id;
        private bool _mustUnderstand;
        private bool _relay;
        private string _actor;
        private MessageVersion _version;
        private EncryptedData _encryptedData;

        public EncryptedHeaderXml(MessageVersion version, bool shouldReadXmlReferenceKeyInfoClause)
        {
            _version = version;
            _encryptedData = new EncryptedData();
            
            // This is for the case when the service send an EncryptedHeader to the client where the KeyInfo clause contains referenceXml clause.
            _encryptedData.ShouldReadXmlReferenceKeyInfoClause = shouldReadXmlReferenceKeyInfoClause;
        }

        public string Actor
        {
            get
            {
                return _actor;
            }
            set
            {
                _actor = value;
            }
        }

        public string EncryptionMethod
        {
            get
            {
                return _encryptedData.EncryptionMethod;
            }
            set
            {
                _encryptedData.EncryptionMethod = value;
            }
        }

        public XmlDictionaryString EncryptionMethodDictionaryString
        {
            get
            {
                return _encryptedData.EncryptionMethodDictionaryString;
            }
            set
            {
                _encryptedData.EncryptionMethodDictionaryString = value;
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
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public SecurityKeyIdentifier KeyIdentifier
        {
            get
            {
                return _encryptedData.KeyIdentifier;
            }
            set
            {
                _encryptedData.KeyIdentifier = value;
            }
        }

        public bool MustUnderstand
        {
            get
            {
                return _mustUnderstand;
            }
            set
            {
                _mustUnderstand = value;
            }
        }

        public bool Relay
        {
            get
            {
                return _relay;
            }
            set
            {
                _relay = value;
            }
        }

        public SecurityTokenSerializer SecurityTokenSerializer
        {
            get
            {
                return _encryptedData.SecurityTokenSerializer;
            }
            set
            {
                _encryptedData.SecurityTokenSerializer = value;
            }
        }

        public byte[] GetDecryptedBuffer()
        {
            return _encryptedData.GetDecryptedBuffer();
        }

        public void ReadFrom(XmlDictionaryReader reader, long maxBufferSize)
        {
            reader.MoveToStartElement(s_ElementName, s_NamespaceUri);
            bool isReferenceParameter;
            MessageHeader.GetHeaderAttributes(reader, _version, out _actor, out _mustUnderstand, out _relay, out isReferenceParameter);
            _id = reader.GetAttribute(XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace);

            reader.ReadStartElement();
            _encryptedData.ReadFrom(reader, maxBufferSize);
            reader.ReadEndElement();
        }

        public void SetUpDecryption(SymmetricAlgorithm algorithm)
        {
            _encryptedData.SetUpDecryption(algorithm);
        }

        public void SetUpEncryption(SymmetricAlgorithm algorithm, MemoryStream source)
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
            //encryptedData.SetUpEncryption(algorithm, new ArraySegment<byte>(source.GetBuffer(), 0, (int) source.Length));
        }

        public void WriteHeaderElement(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement(Prefix, s_ElementName, s_NamespaceUri);
        }

        public void WriteHeaderId(XmlDictionaryWriter writer)
        {
            writer.WriteAttributeString(XD.UtilityDictionary.Prefix.Value, XD.UtilityDictionary.IdAttribute, XD.UtilityDictionary.Namespace, _id);
        }

        public void WriteHeaderContents(XmlDictionaryWriter writer)
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
            //this.encryptedData.WriteTo(writer, ServiceModelDictionaryManager.Instance);
        }
    }
}

