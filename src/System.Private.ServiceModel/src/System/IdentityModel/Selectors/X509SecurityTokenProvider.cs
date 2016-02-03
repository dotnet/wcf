// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IdentityModel.Tokens;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Threading;
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
            Fx.Assert(
                !clone || RuntimeInformation.IsOSPlatform(OSPlatform.Windows),
                "Certificates MUST NOT be cloned on non-Windows platforms"
            );

            if (certificate == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("certificate");
            }

            _clone = clone;
            _certificate = _clone ? new X509Certificate2(certificate.Handle) : _certificate = certificate;
        }

        protected override async Task<SecurityToken> GetTokenCoreAsync(CancellationToken cancellationToken)
        {
            return await Task.FromResult<SecurityToken>(new X509SecurityToken(certificate: _certificate, clone: _clone, disposable:_clone));
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
