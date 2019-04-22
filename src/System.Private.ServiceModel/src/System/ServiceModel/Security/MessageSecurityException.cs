// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Security
{
    public class MessageSecurityException : CommunicationException
    {
        private bool _isReplay = false;

        public MessageSecurityException()
            : base()
        {
        }

        public MessageSecurityException(String message)
            : base(message)
        {
        }

        public MessageSecurityException(String message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MessageSecurityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            throw new PlatformNotSupportedException();
        }

        internal MessageSecurityException(string message, Exception innerException, MessageFault fault)
            : base(message, innerException)
        {
            Fault = fault;
        }

        internal MessageSecurityException(String message, bool isReplay)
            : base(message)
        {
            _isReplay = isReplay;
        }

        internal bool ReplayDetected
        {
            get
            {
                return _isReplay;
            }
        }

        internal MessageFault Fault { get; }
    }
}
