﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.ServiceModel;

namespace System.IdentityModel.Tokens
{
    public class X509AsymmetricSecurityKey : AsymmetricSecurityKey
    {
        private X509Certificate2 _certificate;
        private AsymmetricAlgorithm _privateKey;
        private bool _privateKeyAvailabilityDetermined;
        private AsymmetricAlgorithm _publicKey;
        private bool _publicKeyAvailabilityDetermined;

        public X509AsymmetricSecurityKey(X509Certificate2 certificate)
        {
            _certificate = certificate ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(certificate));
        }

        public override int KeySize
        {
            get { return PublicKey.KeySize; }
        }

        private AsymmetricAlgorithm PrivateKey
        {
            get
            {
                if (!_privateKeyAvailabilityDetermined)
                {
                    lock (ThisLock)
                    {
                        _privateKey = _certificate.GetRSAPrivateKey();
                        if (_privateKey is not null)
                        {
                            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                            {
                                // TODO: Investigate if this code is still needed or if there's a way to do the same thing without RSACryptoServiceProvider
                                RSACryptoServiceProvider rsaCsp = _privateKey as RSACryptoServiceProvider;
                                // ProviderType == 1 is PROV_RSA_FULL provider type that only supports SHA1. Change it to PROV_RSA_AES=24 that supports SHA2 also.
                                if (rsaCsp != null && rsaCsp.CspKeyContainerInfo.ProviderType == 1)
                                {
                                    CspParameters csp = new CspParameters();
                                    csp.ProviderType = 24;
                                    csp.KeyContainerName = rsaCsp.CspKeyContainerInfo.KeyContainerName;
                                    csp.KeyNumber = (int)rsaCsp.CspKeyContainerInfo.KeyNumber;
                                    if (rsaCsp.CspKeyContainerInfo.MachineKeyStore)
                                    {
                                        csp.Flags = CspProviderFlags.UseMachineKeyStore;
                                    }

                                    csp.Flags |= CspProviderFlags.UseExistingKey;
                                    _privateKey = new RSACryptoServiceProvider(csp);
                                }
                            }
                        }
                        else
                        {
                            _privateKey = _certificate.GetDSAPrivateKey();
                            _privateKey ??= _certificate.GetECDsaPrivateKey();
                        }

                        if (_certificate.HasPrivateKey && _privateKey == null)
                        {
                            DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.PrivateKeyNotSupported));
                        }

                        _privateKeyAvailabilityDetermined = true;
                    }
                }
                return _privateKey;
            }
        }

        private AsymmetricAlgorithm PublicKey
        {
            get
            {
                if (!_publicKeyAvailabilityDetermined)
                {
                    lock (ThisLock)
                    {
                        if (!_publicKeyAvailabilityDetermined)
                        {
                            _publicKey = _certificate.GetRSAPublicKey();
                            if (_publicKey == null)
                            {
                                // Need DSACertificateExtensions  to support DSA certificate which is in netstandard2.1 and netcore2.0. As we target netstandard2.0, we don't
                                // have access to DSACertificateExtensions 
                                DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.PublicKeyNotSupported));
                            }

                            _publicKeyAvailabilityDetermined = true;
                        }
                    }
                }
                return _publicKey;
            }
        }

        private Object ThisLock { get; } = new Object();

        public override byte[] DecryptKey(string algorithm, byte[] keyData)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        public override byte[] EncryptKey(string algorithm, byte[] keyData)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        public override AsymmetricAlgorithm GetAsymmetricAlgorithm(string algorithm, bool privateKey)
        {
            if (privateKey)
            {
                if (PrivateKey == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.MissingPrivateKey));
                }

                if (string.IsNullOrEmpty(algorithm))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(algorithm, SRP.Format(SRP.EmptyOrNullArgumentString, nameof(algorithm)));
                }

                switch (algorithm)
                {
                    case SignedXml.XmlDsigDSAUrl:
                        if ((PrivateKey as DSA) != null)
                        {
                            return (PrivateKey as DSA);
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.AlgorithmAndPrivateKeyMisMatch));

                    case SignedXml.XmlDsigRSASHA1Url:
                    case SecurityAlgorithms.RsaSha256Signature:
                    case EncryptedXml.XmlEncRSA15Url:
                    case EncryptedXml.XmlEncRSAOAEPUrl:
                        if ((PrivateKey as RSA) != null)
                        {
                            return (PrivateKey as RSA);
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.AlgorithmAndPrivateKeyMisMatch));
                    default:
                        if (IsSupportedAlgorithm(algorithm))
                        {
                            return PrivateKey;
                        }
                        else
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.UnsupportedCryptoAlgorithm, algorithm)));
                        }
                }
            }
            else
            {
                switch (algorithm)
                {
                    case SignedXml.XmlDsigDSAUrl:
                        if (PublicKey is DSA dsaPrivateKey)
                        {
                            return dsaPrivateKey;
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.AlgorithmAndPublicKeyMisMatch));
                    case SignedXml.XmlDsigRSASHA1Url:
                    case SecurityAlgorithms.RsaSha256Signature:
                    case EncryptedXml.XmlEncRSA15Url:
                    case EncryptedXml.XmlEncRSAOAEPUrl:
                        if ((PublicKey as RSA) != null)
                        {
                            return (PublicKey as RSA);
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.AlgorithmAndPublicKeyMisMatch));
                    default:

                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.UnsupportedCryptoAlgorithm, algorithm)));
                }
            }
        }

        public override HashAlgorithm GetHashAlgorithmForSignature(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(algorithm, SRP.Format(SRP.EmptyOrNullArgumentString, nameof(algorithm)));
            }

            object algorithmObject = CryptoHelper.GetAlgorithmFromConfig(algorithm);

            if (algorithmObject != null)
            {
                SignatureDescription description = algorithmObject as SignatureDescription;
                if (description != null)
                {
                    return description.CreateDigest();
                }

                HashAlgorithm hashAlgorithm = algorithmObject as HashAlgorithm;
                if (hashAlgorithm != null)
                {
                    return hashAlgorithm;
                }

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CryptographicException(SRP.Format(SRP.UnsupportedAlgorithmForCryptoOperation,
                        algorithm, "CreateDigest")));
            }

            switch (algorithm)
            {
                case SignedXml.XmlDsigDSAUrl:
                case SignedXml.XmlDsigRSASHA1Url:
                    return CryptoHelper.NewSha1HashAlgorithm();
                case SecurityAlgorithms.RsaSha256Signature:
                    return CryptoHelper.NewSha256HashAlgorithm();
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.UnsupportedCryptoAlgorithm, algorithm)));
            }
        }

        public override AsymmetricSignatureDeformatter GetSignatureDeformatter(string algorithm)
        {
            // We support one of the two algoritms, but not both.
            //     XmlDsigDSAUrl = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
            //     XmlDsigRSASHA1Url = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

            if (string.IsNullOrEmpty(algorithm))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(algorithm, SRP.Format(SRP.EmptyOrNullArgumentString, nameof(algorithm)));
            }

            object algorithmObject = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            if (algorithmObject != null)
            {
                SignatureDescription description = algorithmObject as SignatureDescription;
                if (description != null)
                {
                    return description.CreateDeformatter(PublicKey);
                }

                try
                {
                    AsymmetricSignatureDeformatter asymmetricSignatureDeformatter = algorithmObject as AsymmetricSignatureDeformatter;
                    if (asymmetricSignatureDeformatter != null)
                    {
                        asymmetricSignatureDeformatter.SetKey(PublicKey);
                        return asymmetricSignatureDeformatter;
                    }
                }
                catch (InvalidCastException e)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.AlgorithmAndPublicKeyMisMatch, e));
                }

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CryptographicException(SRP.Format(SRP.UnsupportedAlgorithmForCryptoOperation,
                       algorithm, nameof(GetSignatureDeformatter))));
            }

            switch (algorithm)
            {
                case SignedXml.XmlDsigDSAUrl:

                    // Ensure that we have a DSA algorithm object.
                    DSA dsa = (PublicKey as DSA);
                    if (dsa == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.PublicKeyNotDSA));
                    }

                    return new DSASignatureDeformatter(dsa);

                case SignedXml.XmlDsigRSASHA1Url:
                case SecurityAlgorithms.RsaSha256Signature:
                    // Ensure that we have an RSA algorithm object.
                    RSA rsa = (PublicKey as RSA);
                    if (rsa == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.PublicKeyNotRSA));
                    }

                    return new RSAPKCS1SignatureDeformatter(rsa);

                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.UnsupportedCryptoAlgorithm, algorithm)));
            }
        }

        public override AsymmetricSignatureFormatter GetSignatureFormatter(string algorithm)
        {
            // One can sign only if the private key is present.
            if (PrivateKey == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.MissingPrivateKey));
            }

            if (string.IsNullOrEmpty(algorithm))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(algorithm, SRP.Format(SRP.EmptyOrNullArgumentString, nameof(algorithm)));
            }

            // We support:
            //     XmlDsigDSAUrl = "http://www.w3.org/2000/09/xmldsig#dsa-sha1";
            //     XmlDsigRSASHA1Url = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
            //     RsaSha256Signature = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
            AsymmetricAlgorithm privateKey = PrivateKey;

            object algorithmObject = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            if (algorithmObject != null)
            {
                SignatureDescription description = algorithmObject as SignatureDescription;
                if (description != null)
                {
                    return description.CreateFormatter(privateKey);
                }

                try
                {
                    AsymmetricSignatureFormatter asymmetricSignatureFormatter = algorithmObject as AsymmetricSignatureFormatter;
                    if (asymmetricSignatureFormatter != null)
                    {
                        asymmetricSignatureFormatter.SetKey(privateKey);
                        return asymmetricSignatureFormatter;
                    }
                }
                catch (InvalidCastException e)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.AlgorithmAndPrivateKeyMisMatch, e));
                }

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CryptographicException(SRP.Format(SRP.UnsupportedAlgorithmForCryptoOperation,
                       algorithm, nameof(GetSignatureFormatter))));
            }

            switch (algorithm)
            {
                case SignedXml.XmlDsigDSAUrl:

                    // Ensure that we have a DSA algorithm object.
                    DSA dsa = (PrivateKey as DSA);
                    if (dsa == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.PrivateKeyNotDSA));
                    }
