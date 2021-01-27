// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Security.Principal;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel.Dispatcher
{
    internal class DispatchOperationRuntime
    {
        private static AsyncCallback s_invokeCallback = Fx.ThunkCallback(DispatchOperationRuntime.InvokeCallback);
        private readonly string _action;
        private IDispatchFaultFormatter _faultFormatter;
        private readonly IDispatchMessageFormatter _formatter;
        private IParameterInspector[] _inspectors;
        private readonly IOperationInvoker _invoker;
        private readonly bool _isSessionOpenNotificationEnabled;
        private readonly string _name;
        private readonly ImmutableDispatchRuntime _parent;
        private readonly string _replyAction;
        private readonly bool _deserializeRequest;
        private readonly bool _serializeReply;
        private readonly bool _isOneWay;
        private readonly bool _disposeParameters;

        internal DispatchOperationRuntime(DispatchOperation operation, ImmutableDispatchRuntime parent)
        {
            if (operation == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("operation");
            }
            if (parent == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("parent");
            }
            if (operation.Invoker == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.RuntimeRequiresInvoker0));
            }

            _disposeParameters = ((operation.AutoDisposeParameters) && (!operation.HasNoDisposableParameters));
            _parent = parent;
            _inspectors = EmptyArray<IParameterInspector>.ToArray(operation.ParameterInspectors);
            _faultFormatter = operation.FaultFormatter;
            _deserializeRequest = operation.DeserializeRequest;
            _serializeReply = operation.SerializeReply;
            _formatter = operation.Formatter;
            _invoker = operation.Invoker;

            _isSessionOpenNotificationEnabled = operation.IsSessionOpenNotificationEnabled;
            _action = operation.Action;
            _name = operation.Name;
            _replyAction = operation.ReplyAction;
            _isOneWay = operation.IsOneWay;

            if (_formatter == null && (_deserializeRequest || _serializeReply))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.DispatchRuntimeRequiresFormatter0, _name)));
            }
        }

        internal string Action
        {
            get { return _action; }
        }

        internal bool DisposeParameters
        {
            get { return _disposeParameters; }
        }

        internal IDispatchFaultFormatter FaultFormatter
        {
            get { return _faultFormatter; }
        }

        internal IDispatchMessageFormatter Formatter
        {
            get { return _formatter; }
        }

        internal IOperationInvoker Invoker
        {
            get { return _invoker; }
        }

        internal bool IsOneWay
        {
            get { return _isOneWay; }
        }

        internal string Name
        {
            get { return _name; }
        }

        internal IParameterInspector[] ParameterInspectors
        {
            get { return _inspectors; }
        }

        internal ImmutableDispatchRuntime Parent
        {
            get { return _parent; }
        }

        internal string ReplyAction
        {
            get { return _replyAction; }
        }

        private void DeserializeInputs(ref MessageRpc rpc)
        {
            bool success = false;
            try
            {
                rpc.InputParameters = this.Invoker.AllocateInputs();
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

                        this.Formatter.DeserializeRequest(rpc.Request, rpc.InputParameters);

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
            if (this.ParameterInspectors.Length > 0)
            {
                InspectInputsCore(ref rpc);
            }
        }

        private void InspectInputsCore(ref MessageRpc rpc)
        {
            for (int i = 0; i < this.ParameterInspectors.Length; i++)
            {
                IParameterInspector inspector = this.ParameterInspectors[i];
                rpc.Correlation[i] = inspector.BeforeCall(this.Name, rpc.InputParameters);
                if (WcfEventSource.Instance.ParameterInspectorBeforeCallInvokedIsEnabled())
                {
                    WcfEventSource.Instance.ParameterInspectorBeforeCallInvoked(rpc.EventTraceActivity, this.ParameterInspectors[i].GetType().FullName);
                }
            }
        }

        private void InspectOutputs(ref MessageRpc rpc)
        {
            if (this.ParameterInspectors.Length > 0)
            {
                InspectOutputsCore(ref rpc);
            }
        }

        private void InspectOutputsCore(ref MessageRpc rpc)
        {
            for (int i = this.ParameterInspectors.Length - 1; i >= 0; i--)
            {
                IParameterInspector inspector = this.ParameterInspectors[i];
                inspector.AfterCall(this.Name, rpc.OutputParameters, rpc.ReturnParameter, rpc.Correlation[i]);
                if (WcfEventSource.Instance.ParameterInspectorAfterCallInvokedIsEnabled())
                {
                    WcfEventSource.Instance.ParameterInspectorAfterCallInvoked(rpc.EventTraceActivity, this.ParameterInspectors[i].GetType().FullName);
                }
            }
        }

        internal void InvokeBegin(ref MessageRpc rpc)
        {
            if (rpc.Error == null)
            {
                object target = rpc.Instance;
                this.DeserializeInputs(ref rpc);
                this.InspectInputs(ref rpc);

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
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SRServiceModel.SFxInvalidAsyncResultState0);
            }

            resume.SignalConditionalResume(result);
        }


        internal void InvokeEnd(ref MessageRpc rpc)
        {
            if ((rpc.Error == null))
            {
                rpc.ReturnParameter = this.Invoker.InvokeEnd(rpc.Instance, out rpc.OutputParameters, rpc.AsyncResult);

                this.InspectOutputs(ref rpc);

                this.SerializeOutputs(ref rpc);
            }
        }

        private void SerializeOutputs(ref MessageRpc rpc)
        {
            if (!this.IsOneWay && _parent.EnableFaults)
            {
                Message reply;
                if (_serializeReply)
                {
                    if (WcfEventSource.Instance.DispatchFormatterSerializeReplyStartIsEnabled())
                    {
                        WcfEventSource.Instance.DispatchFormatterSerializeReplyStart(rpc.EventTraceActivity);
                    }

                    reply = this.Formatter.SerializeReply(rpc.RequestVersion, rpc.OutputParameters, rpc.ReturnParameter);

                    if (WcfEventSource.Instance.DispatchFormatterSerializeReplyStopIsEnabled())
                    {
                        WcfEventSource.Instance.DispatchFormatterSerializeReplyStop(rpc.EventTraceActivity);
                    }

                    if (reply == null)
                    {
                        string message = string.Format(SRServiceModel.SFxNullReplyFromFormatter2, this.Formatter.GetType().ToString(), (_name ?? ""));
                        ErrorBehavior.ThrowAndCatch(new InvalidOperationException(message));
                    }
                }
                else
                {
                    if ((rpc.ReturnParameter == null) && (rpc.OperationContext.RequestContext != null))
                    {
                        string message = string.Format(SRServiceModel.SFxDispatchRuntimeMessageCannotBeNull, _name);
                        ErrorBehavior.ThrowAndCatch(new InvalidOperationException(message));
                    }

                    reply = (Message)rpc.ReturnParameter;

                    if ((reply != null) && (!ProxyOperationRuntime.IsValidAction(reply, this.ReplyAction)))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxInvalidReplyAction, this.Name, reply.Headers.Action ?? "{NULL}", this.ReplyAction)));
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
            if (_parent.ValidateMustUnderstand)
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
