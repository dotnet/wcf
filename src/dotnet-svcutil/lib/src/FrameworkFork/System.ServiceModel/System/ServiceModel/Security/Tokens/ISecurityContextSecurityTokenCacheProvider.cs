// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Security.Tokens
{
    internal interface ISecurityContextSecurityTokenCacheProvider
    {
        ISecurityContextSecurityTokenCache TokenCache { get; }
    }
}
