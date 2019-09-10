// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
