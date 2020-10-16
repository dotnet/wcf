// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    using System.Security.Cryptography;
    using System.ServiceModel.Channels;
    using Microsoft.Xml;

    internal class EncryptedData : EncryptedType
    {
        internal static readonly XmlDictionaryString ElementName = XD.XmlEncryptionDictionary.EncryptedData;
        internal static readonly string ElementType = XmlEncryptionStrings.ElementType;
        internal static readonly string ContentType = XmlEncryptionStrings.ContentType;
        private SymmetricAlgorithm _algorithm;
        private byte[] _decryptedBuffer;
        private ArraySegment<byte> _buffer;
        private byte[] _iv;
        private byte[] _cipherText;

        protected override XmlDictionaryString OpeningElementName
        {
            get { return ElementName; }
        }

        private void EnsureDecryptionSet()
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
            throw new NotImplementedException();
        }
        public byte[] GetDecryptedBuffer()
        {
            EnsureDecryptionSet();
            return _decryptedBuffer;
        }

        protected override void ReadCipherData(XmlDictionaryReader reader)
        {
            _cipherText = reader.ReadContentAsBase64();
        }

        protected override void ReadCipherData(XmlDictionaryReader reader, long maxBufferSize)
        {
            _cipherText = SecurityUtils.ReadContentAsBase64(reader, maxBufferSize);
        }

        private void SetPlainText()
        {
            throw new NotImplementedException();
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
            _algorithm = algorithm;
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
            _algorithm = algorithm;
            _buffer = buffer;
            this.State = EncryptionState.EncryptionSetup;
        }

        protected override void WriteCipherData(XmlDictionaryWriter writer)
        {
            writer.WriteBase64(_iv, 0, _iv.Length);
            writer.WriteBase64(_cipherText, 0, _cipherText.Length);
        }
    }
}
