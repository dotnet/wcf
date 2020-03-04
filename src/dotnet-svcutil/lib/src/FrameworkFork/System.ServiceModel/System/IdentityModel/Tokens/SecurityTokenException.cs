// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.IdentityModel.Tokens
{
    public class SecurityTokenException : Exception
    {
        public SecurityTokenException()
            : base()
        {
        }

        public SecurityTokenException(String message)
            : base(message)
        {
        }

        public SecurityTokenException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
