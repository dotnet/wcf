// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
 