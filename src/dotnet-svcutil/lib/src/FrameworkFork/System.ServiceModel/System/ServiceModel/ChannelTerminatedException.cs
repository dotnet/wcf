// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel
{
    public class ChannelTerminatedException : CommunicationException
    {
        public ChannelTerminatedException() { }
        public ChannelTerminatedException(string message) : base(message) { }
        public ChannelTerminatedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
