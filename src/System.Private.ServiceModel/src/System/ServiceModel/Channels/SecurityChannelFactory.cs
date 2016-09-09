// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;
using System.ServiceModel;
//using System.ServiceModel.Diagnostics.Application;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using ServiceModelActivity = System.ServiceModel.Diagnostics.ServiceModelActivity;
using TraceUtility = System.ServiceModel.Diagnostics.TraceUtility;

namespace System.ServiceModel.Channels
{
    sealed class SecurityChannelFactory<TChannel>
        : LayeredChannelFactory<TChannel>
    {
        private ChannelBuilder _channelBuilder;
        private SecurityProtocolFactory _securityProtocolFactory;
        private SecuritySessionClientSettings<TChannel> _sessionClientSettings;
        private bool _sessionMode;
        private MessageVersion _messageVersion;
        private ISecurityCapabilities _securityCapabilities;

        public SecurityChannelFactory(ISecurityCapabilities securityCapabilities, BindingContext context,
            SecuritySessionClientSettings<TChannel> sessionClientSettings)
            : this(securityCapabilities, context, sessionClientSettings.ChannelBuilder, sessionClientSettings.CreateInnerChannelFactory())
        {
            _sessionMode = true;
            _sessionClientSettings = sessionClientSettings;
        }

        public SecurityChannelFactory(ISecurityCapabilities securityCapabilities, BindingContext context, ChannelBuilder channelBuilder, SecurityProtocolFactory protocolFactory)
            : this(securityCapabilities, context, channelBuilder, protocolFactory, channelBuilder.BuildChannelFactory<TChannel>())
        {
        }

        public SecurityChannelFactory(ISecurityCapabilities securityCapabilities, BindingContext context, ChannelBuilder channelBuilder, SecurityProtocolFactory protocolFactory, IChannelFactory innerChannelFactory)
            : this(securityCapabilities, context, channelBuilder, innerChannelFactory)
        {
            _securityProtocolFactory = protocolFactory;
        }

        SecurityChannelFactory(ISecurityCapabilities securityCapabilities, BindingContext context, ChannelBuilder channelBuilder, IChannelFactory innerChannelFactory)
            : base(context.Binding, innerChannelFactory)
        {
            _channelBuilder = channelBuilder;
            _messageVersion = context.Binding.MessageVersion;
            _securityCapabilities = securityCapabilities;
        }

        // used by internal test code
        internal SecurityChannelFactory(Binding binding, SecurityProtocolFactory protocolFactory, IChannelFactory innerChannelFactory)
            : base(binding, innerChannelFactory)
        {
            _securityProtocolFactory = protocolFactory;
        }

        public ChannelBuilder ChannelBuilder
        {
            get
            {
                return _channelBuilder;
            }
        }

        public SecurityProtocolFactory SecurityProtocolFactory
        {
            get
            {
                return _securityProtocolFactory;
            }
        }

        public SecuritySessionClientSettings<TChannel> SessionClientSettings
        {
            get
            {
                Fx.Assert(SessionMode == true, "SessionClientSettings can only be used if SessionMode == true");
                return _sessionClientSettings;
            }
        }

        public bool SessionMode
        {
            get
            {
                return _sessionMode;
            }
        }

        bool SupportsDuplex
        {
            get
            {
                ThrowIfProtocolFactoryNotSet();
                return _securityProtocolFactory.SupportsDuplex;
            }
        }

        bool SupportsRequestReply
        {
            get
            {
                ThrowIfProtocolFactoryNotSet();
                return _securityProtocolFactory.SupportsRequestReply;
            }
        }

        public MessageVersion MessageVersion
        {
            get
            {
                return _messageVersion;
            }
        }

        void CloseProtocolFactory(bool aborted, TimeSpan timeout)
        {
            if (_securityProtocolFactory != null && !SessionMode)
            {
                _securityProtocolFactory.Close(aborted, timeout);
                _securityProtocolFactory = null;
            }
        }

        async Task CloseProtocolFactoryAsync(TimeSpan timeout)
        {
            if (_securityProtocolFactory != null && !SessionMode)
            {
                await _securityProtocolFactory.CloseAsync(timeout);
                _securityProtocolFactory = null;
            }
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
            CloseProtocolFactory(true, TimeSpan.Zero);
            if (_sessionClientSettings != null)
            {
                _sessionClientSettings.Abort();
            }
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.PlatformNotSupported("SecurityChannelFactory OnBeginClose not supported.");

            //List<OperationWithTimeoutBeginCallback> begins = new List<OperationWithTimeoutBeginCallback>();
            //List<OperationEndCallback> ends = new List<OperationEndCallback>();
            //begins.Add(new OperationWithTimeoutBeginCallback(base.OnBeginClose));
            //ends.Add(new OperationEndCallback(base.OnEndClose));

            //if (securityProtocolFactory != null && !SessionMode)
            //{
            //    begins.Add(new OperationWithTimeoutBeginCallback(securityProtocolFactory.BeginClose));
            //    ends.Add(new OperationEndCallback(securityProtocolFactory.EndClose));
            //}

            //if (sessionClientSettings != null)
            //{
            //    begins.Add(new OperationWithTimeoutBeginCallback(sessionClientSettings.BeginClose));
            //    ends.Add(new OperationEndCallback(sessionClientSettings.EndClose));
            //}

            //return OperationWithTimeoutComposer.BeginComposeAsyncOperations(timeout, begins.ToArray(), ends.ToArray(), callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            throw ExceptionHelper.PlatformNotSupported("SecurityChannelFactory OnEndClose not supported.");

            // OperationWithTimeoutComposer.EndComposeAsyncOperations(result);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            base.OnClose(timeout);
            CloseProtocolFactory(false, timeoutHelper.RemainingTime());
            if (_sessionClientSettings != null)
            {
                _sessionClientSettings.Close(timeoutHelper.RemainingTime());
            }
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            return OnCloseAsyncInternal(timeout);
        }

        private async Task OnCloseAsyncInternal(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await base.OnCloseAsync(timeout);
            await CloseProtocolFactoryAsync(timeoutHelper.RemainingTime());
            if (_sessionClientSettings != null)
            {
                await _sessionClientSettings.CloseAsync(timeoutHelper.RemainingTime());
            }
        }

        protected override TChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            ThrowIfDisposed();
            if (SessionMode)
            {
                return _sessionClientSettings.OnCreateChannel(address, via);
            }

            if (typeof(TChannel) == typeof(IOutputChannel))
            {
                return (TChannel)(object)new SecurityOutputChannel(this, _securityProtocolFactory, ((IChannelFactory<IOutputChannel>)InnerChannelFactory).CreateChannel(address, via), address, via);
            }
            else if (typeof(TChannel) == typeof(IOutputSessionChannel))
            {
                return (TChannel)(object)new SecurityOutputSessionChannel(this, _securityProtocolFactory, ((IChannelFactory<IOutputSessionChannel>)InnerChannelFactory).CreateChannel(address, via), address, via);
            }
            else if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                return (TChannel)(object)new SecurityDuplexChannel(this, _securityProtocolFactory, ((IChannelFactory<IDuplexChannel>)InnerChannelFactory).CreateChannel(address, via), address, via);
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return (TChannel)(object)new SecurityDuplexSessionChannel(this, _securityProtocolFactory, ((IChannelFactory<IDuplexSessionChannel>)InnerChannelFactory).CreateChannel(address, via), address, via);
            }
            else if (typeof(TChannel) == typeof(IRequestChannel))
            {
                return (TChannel)(object)new SecurityRequestChannel(this, _securityProtocolFactory, ((IChannelFactory<IRequestChannel>)InnerChannelFactory).CreateChannel(address, via), address, via);
            }

