﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens;
using System.ServiceModel;
using System.IdentityModel.Selectors;
using System.ServiceModel.Security.Tokens;
using System.IO;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace System.ServiceModel.Security
{
    internal sealed class SecurityHeaderTokenResolver : SecurityTokenResolver, IdentityModel.IWrappedTokenKeyResolver
    {
        private const int InitialTokenArraySize = 10;
        private int _tokenCount;
        private SecurityTokenEntry[] _tokens;
        private ReceiveSecurityHeader _securityHeader;

        public SecurityHeaderTokenResolver()
            : this(null)
        {
        }

        public SecurityHeaderTokenResolver(ReceiveSecurityHeader securityHeader)
        {
            _tokens = new SecurityTokenEntry[InitialTokenArraySize];
            _securityHeader = securityHeader;
        }

        public SecurityToken ExpectedWrapper { get; set; }

        public SecurityTokenParameters ExpectedWrapperTokenParameters { get; set; }

        public void Add(SecurityToken token)
        {
            Add(token, SecurityTokenReferenceStyle.Internal, null);
        }

        public void Add(SecurityToken token, SecurityTokenReferenceStyle allowedReferenceStyle, SecurityTokenParameters tokenParameters)
        {
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(token));
            }

            if ((allowedReferenceStyle == SecurityTokenReferenceStyle.External) && (tokenParameters == null))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.ResolvingExternalTokensRequireSecurityTokenParameters);
            }

            EnsureCapacityToAddToken();
            _tokens[_tokenCount++] = new SecurityTokenEntry(token, tokenParameters, allowedReferenceStyle);
        }

        private void EnsureCapacityToAddToken()
        {
            if (_tokenCount == _tokens.Length)
            {
                SecurityTokenEntry[] newTokens = new SecurityTokenEntry[_tokens.Length * 2];
                Array.Copy(_tokens, 0, newTokens, 0, _tokenCount);
                _tokens = newTokens;
            }
        }

        public bool CheckExternalWrapperMatch(SecurityKeyIdentifier keyIdentifier)
        {
            if (ExpectedWrapper == null || ExpectedWrapperTokenParameters == null)
            {
                return false;
            }

            for (int i = 0; i < keyIdentifier.Count; i++)
            {
                if (ExpectedWrapperTokenParameters.MatchesKeyIdentifierClause(ExpectedWrapper, keyIdentifier[i], SecurityTokenReferenceStyle.External))
                {
                    return true;
                }
            }
            return false;
        }

        internal SecurityToken ResolveToken(SecurityKeyIdentifier keyIdentifier, bool matchOnlyExternalTokens, bool resolveIntrinsicKeyClause)
        {
            if (keyIdentifier == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(keyIdentifier));
            }
            for (int i = 0; i < keyIdentifier.Count; i++)
            {
                SecurityToken token = ResolveToken(keyIdentifier[i], matchOnlyExternalTokens, resolveIntrinsicKeyClause);
                if (token != null)
                {
                    return token;
                }
            }
            return null;
        }

        private SecurityKey ResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, bool createIntrinsicKeys)
        {
            if (keyIdentifierClause == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(keyIdentifierClause)));
            }

            SecurityKey securityKey;
            for (int i = 0; i < _tokenCount; i++)
            {
                securityKey = _tokens[i].Token.ResolveKeyIdentifierClause(keyIdentifierClause);
                if (securityKey != null)
                {
                    return securityKey;
                }
            }

            if (createIntrinsicKeys)
            {
                if (SecurityUtils.TryCreateKeyFromIntrinsicKeyClause(keyIdentifierClause, this, out securityKey))
                {
                    return securityKey;
                }
            }

            return null;
        }

        private bool MatchDirectReference(SecurityToken token, SecurityKeyIdentifierClause keyClause)
        {
            LocalIdKeyIdentifierClause localClause = keyClause as LocalIdKeyIdentifierClause;
            if (localClause == null)
            {
                return false;
            }

            return token.MatchesKeyIdentifierClause(localClause);
        }

        internal SecurityToken ResolveToken(SecurityKeyIdentifierClause keyIdentifierClause, bool matchOnlyExternal, bool resolveIntrinsicKeyClause)
        {
            if (keyIdentifierClause == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(keyIdentifierClause));
            }

            SecurityToken resolvedToken = null;
            for (int i = 0; i < _tokenCount; i++)
            {
                if (matchOnlyExternal && _tokens[i].AllowedReferenceStyle != SecurityTokenReferenceStyle.External)
                {
                    continue;
                }

                SecurityToken token = _tokens[i].Token;
                if (_tokens[i].TokenParameters != null && _tokens[i].TokenParameters.MatchesKeyIdentifierClause(token, keyIdentifierClause, _tokens[i].AllowedReferenceStyle))
                {
                    resolvedToken = token;
                    break;
                }
                else if (_tokens[i].TokenParameters == null)
                {
                    // match it according to the allowed reference style
                    if (_tokens[i].AllowedReferenceStyle == SecurityTokenReferenceStyle.Internal && MatchDirectReference(token, keyIdentifierClause))
                    {
                        resolvedToken = token;
                        break;
                    }
                }
            }

            if ((resolvedToken == null) && (keyIdentifierClause is X509RawDataKeyIdentifierClause) && (!matchOnlyExternal) && (resolveIntrinsicKeyClause))
            {
                resolvedToken = new X509SecurityToken(new X509Certificate2(((X509RawDataKeyIdentifierClause)keyIdentifierClause).GetX509RawData()));
            }

            byte[] derivationNonce = keyIdentifierClause.GetDerivationNonce();
            if ((resolvedToken != null) && (derivationNonce != null))
            {
                // A Implicit Derived Key is specified. Create a derived key off of the resolve token.
                if (SecurityUtils.GetSecurityKey<SymmetricSecurityKey>(resolvedToken) == null)
                {
                    // The resolved token contains no Symmetric Security key and thus we cannot create 
                    // a derived key off of it.
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.Format(SRP.UnableToDeriveKeyFromKeyInfoClause, keyIdentifierClause, resolvedToken)));
                }

                int derivationLength = (keyIdentifierClause.DerivationLength == 0) ? DerivedKeySecurityToken.DefaultDerivedKeyLength : keyIdentifierClause.DerivationLength;
                if (derivationLength > _securityHeader.MaxDerivedKeyLength)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(SRP.Format(SRP.DerivedKeyLengthSpecifiedInImplicitDerivedKeyClauseTooLong, keyIdentifierClause.ToString(), derivationLength, _securityHeader.MaxDerivedKeyLength)));
                }

                bool alreadyDerived = false;
                for (int i = 0; i < _tokenCount; ++i)
                {
                    DerivedKeySecurityToken derivedKeyToken = _tokens[i].Token as DerivedKeySecurityToken;
                    if (derivedKeyToken != null)
                    {
                        if ((derivedKeyToken.Length == derivationLength) &&
                            (SecurityUtils.IsEqual(derivedKeyToken.Nonce, derivationNonce)) &&
                            (derivedKeyToken.TokenToDerive.MatchesKeyIdentifierClause(keyIdentifierClause)))
                        {
                            // This is a implcit derived key for which we have already derived the token.
                            resolvedToken = _tokens[i].Token;
                            alreadyDerived = true;
                            break;
                        }
                    }
                }

                if (!alreadyDerived)
                {
                    string psha1Algorithm = SecurityUtils.GetKeyDerivationAlgorithm(_securityHeader.StandardsManager.MessageSecurityVersion.SecureConversationVersion);

                    resolvedToken = new DerivedKeySecurityToken(-1, 0, derivationLength, null, derivationNonce, resolvedToken, keyIdentifierClause, psha1Algorithm, SecurityUtils.GenerateId());
                    ((DerivedKeySecurityToken)resolvedToken).InitializeDerivedKey(derivationLength);
                    Add(resolvedToken, SecurityTokenReferenceStyle.Internal, null);
                    _securityHeader.EnsureDerivedKeyLimitNotReached();
                }
            }

            return resolvedToken;
        }

        public override string ToString()
        {
            using (StringWriter writer = new StringWriter(CultureInfo.InvariantCulture))
            {
                writer.WriteLine("SecurityTokenResolver");
                writer.WriteLine("    (");
                writer.WriteLine("    TokenCount = {0},", _tokenCount);
                for (int i = 0; i < _tokenCount; i++)
                {
                    writer.WriteLine("    TokenEntry[{0}] = (AllowedReferenceStyle={1}, Token={2}, Parameters={3})",
                        i, _tokens[i].AllowedReferenceStyle, _tokens[i].Token.GetType(), _tokens[i].TokenParameters);
                }
                writer.WriteLine("    )");
                return writer.ToString();
            }
        }

        protected override bool TryResolveTokenCore(SecurityKeyIdentifier keyIdentifier, out SecurityToken token)
        {
            token = ResolveToken(keyIdentifier, false, true);
            return token != null;
        }

        internal bool TryResolveToken(SecurityKeyIdentifier keyIdentifier, bool matchOnlyExternalTokens, bool resolveIntrinsicKeyClause, out SecurityToken token)
        {
            token = ResolveToken(keyIdentifier, matchOnlyExternalTokens, resolveIntrinsicKeyClause);
            return token != null;
        }

        protected override bool TryResolveTokenCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityToken token)
        {
            token = ResolveToken(keyIdentifierClause, false, true);
            return token != null;
        }

        internal bool TryResolveToken(SecurityKeyIdentifierClause keyIdentifierClause, bool matchOnlyExternalTokens, bool resolveIntrinsicKeyClause, out SecurityToken token)
        {
            token = ResolveToken(keyIdentifierClause, matchOnlyExternalTokens, resolveIntrinsicKeyClause);
            return token != null;
        }

        internal bool TryResolveSecurityKey(SecurityKeyIdentifierClause keyIdentifierClause, bool createIntrinsicKeys, out SecurityKey key)
        {
            if (keyIdentifierClause == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(keyIdentifierClause));
            }
            key = ResolveSecurityKeyCore(keyIdentifierClause, createIntrinsicKeys);
            return key != null;
        }

        protected override bool TryResolveSecurityKeyCore(SecurityKeyIdentifierClause keyIdentifierClause, out SecurityKey key)
        {
            key = ResolveSecurityKeyCore(keyIdentifierClause, true);
            return key != null;
        }

        private struct SecurityTokenEntry
        {
            private SecurityTokenReferenceStyle _allowedReferenceStyle;

            public SecurityTokenEntry(SecurityToken token, SecurityTokenParameters tokenParameters, SecurityTokenReferenceStyle allowedReferenceStyle)
            {
                Token = token;
                TokenParameters = tokenParameters;
                _allowedReferenceStyle = allowedReferenceStyle;
            }

            public SecurityToken Token { get; }

            public SecurityTokenParameters TokenParameters { get; }

            public SecurityTokenReferenceStyle AllowedReferenceStyle
            {
                get { return _allowedReferenceStyle; }
            }
        }
    }
}
