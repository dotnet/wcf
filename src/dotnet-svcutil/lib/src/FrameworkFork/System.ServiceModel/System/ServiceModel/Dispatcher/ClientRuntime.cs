// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Net;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Security;
using Microsoft.Xml;

namespace System.ServiceModel.Dispatcher
{
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "Compat is an accepted abbreviation")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ClientRuntimeCompatBase
    {
        internal ClientRuntimeCompatBase() { }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
        public IList<IClientMessageInspector> MessageInspectors
        {
            get
            {
                return this.messageInspectors;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
        public KeyedCollection<string, ClientOperation> Operations
        {
            get
            {
                return this.compatOperations;
            }
        }
        internal SynchronizedCollection<IClientMessageInspector> messageInspectors;
        internal SynchronizedKeyedCollection<string, ClientOperation> operations;
        internal KeyedCollection<string, ClientOperation> compatOperations;
    }

    public sealed class ClientRuntime : ClientRuntimeCompatBase
    {
        private bool _addTransactionFlowProperties = true;
        private Type _callbackProxyType;
        private ProxyBehaviorCollection<IChannelInitializer> _channelInitializers;
        private string _contractName;
        private string _contractNamespace;
        private Type _contractProxyType;
        private DispatchRuntime _dispatchRuntime;
        private IdentityVerifier _identityVerifier;
        private ProxyBehaviorCollection<IInteractiveChannelInitializer> _interactiveChannelInitializers;

        private IClientOperationSelector _operationSelector;
        private ImmutableClientRuntime _runtime;
        private ClientOperation _unhandled;
        private bool _useSynchronizationContext = true;
        private Uri _via;
        private SharedRuntimeState _shared;
        private int _maxFaultSize;
        private bool _messageVersionNoneFaultsEnabled;

        internal ClientRuntime(DispatchRuntime dispatchRuntime, SharedRuntimeState shared)
            : this(dispatchRuntime.EndpointDispatcher.ContractName,
                   dispatchRuntime.EndpointDispatcher.ContractNamespace,
                   shared)
        {
            if (dispatchRuntime == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("dispatchRuntime");

            _dispatchRuntime = dispatchRuntime;
            _shared = shared;

            Fx.Assert(shared.IsOnServer, "Server constructor called on client?");
        }

        internal ClientRuntime(string contractName, string contractNamespace)
            : this(contractName, contractNamespace, new SharedRuntimeState(false))
        {
            Fx.Assert(!_shared.IsOnServer, "Client constructor called on server?");
        }

        private ClientRuntime(string contractName, string contractNamespace, SharedRuntimeState shared)
        {
            _contractName = contractName;
            _contractNamespace = contractNamespace;
            _shared = shared;

            OperationCollection operations = new OperationCollection(this);
            this.operations = operations;
            this.compatOperations = new OperationCollectionWrapper(operations);
            _channelInitializers = new ProxyBehaviorCollection<IChannelInitializer>(this);
            this.messageInspectors = new ProxyBehaviorCollection<IClientMessageInspector>(this);
            _interactiveChannelInitializers = new ProxyBehaviorCollection<IInteractiveChannelInitializer>(this);

            _unhandled = new ClientOperation(this, "*", MessageHeaders.WildcardAction, MessageHeaders.WildcardAction);
            _unhandled.InternalFormatter = new MessageOperationFormatter();
            _maxFaultSize = TransportDefaults.MaxFaultSize;
        }

        internal bool AddTransactionFlowProperties
        {
            get { return _addTransactionFlowProperties; }
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _addTransactionFlowProperties = value;
                }
            }
        }

        public Type CallbackClientType
        {
            get { return _callbackProxyType; }
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _callbackProxyType = value;
                }
            }
        }

        public SynchronizedCollection<IChannelInitializer> ChannelInitializers
        {
            get { return _channelInitializers; }
        }

        public string ContractName
        {
            get { return _contractName; }
        }

        public string ContractNamespace
        {
            get { return _contractNamespace; }
        }

        public Type ContractClientType
        {
            get { return _contractProxyType; }
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _contractProxyType = value;
                }
            }
        }

        internal IdentityVerifier IdentityVerifier
        {
            get
            {
                if (_identityVerifier == null)
                {
                    _identityVerifier = IdentityVerifier.CreateDefault();
                }

                return _identityVerifier;
            }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                this.InvalidateRuntime();

                _identityVerifier = value;
            }
        }

        public Uri Via
        {
            get { return _via; }
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _via = value;
                }
            }
        }

        public bool ValidateMustUnderstand
        {
            get { return _shared.ValidateMustUnderstand; }
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _shared.ValidateMustUnderstand = value;
                }
            }
        }

        public bool MessageVersionNoneFaultsEnabled
        {
            get
            {
                return _messageVersionNoneFaultsEnabled;
            }
            set
            {
                this.InvalidateRuntime();
                _messageVersionNoneFaultsEnabled = value;
            }
        }

        public DispatchRuntime DispatchRuntime
        {
            get { return _dispatchRuntime; }
        }

        public DispatchRuntime CallbackDispatchRuntime
        {
            get
            {
                if (_dispatchRuntime == null)
                    _dispatchRuntime = new DispatchRuntime(this, _shared);

                return _dispatchRuntime;
            }
        }

        internal bool EnableFaults
        {
            get
            {
                if (this.IsOnServer)
                {
                    return _dispatchRuntime.EnableFaults;
                }
                else
                {
                    return _shared.EnableFaults;
                }
            }
            set
            {
                lock (this.ThisLock)
                {
                    if (this.IsOnServer)
                    {
                        string text = SRServiceModel.SFxSetEnableFaultsOnChannelDispatcher0;
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(text));
                    }
                    else
                    {
                        this.InvalidateRuntime();
                        _shared.EnableFaults = value;
                    }
                }
            }
        }

        public SynchronizedCollection<IInteractiveChannelInitializer> InteractiveChannelInitializers
        {
            get { return _interactiveChannelInitializers; }
        }

        public int MaxFaultSize
        {
            get
            {
                return _maxFaultSize;
            }
            set
            {
                this.InvalidateRuntime();
                _maxFaultSize = value;
            }
        }

        internal bool IsOnServer
        {
            get { return _shared.IsOnServer; }
        }

        public bool ManualAddressing
        {
            get
            {
                if (this.IsOnServer)
                {
                    return _dispatchRuntime.ManualAddressing;
                }
                else
                {
                    return _shared.ManualAddressing;
                }
            }
            set
            {
                lock (this.ThisLock)
                {
                    if (this.IsOnServer)
                    {
                        string text = SRServiceModel.SFxSetManualAddresssingOnChannelDispatcher0;
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(text));
                    }
                    else
                    {
                        this.InvalidateRuntime();
                        _shared.ManualAddressing = value;
                    }
                }
            }
        }

        internal int MaxParameterInspectors
        {
            get
            {
                lock (this.ThisLock)
                {
                    int max = 0;

                    for (int i = 0; i < this.operations.Count; i++)
                        max = System.Math.Max(max, this.operations[i].ParameterInspectors.Count);

                    return max;
                }
            }
        }

        public ICollection<IClientMessageInspector> ClientMessageInspectors
        {
            get { return this.MessageInspectors; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new SynchronizedCollection<IClientMessageInspector> MessageInspectors
        {
            get { return this.messageInspectors; }
        }

        public ICollection<ClientOperation> ClientOperations
        {
            get { return this.Operations; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public new SynchronizedKeyedCollection<string, ClientOperation> Operations
        {
            get { return this.operations; }
        }

        public IClientOperationSelector OperationSelector
        {
            get { return _operationSelector; }
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _operationSelector = value;
                }
            }
        }

        internal object ThisLock
        {
            get { return _shared; }
        }

        public ClientOperation UnhandledClientOperation
        {
            get { return _unhandled; }
        }

        internal bool UseSynchronizationContext
        {
            get { return _useSynchronizationContext; }
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _useSynchronizationContext = value;
                }
            }
        }

        internal T[] GetArray<T>(SynchronizedCollection<T> collection)
        {
            lock (collection.SyncRoot)
            {
                if (collection.Count == 0)
                {
                    return Array.Empty<T>();
                }
                else
                {
                    T[] array = new T[collection.Count];
                    collection.CopyTo(array, 0);
                    return array;
                }
            }
        }

        internal ImmutableClientRuntime GetRuntime()
        {
            lock (this.ThisLock)
            {
                if (_runtime == null)
                    _runtime = new ImmutableClientRuntime(this);

                return _runtime;
            }
        }

        internal void InvalidateRuntime()
        {
            lock (this.ThisLock)
            {
                _shared.ThrowIfImmutable();
                _runtime = null;
            }
        }

        internal void LockDownProperties()
        {
            _shared.LockDownProperties();
        }

        internal SynchronizedCollection<T> NewBehaviorCollection<T>()
        {
            return new ProxyBehaviorCollection<T>(this);
        }

        internal bool IsFault(ref Message reply)
        {
            if (reply == null)
            {
                return false;
            }
            if (reply.IsFault)
            {
                return true;
            }
            if (this.MessageVersionNoneFaultsEnabled && IsMessageVersionNoneFault(ref reply, this.MaxFaultSize))
            {
                return true;
            }

            return false;
        }

        internal static bool IsMessageVersionNoneFault(ref Message message, int maxFaultSize)
        {
            if (message.Version != MessageVersion.None || message.IsEmpty)
            {
                return false;
            }
            HttpResponseMessageProperty prop = message.Properties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
            if (prop == null || prop.StatusCode != HttpStatusCode.InternalServerError)
            {
                return false;
            }
            using (MessageBuffer buffer = message.CreateBufferedCopy(maxFaultSize))
            {
                message.Close();
                message = buffer.CreateMessage();
                using (Message copy = buffer.CreateMessage())
                {
                    using (XmlDictionaryReader reader = copy.GetReaderAtBodyContents())
                    {
                        return reader.IsStartElement(XD.MessageDictionary.Fault, MessageVersion.None.Envelope.DictionaryNamespace);
                    }
                }
            }
        }

        internal class ProxyBehaviorCollection<T> : SynchronizedCollection<T>
        {
            private ClientRuntime _outer;

            internal ProxyBehaviorCollection(ClientRuntime outer)
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

        internal class OperationCollection : SynchronizedKeyedCollection<string, ClientOperation>
        {
            private ClientRuntime _outer;

            internal OperationCollection(ClientRuntime outer)
                : base(outer.ThisLock)
            {
                _outer = outer;
            }

            protected override void ClearItems()
            {
                _outer.InvalidateRuntime();
                base.ClearItems();
            }

            protected override string GetKeyForItem(ClientOperation item)
            {
                return item.Name;
            }

            protected override void InsertItem(int index, ClientOperation item)
            {
                if (item == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
                if (item.Parent != _outer)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRServiceModel.SFxMismatchedOperationParent);

                _outer.InvalidateRuntime();
                base.InsertItem(index, item);
            }

            protected override void RemoveItem(int index)
            {
                _outer.InvalidateRuntime();
                base.RemoveItem(index);
            }

            protected override void SetItem(int index, ClientOperation item)
            {
                if (item == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
                if (item.Parent != _outer)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRServiceModel.SFxMismatchedOperationParent);

                _outer.InvalidateRuntime();
                base.SetItem(index, item);
            }

            internal void InternalClearItems() { this.ClearItems(); }
            internal string InternalGetKeyForItem(ClientOperation item) { return this.GetKeyForItem(item); }
            internal void InternalInsertItem(int index, ClientOperation item) { this.InsertItem(index, item); }
            internal void InternalRemoveItem(int index) { this.RemoveItem(index); }
            internal void InternalSetItem(int index, ClientOperation item) { this.SetItem(index, item); }
        }


        internal class OperationCollectionWrapper : KeyedCollection<string, ClientOperation>
        {
            private OperationCollection _inner;
            internal OperationCollectionWrapper(OperationCollection inner) { _inner = inner; }
            protected override void ClearItems() { _inner.InternalClearItems(); }
            protected override string GetKeyForItem(ClientOperation item) { return _inner.InternalGetKeyForItem(item); }
            protected override void InsertItem(int index, ClientOperation item) { _inner.InternalInsertItem(index, item); }
            protected override void RemoveItem(int index) { _inner.InternalRemoveItem(index); }
            protected override void SetItem(int index, ClientOperation item) { _inner.InternalSetItem(index, item); }
        }
    }
}
