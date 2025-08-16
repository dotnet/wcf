// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF.Description;
using CoreWCF.IdentityModel.Selectors;

namespace WcfService
{
    public class MultiCredentialServiceCredentials : ServiceCredentials
    {
        private readonly Dictionary<string, ServiceCredentials> _serviceCredentialsMap = new();

        public void AddServiceCredentials(string path, ServiceCredentials credentials)
        {
            _serviceCredentialsMap[path] = credentials;
        }
        
        public IReadOnlyDictionary<string, ServiceCredentials> ServiceCredentialsMap => _serviceCredentialsMap;

        public override SecurityTokenManager CreateSecurityTokenManager()
        {
            return new MultiCredentialSecurityTokenManager(this, _serviceCredentialsMap);
        }

        internal SecurityTokenManager CreateOriginalSecurityTokenManager()
        {
            return base.CreateSecurityTokenManager();
        }

        protected override ServiceCredentials CloneCore()
        {
            var clone = new MultiCredentialServiceCredentials();
            foreach (var kvp in _serviceCredentialsMap)
            {
                clone.AddServiceCredentials(kvp.Key, kvp.Value.Clone());
            }
            return clone;
        }
    }
}
#endif
