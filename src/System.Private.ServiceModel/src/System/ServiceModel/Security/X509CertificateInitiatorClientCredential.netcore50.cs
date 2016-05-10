// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if FEATURE_NETNATIVE

using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using Windows.Security.Cryptography.Certificates;

namespace System.ServiceModel.Security
{
    public partial class X509CertificateInitiatorClientCredential
    {
        public X509Certificate2 Certificate
        {
            get
            {
                return _certificate;
            }
            set
            {
                ThrowIfImmutable();
                if (value == null)
                {
                    _certificate = value;
                }
                else
                {
                    throw ExceptionHelper.PlatformNotSupported("Directly setting the Certificate is not supported yet. Use SetCertificate instead");
                }
            }
        }

        internal bool CloneCertificate
        {
            get
            {
                return false;
            }
        }

        public void SetCertificate(StoreLocation storeLocation, StoreName storeName, X509FindType findType, object findValue)
        {
            if (findValue == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("findValue");
            }
            ThrowIfImmutable();
            var dotNetCertificate = SecurityUtils.GetCertificateFromStore(storeName, storeLocation, findType, findValue, null);
            IReadOnlyList<Certificate> uwpCertificates;
            try
            {
                uwpCertificates = GetCertificatesFromWinRTStore(dotNetCertificate);
            }
            catch (Exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(SecurityUtils.CreateCertificateLoadException(
                    storeName, storeLocation, findType, findValue, null, 0));
            }

            if (uwpCertificates.Count != 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(SecurityUtils.CreateCertificateLoadException(
                    storeName, storeLocation, findType, findValue, null, uwpCertificates.Count));
            }

            AttachUwpCertificate(dotNetCertificate, uwpCertificates[0]);
            _certificate = dotNetCertificate;
        }

        private IReadOnlyList<Certificate> GetCertificatesFromWinRTStore(X509Certificate2 dotNetCertificate)
        {
            var query = new CertificateQuery
            {
                Thumbprint = dotNetCertificate.GetCertHash(),
                IncludeDuplicates = false
            };

            return CertificateStores.FindAllAsync(query).AsTask().GetAwaiter().GetResult();
        }

        private void AttachUwpCertificate(X509Certificate2 dotNetCertificate, Certificate uwpCertificate)
        {
            dotNetCertificate.Extensions.Add(new X509UwpCertificateAttachmentExtension(uwpCertificate));
        }

        internal class X509UwpCertificateAttachmentExtension : X509Extension
        {
            private Certificate _attachedCertificate;

            public X509UwpCertificateAttachmentExtension(Certificate uwpCertificate)
            {
                _attachedCertificate = uwpCertificate;
            }

            public Certificate AttachedCertificate { get { return _attachedCertificate; } }
        }
    }
}
#endif // FEATURE_NETNATIVE