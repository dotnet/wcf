// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel.Channels;

namespace Microsoft.Samples.MessageInterceptor
{
    /// <summary>
    /// Base channel class that uses an ChannelMessageInterceptor
    /// </summary>
    internal class InterceptingChannelBase<TChannel> : ChannelBase
        where TChannel : class, IChannel
    {
        private ChannelMessageInterceptor _interceptor;
        private TChannel _innerChannel;

        protected InterceptingChannelBase(
            ChannelManagerBase manager, ChannelMessageInterceptor interceptor, TChannel innerChannel)
            : base(manager)
        {
            if (innerChannel == null)
            {
                throw new ArgumentException("InterceptingChannelBase requires a non-null inner channel.", "innerChannel");
            }

            _interceptor = interceptor;
            _innerChannel = innerChannel;
        }

        protected TChannel InnerChannel
        {
            get
            {
                return _innerChannel;
            }
        }

        public override T GetProperty<T>()
        {
            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            return this.InnerChannel.GetProperty<T>();
        }

        protected override void OnAbort()
        {
            _innerChannel.Abort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerChannel.BeginClose(timeout, callback, state);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _innerChannel.BeginOpen(timeout, callback, state);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            _innerChannel.Close(timeout);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            _innerChannel.EndClose(result);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            _innerChannel.EndOpen(result);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            _innerChannel.Open(timeout);
        }

        protected void OnReceive(ref Message message)
        {
            _interceptor.OnReceive(ref message);
        }

        protected void OnSend(ref Message message)
        {
            _interceptor.OnSend(ref message);
        }
    }
}
