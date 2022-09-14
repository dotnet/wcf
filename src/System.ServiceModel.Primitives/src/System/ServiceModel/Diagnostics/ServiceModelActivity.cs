// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Diagnostics;
using System.Threading;

namespace System.ServiceModel.Diagnostics
{
    internal class ServiceModelActivity : IDisposable
    {
        [ThreadStatic]
        private static ServiceModelActivity s_currentActivity;

        private static string[] s_activityTypeNames = new string[(int)ActivityType.NumItems];
        private bool _autoStop = false;
        private bool _autoResume = false;
        private bool _disposed = false;
        private bool _isAsync = false;
        private int _stopCount = 0;
        private const int AsyncStopCount = 2;
        private TransferActivity _activity = null;

        static ServiceModelActivity()
        {
            s_activityTypeNames[(int)ActivityType.Unknown] = "Unknown";
            s_activityTypeNames[(int)ActivityType.Close] = "Close";
            s_activityTypeNames[(int)ActivityType.Construct] = "Construct";
            s_activityTypeNames[(int)ActivityType.ExecuteUserCode] = "ExecuteUserCode";
            s_activityTypeNames[(int)ActivityType.ListenAt] = "ListenAt";
            s_activityTypeNames[(int)ActivityType.Open] = "Open";
            s_activityTypeNames[(int)ActivityType.OpenClient] = "Open";
            s_activityTypeNames[(int)ActivityType.ProcessMessage] = "ProcessMessage";
            s_activityTypeNames[(int)ActivityType.ProcessAction] = "ProcessAction";
            s_activityTypeNames[(int)ActivityType.ReceiveBytes] = "ReceiveBytes";
            s_activityTypeNames[(int)ActivityType.SecuritySetup] = "SecuritySetup";
            s_activityTypeNames[(int)ActivityType.TransferToComPlus] = "TransferToComPlus";
            s_activityTypeNames[(int)ActivityType.WmiGetObject] = "WmiGetObject";
            s_activityTypeNames[(int)ActivityType.WmiPutInstance] = "WmiPutInstance";
        }

        private ServiceModelActivity(Guid activityId)
        {
            Id = activityId;
            PreviousActivity = ServiceModelActivity.Current;
        }


        internal ActivityType ActivityType { get; private set; } = ActivityType.Unknown;

        internal ServiceModelActivity PreviousActivity { get; } = null;

        internal static Activity BoundOperation(ServiceModelActivity activity)
        {
            if (!DiagnosticUtility.ShouldUseActivity)
            {
                return null;
            }
            return ServiceModelActivity.BoundOperation(activity, false);
        }

        static internal Activity BoundOperation(ServiceModelActivity activity, bool addTransfer)
        {
            return activity == null ? null : ServiceModelActivity.BoundOperationCore(activity, addTransfer);
        }

        private static Activity BoundOperationCore(ServiceModelActivity activity, bool addTransfer)
        {
            if (!DiagnosticUtility.ShouldUseActivity)
            {
                return null;
            }
            TransferActivity retval = null;
            if (activity != null)
            {
                retval = TransferActivity.CreateActivity(activity.Id, addTransfer);
                if (retval != null)
                {
                    retval.SetPreviousServiceModelActivity(ServiceModelActivity.Current);
                }
                ServiceModelActivity.Current = activity;
            }
            return retval;
        }

        internal static ServiceModelActivity CreateActivity()
        {
            if (!DiagnosticUtility.ShouldUseActivity)
            {
                return null;
            }
            return ServiceModelActivity.CreateActivity(Guid.NewGuid(), true);
        }

        internal static ServiceModelActivity CreateActivity(bool autoStop)
        {
            if (!DiagnosticUtility.ShouldUseActivity)
            {
                return null;
            }
            ServiceModelActivity activity = ServiceModelActivity.CreateActivity(Guid.NewGuid(), true);
            if (activity != null)
            {
                activity._autoStop = autoStop;
            }
            return activity;
        }

        internal static ServiceModelActivity CreateActivity(bool autoStop, string activityName, ActivityType activityType)
        {
            if (!DiagnosticUtility.ShouldUseActivity)
            {
                return null;
            }
            ServiceModelActivity activity = ServiceModelActivity.CreateActivity(autoStop);
            ServiceModelActivity.Start(activity, activityName, activityType);
            return activity;
        }

        internal static ServiceModelActivity CreateAsyncActivity()
        {
            if (!DiagnosticUtility.ShouldUseActivity)
            {
                return null;
            }
            ServiceModelActivity activity = ServiceModelActivity.CreateActivity(true);
            if (activity != null)
            {
                activity._isAsync = true;
            }
            return activity;
        }

        internal static ServiceModelActivity CreateBoundedActivity()
        {
            return ServiceModelActivity.CreateBoundedActivity(false);
        }

        internal static ServiceModelActivity CreateBoundedActivity(bool suspendCurrent)
        {
            if (!DiagnosticUtility.ShouldUseActivity)
            {
                return null;
            }
            ServiceModelActivity activityToSuspend = ServiceModelActivity.Current;
            ServiceModelActivity retval = ServiceModelActivity.CreateActivity(true);
            if (retval != null)
            {
                retval._activity = (TransferActivity)ServiceModelActivity.BoundOperation(retval, true);
                retval._activity.SetPreviousServiceModelActivity(activityToSuspend);
                if (suspendCurrent)
                {
                    retval._autoResume = true;
                }
            }
            if (suspendCurrent && activityToSuspend != null)
            {
                activityToSuspend.Suspend();
            }
            return retval;
        }

