// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Security.Cryptography;
using System.ServiceModel;

namespace System.IdentityModel
{
    internal static class CryptoHelper
    {
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

