// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;

namespace System.ServiceModel.Dispatcher
{
    internal class DispatchOperationRuntime
    {
        private static AsyncCallback s_invokeCallback = Fx.ThunkCallback(DispatchOperationRuntime.InvokeCallback);
        private readonly bool _isSessionOpenNotificationEnabled;
        private readonly bool _deserializeRequest;
        private readonly bool _serializeReply;
        private readonly bool _disposeParameters;

        internal DispatchOperationRuntime(DispatchOperation operation, ImmutableDispatchRuntime parent)
        {
            if (operation == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(operation));
            }

            if (operation.Invoker == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.RuntimeRequiresInvoker0));
            }

            _disposeParameters = ((operation.AutoDisposeParameters) && (!operation.HasNoDisposableParameters));
            Parent = parent ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(parent));
            ParameterInspectors = EmptyArray<IParameterInspector>.ToArray(operation.ParameterInspectors);
            FaultFormatter = operation.FaultFormatter;
            _deserializeRequest = operation.DeserializeRequest;
            _serializeReply = operation.SerializeReply;
            Formatter = operation.Formatter;
            Invoker = operation.Invoker;
            IsTerminating = operation.IsTerminating;
            _isSessionOpenNotificationEnabled = operation.IsSessionOpenNotificationEnabled;
            Action = operation.Action;
            Name = operation.Name;
            ReplyAction = operation.ReplyAction;
            IsOneWay = operation.IsOneWay;

            if (Formatter == null && (_deserializeRequest || _serializeReply))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.DispatchRuntimeRequiresFormatter0, Name)));
            }
        }

        internal string Action { get; }

        internal bool DisposeParameters
        {
            get { return _disposeParameters; }
        }

        internal IDispatchFaultFormatter FaultFormatter { get; }

        internal IDispatchMessageFormatter Formatter { get; }

        internal IOperationInvoker Invoker { get; }

        internal bool IsOneWay { get; }

        internal bool IsTerminating { get; }

        internal string Name { get; }

        internal IParameterInspector[] ParameterInspectors { get; }

        internal ImmutableDispatchRuntime Parent { get; }

        internal string ReplyAction { get; }

        private void DeserializeInputs(ref MessageRpc rpc)
        {
            bool success = false;
            try
            {
                rpc.InputParameters = Invoker.AllocateInputs();
                // If the field is true, then this operation is to be invoked at the time the service 
                // channel is opened. The incoming message is created at ChannelHandler level with no 
                // content, so we don't need to deserialize the message.
                if (!_isSessionOpenNotificationEnabled)
                {
                    if (_deserializeRequest)
                    {
                        if (WcfEventSource.Instance.DispatchFormatterDeserializeRequestStartIsEnabled())
                        {
                            WcfEventSource.Instance.DispatchFormatterDeserializeRequestStart(rpc.EventTraceActivity);
                        }

                        Formatter.DeserializeRequest(rpc.Request, rpc.InputParameters);

                        if (WcfEventSource.Instance.DispatchFormatterDeserializeRequestStopIsEnabled())
                        {
                            WcfEventSource.Instance.DispatchFormatterDeserializeRequestStop(rpc.EventTraceActivity);
                        }
                    }
                    else
                    {
                        rpc.InputParameters[0] = rpc.Request;
                    }
                }

                success = true;
            }
            finally
            {
                rpc.DidDeserializeRequestBody = (rpc.Request.State != MessageState.Created);

                if (!success && MessageLogger.LoggingEnabled)
                {
                    MessageLogger.LogMessage(ref rpc.Request, MessageLoggingSource.Malformed);
                }
            }
        }

        private void InspectInputs(ref MessageRpc rpc)
        {
            if (ParameterInspectors.Length > 0)
            {
                InspectInputsCore(ref rpc);
            }
        }

        private void InspectInputsCore(ref MessageRpc rpc)
        {
            for (int i = 0; i < ParameterInspectors.Length; i++)
            {
                IParameterInspector inspector = ParameterInspectors[i];
                rpc.Correlation[i] = inspector.BeforeCall(Name, rpc.InputParameters);
                if (WcfEventSource.Instance.ParameterInspectorBeforeCallInvokedIsEnabled())
                {
                    WcfEventSource.Instance.ParameterInspectorBeforeCallInvoked(rpc.EventTraceActivity, ParameterInspectors[i].GetType().FullName);
                }
            }
        }

        private void InspectOutputs(ref MessageRpc rpc)
        {
            if (ParameterInspectors.Length > 0)
            {
                InspectOutputsCore(ref rpc);
            }
        }

        private void InspectOutputsCore(ref MessageRpc rpc)
        {
            for (int i = ParameterInspectors.Length - 1; i >= 0; i--)
            {
                IParameterInspector inspector = ParameterInspectors[i];
                inspector.AfterCall(Name, rpc.OutputParameters, rpc.ReturnParameter, rpc.Correlation[i]);
                if (WcfEventSource.Instance.ParameterInspectorAfterCallInvokedIsEnabled())
                {
                    WcfEventSource.Instance.ParameterInspectorAfterCallInvoked(rpc.EventTraceActivity, ParameterInspectors[i].GetType().FullName);
                }
            }
        }

        internal void InvokeBegin(ref MessageRpc rpc)
        {
            if (rpc.Error == null)
            {
                object target = rpc.Instance;
                DeserializeInputs(ref rpc);
                InspectInputs(ref rpc);

                ValidateMustUnderstand(ref rpc);

                IAsyncResult result;
                bool isBeginSuccessful = false;

                IResumeMessageRpc resumeRpc = rpc.Pause();
                try
                {
                    result = Invoker.InvokeBegin(target, rpc.InputParameters, s_invokeCallback, resumeRpc);
                    isBeginSuccessful = true;
                }
                finally
                {
                    if (!isBeginSuccessful)
                    {
                        rpc.UnPause();
                    }
                }

                if (result == null)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentNullException("IOperationInvoker.BeginDispatch"),
                        rpc.Request);
                }

                if (result.CompletedSynchronously)
                {
                    // if the async call completed synchronously, then the responsibility to call
                    // ProcessMessage{6,7,Cleanup} still remains on this thread
                    rpc.UnPause();
                    rpc.AsyncResult = result;
                }
            }
        }

        private static void InvokeCallback(IAsyncResult result)
        {
            if (result.CompletedSynchronously)
            {
                return;
            }

            IResumeMessageRpc resume = result.AsyncState as IResumeMessageRpc;

            if (resume == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRP.SFxInvalidAsyncResultState0);
            }

            resume.SignalConditionalResume(result);
        }


        internal void InvokeEnd(ref MessageRpc rpc)
        {
            if ((rpc.Error == null))
            {
                rpc.ReturnParameter = Invoker.InvokeEnd(rpc.Instance, out rpc.OutputParameters, rpc.AsyncResult);

                InspectOutputs(ref rpc);

                SerializeOutputs(ref rpc);
            }
        }

        private void SerializeOutputs(ref MessageRpc rpc)
        {
            if (!IsOneWay && Parent.EnableFaults)
            {
                Message reply;
                if (_serializeReply)
                {
                    if (WcfEventSource.Instance.DispatchFormatterSerializeReplyStartIsEnabled())
                    {
                        WcfEventSource.Instance.DispatchFormatterSerializeReplyStart(rpc.EventTraceActivity);
                    }

                    reply = Formatter.SerializeReply(rpc.RequestVersion, rpc.OutputParameters, rpc.ReturnParameter);

                    if (WcfEventSource.Instance.DispatchFormatterSerializeReplyStopIsEnabled())
                    {
                        WcfEventSource.Instance.DispatchFormatterSerializeReplyStop(rpc.EventTraceActivity);
                    }

                    if (reply == null)
                    {
                        string message = SRP.Format(SRP.SFxNullReplyFromFormatter2, Formatter.GetType().ToString(), (Name ?? ""));
                        ErrorBehavior.ThrowAndCatch(new InvalidOperationException(message));
                    }
                }
                else
                {
                    if ((rpc.ReturnParameter == null) && (rpc.OperationContext.RequestContext != null))
                    {
                        string message = SRP.Format(SRP.SFxDispatchRuntimeMessageCannotBeNull, Name);
                        ErrorBehavior.ThrowAndCatch(new InvalidOperationException(message));
                    }

                    reply = (Message)rpc.ReturnParameter;

                    if ((reply != null) && (!ProxyOperationRuntime.IsValidAction(reply, ReplyAction)))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SFxInvalidReplyAction, Name, reply.Headers.Action ?? "{NULL}", ReplyAction)));
                    }
                }

                if (DiagnosticUtility.ShouldUseActivity && rpc.Activity != null && reply != null)
                {
                    TraceUtility.SetActivity(reply, rpc.Activity);
                    if (TraceUtility.ShouldPropagateActivity)
                    {
                        TraceUtility.AddActivityHeader(reply);
                    }
                }
                else if (TraceUtility.ShouldPropagateActivity && reply != null && rpc.ResponseActivityId != Guid.Empty)
                {
                    ActivityIdHeader header = new ActivityIdHeader(rpc.ResponseActivityId);
                    header.AddTo(reply);
                }

                //rely on the property set during the message receive to correlate the trace
                if (TraceUtility.MessageFlowTracingOnly)
                {
                    //Guard against MEX scenarios where the message is closed by now
                    if (null != rpc.OperationContext.IncomingMessage && MessageState.Closed != rpc.OperationContext.IncomingMessage.State)
                    {
                        FxTrace.Trace.SetAndTraceTransfer(TraceUtility.GetReceivedActivityId(rpc.OperationContext), true);
                    }
                    else
                    {
                        if (rpc.ResponseActivityId != Guid.Empty)
                        {
                            FxTrace.Trace.SetAndTraceTransfer(rpc.ResponseActivityId, true);
                        }
                    }
                }

                if (MessageLogger.LoggingEnabled && null != reply)
                {
                    MessageLogger.LogMessage(ref reply, MessageLoggingSource.ServiceLevelSendReply | MessageLoggingSource.LastChance);
                }
                rpc.Reply = reply;
            }
        }


        private void ValidateMustUnderstand(ref MessageRpc rpc)
        {
            if (Parent.ValidateMustUnderstand)
            {
                rpc.NotUnderstoodHeaders = rpc.Request.Headers.GetHeadersNotUnderstood();
                if (rpc.NotUnderstoodHeaders != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new MustUnderstandSoapException(rpc.NotUnderstoodHeaders, rpc.Request.Version.Envelope));
                }
            }
        }
    }
}
