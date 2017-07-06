// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;

namespace System.IdentityModel.Claims
{
    public class X509CertificateClaimSet : ClaimSet, IIdentityInfo, IDisposable
    {
        private X509Certificate2 _certificate;
        private DateTime _expirationTime = SecurityUtils.MinUtcDateTime;
        private ClaimSet _issuer;
        private X509Identity _identity;
        private X509ChainElementCollection _elements;
        private IList<Claim> _claims;
        private int _index;
        private bool _disposed = false;

        public X509CertificateClaimSet(X509Certificate2 certificate)
            : this(certificate, true)
        {
        }

        internal X509CertificateClaimSet(X509Certificate2 certificate, bool clone)
        {
            if (certificate == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("certificate");
            
             _certificate = clone ? new X509Certificate2(certificate) : certificate;
        }

        private X509CertificateClaimSet(X509CertificateClaimSet from)
            : this(from.X509Certificate, true)
        {
        }

        private X509CertificateClaimSet(X509ChainElementCollection elements, int index)
        {
            _elements = elements;
            _index = index;
            _certificate = elements[index].Certificate;
        }

        public override Claim this[int index]
        {
            get
            {
                ThrowIfDisposed();
                EnsureClaims();
                return _claims[index];
            }
        }

        public override int Count
        {
            get
            {
                ThrowIfDisposed();
                EnsureClaims();
                return _claims.Count;
            }
        }

        IIdentity IIdentityInfo.Identity
        {
            get
            {
                ThrowIfDisposed();
                if (_identity == null)
                    _identity = new X509Identity(_certificate, false, false);
                return _identity;
            }
        }

        public DateTime ExpirationTime
        {
            get
            {
                ThrowIfDisposed();
                if (_expirationTime == SecurityUtils.MinUtcDateTime)
                    _expirationTime = _certificate.NotAfter.ToUniversalTime();
                return _expirationTime;
            }
        }

        public override ClaimSet Issuer
        {
            get
            {
                ThrowIfDisposed();
                if (_issuer == null)
                {
                    if (_elements == null)
                    {
                        X509Chain chain = new X509Chain();
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
                        chain.Build(_certificate);
                        _index = 0;
                        _elements = chain.ChainElements;
                    }

                    if (_index + 1 < _elements.Count)
                    {
                        _issuer = new X509CertificateClaimSet(_elements, _index + 1);
                        _elements = null;
                    }
                    // SelfSigned?
                    else if (StringComparer.OrdinalIgnoreCase.Equals(_certificate.SubjectName.Name, _certificate.IssuerName.Name))
                        _issuer = this;
                    else
                        _issuer = new X500DistinguishedNameClaimSet(_certificate.IssuerName);
                }
                return _issuer;
            }
        }

        public X509Certificate2 X509Certificate
        {
            get
            {
                ThrowIfDisposed();
                return _certificate;
            }
        }

        internal X509CertificateClaimSet Clone()
        {
            ThrowIfDisposed();
            return new X509CertificateClaimSet(this);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                SecurityUtils.DisposeIfNecessary(_identity);
                if (_issuer != null)
                {
                    if (_issuer != this)
                    {
                        SecurityUtils.DisposeIfNecessary(_issuer as IDisposable);
                    }
                }
                if (_elements != null)
                {
                    for (int i = _index + 1; i < _elements.Count; ++i)
                    {
                        SecurityUtils.ResetCertificate(_elements[i].Certificate);
                    }
                }
                SecurityUtils.ResetCertificate(_certificate);
            }
        }

