// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net;
using System.Net.Security;
using System.Runtime;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading;
using Microsoft.Xml;

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
            if (impersonationLevel == TokenImpersonationLevel.None)
            {
                return "none";
            }
            if (impersonationLevel == TokenImpersonationLevel.Anonymous)
            {
                return "anonymous";
            }
            if (impersonationLevel == TokenImpersonationLevel.Impersonation)
            {
                return "impersonation";
            }
            if (impersonationLevel == TokenImpersonationLevel.Delegation)
            {
                return "delegation";
            }

            Fx.Assert("unknown token impersonation level");
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("impersonationLevel", (int)impersonationLevel,
            typeof(TokenImpersonationLevel)));
        }

        internal static bool IsGreaterOrEqual(TokenImpersonationLevel x, TokenImpersonationLevel y)
        {
            Validate(x);
            Validate(y);

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

        internal static IIdentity AnonymousIdentity
        {
            get
            {
                if (s_anonymousIdentity == null)
                {
                    s_anonymousIdentity = CreateIdentity(string.Empty);
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
            return CreateWindowsIdentity();
        }

#if !SUPPORTS_WINDOWSIDENTITY
        internal static EndpointIdentity CreateWindowsIdentity(bool spnOnly)
        {
            EndpointIdentity identity = null;
            if (spnOnly)
            {
                identity = EndpointIdentity.CreateSpnIdentity(String.Format(CultureInfo.InvariantCulture, "host/{0}", DnsCache.MachineName));
            }
            else
            {
                throw ExceptionHelper.PlatformNotSupported();
            }

            return identity;
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
            return CloneWindowsIdentityIfNecessary(wid, null);
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
            return new WindowsIdentity(token);
        }
#endif // !SUPPORTS_WINDOWSIDENTITY

        internal static string GetSpnFromIdentity(EndpointIdentity identity, EndpointAddress target)
        {
            bool foundSpn = false;
            string spn = null;
            if (identity != null)
            {
                if (ClaimTypes.Spn.Equals(identity.IdentityClaim.ClaimType))
                {
                    spn = (string)identity.IdentityClaim.Resource;
                    foundSpn = true;
                }
                else if (ClaimTypes.Upn.Equals(identity.IdentityClaim.ClaimType))
                {
                    spn = (string)identity.IdentityClaim.Resource;
                    foundSpn = true;
                }
                else if (ClaimTypes.Dns.Equals(identity.IdentityClaim.ClaimType))
                {
                    spn = string.Format(CultureInfo.InvariantCulture, "host/{0}", (string)identity.IdentityClaim.Resource);
                    foundSpn = true;
                }
            }
            if (!foundSpn)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(string.Format(SRServiceModel.CannotDetermineSPNBasedOnAddress, target)));
            }
            return spn;
        }

        internal static string GetSpnFromTarget(EndpointAddress target)
        {
            if (target == null)
            {
                throw Fx.AssertAndThrow("target should not be null - expecting an EndpointAddress");
            }

            return string.Format(CultureInfo.InvariantCulture, "host/{0}", target.Uri.DnsSafeHost);
        }

        internal static byte[] ReadContentAsBase64(XmlDictionaryReader reader, long maxBufferSize)
        {
            if (reader == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");

            // Code cloned from System.Xml.XmlDictionaryReder.
            byte[][] buffers = new byte[32][];
            byte[] buffer;
            // Its best to read in buffers that are a multiple of 3 so we don't break base64 boundaries when converting text
            int count = 384;
            int bufferCount = 0;
            int totalRead = 0;
            while (true)
            {
                buffer = new byte[count];
                buffers[bufferCount++] = buffer;
                int read = 0;
                while (read < buffer.Length)
                {
                    int actual = reader.ReadContentAsBase64(buffer, read, buffer.Length - read);
                    if (actual == 0)
                        break;
                    read += actual;
                }
                if (totalRead > maxBufferSize - read)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new QuotaExceededException(string.Format(SRServiceModel.BufferQuotaExceededReadingBase64, maxBufferSize)));
                totalRead += read;
                if (read < buffer.Length)
                    break;
                count = count * 2;
            }
            buffer = new byte[totalRead];
            int offset = 0;
            for (int i = 0; i < bufferCount - 1; i++)
            {
                Buffer.BlockCopy(buffers[i], 0, buffer, offset, buffers[i].Length);
                offset += buffers[i].Length;
            }
            Buffer.BlockCopy(buffers[bufferCount - 1], 0, buffer, offset, totalRead - offset);
            return buffer;
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

        internal static ReadOnlyCollection<IAuthorizationPolicy> CreatePrincipalNameAuthorizationPolicies(string principalName)
        {
            if (principalName == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("principalName");

            Claim identityClaim;
            Claim primaryPrincipal;
            if (principalName.Contains("@") || principalName.Contains(@"\"))
            {
                identityClaim = new Claim(ClaimTypes.Upn, principalName, Rights.Identity);
#if SUPPORTS_WINDOWSIDENTITY
                primaryPrincipal = Claim.CreateUpnClaim(principalName);
#else
                throw ExceptionHelper.PlatformNotSupported("UPN claim not supported");
#endif // SUPPORTS_WINDOWSIDENTITY
            }
            else
            {
                identityClaim = new Claim(ClaimTypes.Spn, principalName, Rights.Identity);
                primaryPrincipal = Claim.CreateSpnClaim(principalName);
            }

            List<Claim> claims = new List<Claim>(2);
            claims.Add(identityClaim);
            claims.Add(primaryPrincipal);

            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>(1);
            policies.Add(new UnconditionalPolicy(SecurityUtils.CreateIdentity(principalName), new DefaultClaimSet(ClaimSet.Anonymous, claims)));
            return policies.AsReadOnly();
        }

        internal static string GetIdentityNamesFromContext(AuthorizationContext authContext)
        {
            if (authContext == null)
                return String.Empty;

            StringBuilder str = new StringBuilder(256);
            for (int i = 0; i < authContext.ClaimSets.Count; ++i)
            {
                ClaimSet claimSet = authContext.ClaimSets[i];

                // Windows
                WindowsClaimSet windows = claimSet as WindowsClaimSet;
                if (windows != null)
                {
#if SUPPORTS_WINDOWSIDENTITY
                    if (str.Length > 0)
                        str.Append(", ");

                    AppendIdentityName(str, windows.WindowsIdentity);
#else
                    throw ExceptionHelper.PlatformNotSupported(ExceptionHelper.WinsdowsStreamSecurityNotSupported);
#endif // SUPPORTS_WINDOWSIDENTITY 
                }
                else
                {
                    // X509
                    X509CertificateClaimSet x509 = claimSet as X509CertificateClaimSet;
                    if (x509 != null)
                    {
                        if (str.Length > 0)
                            str.Append(", ");

                        AppendCertificateIdentityName(str, x509.X509Certificate);
                    }
                }
            }

            if (str.Length <= 0)
            {
                List<IIdentity> identities = null;
                object obj;
                if (authContext.Properties.TryGetValue(SecurityUtils.Identities, out obj))
                {
                    identities = obj as List<IIdentity>;
                }
                if (identities != null)
                {
                    for (int i = 0; i < identities.Count; ++i)
                    {
                        IIdentity identity = identities[i];
                        if (identity != null)
                        {
                            if (str.Length > 0)
                                str.Append(", ");

                            AppendIdentityName(str, identity);
                        }
                    }
                }
            }
            return str.Length <= 0 ? String.Empty : str.ToString();
        }

        internal static void AppendCertificateIdentityName(StringBuilder str, X509Certificate2 certificate)
        {
            string value = certificate.SubjectName.Name;
            if (String.IsNullOrEmpty(value))
            {
                value = certificate.GetNameInfo(X509NameType.DnsName, false);
                if (String.IsNullOrEmpty(value))
                {
                    value = certificate.GetNameInfo(X509NameType.SimpleName, false);
                    if (String.IsNullOrEmpty(value))
                    {
                        value = certificate.GetNameInfo(X509NameType.EmailName, false);
                        if (String.IsNullOrEmpty(value))
                        {
                            value = certificate.GetNameInfo(X509NameType.UpnName, false);
                        }
                    }
                }
            }
            // Same format as X509Identity
            str.Append(String.IsNullOrEmpty(value) ? "<x509>" : value);
            str.Append("; ");
            str.Append(certificate.Thumbprint);
        }

        internal static void AppendIdentityName(StringBuilder str, IIdentity identity)
        {
            string name = null;
            try
            {
                name = identity.Name;
            }
#pragma warning disable 56500
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                // suppress exception, this is just info.
            }

            str.Append(String.IsNullOrEmpty(name) ? "<null>" : name);
#if SUPPORTS_WINDOWSIDENTITY // NegotiateStream
            WindowsIdentity windows = identity as WindowsIdentity;
            if (windows != null)
            {
                if (windows.User != null)
                {
                    str.Append("; ");
                    str.Append(windows.User.ToString());
                }
            }
            else
            {
                WindowsSidIdentity sid = identity as WindowsSidIdentity;
                if (sid != null)
                {
                    str.Append("; ");
                    str.Append(sid.SecurityIdentifier.ToString());
                }
            }
#else
            throw ExceptionHelper.PlatformNotSupported(ExceptionHelper.WinsdowsStreamSecurityNotSupported);
#endif // SUPPORTS_WINDOWSIDENTITY
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

        internal static SecurityStandardsManager CreateSecurityStandardsManager(SecurityTokenRequirement requirement, SecurityTokenManager tokenManager)
        {
            MessageSecurityTokenVersion securityVersion = (MessageSecurityTokenVersion)requirement.GetProperty<MessageSecurityTokenVersion>(ServiceModelSecurityTokenRequirement.MessageSecurityVersionProperty);
            if (securityVersion == MessageSecurityTokenVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005BasicSecurityProfile10)
                return CreateSecurityStandardsManager(MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10, tokenManager);
            if (securityVersion == MessageSecurityTokenVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005)
                return CreateSecurityStandardsManager(MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11, tokenManager);
            if (securityVersion == MessageSecurityTokenVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005BasicSecurityProfile10)
                return CreateSecurityStandardsManager(MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10, tokenManager);
            if (securityVersion == MessageSecurityTokenVersion.WSSecurity10WSTrust13WSSecureConversation13BasicSecurityProfile10)
                return CreateSecurityStandardsManager(MessageSecurityVersion.WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10, tokenManager);
            if (securityVersion == MessageSecurityTokenVersion.WSSecurity11WSTrust13WSSecureConversation13)
                return CreateSecurityStandardsManager(MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12, tokenManager);
            if (securityVersion == MessageSecurityTokenVersion.WSSecurity11WSTrust13WSSecureConversation13BasicSecurityProfile10)
                return CreateSecurityStandardsManager(MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10, tokenManager);
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
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
                result = new NetworkCredential(networkCredential.UserName, networkCredential.Password, networkCredential.Domain);
            }
            else
            {
                result = networkCredential;
            }
            return result;
        }

        internal static NetworkCredential GetNetworkCredentialOrDefault(NetworkCredential credential)
        {
            // Because CredentialCache.DefaultNetworkCredentials is not immutable, we dont use it in our OM. Instead we
            // use an empty NetworkCredential to denote the default credentials.
            if (NetworkCredentialHelper.IsNullOrEmpty(credential))
            {
                return CredentialCache.DefaultNetworkCredentials;
            }

            return credential;
        }

        internal static string AppendWindowsAuthenticationInfo(string inputString, NetworkCredential credential,
            AuthenticationLevel authenticationLevel, TokenImpersonationLevel impersonationLevel)
        {
            const string delimiter = "\0"; // nonprintable characters are invalid for SSPI Domain/UserName/Password

            if (NetworkCredentialHelper.IsDefault(credential))
            {
                string sid = NetworkCredentialHelper.GetCurrentUserIdAsString(credential);
                return string.Concat(inputString, delimiter,
                    sid, delimiter,
                    AuthenticationLevelHelper.ToString(authenticationLevel), delimiter,
                    TokenImpersonationLevelHelper.ToString(impersonationLevel));
            }
            return string.Concat(inputString, delimiter,
                credential.Domain, delimiter,
                credential.UserName, delimiter,
                credential.Password, delimiter,
                AuthenticationLevelHelper.ToString(authenticationLevel), delimiter,
                TokenImpersonationLevelHelper.ToString(impersonationLevel));
        }

        internal static class NetworkCredentialHelper
        {
            static internal bool IsNullOrEmpty(NetworkCredential credential)
            {
                return credential == null ||
                        (
                            string.IsNullOrEmpty(credential.UserName) &&
                            string.IsNullOrEmpty(credential.Domain) &&
                            string.IsNullOrEmpty(credential.Password)
                        );
            }

            static internal bool IsDefault(NetworkCredential credential)
            {
                return CredentialCache.DefaultNetworkCredentials.Equals(credential);
            }

            internal static string GetCurrentUserIdAsString(NetworkCredential credential)
            {
#if SUPPORTS_WINDOWSIDENTITY
                using (WindowsIdentity self = WindowsIdentity.GetCurrent())
                {
                    return self.User.Value;
                }
#else
                // There's no way to retrieve the current logged in user Id in UWP apps or in
                // *NIX so returning a username which is very unlikely to be a real username;
                return "____CURRENTUSER_NOT_AVAILABLE____";
#endif
            }
        }
        internal static byte[] CloneBuffer(byte[] buffer)
        {
            byte[] copy = Fx.AllocateByteArray(buffer.Length);
            Buffer.BlockCopy(buffer, 0, copy, 0, buffer.Length);
            return copy;
        }

        internal static bool TryCreateX509CertificateFromRawData(byte[] rawData, out X509Certificate2 certificate)
        {
            certificate = (rawData == null || rawData.Length == 0) ? null : new X509Certificate2(rawData);
            return certificate != null && certificate.Handle != IntPtr.Zero;
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

        internal static X509Certificate2 GetCertificateFromStore(StoreName storeName, StoreLocation storeLocation,
            X509FindType findType, object findValue, EndpointAddress target)
        {
            X509Certificate2 certificate = GetCertificateFromStoreCore(storeName, storeLocation, findType, findValue, target, true);
            if (certificate == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.CannotFindCert, storeName, storeLocation, findType, findValue)));

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
                ResetAllCertificates(certs);
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
                    return new InvalidOperationException(string.Format(SRServiceModel.CannotFindCert, storeName, storeLocation, findType, findValue));
                }
                return new InvalidOperationException(string.Format(SRServiceModel.CannotFindCertForTarget, storeName, storeLocation, findType, findValue, target));
            }
            if (target == null)
            {
                return new InvalidOperationException(string.Format(SRServiceModel.FoundMultipleCerts, storeName, storeLocation, findType, findValue));
            }
            return new InvalidOperationException(string.Format(SRServiceModel.FoundMultipleCertsForTarget, storeName, storeLocation, findType, findValue, target));
        }

        internal static void FixNetworkCredential(ref NetworkCredential credential)
        {
            if (credential == null)
            {
                return;
            }
            string username = credential.UserName;
            string domain = credential.Domain;
            if (!string.IsNullOrEmpty(username) && string.IsNullOrEmpty(domain))
            {
                // do the splitting only if there is exactly 1 \ or exactly 1 @
                string[] partsWithSlashDelimiter = username.Split('\\');
                string[] partsWithAtDelimiter = username.Split('@');
                if (partsWithSlashDelimiter.Length == 2 && partsWithAtDelimiter.Length == 1)
                {
                    if (!string.IsNullOrEmpty(partsWithSlashDelimiter[0]) && !string.IsNullOrEmpty(partsWithSlashDelimiter[1]))
                    {
                        credential = new NetworkCredential(partsWithSlashDelimiter[1], credential.Password, partsWithSlashDelimiter[0]);
                    }
                }
                else if (partsWithSlashDelimiter.Length == 1 && partsWithAtDelimiter.Length == 2)
                {
                    if (!string.IsNullOrEmpty(partsWithAtDelimiter[0]) && !string.IsNullOrEmpty(partsWithAtDelimiter[1]))
                    {
                        credential = new NetworkCredential(partsWithAtDelimiter[0], credential.Password, partsWithAtDelimiter[1]);
                    }
                }
            }
        }

#if SUPPORTS_WINDOWSIDENTITY // NegotiateStream
        public static void ValidateAnonymityConstraint(WindowsIdentity identity, bool allowUnauthenticatedCallers)
        {
            if (!allowUnauthenticatedCallers && identity.User.IsWellKnown(WellKnownSidType.AnonymousSid))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(
                    new SecurityTokenValidationException(SR.AnonymousLogonsAreNotAllowed));
            }
        }
#endif // SUPPORTS_WINDOWSIDENTITY 

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
            return Create(s_commonPrefix);
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
#pragma warning disable 56500 // covered by FxCOP
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
            End<OperationWithTimeoutAsyncResult>(result);
        }
    }
}
