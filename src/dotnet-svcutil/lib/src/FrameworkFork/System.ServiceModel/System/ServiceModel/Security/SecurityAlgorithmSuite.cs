// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.IdentityModel.Selectors;
using System.Security.Cryptography;
using System.ServiceModel.Security.Tokens;

using System.Text;
using System.Runtime.Serialization;
using Microsoft.Xml;
using System.ComponentModel;

namespace System.ServiceModel.Security
{
    public abstract class SecurityAlgorithmSuite
    {
        private static SecurityAlgorithmSuite s_basic256;
        private static SecurityAlgorithmSuite s_basic192;
        private static SecurityAlgorithmSuite s_basic128;
        private static SecurityAlgorithmSuite s_tripleDes;
        private static SecurityAlgorithmSuite s_basic256Rsa15;
        private static SecurityAlgorithmSuite s_basic192Rsa15;
        private static SecurityAlgorithmSuite s_basic128Rsa15;
        private static SecurityAlgorithmSuite s_tripleDesRsa15;
        private static SecurityAlgorithmSuite s_basic256Sha256;
        private static SecurityAlgorithmSuite s_basic192Sha256;
        private static SecurityAlgorithmSuite s_basic128Sha256;
        private static SecurityAlgorithmSuite s_tripleDesSha256;
        private static SecurityAlgorithmSuite s_basic256Sha256Rsa15;
        private static SecurityAlgorithmSuite s_basic192Sha256Rsa15;
        private static SecurityAlgorithmSuite s_basic128Sha256Rsa15;
        private static SecurityAlgorithmSuite s_tripleDesSha256Rsa15;

