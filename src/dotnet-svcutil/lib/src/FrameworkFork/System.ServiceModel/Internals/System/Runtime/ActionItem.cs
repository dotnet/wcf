// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.Runtime.Diagnostics;
using System.Security;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime
{
    internal abstract class ActionItem
    {
        private bool _isScheduled;
        private bool _lowPriority;

        protected ActionItem()
        {
        }

        public bool LowPriority
        {
            get
            {
                return _lowPriority;
            }
            protected set
            {
                _lowPriority = value;
            }
        }

        public static void Schedule(Action<object> callback, object state)
        {
            Contract.Assert(callback != null, "Cannot schedule a null callback");
            Task.Factory.StartNew(callback, state, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        [Fx.Tag.SecurityNote(Critical = "Called after applying the user context on the stack or (potentially) " +
            "without any user context on the stack")]
        [SecurityCritical]
        protected abstract void Invoke();

        [Fx.Tag.SecurityNote(Critical = "Access critical field context and critical property " +
            "CallbackHelper.InvokeWithContextCallback, calls into critical method " +
            "PartialTrustHelpers.CaptureSecurityContextNoIdentityFlow, calls into critical method ScheduleCallback; " +
            "since the invoked method and the capturing of the security contex are de-coupled, can't " +
            "be treated as safe")]
        [SecurityCritical]
        protected void Schedule()
        {
            if (_isScheduled)
            {
                throw Fx.Exception.AsError(new InvalidOperationException(InternalSR.ActionItemIsAlreadyScheduled));
            }

            _isScheduled = true;
            ScheduleCallback(CallbackHelper.InvokeCallbackAction);
        }
        [Fx.Tag.SecurityNote(Critical = "Calls into critical static method ScheduleCallback")]
        [SecurityCritical]

        private void ScheduleCallback(Action<object> callback)
        {
            Fx.Assert(callback != null, "Cannot schedule a null callback");
            Task.Factory.StartNew(callback, this, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        [SecurityCritical]
        internal static class CallbackHelper
        {
            [Fx.Tag.SecurityNote(Critical = "Stores a delegate to a critical method")]
            private static Action<object> s_invokeCallback;

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

            [Fx.Tag.SecurityNote(Critical = "Called by the scheduler without any user context on the stack")]
            private static void InvokeCallback(object state)
            {
                ((ActionItem)state).Invoke();
                ((ActionItem)state)._isScheduled = false;
            }
        }

        internal class DefaultActionItem : ActionItem
        {
            [Fx.Tag.SecurityNote(Critical = "Stores a delegate that will be called later, at a particular context")]
            [SecurityCritical]
            private Action<object> _callback;
            [Fx.Tag.SecurityNote(Critical = "Stores an object that will be passed to the delegate that will be " +
                            "called later, at a particular context")]
            [SecurityCritical]
            private object _state;

            private bool _flowLegacyActivityId;
            private Guid _activityId;
            private EventTraceActivity _eventTraceActivity;

            [Fx.Tag.SecurityNote(Critical = "Access critical fields callback and state",
                Safe = "Doesn't leak information or resources")]
            [SecuritySafeCritical]
            public DefaultActionItem(Action<object> callback, object state, bool isLowPriority)
            {
                Fx.Assert(callback != null, "Shouldn't instantiate an object to wrap a null callback");
                base.LowPriority = isLowPriority;
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

            [Fx.Tag.SecurityNote(Critical = "Implements a the critical abstract ActionItem.Invoke method, " +
                "Access critical fields callback and state")]
            [SecurityCritical]
            protected override void Invoke()
            {
                if (_flowLegacyActivityId || Fx.Trace.IsEnd2EndActivityTracingEnabled)
                {
                    TraceAndInvoke();
                }
                else
                {
                    _callback(_state);
                }
            }
            [Fx.Tag.SecurityNote(Critical = "Implements a the critical abstract Trace method, " +
                                "Access critical fields callback and state")]
            [SecurityCritical]

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
                    _callback(_state);
                }
            }
        }
    }
}
