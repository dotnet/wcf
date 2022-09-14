// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal abstract class OutputChannel : ChannelBase, IOutputChannel
    {
        protected OutputChannel(ChannelManagerBase manager)
            : base(manager)
        {
        }

        public abstract EndpointAddress RemoteAddress { get; }
        public abstract Uri Via { get; }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return BeginSend(message, DefaultSendTimeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));
            }

            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException(nameof(timeout), timeout, SR.SFxTimeoutOutOfRange0));
            }

            this.ThrowIfDisposedOrNotOpen();
            AddHeadersTo(message);
            EmitTrace(message);
            return OnSendAsync(message, timeout).ToApm(callback, state);
        }

        public void EndSend(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(IOutputChannel))
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

        protected abstract void OnSend(Message message, TimeSpan timeout);

        protected abstract Task OnSendAsync(Message message, TimeSpan timeout);

        public void Send(Message message)
        {
            Send(message, DefaultSendTimeout);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));
            }

            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException(nameof(timeout), timeout, SR.SFxTimeoutOutOfRange0));
            }

            this.ThrowIfDisposedOrNotOpen();

            AddHeadersTo(message);
            EmitTrace(message);
            OnSend(message, timeout);
        }

        public Task SendAsync(Message message)
        {
            return SendAsync(message, DefaultSendTimeout);
        }

        public Task SendAsync(Message message, TimeSpan timeout)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(message));
            }

            if (timeout < TimeSpan.Zero)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException(nameof(timeout), timeout, SR.SFxTimeoutOutOfRange0));
            }

            this.ThrowIfDisposedOrNotOpen();

            AddHeadersTo(message);
            EmitTrace(message);
            return OnSendAsync(message, timeout);
        }

        private void EmitTrace(Message message)
        {
        }

        protected virtual void AddHeadersTo(Message message)
        {
        }
    }
}
