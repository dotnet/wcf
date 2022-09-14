// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime;
using System.ServiceModel.Security;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal sealed class SecurityChannelFactory<TChannel> : LayeredChannelFactory<TChannel>
    {
        private SecuritySessionClientSettings<TChannel> _sessionClientSettings;
        private ISecurityCapabilities _securityCapabilities;

        public SecurityChannelFactory(ISecurityCapabilities securityCapabilities, BindingContext context,
            SecuritySessionClientSettings<TChannel> sessionClientSettings)
            : this(securityCapabilities, context, sessionClientSettings.ChannelBuilder, sessionClientSettings.CreateInnerChannelFactory())
        {
            SessionMode = true;
            _sessionClientSettings = sessionClientSettings;
        }

        public SecurityChannelFactory(ISecurityCapabilities securityCapabilities, BindingContext context, ChannelBuilder channelBuilder, SecurityProtocolFactory protocolFactory)
            : this(securityCapabilities, context, channelBuilder, protocolFactory, channelBuilder.BuildChannelFactory<TChannel>())
        {
        }

        public SecurityChannelFactory(ISecurityCapabilities securityCapabilities, BindingContext context, ChannelBuilder channelBuilder, SecurityProtocolFactory protocolFactory, IChannelFactory innerChannelFactory)
            : this(securityCapabilities, context, channelBuilder, innerChannelFactory)
        {
            SecurityProtocolFactory = protocolFactory;
        }

        private SecurityChannelFactory(ISecurityCapabilities securityCapabilities, BindingContext context, ChannelBuilder channelBuilder, IChannelFactory innerChannelFactory)
            : base(context.Binding, innerChannelFactory)
        {
            ChannelBuilder = channelBuilder;
            MessageVersion = context.Binding.MessageVersion;
            _securityCapabilities = securityCapabilities;
        }

        public ChannelBuilder ChannelBuilder { get; }

        public SecurityProtocolFactory SecurityProtocolFactory { get; private set; }

        public SecuritySessionClientSettings<TChannel> SessionClientSettings
        {
            get
            {
                Fx.Assert(SessionMode == true, "SessionClientSettings can only be used if SessionMode == true");
                return _sessionClientSettings;
            }
        }

        public bool SessionMode { get; }

        private bool SupportsDuplex
        {
            get
            {
                ThrowIfProtocolFactoryNotSet();
                return SecurityProtocolFactory.SupportsDuplex;
            }
        }

        private bool SupportsRequestReply
        {
            get
            {
                ThrowIfProtocolFactoryNotSet();
                return SecurityProtocolFactory.SupportsRequestReply;
            }
        }

        public MessageVersion MessageVersion { get; }

        private Task CloseProtocolFactoryAsync(bool aborted, TimeSpan timeout)
        {
            if (SecurityProtocolFactory != null && !SessionMode)
            {
                var factory = SecurityProtocolFactory;
                SecurityProtocolFactory = null;
                return factory.CloseAsync(aborted, timeout);
            }

            return Task.CompletedTask;
        }

        public override T GetProperty<T>()
        {
            if (SessionMode && (typeof(T) == typeof(IChannelSecureConversationSessionSettings)))
            {
                return (T)(object)SessionClientSettings;
            }
            else if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T)(object)_securityCapabilities;
            }

            return base.GetProperty<T>();
        }

        protected override void OnAbort()
        {
            base.OnAbort();
            CloseProtocolFactoryAsync(true, TimeSpan.Zero);
            if (_sessionClientSettings != null)
            {
                _sessionClientSettings.Abort();
            }
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnCloseAsync(timeout).ToApm(callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected internal override async Task OnCloseAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await base.OnCloseAsync(timeout);
            await CloseProtocolFactoryAsync(false, timeoutHelper.RemainingTime());
            if (_sessionClientSettings != null)
            {
                await _sessionClientSettings.CloseAsync(timeoutHelper.RemainingTime());
            }
        }

        protected override void OnClose(TimeSpan timeout)
        {
            OnCloseAsync(timeout).Wait();
        }

        protected override TChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            ThrowIfDisposed();
            if (SessionMode)
            {
                return _sessionClientSettings.OnCreateChannel(address, via);
            }

            if (typeof(TChannel) == typeof(IAsyncOutputChannel) || typeof(TChannel) == typeof(IOutputChannel))
            {
                return (TChannel)(object)new SecurityOutputChannel(this, SecurityProtocolFactory, ((IChannelFactory<IOutputChannel>)InnerChannelFactory).CreateChannel(address, via), address, via);
            }
            else if (typeof(TChannel) == typeof(IAsyncOutputSessionChannel) || typeof(TChannel) == typeof(IOutputSessionChannel))
            {
                return (TChannel)(object)new SecurityOutputSessionChannel(this, SecurityProtocolFactory, ((IChannelFactory<IOutputSessionChannel>)InnerChannelFactory).CreateChannel(address, via), address, via);
            }
            else if (typeof(TChannel) == typeof(IAsyncDuplexChannel) || typeof(TChannel) == typeof(IDuplexChannel))
            {
                return (TChannel)(object)new SecurityDuplexChannel(this, SecurityProtocolFactory, ((IChannelFactory<IDuplexChannel>)InnerChannelFactory).CreateChannel(address, via), address, via);
            }
            else if (typeof(TChannel) == typeof(IAsyncDuplexSessionChannel) || typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return (TChannel)(object)new SecurityDuplexSessionChannel(this, SecurityProtocolFactory, ((IChannelFactory<IDuplexSessionChannel>)InnerChannelFactory).CreateChannel(address, via), address, via);
            }
            else if (typeof(TChannel) == typeof(IAsyncRequestChannel) || typeof(TChannel) == typeof(IRequestChannel))
            {
                return (TChannel)(object)new SecurityRequestChannel(this, SecurityProtocolFactory, ((IChannelFactory<IRequestChannel>)InnerChannelFactory).CreateChannel(address, via), address, via);
            }

            //typeof(TChannel) == typeof(IRequestSessionChannel)
            return (TChannel)(object)new SecurityRequestSessionChannel(this, SecurityProtocolFactory, ((IChannelFactory<IRequestSessionChannel>)InnerChannelFactory).CreateChannel(address, via), address, via);
        }

        protected internal override async Task OnOpenAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await OnOpenCoreAsync(timeoutHelper.RemainingTime());
            await base.OnOpenAsync(timeoutHelper.RemainingTime());
            SetBufferManager();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            OnOpenAsync(timeout).Wait();
        }

        private void SetBufferManager()
        {
            ITransportFactorySettings transportSettings = GetProperty<ITransportFactorySettings>();

            if (transportSettings == null)
            {
                return;
            }

            BufferManager bufferManager = transportSettings.BufferManager;

            if (bufferManager == null)
            {
                return;
            }

            if (SessionMode && SessionClientSettings != null && SessionClientSettings.SessionProtocolFactory != null)
            {
                SessionClientSettings.SessionProtocolFactory.StreamBufferManager = bufferManager;
            }
            else
            {
                ThrowIfProtocolFactoryNotSet();
                SecurityProtocolFactory.StreamBufferManager = bufferManager;
            }
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnOpenAsync(timeout).ToApm(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        private Task OnOpenCoreAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            if (SessionMode)
            {
                return SessionClientSettings.OpenAsync(this, InnerChannelFactory, ChannelBuilder, timeoutHelper.RemainingTime());
            }
            else
            {
                ThrowIfProtocolFactoryNotSet();
                return SecurityProtocolFactory.OpenAsync(true, timeoutHelper.RemainingTime());
            }
        }

        private void ThrowIfProtocolFactoryNotSet()
        {
            if (SecurityProtocolFactory == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SRP.Format(SRP.SecurityProtocolFactoryShouldBeSetBeforeThisOperation)));
            }
        }

        private abstract class ClientSecurityChannel<UChannel> : SecurityChannel<UChannel>
            where UChannel : class, IChannel
        {
            private ChannelParameterCollection _channelParameters;

            protected ClientSecurityChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory,
                UChannel innerChannel, EndpointAddress to, Uri via)
                : base(factory, innerChannel)
            {
                RemoteAddress = to;
                Via = via;
                SecurityProtocolFactory = securityProtocolFactory;
                _channelParameters = new ChannelParameterCollection(this);
            }

            protected SecurityProtocolFactory SecurityProtocolFactory { get; }

            public EndpointAddress RemoteAddress { get; }

            public Uri Via { get; }

            protected bool TryGetSecurityFaultException(Message faultMessage, out Exception faultException)
            {
                faultException = null;
                if (!faultMessage.IsFault)
                {
                    return false;
                }
                MessageFault fault = MessageFault.CreateFault(faultMessage, TransportDefaults.MaxSecurityFaultSize);
                faultException = SecurityUtils.CreateSecurityFaultException(fault);
                return true;
            }

            protected internal override async Task OnOpenAsync(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                EnableChannelBindingSupport();

                SecurityProtocol securityProtocol = SecurityProtocolFactory.CreateSecurityProtocol(
                    RemoteAddress,
                    Via,
                    null,
                    typeof(TChannel) == typeof(IRequestChannel),
                    timeoutHelper.RemainingTime());
                OnProtocolCreationComplete(securityProtocol);
                await SecurityProtocol.OpenAsync(timeoutHelper.RemainingTime());
                await base.OnOpenAsync(timeoutHelper.RemainingTime());
            }

            private void EnableChannelBindingSupport()
            {
                if (SecurityProtocolFactory != null && SecurityProtocolFactory.ExtendedProtectionPolicy != null && SecurityProtocolFactory.ExtendedProtectionPolicy.CustomChannelBinding != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.ExtendedProtectionPolicyCustomChannelBindingNotSupported)));
                }

                // Do not enable channel binding if there is no reason as it sets up chunking mode.
                if ((SecurityUtils.IsChannelBindingDisabled) || (!SecurityUtils.IsSecurityBindingSuitableForChannelBinding(SecurityProtocolFactory.SecurityBindingElement as TransportSecurityBindingElement)))
                {
                    return;
                }

                if (InnerChannel != null)
                {
                    IChannelBindingProvider cbp = InnerChannel.GetProperty<IChannelBindingProvider>();
                    if (cbp != null)
                    {
                        cbp.EnableChannelBindingSupport();
                    }
                }
            }

            private void OnProtocolCreationComplete(SecurityProtocol securityProtocol)
            {
                SecurityProtocol = securityProtocol;
                SecurityProtocol.ChannelParameters = _channelParameters;
            }

            public override T GetProperty<T>()
            {
                if (typeof(T) == typeof(ChannelParameterCollection))
                {
                    return (T)(object)_channelParameters;
                }

                return base.GetProperty<T>();
            }
        }

        private class SecurityOutputChannel : ClientSecurityChannel<IOutputChannel>, IOutputChannel, IAsyncOutputChannel
        {
            public SecurityOutputChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory, IOutputChannel innerChannel, EndpointAddress to, Uri via)
                : base(factory, securityProtocolFactory, innerChannel, to, via)
            {
            }

            public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
            {
                return BeginSend(message, DefaultSendTimeout, callback, state);
            }

            public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            {
                return SendAsync(message, timeout).ToApm(callback, state);
            }

            public void EndSend(IAsyncResult result)
            {
                result.ToApmEnd();
            }

            public Task SendAsync(Message message)
            {
                return SendAsync(message, DefaultSendTimeout);
            }

            public async Task SendAsync(Message message, TimeSpan timeout)
            {
                ThrowIfFaulted();
                ThrowIfDisposedOrNotOpen(message);
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                message = await SecurityProtocol.SecureOutgoingMessageAsync(message, timeoutHelper.RemainingTime());
                if (InnerChannel is IAsyncOutputChannel asyncOutputChannel)
                {
                    await asyncOutputChannel.SendAsync(message, timeoutHelper.RemainingTime());
                }
                else
                {
                    await Task.Factory.FromAsync(InnerChannel.BeginSend, InnerChannel.EndSend, message, timeoutHelper.RemainingTime(), null);
                }
            }

            public void Send(Message message)
            {
                Send(message, DefaultSendTimeout);
            }

            public void Send(Message message, TimeSpan timeout)
            {
                SendAsync(message, timeout).GetAwaiter().GetResult();
            }
        }

        private sealed class SecurityOutputSessionChannel : SecurityOutputChannel, IOutputSessionChannel
        {
            public SecurityOutputSessionChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory, IOutputSessionChannel innerChannel, EndpointAddress to, Uri via)
                : base(factory, securityProtocolFactory, innerChannel, to, via)
            {
            }

            public IOutputSession Session
            {
                get
                {
                    return ((IOutputSessionChannel)InnerChannel).Session;
                }
            }
        }

        private class SecurityRequestChannel : ClientSecurityChannel<IRequestChannel>, IAsyncRequestChannel
        {
            public SecurityRequestChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory, IRequestChannel innerChannel, EndpointAddress to, Uri via)
                : base(factory, securityProtocolFactory, innerChannel, to, via)
            {
            }

            public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
            {
                return BeginRequest(message, DefaultSendTimeout, callback, state);
            }

            public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            {
                return RequestAsyncInternal(message, timeout).ToApm(callback, state);
            }

            public Message EndRequest(IAsyncResult result)
            {
                return result.ToApmEnd<Message>();
            }

            public Message Request(Message message)
            {
                return Request(message, DefaultSendTimeout);
            }

            internal Message ProcessReply(Message reply, SecurityProtocolCorrelationState correlationState, TimeSpan timeout)
            {
                if (reply != null)
                {
                    Message unverifiedMessage = reply;
                    Exception faultException = null;
                    try
                    {
                        SecurityProtocol.VerifyIncomingMessage(ref reply, timeout, correlationState);
                    }
                    catch (MessageSecurityException)
                    {
                        TryGetSecurityFaultException(unverifiedMessage, out faultException);
                        if (faultException == null)
                        {
                            throw;
                        }
                    }
                    if (faultException != null)
                    {
                        Fault(faultException);
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(faultException);
                    }
                }
                return reply;
            }

            public Task<Message> RequestAsync(Message message)
            {
                return RequestAsync(message, DefaultSendTimeout);
            }

            public async Task<Message> RequestAsync(Message message, TimeSpan timeout)
            {
                ThrowIfFaulted();
                ThrowIfDisposedOrNotOpen(message);
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                SecurityProtocolCorrelationState correlationState;
                (correlationState, message) = await SecurityProtocol.SecureOutgoingMessageAsync(message, timeoutHelper.RemainingTime(), null);
                Message reply = await Task.Factory.FromAsync(InnerChannel.BeginRequest, InnerChannel.EndRequest, message, timeoutHelper.RemainingTime(), null);

                return ProcessReply(reply, correlationState, timeoutHelper.RemainingTime());
            }

            private async Task<Message> RequestAsyncInternal(Message message, TimeSpan timeout)
            {
                await TaskHelpers.EnsureDefaultTaskScheduler();
                return await RequestAsync(message, timeout);
            }

            public Message Request(Message message, TimeSpan timeout)
            {
                return RequestAsyncInternal(message, timeout).GetAwaiter().GetResult();
            }
        }

        private sealed class SecurityRequestSessionChannel : SecurityRequestChannel, IAsyncRequestSessionChannel
        {
            public SecurityRequestSessionChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory, IRequestSessionChannel innerChannel, EndpointAddress to, Uri via)
                : base(factory, securityProtocolFactory, innerChannel, to, via)
            {
            }

            public IOutputSession Session
            {
                get
                {
                    return ((IRequestSessionChannel)InnerChannel).Session;
                }
            }
        }

        private class SecurityDuplexChannel : SecurityOutputChannel, IDuplexChannel, IAsyncDuplexChannel
        {
            public SecurityDuplexChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory, IDuplexChannel innerChannel, EndpointAddress to, Uri via)
                : base(factory, securityProtocolFactory, innerChannel, to, via)
            {
            }

            internal IDuplexChannel InnerDuplexChannel
            {
                get { return (IDuplexChannel)InnerChannel; }
            }

            public EndpointAddress LocalAddress
            {
                get
                {
                    return InnerDuplexChannel.LocalAddress;
                }
            }

            internal virtual bool AcceptUnsecuredFaults
            {
                get { return false; }
            }

            public Task<Message> ReceiveAsync()
            {
                return ReceiveAsync(DefaultReceiveTimeout);
            }

            public Task<Message> ReceiveAsync(TimeSpan timeout)
            {
                return InputChannel.HelpReceiveAsync(this, timeout);
            }

            public Message Receive()
            {
                return Receive(DefaultReceiveTimeout);
            }

            public Message Receive(TimeSpan timeout)
            {
                return ReceiveAsync(timeout).GetAwaiter().GetResult();
            }

            public IAsyncResult BeginReceive(AsyncCallback callback, object state)
            {
                return BeginReceive(DefaultReceiveTimeout, callback, state);
            }

            public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return ReceiveAsync(timeout).ToApm(callback, state);
            }

            public Message EndReceive(IAsyncResult result)
            {
                return result.ToApmEnd<Message>();
            }

            public virtual IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return TryReceiveAsync(timeout).ToApm(callback, state);
            }

            public virtual bool EndTryReceive(IAsyncResult result, out Message message)
            {
                bool success;
                (success, message) = result.ToApmEnd<(bool, Message)>();
                return success;
            }

            internal Message ProcessMessage(Message message, TimeSpan timeout)
            {
                if (message == null)
                {
                    return null;
                }
                Message unverifiedMessage = message;
                Exception faultException = null;
                try
                {
                    SecurityProtocol.VerifyIncomingMessage(ref message, timeout);
                }
                catch (MessageSecurityException)
                {
                    TryGetSecurityFaultException(unverifiedMessage, out faultException);
                    if (faultException == null)
                    {
                        throw;
                    }
                }
                if (faultException != null)
                {
                    if (AcceptUnsecuredFaults)
                    {
                        Fault(faultException);
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(faultException);
                }
                return message;
            }

            public async Task<(bool, Message)> TryReceiveAsync(TimeSpan timeout)
            {
                if (DoneReceivingInCurrentState())
                {
                    return (true, null);
                }

                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                bool success;
                Message message;
                if (InnerDuplexChannel is IAsyncDuplexChannel asyncDuplexChannel)
                {
                    (success, message) = await asyncDuplexChannel.TryReceiveAsync(timeoutHelper.RemainingTime());
                }
                else
                {
                    (success, message) = await TaskHelpers.FromAsync<TimeSpan, bool, Message>(InnerDuplexChannel.BeginTryReceive, InnerDuplexChannel.EndTryReceive, timeout, null);
                }
                if (success)
                {
                    message = ProcessMessage(message, timeoutHelper.RemainingTime());
                }

                return (success, message);
            }


            public bool TryReceive(TimeSpan timeout, out Message message)
            {
                bool success;
                (success, message) = TryReceiveAsync(timeout).GetAwaiter().GetResult();
                return success;
            }

            public Task<bool> WaitForMessageAsync(TimeSpan timeout)
            {
                if (InnerDuplexChannel is IAsyncDuplexChannel asyncDuplexChannel)
                {
                    return asyncDuplexChannel.WaitForMessageAsync(timeout);
                }
                else
                {
                    return Task.Factory.FromAsync(InnerDuplexChannel.BeginWaitForMessage, InnerDuplexChannel.EndWaitForMessage, timeout, null);
                }
            }

            public bool WaitForMessage(TimeSpan timeout)
            {
                return InnerDuplexChannel.WaitForMessage(timeout);
            }

            public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return InnerDuplexChannel.BeginWaitForMessage(timeout, callback, state);
            }

            public bool EndWaitForMessage(IAsyncResult result)
            {
                return InnerDuplexChannel.EndWaitForMessage(result);
            }
        }

        private sealed class SecurityDuplexSessionChannel : SecurityDuplexChannel, IDuplexSessionChannel, IAsyncDuplexSessionChannel
        {
            public SecurityDuplexSessionChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory, IDuplexSessionChannel innerChannel, EndpointAddress to, Uri via)
                : base(factory, securityProtocolFactory, innerChannel, to, via)
            {
            }

            IDuplexSession ISessionChannel<IDuplexSession>.Session
            {
                get
                {
                    return ((ISessionChannel<IDuplexSession>)InnerChannel).Session;
                }
            }

            IAsyncDuplexSession ISessionChannel<IAsyncDuplexSession>.Session
            {
                get
                {
                    return ((ISessionChannel<IAsyncDuplexSession>)InnerChannel).Session;
                }
            }

            internal override bool AcceptUnsecuredFaults
            {
                get { return true; }
            }
        }
    }
}
