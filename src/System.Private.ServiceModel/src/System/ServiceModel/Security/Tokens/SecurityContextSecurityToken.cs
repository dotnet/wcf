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
    public class SecurityContextSecurityToken : SecurityToken, TimeBoundedCache.IExpirableItem, IDisposable
    {
        private UniqueId _keyGeneration = null;
        private DateTime _tokenEffectiveTime;
        private DateTime _tokenExpirationTime;
        private byte[] _key;
        private string _keyString;
        private ReadOnlyCollection<IAuthorizationPolicy> _authorizationPolicies;
        private ReadOnlyCollection<SecurityKey> _securityKeys;
        private string _id;
        private bool _disposed = false;

        public SecurityContextSecurityToken(UniqueId contextId, byte[] key, DateTime validFrom, DateTime validTo)
            : this(contextId, SecurityUtils.GenerateId(), key, validFrom, validTo)
        { }

        public SecurityContextSecurityToken(UniqueId contextId, string id, byte[] key, DateTime validFrom, DateTime validTo)
            : this(contextId, id, key, validFrom, validTo, null)
        { }

        public SecurityContextSecurityToken(UniqueId contextId, string id, byte[] key, DateTime validFrom, DateTime validTo, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies)
            : base()
        {
            _id = id;
            Initialize(contextId, key, validFrom, validTo, authorizationPolicies, false, null, validFrom, validTo);
        }

        public SecurityContextSecurityToken(UniqueId contextId, string id, byte[] key, DateTime validFrom, DateTime validTo, UniqueId keyGeneration, DateTime keyEffectiveTime, DateTime keyExpirationTime, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies)
            : base()
        {
            _id = id;
            Initialize(contextId, key, validFrom, validTo, authorizationPolicies, false, keyGeneration, keyEffectiveTime, keyExpirationTime);
        }

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

        internal SecurityContextSecurityToken(UniqueId contextId, string id, byte[] key, DateTime validFrom, DateTime validTo, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies, bool isCookieMode, byte[] cookieBlob)
            : this(contextId, id, key, validFrom, validTo, authorizationPolicies, isCookieMode, cookieBlob, null, validFrom, validTo)
        {
        }

        internal SecurityContextSecurityToken(UniqueId contextId, string id, byte[] key, DateTime validFrom, DateTime validTo, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies, bool isCookieMode, byte[] cookieBlob,
            UniqueId keyGeneration, DateTime keyEffectiveTime, DateTime keyExpirationTime)
            : base()
        {
            _id = id;
            Initialize(contextId, key, validFrom, validTo, authorizationPolicies, isCookieMode, keyGeneration, keyEffectiveTime, keyExpirationTime);
            CookieBlob = cookieBlob;
        }

        private SecurityContextSecurityToken(SecurityContextSecurityToken from)
        {
            ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies = System.IdentityModel.SecurityUtils.CloneAuthorizationPoliciesIfNecessary(from._authorizationPolicies);
            _id = from._id;
            Initialize(from.ContextId, from._key, from._tokenEffectiveTime, from._tokenExpirationTime, authorizationPolicies, from.IsCookieMode, from._keyGeneration, from.KeyEffectiveTime, from.KeyExpirationTime);
            CookieBlob = from.CookieBlob;
            BootstrapMessageProperty = (from.BootstrapMessageProperty == null) ? null : (SecurityMessageProperty)from.BootstrapMessageProperty.CreateCopy();
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

        internal string GetBase64KeyString()
        {
            if (_keyString == null)
            {
                _keyString = Convert.ToBase64String(_key);
            }
            return _keyString;
        }

        internal byte[] GetKeyBytes()
        {
            byte[] retval = new byte[_key.Length];
            Buffer.BlockCopy(_key, 0, retval, 0, _key.Length);
            return retval;
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("validFrom", SR.EffectiveGreaterThanExpiration);
            }
            _tokenEffectiveTime = tokenEffectiveTimeUtc;
            _tokenExpirationTime = tokenExpirationTimeUtc;

            KeyEffectiveTime = keyEffectiveTime.ToUniversalTime();
            KeyExpirationTime = keyExpirationTime.ToUniversalTime();
            if (KeyEffectiveTime > KeyExpirationTime)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("keyEffectiveTime", SR.EffectiveGreaterThanExpiration);
            }
            if ((KeyEffectiveTime < tokenEffectiveTimeUtc) || (KeyExpirationTime > tokenExpirationTimeUtc))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.KeyLifetimeNotWithinTokenLifetime);
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

        public static SecurityContextSecurityToken CreateCookieSecurityContextToken(UniqueId contextId, string id, byte[] key,
            DateTime validFrom, DateTime validTo, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies, SecurityStateEncoder securityStateEncoder)
        {
            return CreateCookieSecurityContextToken(contextId, id, key, validFrom, validTo, null, validFrom, validTo, authorizationPolicies, securityStateEncoder);
        }


        public static SecurityContextSecurityToken CreateCookieSecurityContextToken(UniqueId contextId, string id, byte[] key,
            DateTime validFrom, DateTime validTo, UniqueId keyGeneration, DateTime keyEffectiveTime,
            DateTime keyExpirationTime, ReadOnlyCollection<IAuthorizationPolicy> authorizationPolicies, SecurityStateEncoder securityStateEncoder)
        {
            SecurityContextCookieSerializer cookieSerializer = new SecurityContextCookieSerializer(securityStateEncoder, null);
            byte[] cookieBlob = cookieSerializer.CreateCookieFromSecurityContext(contextId, id, key, validFrom, validTo, keyGeneration,
                                keyEffectiveTime, keyExpirationTime, authorizationPolicies);

            return new SecurityContextSecurityToken(contextId, id, key, validFrom, validTo,
                authorizationPolicies, true, cookieBlob, keyGeneration, keyEffectiveTime, keyExpirationTime);
        }

        internal SecurityContextSecurityToken Clone()
        {
            ThrowIfDisposed();
            return new SecurityContextSecurityToken(this);
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
