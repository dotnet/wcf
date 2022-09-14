// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.ObjectModel;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.Security;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.Xml;

namespace System.ServiceModel.Dispatcher
{
    internal delegate void MessageRpcProcessor(ref MessageRpc rpc);

    internal struct MessageRpc
    {
        internal readonly ServiceChannel Channel;
        internal readonly ChannelHandler channelHandler;
        internal readonly object[] Correlation;
        internal readonly OperationContext OperationContext;
        internal ServiceModelActivity Activity;
        internal Guid ResponseActivityId;
        internal IAsyncResult AsyncResult;
        internal bool CanSendReply;
        internal bool SuccessfullySendReply;
        internal object[] InputParameters;
        internal object[] OutputParameters;
        internal object ReturnParameter;
        internal bool ParametersDisposed;
        internal bool DidDeserializeRequestBody;
        internal Exception Error;
        internal MessageRpcProcessor ErrorProcessor;
        internal ErrorHandlerFaultInfo FaultInfo;
        internal bool HasSecurityContext;
        internal object Instance;
        internal bool MessageRpcOwnsInstanceContextThrottle;
        internal MessageRpcProcessor NextProcessor;
        internal Collection<MessageHeaderInfo> NotUnderstoodHeaders;
        internal DispatchOperationRuntime Operation;
        internal Message Request;
        internal RequestContext RequestContext;
        internal bool RequestContextThrewOnReply;
        internal UniqueId RequestID;
        internal Message Reply;
        internal TimeoutHelper ReplyTimeoutHelper;
        internal RequestReplyCorrelator.ReplyToInfo ReplyToInfo;
        internal MessageVersion RequestVersion;
        internal ServiceSecurityContext SecurityContext;
        internal InstanceContext InstanceContext;
        internal bool SuccessfullyBoundInstance;
        internal bool SuccessfullyIncrementedActivity;
        internal bool SuccessfullyLockedInstance;
        internal MessageRpcInvokeNotification InvokeNotification;
        internal EventTraceActivity EventTraceActivity;
        private bool _isInstanceContextSingleton;
        private SignalGate<IAsyncResult> _invokeContinueGate;

        internal MessageRpc(RequestContext requestContext, Message request, DispatchOperationRuntime operation,
            ServiceChannel channel, ChannelHandler channelHandler, bool cleanThread,
            OperationContext operationContext, InstanceContext instanceContext, EventTraceActivity eventTraceActivity)
        {
            Fx.Assert((operationContext != null), "System.ServiceModel.Dispatcher.MessageRpc.MessageRpc(), operationContext == null");
            Fx.Assert(channelHandler != null, "System.ServiceModel.Dispatcher.MessageRpc.MessageRpc(), channelHandler == null");

            Activity = null;
            EventTraceActivity = eventTraceActivity;
            AsyncResult = null;
            CanSendReply = true;
            Channel = channel;
            this.channelHandler = channelHandler;
            Correlation = EmptyArray<object>.Allocate(operation.Parent.CorrelationCount);
            DidDeserializeRequestBody = false;
            Error = null;
            ErrorProcessor = null;
            FaultInfo = new ErrorHandlerFaultInfo(request.Version.Addressing.DefaultFaultAction);
            HasSecurityContext = false;
            Instance = null;
            MessageRpcOwnsInstanceContextThrottle = false;
            NextProcessor = null;
            NotUnderstoodHeaders = null;
            Operation = operation;
            OperationContext = operationContext;
            IsPaused = false;
            ParametersDisposed = false;
            Request = request;
            RequestContext = requestContext;
            RequestContextThrewOnReply = false;
            SuccessfullySendReply = false;
            RequestVersion = request.Version;
            Reply = null;
            ReplyTimeoutHelper = new TimeoutHelper();
            SecurityContext = null;
            InstanceContext = instanceContext;
            SuccessfullyBoundInstance = false;
            SuccessfullyIncrementedActivity = false;
            SuccessfullyLockedInstance = false;
            SwitchedThreads = !cleanThread;
            InputParameters = null;
            OutputParameters = null;
            ReturnParameter = null;
            _isInstanceContextSingleton = false;
            _invokeContinueGate = null;

            if (!operation.IsOneWay && !operation.Parent.ManualAddressing)
            {
                RequestID = request.Headers.MessageId;
                ReplyToInfo = new RequestReplyCorrelator.ReplyToInfo(request);
            }
            else
            {
                RequestID = null;
                ReplyToInfo = new RequestReplyCorrelator.ReplyToInfo();
            }

            if (DiagnosticUtility.ShouldUseActivity)
            {
                Activity = TraceUtility.ExtractActivity(Request);
            }

            if (DiagnosticUtility.ShouldUseActivity || TraceUtility.ShouldPropagateActivity)
            {
                ResponseActivityId = ActivityIdHeader.ExtractActivityId(Request);
            }
            else
            {
                ResponseActivityId = Guid.Empty;
            }

            InvokeNotification = new MessageRpcInvokeNotification(Activity, this.channelHandler);

            if (EventTraceActivity == null && FxTrace.Trace.IsEnd2EndActivityTracingEnabled)
            {
                if (Request != null)
                {
                    EventTraceActivity = EventTraceActivityHelper.TryExtractActivity(Request, true);
                }
            }
        }

