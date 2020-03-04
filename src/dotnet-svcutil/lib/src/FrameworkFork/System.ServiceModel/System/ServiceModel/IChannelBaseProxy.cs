// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.ServiceModel.Channels;

namespace System.ServiceModel
{
    /// <summary>
    /// An interface used by ChannelBase to override the ServiceChannel that would normally be returned by ClientBase. 
    /// </summary>
    internal interface IChannelBaseProxy
    {
        ServiceChannel GetServiceChannel();
    }
}
