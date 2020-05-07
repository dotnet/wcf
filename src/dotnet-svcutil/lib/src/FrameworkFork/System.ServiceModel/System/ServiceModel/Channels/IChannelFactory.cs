// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
