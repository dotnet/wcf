// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.IdentityModel.Selectors;

namespace System.ServiceModel.Security
{
    public abstract class SecurityCredentialsManager
    {
        protected SecurityCredentialsManager() { }

        public abstract SecurityTokenManager CreateSecurityTokenManager();
    }
}
