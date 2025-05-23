// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Infrastructure.Common
{
    // This class facilitates using a private keychain on OSX
    public class SafeKeychainHandle : SafeHandle
    {
        private const string CoreFoundation = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";
        private const string SecurityFramework = "/System/Library/Frameworks/Security.framework/Security";

        [DllImport(SecurityFramework)]
        private static extern int SecKeychainUnlock(SafeKeychainHandle handle, int passphraseLength, byte[] passphraseUtf8, bool usePassword);

        [DllImport(SecurityFramework)]
        private static extern int SecKeychainOpen(string path, out SafeKeychainHandle keychain);

        [DllImport(SecurityFramework)]
        private static extern int SecKeychainCreate(string pathName, int passphraseLength, byte[] passphraseUtf8, bool promptUser, IntPtr initialAccessNull, out SafeKeychainHandle keychain);

        [DllImport(SecurityFramework)]
        private static extern int SecKeychainDelete(SafeKeychainHandle handle);

        [DllImport(CoreFoundation)]
        private static extern void CFRelease(IntPtr ptr);

        internal SafeKeychainHandle()
            : base(IntPtr.Zero, ownsHandle: true)
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        protected override bool ReleaseHandle()
        {
            CFRelease(handle);
            SetHandle(IntPtr.Zero);
            return true;
        }

        public override bool IsInvalid
        {
            get
            {
                return handle == IntPtr.Zero;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static SafeKeychainHandle Create(string pathName, string passphrase)
        {
            byte[] utf8Passphrase = Encoding.UTF8.GetBytes(passphrase);
            SafeKeychainHandle keychain;
            int osStatus = SecKeychainCreate(pathName, utf8Passphrase.Length, utf8Passphrase, false, IntPtr.Zero, out keychain);

            if (osStatus != 0)
            {
                keychain.Dispose();
                throw new InvalidOperationException(string.Format("OSStatus={0}", osStatus));
            }

            return keychain;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static SafeKeychainHandle Open(string pathName, string passphrase)
        {
            SafeKeychainHandle keychain;
            int osStatus = SecKeychainOpen(pathName, out keychain);

            if (osStatus != 0)
            {
                keychain.Dispose();
                throw new InvalidOperationException(string.Format("OSStatus={0}", osStatus));
            }

            if(!string.IsNullOrEmpty(passphrase))
            {
                byte[] utf8Passphrase = Encoding.UTF8.GetBytes(passphrase);
                osStatus = SecKeychainUnlock(keychain, utf8Passphrase.Length, utf8Passphrase, true);

                if (osStatus != 0)
                {
                    keychain.Dispose();
                    throw new InvalidOperationException(string.Format("OSStatus={0}", osStatus));
                }
            }

            return keychain;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Delete(SafeKeychainHandle handle)
        {
            if (handle.IsInvalid)
                return;

            int osStatus = SecKeychainDelete(handle);
            handle.Dispose();

            if (osStatus != 0)
            {
                throw new InvalidOperationException(string.Format("OSStatus={0}", osStatus));
            }
        }
    }
}
