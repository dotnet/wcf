// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Principal;
using System.ServiceModel;

namespace System.IdentityModel.Tokens
{
    public class KerberosRequestorSecurityToken : SecurityToken
    {
        private string _id;
        private readonly string _servicePrincipalName;
        private DateTime _effectiveTime;
        private DateTime _expirationTime;

        internal KerberosRequestorSecurityToken(string servicePrincipalName, TokenImpersonationLevel tokenImpersonationLevel, NetworkCredential networkCredential, string id)
        {
            if (servicePrincipalName == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("servicePrincipalName");
            if (tokenImpersonationLevel != TokenImpersonationLevel.Identification && tokenImpersonationLevel != TokenImpersonationLevel.Impersonation)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("tokenImpersonationLevel",
                    string.Format(SRServiceModel.ImpersonationLevelNotSupported, tokenImpersonationLevel)));
            }
            if (id == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("id");

            _servicePrincipalName = servicePrincipalName;
            if (networkCredential != null && networkCredential != CredentialCache.DefaultNetworkCredentials)
            {
                if (string.IsNullOrEmpty(networkCredential.UserName))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRServiceModel.ProvidedNetworkCredentialsForKerberosHasInvalidUserName);
                }
                // Note: we don't check the domain, since Lsa accepts
                // FQ userName.
            }
            _id = id;
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

        public string ServicePrincipalName
        {
            get { return _servicePrincipalName; }
        }
    }
}
