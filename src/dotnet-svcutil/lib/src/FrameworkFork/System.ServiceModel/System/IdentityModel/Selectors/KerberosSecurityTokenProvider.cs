// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Principal;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace System.IdentityModel.Selectors
{
    public class KerberosSecurityTokenProvider : SecurityTokenProvider
    {
        private readonly string _servicePrincipalName;
        private readonly TokenImpersonationLevel _tokenImpersonationLevel;
        private readonly NetworkCredential _networkCredential;

        public KerberosSecurityTokenProvider(string servicePrincipalName)
            : this(servicePrincipalName, TokenImpersonationLevel.Identification)
        {
        }

        public KerberosSecurityTokenProvider(string servicePrincipalName, TokenImpersonationLevel tokenImpersonationLevel)
            : this(servicePrincipalName, tokenImpersonationLevel, null)
        {
        }

        public KerberosSecurityTokenProvider(string servicePrincipalName, TokenImpersonationLevel tokenImpersonationLevel, NetworkCredential networkCredential)
        {
            if (servicePrincipalName == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("servicePrincipalName");
            if (tokenImpersonationLevel != TokenImpersonationLevel.Identification && tokenImpersonationLevel != TokenImpersonationLevel.Impersonation)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("tokenImpersonationLevel",
                    string.Format(SRServiceModel.ImpersonationLevelNotSupported, tokenImpersonationLevel)));
            }

            _servicePrincipalName = servicePrincipalName;
            _tokenImpersonationLevel = tokenImpersonationLevel;
            _networkCredential = networkCredential;
        }

        public string ServicePrincipalName
        {
            get { return _servicePrincipalName; }
        }

        public TokenImpersonationLevel TokenImpersonationLevel
        {
            get { return _tokenImpersonationLevel; }
        }

        public NetworkCredential NetworkCredential
        {
            get { return _networkCredential; }
        }

        internal SecurityToken GetToken(CancellationToken cancellationToken, ChannelBinding channelbinding)
        {
            return new KerberosRequestorSecurityToken(ServicePrincipalName,
                TokenImpersonationLevel, NetworkCredential,
                SecurityUniqueId.Create().Value);
        }

        protected override Task<SecurityToken> GetTokenCoreAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(GetToken(cancellationToken, null));
        }
    }
}
