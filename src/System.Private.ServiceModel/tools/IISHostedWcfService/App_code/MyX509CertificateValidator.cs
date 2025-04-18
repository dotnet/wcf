// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF.IdentityModel.Selectors;
using CoreWCF.IdentityModel.Tokens;
#else
using System;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
#endif
using System.Security.Cryptography.X509Certificates;

namespace WcfService
{
    public class MyX509CertificateValidator : X509CertificateValidator
    {
        private string _allowedIssuerName;

        public MyX509CertificateValidator(string allowedIssuerName)
        {
            if (string.IsNullOrEmpty(allowedIssuerName))
            {
                throw new ArgumentNullException("allowedIssuerName", "[MyX509CertificateValidator] The string parameter allowedIssuerName was null or empty.");
            }

            _allowedIssuerName = allowedIssuerName;
        }

        public override void Validate(X509Certificate2 certificate)
        {
            // Check that there is a certificate.
            if (certificate == null)
            {
                throw new ArgumentNullException("certificate", "[MyX509CertificateValidator] The X509Certificate2 parameter certificate was null.");
            }

            // Check that the certificate issuer matches the configured issuer.
            if (!certificate.IssuerName.Name.Contains(_allowedIssuerName))
            {
                throw new SecurityTokenValidationException
                  (string.Format("Certificate was not issued by a trusted issuer. Expected: {0}, Actual: {1}", _allowedIssuerName, certificate.IssuerName.Name));
            }
        }
    }
}
