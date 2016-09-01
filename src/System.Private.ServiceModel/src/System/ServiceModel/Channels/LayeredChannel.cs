// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal abstract class LayeredChannel<TInnerChannel> : ChannelBase
        where TInnerChannel : class, IChannel
    {
        private TInnerChannel _innerChannel;
        private EventHandler _onInnerChannelFaulted;

        protected LayeredChannel(ChannelManagerBase channelManager, TInnerChannel innerChannel)
            : base(channelManager)
        {
            Fx.Assert(innerChannel != null, "innerChannel cannot be null");

            _innerChannel = innerChannel;
            _onInnerChannelFaulted = new EventHandler(OnInnerChannelFaulted);
            _innerChannel.Faulted += _onInnerChannelFaulted;
        }

        protected TInnerChannel InnerChannel
        {
            get { return _innerChannel; }
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

        protected override void OnClosing()
        {
            _innerChannel.Faulted -= _onInnerChannelFaulted;
            base.OnClosing();
        }

        protected override void OnAbort()
        {
            _innerChannel.Abort();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            _innerChannel.Close(timeout);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            _innerChannel.Open(timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        protected internal override Task OnOpenAsync(TimeSpan timeout)
        {
            return ((IAsyncCommunicationObject)_innerChannel).OpenAsync(timeout);
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            return ((IAsyncCommunicationObject)_innerChannel).CloseAsync(timeout);
        }

        private void OnInnerChannelFaulted(object sender, EventArgs e)
        {
            Fault();
        }
    }
}
