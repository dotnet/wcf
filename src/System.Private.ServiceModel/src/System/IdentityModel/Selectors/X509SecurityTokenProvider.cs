// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace System.IdentityModel.Selectors
{
    public class X509SecurityTokenProvider : SecurityTokenProvider, IDisposable
    {
        private X509Certificate2 _certificate;

        public X509SecurityTokenProvider(X509Certificate2 certificate)
        {
            if (certificate == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("certificate");
            }

            _certificate = new X509Certificate2(certificate.Handle);
        }

        public X509SecurityTokenProvider(StoreLocation storeLocation, StoreName storeName, X509FindType findType, object findValue)
        {
            if (findValue == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("findValue");
            }

            X509Store store = new X509Store(storeName, storeLocation);
            X509Certificate2Collection certificates = null;
            try
            {
                store.Open(OpenFlags.ReadOnly);
                certificates = store.Certificates.Find(findType, findValue, false);
                if (certificates.Count < 1)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.CannotFindCert, storeName, storeLocation, findType, findValue)));
                }
                if (certificates.Count > 1)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenException(SR.Format(SR.FoundMultipleCerts, storeName, storeLocation, findType, findValue)));
                }

                _certificate = new X509Certificate2(certificates[0].Handle);
            }
            finally
            {
                System.ServiceModel.Security.SecurityUtils.ResetAllCertificates(certificates);
                store.Dispose();
            }
        }

        public X509Certificate2 Certificate
        {
            get { return _certificate; }
        }

        protected override async Task<SecurityToken> GetTokenCoreAsync(CancellationToken cancellationToken)
        {
            return await Task.FromResult<SecurityToken>(new X509SecurityToken(_certificate));
        }

        public void Dispose()
        {
            System.ServiceModel.Security.SecurityUtils.ResetCertificate(_certificate);
        }
    }
}
