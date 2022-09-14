// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.ServiceModel;

namespace System.IdentityModel.Claims
{
    internal class ClaimComparer : IEqualityComparer<Claim>
    {
        private static IEqualityComparer<Claim> s_defaultComparer;
        private static IEqualityComparer<Claim> s_hashComparer;
        private static IEqualityComparer<Claim> s_dnsComparer;
        private static IEqualityComparer<Claim> s_rsaComparer;
        private static IEqualityComparer<Claim> s_thumbprintComparer;
        private static IEqualityComparer<Claim> s_upnComparer;
        private static IEqualityComparer<Claim> s_x500DistinguishedNameComparer;
        private IEqualityComparer _resourceComparer;

        private ClaimComparer(IEqualityComparer resourceComparer)
        {
            _resourceComparer = resourceComparer;
        }

        public static IEqualityComparer<Claim> GetComparer(string claimType)
        {
            if (claimType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(claimType));
            }

            if (claimType == ClaimTypes.Dns)
            {
                return Dns;
            }

            if (claimType == ClaimTypes.Hash)
            {
                return Hash;
            }

            if (claimType == ClaimTypes.Rsa)
            {
                return Rsa;
            }

            if (claimType == ClaimTypes.Thumbprint)
            {
                return Thumbprint;
            }

            if (claimType == ClaimTypes.Upn)
            {
                return Upn;
            }

            if (claimType == ClaimTypes.X500DistinguishedName)
            {
                return X500DistinguishedName;
            }

            return Default;
        }

        public static IEqualityComparer<Claim> Default
        {
            get
            {
                if (s_defaultComparer == null)
                {
                    s_defaultComparer = new ClaimComparer(new ObjectComparer());
                }
                return s_defaultComparer;
            }
        }

        public static IEqualityComparer<Claim> Dns
        {
            get
            {
                if (s_dnsComparer == null)
                {
                    s_dnsComparer = new ClaimComparer(StringComparer.OrdinalIgnoreCase);
                }
                return s_dnsComparer;
            }
        }

        public static IEqualityComparer<Claim> Hash
        {
            get
            {
                if (s_hashComparer == null)
                {
                    s_hashComparer = new ClaimComparer(new BinaryObjectComparer());
                }
                return s_hashComparer;
            }
        }

        public static IEqualityComparer<Claim> Rsa
        {
            get
            {
                if (s_rsaComparer == null)
                {
                    s_rsaComparer = new ClaimComparer(new RsaObjectComparer());
                }
                return s_rsaComparer;
            }
        }

        public static IEqualityComparer<Claim> Thumbprint
        {
            get
            {
                if (s_thumbprintComparer == null)
                {
                    s_thumbprintComparer = new ClaimComparer(new BinaryObjectComparer());
                }
                return s_thumbprintComparer;
            }
        }

        public static IEqualityComparer<Claim> Upn
        {
            get
            {
                if (s_upnComparer == null)
                {
                    s_upnComparer = new ClaimComparer(new UpnObjectComparer());
                }
                return s_upnComparer;
            }
        }

        public static IEqualityComparer<Claim> X500DistinguishedName
        {
            get
            {
                if (s_x500DistinguishedNameComparer == null)
                {
                    s_x500DistinguishedNameComparer = new ClaimComparer(new X500DistinguishedNameObjectComparer());
                }
                return s_x500DistinguishedNameComparer;
            }
        }

        // we still need to review how the default equals works, this is not how Doug envisioned it.
        public bool Equals(Claim claim1, Claim claim2)
        {
            if (ReferenceEquals(claim1, claim2))
            {
                return true;
            }

            if (claim1 == null || claim2 == null)
            {
                return false;
            }

            if (claim1.ClaimType != claim2.ClaimType || claim1.Right != claim2.Right)
            {
                return false;
            }

            return _resourceComparer.Equals(claim1.Resource, claim2.Resource);
        }

        public int GetHashCode(Claim claim)
        {
            if (claim == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(claim));
            }

