// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel
{
    public class CommunicationObjectFaultedException : CommunicationException
    {
        public CommunicationObjectFaultedException() { }
        public CommunicationObjectFaultedException(string message) : base(message) { }
        public CommunicationObjectFaultedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
