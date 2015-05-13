// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

namespace System.IdentityModel.Selectors
{
    public abstract class SecurityTokenVersion
    {
        public abstract ReadOnlyCollection<string> GetSecuritySpecifications();
    }
}
