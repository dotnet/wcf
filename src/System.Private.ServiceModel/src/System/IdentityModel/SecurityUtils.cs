// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.Runtime;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;

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
                    s_anonymousIdentity = SecurityUtils.CreateIdentity(String.Empty);
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
    }

    internal static class EmptyReadOnlyCollection<T>
    {
        public static ReadOnlyCollection<T> Instance = new ReadOnlyCollection<T>(new List<T>());
    }
}
