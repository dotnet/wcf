// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net;
using System.Net.Security;
using System.Runtime;
using System.Security;
using System.Security.Authentication;
#if FEATURE_CORECLR // X509Certificate
using System.Security.Cryptography.X509Certificates;
#endif // FEATURE_CORECLR
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.Threading;

namespace System.ServiceModel.Security
{
    public static class ProtectionLevelHelper
    {
        public static bool IsDefined(ProtectionLevel value)
        {
            return (value == ProtectionLevel.None
                || value == ProtectionLevel.Sign
                || value == ProtectionLevel.EncryptAndSign);
        }

        public static void Validate(ProtectionLevel value)
        {
            if (!IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("value", (int)value,
                    typeof(ProtectionLevel)));
            }
        }

        public static bool IsStronger(ProtectionLevel v1, ProtectionLevel v2)
        {
            return ((v1 == ProtectionLevel.EncryptAndSign && v2 != ProtectionLevel.EncryptAndSign)
                    || (v1 == ProtectionLevel.Sign && v2 == ProtectionLevel.None));
        }

        public static bool IsStrongerOrEqual(ProtectionLevel v1, ProtectionLevel v2)
        {
            return (v1 == ProtectionLevel.EncryptAndSign
                    || (v1 == ProtectionLevel.Sign && v2 != ProtectionLevel.EncryptAndSign));
        }

        public static ProtectionLevel Max(ProtectionLevel v1, ProtectionLevel v2)
        {
            return IsStronger(v1, v2) ? v1 : v2;
        }

