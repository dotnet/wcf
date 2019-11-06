// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.MessageInterceptor
{
    internal class InterceptingChannelListener<TChannel>
        : ChannelListenerBase<TChannel>
        where TChannel : class, IChannel
    {
        private ChannelMessageInterceptor _interceptor;
        private IChannelListener<TChannel> _innerChannelListener;

        public InterceptingChannelListener(ChannelMessageInterceptor interceptor, BindingContext context)
        {
            _interceptor = interceptor;
            _innerChannelListener = context.BuildInnerChannelListener<TChannel>();
            if (_innerChannelListener == null)
            {
                throw new InvalidOperationException(
                    "InterceptingChannelListener requires an inner IChannelListener.");
            }
        }

        public ChannelMessageInterceptor Interceptor
        {
            get { return _interceptor; }
        }

        public override Uri Uri
        {
            get
            {
                return _innerChannelListener.Uri;
            }
        }

        public override T GetProperty<T>()
        {
            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            return _innerChannelListener.GetProperty<T>();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            _innerChannelListener.Open(timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerChannelListener.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            _innerChannelListener.EndOpen(result);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            _innerChannelListener.Close(timeout);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerChannelListener.BeginClose(timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            _innerChannelListener.EndClose(result);
        }

        protected override void OnAbort()
        {
            _innerChannelListener.Abort();
        }

        protected override TChannel OnAcceptChannel(TimeSpan timeout)
        {
            TChannel innerChannel = _innerChannelListener.AcceptChannel(timeout);
            return WrapChannel(innerChannel);
        }

        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerChannelListener.BeginAcceptChannel(timeout, callback, state);
        }

        protected override TChannel OnEndAcceptChannel(IAsyncResult result)
        {
            TChannel innerChannel = _innerChannelListener.EndAcceptChannel(result);
            return WrapChannel(innerChannel);
        }

        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            return _innerChannelListener.WaitForChannel(timeout);
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerChannelListener.BeginWaitForChannel(timeout, callback, state);
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            return _innerChannelListener.EndWaitForChannel(result);
        }

        private TChannel WrapChannel(TChannel innerChannel)
        {
            if (innerChannel == null)
            {
                return null;
            }

            if (typeof(TChannel) == typeof(IInputChannel))
            {
                return (TChannel)(object)new InterceptingInputChannel<IInputChannel>(this, this.Interceptor, (IInputChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IReplyChannel))
            {
                return (TChannel)(object)new InterceptingReplyChannel(this, (IReplyChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                return (TChannel)(object)new InterceptingDuplexChannel(this, Interceptor, (IDuplexChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IInputSessionChannel))
            {
                return (TChannel)(object)new InterceptingInputSessionChannel(this,
                    (IInputSessionChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IReplySessionChannel))
            {
                return (TChannel)(object)new InterceptingReplySessionChannel(this,
                    (IReplySessionChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return (TChannel)(object)new InterceptingDuplexSessionChannel(this, Interceptor,
                    (IDuplexSessionChannel)innerChannel);
            }

            // Cannot wrap this channel.
            return innerChannel;
        }

        private class InterceptingReplyChannel : InterceptingChannelBase<IReplyChannel>, IReplyChannel
        {
            public InterceptingReplyChannel(
                InterceptingChannelListener<TChannel> listener, IReplyChannel innerChannel)
                : base(listener, listener.Interceptor, innerChannel)
            {
                // empty
            }

            public EndpointAddress LocalAddress
            {
                get
                {
                    return this.InnerChannel.LocalAddress;
                }
            }

            public RequestContext ReceiveRequest()
            {
                return ReceiveRequest(DefaultReceiveTimeout);
            }

            public RequestContext ReceiveRequest(TimeSpan timeout)
            {
                RequestContext requestContext;
                while (true)
                {
                    requestContext = this.InnerChannel.ReceiveRequest(timeout);
                    if (ProcessRequestContext(ref requestContext))
                    {
                        break;
                    }
                }

                return requestContext;
            }

            public IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state)
            {
                return BeginReceiveRequest(DefaultReceiveTimeout, callback, state);
            }

            public IAsyncResult BeginReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
            {
                ReceiveRequestAsyncResult result = new ReceiveRequestAsyncResult(this, timeout, callback, state);
                result.Begin();
                return result;
            }

            public RequestContext EndReceiveRequest(IAsyncResult result)
            {
                return ReceiveRequestAsyncResult.End(result);
            }

            public bool TryReceiveRequest(TimeSpan timeout, out RequestContext requestContext)
            {
                bool result;

                while (true)
                {
                    result = this.InnerChannel.TryReceiveRequest(timeout, out requestContext);
                    if (!result || ProcessRequestContext(ref requestContext))
                    {
                        break;
                    }
                }

                return result;
            }

            public IAsyncResult BeginTryReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
            {
                TryReceiveRequestAsyncResult result = new TryReceiveRequestAsyncResult(this, timeout, callback, state);
                result.Begin();
                return result;
            }

            public bool EndTryReceiveRequest(IAsyncResult result, out RequestContext requestContext)
            {
                return TryReceiveRequestAsyncResult.End(result, out requestContext);
            }

            public bool WaitForRequest(TimeSpan timeout)
            {
                return this.InnerChannel.WaitForRequest(timeout);
            }

            public IAsyncResult BeginWaitForRequest(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return this.InnerChannel.BeginWaitForRequest(timeout, callback, state);
            }

            public bool EndWaitForRequest(IAsyncResult result)
            {
                return this.InnerChannel.EndWaitForRequest(result);
            }

            private bool ProcessRequestContext(ref RequestContext requestContext)
            {
                if (requestContext == null)
                {
                    return true;
                }

                Message m = requestContext.RequestMessage;
                Message originalMessage = m;

                this.OnReceive(ref m);
                if (m != null || originalMessage == null)
                {
                    requestContext = new InterceptingRequestContext(this, requestContext);
                }
                else
                {
                    requestContext.Close();
                    requestContext = null;
                }

                return requestContext != null;
            }

            private abstract class ReceiveRequestAsyncResultBase : AsyncResult
            {
                private RequestContext _requestContext;
                private InterceptingReplyChannel _channel;
                private AsyncCallback _onReceive;

                protected ReceiveRequestAsyncResultBase(InterceptingReplyChannel channel,
                    AsyncCallback callback, object state)
                    : base(callback, state)
                {
                    _channel = channel;
                    _onReceive = new AsyncCallback(OnReceive);
                }

                protected RequestContext RequestContext
                {
                    get { return _requestContext; }
                }

                public void Begin()
                {
                    IAsyncResult result = BeginReceiveRequest(_onReceive, null);
                    if (result.CompletedSynchronously)
                    {
                        if (HandleReceiveComplete(result))
                        {
                            base.Complete(true);
                        }
                    }
                }

                protected abstract IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state);
                protected abstract RequestContext EndReceiveRequest(IAsyncResult result);

                private bool HandleReceiveComplete(IAsyncResult result)
                {
                    while (true)
                    {
                        _requestContext = EndReceiveRequest(result);
                        if (_channel.ProcessRequestContext(ref _requestContext))
                        {
                            return true;
                        }

                        // try again
                        result = BeginReceiveRequest(_onReceive, null);
                        if (!result.CompletedSynchronously)
                        {
                            return false;
                        }
                    }
                }

                private void OnReceive(IAsyncResult result)
                {
                    if (result.CompletedSynchronously)
                    {
                        return;
                    }

                    bool completeSelf = false;
                    Exception completeException = null;
                    try
                    {
                        completeSelf = HandleReceiveComplete(result);
                    }
                    catch (Exception e)
                    {
                        completeException = e;
                        completeSelf = true;
                    }

                    if (completeSelf)
                    {
                        base.Complete(false, completeException);
                    }
                }
            }

            private class TryReceiveRequestAsyncResult : ReceiveRequestAsyncResultBase
            {
                private IReplyChannel _innerChannel;
                private TimeSpan _timeout;
                private bool _returnValue;

                public TryReceiveRequestAsyncResult(InterceptingReplyChannel channel, TimeSpan timeout,
                    AsyncCallback callback, object state)
                    : base(channel, callback, state)
                {
                    _innerChannel = channel.InnerChannel;
                    _timeout = timeout;
                }

                protected override IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state)
                {
                    return _innerChannel.BeginTryReceiveRequest(_timeout, callback, state);
                }

                protected override RequestContext EndReceiveRequest(IAsyncResult result)
                {
                    RequestContext requestContext;
                    _returnValue = _innerChannel.EndTryReceiveRequest(result, out requestContext);
                    return requestContext;
                }

                public static bool End(IAsyncResult result, out RequestContext requestContext)
                {
                    TryReceiveRequestAsyncResult thisPtr = AsyncResult.End<TryReceiveRequestAsyncResult>(result);
                    requestContext = thisPtr.RequestContext;
                    return thisPtr._returnValue;
                }
            }

            private class ReceiveRequestAsyncResult : ReceiveRequestAsyncResultBase
            {
                private IReplyChannel _innerChannel;
                private TimeSpan _timeout;

                public ReceiveRequestAsyncResult(InterceptingReplyChannel channel, TimeSpan timeout, AsyncCallback callback, object state)
                    : base(channel, callback, state)
                {
                    _innerChannel = channel.InnerChannel;
                    _timeout = timeout;
                }

                protected override IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state)
                {
                    return _innerChannel.BeginReceiveRequest(_timeout, callback, state);
                }

                protected override RequestContext EndReceiveRequest(IAsyncResult result)
                {
                    return _innerChannel.EndReceiveRequest(result);
                }

                public static RequestContext End(IAsyncResult result)
                {
                    ReceiveRequestAsyncResult thisPtr = AsyncResult.End<ReceiveRequestAsyncResult>(result);
                    return thisPtr.RequestContext;
                }
            }


            private class InterceptingRequestContext : RequestContext
            {
                private InterceptingReplyChannel _channel;
                private RequestContext _innerContext;

                public InterceptingRequestContext(InterceptingReplyChannel channel, RequestContext innerContext)
                {
                    _channel = channel;
                    _innerContext = innerContext;
                }

                public override Message RequestMessage
                {
                    get
                    {
                        return _innerContext.RequestMessage;
                    }
                }

                public override void Abort()
                {
                    _innerContext.Abort();
                }

                public override IAsyncResult BeginReply(Message message, AsyncCallback callback, object state)
                {
                    return BeginReply(message, _channel.DefaultSendTimeout, callback, state);
                }

                public override IAsyncResult BeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
                {
                    Message m = message;
                    this.OnSend(ref m);
                    return _innerContext.BeginReply(m, timeout, callback, state);
                }

                public override void Close()
                {
                    _innerContext.Close();
                }

                public override void Close(TimeSpan timeout)
                {
                    _innerContext.Close(timeout);
                }

                protected override void Dispose(bool disposing)
                {
                    try
                    {
                        if (disposing)
                            ((IDisposable)_innerContext).Dispose();
                    }
                    finally
                    {
                        base.Dispose(disposing);
                    }
                }

                public override void EndReply(IAsyncResult result)
                {
                    _innerContext.EndReply(result);
                }

                private void OnSend(ref Message message)
                {
                    _channel.OnSend(ref message);
                }

                public override void Reply(Message message)
                {
                    Reply(message, _channel.DefaultSendTimeout);
                }

                public override void Reply(Message message, TimeSpan timeout)
                {
                    Message m = message;
                    this.OnSend(ref m);
                    _innerContext.Reply(m, timeout);
                }
            }
        }

        private class InterceptingInputSessionChannel : InterceptingInputChannel<IInputSessionChannel>, IInputSessionChannel
        {
            private IInputSessionChannel _innerSessionChannel;

            public InterceptingInputSessionChannel(
                InterceptingChannelListener<TChannel> listener, IInputSessionChannel innerChannel)
                : base(listener, listener.Interceptor, innerChannel)
            {
                _innerSessionChannel = innerChannel;
            }

            public IInputSession Session
            {
                get
                {
                    return _innerSessionChannel.Session;
                }
            }
        }

        private class InterceptingReplySessionChannel : InterceptingReplyChannel, IReplySessionChannel
        {
            private IReplySessionChannel _innerSessionChannel;

            public InterceptingReplySessionChannel(
                InterceptingChannelListener<TChannel> listener, IReplySessionChannel innerChannel)
                : base(listener, innerChannel)
            {
                _innerSessionChannel = innerChannel;
            }

            public IInputSession Session
            {
                get
                {
                    return _innerSessionChannel.Session;
                }
            }
        }
    }
}
