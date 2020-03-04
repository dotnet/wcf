// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    public sealed class UnknownMessageReceivedEventArgs : EventArgs
    {
        private Message _message;

        internal UnknownMessageReceivedEventArgs(Message message)
        {
            _message = message;
        }

        public Message Message
        {
            get { return _message; }
        }
    }
}
