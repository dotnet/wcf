// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime
{
    internal abstract class ActionItem
    {
        private bool _isScheduled;

        protected ActionItem()
        {
        }

        public static void Schedule(Action<object> callback, object state)
        {
            Fx.Assert(callback != null, "A null callback was passed for Schedule!");
            if (WaitCallbackActionItem.ShouldUseActivity ||
                Fx.Trace.IsEnd2EndActivityTracingEnabled)
            {
                new DefaultActionItem(callback, state).Schedule();
            }
            else
            {
                ScheduleCallback(callback, state);
            }
        }

        public static void Schedule(Func<object, Task> callback, object state)
        {
            Fx.Assert(callback != null, "A null callback was passed for Schedule!");
            if (WaitCallbackActionItem.ShouldUseActivity ||
    Fx.Trace.IsEnd2EndActivityTracingEnabled)
            {
                new DefaultActionItem(callback, state).ScheduleAsync();
            }
            else
            {
                ScheduleCallback(callback, state);
            }
        }

        protected abstract void Invoke();

        protected abstract Task InvokeAsync();

        protected void Schedule()
        {
            if (_isScheduled)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(InternalSR.ActionItemIsAlreadyScheduled));
            }

            _isScheduled = true;
            ScheduleCallback(CallbackHelper.InvokeCallbackAction);
        }

        protected void ScheduleAsync()
        {
            if (_isScheduled)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(InternalSR.ActionItemIsAlreadyScheduled));
            }

            _isScheduled = true;
            ScheduleCallback(CallbackHelper.InvokeAsyncCallbackFunc);
        }

        private static void ScheduleCallback(Action<object> callback, object state)
        {
            Fx.Assert(callback != null, "Cannot schedule a null callback");
            IOThreadScheduler.ScheduleCallbackNoFlow(callback, state);
        }

        private static void ScheduleCallback(Func<object, Task> callback, object state)
        {
            Fx.Assert(callback != null, "Cannot schedule a null callback");
            // The trick here is using CallbackHelper.IOTaskSchedule as the TaskScheduler. This is a special TaskScheduler created from a sync context
            // which posts action's to the IOThreadScheduler. So instead of directly posting a Task to the IOThreadScheduler, we let the TaskScheduler
            // break up the Task into individual Action<object> delegates and post them for us.
            Task<Task>.Factory.StartNew(callback, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, IOThreadScheduler.IOTaskScheduler);
        }

        private void ScheduleCallback(Action<object> callback)
        {
            ScheduleCallback(callback, this);
        }

        private void ScheduleCallback(Func<object, Task> callback)
        {
            ScheduleCallback(callback, this);
        }

        internal static class CallbackHelper
        {
            private static Action<object> s_invokeCallback;
            private static Func<object, Task> s_invokeAsyncCallback;

            public static Action<object> InvokeCallbackAction
            {
                get
                {
                    if (s_invokeCallback == null)
                    {
                        s_invokeCallback = new Action<object>(InvokeCallback);
                    }
                    return s_invokeCallback;
                }
            }

            private static void InvokeCallback(object state)
            {
                ((ActionItem)state).Invoke();
                ((ActionItem)state)._isScheduled = false;
            }

            public static Func<object, Task> InvokeAsyncCallbackFunc
            {
                get
                {
                    if (s_invokeAsyncCallback == null)
                    {
                        s_invokeAsyncCallback = new Func<object, Task>(InvokeAsyncCallback);
                    }
                    return s_invokeAsyncCallback;
                }
            }

            private static async Task InvokeAsyncCallback(object state)
            {
                await ((ActionItem)state).InvokeAsync();
                ((ActionItem)state)._isScheduled = false;
            }
        }

        internal class DefaultActionItem : ActionItem
        {
            private Action<object> _callback;
            private object _state;

            private bool _flowLegacyActivityId;
            private Guid _activityId;
            private EventTraceActivity _eventTraceActivity;
            private Func<object, Task> _asyncCallback;

            public DefaultActionItem(Action<object> callback, object state)
            {
                Fx.Assert(callback != null, "Shouldn't instantiate an object to wrap a null callback");
                _callback = callback;
                _state = state;
                if (WaitCallbackActionItem.ShouldUseActivity)
                {
                    _flowLegacyActivityId = true;
                    _activityId = EtwDiagnosticTrace.ActivityId;
                }
                if (Fx.Trace.IsEnd2EndActivityTracingEnabled)
                {
                    _eventTraceActivity = EventTraceActivity.GetFromThreadOrCreate();
                    if (TraceCore.ActionItemScheduledIsEnabled(Fx.Trace))
                    {
                        TraceCore.ActionItemScheduled(Fx.Trace, _eventTraceActivity);
                    }
                }
            }

            public DefaultActionItem(Func<object, Task> callback, object state)
            {
                Fx.Assert(callback != null, "Shouldn't instantiate an object to wrap a null callback");
                _asyncCallback = callback;
                _state = state;
                if (WaitCallbackActionItem.ShouldUseActivity)
                {
                    _flowLegacyActivityId = true;
                    _activityId = EtwDiagnosticTrace.ActivityId;
                }
                if (Fx.Trace.IsEnd2EndActivityTracingEnabled)
                {
                    _eventTraceActivity = EventTraceActivity.GetFromThreadOrCreate();
                    if (TraceCore.ActionItemScheduledIsEnabled(Fx.Trace))
                    {
                        TraceCore.ActionItemScheduled(Fx.Trace, _eventTraceActivity);
                    }
                }
            }

            protected override void Invoke()
            {
                Fx.Assert(_asyncCallback == null, "Can't call Invoke on async ActionItem");
                if (_flowLegacyActivityId || Fx.Trace.IsEnd2EndActivityTracingEnabled)
                {
                    TraceAndInvoke();
                }
                else
                {
                    _callback(_state);
                }
            }

            protected override Task InvokeAsync()
            {
                Fx.Assert(_callback == null, "Can't call InvokeAsync on sync ActionItem");
                if (_flowLegacyActivityId || Fx.Trace.IsEnd2EndActivityTracingEnabled)
                {
                    return TraceAndInvokeAsync();
                }
                else
                {
                    return _asyncCallback(_state);
                }
            }

            private void TraceAndInvoke()
            {
                if (_flowLegacyActivityId)
                {
                    Guid currentActivityId = EtwDiagnosticTrace.ActivityId;
                    try
                    {
                        EtwDiagnosticTrace.ActivityId = _activityId;
                        _callback(_state);
                    }
                    finally
                    {
                        EtwDiagnosticTrace.ActivityId = currentActivityId;
                    }
                }
                else
                {
                    Guid previous = Guid.Empty;
                    bool restoreActivityId = false;
                    try
                    {
                        if (_eventTraceActivity != null)
                        {
                            previous = Trace.CorrelationManager.ActivityId;
                            restoreActivityId = true;
                            Trace.CorrelationManager.ActivityId = _eventTraceActivity.ActivityId;
                            if (TraceCore.ActionItemCallbackInvokedIsEnabled(Fx.Trace))
                            {
                                TraceCore.ActionItemCallbackInvoked(Fx.Trace, _eventTraceActivity);
                            }
                        }

                        _callback(_state);
                    }
                    finally
                    {
                        if (restoreActivityId)
                        {
                            Trace.CorrelationManager.ActivityId = previous;
                        }
                    }
                }
            }

            private async Task TraceAndInvokeAsync()
            {
                if (_flowLegacyActivityId)
                {
                    Guid currentActivityId = EtwDiagnosticTrace.ActivityId;
                    try
                    {
                        EtwDiagnosticTrace.ActivityId = _activityId;
                        await _asyncCallback(_state);
                    }
                    finally
                    {
                        EtwDiagnosticTrace.ActivityId = currentActivityId;
                    }
                }
                else
                {
                    Guid previous = Guid.Empty;
                    bool restoreActivityId = false;
                    try
                    {
                        if (_eventTraceActivity != null)
                        {
                            previous = Trace.CorrelationManager.ActivityId;
                            restoreActivityId = true;
                            Trace.CorrelationManager.ActivityId = _eventTraceActivity.ActivityId;
                            if (TraceCore.ActionItemCallbackInvokedIsEnabled(Fx.Trace))
                            {
                                TraceCore.ActionItemCallbackInvoked(Fx.Trace, _eventTraceActivity);
                            }
                        }

                        await _asyncCallback(_state);
                    }
                    finally
                    {
                        if (restoreActivityId)
                        {
                            Trace.CorrelationManager.ActivityId = previous;
                        }
                    }
                }
            }
        }
    }
}
