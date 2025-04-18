// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF.IdentityModel.Tokens;
#else
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
#endif
using System.Collections.ObjectModel;
using System.Security.Claims;
using System.Security.Principal;
using System.Xml;

namespace WcfService
{
    internal class AcceptAnyUsernameSecurityTokenHandler : UserNameSecurityTokenHandler
    {
        public override bool CanValidateToken { get { return true; } }

        public override ReadOnlyCollection<ClaimsIdentity> ValidateToken(SecurityToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException("token");
            }

            UserNameSecurityToken usernameToken = token as UserNameSecurityToken;
            if (usernameToken == null)
            {
                throw new ArgumentException("token", "Not a UserNameSecurityToken");
            }

            try
            {
                string userName = usernameToken.UserName;
                string password = usernameToken.Password;
                var identity = new GenericIdentity(userName);
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(DateTime.UtcNow, "yyyy-MM-ddTHH:mm:ss.fffZ"), ClaimValueTypes.DateTime));
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, AuthenticationMethods.Password));

                if (this.Configuration.SaveBootstrapContext)
                {
                    if (RetainPassword)
                    {
                        identity.BootstrapContext = new BootstrapContext(usernameToken, this);
                    }
                    else
                    {
                        identity.BootstrapContext = new BootstrapContext(new UserNameSecurityToken(usernameToken.UserName, null), this);
                    }
                }

                TraceTokenValidationSuccess(token);

                List<ClaimsIdentity> identities = new List<ClaimsIdentity>(1);
                identities.Add(identity);
                return identities.AsReadOnly();
            }
            catch (Exception e)
            {
                TraceTokenValidationFailure(token, e.Message);
                throw e;
            }
        }
    }
}
