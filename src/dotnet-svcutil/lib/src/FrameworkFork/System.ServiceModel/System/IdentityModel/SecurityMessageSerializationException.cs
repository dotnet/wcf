// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.IdentityModel
{
    public class SecurityMessageSerializationException : Exception
    {
        public SecurityMessageSerializationException()
            : base()
        {
        }

        public SecurityMessageSerializationException(String message)
            : base(message)
        {
        }

        public SecurityMessageSerializationException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