        internal bool IsPaused { get; private set; }

        internal bool SwitchedThreads { get; private set; }


        internal void Abort()
        {
            AbortRequestContext();
            AbortChannel();
            AbortInstanceContext();
        }

        private void AbortRequestContext(RequestContext requestContext)
        {
            try
            {
                requestContext.Abort();
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                channelHandler.HandleError(e);
            }
        }

        internal void AbortRequestContext()
        {
            if (OperationContext.RequestContext != null)
            {
                AbortRequestContext(OperationContext.RequestContext);
            }
            if ((RequestContext != null) && (RequestContext != OperationContext.RequestContext))
            {
                AbortRequestContext(RequestContext);
            }
            TraceCallDurationInDispatcherIfNecessary(false);
        }

        private void TraceCallDurationInDispatcherIfNecessary(bool requestContextWasClosedSuccessfully)
        {
        }

        internal void CloseRequestContext()
        {
            if (OperationContext.RequestContext != null)
            {
                DisposeRequestContext(OperationContext.RequestContext);
            }
            if ((RequestContext != null) && (RequestContext != OperationContext.RequestContext))
            {
                DisposeRequestContext(RequestContext);
            }
            TraceCallDurationInDispatcherIfNecessary(true);
        }

        private void DisposeRequestContext(RequestContext context)
        {
            try
            {
                context.Close();
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                AbortRequestContext(context);
                channelHandler.HandleError(e);
            }
        }

        internal void AbortChannel()
        {
            if ((Channel != null) && Channel.HasSession)
            {
                try
                {
                    Channel.Abort();
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    channelHandler.HandleError(e);
                }
            }
        }

        internal void CloseChannel()
        {
            if ((Channel != null) && Channel.HasSession)
            {
                try
                {
                    Channel.Close(ChannelHandler.CloseAfterFaultTimeout);
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    channelHandler.HandleError(e);
                }
            }
        }

        internal void AbortInstanceContext()
        {
            if (InstanceContext != null && !_isInstanceContextSingleton)
            {
                try
                {
                    InstanceContext.Abort();
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    channelHandler.HandleError(e);
                }
            }
        }

        internal void EnsureReceive()
        {
            using (ServiceModelActivity.BoundOperation(Activity))
            {
                ChannelHandler.Register(channelHandler);
            }
        }

        private bool ProcessError(Exception e)
        {
            MessageRpcProcessor handler = ErrorProcessor;
            try
            {
                if (TraceUtility.MessageFlowTracingOnly)
                {
                    TraceUtility.SetActivityId(Request.Properties);
                    if (Guid.Empty == DiagnosticTraceBase.ActivityId)
                    {
                        Guid receivedActivityId = TraceUtility.ExtractActivityId(Request);
                        if (Guid.Empty != receivedActivityId)
                        {
                            DiagnosticTraceBase.ActivityId = receivedActivityId;
                        }
                    }
                }


                Error = e;

                if (ErrorProcessor != null)
                {
                    ErrorProcessor(ref this);
                }

                return (Error == null);
            }
            catch (Exception e2)
            {
                if (Fx.IsFatal(e2))
                {
                    throw;
                }

                return ((handler != ErrorProcessor) && ProcessError(e2));
            }
        }

        internal void DisposeParameters(bool excludeInput)
        {
            if (Operation.DisposeParameters)
            {
                DisposeParametersCore(excludeInput);
            }
        }

        internal void DisposeParametersCore(bool excludeInput)
        {
            if (!ParametersDisposed)
            {
                if (!excludeInput)
                {
                    DisposeParameterList(InputParameters);
                }

                DisposeParameterList(OutputParameters);

                IDisposable disposableParameter = ReturnParameter as IDisposable;
                if (disposableParameter != null)
                {
                    try
                    {
                        disposableParameter.Dispose();
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                        {
                            throw;
                        }
                        channelHandler.HandleError(e);
                    }
                }
                ParametersDisposed = true;
            }
        }

        private void DisposeParameterList(object[] parameters)
        {
            IDisposable disposableParameter = null;
            if (parameters != null)
            {
                foreach (Object obj in parameters)
                {
                    disposableParameter = obj as IDisposable;
                    if (disposableParameter != null)
                    {
                        try
                        {
                            disposableParameter.Dispose();
                        }
                        catch (Exception e)
                        {
                            if (Fx.IsFatal(e))
                            {
                                throw;
                            }
                            channelHandler.HandleError(e);
                        }
                    }
                }
            }
        }

        // See notes on UnPause and Resume (mutually exclusive)
        // Pausing will Increment the BusyCount for the hosting environment
        internal IResumeMessageRpc Pause()
        {
            Wrapper wrapper = new Wrapper(ref this);
            IsPaused = true;
            return wrapper;
        }

