// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


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
