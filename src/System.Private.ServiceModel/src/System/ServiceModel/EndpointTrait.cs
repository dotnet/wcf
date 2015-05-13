// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.ServiceModel
{
    internal abstract class EndpointTrait<TChannel>
        where TChannel : class
    {
        public abstract ChannelFactory<TChannel> CreateChannelFactory();
    }
}
