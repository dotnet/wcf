// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Globalization;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.Xml;
using SessionIdleManager = System.ServiceModel.Channels.ServiceChannel.SessionIdleManager;

namespace System.ServiceModel.Dispatcher
{
    internal class ChannelHandler
    {
        public static readonly TimeSpan CloseAfterFaultTimeout = TimeSpan.FromSeconds(10);
        public const string MessageBufferPropertyName = "_RequestMessageBuffer_";
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
        private bool _doneReceiving;
        private bool _hasSession;
        private int _isPumpAcquired;
        private bool _isChannelTerminated;
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
            Binder = binder;
            Channel = channel;

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
            Binder = binder;
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
                Binder = new BufferedReceiveBinder(Binder);
            }

            _receiver = new ErrorHandlingReceiver(Binder, channelDispatcher);
            _idleManager = idleManager;
            Fx.Assert((_idleManager != null) == (Binder.HasSession && _listener.ChannelDispatcher.DefaultCommunicationTimeouts.ReceiveTimeout != TimeSpan.MaxValue), "idle manager is present only when there is a session with a finite receive timeout");

            _requestInfo = new RequestInfo(this);

            if (_listener.State == CommunicationState.Opened)
            {
                _listener.ChannelDispatcher.Channels.IncrementActivityCount();
                _incrementedActivityCountInConstructor = true;
            }
        }

        internal IChannelBinder Binder { get; }

        internal ServiceChannel Channel { get; private set; }

        internal bool HasRegisterBeenCalled { get; private set; }

        private bool IsOpen
        {
            get { return Binder.Channel.State == CommunicationState.Opened; }
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
            HasRegisterBeenCalled = true;
            if (Binder.Channel.State == CommunicationState.Created)
            {
                ActionItem.Schedule(s_openAndEnsurePump, this);
            }
            else
            {
                EnsurePump();
            }
        }

        private void AsyncMessagePump()
        {
            IAsyncResult result = BeginTryReceive();

            if ((result != null) && result.CompletedSynchronously)
            {
                AsyncMessagePump(result);
            }
        }

        private void AsyncMessagePump(IAsyncResult result)
        {
            if (WcfEventSource.Instance.ChannelReceiveStopIsEnabled())
            {
                WcfEventSource.Instance.ChannelReceiveStop(EventTraceActivity, GetHashCode());
            }

            for (; ; )
            {
                RequestContext request;

                while (!EndTryReceive(result, out request))
                {
                    result = BeginTryReceive();

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

                result = BeginTryReceive();

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
                WcfEventSource.Instance.ChannelReceiveStart(EventTraceActivity, GetHashCode());
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
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SFxNoEndpointMatchingAddressForConnectionOpeningMessage, message.Headers.Action, "Open")));
                }

                if (MessageLogger.LoggingEnabled)
                {
                    MessageLogger.LogMessage(ref message, (operation.IsOneWay ? MessageLoggingSource.ServiceLevelReceiveDatagram : MessageLoggingSource.ServiceLevelReceiveRequest) | MessageLoggingSource.LastChance);
                }

                if (operation.IsTerminating && _hasSession)
                {
                    _isChannelTerminated = true;
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
                return HandleError(e, request, channel);
            }
            finally
            {
                if (!releasedPump)
                {
                    ReleasePump();
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
                HandleReceiveComplete(requestContext);
            }

            return valid;
        }

        private void EnsureChannelAndEndpoint(RequestContext request)
        {
            _requestInfo.Channel = Channel;

            if (_requestInfo.Channel == null)
            {
                bool addressMatched;
                if (_hasSession)
                {
                    _requestInfo.Channel = GetSessionChannel(request.RequestMessage, out _requestInfo.Endpoint, out addressMatched);
                }
                else
                {
                    _requestInfo.Channel = GetDatagramChannel(request.RequestMessage, out _requestInfo.Endpoint, out addressMatched);
                }

                if (_requestInfo.Channel == null)
                {
                    if (addressMatched)
                    {
                        ReplyContractFilterDidNotMatch(request);
                    }
                    else
                    {
                        ReplyAddressFilterDidNotMatch(request);
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
                    IAsyncResult result = BeginTryReceive();
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
            endpoint = GetEndpointDispatcher(message, out addressMatched);

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
                        endpoint.DatagramChannel = new ServiceChannel(Binder, endpoint, _listener.ChannelDispatcher, _idleManager);
                        InitializeServiceChannel(endpoint.DatagramChannel);
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

            if (Channel == null)
            {
                lock (ThisLock)
                {
                    if (Channel == null)
                    {
                        endpoint = GetEndpointDispatcher(message, out addressMatched);
                        if (endpoint != null)
                        {
                            Channel = new ServiceChannel(Binder, endpoint, _listener.ChannelDispatcher, _idleManager);
                            InitializeServiceChannel(Channel);
                        }
                    }
                }
            }

            if (Channel == null)
            {
                endpoint = null;
            }
            else
            {
                endpoint = Channel.EndpointDispatcher;
            }
            return Channel;
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
                _listener.ChannelDispatcher.ProvideFault(e, _requestInfo.Channel == null ? Binder.Channel.GetProperty<FaultConverter>() : _requestInfo.Channel.GetProperty<FaultConverter>(), ref faultInfo);
            }
            else if (Channel != null)
            {
                DispatchRuntime dispatchBehavior = Channel.ClientRuntime.CallbackDispatchRuntime;
                dispatchBehavior.ChannelDispatcher.ProvideFault(e, Channel.GetProperty<FaultConverter>(), ref faultInfo);
            }
        }

        internal bool HandleError(Exception e)
        {
            ErrorHandlerFaultInfo dummy = new ErrorHandlerFaultInfo();
            return HandleError(e, ref dummy);
        }

        private bool HandleError(Exception e, ref ErrorHandlerFaultInfo faultInfo)
        {
            if (!(e != null))
            {
                Fx.Assert(SRP.SFxNonExceptionThrown);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SFxNonExceptionThrown));
            }
            if (_listener != null)
            {
                return _listener.ChannelDispatcher.HandleError(e, ref faultInfo);
            }
            else if (Channel != null)
            {
                return Channel.ClientRuntime.CallbackDispatchRuntime.ChannelDispatcher.HandleError(e, ref faultInfo);
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
                return HandleErrorContinuation(e, request, channel, ref faultInfo, replied);
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
                    HandleError(e1);
                }
            }
            else
            {
                request.Abort();
            }
            if (!HandleError(e, ref faultInfo) && _hasSession)
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
                            HandleError(e2);
                        }
                        try
                        {
                            Binder.CloseAfterFault(timeoutHelper.RemainingTime());
                        }
                        catch (Exception e3)
                        {
                            if (Fx.IsFatal(e3))
                            {
                                throw;
                            }
                            HandleError(e3);
                        }
                    }
                    else
                    {
                        channel.Abort();
                        Binder.Abort();
                    }
                }
                else
                {
                    if (replied)
                    {
                        try
                        {
                            Binder.CloseAfterFault(CloseAfterFaultTimeout);
                        }
                        catch (Exception e4)
                        {
                            if (Fx.IsFatal(e4))
                            {
                                throw;
                            }
                            HandleError(e4);
                        }
                    }
                    else
                    {
                        Binder.Abort();
                    }
                }
            }

            return true;
        }

        private void HandleReceiveComplete(RequestContext context)
        {
            try
            {
                if (Channel != null)
                {
                    Channel.HandleReceiveComplete(context);
                }
                else
                {
                    if (context == null && _hasSession)
                    {
                        bool close;
                        lock (ThisLock)
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
                if (HandleRequestAsReply(request))
                {
                    ReleasePump();
                    return true;
                }

                if (_isChannelTerminated)
                {
                    ReleasePump();
                    ReplyChannelTerminated(request);
                    return true;
                }

                if (_requestInfo.RequestContext != null)
                {
                    Fx.Assert("ChannelHandler.HandleRequest: this.requestInfo.RequestContext != null");
                }

                _requestInfo.RequestContext = request;

                if (!TryRetrievingInstanceContext(request))
                {
                    //Would have replied and close the request.
                    return true;
                }

                _requestInfo.Channel.CompletedIOOperation();

                if (!DispatchAndReleasePump(request, true, currentOperationContext))
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
                Binder.Channel.Open();
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

                bool errorHandled = HandleError(exception);

                if (_incrementedActivityCountInConstructor)
                {
                    _listener.ChannelDispatcher.Channels.DecrementActivityCount();
                }

                if (!errorHandled)
                {
                    Binder.Channel.Abort();
                }
            }
            else
            {
                EnsurePump();
            }
        }

        private bool TryReceive(TimeSpan timeout, out RequestContext requestContext)
        {
            _shouldRejectMessageWithOnOpenActionHeader = false;
            bool valid = _receiver.TryReceive(timeout, out requestContext);

            if (valid)
            {
                HandleReceiveComplete(requestContext);
            }

            return valid;
        }

        private void ReplyAddressFilterDidNotMatch(RequestContext request)
        {
            FaultCode code = FaultCode.CreateSenderFaultCode(AddressingStrings.DestinationUnreachable,
                _messageVersion.Addressing.Namespace);
            string reason = SRP.Format(SRP.SFxNoEndpointMatchingAddress, request.RequestMessage.Headers.To);

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
                    SRP.Format(SRP.SFxMissingActionHeader, addressingVersion.Namespace), AddressingStrings.Action, addressingVersion.Namespace));
            }
            else
            {
                // some of this code is duplicated in DispatchRuntime.UnhandledActionInvoker
                // ideally both places would use FaultConverter and ActionNotSupportedException
                FaultCode code = FaultCode.CreateSenderFaultCode(AddressingStrings.ActionNotSupported,
                    _messageVersion.Addressing.Namespace);
                string reason = SRP.Format(SRP.SFxNoEndpointMatchingContract, request.RequestMessage.Headers.Action);
                ReplyFailure(request, code, reason, _messageVersion.Addressing.FaultAction);
            }
        }

        private void ReplyChannelTerminated(RequestContext request)
        {
            FaultCode code = FaultCode.CreateSenderFaultCode(FaultCodeConstants.Codes.SessionTerminated,
                FaultCodeConstants.Namespaces.NetDispatch);
            string reason = SRP.Format(SRP.SFxChannelTerminated0);
            string action = FaultCodeConstants.Actions.NetDispatcher;
            Message fault = Message.CreateMessage(_messageVersion, code, reason, action);
            ReplyFailure(request, fault, action, reason, code);
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
            HandleError(exception, ref faultInfo);
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
            else if (Channel != null && Channel.IsClient)
            {
                enableFaults = Channel.ClientRuntime.EnableFaults;
            }

            if ((!requestMessageIsFault) && enableFaults)
            {
                ProvideFault(exception, ref faultInfo);
                if (faultInfo.Fault != null)
                {
                    Message reply = faultInfo.Fault;
                    try
                    {
                        try
                        {
                            if (PrepareReply(request, reply))
                            {
                                if (_sendAsynchronously)
                                {
                                    var state = new ContinuationState { ChannelHandler = this, Channel = Channel, Exception = exception, FaultInfo = faultInfo, Request = request, Reply = reply };
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
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                        {
                            throw;
                        }
                        HandleError(e);
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
            return IsOpen && canSendReply;
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
                    EnsureChannelAndEndpoint(request);
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
                        HandleError(e, request, Channel);
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

                HandleError(e, request, Channel);
                return false;
            }
            finally
            {
                if (releasePump)
                {
                    ReleasePump();
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
                Endpoint = null;
                ExistingInstanceContext = null;
                Channel = null;
                EndpointLookupDone = false;
                DispatchRuntime = null;
                RequestContext = null;
                ChannelHandler = channelHandler;
            }

            public void Cleanup()
            {
                Endpoint = null;
                ExistingInstanceContext = null;
                Channel = null;
                EndpointLookupDone = false;
                RequestContext = null;
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
