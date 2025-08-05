// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF.Description;
using CoreWCF.IdentityModel.Selectors;
using CoreWCF.Security;
using CoreWCF.Security.Tokens;

namespace WcfService
{
    public class MultiCredentialSecurityTokenManager : ServiceCredentialsSecurityTokenManager
    {
        private readonly MultiCredentialServiceCredentials _parent;
        private readonly Dictionary<string, ServiceCredentials> _map;

        public MultiCredentialSecurityTokenManager(
            MultiCredentialServiceCredentials parent,
            Dictionary<string, ServiceCredentials> map)
            : base(parent)
        {
            _parent = parent;
            _map = map;
        }

        public override SecurityTokenProvider CreateSecurityTokenProvider(SecurityTokenRequirement tokenRequirement)
        {
            if (tokenRequirement is RecipientServiceModelSecurityTokenRequirement recipientRequirement)
            {
                var uri = recipientRequirement.ListenUri?.AbsolutePath;
                if (!string.IsNullOrEmpty(uri) && _map.TryGetValue(uri, out var creds))
                {
                    return creds.CreateSecurityTokenManager().CreateSecurityTokenProvider(tokenRequirement);
                }
            }
            return _parent.CreateOriginalSecurityTokenManager().CreateSecurityTokenProvider(tokenRequirement);
        }

        public override SecurityTokenAuthenticator CreateSecurityTokenAuthenticator(SecurityTokenRequirement tokenRequirement, out SecurityTokenResolver outOfBandTokenResolver)
        {
            if (tokenRequirement is RecipientServiceModelSecurityTokenRequirement recipientRequirement)
            {
                var uri = recipientRequirement.ListenUri?.AbsolutePath;
                if (!string.IsNullOrEmpty(uri) && _map.TryGetValue(uri, out var creds))
                {
                    return creds.CreateSecurityTokenManager().CreateSecurityTokenAuthenticator(tokenRequirement, out outOfBandTokenResolver);
                }
            }

            return _parent.CreateOriginalSecurityTokenManager().CreateSecurityTokenAuthenticator(tokenRequirement, out outOfBandTokenResolver);
        }
    }
}
#endif
