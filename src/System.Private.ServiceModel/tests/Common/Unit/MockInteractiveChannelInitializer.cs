// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

public class MockInteractiveChannelInitializer : IInteractiveChannelInitializer
{
    public MockInteractiveChannelInitializer()
    {
        BeginDisplayInitializationUIOverride = DefaultBeginDisplayInitializationUI;
        EndDisplayInitializationUIOverride = DefaultEndDisplayInitializationUI;
    }

    public Func<IClientChannel, AsyncCallback, object, IAsyncResult> BeginDisplayInitializationUIOverride { get; set; }
    public Action<IAsyncResult> EndDisplayInitializationUIOverride { get; set; }

    public IAsyncResult BeginDisplayInitializationUI(IClientChannel channel, AsyncCallback callback, object state)
    {
        return BeginDisplayInitializationUIOverride(channel, callback, state);
    }

    public IAsyncResult DefaultBeginDisplayInitializationUI(IClientChannel channel, AsyncCallback callback, object state)
    {
        MockAsyncResult result = new MockAsyncResult(TimeSpan.FromSeconds(30), callback, state);
        result.Complete();
        return result;
    }

    public void EndDisplayInitializationUI(IAsyncResult result)
    {
        EndDisplayInitializationUIOverride(result);
    }

    public void DefaultEndDisplayInitializationUI(IAsyncResult result)
    {
    }
}