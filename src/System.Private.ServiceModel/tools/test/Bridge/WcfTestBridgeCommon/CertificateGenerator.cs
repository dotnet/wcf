﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using X509Certificate2 = System.Security.Cryptography.X509Certificates.X509Certificate2;
using X509KeyStorageFlags = System.Security.Cryptography.X509Certificates.X509KeyStorageFlags;

namespace WcfTestBridgeCommon
{
    // NOT THREADSAFE. Callers should lock before doing work with this class if multithreaded operation is expected
    public class CertificateGenerator
    {
        private bool _isInitialized;

        // Settable properties prior to initialization
        private string _crlUri;
        private string _crlUriBridgeHost;
        private string _crlUriRelativePath;
        private string _password;
        private TimeSpan _validityPeriod = TimeSpan.FromDays(1);

        // This can't be too short as there might be a time skew between machines,
        // but also can't be too long, as the CRL is cached by the machine
        private TimeSpan _crlValidityPeriod = TimeSpan.FromMinutes(2);

        // Give the cert a grace period in case there's a time skew between machines
        private readonly TimeSpan _gracePeriod = TimeSpan.FromHours(1);

        private const string _authorityCanonicalName = "DO_NOT_TRUST_WcfBridgeRootCA";
        private const string _signatureAlthorithm = "SHA1WithRSAEncryption";
        private const string _upnObjectId = "1.3.6.1.4.1.311.20.2.3";
        private const int _keyLengthInBits = 2048;
        
        private static readonly X509V3CertificateGenerator _certGenerator = new X509V3CertificateGenerator();
        private static readonly X509V2CrlGenerator _crlGenerator = new X509V2CrlGenerator();

        // key: serial number, value: revocation time
        private static Dictionary<string, DateTime> _revokedCertificates = new Dictionary<string, DateTime>(); 
        
        private RsaKeyPairGenerator _keyPairGenerator;
        private SecureRandom _random;

        private DateTime _initializationDateTime;
        private DateTime _defaultValidityNotBefore;
        private DateTime _defaultValidityNotAfter;

        // We need to hang onto the _authorityKeyPair and _authorityCertificate - all certificates generated 
        // by this instance will be signed by this Authority certificate and private key
        private AsymmetricCipherKeyPair _authorityKeyPair;
        private X509CertificateContainer _authorityCertificate;

        public void Initialize()
        {
            if (!_isInitialized)
            {
                if (string.IsNullOrWhiteSpace(_authorityCanonicalName))
                {
                    throw new ArgumentException("AuthorityCanonicalName must not be an empty string or only whitespace", "AuthorityCanonicalName");
                }

                if (string.IsNullOrWhiteSpace(_password))
                {
                    throw new ArgumentException("Password must not be an empty string or only whitespace", "Password");
                }

                Uri dummy;
                if (string.IsNullOrWhiteSpace(_crlUriRelativePath) && !Uri.TryCreate(_crlUriRelativePath, UriKind.Relative, out dummy))
                {
                    throw new ArgumentException("CrlUri must be a valid relative URI", "CrlUriRelativePath");
                }

                _crlUri = string.Format("{0}{1}", _crlUriBridgeHost, _crlUriRelativePath); 

                _initializationDateTime = DateTime.UtcNow;
                _defaultValidityNotBefore = _initializationDateTime.Subtract(_gracePeriod);
                _defaultValidityNotAfter = _initializationDateTime.Add(_validityPeriod);

                _random = new SecureRandom(new CryptoApiRandomGenerator());
                _keyPairGenerator = new RsaKeyPairGenerator();
                _keyPairGenerator.Init(new KeyGenerationParameters(_random, _keyLengthInBits));
                _authorityKeyPair = _keyPairGenerator.GenerateKeyPair();

                _isInitialized = true;

                Trace.WriteLine("[CertificateGenerator] initialized with the following configuration:");
                Trace.WriteLine(string.Format("    {0} = {1}", "AuthorityCanonicalName", _authorityCanonicalName));
                Trace.WriteLine(string.Format("    {0} = {1}", "CrlUri", _crlUri));
                Trace.WriteLine(string.Format("    {0} = {1}", "Password", _password));
                Trace.WriteLine(string.Format("    {0} = {1}", "ValidityPeriod", _validityPeriod));
                Trace.WriteLine(string.Format("    {0} = {1}", "Valid to", _defaultValidityNotAfter));

                _authorityCertificate = CreateCertificate(isAuthority: true, isMachineCert: false, signingCertificate: null, certificateCreationSettings: null);
            }
        }

        public void Reset()
        {
            _certGenerator.Reset();
            _crlGenerator.Reset();
            _authorityCertificate = null;
            _isInitialized = false;
        }

        public X509CertificateContainer AuthorityCertificate
        {
            get
            {
                EnsureInitialized();
                return _authorityCertificate;
            }
        }

