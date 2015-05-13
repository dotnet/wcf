// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ServiceModel.Channels;
using System.Collections.Generic;

namespace System.ServiceModel.Dispatcher
{
    internal class EndpointDispatcherTable
    {
        private object _thisLock;
        private const int optimizationThreshold = 2;
        private List<EndpointDispatcher> _cachedEndpoints;

        public EndpointDispatcherTable(object thisLock)
        {
            _thisLock = thisLock;
        }

        private object ThisLock
        {
            get { return _thisLock; }
        }

        public void AddEndpoint(EndpointDispatcher endpoint)
        {
            lock (ThisLock)
            {
                int priority = endpoint.FilterPriority;

                if (_cachedEndpoints == null)
                {
                    _cachedEndpoints = new List<EndpointDispatcher>(optimizationThreshold);
                }

                if (_cachedEndpoints.Count < optimizationThreshold)
                {
                    _cachedEndpoints.Add(endpoint);
                }
            }
        }

        public void RemoveEndpoint(EndpointDispatcher endpoint)
        {
            lock (ThisLock)
            {
                if (_cachedEndpoints != null && _cachedEndpoints.Contains(endpoint))
                {
                    _cachedEndpoints.Remove(endpoint);
                }
            }
        }

        public EndpointDispatcher Lookup(Message message, out bool addressMatched)
        {
            addressMatched = false;
            return null;
        }
    }
}
