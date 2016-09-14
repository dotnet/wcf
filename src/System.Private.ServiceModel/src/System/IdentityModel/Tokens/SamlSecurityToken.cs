// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using DiagnosticUtility = System.ServiceModel.DiagnosticUtility;

namespace System.IdentityModel.Tokens
{
    public class SamlSecurityToken : SecurityToken
    {
        SamlAssertion assertion;

        protected SamlSecurityToken()
        {
        }

        public SamlSecurityToken(SamlAssertion assertion)
        {
            Initialize(assertion);
        }

        protected void Initialize(SamlAssertion assertion)
        {
            if (assertion == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("assertion");

            this.assertion = assertion;
            this.assertion.MakeReadOnly();
        }

        public override string Id
        {
            get { return this.assertion.AssertionId; }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                throw ServiceModel.ExceptionHelper.PlatformNotSupported();  // Issue #31 in progress
                //return this.assertion.SecurityKeys;
            }
        }

        public SamlAssertion Assertion
        {
            get { return this.assertion; }
        }

        public override DateTime ValidFrom
        {
            get
            {
                throw ServiceModel.ExceptionHelper.PlatformNotSupported();  // Issue #31 in progress
                //if (this.assertion.Conditions != null)
                //{
                //    return this.assertion.Conditions.NotBefore;
                //}

                //return SecurityUtils.MinUtcDateTime;
            }
        }

        public override DateTime ValidTo
        {
            get
            {
                throw ServiceModel.ExceptionHelper.PlatformNotSupported();  // Issue #31 in progress
                //if (this.assertion.Conditions != null)
                //{
                //    return this.assertion.Conditions.NotOnOrAfter;
                //}

                //return SecurityUtils.MaxUtcDateTime;
            }
        }

        public override bool CanCreateKeyIdentifierClause<T>()
        {
            throw ServiceModel.ExceptionHelper.PlatformNotSupported();  // Issue #31 in progress
            //if (typeof(T) == typeof(SamlAssertionKeyIdentifierClause))
            //    return true;

            //return false;
        }

        public override T CreateKeyIdentifierClause<T>()
        {
            throw ServiceModel.ExceptionHelper.PlatformNotSupported();  // Issue #31 in progress

            //if (typeof(T) == typeof(SamlAssertionKeyIdentifierClause))
            //    return new SamlAssertionKeyIdentifierClause(this.Id) as T;

            //throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.UnableToCreateTokenReference)));
        }

        public override bool MatchesKeyIdentifierClause(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            throw ServiceModel.ExceptionHelper.PlatformNotSupported();  // Issue #31 in progress
            //SamlAssertionKeyIdentifierClause samlKeyIdentifierClause = keyIdentifierClause as SamlAssertionKeyIdentifierClause;
            //if (samlKeyIdentifierClause != null)
            //    return samlKeyIdentifierClause.Matches(this.Id);

            //return false;
        }
    }
}

