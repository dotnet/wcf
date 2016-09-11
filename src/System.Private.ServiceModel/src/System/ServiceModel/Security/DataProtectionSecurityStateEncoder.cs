// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using System.Security.Cryptography;
using System.Runtime;

namespace System.ServiceModel.Security
{
    public class DataProtectionSecurityStateEncoder : SecurityStateEncoder
    {
        private byte[] _entropy;
        private bool _useCurrentUserProtectionScope;

        public DataProtectionSecurityStateEncoder()
            : this(true)
        {
            // empty
        }

        public DataProtectionSecurityStateEncoder(bool useCurrentUserProtectionScope)
            : this(useCurrentUserProtectionScope, null)
        { }

        public DataProtectionSecurityStateEncoder(bool useCurrentUserProtectionScope, byte[] entropy)
        {
            _useCurrentUserProtectionScope = useCurrentUserProtectionScope;
            if (entropy == null)
            {
                _entropy = null;
            }
            else
            {
                _entropy = Fx.AllocateByteArray(entropy.Length);
                Buffer.BlockCopy(entropy, 0, _entropy, 0, entropy.Length);
            }
        }

        public bool UseCurrentUserProtectionScope
        {
            get
            {
                return _useCurrentUserProtectionScope;
            }
        }

        public byte[] GetEntropy()
        {
            byte[] result = null;
            if (_entropy != null)
            {
                result = Fx.AllocateByteArray(_entropy.Length);
                Buffer.BlockCopy(_entropy, 0, result, 0, _entropy.Length);
            }
            return result;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            result.Append(this.GetType().ToString());
            result.AppendFormat("{0}  UseCurrentUserProtectionScope={1}", Environment.NewLine, _useCurrentUserProtectionScope);
            result.AppendFormat("{0}  Entropy Length={1}", Environment.NewLine, (_entropy == null) ? 0 : _entropy.Length);
            return result.ToString();
        }

        protected internal override byte[] DecodeSecurityState( byte[] data )
        {
            try
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                //return ProtectedData.Unprotect(data, this.entropy, (this.useCurrentUserProtectionScope) ? DataProtectionScope.CurrentUser : DataProtectionScope.LocalMachine);
            }
            catch (CryptographicException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CryptographicException(SR.Format(SR.SecurityStateEncoderDecodingFailure), exception));
            }

        }

        protected internal override byte[] EncodeSecurityState( byte[] data )
        {
            try
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                //return ProtectedData.Protect(data, this.entropy, (this.useCurrentUserProtectionScope) ? DataProtectionScope.CurrentUser : DataProtectionScope.LocalMachine);
            }
            catch (CryptographicException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CryptographicException(SR.Format(SR.SecurityStateEncoderEncodingFailure), exception));
            }
        }
    }
}

