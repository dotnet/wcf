// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace System.ServiceModel.Security
{
    public sealed partial class X509CertificateInitiatorClientCredential
    {
        internal const StoreLocation DefaultStoreLocation = StoreLocation.CurrentUser;
        internal const StoreName DefaultStoreName = StoreName.My;
        internal const X509FindType DefaultFindType = X509FindType.FindBySubjectDistinguishedName;

        private X509Certificate2 _certificate;
        private bool _isReadOnly;

        internal X509CertificateInitiatorClientCredential()
        {
            // empty
        }

        internal X509CertificateInitiatorClientCredential(X509CertificateInitiatorClientCredential other)
        {
            _certificate = other._certificate;
            _isReadOnly = other._isReadOnly;
        }

        internal bool CloneCertificate
        {
            get
            {
                // The clone constructor of X509Certificate2 doesn't copy the private key
                // on non-windows platforms so we have to use the same certificate object
                // and risk the user Dispose'ing the certificate.
                return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            }
        }

        public X509Certificate2 Certificate
        {
            get
            {
                return _certificate;
            }
            set
            {
                ThrowIfImmutable();
                _certificate = value;
            }
        }

        public void SetCertificate(string subjectName, StoreLocation storeLocation, StoreName storeName)
        {
            if (subjectName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(subjectName));
            }

            SetCertificate(storeLocation, storeName, DefaultFindType, subjectName);
        }

        public void SetCertificate(StoreLocation storeLocation, StoreName storeName, X509FindType findType, object findValue)
        {
            if (findValue == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(findValue));
            }

            ThrowIfImmutable();
            _certificate = SecurityUtils.GetCertificateFromStore(storeName, storeLocation, findType, findValue, null);
        }

        internal void MakeReadOnly()
        {
            _isReadOnly = true;
        }

        private void ThrowIfImmutable()
        {
            if (_isReadOnly)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ObjectIsReadOnly)));
            }
        }
    }
}