#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms
                    return new DSASignatureFormatter(dsa);
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms

                case SignedXml.XmlDsigRSASHA1Url:
                    // Ensure that we have an RSA algorithm object.
                    RSA rsa = (PrivateKey as RSA);
                    if (rsa == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.PrivateKeyNotRSA));
                    }

                    return new RSAPKCS1SignatureFormatter(rsa);

                case SecurityAlgorithms.RsaSha256Signature:
                    // Ensure that we have an RSA algorithm object.
                    RSA rsaSha256 = (privateKey as RSA);
                    if (rsaSha256 == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.PrivateKeyNotRSA));
                    }

                    return new RSAPKCS1SignatureFormatter(rsaSha256);

                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.UnsupportedCryptoAlgorithm, algorithm)));
            }
        }

        public override bool HasPrivateKey()
        {
            return (PrivateKey != null);
        }

        public override bool IsAsymmetricAlgorithm(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(algorithm, SRP.Format(SRP.EmptyOrNullArgumentString, nameof(algorithm)));
            }

            return (CryptoHelper.IsAsymmetricAlgorithm(algorithm));
        }

        public override bool IsSupportedAlgorithm(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(algorithm, SRP.Format(SRP.EmptyOrNullArgumentString, nameof(algorithm)));
            }

            object algorithmObject = null;
            try
            {
                algorithmObject = CryptoHelper.GetAlgorithmFromConfig(algorithm);
            }
            catch (InvalidOperationException)
            {
                algorithm = null;
            }

            if (algorithmObject != null)
            {
                SignatureDescription signatureDescription = algorithmObject as SignatureDescription;
                if (signatureDescription != null)
                {
                    return true;
                }

                AsymmetricAlgorithm asymmetricAlgorithm = algorithmObject as AsymmetricAlgorithm;
                if (asymmetricAlgorithm != null)
                {
                    return true;
                }

                return false;
            }

            switch (algorithm)
            {
                case SignedXml.XmlDsigDSAUrl:
                    return (PublicKey is DSA);

                case SignedXml.XmlDsigRSASHA1Url:
                case SecurityAlgorithms.RsaSha256Signature:
                case EncryptedXml.XmlEncRSA15Url:
                case EncryptedXml.XmlEncRSAOAEPUrl:
                    return (PublicKey is RSA);
                default:
                    return false;
            }
        }

        public override bool IsSymmetricAlgorithm(string algorithm)
        {
            return CryptoHelper.IsSymmetricAlgorithm(algorithm);
        }
    }
}
