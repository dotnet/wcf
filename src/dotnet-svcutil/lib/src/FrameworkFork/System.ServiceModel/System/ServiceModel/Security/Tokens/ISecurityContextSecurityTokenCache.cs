// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Xml;
using System.Collections.ObjectModel;

namespace System.ServiceModel.Security.Tokens
{
    public interface ISecurityContextSecurityTokenCache
    {
        void AddContext(SecurityContextSecurityToken token);
        bool TryAddContext(SecurityContextSecurityToken token);
        void ClearContexts();
        void RemoveContext(UniqueId contextId, UniqueId generation);
        void RemoveAllContexts(UniqueId contextId);
        SecurityContextSecurityToken GetContext(UniqueId contextId, UniqueId generation);
        Collection<SecurityContextSecurityToken> GetAllContexts(UniqueId contextId);
        void UpdateContextCachingTime(SecurityContextSecurityToken context, DateTime expirationTime);
    }
}
