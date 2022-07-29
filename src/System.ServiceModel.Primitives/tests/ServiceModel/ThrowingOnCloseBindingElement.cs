// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;

public class ThrowingOnCloseBindingElement : BindingElement
{
    private Exception _exception;
    private bool _channelThrows;

    public ThrowingOnCloseBindingElement()
    {
        _exception = new CommunicationException("Unspecified communication exception");
    }

    public ThrowingOnCloseBindingElement(Exception exception, bool channelThrows)
    {
        _exception = exception;
        _channelThrows = channelThrows;
    }

    public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
    {
        return new ThrowingChannelFactory<TChannel>(this, base.BuildChannelFactory<TChannel>(context));
    }

    public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
    {
        return base.CanBuildChannelFactory<TChannel>(context);
    }

    public override BindingElement Clone()
    {
        var clone = new ThrowingOnCloseBindingElement();
        clone._exception = _exception;
        clone._channelThrows = _channelThrows;
        return clone;
    }

    public override T GetProperty<T>(BindingContext context)
    {
        return context.GetInnerProperty<T>();
    }

    private class ThrowingChannelFactory<TChannel> : ChannelFactoryBase<TChannel>
    {
        private readonly IChannelFactory<TChannel> _innerFactory;
        private readonly ThrowingOnCloseBindingElement _parent;

        public ThrowingChannelFactory(ThrowingOnCloseBindingElement parent, IChannelFactory<TChannel> channelFactory)
        {
            _innerFactory = channelFactory;
            _parent = parent;
        }

