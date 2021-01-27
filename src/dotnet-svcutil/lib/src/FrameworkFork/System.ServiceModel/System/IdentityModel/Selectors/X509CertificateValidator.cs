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
        private static X509CertificateValidator s_chainTrust;
        private static X509CertificateValidator s_none;

        public static X509CertificateValidator None
        {
            get
            {
                if (s_none == null)
                    s_none = new NoneX509CertificateValidator();
                return s_none;
            }
        }

        public static X509CertificateValidator ChainTrust
        {
            get
            {
                if (s_chainTrust == null)
                    s_chainTrust = new ChainTrustValidator();
                return s_chainTrust;
            }
        }

        public static X509CertificateValidator CreateChainTrustValidator(bool useMachineContext, X509ChainPolicy chainPolicy)
        {
            if (chainPolicy == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("chainPolicy");
            return new ChainTrustValidator(useMachineContext, chainPolicy, X509CertificateChain.DefaultChainPolicyOID);
        }

        public abstract void Validate(X509Certificate2 certificate);

        private class NoneX509CertificateValidator : X509CertificateValidator
        {
            public override void Validate(X509Certificate2 certificate)
            {
                if (certificate == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("certificate");
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
                Contract.Assert(useMachineContext == false, "CoreCLR does not have ctor allowing useMachineContext = true");

                _useMachineContext = useMachineContext;
                _chainPolicy = chainPolicy;
                _chainPolicyOID = chainPolicyOID;
            }

            public override void Validate(X509Certificate2 certificate)
            {
                if (certificate == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("certificate");

                // implies _useMachineContext = false
                // ctor for X509Chain(_useMachineContext, _chainPolicyOID) not present in CoreCLR
                X509Chain chain = new X509Chain();

                if (_chainPolicy != null)
                {
                    chain.ChainPolicy = _chainPolicy;
                }

                if (!chain.Build(certificate))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new SecurityTokenValidationException(string.Format(SRServiceModel.X509ChainBuildFail,
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
    }
}

