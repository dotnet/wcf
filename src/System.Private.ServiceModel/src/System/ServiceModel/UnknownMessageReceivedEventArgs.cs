// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
