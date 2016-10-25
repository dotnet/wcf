// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

public class MockChannelInitializer : IChannelInitializer
{
    public MockChannelInitializer()
    {
        InitializeOverride = DefaultInitialize;
    }

    public Action<IClientChannel> InitializeOverride { get; set; }

    public void Initialize(IClientChannel channel)
    {
        InitializeOverride(channel);
    }

    public void DefaultInitialize(IClientChannel channel)
    {
    }
}