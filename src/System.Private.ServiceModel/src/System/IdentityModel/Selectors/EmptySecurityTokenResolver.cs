// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens;

namespace System.IdentityModel.Selectors
{
    internal static class EmptySecurityTokenResolver
    {
        public static SecurityTokenResolver Instance { get; } = SecurityTokenResolver.CreateDefaultSecurityTokenResolver(EmptyReadOnlyCollection<SecurityToken>.Instance, false);
    }
}
