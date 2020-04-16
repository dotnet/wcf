// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Security;

namespace Microsoft.Xml
{
    // SecureStringHasher is a hash code provider for strings. The hash codes calculation starts with a seed (hasCodeRandomizer) which is usually
    // different for each instance of SecureStringHasher. Since the hash code depend on the seed, the chance of hashtable DoS attack in case when 
    // someone passes in lots of strings that hash to the same hash code is greatly reduced.
    // The SecureStringHasher implements IEqualityComparer for strings and therefore can be used in generic IDictionary.
    internal class SecureStringHasher : IEqualityComparer<String>
    {
        [SecurityCritical]
        private delegate int HashCodeOfStringDelegate3(string s, int sLen, long additionalEntropy);

        [SecurityCritical]
        private delegate int HashCodeOfStringDelegate1(string s);

        // Value is guaranteed to be null by the spec.
        // No explicit assignment because it will require adding SecurityCritical on .cctor
        // which could hurt the performance
        [SecurityCritical]
        private static HashCodeOfStringDelegate3 s_hashCodeDelegate3;

        [SecurityCritical]
        private static HashCodeOfStringDelegate1 s_hashCodeDelegate1;

        private int _hashCodeRandomizer;

        public SecureStringHasher()
        {
            _hashCodeRandomizer = Environment.TickCount;
        }

#if false // This is here only for debugging of hashing issues
        public SecureStringHasher( int hashCodeRandomizer ) {
            this.hashCodeRandomizer = hashCodeRandomizer;
        }
#endif
        internal static SecureStringHasher Instance { get; } = new SecureStringHasher();

        public bool Equals(String x, String y)
        {
            return String.Equals(x, y, StringComparison.Ordinal);
        }

        [SecuritySafeCritical]
        public int GetHashCode(String key)
        {
            if (s_hashCodeDelegate1 == null && s_hashCodeDelegate3 == null)
            {
                GetHashCodeDelegate();
            }

            if (s_hashCodeDelegate1 != null)
            {
                return s_hashCodeDelegate1(key);
            }

            return s_hashCodeDelegate3(key, key.Length, _hashCodeRandomizer);
        }

        [SecurityCritical]
        private static int GetHashCodeOfString(string key, int sLen, long additionalEntropy)
        {
            int hashCode = unchecked((int)additionalEntropy);
            // use key.Length to eliminate the rangecheck
            for (int i = 0; i < key.Length; i++)
            {
                hashCode += (hashCode << 7) ^ key[i];
            }
            // mix it a bit more
            hashCode -= hashCode >> 17;
            hashCode -= hashCode >> 11;
            hashCode -= hashCode >> 5;
            return hashCode;
        }

        [SecuritySafeCritical]
        private void GetHashCodeDelegate()
        {
            // If we find the Marvin hash method, we use that
            // Otherwise, we use the old string hashing function.
            // The Marvin hash method signature changed in netcore 2.1-pre to have a single argument and apparently was removed in 2.1 RTM.
            // Note: calling Type.GetMethod with the exact param types does not always work, need to get the method info and check the parameters.

            MethodInfo getHashCodeMethodInfo = typeof(String).GetMethod("InternalMarvin32HashString", BindingFlags.NonPublic | BindingFlags.Static);
            if (getHashCodeMethodInfo != null)
            {
                ParameterInfo[] paramTypes = getHashCodeMethodInfo.GetParameters();
                if (paramTypes.Length == 1 && paramTypes[0].ParameterType == typeof(string))
                {
                    s_hashCodeDelegate1 = (HashCodeOfStringDelegate1)getHashCodeMethodInfo.CreateDelegate(typeof(HashCodeOfStringDelegate1));
                }
                else if (paramTypes.Length == 3 && paramTypes[0].ParameterType == typeof(string) && paramTypes[1].ParameterType == typeof(int) && paramTypes[2].ParameterType == typeof(long))
                {
                    s_hashCodeDelegate3 = (HashCodeOfStringDelegate3)getHashCodeMethodInfo.CreateDelegate(typeof(HashCodeOfStringDelegate3));
                }
            }

            if (s_hashCodeDelegate1 == null && s_hashCodeDelegate3 == null)
            {
                // This will fall through and return a delegate to the old hash function
                s_hashCodeDelegate3 = new HashCodeOfStringDelegate3(GetHashCodeOfString);
            }
        }
    }
}