            return claim.ClaimType.GetHashCode() ^ claim.Right.GetHashCode()
                   ^ ((claim.Resource == null) ? 0 : _resourceComparer.GetHashCode(claim.Resource));
        }

        private class ObjectComparer : IEqualityComparer
        {
            bool IEqualityComparer.Equals(object obj1, object obj2)
            {
                if (obj1 == null && obj2 == null)
                {
                    return true;
                }

                if (obj1 == null || obj2 == null)
                {
                    return false;
                }

                return obj1.Equals(obj2);
            }

            int IEqualityComparer.GetHashCode(object obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                return obj.GetHashCode();
            }
        }

        private class BinaryObjectComparer : IEqualityComparer
        {
            bool IEqualityComparer.Equals(object obj1, object obj2)
            {
                if (ReferenceEquals(obj1, obj2))
                {
                    return true;
                }

                byte[] bytes1 = obj1 as byte[];
                byte[] bytes2 = obj2 as byte[];
                if (bytes1 == null || bytes2 == null)
                {
                    return false;
                }

                if (bytes1.Length != bytes2.Length)
                {
                    return false;
                }

                for (int i = 0; i < bytes1.Length; ++i)
                {
                    if (bytes1[i] != bytes2[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            int IEqualityComparer.GetHashCode(object obj)
            {
                byte[] bytes = obj as byte[];
                if (bytes == null)
                {
                    return 0;
                }

                int hashCode = 0;
                for (int i = 0; i < bytes.Length && i < 4; ++i)
                {
                    hashCode = (hashCode << 8) | bytes[i];
                }

                return hashCode ^ bytes.Length;
            }
        }

        private class RsaObjectComparer : IEqualityComparer
        {
            bool IEqualityComparer.Equals(object obj1, object obj2)
            {
                if (ReferenceEquals(obj1, obj2))
                {
                    return true;
                }

                throw ExceptionHelper.PlatformNotSupported();
            }

            int IEqualityComparer.GetHashCode(object obj)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
        }

        private class X500DistinguishedNameObjectComparer : IEqualityComparer
        {
            private IEqualityComparer _binaryComparer;

            public X500DistinguishedNameObjectComparer()
            {
                _binaryComparer = new BinaryObjectComparer();
            }

            bool IEqualityComparer.Equals(object obj1, object obj2)
            {
                if (ReferenceEquals(obj1, obj2))
                {
                    return true;
                }

                throw ExceptionHelper.PlatformNotSupported();
            }

            int IEqualityComparer.GetHashCode(object obj)
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
        }

        private class UpnObjectComparer : IEqualityComparer
        {
            bool IEqualityComparer.Equals(object obj1, object obj2)
            {
                if (StringComparer.OrdinalIgnoreCase.Equals(obj1 as string, obj2 as string))
                {
                    return true;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    string upn1 = obj1 as string;
                    string upn2 = obj2 as string;
                    if (upn1 == null || upn2 == null)
                    {
                        return false;
                    }

                    SecurityIdentifier sid1;
                    if (!TryLookupSidFromName(upn1, out sid1))
                    {
                        return false;
                    }

                    // Normalize to sid
                    SecurityIdentifier sid2;
                    if (!TryLookupSidFromName(upn2, out sid2))
                    {
                        return false;
                    }

                    return sid1 == sid2;
                }
                else
                {
                    return false;
                }
            }

            int IEqualityComparer.GetHashCode(object obj)
            {
                string upn = obj as string;
                if (upn == null)
                {
                    return 0;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Normalize to sid
                    SecurityIdentifier sid;
                    if (TryLookupSidFromName(upn, out sid))
                    {
                        return sid.GetHashCode();
                    }
                }

                return StringComparer.OrdinalIgnoreCase.GetHashCode(upn);
            }

            [SupportedOSPlatform("windows")]
            private bool TryLookupSidFromName(string upn, out SecurityIdentifier sid)
            {
                sid = null;
                try
                {
                    NTAccount acct = new NTAccount(upn);
                    sid = acct.Translate(typeof(SecurityIdentifier)) as SecurityIdentifier;
                }
                catch (IdentityNotMappedException)
                {
                }
                return sid != null;
            }
        }
    }
}
