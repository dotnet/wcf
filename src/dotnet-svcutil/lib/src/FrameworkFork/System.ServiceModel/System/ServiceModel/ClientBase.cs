// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.ServiceModel.Dispatcher;
using System.Threading;

namespace System.ServiceModel
{
    public abstract class ClientBase<TChannel> : ICommunicationObject, IDisposable
        where TChannel : class
    {
        private TChannel _channel;
        private ChannelFactory<TChannel> _channelFactory;

        private object _syncRoot = new object();

        private static AsyncCallback s_onAsyncCallCompleted = Fx.ThunkCallback(new AsyncCallback(OnAsyncCallCompleted));

        protected ClientBase()
        {
            throw new PlatformNotSupportedException(SRServiceModel.ConfigurationFilesNotSupported);
        }

        protected ClientBase(string endpointConfigurationName)
        {
            throw new PlatformNotSupportedException(SRServiceModel.ConfigurationFilesNotSupported);
        }

        protected ClientBase(string endpointConfigurationName, string remoteAddress)
        {
            throw new PlatformNotSupportedException(SRServiceModel.ConfigurationFilesNotSupported);
        }

        protected ClientBase(string endpointConfigurationName, EndpointAddress remoteAddress)
        {
            throw new PlatformNotSupportedException(SRServiceModel.ConfigurationFilesNotSupported);
        }

        protected ClientBase(Binding binding, EndpointAddress remoteAddress)
        {
            if (binding == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("binding");
            if (remoteAddress == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("remoteAddress");

            _channelFactory = new ChannelFactory<TChannel>(binding, remoteAddress);
            _channelFactory.TraceOpenAndClose = false;
        }

        protected ClientBase(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress)
        {
            if (callbackInstance == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("callbackInstance");
            if (binding == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("binding");
            if (remoteAddress == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("remoteAddress");

            _channelFactory = new DuplexChannelFactory<TChannel>(callbackInstance, binding, remoteAddress);
            _channelFactory.TraceOpenAndClose = false;
        }

        protected T GetDefaultValueForInitialization<T>()
        {
            return default(T);
        }

        private object ThisLock
        {
            get
            {
                return _syncRoot;
            }
        }

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
                                    ServiceModelActivity.Start(activity, string.Format(SRServiceModel.ActivityOpenClientBase, typeof(TChannel).FullName), ActivityType.OpenClient);
                                }

                                CreateChannelInternal();
                            }
                        }
                    }
                }
                return _channel;
            }
        }

        public ChannelFactory<TChannel> ChannelFactory
        {
            get
            {
                return _channelFactory;
            }
        }

        public ClientCredentials ClientCredentials
        {
            get
            {
                return _channelFactory.Credentials;
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
                    return _channelFactory.State;
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
                return _channelFactory.Endpoint;
            }
        }

        public void Open()
        {
            ((ICommunicationObject)this).Open(_channelFactory.InternalOpenTimeout);
        }

        public void Abort()
        {
            IChannel channel = (IChannel)_channel;
            if (channel != null)
            {
                channel.Abort();
            }
            _channelFactory.Abort();
        }

        public void Close()
        {
            ((ICommunicationObject)this).Close(_channelFactory.InternalCloseTimeout);
        }

        private void CreateChannelInternal()
        {
            _channel = CreateChannel();
        }

        protected virtual TChannel CreateChannel()
        {
            return _channelFactory.CreateChannel();
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        void ICommunicationObject.Open(TimeSpan timeout)
        {
            TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);
            _channelFactory.Open(timeoutHelper.RemainingTime());
            InnerChannel.Open(timeoutHelper.RemainingTime());
        }

        void ICommunicationObject.Close(TimeSpan timeout)
        {
            using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivity() : null)
            {
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    ServiceModelActivity.Start(activity, string.Format(SRServiceModel.ActivityCloseClientBase, typeof(TChannel).FullName), ActivityType.Close);
                }
                TimeoutHelper timeoutHelper = new TimeoutHelper(timeout);

                if (_channel != null)
                {
                    InnerChannel.Close(timeoutHelper.RemainingTime());
                }

                _channelFactory.Close(timeoutHelper.RemainingTime());
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
            return ((ICommunicationObject)this).BeginClose(_channelFactory.InternalCloseTimeout, callback, state);
        }

        IAsyncResult ICommunicationObject.BeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ChainedAsyncResult(timeout, callback, state, BeginChannelClose, EndChannelClose, BeginFactoryClose, EndFactoryClose);
        }

        void ICommunicationObject.EndClose(IAsyncResult result)
        {
            ChainedAsyncResult.End(result);
        }

        IAsyncResult ICommunicationObject.BeginOpen(AsyncCallback callback, object state)
        {
            return ((ICommunicationObject)this).BeginOpen(_channelFactory.InternalOpenTimeout, callback, state);
        }

        IAsyncResult ICommunicationObject.BeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return new ChainedAsyncResult(timeout, callback, state, BeginFactoryOpen, EndFactoryOpen, BeginChannelOpen, EndChannelOpen);
        }

        void ICommunicationObject.EndOpen(IAsyncResult result)
        {
            ChainedAsyncResult.End(result);
        }

        //ChainedAsyncResult methods for opening and closing ChannelFactory<T>

        internal IAsyncResult BeginFactoryOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _channelFactory.BeginOpen(timeout, callback, state);
        }

        internal void EndFactoryOpen(IAsyncResult result)
        {
            _channelFactory.EndOpen(result);
        }

        internal IAsyncResult BeginChannelOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return InnerChannel.BeginOpen(timeout, callback, state);
        }

        internal void EndChannelOpen(IAsyncResult result)
        {
            InnerChannel.EndOpen(result);
        }

        internal IAsyncResult BeginFactoryClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _channelFactory.BeginClose(timeout, callback, state);
        }

        internal void EndFactoryClose(IAsyncResult result)
        {
            if (typeof(CompletedAsyncResult).IsAssignableFrom(result.GetType()))
            {
                CompletedAsyncResult.End(result);
            }
            else
            {
                _channelFactory.EndClose(result);
            }
        }

        internal IAsyncResult BeginChannelClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            if (_channel != null)
            {
                return InnerChannel.BeginClose(timeout, callback, state);
            }
            else
            {
                return new CompletedAsyncResult(callback, state);
            }
        }

        internal void EndChannelClose(IAsyncResult result)
        {
            if (typeof(CompletedAsyncResult).IsAssignableFrom(result.GetType()))
            {
                CompletedAsyncResult.End(result);
            }
            else
            {
                InnerChannel.EndClose(result);
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
            private object[] _results;

            internal InvokeAsyncCompletedEventArgs(object[] results, Exception error, bool cancelled, object userState)
                : base(error, cancelled, userState)
            {
                _results = results;
            }

            public object[] Results
            {
                get
                {
                    return _results;
                }
            }
        }

        // WARNING: Any changes in the signature/name of the following method ctor must be applied to the 
        // ClientClassGenerator.cs as well, otherwise the ClientClassGenerator would generate wrong code.
        protected void InvokeAsync(BeginOperationDelegate beginOperationDelegate, object[] inValues,
            EndOperationDelegate endOperationDelegate, SendOrPostCallback operationCompletedCallback, object userState)
        {
            if (beginOperationDelegate == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("beginOperationDelegate");
            }
            if (endOperationDelegate == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endOperationDelegate");
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
            private AsyncOperation _asyncOperation;
            private EndOperationDelegate _endDelegate;
            private SendOrPostCallback _completionCallback;

            internal AsyncOperationContext(AsyncOperation asyncOperation, EndOperationDelegate endDelegate, SendOrPostCallback completionCallback)
            {
                _asyncOperation = asyncOperation;
                _endDelegate = endDelegate;
                _completionCallback = completionCallback;
            }

            internal AsyncOperation AsyncOperation
            {
                get
                {
                    return _asyncOperation;
                }
            }

            internal EndOperationDelegate EndDelegate
            {
                get
                {
                    return _endDelegate;
                }
            }

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
            private System.ServiceModel.Dispatcher.ImmutableClientRuntime _runtime;

            protected ChannelBase(ClientBase<T> client)
            {
                if (client.Endpoint.Address == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxChannelFactoryEndpointAddressUri));
                }

                ChannelFactory<T> cf = client._channelFactory;
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

            private System.ServiceModel.Dispatcher.ProxyOperationRuntime GetOperationByName(string methodName)
            {
                ProxyOperationRuntime op = _runtime.GetOperationByName(methodName);
                if (op == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(string.Format(SRServiceModel.SFxMethodNotSupported1, methodName)));
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
