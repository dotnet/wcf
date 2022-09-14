// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using System.Xml;

namespace System.ServiceModel.Security
{
    public abstract class SecurityAlgorithmSuite
    {
        private static SecurityAlgorithmSuite s_basic256;
        private static SecurityAlgorithmSuite s_tripleDes;
        private static SecurityAlgorithmSuite s_basic256Sha256;

        static public SecurityAlgorithmSuite Default
        {
            get
            {
                return Basic256;
            }
        }

        static public SecurityAlgorithmSuite Basic256
        {
            get
            {
                if (s_basic256 == null)
                {
                    s_basic256 = new Basic256SecurityAlgorithmSuite();
                }

                return s_basic256;
            }
        }

        static public SecurityAlgorithmSuite TripleDes
        {
            get
            {
                if (s_tripleDes == null)
                {
                    s_tripleDes = new TripleDesSecurityAlgorithmSuite();
                }
                    
                return s_tripleDes;
            }
        }

        static public SecurityAlgorithmSuite Basic256Sha256
        {
            get
            {
                if (s_basic256Sha256 == null)
                {
                    s_basic256Sha256 = new Basic256Sha256SecurityAlgorithmSuite();
                }
                    
                return s_basic256Sha256;
            }
        }

        public abstract string DefaultCanonicalizationAlgorithm { get; }
        public abstract string DefaultDigestAlgorithm { get; }
        public abstract string DefaultEncryptionAlgorithm { get; }
        public abstract int DefaultEncryptionKeyDerivationLength { get; }
        public abstract string DefaultSymmetricKeyWrapAlgorithm { get; }
        public abstract string DefaultAsymmetricKeyWrapAlgorithm { get; }
        public abstract string DefaultSymmetricSignatureAlgorithm { get; }
        public abstract string DefaultAsymmetricSignatureAlgorithm { get; }
        public abstract int DefaultSignatureKeyDerivationLength { get; }
        public abstract int DefaultSymmetricKeyLength { get; }

        internal virtual XmlDictionaryString DefaultCanonicalizationAlgorithmDictionaryString { get { return null; } }
        internal virtual XmlDictionaryString DefaultDigestAlgorithmDictionaryString { get { return null; } }
        internal virtual XmlDictionaryString DefaultEncryptionAlgorithmDictionaryString { get { return null; } }
        internal virtual XmlDictionaryString DefaultSymmetricKeyWrapAlgorithmDictionaryString { get { return null; } }
        internal virtual XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString { get { return null; } }
        internal virtual XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString { get { return null; } }
        internal virtual XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString { get { return null; } }

        protected SecurityAlgorithmSuite() { }

        public virtual bool IsCanonicalizationAlgorithmSupported(string algorithm) { return algorithm == DefaultCanonicalizationAlgorithm; }
        public virtual bool IsDigestAlgorithmSupported(string algorithm) { return algorithm == DefaultDigestAlgorithm; }
        public virtual bool IsEncryptionAlgorithmSupported(string algorithm) { return algorithm == DefaultEncryptionAlgorithm; }
        public virtual bool IsEncryptionKeyDerivationAlgorithmSupported(string algorithm) { return (algorithm == SecurityAlgorithms.Psha1KeyDerivation) || (algorithm == SecurityAlgorithms.Psha1KeyDerivationDec2005); }
        public virtual bool IsSymmetricKeyWrapAlgorithmSupported(string algorithm) { return algorithm == DefaultSymmetricKeyWrapAlgorithm; }
        public virtual bool IsAsymmetricKeyWrapAlgorithmSupported(string algorithm) { return algorithm == DefaultAsymmetricKeyWrapAlgorithm; }
        public virtual bool IsSymmetricSignatureAlgorithmSupported(string algorithm) { return algorithm == DefaultSymmetricSignatureAlgorithm; }
        public virtual bool IsAsymmetricSignatureAlgorithmSupported(string algorithm) { return algorithm == DefaultAsymmetricSignatureAlgorithm; }
        public virtual bool IsSignatureKeyDerivationAlgorithmSupported(string algorithm) { return (algorithm == SecurityAlgorithms.Psha1KeyDerivation) || (algorithm == SecurityAlgorithms.Psha1KeyDerivationDec2005); }
        public abstract bool IsSymmetricKeyLengthSupported(int length);
        public abstract bool IsAsymmetricKeyLengthSupported(int length);

