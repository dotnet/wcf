// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using X509Certificate2 = System.Security.Cryptography.X509Certificates.X509Certificate2;
using X509KeyStorageFlags = System.Security.Cryptography.X509Certificates.X509KeyStorageFlags;

namespace WcfTestCommon
{
    // NOT THREADSAFE. Callers should lock before doing work with this class if multithreaded operation is expected
    public class CertificateGenerator
    {
        private bool _isInitialized;

        // Settable properties prior to initialization
        private string _crlUri;
        private string _crlServiceUri;
        private string _crlUriRelativePath;
        private string _password;
        private TimeSpan _validityPeriod = TimeSpan.FromDays(1);

        // This can't be too short as there might be a time skew between machines,
        // but also can't be too long, as the CRL is cached by the machine
        private TimeSpan _crlValidityGracePeriodStart = TimeSpan.FromMinutes(5);
        private TimeSpan _crlValidityGracePeriodEnd = TimeSpan.FromMinutes(5);

        // Give the cert a grace period in case there's a time skew between machines
        private readonly TimeSpan _gracePeriod = TimeSpan.FromHours(1);

        private readonly string _authorityCanonicalName = "DO_NOT_TRUST_WcfBridgeRootCA";
        private readonly string _signatureAlgorithm = Org.BouncyCastle.Asn1.Pkcs.PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id;
        private readonly string _upnObjectId = "1.3.6.1.4.1.311.20.2.3";
        private readonly int _keyLengthInBits = 2048;

        private static readonly X509V3CertificateGenerator s_certGenerator = new X509V3CertificateGenerator();
        private static readonly X509V2CrlGenerator s_crlGenerator = new X509V2CrlGenerator();

        // key: serial number, value: revocation time
        private static Dictionary<string, DateTime> s_revokedCertificates = new Dictionary<string, DateTime>();

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

                _crlUri = new Uri(string.Format("http://{0}{1}", _crlServiceUri, _crlUriRelativePath)).AbsoluteUri;

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
            s_certGenerator.Reset();
            s_crlGenerator.Reset();
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

        public string CrlServiceUri
        {
            get
            {
                return _crlServiceUri;
            }
            set
            {
                EnsureNotInitialized("CrlServiceUri");
                _crlServiceUri = value;
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
                List<string> retVal = new List<string>(s_revokedCertificates.Keys);
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
            return CreateCertificate(false, true, _authorityCertificate.InternalCertificate, creationSettings);
        }

        public X509CertificateContainer CreateUserCertificate(CertificateCreationSettings creationSettings)
        {
            EnsureInitialized();
            return CreateCertificate(false, false, _authorityCertificate.InternalCertificate, creationSettings);
        }

        public static BigInteger HashFriendlyName(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // Convert the input string to a byte array
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);

                // Compute the hash value of the input bytes, and take the first 20 bytes
                byte[] hashBytes = sha256.ComputeHash(inputBytes).Take(20).ToArray();

                // return a Positive BigInt of the hash
                var bigInteger = new BigInteger(1, hashBytes.ToArray());
                return bigInteger;
            }
        }

        public static string HashFriendlyNameToString(string input) => HashFriendlyName(input).ToString(16).ToUpper();

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

            if (!isAuthority ^ (signingCertificate != null))
            {
                throw new ArgumentException("Either isAuthority == true or signingCertificate is not null");
            }
            string subject = certificateCreationSettings.Subject;

            // If certificateCreationSettings.SubjectAlternativeNames == null, then we should add exactly one SubjectAlternativeName == Subject
            // so that the default certificate generated is compatible with mainline scenarios
            // However, if certificateCreationSettings.SubjectAlternativeNames == string[0], then allow this as this is a legit scenario we want to test out
            if (certificateCreationSettings.SubjectAlternativeNames == null)
            {
                certificateCreationSettings.SubjectAlternativeNames = new string[1] { subject };
            }

            string[] subjectAlternativeNames = certificateCreationSettings.SubjectAlternativeNames;

            if (!isAuthority && string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("Certificate Subject must not be an empty string or only whitespace", "creationSettings.Subject");
            }

            EnsureInitialized();

            s_certGenerator.Reset();

