// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel;

namespace System.Security.Authentication.ExtendedProtection
{
#if FEATURE_NETNATIVE
    public enum ProtectionScenario
    {
        TransportSelected,
        TrustedProxy
    }

    public partial class ExtendedProtectionPolicy //: System.Runtime.Serialization.ISerializable
    {
        private PolicyEnforcement _policyEnforcement;
        private ProtectionScenario _protectionScenario;

        public ExtendedProtectionPolicy(System.Security.Authentication.ExtendedProtection.PolicyEnforcement policyEnforcement)
        {
            _policyEnforcement = policyEnforcement;
            _protectionScenario = ExtendedProtection.ProtectionScenario.TransportSelected;
        }

        public static bool OSSupportsExtendedProtection
        {
            get
            {
                return false;
            }
        }
        public System.Security.Authentication.ExtendedProtection.PolicyEnforcement PolicyEnforcement
        {
            get
            {
                return _policyEnforcement;
            }
        }
        public System.Security.Authentication.ExtendedProtection.ProtectionScenario ProtectionScenario
        {
            get
            {
                return _protectionScenario;
            }
        }
        public override string ToString() { throw ExceptionHelper.PlatformNotSupported(); }
    }
    public enum PolicyEnforcement
    {
        Never,
        WhenSupported,
        Always
    }
#endif //FEATURE_NETNATIVE
}

#if FEATURE_NETNATIVE
namespace System.Security.Principal
{
    internal class SecurityIdentifier { }
    internal class WindowsIdentity : IDisposable
    {
        internal WindowsIdentity(IntPtr token) { }
        internal WindowsIdentity(IntPtr token, string authType) { }
        public static WindowsIdentity GetCurrent() { return null; }
        public SecurityIdentifier User { get { return null; } }

        void IDisposable.Dispose() { }
    }
}
#endif // FEATURE_NETNATIVE
