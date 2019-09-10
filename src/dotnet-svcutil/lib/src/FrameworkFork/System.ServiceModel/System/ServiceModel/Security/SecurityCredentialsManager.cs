// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IdentityModel.Selectors;

namespace System.ServiceModel.Security
{
    public abstract class SecurityCredentialsManager
    {
        protected SecurityCredentialsManager() { }

        public abstract SecurityTokenManager CreateSecurityTokenManager();
    }
}
