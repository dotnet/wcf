// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
