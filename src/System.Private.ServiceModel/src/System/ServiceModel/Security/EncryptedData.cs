// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.Xml;

namespace System.ServiceModel.Security
{
    class EncryptedData : EncryptedType
    {
        internal static readonly XmlDictionaryString s_ElementName = System.IdentityModel.XD.XmlEncryptionDictionary.EncryptedData;
        internal static readonly string s_ElementType = XmlEncryptionStrings.ElementType;
        internal static readonly string s_ContentType = XmlEncryptionStrings.ContentType;
        private SymmetricAlgorithm _algorithm;
        private byte[] _decryptedBuffer;
        private ArraySegment<byte> _buffer;
        private byte[] _iv;
        private byte[] _cipherText;

        protected override XmlDictionaryString OpeningElementName
        {
            get { return s_ElementName; }
        }

        void EnsureDecryptionSet()
        {
            if (this.State == EncryptionState.DecryptionSetup)
            {
                SetPlainText();
            }
            else if (this.State != EncryptionState.Decrypted)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SR.Format(SR.BadEncryptionState)));
            }
        }

        protected override void ForceEncryption()
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
            //CryptoHelper.GenerateIVAndEncrypt(this.algorithm, this.buffer, out this.iv, out this.cipherText);
            //this.State = EncryptionState.Encrypted;
            //this.buffer = new ArraySegment<byte>(CryptoHelper.EmptyBuffer);
        }

        public byte[] GetDecryptedBuffer()
        {
            EnsureDecryptionSet();
            return this._decryptedBuffer;
        }

        protected override void ReadCipherData(XmlDictionaryReader reader)
        {
            this._cipherText = reader.ReadContentAsBase64();
        }

        protected override void ReadCipherData(XmlDictionaryReader reader, long maxBufferSize)
        {
            this._cipherText = SecurityUtils.ReadContentAsBase64(reader, maxBufferSize);
        }

        void SetPlainText()
        {
            throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
            //this.decryptedBuffer = CryptoHelper.ExtractIVAndDecrypt(this.algorithm, this.cipherText, 0, this.cipherText.Length);
            //this.State = EncryptionState.Decrypted;
        }

        public void SetUpDecryption(SymmetricAlgorithm algorithm)
        {
            if (this.State != EncryptionState.Read)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SR.Format(SR.BadEncryptionState)));
            }
            if (algorithm == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("algorithm");
            }
            this._algorithm = algorithm;
            this.State = EncryptionState.DecryptionSetup;
        }

        public void SetUpEncryption(SymmetricAlgorithm algorithm, ArraySegment<byte> buffer)
        {
            if (this.State != EncryptionState.New)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SR.Format(SR.BadEncryptionState)));
            }
            if (algorithm == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("algorithm");
            }
            this._algorithm = algorithm;
            this._buffer = buffer;
            this.State = EncryptionState.EncryptionSetup;
        }

        protected override void WriteCipherData(XmlDictionaryWriter writer)
        {
            writer.WriteBase64(this._iv, 0, this._iv.Length);
            writer.WriteBase64(this._cipherText, 0, this._cipherText.Length);
        }
    }
}

