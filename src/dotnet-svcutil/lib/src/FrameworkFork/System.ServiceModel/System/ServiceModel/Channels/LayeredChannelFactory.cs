// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;

namespace System.ServiceModel.Channels
{
    internal abstract class LayeredChannelFactory<TChannel> : ChannelFactoryBase<TChannel>
    {
        private IChannelFactory _innerChannelFactory;

        public LayeredChannelFactory(IDefaultCommunicationTimeouts timeouts, IChannelFactory innerChannelFactory)
            : base(timeouts)
        {
            _innerChannelFactory = innerChannelFactory;
        }

        protected IChannelFactory InnerChannelFactory
        {
            get { return _innerChannelFactory; }
        }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(IChannelFactory<TChannel>))
            {
                return (T)(object)this;
            }

            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            return _innerChannelFactory.GetProperty<T>();
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerChannelFactory.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            _innerChannelFactory.EndOpen(result);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ChainedCloseAsyncResult(timeout, callback, state, base.OnBeginClose, base.OnEndClose, _innerChannelFactory);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            ChainedCloseAsyncResult.End(result);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            base.OnClose(timeoutHelper.RemainingTime());
            _innerChannelFactory.Close(timeoutHelper.RemainingTime());
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            _innerChannelFactory.Open(timeout);
        }

        protected override void OnAbort()
        {
            base.OnAbort();
            _innerChannelFactory.Abort();
        }
    }

    internal class LayeredInputChannel : LayeredChannel<IInputChannel>, IInputChannel
    {
        public LayeredInputChannel(ChannelManagerBase channelManager, IInputChannel innerChannel)
            : base(channelManager, innerChannel)
        {
        }

        public virtual EndpointAddress LocalAddress
        {
            get { return InnerChannel.LocalAddress; }
        }

        private void InternalOnReceive(Message message)
        {
            if (message != null)
            {
                this.OnReceive(message);
            }
        }

        protected virtual void OnReceive(Message message)
        {
        }

        public Message Receive()
        {
            Message message = InnerChannel.Receive();
            this.InternalOnReceive(message);
            return message;
        }

        public Message Receive(TimeSpan timeout)
        {
            Message message = InnerChannel.Receive(timeout);
            this.InternalOnReceive(message);
            return message;
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return InnerChannel.BeginReceive(callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return InnerChannel.BeginReceive(timeout, callback, state);
        }

        public Message EndReceive(IAsyncResult result)
        {
            Message message = InnerChannel.EndReceive(result);
            this.InternalOnReceive(message);
            return message;
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return InnerChannel.BeginTryReceive(timeout, callback, state);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            bool retVal = InnerChannel.EndTryReceive(result, out message);
            this.InternalOnReceive(message);
            return retVal;
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            bool retVal = InnerChannel.TryReceive(timeout, out message);
            this.InternalOnReceive(message);
            return retVal;
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            return InnerChannel.WaitForMessage(timeout);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return InnerChannel.BeginWaitForMessage(timeout, callback, state);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return InnerChannel.EndWaitForMessage(result);
        }
    }

    internal class LayeredDuplexChannel : LayeredInputChannel, IDuplexChannel
    {
        private IOutputChannel _innerOutputChannel;
        private EndpointAddress _localAddress;
        private EventHandler _onInnerOutputChannelFaulted;

        public LayeredDuplexChannel(ChannelManagerBase channelManager, IInputChannel innerInputChannel, EndpointAddress localAddress, IOutputChannel innerOutputChannel)
            : base(channelManager, innerInputChannel)
        {
            _localAddress = localAddress;
            _innerOutputChannel = innerOutputChannel;
            _onInnerOutputChannelFaulted = new EventHandler(OnInnerOutputChannelFaulted);
            _innerOutputChannel.Faulted += _onInnerOutputChannelFaulted;
        }

        public override EndpointAddress LocalAddress
        {
            get { return _localAddress; }
        }

        public EndpointAddress RemoteAddress
        {
            get { return _innerOutputChannel.RemoteAddress; }
        }

        public Uri Via
        {
            get { return _innerOutputChannel.Via; }
        }

        protected override void OnClosing()
        {
            _innerOutputChannel.Faulted -= _onInnerOutputChannelFaulted;
            base.OnClosing();
        }

        protected override void OnAbort()
        {
            _innerOutputChannel.Abort();
            base.OnAbort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ChainedCloseAsyncResult(timeout, callback, state, base.OnBeginClose, base.OnEndClose, _innerOutputChannel);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            ChainedCloseAsyncResult.End(result);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            _innerOutputChannel.Close(timeoutHelper.RemainingTime());
            base.OnClose(timeoutHelper.RemainingTime());
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ChainedOpenAsyncResult(timeout, callback, state, base.OnBeginOpen, base.OnEndOpen, _innerOutputChannel);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            ChainedOpenAsyncResult.End(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            base.OnOpen(timeoutHelper.RemainingTime());
            _innerOutputChannel.Open(timeoutHelper.RemainingTime());
        }

        public void Send(Message message)
        {
            this.Send(message, this.DefaultSendTimeout);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            _innerOutputChannel.Send(message, timeout);
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return this.BeginSend(message, this.DefaultSendTimeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerOutputChannel.BeginSend(message, timeout, callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            _innerOutputChannel.EndSend(result);
        }

        private void OnInnerOutputChannelFaulted(object sender, EventArgs e)
        {
            this.Fault();
        }
    }
}
