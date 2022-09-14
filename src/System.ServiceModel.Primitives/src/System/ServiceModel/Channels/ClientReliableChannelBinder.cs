// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Security;
using System.ServiceModel.Diagnostics;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal abstract class ClientReliableChannelBinder<TChannel> : ReliableChannelBinder<TChannel>,
        IClientReliableChannelBinder
        where TChannel : class, IChannel
    {
        private ChannelParameterCollection _channelParameters;
        private IChannelFactory<TChannel> _factory;
        private EndpointAddress _to;

        protected ClientReliableChannelBinder(EndpointAddress to, Uri via, IChannelFactory<TChannel> factory,
            MaskingMode maskingMode, TolerateFaultsMode faultMode, ChannelParameterCollection channelParameters,
            TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
            : base(factory.CreateChannel(to, via), maskingMode, faultMode,
            defaultCloseTimeout, defaultSendTimeout)
        {
            _to = to;
            Via = via;
            _factory = factory;
            _channelParameters = channelParameters ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(channelParameters));
        }

        // The server side must get a message to determine where the channel should go, thus it is
        // pointless to create a channel for the sake of receiving on the client side. Also, since
        // the client side can create channels there receive may enter an infinite loop if open
        // persistently throws.
        protected override bool CanGetChannelForReceive
        {
            get
            {
                return false;
            }
        }

        public override bool CanSendAsynchronously
        {
            get
            {
                return true;
            }
        }

        public override ChannelParameterCollection ChannelParameters
        {
            get
            {
                return _channelParameters;
            }
        }

        protected override bool MustCloseChannel
        {
            get
            {
                return true;
            }
        }

        protected override bool MustOpenChannel
        {
            get
            {
                return true;
            }
        }

        public Uri Via { get; }

        public static IClientReliableChannelBinder CreateBinder(EndpointAddress to, Uri via,
            IChannelFactory<TChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode,
            ChannelParameterCollection channelParameters,
            TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
        {
            Type type = typeof(TChannel);

            if (type == typeof(IDuplexChannel))
            {
                return new DuplexClientReliableChannelBinder(to, via, (IChannelFactory<IDuplexChannel>)(object)factory, maskingMode,
                    channelParameters, defaultCloseTimeout, defaultSendTimeout);
            }
            else if (type == typeof(IDuplexSessionChannel))
            {
                return new DuplexSessionClientReliableChannelBinder(to, via, (IChannelFactory<IDuplexSessionChannel>)(object)factory, maskingMode,
                    faultMode, channelParameters, defaultCloseTimeout, defaultSendTimeout);
            }
            else if (type == typeof(IRequestChannel))
            {
                return new RequestClientReliableChannelBinder(to, via, (IChannelFactory<IRequestChannel>)(object)factory, maskingMode,
                    channelParameters, defaultCloseTimeout, defaultSendTimeout);
            }
            else if (type == typeof(IRequestSessionChannel))
            {
                return new RequestSessionClientReliableChannelBinder(to, via, (IChannelFactory<IRequestSessionChannel>)(object)factory, maskingMode,
                    faultMode, channelParameters, defaultCloseTimeout, defaultSendTimeout);
            }
            else
            {
                throw Fx.AssertAndThrow("ClientReliableChannelBinder supports creation of IDuplexChannel, IDuplexSessionChannel, IRequestChannel, and IRequestSessionChannel only.");
            }
        }

        public Task<bool> EnsureChannelForRequestAsync()
        {
            return Synchronizer.EnsureChannelAsync();
        }

        protected override void OnAbort()
        {
        }

        protected override Task OnCloseAsync(TimeSpan timeout)
        {
            return Task.CompletedTask;
        }

        protected override Task OnOpenAsync(TimeSpan timeout)
        {
            return Task.CompletedTask;
        }

        protected virtual Task<Message> OnRequestAsync(TChannel channel, Message message, TimeSpan timeout,
            MaskingMode maskingMode)
        {
            throw Fx.AssertAndThrow("The derived class does not support the OnRequest operation.");
        }

        public Task<Message> RequestAsync(Message message, TimeSpan timeout)
        {
            return RequestAsync(message, timeout, DefaultMaskingMode);
        }

        public async Task<Message> RequestAsync(Message message, TimeSpan timeout, MaskingMode maskingMode)
        {
            if (!ValidateOutputOperation(message, timeout, maskingMode))
            {
                return null;
            }

            bool autoAborted = false;

            try
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                (bool success, TChannel channel) = await Synchronizer.TryGetChannelForOutputAsync(timeoutHelper.RemainingTime(), maskingMode);

                if (!success)
                {
                    if (!ReliableChannelBinderHelper.MaskHandled(maskingMode))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new TimeoutException(SRP.Format(SRP.TimeoutOnRequest, timeout)));
                    }

                    return null;
                }

                if (channel == null)
                {
                    return null;
                }

                try
                {
                    return await OnRequestAsync(channel, message, timeoutHelper.RemainingTime(),
                        maskingMode);
                }
                finally
                {
                    autoAborted = Synchronizer.Aborting;
                    Synchronizer.ReturnChannel();
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                if (!HandleException(e, maskingMode, autoAborted))
                {
                    throw;
                }
                else
                {
                    return null;
                }
            }
        }

        protected override Task<bool> TryGetChannelAsync(TimeSpan timeout)
        {
            CommunicationState currentState = State;
            TChannel channel = null;

            if ((currentState == CommunicationState.Created)
               || (currentState == CommunicationState.Opening)
               || (currentState == CommunicationState.Opened))
            {
                channel = _factory.CreateChannel(_to, Via);
                if (!Synchronizer.SetChannel(channel))
                {
                    channel.Abort();
                }
            }
            else
            {
                channel = null;
            }

            return Task.FromResult(true);
        }

        private abstract class DuplexClientReliableChannelBinder<TDuplexChannel>
            : ClientReliableChannelBinder<TDuplexChannel>
            where TDuplexChannel : class, IDuplexChannel
        {
            public DuplexClientReliableChannelBinder(EndpointAddress to, Uri via,
                IChannelFactory<TDuplexChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode,
                ChannelParameterCollection channelParameters,
                TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
                : base(to, via, factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout,
                defaultSendTimeout)
            {
            }

            public override EndpointAddress LocalAddress
            {
                get
                {
                    IDuplexChannel channel = Synchronizer.CurrentChannel;
                    if (channel == null)
                    {
                        return null;
                    }
                    else
                    {
                        return channel.LocalAddress;
                    }
                }
            }

            public override EndpointAddress RemoteAddress
            {
                get
                {
                    IDuplexChannel channel = Synchronizer.CurrentChannel;
                    if (channel == null)
                    {
                        return null;
                    }
                    else
                    {
                        return channel.RemoteAddress;
                    }
                }
            }

            protected virtual void OnReadNullMessage()
            {
            }

            protected override Task OnSendAsync(TDuplexChannel channel, Message message,
                TimeSpan timeout)
            {
                if (channel is IAsyncDuplexSessionChannel)
                {
                    return ((IAsyncDuplexSessionChannel)channel).SendAsync(message, timeout);
                }
                else
                {
                    return Task.Factory.FromAsync(channel.BeginSend, channel.EndSend, message, timeout, null);
                }
            }

            protected override async Task<(bool, RequestContext)> OnTryReceiveAsync(TDuplexChannel channel, TimeSpan timeout)
            {
                bool success;
                Message message;
                if (channel is IAsyncDuplexSessionChannel)
                {
                    (success, message) = await ((IAsyncDuplexSessionChannel)channel).TryReceiveAsync(timeout);
                }
                else
                {
                    (success, message) = await TaskHelpers.FromAsync<TimeSpan, bool, Message>(channel.BeginTryReceive, channel.EndTryReceive, timeout, null);
                }

                if (success && message == null)
                {
                    OnReadNullMessage();
                }

                RequestContext requestContext = WrapMessage(message);
                return (success, requestContext);
            }
        }

        private sealed class DuplexClientReliableChannelBinder
            : DuplexClientReliableChannelBinder<IDuplexChannel>
        {
            public DuplexClientReliableChannelBinder(EndpointAddress to, Uri via,
                IChannelFactory<IDuplexChannel> factory, MaskingMode maskingMode,
                ChannelParameterCollection channelParameters,
                TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
                : base(to, via, factory, maskingMode, TolerateFaultsMode.Never, channelParameters,
                defaultCloseTimeout, defaultSendTimeout)
            {
            }

            public override bool HasSession
            {
                get
                {
                    return false;
                }
            }

            public override ISession GetInnerSession()
            {
                return null;
            }

            protected override bool HasSecuritySession(IDuplexChannel channel)
            {
                return false;
            }
        }

        private sealed class DuplexSessionClientReliableChannelBinder
            : DuplexClientReliableChannelBinder<IDuplexSessionChannel>
        {
            public DuplexSessionClientReliableChannelBinder(EndpointAddress to, Uri via,
                IChannelFactory<IDuplexSessionChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode,
                ChannelParameterCollection channelParameters,
                TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
                : base(to, via, factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout,
                defaultSendTimeout)
            {
            }

            public override bool HasSession
            {
                get
                {
                    return true;
                }
            }

            public override ISession GetInnerSession()
            {
                return ((ISessionChannel<IAsyncDuplexSession>)Synchronizer.CurrentChannel).Session;
            }

            protected override Task CloseChannelAsync(IDuplexSessionChannel channel, TimeSpan timeout)
            {
                return ReliableChannelBinderHelper.CloseDuplexSessionChannelAsync(this, channel, timeout);
            }

            protected override bool HasSecuritySession(IDuplexSessionChannel channel)
            {
                return ((ISessionChannel<IAsyncDuplexSession>)channel).Session is ISecuritySession;
            }

            protected override void OnReadNullMessage()
            {
                Synchronizer.OnReadEof();
            }
        }

        private abstract class RequestClientReliableChannelBinder<TRequestChannel>
            : ClientReliableChannelBinder<TRequestChannel>
            where TRequestChannel : class, IRequestChannel
        {
            private InputQueue<Message> _inputMessages;

            public RequestClientReliableChannelBinder(EndpointAddress to, Uri via,
                IChannelFactory<TRequestChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode,
                ChannelParameterCollection channelParameters,
                TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
                : base(to, via, factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout,
                defaultSendTimeout)
            {
            }
            protected void EnqueueMessageIfNotNull(Message message)
            {
                if (message != null)
                {
                    GetInputMessages().EnqueueAndDispatch(message);
                }
            }

            private InputQueue<Message> GetInputMessages()
            {
                lock (ThisLock)
                {
                    if (State == CommunicationState.Created)
                    {
                        throw Fx.AssertAndThrow("The method GetInputMessages() cannot be called when the binder is in the Created state.");
                    }

                    if (State == CommunicationState.Opening)
                    {
                        throw Fx.AssertAndThrow("The method GetInputMessages() cannot be called when the binder is in the Opening state.");
                    }

                    if (_inputMessages == null)
                    {
                        _inputMessages = TraceUtility.CreateInputQueue<Message>();
                    }
                }

                return _inputMessages;
            }

            public override EndpointAddress LocalAddress
            {
                get
                {
                    return EndpointAddress.AnonymousAddress;
                }
            }

            public override EndpointAddress RemoteAddress
            {
                get
                {
                    IRequestChannel channel = Synchronizer.CurrentChannel;
                    if (channel == null)
                    {
                        return null;
                    }
                    else
                    {
                        return channel.RemoteAddress;
                    }
                }
            }

            protected override Task<Message> OnRequestAsync(TRequestChannel channel, Message message,
                TimeSpan timeout, MaskingMode maskingMode)
            {
                if (channel is IAsyncRequestChannel)
                {
                    return ((IAsyncRequestChannel)channel).RequestAsync(message, timeout);
                }
                else
                {
                    return Task.Factory.FromAsync(channel.BeginRequest, channel.EndRequest, message, timeout, null);
                }
            }

            protected override async Task OnSendAsync(TRequestChannel channel, Message message,
                TimeSpan timeout)
            {
                message = await OnRequestAsync(channel, message, timeout, DefaultMaskingMode);
                EnqueueMessageIfNotNull(message);
            }

            protected override void OnShutdown()
            {
                if (_inputMessages != null)
                {
                    _inputMessages.Close();
                }
            }

            public override async Task<(bool, RequestContext)> TryReceiveAsync(TimeSpan timeout)
            {
                (bool success, Message message) = await GetInputMessages().TryDequeueAsync(timeout);
                RequestContext requestContext = WrapMessage(message);
                return (success, requestContext);
            }
        }

        private sealed class RequestClientReliableChannelBinder
           : RequestClientReliableChannelBinder<IRequestChannel>
        {
            public RequestClientReliableChannelBinder(EndpointAddress to, Uri via,
                IChannelFactory<IRequestChannel> factory, MaskingMode maskingMode,
                ChannelParameterCollection channelParameters,
                TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
                : base(to, via, factory, maskingMode, TolerateFaultsMode.Never, channelParameters,
                defaultCloseTimeout, defaultSendTimeout)
            {
            }

            public override bool HasSession
            {
                get
                {
                    return false;
                }
            }

            public override ISession GetInnerSession()
            {
                return null;
            }

            protected override bool HasSecuritySession(IRequestChannel channel)
            {
                return false;
            }
        }

        private sealed class RequestSessionClientReliableChannelBinder
            : RequestClientReliableChannelBinder<IRequestSessionChannel>
        {
            public RequestSessionClientReliableChannelBinder(EndpointAddress to, Uri via,
                IChannelFactory<IRequestSessionChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode,
                ChannelParameterCollection channelParameters,
                TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
                : base(to, via, factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout,
                defaultSendTimeout)
            {
            }

            public override bool HasSession
            {
                get
                {
                    return true;
                }
            }

            public override ISession GetInnerSession()
            {
                return Synchronizer.CurrentChannel.Session;
            }

            protected override bool HasSecuritySession(IRequestSessionChannel channel)
            {
                return channel.Session is ISecuritySession;
            }
        }
    }
}
