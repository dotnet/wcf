// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.IdentityModel.Selectors
{
    using System.IdentityModel.Tokens;

    internal static class EmptySecurityTokenResolver
    {
        public static SecurityTokenResolver Instance { get; } = SecurityTokenResolver.CreateDefaultSecurityTokenResolver(EmptyReadOnlyCollection<SecurityToken>.Instance, false);
    }
}