        public override T GetProperty<T>()
        {
            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            return _innerFactory.GetProperty<T>();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            _innerFactory.Open(timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return ToApm(Task.Factory.FromAsync(_innerFactory.BeginOpen, _innerFactory.EndOpen, timeout, null), callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            ToApmEnd(result);
        }

        protected override void OnAbort()
        {
            base.OnAbort();
            _innerFactory.Abort();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            if (!_parent._channelThrows)
            {
                throw _parent._exception;
            }

            base.OnClose(timeout);
            _innerFactory.Close(timeout);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return ToApm(OnCloseAsyncImpl(timeout), callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            ToApmEnd(result);
        }

        private async Task OnCloseAsyncImpl(TimeSpan timeout)
        {
            if (!_parent._channelThrows)
            {
                throw _parent._exception;
            }

            await Task.Factory.FromAsync(base.OnBeginClose, base.OnEndClose, timeout, null);
            await Task.Factory.FromAsync(_innerFactory.BeginClose, _innerFactory.EndClose, timeout, null);
        }

        protected override TChannel OnCreateChannel(EndpointAddress to, Uri via)
        {
            TChannel innerChannel = _innerFactory.CreateChannel(to, via);
            if (!_parent._channelThrows)
            {
                return innerChannel;
            }
            if (typeof(TChannel) == typeof(IOutputChannel))
            {
                return (TChannel)(object)new ThrowingOutputChannel(this, _parent._exception, (IOutputChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IRequestChannel))
            {
                return (TChannel)(object)new ThrowingRequestChannel(this, _parent._exception, (IRequestChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                return (TChannel)(object)new ThrowingDuplexChannel(this, _parent._exception, (IDuplexChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IOutputSessionChannel))
            {
                return (TChannel)(object)new ThrowingOutputSessionChannel(this, _parent._exception, (IOutputSessionChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IRequestSessionChannel))
            {
                return (TChannel)(object)new ThrowingRequestSessionChannel(this, _parent._exception, (IRequestSessionChannel)innerChannel);
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return (TChannel)(object)new ThrowingDuplexSessionChannel(this, _parent._exception, (IDuplexSessionChannel)innerChannel);
            }

            throw new InvalidOperationException();
        }

        private static Task ToApm(Task task, AsyncCallback callback, object state)
        {
            var tcs = new TaskCompletionSource<bool>(state);
            task.ContinueWith(delegate
            {
                if (task.IsFaulted)
                    tcs.TrySetException(task.Exception.InnerExceptions);
                else if (task.IsCanceled)
                    tcs.TrySetCanceled();
                else
                    tcs.TrySetResult(false);

                if (callback != null)
                    callback(tcs.Task);

            }, CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
            return tcs.Task;
        }

        private static void ToApmEnd(IAsyncResult iar)
        {
            ((Task)iar).GetAwaiter().GetResult();
        }

        private class ThrowingOutputChannel : ThrowingChannelBase<IOutputChannel>, IOutputChannel
        {
            public ThrowingOutputChannel(ThrowingChannelFactory<TChannel> factory, Exception exception, IOutputChannel innerChannel)
                : base(factory, exception, innerChannel)
            {
                // empty
            }

            public EndpointAddress RemoteAddress => InnerChannel.RemoteAddress;
            public Uri Via => InnerChannel.Via;
            public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state) => InnerChannel.BeginSend(message, callback, state);
            public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state) => InnerChannel.BeginSend(message, timeout, callback, state);
            public void EndSend(IAsyncResult result) => InnerChannel.EndSend(result);
            public void Send(Message message) => InnerChannel.Send(message);
            public void Send(Message message, TimeSpan timeout) => InnerChannel.Send(message, timeout);
        }

        private class ThrowingRequestChannel : ThrowingChannelBase<IRequestChannel>, IRequestChannel
        {
            public ThrowingRequestChannel(ThrowingChannelFactory<TChannel> factory, Exception exception, IRequestChannel innerChannel)
                : base(factory, exception, innerChannel)
            {
                // empty
            }

            public EndpointAddress RemoteAddress => InnerChannel.RemoteAddress;
            public Uri Via => InnerChannel.Via;
            public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state) => InnerChannel.BeginRequest(message, callback, state);
            public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state) => InnerChannel.BeginRequest(message, timeout, callback, state);
            public Message EndRequest(IAsyncResult result) => InnerChannel.EndRequest(result);
            public Message Request(Message message) => InnerChannel.Request(message);
            public Message Request(Message message, TimeSpan timeout) => InnerChannel.Request(message, timeout);
        }

        private class ThrowingOutputSessionChannel : ThrowingOutputChannel, IOutputSessionChannel
        {
            private readonly IOutputSessionChannel _innerSessionChannel;

            public ThrowingOutputSessionChannel(
                ThrowingChannelFactory<TChannel> factory, Exception exception, IOutputSessionChannel innerChannel)
                : base(factory, exception, innerChannel)
            {
                _innerSessionChannel = innerChannel;
            }
            public IOutputSession Session => _innerSessionChannel.Session;
        }

        private class ThrowingRequestSessionChannel : ThrowingRequestChannel, IRequestSessionChannel
        {
            private readonly IRequestSessionChannel _innerSessionChannel;

            public ThrowingRequestSessionChannel(
                ThrowingChannelFactory<TChannel> factory, Exception exception, IRequestSessionChannel innerChannel)
                : base(factory, exception, innerChannel)
            {
                _innerSessionChannel = innerChannel;
            }

            public IOutputSession Session => _innerSessionChannel.Session;
        }

        private class ThrowingInputChannel<T> : ThrowingChannelBase<T>, IInputChannel where T : class, IInputChannel
        {
            public ThrowingInputChannel(ChannelManagerBase manager, Exception exception, T innerChannel) : base(manager, exception, innerChannel)
            {
                // empty
            }

            public EndpointAddress LocalAddress => InnerChannel.LocalAddress;
            public Message Receive() => InnerChannel.Receive();
            public Message Receive(TimeSpan timeout) => InnerChannel.Receive(timeout);
            public IAsyncResult BeginReceive(AsyncCallback callback, object state) => InnerChannel.BeginReceive(callback, state);
            public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state) => InnerChannel.BeginReceive(timeout, callback, state);
            public Message EndReceive(IAsyncResult result) => InnerChannel.EndReceive(result);
            public bool TryReceive(TimeSpan timeout, out Message message) => InnerChannel.TryReceive(timeout, out message);
            public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state) => InnerChannel.BeginTryReceive(timeout, callback, state);
            public bool EndTryReceive(IAsyncResult result, out Message message) => InnerChannel.EndTryReceive(result, out message);
            public bool WaitForMessage(TimeSpan timeout) => InnerChannel.WaitForMessage(timeout);
            public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state) => InnerChannel.BeginWaitForMessage(timeout, callback, state);
            public bool EndWaitForMessage(IAsyncResult result) => InnerChannel.EndWaitForMessage(result);
        }

        private class ThrowingDuplexChannel : ThrowingInputChannel<IDuplexChannel>, IDuplexChannel
        {
            public ThrowingDuplexChannel(ChannelManagerBase manager, Exception exception, IDuplexChannel innerChannel) : base(manager, exception, innerChannel)
            {
                // empty
            }

            public EndpointAddress RemoteAddress => InnerChannel.RemoteAddress;
            public Uri Via => InnerChannel.Via;
            public void Send(Message message) => InnerChannel.Send(message);
            public void Send(Message message, TimeSpan timeout) => InnerChannel.Send(message, timeout);
            public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state) => InnerChannel.BeginSend(message, callback, state);
            public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state) => InnerChannel.BeginSend(message, callback, state);
            public void EndSend(IAsyncResult result) => InnerChannel.EndSend(result);
        }

        private class ThrowingDuplexSessionChannel : ThrowingDuplexChannel, IDuplexSessionChannel
        {
            private readonly IDuplexSessionChannel _innerSessionChannel;

            public ThrowingDuplexSessionChannel(ChannelManagerBase manager, Exception exception, IDuplexSessionChannel innerChannel) : base(manager, exception, innerChannel)
            {
                _innerSessionChannel = innerChannel;
            }

            public IDuplexSession Session => _innerSessionChannel.Session;
        }
    }
}
