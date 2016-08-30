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
            get { return this._carriedKeyName; }
            set { this._carriedKeyName = value; }
        }

        public string Recipient
        {
            get { return this._recipient; }
            set { this._recipient = value; }
        }

        public ReferenceList ReferenceList
        {
            get { return this._referenceList; }
            set { this._referenceList = value; }
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
            return this._wrappedKey;
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
            this._wrappedKey = wrappedKey;
            this.State = EncryptionState.Encrypted;
        }

        protected override void ReadAdditionalAttributes(XmlDictionaryReader reader)
        {
            this._recipient = reader.GetAttribute(s_RecipientAttribute, null);
        }

        protected override void ReadAdditionalElements(XmlDictionaryReader reader)
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

            //if (reader.IsStartElement(ReferenceList.ElementName, EncryptedType.NamespaceUri))
            //{
            //    this.referenceList = new ReferenceList();
            //    this.referenceList.ReadFrom(reader);
            //}
            //if (reader.IsStartElement(CarriedKeyElementName, EncryptedType.NamespaceUri))
            //{
            //    reader.ReadStartElement(CarriedKeyElementName, EncryptedType.NamespaceUri);
            //    this.carriedKeyName = reader.ReadString();
            //    reader.ReadEndElement();
            //}
        }

        protected override void ReadCipherData(XmlDictionaryReader reader)
        {
            this._wrappedKey = reader.ReadContentAsBase64();
        }

        protected override void ReadCipherData(XmlDictionaryReader reader, long maxBufferSize)
        {
            this._wrappedKey = SecurityUtils.ReadContentAsBase64(reader, maxBufferSize);
        }

        protected override void WriteAdditionalAttributes(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            if (this._recipient != null)
            {
                writer.WriteAttributeString(s_RecipientAttribute, null, this._recipient);
            }
        }

        protected override void WriteAdditionalElements(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            if (this._carriedKeyName != null)
            {
                writer.WriteStartElement(s_CarriedKeyElementName, EncryptedType.s_NamespaceUri);
                writer.WriteString(this._carriedKeyName);
                writer.WriteEndElement(); // CarriedKeyName
            }
            if (this._referenceList != null)
            {
                this._referenceList.WriteTo(writer, dictionaryManager);
            }
        }

        protected override void WriteCipherData(XmlDictionaryWriter writer)
        {
            writer.WriteBase64(this._wrappedKey, 0, this._wrappedKey.Length);
        }
    }
}
