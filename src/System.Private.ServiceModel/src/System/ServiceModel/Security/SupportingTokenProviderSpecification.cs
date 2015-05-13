// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IdentityModel.Selectors;
using System.ServiceModel;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
    internal class SupportingTokenProviderSpecification
    {
        private SecurityTokenAttachmentMode _tokenAttachmentMode;
        private SecurityTokenProvider _tokenProvider;
        private SecurityTokenParameters _tokenParameters;

        public SupportingTokenProviderSpecification(SecurityTokenProvider tokenProvider, SecurityTokenAttachmentMode attachmentMode, SecurityTokenParameters tokenParameters)
        {
            if (tokenProvider == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenProvider");
            }
            SecurityTokenAttachmentModeHelper.Validate(attachmentMode);
            if (tokenParameters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("tokenParameters");
            }
            _tokenProvider = tokenProvider;
            _tokenAttachmentMode = attachmentMode;
            _tokenParameters = tokenParameters;
        }

        public SecurityTokenProvider TokenProvider
        {
            get { return _tokenProvider; }
        }

        public SecurityTokenAttachmentMode SecurityTokenAttachmentMode
        {
            get { return _tokenAttachmentMode; }
        }

        public SecurityTokenParameters TokenParameters
        {
            get { return _tokenParameters; }
        }
    }
}