            //typeof(TChannel) == typeof(IRequestSessionChannel)
            return (TChannel)(object)new SecurityRequestSessionChannel(this, _securityProtocolFactory, ((IChannelFactory<IRequestSessionChannel>)InnerChannelFactory).CreateChannel(address, via), address, via);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            OnOpenCore(timeoutHelper.RemainingTime());
            base.OnOpen(timeoutHelper.RemainingTime());
            SetBufferManager();
        }

        protected internal override Task OnOpenAsync(TimeSpan timeout)
        {
            return OnOpenAsyncInternal(timeout);
        }

        private async Task OnOpenAsyncInternal(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await OnOpenCoreAsync(timeoutHelper.RemainingTime());
            await base.OnOpenAsync(timeoutHelper.RemainingTime());
            SetBufferManager();
        }

        void SetBufferManager()
        {
            ITransportFactorySettings transportSettings = GetProperty<ITransportFactorySettings>();

            if (transportSettings == null)
                return;

            BufferManager bufferManager = transportSettings.BufferManager;

            if (bufferManager == null)
                return;

            if (SessionMode && SessionClientSettings != null && SessionClientSettings.SessionProtocolFactory != null)
            {
                SessionClientSettings.SessionProtocolFactory.StreamBufferManager = bufferManager;
            }
            else
            {
                ThrowIfProtocolFactoryNotSet();
                _securityProtocolFactory.StreamBufferManager = bufferManager;
            }
        }


        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.PlatformNotSupported(); // Issue #31 in progress
            //return new OperationWithTimeoutAsyncResult(new OperationWithTimeoutCallback(OnOpen), timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            throw ExceptionHelper.PlatformNotSupported(); // Issue #31 in progress
            //OperationWithTimeoutAsyncResult.End(result);
        }

        void OnOpenCore(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            if (SessionMode)
            {
                SessionClientSettings.Open(this, InnerChannelFactory, ChannelBuilder, timeoutHelper.RemainingTime());
            }
            else
            {
                ThrowIfProtocolFactoryNotSet();
                _securityProtocolFactory.Open(true, timeoutHelper.RemainingTime());
            }
        }

        private async Task OnOpenCoreAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            if (SessionMode)
            {
                await SessionClientSettings.OpenAsync(this, InnerChannelFactory, ChannelBuilder, timeoutHelper.RemainingTime());
            }
            else
            {
                ThrowIfProtocolFactoryNotSet();
                await _securityProtocolFactory.OpenAsync(true, timeoutHelper.RemainingTime());
            }
        }

        void ThrowIfDuplexNotSupported()
        {
            if (!SupportsDuplex)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SR.Format(SR.SecurityProtocolFactoryDoesNotSupportDuplex, _securityProtocolFactory)));
            }
        }

        void ThrowIfProtocolFactoryNotSet()
        {
            if (_securityProtocolFactory == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SR.Format(SR.SecurityProtocolFactoryShouldBeSetBeforeThisOperation)));
            }
        }

        void ThrowIfRequestReplyNotSupported()
        {
            if (!SupportsRequestReply)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    SR.Format(SR.SecurityProtocolFactoryDoesNotSupportRequestReply, _securityProtocolFactory)));
            }
        }


        abstract class ClientSecurityChannel<UChannel> : SecurityChannel<UChannel>
            where UChannel : class, IChannel
        {
            private EndpointAddress _to;
            private Uri _via;
            private SecurityProtocolFactory _securityProtocolFactory;
            private ChannelParameterCollection _channelParameters;

            protected ClientSecurityChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory,
                UChannel innerChannel, EndpointAddress to, Uri via)
                : base(factory, innerChannel)
            {
                _to = to;
                _via = via;
                _securityProtocolFactory = securityProtocolFactory;
                _channelParameters = new ChannelParameterCollection(this);
            }

            protected SecurityProtocolFactory SecurityProtocolFactory
            {
                get { return _securityProtocolFactory; }
            }

            public EndpointAddress RemoteAddress
            {
                get { return _to; }
            }

            public Uri Via
            {
                get { return _via; }
            }

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

            protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            {
                throw ExceptionHelper.PlatformNotSupported(); // Issue #31 in progress

                //EnableChannelBindingSupport();

                //return new OpenAsyncResult(this, timeout, callback, state);
            }

            protected override void OnEndOpen(IAsyncResult result)
            {
                throw ExceptionHelper.PlatformNotSupported(); // Issue #31 in progress
                //OpenAsyncResult.End(result);
            }

            protected override void OnOpen(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                EnableChannelBindingSupport();

                SecurityProtocol securityProtocol = SecurityProtocolFactory.CreateSecurityProtocol(
                    _to,
                    Via,
                    null,
                    typeof(TChannel) == typeof(IRequestChannel),
                    timeoutHelper.RemainingTime());
                OnProtocolCreationComplete(securityProtocol);
                SecurityProtocol.Open(timeoutHelper.RemainingTime());
                base.OnOpen(timeoutHelper.RemainingTime());
            }

            protected internal override Task OnOpenAsync(TimeSpan timeout)
            {
                return OnOpenAsyncInternal(timeout);
            }

            private async Task OnOpenAsyncInternal(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

                EnableChannelBindingSupport();

                SecurityProtocol securityProtocol = SecurityProtocolFactory.CreateSecurityProtocol(
                    _to,
                    Via,
                    null,
                    typeof(TChannel) == typeof(IRequestChannel),
                    timeoutHelper.RemainingTime());
                OnProtocolCreationComplete(securityProtocol);
                await SecurityProtocol.OpenAsync(timeoutHelper.RemainingTime());
                await base.OnOpenAsync(timeoutHelper.RemainingTime());
            }

            void EnableChannelBindingSupport()
            {
                if (_securityProtocolFactory != null && _securityProtocolFactory.ExtendedProtectionPolicy != null && _securityProtocolFactory.ExtendedProtectionPolicy.CustomChannelBinding != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SR.Format(SR.ExtendedProtectionPolicyCustomChannelBindingNotSupported)));
                }

                // Do not enable channel binding if there is no reason as it sets up chunking mode.
                if (!SecurityUtils.IsSecurityBindingSuitableForChannelBinding(SecurityProtocolFactory.SecurityBindingElement as TransportSecurityBindingElement))
                    return;

                if (InnerChannel != null)
                {
                    IChannelBindingProvider cbp = InnerChannel.GetProperty<IChannelBindingProvider>();
                    if (cbp != null)
                    {
                        cbp.EnableChannelBindingSupport();
                    }
                }
            }

            void OnProtocolCreationComplete(SecurityProtocol securityProtocol)
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

        class SecurityOutputChannel : ClientSecurityChannel<IOutputChannel>, IOutputChannel
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
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                //ThrowIfFaulted();
                //ThrowIfDisposedOrNotOpen(message);
                //return new OutputChannelSendAsyncResult(message, SecurityProtocol, InnerChannel, timeout, callback, state);
            }

            public void EndSend(IAsyncResult result)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                // OutputChannelSendAsyncResult.End(result);
            }

            public void Send(Message message)
            {
                Send(message, DefaultSendTimeout);
            }

            public void Send(Message message, TimeSpan timeout)
            {
                ThrowIfFaulted();
                ThrowIfDisposedOrNotOpen(message);
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                SecurityProtocol.SecureOutgoingMessage(ref message, timeoutHelper.RemainingTime());
                InnerChannel.Send(message, timeoutHelper.RemainingTime());
            }
        }

        sealed class SecurityOutputSessionChannel : SecurityOutputChannel, IOutputSessionChannel
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

        class SecurityRequestChannel : ClientSecurityChannel<IRequestChannel>, IRequestChannel
        {
            public SecurityRequestChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory, IRequestChannel innerChannel, EndpointAddress to, Uri via)
                : base(factory, securityProtocolFactory, innerChannel, to, via)
            {
            }

            public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issues #31 and #1494 in progress
                //return BeginRequest(message, DefaultSendTimeout, callback, state);
            }

            public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issues #31 and #1494 in progress
                //ThrowIfFaulted();
                //ThrowIfDisposedOrNotOpen(message);
                //return new RequestChannelSendAsyncResult(message, SecurityProtocol, InnerChannel, this, timeout, callback, state);
            }

            public Message EndRequest(IAsyncResult result)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issues #31 and #1494 in progress
                //return RequestChannelSendAsyncResult.End(result);
            }

            public Message Request(Message message)
            {
                return Request(message, DefaultSendTimeout);
            }

            internal Message ProcessReply(Message reply, SecurityProtocolCorrelationState correlationState, TimeSpan timeout)
            {
                if (reply != null)
                {
                    if (DiagnosticUtility.ShouldUseActivity)
                    {
                        ServiceModelActivity replyActivity = TraceUtility.ExtractActivity(reply);
                        if (replyActivity != null &&
                            correlationState != null &&
                            correlationState.Activity != null &&
                            replyActivity.Id != correlationState.Activity.Id)
                        {
                            using (ServiceModelActivity.BoundOperation(replyActivity))
                            {
                                if (null != FxTrace.Trace)
                                {
                                    FxTrace.Trace.TraceTransfer(correlationState.Activity.Id);
                                }
                                replyActivity.Stop();
                            }
                        }
                    }
                    ServiceModelActivity activity = correlationState == null ? null : correlationState.Activity;
                    using (ServiceModelActivity.BoundOperation(activity))
                    {
                        if (DiagnosticUtility.ShouldUseActivity)
                        {
                            TraceUtility.SetActivity(reply, activity);
                        }
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
                }
                return reply;
            }

            public Message Request(Message message, TimeSpan timeout)
            {
                ThrowIfFaulted();
                ThrowIfDisposedOrNotOpen(message);
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                SecurityProtocolCorrelationState correlationState = SecurityProtocol.SecureOutgoingMessage(ref message, timeoutHelper.RemainingTime(), null);
                Message reply = InnerChannel.Request(message, timeoutHelper.RemainingTime());
                return ProcessReply(reply, correlationState, timeoutHelper.RemainingTime());
            }
        }

        sealed class SecurityRequestSessionChannel : SecurityRequestChannel, IRequestSessionChannel
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

        class SecurityDuplexChannel : SecurityOutputChannel, IDuplexChannel
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

            public Message Receive()
            {
                return Receive(DefaultReceiveTimeout);
            }

            public Message Receive(TimeSpan timeout)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                //return InputChannel.HelpReceive(this, timeout);
            }

            public IAsyncResult BeginReceive(AsyncCallback callback, object state)
            {
                return BeginReceive(DefaultReceiveTimeout, callback, state);
            }

            public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                //return InputChannel.HelpBeginReceive(this, timeout, callback, state);
            }

            public Message EndReceive(IAsyncResult result)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                //return InputChannel.HelpEndReceive(result);
            }

            public virtual IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                //if (DoneReceivingInCurrentState())
                //{
                //    throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                //    //return new DoneReceivingAsyncResult(callback, state);
                //}

                //ClientDuplexReceiveMessageAndVerifySecurityAsyncResult result =
                //    new ClientDuplexReceiveMessageAndVerifySecurityAsyncResult(this, InnerDuplexChannel, timeout, callback, state);
                //result.Start();
                //return result;
            }

            public virtual bool EndTryReceive(IAsyncResult result, out Message message)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                //DoneReceivingAsyncResult doneRecevingResult = result as DoneReceivingAsyncResult;
                //if (doneRecevingResult != null)
                //{
                //    return DoneReceivingAsyncResult.End(doneRecevingResult, out message);
                //}

                //return ClientDuplexReceiveMessageAndVerifySecurityAsyncResult.End(result, out message);
            }

            internal Message ProcessMessage(Message message, TimeSpan timeout)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress

                //if (message == null)
                //{
                //    return null;
                //}
                //Message unverifiedMessage = message;
                //Exception faultException = null;
                //try
                //{
                //    SecurityProtocol.VerifyIncomingMessage(ref message, timeout);
                //}
                //catch (MessageSecurityException)
                //{
                //    TryGetSecurityFaultException(unverifiedMessage, out faultException);
                //    if (faultException == null)
                //    {
                //        throw;
                //    }
                //}
                //if (faultException != null)
                //{
                //    if (AcceptUnsecuredFaults)
                //    {
                //        Fault(faultException);
                //    }
                //    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(faultException);
                //}
                //return message;
            }


            public bool TryReceive(TimeSpan timeout, out Message message)
            {
                if (DoneReceivingInCurrentState())
                {
                    message = null;
                    return true;
                }

                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                if (!InnerDuplexChannel.TryReceive(timeoutHelper.RemainingTime(), out message))
                {
                    return false;
                }
                message = ProcessMessage(message, timeoutHelper.RemainingTime());
                return true;
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

        sealed class SecurityDuplexSessionChannel : SecurityDuplexChannel, IDuplexSessionChannel
        {
            public SecurityDuplexSessionChannel(ChannelManagerBase factory, SecurityProtocolFactory securityProtocolFactory, IDuplexSessionChannel innerChannel, EndpointAddress to, Uri via)
                : base(factory, securityProtocolFactory, innerChannel, to, via)
            {
            }

            public IDuplexSession Session
            {
                get
                {
                    return ((IDuplexSessionChannel)InnerChannel).Session;
                }
            }

            internal override bool AcceptUnsecuredFaults
            {
                get { return true; }
            }
        }
    }
}

