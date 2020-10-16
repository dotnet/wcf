// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Runtime;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal abstract class ConnectionOrientedTransportChannelFactory<TChannel> : TransportChannelFactory<TChannel>, IConnectionOrientedTransportChannelFactorySettings
    {
        private int _connectionBufferSize;
        private IConnectionInitiator _connectionInitiator;

        private ConnectionPool _connectionPool;
        private string _connectionPoolGroupName;
        private bool _exposeConnectionProperty;
        private TimeSpan _idleTimeout;
        private int _maxBufferSize;
        private int _maxOutboundConnectionsPerEndpoint;
        private TimeSpan _maxOutputDelay;
        private TransferMode _transferMode;
        private ISecurityCapabilities _securityCapabilities;
        private StreamUpgradeProvider _upgrade;
        private bool _flowIdentity;

        internal ConnectionOrientedTransportChannelFactory(
            ConnectionOrientedTransportBindingElement bindingElement, BindingContext context,
            string connectionPoolGroupName, TimeSpan idleTimeout, int maxOutboundConnectionsPerEndpoint, bool supportsImpersonationDuringAsyncOpen)
            : base(bindingElement, context)
        {
            if (bindingElement.TransferMode == TransferMode.Buffered && bindingElement.MaxReceivedMessageSize > int.MaxValue)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new ArgumentOutOfRangeException("bindingElement.MaxReceivedMessageSize",
                    SRServiceModel.MaxReceivedMessageSizeMustBeInIntegerRange));
            }

            _connectionBufferSize = bindingElement.ConnectionBufferSize;
            _connectionPoolGroupName = connectionPoolGroupName;
            _exposeConnectionProperty = bindingElement.ExposeConnectionProperty;
            _idleTimeout = idleTimeout;
            _maxBufferSize = bindingElement.MaxBufferSize;
            _maxOutboundConnectionsPerEndpoint = maxOutboundConnectionsPerEndpoint;
            _maxOutputDelay = bindingElement.MaxOutputDelay;
            _transferMode = bindingElement.TransferMode;

            Collection<StreamUpgradeBindingElement> upgradeBindingElements =
                context.BindingParameters.FindAll<StreamUpgradeBindingElement>();

            if (upgradeBindingElements.Count > 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.MultipleStreamUpgradeProvidersInParameters));
            }
            else if ((upgradeBindingElements.Count == 1) && this.SupportsUpgrade(upgradeBindingElements[0]))
            {
                _upgrade = upgradeBindingElements[0].BuildClientStreamUpgradeProvider(context);
                context.BindingParameters.Remove<StreamUpgradeBindingElement>();
                _securityCapabilities = upgradeBindingElements[0].GetProperty<ISecurityCapabilities>(context);
                // flow the identity only if the channel factory supports impersonating during an async open AND
                // there is the binding is configured with security
                _flowIdentity = supportsImpersonationDuringAsyncOpen;
            }
        }

        public int ConnectionBufferSize
        {
            get
            {
                return _connectionBufferSize;
            }
        }

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
                            _connectionInitiator = GetConnectionInitiator();
                        }
                    }
                }

                return _connectionInitiator;
            }
        }

        public string ConnectionPoolGroupName
        {
            get
            {
                return _connectionPoolGroupName;
            }
        }

        public TimeSpan IdleTimeout
        {
            get
            {
                return _idleTimeout;
            }
        }

        public int MaxBufferSize
        {
            get
            {
                return _maxBufferSize;
            }
        }

        public int MaxOutboundConnectionsPerEndpoint
        {
            get
            {
                return _maxOutboundConnectionsPerEndpoint;
            }
        }

        public TimeSpan MaxOutputDelay
        {
            get
            {
                return _maxOutputDelay;
            }
        }

        public StreamUpgradeProvider Upgrade
        {
            get
            {
                StreamUpgradeProvider localUpgrade = _upgrade;
                CommunicationObjectInternal.ThrowIfDisposed(this);
                return localUpgrade;
            }
        }

        public TransferMode TransferMode
        {
            get
            {
                return _transferMode;
            }
        }

        int IConnectionOrientedTransportFactorySettings.MaxBufferSize
        {
            get { return MaxBufferSize; }
        }

        TransferMode IConnectionOrientedTransportFactorySettings.TransferMode
        {
            get { return TransferMode; }
        }

        StreamUpgradeProvider IConnectionOrientedTransportFactorySettings.Upgrade
        {
            get { return Upgrade; }
        }

        public override T GetProperty<T>()
        {
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T)(object)_securityCapabilities;
            }

            T result = base.GetProperty<T>();
            if (result == null && _upgrade != null)
            {
                result = _upgrade.GetProperty<T>();
            }

            return result;
        }

        public override int GetMaxBufferSize()
        {
            return this.MaxBufferSize;
        }

        internal abstract IConnectionInitiator GetConnectionInitiator();

        internal abstract ConnectionPool GetConnectionPool();

        internal abstract void ReleaseConnectionPool(ConnectionPool pool, TimeSpan timeout);

        protected override TChannel OnCreateChannel(EndpointAddress address, Uri via)
        {
            base.ValidateScheme(via);


            if (TransferMode == TransferMode.Buffered)
            {
                // typeof(TChannel) == typeof(IDuplexSessionChannel)
                return (TChannel)(object)new ClientFramingDuplexSessionChannel(this, this, address, via,
                    ConnectionInitiator, _connectionPool, _exposeConnectionProperty, _flowIdentity);
            }

            throw ExceptionHelper.PlatformNotSupported("StreamedFramingRequestChannel not yet implemented");
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
            StreamUpgradeProvider localUpgrade;
            ConnectionPool localConnectionPool;
            if (GetUpgradeAndConnectionPool(out localUpgrade, out localConnectionPool))
            {
                if (localConnectionPool != null)
                {
                    ReleaseConnectionPool(localConnectionPool, TimeSpan.Zero);
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
                    ReleaseConnectionPool(localConnectionPool, timeoutHelper.RemainingTime());
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
            _connectionPool = GetConnectionPool(); // returns an already opened pool
            Contract.Assert(_connectionPool != null, "ConnectionPool should always be found");
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw ExceptionHelper.PlatformNotSupported("ConnectionOrientedTransportChannelFactory async open path");
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            throw ExceptionHelper.PlatformNotSupported("ConnectionOrientedTransportChannelFactory async open path");
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            StreamUpgradeProvider localUpgrade = this.Upgrade;
            if (localUpgrade != null)
            {
                localUpgrade.Open(timeout);
            }
        }

        protected internal override async Task OnOpenAsync(TimeSpan timeout)
        {
            StreamUpgradeProvider localUpgrade = this.Upgrade;
            if (localUpgrade != null)
            {
                await ((IAsyncCommunicationObject)localUpgrade).OpenAsync(timeout);
            }
        }

        protected internal override async Task OnCloseAsync(TimeSpan timeout)
        {
            StreamUpgradeProvider localUpgrade;
            ConnectionPool localConnectionPool;

            if (this.GetUpgradeAndConnectionPool(out localUpgrade, out localConnectionPool))
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

                if (localConnectionPool != null)
                {
                    this.ReleaseConnectionPool(localConnectionPool, timeoutHelper.RemainingTime());
                }

                if (localUpgrade != null)
                {
                    await ((IAsyncCommunicationObject)localUpgrade).CloseAsync(timeoutHelper.RemainingTime());
                }
            }
        }

        protected virtual bool SupportsUpgrade(StreamUpgradeBindingElement upgradeBindingElement)
        {
            return true;
        }
    }
}
