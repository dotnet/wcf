// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.ServiceModel.Diagnostics;
using System.Threading;

namespace System.ServiceModel.Channels
{
    public abstract class IdlingCommunicationPool<TKey, TItem>
        : CommunicationPool<TKey, TItem>
        where TKey : class
        where TItem : class
    {
        private TimeSpan _idleTimeout;
        private TimeSpan _leaseTimeout;

        protected IdlingCommunicationPool(int maxCount, TimeSpan idleTimeout, TimeSpan leaseTimeout)
            : base(maxCount)
        {
            _idleTimeout = idleTimeout;
            _leaseTimeout = leaseTimeout;
        }

        public TimeSpan IdleTimeout
        {
            get { return _idleTimeout; }
        }

        protected TimeSpan LeaseTimeout
        {
            get { return _leaseTimeout; }
        }

        protected override void CloseItemAsync(TItem item, TimeSpan timeout)
        {
            // Default behavior is sync. Derived classes can override.
            this.CloseItem(item, timeout);
        }

        protected override EndpointConnectionPool CreateEndpointConnectionPool(TKey key)
        {
            if (_idleTimeout != TimeSpan.MaxValue || _leaseTimeout != TimeSpan.MaxValue)
            {
                return new IdleTimeoutEndpointConnectionPool(this, key);
            }
            else
            {
                return base.CreateEndpointConnectionPool(key);
            }
        }

        protected class IdleTimeoutEndpointConnectionPool : EndpointConnectionPool
        {
            private IdleTimeoutIdleConnectionPool _connections;

            public IdleTimeoutEndpointConnectionPool(IdlingCommunicationPool<TKey, TItem> parent, TKey key)
                : base(parent, key)
            {
                _connections = new IdleTimeoutIdleConnectionPool(this, this.ThisLock);
            }

            protected override IdleConnectionPool GetIdleConnectionPool()
            {
                return _connections;
            }

            protected override void AbortItem(TItem item)
            {
                _connections.OnItemClosing(item);
                base.AbortItem(item);
            }

            protected override void CloseItemAsync(TItem item, TimeSpan timeout)
            {
                _connections.OnItemClosing(item);
                base.CloseItemAsync(item, timeout);
            }

            protected override void CloseItem(TItem item, TimeSpan timeout)
            {
                _connections.OnItemClosing(item);
                base.CloseItem(item, timeout);
            }

            public override void Prune(List<TItem> itemsToClose)
            {
                if (_connections != null)
                {
                    _connections.Prune(itemsToClose, false);
                }
            }

            protected class IdleTimeoutIdleConnectionPool : PoolIdleConnectionPool
            {
                // for performance reasons we don't just blindly start a timer up to clean up 
                // idle connections. However, if we're above a certain threshold of connections
                private const int timerThreshold = 1;

                private IdleTimeoutEndpointConnectionPool _parent;
                private TimeSpan _idleTimeout;
                private TimeSpan _leaseTimeout;
                private Timer _idleTimer;
                private static Action<object> s_onIdle;
                private object _thisLock;
                private Exception _pendingException;

                // Note that Take/Add/Return are already synchronized by ThisLock, so we don't need an extra
                // lock around our Dictionary access
                private Dictionary<TItem, IdlingConnectionSettings> _connectionMapping;

                public IdleTimeoutIdleConnectionPool(IdleTimeoutEndpointConnectionPool parent, object thisLock)
                    : base(parent.Parent.MaxIdleConnectionPoolCount)
                {
                    _parent = parent;
                    IdlingCommunicationPool<TKey, TItem> idlingCommunicationPool = ((IdlingCommunicationPool<TKey, TItem>)parent.Parent);
                    _idleTimeout = idlingCommunicationPool._idleTimeout;
                    _leaseTimeout = idlingCommunicationPool._leaseTimeout;
                    _thisLock = thisLock;
                    _connectionMapping = new Dictionary<TItem, IdlingConnectionSettings>();
                }

                public override bool Add(TItem connection)
                {
                    this.ThrowPendingException();

                    bool result = base.Add(connection);
                    if (result)
                    {
                        _connectionMapping.Add(connection, new IdlingConnectionSettings());
                        StartTimerIfNecessary();
                    }
                    return result;
                }

                public override bool Return(TItem connection)
                {
                    this.ThrowPendingException();

                    if (!_connectionMapping.ContainsKey(connection))
                    {
                        return false;
                    }

                    bool result = base.Return(connection);
                    if (result)
                    {
                        _connectionMapping[connection].LastUsage = DateTime.UtcNow;
                        StartTimerIfNecessary();
                    }
                    return result;
                }

                public override TItem Take(out bool closeItem)
                {
                    this.ThrowPendingException();

                    DateTime now = DateTime.UtcNow;
                    TItem item = base.Take(out closeItem);

                    if (!closeItem)
                    {
                        closeItem = IdleOutConnection(item, now);
                    }
                    return item;
                }

                public void OnItemClosing(TItem connection)
                {
                    this.ThrowPendingException();

                    lock (_thisLock)
                    {
                        _connectionMapping.Remove(connection);
                    }
                }

                private void CancelTimer()
                {
                    if (_idleTimer != null)
                    {
                        _idleTimer.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
                    }
                }

                private void StartTimerIfNecessary()
                {
                    if (this.Count > timerThreshold)
                    {
                        if (_idleTimer == null)
                        {
                            if (s_onIdle == null)
                            {
                                s_onIdle = new Action<object>(OnIdle);
                            }

                            _idleTimer = new Timer(new TimerCallback(new Action<object>(s_onIdle)), this, _idleTimeout, TimeSpan.FromMilliseconds(-1));
                        }
                        else
                        {
                            _idleTimer.Change(_idleTimeout, TimeSpan.FromMilliseconds(-1));
                        }
                    }
                }

                private static void OnIdle(object state)
                {
                    IdleTimeoutIdleConnectionPool pool = (IdleTimeoutIdleConnectionPool)state;
                    pool.OnIdle();
                }

                private void OnIdle()
                {
                    List<TItem> itemsToClose = new List<TItem>();
                    lock (_thisLock)
                    {
                        try
                        {
                            this.Prune(itemsToClose, true);
                        }
                        catch (Exception e)
                        {
                            if (Fx.IsFatal(e))
                            {
                                throw;
                            }
                            _pendingException = e;
                            this.CancelTimer();
                        }
                    }

                    // allocate half the idle timeout for our graceful shutdowns
                    TimeoutHelper timeoutHelper = new TimeoutHelper(TimeoutHelper.Divide(_idleTimeout, 2));
                    for (int i = 0; i < itemsToClose.Count; i++)
                    {
                        _parent.CloseIdleConnection(itemsToClose[i], timeoutHelper.RemainingTime());
                    }
                }

                public void Prune(List<TItem> itemsToClose, bool calledFromTimer)
                {
                    if (!calledFromTimer)
                    {
                        this.ThrowPendingException();
                    }

                    if (this.Count == 0)
                        return;

                    DateTime now = DateTime.UtcNow;
                    bool setTimer = false;

                    lock (_thisLock)
                    {
                        TItem[] connectionsCopy = new TItem[this.Count];
                        for (int i = 0; i < connectionsCopy.Length; i++)
                        {
                            bool closeItem;
                            connectionsCopy[i] = base.Take(out closeItem);
                            Fx.Assert(connectionsCopy[i] != null, "IdleConnections should only be modified under thisLock");
                            if (closeItem || IdleOutConnection(connectionsCopy[i], now))
                            {
                                itemsToClose.Add(connectionsCopy[i]);
                                connectionsCopy[i] = null;
                            }
                        }

                        for (int i = 0; i < connectionsCopy.Length; i++)
                        {
                            if (connectionsCopy[i] != null)
                            {
                                bool successfulReturn = base.Return(connectionsCopy[i]);
                                Fx.Assert(successfulReturn, "IdleConnections should only be modified under thisLock");
                            }
                        }

                        setTimer = (this.Count > 0);
                    }

                    if (calledFromTimer && setTimer)
                    {
                        _idleTimer.Change(_idleTimeout, TimeSpan.FromMilliseconds(-1));
                    }
                }

                private bool IdleOutConnection(TItem connection, DateTime now)
                {
                    if (connection == null)
                    {
                        return false;
                    }

                    bool result = false;
                    IdlingConnectionSettings idlingSettings = _connectionMapping[connection];
                    if (now > (idlingSettings.LastUsage + _idleTimeout))
                    {
                        TraceConnectionIdleTimeoutExpired();
                        result = true;
                    }
                    else if (now - idlingSettings.CreationTime >= _leaseTimeout)
                    {
                        TraceConnectionLeaseTimeoutExpired();
                        result = true;
                    }

                    return result;
                }

                private void ThrowPendingException()
                {
                    if (_pendingException != null)
                    {
                        lock (_thisLock)
                        {
                            if (_pendingException != null)
                            {
                                Exception exceptionToThrow = _pendingException;
                                _pendingException = null;
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exceptionToThrow);
                            }
                        }
                    }
                }

                private void TraceConnectionLeaseTimeoutExpired()
                {
                    if (WcfEventSource.Instance.LeaseTimeoutIsEnabled())
                    {
                        WcfEventSource.Instance.LeaseTimeout(string.Format(SRServiceModel.TraceCodeConnectionPoolLeaseTimeoutReached, _leaseTimeout), _parent.Key.ToString());
                    }
                }

                private void TraceConnectionIdleTimeoutExpired()
                {
                    if (WcfEventSource.Instance.IdleTimeoutIsEnabled())
                    {
                        WcfEventSource.Instance.IdleTimeout(string.Format(SRServiceModel.TraceCodeConnectionPoolIdleTimeoutReached, _idleTimeout), _parent.Key.ToString());
                    }
                }

                internal class IdlingConnectionSettings
                {
                    private DateTime _creationTime;
                    private DateTime _lastUsage;

                    public IdlingConnectionSettings()
                    {
                        _creationTime = DateTime.UtcNow;
                        _lastUsage = _creationTime;
                    }

                    public DateTime CreationTime
                    {
                        get { return _creationTime; }
                    }

                    public DateTime LastUsage
                    {
                        get { return _lastUsage; }
                        set { _lastUsage = value; }
                    }
                }
            }
        }
    }
}
