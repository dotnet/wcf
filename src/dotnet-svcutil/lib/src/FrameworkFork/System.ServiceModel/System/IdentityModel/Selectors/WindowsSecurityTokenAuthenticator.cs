// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.ServiceModel;

namespace System.IdentityModel.Selectors
{
    public class WindowsSecurityTokenAuthenticator : SecurityTokenAuthenticator
    {
        private bool _includeWindowsGroups;

        public WindowsSecurityTokenAuthenticator()
            : this(WindowsClaimSet.DefaultIncludeWindowsGroups)
        {
        }

        public WindowsSecurityTokenAuthenticator(bool includeWindowsGroups)
        {
            _includeWindowsGroups = includeWindowsGroups;
        }

        protected override bool CanValidateTokenCore(SecurityToken token)
        {
            return token is WindowsSecurityToken;
        }

        protected override ReadOnlyCollection<IAuthorizationPolicy> ValidateTokenCore(SecurityToken token)
        {
#if SUPPORTS_WINDOWSIDENTITY // NegotiateStream
            WindowsSecurityToken windowsToken = (WindowsSecurityToken)token;
            WindowsClaimSet claimSet = new WindowsClaimSet(windowsToken.WindowsIdentity, windowsToken.AuthenticationType, this.includeWindowsGroups, windowsToken.ValidTo);
            return SecurityUtils.CreateAuthorizationPolicies(claimSet, windowsToken.ValidTo);
#else // SUPPORTS_WINDOWSIDENTITY
            throw ExceptionHelper.PlatformNotSupported(ExceptionHelper.WinsdowsStreamSecurityNotSupported);
#endif // SUPPORTS_WINDOWSIDENTITY
        }
    }
}
