// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal abstract class DuplexChannel : InputQueueChannel<Message>, IDuplexChannel, IAsyncDuplexChannel
    {
        protected DuplexChannel(ChannelManagerBase channelManager, EndpointAddress localAddress) : base(channelManager)
        {
            LocalAddress = localAddress;
        }

        public virtual EndpointAddress LocalAddress { get; }
        public abstract EndpointAddress RemoteAddress { get; }
        public abstract Uri Via { get; }

        public Task SendAsync(Message message)
        {
            return SendAsync(message, DefaultSendTimeout);
        }

        public Task SendAsync(Message message, TimeSpan timeout)
        {
            if (message == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));

            if (timeout < TimeSpan.Zero)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException(nameof(timeout), timeout, SRP.SFxTimeoutOutOfRange0));

            ThrowIfDisposedOrNotOpen();

            AddHeadersTo(message);
            return OnSendAsync(message, timeout);
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return this.BeginSend(message, this.DefaultSendTimeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return SendAsync(message).ToApm(callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(IDuplexChannel))
            {
                return (T)(object)this;
            }

            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            return default(T);
        }

        protected abstract Task OnSendAsync(Message message, TimeSpan timeout);

        public void Send(Message message)
        {
            this.Send(message, this.DefaultSendTimeout);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            TaskHelpers.WaitForCompletionNoSpin(SendAsync(message, timeout));
        }

        protected virtual void AddHeadersTo(Message message)
        {
        }

        public Task<Message> ReceiveAsync()
        {
            return ReceiveAsync(DefaultReceiveTimeout);
        }

        public Task<Message> ReceiveAsync(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException(nameof(timeout), timeout, SRP.SFxTimeoutOutOfRange0));

            this.ThrowPending();
            return InputChannel.HelpReceiveAsync(this, timeout);
        }

        public Message Receive()
        {
            return this.Receive(this.DefaultReceiveTimeout);
        }

        public Message Receive(TimeSpan timeout)
        {
            return TaskHelpers.WaitForCompletion(ReceiveAsync(timeout));
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return this.BeginReceive(this.DefaultReceiveTimeout, callback, state);
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return ReceiveAsync(timeout).ToApm(callback, state);
        }

        public Message EndReceive(IAsyncResult result)
        {
            return result.ToApmEnd<Message>();
        }

        public Task<(bool, Message)> TryReceiveAsync(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException(nameof(timeout), timeout, SRP.SFxTimeoutOutOfRange0));

            this.ThrowPending();
            return base.DequeueAsync(timeout);
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            bool success;
            (success, message) = TaskHelpers.WaitForCompletion(TryReceiveAsync(timeout));
            return success;
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return TryReceiveAsync(timeout).ToApm(callback, state);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            bool success;
            (success, message) = result.ToApmEnd<(bool, Message)>();
            return success;
        }

        public Task<bool> WaitForMessageAsync(TimeSpan timeout)
        {
            if (timeout < TimeSpan.Zero)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException(nameof(timeout), timeout, SRP.SFxTimeoutOutOfRange0));

            this.ThrowPending();
            return base.WaitForItemAsync(timeout);
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            return TaskHelpers.WaitForCompletion(WaitForMessageAsync(timeout));
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
}
