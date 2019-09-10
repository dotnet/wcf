//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.ServiceModel.Security
{
    using System.Security.Cryptography;
    using System.ServiceModel.Channels;
    using Microsoft.Xml;

    class EncryptedData : EncryptedType
    {
        internal static readonly XmlDictionaryString ElementName = XD.XmlEncryptionDictionary.EncryptedData;
        internal static readonly string ElementType = XmlEncryptionStrings.ElementType;
        internal static readonly string ContentType = XmlEncryptionStrings.ContentType;
        SymmetricAlgorithm algorithm;
        byte[] decryptedBuffer;
        ArraySegment<byte> buffer;
        byte[] iv;
        byte[] cipherText;

        protected override XmlDictionaryString OpeningElementName
        {
            get { return ElementName; }
        }

        void EnsureDecryptionSet()
        {
            if (this.State == EncryptionState.DecryptionSetup)
            {
                SetPlainText();
            }
            else if (this.State != EncryptionState.Decrypted)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRServiceModel.BadEncryptionState));
            }
        }

        protected override void ForceEncryption()
        {
#if disabled
            CryptoHelper.GenerateIVAndEncrypt(this.algorithm, this.buffer, out this.iv, out this.cipherText);
            this.State = EncryptionState.Encrypted;
            this.buffer = new ArraySegment<byte>(CryptoHelper.EmptyBuffer);
#else
            throw new NotImplementedException();
#endif
        }
        public byte[] GetDecryptedBuffer()
        {
            EnsureDecryptionSet();
            return this.decryptedBuffer;
        }

        protected override void ReadCipherData(XmlDictionaryReader reader)
        {
            this.cipherText = reader.ReadContentAsBase64();
        }

        protected override void ReadCipherData(XmlDictionaryReader reader, long maxBufferSize)
        {
            this.cipherText = SecurityUtils.ReadContentAsBase64(reader, maxBufferSize);
        }

        void SetPlainText()
        {
#if disabled
            this.decryptedBuffer = CryptoHelper.ExtractIVAndDecrypt(this.algorithm, this.cipherText, 0, this.cipherText.Length);
            this.State = EncryptionState.Decrypted;
#else
            throw new NotImplementedException();
#endif
        }

        public void SetUpDecryption(SymmetricAlgorithm algorithm)
        {
            if (this.State != EncryptionState.Read)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRServiceModel.BadEncryptionState));
            }
            if (algorithm == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("algorithm");
            }
            this.algorithm = algorithm;
            this.State = EncryptionState.DecryptionSetup;
        }

        public void SetUpEncryption(SymmetricAlgorithm algorithm, ArraySegment<byte> buffer)
        {
            if (this.State != EncryptionState.New)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRServiceModel.BadEncryptionState));
            }
            if (algorithm == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("algorithm");
            }
            this.algorithm = algorithm;
            this.buffer = buffer;
            this.State = EncryptionState.EncryptionSetup;
        }

        protected override void WriteCipherData(XmlDictionaryWriter writer)
        {
            writer.WriteBase64(this.iv, 0, this.iv.Length);
            writer.WriteBase64(this.cipherText, 0, this.cipherText.Length);
        }
    }
}
