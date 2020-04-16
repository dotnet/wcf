// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.IdentityModel.Selectors;

namespace System.ServiceModel.Security.Tokens
{
    internal class SecurityTokenProviderContainer
    {
        private SecurityTokenProvider _tokenProvider;

        public SecurityTokenProviderContainer(SecurityTokenProvider tokenProvider)
        {
            if (tokenProvider == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenProvider");
            }
            _tokenProvider = tokenProvider;
        }

        public SecurityTokenProvider TokenProvider
        {
            get { return _tokenProvider; }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Close(TimeSpan timeout)
        {
            SecurityUtils.CloseTokenProviderIfRequired(_tokenProvider, timeout);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Open(TimeSpan timeout)
        {
            SecurityUtils.OpenTokenProviderIfRequired(_tokenProvider, timeout);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Abort()
        {
            SecurityUtils.AbortTokenProviderIfRequired(_tokenProvider);
        }
    }
}
