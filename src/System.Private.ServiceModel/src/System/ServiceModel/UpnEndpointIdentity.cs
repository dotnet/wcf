// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Principal;
using System.IdentityModel.Claims;

namespace System.ServiceModel
{
    public class UpnEndpointIdentity : EndpointIdentity
    {
#if SUPPORTS_WINDOWSIDENTITY
#pragma warning disable 0414 // We don't use this yet in the initial stubbing - remove this once we have references again. 
        private SecurityIdentifier _upnSid;
        private bool _hasUpnSidBeenComputed;
        private WindowsIdentity _windowsIdentity;
#pragma warning restore 0414
#endif // SUPPORTS_WINDOWSIDENTITY

        private Object _thisLock = new Object();

        public UpnEndpointIdentity(string upnName)
        {
            if (upnName == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("upnName");
#if SUPPORTS_WINDOWSIDENTITY
            base.Initialize(Claim.CreateUpnClaim(upnName));
#else 
            throw ExceptionHelper.PlatformNotSupported("UpnEndpointIdentity is not supported on this platform");
#endif // SUPPORTS_WINDOWSIDENTITY
        }

        public UpnEndpointIdentity(Claim identity)
        {
            if (identity == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("identity");

            // PreSharp Bug: Parameter 'identity.ResourceType' to this public method must be validated: A null-dereference can occur here.
            if (!identity.ClaimType.Equals(ClaimTypes.Upn))
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.Format(SR.UnrecognizedClaimTypeForIdentity, identity.ClaimType, ClaimTypes.Upn));

            base.Initialize(identity);
        }

#if SUPPORTS_WINDOWSIDENTITY
        internal UpnEndpointIdentity(WindowsIdentity windowsIdentity)
        {
            if (windowsIdentity == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("windowsIdentity");

            _windowsIdentity = windowsIdentity;
            _upnSid = windowsIdentity.User;
            _hasUpnSidBeenComputed = true;
        }
#endif // SUPPORTS_WINDOWSIDENTITY
    }
}
