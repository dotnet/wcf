// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Diagnostics;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace System.IdentityModel
{
    internal static class SecurityUtils
    {
        public const string Identities = "Identities";
        private static IIdentity s_anonymousIdentity;

        // these should be kept in sync with IIS70
        public const string AuthTypeNTLM = "NTLM";
        public const string AuthTypeNegotiate = "Negotiate";
        public const string AuthTypeKerberos = "Kerberos";
        public const string AuthTypeAnonymous = "";
        public const string AuthTypeCertMap = "SSL/PCT"; // mapped from a cert
        public const string AuthTypeBasic = "Basic"; //LogonUser

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

        internal static IIdentity CreateIdentity(string name, string authenticationType)
        {
            return new GenericIdentity(name, authenticationType);
        }

        internal static IIdentity CreateIdentity(string name)
        {
            return new GenericIdentity(name);
        }

        internal static byte[] CloneBuffer(byte[] buffer)
        {
            return CloneBuffer(buffer, 0, buffer.Length);
        }

        internal static byte[] CloneBuffer(byte[] buffer, int offset, int len)
        {
            DiagnosticUtility.DebugAssert(offset >= 0, "Negative offset passed to CloneBuffer.");
            DiagnosticUtility.DebugAssert(len >= 0, "Negative len passed to CloneBuffer.");
            DiagnosticUtility.DebugAssert(buffer.Length - offset >= len, "Invalid parameters to CloneBuffer.");

            byte[] copy = Fx.AllocateByteArray(len);
            Buffer.BlockCopy(buffer, offset, copy, 0, len);
            return copy;
        }

        internal static bool MatchesBuffer(byte[] src, byte[] dst)
        {
            return MatchesBuffer(src, 0, dst, 0);
        }

        internal static bool MatchesBuffer(byte[] src, int srcOffset, byte[] dst, int dstOffset)
        {
            DiagnosticUtility.DebugAssert(dstOffset >= 0, "Negative dstOffset passed to MatchesBuffer.");
            DiagnosticUtility.DebugAssert(srcOffset >= 0, "Negative srcOffset passed to MatchesBuffer.");

            // defensive programming
            if ((dstOffset < 0) || (srcOffset < 0))
                return false;

            if (src == null || srcOffset >= src.Length)
                return false;
            if (dst == null || dstOffset >= dst.Length)
                return false;
            if ((src.Length - srcOffset) != (dst.Length - dstOffset))
                return false;

            for (int i = srcOffset, j = dstOffset; i < src.Length; i++, j++)
            {
                if (src[i] != dst[j])
                    return false;
            }
            return true;
        }

        internal static ReadOnlyCollection<IAuthorizationPolicy> CreateAuthorizationPolicies(ClaimSet claimSet)
        {
            return CreateAuthorizationPolicies(claimSet, SecurityUtils.MaxUtcDateTime);
        }

        internal static ReadOnlyCollection<IAuthorizationPolicy> CreateAuthorizationPolicies(ClaimSet claimSet, DateTime expirationTime)
        {
            List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>(1);
            policies.Add(new UnconditionalPolicy(claimSet, expirationTime));
            return policies.AsReadOnly();
        }

        internal static AuthorizationContext CreateDefaultAuthorizationContext(IList<IAuthorizationPolicy> authorizationPolicies)
        {
            AuthorizationContext _authorizationContext;
            // This is faster than Policy evaluation.
            if (authorizationPolicies != null && authorizationPolicies.Count == 1 && authorizationPolicies[0] is UnconditionalPolicy)
            {
                _authorizationContext = new SimpleAuthorizationContext(authorizationPolicies);
            }
            // degenerate case
            else if (authorizationPolicies == null || authorizationPolicies.Count <= 0)
            {
                return DefaultAuthorizationContext.Empty;
            }
            else
            {
                // there are some policies, run them until they are all done
                DefaultEvaluationContext evaluationContext = new DefaultEvaluationContext();
                object[] policyState = new object[authorizationPolicies.Count];
                object done = new object();

                int oldContextCount;
                do
                {
                    oldContextCount = evaluationContext.Generation;

                    for (int i = 0; i < authorizationPolicies.Count; i++)
                    {
                        if (policyState[i] == done)
                            continue;

                        IAuthorizationPolicy policy = authorizationPolicies[i];
                        if (policy == null)
                        {
                            policyState[i] = done;
                            continue;
                        }

                        if (policy.Evaluate(evaluationContext, ref policyState[i]))
                        {
                            policyState[i] = done;
                        }
                    }
                } while (oldContextCount < evaluationContext.Generation);

                _authorizationContext = new DefaultAuthorizationContext(evaluationContext);
            }

            return _authorizationContext;
        }


        internal static string ClaimSetToString(ClaimSet claimSet)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ClaimSet [");
            for (int i = 0; i < claimSet.Count; i++)
            {
                Claim claim = claimSet[i];
                if (claim != null)
                {
                    sb.Append("  ");
                    sb.AppendLine(claim.ToString());
                }
            }
            string prefix = "] by ";
            ClaimSet issuer = claimSet;
            do
            {
                issuer = issuer.Issuer;
                sb.AppendFormat("{0}{1}", prefix, issuer == claimSet ? "Self" : (issuer.Count <= 0 ? "Unknown" : issuer[0].ToString()));
                prefix = " -> ";
            } while (issuer.Issuer != issuer);
            return sb.ToString();
        }

        internal static IIdentity CloneIdentityIfNecessary(IIdentity identity)
        {
            if (identity != null)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
            return identity;
        }

#if SUPPORTS_WINDOWSIDENTITY // NegotiateStream
        internal static WindowsIdentity CloneWindowsIdentityIfNecessary(WindowsIdentity wid)
        {
            return CloneWindowsIdentityIfNecessary(wid, wid.AuthenticationType);
        }

        internal static WindowsIdentity CloneWindowsIdentityIfNecessary(WindowsIdentity wid, string authenticationType)
        {

            if (wid != null)
            {
                IntPtr token = wid.AccessToken.DangerousGetHandle();
                if (token != null)
                {
                    return UnsafeCreateWindowsIdentityFromToken(token, authenticationType);
                }
            }
            return wid;
        }


        static IntPtr UnsafeGetWindowsIdentityToken(WindowsIdentity wid)
        {
            return wid.AccessToken.DangerousGetHandle();
        }

        static WindowsIdentity UnsafeCreateWindowsIdentityFromToken(IntPtr token, string authenticationType)
        {
            if (authenticationType != null)
            {
                return new WindowsIdentity(token, authenticationType);
            }
            else
            {
                return new WindowsIdentity(token);
            }
        }
#endif // SUPPORTS_WINDOWSIDENTITY 

        internal static ClaimSet CloneClaimSetIfNecessary(ClaimSet claimSet)
        {
            if (claimSet != null)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
            return claimSet;
        }

        internal static ReadOnlyCollection<ClaimSet> CloneClaimSetsIfNecessary(ReadOnlyCollection<ClaimSet> claimSets)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static void DisposeClaimSetIfNecessary(ClaimSet claimSet)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static void DisposeClaimSetsIfNecessary(ReadOnlyCollection<ClaimSet> claimSets)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static ReadOnlyCollection<SecurityKey> CreateSymmetricSecurityKeys(byte[] key)
        {
            List<SecurityKey> temp = new List<SecurityKey>(1);
            temp.Add(new InMemorySymmetricSecurityKey(key));
            return temp.AsReadOnly();
        }


        internal static string GetCertificateId(X509Certificate2 certificate)
        {
            StringBuilder str = new StringBuilder(256);
            AppendCertificateIdentityName(str, certificate);
            return str.ToString();
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

        internal static ReadOnlyCollection<IAuthorizationPolicy> CloneAuthorizationPoliciesIfNecessary(ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies)
        {
            if (authorizationPolicies != null && authorizationPolicies.Count > 0)
            {
                bool clone = false;
                for (int i = 0; i < authorizationPolicies.Count; ++i)
                {
                    UnconditionalPolicy policy = authorizationPolicies[i] as UnconditionalPolicy;
                    if (policy != null && policy.IsDisposable)
                    {
                        clone = true;
                        break;
                    }
                }
                if (clone)
                {
                    List<IAuthorizationPolicy> ret = new List<IAuthorizationPolicy>(authorizationPolicies.Count);
                    for (int i = 0; i < authorizationPolicies.Count; ++i)
                    {
                        UnconditionalPolicy policy = authorizationPolicies[i] as UnconditionalPolicy;
                        if (policy != null)
                        {
                            ret.Add(policy.Clone());
                        }
                        else
                        {
                            ret.Add(authorizationPolicies[i]);
                        }
                    }
                    return new ReadOnlyCollection<IAuthorizationPolicy>(ret);
                }
            }
            return authorizationPolicies;
        }

        public static void DisposeAuthorizationPoliciesIfNecessary(ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies)
        {
            if (authorizationPolicies != null && authorizationPolicies.Count > 0)
            {
                for (int i = 0; i < authorizationPolicies.Count; ++i)
                {
                    DisposeIfNecessary(authorizationPolicies[i] as UnconditionalPolicy);
                }
            }
        }

        public static void DisposeIfNecessary(IDisposable obj)
        {
            if (obj != null)
            {
                obj.Dispose();
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
    }

    internal static class EmptyReadOnlyCollection<T>
    {
        public static ReadOnlyCollection<T> Instance = new ReadOnlyCollection<T>(new List<T>());
    }

    internal class SimpleAuthorizationContext : AuthorizationContext
    {
        private SecurityUniqueId _id;
        private UnconditionalPolicy _policy;
        private IDictionary<string, object> _properties;

        public SimpleAuthorizationContext(IList<IAuthorizationPolicy> authorizationPolicies)
        {
            _policy = (UnconditionalPolicy)authorizationPolicies[0];
            Dictionary<string, object> properties = new Dictionary<string, object>();
            if (_policy.PrimaryIdentity != null && _policy.PrimaryIdentity != SecurityUtils.AnonymousIdentity)
            {
                List<IIdentity> identities = new List<IIdentity>();
                identities.Add(_policy.PrimaryIdentity);
                properties.Add(SecurityUtils.Identities, identities);
            }

            _properties = properties;
        }

        public override string Id
        {
            get
            {
                if (_id == null)
                    _id = SecurityUniqueId.Create();
                return _id.Value;
            }
        }
        public override ReadOnlyCollection<ClaimSet> ClaimSets { get { return _policy.Issuances; } }
        public override DateTime ExpirationTime { get { return _policy.ExpirationTime; } }
        public override IDictionary<string, object> Properties { get { return _properties; } }
    }
}
