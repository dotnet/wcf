// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace System.ServiceModel.Dispatcher
{
    public interface IChannelInitializer
    {
        void Initialize(IClientChannel channel);
    }
}
