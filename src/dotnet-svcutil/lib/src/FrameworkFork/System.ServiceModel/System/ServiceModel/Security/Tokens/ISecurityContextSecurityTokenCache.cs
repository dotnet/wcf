// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
