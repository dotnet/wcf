// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class ReliableChannelFactory<TChannel, InnerChannel> : ChannelFactoryBase<TChannel>, IReliableFactorySettings where InnerChannel : class, IChannel
    {
        public ReliableChannelFactory(ReliableSessionBindingElement settings, IChannelFactory<InnerChannel> innerChannelFactory, Binding binding)
            : base(binding)
        {
            AcknowledgementInterval = settings.AcknowledgementInterval;
            FlowControlEnabled = settings.FlowControlEnabled;
            InactivityTimeout = settings.InactivityTimeout;
            MaxPendingChannels = settings.MaxPendingChannels;
            MaxRetryCount = settings.MaxRetryCount;
            MaxTransferWindowSize = settings.MaxTransferWindowSize;
            MessageVersion = binding.MessageVersion;
            Ordered = settings.Ordered;
            ReliableMessagingVersion = settings.ReliableMessagingVersion;

            InnerChannelFactory = innerChannelFactory;
            FaultHelper = new SendFaultHelper(binding.SendTimeout, binding.CloseTimeout);
        }

        public TimeSpan AcknowledgementInterval { get; }
        public FaultHelper FaultHelper { get; }
        public bool FlowControlEnabled { get; }
        public TimeSpan InactivityTimeout { get; }
        protected IChannelFactory<InnerChannel> InnerChannelFactory { get; }
        public int MaxPendingChannels { get; }
        public int MaxRetryCount { get; }
        public MessageVersion MessageVersion { get; }
        public int MaxTransferWindowSize { get; }
        public bool Ordered { get; }
        public ReliableMessagingVersion ReliableMessagingVersion { get; }

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

        public TimeSpan SendTimeout => InternalSendTimeout;

        protected override void OnAbort()
        {
            // Aborting base first to abort channels. Must abort higher channels before aborting lower channels.
            base.OnAbort();
            FaultHelper.Abort();
            InnerChannelFactory.Abort();
        }

        protected internal override Task OnOpenAsync(TimeSpan timeout)
        {
            return InnerChannelFactory.OpenHelperAsync(timeout);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            CommunicationObjectInternal.OnOpen(this, timeout);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return CommunicationObjectInternal.OnBeginOpen(this, timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CommunicationObjectInternal.OnEnd(result);
        }

        protected internal override async Task OnCloseAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            // Closing base first to close channels.  Must close higher channels before closing lower channels.
            await base.OnCloseAsync(timeoutHelper.RemainingTime());
            await FaultHelper.CloseAsync(timeoutHelper.RemainingTime());
            await InnerChannelFactory.CloseHelperAsync(timeoutHelper.RemainingTime());
        }

        protected override void OnClose(TimeSpan timeout)
        {
            CommunicationObjectInternal.OnClose(this, timeout);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return CommunicationObjectInternal.OnBeginClose(this, timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CommunicationObjectInternal.OnEnd(result);
        }

        protected override TChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            LateBoundChannelParameterCollection channelParameters = new LateBoundChannelParameterCollection();

            IClientReliableChannelBinder binder = ClientReliableChannelBinder<InnerChannel>.CreateBinder(address, via,
                InnerChannelFactory, MaskingMode.All,
                TolerateFaultsMode.IfNotSecuritySession, channelParameters, DefaultCloseTimeout,
                DefaultSendTimeout);

            if (typeof(TChannel) == typeof(IOutputSessionChannel))
            {
                if (typeof(InnerChannel) == typeof(IDuplexChannel) || typeof(InnerChannel) == typeof(IDuplexSessionChannel))
                    return (TChannel)(object)new ReliableOutputSessionChannelOverDuplex(this, this, binder, FaultHelper, channelParameters);

                // typeof(InnerChannel) == typeof(IRequestChannel) || typeof(InnerChannel) == typeof(IRequestSessionChannel))
                return (TChannel)(object)new ReliableOutputSessionChannelOverRequest(this, this, binder, FaultHelper, channelParameters);
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return (TChannel)(object)new ClientReliableDuplexSessionChannel(this, this, binder, FaultHelper, channelParameters, WsrmUtilities.NextSequenceId());
            }

            // (typeof(TChannel) == typeof(IRequestSessionChannel)
            return (TChannel)(object)new ReliableRequestSessionChannel(this, this, binder, FaultHelper, channelParameters, WsrmUtilities.NextSequenceId());
        }
    }
}
