// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !SUPPORTS_WINDOWSIDENTITY

using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Runtime;
using System.Security;
using System.Security.Principal;
using System.ServiceModel;

namespace System.IdentityModel.Claims
{
    // Stub implementation of WindowsClaimSet, since WindowsIdentity is not supported in UWP contracts or Unix yet

    public class WindowsClaimSet : ClaimSet, IIdentityInfo, IDisposable
    {
        internal const bool DefaultIncludeWindowsGroups = true;
        private const string WindowsStreamSecurityNotSupportedUWP = "Windows Stream Security not yet supported on UWP"; 

        public override Claim this[int index]
        {
            get
            {
                throw ExceptionHelper.PlatformNotSupported(WindowsStreamSecurityNotSupportedUWP); 
            }
        }

        public override int Count
        {
            get
            {
                throw ExceptionHelper.PlatformNotSupported(WindowsStreamSecurityNotSupportedUWP); 
            }
        }

        public IIdentity Identity
        {
            get
            {
                throw ExceptionHelper.PlatformNotSupported(WindowsStreamSecurityNotSupportedUWP); 
            }
        }

        public override ClaimSet Issuer
        {
            get
            {
                throw ExceptionHelper.PlatformNotSupported(WindowsStreamSecurityNotSupportedUWP); 
            }
        }

        public void Dispose()
        {
            throw ExceptionHelper.PlatformNotSupported(WindowsStreamSecurityNotSupportedUWP); 
        }

        public override IEnumerable<Claim> FindClaims(string claimType, string right)
        {
            throw ExceptionHelper.PlatformNotSupported(WindowsStreamSecurityNotSupportedUWP); 
        }

        public override IEnumerator<Claim> GetEnumerator()
        {
            throw ExceptionHelper.PlatformNotSupported(WindowsStreamSecurityNotSupportedUWP); 
        }
    }
}

#endif // !SUPPORTS_WINDOWSIDENTITY
