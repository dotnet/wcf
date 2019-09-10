// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Net;
using System.Security.Principal;

namespace System.ServiceModel.Security
{
    public sealed class HttpDigestClientCredential
    {
        private NetworkCredential _digestCredentials;
        private bool _isReadOnly;

        internal HttpDigestClientCredential()
        {
            _digestCredentials = new NetworkCredential();
        }

        internal HttpDigestClientCredential(HttpDigestClientCredential other)
        {
            _digestCredentials = SecurityUtils.GetNetworkCredentialsCopy(other._digestCredentials);
            _isReadOnly = other._isReadOnly;
        }

        public NetworkCredential ClientCredential
        {
            get
            {
                return _digestCredentials;
            }
            set
            {
                ThrowIfImmutable();
                _digestCredentials = value;
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ObjectIsReadOnly));
            }
        }
    }
}
