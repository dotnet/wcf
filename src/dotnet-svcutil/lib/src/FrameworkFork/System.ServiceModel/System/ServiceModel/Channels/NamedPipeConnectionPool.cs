// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime;

namespace System.ServiceModel.Channels
{
    class NamedPipeConnectionPoolRegistry : ConnectionPoolRegistry
    {
        public NamedPipeConnectionPoolRegistry()
            : base()
        {
        }

        protected override ConnectionPool CreatePool(IConnectionOrientedTransportChannelFactorySettings settings)
        {
            Fx.Assert(settings is IPipeTransportFactorySettings, "NamedPipeConnectionPool requires an IPipeTransportFactorySettings.");
            return new NamedPipeConnectionPool((IPipeTransportFactorySettings)settings);
        }

        class NamedPipeConnectionPool : ConnectionPool
        {
            PipeNameCache _pipeNameCache;
            IPipeTransportFactorySettings _transportFactorySettings;

            public NamedPipeConnectionPool(IPipeTransportFactorySettings settings)
                : base(settings, TimeSpan.MaxValue)
            {
                this._pipeNameCache = new PipeNameCache();
                this._transportFactorySettings = settings;
            }

            protected override EndpointConnectionPool CreateEndpointConnectionPool(string key)
            {
                return new NamedPipeEndpointConnectionPool(this, key);
            }

            protected override string GetPoolKey(EndpointAddress address, Uri via)
            {
                string result;
                lock (base.ThisLock)
                {
                    if (!this._pipeNameCache.TryGetValue(via, out result))
                    {
                        result = PipeConnectionInitiator.GetPipeName(via);
                        this._pipeNameCache.Add(via, result);
                    }
                }
                return result;
            }

            protected override void OnClosed()
            {
                base.OnClosed();
                this._pipeNameCache.Clear();
            }

            void OnConnectionAborted(string pipeName)
            {
                // the underlying pipe name may have changed; purge the old one from the cache
                lock (base.ThisLock)
                {
                    this._pipeNameCache.Purge(pipeName);
                }
            }

            protected class NamedPipeEndpointConnectionPool : IdleTimeoutEndpointConnectionPool
            {
                NamedPipeConnectionPool _parent;

                public NamedPipeEndpointConnectionPool(NamedPipeConnectionPool parent, string key)
                    : base(parent, key)
                {
                    this._parent = parent;
                }

                protected override void OnConnectionAborted()
                {
                    _parent.OnConnectionAborted(this.Key);
                }
            }
        }

        // not thread-safe
        class PipeNameCache
        {
            Dictionary<Uri, string> _forwardTable = new Dictionary<Uri, string>();
            Dictionary<string, ICollection<Uri>> _reverseTable = new Dictionary<string, ICollection<Uri>>();

            public void Add(Uri uri, string pipeName)
            {
                this._forwardTable.Add(uri, pipeName);

                ICollection<Uri> uris;
                if (!this._reverseTable.TryGetValue(pipeName, out uris))
                {
                    uris = new Collection<Uri>();
                    this._reverseTable.Add(pipeName, uris);
                }
                uris.Add(uri);
            }

            public void Clear()
            {
                this._forwardTable.Clear();
                this._reverseTable.Clear();
            }

            public void Purge(string pipeName)
            {
                ICollection<Uri> uris;
                if (this._reverseTable.TryGetValue(pipeName, out uris))
                {
                    this._reverseTable.Remove(pipeName);
                    foreach (Uri uri in uris)
                    {
                        this._forwardTable.Remove(uri);
                    }
                }
            }

            public bool TryGetValue(Uri uri, out string pipeName)
            {
                return this._forwardTable.TryGetValue(uri, out pipeName);
            }
        }
    }
}
