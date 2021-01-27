// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    using System.IdentityModel;
    using System.Runtime.CompilerServices;
    using Microsoft.Xml;

    [TypeForwardedFrom("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    internal sealed class EncryptedKey : EncryptedType
    {
        internal static readonly XmlDictionaryString CarriedKeyElementName = XD.XmlEncryptionDictionary.CarriedKeyName;
        internal static readonly XmlDictionaryString ElementName = XD.XmlEncryptionDictionary.EncryptedKey;
        internal static readonly XmlDictionaryString RecipientAttribute = XD.XmlEncryptionDictionary.Recipient;

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
            get { return ElementName; }
        }

        // TODO: find implementation
        public WSSecurityTokenSerializer SecurityTokenSerializer { get; internal set; }

        protected override void ForceEncryption()
        {
            // no work to be done here since, unlike bulk encryption, key wrapping is done eagerly
        }

        public byte[] GetWrappedKey()
        {
            if (this.State == EncryptionState.New)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.BadEncryptionState));
            }
            return _wrappedKey;
        }

        public void SetUpKeyWrap(byte[] wrappedKey)
        {
            if (this.State != EncryptionState.New)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.BadEncryptionState));
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
            _recipient = reader.GetAttribute(RecipientAttribute, null);
        }

        protected override void ReadAdditionalElements(XmlDictionaryReader reader)
        {
            if (reader.IsStartElement(ReferenceList.ElementName, EncryptedType.NamespaceUri))
            {
                _referenceList = new ReferenceList();
                _referenceList.ReadFrom(reader);
            }
            if (reader.IsStartElement(CarriedKeyElementName, EncryptedType.NamespaceUri))
            {
                reader.ReadStartElement(CarriedKeyElementName, EncryptedType.NamespaceUri);
                _carriedKeyName = reader.ReadString();
                reader.ReadEndElement();
            }
        }

        protected override void ReadCipherData(XmlDictionaryReader reader)
        {
            _wrappedKey = reader.ReadContentAsBase64();
        }

        protected override void ReadCipherData(XmlDictionaryReader reader, long maxBufferSize)
        {
            throw new NotImplementedException();
        }

        protected override void WriteAdditionalAttributes(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            if (_recipient != null)
            {
                writer.WriteAttributeString(RecipientAttribute, null, _recipient);
            }
        }

        protected override void WriteAdditionalElements(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            if (_carriedKeyName != null)
            {
                writer.WriteStartElement(CarriedKeyElementName, EncryptedType.NamespaceUri);
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
