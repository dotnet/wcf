// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Cryptography;
using System.ServiceModel;

namespace System.IdentityModel
{
    internal static class CryptoHelper
    {
        private static RandomNumberGenerator s_random;
        private const string SHAString = "SHA";
        private const string SHA1String = "SHA1";
        private const string SHA256String = "SHA256";
        private const string SystemSecurityCryptographySha1String = "System.Security.Cryptography.SHA1";

        private static Dictionary<string, Func<object>> s_algorithmDelegateDictionary = new Dictionary<string, Func<object>>();
        private static object s_algorithmDictionaryLock = new object();

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
            object algorithmObject = GetAlgorithmFromConfig(algorithm);

            if (algorithmObject != null)
            {
                KeyedHashAlgorithm keyedHashAlgorithm = algorithmObject as KeyedHashAlgorithm;
                if (keyedHashAlgorithm != null)
                {
                    keyedHashAlgorithm.Key = key;
                    return keyedHashAlgorithm;
                }

                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SRP.Format(SRP.CustomCryptoAlgorithmIsNotValidKeyedHashAlgorithm, algorithm)));
            }

            switch (algorithm)
            {
                case SecurityAlgorithms.HmacSha1Signature:
#pragma warning disable CA5350 // Do not use insecure cryptographic algorithm SHA1.
                    return new HMACSHA1(key);// CodeQL [SM02200] Insecure cryptographic algorithm HMACSHA1 is needed here as a requirement of SOAP protocols
#pragma warning restore CA5350 // Do not use insecure cryptographic algorithm SHA1.
                case SecurityAlgorithms.HmacSha256Signature:
                    return new HMACSHA256(key);
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SRP.Format(SRP.UnsupportedCryptoAlgorithm, algorithm)));
            }
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
            bool found = false;
            object algorithmObject = null;

            try
            {
                algorithmObject = GetAlgorithmFromConfig(algorithm);
            }
            catch (InvalidOperationException)
            {
                // We swallow the exception and continue.
            }
            if (algorithmObject != null)
            {
                SymmetricAlgorithm symmetricAlgorithm = algorithmObject as SymmetricAlgorithm;
                KeyedHashAlgorithm keyedHashAlgorithm = algorithmObject as KeyedHashAlgorithm;

                if (symmetricAlgorithm != null || keyedHashAlgorithm != null)
                {
                    found = true;
                }
                // The reason we do not return here even when the user has provided a custom algorithm to CryptoConfig 
                // is because we need to check if the user has overwritten an existing standard URI.
            }

            switch (algorithm)
            {
                case SecurityAlgorithms.DsaSha1Signature:
                case SecurityAlgorithms.RsaSha1Signature:
                case SecurityAlgorithms.RsaSha256Signature:
                case SecurityAlgorithms.RsaOaepKeyWrap:
                case SecurityAlgorithms.RsaV15KeyWrap:
                    return false;
                case SecurityAlgorithms.HmacSha1Signature:
                case SecurityAlgorithms.HmacSha256Signature:
                case SecurityAlgorithms.Psha1KeyDerivation:
                case SecurityAlgorithms.Psha1KeyDerivationDec2005:
                    return true;
                case SecurityAlgorithms.Aes128Encryption:
                case SecurityAlgorithms.Aes128KeyWrap:
                    return keySize >= 128 && keySize <= 256;
                case SecurityAlgorithms.Aes192Encryption:
                case SecurityAlgorithms.Aes192KeyWrap:
                    return keySize >= 192 && keySize <= 256;
                case SecurityAlgorithms.Aes256Encryption:
                case SecurityAlgorithms.Aes256KeyWrap:
                    return keySize == 256;
                case SecurityAlgorithms.TripleDesEncryption:
                case SecurityAlgorithms.TripleDesKeyWrap:
                    return keySize == 128 || keySize == 192;
                default:
                    if (found)
                    {
                        return true;
                    }

                    return false;
                    // We do not expect the user to map the uri of an existing standrad algorithm with say key size 128 bit 
                    // to a custom algorithm with keySize 192 bits. If he does that, we anyways make sure that we return false.
            }
        }

        internal static void FillRandomBytes(byte[] buffer)
        {
            RandomNumberGenerator.GetBytes(buffer);
        }

        internal static RandomNumberGenerator RandomNumberGenerator
        {
            get
            {
                if (s_random == null)
                {
                    s_random = RandomNumberGenerator.Create();
                }
                return s_random;
            }
        }

        internal static HashAlgorithm CreateHashAlgorithm(string algorithm)
        {
            object algorithmObject = GetAlgorithmFromConfig(algorithm);

            if (algorithmObject != null)
            {
                HashAlgorithm hashAlgorithm = algorithmObject as HashAlgorithm;
                if (hashAlgorithm != null)
                {
                    return hashAlgorithm;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SRP.Format(SRP.CustomCryptoAlgorithmIsNotValidHashAlgorithm, algorithm)));
            }

            switch (algorithm)
            {
                case SHAString:
                case SHA1String:
                case SystemSecurityCryptographySha1String:
                case SecurityAlgorithms.Sha1Digest:
#pragma warning disable CA5350 // Do not use insecure cryptographic algorithm SHA1.
                    return SHA1.Create();// CodeQL [SM02196] Insecure cryptographic algorithm SHA1 is needed here as a requirement of SOAP protocols
#pragma warning restore CA5350 // Do not use insecure cryptographic algorithm SHA1.
                case SHA256String:
                case SecurityAlgorithms.Sha256Digest:
                    return SHA256.Create();
                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SRP.Format(SRP.UnsupportedCryptoAlgorithm, algorithm)));
            }
        }

        private static object GetDefaultAlgorithm(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(algorithm)));
            }

            switch (algorithm)
            {
                //case SecurityAlgorithms.RsaSha1Signature:
                //case SecurityAlgorithms.DsaSha1Signature:
                // For these algorithms above, crypto config returns internal objects.
                // As we cannot create those internal objects, we are returning null.
                // If no custom algorithm is plugged-in, at least these two algorithms
                // will be inside the delegate dictionary.
                case SecurityAlgorithms.Sha1Digest:
#pragma warning disable CA5350 // Do not use insecure cryptographic algorithm SHA1.
                    return SHA1.Create();// CodeQL [SM02196] Insecure cryptographic algorithm SHA1 is needed here as a requirement of SOAP protocols
#pragma warning restore CA5350 // Do not use insecure cryptographic algorithm SHA1.
                case SecurityAlgorithms.ExclusiveC14n:
                    throw ExceptionHelper.PlatformNotSupported();
                case SHA256String:
                case SecurityAlgorithms.Sha256Digest:
                    return SHA256.Create();
                case SecurityAlgorithms.Sha512Digest:
                    return SHA512.Create();
                case SecurityAlgorithms.Aes128Encryption:
                case SecurityAlgorithms.Aes192Encryption:
                case SecurityAlgorithms.Aes256Encryption:
                case SecurityAlgorithms.Aes128KeyWrap:
                case SecurityAlgorithms.Aes192KeyWrap:
                case SecurityAlgorithms.Aes256KeyWrap:
                    return Aes.Create();
                case SecurityAlgorithms.TripleDesEncryption:
                case SecurityAlgorithms.TripleDesKeyWrap:
#pragma warning disable CA5350 // Do Not Use Weak Cryptographic Algorithms
                    return TripleDES.Create();// CodeQL [SM02192] Weak cryptographic algorithm TripleDES is needed here as a requirement of SOAP protocols
#pragma warning restore CA5350 // Do Not Use Weak Cryptographic Algorithms
                case SecurityAlgorithms.HmacSha1Signature:
#pragma warning disable CA5350 // Do not use insecure cryptographic algorithm SHA1.
                    return new HMACSHA1();// CodeQL [SM02200] Insecure cryptographic algorithm HMACSHA1 is needed here as a requirement of SOAP protocols
#pragma warning restore CA5350 // Do not use insecure cryptographic algorithm SHA1.
                case SecurityAlgorithms.HmacSha256Signature:
                    return new HMACSHA256();
                case SecurityAlgorithms.ExclusiveC14nWithComments:
                    throw ExceptionHelper.PlatformNotSupported();
                case SecurityAlgorithms.Ripemd160Digest:
                    return null;
                case SecurityAlgorithms.DesEncryption:
#pragma warning disable CA5351 // Do Not Use Broken Cryptographic Algorithms
                    return DES.Create();// CodeQL [SM02192] Broken cryptographic algorithm DES is needed here as a requirement of SOAP protocols
#pragma warning restore CA5351 // Do Not Use Broken Cryptographic Algorithms
                default:
                    return null;
            }
        }


        internal static object GetAlgorithmFromConfig(string algorithm)
        {
            if (string.IsNullOrEmpty(algorithm))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(algorithm)));
            }

            object algorithmObject = null;
            object defaultObject = null;
            Func<object> delegateFunction = null;

            if (!s_algorithmDelegateDictionary.TryGetValue(algorithm, out delegateFunction))
            {
                lock (s_algorithmDictionaryLock)
                {
                    if (!s_algorithmDelegateDictionary.ContainsKey(algorithm))
                    {
                        try
                        {
                            algorithmObject = CryptoConfig.CreateFromName(algorithm);
                        }
                        catch (TargetInvocationException)
                        {
                            s_algorithmDelegateDictionary[algorithm] = null;
                        }

                        if (algorithmObject == null)
                        {
                            s_algorithmDelegateDictionary[algorithm] = null;
                        }
                        else
                        {
                            defaultObject = GetDefaultAlgorithm(algorithm);
                            if (defaultObject != null && defaultObject.GetType() == algorithmObject.GetType())
                            {
                                s_algorithmDelegateDictionary[algorithm] = null;
                            }
                            else
                            {
                                // Create a factory delegate which returns new instances of the algorithm type for later calls.
                                Type algorithmType = algorithmObject.GetType();
                                Linq.Expressions.NewExpression algorithmCreationExpression = Linq.Expressions.Expression.New(algorithmType);
                                Linq.Expressions.LambdaExpression creationFunction = Linq.Expressions.Expression.Lambda<Func<object>>(algorithmCreationExpression);
                                delegateFunction = creationFunction.Compile() as Func<object>;

                                if (delegateFunction != null)
                                {
                                    s_algorithmDelegateDictionary[algorithm] = delegateFunction;
                                }
                                return algorithmObject;
                            }
                        }
                    }
                }
            }
            else
            {
                if (delegateFunction != null)
                {
                    return delegateFunction.Invoke();
                }
            }

            //
            // This is a fallback in case CryptoConfig fails to return a valid
            // algorithm object. CrytoConfig does not understand all the uri's and
            // can return a null in that case, in which case it is our responsibility
            // to fallback and create the right algorithm if it is a uri we understand
            //
            switch (algorithm)
            {
                case SHA256String:
                case SecurityAlgorithms.Sha256Digest:
                    return SHA256.Create();
                case SecurityAlgorithms.Sha1Digest:
#pragma warning disable CA5350 // Do not use insecure cryptographic algorithm SHA1.
                    return SHA1.Create();// CodeQL [SM02196] Insecure cryptographic algorithm SHA1 is needed here as a requirement of SOAP protocols
#pragma warning restore CA5350 // Do not use insecure cryptographic algorithm SHA1.
                case SecurityAlgorithms.HmacSha1Signature:
#pragma warning disable CA5350 // Do not use insecure cryptographic algorithm SHA1.
                    return new HMACSHA1();// CodeQL [SM02200] Insecure cryptographic algorithm HMACSHA1 is needed here as a requirement of SOAP protocols
#pragma warning restore CA5350 // Do not use insecure cryptographic algorithm SHA1.
                default:
                    break;
            }

            return null;
        }

        internal static HashAlgorithm NewSha1HashAlgorithm()
        {
            return CreateHashAlgorithm(SecurityAlgorithms.Sha1Digest);
        }

        internal static HashAlgorithm NewSha256HashAlgorithm()
        {
            return CreateHashAlgorithm(SecurityAlgorithms.Sha256Digest);
        }

        internal static KeyedHashAlgorithm NewHmacSha1KeyedHashAlgorithm(byte[] key)
        {
            return CryptoHelper.CreateKeyedHashAlgorithm(key, SecurityAlgorithms.HmacSha1Signature);
        }
    }
}

