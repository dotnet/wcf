// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Principal;

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

            throw ExceptionHelper.PlatformNotSupported("UpnEndpointIdentity is not supported");
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
