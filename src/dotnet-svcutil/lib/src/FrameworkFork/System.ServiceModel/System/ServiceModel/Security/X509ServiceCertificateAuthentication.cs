// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Selectors;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;

namespace System.ServiceModel.Security
{
    public sealed class X509ServiceCertificateAuthentication
    {
        internal const X509CertificateValidationMode DefaultCertificateValidationMode = X509CertificateValidationMode.ChainTrust;
        internal const X509RevocationMode DefaultRevocationMode = X509RevocationMode.Online;
        internal const StoreLocation DefaultTrustedStoreLocation = StoreLocation.CurrentUser;
        private static X509CertificateValidator s_defaultCertificateValidator;

        private X509CertificateValidationMode _certificateValidationMode = DefaultCertificateValidationMode;
        private X509RevocationMode _revocationMode = DefaultRevocationMode;
        private StoreLocation _trustedStoreLocation = DefaultTrustedStoreLocation;
        private X509CertificateValidator _customCertificateValidator = null;
        private bool _isReadOnly;

        public X509ServiceCertificateAuthentication()
        {
        }

        internal X509ServiceCertificateAuthentication(X509ServiceCertificateAuthentication other)
        {
            _certificateValidationMode = other._certificateValidationMode;
            _customCertificateValidator = other._customCertificateValidator;
            _revocationMode = other._revocationMode;
            _trustedStoreLocation = other._trustedStoreLocation;
            _isReadOnly = other._isReadOnly;
        }

        internal static X509CertificateValidator DefaultCertificateValidator
        {
            get
            {
                if (s_defaultCertificateValidator == null)
                {
                    bool useMachineContext = DefaultTrustedStoreLocation == StoreLocation.LocalMachine;
                    X509ChainPolicy chainPolicy = new X509ChainPolicy();
                    chainPolicy.RevocationMode = DefaultRevocationMode;
                    s_defaultCertificateValidator = X509CertificateValidator.CreateChainTrustValidator(useMachineContext, chainPolicy);
                }
                return s_defaultCertificateValidator;
            }
        }

        public X509CertificateValidationMode CertificateValidationMode
        {
            get
            {
                return _certificateValidationMode;
            }
            set
            {
                X509CertificateValidationModeHelper.Validate(value);
                ThrowIfImmutable();
                _certificateValidationMode = value;
            }
        }

        public X509RevocationMode RevocationMode
        {
            get
            {
                return _revocationMode;
            }
            set
            {
                ThrowIfImmutable();
                _revocationMode = value;
            }
        }

        public StoreLocation TrustedStoreLocation
        {
            get
            {
                return _trustedStoreLocation;
            }
            set
            {
                ThrowIfImmutable();
                _trustedStoreLocation = value;
            }
        }

        public X509CertificateValidator CustomCertificateValidator
        {
            get
            {
                return _customCertificateValidator;
            }
            set
            {
                ThrowIfImmutable();
                _customCertificateValidator = value;
            }
        }

        internal bool TryGetCertificateValidator(out X509CertificateValidator validator)
        {
            validator = null;
            if (_certificateValidationMode == X509CertificateValidationMode.None)
            {
                validator = X509CertificateValidator.None;
            }
            else if (_certificateValidationMode == X509CertificateValidationMode.PeerTrust)
            {
                throw ExceptionHelper.PlatformNotSupported("X509CertificateValidationMode.PeerTrust is not supported");
            }
            else if (_certificateValidationMode == X509CertificateValidationMode.Custom)
            {
                validator = _customCertificateValidator;
            }
            else
            {
                bool useMachineContext = _trustedStoreLocation == StoreLocation.LocalMachine;
                X509ChainPolicy chainPolicy = new X509ChainPolicy();
                chainPolicy.RevocationMode = _revocationMode;
                if (_certificateValidationMode == X509CertificateValidationMode.ChainTrust)
                {
                    validator = X509CertificateValidator.CreateChainTrustValidator(useMachineContext, chainPolicy);
                }
                else
                {
                    throw ExceptionHelper.PlatformNotSupported("X509CertificateValidationMode.PeerTrust is not supported");
                }
            }
            return (validator != null);
        }

        internal X509CertificateValidator GetCertificateValidator()
        {
            X509CertificateValidator result;
            if (!TryGetCertificateValidator(out result))
            {
                Fx.Assert(_customCertificateValidator == null, "");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.MissingCustomCertificateValidator));
            }
            return result;
        }

        internal void MakeReadOnly()
        {
            _isReadOnly = true;
        }

        private void ThrowIfImmutable()
        {
            if (_isReadOnly)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
            }
        }
    }
}
