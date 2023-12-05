// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    // code that pools items and closes/aborts them as necessary.
    // shared by IConnection and IChannel users
    internal abstract class CommunicationPool<TKey, TItem>
        where TKey : class
        where TItem : class
    {
        private readonly Dictionary<TKey, EndpointConnectionPool> _endpointPools;
        // need to make sure we prune over a certain number of endpoint pools
        private int _pruneAccrual;
        private const int pruneThreshold = 30;

        protected CommunicationPool(int maxCount)
        {
            MaxIdleConnectionPoolCount = maxCount;
            _endpointPools = new Dictionary<TKey, EndpointConnectionPool>();
            OpenCount = 1;
        }

        public int MaxIdleConnectionPoolCount { get; }

        protected SemaphoreSlim ThisLock { get; } = new SemaphoreSlim(1);

        internal int OpenCount { get; set; }

        protected abstract void AbortItem(TItem item);

        protected abstract ValueTask CloseItemAsync(TItem item, TimeSpan timeout);

        protected abstract TKey GetPoolKey(EndpointAddress address, Uri via);

        protected virtual EndpointConnectionPool CreateEndpointConnectionPool(TKey key)
        {
            return new EndpointConnectionPool(this, key);
        }

        public async ValueTask<bool> CloseAsync(TimeSpan timeout)
        {
            await ThisLock.WaitAsync();
            try
            {
                if (OpenCount <= 0)
                {
                    return true;
                }

                OpenCount--;

                if (OpenCount == 0)
                {
                    await OnCloseAsync(timeout);
                    return true;
                }

                return false;
            }
            finally { ThisLock.Release(); }
        }

        private List<TItem> PruneIfNecessary()
        {
            List<TItem> itemsToClose = null;
            _pruneAccrual++;
            if (_pruneAccrual > pruneThreshold)
            {
                _pruneAccrual = 0;
                itemsToClose = new List<TItem>();

                // first prune the connection pool contents
                foreach (EndpointConnectionPool pool in _endpointPools.Values)
                {
                    pool.Prune(itemsToClose);
                }

                // figure out which connection pools are now empty
                List<TKey> endpointKeysToRemove = null;
                foreach (KeyValuePair<TKey, EndpointConnectionPool> poolEntry in _endpointPools)
                {
                    if (poolEntry.Value.CloseIfEmpty())
                    {
                        if (endpointKeysToRemove == null)
                        {
                            endpointKeysToRemove = new List<TKey>();
                        }
                        endpointKeysToRemove.Add(poolEntry.Key);
                    }
                }

                // and then prune the connection pools themselves
                if (endpointKeysToRemove != null)
                {
                    for (int i = 0; i < endpointKeysToRemove.Count; i++)
                    {
                        _endpointPools.Remove(endpointKeysToRemove[i]);
                    }
                }
            }

            return itemsToClose;
        }

        private EndpointConnectionPool GetEndpointPool(TKey key, TimeSpan timeout)
        {
            EndpointConnectionPool result = null;
            List<TItem> itemsToClose = null;
            ThisLock.Wait();
            try
            {
                if (!_endpointPools.TryGetValue(key, out result))
                {
                    itemsToClose = PruneIfNecessary();
                    result = CreateEndpointConnectionPool(key);
                    _endpointPools.Add(key, result);
                }
            }
            finally { ThisLock.Release(); }

            Contract.Assert(result != null, "EndpointPool must be non-null at this point");
            if (itemsToClose != null && itemsToClose.Count > 0)
            {
                // allocate half the remaining timeout for our graceful shutdowns
                TimeoutHelper timeoutHelper = new TimeoutHelper(TimeoutHelper.Divide(timeout, 2));
                for (int i = 0; i < itemsToClose.Count; i++)
                {
                    result.CloseIdleConnection(itemsToClose[i], timeoutHelper.RemainingTime());
                }
            }

            return result;
        }

        public bool TryOpen()
        {
            ThisLock.Wait();
            try
            {
                if (OpenCount <= 0)
                {
                    // can't reopen connection pools since the registry purges them on close
                    return false;
                }
                else
                {
                    OpenCount++;
                    return true;
                }
            }
            finally { ThisLock.Release(); }
        }

        protected virtual void OnClosed()
        {
        }

        private async ValueTask OnCloseAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            foreach (EndpointConnectionPool pool in _endpointPools.Values)
            {
                try
                {
                    await pool.CloseAsync(timeoutHelper.RemainingTime());
                }
                catch (CommunicationException)
                {
                }
                catch (TimeoutException exception)
                {
                    if (WcfEventSource.Instance.CloseTimeoutIsEnabled())
                    {
                        WcfEventSource.Instance.CloseTimeout(exception.Message);
                    }
                }
            }

            _endpointPools.Clear();
        }

        public void AddConnection(TKey key, TItem connection, TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            EndpointConnectionPool endpointPool = GetEndpointPool(key, timeoutHelper.RemainingTime());
            endpointPool.AddConnection(connection, timeoutHelper.RemainingTime());
        }

        public TItem TakeConnection(EndpointAddress address, Uri via, TimeSpan timeout, out TKey key)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            key = GetPoolKey(address, via);
            EndpointConnectionPool endpointPool = GetEndpointPool(key, timeoutHelper.RemainingTime());
            return endpointPool.TakeConnection(timeoutHelper.RemainingTime());
        }

        public void ReturnConnection(TKey key, TItem connection, bool connectionIsStillGood, TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            EndpointConnectionPool endpointPool = GetEndpointPool(key, timeoutHelper.RemainingTime());
            endpointPool.ReturnConnection(connection, connectionIsStillGood, timeoutHelper.RemainingTime());
        }

        // base class for our collection of Idle connections
        protected abstract class IdleConnectionPool
        {
            public abstract int Count { get; }
            public abstract bool Add(TItem item);
            public abstract bool Return(TItem item);
            public abstract TItem Take(out bool closeItem);
        }

        protected class EndpointConnectionPool
        {
            private readonly List<TItem> _busyConnections;
            private bool _closed;
            private IdleConnectionPool _idleConnections;

            public EndpointConnectionPool(CommunicationPool<TKey, TItem> parent, TKey key)
            {
                Key = key;
                Parent = parent;
                _busyConnections = new List<TItem>();
            }

            protected TKey Key { get; }

            private IdleConnectionPool IdleConnections
            {
                get
                {
                    if (_idleConnections == null)
                    {
                        _idleConnections = GetIdleConnectionPool();
                    }

                    return _idleConnections;
                }
            }

            protected CommunicationPool<TKey, TItem> Parent { get; }

            protected object ThisLock
            {
                get { return this; }
            }

            // close down the pool if empty
            public bool CloseIfEmpty()
            {
                lock (ThisLock)
                {
                    if (!_closed)
                    {
                        if (_busyConnections.Count > 0)
                        {
                            return false;
                        }

                        if (_idleConnections != null && _idleConnections.Count > 0)
                        {
                            return false;
                        }
                        _closed = true;
                    }
                }

                return true;
            }

            protected virtual void AbortItem(TItem item)
            {
                Parent.AbortItem(item);
            }

            protected virtual ValueTask CloseItemAsync(TItem item, TimeSpan timeout)
            {
                return Parent.CloseItemAsync(item, timeout);
            }

            public void Abort()
            {
                if (_closed)
                {
                    return;
                }

                List<TItem> idleItemsToClose = null;
                lock (ThisLock)
                {
                    if (_closed)
                    {
                        return;
                    }

                    _closed = true;
                    idleItemsToClose = SnapshotIdleConnections();
                }

                AbortConnections(idleItemsToClose);
            }

            public async ValueTask CloseAsync(TimeSpan timeout)
            {
                List<TItem> itemsToClose = null;
                lock (ThisLock)
                {
                    if (_closed)
                    {
                        return;
                    }

                    _closed = true;
                    itemsToClose = SnapshotIdleConnections();
                }

                try
                {
                    TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                    var closeTasks = new List<Task>(itemsToClose.Count);
                    for (int i = 0; i < itemsToClose.Count; i++)
                    {
                        var valueTask = CloseItemAsync(itemsToClose[i], timeoutHelper.RemainingTime());
                        if (!valueTask.IsCompletedSuccessfully) closeTasks.Add(valueTask.AsTask());
                    }
                    try
                    {
                        await Task.WhenAll(closeTasks);
                    }
                    catch(AggregateException ae)
                    {
                        var toe = ae.InnerExceptions.FirstOrDefault(e => e is TimeoutException);
                        if (toe is not null) ExceptionDispatchInfo.Throw(toe);
                    }
                    closeTasks.Clear();
                    itemsToClose.Clear();
                }
                finally
                {
                    AbortConnections(itemsToClose);
                }
            }

            private void AbortConnections(List<TItem> idleItemsToClose)
            {
                for (int i = 0; i < idleItemsToClose.Count; i++)
                {
                    AbortItem(idleItemsToClose[i]);
                }

                for (int i = 0; i < _busyConnections.Count; i++)
                {
                    AbortItem(_busyConnections[i]);
                }
                _busyConnections.Clear();
            }

            // must call under lock (ThisLock) since we are calling IdleConnections.Take()
            private List<TItem> SnapshotIdleConnections()
            {
                List<TItem> itemsToClose = new List<TItem>();
                bool dummy;
                for (; ; )
                {
                    TItem item = IdleConnections.Take(out dummy);
                    if (item == null)
                    {
                        break;
                    }

                    itemsToClose.Add(item);
                }

                return itemsToClose;
            }

            public void AddConnection(TItem connection, TimeSpan timeout)
            {
                bool closeConnection = false;
                lock (ThisLock)
                {
                    if (!_closed && Parent.OpenCount > 0)
                    {
                        if (!IdleConnections.Add(connection))
                        {
                            closeConnection = true;
                        }
                    }
                    else
                    {
                        closeConnection = true;
                    }
                }

                if (closeConnection)
                {
                    CloseIdleConnection(connection, timeout);
                }
            }

            protected virtual IdleConnectionPool GetIdleConnectionPool()
            {
                return new PoolIdleConnectionPool(Parent.MaxIdleConnectionPoolCount);
            }

            public virtual void Prune(List<TItem> itemsToClose)
            {
            }

            public TItem TakeConnection(TimeSpan timeout)
            {
                TItem item = null;
                List<TItem> itemsToClose = null;
                lock (ThisLock)
                {
                    if (_closed)
                    {
                        return null;
                    }

                    bool closeItem;
                    while (true)
                    {
                        item = IdleConnections.Take(out closeItem);
                        if (item == null)
                        {
                            break;
                        }

                        if (!closeItem)
                        {
                            _busyConnections.Add(item);
                            break;
                        }

                        if (itemsToClose == null)
                        {
                            itemsToClose = new List<TItem>();
                        }
                        itemsToClose.Add(item);
                    }
                }

                // cleanup any stale items accrued from IdleConnections
                if (itemsToClose != null)
                {
                    // and only allocate half the timeout passed in for our graceful shutdowns
                    TimeoutHelper timeoutHelper = new TimeoutHelper(TimeoutHelper.Divide(timeout, 2));
                    for (int i = 0; i < itemsToClose.Count; i++)
                    {
                        CloseIdleConnection(itemsToClose[i], timeoutHelper.RemainingTime());
                    }
                }

                if (WcfEventSource.Instance.ConnectionPoolMissIsEnabled())
                {
                    if (item == null && _busyConnections != null)
                    {
                        WcfEventSource.Instance.ConnectionPoolMiss(Key != null ? Key.ToString() : string.Empty, _busyConnections.Count);
                    }
                }

                return item;
            }

            public void ReturnConnection(TItem connection, bool connectionIsStillGood, TimeSpan timeout)
            {
                bool closeConnection = false;
                bool abortConnection = false;

                lock (ThisLock)
                {
                    if (!_closed)
                    {
                        if (_busyConnections.Remove(connection) && connectionIsStillGood)
                        {
                            if (Parent.OpenCount == 0 || !IdleConnections.Return(connection))
                            {
                                closeConnection = true;
                            }
                        }
                        else
                        {
                            abortConnection = true;
                        }
                    }
                    else
                    {
                        abortConnection = true;
                    }
                }

                if (closeConnection)
                {
                    CloseIdleConnection(connection, timeout);
                }
                else if (abortConnection)
                {
                    AbortItem(connection);
                    OnConnectionAborted();
                }
            }

            public void CloseIdleConnection(TItem connection, TimeSpan timeout)
            {
                bool throwing = true;
                try
                {
                    CloseItemAsync(connection, timeout);
                    throwing = false;
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                }
                finally
                {
                    if (throwing)
                    {
                        AbortItem(connection);
                    }
                }
            }

            protected virtual void OnConnectionAborted()
            {
            }

            protected class PoolIdleConnectionPool
                : IdleConnectionPool
            {
                private Pool<TItem> _idleConnections;
                private readonly int _maxCount;

                public PoolIdleConnectionPool(int maxCount)
                {
                    _idleConnections = new Pool<TItem>(maxCount);
                    _maxCount = maxCount;
                }

                public override int Count
                {
                    get { return _idleConnections.Count; }
                }

                public override bool Add(TItem connection)
                {
                    return ReturnToPool(connection);
                }

                public override bool Return(TItem connection)
                {
                    return ReturnToPool(connection);
                }

                private bool ReturnToPool(TItem connection)
                {
                    bool result = _idleConnections.Return(connection);

                    if (!result)
                    {
                        if (WcfEventSource.Instance.MaxOutboundConnectionsPerEndpointExceededIsEnabled())
                        {
                            WcfEventSource.Instance.MaxOutboundConnectionsPerEndpointExceeded(SR.Format(SR.TraceCodeConnectionPoolMaxOutboundConnectionsPerEndpointQuotaReached, _maxCount));
                        }
                    }
                    else if (WcfEventSource.Instance.OutboundConnectionsPerEndpointRatioIsEnabled())
                    {
                        WcfEventSource.Instance.OutboundConnectionsPerEndpointRatio(_idleConnections.Count, _maxCount);
                    }

                    return result;
                }

                public override TItem Take(out bool closeItem)
                {
                    closeItem = false;
                    TItem ret = _idleConnections.Take();
                    if (WcfEventSource.Instance.OutboundConnectionsPerEndpointRatioIsEnabled())
                    {
                        WcfEventSource.Instance.OutboundConnectionsPerEndpointRatio(_idleConnections.Count, _maxCount);
                    }
                    return ret;
                }
            }
        }
    }

    // all our connection pools support Idling out of connections and lease timeout
    // (though Named Pipes doesn't leverage the lease timeout)
    internal class ConnectionPool : IdlingCommunicationPool<string, IConnection>
    {
        private readonly int _connectionBufferSize;
        private readonly TimeSpan _maxOutputDelay;
        private readonly IConnectionPoolSettings _poolSettings;
        private readonly IConnectionOrientedTransportChannelFactorySettings _settings;
        private readonly PoolNameCache _poolNameCache;

        public ConnectionPool(IConnectionOrientedTransportChannelFactorySettings settings, TimeSpan leaseTimeout)
            : base(settings.MaxOutboundConnectionsPerEndpoint, settings.IdleTimeout, leaseTimeout)
        {
            if (settings is ChannelFactoryBase channelFactoryBase)
            {
                _poolSettings = channelFactoryBase.GetProperty<IConnectionPoolSettings>();
            }
            _connectionBufferSize = settings.ConnectionBufferSize;
            _maxOutputDelay = settings.MaxOutputDelay;
            Name = settings.ConnectionPoolGroupName;
            _settings = settings;
            _poolNameCache = new PoolNameCache();
        }

        public string Name { get; }

        protected override void AbortItem(IConnection item)
        {
            item.Abort();
        }

        protected override async ValueTask CloseItemAsync(IConnection item, TimeSpan timeout)
        {
            try
            {
                await item.CloseAsync(timeout);
            }
            catch (Exception) { }
        }

        public virtual bool IsCompatible(IConnectionOrientedTransportChannelFactorySettings settings)
        {
            if (_poolSettings is not null && settings is ChannelFactoryBase channelFactoryBase)
            {
                var otherPoolSettings = channelFactoryBase.GetProperty<IConnectionPoolSettings>();
                if (otherPoolSettings is not null && !_poolSettings.IsCompatible(otherPoolSettings))
                {
                    return false;
                }
            }
            return
                (Name == settings.ConnectionPoolGroupName) &&
                (_connectionBufferSize == settings.ConnectionBufferSize) &&
                (MaxIdleConnectionPoolCount == settings.MaxOutboundConnectionsPerEndpoint) &&
                (IdleTimeout == settings.IdleTimeout) &&
                (_maxOutputDelay == settings.MaxOutputDelay);
        }

        protected override string GetPoolKey(EndpointAddress address, Uri via)
        {
            string result;
            ThisLock.Wait();
            try
            {
                if (!_poolNameCache.TryGetValue(via, out result))
                {
                    result = _settings.GetConnectionPoolKey(address, via);
                    _poolNameCache.Add(via, result);
                }
            }
            finally { ThisLock.Release(); }

            return result;
        }

        // not thread-safe
        class PoolNameCache
        {
            readonly Dictionary<Uri, string> _forwardTable = new Dictionary<Uri, string>();
            readonly Dictionary<string, ICollection<Uri>> _reverseTable = new Dictionary<string, ICollection<Uri>>();

            public void Add(Uri uri, string pipeName)
            {
                _forwardTable.Add(uri, pipeName);

                ICollection<Uri> uris;
                if (!_reverseTable.TryGetValue(pipeName, out uris))
                {
                    uris = new Collection<Uri>();
                    _reverseTable.Add(pipeName, uris);
                }
                uris.Add(uri);
            }

            public void Clear()
            {
                _forwardTable.Clear();
                _reverseTable.Clear();
            }

            public void Purge(string pipeName)
            {
                ICollection<Uri> uris;
                if (_reverseTable.TryGetValue(pipeName, out uris))
                {
                    _reverseTable.Remove(pipeName);
                    foreach (Uri uri in uris)
                    {
                        _forwardTable.Remove(uri);
                    }
                }
            }

            public bool TryGetValue(Uri uri, out string pipeName)
            {
                return _forwardTable.TryGetValue(uri, out pipeName);
            }
        }
    }
}
