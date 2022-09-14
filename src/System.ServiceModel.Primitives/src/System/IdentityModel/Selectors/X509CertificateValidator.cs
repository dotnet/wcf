// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Diagnostics.Contracts;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;

namespace System.IdentityModel.Selectors
{
    public abstract class X509CertificateValidator
    {
        private static X509CertificateValidator s_peerTrust;
        private static X509CertificateValidator s_chainTrust;
        private static X509CertificateValidator s_peerOrChainTrust;
        private static X509CertificateValidator s_none;

        public static X509CertificateValidator None
        {
            get
            {
                if (s_none == null)
                {
                    s_none = new NoneX509CertificateValidator();
                }

                return s_none;
            }
        }

        public static X509CertificateValidator PeerTrust
        {
            get
            {
                if (s_peerTrust == null)
                {
                    s_peerTrust = new PeerTrustValidator();
                }

                return s_peerTrust;
            }
        }

        public static X509CertificateValidator ChainTrust
        {
            get
            {
                if (s_chainTrust == null)
                {
                    s_chainTrust = new ChainTrustValidator();
                }

                return s_chainTrust;
            }
        }

        public static X509CertificateValidator PeerOrChainTrust
        {
            get
            {
                if (s_peerOrChainTrust == null)
                {
                    s_peerOrChainTrust = new PeerOrChainTrustValidator();
                }

                return s_peerOrChainTrust;
            }
        }

        public static X509CertificateValidator CreateChainTrustValidator(bool useMachineContext, X509ChainPolicy chainPolicy)
        {
            if (chainPolicy == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(chainPolicy));
            }

            return new ChainTrustValidator(useMachineContext, chainPolicy, X509CertificateChain.DefaultChainPolicyOID);
        }

        public static X509CertificateValidator CreatePeerOrChainTrustValidator(bool useMachineContext, X509ChainPolicy chainPolicy)
        {
            if (chainPolicy == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(chainPolicy));
            }

            return new PeerOrChainTrustValidator(useMachineContext, chainPolicy);
        }

        public abstract void Validate(X509Certificate2 certificate);

        private class NoneX509CertificateValidator : X509CertificateValidator
        {
            public override void Validate(X509Certificate2 certificate)
            {
                if (certificate == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(certificate));
                }
            }
        }

        private class PeerTrustValidator : X509CertificateValidator
        {
            public override void Validate(X509Certificate2 certificate)
            {
                if (certificate == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(certificate));
                }

                Exception exception;
                if (!TryValidate(certificate, out exception))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
                }
            }

            private static bool StoreContainsCertificate(StoreName storeName, X509Certificate2 certificate)
            {
                X509Store store = new X509Store(storeName, StoreLocation.CurrentUser);
                X509Certificate2Collection certificates = null;
                try
                {
                    store.Open(OpenFlags.ReadOnly);
                    certificates = store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false);
                    return certificates.Count > 0;
                }
                finally
                {
                    SecurityUtils.ResetAllCertificates(certificates);
                    store.Dispose();
                }
            }

            internal bool TryValidate(X509Certificate2 certificate, out Exception exception)
            {
                // Checklist
                // 1) time validity of cert
                // 2) in trusted people store
                // 3) not in disallowed store

                // The following code could be written as:
                // DateTime now = DateTime.UtcNow;
                // if (now > certificate.NotAfter.ToUniversalTime() || now < certificate.NotBefore.ToUniversalTime())
                //
                // this is because X509Certificate2.xxx doesn't return UT.  However this would be a SMALL perf hit.
                // I put an Assert so that this will ensure that the we are compatible with the CLR we shipped with

                DateTime now = DateTime.Now;
                Contract.Assert(now.Kind == certificate.NotAfter.Kind && now.Kind == certificate.NotBefore.Kind, "");

                if (now > certificate.NotAfter || now < certificate.NotBefore)
                {
                    exception = new SecurityTokenValidationException(SRP.Format(SRP.X509InvalidUsageTime,
                        SecurityUtils.GetCertificateId(certificate), now, certificate.NotBefore, certificate.NotAfter));
                    return false;
                }

                if (!StoreContainsCertificate(StoreName.TrustedPeople, certificate))
                {
                    exception = new SecurityTokenValidationException(SRP.Format(SRP.X509IsNotInTrustedStore,
                        SecurityUtils.GetCertificateId(certificate)));
                    return false;
                }

                if (StoreContainsCertificate(StoreName.Disallowed, certificate))
                {
                    exception = new SecurityTokenValidationException(SRP.Format(SRP.X509IsInUntrustedStore,
                        SecurityUtils.GetCertificateId(certificate)));
                    return false;
                }
                exception = null;
                return true;
            }
        }

        private class ChainTrustValidator : X509CertificateValidator
        {
            private bool _useMachineContext;
            private X509ChainPolicy _chainPolicy;
            private uint _chainPolicyOID = X509CertificateChain.DefaultChainPolicyOID;

            public ChainTrustValidator()
            {
                _chainPolicy = null;
            }

            public ChainTrustValidator(bool useMachineContext, X509ChainPolicy chainPolicy, uint chainPolicyOID)
            {
                _useMachineContext = useMachineContext;
                _chainPolicy = chainPolicy;
                _chainPolicyOID = chainPolicyOID;
            }

            public override void Validate(X509Certificate2 certificate)
            {
                if (certificate == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(certificate));
                }

                // implies _useMachineContext = false
                // ctor for X509Chain(_useMachineContext, _chainPolicyOID) not present in CoreCLR
                X509Chain chain = new X509Chain();

                if (_chainPolicy != null)
                {
                    _chainPolicy.VerificationTime = DateTime.Now;
                    chain.ChainPolicy = _chainPolicy;
                }

                if (!chain.Build(certificate))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenValidationException(SRP.Format(SRP.X509ChainBuildFail,
                        SecurityUtils.GetCertificateId(certificate), GetChainStatusInformation(chain.ChainStatus))));
                }
            }

            private static string GetChainStatusInformation(X509ChainStatus[] chainStatus)
            {
                if (chainStatus != null)
                {
                    StringBuilder error = new StringBuilder(128);
                    for (int i = 0; i < chainStatus.Length; ++i)
                    {
                        error.Append(chainStatus[i].StatusInformation);
                        error.Append(" ");
                    }
                    return error.ToString();
                }
                return String.Empty;
            }
        }


        private class PeerOrChainTrustValidator : X509CertificateValidator
        {
            private X509CertificateValidator _chain;
            private PeerTrustValidator _peer;

            public PeerOrChainTrustValidator()
            {
                _chain = X509CertificateValidator.ChainTrust;
                _peer = (PeerTrustValidator)X509CertificateValidator.PeerTrust;
            }

            public PeerOrChainTrustValidator(bool useMachineContext, X509ChainPolicy chainPolicy)
            {
                _chain = X509CertificateValidator.CreateChainTrustValidator(useMachineContext, chainPolicy);
                _peer = (PeerTrustValidator)X509CertificateValidator.PeerTrust;
            }

            public override void Validate(X509Certificate2 certificate)
            {
                if (certificate == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(certificate));
                }

                Exception exception;
                if (_peer.TryValidate(certificate, out exception))
                {
                    return;
                }

                try
                {
                    _chain.Validate(certificate);
                }
                catch (SecurityTokenValidationException ex)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenValidationException(exception.Message + " " + ex.Message));
                }
            }
        }
    }
}
