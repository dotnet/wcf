// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

public class MockRequestChannel : MockChannelBase, IRequestChannel
{
    private readonly Uri _via;

    public Uri Via
    {
        get { return this._via; }
    }

    public Func<Message, TimeSpan, Message> RequestOverride { get; set; }
    public Func<Message,TimeSpan,AsyncCallback,object, IAsyncResult> BeginRequestOverride { get; set; }
    public Func<IAsyncResult, Message> EndRequestOverride { get; set; }

    public MockRequestChannel(ChannelManagerBase manager, MessageEncoderFactory encoderFactory, EndpointAddress address, Uri via)
            : base(manager, encoderFactory, address)
    {
        this._via = via;

        RequestOverride = DefaultRequest;
        BeginRequestOverride = DefaultBeginRequest;
        EndRequestOverride = DefaultEndRequest;
    }

    public Message Request(Message message, TimeSpan timeout)
    {
        return RequestOverride(message, timeout);
    }

    public Message DefaultRequest(Message message, TimeSpan timeout)
    {
        // Default is just to loopback the request message.
        // Set RequestOverride to a delegate to do anything else you need.
        return message;
    }

    public Message Request(Message message)
    {
        return this.Request(message, DefaultReceiveTimeout);
    }

    public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
    {
        return BeginRequestOverride(message, timeout, callback, state);
    }

    public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
    {
        return BeginRequest(message, DefaultReceiveTimeout, callback, state);
    }

    public IAsyncResult DefaultBeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
    {
        // Default is to create an already completed IAsyncResult containing
        // the input message.
        MockAsyncResult result = new MockAsyncResult(timeout, callback, state);
        result.Complete(message);
        return result;
    }

    public Message EndRequest(IAsyncResult result)
    {
        return EndRequestOverride(result);
    }

    public Message DefaultEndRequest(IAsyncResult result)
    {
        // Default is to just loopback the input message from BeginRequest
        return (Message)((MockAsyncResult)result).Result;
    }

}