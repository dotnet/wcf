// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    internal class PerSessionInstanceContextProvider : InstanceContextProviderBase
    {
        internal PerSessionInstanceContextProvider(DispatchRuntime dispatchRuntime)
            : base(dispatchRuntime)
        {
        }

        public override InstanceContext GetExistingInstanceContext(Message message, IContextChannel channel)
        {
            // Here is the flow for a Sessionful channel
            //  1. First request comes in on new channel.
            //  2. ServiceChannel.InstanceContext is returned which is null.
            //  3. InstanceBehavior.EnsureInstanceContext will create a new InstanceContext.
            //  4. this.InitializeInstanceContext is called with the newly created InstanceContext and the channel.
            //  5. If the channel is sessionful then its bound to the InstanceContext.
            //  6. Bind channel to the InstanceContext.
            //  7. For all further requests on the same channel, we will return ServiceChannel.InstanceContext which will be non null.
            ServiceChannel serviceChannel = this.GetServiceChannelFromProxy(channel);
            Contract.Assert((serviceChannel != null), "System.ServiceModel.Dispatcher.PerSessionInstanceContextProvider.GetExistingInstanceContext(), serviceChannel != null");
            return (serviceChannel != null) ? serviceChannel.InstanceContext : null;
        }
    }
}
