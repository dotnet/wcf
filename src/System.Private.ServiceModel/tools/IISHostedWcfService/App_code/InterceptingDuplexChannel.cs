// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.MessageInterceptor
{
    internal class InterceptingInputChannel<TChannel>
        : InterceptingChannelBase<TChannel>, IInputChannel
        where TChannel : class, IInputChannel
    {
        public InterceptingInputChannel(
            ChannelManagerBase manager, ChannelMessageInterceptor interceptor, TChannel innerChannel)
            : base(manager, interceptor, innerChannel)
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

        private bool ProcessReceivedMessage(ref Message message)
        {
            Message originalMessage = message;
            this.OnReceive(ref message);
            return (message != null || originalMessage == null);
        }

        public Message Receive()
        {
            return Receive(DefaultReceiveTimeout);
        }

        public Message Receive(TimeSpan timeout)
        {
            Message message;
            while (true)
            {
                message = this.InnerChannel.Receive(timeout);
                if (ProcessReceivedMessage(ref message))
                {
                    break;
                }
            }

            return message;
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return BeginReceive(DefaultReceiveTimeout, callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            ReceiveAsyncResult<TChannel> result = new ReceiveAsyncResult<TChannel>(this, timeout, callback, state);
            result.Begin();
            return result;
        }

        public Message EndReceive(IAsyncResult result)
        {
            return ReceiveAsyncResult<TChannel>.End(result);
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            bool result;
            while (true)
            {
                result = this.InnerChannel.TryReceive(timeout, out message);
                if (ProcessReceivedMessage(ref message))
                {
                    break;
                }
            }

            return result;
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            TryReceiveAsyncResult<TChannel> result = new TryReceiveAsyncResult<TChannel>(this, timeout, callback, state);
            result.Begin();
            return result;
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            return TryReceiveAsyncResult<TChannel>.End(result, out message);
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            return this.InnerChannel.WaitForMessage(timeout);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.InnerChannel.BeginWaitForMessage(timeout, callback, state);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return this.InnerChannel.EndWaitForMessage(result);
        }

        private abstract class ReceiveAsyncResultBase<TInputChannel> : AsyncResult
            where TInputChannel : class, IInputChannel
        {
            private Message _message;
            private InterceptingInputChannel<TInputChannel> _channel;
            private AsyncCallback _onReceive;

            protected ReceiveAsyncResultBase(InterceptingInputChannel<TInputChannel> channel,
                AsyncCallback callback, object state)
                : base(callback, state)
            {
                _channel = channel;
                _onReceive = new AsyncCallback(OnReceive);
            }

            protected Message Message
            {
                get { return _message; }
            }

            public void Begin()
            {
                IAsyncResult result = BeginReceive(_onReceive, null);
                if (result.CompletedSynchronously)
                {
                    if (HandleReceiveComplete(result))
                    {
                        base.Complete(true);
                    }
                }
            }

            protected abstract IAsyncResult BeginReceive(AsyncCallback callback, object state);
            protected abstract Message EndReceive(IAsyncResult result);

            private bool HandleReceiveComplete(IAsyncResult result)
            {
                while (true)
                {
                    _message = EndReceive(result);
                    if (_channel.ProcessReceivedMessage(ref _message))
                    {
                        return true;
                    }

                    // try again
                    result = BeginReceive(_onReceive, null);
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

        private class TryReceiveAsyncResult<TInputChannel> : ReceiveAsyncResultBase<TInputChannel>
            where TInputChannel : class, IInputChannel
        {
            private TInputChannel _innerChannel;
            private TimeSpan _timeout;
            private bool _returnValue;

            public TryReceiveAsyncResult(InterceptingInputChannel<TInputChannel> channel, TimeSpan timeout,
                AsyncCallback callback, object state)
                : base(channel, callback, state)
            {
                _innerChannel = channel.InnerChannel;
                _timeout = timeout;
            }

            protected override IAsyncResult BeginReceive(AsyncCallback callback, object state)
            {
                return _innerChannel.BeginTryReceive(_timeout, callback, state);
            }

            protected override Message EndReceive(IAsyncResult result)
            {
                Message message;
                _returnValue = _innerChannel.EndTryReceive(result, out message);
                return message;
            }

            public static bool End(IAsyncResult result, out Message message)
            {
                TryReceiveAsyncResult<TInputChannel> thisPtr = AsyncResult.End<TryReceiveAsyncResult<TInputChannel>>(result);
                message = thisPtr.Message;
                return thisPtr._returnValue;
            }
        }

        private class ReceiveAsyncResult<TInputChannel> : ReceiveAsyncResultBase<TInputChannel>
           where TInputChannel : class, IInputChannel
        {
            private TInputChannel _innerChannel;
            private TimeSpan _timeout;

            public ReceiveAsyncResult(InterceptingInputChannel<TInputChannel> channel, TimeSpan timeout,
                AsyncCallback callback, object state)
                : base(channel, callback, state)
            {
                _innerChannel = channel.InnerChannel;
                _timeout = timeout;
            }

            protected override IAsyncResult BeginReceive(AsyncCallback callback, object state)
            {
                return _innerChannel.BeginReceive(_timeout, callback, state);
            }

            protected override Message EndReceive(IAsyncResult result)
            {
                return _innerChannel.EndReceive(result);
            }

            public static Message End(IAsyncResult result)
            {
                ReceiveAsyncResult<TInputChannel> thisPtr = AsyncResult.End<ReceiveAsyncResult<TInputChannel>>(result);
                return thisPtr.Message;
            }
        }
    }

    internal class InterceptingDuplexChannel
        : InterceptingInputChannel<IDuplexChannel>, IDuplexChannel
    {
        public InterceptingDuplexChannel(
            ChannelManagerBase manager, ChannelMessageInterceptor interceptor, IDuplexChannel innerChannel)
            : base(manager, interceptor, innerChannel)
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

        public void Send(Message message)
        {
            Send(message, DefaultSendTimeout);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            this.OnSend(ref message);
            this.InnerChannel.Send(message, timeout);
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return BeginSend(message, DefaultSendTimeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            this.OnSend(ref message);
            return this.InnerChannel.BeginSend(message, timeout, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            this.InnerChannel.EndSend(result);
        }
    }


    internal class InterceptingDuplexSessionChannel : InterceptingDuplexChannel, IDuplexSessionChannel
    {
        private IDuplexSessionChannel _innerSessionChannel;

        public InterceptingDuplexSessionChannel(
            ChannelManagerBase manager, ChannelMessageInterceptor interceptor, IDuplexSessionChannel innerChannel)
            : base(manager, interceptor, innerChannel)
        {
            _innerSessionChannel = innerChannel;
        }

        public IDuplexSession Session
        {
            get
            {
                return _innerSessionChannel.Session;
            }
        }
    }
}
