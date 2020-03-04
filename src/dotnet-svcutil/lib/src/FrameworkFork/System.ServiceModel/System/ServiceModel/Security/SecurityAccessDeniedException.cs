// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Security
{
    public class SecurityAccessDeniedException : CommunicationException
    {
        public SecurityAccessDeniedException(String message)
            : base(message)
        {
        }

        public SecurityAccessDeniedException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
