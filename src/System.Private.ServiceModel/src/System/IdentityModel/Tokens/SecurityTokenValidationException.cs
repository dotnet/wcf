// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.IdentityModel.Tokens
{
    public class SecurityTokenValidationException : SecurityTokenException
    {
        public SecurityTokenValidationException()
            : base()
        {
        }

        public SecurityTokenValidationException(String message)
            : base(message)
        {
        }

        public SecurityTokenValidationException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
