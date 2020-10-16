// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Security
{
    public class SecurityNegotiationException : CommunicationException
    {
        public SecurityNegotiationException()
            : base()
        {
        }

        public SecurityNegotiationException(String message)
            : base(message)
        {
        }

        public SecurityNegotiationException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
