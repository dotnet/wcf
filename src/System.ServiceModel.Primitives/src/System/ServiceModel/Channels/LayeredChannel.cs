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
        private EventHandler _onInnerChannelFaulted;

        protected LayeredChannel(ChannelManagerBase channelManager, TInnerChannel innerChannel)
            : base(channelManager)
        {
            Fx.Assert(innerChannel != null, "innerChannel cannot be null");

            InnerChannel = innerChannel;
            _onInnerChannelFaulted = new EventHandler(OnInnerChannelFaulted);
            InnerChannel.Faulted += _onInnerChannelFaulted;
            base.SupportsAsyncOpenClose = true;
        }

        protected TInnerChannel InnerChannel { get; }

        public override T GetProperty<T>()
        {
            T baseProperty = base.GetProperty<T>();
            if (baseProperty != null)
            {
                return baseProperty;
            }

            return InnerChannel.GetProperty<T>();
        }

        protected override void OnClosing()
        {
            InnerChannel.Faulted -= _onInnerChannelFaulted;
            base.OnClosing();
        }

        protected override void OnAbort()
        {
            InnerChannel.Abort();
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            return InnerChannel.CloseHelperAsync(timeout);
        }

        protected override void OnClose(TimeSpan timeout) => throw ExceptionHelper.PlatformNotSupported();

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state) => throw ExceptionHelper.PlatformNotSupported();

        protected override void OnEndClose(IAsyncResult result) => throw ExceptionHelper.PlatformNotSupported();

        protected internal override Task OnOpenAsync(TimeSpan timeout)
        {
            return InnerChannel.OpenHelperAsync(timeout);
        }

        protected override void OnOpen(TimeSpan timeout) => throw ExceptionHelper.PlatformNotSupported();

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state) => throw ExceptionHelper.PlatformNotSupported();

        protected override void OnEndOpen(IAsyncResult result) => throw ExceptionHelper.PlatformNotSupported();

        private void OnInnerChannelFaulted(object sender, EventArgs e)
        {
            Fault();
        }
    }
}
