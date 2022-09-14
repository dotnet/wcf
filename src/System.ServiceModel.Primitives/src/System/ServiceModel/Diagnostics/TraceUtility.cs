// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Runtime.Diagnostics;
using System.Runtime;
using System.Text;
using System.ServiceModel.Channels;
using System.Globalization;
using System.ServiceModel.Dispatcher;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace System.ServiceModel.Diagnostics
{
    internal static class TraceUtility
    {
        private static long s_messageNumber = 0;

        private const string ActivityIdKey = "ActivityId";
        private const string DiagnosticsNamespace = "http://schemas.microsoft.com/2004/09/ServiceModel/Diagnostics";

        private const string AsyncOperationActivityKey = "AsyncOperationActivity";
        private const string AsyncOperationStartTimeKey = "AsyncOperationStartTime";

        public const string E2EActivityId = "E2EActivityId";
        public const string TraceApplicationReference = "TraceApplicationReference";
        public static Func<Action<AsyncCallback, IAsyncResult>> asyncCallbackGenerator;

        static internal void AddActivityHeader(Message message)
        {
            try
            {
                ActivityIdHeader activityIdHeader = new ActivityIdHeader(TraceUtility.ExtractActivityId(message));
                activityIdHeader.AddTo(message);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
            }
        }

        static internal void AddAmbientActivityToMessage(Message message)
        {
            try
            {
                ActivityIdHeader activityIdHeader = new ActivityIdHeader(DiagnosticTraceBase.ActivityId);
                activityIdHeader.AddTo(message);
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
            }
        }

        static internal void CopyActivity(Message source, Message destination)
        {
            if (DiagnosticUtility.ShouldUseActivity)
            {
                TraceUtility.SetActivity(destination, TraceUtility.ExtractActivity(source));
            }
        }

        internal static void SetActivity(Message message, ServiceModelActivity activity)
        {
            if (DiagnosticUtility.ShouldUseActivity && message != null && message.State != MessageState.Closed)
            {
                message.Properties[TraceUtility.ActivityIdKey] = activity;
            }
        }

        internal static long GetUtcBasedDurationForTrace(long startTicks)
        {
            if (startTicks > 0)
            {
                TimeSpan elapsedTime = new TimeSpan(DateTime.UtcNow.Ticks - startTicks);
                return (long)elapsedTime.TotalMilliseconds;
            }
            return 0;
        }

        internal static ServiceModelActivity ExtractActivity(Message message)
        {
            ServiceModelActivity retval = null;

            if ((DiagnosticUtility.ShouldUseActivity || TraceUtility.ShouldPropagateActivityGlobal) &&
                (message != null) &&
                (message.State != MessageState.Closed))
            {
                object property;

                if (message.Properties.TryGetValue(TraceUtility.ActivityIdKey, out property))
                {
                    retval = property as ServiceModelActivity;
                }
            }
            return retval;
        }

        internal static string GetAnnotation(OperationContext context)
        {
            // Desktop obtains annotation from host
            return String.Empty;
        }

        internal static Guid GetReceivedActivityId(OperationContext operationContext)
        {
            object activityIdFromProprties;
            if (!operationContext.IncomingMessageProperties.TryGetValue(E2EActivityId, out activityIdFromProprties))
            {
                return TraceUtility.ExtractActivityId(operationContext.IncomingMessage);
            }
            else
            {
                return (Guid)activityIdFromProprties;
            }
        }

        internal static ServiceModelActivity ExtractAndRemoveActivity(Message message)
        {
            ServiceModelActivity retval = TraceUtility.ExtractActivity(message);
            if (retval != null)
            {
                // If the property is just removed, the item is disposed and we don't want the thing
                // to be disposed of.
                message.Properties[TraceUtility.ActivityIdKey] = false;
            }
            return retval;
        }


        static internal void MessageFlowAtMessageSent(Message message, EventTraceActivity eventTraceActivity)
        {
            if (TraceUtility.MessageFlowTracing)
            {
                Guid activityId;
                Guid correlationId;
                bool activityIdFound = ActivityIdHeader.ExtractActivityAndCorrelationId(message, out activityId, out correlationId);

                if (TraceUtility.MessageFlowTracingOnly)
                {
                    if (activityIdFound && activityId != DiagnosticTraceBase.ActivityId)
                    {
                        DiagnosticTraceBase.ActivityId = activityId;
                    }
                }

                if (WcfEventSource.Instance.MessageSentToTransportIsEnabled())
                {
                    WcfEventSource.Instance.MessageSentToTransport(eventTraceActivity, correlationId);
                }
            }
        }

        static internal void MessageFlowAtMessageReceived(Message message, OperationContext context, EventTraceActivity eventTraceActivity, bool createNewActivityId)
        {
            if (TraceUtility.MessageFlowTracing)
            {
                Guid activityId;
                Guid correlationId;
                bool activityIdFound = ActivityIdHeader.ExtractActivityAndCorrelationId(message, out activityId, out correlationId);
                if (TraceUtility.MessageFlowTracingOnly)
                {
                    if (createNewActivityId)
                    {
                        if (!activityIdFound)
                        {
                            activityId = Guid.NewGuid();
                            activityIdFound = true;
                        }
                        //message flow tracing only - start fresh
                        DiagnosticTraceBase.ActivityId = Guid.Empty;
                    }

                    if (activityIdFound)
                    {
                        FxTrace.Trace.SetAndTraceTransfer(activityId, !createNewActivityId);
                    }
                }
                if (WcfEventSource.Instance.MessageReceivedFromTransportIsEnabled())
                {
                    if (context == null)
                    {
                        context = OperationContext.Current;
                    }

                    WcfEventSource.Instance.MessageReceivedFromTransport(eventTraceActivity, correlationId, TraceUtility.GetAnnotation(context));
                }
            }
        }

        internal static void ProcessIncomingMessage(Message message, EventTraceActivity eventTraceActivity)
        {
            ServiceModelActivity activity = ServiceModelActivity.Current;
            if (activity != null && DiagnosticUtility.ShouldUseActivity)
            {
                ServiceModelActivity incomingActivity = TraceUtility.ExtractActivity(message);
                if (null != incomingActivity && incomingActivity.Id != activity.Id)
                {
                    using (ServiceModelActivity.BoundOperation(incomingActivity))
                    {
                        if (null != FxTrace.Trace)
                        {
                            FxTrace.Trace.TraceTransfer(activity.Id);
                        }
                    }
                }
                TraceUtility.SetActivity(message, activity);
            }

            TraceUtility.MessageFlowAtMessageReceived(message, null, eventTraceActivity, true);

            if (MessageLogger.LogMessagesAtServiceLevel)
            {
                MessageLogger.LogMessage(ref message, MessageLoggingSource.ServiceLevelReceiveReply | MessageLoggingSource.LastChance);
            }
        }

        internal static void ProcessOutgoingMessage(Message message, EventTraceActivity eventTraceActivity)
        {
            ServiceModelActivity activity = ServiceModelActivity.Current;
            if (DiagnosticUtility.ShouldUseActivity)
            {
                TraceUtility.SetActivity(message, activity);
            }
            if (TraceUtility.PropagateUserActivity || TraceUtility.ShouldPropagateActivity)
            {
                TraceUtility.AddAmbientActivityToMessage(message);
            }

            TraceUtility.MessageFlowAtMessageSent(message, eventTraceActivity);

            if (MessageLogger.LogMessagesAtServiceLevel)
            {
                MessageLogger.LogMessage(ref message, MessageLoggingSource.ServiceLevelSendRequest | MessageLoggingSource.LastChance);
            }
        }

        static public bool PropagateUserActivity
        {
            get
            {
                return TraceUtility.ShouldPropagateActivity &&
                    TraceUtility.PropagateUserActivityCore;
            }
        }

        // Most of the time, shouldPropagateActivity will be false.
        // This property will rarely be executed as a result.
        private static bool PropagateUserActivityCore
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            get
            {
                return false;
            }
        }

        internal static string CreateSourceString(object source)
        {
            return source.GetType().ToString() + "/" + source.GetHashCode().ToString(CultureInfo.CurrentCulture);
        }

        static internal string GetCallerInfo(OperationContext context)
        {
            return "null";
        }

        static internal void SetActivityId(MessageProperties properties)
        {
            Guid activityId;
            if ((null != properties) && properties.TryGetValue(TraceUtility.E2EActivityId, out activityId))
            {
                DiagnosticTraceBase.ActivityId = activityId;
            }
        }

        static internal void UpdateAsyncOperationContextWithActivity(object activity)
        {
            if (OperationContext.Current != null && activity != null)
            {
                OperationContext.Current.OutgoingMessageProperties[TraceUtility.AsyncOperationActivityKey] = activity;
            }
        }

        static internal object ExtractAsyncOperationContextActivity()
        {
            object data = null;
            if (OperationContext.Current != null && OperationContext.Current.OutgoingMessageProperties.TryGetValue(TraceUtility.AsyncOperationActivityKey, out data))
            {
                OperationContext.Current.OutgoingMessageProperties.Remove(TraceUtility.AsyncOperationActivityKey);
            }
            return data;
        }

        static internal void UpdateAsyncOperationContextWithStartTime(EventTraceActivity eventTraceActivity, long startTime)
        {
            if (OperationContext.Current != null)
            {
                OperationContext.Current.OutgoingMessageProperties[TraceUtility.AsyncOperationStartTimeKey] = new EventTraceActivityTimeProperty(eventTraceActivity, startTime);
            }
        }

        static internal void ExtractAsyncOperationStartTime(out EventTraceActivity eventTraceActivity, out long startTime)
        {
            EventTraceActivityTimeProperty data = null;
            eventTraceActivity = null;
            startTime = 0;
            if (OperationContext.Current != null && OperationContext.Current.OutgoingMessageProperties.TryGetValue(TraceUtility.AsyncOperationStartTimeKey, out data))
            {
                OperationContext.Current.OutgoingMessageProperties.Remove(TraceUtility.AsyncOperationStartTimeKey);
                eventTraceActivity = data.EventTraceActivity;
                startTime = data.StartTime;
            }
        }

        static internal bool ShouldPropagateActivityGlobal
        {
            get
            {
                return false;
            }
        }

        static internal bool ActivityTracing
        {
            get
            {
                return false;
            }
        }

        internal static void TraceDroppedMessage(Message message, EndpointDispatcher dispatcher)
        {
        }


        internal static Exception ThrowHelperWarning(Exception exception, Message message)
        {
            return exception;
        }

        internal static AsyncCallback WrapExecuteUserCodeAsyncCallback(AsyncCallback callback)
        {
            return (DiagnosticUtility.ShouldUseActivity && callback != null) ?
                (new ExecuteUserCodeAsync(callback)).Callback
                : callback;
        }

        internal sealed class ExecuteUserCodeAsync
        {
            private AsyncCallback _callback;

            public ExecuteUserCodeAsync(AsyncCallback callback)
            {
                _callback = callback;
            }

            public AsyncCallback Callback
            {
                get
                {
                    return Fx.ThunkCallback(new AsyncCallback(ExecuteUserCode));
                }
            }

            private void ExecuteUserCode(IAsyncResult result)
            {
                using (ServiceModelActivity activity = ServiceModelActivity.CreateBoundedActivity())
                {
                    ServiceModelActivity.Start(activity, SRP.ActivityCallback, ActivityType.ExecuteUserCode);
                    _callback(result);
                }
            }
        }

        public static InputQueue<T> CreateInputQueue<T>() where T : class
        {
            if (asyncCallbackGenerator == null)
            {
                asyncCallbackGenerator = new Func<Action<AsyncCallback, IAsyncResult>>(CallbackGenerator);
            }

            return new InputQueue<T>(asyncCallbackGenerator)
            {
                DisposeItemCallback = value =>
                {
                    if (value is ICommunicationObject)
                    {
                        ((ICommunicationObject)value).Abort();
                    }
                }
            };
        }

        internal static Action<AsyncCallback, IAsyncResult> CallbackGenerator()
        {
            if (DiagnosticUtility.ShouldUseActivity)
            {
                ServiceModelActivity callbackActivity = ServiceModelActivity.Current;
                if (callbackActivity != null)
                {
                    return delegate (AsyncCallback callback, IAsyncResult result)
                    {
                        using (ServiceModelActivity.BoundOperation(callbackActivity))
                        {
                            callback(result);
                        }
                    };
                }
            }
            return null;
        }

        internal static Guid ExtractActivityId(Message message)
        {
            Guid guid = Guid.Empty;
            try
            {
                if (message != null && message.State != MessageState.Closed && message.Headers != null)
                {
                    int index = message.Headers.FindHeader(ActivityIdKey, DiagnosticsNamespace);

                    // Check the state again, in case the message was closed after we found the header
                    if (index >= 0)
                    {
                        using (XmlDictionaryReader reader = message.Headers.GetReaderAtHeader(index))
                        {
                            guid = reader.ReadElementContentAsGuid();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (Fx.IsFatal(e))
                {
                    throw;
                }
            }

            return guid;
        }

        internal static Exception ThrowHelperError(Exception exception, Message message)
        {
            DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
            return exception;
        }

        internal static Exception ThrowHelperError(Exception exception, Guid activityId, object source)
        {
            return exception;
        }

        internal static ArgumentException ThrowHelperArgument(string paramName, string message, Message msg)
        {
            return (ArgumentException)ThrowHelperError(new ArgumentException(message, paramName), msg);
        }

        internal static ArgumentNullException ThrowHelperArgumentNull(string paramName, Message message)
        {
            return (ArgumentNullException)ThrowHelperError(new ArgumentNullException(paramName), message);
        }

        internal static void TraceHttpConnectionInformation(string localEndpoint, string remoteEndpoint, object source)
        {
        }

        internal static void TraceUserCodeException(Exception e, MethodInfo method)
        {
        }

        static TraceUtility()
        {
            //Maintain the order of calls
            TraceUtility.SetEtwProviderId();
            TraceUtility.SetEndToEndTracingFlags();
        }

        private static void SetEndToEndTracingFlags()
        {
        }

        static public long RetrieveMessageNumber()
        {
            return Interlocked.Increment(ref TraceUtility.s_messageNumber);
        }

        static internal void SetEtwProviderId()
        {
        }


        static internal bool ShouldPropagateActivity
        {
            get
            {
                return false;
            }
        }

        static internal bool MessageFlowTracing
        {
            get
            {
                return false;
            }
        }

        static internal bool MessageFlowTracingOnly
        {
            get
            {
                return false;
            }
        }

        internal static void TransferFromTransport(Message message)
        {
            if (message != null && DiagnosticUtility.ShouldUseActivity)
            {
                Guid guid = Guid.Empty;

                // Only look if we are allowing user propagation
                if (TraceUtility.ShouldPropagateActivity)
                {
                    guid = ExtractActivityId(message);
                }

                if (guid == Guid.Empty)
                {
                    guid = Guid.NewGuid();
                }

                ServiceModelActivity activity = null;
                bool emitStart = true;
                if (ServiceModelActivity.Current != null)
                {
                    if ((ServiceModelActivity.Current.Id == guid) ||
                        (ServiceModelActivity.Current.ActivityType == ActivityType.ProcessAction))
                    {
                        activity = ServiceModelActivity.Current;
                        emitStart = false;
                    }
                    else if (ServiceModelActivity.Current.PreviousActivity != null &&
                        ServiceModelActivity.Current.PreviousActivity.Id == guid)
                    {
                        activity = ServiceModelActivity.Current.PreviousActivity;
                        emitStart = false;
                    }
                }

                if (activity == null)
                {
                    activity = ServiceModelActivity.CreateActivity(guid);
                }
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    if (emitStart)
                    {
                        if (null != FxTrace.Trace)
                        {
                            FxTrace.Trace.TraceTransfer(guid);
                        }
                        ServiceModelActivity.Start(activity, SRP.Format(SRP.ActivityProcessAction, message.Headers.Action), ActivityType.ProcessAction);
                    }
                }
                message.Properties[TraceUtility.ActivityIdKey] = activity;
            }
        }

        internal static void TraceEvent(TraceEventType severity, int traceCode, string traceDescription)
        {
            TraceEvent(severity, traceCode, traceDescription, null, traceDescription, null);
        }

        internal static void TraceEvent(TraceEventType severity, int traceCode, string traceDescription, object source)
        {
            TraceEvent(severity, traceCode, traceDescription, null, source, null);
        }

        // These methods require a TraceRecord to be allocated, so we want them to show up on profiles if the caller didn't avoid
        // allocating the TraceRecord by using ShouldTrace.
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void TraceEvent(TraceEventType severity, int traceCode, string traceDescription, TraceRecord extendedData, object source, Exception exception)
        {
            if (DiagnosticUtility.ShouldTrace(severity))
            {
                //DiagnosticUtility.DiagnosticTrace.TraceEvent(severity, traceCode, GenerateMsdnTraceCode(traceCode), traceDescription, extendedData, exception, source);
            }
        }

        internal class EventTraceActivityTimeProperty
        {
            private EventTraceActivity _eventTraceActivity;

            public EventTraceActivityTimeProperty(EventTraceActivity eventTraceActivity, long startTime)
            {
                _eventTraceActivity = eventTraceActivity;
                StartTime = startTime;
            }

            internal long StartTime { get; }
            internal EventTraceActivity EventTraceActivity
            {
                get { return _eventTraceActivity; }
            }
        }
    }
}
