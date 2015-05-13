// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.ServiceModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
    public class SupportingTokenSpecification : SecurityTokenSpecification
    {
        private SecurityTokenAttachmentMode _tokenAttachmentMode;
        private SecurityTokenParameters _tokenParameters;

        public SupportingTokenSpecification(SecurityToken token, ReadOnlyCollection<IAuthorizationPolicy> tokenPolicies, SecurityTokenAttachmentMode attachmentMode)
            : this(token, tokenPolicies, attachmentMode, null)
        { }

        public SupportingTokenSpecification(SecurityToken token, ReadOnlyCollection<IAuthorizationPolicy> tokenPolicies, SecurityTokenAttachmentMode attachmentMode, SecurityTokenParameters tokenParameters)
            : base(token, tokenPolicies)
        {
            SecurityTokenAttachmentModeHelper.Validate(attachmentMode);
            _tokenAttachmentMode = attachmentMode;
            _tokenParameters = tokenParameters;
        }

        public SecurityTokenAttachmentMode SecurityTokenAttachmentMode
        {
            get { return _tokenAttachmentMode; }
        }

        internal SecurityTokenParameters SecurityTokenParameters
        {
            get { return _tokenParameters; }
        }
    }
}
