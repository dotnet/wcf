// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;

namespace WcfService
{
    //public enum BasicHttpsSecurityMode
    //{
    //    //
    //    // Summary:
    //    //     The Transport security mode.
    //    Transport,
    //    //
    //    // Summary:
    //    //     The TransportWithMessageCredential security mode.
    //    TransportWithMessageCredential
    //}

    // Cast BasicHttpsSecurityMode (WCF) to BasicHttpSecurityMode (CoreWCF)
    //internal enum BasicHttpsSecurityMode
    //{
    //    None,
    //    Transport,
    //    Message,
    //    TransportWithMessageCredential,
    //    TransportCredentialOnly
    //}

    internal class NetHttpsBinding : NetHttpBinding
    {
        public NetHttpsBinding() : base((BasicHttpSecurityMode)BasicHttpsSecurityMode.Transport)
        {
        }

        public NetHttpsBinding(BasicHttpsSecurityMode securityMode) : base((BasicHttpSecurityMode)securityMode)
        {
        }
    }
}
#endif