        public static int GetOrdinal(Nullable<ProtectionLevel> p)
        {
            if (p.HasValue)
            {
                switch ((ProtectionLevel)p)
                {
                    default:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("p", (int)p,
                        typeof(ProtectionLevel)));
                    case ProtectionLevel.None:
                        return 2;
                    case ProtectionLevel.Sign:
                        return 3;
                    case ProtectionLevel.EncryptAndSign:
                        return 4;
                }
            }
            else
                return 1;
        }
    }

    internal static class SslProtocolsHelper
    {
        internal static bool IsDefined(SslProtocols value)
        {
            SslProtocols allValues = SslProtocols.None;
            foreach (var protocol in Enum.GetValues(typeof(SslProtocols)))
            {
                allValues |= (SslProtocols)protocol;
            }
            return (value & allValues) == value;
        }

        internal static void Validate(SslProtocols value)
        {
            if (!IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("value", (int)value,
                    typeof(SslProtocols)));
            }
        }
    }

    internal static class TokenImpersonationLevelHelper
    {
        internal static bool IsDefined(TokenImpersonationLevel value)
        {
            return (value == TokenImpersonationLevel.None
                || value == TokenImpersonationLevel.Anonymous
                || value == TokenImpersonationLevel.Identification
                || value == TokenImpersonationLevel.Impersonation
                || value == TokenImpersonationLevel.Delegation);
        }

        internal static void Validate(TokenImpersonationLevel value)
        {
            if (!IsDefined(value))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("value", (int)value,
                    typeof(TokenImpersonationLevel)));
            }
        }

        private static TokenImpersonationLevel[] s_TokenImpersonationLevelOrder = new TokenImpersonationLevel[]
            {
                TokenImpersonationLevel.None,
                TokenImpersonationLevel.Anonymous,
                TokenImpersonationLevel.Identification,
                TokenImpersonationLevel.Impersonation,
                TokenImpersonationLevel.Delegation
            };

        internal static string ToString(TokenImpersonationLevel impersonationLevel)
        {
            if (impersonationLevel == TokenImpersonationLevel.Identification)
            {
                return "identification";
            }
            else if (impersonationLevel == TokenImpersonationLevel.None)
            {
                return "none";
            }
            else if (impersonationLevel == TokenImpersonationLevel.Anonymous)
            {
                return "anonymous";
            }
            else if (impersonationLevel == TokenImpersonationLevel.Impersonation)
            {
                return "impersonation";
            }
            else if (impersonationLevel == TokenImpersonationLevel.Delegation)
            {
                return "delegation";
            }

            Fx.Assert("unknown token impersonation level");
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("impersonationLevel", (int)impersonationLevel,
            typeof(TokenImpersonationLevel)));
        }

        internal static bool IsGreaterOrEqual(TokenImpersonationLevel x, TokenImpersonationLevel y)
        {
            TokenImpersonationLevelHelper.Validate(x);
            TokenImpersonationLevelHelper.Validate(y);

            if (x == y)
                return true;

            int px = 0;
            int py = 0;
            for (int i = 0; i < s_TokenImpersonationLevelOrder.Length; i++)
            {
                if (x == s_TokenImpersonationLevelOrder[i])
                    px = i;
                if (y == s_TokenImpersonationLevelOrder[i])
                    py = i;
            }

            return (px > py);
        }

        internal static int Compare(TokenImpersonationLevel x, TokenImpersonationLevel y)
        {
            int result = 0;

            if (x != y)
            {
                switch (x)
                {
                    case TokenImpersonationLevel.Identification:
                        result = -1;
                        break;
                    case TokenImpersonationLevel.Impersonation:
                        switch (y)
                        {
                            case TokenImpersonationLevel.Identification:
                                result = 1;
                                break;
                            case TokenImpersonationLevel.Delegation:
                                result = -1;
                                break;
                            default:
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("y", (int)y,
                                    typeof(TokenImpersonationLevel)));
                        }
                        break;
                    case TokenImpersonationLevel.Delegation:
                        result = 1;
                        break;
                    default:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("x", (int)x,
                            typeof(TokenImpersonationLevel)));
                }
            }

            return result;
        }
    }

    internal static class SecurityUtils
    {
        public const string Principal = "Principal";
        public const string Identities = "Identities";
        private static IIdentity s_anonymousIdentity;
#if FEATURE_CORECLR
        private static X509SecurityTokenAuthenticator s_nonValidatingX509Authenticator;

        internal static X509SecurityTokenAuthenticator NonValidatingX509Authenticator
        {
            get
            {
                if (s_nonValidatingX509Authenticator == null)
                {
                    s_nonValidatingX509Authenticator = new X509SecurityTokenAuthenticator(X509CertificateValidator.None);
                }
                return s_nonValidatingX509Authenticator;
            }
        }
#endif // FEATURE_CORECLR

        internal static IIdentity AnonymousIdentity
        {
            get
            {
                if (s_anonymousIdentity == null)
                {
                    s_anonymousIdentity = SecurityUtils.CreateIdentity(string.Empty);
                }
                return s_anonymousIdentity;
            }
        }

        public static DateTime MaxUtcDateTime
        {
            get
            {
                // + and -  TimeSpan.TicksPerDay is to compensate the DateTime.ParseExact (to localtime) overflow.
                return new DateTime(DateTime.MaxValue.Ticks - TimeSpan.TicksPerDay, DateTimeKind.Utc);
            }
        }

        public static DateTime MinUtcDateTime
        {
            get
            {
                // + and -  TimeSpan.TicksPerDay is to compensate the DateTime.ParseExact (to localtime) overflow.
                return new DateTime(DateTime.MinValue.Ticks + TimeSpan.TicksPerDay, DateTimeKind.Utc);
            }
        }

        internal static IIdentity CreateIdentity(string name)
        {
            return new GenericIdentity(name);
        }

        internal static EndpointIdentity CreateWindowsIdentity()
        {
            return CreateWindowsIdentity(false);
        }

        internal static EndpointIdentity CreateWindowsIdentity(NetworkCredential serverCredential)
        {
            if (serverCredential != null && !NetworkCredentialHelper.IsDefault(serverCredential))
            {
                string upn;
                if (serverCredential.Domain != null && serverCredential.Domain.Length > 0)
                {
                    upn = serverCredential.UserName + "@" + serverCredential.Domain;
                }
                else
                {
                    upn = serverCredential.UserName;
                }
                return EndpointIdentity.CreateUpnIdentity(upn);
            }
            else
            {
                return SecurityUtils.CreateWindowsIdentity();
            }
        }

#if FEATURE_NETNATIVE
        private static bool IsSystemAccount(WindowsIdentity self)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
#else
        private static bool IsSystemAccount(WindowsIdentity self)
        {
            SecurityIdentifier sid = self.User;
            if (sid == null)
            {
                return false;
            }
            // S-1-5-82 is the prefix for the sid that represents the identity that IIS 7.5 Apppool thread runs under.
            return (sid.IsWellKnown(WellKnownSidType.LocalSystemSid)
                    || sid.IsWellKnown(WellKnownSidType.NetworkServiceSid)
                    || sid.IsWellKnown(WellKnownSidType.LocalServiceSid)
                    || self.User.Value.StartsWith("S-1-5-82", StringComparison.OrdinalIgnoreCase));
        }
#endif // FEATURE_NETNATIVE
        internal static EndpointIdentity CreateWindowsIdentity(bool spnOnly)
        {
            EndpointIdentity identity = null;
            using (WindowsIdentity self = WindowsIdentity.GetCurrent())
            {
                bool isSystemAccount = IsSystemAccount(self);
                if (spnOnly || isSystemAccount)
                {
                    identity = EndpointIdentity.CreateSpnIdentity(String.Format(CultureInfo.InvariantCulture, "host/{0}", DnsCache.MachineName));
                }
                else
                {
                    // Save windowsIdentity for delay lookup
                    identity = new UpnEndpointIdentity(CloneWindowsIdentityIfNecessary(self));
                }
            }

            return identity;
        }


        internal static WindowsIdentity CloneWindowsIdentityIfNecessary(WindowsIdentity wid)
        {
            return SecurityUtils.CloneWindowsIdentityIfNecessary(wid, null);
        }

        internal static WindowsIdentity CloneWindowsIdentityIfNecessary(WindowsIdentity wid, string authType)
        {
            if (wid != null)
            {
                IntPtr token = UnsafeGetWindowsIdentityToken(wid);
                if (token != IntPtr.Zero)
                {
                    return UnsafeCreateWindowsIdentityFromToken(token, authType);
                }
            }
            return wid;
        }

        private static IntPtr UnsafeGetWindowsIdentityToken(WindowsIdentity wid)
        {
            throw ExceptionHelper.PlatformNotSupported("UnsafeGetWindowsIdentityToken is not supported");
        }


        private static WindowsIdentity UnsafeCreateWindowsIdentityFromToken(IntPtr token, string authType)
        {
            if (authType != null)
                return new WindowsIdentity(token, authType);
            else
                return new WindowsIdentity(token);
        }


        internal static bool IsSupportedAlgorithm(string algorithm, SecurityToken token)
        {
            if (token.SecurityKeys == null)
            {
                return false;
            }
            for (int i = 0; i < token.SecurityKeys.Count; ++i)
            {
                if (token.SecurityKeys[i].IsSupportedAlgorithm(algorithm))
                {
                    return true;
                }
            }
            return false;
        }

        internal static Claim GetPrimaryIdentityClaim(ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies)
        {
            return GetPrimaryIdentityClaim(AuthorizationContext.CreateDefaultAuthorizationContext(authorizationPolicies));
        }

        internal static Claim GetPrimaryIdentityClaim(AuthorizationContext authContext)
        {
            if (authContext != null)
            {
                for (int i = 0; i < authContext.ClaimSets.Count; ++i)
                {
                    ClaimSet claimSet = authContext.ClaimSets[i];
                    foreach (Claim claim in claimSet.FindClaims(null, Rights.Identity))
                    {
                        return claim;
                    }
                }
            }
            return null;
        }

        internal static string GenerateId()
        {
            return SecurityUniqueId.Create().Value;
        }

        internal static void OpenTokenProviderIfRequired(SecurityTokenProvider tokenProvider, TimeSpan timeout)
        {
            OpenCommunicationObject(tokenProvider as ICommunicationObject, timeout);
        }

        internal static void CloseTokenProviderIfRequired(SecurityTokenProvider tokenProvider, TimeSpan timeout)
        {
            CloseCommunicationObject(tokenProvider, false, timeout);
        }

        internal static void AbortTokenProviderIfRequired(SecurityTokenProvider tokenProvider)
        {
            CloseCommunicationObject(tokenProvider, true, TimeSpan.Zero);
        }

        internal static void OpenTokenAuthenticatorIfRequired(SecurityTokenAuthenticator tokenAuthenticator, TimeSpan timeout)
        {
            OpenCommunicationObject(tokenAuthenticator as ICommunicationObject, timeout);
        }

        internal static void CloseTokenAuthenticatorIfRequired(SecurityTokenAuthenticator tokenAuthenticator, TimeSpan timeout)
        {
            CloseTokenAuthenticatorIfRequired(tokenAuthenticator, false, timeout);
        }

        internal static void CloseTokenAuthenticatorIfRequired(SecurityTokenAuthenticator tokenAuthenticator, bool aborted, TimeSpan timeout)
        {
            CloseCommunicationObject(tokenAuthenticator, aborted, timeout);
        }

        internal static void AbortTokenAuthenticatorIfRequired(SecurityTokenAuthenticator tokenAuthenticator)
        {
            CloseCommunicationObject(tokenAuthenticator, true, TimeSpan.Zero);
        }

        private static void OpenCommunicationObject(ICommunicationObject obj, TimeSpan timeout)
        {
            if (obj != null)
                obj.Open(timeout);
        }

        private static void CloseCommunicationObject(Object obj, bool aborted, TimeSpan timeout)
        {
            if (obj != null)
            {
                ICommunicationObject co = obj as ICommunicationObject;
                if (co != null)
                {
                    if (aborted)
                    {
                        try
                        {
                            co.Abort();
                        }
                        catch (CommunicationException)
                        {
                        }
                    }
                    else
                    {
                        co.Close(timeout);
                    }
                }
                else if (obj is IDisposable)
                {
                    ((IDisposable)obj).Dispose();
                }
            }
        }

        internal static SecurityStandardsManager CreateSecurityStandardsManager(MessageSecurityVersion securityVersion, SecurityTokenManager tokenManager)
        {
            SecurityTokenSerializer tokenSerializer = tokenManager.CreateSecurityTokenSerializer(securityVersion.SecurityTokenVersion);
            return new SecurityStandardsManager(securityVersion, tokenSerializer);
        }

        internal static SecurityStandardsManager CreateSecurityStandardsManager(MessageSecurityVersion securityVersion, SecurityTokenSerializer securityTokenSerializer)
        {
            if (securityVersion == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("securityVersion"));
            }
            if (securityTokenSerializer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("securityTokenSerializer");
            }
            return new SecurityStandardsManager(securityVersion, securityTokenSerializer);
        }

        internal static NetworkCredential GetNetworkCredentialsCopy(NetworkCredential networkCredential)
        {
            NetworkCredential result;
            if (networkCredential != null && !NetworkCredentialHelper.IsDefault(networkCredential))
            {
                result = new NetworkCredential(NetworkCredentialHelper.UnsafeGetUsername(networkCredential), NetworkCredentialHelper.UnsafeGetPassword(networkCredential), NetworkCredentialHelper.UnsafeGetDomain(networkCredential));
            }
            else
            {
                result = networkCredential;
            }
            return result;
        }

        internal static NetworkCredential GetNetworkCredentialOrDefault(NetworkCredential credential)
        {
            // because of VSW 564452, we dont use CredentialCache.DefaultNetworkCredentials in our OM. Instead we
            // use an empty NetworkCredential to denote the default credentials
            if (NetworkCredentialHelper.IsNullOrEmpty(credential))
            {
                // FYI: this will fail with SecurityException in PT due to Demand for EnvironmentPermission.
                // Typically a PT app should not have access to DefaultNetworkCredentials. If there is a valid reason,
                // see UnsafeGetDefaultNetworkCredentials.
                return CredentialCache.DefaultNetworkCredentials;
            }
            else
            {
                return credential;
            }
        }

        internal static class NetworkCredentialHelper
        {
            static internal bool IsNullOrEmpty(NetworkCredential credential)
            {
                return credential == null ||
                        (
                            String.IsNullOrEmpty(UnsafeGetUsername(credential)) &&
                            String.IsNullOrEmpty(UnsafeGetDomain(credential)) &&
                            String.IsNullOrEmpty(UnsafeGetPassword(credential))
                        );
            }

            static internal bool IsDefault(NetworkCredential credential)
            {
                return UnsafeGetDefaultNetworkCredentials().Equals(credential);
            }

            static internal string UnsafeGetUsername(NetworkCredential credential)
            {
                return credential.UserName;
            }

            static internal string UnsafeGetPassword(NetworkCredential credential)
            {
                return credential.Password;
            }

            static internal string UnsafeGetDomain(NetworkCredential credential)
            {
                return credential.Domain;
            }

            private static NetworkCredential UnsafeGetDefaultNetworkCredentials()
            {
                return CredentialCache.DefaultNetworkCredentials;
            }
        }
        internal static byte[] CloneBuffer(byte[] buffer)
        {
            byte[] copy = Fx.AllocateByteArray(buffer.Length);
            Buffer.BlockCopy(buffer, 0, copy, 0, buffer.Length);
            return copy;
        }
        internal static string GetKeyDerivationAlgorithm(SecureConversationVersion version)
        {
            string derivationAlgorithm = null;
            if (version == SecureConversationVersion.WSSecureConversationFeb2005)
            {
                derivationAlgorithm = SecurityAlgorithms.Psha1KeyDerivation;
            }
            else if (version == SecureConversationVersion.WSSecureConversation13)
            {
                derivationAlgorithm = SecurityAlgorithms.Psha1KeyDerivationDec2005;
            }
            else
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }

            return derivationAlgorithm;
        }