        private IList<Claim> InitializeClaimsCore()
        {
            List<Claim> claims = new List<Claim>();
            byte[] thumbprint = _certificate.GetCertHash();
            claims.Add(new Claim(ClaimTypes.Thumbprint, thumbprint, Rights.Identity));
            claims.Add(new Claim(ClaimTypes.Thumbprint, thumbprint, Rights.PossessProperty));

            // Ordering SubjectName, Dns, SimpleName, Email, Upn
            string value = _certificate.SubjectName.Name;
            if (!string.IsNullOrEmpty(value))
                claims.Add(Claim.CreateX500DistinguishedNameClaim(_certificate.SubjectName));

            // A SAN field can have multiple DNS names
            string[] dnsEntries = GetDnsFromExtensions(_certificate);
            if (dnsEntries.Length > 0)
            {
                for (int i = 0; i < dnsEntries.Length; ++i)
                {
                    claims.Add(Claim.CreateDnsClaim(dnsEntries[i]));
                }
            }
            else
            {
                // If no SANs found in certificate, fall back to looking for the CN
                value = _certificate.GetNameInfo(X509NameType.DnsName, false);
                if (!string.IsNullOrEmpty(value))
                {
                    claims.Add(Claim.CreateDnsClaim(value));
                }
            }

            value = _certificate.GetNameInfo(X509NameType.SimpleName, false);
            if (!string.IsNullOrEmpty(value))
                claims.Add(Claim.CreateNameClaim(value));

            value = _certificate.GetNameInfo(X509NameType.UpnName, false);
            if (!string.IsNullOrEmpty(value))
                claims.Add(Claim.CreateUpnClaim(value));

            value = _certificate.GetNameInfo(X509NameType.UrlName, false);
            if (!string.IsNullOrEmpty(value))
                claims.Add(Claim.CreateUriClaim(new Uri(value)));

            //RSA rsa = _certificate.PublicKey.Key as RSA;
            //if (rsa != null)
            //    claims.Add(Claim.CreateRsaClaim(rsa));

            return claims;
        }

        private void EnsureClaims()
        {
            if (_claims != null)
                return;

            _claims = InitializeClaimsCore();
        }

        private static bool SupportedClaimType(string claimType)
        {
            return claimType == null ||
                ClaimTypes.Thumbprint.Equals(claimType) ||
                ClaimTypes.X500DistinguishedName.Equals(claimType) ||
                ClaimTypes.Dns.Equals(claimType) ||
                ClaimTypes.Name.Equals(claimType) ||
                ClaimTypes.Email.Equals(claimType) ||
                ClaimTypes.Upn.Equals(claimType) ||
                ClaimTypes.Uri.Equals(claimType) ||
                ClaimTypes.Rsa.Equals(claimType);
        }

        // Note: null string represents any.
        public override IEnumerable<Claim> FindClaims(string claimType, string right)
        {
            ThrowIfDisposed();
            if (!SupportedClaimType(claimType) || !ClaimSet.SupportedRight(right))
            {
                yield break;
            }
            else if (_claims == null && ClaimTypes.Thumbprint.Equals(claimType))
            {
                if (right == null || Rights.Identity.Equals(right))
                {
                    yield return new Claim(ClaimTypes.Thumbprint, _certificate.GetCertHash(), Rights.Identity);
                }
                if (right == null || Rights.PossessProperty.Equals(right))
                {
                    yield return new Claim(ClaimTypes.Thumbprint, _certificate.GetCertHash(), Rights.PossessProperty);
                }
            }
            else if (_claims == null && ClaimTypes.Dns.Equals(claimType))
            {
                if (right == null || Rights.PossessProperty.Equals(right))
                {
                    // A SAN field can have multiple DNS names
                    string[] dnsEntries = GetDnsFromExtensions(_certificate);
                    if (dnsEntries.Length > 0)
                    {
                        for (int i = 0; i < dnsEntries.Length; ++i)
                        {
                            yield return Claim.CreateDnsClaim(dnsEntries[i]);
                        }
                    }
                    else
                    {
                        // If no SANs found in certificate, fall back to looking at the CN
                        string value = _certificate.GetNameInfo(X509NameType.DnsName, false);
                        if (!string.IsNullOrEmpty(value))
                        {
                            yield return Claim.CreateDnsClaim(value);
                        }
                    }
                }
            }
            else
            {
                EnsureClaims();

                bool anyClaimType = (claimType == null);
                bool anyRight = (right == null);

                for (int i = 0; i < _claims.Count; ++i)
                {
                    Claim claim = _claims[i];
                    if ((claim != null) &&
                        (anyClaimType || claimType.Equals(claim.ClaimType)) &&
                        (anyRight || right.Equals(claim.Right)))
                    {
                        yield return claim;
                    }
                }
            }
        }

