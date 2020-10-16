// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.Diagnostics;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.Threading;
using Microsoft.Xml;
using SessionIdleManager = System.ServiceModel.Channels.ServiceChannel.SessionIdleManager;

namespace System.ServiceModel.Dispatcher
{
    internal class ChannelHandler
    {
        public static readonly TimeSpan CloseAfterFaultTimeout = TimeSpan.FromSeconds(10);
        public const string MessageBufferPropertyName = "_RequestMessageBuffer_";

        private readonly IChannelBinder _binder;
        private readonly DuplexChannelBinder _duplexBinder;
        private readonly bool _incrementedActivityCountInConstructor;
        private readonly bool _isCallback;
        private readonly ListenerHandler _listener;
        private readonly SessionIdleManager _idleManager;
        private readonly bool _sendAsynchronously;

        private static AsyncCallback s_onAsyncReplyComplete = Fx.ThunkCallback(new AsyncCallback(ChannelHandler.OnAsyncReplyComplete));
        private static AsyncCallback s_onAsyncReceiveComplete = Fx.ThunkCallback(new AsyncCallback(ChannelHandler.OnAsyncReceiveComplete));
        private static Action<object> s_onContinueAsyncReceive = new Action<object>(ChannelHandler.OnContinueAsyncReceive);
        private static Action<object> s_onStartSyncMessagePump = new Action<object>(ChannelHandler.OnStartSyncMessagePump);
        private static Action<object> s_onStartAsyncMessagePump = new Action<object>(ChannelHandler.OnStartAsyncMessagePump);
        private static Action<object> s_openAndEnsurePump = new Action<object>(ChannelHandler.OpenAndEnsurePump);

        private RequestInfo _requestInfo;
        private ServiceChannel _channel;
        private bool _doneReceiving;
        private bool _hasRegisterBeenCalled;
        private bool _hasSession;
        private int _isPumpAcquired;
        private bool _isConcurrent;
        private bool _isManualAddressing;
        private MessageVersion _messageVersion;
        private ErrorHandlingReceiver _receiver;
        private bool _receiveSynchronously;
        private RequestContext _replied;
        private EventTraceActivity _eventTraceActivity;
        private bool _shouldRejectMessageWithOnOpenActionHeader;
        private object _acquirePumpLock = new object();

        internal ChannelHandler(MessageVersion messageVersion, IChannelBinder binder, ServiceChannel channel)
        {
            ClientRuntime clientRuntime = channel.ClientRuntime;

            _messageVersion = messageVersion;
            _isManualAddressing = clientRuntime.ManualAddressing;
            _binder = binder;
            _channel = channel;

            _isConcurrent = true;
            _duplexBinder = binder as DuplexChannelBinder;
            _hasSession = binder.HasSession;
            _isCallback = true;

            DispatchRuntime dispatchRuntime = clientRuntime.DispatchRuntime;
            if (dispatchRuntime == null)
            {
                _receiver = new ErrorHandlingReceiver(binder, null);
            }
            else
            {
                _receiver = new ErrorHandlingReceiver(binder, dispatchRuntime.ChannelDispatcher);
            }
            _requestInfo = new RequestInfo(this);
        }

        internal ChannelHandler(MessageVersion messageVersion, IChannelBinder binder,
            ListenerHandler listener, SessionIdleManager idleManager)
        {
            ChannelDispatcher channelDispatcher = listener.ChannelDispatcher;

            _messageVersion = messageVersion;
            _isManualAddressing = channelDispatcher.ManualAddressing;
            _binder = binder;
            _listener = listener;

            _receiveSynchronously = channelDispatcher.ReceiveSynchronously;
            _sendAsynchronously = channelDispatcher.SendAsynchronously;
            _duplexBinder = binder as DuplexChannelBinder;
            _hasSession = binder.HasSession;
            _isConcurrent = ConcurrencyBehavior.IsConcurrent(channelDispatcher, _hasSession);

            if (channelDispatcher.MaxPendingReceives > 1)
            {
                throw NotImplemented.ByDesign;
            }

            if (channelDispatcher.BufferedReceiveEnabled)
            {
                _binder = new BufferedReceiveBinder(_binder);
            }

            _receiver = new ErrorHandlingReceiver(_binder, channelDispatcher);
            _idleManager = idleManager;
            Fx.Assert((_idleManager != null) == (_binder.HasSession && _listener.ChannelDispatcher.DefaultCommunicationTimeouts.ReceiveTimeout != TimeSpan.MaxValue), "idle manager is present only when there is a session with a finite receive timeout");

            _requestInfo = new RequestInfo(this);

            if (_listener.State == CommunicationState.Opened)
            {
                _listener.ChannelDispatcher.Channels.IncrementActivityCount();
                _incrementedActivityCountInConstructor = true;
            }
        }

