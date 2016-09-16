// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal abstract class ServiceChannelFactory : ChannelFactoryBase
    {
        private string _bindingName;
        private List<IChannel> _channelsList;
        private ClientRuntime _clientRuntime;
        private RequestReplyCorrelator _requestReplyCorrelator = new RequestReplyCorrelator();
        private IDefaultCommunicationTimeouts _timeouts;
        private MessageVersion _messageVersion;

        public ServiceChannelFactory(ClientRuntime clientRuntime, Binding binding)
            : base()
        {
            if (clientRuntime == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("clientRuntime");
            }

            _bindingName = binding.Name;
            _channelsList = new List<IChannel>();
            _clientRuntime = clientRuntime;
            _timeouts = new DefaultCommunicationTimeouts(binding);
            _messageVersion = binding.MessageVersion;
        }

        public ClientRuntime ClientRuntime
        {
            get
            {
                this.ThrowIfDisposed();
                return _clientRuntime;
            }
        }

        internal RequestReplyCorrelator RequestReplyCorrelator
        {
            get
            {
                ThrowIfDisposed();
                return _requestReplyCorrelator;
            }
        }

        protected override TimeSpan DefaultCloseTimeout
        {
            get { return _timeouts.CloseTimeout; }
        }

        protected override TimeSpan DefaultReceiveTimeout
        {
            get { return _timeouts.ReceiveTimeout; }
        }

        protected override TimeSpan DefaultOpenTimeout
        {
            get { return _timeouts.OpenTimeout; }
        }

        protected override TimeSpan DefaultSendTimeout
        {
            get { return _timeouts.SendTimeout; }
        }

        public MessageVersion MessageVersion
        {
            get { return _messageVersion; }
        }

        // special overload for security only
        public static ServiceChannelFactory BuildChannelFactory(ChannelBuilder channelBuilder, ClientRuntime clientRuntime)
        {
            if (channelBuilder.CanBuildChannelFactory<IDuplexChannel>())
            {
                return new ServiceChannelFactoryOverDuplex(channelBuilder.BuildChannelFactory<IDuplexChannel>(), clientRuntime,
                    channelBuilder.Binding);
            }
            else if (channelBuilder.CanBuildChannelFactory<IDuplexSessionChannel>())
            {
                return new ServiceChannelFactoryOverDuplexSession(channelBuilder.BuildChannelFactory<IDuplexSessionChannel>(), clientRuntime, channelBuilder.Binding, false);
            }
            else
            {
                return new ServiceChannelFactoryOverRequestSession(channelBuilder.BuildChannelFactory<IRequestSessionChannel>(), clientRuntime, channelBuilder.Binding, false);
            }
        }

        public static ServiceChannelFactory BuildChannelFactory(ServiceEndpoint serviceEndpoint)
        {
            return BuildChannelFactory(serviceEndpoint, false);
        }

        public static ServiceChannelFactory BuildChannelFactory(ServiceEndpoint serviceEndpoint, bool useActiveAutoClose)
        {
            if (serviceEndpoint == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("serviceEndpoint");
            }

            serviceEndpoint.EnsureInvariants();
            serviceEndpoint.ValidateForClient();

            ChannelRequirements requirements;
            ContractDescription contractDescription = serviceEndpoint.Contract;
            ChannelRequirements.ComputeContractRequirements(contractDescription, out requirements);

            BindingParameterCollection parameters;
            ClientRuntime clientRuntime = DispatcherBuilder.BuildProxyBehavior(serviceEndpoint, out parameters);

            Binding binding = serviceEndpoint.Binding;
            Type[] requiredChannels = ChannelRequirements.ComputeRequiredChannels(ref requirements);

            CustomBinding customBinding = new CustomBinding(binding);
            BindingContext context = new BindingContext(customBinding, parameters);
            customBinding = new CustomBinding(context.RemainingBindingElements);
            customBinding.CopyTimeouts(serviceEndpoint.Binding);

            foreach (Type type in requiredChannels)
            {
                if (type == typeof(IOutputChannel) && customBinding.CanBuildChannelFactory<IOutputChannel>(parameters))
                {
                    return new ServiceChannelFactoryOverOutput(customBinding.BuildChannelFactory<IOutputChannel>(parameters), clientRuntime, binding);
                }

                if (type == typeof(IRequestChannel) && customBinding.CanBuildChannelFactory<IRequestChannel>(parameters))
                {
                    return new ServiceChannelFactoryOverRequest(customBinding.BuildChannelFactory<IRequestChannel>(parameters), clientRuntime, binding);
                }

                if (type == typeof(IDuplexChannel) && customBinding.CanBuildChannelFactory<IDuplexChannel>(parameters))
                {
                    if (requirements.usesReply &&
                        binding.CreateBindingElements().Find<TransportBindingElement>().ManualAddressing)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                            SR.CantCreateChannelWithManualAddressing));
                    }

                    return new ServiceChannelFactoryOverDuplex(customBinding.BuildChannelFactory<IDuplexChannel>(parameters), clientRuntime, binding);
                }

                if (type == typeof(IOutputSessionChannel) && customBinding.CanBuildChannelFactory<IOutputSessionChannel>(parameters))
                {
                    return new ServiceChannelFactoryOverOutputSession(customBinding.BuildChannelFactory<IOutputSessionChannel>(parameters), clientRuntime, binding, false);
                }

                if (type == typeof(IRequestSessionChannel) && customBinding.CanBuildChannelFactory<IRequestSessionChannel>(parameters))
                {
                    return new ServiceChannelFactoryOverRequestSession(customBinding.BuildChannelFactory<IRequestSessionChannel>(parameters), clientRuntime, binding, false);
                }

                if (type == typeof(IDuplexSessionChannel) && customBinding.CanBuildChannelFactory<IDuplexSessionChannel>(parameters))
                {
                    if (requirements.usesReply &&
                        binding.CreateBindingElements().Find<TransportBindingElement>().ManualAddressing)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                            SR.CantCreateChannelWithManualAddressing));
                    }

                    return new ServiceChannelFactoryOverDuplexSession(customBinding.BuildChannelFactory<IDuplexSessionChannel>(parameters), clientRuntime, binding, useActiveAutoClose);
                }
            }

            foreach (Type type in requiredChannels)
            {
                // For SessionMode.Allowed or SessionMode.NotAllowed we will accept session-ful variants as well
                if (type == typeof(IOutputChannel) && customBinding.CanBuildChannelFactory<IOutputSessionChannel>(parameters))
                {
                    return new ServiceChannelFactoryOverOutputSession(customBinding.BuildChannelFactory<IOutputSessionChannel>(parameters), clientRuntime, binding, true);
                }

                if (type == typeof(IRequestChannel) && customBinding.CanBuildChannelFactory<IRequestSessionChannel>(parameters))
                {
                    return new ServiceChannelFactoryOverRequestSession(customBinding.BuildChannelFactory<IRequestSessionChannel>(parameters), clientRuntime, binding, true);
                }

                // and for SessionMode.Required, it is possible that the InstanceContextProvider is handling the session management, so 
                // accept datagram variants if that is the case
                if (type == typeof(IRequestSessionChannel) && customBinding.CanBuildChannelFactory<IRequestChannel>(parameters)
                    && customBinding.GetProperty<IContextSessionProvider>(parameters) != null)
                {
                    return new ServiceChannelFactoryOverRequest(customBinding.BuildChannelFactory<IRequestChannel>(parameters), clientRuntime, binding);
                }
            }

            // we put a lot of work into creating a good error message, as this is a common case
            Dictionary<Type, byte> supportedChannels = new Dictionary<Type, byte>();
            if (customBinding.CanBuildChannelFactory<IOutputChannel>(parameters))
            {
                supportedChannels.Add(typeof(IOutputChannel), 0);
            }
            if (customBinding.CanBuildChannelFactory<IRequestChannel>(parameters))
            {
                supportedChannels.Add(typeof(IRequestChannel), 0);
            }
            if (customBinding.CanBuildChannelFactory<IDuplexChannel>(parameters))
            {
                supportedChannels.Add(typeof(IDuplexChannel), 0);
            }
            if (customBinding.CanBuildChannelFactory<IOutputSessionChannel>(parameters))
            {
                supportedChannels.Add(typeof(IOutputSessionChannel), 0);
            }
            if (customBinding.CanBuildChannelFactory<IRequestSessionChannel>(parameters))
            {
                supportedChannels.Add(typeof(IRequestSessionChannel), 0);
            }
            if (customBinding.CanBuildChannelFactory<IDuplexSessionChannel>(parameters))
            {
                supportedChannels.Add(typeof(IDuplexSessionChannel), 0);
            }

            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(ChannelRequirements.CantCreateChannelException(
                supportedChannels.Keys, requiredChannels, binding.Name));
        }

        protected override void OnAbort()
        {
            IChannel channel = null;

            lock (ThisLock)
            {
                channel = (_channelsList.Count > 0) ? _channelsList[_channelsList.Count - 1] : null;
            }

            while (channel != null)
            {
                channel.Abort();

                lock (ThisLock)
                {
                    _channelsList.Remove(channel);
                    channel = (_channelsList.Count > 0) ? _channelsList[_channelsList.Count - 1] : null;
                }
            }
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            while (true)
            {
                int count;
                IChannel channel;
                lock (ThisLock)
                {
                    count = _channelsList.Count;
                    if (count == 0)
                        return;
                    channel = _channelsList[0];
                }
                channel.Close(timeoutHelper.RemainingTime());
            }
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            List<ICommunicationObject> objectList;
            lock (ThisLock)
            {
                objectList = new List<ICommunicationObject>();
                for (int index = 0; index < _channelsList.Count; index++)
                    objectList.Add(_channelsList[index]);
            }
            return new CloseCollectionAsyncResult(timeout, callback, state, objectList);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CloseCollectionAsyncResult.End(result);
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            return OnCloseAsyncInternal(timeout);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            _clientRuntime.LockDownProperties();
        }

        public void ChannelCreated(IChannel channel)
        {
            lock (ThisLock)
            {
                ThrowIfDisposed();
                _channelsList.Add(channel);
            }
        }

        public void ChannelDisposed(IChannel channel)
        {
            lock (ThisLock)
            {
                _channelsList.Remove(channel);
            }
        }

        public virtual ServiceChannel CreateServiceChannel(EndpointAddress address, Uri via)
        {
            IChannelBinder binder = this.CreateInnerChannelBinder(address, via);
            ServiceChannel serviceChannel = new ServiceChannel(this, binder);

            if (binder is DuplexChannelBinder)
            {
                DuplexChannelBinder duplexChannelBinder = binder as DuplexChannelBinder;
                duplexChannelBinder.ChannelHandler = new ChannelHandler(_messageVersion, binder, serviceChannel);
                duplexChannelBinder.DefaultCloseTimeout = this.DefaultCloseTimeout;
                duplexChannelBinder.DefaultSendTimeout = this.DefaultSendTimeout;
                duplexChannelBinder.IdentityVerifier = _clientRuntime.IdentityVerifier;
            }

            return serviceChannel;
        }

        public TChannel CreateChannel<TChannel>(EndpointAddress address)
        {
            return this.CreateChannel<TChannel>(address, null);
        }

        public TChannel CreateChannel<TChannel>(EndpointAddress address, Uri via)
        {
            if (via == null)
            {
                via = this.ClientRuntime.Via;

                if (via == null)
                {
                    via = address.Uri;
                }
            }

            ServiceChannel serviceChannel = this.CreateServiceChannel(address, via);

            serviceChannel.Proxy = CreateProxy<TChannel>(MessageDirection.Input, serviceChannel);

            IClientChannel clientChannel = serviceChannel.Proxy as IClientChannel;
            if (clientChannel == null)
            {
                clientChannel = serviceChannel;
            }

            serviceChannel.ClientRuntime.GetRuntime().InitializeChannel(clientChannel);
            OperationContext current = OperationContext.Current;
            if ((current != null) && (current.InstanceContext != null))
            {
                current.InstanceContext.WmiChannels.Add((IChannel)serviceChannel.Proxy);
            }

            return (TChannel)serviceChannel.Proxy;
        }

        public abstract bool CanCreateChannel<TChannel>();

        internal static object CreateProxy(Type interfaceType, Type proxiedType, MessageDirection direction, ServiceChannel serviceChannel)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        internal static object CreateProxy<TChannel>(MessageDirection direction, ServiceChannel serviceChannel)
        {
            if (!typeof(TChannel).IsInterface())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.SFxChannelFactoryTypeMustBeInterface));
            }

            return ServiceChannelProxy.CreateProxy<TChannel>(direction, serviceChannel);
        }

        internal static ServiceChannel GetServiceChannel(object transparentProxy)
        {
            IChannelBaseProxy cb = transparentProxy as IChannelBaseProxy;
            if (cb != null)
                return cb.GetServiceChannel();

            ServiceChannelProxy proxy = transparentProxy as ServiceChannelProxy;

            if (proxy != null)
                return proxy.GetServiceChannel();
            else
                return null;
        }

        private async Task OnCloseAsyncInternal(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            while (true)
            {
                int count;
                IChannel channel;
                lock (ThisLock)
                {
                    count = _channelsList.Count;
                    if (count == 0)
                        return;
                    channel = _channelsList[0];
                }

                var asyncChannel = channel as IAsyncCommunicationObject;
                if (asyncChannel != null)
                {
                    await asyncChannel.CloseAsync(timeoutHelper.RemainingTime());
                }
                else
                {
                    await Task.Factory.FromAsync(channel.BeginClose, channel.EndClose, TaskCreationOptions.None);
                }
            }
        }

        protected abstract IChannelBinder CreateInnerChannelBinder(EndpointAddress address, Uri via);

        internal abstract class TypedServiceChannelFactory<TChannel> : ServiceChannelFactory
            where TChannel : class, IChannel
        {
            private IChannelFactory<TChannel> _innerChannelFactory;

            protected TypedServiceChannelFactory(IChannelFactory<TChannel> innerChannelFactory,
                ClientRuntime clientRuntime, Binding binding)
                : base(clientRuntime, binding)
            {
                _innerChannelFactory = innerChannelFactory;
            }

            protected IChannelFactory<TChannel> InnerChannelFactory
            {
                get { return _innerChannelFactory; }
            }

            protected override void OnAbort()
            {
                base.OnAbort();
                _innerChannelFactory.Abort();
            }

            protected override void OnOpen(TimeSpan timeout)
            {
                _innerChannelFactory.Open(timeout);
            }

            protected internal override Task OnOpenAsync(TimeSpan timeout)
            {
                return ((IAsyncCommunicationObject)_innerChannelFactory).OpenAsync(timeout);
            }

            protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                //return _innerChannelFactory.BeginOpen(timeout, callback, state);
            }

            protected override void OnEndOpen(IAsyncResult result)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                //_innerChannelFactory.EndOpen(result);
            }

            protected override void OnClose(TimeSpan timeout)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                base.OnClose(timeoutHelper.RemainingTime());
                _innerChannelFactory.Close(timeoutHelper.RemainingTime());
            }


            protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                //return new ChainedAsyncResult(timeout, callback, state, base.OnBeginClose, base.OnEndClose,
                //    _innerChannelFactory.BeginClose, _innerChannelFactory.EndClose);
            }

            protected override void OnEndClose(IAsyncResult result)
            {
                throw ExceptionHelper.PlatformNotSupported();   // Issue #31 in progress
                //ChainedAsyncResult.End(result);
            }

            protected internal override Task OnCloseAsync(TimeSpan timeout)
            {
                return OnCloseAsyncInternal(timeout);
            }

            public override T GetProperty<T>()
            {
                if (typeof(T) == typeof(TypedServiceChannelFactory<TChannel>))
                {
                    return (T)(object)this;
                }

                T baseProperty = base.GetProperty<T>();
                if (baseProperty != null)
                {
                    return baseProperty;
                }

                return _innerChannelFactory.GetProperty<T>();
            }

            private new async Task OnCloseAsyncInternal(TimeSpan timeout)
            {
                await base.OnCloseAsync(timeout);

                IAsyncChannelFactory asyncChannelFactory = _innerChannelFactory as IAsyncChannelFactory;
                if (asyncChannelFactory != null)
                {
                    await asyncChannelFactory.CloseAsync(timeout);
                }
                else
                {
                    await Task.Factory.FromAsync(_innerChannelFactory.BeginClose, _innerChannelFactory.EndClose, TaskCreationOptions.None);
                }
            }
        }

        private class ServiceChannelFactoryOverOutput : TypedServiceChannelFactory<IOutputChannel>
        {
            public ServiceChannelFactoryOverOutput(IChannelFactory<IOutputChannel> innerChannelFactory, ClientRuntime clientRuntime, Binding binding)
                : base(innerChannelFactory, clientRuntime, binding)
            {
            }

            protected override IChannelBinder CreateInnerChannelBinder(EndpointAddress to, Uri via)
            {
                return new OutputChannelBinder(this.InnerChannelFactory.CreateChannel(to, via));
            }

            public override bool CanCreateChannel<TChannel>()
            {
                return (typeof(TChannel) == typeof(IOutputChannel)
                    || typeof(TChannel) == typeof(IRequestChannel));
            }
        }

        private class ServiceChannelFactoryOverDuplex : TypedServiceChannelFactory<IDuplexChannel>
        {
            public ServiceChannelFactoryOverDuplex(IChannelFactory<IDuplexChannel> innerChannelFactory, ClientRuntime clientRuntime, Binding binding)
                : base(innerChannelFactory, clientRuntime, binding)
            {
            }

            protected override IChannelBinder CreateInnerChannelBinder(EndpointAddress to, Uri via)
            {
                return new DuplexChannelBinder(this.InnerChannelFactory.CreateChannel(to, via), this.RequestReplyCorrelator);
            }

            public override bool CanCreateChannel<TChannel>()
            {
                return (typeof(TChannel) == typeof(IOutputChannel)
                    || typeof(TChannel) == typeof(IRequestChannel)
                    || typeof(TChannel) == typeof(IDuplexChannel));
            }
        }

        private class ServiceChannelFactoryOverRequest : TypedServiceChannelFactory<IRequestChannel>
        {
            public ServiceChannelFactoryOverRequest(IChannelFactory<IRequestChannel> innerChannelFactory, ClientRuntime clientRuntime, Binding binding)
                : base(innerChannelFactory, clientRuntime, binding)
            {
            }

            protected override IChannelBinder CreateInnerChannelBinder(EndpointAddress to, Uri via)
            {
                return new RequestChannelBinder(this.InnerChannelFactory.CreateChannel(to, via));
            }

            public override bool CanCreateChannel<TChannel>()
            {
                return (typeof(TChannel) == typeof(IOutputChannel)
                    || typeof(TChannel) == typeof(IRequestChannel));
            }
        }

        internal class ServiceChannelFactoryOverOutputSession : TypedServiceChannelFactory<IOutputSessionChannel>
        {
            private bool _datagramAdapter;
            public ServiceChannelFactoryOverOutputSession(IChannelFactory<IOutputSessionChannel> innerChannelFactory, ClientRuntime clientRuntime, Binding binding, bool datagramAdapter)
                : base(innerChannelFactory, clientRuntime, binding)
            {
                _datagramAdapter = datagramAdapter;
            }

            protected override IChannelBinder CreateInnerChannelBinder(EndpointAddress to, Uri via)
            {
                IOutputChannel channel;
                {
                    channel = this.InnerChannelFactory.CreateChannel(to, via);
                }

                return new OutputChannelBinder(channel);
            }

            public override bool CanCreateChannel<TChannel>()
            {
                return (typeof(TChannel) == typeof(IOutputChannel)
                    || typeof(TChannel) == typeof(IOutputSessionChannel)
                    || typeof(TChannel) == typeof(IRequestChannel)
                    || typeof(TChannel) == typeof(IRequestSessionChannel));
            }
        }

        internal class ServiceChannelFactoryOverDuplexSession : TypedServiceChannelFactory<IDuplexSessionChannel>
        {
            private bool _useActiveAutoClose;

            public ServiceChannelFactoryOverDuplexSession(IChannelFactory<IDuplexSessionChannel> innerChannelFactory, ClientRuntime clientRuntime, Binding binding, bool useActiveAutoClose)
                : base(innerChannelFactory, clientRuntime, binding)
            {
                _useActiveAutoClose = useActiveAutoClose;
            }

            protected override IChannelBinder CreateInnerChannelBinder(EndpointAddress to, Uri via)
            {
                return new DuplexChannelBinder(this.InnerChannelFactory.CreateChannel(to, via), this.RequestReplyCorrelator, _useActiveAutoClose);
            }

            public override bool CanCreateChannel<TChannel>()
            {
                return (typeof(TChannel) == typeof(IOutputChannel)
                    || typeof(TChannel) == typeof(IRequestChannel)
                    || typeof(TChannel) == typeof(IDuplexChannel)
                    || typeof(TChannel) == typeof(IOutputSessionChannel)
                    || typeof(TChannel) == typeof(IRequestSessionChannel)
                    || typeof(TChannel) == typeof(IDuplexSessionChannel));
            }
        }

        internal class ServiceChannelFactoryOverRequestSession : TypedServiceChannelFactory<IRequestSessionChannel>
        {
            private bool _datagramAdapter = false;

            public ServiceChannelFactoryOverRequestSession(IChannelFactory<IRequestSessionChannel> innerChannelFactory, ClientRuntime clientRuntime, Binding binding, bool datagramAdapter)
                : base(innerChannelFactory, clientRuntime, binding)
            {
                _datagramAdapter = datagramAdapter;
            }

            protected override IChannelBinder CreateInnerChannelBinder(EndpointAddress to, Uri via)
            {
                IRequestChannel channel;

                {
                    channel = this.InnerChannelFactory.CreateChannel(to, via);
                }
                return new RequestChannelBinder(channel);
            }

            public override bool CanCreateChannel<TChannel>()
            {
                return (typeof(TChannel) == typeof(IOutputChannel)
                    || typeof(TChannel) == typeof(IOutputSessionChannel)
                    || typeof(TChannel) == typeof(IRequestChannel)
                    || typeof(TChannel) == typeof(IRequestSessionChannel));
            }
        }

        internal class DefaultCommunicationTimeouts : IDefaultCommunicationTimeouts
        {
            private TimeSpan _closeTimeout;
            private TimeSpan _openTimeout;
            private TimeSpan _receiveTimeout;
            private TimeSpan _sendTimeout;

            public DefaultCommunicationTimeouts(IDefaultCommunicationTimeouts timeouts)
            {
                _closeTimeout = timeouts.CloseTimeout;
                _openTimeout = timeouts.OpenTimeout;
                _receiveTimeout = timeouts.ReceiveTimeout;
                _sendTimeout = timeouts.SendTimeout;
            }

            public TimeSpan CloseTimeout
            {
                get { return _closeTimeout; }
            }

            public TimeSpan OpenTimeout
            {
                get { return _openTimeout; }
            }

            public TimeSpan ReceiveTimeout
            {
                get { return _receiveTimeout; }
            }

            public TimeSpan SendTimeout
            {
                get { return _sendTimeout; }
            }
        }
    }
}
