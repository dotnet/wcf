// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
