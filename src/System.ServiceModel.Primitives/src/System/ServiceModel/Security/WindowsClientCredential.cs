// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Net;
using System.Security.Principal;

namespace System.ServiceModel.Security
{
    public sealed class WindowsClientCredential
    {
        internal const TokenImpersonationLevel DefaultImpersonationLevel = TokenImpersonationLevel.Identification;
        private TokenImpersonationLevel _allowedImpersonationLevel = DefaultImpersonationLevel;
        private NetworkCredential _windowsCredentials;
        private bool _allowNtlm = SspiSecurityTokenProvider.DefaultAllowNtlm;
        private bool _isReadOnly;

        internal WindowsClientCredential()
        {
        }

        internal WindowsClientCredential(WindowsClientCredential other)
        {
            if (other._windowsCredentials != null)
            {
                _windowsCredentials = SecurityUtils.GetNetworkCredentialsCopy(other._windowsCredentials);
            }

            _allowedImpersonationLevel = other._allowedImpersonationLevel;
            _allowNtlm = other._allowNtlm;
            _isReadOnly = other._isReadOnly;
        }

        public TokenImpersonationLevel AllowedImpersonationLevel
        {
            get
            {
                return _allowedImpersonationLevel;
            }
            set
            {
                ThrowIfImmutable();

                if (((value == TokenImpersonationLevel.None) || (value == TokenImpersonationLevel.Anonymous))
                    )
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.Format(SRP.UnsupportedTokenImpersonationLevel, "AllowedImpersonationLevel", value.ToString())));
                }

                _allowedImpersonationLevel = value;
            }
        }

        public NetworkCredential ClientCredential
        {
            get
            {
                if (_windowsCredentials == null)
                {
                    _windowsCredentials = new NetworkCredential();
                }

                return _windowsCredentials;
            }
            set
            {
                ThrowIfImmutable();
                _windowsCredentials = value;
            }
        }

        [Obsolete("This property is deprecated and is maintained for backward compatibility only. The local machine policy will be used to determine if NTLM should be used.")]
        public bool AllowNtlm
        {
            get
            {
                return _allowNtlm;
            }
            set
            {
                ThrowIfImmutable();
                _allowNtlm = value;
            }
        }

        internal void MakeReadOnly()
        {
            _isReadOnly = true;
        }

        private void ThrowIfImmutable()
        {
            if (_isReadOnly)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.ObjectIsReadOnly));
            }
        }
    }
}
