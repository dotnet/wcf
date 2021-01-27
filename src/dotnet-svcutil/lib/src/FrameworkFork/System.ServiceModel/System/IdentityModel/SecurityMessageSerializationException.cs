// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
