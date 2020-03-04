// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

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
