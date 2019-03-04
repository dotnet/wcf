// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IdentityModel.Tokens
{
    using System.Diagnostics;
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel;

    public class X509IssuerSerialKeyIdentifierClause : SecurityKeyIdentifierClause
    {
        private readonly string _issuerName;
        private readonly string _issuerSerialNumber;

        public X509IssuerSerialKeyIdentifierClause(string issuerName, string issuerSerialNumber)
            : base(null)
        {
            if (string.IsNullOrEmpty(issuerName))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(issuerName));
            if (string.IsNullOrEmpty(issuerSerialNumber))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(issuerSerialNumber));

            _issuerName = issuerName;
            _issuerSerialNumber = issuerSerialNumber;
        }

        public X509IssuerSerialKeyIdentifierClause(X509Certificate2 certificate)
            : base(null)
        {
            if (certificate == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(certificate));

            _issuerName = certificate.Issuer;
            _issuerSerialNumber = certificate.GetSerialNumberString();
        }

        public string IssuerName
        {
            get { return _issuerName; }
        }

        public string IssuerSerialNumber
        {
            get { return _issuerSerialNumber; }
        }

        public override bool Matches(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            X509IssuerSerialKeyIdentifierClause that = keyIdentifierClause as X509IssuerSerialKeyIdentifierClause;
            return ReferenceEquals(this, that) || (that != null && that.Matches(_issuerName, _issuerSerialNumber));
        }

        public bool Matches(X509Certificate2 certificate)
        {
            if (certificate == null)
                return false;

            return Matches(certificate.Issuer, certificate.GetSerialNumberString());
        }

        public bool Matches(string issuerName, string issuerSerialNumber)
        {
            if (issuerName == null)
            {
                return false;
            }

            // If serial numbers dont match, we can avoid the potentially expensive issuer name comparison
            if (_issuerSerialNumber != issuerSerialNumber)
            {
                return false;
            }

            // Serial numbers match. Do a string comparison of issuer names
            if (_issuerName == issuerName)
            {
                return true;
            }

            // String equality comparison for issuer names failed
            // Do a byte-level comparison of the X500 distinguished names corresponding to the issuer names. 
            // X500DistinguishedName constructor can throw for malformed inputs
            bool x500IssuerNameMatch = false;
            try
            {
                if (System.ServiceModel.Security.SecurityUtils.IsEqual(new X500DistinguishedName(_issuerName).RawData,
                                                                       new X500DistinguishedName(issuerName).RawData))
                {
                    x500IssuerNameMatch = true;
                }
            }
            catch (CryptographicException)
            {
            }

            return x500IssuerNameMatch;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "X509IssuerSerialKeyIdentifierClause(Issuer = '{0}', Serial = '{1}')",
                this.IssuerName, this.IssuerSerialNumber);
        }
    }
}
