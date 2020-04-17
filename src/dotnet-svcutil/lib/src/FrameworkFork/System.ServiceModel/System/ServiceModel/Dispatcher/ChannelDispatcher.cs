// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel.Dispatcher
{
    public class ChannelDispatcher : ChannelDispatcherBase
    {
        private SynchronizedCollection<IChannelInitializer> _channelInitializers;
        private CommunicationObjectManager<IChannel> _channels;
        private EndpointDispatcherCollection _endpointDispatchers;
        private Collection<IErrorHandler> _errorHandlers;
        private EndpointDispatcherTable _filterTable;
        private bool _receiveContextEnabled;
        private readonly IChannelListener _listener = null;
        private ListenerHandler _listenerHandler;
        private int _maxTransactedBatchSize;
        private MessageVersion _messageVersion;
        private SynchronizedChannelCollection<IChannel> _pendingChannels; // app has not yet seen these.
        private bool _receiveSynchronously;
        private bool _sendAsynchronously;
        private int _maxPendingReceives;
        private bool _includeExceptionDetailInFaults;
        private bool _session = false;
        private SharedRuntimeState _shared;
        private IDefaultCommunicationTimeouts _timeouts = null;
        private TimeSpan _transactionTimeout;
        private bool _performDefaultCloseInput;
        private EventTraceActivity _eventTraceActivity;
        private ErrorBehavior _errorBehavior;

        internal ChannelDispatcher(SharedRuntimeState shared)
        {
            this.Initialize(shared);
        }

        private void Initialize(SharedRuntimeState shared)
        {
            _shared = shared;
            _endpointDispatchers = new EndpointDispatcherCollection(this);
            _channelInitializers = this.NewBehaviorCollection<IChannelInitializer>();
            _channels = new CommunicationObjectManager<IChannel>(this.ThisLock);
            _pendingChannels = new SynchronizedChannelCollection<IChannel>(this.ThisLock);
            _errorHandlers = new Collection<IErrorHandler>();
            _receiveSynchronously = false;
            _transactionTimeout = TimeSpan.Zero;
            _maxPendingReceives = 1; //Default maxpending receives is 1;
            if (_listener != null)
            {
                _listener.Faulted += new EventHandler(OnListenerFaulted);
            }
        }

        protected override TimeSpan DefaultCloseTimeout
        {
            get
            {
                if (_timeouts != null)
                {
                    return _timeouts.CloseTimeout;
                }
                else
                {
                    return ServiceDefaults.CloseTimeout;
                }
            }
        }

        protected override TimeSpan DefaultOpenTimeout
        {
            get
            {
                if (_timeouts != null)
                {
                    return _timeouts.OpenTimeout;
                }
                else
                {
                    return ServiceDefaults.OpenTimeout;
                }
            }
        }

        internal EndpointDispatcherTable EndpointDispatcherTable
        {
            get { return _filterTable; }
        }

        internal CommunicationObjectManager<IChannel> Channels
        {
            get { return _channels; }
        }

        public SynchronizedCollection<EndpointDispatcher> Endpoints
        {
            get { return _endpointDispatchers; }
        }

        public Collection<IErrorHandler> ErrorHandlers
        {
            get { return _errorHandlers; }
        }

        public MessageVersion MessageVersion
        {
            get { return _messageVersion; }
            set
            {
                _messageVersion = value;
                this.ThrowIfDisposedOrImmutable();
            }
        }

        internal bool EnableFaults
        {
            get { return _shared.EnableFaults; }
            set
            {
                this.ThrowIfDisposedOrImmutable();
                _shared.EnableFaults = value;
            }
        }

        internal bool IsOnServer
        {
            get { return _shared.IsOnServer; }
        }

        public bool ReceiveContextEnabled
        {
            get
            {
                return _receiveContextEnabled;
            }
            set
            {
                this.ThrowIfDisposedOrImmutable();
                _receiveContextEnabled = value;
            }
        }

        internal bool BufferedReceiveEnabled
        {
            get;
            set;
        }

        public override IChannelListener Listener
        {
            get { return _listener; }
        }

        public int MaxTransactedBatchSize
        {
            get
            {
                return _maxTransactedBatchSize;
            }
            set
            {
                if (value < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value,
                                                    SRServiceModel.ValueMustBeNonNegative));
                }

                this.ThrowIfDisposedOrImmutable();
                _maxTransactedBatchSize = value;
            }
        }

        public bool ManualAddressing
        {
            get { return _shared.ManualAddressing; }
            set
            {
                this.ThrowIfDisposedOrImmutable();
                _shared.ManualAddressing = value;
            }
        }

        internal SynchronizedChannelCollection<IChannel> PendingChannels
        {
            get { return _pendingChannels; }
        }

        public bool ReceiveSynchronously
        {
            get
            {
                return _receiveSynchronously;
            }
            set
            {
                this.ThrowIfDisposedOrImmutable();
                _receiveSynchronously = value;
            }
        }

        public bool SendAsynchronously
        {
            get
            {
                return _sendAsynchronously;
            }
            set
            {
                this.ThrowIfDisposedOrImmutable();
                _sendAsynchronously = value;
            }
        }

        public int MaxPendingReceives
        {
            get
            {
                return _maxPendingReceives;
            }
            set
            {
                this.ThrowIfDisposedOrImmutable();
                _maxPendingReceives = value;
            }
        }

        public bool IncludeExceptionDetailInFaults
        {
            get { return _includeExceptionDetailInFaults; }
            set
            {
                lock (this.ThisLock)
                {
                    this.ThrowIfDisposedOrImmutable();
                    _includeExceptionDetailInFaults = value;
                }
            }
        }

        internal IDefaultCommunicationTimeouts DefaultCommunicationTimeouts
        {
            get { return _timeouts; }
        }

        private void AbortPendingChannels()
        {
            lock (this.ThisLock)
            {
                for (int i = _pendingChannels.Count - 1; i >= 0; i--)
                {
                    _pendingChannels[i].Abort();
                }
            }
        }

        internal override void CloseInput(TimeSpan timeout)
        {
            // we have to perform some slightly convoluted logic here due to 
            // backwards compat. We probably need an IAsyncChannelDispatcher 
            // interface that has timeouts and async
            this.CloseInput();

            if (_performDefaultCloseInput)
            {
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                lock (this.ThisLock)
                {
                    ListenerHandler handler = _listenerHandler;
                    if (handler != null)
                    {
                        handler.CloseInput(timeoutHelper.RemainingTime());
                    }
                }

                if (!_session)
                {
                    ListenerHandler handler = _listenerHandler;
                    if (handler != null)
                    {
                        handler.Close(timeoutHelper.RemainingTime());
                    }
                }
            }
        }

        public override void CloseInput()
        {
            _performDefaultCloseInput = true;
        }

        private void OnListenerFaulted(object sender, EventArgs e)
        {
            this.Fault();
        }

        internal bool HandleError(Exception error)
        {
            ErrorHandlerFaultInfo dummy = new ErrorHandlerFaultInfo();
            return this.HandleError(error, ref dummy);
        }

        internal bool HandleError(Exception error, ref ErrorHandlerFaultInfo faultInfo)
        {
            ErrorBehavior behavior;

            lock (this.ThisLock)
            {
                if (_errorBehavior != null)
                {
                    behavior = _errorBehavior;
                }
                else
                {
                    behavior = new ErrorBehavior(this);
                }
            }

            if (behavior != null)
            {
                return behavior.HandleError(error, ref faultInfo);
            }
            else
            {
                return false;
            }
        }

        internal void InitializeChannel(IClientChannel channel)
        {
            this.ThrowIfDisposedOrNotOpen();
            try
            {
                for (int i = 0; i < _channelInitializers.Count; ++i)
                {
                    _channelInitializers[i].Initialize(channel);
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(e);
            }
        }

        internal SynchronizedCollection<T> NewBehaviorCollection<T>()
        {
            return new ChannelDispatcherBehaviorCollection<T>(this);
        }

        private void OnAddEndpoint(EndpointDispatcher endpoint)
        {
            lock (this.ThisLock)
            {
                endpoint.Attach(this);

                if (this.State == CommunicationState.Opened)
                {
                    _filterTable.AddEndpoint(endpoint);
                }
            }
        }

        private void OnRemoveEndpoint(EndpointDispatcher endpoint)
        {
            lock (this.ThisLock)
            {
                if (this.State == CommunicationState.Opened)
                {
                    _filterTable.RemoveEndpoint(endpoint);
                }

                endpoint.Detach(this);
            }
        }

        protected override void OnAbort()
        {
            if (_listener != null)
            {
                _listener.Abort();
            }

            ListenerHandler handler = _listenerHandler;
            if (handler != null)
            {
                handler.Abort();
            }

            this.AbortPendingChannels();
        }

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

            if (_listener != null)
            {
                _listener.Close(timeoutHelper.RemainingTime());
            }

            ListenerHandler handler = _listenerHandler;
            if (handler != null)
            {
                handler.Close(timeoutHelper.RemainingTime());
            }

            this.AbortPendingChannels();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            List<ICommunicationObject> list = new List<ICommunicationObject>();

            if (_listener != null)
            {
                list.Add(_listener);
            }

            ListenerHandler handler = _listenerHandler;
            if (handler != null)
            {
                list.Add(handler);
            }

            return new CloseCollectionAsyncResult(timeout, callback, state, list);
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            try
            {
                CloseCollectionAsyncResult.End(result);
            }
            finally
            {
                this.AbortPendingChannels();
            }
        }

        protected override void OnClosed()
        {
            base.OnClosed();
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            ThrowIfNoMessageVersion();

            if (_listener != null)
            {
                try
                {
                    _listener.Open(timeout);
                }
                catch (InvalidOperationException e)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateOuterExceptionWithEndpointsInformation(e));
                }
            }
        }

        protected internal override Task OnCloseAsync(TimeSpan timeout)
        {
            this.OnClose(timeout);
            return TaskHelpers.CompletedTask();
        }

        protected internal override Task OnOpenAsync(TimeSpan timeout)
        {
            this.OnOpen(timeout);
            return TaskHelpers.CompletedTask();
        }

        private InvalidOperationException CreateOuterExceptionWithEndpointsInformation(InvalidOperationException e)
        {
            string endpointContractNames = CreateContractListString();

            if (String.IsNullOrEmpty(endpointContractNames))
            {
                return new InvalidOperationException(string.Format(SRServiceModel.SFxChannelDispatcherUnableToOpen1, _listener.Uri), e);
            }
            else
            {
                return new InvalidOperationException(string.Format(SRServiceModel.SFxChannelDispatcherUnableToOpen2, _listener.Uri, endpointContractNames), e);
            }
        }

        internal string CreateContractListString()
        {
            const string OpenQuote = "\"";
            const string CloseQuote = "\"";
            const string Space = " ";

            Collection<string> namesSeen = new Collection<string>();
            StringBuilder endpointContractNames = new StringBuilder();

            lock (this.ThisLock)
            {
                foreach (EndpointDispatcher ed in this.Endpoints)
                {
                    if (!namesSeen.Contains(ed.ContractName))
                    {
                        if (endpointContractNames.Length > 0)
                        {
                            endpointContractNames.Append(CultureInfo.CurrentCulture.TextInfo.ListSeparator);
                            endpointContractNames.Append(Space);
                        }

                        endpointContractNames.Append(OpenQuote);
                        endpointContractNames.Append(ed.ContractName);
                        endpointContractNames.Append(CloseQuote);

                        namesSeen.Add(ed.ContractName);
                    }
                }
            }

            return endpointContractNames.ToString();
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            ThrowIfNoMessageVersion();

            if (_listener != null)
            {
                try
                {
                    return _listener.BeginOpen(timeout, callback, state);
                }
                catch (InvalidOperationException e)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateOuterExceptionWithEndpointsInformation(e));
                }
            }
            else
            {
                return new CompletedAsyncResult(callback, state);
            }
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            if (_listener != null)
            {
                try
                {
                    _listener.EndOpen(result);
                }
                catch (InvalidOperationException e)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(CreateOuterExceptionWithEndpointsInformation(e));
                }
            }
            else
            {
                CompletedAsyncResult.End(result);
            }
        }

        protected override void OnOpening()
        {
            if (WcfEventSource.Instance.ListenerOpenStartIsEnabled())
            {
                _eventTraceActivity = EventTraceActivity.GetFromThreadOrCreate();
                WcfEventSource.Instance.ListenerOpenStart(_eventTraceActivity,
                    (this.Listener != null) ? this.Listener.Uri.ToString() : string.Empty, Guid.Empty);
                // Desktop: (this.host != null && host.EventTraceActivity != null) ? this.host.EventTraceActivity.ActivityId : Guid.Empty);
            }

            base.OnOpening();
        }

        protected override void OnOpened()
        {
            base.OnOpened();

            if (WcfEventSource.Instance.ListenerOpenStopIsEnabled())
            {
                WcfEventSource.Instance.ListenerOpenStop(_eventTraceActivity);
                _eventTraceActivity = null; // clear this since we don't need this anymore.
            }

            _errorBehavior = new ErrorBehavior(this);

            _filterTable = new EndpointDispatcherTable(this.ThisLock);
            for (int i = 0; i < _endpointDispatchers.Count; i++)
            {
                EndpointDispatcher endpoint = _endpointDispatchers[i];

                // Force a build of the runtime to catch any unexpected errors before we are done opening.
                // Lock down the DispatchRuntime.
                endpoint.DispatchRuntime.LockDownProperties();

                _filterTable.AddEndpoint(endpoint);
            }

            IListenerBinder binder = ListenerBinder.GetBinder(_listener, _messageVersion);
            _listenerHandler = new ListenerHandler(binder, this, _timeouts);
            _listenerHandler.Open();  // This never throws, which is why it's ok for it to happen in OnOpened
        }

        internal void ProvideFault(Exception e, FaultConverter faultConverter, ref ErrorHandlerFaultInfo faultInfo)
        {
            ErrorBehavior behavior;

            lock (this.ThisLock)
            {
                if (_errorBehavior != null)
                {
                    behavior = _errorBehavior;
                }
                else
                {
                    behavior = new ErrorBehavior(this);
                }
            }

            behavior.ProvideFault(e, faultConverter, ref faultInfo);
        }

        internal new void ThrowIfDisposedOrImmutable()
        {
            base.ThrowIfDisposedOrImmutable();
            _shared.ThrowIfImmutable();
        }

        private void ThrowIfNoMessageVersion()
        {
            if (_messageVersion == null)
            {
                Exception error = new InvalidOperationException(SRServiceModel.SFxChannelDispatcherNoMessageVersion);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(error);
            }
        }

        internal class EndpointDispatcherCollection : SynchronizedCollection<EndpointDispatcher>
        {
            private ChannelDispatcher _owner;

            internal EndpointDispatcherCollection(ChannelDispatcher owner)
                : base(owner.ThisLock)
            {
                _owner = owner;
            }

            protected override void ClearItems()
            {
                foreach (EndpointDispatcher item in this.Items)
                {
                    _owner.OnRemoveEndpoint(item);
                }
                base.ClearItems();
            }

            protected override void InsertItem(int index, EndpointDispatcher item)
            {
                if (item == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");

                _owner.OnAddEndpoint(item);
                base.InsertItem(index, item);
            }

            protected override void RemoveItem(int index)
            {
                EndpointDispatcher item = this.Items[index];
                base.RemoveItem(index);
                _owner.OnRemoveEndpoint(item);
            }

            protected override void SetItem(int index, EndpointDispatcher item)
            {
                Exception error = new InvalidOperationException(SRServiceModel.SFxCollectionDoesNotSupportSet0);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(error);
            }
        }

        internal class ChannelDispatcherBehaviorCollection<T> : SynchronizedCollection<T>
        {
            private ChannelDispatcher _outer;

            internal ChannelDispatcherBehaviorCollection(ChannelDispatcher outer)
                : base(outer.ThisLock)
            {
                _outer = outer;
            }

            protected override void ClearItems()
            {
                _outer.ThrowIfDisposedOrImmutable();
                base.ClearItems();
            }

            protected override void InsertItem(int index, T item)
            {
                if (item == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
                }

                _outer.ThrowIfDisposedOrImmutable();
                base.InsertItem(index, item);
            }

            protected override void RemoveItem(int index)
            {
                _outer.ThrowIfDisposedOrImmutable();
                base.RemoveItem(index);
            }

            protected override void SetItem(int index, T item)
            {
                if (item == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("item");
                }

                _outer.ThrowIfDisposedOrImmutable();
                base.SetItem(index, item);
            }
        }
    }
}
