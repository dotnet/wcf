// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    public abstract class ChannelDispatcherBase : CommunicationObject
    {
        public abstract IChannelListener Listener { get; }

        public virtual void CloseInput()
        {
        }

        internal virtual void CloseInput(TimeSpan timeout)
        {
            CloseInput(); // back-compat
        }
    }
}
