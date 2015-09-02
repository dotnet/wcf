// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if FEATURE_CORECLR // X509Certificates
using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Security.Claims;
using System.Security.Cryptography;
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
            _certificate = clone ? new X509Certificate2(certificate.Handle) : certificate;
        }

        X509CertificateClaimSet(X509ChainElementCollection elements, int index)
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

        IList<Claim> InitializeClaimsCore()
        {
            List<Claim> claims = new List<Claim>();
            byte[] thumbprint = _certificate.GetCertHash();
            claims.Add(new Claim(ClaimTypes.Thumbprint, thumbprint, Rights.Identity));
            claims.Add(new Claim(ClaimTypes.Thumbprint, thumbprint, Rights.PossessProperty));

            // Ordering SubjectName, Dns, SimpleName, Email, Upn
            string value = _certificate.SubjectName.Name;
            if (!string.IsNullOrEmpty(value))
                claims.Add(Claim.CreateX500DistinguishedNameClaim(_certificate.SubjectName));


            // new behavior as this is the default long term behavior
            // Since a SAN can have multiple DNS entries
            string[] entries = GetDnsFromExtensions(_certificate);
            for (int i = 0; i < entries.Length; ++i)
            {
                claims.Add(Claim.CreateDnsClaim(entries[i]));
            }

            value = _certificate.GetNameInfo(X509NameType.SimpleName, false);
            if (!string.IsNullOrEmpty(value))
                claims.Add(Claim.CreateNameClaim(value));

            value = _certificate.GetNameInfo(X509NameType.EmailName, false);
            if (!string.IsNullOrEmpty(value))
                throw ExceptionHelper.PlatformNotSupported("InitializeClaimsCore - EmailName");

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

        void EnsureClaims()
        {
            if (_claims != null)
                return;

            _claims = InitializeClaimsCore();
        }

        static bool SupportedClaimType(string claimType)
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

                    // new behavior since this is the default long term behavior
                    string[] entries = GetDnsFromExtensions(_certificate);
                    for (int i = 0; i < entries.Length; ++i)
                    {
                        yield return Claim.CreateDnsClaim(entries[i]);
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

        // Fixing Bug 795660: SAN having multiple DNS entries
        private static string[] GetDnsFromExtensions(X509Certificate2 cert)
        {
            foreach (X509Extension ext in cert.Extensions)
            {
                // Extension is SAN or SAN2
                if (ext.Oid.Value == "2.5.29.7" || ext.Oid.Value == "2.5.29.17")
                {
                    string asnString = ext.Format(true);
                    if (string.IsNullOrEmpty(asnString))
                    {
                        return new string[0];
                    }

                    string[] rawDnsEntries = asnString.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] dnsEntries = new string[rawDnsEntries.Length];
                    for (int i = 0; i < rawDnsEntries.Length; ++i)
                    {
                        int equalSignIndex = rawDnsEntries[i].IndexOf('=');
                        dnsEntries[i] = rawDnsEntries[i].Substring(equalSignIndex + 1).Trim();
                    }
                    return dnsEntries;
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

        void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.GetType().FullName));
            }
        }

        class X500DistinguishedNameClaimSet : DefaultClaimSet, IIdentityInfo
        {
            IIdentity _identity;

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
            _certificate = clone ? new X509Certificate2(certificate.Handle) : certificate;
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

        string GetName()
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

        void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.GetType().FullName));
            }
        }
    }
}
#endif // FEATURE_CORECLR
