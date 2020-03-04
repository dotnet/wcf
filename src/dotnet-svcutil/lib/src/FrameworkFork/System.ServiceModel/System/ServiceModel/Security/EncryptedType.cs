// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Security
{
    using System.IdentityModel;
    using System.Runtime.CompilerServices;
    using Microsoft.Xml;

    [TypeForwardedFrom("System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    sealed class EncryptedKey : EncryptedType
    {
        internal static readonly XmlDictionaryString CarriedKeyElementName = XD.XmlEncryptionDictionary.CarriedKeyName;
        internal static readonly XmlDictionaryString ElementName = XD.XmlEncryptionDictionary.EncryptedKey;
        internal static readonly XmlDictionaryString RecipientAttribute = XD.XmlEncryptionDictionary.Recipient;

        string carriedKeyName;
        string recipient;
        ReferenceList referenceList;
        byte[] wrappedKey;

        public string CarriedKeyName
        {
            get { return this.carriedKeyName; }
            set { this.carriedKeyName = value; }
        }

        public string Recipient
        {
            get { return this.recipient; }
            set { this.recipient = value; }
        }

        public ReferenceList ReferenceList
        {
            get { return this.referenceList; }
            set { this.referenceList = value; }
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.SRServiceModel.BadEncryptionState));
            }
            return this.wrappedKey;
        }

        public void SetUpKeyWrap(byte[] wrappedKey)
        {
            if (this.State != EncryptionState.New)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.SRServiceModel.BadEncryptionState));
            }
            if (wrappedKey == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("wrappedKey");
            }
            this.wrappedKey = wrappedKey;
            this.State = EncryptionState.Encrypted;
        }

        protected override void ReadAdditionalAttributes(XmlDictionaryReader reader)
        {
            this.recipient = reader.GetAttribute(RecipientAttribute, null);
        }

        protected override void ReadAdditionalElements(XmlDictionaryReader reader)
        {
            if (reader.IsStartElement(ReferenceList.ElementName, EncryptedType.NamespaceUri))
            {
                this.referenceList = new ReferenceList();
                this.referenceList.ReadFrom(reader);
            }
            if (reader.IsStartElement(CarriedKeyElementName, EncryptedType.NamespaceUri))
            {
                reader.ReadStartElement(CarriedKeyElementName, EncryptedType.NamespaceUri);
                this.carriedKeyName = reader.ReadString();
                reader.ReadEndElement();
            }
        }

        protected override void ReadCipherData(XmlDictionaryReader reader)
        {
            this.wrappedKey = reader.ReadContentAsBase64();
        }

        protected override void ReadCipherData(XmlDictionaryReader reader, long maxBufferSize)
        {
// Not needed in dotnet-svcutil scenario. 
//             this.wrappedKey = SecurityUtils.ReadContentAsBase64(reader, maxBufferSize);

            throw new NotImplementedException();
        }

        protected override void WriteAdditionalAttributes(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            if (this.recipient != null)
            {
                writer.WriteAttributeString(RecipientAttribute, null, this.recipient);
            }
        }

        protected override void WriteAdditionalElements(XmlDictionaryWriter writer, DictionaryManager dictionaryManager)
        {
            if (this.carriedKeyName != null)
            {
                writer.WriteStartElement(CarriedKeyElementName, EncryptedType.NamespaceUri);
                writer.WriteString(this.carriedKeyName);
                writer.WriteEndElement(); // CarriedKeyName
            }
            if (this.referenceList != null)
            {
                this.referenceList.WriteTo(writer, dictionaryManager);
            }
        }

        protected override void WriteCipherData(XmlDictionaryWriter writer)
        {
            writer.WriteBase64(this.wrappedKey, 0, this.wrappedKey.Length);
        }
    }
}
