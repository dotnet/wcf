// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Net.Security;

namespace System.ServiceModel.Channels
{
    internal class SecurityCapabilities : ISecurityCapabilities
    {
        internal bool _supportsServerAuth;
        internal bool _supportsClientAuth;
        internal bool _supportsClientWindowsIdentity;
        internal ProtectionLevel _requestProtectionLevel;
        internal ProtectionLevel _responseProtectionLevel;

        public SecurityCapabilities(bool supportsClientAuth, bool supportsServerAuth, bool supportsClientWindowsIdentity,
            ProtectionLevel requestProtectionLevel, ProtectionLevel responseProtectionLevel)
        {
            _supportsClientAuth = supportsClientAuth;
            _supportsServerAuth = supportsServerAuth;
            _supportsClientWindowsIdentity = supportsClientWindowsIdentity;
            _requestProtectionLevel = requestProtectionLevel;
            _responseProtectionLevel = responseProtectionLevel;
        }

        public ProtectionLevel SupportedRequestProtectionLevel { get { return _requestProtectionLevel; } }
        public ProtectionLevel SupportedResponseProtectionLevel { get { return _responseProtectionLevel; } }
        public bool SupportsClientAuthentication { get { return _supportsClientAuth; } }
        public bool SupportsClientWindowsIdentity { get { return _supportsClientWindowsIdentity; } }
        public bool SupportsServerAuthentication { get { return _supportsServerAuth; } }

        public static SecurityCapabilities None
        {
            get { return new SecurityCapabilities(false, false, false, ProtectionLevel.None, ProtectionLevel.None); }
        }

        public static bool IsEqual(ISecurityCapabilities capabilities1, ISecurityCapabilities capabilities2)
        {
            if (capabilities1 == null)
            {
                capabilities1 = SecurityCapabilities.None;
            }

            if (capabilities2 == null)
            {
                capabilities2 = SecurityCapabilities.None;
            }

            if (capabilities1.SupportedRequestProtectionLevel != capabilities2.SupportedRequestProtectionLevel)
            {
                return false;
            }

            if (capabilities1.SupportedResponseProtectionLevel != capabilities2.SupportedResponseProtectionLevel)
            {
                return false;
            }

            if (capabilities1.SupportsClientAuthentication != capabilities2.SupportsClientAuthentication)
            {
                return false;
            }

            if (capabilities1.SupportsClientWindowsIdentity != capabilities2.SupportsClientWindowsIdentity)
            {
                return false;
            }

            if (capabilities1.SupportsServerAuthentication != capabilities2.SupportsServerAuthentication)
            {
                return false;
            }

            return true;
        }
    }
}
