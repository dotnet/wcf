// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Runtime;
using System.ServiceModel;

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
            this.Initialize();
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
                return (this.TryGetProperty<string>(TokenTypeProperty, out result)) ? result : null;
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
                return (this.TryGetProperty<bool>(IsOptionalTokenProperty, out result)) ? result : defaultIsOptionalToken;
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
                return (this.TryGetProperty<bool>(RequireCryptographicTokenProperty, out result)) ? result : defaultRequireCryptographicToken;
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
                return (this.TryGetProperty<SecurityKeyUsage>(KeyUsageProperty, out result)) ? result : defaultKeyUsage;
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
                return (this.TryGetProperty<SecurityKeyType>(KeyTypeProperty, out result)) ? result : defaultKeyType;
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
                return (this.TryGetProperty<int>(KeySizeProperty, out result)) ? result : defaultKeySize;
            }
            set
            {
                if (value < 0)
                {
                    throw Fx.Exception.ArgumentOutOfRange("value", value, SRServiceModel.ValueMustBeNonNegative);
                }
                this.Properties[KeySizeProperty] = value;
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
            this.KeyType = defaultKeyType;
            this.KeyUsage = defaultKeyUsage;
            this.RequireCryptographicToken = defaultRequireCryptographicToken;
            this.KeySize = defaultKeySize;
            this.IsOptionalToken = defaultIsOptionalToken;
        }

        public TValue GetProperty<TValue>(string propertyName)
        {
            TValue result;
            if (!TryGetProperty<TValue>(propertyName, out result))
            {
                throw Fx.Exception.Argument(propertyName, string.Format(SRServiceModel.SecurityTokenRequirementDoesNotContainProperty, propertyName));
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
                throw Fx.Exception.Argument(propertyName, string.Format(SRServiceModel.SecurityTokenRequirementHasInvalidTypeForProperty, propertyName, dictionaryValue.GetType(), typeof(TValue)));
            }
            result = (TValue)dictionaryValue;
            return true;
        }
    }
}