            // Tag on the generation time to prevent caching of the cert CRL in Linux
            X509Name authorityX509Name = CreateX509Name(string.Format("{0} {1}", _authorityCanonicalName, DateTime.Now.ToString("s")));
            BigInteger serialNum;

            // Search by serial number in Linux/MacOS
            if (!CertificateHelper.CurrentOperatingSystem.IsWindows() && certificateCreationSettings.FriendlyName != null)
            {
                serialNum = HashFriendlyName(certificateCreationSettings.FriendlyName);
            }
            else
            {
                serialNum = new BigInteger(64 /*sizeInBits*/, _random).Abs();
            }

            var keyPair = isAuthority ? _authorityKeyPair : _keyPairGenerator.GenerateKeyPair();
            if (isAuthority)
            {
                s_certGenerator.SetIssuerDN(authorityX509Name);
                s_certGenerator.SetSubjectDN(authorityX509Name);

                var authorityKeyIdentifier = new AuthorityKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(_authorityKeyPair.Public),
                    new GeneralNames(new GeneralName(authorityX509Name)),
                    serialNum);

                s_certGenerator.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, authorityKeyIdentifier);
                s_certGenerator.AddExtension(X509Extensions.KeyUsage, false, new KeyUsage(X509KeyUsage.DigitalSignature | X509KeyUsage.KeyAgreement | X509KeyUsage.KeyCertSign | X509KeyUsage.KeyEncipherment | X509KeyUsage.CrlSign));
            }
            else
            {
                X509Name subjectName = CreateX509Name(subject);
                s_certGenerator.SetIssuerDN(signingCertificate.SubjectDN);
                s_certGenerator.SetSubjectDN(subjectName);

                s_certGenerator.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(_authorityKeyPair.Public));
                s_certGenerator.AddExtension(X509Extensions.KeyUsage, false, new KeyUsage(X509KeyUsage.DigitalSignature | X509KeyUsage.KeyAgreement | X509KeyUsage.KeyEncipherment));
            }

            s_certGenerator.AddExtension(X509Extensions.SubjectKeyIdentifier, false, new SubjectKeyIdentifierStructure(keyPair.Public));

            s_certGenerator.SetSerialNumber(serialNum);
            s_certGenerator.SetNotBefore(certificateCreationSettings.ValidityNotBefore);
            s_certGenerator.SetNotAfter(certificateCreationSettings.ValidityNotAfter);
            s_certGenerator.SetPublicKey(keyPair.Public);

            s_certGenerator.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(isAuthority));
            if (certificateCreationSettings.EKU == null || certificateCreationSettings.EKU.Count == 0)
            {
                s_certGenerator.AddExtension(X509Extensions.ExtendedKeyUsage, false, new ExtendedKeyUsage(KeyPurposeID.id_kp_serverAuth, KeyPurposeID.id_kp_clientAuth));
            }
            else
            {
                s_certGenerator.AddExtension(X509Extensions.ExtendedKeyUsage, false, new ExtendedKeyUsage(certificateCreationSettings.EKU));
            }

            if (!isAuthority)
            {
                if (isMachineCert)
                {
                    List<Asn1Encodable> subjectAlternativeNamesAsAsn1EncodableList = new List<Asn1Encodable>();

                    // All endpoints should also be in the Subject Alt Names 
                    for (int i = 0; i < subjectAlternativeNames.Length; i++)
                    {
                        if (!string.IsNullOrWhiteSpace(subjectAlternativeNames[i]))
                        {
                            // Machine certs can have additional DNS names
                            subjectAlternativeNamesAsAsn1EncodableList.Add(new GeneralName(GeneralName.DnsName, subjectAlternativeNames[i]));
                        }
                    }

                    s_certGenerator.AddExtension(X509Extensions.SubjectAlternativeName, true, new DerSequence(subjectAlternativeNamesAsAsn1EncodableList.ToArray()));
                }
                else
                {
                    if (subjectAlternativeNames.Length > 1)
                    {
                        var subjectAlternativeNamesAsAsn1EncodableList = new Asn1EncodableVector();

                        // Only add a SAN for the user if there are any
                        for (int i = 1; i < subjectAlternativeNames.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(subjectAlternativeNames[i]))
                            {
                                Asn1EncodableVector otherNames = new Asn1EncodableVector();
                                otherNames.Add(new DerObjectIdentifier(_upnObjectId));
                                otherNames.Add(new DerTaggedObject(true, 0, new DerUtf8String(subjectAlternativeNames[i])));

                                Asn1Object genName = new DerTaggedObject(false, 0, new DerSequence(otherNames));

                                subjectAlternativeNamesAsAsn1EncodableList.Add(genName);
                            }
                        }
                        s_certGenerator.AddExtension(X509Extensions.SubjectAlternativeName, true, new DerSequence(subjectAlternativeNamesAsAsn1EncodableList));
                    }
                }
            }

            if (isAuthority || certificateCreationSettings.IncludeCrlDistributionPoint)
            {
                var crlDistributionPoints = new DistributionPoint[1]
                {
                    new DistributionPoint(
                        new DistributionPointName(
                            new GeneralNames(
                                new GeneralName(
                                    GeneralName.UniformResourceIdentifier, string.Format("{0}", _crlUri, serialNum.ToString(radix: 16))))),
                                    null,
                                    null)
                };
                var revocationListExtension = new CrlDistPoint(crlDistributionPoints);
                s_certGenerator.AddExtension(X509Extensions.CrlDistributionPoints, false, revocationListExtension);
            }

            ISignatureFactory signatureFactory = new Asn1SignatureFactory(_signatureAlgorithm, _authorityKeyPair.Private, _random);
            X509Certificate cert = s_certGenerator.Generate(signatureFactory);

            switch (certificateCreationSettings.ValidityType)
            {
                case CertificateValidityType.Revoked:
                    RevokeCertificateBySerialNumber(serialNum.ToString(radix: 16));
                    break;
                case CertificateValidityType.Expired:
                    break;
                default:
                    EnsureCertificateIsValid(cert);
                    break;
            }

            // For now, given that we don't know what format to return it in, preserve the formats so we have 
            // the flexibility to do what we need to

            X509CertificateContainer container = new X509CertificateContainer();

            X509CertificateEntry[] chain = new X509CertificateEntry[1];
            chain[0] = new X509CertificateEntry(cert);

            Pkcs12Store store = new Pkcs12StoreBuilder().Build();
            store.SetKeyEntry(
                certificateCreationSettings.FriendlyName != null ? certificateCreationSettings.FriendlyName : string.Empty,
                new AsymmetricKeyEntry(keyPair.Private),
                chain);

            using (MemoryStream stream = new MemoryStream())
            {
                store.Save(stream, _password.ToCharArray(), _random);
                container.Pfx = stream.ToArray();
            }

            X509Certificate2 outputCert = null;

            if (isAuthority)
            {
                // don't hand out the private key for the cert when it's the authority
                outputCert = new X509Certificate2(cert.GetEncoded());
            }
            else
            {
                // Otherwise, allow encode with the private key. note that X509Certificate2.RawData will not provide the private key
                // you will have to re-export this cert if needed
                if (CertificateHelper.CurrentOperatingSystem.IsMacOS())
                {
                    //string tempKeychainFilePath = Path.GetTempFileName();
                    string tempKeychainFilePath = Path.Combine(Environment.CurrentDirectory, Path.GetRandomFileName());
                    System.Security.Cryptography.X509Certificates.X509Store MacOsTempStore = CertificateHelper.GetMacOSX509Store(tempKeychainFilePath);
                    MacOsTempStore.Certificates.Import(container.Pfx, _password, X509KeyStorageFlags.Exportable);
                    MacOsTempStore.Close();
                    MacOsTempStore.Dispose();

                    MacOsTempStore = CertificateHelper.GetMacOSX509Store(tempKeychainFilePath);

                    outputCert = ((IEnumerable<X509Certificate2>)MacOsTempStore.Certificates).FirstOrDefault();

                    if (outputCert == null)
                    {
                        Console.WriteLine("Couldn't find Certificate..");
                    }

                    MacOsTempStore.Dispose();
                    if (File.Exists(tempKeychainFilePath))
                    {
                        File.Delete(tempKeychainFilePath);
                    }
                }
                else
                {
                    outputCert = new X509Certificate2(container.Pfx, _password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
                }
            }

            container.Subject = subject;
            container.InternalCertificate = cert;
            container.Certificate = outputCert;
            container.Thumbprint = outputCert.Thumbprint;

            Trace.WriteLine("[CertificateGenerator] generated a certificate:");
            Trace.WriteLine(string.Format("    {0} = {1}", "isAuthority", isAuthority));
            if (!isAuthority)
            {
                Trace.WriteLine(string.Format("    {0} = {1}", "Signed by", signingCertificate.SubjectDN));
                Trace.WriteLine(string.Format("    {0} = {1}", "Subject (CN) ", subject));
                Trace.WriteLine(string.Format("    {0} = {1}", "Subject Alt names ", string.Join(", ", subjectAlternativeNames)));
                Trace.WriteLine(string.Format("    {0} = {1}", "Friendly Name ", certificateCreationSettings.FriendlyName));
            }
            Trace.WriteLine(string.Format("    {0} = {1}", "HasPrivateKey:", outputCert.HasPrivateKey));
            Trace.WriteLine(string.Format("    {0} = {1}", "Thumbprint", outputCert.Thumbprint));
            Trace.WriteLine(string.Format("    {0} = {1}", "CertificateValidityType", certificateCreationSettings.ValidityType));

            return container;
        }

        private X509Crl CreateCrl(X509Certificate signingCertificate)
        {
            EnsureInitialized();

            s_crlGenerator.Reset();

            DateTime now = DateTime.UtcNow;

            DateTime updateTime = now.Subtract(_crlValidityGracePeriodEnd);
            // Ensure that the update time for the CRL is no greater than the earliest time that the CA is valid for
            if (_defaultValidityNotBefore > now.Subtract(_crlValidityGracePeriodEnd))
            {
                updateTime = _defaultValidityNotBefore;
            }

            s_crlGenerator.SetThisUpdate(updateTime);
            //There is no need to update CRL.
            s_crlGenerator.SetNextUpdate(now.Add(ValidityPeriod));
            s_crlGenerator.SetIssuerDN(signingCertificate.SubjectDN);

            s_crlGenerator.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(signingCertificate));

            BigInteger crlNumber = new BigInteger(64 /*bits for the number*/, _random).Abs();
            s_crlGenerator.AddExtension(X509Extensions.CrlNumber, false, new CrlNumber(crlNumber));

            foreach (var kvp in s_revokedCertificates)
            {
                s_crlGenerator.AddCrlEntry(new BigInteger(kvp.Key, 16), kvp.Value, CrlReason.CessationOfOperation);
            }

            ISignatureFactory signatureFactory = new Asn1SignatureFactory(_signatureAlgorithm, _authorityKeyPair.Private, _random);
            X509Crl crl = s_crlGenerator.Generate(signatureFactory);
            crl.Verify(_authorityKeyPair.Public);

            Trace.WriteLine(string.Format("[CertificateGenerator] has created a Certificate Revocation List :"));
            Trace.WriteLine(string.Format("    {0} = {1}", "Issuer", crl.IssuerDN));
            Trace.WriteLine(string.Format("    {0} = {1}", "CRL Number", crlNumber));

            return crl;
        }

        // Throws an exception if the certificate is invalid
        private void EnsureCertificateIsValid(X509Certificate certificate)
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

            IList<DerObjectIdentifier> authorityKeyIdOrder = new List<DerObjectIdentifier>();
            IDictionary<DerObjectIdentifier, string> authorityKeyIdName = new Dictionary<DerObjectIdentifier, string>();

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
            BigInteger serialNumBigInt = null;
            try
            {
                serialNumBigInt = new BigInteger(str: serialNum, radix: 16);
                success = true;
            }
            catch (FormatException)
            {
                Trace.WriteLine("[CertificateGenerator] RevokeCertificateBySerialNumber:");
                Trace.WriteLine(string.Format("    Invalid serial number specified: '{0}'", serialNum));
            }

            if (success && !s_revokedCertificates.ContainsKey(serialNum))
            {
                s_revokedCertificates.Add(serialNum, DateTime.UtcNow);
            }

            // Note that we don't actually check against the thumbprints here, we just go ahead and stick the serial 
            // number into the CRL without checking whether or not we've ever generated it
            Trace.WriteLine(string.Format("[CertificateGenerator] Revoke certificate with serial number {0}: ", success ? "succeeded" : "FAILED"));
            return success;
        }
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
