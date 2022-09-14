// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Runtime;

namespace System.IdentityModel.Selectors
{
    public class SecurityTokenRequirement
    {
        private const string Namespace = "http://schemas.microsoft.com/ws/2006/05/identitymodel/securitytokenrequirement";
        private const string tokenTypeProperty = Namespace + "/TokenType";
        private const string keyUsageProperty = Namespace + "/KeyUsage";
        private const string keyTypeProperty = Namespace + "/KeyType";
        private const string keySizeProperty = Namespace + "/KeySize";
        private const string requireCryptographicTokenProperty = Namespace + "/RequireCryptographicToken";
        private const string peerAuthenticationMode = Namespace + "/PeerAuthenticationMode";
        private const string isOptionalTokenProperty = Namespace + "/IsOptionalTokenProperty";

        private const bool defaultRequireCryptographicToken = false;
        private const SecurityKeyUsage defaultKeyUsage = SecurityKeyUsage.Signature;
        private const SecurityKeyType defaultKeyType = SecurityKeyType.SymmetricKey;
        private const int defaultKeySize = 0;
        private const bool defaultIsOptionalToken = false;

        private Dictionary<string, object> _properties;

        public SecurityTokenRequirement()
        {
            _properties = new Dictionary<string, object>();
            Initialize();
        }

        static public string TokenTypeProperty { get { return tokenTypeProperty; } }
        static public string KeyUsageProperty { get { return keyUsageProperty; } }
        static public string KeyTypeProperty { get { return keyTypeProperty; } }
        static public string KeySizeProperty { get { return keySizeProperty; } }
        static public string RequireCryptographicTokenProperty { get { return requireCryptographicTokenProperty; } }
        static public string PeerAuthenticationMode { get { return peerAuthenticationMode; } }
        static public string IsOptionalTokenProperty { get { return isOptionalTokenProperty; } }

        public string TokenType
        {
            get
            {
                string result;
                return (TryGetProperty(TokenTypeProperty, out result)) ? result : null;
            }
            set
            {
                _properties[TokenTypeProperty] = value;
            }
        }

        internal bool IsOptionalToken
        {
            get
            {
                bool result;
                return (TryGetProperty(IsOptionalTokenProperty, out result)) ? result : defaultIsOptionalToken;
            }
            set
            {
                _properties[IsOptionalTokenProperty] = value;
            }
        }

        public bool RequireCryptographicToken
        {
            get
            {
                bool result;
                return (TryGetProperty(RequireCryptographicTokenProperty, out result)) ? result : defaultRequireCryptographicToken;
            }
            set
            {
                _properties[RequireCryptographicTokenProperty] = (object)value;
            }
        }

        public SecurityKeyUsage KeyUsage
        {
            get
            {
                SecurityKeyUsage result;
                return (TryGetProperty(KeyUsageProperty, out result)) ? result : defaultKeyUsage;
            }
            set
            {
                SecurityKeyUsageHelper.Validate(value);
                _properties[KeyUsageProperty] = (object)value;
            }
        }

        public SecurityKeyType KeyType
        {
            get
            {
                SecurityKeyType result;
                return (TryGetProperty(KeyTypeProperty, out result)) ? result : defaultKeyType;
            }
            set
            {
                SecurityKeyTypeHelper.Validate(value);
                _properties[KeyTypeProperty] = (object)value;
            }
        }

        public int KeySize
        {
            get
            {
                int result;
                return (TryGetProperty(KeySizeProperty, out result)) ? result : defaultKeySize;
            }
            set
            {
                if (value < 0)
                {
                    throw Fx.Exception.ArgumentOutOfRange("value", value, SRP.ValueMustBeNonNegative);
                }
                Properties[KeySizeProperty] = value;
            }
        }

        public IDictionary<string, object> Properties
        {
            get
            {
                return _properties;
            }
        }

        private void Initialize()
        {
            KeyType = defaultKeyType;
            KeyUsage = defaultKeyUsage;
            RequireCryptographicToken = defaultRequireCryptographicToken;
            KeySize = defaultKeySize;
            IsOptionalToken = defaultIsOptionalToken;
        }

        public TValue GetProperty<TValue>(string propertyName)
        {
            TValue result;
            if (!TryGetProperty(propertyName, out result))
            {
                throw Fx.Exception.Argument(propertyName, string.Format(SRP.SecurityTokenRequirementDoesNotContainProperty, propertyName));
            }
            return result;
        }

        public bool TryGetProperty<TValue>(string propertyName, out TValue result)
        {
            object dictionaryValue;
            if (!Properties.TryGetValue(propertyName, out dictionaryValue))
            {
                result = default(TValue);
                return false;
            }
            if (dictionaryValue != null && !typeof(TValue).IsAssignableFrom(dictionaryValue.GetType()))
            {
                throw Fx.Exception.Argument(propertyName, string.Format(SRP.SecurityTokenRequirementHasInvalidTypeForProperty, propertyName, dictionaryValue.GetType(), typeof(TValue)));
            }
            result = (TValue)dictionaryValue;
            return true;
        }
    }
}
