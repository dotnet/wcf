// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Threading.Tasks;

namespace System.IdentityModel.Selectors
{
    public class X509SecurityTokenProvider : SecurityTokenProvider, IDisposable
    {
        private X509Certificate2 _certificate;
        private bool _clone;

        public X509SecurityTokenProvider(X509Certificate2 certificate) : this(certificate, true) { }

        internal X509SecurityTokenProvider(X509Certificate2 certificate, bool clone)
        {
            if (certificate == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(certificate));
            }

            _clone = clone;
            if (_clone)
            {
                _certificate = new X509Certificate2(certificate);
            }
            else
            {
                _certificate = certificate;
            }
        }

        protected override SecurityToken GetTokenCore(TimeSpan timeout)
        {
            return new X509SecurityToken(certificate: _certificate, clone: _clone, disposable: _clone);
        }

        internal override Task<SecurityToken> GetTokenCoreInternalAsync(TimeSpan timeout)
        {
            return Task.FromResult<SecurityToken>(GetTokenCore(timeout));
        }

        public void Dispose()
        {
            if (_clone)
            {
                System.ServiceModel.Security.SecurityUtils.ResetCertificate(_certificate);
            }
        }
    }
}