        internal static ServiceModelActivity CreateBoundedActivity(Guid activityId)
        {
            if (!DiagnosticUtility.ShouldUseActivity)
            {
                return null;
            }
            ServiceModelActivity retval = ServiceModelActivity.CreateActivity(activityId, true);
            if (retval != null)
            {
                retval._activity = (TransferActivity)ServiceModelActivity.BoundOperation(retval, true);
            }
            return retval;
        }

        internal static ServiceModelActivity CreateBoundedActivityWithTransferInOnly(Guid activityId)
        {
            if (!DiagnosticUtility.ShouldUseActivity)
            {
                return null;
            }
            ServiceModelActivity retval = ServiceModelActivity.CreateActivity(activityId, true);
            if (retval != null)
            {
                if (null != FxTrace.Trace)
                {
                    FxTrace.Trace.TraceTransfer(activityId);
                }
                retval._activity = (TransferActivity)ServiceModelActivity.BoundOperation(retval);
            }
            return retval;
        }

        internal static ServiceModelActivity CreateLightWeightAsyncActivity(Guid activityId)
        {
            return new ServiceModelActivity(activityId);
        }

        internal static ServiceModelActivity CreateActivity(Guid activityId)
        {
            if (!DiagnosticUtility.ShouldUseActivity)
            {
                return null;
            }
            ServiceModelActivity retval = null;
            if (activityId != Guid.Empty)
            {
                retval = new ServiceModelActivity(activityId);
            }
            if (retval != null)
            {
                ServiceModelActivity.Current = retval;
            }
            return retval;
        }

        internal static ServiceModelActivity CreateActivity(Guid activityId, bool autoStop)
        {
            if (!DiagnosticUtility.ShouldUseActivity)
            {
                return null;
            }
            ServiceModelActivity retval = ServiceModelActivity.CreateActivity(activityId);
            if (retval != null)
            {
                retval._autoStop = autoStop;
            }
            return retval;
        }

        internal static ServiceModelActivity Current
        {
            get { return ServiceModelActivity.s_currentActivity; }
            private set { ServiceModelActivity.s_currentActivity = value; }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                try
                {
                    if (_activity != null)
                    {
                        _activity.Dispose();
                    }
                    if (_autoStop)
                    {
                        Stop();
                    }
                    if (_autoResume &&
                        ServiceModelActivity.Current != null)
                    {
                        ServiceModelActivity.Current.Resume();
                    }
                }
                finally
                {
                    ServiceModelActivity.Current = PreviousActivity;
                    GC.SuppressFinalize(this);
                }
            }
        }

        internal Guid Id { get; }

        private ActivityState LastState { get; set; } = ActivityState.Unknown;

        internal string Name { get; set; } = null;

        internal void Resume()
        {
            if (LastState == ActivityState.Suspend)
            {
                LastState = ActivityState.Resume;
            }
        }

        internal void Resume(string activityName)
        {
            if (string.IsNullOrEmpty(Name))
            {
                Name = activityName;
            }
            Resume();
        }

        static internal void Start(ServiceModelActivity activity, string activityName, ActivityType activityType)
        {
            if (activity != null && activity.LastState == ActivityState.Unknown)
            {
                activity.LastState = ActivityState.Start;
                activity.Name = activityName;
                activity.ActivityType = activityType;
            }
        }

        internal void Stop()
        {
            int newStopCount = 0;
            if (_isAsync)
            {
                newStopCount = Interlocked.Increment(ref _stopCount);
            }
            if (LastState != ActivityState.Stop &&
                (!_isAsync || (_isAsync && newStopCount >= ServiceModelActivity.AsyncStopCount)))
            {
                LastState = ActivityState.Stop;
            }
        }

        static internal void Stop(ServiceModelActivity activity)
        {
            if (activity != null)
            {
                activity.Stop();
            }
        }

        internal void Suspend()
        {
            if (LastState != ActivityState.Stop)
            {
                LastState = ActivityState.Suspend;
            }
        }

        public override string ToString()
        {
            return Id.ToString();
        }


        private enum ActivityState
        {
            Unknown,
            Start,
            Suspend,
            Resume,
            Stop,
        }

        internal class TransferActivity : Activity
        {
            private bool _addTransfer = false;
            private bool _changeCurrentServiceModelActivity = false;
            private ServiceModelActivity _previousActivity = null;

            private TransferActivity(Guid activityId, Guid parentId)
                : base(activityId, parentId)
            {
            }

            internal static TransferActivity CreateActivity(Guid activityId, bool addTransfer)
            {
                if (!DiagnosticUtility.ShouldUseActivity)
                {
                    return null;
                }
                TransferActivity retval = null;
                return retval;
            }

            internal void SetPreviousServiceModelActivity(ServiceModelActivity previous)
            {
                _previousActivity = previous;
                _changeCurrentServiceModelActivity = true;
            }

            public override void Dispose()
            {
                try
                {
                    if (_addTransfer)
                    {
                        // Make sure that we are transferring from our AID to the 
                        // parent. It is possible for someone else to change the ambient
                        // in user code (MB 49318).
                        using (Activity.CreateActivity(Id))
                        {
                            if (null != FxTrace.Trace)
                            {
                                FxTrace.Trace.TraceTransfer(parentId);
                            }
                        }
                    }
                }
                finally
                {
                    if (_changeCurrentServiceModelActivity)
                    {
                        ServiceModelActivity.Current = _previousActivity;
                    }
                    base.Dispose();
                }
            }
        }
    }
}
