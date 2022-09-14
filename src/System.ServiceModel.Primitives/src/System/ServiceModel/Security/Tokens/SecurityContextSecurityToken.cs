// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.Xml;

namespace System.ServiceModel.Security.Tokens
{
    internal class SecurityContextSecurityToken : SecurityToken, TimeBoundedCache.IExpirableItem, IDisposable
    {
        private UniqueId _keyGeneration = null;
        private DateTime _tokenEffectiveTime;
        private DateTime _tokenExpirationTime;
        private byte[] _key;
        private ReadOnlyCollection<IAuthorizationPolicy> _authorizationPolicies;
        private ReadOnlyCollection<SecurityKey> _securityKeys;
        private string _id;
        private bool _disposed = false;

        internal SecurityContextSecurityToken(SecurityContextSecurityToken sourceToken, string id)
            : this(sourceToken, id, sourceToken._key, sourceToken._keyGeneration, sourceToken.KeyEffectiveTime, sourceToken.KeyExpirationTime, sourceToken.AuthorizationPolicies)
        {
        }

        internal SecurityContextSecurityToken(SecurityContextSecurityToken sourceToken, string id, byte[] key, UniqueId keyGeneration, DateTime keyEffectiveTime, DateTime keyExpirationTime, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies)
            : base()
        {
            _id = id;
            Initialize(sourceToken.ContextId, key, sourceToken.ValidFrom, sourceToken.ValidTo, authorizationPolicies, sourceToken.IsCookieMode, keyGeneration, keyEffectiveTime, keyExpirationTime);
            CookieBlob = sourceToken.CookieBlob;
            BootstrapMessageProperty = (sourceToken.BootstrapMessageProperty == null) ? null : (SecurityMessageProperty)sourceToken.BootstrapMessageProperty.CreateCopy();
        }

        /// <summary>
        /// Gets or Sets the SecurityMessageProperty extracted from 
        /// the Bootstrap message. This will contain the original tokens
        /// that the client used to Authenticate with the service. By 
        /// default, this is turned off. To turn this feature on, add a custom 
        /// ServiceCredentialsSecurityTokenManager and override  
        /// CreateSecurityTokenManager. Create the SecurityContextToken Authenticator by calling 
        /// ServiceCredentialsSecurityTokenManager.CreateSecureConversationTokenAuthenticator
        /// with 'preserveBootstrapTokens' parameter to true. 
        /// If there are any UserNameSecurityToken in the bootstrap message, the password in
        /// these tokens will be removed. When 'Cookie' mode SCT is enabled the BootstrapMessageProperty
        /// is not preserved in the Cookie. To preserve the bootstrap tokens in the CookieMode case
        /// write a custom Serializer and serialize the property as part of the cookie.
        /// </summary>
        public SecurityMessageProperty BootstrapMessageProperty { get; set; }

        public override string Id
        {
            get { return _id; }
        }

        public UniqueId ContextId { get; private set; } = null;

        public UniqueId KeyGeneration
        {
            get
            {
                return _keyGeneration;
            }
        }

        public DateTime KeyEffectiveTime { get; private set; }

        public DateTime KeyExpirationTime { get; private set; }

        public ReadOnlyCollection<IAuthorizationPolicy> AuthorizationPolicies
        {
            get
            {
                ThrowIfDisposed();
                return _authorizationPolicies;
            }

            internal set
            {
                _authorizationPolicies = value;
            }
        }

        public override ReadOnlyCollection<SecurityKey> SecurityKeys
        {
            get
            {
                return _securityKeys;
            }
        }

        public override DateTime ValidFrom
        {
            get { return _tokenEffectiveTime; }
        }

        public override DateTime ValidTo
        {
            get { return _tokenExpirationTime; }
        }

        internal byte[] CookieBlob { get; }

        /// <summary>
        /// This is set by the issuer when creating the SCT to be sent in the RSTR
        /// The SecurityContextTokenManager examines this property to determine how to write
        /// out the SCT
        /// This field is set to true when the issuer reads in a cookie mode SCT
        /// </summary>
        public bool IsCookieMode { get; private set; } = false;

        DateTime TimeBoundedCache.IExpirableItem.ExpirationTime
        {
            get
            {
                return ValidTo;
            }
        }

        public override string ToString()
        {
            return String.Format(CultureInfo.CurrentCulture, "SecurityContextSecurityToken(Identifier='{0}', KeyGeneration='{1}')", ContextId, _keyGeneration);
        }

        private void Initialize(UniqueId contextId, byte[] key, DateTime validFrom, DateTime validTo, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies, bool isCookieMode,
            UniqueId keyGeneration, DateTime keyEffectiveTime, DateTime keyExpirationTime)
        {
            if (key == null || key.Length == 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(key));
            }

            DateTime tokenEffectiveTimeUtc = validFrom.ToUniversalTime();
            DateTime tokenExpirationTimeUtc = validTo.ToUniversalTime();
            if (tokenEffectiveTimeUtc > tokenExpirationTimeUtc)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("validFrom", SRP.EffectiveGreaterThanExpiration);
            }
            _tokenEffectiveTime = tokenEffectiveTimeUtc;
            _tokenExpirationTime = tokenExpirationTimeUtc;

            KeyEffectiveTime = keyEffectiveTime.ToUniversalTime();
            KeyExpirationTime = keyExpirationTime.ToUniversalTime();
            if (KeyEffectiveTime > KeyExpirationTime)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("keyEffectiveTime", SRP.EffectiveGreaterThanExpiration);
            }
            if ((KeyEffectiveTime < tokenEffectiveTimeUtc) || (KeyExpirationTime > tokenExpirationTimeUtc))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.KeyLifetimeNotWithinTokenLifetime);
            }

            _key = new byte[key.Length];
            Buffer.BlockCopy(key, 0, _key, 0, key.Length);
            ContextId = contextId ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(contextId));
            _keyGeneration = keyGeneration;
            _authorizationPolicies = authorizationPolicies ?? EmptyReadOnlyCollection<IAuthorizationPolicy>.Instance;
            List<SecurityKey> temp = new List<SecurityKey>(1);
            temp.Add(new InMemorySymmetricSecurityKey(_key, false));
            _securityKeys = new ReadOnlyCollection<SecurityKey>(temp);
            IsCookieMode = isCookieMode;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                System.IdentityModel.SecurityUtils.DisposeAuthorizationPoliciesIfNecessary(_authorizationPolicies);
                if (BootstrapMessageProperty != null)
                {
                    BootstrapMessageProperty.Dispose();
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(GetType().FullName));
            }
        }
    }
}