#if FEATURE_CORECLR // X509Certificate
        internal static X509Certificate2 GetCertificateFromStore(StoreName storeName, StoreLocation storeLocation,
            X509FindType findType, object findValue, EndpointAddress target)
        {
            X509Certificate2 certificate = GetCertificateFromStoreCore(storeName, storeLocation, findType, findValue, target, true);
            if (certificate == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.CannotFindCert, storeName, storeLocation, findType, findValue)));

            return certificate;
        }

        internal static bool TryGetCertificateFromStore(StoreName storeName, StoreLocation storeLocation,
            X509FindType findType, object findValue, EndpointAddress target, out X509Certificate2 certificate)
        {
            certificate = GetCertificateFromStoreCore(storeName, storeLocation, findType, findValue, target, false);
            return (certificate != null);
        }

        private static X509Certificate2 GetCertificateFromStoreCore(StoreName storeName, StoreLocation storeLocation,
            X509FindType findType, object findValue, EndpointAddress target, bool throwIfMultipleOrNoMatch)
        {
            if (findValue == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("findValue");
            }

            X509Store store = new X509Store(storeName, storeLocation);
            X509Certificate2Collection certs = null;
            try
            {
                store.Open(OpenFlags.ReadOnly);
                certs = store.Certificates.Find(findType, findValue, false);
                if (certs.Count == 1)
                {
                    return new X509Certificate2(certs[0].Handle);
                }
                if (throwIfMultipleOrNoMatch)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateCertificateLoadException(
                        storeName, storeLocation, findType, findValue, target, certs.Count));
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                SecurityUtils.ResetAllCertificates(certs);
                store.Dispose();
            }
        }

        private static Exception CreateCertificateLoadException(StoreName storeName, StoreLocation storeLocation,
            X509FindType findType, object findValue, EndpointAddress target, int certCount)
        {
            if (certCount == 0)
            {
                if (target == null)
                {
                    return new InvalidOperationException(SR.Format(SR.CannotFindCert, storeName, storeLocation, findType, findValue));
                }
                else
                {
                    return new InvalidOperationException(SR.Format(SR.CannotFindCertForTarget, storeName, storeLocation, findType, findValue, target));
                }
            }
            else
            {
                if (target == null)
                {
                    return new InvalidOperationException(SR.Format(SR.FoundMultipleCerts, storeName, storeLocation, findType, findValue));
                }
                else
                {
                    return new InvalidOperationException(SR.Format(SR.FoundMultipleCertsForTarget, storeName, storeLocation, findType, findValue, target));
                }
            }
        }

        // This is the workaround, Since store.Certificates returns a full collection
        // of certs in store.  These are holding native resources.
        internal static void ResetAllCertificates(X509Certificate2Collection certificates)
        {
            if (certificates != null)
            {
                for (int i = 0; i < certificates.Count; ++i)
                {
                    ResetCertificate(certificates[i]);
                }
            }
        }

        internal static void ResetCertificate(X509Certificate2 certificate)
        {
            // Check that Dispose() and Reset() do the same thing
            certificate.Dispose();
        }
