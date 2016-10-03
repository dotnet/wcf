// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Tokens;
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
            if (certificate == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("certificate");
            }

            _clone = clone;
            if (_clone)
            {
                // dotnet/wcf#1574
                // ORIGINAL CODE: 
                // _certificate = new X509Certificate2(certificate.Handle);

                _certificate = certificate.CloneCertificateInternal();
            }
            else
            {
                _certificate = certificate;
            }
        }

        protected override Task<SecurityToken> GetTokenCoreAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult<SecurityToken>(new X509SecurityToken(certificate: _certificate, clone: _clone, disposable: _clone));
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
