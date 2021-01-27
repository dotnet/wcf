// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.Diagnostics;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Security;
using System.Globalization;
using System.Collections.Generic;

namespace System.ServiceModel.Diagnostics
{
    public static class TraceUtility
    {
        private const string ActivityIdKey = "ActivityId";
        private const string AsyncOperationActivityKey = "AsyncOperationActivity";
        private const string AsyncOperationStartTimeKey = "AsyncOperationStartTime";
        private static long s_messageNumber = 0;

        public const string E2EActivityId = "E2EActivityId";
        public const string TraceApplicationReference = "TraceApplicationReference";

        static internal void AddActivityHeader(Message message)
        {
            try
            {
                ActivityIdHeader activityIdHeader = new ActivityIdHeader(TraceUtility.ExtractActivityId(message));
                activityIdHeader.AddTo(message);
            }
#pragma warning disable 56500 // covered by FxCOP
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
#pragma warning disable 56500 // covered by FxCOP
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

        internal static Guid ExtractActivityId(Message message)
        {
            if (TraceUtility.MessageFlowTracingOnly)
            {
                return ActivityIdHeader.ExtractActivityId(message);
            }

            ServiceModelActivity activity = ExtractActivity(message);
            return activity == null ? Guid.Empty : activity.Id;
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

        internal static void SetActivity(Message message, ServiceModelActivity activity)
        {
            if (DiagnosticUtility.ShouldUseActivity && message != null && message.State != MessageState.Closed)
            {
                message.Properties[TraceUtility.ActivityIdKey] = activity;
            }
        }

        internal static void TraceDroppedMessage(Message message, EndpointDispatcher dispatcher)
        {
        }


        private static string GenerateMsdnTraceCode(int traceCode)
        {
            int group = (int)(traceCode & 0xFFFF0000);
            string terminatorUri = null;
            switch (group)
            {
                case TraceCode.Administration:
                    terminatorUri = "System.ServiceModel.Administration";
                    break;
                case TraceCode.Channels:
                    terminatorUri = "System.ServiceModel.Channels";
                    break;
                case TraceCode.ComIntegration:
                    terminatorUri = "System.ServiceModel.ComIntegration";
                    break;
                case TraceCode.Diagnostics:
                    terminatorUri = "System.ServiceModel.Diagnostics";
                    break;
                case TraceCode.PortSharing:
                    terminatorUri = "System.ServiceModel.PortSharing";
                    break;
                case TraceCode.Security:
                    terminatorUri = "System.ServiceModel.Security";
                    break;
                case TraceCode.Serialization:
                    terminatorUri = "System.Runtime.Serialization";
                    break;
                case TraceCode.ServiceModel:
                case TraceCode.ServiceModelTransaction:
                    terminatorUri = "System.ServiceModel";
                    break;
                default:
                    terminatorUri = string.Empty;
                    break;
            }

            return string.Empty;
        }

        internal static Exception ThrowHelperError(Exception exception, Message message)
        {
            return exception;
        }

        internal static Exception ThrowHelperError(Exception exception, Guid activityId, object source)
        {
            return exception;
        }

        internal static Exception ThrowHelperWarning(Exception exception, Message message)
        {
            return exception;
        }

        internal static ArgumentException ThrowHelperArgument(string paramName, string message, Message msg)
        {
            return (ArgumentException)TraceUtility.ThrowHelperError(new ArgumentException(message, paramName), msg);
        }

        internal static ArgumentNullException ThrowHelperArgumentNull(string paramName, Message message)
        {
            return (ArgumentNullException)TraceUtility.ThrowHelperError(new ArgumentNullException(paramName), message);
        }

        internal static string CreateSourceString(object source)
        {
            return source.GetType().ToString() + "/" + source.GetHashCode().ToString(CultureInfo.CurrentCulture);
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

        [Fx.Tag.SecurityNote(Critical = "Calls critical method DiagnosticSection.UnsafeGetSection.",
            Safe = "Doesn't leak config section instance, just reads and stores bool values.")]
        [SecuritySafeCritical]
        private static void SetEndToEndTracingFlags()
        {
        }

        static public long RetrieveMessageNumber()
        {
            return Interlocked.Increment(ref TraceUtility.s_messageNumber);
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

        static internal string GetCallerInfo(OperationContext context)
        {
            return "null";
        }

        [Fx.Tag.SecurityNote(Critical = "Calls critical method DiagnosticSection.UnsafeGetSection.",
            Safe = "Doesn't leak config section instance, just reads and stores string values for Guid")]
        [SecuritySafeCritical]
        static internal void SetEtwProviderId()
        {
        }

        static internal void SetActivityId(MessageProperties properties)
        {
            Guid activityId;
            if ((null != properties) && properties.TryGetValue(TraceUtility.E2EActivityId, out activityId))
            {
                DiagnosticTraceBase.ActivityId = activityId;
            }
        }

        static internal bool ShouldPropagateActivity
        {
            get
            {
                return false;
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

        internal static string GetAnnotation(OperationContext context)
        {
            // Desktop obtains annotation from host
            return String.Empty;
        }

        internal static void TransferFromTransport(Message message)
        {
            if (message != null && DiagnosticUtility.ShouldUseActivity)
            {
                Guid guid = Guid.Empty;

                // Only look if we are allowing user propagation
                if (TraceUtility.ShouldPropagateActivity)
                {
                    guid = ActivityIdHeader.ExtractActivityId(message);
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
                        ServiceModelActivity.Start(activity, string.Format(SRServiceModel.ActivityProcessAction, message.Headers.Action), ActivityType.ProcessAction);
                    }
                }
                message.Properties[TraceUtility.ActivityIdKey] = activity;
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
            if (OperationContext.Current != null && OperationContext.Current.OutgoingMessageProperties.TryGetValue<EventTraceActivityTimeProperty>(TraceUtility.AsyncOperationStartTimeKey, out data))
            {
                OperationContext.Current.OutgoingMessageProperties.Remove(TraceUtility.AsyncOperationStartTimeKey);
                eventTraceActivity = data.EventTraceActivity;
                startTime = data.StartTime;
            }
        }

        internal class TracingAsyncCallbackState
        {
            private object _innerState;
            private Guid _activityId;

            internal TracingAsyncCallbackState(object innerState)
            {
                _innerState = innerState;
                _activityId = DiagnosticTraceBase.ActivityId;
            }

            internal object InnerState
            {
                get { return _innerState; }
            }

            internal Guid ActivityId
            {
                get { return _activityId; }
            }
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
                    return Fx.ThunkCallback(new AsyncCallback(this.ExecuteUserCode));
                }
            }

            private void ExecuteUserCode(IAsyncResult result)
            {
                using (ServiceModelActivity activity = ServiceModelActivity.CreateBoundedActivity())
                {
                    ServiceModelActivity.Start(activity, SRServiceModel.ActivityCallback, ActivityType.ExecuteUserCode);
                    _callback(result);
                }
            }
        }


        internal class EventTraceActivityTimeProperty
        {
            private long _startTime;
            private EventTraceActivity _eventTraceActivity;

            public EventTraceActivityTimeProperty(EventTraceActivity eventTraceActivity, long startTime)
            {
                _eventTraceActivity = eventTraceActivity;
                _startTime = startTime;
            }

            internal long StartTime
            {
                get { return _startTime; }
            }
            internal EventTraceActivity EventTraceActivity
            {
                get { return _eventTraceActivity; }
            }
        }
    }
}
