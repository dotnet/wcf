// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;

namespace System.ServiceModel.Security
{
 
    public class X509ClientCertificateAuthentication
    {
        internal const X509CertificateValidationMode DefaultCertificateValidationMode = X509CertificateValidationMode.ChainTrust;
        internal const X509RevocationMode DefaultRevocationMode = X509RevocationMode.Online;
        internal const StoreLocation DefaultTrustedStoreLocation = StoreLocation.LocalMachine;
        static X509CertificateValidator defaultCertificateValidator;
        
        internal static X509CertificateValidator DefaultCertificateValidator
        {
            get
            {
                if (defaultCertificateValidator == null)
                {
                    bool useMachineContext = DefaultTrustedStoreLocation == StoreLocation.LocalMachine;
                    X509ChainPolicy chainPolicy = new X509ChainPolicy();
                    chainPolicy.RevocationMode = DefaultRevocationMode;
                    defaultCertificateValidator = X509CertificateValidator.CreateChainTrustValidator(useMachineContext, chainPolicy);
                }
                return defaultCertificateValidator;
            }
        }
    }
}
