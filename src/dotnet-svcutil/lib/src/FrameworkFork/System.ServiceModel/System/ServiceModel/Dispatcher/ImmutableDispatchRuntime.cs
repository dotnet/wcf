// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.ServiceModel.Channels;
using System.ServiceModel.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace System.ServiceModel.Dispatcher
{
    internal class ImmutableDispatchRuntime
    {
        readonly private int _correlationCount;
        readonly private ConcurrencyBehavior _concurrency;
        readonly private IDemuxer _demuxer;
        readonly private ErrorBehavior _error;
        private readonly bool _enableFaults;
        private InstanceBehavior _instance;
        private readonly bool _manualAddressing;
        private readonly ThreadBehavior _thread;
        private readonly bool _validateMustUnderstand;
        private readonly bool _sendAsynchronously;

        private readonly MessageRpcProcessor _processMessage1;
        private readonly MessageRpcProcessor _processMessage11;
        private readonly MessageRpcProcessor _processMessage2;
        private readonly MessageRpcProcessor _processMessage3;
        private readonly MessageRpcProcessor _processMessage31;
        private readonly MessageRpcProcessor _processMessage4;
        private readonly MessageRpcProcessor _processMessage41;
        private readonly MessageRpcProcessor _processMessage5;
        private readonly MessageRpcProcessor _processMessage6;
        private readonly MessageRpcProcessor _processMessage7;
        private readonly MessageRpcProcessor _processMessage8;
        private readonly MessageRpcProcessor _processMessage9;
        private readonly MessageRpcProcessor _processMessageCleanup;
        private readonly MessageRpcProcessor _processMessageCleanupError;

        private static AsyncCallback s_onReplyCompleted = Fx.ThunkCallback(new AsyncCallback(OnReplyCompletedCallback));

        internal ImmutableDispatchRuntime(DispatchRuntime dispatch)
        {
            _concurrency = new ConcurrencyBehavior(dispatch);
            _error = new ErrorBehavior(dispatch.ChannelDispatcher);
            _enableFaults = dispatch.EnableFaults;
            _instance = new InstanceBehavior(dispatch, this);
            _manualAddressing = dispatch.ManualAddressing;
            _thread = new ThreadBehavior(dispatch);
            _sendAsynchronously = dispatch.ChannelDispatcher.SendAsynchronously;
            _correlationCount = dispatch.MaxParameterInspectors;

            DispatchOperationRuntime unhandled = new DispatchOperationRuntime(dispatch.UnhandledDispatchOperation, this);

            ActionDemuxer demuxer = new ActionDemuxer();
            for (int i = 0; i < dispatch.Operations.Count; i++)
            {
                DispatchOperation operation = dispatch.Operations[i];
                DispatchOperationRuntime operationRuntime = new DispatchOperationRuntime(operation, this);
                demuxer.Add(operation.Action, operationRuntime);
            }

            demuxer.SetUnhandled(unhandled);
            _demuxer = demuxer;

            _processMessage1 = ProcessMessage1;
            _processMessage11 = ProcessMessage11;
            _processMessage2 = ProcessMessage2;
            _processMessage3 = ProcessMessage3;
            _processMessage31 = ProcessMessage31;
            _processMessage4 = ProcessMessage4;
            _processMessage41 = ProcessMessage41;
            _processMessage5 = ProcessMessage5;
            _processMessage6 = ProcessMessage6;
            _processMessage7 = ProcessMessage7;
            _processMessage8 = ProcessMessage8;
            _processMessage9 = ProcessMessage9;
            _processMessageCleanup = ProcessMessageCleanup;
            _processMessageCleanupError = ProcessMessageCleanupError;
        }

        internal int CorrelationCount
        {
            get { return _correlationCount; }
        }

        internal bool EnableFaults
        {
            get { return _enableFaults; }
        }

        internal bool ManualAddressing
        {
            get { return _manualAddressing; }
        }

        internal bool ValidateMustUnderstand
        {
            get { return _validateMustUnderstand; }
        }

        internal void AfterReceiveRequest(ref MessageRpc rpc)
        {
            // IDispatchMessageInspector would normally be called here. That interface isn't in contract.
        }

        private void Reply(ref MessageRpc rpc)
        {
            rpc.RequestContextThrewOnReply = true;
            rpc.SuccessfullySendReply = false;

            try
            {
                rpc.RequestContext.Reply(rpc.Reply, rpc.ReplyTimeoutHelper.RemainingTime());
                rpc.RequestContextThrewOnReply = false;
                rpc.SuccessfullySendReply = true;

                if (WcfEventSource.Instance.DispatchMessageStopIsEnabled())
                {
                    WcfEventSource.Instance.DispatchMessageStop(rpc.EventTraceActivity);
                }
            }
            catch (CommunicationException e)
            {
                _error.HandleError(e);
            }
            catch (TimeoutException e)
            {
                _error.HandleError(e);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                if (!_error.HandleError(e))
                {
                    rpc.RequestContextThrewOnReply = true;
                    rpc.CanSendReply = false;
                }
            }
        }

        private void BeginReply(ref MessageRpc rpc)
        {
            bool success = false;

            try
            {
                IResumeMessageRpc resume = rpc.Pause();

                rpc.AsyncResult = rpc.RequestContext.BeginReply(rpc.Reply, rpc.ReplyTimeoutHelper.RemainingTime(),
                    s_onReplyCompleted, resume);
                success = true;

                if (rpc.AsyncResult.CompletedSynchronously)
                {
                    rpc.UnPause();
                }
            }
            catch (CommunicationException e)
            {
                _error.HandleError(e);
            }
            catch (TimeoutException e)
            {
                _error.HandleError(e);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                if (!_error.HandleError(e))
                {
                    rpc.RequestContextThrewOnReply = true;
                    rpc.CanSendReply = false;
                }
            }
            finally
            {
                if (!success)
                {
                    rpc.UnPause();
                }
            }
        }

        internal bool Dispatch(ref MessageRpc rpc, bool isOperationContextSet)
        {
            rpc.ErrorProcessor = _processMessage8;
            rpc.NextProcessor = _processMessage1;
            return rpc.Process(isOperationContextSet);
        }

        private bool EndReply(ref MessageRpc rpc)
        {
            bool success = false;

            try
            {
                rpc.RequestContext.EndReply(rpc.AsyncResult);
                rpc.RequestContextThrewOnReply = false;
                success = true;

                if (WcfEventSource.Instance.DispatchMessageStopIsEnabled())
                {
                    WcfEventSource.Instance.DispatchMessageStop(rpc.EventTraceActivity);
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                _error.HandleError(e);
            }

            return success;
        }

        private void SetActivityIdOnThread(ref MessageRpc rpc)
        {
            if (FxTrace.Trace.IsEnd2EndActivityTracingEnabled && rpc.EventTraceActivity != null)
            {
                // Propogate the ActivityId to the service operation
                EventTraceActivityHelper.SetOnThread(rpc.EventTraceActivity);
            }
        }

        private void TransferChannelFromPendingList(ref MessageRpc rpc)
        {
            if (rpc.Channel.IsPending)
            {
                rpc.Channel.IsPending = false;

                ChannelDispatcher channelDispatcher = rpc.Channel.ChannelDispatcher;
                IInstanceContextProvider provider = _instance.InstanceContextProvider;

                if (!InstanceContextProviderBase.IsProviderSessionful(provider) &&
                    !InstanceContextProviderBase.IsProviderSingleton(provider))
                {
                    IChannel proxy = rpc.Channel.Proxy as IChannel;
                    if (!rpc.InstanceContext.IncomingChannels.Contains(proxy))
                    {
                        channelDispatcher.Channels.Add(proxy);
                    }
                }

                channelDispatcher.PendingChannels.Remove(rpc.Channel.Binder.Channel);
            }
        }

        private void AddMessageProperties(Message message, OperationContext context, ServiceChannel replyChannel)
        {
            if (context.InternalServiceChannel == replyChannel)
            {
                if (context.HasOutgoingMessageHeaders)
                {
                    message.Headers.CopyHeadersFrom(context.OutgoingMessageHeaders);
                }

                if (context.HasOutgoingMessageProperties)
                {
                    message.Properties.MergeProperties(context.OutgoingMessageProperties);
                }
            }
        }

        private static void OnReplyCompletedCallback(IAsyncResult result)
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

            resume.Resume(result);
        }

        private void PrepareReply(ref MessageRpc rpc)
        {
            RequestContext context = rpc.OperationContext.RequestContext;
            Exception exception = null;
            bool thereIsAnUnhandledException = false;

            if (!rpc.Operation.IsOneWay)
            {
                if ((context != null) && (rpc.Reply != null))
                {
                    try
                    {
                        rpc.CanSendReply = PrepareAndAddressReply(ref rpc);
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                        {
                            throw;
                        }
                        thereIsAnUnhandledException = !_error.HandleError(e);
                        exception = e;
                    }
                }
            }

            if (rpc.Operation.IsOneWay)
            {
                rpc.CanSendReply = false;
            }

            if (!rpc.Operation.IsOneWay && (context != null) && (rpc.Reply != null))
            {
                if (exception != null)
                {
                    // We don't call ProvideFault again, since we have already passed the
                    // point where SFx addresses the reply, and it is reasonable for
                    // ProvideFault to expect that SFx will address the reply.  Instead
                    // we always just do 'internal server error' processing.
                    rpc.Error = exception;
                    _error.ProvideOnlyFaultOfLastResort(ref rpc);

                    try
                    {
                        rpc.CanSendReply = PrepareAndAddressReply(ref rpc);
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                        {
                            throw;
                        }
                        _error.HandleError(e);
                    }
                }
            }
            else if ((exception != null) && thereIsAnUnhandledException)
            {
                rpc.Abort();
            }
        }

        private bool PrepareAndAddressReply(ref MessageRpc rpc)
        {
            bool canSendReply = true;

            if (!_manualAddressing)
            {
                if (!object.ReferenceEquals(rpc.RequestID, null))
                {
                    System.ServiceModel.Channels.RequestReplyCorrelator.PrepareReply(rpc.Reply, rpc.RequestID);
                }

                if (!rpc.Channel.HasSession)
                {
                    canSendReply = System.ServiceModel.Channels.RequestReplyCorrelator.AddressReply(rpc.Reply, rpc.ReplyToInfo);
                }
            }

            AddMessageProperties(rpc.Reply, rpc.OperationContext, rpc.Channel);
            if (FxTrace.Trace.IsEnd2EndActivityTracingEnabled && rpc.EventTraceActivity != null)
            {
                rpc.Reply.Properties[EventTraceActivity.Name] = rpc.EventTraceActivity;
            }

            return canSendReply;
        }

        internal DispatchOperationRuntime GetOperation(ref Message message)
        {
            return _demuxer.GetOperation(ref message);
        }

        private interface IDemuxer
        {
            DispatchOperationRuntime GetOperation(ref Message request);
        }

        internal bool IsConcurrent(ref MessageRpc rpc)
        {
            return _concurrency.IsConcurrent(ref rpc);
        }

        internal void ProcessMessage1(ref MessageRpc rpc)
        {
            rpc.NextProcessor = _processMessage11;

            if (!rpc.IsPaused)
            {
                ProcessMessage11(ref rpc);
            }
        }

        internal void ProcessMessage11(ref MessageRpc rpc)
        {
            rpc.NextProcessor = _processMessage2;

            if (rpc.Operation.IsOneWay)
            {
                rpc.RequestContext.Reply(null);
                rpc.OperationContext.RequestContext = null;
            }
            else
            {
                if (!rpc.Channel.IsReplyChannel &&
                    ((object)rpc.RequestID == null) &&
                    (rpc.Operation.Action != MessageHeaders.WildcardAction))
                {
                    CommunicationException error = new CommunicationException(SRServiceModel.SFxOneWayMessageToTwoWayMethod0);
                    throw TraceUtility.ThrowHelperError(error, rpc.Request);
                }

                if (!_manualAddressing)
                {
                    EndpointAddress replyTo = rpc.ReplyToInfo.ReplyTo;
                    if (replyTo != null && replyTo.IsNone && rpc.Channel.IsReplyChannel)
                    {
                        CommunicationException error = new CommunicationException(SRServiceModel.SFxRequestReplyNone);
                        throw TraceUtility.ThrowHelperError(error, rpc.Request);
                    }
                }
            }

            if (_concurrency.IsConcurrent(ref rpc))
            {
                rpc.Channel.IncrementActivity();
                rpc.SuccessfullyIncrementedActivity = true;
            }

            _instance.EnsureInstanceContext(ref rpc);
            this.TransferChannelFromPendingList(ref rpc);

            if (!rpc.IsPaused)
            {
                this.ProcessMessage2(ref rpc);
            }
        }

        private void ProcessMessage2(ref MessageRpc rpc)
        {
            // Run dispatch message inspectors
            rpc.NextProcessor = _processMessage3;

            this.AfterReceiveRequest(ref rpc);

            _concurrency.LockInstance(ref rpc);

            if (!rpc.IsPaused)
            {
                this.ProcessMessage3(ref rpc);
            }
        }


        private void ProcessMessage3(ref MessageRpc rpc)
        {
            // Manage transactions, can likely go away

            rpc.NextProcessor = _processMessage31;

            rpc.SuccessfullyLockedInstance = true;

            if (!rpc.IsPaused)
            {
                this.ProcessMessage31(ref rpc);
            }
        }

        private void ProcessMessage31(ref MessageRpc rpc)
        {
            // More transaction stuff, can likely go away
            rpc.NextProcessor = _processMessage4;

            if (!rpc.IsPaused)
            {
                this.ProcessMessage4(ref rpc);
            }
        }

        private void ProcessMessage4(ref MessageRpc rpc)
        {
            // Bind request to synchronization context if needed

            rpc.NextProcessor = _processMessage41;

            try
            {
                _thread.BindThread(ref rpc);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(e.Message, e);
            }

            if (!rpc.IsPaused)
            {
                this.ProcessMessage41(ref rpc);
            }
        }


        private void ProcessMessage41(ref MessageRpc rpc)
        {
            rpc.NextProcessor = _processMessage5;

            // This needs to happen after LockInstance--LockInstance guarantees
            // in-order delivery, so we can't receive the next message until we
            // have acquired the lock.
            //
            // This also needs to happen after BindThread, since
            // running on UI thread should guarantee in-order delivery if
            // the SynchronizationContext is single threaded.
            if (_concurrency.IsConcurrent(ref rpc))
            {
                rpc.EnsureReceive();
            }

            _instance.EnsureServiceInstance(ref rpc);

            if (!rpc.IsPaused)
            {
                this.ProcessMessage5(ref rpc);
            }
        }

        private void ProcessMessage5(ref MessageRpc rpc)
        {
            rpc.NextProcessor = _processMessage6;

            bool success = false;
            try
            {
                // If async call completes in sync, it tells us through the gate below
                rpc.PrepareInvokeContinueGate();

                SetActivityIdOnThread(ref rpc);

                rpc.Operation.InvokeBegin(ref rpc);
                success = true;
            }
            finally
            {
                try
                {
                    if (rpc.IsPaused)
                    {
                        // Check if the callback produced the async result and set it back on the RPC on this stack 
                        // and proceed only if the gate was signaled by the callback and completed synchronously
                        if (rpc.UnlockInvokeContinueGate(out rpc.AsyncResult))
                        {
                            rpc.UnPause();
                        }
                    }
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }

                    if (success && !rpc.IsPaused)
                    {
                        throw;
                    }

                    _error.HandleError(e);
                }
            }

            // Proceed if rpc is unpaused and invoke begin was successful.
            if (!rpc.IsPaused)
            {
                this.ProcessMessage6(ref rpc);
            }
        }

        private void ProcessMessage6(ref MessageRpc rpc)
        {
            rpc.NextProcessor = _processMessage7;

            try
            {
                _thread.BindEndThread(ref rpc);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(e.Message, e);
            }

            if (!rpc.IsPaused)
            {
                this.ProcessMessage7(ref rpc);
            }
        }

        private void ProcessMessage7(ref MessageRpc rpc)
        {
            rpc.NextProcessor = null;

            rpc.Operation.InvokeEnd(ref rpc);

            // this never pauses
            this.ProcessMessage8(ref rpc);
        }

        private void ProcessMessage8(ref MessageRpc rpc)
        {
            rpc.NextProcessor = _processMessage9;

            try
            {
                _error.ProvideMessageFault(ref rpc);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }

                _error.HandleError(e);
            }

            this.PrepareReply(ref rpc);

            if (rpc.CanSendReply)
            {
                rpc.ReplyTimeoutHelper = new TimeoutHelper(rpc.Channel.OperationTimeout);
            }

            if (!rpc.IsPaused)
            {
                this.ProcessMessage9(ref rpc);
            }
        }

        private void ProcessMessage9(ref MessageRpc rpc)
        {
            rpc.NextProcessor = _processMessageCleanup;

            if (rpc.CanSendReply)
            {
                if (rpc.Reply != null)
                {
                    TraceUtility.MessageFlowAtMessageSent(rpc.Reply, rpc.EventTraceActivity);
                }

                if (_sendAsynchronously)
                {
                    this.BeginReply(ref rpc);
                }
                else
                {
                    this.Reply(ref rpc);
                }
            }

            if (!rpc.IsPaused)
            {
                this.ProcessMessageCleanup(ref rpc);
            }
        }

        private void ProcessMessageCleanup(ref MessageRpc rpc)
        {
            Fx.Assert(
                !object.ReferenceEquals(rpc.ErrorProcessor, _processMessageCleanupError),
                "ProcessMessageCleanup run twice on the same MessageRpc!");
            rpc.ErrorProcessor = _processMessageCleanupError;

            bool replyWasSent = false;

            if (rpc.CanSendReply)
            {
                if (_sendAsynchronously)
                {
                    replyWasSent = this.EndReply(ref rpc);
                }
                else
                {
                    replyWasSent = rpc.SuccessfullySendReply;
                }
            }

            try
            {
                try
                {
                    if (rpc.DidDeserializeRequestBody)
                    {
                        rpc.Request.Close();
                    }
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    _error.HandleError(e);
                }

                rpc.DisposeParameters(false); //Dispose all input/output/return parameters

                if (rpc.FaultInfo.IsConsideredUnhandled)
                {
                    if (!replyWasSent)
                    {
                        rpc.AbortRequestContext();
                        rpc.AbortChannel();
                    }
                    else
                    {
                        rpc.CloseRequestContext();
                        rpc.CloseChannel();
                    }
                    rpc.AbortInstanceContext();
                }
                else
                {
                    if (rpc.RequestContextThrewOnReply)
                    {
                        rpc.AbortRequestContext();
                    }
                    else
                    {
                        rpc.CloseRequestContext();
                    }
                }


                if ((rpc.Reply != null) && (rpc.Reply != rpc.ReturnParameter))
                {
                    try
                    {
                        rpc.Reply.Close();
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                        {
                            throw;
                        }
                        _error.HandleError(e);
                    }
                }

                if ((rpc.FaultInfo.Fault != null) && (rpc.FaultInfo.Fault.State != MessageState.Closed))
                {
                    // maybe ProvideFault gave a Message, but then BeforeSendReply replaced it
                    // in that case, we need to close the one from ProvideFault
                    try
                    {
                        rpc.FaultInfo.Fault.Close();
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                        {
                            throw;
                        }
                        _error.HandleError(e);
                    }
                }

                try
                {
                    rpc.OperationContext.FireOperationCompleted();
                }
                catch (Exception e)
                {
                    if (Fx.IsFatal(e))
                    {
                        throw;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(e);
                }

                _instance.AfterReply(ref rpc, _error);

                if (rpc.SuccessfullyLockedInstance)
                {
                    try
                    {
                        _concurrency.UnlockInstance(ref rpc);
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                        {
                            throw;
                        }

                        Fx.Assert("Exceptions should be caught by callee");
                        rpc.InstanceContext.FaultInternal();
                        _error.HandleError(e);
                    }
                }


                if (rpc.SuccessfullyIncrementedActivity)
                {
                    try
                    {
                        rpc.Channel.DecrementActivity();
                    }
                    catch (Exception e)
                    {
                        if (Fx.IsFatal(e))
                        {
                            throw;
                        }
                        _error.HandleError(e);
                    }
                }
            }
            finally
            {
                if (rpc.Activity != null && DiagnosticUtility.ShouldUseActivity)
                {
                    rpc.Activity.Stop();
                }
            }

            _error.HandleError(ref rpc);
        }

        private void ProcessMessageCleanupError(ref MessageRpc rpc)
        {
            _error.HandleError(ref rpc);
        }

        private class ActionDemuxer : IDemuxer
        {
            private readonly HybridDictionary _map;
            private DispatchOperationRuntime _unhandled;

            internal ActionDemuxer()
            {
                _map = new HybridDictionary();
            }

            internal void Add(string action, DispatchOperationRuntime operation)
            {
                if (_map.Contains(action))
                {
                    DispatchOperationRuntime existingOperation = (DispatchOperationRuntime)_map[action];
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxActionDemuxerDuplicate, existingOperation.Name, operation.Name, action)));
                }
                _map.Add(action, operation);
            }

            internal void SetUnhandled(DispatchOperationRuntime operation)
            {
                _unhandled = operation;
            }

            public DispatchOperationRuntime GetOperation(ref Message request)
            {
                string action = request.Headers.Action;
                if (action == null)
                {
                    action = MessageHeaders.WildcardAction;
                }
                DispatchOperationRuntime operation = (DispatchOperationRuntime)_map[action];
                if (operation != null)
                {
                    return operation;
                }

                return _unhandled;
            }
        }
    }
}
