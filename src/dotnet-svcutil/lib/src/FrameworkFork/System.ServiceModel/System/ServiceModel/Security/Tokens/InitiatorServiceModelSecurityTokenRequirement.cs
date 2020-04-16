// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Http.Headers;

namespace System.ServiceModel.Security.Tokens
{
    public sealed class InitiatorServiceModelSecurityTokenRequirement : ServiceModelSecurityTokenRequirement
    {
        private HttpHeaders _httpHeaders;

        public InitiatorServiceModelSecurityTokenRequirement()
            : base()
        {
            Properties.Add(IsInitiatorProperty, (object)true);
        }

        public EndpointAddress TargetAddress
        {
            get
            {
                return GetPropertyOrDefault<EndpointAddress>(TargetAddressProperty, null);
            }
            set
            {
                this.Properties[TargetAddressProperty] = value;
            }
        }

        public Uri Via
        {
            get
            {
                return GetPropertyOrDefault<Uri>(ViaProperty, null);
            }
            set
            {
                this.Properties[ViaProperty] = value;
            }
        }

        internal bool IsOutOfBandToken
        {
            get
            {
                return GetPropertyOrDefault<bool>(IsOutOfBandTokenProperty, false);
            }
            set
            {
                this.Properties[IsOutOfBandTokenProperty] = value;
            }
        }

        internal bool PreferSslCertificateAuthenticator
        {
            get
            {
                return GetPropertyOrDefault<bool>(PreferSslCertificateAuthenticatorProperty, false);
            }
            set
            {
                this.Properties[PreferSslCertificateAuthenticatorProperty] = value;
            }
        }

        internal HttpHeaders HttpHeaders
        {
            get
            {
                return _httpHeaders;
            }
            set
            {
                _httpHeaders = value;
            }
        }

        public override string ToString()
        {
            return InternalToString();
        }
    }
}
