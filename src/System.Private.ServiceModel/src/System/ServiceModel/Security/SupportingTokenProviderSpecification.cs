// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.IdentityModel.Selectors;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
    internal class SupportingTokenProviderSpecification
    {
        private SecurityTokenParameters _tokenParameters;

        public SupportingTokenProviderSpecification(SecurityTokenProvider tokenProvider, SecurityTokenAttachmentMode attachmentMode, SecurityTokenParameters tokenParameters)
        {
            SecurityTokenAttachmentModeHelper.Validate(attachmentMode);
            TokenProvider = tokenProvider ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenProvider));
            SecurityTokenAttachmentMode = attachmentMode;
            _tokenParameters = tokenParameters ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(tokenParameters));
        }

        public SecurityTokenProvider TokenProvider { get; }

        public SecurityTokenAttachmentMode SecurityTokenAttachmentMode { get; }

        public SecurityTokenParameters TokenParameters
        {
            get { return _tokenParameters; }
        }
    }
}
