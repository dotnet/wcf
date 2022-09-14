// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Runtime;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    public abstract class NetFramingTransportChannelFactory<TChannel> : ChannelFactoryBase<TChannel>, IConnectionOrientedTransportChannelFactorySettings
    {
        private static Hashtable s_connectionPoolRegistries = new Hashtable();
        private ConnectionPoolRegistry _connectionPoolRegistry;
        private IConnectionInitiator _connectionInitiator;
        private ConnectionPool _connectionPool;
        private int _maxOutboundConnectionsPerEndpoint;
        private ISecurityCapabilities _securityCapabilities;
        private StreamUpgradeProvider _upgrade;

        public NetFramingTransportChannelFactory(ConnectionOrientedTransportBindingElement bindingElement, BindingContext context,
                                                   string connectionPoolGroupName, TimeSpan idleTimeout,
                                                   int maxOutboundConnectionsPerEndpoint) : base(context.Binding)
        {
            ManualAddressing = bindingElement.ManualAddressing;
            MaxBufferPoolSize = bindingElement.MaxBufferPoolSize;
            MaxReceivedMessageSize = bindingElement.MaxReceivedMessageSize;

            Collection<MessageEncodingBindingElement> messageEncoderBindingElements
                = context.BindingParameters.FindAll<MessageEncodingBindingElement>();

            if (messageEncoderBindingElements.Count > 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.MultipleMebesInParameters));
            }
            else if (messageEncoderBindingElements.Count == 1)
            {
                MessageEncoderFactory = messageEncoderBindingElements[0].CreateMessageEncoderFactory();
                context.BindingParameters.Remove<MessageEncodingBindingElement>();
            }
            else
            {
                MessageEncoderFactory = NFTransportDefaults.GetDefaultMessageEncoderFactory();
            }

            if (null != MessageEncoderFactory)
            {
                MessageVersion = MessageEncoderFactory.MessageVersion;
            }
            else
            {
                MessageVersion = MessageVersion.None;
            }

            if (bindingElement.TransferMode == TransferMode.Buffered && bindingElement.MaxReceivedMessageSize > int.MaxValue)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException("bindingElement.MaxReceivedMessageSize",
                    SR.MaxReceivedMessageSizeMustBeInIntegerRange));
            }

            ConnectionBufferSize = bindingElement.ConnectionBufferSize;
            ConnectionPoolGroupName = connectionPoolGroupName;
            IdleTimeout = idleTimeout;
            MaxBufferSize = bindingElement.MaxBufferSize;
            _maxOutboundConnectionsPerEndpoint = maxOutboundConnectionsPerEndpoint;
            MaxOutputDelay = bindingElement.MaxOutputDelay;
            TransferMode = bindingElement.TransferMode;

            Collection<StreamUpgradeBindingElement> upgradeBindingElements =
                context.BindingParameters.FindAll<StreamUpgradeBindingElement>();

            if (upgradeBindingElements.Count > 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.MultipleStreamUpgradeProvidersInParameters));
            }
            else if ((upgradeBindingElements.Count == 1) && SupportsUpgrade(upgradeBindingElements[0]))
            {
                _upgrade = upgradeBindingElements[0].BuildClientStreamUpgradeProvider(context);
                context.BindingParameters.Remove<StreamUpgradeBindingElement>();
                _securityCapabilities = upgradeBindingElements[0].GetProperty<ISecurityCapabilities>(context);
            }
        }

        internal BufferManager BufferManager { get; private set; }

        public int ConnectionBufferSize { get; }

        internal IConnectionInitiator ConnectionInitiator
        {
            get
            {
                if (_connectionInitiator == null)
                {
                    lock (ThisLock)
                    {
                        if (_connectionInitiator == null)
                        {
                            var connectionInitiator = GetConnectionInitiator();
                            _connectionInitiator = new BufferedConnectionInitiator(connectionInitiator, MaxOutputDelay, ConnectionBufferSize);
                        }
                    }
                }

                return _connectionInitiator;
            }
        }

        public string ConnectionPoolGroupName { get; }

        public TimeSpan IdleTimeout { get; }

        internal bool ManualAddressing { get; }

        internal long MaxBufferPoolSize { get; }

        public int MaxBufferSize { get; }

        public int MaxOutboundConnectionsPerEndpoint => _maxOutboundConnectionsPerEndpoint;

        public TimeSpan MaxOutputDelay { get; }

        internal long MaxReceivedMessageSize { get; }

        internal MessageEncoderFactory MessageEncoderFactory { get; }

        internal MessageVersion MessageVersion { get; }

        public StreamUpgradeProvider Upgrade
        {
            get
            {
                StreamUpgradeProvider localUpgrade = _upgrade;
                this.ThrowIfDisposed();
                return localUpgrade;
            }
        }

        public abstract string Scheme { get; }

        public TransferMode TransferMode { get; }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T)(object)_securityCapabilities;
            }

            T result = null;

            if (typeof(T) == typeof(MessageVersion))
            {
                result = (T)(object)MessageVersion;
            }
            else if (typeof(T) == typeof(FaultConverter))
            {
                if (MessageEncoderFactory is not null)
                {
                    result = MessageEncoderFactory.Encoder.GetProperty<T>();
                }
            }
            else if (typeof(T) == typeof(ITransportFactorySettings))
            {
                result = (T)(object)this;
            }

            if (result is null && _upgrade is not null)
            {
                result = _upgrade.GetProperty<T>();
            }

            return result;
        }

        internal int GetMaxBufferSize()
        {
            return MaxBufferSize;
        }

        public abstract IConnectionInitiator GetConnectionInitiator();

        internal ConnectionPool GetConnectionPool()
        {
            EnsureConnectionPoolRegistry();
            return _connectionPoolRegistry.Lookup(this);
        }

        private void EnsureConnectionPoolRegistry()
        {
            if (_connectionPoolRegistry is null)
            {
                // Using Hashtable to avoid taking lock when looking for registry in dictionary
                if (!s_connectionPoolRegistries.ContainsKey(GetType()))
                {
                    lock (s_connectionPoolRegistries)
                    {
                        if (!s_connectionPoolRegistries.ContainsKey(GetType()))
                        {
                            s_connectionPoolRegistries[GetType()] = new ConnectionPoolRegistry();
                        }
                    }
                }

                _connectionPoolRegistry = (ConnectionPoolRegistry)s_connectionPoolRegistries[GetType()];
            }
        }

        internal ValueTask ReleaseConnectionPoolAsync(ConnectionPool pool, TimeSpan timeout)
        {
            EnsureConnectionPoolRegistry();
            return _connectionPoolRegistry.ReleaseAsync(pool, timeout);
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state) => OnCloseAsync(timeout).ToApm(callback, state);

        protected override void OnEndClose(IAsyncResult result) => result.ToApmEnd();

        protected override TChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            ValidateScheme(via);


            if (TransferMode == TransferMode.Buffered)
            {
                // typeof(TChannel) == typeof(IDuplexSessionChannel)
                return (TChannel)(object)new ClientFramingDuplexSessionChannel(this, this, address, via,
                    ConnectionInitiator, _connectionPool);
            }

            // typeof(TChannel) == typeof(IRequestChannel)
            return (TChannel)(object)new StreamedFramingRequestChannel(this, this, address, via,
                ConnectionInitiator, _connectionPool);
        }

        private bool GetUpgradeAndConnectionPool(out StreamUpgradeProvider upgradeCopy, out ConnectionPool poolCopy)
        {
            if (_upgrade != null || _connectionPool != null)
            {
                lock (ThisLock)
                {
                    if (_upgrade != null || _connectionPool != null)
                    {
                        upgradeCopy = _upgrade;
                        poolCopy = _connectionPool;
                        _upgrade = null;
                        _connectionPool = null;
                        return true;
                    }
                }
            }

            upgradeCopy = null;
            poolCopy = null;
            return false;
        }

        protected override void OnAbort()
        {
            /* The following code was original in base.OnAbort but was never called from here
                 OnCloseOrAbort();
                 base.OnAbort();
               I suspect there might be a bug caused by this so needs further investigation.
               For now I'm leaving this code to have the same behavior as NetFx
            */
            StreamUpgradeProvider localUpgrade;
            ConnectionPool localConnectionPool;
            if (GetUpgradeAndConnectionPool(out localUpgrade, out localConnectionPool))
            {
                if (localConnectionPool != null)
                {
                    ReleaseConnectionPoolAsync(localConnectionPool, TimeSpan.Zero).GetAwaiter().GetResult();
                }

                if (localUpgrade != null)
                {
                    localUpgrade.Abort();
                }
            }
        }

        protected override void OnClose(TimeSpan timeout)
        {
            StreamUpgradeProvider localUpgrade;
            ConnectionPool localConnectionPool;

            if (GetUpgradeAndConnectionPool(out localUpgrade, out localConnectionPool))
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

                if (localConnectionPool != null)
                {
                    ReleaseConnectionPoolAsync(localConnectionPool, timeoutHelper.RemainingTime()).GetAwaiter().GetResult();
                }

                if (localUpgrade != null)
                {
                    localUpgrade.Close(timeoutHelper.RemainingTime());
                }
            }
        }

        protected override void OnOpening()
        {
            base.OnOpening();
            BufferManager = BufferManager.CreateBufferManager(MaxBufferPoolSize, GetMaxBufferSize());
            _connectionPool = GetConnectionPool(); // returns an already opened pool
            Contract.Assert(_connectionPool != null, "ConnectionPool should always be found");
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OnOpenAsync(timeout).ToApm(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            StreamUpgradeProvider localUpgrade = Upgrade;
            if (localUpgrade != null)
            {
                localUpgrade.Open(timeout);
            }
        }

        internal protected override Task OnOpenAsync(TimeSpan timeout)
        {
            StreamUpgradeProvider localUpgrade = Upgrade;
            if (localUpgrade != null)
            {
                return Task.Factory.FromAsync(localUpgrade.BeginOpen, localUpgrade.EndOpen, timeout, null);
            }
            return Task.CompletedTask;
        }

        internal protected override async Task OnCloseAsync(TimeSpan timeout)
        {
            StreamUpgradeProvider localUpgrade;
            ConnectionPool localConnectionPool;

            if (GetUpgradeAndConnectionPool(out localUpgrade, out localConnectionPool))
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

                if (localConnectionPool != null)
                {
                    await ReleaseConnectionPoolAsync(localConnectionPool, timeoutHelper.RemainingTime());
                }

                if (localUpgrade != null)
                {
                    await Task.Factory.FromAsync(localUpgrade.BeginClose, localUpgrade.EndClose, timeoutHelper.RemainingTime(), null);
                }
            }
        }

        protected virtual bool SupportsUpgrade(StreamUpgradeBindingElement upgradeBindingElement)
        {
            return true;
        }

        internal void ValidateScheme(Uri via)
        {
            if (via.Scheme != Scheme)
            {
                // URI schemes are case-insensitive, so try a case insensitive compare now
                if (string.Compare(via.Scheme, Scheme, StringComparison.OrdinalIgnoreCase) != 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(nameof(via), SR.Format(SR.InvalidUriScheme,
                        via.Scheme, Scheme));
                }
            }
        }

        protected abstract string GetConnectionPoolKey(EndpointAddress address, Uri via);

        long ITransportFactorySettings.MaxReceivedMessageSize => MaxReceivedMessageSize;
        BufferManager ITransportFactorySettings.BufferManager => BufferManager;
        bool ITransportFactorySettings.ManualAddressing => ManualAddressing;
        MessageEncoderFactory ITransportFactorySettings.MessageEncoderFactory => MessageEncoderFactory;
        MessageVersion ITransportFactorySettings.MessageVersion => MessageVersion;
        int IConnectionOrientedTransportFactorySettings.MaxBufferSize => MaxBufferSize;
        TransferMode IConnectionOrientedTransportFactorySettings.TransferMode => TransferMode;
        StreamUpgradeProvider IConnectionOrientedTransportFactorySettings.Upgrade => Upgrade;
        string IConnectionOrientedTransportChannelFactorySettings.GetConnectionPoolKey(EndpointAddress address, Uri via) => GetConnectionPoolKey(address, via);
    }

}
