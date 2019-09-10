// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
