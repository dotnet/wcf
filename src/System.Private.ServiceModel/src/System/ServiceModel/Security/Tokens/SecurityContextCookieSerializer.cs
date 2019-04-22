// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.Xml;
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel.Security.Tokens
{
    internal struct SecurityContextCookieSerializer
    {
        private const int SupportedPersistanceVersion = 1;

        private SecurityStateEncoder _securityStateEncoder;
        private IList<Type> _knownTypes;

        public SecurityContextCookieSerializer(SecurityStateEncoder securityStateEncoder, IList<Type> knownTypes)
        {
            _securityStateEncoder = securityStateEncoder ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(securityStateEncoder));
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
