// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Security;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Security.Principal;

namespace System.ServiceModel
{
    public class ServiceSecurityContext
    {
        private static ServiceSecurityContext s_anonymous;
        private ReadOnlyCollection<IAuthorizationPolicy> _authorizationPolicies;
        private AuthorizationContext _authorizationContext;
        private IIdentity _primaryIdentity;
        private Claim _identityClaim;

        // Perf: delay created authorizationContext using forward chain.
        public ServiceSecurityContext(ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies)
        {
            if (authorizationPolicies == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("authorizationPolicies");
            }
            _authorizationContext = null;
            _authorizationPolicies = authorizationPolicies;
        }

        public ServiceSecurityContext(AuthorizationContext authorizationContext)
            : this(authorizationContext, EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance)
        {
        }

        public ServiceSecurityContext(AuthorizationContext authorizationContext, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies)
        {
            if (authorizationContext == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("authorizationContext");
            }
            if (authorizationPolicies == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("authorizationPolicies");
            }
            _authorizationContext = authorizationContext;
            _authorizationPolicies = authorizationPolicies;
        }

        public static ServiceSecurityContext Anonymous
        {
            get
            {
                if (s_anonymous == null)
                {
                    s_anonymous = new ServiceSecurityContext(EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance);
                }
                return s_anonymous;
            }
        }


        public bool IsAnonymous
        {
            get
            {
                return this == Anonymous || this.IdentityClaim == null;
            }
        }

        internal Claim IdentityClaim
        {
            get
            {
                if (_identityClaim == null)
                {
                    _identityClaim = SecurityUtils.GetPrimaryIdentityClaim(this.AuthorizationContext);
                }
                return _identityClaim;
            }
        }

        public IIdentity PrimaryIdentity
        {
            get
            {
                if (_primaryIdentity == null)
                {
                    IIdentity primaryIdentity = null;
                    IList<IIdentity> identities = GetIdentities();
                    // Multiple Identities is treated as anonymous
                    if (identities != null && identities.Count == 1)
                    {
                        primaryIdentity = identities[0];
                    }

                    _primaryIdentity = primaryIdentity ?? SecurityUtils.AnonymousIdentity;
                }
                return _primaryIdentity;
            }
        }

        public ReadOnlyCollection<IAuthorizationPolicy> AuthorizationPolicies
        {
            get
            {
                return _authorizationPolicies;
            }
            set
            {
                _authorizationPolicies = value;
            }
        }

        public AuthorizationContext AuthorizationContext
        {
            get
            {
                if (_authorizationContext == null)
                {
                    _authorizationContext = AuthorizationContext.CreateDefaultAuthorizationContext(_authorizationPolicies);
                }
                return _authorizationContext;
            }
        }

        private IList<IIdentity> GetIdentities()
        {
            object identities;
            AuthorizationContext authContext = this.AuthorizationContext;
            if (authContext != null && authContext.Properties.TryGetValue(SecurityUtils.Identities, out identities))
            {
                return identities as IList<IIdentity>;
            }
            return null;
        }
    }
}
