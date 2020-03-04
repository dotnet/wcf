// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IO;
using System.Runtime;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using Microsoft.Xml;

namespace System.ServiceModel.Security.Tokens
{
    internal struct SecurityContextCookieSerializer
    {
        private const int SupportedPersistanceVersion = 1;

        private SecurityStateEncoder _securityStateEncoder;
        private IList<Type> _knownTypes;

        public SecurityContextCookieSerializer(SecurityStateEncoder securityStateEncoder, IList<Type> knownTypes)
        {
            if (securityStateEncoder == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("securityStateEncoder");
            }
            _securityStateEncoder = securityStateEncoder;
            _knownTypes = knownTypes ?? new List<Type>();
        }

        public byte[] CreateCookieFromSecurityContext(UniqueId contextId, string id, byte[] key, DateTime tokenEffectiveTime,
            DateTime tokenExpirationTime, UniqueId keyGeneration, DateTime keyEffectiveTime, DateTime keyExpirationTime,
            ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
    }
}