        public byte[] CrlEncoded
        {
            get
            {
                EnsureInitialized(); 
                return CreateCrl(_authorityCertificate.InternalCertificate).GetEncoded();
            }
        }

        public bool Initialized
        {
            get { return _isInitialized; }
        }

        public string AuthorityCanonicalName
        {
            get { return _authorityCanonicalName; }
        }

        public string AuthorityDistinguishedName
        {
            get
            {
                EnsureInitialized(); 
                return CreateX509Name(_authorityCanonicalName).ToString(); 
            }
        }

        public string CertificatePassword
        {
            get { return _password; }
            set
            {
                EnsureNotInitialized("CertificatePassword");
                _password = value; 
            }
        }

        public string CrlUri
        {
            get
            {
                EnsureInitialized(); 
                return _crlUri;
            }
        }

        public string CrlUriBridgeHost
        {
            get
            {
                return _crlUriBridgeHost; 
            }
            set
            {
                EnsureNotInitialized("CrlUriBridgeHost");
                _crlUriBridgeHost = value; 
            }
        }

        public string CrlUriRelativePath
        {
            get
            {
                return _crlUriRelativePath;
            }
            set
            {
                EnsureNotInitialized("CrlUriRelativePath");
                _crlUriRelativePath = value;
            }
        }

        public List<string> RevokedCertificates
        {
            get
            {
                List<string> retVal = new List<string>(_revokedCertificates.Keys);
                return retVal; 
            }
        }

        public TimeSpan ValidityPeriod
        {
            get { return _validityPeriod; }
            set
            {
                EnsureNotInitialized("ValidityPeriod");
                _validityPeriod = value; 
            }
        }

        public X509CertificateContainer CreateMachineCertificate(CertificateCreationSettings creationSettings)
        {
            EnsureInitialized();
            return CreateCertificate(false, true, _authorityCertificate.InternalCertificate , creationSettings);
        }

        public X509CertificateContainer CreateUserCertificate(CertificateCreationSettings creationSettings)
        {
            EnsureInitialized();
            return CreateCertificate(false, false, _authorityCertificate.InternalCertificate, creationSettings);
        }

