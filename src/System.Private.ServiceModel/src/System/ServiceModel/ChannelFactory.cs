// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Security;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public abstract class ChannelFactory : CommunicationObject, IChannelFactory, IDisposable
    {
        private string _configurationName;
        private ClientCredentials _readOnlyClientCredentials;
        private object _openLock = new object();

        //Overload for activation DuplexChannelFactory
        protected ChannelFactory()
            : base()
        {
            TraceUtility.SetEtwProviderId();
            TraceOpenAndClose = true;
        }

        public ClientCredentials Credentials
        {
            get
            {
                if (Endpoint == null)
                {
                    return null;
                }

                if (State == CommunicationState.Created || State == CommunicationState.Opening)
                {
                    return EnsureCredentials(Endpoint);
                }
                else
                {
                    if (_readOnlyClientCredentials == null)
                    {
                        ClientCredentials c = new ClientCredentials();
                        c.MakeReadOnly();
                        _readOnlyClientCredentials = c;
                    }
                    return _readOnlyClientCredentials;
                }
            }
        }

        protected override TimeSpan DefaultCloseTimeout
        {
            get
            {
                if (Endpoint != null && Endpoint.Binding != null)
                {
                    return Endpoint.Binding.CloseTimeout;
                }
                else
                {
                    return ServiceDefaults.CloseTimeout;
                }
            }
        }

        protected override TimeSpan DefaultOpenTimeout
        {
            get
            {
                if (Endpoint != null && Endpoint.Binding != null)
                {
                    return Endpoint.Binding.OpenTimeout;
                }
                else
                {
                    return ServiceDefaults.OpenTimeout;
                }
            }
        }

        public ServiceEndpoint Endpoint { get; private set; }

        internal IChannelFactory InnerFactory { get; private set; }

        // This boolean is used to determine if we should read ahead by a single
        // Message for IDuplexSessionChannels in order to detect null and
        // autoclose the underlying channel in that case.
        // Currently only accessed from the Send activity.
        [Fx.Tag.FriendAccessAllowed("System.ServiceModel.Activities")]
        internal bool UseActiveAutoClose
        {
            get;
            set;
        }

        protected internal void EnsureOpened()
        {
            base.ThrowIfDisposed();
            if (State != CommunicationState.Opened)
            {
                lock (_openLock)
                {
                    if (State != CommunicationState.Opened)
                    {
                        Open();
                    }
                }
            }
        }

        // configurationName can be:
        // 1. null: don't bind any per-endpoint config (load common behaviors only)
        // 2. "*" (wildcard): match any endpoint config provided there's exactly 1
        // 3. anything else (including ""): match the endpoint config with the same name
        protected virtual void ApplyConfiguration(string configurationName)
        {
            // This method is in the public contract but is not supported on CORECLR or NETNATIVE
            if (!String.IsNullOrEmpty(configurationName))
            {
                throw ExceptionHelper.PlatformNotSupported();
            }
        }

        protected abstract ServiceEndpoint CreateDescription();

        internal EndpointAddress CreateEndpointAddress(ServiceEndpoint endpoint)
        {
            if (endpoint.Address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.SFxChannelFactoryEndpointAddressUri));
            }

            return endpoint.Address;
        }

        protected virtual IChannelFactory CreateFactory()
        {
            if (Endpoint == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.SFxChannelFactoryCannotCreateFactoryWithoutDescription));
            }

            if (Endpoint.Binding == null)
            {
                if (_configurationName != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SFxChannelFactoryNoBindingFoundInConfig1, _configurationName)));
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.SFxChannelFactoryNoBindingFoundInConfigOrCode));
                }
            }

            return ServiceChannelFactory.BuildChannelFactory(Endpoint, UseActiveAutoClose);
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        private void EnsureSecurityCredentialsManager(ServiceEndpoint endpoint)
        {
            Fx.Assert(State == CommunicationState.Created || State == CommunicationState.Opening, "");
            if (endpoint.Behaviors.Find<SecurityCredentialsManager>() == null)
            {
                endpoint.Behaviors.Add(new ClientCredentials());
            }
        }

        private ClientCredentials EnsureCredentials(ServiceEndpoint endpoint)
        {
            Fx.Assert(State == CommunicationState.Created || State == CommunicationState.Opening, "");
            ClientCredentials c = endpoint.Behaviors.Find<ClientCredentials>();
            if (c == null)
            {
                c = new ClientCredentials();
                endpoint.Behaviors.Add(c);
            }
            return c;
        }

        public T GetProperty<T>() where T : class
        {
            if (InnerFactory != null)
            {
                return InnerFactory.GetProperty<T>();
            }
            else
            {
                return null;
            }
        }

        internal bool HasDuplexOperations()
        {
            OperationDescriptionCollection operations = Endpoint.Contract.Operations;
            for (int i = 0; i < operations.Count; i++)
            {
                OperationDescription operation = operations[i];
                if (operation.IsServerInitiated())
                {
                    return true;
                }
            }

            return false;
        }

        protected void InitializeEndpoint(string configurationName, EndpointAddress address)
        {
            Endpoint = CreateDescription();

            ServiceEndpoint serviceEndpointFromConfig = null;

            // Project N and K do not support System.Configuration, but this method is part of Windows Store contract.
            // The configurationName==null path occurs in normal use.
            if (configurationName != null)
            {
                throw ExceptionHelper.PlatformNotSupported();
                // serviceEndpointFromConfig = ConfigLoader.LookupEndpoint(configurationName, address, this.serviceEndpoint.Contract);
            }

            if (serviceEndpointFromConfig != null)
            {
                Endpoint = serviceEndpointFromConfig;
            }
            else
            {
                if (address != null)
                {
                    Endpoint.Address = address;
                }

                ApplyConfiguration(configurationName);
            }
            _configurationName = configurationName;
            EnsureSecurityCredentialsManager(Endpoint);
        }

        protected void InitializeEndpoint(ServiceEndpoint endpoint)
        {
            Endpoint = endpoint ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(endpoint));

            ApplyConfiguration(null);
            EnsureSecurityCredentialsManager(Endpoint);
        }

        protected void InitializeEndpoint(Binding binding, EndpointAddress address)
        {
            Endpoint = CreateDescription();

            if (binding != null)
            {
                Endpoint.Binding = binding;
            }
            if (address != null)
            {
                Endpoint.Address = address;
            }

            ApplyConfiguration(null);
            EnsureSecurityCredentialsManager(Endpoint);
        }

        protected override void OnOpened()
        {
            // if a client credentials has been configured cache a readonly snapshot of it
            if (Endpoint != null)
            {
                ClientCredentials credentials = Endpoint.Behaviors.Find<ClientCredentials>();
                if (credentials != null)
                {
                    ClientCredentials credentialsCopy = credentials.Clone();
                    credentialsCopy.MakeReadOnly();
                    _readOnlyClientCredentials = credentialsCopy;
                }
            }
            base.OnOpened();
        }

        protected override void OnAbort()
        {
            if (InnerFactory != null)
            {
                InnerFactory.Abort();
            }
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return CommunicationObjectInternal.OnBeginClose(this, timeout, callback, state);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CommunicationObjectInternal.OnEnd(result);
        }

        internal protected override async Task OnCloseAsync(TimeSpan timeout)
        {
            if (InnerFactory != null)
            {
                await CloseOtherAsync(InnerFactory, timeout);
            }
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return CommunicationObjectInternal.OnBeginOpen(this, timeout, callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CommunicationObjectInternal.OnEnd(result);
        }

        protected internal override async Task OnOpenAsync(TimeSpan timeout)
        {
            if (InnerFactory != null)
            {
                await OpenOtherAsync(InnerFactory, timeout);
            }
        }

        protected override void OnClose(TimeSpan timeout)
        {
            if (InnerFactory != null)
            {
                InnerFactory.Close(timeout);
            }
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            InnerFactory.Open(timeout);
        }

        protected override void OnOpening()
        {
            base.OnOpening();

            InnerFactory = CreateFactory();

            if (WcfEventSource.Instance.ChannelFactoryCreatedIsEnabled())
            {
                WcfEventSource.Instance.ChannelFactoryCreated(this);
            }


            if (InnerFactory == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.InnerChannelFactoryWasNotSet));
            }
        }
    }

    public class ChannelFactory<TChannel> : ChannelFactory, IChannelFactory<TChannel>
    {
        private Type _channelType;
        private TypeLoader _typeLoader;

        //Overload for activation DuplexChannelFactory
        protected ChannelFactory(Type channelType)
            : base()
        {
            if (channelType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(channelType));
            }

            if (!channelType.IsInterface())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.SFxChannelFactoryTypeMustBeInterface));
            }

            _channelType = channelType;
        }

        // TChannel provides ContractDescription
        public ChannelFactory()
            : this(typeof(TChannel))
        {
            using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : null)
            {
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    ServiceModelActivity.Start(activity, SR.Format(SR.ActivityConstructChannelFactory, typeof(TChannel).FullName), ActivityType.Construct);
                }
                InitializeEndpoint((string)null, null);
            }
        }

        // TChannel provides ContractDescription, attr/config [TChannel,name] provides Address,Binding
        public ChannelFactory(string endpointConfigurationName)
            : this(endpointConfigurationName, null)
        {
        }

        // TChannel provides ContractDescription, attr/config [TChannel,name] provides Binding, provide Address explicitly
        public ChannelFactory(string endpointConfigurationName, EndpointAddress remoteAddress)
            : this(typeof(TChannel))
        {
            using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : null)
            {
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    ServiceModelActivity.Start(activity, SR.Format(SR.ActivityConstructChannelFactory, typeof(TChannel).FullName), ActivityType.Construct);
                }
                if (endpointConfigurationName == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(endpointConfigurationName));
                }

                InitializeEndpoint(endpointConfigurationName, remoteAddress);
            }
        }

        // TChannel provides ContractDescription, attr/config [TChannel,name] provides Address,Binding
        public ChannelFactory(Binding binding)
            : this(binding, (EndpointAddress)null)
        {
        }

        public ChannelFactory(Binding binding, String remoteAddress)
            : this(binding, new EndpointAddress(remoteAddress))
        {
        }

        // TChannel provides ContractDescription, provide Address,Binding explicitly
        public ChannelFactory(Binding binding, EndpointAddress remoteAddress)
            : this(typeof(TChannel))
        {
            using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : null)
            {
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    ServiceModelActivity.Start(activity, SR.Format(SR.ActivityConstructChannelFactory, typeof(TChannel).FullName), ActivityType.Construct);
                }
                if (binding == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(binding));
                }

                InitializeEndpoint(binding, remoteAddress);
            }
        }

        // provide ContractDescription,Address,Binding explicitly
        public ChannelFactory(ServiceEndpoint endpoint)
            : this(typeof(TChannel))
        {
            using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : null)
            {
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    ServiceModelActivity.Start(activity, SR.Format(SR.ActivityConstructChannelFactory, typeof(TChannel).FullName), ActivityType.Construct);
                }
                if (endpoint == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(endpoint));
                }

                InitializeEndpoint(endpoint);
            }
        }

        internal InstanceContext CallbackInstance { get; set; }

        internal Type CallbackType { get; set; }

        internal ServiceChannelFactory ServiceChannelFactory
        {
            get { return (ServiceChannelFactory)InnerFactory; }
        }

        internal TypeLoader TypeLoader
        {
            get
            {
                if (_typeLoader == null)
                {
                    _typeLoader = new TypeLoader();
                }

                return _typeLoader;
            }
        }


        public TChannel CreateChannel(EndpointAddress address)
        {
            if (address == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(address));
            }

            return CreateChannel(address, address.Uri);
        }

        public virtual TChannel CreateChannel(EndpointAddress address, Uri via)
        {
            bool traceOpenAndClose = TraceOpenAndClose;
            try
            {
                if (address == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(address));
                }

                if (HasDuplexOperations())
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SFxCreateNonDuplexChannel1, Endpoint.Contract.Name)));
                }

                EnsureOpened();
                return (TChannel)ServiceChannelFactory.CreateChannel<TChannel>(address, via);
            }
            finally
            {
                TraceOpenAndClose = traceOpenAndClose;
            }
        }

        public TChannel CreateChannel()
        {
            return CreateChannel(CreateEndpointAddress(Endpoint), null);
        }

        internal UChannel CreateChannel<UChannel>(EndpointAddress address, Uri via)
        {
            EnsureOpened();
            return ServiceChannelFactory.CreateChannel<UChannel>(address, via);
        }

        internal UChannel CreateChannel<UChannel>(EndpointAddress address)
        {
            EnsureOpened();
            return ServiceChannelFactory.CreateChannel<UChannel>(address);
        }

        protected override ServiceEndpoint CreateDescription()
        {
            ContractDescription contractDescription = TypeLoader.LoadCachedContractDescription(_channelType);

            ServiceEndpoint endpoint = new ServiceEndpoint(contractDescription);
            ReflectOnCallbackInstance(endpoint);
            TypeLoader.AddBehaviorsSFx(endpoint, _channelType);

            return endpoint;
        }

        private void ReflectOnCallbackInstance(ServiceEndpoint endpoint)
        {
            if (CallbackType != null)
            {
                if (endpoint.Contract.CallbackContractType == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SfxCallbackTypeCannotBeNull, endpoint.Contract.ContractType.FullName)));
                }

                TypeLoader.AddBehaviorsFromImplementationType(endpoint, CallbackType);
            }
            else if (CallbackInstance != null && CallbackInstance.UserObject != null)
            {
                if (endpoint.Contract.CallbackContractType == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SfxCallbackTypeCannotBeNull, endpoint.Contract.ContractType.FullName)));
                }

                object implementation = CallbackInstance.UserObject;
                Type implementationType = implementation.GetType();

                TypeLoader.AddBehaviorsFromImplementationType(endpoint, implementationType);

                IEndpointBehavior channelBehavior = implementation as IEndpointBehavior;
                if (channelBehavior != null)
                {
                    endpoint.Behaviors.Add(channelBehavior);
                }
                IContractBehavior contractBehavior = implementation as IContractBehavior;
                if (contractBehavior != null)
                {
                    endpoint.Contract.Behaviors.Add(contractBehavior);
                }
            }
        }

        //Static funtions to create channels
        protected static TChannel CreateChannel(String endpointConfigurationName)
        {
            ChannelFactory<TChannel> channelFactory = new ChannelFactory<TChannel>(endpointConfigurationName);

            if (channelFactory.HasDuplexOperations())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SFxInvalidStaticOverloadCalledForDuplexChannelFactory1, channelFactory._channelType.Name)));
            }

            TChannel channel = channelFactory.CreateChannel();
            SetFactoryToAutoClose(channel);
            return channel;
        }

        public static TChannel CreateChannel(Binding binding, EndpointAddress endpointAddress)
        {
            ChannelFactory<TChannel> channelFactory = new ChannelFactory<TChannel>(binding, endpointAddress);

            if (channelFactory.HasDuplexOperations())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SFxInvalidStaticOverloadCalledForDuplexChannelFactory1, channelFactory._channelType.Name)));
            }

            TChannel channel = channelFactory.CreateChannel();
            SetFactoryToAutoClose(channel);
            return channel;
        }

        public static TChannel CreateChannel(Binding binding, EndpointAddress endpointAddress, Uri via)
        {
            ChannelFactory<TChannel> channelFactory = new ChannelFactory<TChannel>(binding);

            if (channelFactory.HasDuplexOperations())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SFxInvalidStaticOverloadCalledForDuplexChannelFactory1, channelFactory._channelType.Name)));
            }

            TChannel channel = channelFactory.CreateChannel(endpointAddress, via);
            SetFactoryToAutoClose(channel);
            return channel;
        }

        internal static void SetFactoryToAutoClose(TChannel channel)
        {
            //Set the Channel to auto close its ChannelFactory.
            ServiceChannel serviceChannel = ServiceChannelFactory.GetServiceChannel(channel);
            serviceChannel.CloseFactory = true;
        }
    }
}