        private static string[] GetDnsFromExtensions(X509Certificate2 cert)
        {
            foreach (X509Extension ext in cert.Extensions)
            {
                // Extension is SAN2
                if (ext.Oid.Value == X509SubjectAlternativeNameConstants.Oid)
                {
                    string asnString = ext.Format(false);
                    if (string.IsNullOrWhiteSpace(asnString))
                    {
                        return new string[0];
                    }

                    // SubjectAlternativeNames might contain something other than a dNSName, 
                    // so we have to parse through and only use the dNSNames
                    // <identifier><delimter><value><separator(s)>

                    string[] rawDnsEntries =
                        asnString.Split(new string[1] { X509SubjectAlternativeNameConstants.Separator }, StringSplitOptions.RemoveEmptyEntries);

                    List<string> dnsEntries = new List<string>();

                    for (int i = 0; i < rawDnsEntries.Length; i++)
                    {
                        string[] keyval = rawDnsEntries[i].Split(X509SubjectAlternativeNameConstants.Delimiter);
                        if (string.Equals(keyval[0], X509SubjectAlternativeNameConstants.Identifier))
                        {
                            dnsEntries.Add(keyval[1]);
                        }
                    }

                    return dnsEntries.ToArray();
                }
            }
            return new string[0];
        }

        public override IEnumerator<Claim> GetEnumerator()
        {
            ThrowIfDisposed();
            EnsureClaims();
            return _claims.GetEnumerator();
        }

