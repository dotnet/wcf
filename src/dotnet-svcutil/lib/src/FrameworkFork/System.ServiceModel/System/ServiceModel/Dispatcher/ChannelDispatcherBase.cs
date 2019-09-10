// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
