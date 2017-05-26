// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

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

        protected SecurityTokenException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            throw new PlatformNotSupportedException();
        }
    }
}
