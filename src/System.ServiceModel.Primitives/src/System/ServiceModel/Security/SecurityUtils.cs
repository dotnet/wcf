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
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security.Tokens;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Security
{
    internal static class ProtectionLevelHelper
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException(nameof(value), (int)value,
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

        private static readonly TokenImpersonationLevel[] s_tokenImpersonationLevelOrder = new TokenImpersonationLevel[]
            {
                TokenImpersonationLevel.None,
                TokenImpersonationLevel.Anonymous,
                TokenImpersonationLevel.Identification,
                TokenImpersonationLevel.Impersonation,
                TokenImpersonationLevel.Delegation
            };

        internal static string ToString(TokenImpersonationLevel impersonationLevel)
        {
            switch (impersonationLevel)
            {
                case TokenImpersonationLevel.Identification:
                    return "identification";
                case TokenImpersonationLevel.None:
                    return "none";
                case TokenImpersonationLevel.Anonymous:
                    return "anonymous";
                case TokenImpersonationLevel.Impersonation:
                    return "impersonation";
                case TokenImpersonationLevel.Delegation:
                    return "delegation";
            }

            Fx.Assert("unknown token impersonation level");
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException(nameof(impersonationLevel), (int)impersonationLevel,
            typeof(TokenImpersonationLevel)));
        }

        internal static bool IsGreaterOrEqual(TokenImpersonationLevel x, TokenImpersonationLevel y)
        {
            Validate(x);
            Validate(y);

            if (x == y)
            {
                return true;
            }

            int px = 0;
            int py = 0;
            for (int i = 0; i < s_tokenImpersonationLevelOrder.Length; i++)
            {
                if (x == s_tokenImpersonationLevelOrder[i])
                {
                    px = i;
                }

                if (y == s_tokenImpersonationLevelOrder[i])
                {
                    py = i;
                }
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

    internal class ServiceModelDictionaryManager
    {
        private static IdentityModel.DictionaryManager s_dictionaryManager;

        public static IdentityModel.DictionaryManager Instance
        {
            get
            {
                if (s_dictionaryManager == null)
                {
                    s_dictionaryManager = new IdentityModel.DictionaryManager(BinaryMessageEncoderFactory.XmlDictionary);
                }

                return s_dictionaryManager;
            }
        }
    }

    internal static partial class SecurityUtils
    {
        public const string Identities = "Identities";
        public const string Principal = "Principal";
        private static IIdentity s_anonymousIdentity;
        private static X509SecurityTokenAuthenticator s_nonValidatingX509Authenticator;

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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.Format(SRP.CannotDetermineSPNBasedOnAddress, target)));
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

        public static ChannelBinding GetChannelBindingFromMessage(Message message)
        {
            if (message == null)
            {
                return null;
            }

            ChannelBindingMessageProperty channelBindingMessageProperty = null;
            ChannelBindingMessageProperty.TryGet(message, out channelBindingMessageProperty);
            ChannelBinding channelBinding = null;

            if (channelBindingMessageProperty != null)
            {
                channelBinding = channelBindingMessageProperty.ChannelBinding;
            }

            return channelBinding;
        }

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

        internal static T GetSecurityKey<T>(SecurityToken token) where T : SecurityKey
        {
            T result = null;
            if (token.SecurityKeys != null)
            {
                for (int i = 0; i < token.SecurityKeys.Count; ++i)
                {
                    T temp = (token.SecurityKeys[i] as T);
                    if (temp != null)
                    {
                        if (result != null)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SRP.Format(SRP.MultipleMatchingCryptosFound, typeof(T).ToString())));
                        }
                        else
                        {
                            result = temp;
                        }
                    }
                }
            }

            return result;
        }

        internal static byte[] GenerateDerivedKey(SecurityToken tokenToDerive, string derivationAlgorithm, byte[] label, byte[] nonce,
            int keySize, int offset)
        {
            SymmetricSecurityKey symmetricSecurityKey = SecurityUtils.GetSecurityKey<SymmetricSecurityKey>(tokenToDerive);
            if (symmetricSecurityKey == null || !symmetricSecurityKey.IsSupportedAlgorithm(derivationAlgorithm))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new MessageSecurityException(SRP.Format(SRP.CannotFindMatchingCrypto, derivationAlgorithm)));
            }

            return symmetricSecurityKey.GenerateDerivedKey(derivationAlgorithm, label, nonce, keySize, offset);
        }

        // Originally S.SM.Security.CryptoHelper.IsEqual
        internal static bool IsEqual(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    return false;
                }
            }

            return true;
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
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(principalName));
            }

            Claim identityClaim;
            Claim primaryPrincipal;
            if (principalName.Contains("@") || principalName.Contains(@"\"))
            {
                identityClaim = new Claim(ClaimTypes.Upn, principalName, Rights.Identity);
                primaryPrincipal = Claim.CreateUpnClaim(principalName);
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

        internal static bool IsChannelBindingDisabled
        {
            get
            {
                // This used to check registry key HKEY_LOCAL_MACHINE\System\CurrentControlSet\Control\Lsa value name SuppressChannelBindingInfo
                return false;
            }
        }

        internal static bool IsSecurityBindingSuitableForChannelBinding(TransportSecurityBindingElement securityBindingElement)
        {
            if (securityBindingElement == null)
            {
                return false;
            }

            // channel binding of OperationSupportingTokenParameters, OptionalEndpointSupportingTokenParameters, or OptionalOperationSupportingTokenParameters
            // is not supported in Win7
            if (AreSecurityTokenParametersSuitableForChannelBinding(securityBindingElement.EndpointSupportingTokenParameters.Endorsing))
            {
                return true;
            }

            if (AreSecurityTokenParametersSuitableForChannelBinding(securityBindingElement.EndpointSupportingTokenParameters.Signed))
            {
                return true;
            }

            if (AreSecurityTokenParametersSuitableForChannelBinding(securityBindingElement.EndpointSupportingTokenParameters.SignedEncrypted))
            {
                return true;
            }

            if (AreSecurityTokenParametersSuitableForChannelBinding(securityBindingElement.EndpointSupportingTokenParameters.SignedEndorsing))
            {
                return true;
            }

            return false;
        }

        internal static bool AreSecurityTokenParametersSuitableForChannelBinding(Collection<SecurityTokenParameters> tokenParameters)
        {
            if (tokenParameters == null)
            {
                return false;
            }

            foreach (SecurityTokenParameters stp in tokenParameters)
            {
                SecureConversationSecurityTokenParameters scstp = stp as SecureConversationSecurityTokenParameters;
                if (scstp != null)
                {
                    return IsSecurityBindingSuitableForChannelBinding(scstp.BootstrapSecurityBindingElement as TransportSecurityBindingElement);
                }
            }

            return false;
        }

        internal static Task OpenTokenProviderIfRequiredAsync(SecurityTokenProvider tokenProvider, TimeSpan timeout)
        {
            var aco = tokenProvider as IAsyncCommunicationObject;
            if (aco != null)
            {
                return OpenCommunicationObjectAsync(aco, timeout);
            }

            if (tokenProvider is ICommunicationObject communicationObject && communicationObject != null)
            {
                return Task.Factory.FromAsync(communicationObject.BeginOpen, communicationObject.EndOpen, timeout, null, TaskCreationOptions.None);
            }

            return Task.CompletedTask;
        }

        internal static void CloseTokenProviderIfRequired(SecurityTokenProvider tokenProvider, TimeSpan timeout)
        {
            CloseCommunicationObject(tokenProvider, false, timeout);
        }

        internal static Task CloseTokenProviderIfRequiredAsync(SecurityTokenProvider tokenProvider, TimeSpan timeout)
        {
            var aco = tokenProvider as IAsyncCommunicationObject;
            if (aco != null)
            {
                return CloseCommunicationObjectAsync(aco, false, timeout);
            }

            if (tokenProvider is ICommunicationObject communicationObject && communicationObject != null)
            {
                return Task.Factory.FromAsync(communicationObject.BeginClose, communicationObject.EndClose, timeout, null, TaskCreationOptions.None);
            }

            return Task.CompletedTask;
        }

        internal static void AbortTokenProviderIfRequired(SecurityTokenProvider tokenProvider)
        {
            CloseCommunicationObject(tokenProvider, true, TimeSpan.Zero);
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

        private static Task OpenCommunicationObjectAsync(IAsyncCommunicationObject obj, TimeSpan timeout)
        {
            if (obj != null)
            {
                return obj.OpenAsync(timeout);
            }

            return Task.CompletedTask;
        }

        private static Task CloseCommunicationObjectAsync(IAsyncCommunicationObject obj, bool aborted, TimeSpan timeout)
        {
            if (obj != null)
            {
                if (aborted)
                {
                    try
                    {
                        obj.Abort();
                    }
                    catch (CommunicationException)
                    {
                    }
                }
                else
                {
                    return obj.CloseAsync(timeout);
                }
            }

            return Task.CompletedTask;
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
            {
                return CreateSecurityStandardsManager(MessageSecurityVersion.WSSecurity10WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10, tokenManager);
            }

            if (securityVersion == MessageSecurityTokenVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005)
            {
                return CreateSecurityStandardsManager(MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11, tokenManager);
            }

            if (securityVersion == MessageSecurityTokenVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005BasicSecurityProfile10)
            {
                return CreateSecurityStandardsManager(MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10, tokenManager);
            }

            if (securityVersion == MessageSecurityTokenVersion.WSSecurity10WSTrust13WSSecureConversation13BasicSecurityProfile10)
            {
                return CreateSecurityStandardsManager(MessageSecurityVersion.WSSecurity10WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10, tokenManager);
            }

            if (securityVersion == MessageSecurityTokenVersion.WSSecurity11WSTrust13WSSecureConversation13)
            {
                return CreateSecurityStandardsManager(MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12, tokenManager);
            }

            if (securityVersion == MessageSecurityTokenVersion.WSSecurity11WSTrust13WSSecureConversation13BasicSecurityProfile10)
            {
                return CreateSecurityStandardsManager(MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10, tokenManager);
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        internal static SecurityStandardsManager CreateSecurityStandardsManager(MessageSecurityVersion securityVersion, SecurityTokenSerializer securityTokenSerializer)
        {
            if (securityVersion == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(securityVersion)));
            }

            if (securityTokenSerializer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(securityTokenSerializer));
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

        public static bool TryCreateKeyFromIntrinsicKeyClause(SecurityKeyIdentifierClause keyIdentifierClause, SecurityTokenResolver resolver, out SecurityKey key)
        {
            key = null;
            if (keyIdentifierClause.CanCreateKey)
            {
                key = keyIdentifierClause.CreateKey();
                return true;
            }

            if (keyIdentifierClause is EncryptedKeyIdentifierClause)
            {
                var keyClause = (EncryptedKeyIdentifierClause)keyIdentifierClause;
                // PreSharp Bug: Parameter 'keyClause' to this public method must be validated: A null-dereference can occur here.
                for (int i = 0; i < keyClause.EncryptingKeyIdentifier.Count; i++)
                {
                    SecurityKey unwrappingSecurityKey = null;
                    if (resolver.TryResolveSecurityKey(keyClause.EncryptingKeyIdentifier[i], out unwrappingSecurityKey))
                    {
                        byte[] wrappedKey = keyClause.GetEncryptedKey();
                        string wrappingAlgorithm = keyClause.EncryptionMethod;
                        byte[] unwrappedKey = unwrappingSecurityKey.DecryptKey(wrappingAlgorithm, wrappedKey);
                        key = new InMemorySymmetricSecurityKey(unwrappedKey, false);
                        return true;
                    }
                }
            }

            return false;
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

        internal static SecurityToken CreateTokenFromEncryptedKeyClause(EncryptedKeyIdentifierClause keyClause, SecurityToken unwrappingToken)
        {
            throw new NotImplementedException();
        }

        internal static class NetworkCredentialHelper
        {
            private static string s_currentUser = string.Empty;
            private const string DefaultCurrentUser = "____CURRENTUSER_NOT_AVAILABLE____";
            internal static bool IsNullOrEmpty(NetworkCredential credential)
            {
                return credential == null ||
                        (
                            string.IsNullOrEmpty(credential.UserName) &&
                            string.IsNullOrEmpty(credential.Domain) &&
                            string.IsNullOrEmpty(credential.Password)
                        );
            }

            internal static bool IsDefault(NetworkCredential credential)
            {
                return CredentialCache.DefaultNetworkCredentials.Equals(credential);
            }

            internal static string GetCurrentUserIdAsString(NetworkCredential credential)
            {
                if (!string.IsNullOrEmpty(s_currentUser))
                {
                    return s_currentUser;
                }

                // CurrentUser could be set muliple times
                // This is fine because it does not affect the value returned.
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    using (WindowsIdentity self = WindowsIdentity.GetCurrent())
                    {
                        s_currentUser = self.User.Value;
                    }
                }
                else
                {
                    //WindowsIdentity is not supported on *NIX
                    //so returning a username which is very unlikely to be a real username;
                    s_currentUser = DefaultCurrentUser;
                }

                return s_currentUser;
            }
        }

        internal static byte[] CloneBuffer(byte[] buffer)
        {
            byte[] copy = Fx.AllocateByteArray(buffer.Length);
            Buffer.BlockCopy(buffer, 0, copy, 0, buffer.Length);
            return copy;
        }

        internal static ReadOnlyCollection<SecurityKey> CreateSymmetricSecurityKeys(byte[] key)
        {
            List<SecurityKey> temp = new List<SecurityKey>(1);
            temp.Add(new InMemorySymmetricSecurityKey(key));
            return temp.AsReadOnly();
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
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.CannotFindCert, storeName, storeLocation, findType, findValue)));
            }

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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(findValue));
            }

            X509Store store = new X509Store(storeName, storeLocation);
            X509Certificate2Collection certs = null;
            try
            {
                store.Open(OpenFlags.ReadOnly);
                certs = store.Certificates.Find(findType, findValue, false);
                if (certs.Count == 1)
                {
                    return new X509Certificate2(certs[0]);
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

        internal static Exception CreateCertificateLoadException(StoreName storeName, StoreLocation storeLocation,
            X509FindType findType, object findValue, EndpointAddress target, int certCount)
        {
            if (certCount == 0)
            {
                if (target == null)
                {
                    return new InvalidOperationException(SRP.Format(SRP.CannotFindCert, storeName, storeLocation, findType, findValue));
                }
                return new InvalidOperationException(SRP.Format(SRP.CannotFindCertForTarget, storeName, storeLocation, findType, findValue, target));
            }
            if (target == null)
            {
                return new InvalidOperationException(SRP.Format(SRP.FoundMultipleCerts, storeName, storeLocation, findType, findValue));
            }
            return new InvalidOperationException(SRP.Format(SRP.FoundMultipleCertsForTarget, storeName, storeLocation, findType, findValue, target));
        }

        public static SecurityBindingElement GetIssuerSecurityBindingElement(ServiceModelSecurityTokenRequirement requirement)
        {
            SecurityBindingElement bindingElement = requirement.SecureConversationSecurityBindingElement;
            if (bindingElement != null)
            {
                return bindingElement;
            }

            Binding binding = requirement.IssuerBinding;
            if (binding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.Format(SRP.IssuerBindingNotPresentInTokenRequirement, requirement));
            }

            BindingElementCollection bindingElements = binding.CreateBindingElements();
            return bindingElements.Find<SecurityBindingElement>();
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

        internal static void ThrowIfNegotiationFault(Message message, EndpointAddress target)
        {
            if (message.IsFault)
            {
                MessageFault fault = MessageFault.CreateFault(message, TransportDefaults.MaxSecurityFaultSize);
                Exception faultException = new FaultException(fault, message.Headers.Action);
                if (fault.Code != null && fault.Code.IsReceiverFault && fault.Code.SubCode != null)
                {
                    FaultCode subCode = fault.Code.SubCode;
                    if (subCode.Name == DotNetSecurityStrings.SecurityServerTooBusyFault && subCode.Namespace == DotNetSecurityStrings.Namespace)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ServerTooBusyException(SRP.Format(SRP.SecurityServerTooBusy, target), faultException));
                    }
                    else if (subCode.Name == AddressingStrings.EndpointUnavailable && subCode.Namespace == message.Version.Addressing.Namespace)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new EndpointNotFoundException(SRP.Format(SRP.SecurityEndpointNotFound, target), faultException));
                    }
                }

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(faultException);
            }
        }

        internal static bool IsSecurityFault(MessageFault fault, SecurityStandardsManager standardsManager)
        {
            if (fault.Code.IsSenderFault)
            {
                FaultCode subCode = fault.Code.SubCode;
                if (subCode != null)
                {
                    return (subCode.Namespace == standardsManager.SecurityVersion.HeaderNamespace.Value
                        || subCode.Namespace == standardsManager.SecureConversationDriver.Namespace.Value
                        || subCode.Namespace == standardsManager.TrustDriver.Namespace.Value
                        || subCode.Namespace == DotNetSecurityStrings.Namespace);
                }
            }

            return false;
        }

        internal static Exception CreateSecurityFaultException(MessageFault fault)
        {
            FaultException faultException = FaultException.CreateFault(fault, typeof(string), typeof(object));
            return new MessageSecurityException(SRP.UnsecuredMessageFaultReceived, faultException);
        }

        public static bool TryCreateX509CertificateFromRawData(byte[] rawData, out X509Certificate2 certificate)
        {
            certificate = (rawData == null || rawData.Length == 0) ? null : new X509Certificate2(rawData);
            return certificate != null && certificate.Handle != IntPtr.Zero;
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
                {
                    _val = _prefix + _id.ToString(CultureInfo.InvariantCulture);
                }

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
