// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel;
using System.Collections.ObjectModel;
using System.IdentityModel.Tokens;
using Microsoft.Xml;

namespace System.IdentityModel.Selectors
{
    public abstract class SecurityTokenResolver // TODO: ICustomIdentityConfiguration
    {
        public SecurityToken ResolveToken(SecurityKeyIdentifier keyIdentifier)
        {
            if (keyIdentifier == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifier");
            }
            SecurityToken token;
            if (!this.TryResolveTokenCore(keyIdentifier, out token))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(string.Format(SRServiceModel.UnableToResolveTokenReference, keyIdentifier)));
            }
            return token;
        }

        public bool TryResolveToken(SecurityKeyIdentifier keyIdentifier, out SecurityToken token)
        {
            if (keyIdentifier == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifier");
            }
            return TryResolveTokenCore(keyIdentifier, out token);
        }

        public SecurityToken ResolveToken(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (keyIdentifierClause == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifierClause");
            }
            SecurityToken token;
            if (!this.TryResolveTokenCore(keyIdentifierClause, out token))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(string.Format(SRServiceModel.UnableToResolveTokenReference, keyIdentifierClause)));
            }
            return token;
        }

        public bool TryResolveToken(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityToken token)
        {
            if (keyIdentifierClause == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifierClause");
            }
            return this.TryResolveTokenCore(keyIdentifierClause, out token);
        }

        public SecurityKey ResolveSecurityKey(SecurityKeyIdentifierClause keyIdentifierClause)
        {
            if (keyIdentifierClause == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifierClause");
            }
            SecurityKey key;
            if (!this.TryResolveSecurityKeyCore(keyIdentifierClause, out key))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(string.Format(SRServiceModel.UnableToResolveKeyReference, keyIdentifierClause)));
            }
            return key;
        }

        public bool TryResolveSecurityKey(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
        {
            if (keyIdentifierClause == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifierClause");
            }
            return this.TryResolveSecurityKeyCore(keyIdentifierClause, out key);
        }

        /// <summary>
        /// Load custom configuration from Xml
        /// </summary>
        /// <param name="nodelist">Custom configuration elements</param>
        public virtual void LoadCustomConfiguration(XmlNodeList nodelist)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotImplementedException(string.Format(SRServiceModel.ID0023, this.GetType().AssemblyQualifiedName)));
        }

        // protected methods
        protected abstract bool TryResolveTokenCore(SecurityKeyIdentifier keyIdentifier, out SecurityToken token);
        protected abstract bool TryResolveTokenCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityToken token);
        protected abstract bool TryResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key);

        public static SecurityTokenResolver CreateDefaultSecurityTokenResolver(ReadOnlyCollection<SecurityToken> tokens, bool canMatchLocalId)
        {
            return new SimpleTokenResolver(tokens, canMatchLocalId);
        }

        private class SimpleTokenResolver : SecurityTokenResolver
        {
            private ReadOnlyCollection<SecurityToken> _tokens;
            private bool _canMatchLocalId;

            public SimpleTokenResolver(ReadOnlyCollection<SecurityToken> tokens, bool canMatchLocalId)
            {
                if (tokens == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokens");

                _tokens = tokens;
                _canMatchLocalId = canMatchLocalId;
            }

            protected override bool TryResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
            {
                if (keyIdentifierClause == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifierClause");

                key = null;
                for (int i = 0; i < _tokens.Count; ++i)
                {
                    SecurityKey securityKey = _tokens[i].ResolveKeyIdentifierClause(keyIdentifierClause);
                    if (securityKey != null)
                    {
                        key = securityKey;
                        return true;
                    }
                }

                if (keyIdentifierClause is EncryptedKeyIdentifierClause)
                {
                    EncryptedKeyIdentifierClause keyClause = (EncryptedKeyIdentifierClause)keyIdentifierClause;
                    SecurityKeyIdentifier keyIdentifier = keyClause.EncryptingKeyIdentifier;
                    if (keyIdentifier != null && keyIdentifier.Count > 0)
                    {
                        for (int i = 0; i < keyIdentifier.Count; i++)
                        {
                            SecurityKey unwrappingSecurityKey = null;
                            if (TryResolveSecurityKey(keyIdentifier[i], out unwrappingSecurityKey))
                            {
                                byte[] wrappedKey = keyClause.GetEncryptedKey();
                                string wrappingAlgorithm = keyClause.EncryptionMethod;
                                byte[] unwrappedKey = unwrappingSecurityKey.DecryptKey(wrappingAlgorithm, wrappedKey);
                                key = new InMemorySymmetricSecurityKey(unwrappedKey, false);
                                return true;
                            }
                        }
                    }
                }

                return key != null;
            }

            protected override bool TryResolveTokenCore(SecurityKeyIdentifier keyIdentifier, out SecurityToken token)
            {
                if (keyIdentifier == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifier");

                token = null;
                for (int i = 0; i < keyIdentifier.Count; ++i)
                {
                    SecurityToken securityToken = ResolveSecurityToken(keyIdentifier[i]);
                    if (securityToken != null)
                    {
                        token = securityToken;
                        break;
                    }
                }

                return (token != null);
            }

            protected override bool TryResolveTokenCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityToken token)
            {
                if (keyIdentifierClause == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifierClause");

                token = null;

                SecurityToken securityToken = ResolveSecurityToken(keyIdentifierClause);
                if (securityToken != null)
                    token = securityToken;

                return (token != null);
            }

            private SecurityToken ResolveSecurityToken(SecurityKeyIdentifierClause keyIdentifierClause)
            {
                if (keyIdentifierClause == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("keyIdentifierClause");

                if (!_canMatchLocalId && keyIdentifierClause is LocalIdKeyIdentifierClause)
                    return null;

                for (int i = 0; i < _tokens.Count; ++i)
                {
                    if (_tokens[i].MatchesKeyIdentifierClause(keyIdentifierClause))
                        return _tokens[i];
                }

                return null;
            }
        }
    }
}
