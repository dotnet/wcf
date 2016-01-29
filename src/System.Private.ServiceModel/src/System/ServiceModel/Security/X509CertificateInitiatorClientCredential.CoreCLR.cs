// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if FEATURE_CORECLR
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

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
                _certificate = value;
            }
        }

        internal bool CloneCertificate
        {
            get
            {
                return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            }
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
    }
}
#endif // FEATURE_CORECLR
