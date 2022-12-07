// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal abstract class LayeredChannelFactory<TChannel> : ChannelFactoryBase<TChannel>
    {
        public LayeredChannelFactory(IDefaultCommunicationTimeouts timeouts, IChannelFactory innerChannelFactory)
            : base(timeouts)
        {
            InnerChannelFactory = innerChannelFactory;
        }

        protected IChannelFactory InnerChannelFactory { get; }

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

            return InnerChannelFactory.GetProperty<T>();
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return InnerChannelFactory.BeginOpen(timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            InnerChannelFactory.EndOpen(result);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnCloseAsync(timeout).ToApm(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected internal override async Task OnCloseAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await base.OnCloseAsync(timeoutHelper.RemainingTime());
            await InnerChannelFactory.CloseHelperAsync(timeoutHelper.RemainingTime());
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            base.OnClose(timeoutHelper.RemainingTime());
            InnerChannelFactory.Close(timeoutHelper.RemainingTime());
        }

        protected internal override Task OnOpenAsync(TimeSpan timeout)
        {
            return InnerChannelFactory.OpenHelperAsync(timeout);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            InnerChannelFactory.Open(timeout);
        }

        protected override void OnAbort()
        {
            base.OnAbort();
            InnerChannelFactory.Abort();
        }
    }

    internal class LayeredInputChannel : LayeredChannel<IInputChannel>, IAsyncInputChannel
    {
        public LayeredInputChannel(ChannelManagerBase channelManager, IInputChannel innerChannel)
            : base(channelManager, innerChannel)
        {
        }

        public virtual EndpointAddress LocalAddress
        {
            get { return InnerChannel.LocalAddress; }
        }

        private Task InternalOnReceiveAsync(Message message)
        {
            if (message != null)
            {
                return OnReceiveAsync(message);
            }

            return Task.CompletedTask;
        }

        protected virtual Task OnReceiveAsync(Message message)
        {
            return Task.CompletedTask;
        }

        public Message Receive()
        {
            return ReceiveAsync().GetAwaiter().GetResult();
        }

        public async Task<Message> ReceiveAsync(TimeSpan timeout)
        {
            Message message;
            if (InnerChannel is IAsyncInputChannel asyncInputChannel)
            {
                message = await asyncInputChannel.ReceiveAsync(timeout);
            }
            else
            {
                message = await Task.Factory.FromAsync(InnerChannel.BeginReceive, InnerChannel.EndReceive, timeout, null);
            }

            await InternalOnReceiveAsync(message);
            return message;
        }

        public async Task<Message> ReceiveAsync()
        {
            Message message;
            if (InnerChannel is IAsyncInputChannel asyncInputChannel)
            {
                message = await asyncInputChannel.ReceiveAsync();
            }
            else
            {
                message = await Task.Factory.FromAsync(InnerChannel.BeginReceive, InnerChannel.EndReceive, null);
            }

            await InternalOnReceiveAsync(message);
            return message;
        }

        public Message Receive(TimeSpan timeout)
        {
            return ReceiveAsync(timeout).GetAwaiter().GetResult();
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return ReceiveAsync().ToApm(callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return ReceiveAsync(timeout).ToApm(callback, state);
        }

        public Message EndReceive(IAsyncResult result)
        {
            return result.ToApmEnd<Message>();
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return TryReceiveAsync(timeout).ToApm(callback, state);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            bool retVal;
            (retVal, message) = result.ToApmEnd<(bool, Message)>();
            return retVal;
        }

        public async Task<(bool, Message)> TryReceiveAsync(TimeSpan timeout)
        {
            bool retVal;
            Message message;
            if (InnerChannel is IAsyncInputChannel asyncInputChannel)
            {
                (retVal, message) = await asyncInputChannel.TryReceiveAsync(timeout);
            }
            else
            {
                (retVal, message) = await TaskHelpers.FromAsync<TimeSpan, bool, Message>(InnerChannel.BeginTryReceive, InnerChannel.EndTryReceive, timeout, null);
            }

            await InternalOnReceiveAsync(message);
            return (retVal, message);
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            bool retVal;
            (retVal, message) = TryReceiveAsync(timeout).GetAwaiter().GetResult();
            return retVal;
        }

        public Task<bool> WaitForMessageAsync(TimeSpan timeout)
        {
            if (InnerChannel is IAsyncInputChannel asyncInputChannel)
            {
                return asyncInputChannel.WaitForMessageAsync(timeout);
            }
            else
            {
                return Task.Factory.FromAsync(InnerChannel.BeginWaitForMessage, InnerChannel.EndWaitForMessage, timeout, null);
            }
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            return WaitForMessageAsync(timeout).GetAwaiter().GetResult();
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return WaitForMessageAsync(timeout).ToApm(callback, state);
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            return result.ToApmEnd<bool>();
        }
    }

    internal class LayeredDuplexChannel : LayeredInputChannel, IAsyncDuplexChannel
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

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state) => throw ExceptionHelper.PlatformNotSupported();

        protected override void OnEndClose(IAsyncResult result) => throw ExceptionHelper.PlatformNotSupported();

        protected internal override async Task OnCloseAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await _innerOutputChannel.CloseHelperAsync(timeout);
            await base.OnCloseAsync(timeoutHelper.RemainingTime());
        }

        protected override void OnClose(TimeSpan timeout) => throw ExceptionHelper.PlatformNotSupported();

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state) => throw ExceptionHelper.PlatformNotSupported();

        protected override void OnEndOpen(IAsyncResult result) => throw ExceptionHelper.PlatformNotSupported();

        protected internal override async Task OnOpenAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await base.OnOpenAsync(timeoutHelper.RemainingTime());
            await _innerOutputChannel.OpenHelperAsync(timeoutHelper.RemainingTime());
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            base.OnOpen(timeoutHelper.RemainingTime());
            _innerOutputChannel.Open(timeoutHelper.RemainingTime());
        }

        public Task SendAsync(Message message)
        {
            return SendAsync(message, DefaultSendTimeout);
        }

        public Task SendAsync(Message message, TimeSpan timeout)
        {
            if (_innerOutputChannel is IAsyncOutputChannel asyncOutputChannel)
            {
                return asyncOutputChannel.SendAsync(message, timeout);
            }
            else
            {
                return Task.Factory.FromAsync(_innerOutputChannel.BeginSend, _innerOutputChannel.EndSend, message, timeout, null);
            }
        }

        public void Send(Message message)
        {
            Send(message, DefaultSendTimeout);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            SendAsync(message, timeout).GetAwaiter().GetResult();
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return BeginSend(message, DefaultSendTimeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return SendAsync(message, timeout).ToApm(callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        private void OnInnerOutputChannelFaulted(object sender, EventArgs e)
        {
            Fault();
        }
    }
}
