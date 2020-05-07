// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Selectors;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
    internal class SupportingTokenAuthenticatorSpecification
    {
        private SecurityTokenAttachmentMode _tokenAttachmentMode;
        private SecurityTokenAuthenticator _tokenAuthenticator;
        private SecurityTokenResolver _tokenResolver;
        private SecurityTokenParameters _tokenParameters;
        private bool _isTokenOptional;

        public SupportingTokenAuthenticatorSpecification(SecurityTokenAuthenticator tokenAuthenticator, SecurityTokenResolver securityTokenResolver, SecurityTokenAttachmentMode attachmentMode, SecurityTokenParameters tokenParameters)
            : this(tokenAuthenticator, securityTokenResolver, attachmentMode, tokenParameters, false)
        {
        }

        internal SupportingTokenAuthenticatorSpecification(SecurityTokenAuthenticator tokenAuthenticator, SecurityTokenResolver securityTokenResolver, SecurityTokenAttachmentMode attachmentMode, SecurityTokenParameters tokenParameters, bool isTokenOptional)
        {
            if (tokenAuthenticator == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenAuthenticator");
            }

            SecurityTokenAttachmentModeHelper.Validate(attachmentMode);

            if (tokenParameters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenParameters");
            }
            _tokenAuthenticator = tokenAuthenticator;
            _tokenResolver = securityTokenResolver;
            _tokenAttachmentMode = attachmentMode;
            _tokenParameters = tokenParameters;
            _isTokenOptional = isTokenOptional;
        }

        public SecurityTokenAuthenticator TokenAuthenticator
        {
            get { return _tokenAuthenticator; }
        }

        public SecurityTokenResolver TokenResolver
        {
            get { return _tokenResolver; }
        }

        public SecurityTokenAttachmentMode SecurityTokenAttachmentMode
        {
            get { return _tokenAttachmentMode; }
        }

        public SecurityTokenParameters TokenParameters
        {
            get { return _tokenParameters; }
        }

        internal bool IsTokenOptional
        {
            get { return _isTokenOptional; }
            set { _isTokenOptional = value; }
        }
    }
}
