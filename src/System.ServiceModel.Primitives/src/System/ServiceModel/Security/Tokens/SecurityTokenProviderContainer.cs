// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.CompilerServices;
using System.IdentityModel.Selectors;
using System.Threading.Tasks;

namespace System.ServiceModel.Security.Tokens
{
    internal class SecurityTokenProviderContainer
    {
        public SecurityTokenProviderContainer(SecurityTokenProvider tokenProvider)
        {
            TokenProvider = tokenProvider ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenProvider));
        }

        public SecurityTokenProvider TokenProvider { get; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Close(TimeSpan timeout)
        {
            SecurityUtils.CloseTokenProviderIfRequired(TokenProvider, timeout);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Task OpenAsync(TimeSpan timeout)
        {
            return SecurityUtils.OpenTokenProviderIfRequiredAsync(TokenProvider, timeout);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Abort()
        {
            SecurityUtils.AbortTokenProviderIfRequired(TokenProvider);
        }
    }
}