        internal bool Process(bool isOperationContextSet)
        {
            using (ServiceModelActivity.BoundOperation(Activity))
            {
                bool completed = true;

                if (NextProcessor != null)
                {
                    MessageRpcProcessor processor = NextProcessor;
                    NextProcessor = null;

                    OperationContext originalContext;
                    if (!isOperationContextSet)
                    {
                        originalContext = OperationContext.Current;
                    }
                    else
                    {
                        originalContext = null;
                    }
                    IncrementBusyCount();

                    try
                    {
                        if (!isOperationContextSet)
                        {
                            OperationContext.Current = OperationContext;
                        }

                        processor(ref this);

                        if (!IsPaused)
                        {
                            OperationContext.SetClientReply(null, false);
                        }
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                        {
                            throw;
                        }
                        if (!ProcessError(e) && FaultInfo.Fault == null)
                        {
                            Abort();
                        }
                    }
                    finally
                    {
                        try
                        {
                            DecrementBusyCount();

                            if (!isOperationContextSet)
                            {
                                OperationContext.Current = originalContext;
                            }

                            completed = !IsPaused;
                            if (completed)
                            {
                                channelHandler.DispatchDone();
                                OperationContext.ClearClientReplyNoThrow();
                            }
                        }
                        catch (Exception e)
                        {
                            if (Fx.IsFatal(e))
                            {
                                throw;
                            }
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(e.Message, e);
                        }
                    }
                }

                return completed;
            }
        }

        // UnPause is called on the original MessageRpc to continue work on the current thread, and the copy is ignored.
        // Since the copy is ignored, Decrement the BusyCount
        internal void UnPause()
        {
            IsPaused = false;
            DecrementBusyCount();
        }

        internal bool UnlockInvokeContinueGate(out IAsyncResult result)
        {
            return _invokeContinueGate.Unlock(out result);
        }

        internal void PrepareInvokeContinueGate()
        {
            _invokeContinueGate = new SignalGate<IAsyncResult>();
        }

        private void IncrementBusyCount()
        {
        }

        private void DecrementBusyCount()
        {
        }

        private class CallbackState
        {
            public ChannelHandler ChannelHandler
            {
                get;
                set;
            }
        }

        internal class Wrapper : IResumeMessageRpc
        {
            private MessageRpc _rpc;
            private bool _alreadyResumed;

            internal Wrapper(ref MessageRpc rpc)
            {
                _rpc = rpc;
                if (rpc.NextProcessor == null)
                {
                    Fx.Assert("MessageRpc.Wrapper.Wrapper: (rpc.NextProcessor != null)");
                }
                _rpc.IncrementBusyCount();
            }

            public InstanceContext GetMessageInstanceContext()
            {
                return _rpc.InstanceContext;
            }

            // Resume is called on the copy on some completing thread, whereupon work continues on that thread.
            // BusyCount is Decremented as the copy is now complete
            public void Resume(out bool alreadyResumedNoLock)
            {
                try
                {
                    alreadyResumedNoLock = _alreadyResumed;
                    _alreadyResumed = true;

                    _rpc.SwitchedThreads = true;
                    if (_rpc.Process(false) && !_rpc.InvokeNotification.DidInvokerEnsurePump)
                    {
                        _rpc.EnsureReceive();
                    }
                }
                finally
                {
                    _rpc.DecrementBusyCount();
                }
            }

            public void Resume(IAsyncResult result)
            {
                _rpc.AsyncResult = result;
                Resume();
            }

            public void Resume(object instance)
            {
                _rpc.Instance = instance;
                Resume();
            }

            public void Resume()
            {
                using (ServiceModelActivity.BoundOperation(_rpc.Activity, true))
                {
                    bool alreadyResumedNoLock;
                    Resume(out alreadyResumedNoLock);
                    if (alreadyResumedNoLock)
                    {
                        string text = SRP.Format(SRP.SFxMultipleCallbackFromAsyncOperation,
                            String.Empty);
                        Exception error = new InvalidOperationException(text);
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(error);
                    }
                }
            }

            public void SignalConditionalResume(IAsyncResult result)
            {
                if (_rpc._invokeContinueGate.Signal(result))
                {
                    _rpc.AsyncResult = result;
                    Resume();
                }
            }
        }
    }

    internal class MessageRpcInvokeNotification : IInvokeReceivedNotification
    {
        private ServiceModelActivity _activity;
        private ChannelHandler _handler;

        public MessageRpcInvokeNotification(ServiceModelActivity activity, ChannelHandler handler)
        {
            _activity = activity;
            _handler = handler;
        }

        public bool DidInvokerEnsurePump { get; set; }

        public void NotifyInvokeReceived()
        {
            using (ServiceModelActivity.BoundOperation(_activity))
            {
                ChannelHandler.Register(_handler);
            }
            DidInvokerEnsurePump = true;
        }

        public void NotifyInvokeReceived(RequestContext request)
        {
            using (ServiceModelActivity.BoundOperation(_activity))
            {
                ChannelHandler.Register(_handler, request);
            }
            DidInvokerEnsurePump = true;
        }
    }
}
