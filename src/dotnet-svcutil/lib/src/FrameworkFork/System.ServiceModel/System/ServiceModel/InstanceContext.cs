// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public sealed class InstanceContext : CommunicationObject, IExtensibleObject<InstanceContext>
    {
        private InstanceBehavior _behavior;
        private ConcurrencyInstanceContextFacet _concurrency;
        private ServiceChannelManager _channels;
        private ExtensionCollection<InstanceContext> _extensions;
        private object _serviceInstanceLock = new object();
        private SynchronizationContext _synchronizationContext;
        private object _userObject;
        private bool _wellKnown;
        private SynchronizedCollection<IChannel> _wmiChannels;
        private bool _isUserCreated;

        public InstanceContext(object implementation)
            : this(implementation, true)
        {
        }

        internal InstanceContext(object implementation, bool isUserCreated)
            : this(implementation, true, isUserCreated)
        {
        }

        internal InstanceContext(object implementation, bool wellKnown, bool isUserCreated)
        {
            if (implementation != null)
            {
                _userObject = implementation;
                _wellKnown = wellKnown;
            }
            _channels = new ServiceChannelManager(this);
            _isUserCreated = isUserCreated;
        }

        internal InstanceBehavior Behavior
        {
            get { return _behavior; }
            set
            {
                if (_behavior == null)
                {
                    _behavior = value;
                }
            }
        }

        internal ConcurrencyInstanceContextFacet Concurrency
        {
            get
            {
                if (_concurrency == null)
                {
                    lock (this.ThisLock)
                    {
                        if (_concurrency == null)
                            _concurrency = new ConcurrencyInstanceContextFacet();
                    }
                }

                return _concurrency;
            }
        }

        protected override TimeSpan DefaultCloseTimeout
        {
            get
            {
                return ServiceDefaults.CloseTimeout;
            }
        }

        protected override TimeSpan DefaultOpenTimeout
        {
            get
            {
                return ServiceDefaults.OpenTimeout;
            }
        }

        public IExtensionCollection<InstanceContext> Extensions
        {
            get
            {
                this.ThrowIfClosed();
                lock (this.ThisLock)
                {
                    if (_extensions == null)
                        _extensions = new ExtensionCollection<InstanceContext>(this, this.ThisLock);
                    return _extensions;
                }
            }
        }

        public ICollection<IChannel> IncomingChannels
        {
            get
            {
                this.ThrowIfClosed();
                return _channels.IncomingChannels;
            }
        }

        public ICollection<IChannel> OutgoingChannels
        {
            get
            {
                this.ThrowIfClosed();
                return _channels.OutgoingChannels;
            }
        }

        public SynchronizationContext SynchronizationContext
        {
            get { return _synchronizationContext; }
            set
            {
                this.ThrowIfClosedOrOpened();
                _synchronizationContext = value;
            }
        }

        new internal object ThisLock
        {
            get { return base.ThisLock; }
        }

        internal object UserObject
        {
            get { return _userObject; }
        }

        internal ICollection<IChannel> WmiChannels
        {
            get
            {
                if (_wmiChannels == null)
                {
                    lock (this.ThisLock)
                    {
                        if (_wmiChannels == null)
                        {
                            _wmiChannels = new SynchronizedCollection<IChannel>();
                        }
                    }
                }
                return _wmiChannels;
            }
        }

        protected override void OnAbort()
        {
            _channels.Abort();
        }

        internal void BindRpc(ref MessageRpc rpc)
        {
            this.ThrowIfClosed();
            _channels.IncrementActivityCount();
            rpc.SuccessfullyBoundInstance = true;
        }

        internal void FaultInternal()
        {
            this.Fault();
        }

        public object GetServiceInstance(Message message)
        {
            lock (_serviceInstanceLock)
            {
                this.ThrowIfClosedOrNotOpen();

                object current = _userObject;

                if (current != null)
                {
                    return current;
                }

                if (_behavior == null)
                {
                    Exception error = new InvalidOperationException(SRServiceModel.SFxInstanceNotInitialized);
                    if (message != null)
                    {
                        throw TraceUtility.ThrowHelperError(error, message);
                    }
                    else
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(error);
                    }
                }

                object newUserObject;
                if (message != null)
                {
                    newUserObject = _behavior.GetInstance(this, message);
                }
                else
                {
                    newUserObject = _behavior.GetInstance(this);
                }
                if (newUserObject != null)
                {
                    SetUserObject(newUserObject);
                }

                return newUserObject;
            }
        }


        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CloseAsyncResult(timeout, callback, state, this);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CloseAsyncResult.End(result);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new CompletedAsyncResult(callback, state);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            _channels.Close(timeout);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
        }

        protected override void OnOpened()
        {
            base.OnOpened();
        }

        protected override void OnOpening()
        {
            base.OnOpening();
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            this.OnClose(timeout);
            return TaskHelpers.CompletedTask();
        }

        protected internal override Task OnOpenAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            return TaskHelpers.CompletedTask();
        }

        private void SetUserObject(object newUserObject)
        {
            if (_behavior != null && !_wellKnown)
            {
                object oldUserObject = Interlocked.Exchange(ref _userObject, newUserObject);
            }
        }

        internal void UnbindRpc(ref MessageRpc rpc)
        {
            if (rpc.InstanceContext == this && rpc.SuccessfullyBoundInstance)
            {
                _channels.DecrementActivityCount();
            }
        }

        internal class CloseAsyncResult : AsyncResult
        {
            private InstanceContext _instanceContext;
            private TimeoutHelper _timeoutHelper;

            public CloseAsyncResult(TimeSpan timeout, AsyncCallback callback, object state, InstanceContext instanceContext)
                : base(callback, state)
            {
                _timeoutHelper = new TimeoutHelper(timeout);
                _instanceContext = instanceContext;
                IAsyncResult result = _instanceContext._channels.BeginClose(_timeoutHelper.RemainingTime(), PrepareAsyncCompletion(new AsyncCompletion(CloseChannelsCallback)), this);
                if (result.CompletedSynchronously && CloseChannelsCallback(result))
                {
                    base.Complete(true);
                }
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<CloseAsyncResult>(result);
            }

            private bool CloseChannelsCallback(IAsyncResult result)
            {
                Fx.Assert(object.ReferenceEquals(this, result.AsyncState), "AsyncState should be this");
                _instanceContext._channels.EndClose(result);
                return true;
            }
        }
    }
}