        static internal SecurityAlgorithmSuite KerberosDefault
        {
            get
            {
                return Basic128;
            }
        }
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
                    s_basic256 = new Basic256SecurityAlgorithmSuite();
                return s_basic256;
            }
        }
        static public SecurityAlgorithmSuite Basic192
        {
            get
            {
                if (s_basic192 == null)
                    s_basic192 = new Basic192SecurityAlgorithmSuite();
                return s_basic192;
            }
        }
        static public SecurityAlgorithmSuite Basic128
        {
            get
            {
                if (s_basic128 == null)
                    s_basic128 = new Basic128SecurityAlgorithmSuite();
                return s_basic128;
            }
        }
        static public SecurityAlgorithmSuite TripleDes
        {
            get
            {
                if (s_tripleDes == null)
                    s_tripleDes = new TripleDesSecurityAlgorithmSuite();
                return s_tripleDes;
            }
        }
        static public SecurityAlgorithmSuite Basic256Rsa15
        {
            get
            {
                if (s_basic256Rsa15 == null)
                    s_basic256Rsa15 = new Basic256Rsa15SecurityAlgorithmSuite();
                return s_basic256Rsa15;
            }
        }
        static public SecurityAlgorithmSuite Basic192Rsa15
        {
            get
            {
                if (s_basic192Rsa15 == null)
                    s_basic192Rsa15 = new Basic192Rsa15SecurityAlgorithmSuite();
                return s_basic192Rsa15;
            }
        }
        static public SecurityAlgorithmSuite Basic128Rsa15
        {
            get
            {
                if (s_basic128Rsa15 == null)
                    s_basic128Rsa15 = new Basic128Rsa15SecurityAlgorithmSuite();
                return s_basic128Rsa15;
            }
        }
        static public SecurityAlgorithmSuite TripleDesRsa15
        {
            get
            {
                if (s_tripleDesRsa15 == null)
                    s_tripleDesRsa15 = new TripleDesRsa15SecurityAlgorithmSuite();
                return s_tripleDesRsa15;
            }
        }

        static public SecurityAlgorithmSuite Basic256Sha256
        {
            get
            {
                if (s_basic256Sha256 == null)
                    s_basic256Sha256 = new Basic256Sha256SecurityAlgorithmSuite();
                return s_basic256Sha256;
            }
        }
        static public SecurityAlgorithmSuite Basic192Sha256
        {
            get
            {
                if (s_basic192Sha256 == null)
                    s_basic192Sha256 = new Basic192Sha256SecurityAlgorithmSuite();
                return s_basic192Sha256;
            }
        }
        static public SecurityAlgorithmSuite Basic128Sha256
        {
            get
            {
                if (s_basic128Sha256 == null)
                    s_basic128Sha256 = new Basic128Sha256SecurityAlgorithmSuite();
                return s_basic128Sha256;
            }
        }
        static public SecurityAlgorithmSuite TripleDesSha256
        {
            get
            {
                if (s_tripleDesSha256 == null)
                    s_tripleDesSha256 = new TripleDesSha256SecurityAlgorithmSuite();
                return s_tripleDesSha256;
            }
        }
        static public SecurityAlgorithmSuite Basic256Sha256Rsa15
        {
            get
            {
                if (s_basic256Sha256Rsa15 == null)
                    s_basic256Sha256Rsa15 = new Basic256Sha256Rsa15SecurityAlgorithmSuite();
                return s_basic256Sha256Rsa15;
            }
        }
        static public SecurityAlgorithmSuite Basic192Sha256Rsa15
        {
            get
            {
                if (s_basic192Sha256Rsa15 == null)
                    s_basic192Sha256Rsa15 = new Basic192Sha256Rsa15SecurityAlgorithmSuite();
                return s_basic192Sha256Rsa15;
            }
        }
        static public SecurityAlgorithmSuite Basic128Sha256Rsa15
        {
            get
            {
                if (s_basic128Sha256Rsa15 == null)
                    s_basic128Sha256Rsa15 = new Basic128Sha256Rsa15SecurityAlgorithmSuite();
                return s_basic128Sha256Rsa15;
            }
        }
        static public SecurityAlgorithmSuite TripleDesSha256Rsa15
        {
            get
            {
                if (s_tripleDesSha256Rsa15 == null)
                    s_tripleDesSha256Rsa15 = new TripleDesSha256Rsa15SecurityAlgorithmSuite();
                return s_tripleDesSha256Rsa15;
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

        internal static bool IsRsaSHA256(SecurityAlgorithmSuite suite)
        {
            if (suite == null)
                return false;

            return (suite == Basic128Sha256 || suite == Basic128Sha256Rsa15 || suite == Basic192Sha256 || suite == Basic192Sha256Rsa15 ||
                suite == Basic256Sha256 || suite == Basic256Sha256Rsa15 || suite == TripleDesSha256 || suite == TripleDesSha256Rsa15);
        }

        internal string GetEncryptionKeyDerivationAlgorithm(SecurityToken token, SecureConversationVersion version)
        {
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");

            string derivationAlgorithm = SecurityUtils.GetKeyDerivationAlgorithm(version);
            if (SecurityUtils.IsSupportedAlgorithm(derivationAlgorithm, token))
                return derivationAlgorithm;
            else
                return null;
        }

        internal int GetEncryptionKeyDerivationLength(SecurityToken token, SecureConversationVersion version)
        {
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");

            string derivationAlgorithm = SecurityUtils.GetKeyDerivationAlgorithm(version);
            if (SecurityUtils.IsSupportedAlgorithm(derivationAlgorithm, token))
            {
                if (this.DefaultEncryptionKeyDerivationLength % 8 != 0)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(string.Format(SRServiceModel.Psha1KeyLengthInvalid, this.DefaultEncryptionKeyDerivationLength)));

                return this.DefaultEncryptionKeyDerivationLength / 8;
            }
            else
                return 0;
        }

        internal void GetKeyWrapAlgorithm(SecurityToken token, out string keyWrapAlgorithm, out XmlDictionaryString keyWrapAlgorithmDictionaryString)
        {
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");

            if (SecurityUtils.IsSupportedAlgorithm(this.DefaultSymmetricKeyWrapAlgorithm, token))
            {
                keyWrapAlgorithm = this.DefaultSymmetricKeyWrapAlgorithm;
                keyWrapAlgorithmDictionaryString = this.DefaultSymmetricKeyWrapAlgorithmDictionaryString;
            }
            else
            {
                keyWrapAlgorithm = this.DefaultAsymmetricKeyWrapAlgorithm;
                keyWrapAlgorithmDictionaryString = this.DefaultAsymmetricKeyWrapAlgorithmDictionaryString;
            }
        }

        internal void GetSignatureAlgorithmAndKey(SecurityToken token, out string signatureAlgorithm, out SecurityKey key, out XmlDictionaryString signatureAlgorithmDictionaryString)
        {
            ReadOnlyCollection<SecurityKey> keys = token.SecurityKeys;
            if (keys == null || keys.Count == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SigningTokenHasNoKeys, token)));
            }

            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].IsSupportedAlgorithm(this.DefaultSymmetricSignatureAlgorithm))
                {
                    signatureAlgorithm = this.DefaultSymmetricSignatureAlgorithm;
                    signatureAlgorithmDictionaryString = this.DefaultSymmetricSignatureAlgorithmDictionaryString;
                    key = keys[i];
                    return;
                }
                else if (keys[i].IsSupportedAlgorithm(this.DefaultAsymmetricSignatureAlgorithm))
                {
                    signatureAlgorithm = this.DefaultAsymmetricSignatureAlgorithm;
                    signatureAlgorithmDictionaryString = this.DefaultAsymmetricSignatureAlgorithmDictionaryString;
                    key = keys[i];
                    return;
                }
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SigningTokenHasNoKeysSupportingTheAlgorithmSuite, token, this)));
        }

        internal string GetSignatureKeyDerivationAlgorithm(SecurityToken token, SecureConversationVersion version)
        {
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");

            string derivationAlgorithm = SecurityUtils.GetKeyDerivationAlgorithm(version);
            if (SecurityUtils.IsSupportedAlgorithm(derivationAlgorithm, token))
                return derivationAlgorithm;
            else
                return null;
        }

        internal int GetSignatureKeyDerivationLength(SecurityToken token, SecureConversationVersion version)
        {
            if (token == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");

            string derivationAlgorithm = SecurityUtils.GetKeyDerivationAlgorithm(version);
            if (SecurityUtils.IsSupportedAlgorithm(derivationAlgorithm, token))
            {
                if (this.DefaultSignatureKeyDerivationLength % 8 != 0)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(string.Format(SRServiceModel.Psha1KeyLengthInvalid, this.DefaultSignatureKeyDerivationLength)));

                return this.DefaultSignatureKeyDerivationLength / 8;
            }
            else
                return 0;
        }

        internal void EnsureAcceptableSymmetricSignatureAlgorithm(string algorithm)
        {
            if (!IsSymmetricSignatureAlgorithmSupported(algorithm))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(string.Format(SRServiceModel.SuiteDoesNotAcceptAlgorithm,
                    algorithm, "SymmetricSignature", this)));
            }
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

    public class Basic192SecurityAlgorithmSuite : SecurityAlgorithmSuite
    {
        public Basic192SecurityAlgorithmSuite() : base() { }

        public override string DefaultCanonicalizationAlgorithm { get { return DefaultCanonicalizationAlgorithmDictionaryString.Value; } }
        public override string DefaultDigestAlgorithm { get { return DefaultDigestAlgorithmDictionaryString.Value; } }
        public override string DefaultEncryptionAlgorithm { get { return DefaultEncryptionAlgorithmDictionaryString.Value; } }
        public override int DefaultEncryptionKeyDerivationLength { get { return 192; } }
        public override string DefaultSymmetricKeyWrapAlgorithm { get { return DefaultSymmetricKeyWrapAlgorithmDictionaryString.Value; } }
        public override string DefaultAsymmetricKeyWrapAlgorithm { get { return DefaultAsymmetricKeyWrapAlgorithmDictionaryString.Value; } }
        public override string DefaultSymmetricSignatureAlgorithm { get { return DefaultSymmetricSignatureAlgorithmDictionaryString.Value; } }
        public override string DefaultAsymmetricSignatureAlgorithm { get { return DefaultAsymmetricSignatureAlgorithmDictionaryString.Value; } }
        public override int DefaultSignatureKeyDerivationLength { get { return 192; } }
        public override int DefaultSymmetricKeyLength { get { return 192; } }
        public override bool IsSymmetricKeyLengthSupported(int length) { return length >= 192 && length <= 256; }
        public override bool IsAsymmetricKeyLengthSupported(int length) { return length >= 1024 && length <= 4096; }

        internal override XmlDictionaryString DefaultCanonicalizationAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.ExclusiveC14n; } }
        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Sha1Digest; } }
        internal override XmlDictionaryString DefaultEncryptionAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Aes192Encryption; } }
        internal override XmlDictionaryString DefaultSymmetricKeyWrapAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Aes192KeyWrap; } }
        internal override XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaOaepKeyWrap; } }
        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.HmacSha1Signature; } }
        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaSha1Signature; } }

        public override string ToString()
        {
            return "Basic192";
        }
    }

    public class Basic128SecurityAlgorithmSuite : SecurityAlgorithmSuite
    {
        public Basic128SecurityAlgorithmSuite() : base() { }

        public override string DefaultCanonicalizationAlgorithm { get { return this.DefaultCanonicalizationAlgorithmDictionaryString.Value; } }
        public override string DefaultDigestAlgorithm { get { return this.DefaultDigestAlgorithmDictionaryString.Value; } }
        public override string DefaultEncryptionAlgorithm { get { return this.DefaultEncryptionAlgorithmDictionaryString.Value; } }
        public override int DefaultEncryptionKeyDerivationLength { get { return 128; } }
        public override string DefaultSymmetricKeyWrapAlgorithm { get { return this.DefaultSymmetricKeyWrapAlgorithmDictionaryString.Value; } }
        public override string DefaultAsymmetricKeyWrapAlgorithm { get { return this.DefaultAsymmetricKeyWrapAlgorithmDictionaryString.Value; } }
        public override string DefaultSymmetricSignatureAlgorithm { get { return this.DefaultSymmetricSignatureAlgorithmDictionaryString.Value; } }
        public override string DefaultAsymmetricSignatureAlgorithm { get { return this.DefaultAsymmetricSignatureAlgorithmDictionaryString.Value; } }
        public override int DefaultSignatureKeyDerivationLength { get { return 128; } }
        public override int DefaultSymmetricKeyLength { get { return 128; } }
        public override bool IsSymmetricKeyLengthSupported(int length) { return length >= 128 && length <= 256; }
        public override bool IsAsymmetricKeyLengthSupported(int length) { return length >= 1024 && length <= 4096; }

        internal override XmlDictionaryString DefaultCanonicalizationAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.ExclusiveC14n; } }
        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Sha1Digest; } }
        internal override XmlDictionaryString DefaultEncryptionAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Aes128Encryption; } }
        internal override XmlDictionaryString DefaultSymmetricKeyWrapAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Aes128KeyWrap; } }
        internal override XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaOaepKeyWrap; } }
        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.HmacSha1Signature; } }
        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaSha1Signature; } }

        public override string ToString()
        {
            return "Basic128";
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

    internal class Basic128Rsa15SecurityAlgorithmSuite : Basic128SecurityAlgorithmSuite
    {
        public Basic128Rsa15SecurityAlgorithmSuite() : base() { }

        internal override XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaV15KeyWrap; } }

        public override string ToString()
        {
            return "Basic128Rsa15";
        }
    }

    internal class Basic192Rsa15SecurityAlgorithmSuite : Basic192SecurityAlgorithmSuite
    {
        public Basic192Rsa15SecurityAlgorithmSuite() : base() { }

        internal override XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaV15KeyWrap; } }

        public override string ToString()
        {
            return "Basic192Rsa15";
        }
    }

    internal class Basic256Rsa15SecurityAlgorithmSuite : Basic256SecurityAlgorithmSuite
    {
        public Basic256Rsa15SecurityAlgorithmSuite() : base() { }

        internal override XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaV15KeyWrap; } }

        public override string ToString()
        {
            return "Basic256Rsa15";
        }
    }

    internal class TripleDesRsa15SecurityAlgorithmSuite : TripleDesSecurityAlgorithmSuite
    {
        public TripleDesRsa15SecurityAlgorithmSuite() : base() { }

        internal override XmlDictionaryString DefaultAsymmetricKeyWrapAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaV15KeyWrap; } }

        public override string ToString()
        {
            return "TripleDesRsa15";
        }
    }

    internal class Basic256Sha256SecurityAlgorithmSuite : Basic256SecurityAlgorithmSuite
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

    internal class Basic192Sha256SecurityAlgorithmSuite : Basic192SecurityAlgorithmSuite
    {
        public Basic192Sha256SecurityAlgorithmSuite() : base() { }

        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Sha256Digest; } }
        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.HmacSha256Signature; } }
        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaSha256Signature; } }

        public override string ToString()
        {
            return "Basic192Sha256";
        }
    }

    internal class Basic128Sha256SecurityAlgorithmSuite : Basic128SecurityAlgorithmSuite
    {
        public Basic128Sha256SecurityAlgorithmSuite() : base() { }

        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Sha256Digest; } }
        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.HmacSha256Signature; } }
        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaSha256Signature; } }

        public override string ToString()
        {
            return "Basic128Sha256";
        }
    }

    internal class TripleDesSha256SecurityAlgorithmSuite : TripleDesSecurityAlgorithmSuite
    {
        public TripleDesSha256SecurityAlgorithmSuite() : base() { }

        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Sha256Digest; } }
        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.HmacSha256Signature; } }
        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaSha256Signature; } }

        public override string ToString()
        {
            return "TripleDesSha256";
        }
    }

    internal class Basic256Sha256Rsa15SecurityAlgorithmSuite : Basic256Rsa15SecurityAlgorithmSuite
    {
        public Basic256Sha256Rsa15SecurityAlgorithmSuite() : base() { }

        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Sha256Digest; } }
        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.HmacSha256Signature; } }
        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaSha256Signature; } }

        public override string ToString()
        {
            return "Basic256Sha256Rsa15";
        }
    }

    internal class Basic192Sha256Rsa15SecurityAlgorithmSuite : Basic192Rsa15SecurityAlgorithmSuite
    {
        public Basic192Sha256Rsa15SecurityAlgorithmSuite() : base() { }

        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Sha256Digest; } }
        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.HmacSha256Signature; } }
        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaSha256Signature; } }

        public override string ToString()
        {
            return "Basic192Sha256Rsa15";
        }
    }

    internal class Basic128Sha256Rsa15SecurityAlgorithmSuite : Basic128Rsa15SecurityAlgorithmSuite
    {
        public Basic128Sha256Rsa15SecurityAlgorithmSuite() : base() { }

        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Sha256Digest; } }
        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.HmacSha256Signature; } }
        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaSha256Signature; } }

        public override string ToString()
        {
            return "Basic128Sha256Rsa15";
        }
    }

    internal class TripleDesSha256Rsa15SecurityAlgorithmSuite : TripleDesRsa15SecurityAlgorithmSuite
    {
        public TripleDesSha256Rsa15SecurityAlgorithmSuite() : base() { }

        internal override XmlDictionaryString DefaultDigestAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.Sha256Digest; } }
        internal override XmlDictionaryString DefaultSymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.HmacSha256Signature; } }
        internal override XmlDictionaryString DefaultAsymmetricSignatureAlgorithmDictionaryString { get { return XD.SecurityAlgorithmDictionary.RsaSha256Signature; } }

        public override string ToString()
        {
            return "TripleDesSha256Rsa15";
        }
    }
}
