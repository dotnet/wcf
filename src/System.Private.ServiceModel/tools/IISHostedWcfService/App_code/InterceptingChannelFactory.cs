// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.MessageInterceptor
{
    /// <summary>
    /// ChannelFactory that performs message Interception
    /// </summary>
    internal class InterceptingChannelFactory<TChannel>
        : ChannelFactoryBase<TChannel>
    {
        private ChannelMessageInterceptor _interceptor;
        private IChannelFactory<TChannel> _innerChannelFactory;

        public InterceptingChannelFactory(ChannelMessageInterceptor interceptor, BindingContext context)
        {
            _interceptor = interceptor;
            _innerChannelFactory = context.BuildInnerChannelFactory<TChannel>();
            if (_innerChannelFactory == null)
            {
                throw new InvalidOperationException("InterceptingChannelFactory requires an inner IChannelFactory.");
            }
        }

        public ChannelMessageInterceptor Interceptor
        {
            get { return _interceptor; }
        }

        public override T GetProperty<T>()
        {
            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            return _innerChannelFactory.GetProperty<T>();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            _innerChannelFactory.Open(timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerChannelFactory.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            _innerChannelFactory.EndOpen(result);
        }

        protected override void OnAbort()
        {
            base.OnAbort();
            _innerChannelFactory.Abort();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            base.OnClose(timeoutHelper.RemainingTime());
            _innerChannelFactory.Close(timeoutHelper.RemainingTime());
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ChainedAsyncResult(timeout, callback, state, base.OnBeginClose, base.OnEndClose, _innerChannelFactory.BeginClose, _innerChannelFactory.EndClose);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            ChainedAsyncResult.End(result);
        }

        protected override TChannel OnCreateChannel(EndpointAddress to, Uri via)
        {
            TChannel innerChannel = _innerChannelFactory.CreateChannel(to, via);
            if (typeof(TChannel) == typeof(IOutputChannel))
            {
                return (TChannel)(object)new InterceptingOutputChannel(this, (IOutputChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IRequestChannel))
            {
                return (TChannel)(object)new InterceptingRequestChannel(this, (IRequestChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                return (TChannel)(object)new InterceptingDuplexChannel(this, Interceptor, (IDuplexChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IOutputSessionChannel))
            {
                return (TChannel)(object)new InterceptingOutputSessionChannel(this, (IOutputSessionChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IRequestSessionChannel))
            {
                return (TChannel)(object)new InterceptingRequestSessionChannel(this,
                    (IRequestSessionChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return (TChannel)(object)new InterceptingDuplexSessionChannel(this, Interceptor, (IDuplexSessionChannel)innerChannel);
            }

            throw new InvalidOperationException();
        }

        private class InterceptingOutputChannel
            : InterceptingChannelBase<IOutputChannel>, IOutputChannel
        {
            public InterceptingOutputChannel(InterceptingChannelFactory<TChannel> factory, IOutputChannel innerChannel)
                : base(factory, factory.Interceptor, innerChannel)
            {
                // empty
            }

            public EndpointAddress RemoteAddress
            {
                get
                {
                    return this.InnerChannel.RemoteAddress;
                }
            }

            public Uri Via
            {
                get
                {
                    return this.InnerChannel.Via;
                }
            }

            public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
            {
                return BeginSend(message, DefaultSendTimeout, callback, state);
            }

            public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            {
                this.OnSend(ref message);
                return new SendAsyncResult(this, message, timeout, callback, state);
            }

            public void EndSend(IAsyncResult result)
            {
                SendAsyncResult.End(result);
            }

            public void Send(Message message)
            {
                Send(message, DefaultSendTimeout);
            }

            public void Send(Message message, TimeSpan timeout)
            {
                base.OnSend(ref message);

                if (message != null)
                {
                    this.InnerChannel.Send(message, timeout);
                }
            }

            private class SendAsyncResult : AsyncResult
            {
                private IOutputChannel _channel;
                private AsyncCallback _sendCallback = new AsyncCallback(OnSend);

                public SendAsyncResult(IOutputChannel channel, Message message, TimeSpan timeout, AsyncCallback callback, object state)
                    : base(callback, state)
                {
                    if (message != null)
                    {
                        _channel = channel;

                        IAsyncResult sendResult = channel.BeginSend(message, timeout, _sendCallback, this);
                        if (!sendResult.CompletedSynchronously)
                        {
                            return;
                        }

                        CompleteSend(sendResult);
                    }

                    base.Complete(true);
                }

                private void CompleteSend(IAsyncResult result)
                {
                    _channel.EndSend(result);
                }

                private static void OnSend(IAsyncResult result)
                {
                    if (result.CompletedSynchronously)
                    {
                        return;
                    }

                    SendAsyncResult thisPtr = (SendAsyncResult)result.AsyncState;
                    Exception completionException = null;

                    try
                    {
                        thisPtr.CompleteSend(result);
                    }
                    catch (Exception e)
                    {
                        completionException = e;
                    }

                    thisPtr.Complete(false, completionException);
                }

                public static void End(IAsyncResult result)
                {
                    AsyncResult.End<SendAsyncResult>(result);
                }
            }
        }

        private class InterceptingRequestChannel
            : InterceptingChannelBase<IRequestChannel>, IRequestChannel
        {
            public InterceptingRequestChannel(
                InterceptingChannelFactory<TChannel> factory, IRequestChannel innerChannel)
                : base(factory, factory.Interceptor, innerChannel)
            {
                // empty
            }

            public EndpointAddress RemoteAddress
            {
                get
                {
                    return this.InnerChannel.RemoteAddress;
                }
            }

            public Uri Via
            {
                get
                {
                    return this.InnerChannel.Via;
                }
            }

            public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
            {
                return BeginRequest(message, this.DefaultSendTimeout, callback, state);
            }

            public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            {
                base.OnSend(ref message);
                return new RequestAsyncResult(this, message, timeout, callback, state);
            }

            public Message EndRequest(IAsyncResult result)
            {
                Message reply = RequestAsyncResult.End(result);
                this.OnReceive(ref reply);
                return reply;
            }

            public Message Request(Message message)
            {
                return Request(message, this.DefaultSendTimeout);
            }

            public Message Request(Message message, TimeSpan timeout)
            {
                this.OnSend(ref message);
                Message reply = null;
                if (message != null)
                {
                    reply = this.InnerChannel.Request(message);
                }

                this.OnReceive(ref reply);
                return reply;
            }

            private class RequestAsyncResult : AsyncResult
            {
                private Message _replyMessage;
                private InterceptingRequestChannel _channel;
                private AsyncCallback _requestCallback = new AsyncCallback(OnRequest);

                public RequestAsyncResult(InterceptingRequestChannel channel, Message message, TimeSpan timeout, AsyncCallback callback, object state)
                    : base(callback, state)
                {
                    if (message != null)
                    {
                        _channel = channel;

                        IAsyncResult requestResult = channel.InnerChannel.BeginRequest(message, timeout, _requestCallback, this);
                        if (!requestResult.CompletedSynchronously)
                        {
                            return;
                        }

                        CompleteRequest(requestResult);
                    }

                    base.Complete(true);
                }

                private void CompleteRequest(IAsyncResult result)
                {
                    _replyMessage = _channel.InnerChannel.EndRequest(result);
                }

                private static void OnRequest(IAsyncResult result)
                {
                    if (result.CompletedSynchronously)
                    {
                        return;
                    }

                    RequestAsyncResult thisPtr = (RequestAsyncResult)result.AsyncState;
                    Exception completionException = null;

                    try
                    {
                        thisPtr.CompleteRequest(result);
                    }
                    catch (Exception e)
                    {
                        completionException = e;
                    }

                    thisPtr.Complete(false, completionException);
                }

                public static Message End(IAsyncResult result)
                {
                    RequestAsyncResult thisPtr = AsyncResult.End<RequestAsyncResult>(result);
                    return thisPtr._replyMessage;
                }
            }
        }

        private class InterceptingOutputSessionChannel : InterceptingOutputChannel, IOutputSessionChannel
        {
            private IOutputSessionChannel _innerSessionChannel;

            public InterceptingOutputSessionChannel(
                InterceptingChannelFactory<TChannel> factory, IOutputSessionChannel innerChannel)
                : base(factory, innerChannel)
            {
                _innerSessionChannel = innerChannel;
            }

            public IOutputSession Session
            {
                get
                {
                    return _innerSessionChannel.Session;
                }
            }
        }

        private class InterceptingRequestSessionChannel : InterceptingRequestChannel, IRequestSessionChannel
        {
            private IRequestSessionChannel _innerSessionChannel;

            public InterceptingRequestSessionChannel(
                InterceptingChannelFactory<TChannel> factory, IRequestSessionChannel innerChannel)
                : base(factory, innerChannel)
            {
                _innerSessionChannel = innerChannel;
            }

            public IOutputSession Session
            {
                get
                {
                    return _innerSessionChannel.Session;
                }
            }
        }
    }
}