        public override string ToString()
        {
            return _disposed ? base.ToString() : SecurityUtils.ClaimSetToString(this);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.GetType().FullName));
            }
        }

        private class X500DistinguishedNameClaimSet : DefaultClaimSet, IIdentityInfo
        {
            private IIdentity _identity;

            public X500DistinguishedNameClaimSet(X500DistinguishedName x500DistinguishedName)
            {
                if (x500DistinguishedName == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("x500DistinguishedName");

                _identity = new X509Identity(x500DistinguishedName);
                List<Claim> claims = new List<Claim>(2);
                claims.Add(new Claim(ClaimTypes.X500DistinguishedName, x500DistinguishedName, Rights.Identity));
                claims.Add(Claim.CreateX500DistinguishedNameClaim(x500DistinguishedName));
                Initialize(ClaimSet.Anonymous, claims);
            }

            public IIdentity Identity
            {
                get { return _identity; }
            }
        }

        // We don't have a strongly typed extension to parse Subject Alt Names, so we have to do a workaround 
        // to figure out what the identifier, delimiter, and separator is by using a well-known extension
        private static class X509SubjectAlternativeNameConstants
        {
            public const string Oid = "2.5.29.17";

            private static readonly string s_identifier;
            private static readonly char s_delimiter;
            private static readonly string s_separator;

            private static bool s_successfullyInitialized = false;
            private static Exception s_initializationException;

            public static string Identifier
            {
                get
                {
                    EnsureInitialized();
                    return s_identifier;
                }
            }

            public static char Delimiter
            {
                get
                {
                    EnsureInitialized();
                    return s_delimiter;
                }
            }
            public static string Separator
            {
                get
                {
                    EnsureInitialized();
                    return s_separator;
                }
            }

            private static void EnsureInitialized()
            {
                if (!s_successfullyInitialized)
                {
                    throw new FormatException(string.Format(
                        "There was an error detecting the identifier, delimiter, and separator for X509CertificateClaims on this platform.{0}" +
                        "Detected values were: Identifier: '{1}'; Delimiter:'{2}'; Separator:'{3}'",
                        Environment.NewLine,
                        s_identifier,
                        s_delimiter,
                        s_separator
                    ), s_initializationException);
                }
            }

            // static initializer runs only when one of the properties is accessed
            static X509SubjectAlternativeNameConstants()
            {
                // Extracted a well-known X509Extension
                byte[] x509ExtensionBytes = new byte[] {
                    48, 36, 130, 21, 110, 111, 116, 45, 114, 101, 97, 108, 45, 115, 117, 98, 106, 101, 99,
                    116, 45, 110, 97, 109, 101, 130, 11, 101, 120, 97, 109, 112, 108, 101, 46, 99, 111, 109
                };
                const string subjectName1 = "not-real-subject-name";

                try
                {
                    X509Extension x509Extension = new X509Extension(Oid, x509ExtensionBytes, true);
                    string x509ExtensionFormattedString = x509Extension.Format(false);

                    // Each OS has a different dNSName identifier and delimiter
                    // On Windows, dNSName == "DNS Name" (localizable), on Linux, dNSName == "DNS"
                    // e.g.,
                    // Windows: x509ExtensionFormattedString is: "DNS Name=not-real-subject-name, DNS Name=example.com"
                    // Linux:   x509ExtensionFormattedString is: "DNS:not-real-subject-name, DNS:example.com"
                    // Parse: <identifier><delimter><value><separator(s)>

                    int delimiterIndex = x509ExtensionFormattedString.IndexOf(subjectName1) - 1;
                    s_delimiter = x509ExtensionFormattedString[delimiterIndex];

                    // Make an assumption that all characters from the the start of string to the delimiter 
                    // are part of the identifier
                    s_identifier = x509ExtensionFormattedString.Substring(0, delimiterIndex);

                    int separatorFirstChar = delimiterIndex + subjectName1.Length + 1;
                    int separatorLength = 1;
                    for (int i = separatorFirstChar + 1; i < x509ExtensionFormattedString.Length; i++)
                    {
                        // We advance until the first character of the identifier to determine what the
                        // separator is. This assumes that the identifier assumption above is correct
                        if (x509ExtensionFormattedString[i] == s_identifier[0])
                        {
                            break;
                        }

                        separatorLength++;
                    }

                    s_separator = x509ExtensionFormattedString.Substring(separatorFirstChar, separatorLength);

                    s_successfullyInitialized = true;
                }
                catch (Exception ex)
                {
                    s_successfullyInitialized = false;
                    s_initializationException = ex;
                }
            }
        }
    }

    internal class X509Identity : GenericIdentity, IDisposable
    {
        private const string X509 = "X509";
        private const string Thumbprint = "; ";
        private X500DistinguishedName _x500DistinguishedName;
        private X509Certificate2 _certificate;
        private string _name;
        private bool _disposed = false;
        private bool _disposable = true;

        public X509Identity(X509Certificate2 certificate)
            : this(certificate, true, true)
        {
        }

        public X509Identity(X500DistinguishedName x500DistinguishedName)
            : base(X509, X509)
        {
            _x500DistinguishedName = x500DistinguishedName;
        }

        internal X509Identity(X509Certificate2 certificate, bool clone, bool disposable)
            : base(X509, X509)
        {
             _certificate = clone ? new X509Certificate2(certificate) : certificate;

            _disposable = clone || disposable;
        }

        public override string Name
        {
            get
            {
                ThrowIfDisposed();
                if (_name == null)
                {
                    //
                    // PrincipalPermission authorization using certificates could cause Elevation of Privilege.
                    // because there could be duplicate subject name.  In order to be more unique, we use SubjectName + Thumbprint
                    // instead
                    //
                    _name = GetName() + Thumbprint + _certificate.Thumbprint;
                }
                return _name;
            }
        }

        private string GetName()
        {
            if (_x500DistinguishedName != null)
                return _x500DistinguishedName.Name;

            string value = _certificate.SubjectName.Name;
            if (!string.IsNullOrEmpty(value))
                return value;

            value = _certificate.GetNameInfo(X509NameType.DnsName, false);
            if (!string.IsNullOrEmpty(value))
                return value;

            value = _certificate.GetNameInfo(X509NameType.SimpleName, false);
            if (!string.IsNullOrEmpty(value))
                return value;

            value = _certificate.GetNameInfo(X509NameType.EmailName, false);
            if (!string.IsNullOrEmpty(value))
                return value;

            value = _certificate.GetNameInfo(X509NameType.UpnName, false);
            if (!string.IsNullOrEmpty(value))
                return value;

            return String.Empty;
        }

        public override ClaimsIdentity Clone()
        {
            return _certificate != null ? new X509Identity(_certificate) : new X509Identity(_x500DistinguishedName);
        }

        public void Dispose()
        {
            if (_disposable && !_disposed)
            {
                _disposed = true;
                if (_certificate != null)
                {
                    _certificate.Dispose();
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.GetType().FullName));
            }
        }
    }
}
