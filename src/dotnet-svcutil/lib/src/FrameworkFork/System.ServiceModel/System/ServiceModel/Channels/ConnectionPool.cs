// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Channels
{
    // code that pools items and closes/aborts them as necessary.
    // shared by IConnection and IChannel users
    public abstract class CommunicationPool<TKey, TItem>
        where TKey : class
        where TItem : class
    {
        private Dictionary<TKey, EndpointConnectionPool> _endpointPools;
        private int _maxCount;
        private int _openCount;
        // need to make sure we prune over a certain number of endpoint pools
        private int _pruneAccrual;
        private const int pruneThreshold = 30;

        protected CommunicationPool(int maxCount)
        {
            _maxCount = maxCount;
            _endpointPools = new Dictionary<TKey, EndpointConnectionPool>();
            _openCount = 1;
        }

        public int MaxIdleConnectionPoolCount
        {
            get { return _maxCount; }
        }

        protected object ThisLock
        {
            get { return this; }
        }

        protected abstract void AbortItem(TItem item);

        protected abstract void CloseItem(TItem item, TimeSpan timeout);
        protected abstract void CloseItemAsync(TItem item, TimeSpan timeout);

        protected abstract TKey GetPoolKey(EndpointAddress address, Uri via);

        protected virtual EndpointConnectionPool CreateEndpointConnectionPool(TKey key)
        {
            return new EndpointConnectionPool(this, key);
        }

        public bool Close(TimeSpan timeout)
        {
            lock (ThisLock)
            {
                if (_openCount <= 0)
                {
                    return true;
                }

                _openCount--;

                if (_openCount == 0)
                {
                    this.OnClose(timeout);
                    return true;
                }

                return false;
            }
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
            lock (ThisLock)
            {
                if (!_endpointPools.TryGetValue(key, out result))
                {
                    itemsToClose = PruneIfNecessary();
                    result = CreateEndpointConnectionPool(key);
                    _endpointPools.Add(key, result);
                }
            }

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
            lock (ThisLock)
            {
                if (_openCount <= 0)
                {
                    // can't reopen connection pools since the registry purges them on close
                    return false;
                }
                else
                {
                    _openCount++;
                    return true;
                }
            }
        }

        protected virtual void OnClosed()
        {
        }

        private void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            foreach (EndpointConnectionPool pool in _endpointPools.Values)
            {
                try
                {
                    pool.Close(timeoutHelper.RemainingTime());
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
            key = this.GetPoolKey(address, via);
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
            private TKey _key;
            private List<TItem> _busyConnections;
            private bool _closed;
            private IdleConnectionPool _idleConnections;
            private CommunicationPool<TKey, TItem> _parent;

            public EndpointConnectionPool(CommunicationPool<TKey, TItem> parent, TKey key)
            {
                _key = key;
                _parent = parent;
                _busyConnections = new List<TItem>();
            }

            protected TKey Key
            {
                get { return _key; }
            }

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

            protected CommunicationPool<TKey, TItem> Parent
            {
                get { return _parent; }
            }

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
                _parent.AbortItem(item);
            }

            protected virtual void CloseItem(TItem item, TimeSpan timeout)
            {
                _parent.CloseItem(item, timeout);
            }

            protected virtual void CloseItemAsync(TItem item, TimeSpan timeout)
            {
                _parent.CloseItemAsync(item, timeout);
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
                        return;

                    _closed = true;
                    idleItemsToClose = SnapshotIdleConnections();
                }

                AbortConnections(idleItemsToClose);
            }

            public void Close(TimeSpan timeout)
            {
                List<TItem> itemsToClose = null;
                lock (ThisLock)
                {
                    if (_closed)
                        return;

                    _closed = true;
                    itemsToClose = SnapshotIdleConnections();
                }

                try
                {
                    TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                    for (int i = 0; i < itemsToClose.Count; i++)
                    {
                        this.CloseItem(itemsToClose[i], timeoutHelper.RemainingTime());
                    }

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
                    this.AbortItem(idleItemsToClose[i]);
                }

                for (int i = 0; i < _busyConnections.Count; i++)
                {
                    this.AbortItem(_busyConnections[i]);
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
                        break;

                    itemsToClose.Add(item);
                }

                return itemsToClose;
            }

            public void AddConnection(TItem connection, TimeSpan timeout)
            {
                bool closeConnection = false;
                lock (ThisLock)
                {
                    if (!_closed)
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
                return new PoolIdleConnectionPool(_parent.MaxIdleConnectionPoolCount);
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
                        return null;

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
                        WcfEventSource.Instance.ConnectionPoolMiss(_key != null ? _key.ToString() : string.Empty, _busyConnections.Count);
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
                            if (!IdleConnections.Return(connection))
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
                    this.AbortItem(connection);
                    OnConnectionAborted();
                }
            }

            public void CloseIdleConnection(TItem connection, TimeSpan timeout)
            {
                bool throwing = true;
                try
                {
                    this.CloseItemAsync(connection, timeout);
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
                        this.AbortItem(connection);
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
                private int _maxCount;

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
                            WcfEventSource.Instance.MaxOutboundConnectionsPerEndpointExceeded(string.Format(SRServiceModel.TraceCodeConnectionPoolMaxOutboundConnectionsPerEndpointQuotaReached, _maxCount));
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
    public abstract class ConnectionPool : IdlingCommunicationPool<string, IConnection>
    {
        private int _connectionBufferSize;
        private TimeSpan _maxOutputDelay;
        private string _name;

        protected ConnectionPool(IConnectionOrientedTransportChannelFactorySettings settings, TimeSpan leaseTimeout)
            : base(settings.MaxOutboundConnectionsPerEndpoint, settings.IdleTimeout, leaseTimeout)
        {
            _connectionBufferSize = settings.ConnectionBufferSize;
            _maxOutputDelay = settings.MaxOutputDelay;
            _name = settings.ConnectionPoolGroupName;
        }

        public string Name
        {
            get { return _name; }
        }

        protected override void AbortItem(IConnection item)
        {
            item.Abort();
        }

        protected override void CloseItem(IConnection item, TimeSpan timeout)
        {
            item.Close(timeout, false);
        }

        protected override void CloseItemAsync(IConnection item, TimeSpan timeout)
        {
            item.Close(timeout, true);
        }

        public virtual bool IsCompatible(IConnectionOrientedTransportChannelFactorySettings settings)
        {
            return (
                (_name == settings.ConnectionPoolGroupName) &&
                (_connectionBufferSize == settings.ConnectionBufferSize) &&
                (this.MaxIdleConnectionPoolCount == settings.MaxOutboundConnectionsPerEndpoint) &&
                (this.IdleTimeout == settings.IdleTimeout) &&
                (_maxOutputDelay == settings.MaxOutputDelay)
                );
        }
    }
}
