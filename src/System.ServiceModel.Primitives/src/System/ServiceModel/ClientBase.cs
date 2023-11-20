// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ComponentModel;
using System.Diagnostics;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel
{
    public abstract class ClientBase<TChannel> : ICommunicationObject, IDisposable, IAsyncDisposable
        where TChannel : class
    {
        private TChannel _channel;
        private ChannelFactoryRef<TChannel> _channelFactoryRef;
        private EndpointTrait<TChannel> _endpointTrait;

        // Determine whether the proxy can share factory with others. It is false only if the public getters
        // are invoked.
        private bool _canShareFactory = true;

        // Determine whether the proxy is currently holding a cached factory
        private bool _useCachedFactory;

        // Determine whether we have locked down sharing for this proxy. This is turned on only when the channel
        // is created.
        private bool _sharingFinalized;

        // Determine whether the ChannelFactoryRef has been released. We should release it only once per proxy
        private bool _channelFactoryRefReleased;

        // Determine whether we have released the last ref count of the ChannelFactory so that we could abort it when it was closing.
        private bool _releasedLastRef;
        private object finalizeLock = new object();

        // Cache at most 32 ChannelFactories
        private const int MaxNumChannelFactories = 32;
        private static ChannelFactoryRefCache<TChannel> s_factoryRefCache = new ChannelFactoryRefCache<TChannel>(MaxNumChannelFactories);
        private static object s_staticLock = new object();
        private static object s_cacheLock = new object();
        private static CacheSetting s_cacheSetting = CacheSetting.Default;
        private static bool s_isCacheSettingReadOnly;

        private static AsyncCallback s_onAsyncCallCompleted = Fx.ThunkCallback(new AsyncCallback(OnAsyncCallCompleted));

        protected ClientBase()
        {
            throw new PlatformNotSupportedException(SRP.ConfigurationFilesNotSupported);
        }

        protected ClientBase(Binding binding, EndpointAddress remoteAddress)
        {
            if (binding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(binding));
            }

            if (remoteAddress == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(remoteAddress));
            }

            MakeCacheSettingReadOnly();

            if (s_cacheSetting == CacheSetting.AlwaysOn)
            {
                _endpointTrait = new ProgrammaticEndpointTrait<TChannel>(binding, remoteAddress, null);
                InitializeChannelFactoryRef();
            }
            else
            {
                _channelFactoryRef = new ChannelFactoryRef<TChannel>(new ChannelFactory<TChannel>(binding, remoteAddress));
                _channelFactoryRef.ChannelFactory.TraceOpenAndClose = false;
                TryDisableSharing();
            }
        }

        protected ClientBase(ServiceEndpoint endpoint)
        {
            if (endpoint == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(endpoint));
            }

            MakeCacheSettingReadOnly();

            if (s_cacheSetting == CacheSetting.AlwaysOn)
            {
                _endpointTrait = new ServiceEndpointTrait<TChannel>(endpoint, null);
                InitializeChannelFactoryRef();
            }
            else
            {
                _channelFactoryRef = new ChannelFactoryRef<TChannel>(new ChannelFactory<TChannel>(endpoint));
                _channelFactoryRef.ChannelFactory.TraceOpenAndClose = false;
                TryDisableSharing();
            }
        }

        protected ClientBase(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
        {
            if (callbackInstance == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(callbackInstance));
            }

            if (binding == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(binding));
            }

            if (remoteAddress == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(remoteAddress));
            }

            MakeCacheSettingReadOnly();

            if (s_cacheSetting == CacheSetting.AlwaysOn)
            {
                _endpointTrait = new ProgrammaticEndpointTrait<TChannel>(binding, remoteAddress, callbackInstance);
                InitializeChannelFactoryRef();
            }
            else
            {
                _channelFactoryRef = new ChannelFactoryRef<TChannel>(
                    new DuplexChannelFactory<TChannel>(callbackInstance, binding, remoteAddress));
                _channelFactoryRef.ChannelFactory.TraceOpenAndClose = false;
                TryDisableSharing();
            }
        }

        protected T GetDefaultValueForInitialization<T>()
        {
            return default(T);
        }

        private object ThisLock { get; } = new object();

        protected TChannel Channel
        {
            get
            {
                // created on demand, so that Mort can modify .Endpoint before calling methods on the client
                if (_channel == null)
                {
                    lock (ThisLock)
                    {
                        if (_channel == null)
                        {
                            using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : null)
                            {
                                if (DiagnosticUtility.ShouldUseActivity)
                                {
                                    ServiceModelActivity.Start(activity, SRP.Format(SRP.ActivityOpenClientBase, typeof(TChannel).FullName), ActivityType.OpenClient);
                                }

                                if (_useCachedFactory)
                                {
                                    try
                                    {
                                        CreateChannelInternal();
                                    }
                                    catch (Exception ex)
                                    {
                                        if (_useCachedFactory &&
                                            (ex is CommunicationException ||
                                            ex is ObjectDisposedException ||
                                            ex is TimeoutException))
                                        {
                                            DiagnosticUtility.TraceHandledException(ex, TraceEventType.Warning);
                                            InvalidateCacheAndCreateChannel();
                                        }
                                        else
                                        {
                                            throw;
                                        }
                                    }
                                }
                                else
                                {
                                    CreateChannelInternal();
                                }
                            }
                        }
                    }
                }

                return _channel;
            }
        }

        public static CacheSetting CacheSetting
        {
            get
            {
                return s_cacheSetting;
            }
            set
            {
                lock (s_cacheLock)
                {
                    if (s_isCacheSettingReadOnly && s_cacheSetting != value)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SFxImmutableClientBaseCacheSetting, typeof(TChannel).ToString())));
                    }
                    else
                    {
                        s_cacheSetting = value;
                    }
                }
            }
        }

        public ChannelFactory<TChannel> ChannelFactory
        {
            get
            {
                if (s_cacheSetting == CacheSetting.Default)
                {
                    TryDisableSharing();
                }

                return GetChannelFactory();
            }
        }

        public ClientCredentials ClientCredentials
        {
            get
            {
                return ChannelFactory.Credentials;
            }
        }

        public CommunicationState State
        {
            get
            {
                IChannel channel = (IChannel)_channel;
                if (channel != null)
                {
                    return channel.State;
                }
                else
                {
                    // we may have failed to create the channel under open, in which case we our factory wouldn't be open
                    if (!_useCachedFactory)
                    {
                        return GetChannelFactory().State;
                    }
                    else
                    {
                        return CommunicationState.Created;
                    }
                }
            }
        }

        public IClientChannel InnerChannel
        {
            get
            {
                return (IClientChannel)Channel;
            }
        }

        public ServiceEndpoint Endpoint
        {
            get
            {
                return ChannelFactory.Endpoint;
            }
        }

        public void Open()
        {
            ((ICommunicationObject)this).Open(GetChannelFactory().InternalOpenTimeout);
        }

        public Task OpenAsync()
        {
            return OpenAsync(GetChannelFactory().InternalOpenTimeout);
        }

        private async Task OpenAsync(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            await TaskHelpers.EnsureDefaultTaskScheduler();
            if (!_useCachedFactory)
            {
                await GetChannelFactory().OpenHelperAsync(timeoutHelper.RemainingTime());
            }

            await InnerChannel.OpenHelperAsync(timeoutHelper.RemainingTime());
        }

        public void Abort()
        {
            IChannel channel = (IChannel)_channel;
            if (channel != null)
            {
                channel.Abort();
            }

            if (!_channelFactoryRefReleased)
            {
                lock (s_staticLock)
                {
                    if (!_channelFactoryRefReleased)
                    {
                        if (_channelFactoryRef.Release())
                        {
                            _releasedLastRef = true;
                        }

                        _channelFactoryRefReleased = true;
                    }
                }
            }

            // Abort the ChannelFactory if we released the last one. We should be able to abort it when another thread is closing it.
            if (_releasedLastRef)
            {
                _channelFactoryRef.Abort();
            }
        }

        public void Close()
        {
            ((ICommunicationObject)this).Close(GetChannelFactory().InternalCloseTimeout);
        }

        public Task CloseAsync()
        {
            var timeout = GetChannelFactory().InternalCloseTimeout;
            return CloseAsync(timeout);
        }

        private async Task CloseAsync(TimeSpan timeout)
        {
            using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : null)
            {
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    ServiceModelActivity.Start(activity, SRP.Format(SRP.ActivityCloseClientBase, typeof(TChannel).FullName), ActivityType.Close);
                }

                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                await TaskHelpers.EnsureDefaultTaskScheduler();
                if (_channel != null)
                {
                    await InnerChannel.CloseHelperAsync(timeoutHelper.RemainingTime());
                }

                if (!_channelFactoryRefReleased)
                {
                    lock (s_staticLock)
                    {
                        if (!_channelFactoryRefReleased)
                        {
                            if (_channelFactoryRef.Release())
                            {
                                _releasedLastRef = true;
                            }

                            _channelFactoryRefReleased = true;
                        }
                    }

                    // Close the factory outside of the lock so that we can abort from a different thread.
                    if (_releasedLastRef)
                    {
                        if (_useCachedFactory)
                        {
                            _channelFactoryRef.Abort();
                        }
                        else
                        {
                            await GetChannelFactory().CloseHelperAsync(timeoutHelper.RemainingTime());
                        }
                    }
                }
            }
        }

        // This ensures that the cachesetting (on, off or default) cannot be modified by 
        // another ClientBase instance of matching TChannel after the first instance is created.
        private void MakeCacheSettingReadOnly()
        {
            if (s_isCacheSettingReadOnly)
            {
                return;
            }

            lock (s_cacheLock)
            {
                s_isCacheSettingReadOnly = true;
            }
        }

        void CreateChannelInternal()
        {
            try
            {
                _channel = CreateChannel();
                if (_sharingFinalized)
                {
                    if (_canShareFactory && !_useCachedFactory)
                    {
                        // It is OK to add ChannelFactory to the cache now.
                        TryAddChannelFactoryToCache();
                    }
                }
            }
            finally
            {
                if (!_sharingFinalized && s_cacheSetting == CacheSetting.Default)
                {
                    // this.CreateChannel() is not called. For safety, we disable sharing.
                    TryDisableSharing();
                }
            }
        }

        protected virtual TChannel CreateChannel()
        {
            if (_sharingFinalized)
            {
                return GetChannelFactory().CreateChannel();
            }

            lock (finalizeLock)
            {
                _sharingFinalized = true;
                return GetChannelFactory().CreateChannel();
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            try
            {
                // Only want to call Close if it is in the Opened state
                if (State == CommunicationState.Opened)
                {
                    await CloseAsync();
                }
                // Anything not closed by this point should be aborted
                if (State != CommunicationState.Closed)
                {
                    Abort();
                }
            }
            catch (CommunicationException)
            {
                Abort();
            }
            catch (TimeoutException)
            {
                Abort();
            }
        }

        void ICommunicationObject.Open(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            if (!_useCachedFactory)
            {
                GetChannelFactory().Open(timeoutHelper.RemainingTime());
            }

            InnerChannel.Open(timeoutHelper.RemainingTime());
        }

        void ICommunicationObject.Close(TimeSpan timeout)
        {
            using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : null)
            {
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    ServiceModelActivity.Start(activity, SRP.Format(SRP.ActivityCloseClientBase, typeof(TChannel).FullName), ActivityType.Close);
                }

                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
                if (_channel != null)
                {
                    InnerChannel.Close(timeoutHelper.RemainingTime());
                }

                if (!_channelFactoryRefReleased)
                {
                    lock (s_staticLock)
                    {
                        if (!_channelFactoryRefReleased)
                        {
                            if (_channelFactoryRef.Release())
                            {
                                _releasedLastRef = true;
                            }

                            _channelFactoryRefReleased = true;
                        }
                    }

                    // Close the factory outside of the lock so that we can abort from a different thread.
                    if (_releasedLastRef)
                    {
                        if (_useCachedFactory)
                        {
                            _channelFactoryRef.Abort();
                        }
                        else
                        {
                            _channelFactoryRef.Close(timeoutHelper.RemainingTime());
                        }
                    }
                }
            }
        }

        event EventHandler ICommunicationObject.Closed
        {
            add
            {
                InnerChannel.Closed += value;
            }
            remove
            {
                InnerChannel.Closed -= value;
            }
        }

        event EventHandler ICommunicationObject.Closing
        {
            add
            {
                InnerChannel.Closing += value;
            }
            remove
            {
                InnerChannel.Closing -= value;
            }
        }

        event EventHandler ICommunicationObject.Faulted
        {
            add
            {
                InnerChannel.Faulted += value;
            }
            remove
            {
                InnerChannel.Faulted -= value;
            }
        }

        event EventHandler ICommunicationObject.Opened
        {
            add
            {
                InnerChannel.Opened += value;
            }
            remove
            {
                InnerChannel.Opened -= value;
            }
        }

        event EventHandler ICommunicationObject.Opening
        {
            add
            {
                InnerChannel.Opening += value;
            }
            remove
            {
                InnerChannel.Opening -= value;
            }
        }

        IAsyncResult ICommunicationObject.BeginClose(AsyncCallback callback, object state)
        {
            return ((ICommunicationObject)this).BeginClose(GetChannelFactory().InternalCloseTimeout, callback, state);
        }

        IAsyncResult ICommunicationObject.BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return CloseAsync(timeout).ToApm(callback, state);
        }

        void ICommunicationObject.EndClose(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        IAsyncResult ICommunicationObject.BeginOpen(AsyncCallback callback, object state)
        {
            return ((ICommunicationObject)this).BeginOpen(GetChannelFactory().InternalOpenTimeout, callback, state);
        }

        IAsyncResult ICommunicationObject.BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return OpenAsync(timeout).ToApm(callback, state);
        }

        void ICommunicationObject.EndOpen(IAsyncResult result)
        {
            result.ToApmEnd();
        }

        ChannelFactory<TChannel> GetChannelFactory()
        {
            return _channelFactoryRef.ChannelFactory;
        }

        void InitializeChannelFactoryRef()
        {
            Fx.Assert(_channelFactoryRef == null, "The channelFactory should have never been assigned");
            Fx.Assert(_canShareFactory, "GetChannelFactoryFromCache can be called only when canShareFactory is true");
            lock (s_staticLock)
            {
                ChannelFactoryRef<TChannel> factoryRef;
                if (s_factoryRefCache.TryGetValue(_endpointTrait, out factoryRef))
                {
                    if (factoryRef.ChannelFactory.State != CommunicationState.Opened)
                    {
                        // Remove the bad ChannelFactory.
                        s_factoryRefCache.Remove(_endpointTrait);
                    }
                    else
                    {
                        _channelFactoryRef = factoryRef;
                        _channelFactoryRef.AddRef();
                        _useCachedFactory = true;
                        if (WcfEventSource.Instance.ClientBaseChannelFactoryCacheHitIsEnabled())
                        {
                            WcfEventSource.Instance.ClientBaseChannelFactoryCacheHit(this);
                        }
                        return;
                    }
                }
            }

            if (_channelFactoryRef == null)
            {
                // Creating the ChannelFactory at initial time to catch configuration exception earlier.
                _channelFactoryRef = CreateChannelFactoryRef(_endpointTrait);
            }
        }

        static ChannelFactoryRef<TChannel> CreateChannelFactoryRef(EndpointTrait<TChannel> endpointTrait)
        {
            Fx.Assert(endpointTrait != null, "The endpointTrait should not be null when the factory can be shared.");

            ChannelFactory<TChannel> channelFactory = endpointTrait.CreateChannelFactory();
            channelFactory.TraceOpenAndClose = false;
            return new ChannelFactoryRef<TChannel>(channelFactory);
        }

        // Once the channel is created, we can't disable caching.
        // This method can be called safely multiple times.  
        // this.sharingFinalized is set the first time the method is called.
        // Subsequent calls are essentially no-ops.
        void TryDisableSharing()
        {
            if (_sharingFinalized)
            {
                return;
            }

            lock (finalizeLock)
            {
                if (_sharingFinalized)
                {
                    return;
                }

                _canShareFactory = false;
                _sharingFinalized = true;

                if (_useCachedFactory)
                {
                    ChannelFactoryRef<TChannel> pendingFactoryRef = _channelFactoryRef;
                    _channelFactoryRef = CreateChannelFactoryRef(_endpointTrait);
                    _useCachedFactory = false;

                    lock (s_staticLock)
                    {
                        if (!pendingFactoryRef.Release())
                        {
                            pendingFactoryRef = null;
                        }
                    }

                    if (pendingFactoryRef != null)
                    {
                        pendingFactoryRef.Abort();
                    }
                }
            }

            // can be done outside the lock since the lines below do not access shared data.
            // also the use of this.sharingFinalized in the lines above ensures that tracing 
            // happens only once and only when needed.
            if (WcfEventSource.Instance.ClientBaseUsingLocalChannelFactoryIsEnabled())
            {
                WcfEventSource.Instance.ClientBaseUsingLocalChannelFactory(this);
            }
        }

        void TryAddChannelFactoryToCache()
        {
            Fx.Assert(_canShareFactory, "This should be called only when this proxy can share ChannelFactory.");
            Fx.Assert(_channelFactoryRef.ChannelFactory.State == CommunicationState.Opened,
                "The ChannelFactory must be in Opened state for caching.");

            // Lock the cache and add the item to synchronize with lookup.
            lock (s_staticLock)
            {
                ChannelFactoryRef<TChannel> cfRef;
                if (!s_factoryRefCache.TryGetValue(_endpointTrait, out cfRef))
                {
                    // Increment the ref count before adding to the cache.
                    _channelFactoryRef.AddRef();
                    s_factoryRefCache.Add(_endpointTrait, _channelFactoryRef);
                    _useCachedFactory = true;
                    if (WcfEventSource.Instance.ClientBaseCachedChannelFactoryCountIsEnabled())
                    {
                        WcfEventSource.Instance.ClientBaseCachedChannelFactoryCount(s_factoryRefCache.Count, MaxNumChannelFactories, this);
                    }
                }
            }
        }

        // NOTE: This should be called inside ThisLock
        void InvalidateCacheAndCreateChannel()
        {
            RemoveFactoryFromCache();
            TryDisableSharing();
            CreateChannelInternal();
        }

        void RemoveFactoryFromCache()
        {
            lock (s_staticLock)
            {
                ChannelFactoryRef<TChannel> factoryRef;
                if (s_factoryRefCache.TryGetValue(_endpointTrait, out factoryRef))
                {
                    if (object.ReferenceEquals(_channelFactoryRef, factoryRef))
                    {
                        s_factoryRefCache.Remove(_endpointTrait);
                    }
                }
            }
        }

        // WARNING: changes in the signature/name of the following delegates must be applied to the 
        // ClientClassGenerator.cs as well, otherwise the ClientClassGenerator would generate wrong code.
        protected delegate IAsyncResult BeginOperationDelegate(object[] inValues, AsyncCallback asyncCallback, object state);
        protected delegate object[] EndOperationDelegate(IAsyncResult result);

        // WARNING: Any changes in the signature/name of the following type and its ctor must be applied to the 
        // ClientClassGenerator.cs as well, otherwise the ClientClassGenerator would generate wrong code.
        protected class InvokeAsyncCompletedEventArgs : AsyncCompletedEventArgs
        {
            internal InvokeAsyncCompletedEventArgs(object[] results, Exception error, bool cancelled, object userState)
                : base(error, cancelled, userState)
            {
                Results = results;
            }

            public object[] Results { get; }
        }

        // WARNING: Any changes in the signature/name of the following method ctor must be applied to the 
        // ClientClassGenerator.cs as well, otherwise the ClientClassGenerator would generate wrong code.
        protected void InvokeAsync(BeginOperationDelegate beginOperationDelegate, object[] inValues,
            EndOperationDelegate endOperationDelegate, SendOrPostCallback operationCompletedCallback, object userState)
        {
            if (beginOperationDelegate == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(beginOperationDelegate));
            }
            if (endOperationDelegate == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(endOperationDelegate));
            }

            AsyncOperation asyncOperation = AsyncOperationManager.CreateOperation(userState);
            AsyncOperationContext context = new AsyncOperationContext(asyncOperation, endOperationDelegate, operationCompletedCallback);

            Exception error = null;
            object[] results = null;
            IAsyncResult result = null;
            try
            {
                result = beginOperationDelegate(inValues, s_onAsyncCallCompleted, context);
                if (result.CompletedSynchronously)
                {
                    results = endOperationDelegate(result);
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                error = e;
            }

            if (error != null || result.CompletedSynchronously) /* result cannot be null if error == null */
            {
                CompleteAsyncCall(context, results, error);
            }
        }

        private static void OnAsyncCallCompleted(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            AsyncOperationContext context = (AsyncOperationContext)result.AsyncState;
            Exception error = null;
            object[] results = null;
            try
            {
                results = context.EndDelegate(result);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                error = e;
            }

            CompleteAsyncCall(context, results, error);
        }

        private static void CompleteAsyncCall(AsyncOperationContext context, object[] results, Exception error)
        {
            if (context.CompletionCallback != null)
            {
                InvokeAsyncCompletedEventArgs e = new InvokeAsyncCompletedEventArgs(results, error, false, context.AsyncOperation.UserSuppliedState);
                context.AsyncOperation.PostOperationCompleted(context.CompletionCallback, e);
            }
            else
            {
                context.AsyncOperation.OperationCompleted();
            }
        }

        protected class AsyncOperationContext
        {
            private SendOrPostCallback _completionCallback;

            internal AsyncOperationContext(AsyncOperation asyncOperation, EndOperationDelegate endDelegate, SendOrPostCallback completionCallback)
            {
                AsyncOperation = asyncOperation;
                EndDelegate = endDelegate;
                _completionCallback = completionCallback;
            }

            internal AsyncOperation AsyncOperation { get; }

            internal EndOperationDelegate EndDelegate { get; }

            internal SendOrPostCallback CompletionCallback
            {
                get
                {
                    return _completionCallback;
                }
            }
        }

        protected class ChannelBase<T> : IClientChannel, IOutputChannel, IRequestChannel, IChannelBaseProxy
            where T : class
        {
            private ServiceChannel _channel;
            private ImmutableClientRuntime _runtime;

            protected ChannelBase(ClientBase<T> client)
            {
                if (client.Endpoint.Address == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SFxChannelFactoryEndpointAddressUri));
                }

                ChannelFactory<T> cf = client.ChannelFactory;
                cf.EnsureOpened();  // to prevent the NullReferenceException that is thrown if the ChannelFactory is not open when cf.ServiceChannelFactory is accessed.
                _channel = cf.ServiceChannelFactory.CreateServiceChannel(client.Endpoint.Address, client.Endpoint.Address.Uri);
                _channel.InstanceContext = cf.CallbackInstance;
                _runtime = _channel.ClientRuntime.GetRuntime();
            }

            protected IAsyncResult BeginInvoke(string methodName, object[] args, AsyncCallback callback, object state)
            {
                object[] inArgs = new object[args.Length + 2];
                Array.Copy(args, inArgs, args.Length);
                inArgs[inArgs.Length - 2] = callback;
                inArgs[inArgs.Length - 1] = state;

                MethodCall methodCall = new MethodCall(inArgs);
                ProxyOperationRuntime op = GetOperationByName(methodName);
                object[] ins = op.MapAsyncBeginInputs(methodCall, out callback, out state);
                return _channel.BeginCall(op.Action, op.IsOneWay, op, ins, callback, state);
            }

            protected object EndInvoke(string methodName, object[] args, IAsyncResult result)
            {
                object[] inArgs = new object[args.Length + 1];
                Array.Copy(args, inArgs, args.Length);
                inArgs[inArgs.Length - 1] = result;

                MethodCall methodCall = new MethodCall(inArgs);
                ProxyOperationRuntime op = GetOperationByName(methodName);
                object[] outs;
                op.MapAsyncEndInputs(methodCall, out result, out outs);
                object ret = _channel.EndCall(op.Action, outs, result);
                object[] retArgs = op.MapAsyncOutputs(methodCall, outs, ref ret);
                if (retArgs != null)
                {
                    Fx.Assert(retArgs.Length == inArgs.Length, "retArgs.Length should be equal to inArgs.Length");
                    Array.Copy(retArgs, args, args.Length);
                }
                return ret;
            }

            private ProxyOperationRuntime GetOperationByName(string methodName)
            {
                ProxyOperationRuntime op = _runtime.GetOperationByName(methodName);
                if (op == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(SRP.Format(SRP.SFxMethodNotSupported1, methodName)));
                }
                return op;
            }

            bool IClientChannel.AllowInitializationUI
            {
                get { return ((IClientChannel)_channel).AllowInitializationUI; }
                set { ((IClientChannel)_channel).AllowInitializationUI = value; }
            }

            bool IClientChannel.DidInteractiveInitialization
            {
                get { return ((IClientChannel)_channel).DidInteractiveInitialization; }
            }

            Uri IClientChannel.Via
            {
                get { return ((IClientChannel)_channel).Via; }
            }

            event EventHandler<UnknownMessageReceivedEventArgs> IClientChannel.UnknownMessageReceived
            {
                add { ((IClientChannel)_channel).UnknownMessageReceived += value; }
                remove { ((IClientChannel)_channel).UnknownMessageReceived -= value; }
            }

            void IClientChannel.DisplayInitializationUI()
            {
                ((IClientChannel)_channel).DisplayInitializationUI();
            }

            IAsyncResult IClientChannel.BeginDisplayInitializationUI(AsyncCallback callback, object state)
            {
                return ((IClientChannel)_channel).BeginDisplayInitializationUI(callback, state);
            }

            void IClientChannel.EndDisplayInitializationUI(IAsyncResult result)
            {
                ((IClientChannel)_channel).EndDisplayInitializationUI(result);
            }

            bool IContextChannel.AllowOutputBatching
            {
                get { return ((IContextChannel)_channel).AllowOutputBatching; }
                set { ((IContextChannel)_channel).AllowOutputBatching = value; }
            }

            IInputSession IContextChannel.InputSession
            {
                get { return ((IContextChannel)_channel).InputSession; }
            }

            EndpointAddress IContextChannel.LocalAddress
            {
                get { return ((IContextChannel)_channel).LocalAddress; }
            }

            TimeSpan IContextChannel.OperationTimeout
            {
                get { return ((IContextChannel)_channel).OperationTimeout; }
                set { ((IContextChannel)_channel).OperationTimeout = value; }
            }

            IOutputSession IContextChannel.OutputSession
            {
                get { return ((IContextChannel)_channel).OutputSession; }
            }

            EndpointAddress IContextChannel.RemoteAddress
            {
                get { return ((IContextChannel)_channel).RemoteAddress; }
            }

            string IContextChannel.SessionId
            {
                get { return ((IContextChannel)_channel).SessionId; }
            }

            TProperty IChannel.GetProperty<TProperty>()
            {
                return ((IChannel)_channel).GetProperty<TProperty>();
            }

            CommunicationState ICommunicationObject.State
            {
                get { return ((ICommunicationObject)_channel).State; }
            }

            event EventHandler ICommunicationObject.Closed
            {
                add { ((ICommunicationObject)_channel).Closed += value; }
                remove { ((ICommunicationObject)_channel).Closed -= value; }
            }

            event EventHandler ICommunicationObject.Closing
            {
                add { ((ICommunicationObject)_channel).Closing += value; }
                remove { ((ICommunicationObject)_channel).Closing -= value; }
            }

            event EventHandler ICommunicationObject.Faulted
            {
                add { ((ICommunicationObject)_channel).Faulted += value; }
                remove { ((ICommunicationObject)_channel).Faulted -= value; }
            }

            event EventHandler ICommunicationObject.Opened
            {
                add { ((ICommunicationObject)_channel).Opened += value; }
                remove { ((ICommunicationObject)_channel).Opened -= value; }
            }

            event EventHandler ICommunicationObject.Opening
            {
                add { ((ICommunicationObject)_channel).Opening += value; }
                remove { ((ICommunicationObject)_channel).Opening -= value; }
            }

            void ICommunicationObject.Abort()
            {
                ((ICommunicationObject)_channel).Abort();
            }

            void ICommunicationObject.Close()
            {
                ((ICommunicationObject)_channel).Close();
            }

            void ICommunicationObject.Close(TimeSpan timeout)
            {
                ((ICommunicationObject)_channel).Close(timeout);
            }

            IAsyncResult ICommunicationObject.BeginClose(AsyncCallback callback, object state)
            {
                return ((ICommunicationObject)_channel).BeginClose(callback, state);
            }

            IAsyncResult ICommunicationObject.BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return ((ICommunicationObject)_channel).BeginClose(timeout, callback, state);
            }

            void ICommunicationObject.EndClose(IAsyncResult result)
            {
                ((ICommunicationObject)_channel).EndClose(result);
            }

            void ICommunicationObject.Open()
            {
                ((ICommunicationObject)_channel).Open();
            }

            void ICommunicationObject.Open(TimeSpan timeout)
            {
                ((ICommunicationObject)_channel).Open(timeout);
            }

            IAsyncResult ICommunicationObject.BeginOpen(AsyncCallback callback, object state)
            {
                return ((ICommunicationObject)_channel).BeginOpen(callback, state);
            }

            IAsyncResult ICommunicationObject.BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
            {
                return ((ICommunicationObject)_channel).BeginOpen(timeout, callback, state);
            }

            void ICommunicationObject.EndOpen(IAsyncResult result)
            {
                ((ICommunicationObject)_channel).EndOpen(result);
            }

            IExtensionCollection<IContextChannel> IExtensibleObject<IContextChannel>.Extensions
            {
                get { return ((IExtensibleObject<IContextChannel>)_channel).Extensions; }
            }

            void IDisposable.Dispose()
            {
                ((IDisposable)_channel).Dispose();
            }

            Uri IOutputChannel.Via
            {
                get { return ((IOutputChannel)_channel).Via; }
            }

            EndpointAddress IOutputChannel.RemoteAddress
            {
                get { return ((IOutputChannel)_channel).RemoteAddress; }
            }

            void IOutputChannel.Send(Message message)
            {
                ((IOutputChannel)_channel).Send(message);
            }

            void IOutputChannel.Send(Message message, TimeSpan timeout)
            {
                ((IOutputChannel)_channel).Send(message, timeout);
            }

            IAsyncResult IOutputChannel.BeginSend(Message message, AsyncCallback callback, object state)
            {
                return ((IOutputChannel)_channel).BeginSend(message, callback, state);
            }

            IAsyncResult IOutputChannel.BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            {
                return ((IOutputChannel)_channel).BeginSend(message, timeout, callback, state);
            }

            void IOutputChannel.EndSend(IAsyncResult result)
            {
                ((IOutputChannel)_channel).EndSend(result);
            }

            Uri IRequestChannel.Via
            {
                get { return ((IRequestChannel)_channel).Via; }
            }

            EndpointAddress IRequestChannel.RemoteAddress
            {
                get { return ((IRequestChannel)_channel).RemoteAddress; }
            }

            Message IRequestChannel.Request(Message message)
            {
                return ((IRequestChannel)_channel).Request(message);
            }

            Message IRequestChannel.Request(Message message, TimeSpan timeout)
            {
                return ((IRequestChannel)_channel).Request(message, timeout);
            }

            IAsyncResult IRequestChannel.BeginRequest(Message message, AsyncCallback callback, object state)
            {
                return ((IRequestChannel)_channel).BeginRequest(message, callback, state);
            }

            IAsyncResult IRequestChannel.BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
            {
                return ((IRequestChannel)_channel).BeginRequest(message, timeout, callback, state);
            }

            Message IRequestChannel.EndRequest(IAsyncResult result)
            {
                return ((IRequestChannel)_channel).EndRequest(result);
            }

            ServiceChannel IChannelBaseProxy.GetServiceChannel()
            {
                return _channel;
            }
        }
    }
}
