// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    internal class ConnectionPoolRegistry
    {
        private Dictionary<string, List<ConnectionPool>> _registry;

        public ConnectionPoolRegistry()
        {
            _registry = new Dictionary<string, List<ConnectionPool>>();
        }

        private SemaphoreSlim ThisLock { get; } = new SemaphoreSlim(1);

        // NOTE: performs the open on the pool for you
        public ConnectionPool Lookup(IConnectionOrientedTransportChannelFactorySettings settings)
        {
            ConnectionPool result = null;
            string key = settings.ConnectionPoolGroupName;

            ThisLock.Wait();
            try
            {
                List<ConnectionPool> registryEntry = null;

                if (_registry.TryGetValue(key, out registryEntry))
                {
                    for (int i = 0; i < registryEntry.Count; i++)
                    {
                        if (registryEntry[i].IsCompatible(settings) && registryEntry[i].TryOpen())
                        {
                            result = registryEntry[i];
                            break;
                        }
                    }
                }
                else
                {
                    registryEntry = new List<ConnectionPool>();
                    _registry.Add(key, registryEntry);
                }

                if (result == null)
                {
                    result = CreatePool(settings);
                    registryEntry.Add(result);
                }
            }
            finally { ThisLock.Release(); }

            return result;
        }

        protected ConnectionPool CreatePool(IConnectionOrientedTransportChannelFactorySettings settings)
        {
            TimeSpan leaseTimeout = TimeSpan.MaxValue;
            if (settings is ChannelFactoryBase channelFactoryBase)
            {
                var connectionPoolSettings = channelFactoryBase.GetProperty<IConnectionPoolSettings>();
                if (connectionPoolSettings != null)
                {
                    TimeSpan poolLeaseTimeout = connectionPoolSettings.GetConnectionPoolSetting<TimeSpan>("LeaseTimeout");
                    if (poolLeaseTimeout != default) leaseTimeout = poolLeaseTimeout;
                }
            }

            return new ConnectionPool(settings, leaseTimeout);
        }

        public async ValueTask ReleaseAsync(ConnectionPool pool, TimeSpan timeout)
        {
            await ThisLock.WaitAsync();
            try
            {
                if (await pool.CloseAsync(timeout))
                {
                    List<ConnectionPool> registryEntry = _registry[pool.Name];
                    for (int i = 0; i < registryEntry.Count; i++)
                    {
                        if (ReferenceEquals(registryEntry[i], pool))
                        {
                            registryEntry.RemoveAt(i);
                            break;
                        }
                    }

                    if (registryEntry.Count == 0)
                    {
                        _registry.Remove(pool.Name);
                    }
                }
            }
            finally { ThisLock.Release(); }
        }
    }
}


