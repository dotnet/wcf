// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.ServiceModel.Channels
{
    public abstract class ConnectionPoolRegistry
    {
        private Dictionary<string, List<ConnectionPool>> _registry;

        protected ConnectionPoolRegistry()
        {
            _registry = new Dictionary<string, List<ConnectionPool>>();
        }

        private object ThisLock
        {
            get { return _registry; }
        }

        // NOTE: performs the open on the pool for you
        public ConnectionPool Lookup(IConnectionOrientedTransportChannelFactorySettings settings)
        {
            ConnectionPool result = null;
            string key = settings.ConnectionPoolGroupName;

            lock (ThisLock)
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

            return result;
        }

        protected abstract ConnectionPool CreatePool(IConnectionOrientedTransportChannelFactorySettings settings);

        public void Release(ConnectionPool pool, TimeSpan timeout)
        {
            lock (ThisLock)
            {
                if (pool.Close(timeout))
                {
                    List<ConnectionPool> registryEntry = _registry[pool.Name];
                    for (int i = 0; i < registryEntry.Count; i++)
                    {
                        if (object.ReferenceEquals(registryEntry[i], pool))
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
        }
    }
}


