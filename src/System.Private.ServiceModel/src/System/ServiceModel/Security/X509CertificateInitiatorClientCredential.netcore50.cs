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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(findValue));
            }

            ThrowIfImmutable();
            var dotNetCertificate = SecurityUtils.GetCertificateFromStore(storeName, storeLocation, findType, findValue, null);
            IReadOnlyList<Certificate> uapCertificates;
            try
            {
                uapCertificates = GetCertificatesFromWinRTStore(dotNetCertificate);
            }
            catch (Exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(SecurityUtils.CreateCertificateLoadException(
                    storeName, storeLocation, findType, findValue, null, 0));
            }

            if (uapCertificates.Count != 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(SecurityUtils.CreateCertificateLoadException(
                    storeName, storeLocation, findType, findValue, null, uapCertificates.Count));
            }

            AttachUapCertificate(dotNetCertificate, uapCertificates[0]);
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

        internal static Certificate TryGetUapCertificate(X509Certificate2 certificate, out bool success)
        {
            foreach (var extension in certificate.Extensions)
            {
                if (extension is X509UwpCertificateAttachmentExtension attachmentExtension)
                {
                    success = true;
                    return attachmentExtension.AttachedCertificate;
                }
            }

            success = false;
            return null;
        }

        private void AttachUapCertificate(X509Certificate2 dotNetCertificate, Certificate uapCertificate)
        {
            dotNetCertificate.Extensions.Add(new X509UwpCertificateAttachmentExtension(uapCertificate));
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