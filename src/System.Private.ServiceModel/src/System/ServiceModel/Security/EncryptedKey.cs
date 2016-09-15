// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel;
using System.Runtime.CompilerServices;
using System.Xml;

namespace System.ServiceModel.Security
{
    sealed class EncryptedKey : EncryptedType
    {
        internal static readonly XmlDictionaryString s_CarriedKeyElementName = System.IdentityModel.XD.XmlEncryptionDictionary.CarriedKeyName;
        internal static readonly XmlDictionaryString s_ElementName = System.IdentityModel.XD.XmlEncryptionDictionary.EncryptedKey;
        internal static readonly XmlDictionaryString s_RecipientAttribute = System.IdentityModel.XD.XmlEncryptionDictionary.Recipient;

        private string _carriedKeyName;
        private string _recipient;
        private ReferenceList _referenceList;
        private byte[] _wrappedKey;

        public string CarriedKeyName
        {
            get { return _carriedKeyName; }
            set { _carriedKeyName = value; }
        }

        public string Recipient
        {
            get { return _recipient; }
            set { _recipient = value; }
        }

        public ReferenceList ReferenceList
        {
            get { return _referenceList; }
            set { _referenceList = value; }
        }

        protected override XmlDictionaryString OpeningElementName
        {
            get { return s_ElementName; }
        }

        protected override void ForceEncryption()
        {
            // no work to be done here since, unlike bulk encryption, key wrapping is done eagerly
        }

        public byte[] GetWrappedKey()
        {
            if (this.State == EncryptionState.New)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.BadEncryptionState)));
            }
            return _wrappedKey;
        }

        public void SetUpKeyWrap(byte[] wrappedKey)
        {
            if (this.State != EncryptionState.New)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.BadEncryptionState)));
            }
            if (wrappedKey == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("wrappedKey");
            }
            _wrappedKey = wrappedKey;
            this.State = EncryptionState.Encrypted;
        }

        protected override void ReadAdditionalAttributes(XmlDictionaryReader reader)
        {
            _recipient = reader.GetAttribute(s_RecipientAttribute, null);
        }

        protected override void ReadAdditionalElements(XmlDictionaryReader reader)
        {
            if (reader.IsStartElement(ReferenceList.s_ElementName, EncryptedType.s_NamespaceUri))
            {
                _referenceList = new ReferenceList();
                _referenceList.ReadFrom(reader);
            }
            if (reader.IsStartElement(s_CarriedKeyElementName, EncryptedType.s_NamespaceUri))
            {
                reader.ReadStartElement(s_CarriedKeyElementName, EncryptedType.s_NamespaceUri);
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress (NET Standard 2.0: needs ReadString() API)
                //_carriedKeyName = reader.ReadString();
                //reader.ReadEndElement();
            }
        }

        protected override void ReadCipherData(XmlDictionaryReader reader)
        {
            _wrappedKey = reader.ReadContentAsBase64();
        }

        protected override void ReadCipherData(XmlDictionaryReader reader, long maxBufferSize)
        {
            _wrappedKey = SecurityUtils.ReadContentAsBase64(reader, maxBufferSize);
        }

        protected override void WriteAdditionalAttributes(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            if (_recipient != null)
            {
                writer.WriteAttributeString(s_RecipientAttribute, null, _recipient);
            }
        }

        protected override void WriteAdditionalElements(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            if (_carriedKeyName != null)
            {
                writer.WriteStartElement(s_CarriedKeyElementName, EncryptedType.s_NamespaceUri);
                writer.WriteString(_carriedKeyName);
                writer.WriteEndElement(); // CarriedKeyName
            }
            if (_referenceList != null)
            {
                _referenceList.WriteTo(writer, dictionaryManager);
            }
        }

        protected override void WriteCipherData(XmlDictionaryWriter writer)
        {
            writer.WriteBase64(_wrappedKey, 0, _wrappedKey.Length);
        }
    }
}
