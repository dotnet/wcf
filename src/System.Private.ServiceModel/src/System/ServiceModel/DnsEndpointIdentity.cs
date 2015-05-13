// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace System.ServiceModel
{
    public class DnsEndpointIdentity : EndpointIdentity
    {
        public DnsEndpointIdentity(string dnsName)
        {
            if (dnsName == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("dnsName");

            throw ExceptionHelper.PlatformNotSupported("DnsEndpointIdentity is not supported.");
        }
    }
}
