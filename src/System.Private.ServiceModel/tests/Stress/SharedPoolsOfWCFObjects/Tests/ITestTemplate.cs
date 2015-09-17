// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using WcfService1;

namespace SharedPoolsOfWCFObjects
{
    public interface ITestTemplate<C>
    {
        EndpointAddress CreateEndPointAddress();
        Binding CreateBinding();
        ChannelFactory<C> CreateChannelFactory();
        void CloseFactory(ChannelFactory<C> factory);
        Task CloseFactoryAsync(ChannelFactory<C> factory);
        C CreateChannel(ChannelFactory<C> factory);
        void CloseChannel(C channel);
        Task CloseChannelAsync(C channel);
        Action<C> UseChannel();
        Func<C, Task> UseAsyncChannel();
    }
}
