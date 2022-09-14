// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.ObjectModel;
using System.Net;
using System.Security.Principal;
using System.ServiceModel;

namespace System.IdentityModel.Tokens
{
    public class KerberosRequestorSecurityToken : SecurityToken
    {
        private string _id;
        private DateTime _effectiveTime;
        private DateTime _expirationTime;

        internal KerberosRequestorSecurityToken(string servicePrincipalName, TokenImpersonationLevel tokenImpersonationLevel, NetworkCredential networkCredential, string id)
        {
            if (tokenImpersonationLevel != TokenImpersonationLevel.Identification && tokenImpersonationLevel != TokenImpersonationLevel.Impersonation)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException(nameof(tokenImpersonationLevel),
                    SRP.Format(SRP.ImpersonationLevelNotSupported, tokenImpersonationLevel)));
            }

            ServicePrincipalName = servicePrincipalName ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(servicePrincipalName));
            if (networkCredential != null && networkCredential != CredentialCache.DefaultNetworkCredentials)
            {
                if (string.IsNullOrEmpty(networkCredential.UserName))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.ProvidedNetworkCredentialsForKerberosHasInvalidUserName);
                }
                // Note: we don't check the domain, since Lsa accepts
                // FQ userName.
            }
            _id = id ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(id));
            _effectiveTime = default;
            _expirationTime = default;
        }

        public override string Id
        {
            get { return _id; }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get { return EmptyReadOnlyCollection<SecurityKey>.Instance; }
        }

        public override DateTime ValidFrom
        {
            get { return _effectiveTime; }
        }

        public override DateTime ValidTo
        {
            get { return _expirationTime; }
        }

        public string ServicePrincipalName { get; }
    }
}
