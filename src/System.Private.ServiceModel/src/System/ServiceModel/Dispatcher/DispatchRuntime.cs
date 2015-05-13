// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IdentityModel.Policy;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.Threading;
using System.Runtime.Diagnostics;
using System.Diagnostics.Contracts;

namespace System.ServiceModel.Dispatcher
{
    public sealed class DispatchRuntime
    {
        private ConcurrencyMode _concurrencyMode;
        private bool _ensureOrderedDispatch;
        private bool _automaticInputSessionShutdown;
        private ChannelDispatcher _channelDispatcher;
        private EndpointDispatcher _endpointDispatcher = null;
        private IInstanceContextProvider _instanceContextProvider;
        private bool _ignoreTransactionMessageProperty;
        private OperationCollection _operations;
        private ClientRuntime _proxyRuntime;
        private SynchronizationContext _synchronizationContext;
        private DispatchOperation _unhandled;
        private SharedRuntimeState _shared;

        internal DispatchRuntime(ClientRuntime proxyRuntime, SharedRuntimeState shared)
            : this(shared)
        {
            if (proxyRuntime == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("proxyRuntime");
            }

            _proxyRuntime = proxyRuntime;
            _channelDispatcher = new ChannelDispatcher(shared);
            Fx.Assert(!shared.IsOnServer, "Client constructor called on server?");
        }

        private DispatchRuntime(SharedRuntimeState shared)
        {
            _shared = shared;

            _operations = new OperationCollection(this);
            _synchronizationContext = ThreadBehavior.GetCurrentSynchronizationContext();
            _automaticInputSessionShutdown = true;

            _unhandled = new DispatchOperation(this, "*", MessageHeaders.WildcardAction, MessageHeaders.WildcardAction);
        }

        public IInstanceContextProvider InstanceContextProvider
        {
            get
            {
                return _instanceContextProvider;
            }

            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("value"));
                }

                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _instanceContextProvider = value;
                }
            }
        }

        public ConcurrencyMode ConcurrencyMode
        {
            get
            {
                return _concurrencyMode;
            }
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _concurrencyMode = value;
                }
            }
        }


        public bool EnsureOrderedDispatch
        {
            get
            {
                return _ensureOrderedDispatch;
            }
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _ensureOrderedDispatch = value;
                }
            }
        }

        public bool AutomaticInputSessionShutdown
        {
            get { return _automaticInputSessionShutdown; }
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _automaticInputSessionShutdown = value;
                }
            }
        }

        public ChannelDispatcher ChannelDispatcher
        {
            get { return _channelDispatcher ?? _endpointDispatcher.ChannelDispatcher; }
        }

        public ClientRuntime CallbackClientRuntime
        {
            get
            {
                if (_proxyRuntime == null)
                {
                    lock (this.ThisLock)
                    {
                        if (_proxyRuntime == null)
                        {
                            _proxyRuntime = new ClientRuntime(this, _shared);
                        }
                    }
                }

                return _proxyRuntime;
            }
        }

        public EndpointDispatcher EndpointDispatcher
        {
            get { return _endpointDispatcher; }
        }

        public bool IgnoreTransactionMessageProperty
        {
            get { return _ignoreTransactionMessageProperty; }
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _ignoreTransactionMessageProperty = value;
                }
            }
        }

        public SynchronizedKeyedCollection<string, DispatchOperation> Operations
        {
            get { return _operations; }
        }

        public SynchronizationContext SynchronizationContext
        {
            get { return _synchronizationContext; }
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _synchronizationContext = value;
                }
            }
        }

        public DispatchOperation UnhandledDispatchOperation
        {
            get { return _unhandled; }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }

                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _unhandled = value;
                }
            }
        }

        internal bool HasMatchAllOperation
        {
            get
            {
                return false;
            }
        }

        internal bool EnableFaults
        {
            get
            {
                if (this.IsOnServer)
                {
                    ChannelDispatcher channelDispatcher = this.ChannelDispatcher;
                    return (channelDispatcher != null) && channelDispatcher.EnableFaults;
                }
                else
                {
                    return _shared.EnableFaults;
                }
            }
        }

        internal bool IsOnServer
        {
            get { return _shared.IsOnServer; }
        }

        internal bool ManualAddressing
        {
            get
            {
                if (this.IsOnServer)
                {
                    ChannelDispatcher channelDispatcher = this.ChannelDispatcher;
                    return (channelDispatcher != null) && channelDispatcher.ManualAddressing;
                }
                else
                {
                    return _shared.ManualAddressing;
                }
            }
        }

        // Internal access to CallbackClientRuntime, but this one doesn't create on demand
        internal ClientRuntime ClientRuntime
        {
            get { return _proxyRuntime; }
        }

        internal object ThisLock
        {
            get { return _shared; }
        }



        internal void InvalidateRuntime()
        {
            lock (this.ThisLock)
            {
                _shared.ThrowIfImmutable();
            }
        }

        internal void LockDownProperties()
        {
            _shared.LockDownProperties();
        }

        internal SynchronizedCollection<T> NewBehaviorCollection<T>()
        {
            return new DispatchBehaviorCollection<T>(this);
        }

        internal class DispatchBehaviorCollection<T> : SynchronizedCollection<T>
        {
            private DispatchRuntime _outer;

            internal DispatchBehaviorCollection(DispatchRuntime outer)
                : base(outer.ThisLock)
            {
                _outer = outer;
            }

            protected override void ClearItems()
            {
                _outer.InvalidateRuntime();
                base.ClearItems();
            }

            protected override void InsertItem(int index, T item)
            {
                if (item == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
                }

                _outer.InvalidateRuntime();
                base.InsertItem(index, item);
            }

            protected override void RemoveItem(int index)
            {
                _outer.InvalidateRuntime();
                base.RemoveItem(index);
            }

            protected override void SetItem(int index, T item)
            {
                if (item == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
                }

                _outer.InvalidateRuntime();
                base.SetItem(index, item);
            }
        }

        internal class OperationCollection : SynchronizedKeyedCollection<string, DispatchOperation>
        {
            private DispatchRuntime _outer;

            internal OperationCollection(DispatchRuntime outer)
                : base(outer.ThisLock)
            {
                _outer = outer;
            }

            protected override void ClearItems()
            {
                _outer.InvalidateRuntime();
                base.ClearItems();
            }

            protected override string GetKeyForItem(DispatchOperation item)
            {
                return item.Name;
            }

            protected override void InsertItem(int index, DispatchOperation item)
            {
                if (item == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
                }
                if (item.Parent != _outer)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.SFxMismatchedOperationParent);
                }

                _outer.InvalidateRuntime();
                base.InsertItem(index, item);
            }

            protected override void RemoveItem(int index)
            {
                _outer.InvalidateRuntime();
                base.RemoveItem(index);
            }

            protected override void SetItem(int index, DispatchOperation item)
            {
                if (item == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
                }
                if (item.Parent != _outer)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.SFxMismatchedOperationParent);
                }

                _outer.InvalidateRuntime();
                base.SetItem(index, item);
            }
        }
    }
}
