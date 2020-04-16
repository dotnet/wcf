// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if !SUPPORTS_WINDOWSIDENTITY //NegotiateStream

using System.Collections.ObjectModel;
using System.Security.Principal;
using System.ServiceModel;

namespace System.IdentityModel.Tokens
{
    public class WindowsSecurityToken : SecurityToken, IDisposable
    {
        public override string Id
        {
            get
            {
                throw ExceptionHelper.PlatformNotSupported("Windows Stream Security is not supported on UWP yet");
            }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                throw ExceptionHelper.PlatformNotSupported("Windows Stream Security is not supported on UWP yet");
            }
        }

        public override DateTime ValidFrom
        {
            get
            {
                throw ExceptionHelper.PlatformNotSupported("Windows Stream Security is not supported on UWP yet");
            }
        }

        public override DateTime ValidTo
        {
            get
            {
                throw ExceptionHelper.PlatformNotSupported("Windows Stream Security is not supported on UWP yet");
            }
        }

        public void Dispose()
        {
            throw ExceptionHelper.PlatformNotSupported("Windows Stream Security is not supported on UWP yet");
        }
    }
}
#endif // !SUPPORTS_WINDOWSIDENTITY

