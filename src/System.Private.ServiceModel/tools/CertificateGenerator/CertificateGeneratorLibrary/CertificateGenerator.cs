// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace WcfTestCommon
{
    // NOT THREADSAFE. Callers should lock before doing work with this class if multithreaded operation is expected.
    // This generator uses the .NET built-in X.509 APIs (CertificateRequest / RSA) so the produced
    // DER encodings are compatible with all platform X.509 stacks (including macOS Apple Security framework).
    public class CertificateGenerator
    {
        // OIDs
        private const string OidServerAuth = "1.3.6.1.5.5.7.3.1";
        private const string OidClientAuth = "1.3.6.1.5.5.7.3.2";
        private const string OidUpn = "1.3.6.1.4.1.311.20.2.3";
        private const string OidExtAuthorityKeyIdentifier = "2.5.29.35";
        private const string OidExtSubjectAlternativeName = "2.5.29.17";
        private const string OidExtCrlDistributionPoints = "2.5.29.31";
        private const string OidExtAuthorityInfoAccess = "1.3.6.1.5.5.7.1.1";
        private const string OidExtCrlNumber = "2.5.29.20";
        private const string OidSha256WithRsa = "1.2.840.113549.1.1.11";

        private bool _isInitialized;

        // Settable properties prior to initialization
        private string _crlUri;
        private string _crlServiceUri;
        private string _crlUriRelativePath;
        private string _password;
        private TimeSpan _validityPeriod = TimeSpan.FromDays(1);

        // This can't be too short as there might be a time skew between machines,
        // but also can't be too long, as the CRL is cached by the machine.
        private TimeSpan _crlValidityGracePeriodEnd = TimeSpan.FromMinutes(5);

        // Give the cert a grace period in case there's a time skew between machines
        private readonly TimeSpan _gracePeriod = TimeSpan.FromHours(1);

        private readonly string _authorityCanonicalName = "DO_NOT_TRUST_WcfBridgeRootCA";
        private readonly int _keyLengthInBits = 2048;

        // key: serial number (lowercase hex), value: revocation time
        private static Dictionary<string, DateTime> s_revokedCertificates = new Dictionary<string, DateTime>();

        private DateTime _initializationDateTime;
        private DateTime _defaultValidityNotBefore;
        private DateTime _defaultValidityNotAfter;

        // Authority private key + cert (with private key) used to sign all issued certificates
        private RSA _authorityKey;
        private X509Certificate2 _authorityCertWithKey;
        private X509CertificateContainer _authorityCertificate;

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

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

            _isInitialized = true;

            Trace.WriteLine("[CertificateGenerator] initialized with the following configuration:");
            Trace.WriteLine(string.Format("    {0} = {1}", "AuthorityCanonicalName", _authorityCanonicalName));
            Trace.WriteLine(string.Format("    {0} = {1}", "CrlUri", _crlUri));
            Trace.WriteLine(string.Format("    {0} = {1}", "Password", _password));
            Trace.WriteLine(string.Format("    {0} = {1}", "ValidityPeriod", _validityPeriod));
            Trace.WriteLine(string.Format("    {0} = {1}", "Valid to", _defaultValidityNotAfter));

            _authorityCertificate = CreateCertificate(isAuthority: true, isMachineCert: false, signingCertificate: null, certificateCreationSettings: null);
        }

        public void Reset()
        {
            _authorityCertificate = null;
            _authorityCertWithKey = null;
            if (_authorityKey != null)
            {
                _authorityKey.Dispose();
                _authorityKey = null;
            }
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
                return CreateCrl();
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
                return BuildDistinguishedName(_authorityCanonicalName).Name;
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
            get { return _crlServiceUri; }
            set
            {
                EnsureNotInitialized("CrlServiceUri");
                _crlServiceUri = value;
            }
        }

        public string CrlUriRelativePath
        {
            get { return _crlUriRelativePath; }
            set
            {
                EnsureNotInitialized("CrlUriRelativePath");
                _crlUriRelativePath = value;
            }
        }

        public List<string> RevokedCertificates
        {
            get { return new List<string>(s_revokedCertificates.Keys); }
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
            return CreateCertificate(false, true, _authorityCertWithKey, creationSettings);
        }

        public X509CertificateContainer CreateUserCertificate(CertificateCreationSettings creationSettings)
        {
            EnsureInitialized();
            return CreateCertificate(false, false, _authorityCertWithKey, creationSettings);
        }

        public static BigInteger HashFriendlyName(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                // Take first 20 bytes (160 bits) to fit in a typical serial number range
                byte[] hashBytes = sha256.ComputeHash(inputBytes).Take(20).ToArray();

                // Force a positive BigInteger by appending a zero byte if the high bit is set
                if ((hashBytes[0] & 0x80) != 0)
                {
                    byte[] padded = new byte[hashBytes.Length + 1];
                    Array.Copy(hashBytes, 0, padded, 1, hashBytes.Length);
                    hashBytes = padded;
                }

                // BigInteger ctor expects little-endian; reverse for big-endian hash
                Array.Reverse(hashBytes);
                return new BigInteger(hashBytes);
            }
        }

        public static string HashFriendlyNameToString(string input)
        {
            return HashFriendlyName(input).ToString("X").TrimStart('0');
        }

        // Only Initialize() should be calling with isAuthority = true.
        // If isAuthority, value for isMachineCert doesn't matter.
        private X509CertificateContainer CreateCertificate(bool isAuthority, bool isMachineCert, X509Certificate2 signingCertificate, CertificateCreationSettings certificateCreationSettings)
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

            if (certificateCreationSettings.ValidityNotBefore == default(DateTime))
            {
                certificateCreationSettings.ValidityNotBefore = _defaultValidityNotBefore;
            }
            if (certificateCreationSettings.ValidityNotAfter == default(DateTime))
            {
                certificateCreationSettings.ValidityNotAfter = _defaultValidityNotAfter;
            }

            // The authority cert needs a validity window wide enough to contain every child cert (including
            // intentionally expired ones with NotBefore in the past). Unlike BouncyCastle, .NET's
            // CertificateRequest.Create(issuer, notBefore, notAfter, ...) enforces that the issued cert's
            // window is contained within the issuer's. Use a generous ±10 year window for the authority.
            if (isAuthority)
            {
                certificateCreationSettings.ValidityNotBefore = _initializationDateTime.AddYears(-10);
                certificateCreationSettings.ValidityNotAfter = _initializationDateTime.AddYears(10);
            }
            else if (signingCertificate != null)
            {
                // Defensive clamp in case a caller passes dates outside the issuer window.
                if (certificateCreationSettings.ValidityNotBefore < signingCertificate.NotBefore.ToUniversalTime())
                {
                    certificateCreationSettings.ValidityNotBefore = signingCertificate.NotBefore.ToUniversalTime();
                }
                if (certificateCreationSettings.ValidityNotAfter > signingCertificate.NotAfter.ToUniversalTime())
                {
                    certificateCreationSettings.ValidityNotAfter = signingCertificate.NotAfter.ToUniversalTime();
                }
            }

            if (!isAuthority ^ (signingCertificate != null))
            {
                throw new ArgumentException("Either isAuthority == true or signingCertificate is not null");
            }
            string subject = certificateCreationSettings.Subject;

            // If certificateCreationSettings.SubjectAlternativeNames == null, then we should add exactly one SubjectAlternativeName == Subject
            // so that the default certificate generated is compatible with mainline scenarios.
            // However, if certificateCreationSettings.SubjectAlternativeNames == string[0], then allow this as this is a legit scenario we want to test out.
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

            // Tag on the generation time to prevent caching of the cert CRL on Linux
            X500DistinguishedName authorityDn = BuildDistinguishedName(string.Format("{0} {1}", _authorityCanonicalName, DateTime.Now.ToString("s")));

            byte[] serialNum = ComputeSerialNumber(certificateCreationSettings);

            RSA subjectKey = isAuthority ? (_authorityKey = RSA.Create(_keyLengthInBits)) : RSA.Create(_keyLengthInBits);

            X500DistinguishedName subjectDn;
            CertificateRequest req;

            if (isAuthority)
            {
                subjectDn = authorityDn;
                req = new CertificateRequest(subjectDn, subjectKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                req.CertificateExtensions.Add(new X509BasicConstraintsExtension(certificateAuthority: true, hasPathLengthConstraint: false, pathLengthConstraint: 0, critical: true));
                req.CertificateExtensions.Add(new X509KeyUsageExtension(
                    X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyAgreement | X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.CrlSign,
                    critical: false));
            }
            else
            {
                subjectDn = BuildDistinguishedName(subject);
                req = new CertificateRequest(subjectDn, subjectKey, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                req.CertificateExtensions.Add(new X509BasicConstraintsExtension(certificateAuthority: false, hasPathLengthConstraint: false, pathLengthConstraint: 0, critical: true));
                req.CertificateExtensions.Add(new X509KeyUsageExtension(
                    X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyAgreement | X509KeyUsageFlags.KeyEncipherment,
                    critical: false));
            }

            // SubjectKeyIdentifier
            X509SubjectKeyIdentifierExtension ski = new X509SubjectKeyIdentifierExtension(req.PublicKey, critical: false);
            req.CertificateExtensions.Add(ski);

            // AuthorityKeyIdentifier — built manually since X509AuthorityKeyIdentifierExtension is .NET 7+
            byte[] authorityKeyId = isAuthority
                ? HexToBytes(ski.SubjectKeyIdentifier)
                : GetSubjectKeyIdentifierBytes(signingCertificate);
            req.CertificateExtensions.Add(BuildAuthorityKeyIdentifierExtension(authorityKeyId));

            // Extended Key Usage
            OidCollection ekuOids = new OidCollection();
            if (certificateCreationSettings.EKU == null || certificateCreationSettings.EKU.Count == 0)
            {
                ekuOids.Add(new Oid(OidServerAuth));
                ekuOids.Add(new Oid(OidClientAuth));
            }
            else
            {
                foreach (string ekuOid in certificateCreationSettings.EKU)
                {
                    ekuOids.Add(new Oid(ekuOid));
                }
            }
            if (ekuOids.Count > 0)
            {
                req.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(ekuOids, critical: false));
            }

            // Subject Alternative Name
            if (!isAuthority)
            {
                if (isMachineCert)
                {
                    SubjectAlternativeNameBuilder sanBuilder = new SubjectAlternativeNameBuilder();
                    bool any = false;
                    foreach (string san in subjectAlternativeNames)
                    {
                        if (!string.IsNullOrWhiteSpace(san))
                        {
                            sanBuilder.AddDnsName(san);
                            any = true;
                        }
                    }
                    if (any)
                    {
                        // SubjectAlternativeNameBuilder.Build defaults to non-critical; rebuild as critical.
                        X509Extension defaultSan = sanBuilder.Build(critical: true);
                        req.CertificateExtensions.Add(defaultSan);
                    }
                }
                else
                {
                    // User cert: skip the first SAN (which mirrors Subject) and emit remaining as UPN OtherName entries.
                    if (subjectAlternativeNames.Length > 1)
                    {
                        List<string> upns = new List<string>();
                        for (int i = 1; i < subjectAlternativeNames.Length; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(subjectAlternativeNames[i]))
                            {
                                upns.Add(subjectAlternativeNames[i]);
                            }
                        }
                        if (upns.Count > 0)
                        {
                            req.CertificateExtensions.Add(BuildUpnSubjectAlternativeNameExtension(upns));
                        }
                    }
                }
            }

            // CRL Distribution Points
            if (isAuthority || certificateCreationSettings.IncludeCrlDistributionPoint)
            {
                req.CertificateExtensions.Add(BuildCrlDistributionPointsExtension(_crlUri));
            }

            X509Certificate2 cert;
            if (isAuthority)
            {
                cert = req.CreateSelfSigned(certificateCreationSettings.ValidityNotBefore, certificateCreationSettings.ValidityNotAfter);
            }
            else
            {
                cert = req.Create(signingCertificate, certificateCreationSettings.ValidityNotBefore, certificateCreationSettings.ValidityNotAfter, serialNum);
            }

            // Build a complete X509Certificate2 with the private key attached.
            // CreateSelfSigned already attaches the private key; req.Create(issuer,...) does not.
            X509Certificate2 certWithKey = cert.HasPrivateKey ? cert : cert.CopyWithPrivateKey(subjectKey);

            // For consistency with previous behavior, always export the cert with private key as PFX.
            // X509KeyStorageFlags.Exportable lets callers re-export later.
            byte[] pfxBytes = certWithKey.Export(X509ContentType.Pkcs12, _password);

            // On Windows (and Linux), round-trip through X509CertificateLoader.LoadPkcs12 with
            // PersistKeySet so the private key lands in the platform key container; otherwise the
            // in-memory ephemeral key from CopyWithPrivateKey won't survive being added to a cert
            // store and lookups later return a cert with no usable private key.
            //
            // On macOS, LoadPkcs12 fails with "The specified keychain could not be found" because
            // the Apple loader needs a keychain handle to attach the key. CertificateManager bypasses
            // X509Store on macOS (uses the security CLI), so the in-memory CopyWithPrivateKey result
            // works there.
            X509Certificate2 outputCert;
            if (CertificateHelper.CurrentOperatingSystem.IsMacOS())
            {
                outputCert = certWithKey;
            }
            else
            {
                outputCert = X509CertificateLoader.LoadPkcs12(
                    pfxBytes,
                    _password,
                    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
            }

            switch (certificateCreationSettings.ValidityType)
            {
                case CertificateValidityType.Revoked:
                    RevokeCertificateBySerialNumber(SerialToHex(serialNum));
                    break;
                case CertificateValidityType.Expired:
                    break;
                default:
                    EnsureCertificateIsValid(outputCert);
                    break;
            }

            X509CertificateContainer container = new X509CertificateContainer
            {
                Subject = subject,
                Pfx = pfxBytes,
                Certificate = outputCert,
                Thumbprint = outputCert.Thumbprint
            };

            if (isAuthority)
            {
                _authorityCertWithKey = certWithKey;
                // certWithKey is the same instance as cert (CreateSelfSigned attaches the key in-place); do not dispose.
                // On non-macOS, outputCert is a separate (round-tripped) instance owned by the container.
            }
            else
            {
                // Dispose the keyless original if it's a separate instance.
                if (!ReferenceEquals(cert, certWithKey))
                {
                    cert.Dispose();
                }
                // On non-macOS, outputCert is the round-tripped instance owned by the container;
                // certWithKey is orphaned and should be disposed. On macOS, outputCert == certWithKey.
                if (!ReferenceEquals(outputCert, certWithKey))
                {
                    certWithKey.Dispose();
                }
            }

            Trace.WriteLine("[CertificateGenerator] generated a certificate:");
            Trace.WriteLine(string.Format("    {0} = {1}", "isAuthority", isAuthority));
            if (!isAuthority)
            {
                Trace.WriteLine(string.Format("    {0} = {1}", "Signed by", signingCertificate.SubjectName.Name));
                Trace.WriteLine(string.Format("    {0} = {1}", "Subject (CN) ", subject));
                Trace.WriteLine(string.Format("    {0} = {1}", "Subject Alt names ", string.Join(", ", subjectAlternativeNames)));
                Trace.WriteLine(string.Format("    {0} = {1}", "Friendly Name ", certificateCreationSettings.FriendlyName));
            }
            Trace.WriteLine(string.Format("    {0} = {1}", "HasPrivateKey:", outputCert.HasPrivateKey));
            Trace.WriteLine(string.Format("    {0} = {1}", "Thumbprint", outputCert.Thumbprint));
            Trace.WriteLine(string.Format("    {0} = {1}", "CertificateValidityType", certificateCreationSettings.ValidityType));

            return container;
        }

        private byte[] ComputeSerialNumber(CertificateCreationSettings settings)
        {
            // On non-Windows hosts, use a deterministic serial derived from FriendlyName so cleanup-by-serial works.
            BigInteger serialBigInt;
            if (!CertificateHelper.CurrentOperatingSystem.IsWindows() && settings != null && settings.FriendlyName != null)
            {
                serialBigInt = HashFriendlyName(settings.FriendlyName);
            }
            else
            {
                byte[] rand = new byte[8];
                using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(rand);
                }
                // Force positive
                rand[0] &= 0x7F;
                Array.Reverse(rand);
                serialBigInt = new BigInteger(rand);
                if (serialBigInt.Sign < 0)
                {
                    serialBigInt = -serialBigInt;
                }
            }

            // Convert to big-endian byte[] for CertificateRequest.Create(serialNumber)
            byte[] little = serialBigInt.ToByteArray();
            // Trim trailing zero (sign byte) if present
            int len = little.Length;
            if (len > 1 && little[len - 1] == 0 && (little[len - 2] & 0x80) == 0)
            {
                len--;
            }
            byte[] big = new byte[len];
            for (int i = 0; i < len; i++)
            {
                big[i] = little[len - 1 - i];
            }
            return big;
        }

        private static string SerialToHex(byte[] serialBigEndian)
        {
            StringBuilder sb = new StringBuilder(serialBigEndian.Length * 2);
            for (int i = 0; i < serialBigEndian.Length; i++)
            {
                sb.Append(serialBigEndian[i].ToString("x2"));
            }
            // Trim leading zeros for hex string compatibility
            string s = sb.ToString().TrimStart('0');
            return s.Length == 0 ? "0" : s;
        }

        private byte[] CreateCrl()
        {
            EnsureInitialized();

            DateTime now = DateTime.UtcNow;
            DateTime updateTime = now.Subtract(_crlValidityGracePeriodEnd);
            if (_defaultValidityNotBefore > updateTime)
            {
                updateTime = _defaultValidityNotBefore;
            }
            DateTime nextUpdate = now.Add(_validityPeriod);

            // Build TBSCertList
            byte[] tbs = BuildTbsCertList(updateTime, nextUpdate);

            // Sign TBSCertList
            byte[] signature = _authorityKey.SignData(tbs, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            // Build CertificateList
            AsnWriter w = new AsnWriter(AsnEncodingRules.DER);
            using (w.PushSequence())
            {
                w.WriteEncodedValue(tbs);
                WriteSha256RsaAlgorithmIdentifier(w);
                w.WriteBitString(signature);
            }

            byte[] crl = w.Encode();

            Trace.WriteLine(string.Format("[CertificateGenerator] has created a Certificate Revocation List:"));
            Trace.WriteLine(string.Format("    {0} = {1}", "Issuer", _authorityCertWithKey.SubjectName.Name));
            Trace.WriteLine(string.Format("    {0} = {1} bytes", "Length", crl.Length));

            return crl;
        }

        private byte[] BuildTbsCertList(DateTime thisUpdate, DateTime nextUpdate)
        {
            AsnWriter w = new AsnWriter(AsnEncodingRules.DER);
            using (w.PushSequence())
            {
                // version v2 = INTEGER 1
                w.WriteInteger(1);

                // signature AlgorithmIdentifier
                WriteSha256RsaAlgorithmIdentifier(w);

                // issuer Name (DER bytes from authority cert subject)
                w.WriteEncodedValue(_authorityCertWithKey.SubjectName.RawData);

                // thisUpdate
                WriteX509Time(w, thisUpdate);

                // nextUpdate
                WriteX509Time(w, nextUpdate);

                // revokedCertificates OPTIONAL
                if (s_revokedCertificates.Count > 0)
                {
                    using (w.PushSequence())
                    {
                        foreach (KeyValuePair<string, DateTime> kvp in s_revokedCertificates)
                        {
                            using (w.PushSequence())
                            {
                                // userCertificate (CertificateSerialNumber INTEGER)
                                BigInteger serial = HexToBigInteger(kvp.Key);
                                w.WriteInteger(serial);
                                // revocationDate
                                WriteX509Time(w, kvp.Value);
                            }
                        }
                    }
                }

                // crlExtensions [0] EXPLICIT Extensions
                using (w.PushSequence(new Asn1Tag(TagClass.ContextSpecific, 0, isConstructed: true)))
                {
                    using (w.PushSequence())
                    {
                        // CRL Number extension
                        WriteExtension(w, OidExtCrlNumber, critical: false, value: BuildCrlNumberValue());
                        // AKI extension (key id only)
                        byte[] aki = GetSubjectKeyIdentifierBytes(_authorityCertWithKey);
                        WriteExtension(w, OidExtAuthorityKeyIdentifier, critical: false, value: BuildAuthorityKeyIdentifierValue(aki));
                    }
                }
            }
            return w.Encode();
        }

        private static byte[] BuildCrlNumberValue()
        {
            // Use a random positive 64-bit integer as the CRL number.
            byte[] rand = new byte[8];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(rand);
            }
            rand[0] &= 0x7F;
            Array.Reverse(rand);
            BigInteger n = new BigInteger(rand);
            if (n.Sign < 0)
            {
                n = -n;
            }
            AsnWriter w = new AsnWriter(AsnEncodingRules.DER);
            w.WriteInteger(n);
            return w.Encode();
        }

        private static void WriteExtension(AsnWriter w, string oid, bool critical, byte[] value)
        {
            using (w.PushSequence())
            {
                w.WriteObjectIdentifier(oid);
                if (critical)
                {
                    w.WriteBoolean(true);
                }
                w.WriteOctetString(value);
            }
        }

        private static void WriteSha256RsaAlgorithmIdentifier(AsnWriter w)
        {
            using (w.PushSequence())
            {
                w.WriteObjectIdentifier(OidSha256WithRsa);
                w.WriteNull();
            }
        }

        // RFC 5280: Time ::= CHOICE { utcTime UTCTime, generalTime GeneralizedTime }
        // CAs MUST encode dates through 2049 as UTCTime; dates 2050+ as GeneralizedTime.
        private static void WriteX509Time(AsnWriter w, DateTime utc)
        {
            DateTimeOffset dto = new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc));
            if (dto.UtcDateTime.Year < 2050)
            {
                w.WriteUtcTime(dto);
            }
            else
            {
                w.WriteGeneralizedTime(dto, omitFractionalSeconds: true);
            }
        }

        private static X500DistinguishedName BuildDistinguishedName(string canonicalName)
        {
            // Order in DN: CN, O, OU (encoded inner-to-outer / RFC 4514 reverse of issuance)
            // Use X500DistinguishedName parser; quote values that may contain spaces.
            string dn = string.Format("CN={0}, O=DO_NOT_TRUST, OU=Created by https://github.com/dotnet/wcf",
                EscapeDnComponent(canonicalName));
            return new X500DistinguishedName(dn);
        }

        private static string EscapeDnComponent(string value)
        {
            // Minimal escaping for special chars in RFC 4514 DN strings.
            StringBuilder sb = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                if (c == ',' || c == '+' || c == '"' || c == '\\' || c == '<' || c == '>' || c == ';' || c == '=' || c == '#')
                {
                    sb.Append('\\');
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        private static byte[] GetSubjectKeyIdentifierBytes(X509Certificate2 cert)
        {
            foreach (X509Extension ext in cert.Extensions)
            {
                if (ext.Oid != null && ext.Oid.Value == "2.5.29.14")
                {
                    // SubjectKeyIdentifier extension; value is OCTET STRING containing OCTET STRING (the key id)
                    AsnReader r = new AsnReader(ext.RawData, AsnEncodingRules.DER);
                    return r.ReadOctetString();
                }
            }

            // Fallback: compute SHA-1 of the DER-encoded SubjectPublicKey BIT STRING (just the key bytes).
            using (SHA1 sha1 = SHA1.Create())
            {
                return sha1.ComputeHash(cert.PublicKey.EncodedKeyValue.RawData);
            }
        }

        // Builds X509Extension for AuthorityKeyIdentifier (only keyIdentifier field).
        // AuthorityKeyIdentifier ::= SEQUENCE { keyIdentifier [0] IMPLICIT OCTET STRING OPTIONAL, ... }
        private static X509Extension BuildAuthorityKeyIdentifierExtension(byte[] keyIdentifier)
        {
            return new X509Extension(OidExtAuthorityKeyIdentifier, BuildAuthorityKeyIdentifierValue(keyIdentifier), critical: false);
        }

        private static byte[] BuildAuthorityKeyIdentifierValue(byte[] keyIdentifier)
        {
            AsnWriter w = new AsnWriter(AsnEncodingRules.DER);
            using (w.PushSequence())
            {
                w.WriteOctetString(keyIdentifier, new Asn1Tag(TagClass.ContextSpecific, 0, isConstructed: false));
            }
            return w.Encode();
        }

        // Builds CRLDistributionPoints extension for a single fullName URI distribution point.
        // CRLDistributionPoints ::= SEQUENCE OF DistributionPoint
        // DistributionPoint ::= SEQUENCE { distributionPoint [0] EXPLICIT DistributionPointName OPTIONAL, ... }
        // DistributionPointName ::= CHOICE { fullName [0] IMPLICIT GeneralNames, ... }
        // GeneralName ::= CHOICE { uniformResourceIdentifier [6] IMPLICIT IA5String, ... }
        private static X509Extension BuildCrlDistributionPointsExtension(string url)
        {
            AsnWriter w = new AsnWriter(AsnEncodingRules.DER);
            using (w.PushSequence())
            {
                using (w.PushSequence())
                {
                    using (w.PushSequence(new Asn1Tag(TagClass.ContextSpecific, 0, isConstructed: true)))
                    {
                        using (w.PushSequence(new Asn1Tag(TagClass.ContextSpecific, 0, isConstructed: true)))
                        {
                            w.WriteCharacterString(UniversalTagNumber.IA5String, url, new Asn1Tag(TagClass.ContextSpecific, 6, isConstructed: false));
                        }
                    }
                }
            }
            return new X509Extension(OidExtCrlDistributionPoints, w.Encode(), critical: false);
        }

        // SubjectAltName extension containing UPN OtherName entries.
        // GeneralName ::= CHOICE { otherName [0] IMPLICIT OtherName, ... }
        // OtherName ::= SEQUENCE { type-id OBJECT IDENTIFIER, value [0] EXPLICIT ANY DEFINED BY type-id }
        private static X509Extension BuildUpnSubjectAlternativeNameExtension(IEnumerable<string> upns)
        {
            AsnWriter w = new AsnWriter(AsnEncodingRules.DER);
            using (w.PushSequence())
            {
                foreach (string upn in upns)
                {
                    using (w.PushSequence(new Asn1Tag(TagClass.ContextSpecific, 0, isConstructed: true)))
                    {
                        w.WriteObjectIdentifier(OidUpn);
                        using (w.PushSequence(new Asn1Tag(TagClass.ContextSpecific, 0, isConstructed: true)))
                        {
                            w.WriteCharacterString(UniversalTagNumber.UTF8String, upn);
                        }
                    }
                }
            }
            return new X509Extension(OidExtSubjectAlternativeName, w.Encode(), critical: true);
        }

        private static byte[] HexToBytes(string hex)
        {
            if (hex == null)
            {
                return new byte[0];
            }
            // Strip whitespace and hyphens
            StringBuilder cleaned = new StringBuilder(hex.Length);
            foreach (char c in hex)
            {
                if (!char.IsWhiteSpace(c) && c != '-' && c != ':')
                {
                    cleaned.Append(c);
                }
            }
            string s = cleaned.ToString();
            if ((s.Length & 1) == 1)
            {
                s = "0" + s;
            }
            byte[] bytes = new byte[s.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = byte.Parse(s.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return bytes;
        }

        private static BigInteger HexToBigInteger(string hex)
        {
            if (string.IsNullOrEmpty(hex))
            {
                return BigInteger.Zero;
            }
            // Ensure positive parsing
            string s = hex.StartsWith("0", StringComparison.Ordinal) ? hex : "0" + hex;
            return BigInteger.Parse(s, System.Globalization.NumberStyles.HexNumber);
        }

        // Throws an exception if the certificate is invalid.
        private void EnsureCertificateIsValid(X509Certificate2 certificate)
        {
            DateTime now = DateTime.UtcNow;
            if (now < certificate.NotBefore.ToUniversalTime() || now > certificate.NotAfter.ToUniversalTime())
            {
                throw new CryptographicException(string.Format("Certificate is outside its validity window: {0} - {1}", certificate.NotBefore, certificate.NotAfter));
            }
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

        public bool RevokeCertificateBySerialNumber(string serialNum)
        {
            bool success = false;
            try
            {
                // Validate hex parses
                HexToBigInteger(serialNum);
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
            // number into the CRL without checking whether or not we've ever generated it.
            Trace.WriteLine(string.Format("[CertificateGenerator] Revoke certificate with serial number {0}: ", success ? "succeeded" : "FAILED"));
            return success;
        }
    }

    public class X509CertificateContainer
    {
        public string Subject { get; internal set; }
        public X509Certificate2 Certificate { get; internal set; }
        public byte[] Pfx { get; internal set; }
        public string Thumbprint { get; internal set; }
    }
}