        internal IChannelBinder Binder
        {
            get { return _binder; }
        }

        internal ServiceChannel Channel
        {
            get { return _channel; }
        }

        internal bool HasRegisterBeenCalled
        {
            get { return _hasRegisterBeenCalled; }
        }

        private bool IsOpen
        {
            get { return _binder.Channel.State == CommunicationState.Opened; }
        }

        private object ThisLock
        {
            get { return this; }
        }

        private EventTraceActivity EventTraceActivity
        {
            get
            {
                if (_eventTraceActivity == null)
                {
                    _eventTraceActivity = new EventTraceActivity();
                }
                return _eventTraceActivity;
            }
        }

        internal static void Register(ChannelHandler handler)
        {
            handler.Register();
        }

        internal static void Register(ChannelHandler handler, RequestContext request)
        {
            BufferedReceiveBinder bufferedBinder = handler.Binder as BufferedReceiveBinder;
            Fx.Assert(bufferedBinder != null, "ChannelHandler.Binder is not a BufferedReceiveBinder");

            bufferedBinder.InjectRequest(request);
            handler.Register();
        }

        private void Register()
        {
            _hasRegisterBeenCalled = true;
            if (_binder.Channel.State == CommunicationState.Created)
            {
                ActionItem.Schedule(s_openAndEnsurePump, this);
            }
            else
            {
                this.EnsurePump();
            }
        }

        private void AsyncMessagePump()
        {
            IAsyncResult result = this.BeginTryReceive();

            if ((result != null) && result.CompletedSynchronously)
            {
                this.AsyncMessagePump(result);
            }
        }

        private void AsyncMessagePump(IAsyncResult result)
        {
            if (WcfEventSource.Instance.ChannelReceiveStopIsEnabled())
            {
                WcfEventSource.Instance.ChannelReceiveStop(this.EventTraceActivity, this.GetHashCode());
            }

            for (; ; )
            {
                RequestContext request;

                while (!this.EndTryReceive(result, out request))
                {
                    result = this.BeginTryReceive();

                    if ((result == null) || !result.CompletedSynchronously)
                    {
                        return;
                    }
                }

                if (!HandleRequest(request, null))
                {
                    break;
                }

                if (!TryAcquirePump())
                {
                    break;
                }

                result = this.BeginTryReceive();

                if (result == null || !result.CompletedSynchronously)
                {
                    break;
                }
            }
        }

        private IAsyncResult BeginTryReceive()
        {
            _requestInfo.Cleanup();

            if (WcfEventSource.Instance.ChannelReceiveStartIsEnabled())
            {
                WcfEventSource.Instance.ChannelReceiveStart(this.EventTraceActivity, this.GetHashCode());
            }

            return _receiver.BeginTryReceive(TimeSpan.MaxValue, ChannelHandler.s_onAsyncReceiveComplete, this);
        }

        private bool DispatchAndReleasePump(RequestContext request, bool cleanThread, OperationContext currentOperationContext)
        {
            ServiceChannel channel = _requestInfo.Channel;
            EndpointDispatcher endpoint = _requestInfo.Endpoint;
            bool releasedPump = false;

            try
            {
                DispatchRuntime dispatchBehavior = _requestInfo.DispatchRuntime;

                if (channel == null || dispatchBehavior == null)
                {
                    Fx.Assert("System.ServiceModel.Dispatcher.ChannelHandler.Dispatch(): (channel == null || dispatchBehavior == null)");
                    return true;
                }

                EventTraceActivity eventTraceActivity = TraceDispatchMessageStart(request.RequestMessage);
                Message message = request.RequestMessage;

                DispatchOperationRuntime operation = dispatchBehavior.GetOperation(ref message);
                if (operation == null)
                {
                    Fx.Assert("ChannelHandler.Dispatch (operation == null)");
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "No DispatchOperationRuntime found to process message.")));
                }

