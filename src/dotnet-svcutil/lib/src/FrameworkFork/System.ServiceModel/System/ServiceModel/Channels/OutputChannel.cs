// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Diagnostics;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    public abstract class OutputChannel : ChannelBase, IOutputChannel
    {
        protected OutputChannel(ChannelManagerBase manager)
            : base(manager)
        {
        }

        public abstract EndpointAddress RemoteAddress { get; }
        public abstract Uri Via { get; }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            return this.BeginSend(message, this.DefaultSendTimeout, callback, state);
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            if (message == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");

            if (timeout < TimeSpan.Zero)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException("timeout", timeout, SRServiceModel.SFxTimeoutOutOfRange0));

            ThrowIfDisposedOrNotOpen();
            AddHeadersTo(message);
            this.EmitTrace(message);
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
            this.Send(message, this.DefaultSendTimeout);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            if (message == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");

            if (timeout < TimeSpan.Zero)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException("timeout", timeout, SRServiceModel.SFxTimeoutOutOfRange0));

            ThrowIfDisposedOrNotOpen();

            AddHeadersTo(message);
            this.EmitTrace(message);
            OnSend(message, timeout);
        }


        private void EmitTrace(Message message)
        {
        }

        protected virtual void AddHeadersTo(Message message)
        {
        }
    }
}
