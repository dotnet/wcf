// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.Diagnostics;
using System.Security;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using Microsoft.Xml;

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

        private bool _paused;
        private bool _switchedThreads;
        private bool _isInstanceContextSingleton;
        private SignalGate<IAsyncResult> _invokeContinueGate;

        internal MessageRpc(RequestContext requestContext, Message request, DispatchOperationRuntime operation,
            ServiceChannel channel, ChannelHandler channelHandler, bool cleanThread,
            OperationContext operationContext, InstanceContext instanceContext, EventTraceActivity eventTraceActivity)
        {
            Fx.Assert((operationContext != null), "System.ServiceModel.Dispatcher.MessageRpc.MessageRpc(), operationContext == null");
            Fx.Assert(channelHandler != null, "System.ServiceModel.Dispatcher.MessageRpc.MessageRpc(), channelHandler == null");

            this.Activity = null;
            this.EventTraceActivity = eventTraceActivity;
            this.AsyncResult = null;
            this.CanSendReply = true;
            this.Channel = channel;
            this.channelHandler = channelHandler;
            this.Correlation = EmptyArray<object>.Allocate(operation.Parent.CorrelationCount);
            this.DidDeserializeRequestBody = false;
            this.Error = null;
            this.ErrorProcessor = null;
            this.FaultInfo = new ErrorHandlerFaultInfo(request.Version.Addressing.DefaultFaultAction);
            this.HasSecurityContext = false;
            this.Instance = null;
            this.MessageRpcOwnsInstanceContextThrottle = false;
            this.NextProcessor = null;
            this.NotUnderstoodHeaders = null;
            this.Operation = operation;
            this.OperationContext = operationContext;
            _paused = false;
            this.ParametersDisposed = false;
            this.Request = request;
            this.RequestContext = requestContext;
            this.RequestContextThrewOnReply = false;
            this.SuccessfullySendReply = false;
            this.RequestVersion = request.Version;
            this.Reply = null;
            this.ReplyTimeoutHelper = new TimeoutHelper();
            this.SecurityContext = null;
            this.InstanceContext = instanceContext;
            this.SuccessfullyBoundInstance = false;
            this.SuccessfullyIncrementedActivity = false;
            this.SuccessfullyLockedInstance = false;
            _switchedThreads = !cleanThread;
            this.InputParameters = null;
            this.OutputParameters = null;
            this.ReturnParameter = null;
            _isInstanceContextSingleton = false;
            _invokeContinueGate = null;

            if (!operation.IsOneWay && !operation.Parent.ManualAddressing)
            {
                this.RequestID = request.Headers.MessageId;
                this.ReplyToInfo = new RequestReplyCorrelator.ReplyToInfo(request);
            }
            else
            {
                this.RequestID = null;
                this.ReplyToInfo = new RequestReplyCorrelator.ReplyToInfo();
            }

            if (DiagnosticUtility.ShouldUseActivity)
            {
                this.Activity = TraceUtility.ExtractActivity(this.Request);
            }

            if (DiagnosticUtility.ShouldUseActivity || TraceUtility.ShouldPropagateActivity)
            {
                this.ResponseActivityId = ActivityIdHeader.ExtractActivityId(this.Request);
            }
            else
            {
                this.ResponseActivityId = Guid.Empty;
            }

            this.InvokeNotification = new MessageRpcInvokeNotification(this.Activity, this.channelHandler);

            if (this.EventTraceActivity == null && FxTrace.Trace.IsEnd2EndActivityTracingEnabled)
            {
                if (this.Request != null)
                {
                    this.EventTraceActivity = EventTraceActivityHelper.TryExtractActivity(this.Request, true);
                }
            }
        }

        internal bool IsPaused
        {
            get { return _paused; }
        }

        internal bool SwitchedThreads
        {
            get { return _switchedThreads; }
        }


        internal void Abort()
        {
            this.AbortRequestContext();
            this.AbortChannel();
            this.AbortInstanceContext();
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
                this.channelHandler.HandleError(e);
            }
        }

        internal void AbortRequestContext()
        {
            if (this.OperationContext.RequestContext != null)
            {
                this.AbortRequestContext(this.OperationContext.RequestContext);
            }
            if ((this.RequestContext != null) && (this.RequestContext != this.OperationContext.RequestContext))
            {
                this.AbortRequestContext(this.RequestContext);
            }
            TraceCallDurationInDispatcherIfNecessary(false);
        }

        private void TraceCallDurationInDispatcherIfNecessary(bool requestContextWasClosedSuccessfully)
        {
        }

        internal void CloseRequestContext()
        {
            if (this.OperationContext.RequestContext != null)
            {
                this.DisposeRequestContext(this.OperationContext.RequestContext);
            }
            if ((this.RequestContext != null) && (this.RequestContext != this.OperationContext.RequestContext))
            {
                this.DisposeRequestContext(this.RequestContext);
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
                this.AbortRequestContext(context);
                this.channelHandler.HandleError(e);
            }
        }

        internal void AbortChannel()
        {
            if ((this.Channel != null) && this.Channel.HasSession)
            {
                try
                {
                    this.Channel.Abort();
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    this.channelHandler.HandleError(e);
                }
            }
        }

        internal void CloseChannel()
        {
            if ((this.Channel != null) && this.Channel.HasSession)
            {
                try
                {
                    this.Channel.Close(ChannelHandler.CloseAfterFaultTimeout);
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    this.channelHandler.HandleError(e);
                }
            }
        }

        internal void AbortInstanceContext()
        {
            if (this.InstanceContext != null && !_isInstanceContextSingleton)
            {
                try
                {
                    this.InstanceContext.Abort();
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    this.channelHandler.HandleError(e);
                }
            }
        }

        internal void EnsureReceive()
        {
            using (ServiceModelActivity.BoundOperation(this.Activity))
            {
                ChannelHandler.Register(this.channelHandler);
            }
        }

        private bool ProcessError(Exception e)
        {
            MessageRpcProcessor handler = this.ErrorProcessor;
            try
            {
                if (TraceUtility.MessageFlowTracingOnly)
                {
                    TraceUtility.SetActivityId(this.Request.Properties);
                    if (Guid.Empty == DiagnosticTraceBase.ActivityId)
                    {
                        Guid receivedActivityId = TraceUtility.ExtractActivityId(this.Request);
                        if (Guid.Empty != receivedActivityId)
                        {
                            DiagnosticTraceBase.ActivityId = receivedActivityId;
                        }
                    }
                }


                this.Error = e;

                if (this.ErrorProcessor != null)
                {
                    this.ErrorProcessor(ref this);
                }

                return (this.Error == null);
            }
#pragma warning disable 56500 // covered by FxCOP
            catch (Exception e2)
            {
                if (Fx.IsFatal(e2))
                {
                    throw;
                }

                return ((handler != this.ErrorProcessor) && this.ProcessError(e2));
            }
        }

        internal void DisposeParameters(bool excludeInput)
        {
            if (this.Operation.DisposeParameters)
            {
                this.DisposeParametersCore(excludeInput);
            }
        }

        internal void DisposeParametersCore(bool excludeInput)
        {
            if (!this.ParametersDisposed)
            {
                if (!excludeInput)
                {
                    this.DisposeParameterList(this.InputParameters);
                }

                this.DisposeParameterList(this.OutputParameters);

                IDisposable disposableParameter = this.ReturnParameter as IDisposable;
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
                        this.channelHandler.HandleError(e);
                    }
                }
                this.ParametersDisposed = true;
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
                            this.channelHandler.HandleError(e);
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
            _paused = true;
            return wrapper;
        }

        [Fx.Tag.SecurityNote(Critical = "Calls SecurityCritical method ApplyHostingIntegrationContext.",
            Safe = "Does call properly and calls Dispose, doesn't leak control of the IDisposable out of the function.")]
        [SecuritySafeCritical]
        internal bool Process(bool isOperationContextSet)
        {
            using (ServiceModelActivity.BoundOperation(this.Activity))
            {
                bool completed = true;

                if (this.NextProcessor != null)
                {
                    MessageRpcProcessor processor = this.NextProcessor;
                    this.NextProcessor = null;

                    OperationContext originalContext;
                    OperationContext.Holder contextHolder;
                    if (!isOperationContextSet)
                    {
                        contextHolder = OperationContext.CurrentHolder;
                        originalContext = contextHolder.Context;
                    }
                    else
                    {
                        contextHolder = null;
                        originalContext = null;
                    }
                    IncrementBusyCount();

                    try
                    {
                        if (!isOperationContextSet)
                        {
                            contextHolder.Context = this.OperationContext;
                        }

                        processor(ref this);

                        if (!_paused)
                        {
                            this.OperationContext.SetClientReply(null, false);
                        }
                    }
#pragma warning disable 56500 // covered by FxCOP
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                        {
                            throw;
                        }
                        if (!this.ProcessError(e) && this.FaultInfo.Fault == null)
                        {
                            this.Abort();
                        }
                    }
                    finally
                    {
                        try
                        {
                            DecrementBusyCount();

                            if (!isOperationContextSet)
                            {
                                contextHolder.Context = originalContext;
                            }

                            completed = !_paused;
                            if (completed)
                            {
                                this.channelHandler.DispatchDone();
                                this.OperationContext.ClearClientReplyNoThrow();
                            }
                        }
#pragma warning disable 56500 // covered by FxCOP
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
            _paused = false;
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

                    _rpc._switchedThreads = true;
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
                this.Resume();
            }

            public void Resume(object instance)
            {
                _rpc.Instance = instance;
                this.Resume();
            }

            public void Resume()
            {
                using (ServiceModelActivity.BoundOperation(_rpc.Activity, true))
                {
                    bool alreadyResumedNoLock;
                    this.Resume(out alreadyResumedNoLock);
                    if (alreadyResumedNoLock)
                    {
                        string text = string.Format(SRServiceModel.SFxMultipleCallbackFromAsyncOperation,
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
            this.DidInvokerEnsurePump = true;
        }

        public void NotifyInvokeReceived(RequestContext request)
        {
            using (ServiceModelActivity.BoundOperation(_activity))
            {
                ChannelHandler.Register(_handler, request);
            }
            this.DidInvokerEnsurePump = true;
        }
    }
}
