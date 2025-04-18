// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
#else
using System;
using System.IdentityModel;
using System.IdentityModel.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.ServiceModel;
#endif
using System.Security.Claims;

namespace WcfService
{
    internal class SelfHostSecurityTokenService : SecurityTokenService
    {
        public SelfHostSecurityTokenService(SecurityTokenServiceConfiguration configuration) : base(configuration) { }

        protected override Scope GetScope(ClaimsPrincipal principal, RequestSecurityToken request)
        {
            if (principal == null)
                throw new ArgumentNullException("principal");

            if (request == null)
                throw new ArgumentNullException("request");

            if (request.AppliesTo == null)
                throw new FaultException<InvalidProgramException>(new InvalidProgramException("request.AppliesTo cannot be null"), new FaultReason("request.AppliesTo cannot be null"), new FaultCode("AppliesToNull"), "Set Applies To");

            if (request.AppliesTo.Uri == null)
                throw new InvalidProgramException("request.AppliesTo.Uri cannot be null");

            if (string.IsNullOrWhiteSpace(request.AppliesTo.Uri.OriginalString))
                throw new InvalidProgramException("request.AppliesTo.Uri.AbsoluteUri cannot be null or only whitespace");

            var scope = new Scope(request.AppliesTo.Uri.OriginalString, SecurityTokenServiceConfiguration.SigningCredentials)
            {
                TokenEncryptionRequired = false,
                SymmetricKeyEncryptionRequired = false
            };

            if (string.IsNullOrEmpty(request.ReplyTo))
                scope.ReplyToAddress = scope.AppliesToAddress;
            else
                scope.ReplyToAddress = request.ReplyTo;

            return scope;
        }

        protected override ClaimsIdentity GetOutputClaimsIdentity(ClaimsPrincipal principal, RequestSecurityToken request, Scope scope)
        {
            if (principal == null)
                throw new ArgumentNullException("principal");

            if (request == null)
                throw new ArgumentNullException("request");

            if (scope == null)
                throw new ArgumentNullException("scope");

            var outputIdentity = principal.Identity as ClaimsIdentity;
            if (request.ActAs != null)
            {
                var currIdentity = outputIdentity;
                foreach (var identity in request.ActAs.GetIdentities())
                {
                    currIdentity.Actor = identity;
                    currIdentity = identity.Actor;
                }
            }

            return outputIdentity;
        }
    }
}
