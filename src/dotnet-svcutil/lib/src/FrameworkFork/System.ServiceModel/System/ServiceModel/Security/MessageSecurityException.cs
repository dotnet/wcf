// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.ServiceModel.Channels;

namespace System.ServiceModel.Security
{
    public class MessageSecurityException : CommunicationException
    {
        private MessageFault _fault;
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

        internal MessageSecurityException(string message, Exception innerException, MessageFault fault)
            : base(message, innerException)
        {
            _fault = fault;
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

        internal MessageFault Fault
        {
            get { return _fault; }
        }
    }
}
