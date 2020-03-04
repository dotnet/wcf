// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.ServiceModel;

namespace System.ServiceModel.Channels
{
    public interface IChannelFactory : ICommunicationObject
    {
        T GetProperty<T>() where T : class;
    }

    public interface IChannelFactory<TChannel> : IChannelFactory
    {
        TChannel CreateChannel(EndpointAddress to);
        TChannel CreateChannel(EndpointAddress to, Uri via);
    }
}
