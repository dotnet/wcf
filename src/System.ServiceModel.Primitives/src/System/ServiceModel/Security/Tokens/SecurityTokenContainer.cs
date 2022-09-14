// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Tokens;

namespace System.ServiceModel.Security.Tokens
{
    internal class SecurityTokenContainer
    {
        public SecurityTokenContainer(SecurityToken token)
        {
            Token = token ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(token));
        }

        public SecurityToken Token { get; }
    }
}
