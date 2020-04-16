// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IdentityModel.Claims;
    using System.Runtime;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    // using System.ServiceModel.ComIntegration;
    using System.ServiceModel.Diagnostics;
    using System.Text;
    using Microsoft.Xml;

    public class UpnEndpointIdentity : EndpointIdentity
    {
        private bool _hasUpnSidBeenComputed;

        private Object _thisLock = new Object();

        public UpnEndpointIdentity(string upnName)
        {
            if (upnName == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("upnName");

            base.Initialize(Claim.CreateUpnClaim(upnName));
            _hasUpnSidBeenComputed = false;
        }
    }
}
