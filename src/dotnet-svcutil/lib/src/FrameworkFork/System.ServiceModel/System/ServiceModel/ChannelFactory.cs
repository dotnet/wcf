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
        private IChannelFactory _innerFactory;
        private ServiceEndpoint _serviceEndpoint;
        private ClientCredentials _readOnlyClientCredentials;
        private object _openLock = new object();

        //Overload for activation DuplexChannelFactory
        protected ChannelFactory()
            : base()
        {
            TraceUtility.SetEtwProviderId();
            this.TraceOpenAndClose = true;
        }

        public ClientCredentials Credentials
        {
            get
            {
                if (this.Endpoint == null)
                    return null;
                if (this.State == CommunicationState.Created || this.State == CommunicationState.Opening)
                {
                    return EnsureCredentials(this.Endpoint);
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
                if (this.Endpoint != null && this.Endpoint.Binding != null)
                {
                    return this.Endpoint.Binding.CloseTimeout;
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
                if (this.Endpoint != null && this.Endpoint.Binding != null)
                {
                    return this.Endpoint.Binding.OpenTimeout;
                }
                else
                {
                    return ServiceDefaults.OpenTimeout;
                }
            }
        }

        public ServiceEndpoint Endpoint
        {
            get
            {
                return _serviceEndpoint;
            }
        }

        internal IChannelFactory InnerFactory
        {
            get { return _innerFactory; }
        }

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
            if (this.State != CommunicationState.Opened)
            {
                lock (_openLock)
                {
                    if (this.State != CommunicationState.Opened)
                    {
                        this.Open();
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxChannelFactoryEndpointAddressUri));
            }

            return endpoint.Address;
        }

        protected virtual IChannelFactory CreateFactory()
        {
            if (this.Endpoint == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxChannelFactoryCannotCreateFactoryWithoutDescription));
            }

            if (this.Endpoint.Binding == null)
            {
                if (_configurationName != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxChannelFactoryNoBindingFoundInConfig1, _configurationName)));
                }
                else
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxChannelFactoryNoBindingFoundInConfigOrCode));
                }
            }

            return ServiceChannelFactory.BuildChannelFactory(this.Endpoint, this.UseActiveAutoClose);
        }

        void IDisposable.Dispose()
        {
            this.Close();
        }

        private void EnsureSecurityCredentialsManager(ServiceEndpoint endpoint)
        {
            Fx.Assert(this.State == CommunicationState.Created || this.State == CommunicationState.Opening, "");
            if (endpoint.Behaviors.Find<SecurityCredentialsManager>() == null)
            {
                endpoint.Behaviors.Add(new ClientCredentials());
            }
        }

        private ClientCredentials EnsureCredentials(ServiceEndpoint endpoint)
        {
            Fx.Assert(this.State == CommunicationState.Created || this.State == CommunicationState.Opening, "");
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
            if (_innerFactory != null)
            {
                return _innerFactory.GetProperty<T>();
            }
            else
            {
                return null;
            }
        }

        internal bool HasDuplexOperations()
        {
            OperationDescriptionCollection operations = this.Endpoint.Contract.Operations;
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
            _serviceEndpoint = this.CreateDescription();

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
                _serviceEndpoint = serviceEndpointFromConfig;
            }
            else
            {
                if (address != null)
                {
                    this.Endpoint.Address = address;
                }

                ApplyConfiguration(configurationName);
            }
            _configurationName = configurationName;
            EnsureSecurityCredentialsManager(_serviceEndpoint);
        }

        protected void InitializeEndpoint(ServiceEndpoint endpoint)
        {
            if (endpoint == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpoint");
            }

            _serviceEndpoint = endpoint;

            ApplyConfiguration(null);
            EnsureSecurityCredentialsManager(_serviceEndpoint);
        }

        protected void InitializeEndpoint(Binding binding, EndpointAddress address)
        {
            _serviceEndpoint = this.CreateDescription();

            if (binding != null)
            {
                this.Endpoint.Binding = binding;
            }
            if (address != null)
            {
                this.Endpoint.Address = address;
            }

            ApplyConfiguration(null);
            EnsureSecurityCredentialsManager(_serviceEndpoint);
        }

        protected override void OnOpened()
        {
            // if a client credentials has been configured cache a readonly snapshot of it
            if (this.Endpoint != null)
            {
                ClientCredentials credentials = this.Endpoint.Behaviors.Find<ClientCredentials>();
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
            if (_innerFactory != null)
            {
                _innerFactory.Abort();
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
            if (_innerFactory != null)
            {
                IAsyncChannelFactory asyncFactory = _innerFactory as IAsyncChannelFactory;
                if (asyncFactory != null)
                {
                    await asyncFactory.CloseAsync(timeout);
                }
                else
                {
                    _innerFactory.Close(timeout);
                }
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
            if (_innerFactory != null)
            {
                IAsyncChannelFactory asyncFactory = _innerFactory as IAsyncChannelFactory;
                if (asyncFactory != null)
                {
                    await asyncFactory.OpenAsync(timeout);
                }
                else
                {
                    _innerFactory.Open(timeout);
                }
            }
        }

        protected override void OnClose(TimeSpan timeout)
        {
            if (_innerFactory != null)
            {
                _innerFactory.Close(timeout);
            }
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            _innerFactory.Open(timeout);
        }

        protected override void OnOpening()
        {
            base.OnOpening();

            _innerFactory = CreateFactory();

            if (WcfEventSource.Instance.ChannelFactoryCreatedIsEnabled())
            {
                WcfEventSource.Instance.ChannelFactoryCreated(this);
            }


            if (_innerFactory == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.InnerChannelFactoryWasNotSet));
        }
    }

    public class ChannelFactory<TChannel> : ChannelFactory, IChannelFactory<TChannel>
    {
        private InstanceContext _callbackInstance;
        private Type _channelType;
        private TypeLoader _typeLoader;
        private Type _callbackType;

        //Overload for activation DuplexChannelFactory
        protected ChannelFactory(Type channelType)
            : base()
        {
            if (channelType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channelType");
            }

            if (!channelType.IsInterface())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxChannelFactoryTypeMustBeInterface));
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
                    ServiceModelActivity.Start(activity, string.Format(SRServiceModel.ActivityConstructChannelFactory, typeof(TChannel).FullName), ActivityType.Construct);
                }
                this.InitializeEndpoint((string)null, null);
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
                    ServiceModelActivity.Start(activity, string.Format(SRServiceModel.ActivityConstructChannelFactory, typeof(TChannel).FullName), ActivityType.Construct);
                }
                if (endpointConfigurationName == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpointConfigurationName");
                }

                this.InitializeEndpoint(endpointConfigurationName, remoteAddress);
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
                    ServiceModelActivity.Start(activity, string.Format(SRServiceModel.ActivityConstructChannelFactory, typeof(TChannel).FullName), ActivityType.Construct);
                }
                if (binding == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("binding");
                }

                this.InitializeEndpoint(binding, remoteAddress);
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
                    ServiceModelActivity.Start(activity, string.Format(SRServiceModel.ActivityConstructChannelFactory, typeof(TChannel).FullName), ActivityType.Construct);
                }
                if (endpoint == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpoint");
                }

                this.InitializeEndpoint(endpoint);
            }
        }

        internal InstanceContext CallbackInstance
        {
            get { return _callbackInstance; }
            set { _callbackInstance = value; }
        }

        internal Type CallbackType
        {
            get { return _callbackType; }
            set { _callbackType = value; }
        }

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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("address");
            }

            return CreateChannel(address, address.Uri);
        }

        public virtual TChannel CreateChannel(EndpointAddress address, Uri via)
        {
            bool traceOpenAndClose = this.TraceOpenAndClose;
            try
            {
                if (address == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("address");
                }

                if (this.HasDuplexOperations())
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxCreateNonDuplexChannel1, this.Endpoint.Contract.Name)));
                }

                EnsureOpened();
                return (TChannel)this.ServiceChannelFactory.CreateChannel<TChannel>(address, via);
            }
            finally
            {
                this.TraceOpenAndClose = traceOpenAndClose;
            }
        }

        public TChannel CreateChannel()
        {
            return CreateChannel(this.CreateEndpointAddress(this.Endpoint), null);
        }

        protected override ServiceEndpoint CreateDescription()
        {
            ContractDescription contractDescription = this.TypeLoader.LoadContractDescription(_channelType);

            ServiceEndpoint endpoint = new ServiceEndpoint(contractDescription);
            ReflectOnCallbackInstance(endpoint);
            this.TypeLoader.AddBehaviorsSFx(endpoint, _channelType);

            return endpoint;
        }

        private void ReflectOnCallbackInstance(ServiceEndpoint endpoint)
        {
            if (_callbackType != null)
            {
                if (endpoint.Contract.CallbackContractType == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SfxCallbackTypeCannotBeNull, endpoint.Contract.ContractType.FullName)));
                }

                this.TypeLoader.AddBehaviorsFromImplementationType(endpoint, _callbackType);
            }
            else if (this.CallbackInstance != null && this.CallbackInstance.UserObject != null)
            {
                if (endpoint.Contract.CallbackContractType == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SfxCallbackTypeCannotBeNull, endpoint.Contract.ContractType.FullName)));
                }

                object implementation = this.CallbackInstance.UserObject;
                Type implementationType = implementation.GetType();

                this.TypeLoader.AddBehaviorsFromImplementationType(endpoint, implementationType);

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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxInvalidStaticOverloadCalledForDuplexChannelFactory1, channelFactory._channelType.Name)));
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxInvalidStaticOverloadCalledForDuplexChannelFactory1, channelFactory._channelType.Name)));
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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxInvalidStaticOverloadCalledForDuplexChannelFactory1, channelFactory._channelType.Name)));
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
