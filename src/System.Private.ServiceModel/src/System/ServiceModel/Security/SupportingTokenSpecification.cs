// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.IdentityModel.Tokens;
using System.ServiceModel.Security.Tokens;

namespace System.ServiceModel.Security
{
    public class SupportingTokenSpecification : SecurityTokenSpecification
    {
        private SecurityTokenParameters _tokenParameters;

        public SupportingTokenSpecification(SecurityToken token, ReadOnlyCollection<IAuthorizationPolicy> tokenPolicies, SecurityTokenAttachmentMode attachmentMode)
            : this(token, tokenPolicies, attachmentMode, null)
        { }

        public SupportingTokenSpecification(SecurityToken token, ReadOnlyCollection<IAuthorizationPolicy> tokenPolicies, SecurityTokenAttachmentMode attachmentMode, SecurityTokenParameters tokenParameters)
            : base(token, tokenPolicies)
        {
            SecurityTokenAttachmentModeHelper.Validate(attachmentMode);
            SecurityTokenAttachmentMode = attachmentMode;
            _tokenParameters = tokenParameters;
        }

        public SecurityTokenAttachmentMode SecurityTokenAttachmentMode { get; }

        internal SecurityTokenParameters SecurityTokenParameters
        {
            get { return _tokenParameters; }
        }
    }
}
