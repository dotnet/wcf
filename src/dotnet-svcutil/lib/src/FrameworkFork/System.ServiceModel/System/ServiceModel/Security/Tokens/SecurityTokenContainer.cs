// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.IdentityModel.Tokens;

namespace System.ServiceModel.Security.Tokens
{
    internal class SecurityTokenContainer
    {
        private SecurityToken _token;

        public SecurityTokenContainer(SecurityToken token)
        {
            if (token == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("token");
            }
            _token = token;
        }

        public SecurityToken Token
        {
            get { return _token; }
        }
    }
}
