// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Reflection;
using System.Runtime;
using System.ServiceModel.Channels;

namespace System.ServiceModel.Dispatcher
{
    internal class ImmutableClientRuntime
    {
        private bool _addTransactionFlowProperties;
        private IInteractiveChannelInitializer[] _interactiveChannelInitializers;
        private IChannelInitializer[] _channelInitializers;
        private IClientMessageInspector[] _messageInspectors;
        private Dictionary<string, ProxyOperationRuntime> _operations;
        private bool _validateMustUnderstand;

        internal ImmutableClientRuntime(ClientRuntime behavior)
        {
            _channelInitializers = EmptyArray<IChannelInitializer>.ToArray(behavior.ChannelInitializers);
            _interactiveChannelInitializers = EmptyArray<IInteractiveChannelInitializer>.ToArray(behavior.InteractiveChannelInitializers);
            _messageInspectors = EmptyArray<IClientMessageInspector>.ToArray(behavior.MessageInspectors);

            OperationSelector = behavior.OperationSelector;
            UseSynchronizationContext = behavior.UseSynchronizationContext;
            _validateMustUnderstand = behavior.ValidateMustUnderstand;

            UnhandledProxyOperation = new ProxyOperationRuntime(behavior.UnhandledClientOperation, this);

            _addTransactionFlowProperties = behavior.AddTransactionFlowProperties;

            _operations = new Dictionary<string, ProxyOperationRuntime>();

            for (int i = 0; i < behavior.Operations.Count; i++)
            {
                ClientOperation operation = behavior.Operations[i];
                ProxyOperationRuntime operationRuntime = new ProxyOperationRuntime(operation, this);
                _operations.Add(operation.Name, operationRuntime);
            }

            CorrelationCount = _messageInspectors.Length + behavior.MaxParameterInspectors;
        }

        internal int MessageInspectorCorrelationOffset
        {
            get { return 0; }
        }

        internal int ParameterInspectorCorrelationOffset
        {
            get { return _messageInspectors.Length; }
        }

        internal int CorrelationCount { get; }

        internal IClientOperationSelector OperationSelector { get; }

        internal ProxyOperationRuntime UnhandledProxyOperation { get; }

        internal bool UseSynchronizationContext { get; }

        internal bool ValidateMustUnderstand
        {
            get { return _validateMustUnderstand; }
            set { _validateMustUnderstand = value; }
        }