        // Only the ctor should be calling with isAuthority = true
        // if isAuthority, value for isMachineCert doesn't matter
        private X509CertificateContainer CreateCertificate(bool isAuthority, bool isMachineCert, X509Certificate signingCertificate, CertificateCreationSettings certificateCreationSettings)
        {
            if (certificateCreationSettings == null)
            {
                if (isAuthority)
                {
                    certificateCreationSettings = new CertificateCreationSettings();
                }
                else
                {
                    throw new Exception("Parameter certificateCreationSettings cannot be null when isAuthority is false");
                }
            }

            // Set to default cert creation settings if not set
            if (certificateCreationSettings.ValidityNotBefore == default(DateTime))
            {
                certificateCreationSettings.ValidityNotBefore = _defaultValidityNotBefore;
            }
            if (certificateCreationSettings.ValidityNotAfter == default(DateTime))
            {
                certificateCreationSettings.ValidityNotAfter = _defaultValidityNotAfter;
            }

            string[] subjects = certificateCreationSettings.Subjects;
            if (!isAuthority ^ (signingCertificate != null))
            {
                throw new ArgumentException("Either isAuthority == true or signingCertificate is not null");
            }

            if (!isAuthority && (subjects == null || subjects.Length == 0))
            {
                throw new ArgumentException("If not creating an authority, must specify at least one Subject", "subjects");
            }

            if (!isAuthority && string.IsNullOrWhiteSpace(subjects[0]))
            {
                throw new ArgumentException("Certificate Subject must not be an empty string or only whitespace", "creationSettings.Subjects");
            }

            EnsureInitialized();

            _certGenerator.Reset();
            _certGenerator.SetSignatureAlgorithm(_signatureAlthorithm);

            X509Name authorityX509Name = CreateX509Name(_authorityCanonicalName);
            
            var keyPair = isAuthority ? _authorityKeyPair : _keyPairGenerator.GenerateKeyPair();
            if (isAuthority)
            {
                _certGenerator.SetIssuerDN(authorityX509Name);
                _certGenerator.SetSubjectDN(authorityX509Name);

                var authorityKeyIdentifier = new AuthorityKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(_authorityKeyPair.Public),
                    new GeneralNames(new GeneralName(authorityX509Name)),
                    new BigInteger(7, _random).Abs());

                _certGenerator.AddExtension(X509Extensions.AuthorityKeyIdentifier, true, authorityKeyIdentifier);
                _certGenerator.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(X509KeyUsage.DigitalSignature | X509KeyUsage.KeyAgreement | X509KeyUsage.KeyCertSign | X509KeyUsage.KeyEncipherment | X509KeyUsage.CrlSign));

            }
            else
            {
                X509Name subjectName = CreateX509Name(subjects[0]);
                _certGenerator.SetIssuerDN(PrincipalUtilities.GetSubjectX509Principal(signingCertificate));
                _certGenerator.SetSubjectDN(subjectName);

                _certGenerator.AddExtension(X509Extensions.AuthorityKeyIdentifier, true, new AuthorityKeyIdentifierStructure(_authorityKeyPair.Public));
                _certGenerator.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(X509KeyUsage.DigitalSignature | X509KeyUsage.KeyAgreement | X509KeyUsage.KeyEncipherment));

            }

            _certGenerator.AddExtension(X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifierStructure(keyPair.Public));

            _certGenerator.SetSerialNumber(new BigInteger(64 /*sizeInBits*/, _random).Abs());
            _certGenerator.SetNotBefore(certificateCreationSettings.ValidityNotBefore);
            _certGenerator.SetNotAfter(certificateCreationSettings.ValidityNotAfter);
            _certGenerator.SetPublicKey(keyPair.Public);

            _certGenerator.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(isAuthority));
            _certGenerator.AddExtension(X509Extensions.ExtendedKeyUsage, true, new ExtendedKeyUsage(KeyPurposeID.IdKPServerAuth, KeyPurposeID.IdKPClientAuth));

            if (!isAuthority)
            {
                if (isMachineCert)
                {
                    List<Asn1Encodable> subjectAlternativeNames = new List<Asn1Encodable>(); 
                    
                    // All endpoints should also be in the Subject Alt Names 
                    for (int i = 0; i < subjects.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(subjects[i]))
                        {
                            // Machine certs can have additional DNS names
                            subjectAlternativeNames.Add(new GeneralName(GeneralName.DnsName, subjects[i]));
                        }
                    }

                    _certGenerator.AddExtension(X509Extensions.SubjectAlternativeName, true, new DerSequence(subjectAlternativeNames.ToArray()));
                }
                else
                {
                    if (subjects.Length > 1)
                    {
                        var subjectAlternativeNames = new Asn1EncodableVector();
                    
                        // Only add a SAN for the user if there are any
                        for (int i = 1; i < subjects.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(subjects[i]))
                            {
                                Asn1EncodableVector otherNames = new Asn1EncodableVector();
                                otherNames.Add(new DerObjectIdentifier(_upnObjectId));
                                otherNames.Add(new DerTaggedObject(true, 0, new DerUtf8String(subjects[i])));

                                Asn1Object genName = new DerTaggedObject(false, 0, new DerSequence(otherNames));

                                subjectAlternativeNames.Add(genName);
                            }
                        }
                        _certGenerator.AddExtension(X509Extensions.SubjectAlternativeName, true, new DerSequence(subjectAlternativeNames));
                    }
                }
            }

            var crlDistributionPoints = new DistributionPoint[1] {
                new DistributionPoint(new DistributionPointName(
                    new GeneralNames(new GeneralName(GeneralName.UniformResourceIdentifier, _crlUri))), null, new GeneralNames(new GeneralName(authorityX509Name)))
                };
            var revocationListExtension = new CrlDistPoint(crlDistributionPoints);
            _certGenerator.AddExtension(X509Extensions.CrlDistributionPoints, false, revocationListExtension);

            X509Certificate cert = _certGenerator.Generate(_authorityKeyPair.Private, _random);
            if (certificateCreationSettings.IsValidCert)
            {
                EnsureCertificateValidity(cert);
            }

            // For now, given that we don't know what format to return it in, preserve the formats so we have 
            // the flexibility to do what we need to

            X509CertificateContainer container = new X509CertificateContainer(); 

            X509CertificateEntry[] chain = new X509CertificateEntry[1];
            chain[0] = new X509CertificateEntry(cert);

            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.SetKeyEntry("", new AsymmetricKeyEntry(keyPair.Private), chain);

            using (MemoryStream stream = new MemoryStream())
            {
                store.Save(stream, _password.ToCharArray(), _random);
                container.Pfx = stream.ToArray(); 
            }

            X509Certificate2 outputCert;
            if (isAuthority)
            {
                // don't hand out the private key for the cert when it's the authority
                outputCert = new X509Certificate2(cert.GetEncoded()); 
            } 
            else
            {
                // Otherwise, allow encode with the private key. note that X509Certificate2.RawData will not provide the private key
                // you will have to re-export this cert if needed
                outputCert = new X509Certificate2(container.Pfx, _password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
            }

            container.Subject = subjects[0];
            container.InternalCertificate = cert;
            container.Certificate = outputCert;
            container.Thumbprint = outputCert.Thumbprint;

            Trace.WriteLine("[CertificateGenerator] generated a certificate:");
            Trace.WriteLine(string.Format("    {0} = {1}", "isAuthority", isAuthority));
            if (!isAuthority)
            {
                Trace.WriteLine(string.Format("    {0} = {1}", "Signed by", signingCertificate.SubjectDN));
                Trace.WriteLine(string.Format("    {0} = {1}", "Subject (CN) ", subjects[0]));
                Trace.WriteLine(string.Format("    {0} = {1}", "Alt names ", string.Join(", ", subjects)));
            }
            Trace.WriteLine(string.Format("    {0} = {1}", "HasPrivateKey:", outputCert.HasPrivateKey));
            Trace.WriteLine(string.Format("    {0} = {1}", "Thumbprint", outputCert.Thumbprint));

            return container;
        }

        private X509Crl CreateCrl(X509Certificate signingCertificate)
        {
            EnsureInitialized();
            
            _crlGenerator.Reset();

            // Use UtcNow does not work. current local time works.
            // Could be a BouncyCastle issue.
            // We need to assume two test machines are in the same time zone.
            DateTime now = DateTime.Now;
            _crlGenerator.SetThisUpdate(now);
            _crlGenerator.SetNextUpdate(now.Add(_crlValidityPeriod));
            _crlGenerator.SetIssuerDN(PrincipalUtilities.GetSubjectX509Principal(signingCertificate));
            _crlGenerator.SetSignatureAlgorithm(_signatureAlthorithm);

            _crlGenerator.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(signingCertificate));

            BigInteger crlNumber = new BigInteger(7 /*bits for the number*/, _random).Abs();
            _crlGenerator.AddExtension(X509Extensions.CrlNumber, false, new CrlNumber(crlNumber));

            foreach (var kvp in _revokedCertificates)
            {
                _crlGenerator.AddCrlEntry(new BigInteger(kvp.Key, 16), kvp.Value, CrlReason.CessationOfOperation); 
            }

            X509Crl crl = _crlGenerator.Generate(_authorityKeyPair.Private, _random);
            crl.Verify(_authorityKeyPair.Public);

            Trace.WriteLine(string.Format("[CertificateGenerator] has created a Certificate Revocation List :"));
            Trace.WriteLine(string.Format("    {0} = {1}", "Issuer", crl.IssuerDN));
            Trace.WriteLine(string.Format("    {0} = {1}", "CRL Number", crlNumber));

            return crl;
        }

        private void EnsureCertificateValidity(X509Certificate certificate)
        {
            certificate.CheckValidity(DateTime.UtcNow);
            certificate.Verify(_authorityKeyPair.Public);
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                Initialize();
            }
        }

        private void EnsureNotInitialized(string paramName)
        {
            if (_isInitialized)
            {
                throw new ArgumentException(paramName + " cannot be set as the generator has already been initialized.", paramName);
            }
        }

        private static X509Name CreateX509Name(string canonicalName)
        {
            X509Name authorityX509Name;

            IList authorityKeyIdOrder = new ArrayList();
            IDictionary authorityKeyIdName = new Hashtable();

            authorityKeyIdOrder.Add(X509Name.OU);
            authorityKeyIdOrder.Add(X509Name.O);
            authorityKeyIdOrder.Add(X509Name.CN);

            authorityKeyIdName.Add(X509Name.CN, canonicalName);
            authorityKeyIdName.Add(X509Name.O, "DO_NOT_TRUST");
            authorityKeyIdName.Add(X509Name.OU, "Created by https://github.com/dotnet/wcf");

            authorityX509Name = new X509Name(authorityKeyIdOrder, authorityKeyIdName);

            return authorityX509Name;
        }

        public bool RevokeCertificateBySerialNumber(string serialNum)
        {
            bool success = false;
            BigInteger thumbprintBigInt = null;
            try
            {
                thumbprintBigInt = new BigInteger(serialNum, 16 /* radix */);
                success = true;
            }
            catch(FormatException)
            {
                Trace.WriteLine("[CertificateGenerator] RevokeCertificateBySerialNumber:");
                Trace.WriteLine(string.Format("    Invalid serial number specified: '{0}'", serialNum));
            }

            if (success && !_revokedCertificates.ContainsKey(serialNum))
            {
                _revokedCertificates.Add(serialNum, DateTime.UtcNow);
            }

            // Note that we don't actually check against the thumbprints here, we just go ahead and stick the serial 
            // number into the CRL without checking whether or not we've ever generated it
            Trace.WriteLine(string.Format("[CertificateGenerator] Revoke certificate with serial number {0}: ", success ? "succeeded" : "FAILED"));
            return success;
        }

        public class X509CertificateContainer
        {
            public string Subject { get; internal set; }
            internal X509Certificate InternalCertificate { get; set; }
            public X509Certificate2 Certificate { get; internal set; }
            internal byte[] Pfx { get; set; }
            public string Thumbprint { get; internal set; }
        }
    }
}
