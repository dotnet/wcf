// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Net;

namespace System.ServiceModel.Security.Tokens
{
    public sealed class InitiatorServiceModelSecurityTokenRequirement : ServiceModelSecurityTokenRequirement
    {
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
                Properties[TargetAddressProperty] = value;
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
                Properties[ViaProperty] = value;
            }
        }

        internal bool IsOutOfBandToken
        {
            get
            {
                return GetPropertyOrDefault(IsOutOfBandTokenProperty, false);
            }
            set
            {
                Properties[IsOutOfBandTokenProperty] = value;
            }
        }

        internal bool PreferSslCertificateAuthenticator
        {
            get
            {
                return GetPropertyOrDefault(PreferSslCertificateAuthenticatorProperty, false);
            }
            set
            {
                Properties[PreferSslCertificateAuthenticatorProperty] = value;
            }
        }

        public override string ToString()
        {
            return InternalToString();
        }
    }
}
