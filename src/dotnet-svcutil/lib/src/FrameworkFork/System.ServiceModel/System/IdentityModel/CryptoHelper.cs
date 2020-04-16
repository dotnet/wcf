// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Security;

namespace System.IdentityModel
{
    internal static class CryptoHelper
    {
        private static Dictionary<string, Func<object>> s_algorithmDelegateDictionary = new Dictionary<string, Func<object>>();
        private static object s_AlgorithmDictionaryLock = new object();
        public const int WindowsVistaMajorNumber = 6;
        private const string SHAString = "SHA";
        private const string SHA1String = "SHA1";
        private const string SHA256String = "SHA256";
        private const string SystemSecurityCryptographySha1String = "System.Security.Cryptography.SHA1";

        internal static bool IsSymmetricAlgorithm(string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static byte[] UnwrapKey(byte[] wrappingKey, byte[] wrappedKey, string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static byte[] WrapKey(byte[] wrappingKey, byte[] keyToBeWrapped, string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static byte[] GenerateDerivedKey(byte[] key, string algorithm, byte[] label, byte[] nonce, int derivedKeySize, int position)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static int GetIVSize(string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

#pragma warning disable 0436 // ICryptoTransform, KeyedHashAlgorithm, SymmetricAlgorithm conflict with imported types 
        internal static ICryptoTransform CreateDecryptor(byte[] key, byte[] iv, string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static ICryptoTransform CreateEncryptor(byte[] key, byte[] iv, string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static KeyedHashAlgorithm CreateKeyedHashAlgorithm(byte[] key, string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static SymmetricAlgorithm GetSymmetricAlgorithm(byte[] key, string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
#pragma warning restore 0436 

        internal static bool IsAsymmetricAlgorithm(string algorithm)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static bool IsSymmetricSupportedAlgorithm(string algorithm, int keySize)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
    }
}