                if (_shouldRejectMessageWithOnOpenActionHeader && message.Headers.Action == OperationDescription.SessionOpenedAction)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxNoEndpointMatchingAddressForConnectionOpeningMessage, message.Headers.Action, "Open")));
                }

                if (MessageLogger.LoggingEnabled)
                {
                    MessageLogger.LogMessage(ref message, (operation.IsOneWay ? MessageLoggingSource.ServiceLevelReceiveDatagram : MessageLoggingSource.ServiceLevelReceiveRequest) | MessageLoggingSource.LastChance);
                }

                bool hasOperationContextBeenSet;
                if (currentOperationContext != null)
                {
                    hasOperationContextBeenSet = true;
                    currentOperationContext.ReInit(request, message, channel);
                }
                else
                {
                    hasOperationContextBeenSet = false;
                    currentOperationContext = new OperationContext(request, message, channel);
                }

                MessageRpc rpc = new MessageRpc(request, message, operation, channel,
                    this, cleanThread, currentOperationContext, _requestInfo.ExistingInstanceContext, eventTraceActivity);

                TraceUtility.MessageFlowAtMessageReceived(message, currentOperationContext, eventTraceActivity, true);

                // These need to happen before Dispatch but after accessing any ChannelHandler
                // state, because we go multi-threaded after this until we reacquire pump mutex.
                ReleasePump();
                releasedPump = true;

                return operation.Parent.Dispatch(ref rpc, hasOperationContextBeenSet);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                return this.HandleError(e, request, channel);
            }
            finally
            {
                if (!releasedPump)
                {
                    this.ReleasePump();
                }
            }
        }

        internal void DispatchDone()
        {
        }


        private bool EndTryReceive(IAsyncResult result, out RequestContext requestContext)
        {
            bool valid;

            {
                valid = _receiver.EndTryReceive(result, out requestContext);
            }

            if (valid)
            {
                this.HandleReceiveComplete(requestContext);
            }

            return valid;
        }

        private void EnsureChannelAndEndpoint(RequestContext request)
        {
            _requestInfo.Channel = _channel;

            if (_requestInfo.Channel == null)
            {
                bool addressMatched;
                if (_hasSession)
                {
                    _requestInfo.Channel = this.GetSessionChannel(request.RequestMessage, out _requestInfo.Endpoint, out addressMatched);
                }
                else
                {
                    _requestInfo.Channel = this.GetDatagramChannel(request.RequestMessage, out _requestInfo.Endpoint, out addressMatched);
                }

                if (_requestInfo.Channel == null)
                {
                    if (addressMatched)
                    {
                        this.ReplyContractFilterDidNotMatch(request);
                    }
                    else
                    {
                        this.ReplyAddressFilterDidNotMatch(request);
                    }
                }
            }
            else
            {
                _requestInfo.Endpoint = _requestInfo.Channel.EndpointDispatcher;
            }

            _requestInfo.EndpointLookupDone = true;

            if (_requestInfo.Channel == null)
            {
                // SFx drops a message here
                TraceUtility.TraceDroppedMessage(request.RequestMessage, _requestInfo.Endpoint);
                request.Close();
                return;
            }

            if (_requestInfo.Channel.HasSession || _isCallback)
            {
                _requestInfo.DispatchRuntime = _requestInfo.Channel.DispatchRuntime;
            }
            else
            {
                _requestInfo.DispatchRuntime = _requestInfo.Endpoint.DispatchRuntime;
            }
        }

        private void EnsurePump()
        {
            if (TryAcquirePump())
            {
                if (_receiveSynchronously)
                {
                    ActionItem.Schedule(ChannelHandler.s_onStartSyncMessagePump, this);
                }
                else
                {
                    IAsyncResult result = this.BeginTryReceive();
                    if ((result != null) && result.CompletedSynchronously)
                    {
                        ActionItem.Schedule(ChannelHandler.s_onContinueAsyncReceive, result);
                    }
                }
            }
        }

        private ServiceChannel GetDatagramChannel(Message message, out EndpointDispatcher endpoint, out bool addressMatched)
        {
            addressMatched = false;
            endpoint = this.GetEndpointDispatcher(message, out addressMatched);

            if (endpoint == null)
            {
                return null;
            }

            if (endpoint.DatagramChannel == null)
            {
                lock (_listener.ThisLock)
                {
                    if (endpoint.DatagramChannel == null)
                    {
                        endpoint.DatagramChannel = new ServiceChannel(_binder, endpoint, _listener.ChannelDispatcher, _idleManager);
                        this.InitializeServiceChannel(endpoint.DatagramChannel);
                    }
                }
            }

            return endpoint.DatagramChannel;
        }

        private EndpointDispatcher GetEndpointDispatcher(Message message, out bool addressMatched)
        {
            return _listener.Endpoints.Lookup(message, out addressMatched);
        }

        private ServiceChannel GetSessionChannel(Message message, out EndpointDispatcher endpoint, out bool addressMatched)
        {
            addressMatched = false;

            if (_channel == null)
            {
                lock (this.ThisLock)
                {
                    if (_channel == null)
                    {
                        endpoint = this.GetEndpointDispatcher(message, out addressMatched);
                        if (endpoint != null)
                        {
                            _channel = new ServiceChannel(_binder, endpoint, _listener.ChannelDispatcher, _idleManager);
                            this.InitializeServiceChannel(_channel);
                        }
                    }
                }
            }

            if (_channel == null)
            {
                endpoint = null;
            }
            else
            {
                endpoint = _channel.EndpointDispatcher;
            }
            return _channel;
        }

        private void InitializeServiceChannel(ServiceChannel channel)
        {
            ClientRuntime clientRuntime = channel.ClientRuntime;
            if (clientRuntime != null)
            {
                Type contractType = clientRuntime.ContractClientType;
                Type callbackType = clientRuntime.CallbackClientType;

                if (contractType != null)
                {
                    channel.Proxy = ServiceChannelFactory.CreateProxy(contractType, callbackType, MessageDirection.Output, channel);
                }
            }

            if (_listener != null)
            {
                _listener.ChannelDispatcher.InitializeChannel((IClientChannel)channel.Proxy);
            }

            ((IChannel)channel).Open();
        }

        private void ProvideFault(Exception e, ref ErrorHandlerFaultInfo faultInfo)
        {
            if (_listener != null)
            {
                _listener.ChannelDispatcher.ProvideFault(e, _requestInfo.Channel == null ? _binder.Channel.GetProperty<FaultConverter>() : _requestInfo.Channel.GetProperty<FaultConverter>(), ref faultInfo);
            }
            else if (_channel != null)
            {
                DispatchRuntime dispatchBehavior = _channel.ClientRuntime.CallbackDispatchRuntime;
                dispatchBehavior.ChannelDispatcher.ProvideFault(e, _channel.GetProperty<FaultConverter>(), ref faultInfo);
            }
        }

        internal bool HandleError(Exception e)
        {
            ErrorHandlerFaultInfo dummy = new ErrorHandlerFaultInfo();
            return this.HandleError(e, ref dummy);
        }

        private bool HandleError(Exception e, ref ErrorHandlerFaultInfo faultInfo)
        {
            if (!(e != null))
            {
                Fx.Assert(SRServiceModel.SFxNonExceptionThrown);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.SFxNonExceptionThrown));
            }
            if (_listener != null)
            {
                return _listener.ChannelDispatcher.HandleError(e, ref faultInfo);
            }
            else if (_channel != null)
            {
                return _channel.ClientRuntime.CallbackDispatchRuntime.ChannelDispatcher.HandleError(e, ref faultInfo);
            }
            else
            {
                return false;
            }
        }

        private bool HandleError(Exception e, RequestContext request, ServiceChannel channel)
        {
            ErrorHandlerFaultInfo faultInfo = new ErrorHandlerFaultInfo(_messageVersion.Addressing.DefaultFaultAction);
            bool replied, replySentAsync;
            ProvideFaultAndReplyFailure(request, e, ref faultInfo, out replied, out replySentAsync);

            if (!replySentAsync)
            {
                return this.HandleErrorContinuation(e, request, channel, ref faultInfo, replied);
            }
            else
            {
                return false;
            }
        }

        private bool HandleErrorContinuation(Exception e, RequestContext request, ServiceChannel channel, ref ErrorHandlerFaultInfo faultInfo, bool replied)
        {
            if (replied)
            {
                try
                {
                    request.Close();
                }
                catch (Exception e1)
                {
                    if (Fx.IsFatal(e1))
                    {
                        throw;
                    }
                    this.HandleError(e1);
                }
            }
            else
            {
                request.Abort();
            }
            if (!this.HandleError(e, ref faultInfo) && _hasSession)
            {
                if (channel != null)
                {
                    if (replied)
                    {
                        TimeoutHelper timeoutHelper = new TimeoutHelper(CloseAfterFaultTimeout);
                        try
                        {
                            channel.Close(timeoutHelper.RemainingTime());
                        }
                        catch (Exception e2)
                        {
                            if (Fx.IsFatal(e2))
                            {
                                throw;
                            }
                            this.HandleError(e2);
                        }
                        try
                        {
                            _binder.CloseAfterFault(timeoutHelper.RemainingTime());
                        }
                        catch (Exception e3)
                        {
                            if (Fx.IsFatal(e3))
                            {
                                throw;
                            }
                            this.HandleError(e3);
                        }
                    }
                    else
                    {
                        channel.Abort();
                        _binder.Abort();
                    }
                }
                else
                {
                    if (replied)
                    {
                        try
                        {
                            _binder.CloseAfterFault(CloseAfterFaultTimeout);
                        }
                        catch (Exception e4)
                        {
                            if (Fx.IsFatal(e4))
                            {
                                throw;
                            }
                            this.HandleError(e4);
                        }
                    }
                    else
                    {
                        _binder.Abort();
                    }
                }
            }

            return true;
        }

        private void HandleReceiveComplete(RequestContext context)
        {
            try
            {
                if (_channel != null)
                {
                    _channel.HandleReceiveComplete(context);
                }
                else
                {
                    if (context == null && _hasSession)
                    {
                        bool close;
                        lock (this.ThisLock)
                        {
                            close = !_doneReceiving;
                            _doneReceiving = true;
                        }

                        if (close)
                        {
                            _receiver.Close();

                            if (_idleManager != null)
                            {
                                _idleManager.CancelTimer();
                            }
                        }
                    }
                }
            }
            finally
            {
                if ((context == null) && _incrementedActivityCountInConstructor)
                {
                    _listener.ChannelDispatcher.Channels.DecrementActivityCount();
                }
            }
        }

        private bool HandleRequest(RequestContext request, OperationContext currentOperationContext)
        {
            if (request == null)
            {
                // channel EOF, stop receiving
                return false;
            }

            ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? TraceUtility.ExtractActivity(request.RequestMessage) : null;
            using (ServiceModelActivity.BoundOperation(activity))
            {
                if (this.HandleRequestAsReply(request))
                {
                    this.ReleasePump();
                    return true;
                }
                if (_requestInfo.RequestContext != null)
                {
                    Fx.Assert("ChannelHandler.HandleRequest: this.requestInfo.RequestContext != null");
                }

                _requestInfo.RequestContext = request;

                if (!this.TryRetrievingInstanceContext(request))
                {
                    //Would have replied and close the request.
                    return true;
                }

                _requestInfo.Channel.CompletedIOOperation();

                if (!this.DispatchAndReleasePump(request, true, currentOperationContext))
                {
                    // this.DispatchDone will be called to continue
                    return false;
                }
            }
            return true;
        }

        private bool HandleRequestAsReply(RequestContext request)
        {
            if (_duplexBinder != null)
            {
                if (_duplexBinder.HandleRequestAsReply(request.RequestMessage))
                {
                    return true;
                }
            }
            return false;
        }

        private static void OnStartAsyncMessagePump(object state)
        {
            ((ChannelHandler)state).AsyncMessagePump();
        }

        private static void OnStartSyncMessagePump(object state)
        {
            ChannelHandler handler = state as ChannelHandler;

            if (WcfEventSource.Instance.ChannelReceiveStopIsEnabled())
            {
                WcfEventSource.Instance.ChannelReceiveStop(handler.EventTraceActivity, state.GetHashCode());
            }
            handler.SyncMessagePump();
        }

        private static void OnAsyncReceiveComplete(IAsyncResult result)
        {
            if (!result.CompletedSynchronously)
            {
                ((ChannelHandler)result.AsyncState).AsyncMessagePump(result);
            }
        }

        private static void OnContinueAsyncReceive(object state)
        {
            IAsyncResult result = (IAsyncResult)state;
            ((ChannelHandler)result.AsyncState).AsyncMessagePump(result);
        }

        private static void OpenAndEnsurePump(object state)
        {
            ((ChannelHandler)state).OpenAndEnsurePump();
        }

        private void OpenAndEnsurePump()
        {
            Exception exception = null;
            try
            {
                _binder.Channel.Open();
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                exception = e;
            }

            if (exception != null)
            {
                SessionIdleManager idleManager = _idleManager;
                if (idleManager != null)
                {
                    idleManager.CancelTimer();
                }

                bool errorHandled = this.HandleError(exception);

                if (_incrementedActivityCountInConstructor)
                {
                    _listener.ChannelDispatcher.Channels.DecrementActivityCount();
                }

                if (!errorHandled)
                {
                    _binder.Channel.Abort();
                }
            }
            else
            {
                this.EnsurePump();
            }
        }

        private bool TryReceive(TimeSpan timeout, out RequestContext requestContext)
        {
            _shouldRejectMessageWithOnOpenActionHeader = false;
            bool valid = _receiver.TryReceive(timeout, out requestContext);

            if (valid)
            {
                this.HandleReceiveComplete(requestContext);
            }

            return valid;
        }

        private void ReplyAddressFilterDidNotMatch(RequestContext request)
        {
            FaultCode code = FaultCode.CreateSenderFaultCode(AddressingStrings.DestinationUnreachable,
                _messageVersion.Addressing.Namespace);
            string reason = string.Format(SRServiceModel.SFxNoEndpointMatchingAddress, request.RequestMessage.Headers.To);

            ReplyFailure(request, code, reason);
        }

        private void ReplyContractFilterDidNotMatch(RequestContext request)
        {
            // By default, the contract filter is just a filter over the set of initiating actions in 
            // the contract, so we do error messages accordingly
            AddressingVersion addressingVersion = _messageVersion.Addressing;
            if (addressingVersion != AddressingVersion.None && request.RequestMessage.Headers.Action == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new MessageHeaderException(
                    string.Format(SRServiceModel.SFxMissingActionHeader, addressingVersion.Namespace), AddressingStrings.Action, addressingVersion.Namespace));
            }
            else
            {
                // some of this code is duplicated in DispatchRuntime.UnhandledActionInvoker
                // ideally both places would use FaultConverter and ActionNotSupportedException
                FaultCode code = FaultCode.CreateSenderFaultCode(AddressingStrings.ActionNotSupported,
                    _messageVersion.Addressing.Namespace);
                string reason = string.Format(SRServiceModel.SFxNoEndpointMatchingContract, request.RequestMessage.Headers.Action);
                ReplyFailure(request, code, reason, _messageVersion.Addressing.FaultAction);
            }
        }

        private void ReplyFailure(RequestContext request, FaultCode code, string reason)
        {
            string action = _messageVersion.Addressing.DefaultFaultAction;
            ReplyFailure(request, code, reason, action);
        }

        private void ReplyFailure(RequestContext request, FaultCode code, string reason, string action)
        {
            Message fault = Message.CreateMessage(_messageVersion, code, reason, action);
            ReplyFailure(request, fault, action, reason, code);
        }

        private void ReplyFailure(RequestContext request, Message fault, string action, string reason, FaultCode code)
        {
            FaultException exception = new FaultException(reason, code);
            ErrorBehavior.ThrowAndCatch(exception);
            ErrorHandlerFaultInfo faultInfo = new ErrorHandlerFaultInfo(action);
            faultInfo.Fault = fault;
            bool replied, replySentAsync;
            ProvideFaultAndReplyFailure(request, exception, ref faultInfo, out replied, out replySentAsync);
            this.HandleError(exception, ref faultInfo);
        }

        private void ProvideFaultAndReplyFailure(RequestContext request, Exception exception, ref ErrorHandlerFaultInfo faultInfo, out bool replied, out bool replySentAsync)
        {
            replied = false;
            replySentAsync = false;
            bool requestMessageIsFault = false;
            try
            {
                requestMessageIsFault = request.RequestMessage.IsFault;
            }
#pragma warning disable 56500 // covered by FxCOP
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                // do not propagate non-fatal exceptions
            }

            bool enableFaults = false;
            if (_listener != null)
            {
                enableFaults = _listener.ChannelDispatcher.EnableFaults;
            }
            else if (_channel != null && _channel.IsClient)
            {
                enableFaults = _channel.ClientRuntime.EnableFaults;
            }

            if ((!requestMessageIsFault) && enableFaults)
            {
                this.ProvideFault(exception, ref faultInfo);
                if (faultInfo.Fault != null)
                {
                    Message reply = faultInfo.Fault;
                    try
                    {
                        try
                        {
                            if (this.PrepareReply(request, reply))
                            {
                                if (_sendAsynchronously)
                                {
                                    var state = new ContinuationState { ChannelHandler = this, Channel = _channel, Exception = exception, FaultInfo = faultInfo, Request = request, Reply = reply };
                                    var result = request.BeginReply(reply, ChannelHandler.s_onAsyncReplyComplete, state);
                                    if (result.CompletedSynchronously)
                                    {
                                        ChannelHandler.AsyncReplyComplete(result, state);
                                        replied = true;
                                    }
                                    else
                                    {
                                        replySentAsync = true;
                                    }
                                }
                                else
                                {
                                    request.Reply(reply);
                                    replied = true;
                                }
                            }
                        }
                        finally
                        {
                            if (!replySentAsync)
                            {
                                reply.Close();
                            }
                        }
                    }
#pragma warning disable 56500 // covered by FxCOP
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                        {
                            throw;
                        }
                        this.HandleError(e);
                    }
                }
            }
        }

        /// <summary>
        /// Prepares a reply that can either be sent asynchronously or synchronously depending on the value of 
        /// sendAsynchronously
        /// </summary>
        /// <param name="request">The request context to prepare</param>
        /// <param name="reply">The reply to prepare</param>
        /// <returns>True if channel is open and prepared reply should be sent; otherwise false.</returns>
        private bool PrepareReply(RequestContext request, Message reply)
        {
            // Ensure we only reply once (we may hit the same error multiple times)
            if (_replied == request)
            {
                return false;
            }
            _replied = request;

            bool canSendReply = true;

            Message requestMessage = null;
            try
            {
                requestMessage = request.RequestMessage;
            }
#pragma warning disable 56500 // covered by FxCOP
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                // do not propagate non-fatal exceptions
            }
            if (!object.ReferenceEquals(requestMessage, null))
            {
                UniqueId requestID = null;
                try
                {
                    requestID = requestMessage.Headers.MessageId;
                }
                catch (MessageHeaderException)
                {
                    // Do not propagate this exception - we don't need to correlate the reply if the MessageId header is bad
                }
                if (!object.ReferenceEquals(requestID, null) && !_isManualAddressing)
                {
                    System.ServiceModel.Channels.RequestReplyCorrelator.PrepareReply(reply, requestID);
                }
                if (!_hasSession && !_isManualAddressing)
                {
                    try
                    {
                        canSendReply = System.ServiceModel.Channels.RequestReplyCorrelator.AddressReply(reply, requestMessage);
                    }
                    catch (MessageHeaderException)
                    {
                        // Do not propagate this exception - we don't need to address the reply if the FaultTo header is bad
                    }
                }
            }

            // ObjectDisposeException can happen
            // if the channel is closed in a different
            // thread. 99% this check will avoid false
            // exceptions.
            return this.IsOpen && canSendReply;
        }

        private static void AsyncReplyComplete(IAsyncResult result, ContinuationState state)
        {
            try
            {
                state.Request.EndReply(result);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                state.ChannelHandler.HandleError(e);
            }

            try
            {
                state.Reply.Close();
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                state.ChannelHandler.HandleError(e);
            }

            try
            {
                state.ChannelHandler.HandleErrorContinuation(state.Exception, state.Request, state.Channel, ref state.FaultInfo, true);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                state.ChannelHandler.HandleError(e);
            }

            state.ChannelHandler.EnsurePump();
        }

        private static void OnAsyncReplyComplete(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            try
            {
                var state = (ContinuationState)result.AsyncState;
                ChannelHandler.AsyncReplyComplete(result, state);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
            }
        }

        private void ReleasePump()
        {
            if (_isConcurrent)
            {
                lock (_acquirePumpLock)
                {
                    _isPumpAcquired = 0;
                }
            }
        }

        private void SyncMessagePump()
        {
            OperationContext existingOperationContext = OperationContext.Current;
            try
            {
                OperationContext currentOperationContext = new OperationContext();
                OperationContext.Current = currentOperationContext;

                for (; ; )
                {
                    RequestContext request;

                    _requestInfo.Cleanup();

                    while (!TryReceive(TimeSpan.MaxValue, out request))
                    {
                    }

                    if (!HandleRequest(request, currentOperationContext))
                    {
                        break;
                    }

                    if (!TryAcquirePump())
                    {
                        break;
                    }

                    currentOperationContext.Recycle();
                }
            }
            finally
            {
                OperationContext.Current = existingOperationContext;
            }
        }

        //Return: False denotes failure, Caller should discard the request.
        //      : True denotes operation is successful.
        private bool TryRetrievingInstanceContext(RequestContext request)
        {
            bool releasePump = true;
            try
            {
                if (!_requestInfo.EndpointLookupDone)
                {
                    this.EnsureChannelAndEndpoint(request);
                }

                if (_requestInfo.Channel == null)
                {
                    return false;
                }

                if (_requestInfo.DispatchRuntime != null)
                {
                    IContextChannel transparentProxy = _requestInfo.Channel.Proxy as IContextChannel;
                    try
                    {
                        _requestInfo.ExistingInstanceContext = _requestInfo.DispatchRuntime.InstanceContextProvider.GetExistingInstanceContext(request.RequestMessage, transparentProxy);
                        releasePump = false;
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                        {
                            throw;
                        }
                        _requestInfo.Channel = null;
                        this.HandleError(e, request, _channel);
                        return false;
                    }
                }
                else
                {
                    // This can happen if we are pumping for an async client,
                    // and we receive a bogus reply.  In that case, there is no
                    // DispatchRuntime, because we are only expecting replies.
                    //
                    // One possible fix for this would be in DuplexChannelBinder
                    // to drop all messages with a RelatesTo that do not match a
                    // pending request.
                    //
                    // However, that would not fix:
                    // (a) we could get a valid request message with a
                    // RelatesTo that we should try to process.
                    // (b) we could get a reply message that does not have
                    // a RelatesTo.
                    //
                    // So we do the null check here.
                    //
                    // SFx drops a message here
                    TraceUtility.TraceDroppedMessage(request.RequestMessage, _requestInfo.Endpoint);
                    request.Close();
                    return false;
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                this.HandleError(e, request, _channel);
                return false;
            }
            finally
            {
                if (releasePump)
                {
                    this.ReleasePump();
                }
            }
            return true;
        }

        private bool TryAcquirePump()
        {
            if (_isConcurrent)
            {
                lock (_acquirePumpLock)
                {
                    if (_isPumpAcquired != 0)
                    {
                        return false;
                    }

                    _isPumpAcquired = 1;
                    return true;
                }
            }

            return true;
        }

        private struct RequestInfo
        {
            public EndpointDispatcher Endpoint;
            public InstanceContext ExistingInstanceContext;
            public ServiceChannel Channel;
            public bool EndpointLookupDone;
            public DispatchRuntime DispatchRuntime;
            public RequestContext RequestContext;
            public ChannelHandler ChannelHandler;

            public RequestInfo(ChannelHandler channelHandler)
            {
                this.Endpoint = null;
                this.ExistingInstanceContext = null;
                this.Channel = null;
                this.EndpointLookupDone = false;
                this.DispatchRuntime = null;
                this.RequestContext = null;
                this.ChannelHandler = channelHandler;
            }

            public void Cleanup()
            {
                this.Endpoint = null;
                this.ExistingInstanceContext = null;
                this.Channel = null;
                this.EndpointLookupDone = false;
                this.RequestContext = null;
            }
        }

        private EventTraceActivity TraceDispatchMessageStart(Message message)
        {
            if (FxTrace.Trace.IsEnd2EndActivityTracingEnabled && message != null)
            {
                EventTraceActivity eventTraceActivity = EventTraceActivityHelper.TryExtractActivity(message);
                if (WcfEventSource.Instance.DispatchMessageStartIsEnabled())
                {
                    WcfEventSource.Instance.DispatchMessageStart(eventTraceActivity);
                }
                return eventTraceActivity;
            }

            return null;
        }

        /// <summary>
        /// Data structure used to carry state for asynchronous replies
        /// </summary>
        private struct ContinuationState
        {
            public ChannelHandler ChannelHandler;
            public Exception Exception;
            public RequestContext Request;
            public Message Reply;
            public ServiceChannel Channel;
            public ErrorHandlerFaultInfo FaultInfo;
        }
    }
}
