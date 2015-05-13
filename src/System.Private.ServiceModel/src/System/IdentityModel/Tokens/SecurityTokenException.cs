// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