#endif // FEATURE_CORECLR
    }

    internal struct SecurityUniqueId
    {
        private static long s_nextId = 0;
        private static string s_commonPrefix = "uuid-" + Guid.NewGuid().ToString() + "-";

        private long _id;
        private string _prefix;
        private string _val;

        private SecurityUniqueId(string prefix, long id)
        {
            _id = id;
            _prefix = prefix;
            _val = null;
        }

        public static SecurityUniqueId Create()
        {
            return SecurityUniqueId.Create(s_commonPrefix);
        }

        public static SecurityUniqueId Create(string prefix)
        {
            return new SecurityUniqueId(prefix, Interlocked.Increment(ref s_nextId));
        }

        public string Value
        {
            get
            {
                if (_val == null)
                    _val = _prefix + _id.ToString(CultureInfo.InvariantCulture);

                return _val;
            }
        }
    }

    internal static class EmptyReadOnlyCollection<T>
    {
        public static ReadOnlyCollection<T> Instance = new ReadOnlyCollection<T>(new List<T>());
    }

    internal class OperationWithTimeoutAsyncResult : TraceAsyncResult
    {
        private static readonly Action<object> s_scheduledCallback = new Action<object>(OnScheduled);
        private TimeoutHelper _timeoutHelper;
        private Action<TimeSpan> _operationWithTimeout;

        public OperationWithTimeoutAsyncResult(Action<TimeSpan> operationWithTimeout, TimeSpan timeout, AsyncCallback callback, object state)
            : base(callback, state)
        {
            _operationWithTimeout = operationWithTimeout;
            _timeoutHelper = new TimeoutHelper(timeout);
            ActionItem.Schedule(s_scheduledCallback, this);
        }

        private static void OnScheduled(object state)
        {
            OperationWithTimeoutAsyncResult thisResult = (OperationWithTimeoutAsyncResult)state;
            Exception completionException = null;
            try
            {
                using (thisResult.CallbackActivity == null ? null : ServiceModelActivity.BoundOperation(thisResult.CallbackActivity))
                {
                    thisResult._operationWithTimeout(thisResult._timeoutHelper.RemainingTime());
                }
            }
#pragma warning suppress 56500 // covered by FxCOP
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                completionException = e;
            }
            thisResult.Complete(false, completionException);
        }

        public static void End(IAsyncResult result)
        {
            AsyncResult.End<OperationWithTimeoutAsyncResult>(result);
        }
    }
}
