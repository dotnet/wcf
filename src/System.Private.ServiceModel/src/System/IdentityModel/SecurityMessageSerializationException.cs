// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
