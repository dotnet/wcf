﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IdentityModel.Tokens;
using System.Runtime.Serialization;

namespace System.ServiceModel.Security.Tokens
{
    [Serializable]
    class SecurityContextTokenValidationException : SecurityTokenValidationException
    {
        public SecurityContextTokenValidationException() : base() { }
        public SecurityContextTokenValidationException(string message) : base(message) { }
        public SecurityContextTokenValidationException(string message, Exception innerException) : base(message, innerException) { }
        protected SecurityContextTokenValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