        internal void GetSignatureAlgorithmAndKey(SecurityToken token, out string signatureAlgorithm, out SecurityKey key, out XmlDictionaryString signatureAlgorithmDictionaryString)
        {
            ReadOnlyCollection<SecurityKey> keys = token.SecurityKeys;
            if (keys == null || keys.Count == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SigningTokenHasNoKeys, token)));
            }

            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].IsSupportedAlgorithm(DefaultSymmetricSignatureAlgorithm))
                {
                    signatureAlgorithm = DefaultSymmetricSignatureAlgorithm;
                    signatureAlgorithmDictionaryString = DefaultSymmetricSignatureAlgorithmDictionaryString;
                    key = keys[i];
                    return;
                }
                else if (keys[i].IsSupportedAlgorithm(DefaultAsymmetricSignatureAlgorithm))
                {
                    signatureAlgorithm = DefaultAsymmetricSignatureAlgorithm;
                    signatureAlgorithmDictionaryString = DefaultAsymmetricSignatureAlgorithmDictionaryString;
                    key = keys[i];
                    return;
                }
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SigningTokenHasNoKeysSupportingTheAlgorithmSuite, token, this)));
        }
    }

    public class Basic256SecurityAlgorithmSuite : SecurityAlgorithmSuite
    {
        public Basic256SecurityAlgorithmSuite() : base() { }

        public override string DefaultCanonicalizationAlgorithm { get { return DefaultCanonicalizationAlgorithmDictionaryString.Value; } }
        public override string DefaultDigestAlgorithm { get { return DefaultDigestAlgorithmDictionaryString.Value; } }
        public override string DefaultEncryptionAlgorithm { get { return DefaultEncryptionAlgorithmDictionaryString.Value; } }
        public override int DefaultEncryptionKeyDerivationLength { get { return 256; } }
        public override string DefaultSymmetricKeyWrapAlgorithm { get { return DefaultSymmetricKeyWrapAlgorithmDictionaryString.Value; } }
        public override string DefaultAsymmetricKeyWrapAlgorithm { get { return DefaultAsymmetricKeyWrapAlgorithmDictionaryString.Value; } }
        public override string DefaultSymmetricSignatureAlgorithm { get { return DefaultSymmetricSignatureAlgorithmDictionaryString.Value; } }
        public override string DefaultAsymmetricSignatureAlgorithm { get { return DefaultAsymmetricSignatureAlgorithmDictionaryString.Value; } }
        public override int DefaultSignatureKeyDerivationLength { get { return 192; } }
        public override int DefaultSymmetricKeyLength { get { return 256; } }
        public override bool IsSymmetricKeyLengthSupported(int length) { return length == 256; }
        public override bool IsAsymmetricKeyLengthSupported(int length) { return length >= 1024 && length <= 4096; }

        internal override XmlDictionaryString DefaultCanonicalizationAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.ExclusiveC14n; } }
        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Sha1Digest; } }
        internal override XmlDictionaryString DefaultEncryptionAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Aes256Encryption; } }
        internal override XmlDictionaryString DefaultSymmetricKeyWrapAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Aes256KeyWrap; } }
        internal override XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaOaepKeyWrap; } }
        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.HmacSha1Signature; } }
        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaSha1Signature; } }

        public override string ToString()
        {
            return "Basic256";
        }
    }

    public class TripleDesSecurityAlgorithmSuite : SecurityAlgorithmSuite
    {
        public TripleDesSecurityAlgorithmSuite() : base() { }

        public override string DefaultCanonicalizationAlgorithm { get { return DefaultCanonicalizationAlgorithmDictionaryString.Value; } }
        public override string DefaultDigestAlgorithm { get { return DefaultDigestAlgorithmDictionaryString.Value; } }
        public override string DefaultEncryptionAlgorithm { get { return DefaultEncryptionAlgorithmDictionaryString.Value; } }
        public override int DefaultEncryptionKeyDerivationLength { get { return 192; } }
        public override string DefaultSymmetricKeyWrapAlgorithm { get { return DefaultSymmetricKeyWrapAlgorithmDictionaryString.Value; } }
        public override string DefaultAsymmetricKeyWrapAlgorithm { get { return this.DefaultAsymmetricKeyWrapAlgorithmDictionaryString.Value; } }

        public override string DefaultSymmetricSignatureAlgorithm { get { return DefaultSymmetricSignatureAlgorithmDictionaryString.Value; } }
        public override string DefaultAsymmetricSignatureAlgorithm { get { return DefaultAsymmetricSignatureAlgorithmDictionaryString.Value; } }
        public override int DefaultSignatureKeyDerivationLength { get { return 192; } }
        public override int DefaultSymmetricKeyLength { get { return 192; } }
        public override bool IsSymmetricKeyLengthSupported(int length) { return length >= 192 && length <= 256; }
        public override bool IsAsymmetricKeyLengthSupported(int length) { return length >= 1024 && length <= 4096; }

        internal override XmlDictionaryString DefaultCanonicalizationAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.ExclusiveC14n; } }
        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Sha1Digest; } }
        internal override XmlDictionaryString DefaultEncryptionAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.TripleDesEncryption; } }
        internal override XmlDictionaryString DefaultSymmetricKeyWrapAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.TripleDesKeyWrap; } }
        internal override XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaOaepKeyWrap; } }
        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.HmacSha1Signature; } }
        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaSha1Signature; } }

        public override string ToString()
        {
            return "TripleDes";
        }
    }

    public class Basic256Sha256SecurityAlgorithmSuite : Basic256SecurityAlgorithmSuite
    {
        public Basic256Sha256SecurityAlgorithmSuite() : base() { }

        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Sha256Digest; } }
        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.HmacSha256Signature; } }
        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaSha256Signature; } }

        public override string ToString()
        {
            return "Basic256Sha256";
        }
    }
}
