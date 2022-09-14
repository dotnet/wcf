// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    internal static class ListenerBinder
    {
        internal static IListenerBinder GetBinder(IChannelListener listener, MessageVersion messageVersion)
        {
            IChannelListener<IInputChannel> input = listener as IChannelListener<IInputChannel>;
            if (input != null)
            {
                return new InputListenerBinder(input, messageVersion);
            }

            IChannelListener<IInputSessionChannel> inputSession = listener as IChannelListener<IInputSessionChannel>;
            if (inputSession != null)
            {
                return new InputSessionListenerBinder(inputSession, messageVersion);
            }

            IChannelListener<IReplyChannel> reply = listener as IChannelListener<IReplyChannel>;
            if (reply != null)
            {
                return new ReplyListenerBinder(reply, messageVersion);
            }

            IChannelListener<IReplySessionChannel> replySession = listener as IChannelListener<IReplySessionChannel>;
            if (replySession != null)
            {
                return new ReplySessionListenerBinder(replySession, messageVersion);
            }

            IChannelListener<IDuplexChannel> duplex = listener as IChannelListener<IDuplexChannel>;
            if (duplex != null)
            {
                return new DuplexListenerBinder(duplex, messageVersion);
            }

            IChannelListener<IDuplexSessionChannel> duplexSession = listener as IChannelListener<IDuplexSessionChannel>;
            if (duplexSession != null)
            {
                return new DuplexSessionListenerBinder(duplexSession, messageVersion);
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.UnknownListenerType1, listener.Uri.AbsoluteUri)));
        }

        // ------------------------------------------------------------------------------------------------------------
        // Listener Binders

        internal class DuplexListenerBinder : IListenerBinder
        {
            private IRequestReplyCorrelator _correlator;
            private IChannelListener<IDuplexChannel> _listener;

            internal DuplexListenerBinder(IChannelListener<IDuplexChannel> listener, MessageVersion messageVersion)
            {
                _correlator = new RequestReplyCorrelator();
                _listener = listener;
                MessageVersion = messageVersion;
            }

            public IChannelListener Listener
            {
                get { return _listener; }
            }

            public MessageVersion MessageVersion { get; }

            public IChannelBinder Accept(TimeSpan timeout)
            {
                IDuplexChannel channel = _listener.AcceptChannel(timeout);
                if (channel == null)
                {
                    return null;
                }

                return new DuplexChannelBinder(channel, _correlator, _listener.Uri);
            }

            public IAsyncResult BeginAccept(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return _listener.BeginAcceptChannel(timeout, callback, state);
            }

            public IChannelBinder EndAccept(IAsyncResult result)
            {
                IDuplexChannel channel = _listener.EndAcceptChannel(result);
                if (channel == null)
                {
                    return null;
                }

                return new DuplexChannelBinder(channel, _correlator, _listener.Uri);
            }
        }

        internal class DuplexSessionListenerBinder : IListenerBinder
        {
            private IRequestReplyCorrelator _correlator;
            private IChannelListener<IDuplexSessionChannel> _listener;

            internal DuplexSessionListenerBinder(IChannelListener<IDuplexSessionChannel> listener, MessageVersion messageVersion)
            {
                _correlator = new RequestReplyCorrelator();
                _listener = listener;
                MessageVersion = messageVersion;
            }

            public IChannelListener Listener
            {
                get { return _listener; }
            }

            public MessageVersion MessageVersion { get; }

            public IChannelBinder Accept(TimeSpan timeout)
            {
                IDuplexSessionChannel channel = _listener.AcceptChannel(timeout);
                if (channel == null)
                {
                    return null;
                }

                return new DuplexChannelBinder(channel, _correlator, _listener.Uri);
            }

            public IAsyncResult BeginAccept(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return _listener.BeginAcceptChannel(timeout, callback, state);
            }

            public IChannelBinder EndAccept(IAsyncResult result)
            {
                IDuplexSessionChannel channel = _listener.EndAcceptChannel(result);
                if (channel == null)
                {
                    return null;
                }

                return new DuplexChannelBinder(channel, _correlator, _listener.Uri);
            }
        }

        internal class InputListenerBinder : IListenerBinder
        {
            private IChannelListener<IInputChannel> _listener;

            internal InputListenerBinder(IChannelListener<IInputChannel> listener, MessageVersion messageVersion)
            {
                _listener = listener;
                MessageVersion = messageVersion;
            }

            public IChannelListener Listener
            {
                get { return _listener; }
            }

            public MessageVersion MessageVersion { get; }

            public IChannelBinder Accept(TimeSpan timeout)
            {
                IInputChannel channel = _listener.AcceptChannel(timeout);
                if (channel == null)
                {
                    return null;
                }

                return new InputChannelBinder(channel, _listener.Uri);
            }

            public IAsyncResult BeginAccept(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return _listener.BeginAcceptChannel(timeout, callback, state);
            }

            public IChannelBinder EndAccept(IAsyncResult result)
            {
                IInputChannel channel = _listener.EndAcceptChannel(result);
                if (channel == null)
                {
                    return null;
                }

                return new InputChannelBinder(channel, _listener.Uri);
            }
        }

        internal class InputSessionListenerBinder : IListenerBinder
        {
            private IChannelListener<IInputSessionChannel> _listener;

            internal InputSessionListenerBinder(IChannelListener<IInputSessionChannel> listener, MessageVersion messageVersion)
            {
                _listener = listener;
                MessageVersion = messageVersion;
            }

            public IChannelListener Listener
            {
                get { return _listener; }
            }

            public MessageVersion MessageVersion { get; }

            public IChannelBinder Accept(TimeSpan timeout)
            {
                IInputSessionChannel channel = _listener.AcceptChannel(timeout);
                if (null == channel)
                {
                    return null;
                }

                return new InputChannelBinder(channel, _listener.Uri);
            }

            public IAsyncResult BeginAccept(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return _listener.BeginAcceptChannel(timeout, callback, state);
            }

            public IChannelBinder EndAccept(IAsyncResult result)
            {
                IInputSessionChannel channel = _listener.EndAcceptChannel(result);
                if (channel == null)
                {
                    return null;
                }

                return new InputChannelBinder(channel, _listener.Uri);
            }
        }

        internal class ReplyListenerBinder : IListenerBinder
        {
            private IChannelListener<IReplyChannel> _listener;

            internal ReplyListenerBinder(IChannelListener<IReplyChannel> listener, MessageVersion messageVersion)
            {
                _listener = listener;
                MessageVersion = messageVersion;
            }

            public IChannelListener Listener
            {
                get { return _listener; }
            }

            public MessageVersion MessageVersion { get; }

            public IChannelBinder Accept(TimeSpan timeout)
            {
                IReplyChannel channel = _listener.AcceptChannel(timeout);
                if (channel == null)
                {
                    return null;
                }

                return new ReplyChannelBinder(channel, _listener.Uri);
            }

            public IAsyncResult BeginAccept(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return _listener.BeginAcceptChannel(timeout, callback, state);
            }

            public IChannelBinder EndAccept(IAsyncResult result)
            {
                IReplyChannel channel = _listener.EndAcceptChannel(result);
                if (channel == null)
                {
                    return null;
                }

                return new ReplyChannelBinder(channel, _listener.Uri);
            }
        }

        internal class ReplySessionListenerBinder : IListenerBinder
        {
            private IChannelListener<IReplySessionChannel> _listener;

            internal ReplySessionListenerBinder(IChannelListener<IReplySessionChannel> listener, MessageVersion messageVersion)
            {
                _listener = listener;
                MessageVersion = messageVersion;
            }

            public IChannelListener Listener
            {
                get { return _listener; }
            }

            public MessageVersion MessageVersion { get; }

            public IChannelBinder Accept(TimeSpan timeout)
            {
                IReplySessionChannel channel = _listener.AcceptChannel(timeout);
                if (channel == null)
                {
                    return null;
                }

                return new ReplyChannelBinder(channel, _listener.Uri);
            }

            public IAsyncResult BeginAccept(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return _listener.BeginAcceptChannel(timeout, callback, state);
            }

            public IChannelBinder EndAccept(IAsyncResult result)
            {
                IReplySessionChannel channel = _listener.EndAcceptChannel(result);
                if (channel == null)
                {
                    return null;
                }

                return new ReplyChannelBinder(channel, _listener.Uri);
            }
        }
    }
}
