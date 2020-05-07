// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
using System.Threading.Tasks;

namespace System.ServiceModel.Dispatcher
{
    public sealed class DispatchRuntime
    {
        private ConcurrencyMode _concurrencyMode;
        private bool _ensureOrderedDispatch;
        private bool _automaticInputSessionShutdown;
        private ChannelDispatcher _channelDispatcher;
        private EndpointDispatcher _endpointDispatcher = null;
        private IInstanceProvider _instanceProvider;
        private IInstanceContextProvider _instanceContextProvider;
        private OperationCollection _operations;
        private ClientRuntime _proxyRuntime;
        private ImmutableDispatchRuntime _runtime;
        private SynchronizationContext _synchronizationContext;
        private Type _type;
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
            _instanceProvider = new CallbackInstanceProvider();
            _channelDispatcher = new ChannelDispatcher(shared);
            _instanceContextProvider = InstanceContextProviderBase.GetProviderForMode(InstanceContextMode.PerSession, this);
            Fx.Assert(!shared.IsOnServer, "Client constructor called on server?");
        }

        private DispatchRuntime(SharedRuntimeState shared)
        {
            _shared = shared;

            _operations = new OperationCollection(this);
            _synchronizationContext = ThreadBehavior.GetCurrentSynchronizationContext();
            _automaticInputSessionShutdown = true;

            _unhandled = new DispatchOperation(this, "*", MessageHeaders.WildcardAction, MessageHeaders.WildcardAction);
            _unhandled.InternalFormatter = MessageOperationFormatter.Instance;
            _unhandled.InternalInvoker = new UnhandledActionInvoker(this);
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

        public IInstanceProvider InstanceProvider
        {
            get { return _instanceProvider; }
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _instanceProvider = value;
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

        public Type Type
        {
            get { return _type; }
            set
            {
                lock (this.ThisLock)
                {
                    this.InvalidateRuntime();
                    _type = value;
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

        internal int MaxParameterInspectors
        {
            get
            {
                lock (this.ThisLock)
                {
                    int max = 0;

                    for (int i = 0; i < _operations.Count; i++)
                    {
                        max = System.Math.Max(max, _operations[i].ParameterInspectors.Count);
                    }
                    max = System.Math.Max(max, _unhandled.ParameterInspectors.Count);
                    return max;
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

        internal DispatchOperationRuntime GetOperation(ref Message message)
        {
            ImmutableDispatchRuntime runtime = this.GetRuntime();
            return runtime.GetOperation(ref message);
        }

        internal ImmutableDispatchRuntime GetRuntime()
        {
            ImmutableDispatchRuntime runtime = _runtime;
            if (runtime != null)
            {
                return runtime;
            }
            else
            {
                return GetRuntimeCore();
            }
        }

        private ImmutableDispatchRuntime GetRuntimeCore()
        {
            lock (this.ThisLock)
            {
                if (_runtime == null)
                {
                    _runtime = new ImmutableDispatchRuntime(this);
                }

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
            return new DispatchBehaviorCollection<T>(this);
        }

        internal class UnhandledActionInvoker : IOperationInvoker
        {
            private readonly DispatchRuntime _dispatchRuntime;

            public UnhandledActionInvoker(DispatchRuntime dispatchRuntime)
            {
                _dispatchRuntime = dispatchRuntime;
            }

            public object[] AllocateInputs()
            {
                return new object[1];
            }

            public Task<object> InvokeAsync(object instance, object[] inputs, out object[] outputs)
            {
                outputs = EmptyArray<object>.Allocate(0);

                Message message = inputs[0] as Message;
                if (message == null)
                {
                    return null;
                }

                string action = message.Headers.Action;

                FaultCode code = FaultCode.CreateSenderFaultCode(AddressingStrings.ActionNotSupported,
                    message.Version.Addressing.Namespace);
                string reasonText = string.Format(SRServiceModel.SFxNoEndpointMatchingContract, action);
                FaultReason reason = new FaultReason(reasonText);

                FaultException exception = new FaultException(reason, code);
                ErrorBehavior.ThrowAndCatch(exception);

                ServiceChannel serviceChannel = OperationContext.Current.InternalServiceChannel;
                OperationContext.Current.OperationCompleted +=
                    delegate (object sender, EventArgs e)
                    {
                        ChannelDispatcher channelDispatcher = _dispatchRuntime.ChannelDispatcher;
                        if (!channelDispatcher.HandleError(exception) && serviceChannel.HasSession)
                        {
                            try
                            {
                                serviceChannel.Close(ChannelHandler.CloseAfterFaultTimeout);
                            }
                            catch (Exception ex)
                            {
                                if (Fx.IsFatal(ex))
                                {
                                    throw;
                                }
                                channelDispatcher.HandleError(ex);
                            }
                        }
                    };

                if (_dispatchRuntime._shared.EnableFaults)
                {
                    MessageFault fault = MessageFault.CreateFault(code, reason, action);
                    return Task.FromResult((object)Message.CreateMessage(message.Version, fault, message.Version.Addressing.DefaultFaultAction));
                }
                else
                {
                    OperationContext.Current.RequestContext.Close();
                    OperationContext.Current.RequestContext = null;
                    return Task.FromResult((object)null);
                }
            }

            public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotImplementedException());
            }

            public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotImplementedException());
            }
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRServiceModel.SFxMismatchedOperationParent);
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRServiceModel.SFxMismatchedOperationParent);
                }

                _outer.InvalidateRuntime();
                base.SetItem(index, item);
            }
        }

        private class CallbackInstanceProvider : IInstanceProvider
        {
            object IInstanceProvider.GetInstance(InstanceContext instanceContext)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxCannotActivateCallbackInstace));
            }

            object IInstanceProvider.GetInstance(InstanceContext instanceContext, Message message)
            {
                throw TraceUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxCannotActivateCallbackInstace), message);
            }

            void IInstanceProvider.ReleaseInstance(InstanceContext instanceContext, object instance)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxCannotActivateCallbackInstace));
            }
        }
    }
}