        internal void AfterReceiveReply(ref ProxyRpc rpc)
        {
            int offset = MessageInspectorCorrelationOffset;
            try
            {
                for (int i = 0; i < _messageInspectors.Length; i++)
                {
                    _messageInspectors[i].AfterReceiveReply(ref rpc.Reply, rpc.Correlation[offset + i]);
                    if (WcfEventSource.Instance.ClientMessageInspectorAfterReceiveInvokedIsEnabled())
                    {
                        WcfEventSource.Instance.ClientMessageInspectorAfterReceiveInvoked(rpc.EventTraceActivity, _messageInspectors[i].GetType().FullName);
                    }
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                if (ErrorBehavior.ShouldRethrowClientSideExceptionAsIs(e))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(e);
            }
        }

        internal void BeforeSendRequest(ref ProxyRpc rpc)
        {
            int offset = MessageInspectorCorrelationOffset;
            try
            {
                for (int i = 0; i < _messageInspectors.Length; i++)
                {
                    ServiceChannel clientChannel = ServiceChannelFactory.GetServiceChannel(rpc.Channel.Proxy);
                    rpc.Correlation[offset + i] = _messageInspectors[i].BeforeSendRequest(ref rpc.Request, clientChannel);
                    if (WcfEventSource.Instance.ClientMessageInspectorBeforeSendInvokedIsEnabled())
                    {
                        WcfEventSource.Instance.ClientMessageInspectorBeforeSendInvoked(rpc.EventTraceActivity, _messageInspectors[i].GetType().FullName);
                    }
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                if (ErrorBehavior.ShouldRethrowClientSideExceptionAsIs(e))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(e);
            }
        }

        internal void DisplayInitializationUI(ServiceChannel channel)
        {
            EndDisplayInitializationUI(BeginDisplayInitializationUI(channel, null, null));
        }

        internal IAsyncResult BeginDisplayInitializationUI(ServiceChannel channel, AsyncCallback callback, object state)
        {
            return new DisplayInitializationUIAsyncResult(channel, _interactiveChannelInitializers, callback, state);
        }

        internal void EndDisplayInitializationUI(IAsyncResult result)
        {
            DisplayInitializationUIAsyncResult.End(result);
        }

        internal void InitializeChannel(IClientChannel channel)
        {
            try
            {
                for (int i = 0; i < _channelInitializers.Length; ++i)
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
                if (ErrorBehavior.ShouldRethrowClientSideExceptionAsIs(e))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(e);
            }
        }

        internal ProxyOperationRuntime GetOperation(MethodBase methodBase, object[] args, out bool canCacheResult)
        {
            if (OperationSelector == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException
                                                        (SRP.Format(SRP.SFxNeedProxyBehaviorOperationSelector2,
                                                                      methodBase.Name,
                                                                      methodBase.DeclaringType.Name)));
            }

            try
            {
                if (OperationSelector.AreParametersRequiredForSelection)
                {
                    canCacheResult = false;
                }
                else
                {
                    args = null;
                    canCacheResult = true;
                }
                string operationName = OperationSelector.SelectOperation(methodBase, args);
                ProxyOperationRuntime operation;
                if ((operationName != null) && _operations.TryGetValue(operationName, out operation))
                {
                    return operation;
                }
                else
                {
                    // did not find the right operation, will not know how 
                    // to invoke the method.
                    return null;
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                if (ErrorBehavior.ShouldRethrowClientSideExceptionAsIs(e))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(e);
            }
        }

        internal ProxyOperationRuntime GetOperationByName(string operationName)
        {
            ProxyOperationRuntime operation = null;
            if (_operations.TryGetValue(operationName, out operation))
            {
                return operation;
            }
            else
            {
                return null;
            }
        }

        internal class DisplayInitializationUIAsyncResult : AsyncResult
        {
            private ServiceChannel _channel;
            private int _index = -1;
            private IInteractiveChannelInitializer[] _initializers;
            private IClientChannel _proxy;

            private static AsyncCallback s_callback = Fx.ThunkCallback(new AsyncCallback(DisplayInitializationUIAsyncResult.Callback));

            internal DisplayInitializationUIAsyncResult(ServiceChannel channel,
                                                        IInteractiveChannelInitializer[] initializers,
                                                        AsyncCallback callback, object state)
                : base(callback, state)
            {
                _channel = channel;
                _initializers = initializers;
                _proxy = ServiceChannelFactory.GetServiceChannel(channel.Proxy);
                CallBegin(true);
            }

            private void CallBegin(bool completedSynchronously)
            {
                while (++_index < _initializers.Length)
                {
                    IAsyncResult result = null;
                    Exception exception = null;

                    try
                    {
                        result = _initializers[_index].BeginDisplayInitializationUI(
                            _proxy,
                            DisplayInitializationUIAsyncResult.s_callback,
                            this
                        );
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                        {
                            throw;
                        }

                        exception = e;
                    }

                    if (exception == null)
                    {
                        if (!result.CompletedSynchronously)
                        {
                            return;
                        }

                        CallEnd(result, out exception);
                    }

                    if (exception != null)
                    {
                        CallComplete(completedSynchronously, exception);
                        return;
                    }
                }

                CallComplete(completedSynchronously, null);
            }

            private static void Callback(IAsyncResult result)
            {
                if (result.CompletedSynchronously)
                {
                    return;
                }

                DisplayInitializationUIAsyncResult outer = (DisplayInitializationUIAsyncResult)result.AsyncState;
                Exception exception = null;

                outer.CallEnd(result, out exception);

                if (exception != null)
                {
                    outer.CallComplete(false, exception);
                    return;
                }

                outer.CallBegin(false);
            }

            private void CallEnd(IAsyncResult result, out Exception exception)
            {
                try
                {
                    _initializers[_index].EndDisplayInitializationUI(result);
                    exception = null;
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    exception = e;
                }
            }

            private void CallComplete(bool completedSynchronously, Exception exception)
            {
                Complete(completedSynchronously, exception);
            }

            internal static void End(IAsyncResult result)
            {
                System.Runtime.AsyncResult.End<DisplayInitializationUIAsyncResult>(result);
            }
        }
    }
}
