// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Cryptography.X509Certificates;

namespace System.ServiceModel.Security
{
    public sealed class X509CertificateInitiatorClientCredential
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("subjectName");
            }
            SetCertificate(storeLocation, storeName, DefaultFindType, subjectName);
        }

        public void SetCertificate(StoreLocation storeLocation, StoreName storeName, X509FindType findType, object findValue)
        {
            if (findValue == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("findValue");
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
