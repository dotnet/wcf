// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    public abstract class ChannelFactoryBase : ChannelManagerBase, IChannelFactory, IAsyncChannelFactory
    {
        private TimeSpan _closeTimeout = ServiceDefaults.CloseTimeout;
        private TimeSpan _openTimeout = ServiceDefaults.OpenTimeout;
        private TimeSpan _receiveTimeout = ServiceDefaults.ReceiveTimeout;
        private TimeSpan _sendTimeout = ServiceDefaults.SendTimeout;

        protected ChannelFactoryBase()
        {
        }

        protected ChannelFactoryBase(IDefaultCommunicationTimeouts timeouts)
        {
            InitializeTimeouts(timeouts);
        }

        protected override TimeSpan DefaultCloseTimeout
        {
            get { return _closeTimeout; }
        }

        protected override TimeSpan DefaultOpenTimeout
        {
            get { return _openTimeout; }
        }

        protected override TimeSpan DefaultReceiveTimeout
        {
            get { return _receiveTimeout; }
        }

        protected override TimeSpan DefaultSendTimeout
        {
            get { return _sendTimeout; }
        }

        public virtual T GetProperty<T>()
            where T : class
        {
            if (typeof(T) == typeof(IChannelFactory))
            {
                return (T)(object)this;
            }

            return default(T);
        }

        protected override void OnAbort()
        {
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CompletedAsyncResult(callback, state);
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            return TaskHelpers.CompletedTask();
        }

        protected override void OnClose(TimeSpan timeout)
        {
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        private void InitializeTimeouts(IDefaultCommunicationTimeouts timeouts)
        {
            if (timeouts != null)
            {
                _closeTimeout = timeouts.CloseTimeout;
                _openTimeout = timeouts.OpenTimeout;
                _receiveTimeout = timeouts.ReceiveTimeout;
                _sendTimeout = timeouts.SendTimeout;
            }
        }
    }

    public abstract class ChannelFactoryBase<TChannel> : ChannelFactoryBase, IChannelFactory<TChannel>
    {
        private CommunicationObjectManager<IChannel> _channels;

        protected ChannelFactoryBase()
            : this(null)
        {
        }

        protected ChannelFactoryBase(IDefaultCommunicationTimeouts timeouts)
            : base(timeouts)
        {
            _channels = new CommunicationObjectManager<IChannel>(ThisLock);
        }

        public TChannel CreateChannel(EndpointAddress address)
        {
            if (address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(address));
            }

            return InternalCreateChannel(address, address.Uri);
        }

        public TChannel CreateChannel(EndpointAddress address, Uri via)
        {
            if (address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(address));
            }

            if (via == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(via));
            }

            return InternalCreateChannel(address, via);
        }

        private TChannel InternalCreateChannel(EndpointAddress address, Uri via)
        {
            ValidateCreateChannel();
            TChannel channel = OnCreateChannel(address, via);

            bool success = false;

            try
            {
                _channels.Add((IChannel)(object)channel);
                success = true;
            }
            finally
            {
                if (!success)
                {
                    ((IChannel)(object)channel).Abort();
                }
            }

            return channel;
        }

        protected abstract TChannel OnCreateChannel(EndpointAddress address, Uri via);

        protected void ValidateCreateChannel()
        {
            ThrowIfDisposed();
            if (State != CommunicationState.Opened)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.ChannelFactoryCannotBeUsedToCreateChannels, GetType().ToString())));
            }
        }

        protected override void OnAbort()
        {
            IChannel[] currentChannels = _channels.ToArray();
            foreach (IChannel channel in currentChannels)
            {
                channel.Abort();
            }

            _channels.Abort();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            IChannel[] currentChannels = _channels.ToArray();
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            foreach (IChannel channel in currentChannels)
            {
                channel.Close(timeoutHelper.RemainingTime());
            }

            _channels.Close(timeoutHelper.RemainingTime());
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            return OnCloseAsyncInternal(timeout);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnCloseAsync(timeout).ToApm(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        private async Task OnCloseAsyncInternal(TimeSpan timeout)
        {
            IChannel[] currentChannels = _channels.ToArray();
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            foreach (IChannel channel in currentChannels)
            {
                await CloseOtherAsync(channel, timeoutHelper.RemainingTime());
            }

            // CommunicationObjectManager (_channels) is not a CommunicationObject,
            // and it's close method waits for it to not be busy. Calling CloseAsync
            // is the correct option here as there's no need to block this thread
            // waiting on a signal from another thread to notify it's no longer busy.
            await _channels.CloseAsync(timeoutHelper.RemainingTime());
        }
    }
}
