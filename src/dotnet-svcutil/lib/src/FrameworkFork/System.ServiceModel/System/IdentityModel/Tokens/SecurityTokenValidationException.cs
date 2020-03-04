// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
