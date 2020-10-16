// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.Diagnostics;

namespace System.ServiceModel.Diagnostics
{
    internal class ServiceModelActivity : IDisposable
    {
        [ThreadStatic]
        private static ServiceModelActivity s_currentActivity;

        private static string[] s_ActivityTypeNames = new string[(int)ActivityType.NumItems];

        private ServiceModelActivity _previousActivity = null;
        private static string s_activityBoundaryDescription = null;
        private ActivityState _lastState = ActivityState.Unknown;
        private string _name = null;
        private bool _autoStop = false;
        private bool _autoResume = false;
        private Guid _activityId;
        private bool _disposed = false;
        private bool _isAsync = false;
        private int _stopCount = 0;
        private const int AsyncStopCount = 2;
        private TransferActivity _activity = null;
        private ActivityType _activityType = ActivityType.Unknown;

        static ServiceModelActivity()
        {
            s_ActivityTypeNames[(int)ActivityType.Unknown] = "Unknown";
            s_ActivityTypeNames[(int)ActivityType.Close] = "Close";
            s_ActivityTypeNames[(int)ActivityType.Construct] = "Construct";
            s_ActivityTypeNames[(int)ActivityType.ExecuteUserCode] = "ExecuteUserCode";
            s_ActivityTypeNames[(int)ActivityType.ListenAt] = "ListenAt";
            s_ActivityTypeNames[(int)ActivityType.Open] = "Open";
            s_ActivityTypeNames[(int)ActivityType.OpenClient] = "Open";
            s_ActivityTypeNames[(int)ActivityType.ProcessMessage] = "ProcessMessage";
            s_ActivityTypeNames[(int)ActivityType.ProcessAction] = "ProcessAction";
            s_ActivityTypeNames[(int)ActivityType.ReceiveBytes] = "ReceiveBytes";
            s_ActivityTypeNames[(int)ActivityType.SecuritySetup] = "SecuritySetup";
            s_ActivityTypeNames[(int)ActivityType.TransferToComPlus] = "TransferToComPlus";
            s_ActivityTypeNames[(int)ActivityType.WmiGetObject] = "WmiGetObject";
            s_ActivityTypeNames[(int)ActivityType.WmiPutInstance] = "WmiPutInstance";
        }

        private ServiceModelActivity(Guid activityId)
        {
            _activityId = activityId;
            _previousActivity = ServiceModelActivity.Current;
        }

        private static string ActivityBoundaryDescription
        {
            get
            {
                if (ServiceModelActivity.s_activityBoundaryDescription == null)
                {
                    ServiceModelActivity.s_activityBoundaryDescription = SRServiceModel.ActivityBoundary;
                }
                return ServiceModelActivity.s_activityBoundaryDescription;
            }
        }

        internal ActivityType ActivityType
        {
            get { return _activityType; }
        }

        internal ServiceModelActivity PreviousActivity
        {
            get { return _previousActivity; }
        }

        static internal Activity BoundOperation(ServiceModelActivity activity)
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
                retval = TransferActivity.CreateActivity(activity._activityId, addTransfer);
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
                        this.Stop();
                    }
                    if (_autoResume &&
                        ServiceModelActivity.Current != null)
                    {
                        ServiceModelActivity.Current.Resume();
                    }
                }
                finally
                {
                    ServiceModelActivity.Current = _previousActivity;
                    GC.SuppressFinalize(this);
                }
            }
        }

        internal Guid Id
        {
            get { return _activityId; }
        }

        private ActivityState LastState
        {
            get { return _lastState; }
            set { _lastState = value; }
        }

        internal string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        internal void Resume()
        {
            if (this.LastState == ActivityState.Suspend)
            {
                this.LastState = ActivityState.Resume;
            }
        }

        internal void Resume(string activityName)
        {
            if (string.IsNullOrEmpty(this.Name))
            {
                _name = activityName;
            }
            this.Resume();
        }

        static internal void Start(ServiceModelActivity activity, string activityName, ActivityType activityType)
        {
            if (activity != null && activity.LastState == ActivityState.Unknown)
            {
                activity.LastState = ActivityState.Start;
                activity._name = activityName;
                activity._activityType = activityType;
            }
        }

        internal void Stop()
        {
            int newStopCount = 0;
            if (_isAsync)
            {
                newStopCount = Interlocked.Increment(ref _stopCount);
            }
            if (this.LastState != ActivityState.Stop &&
                (!_isAsync || (_isAsync && newStopCount >= ServiceModelActivity.AsyncStopCount)))
            {
                this.LastState = ActivityState.Stop;
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
            if (this.LastState != ActivityState.Stop)
            {
                this.LastState = ActivityState.Suspend;
            }
        }

        public override string ToString()
        {
            return this.Id.ToString();
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
                        using (Activity.CreateActivity(this.Id))
                        {
                            if (null != FxTrace.Trace)
                            {
                                FxTrace.Trace.TraceTransfer(this.parentId);
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
