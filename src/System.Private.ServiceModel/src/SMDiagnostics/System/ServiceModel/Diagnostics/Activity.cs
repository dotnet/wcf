// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.Diagnostics;

namespace System.ServiceModel.Diagnostics
{
    internal class Activity : IDisposable
    {
        protected Guid parentId;
        private Guid _currentId;
        private bool _mustDispose = false;

        protected Activity(Guid activityId, Guid parentId)
        {
            _currentId = activityId;
            this.parentId = parentId;
            _mustDispose = true;
            DiagnosticTraceBase.ActivityId = _currentId;
        }

        internal static Activity CreateActivity(Guid activityId)
        {
            Activity retval = null;
            if (activityId != Guid.Empty)
            {
                Guid currentActivityId = DiagnosticTraceBase.ActivityId;
                if (activityId != currentActivityId)
                {
                    retval = new Activity(activityId, currentActivityId);
                }
            }
            return retval;
        }

        public virtual void Dispose()
        {
            if (_mustDispose)
            {
                _mustDispose = false;
                DiagnosticTraceBase.ActivityId = this.parentId;
            }
            GC.SuppressFinalize(this);
        }

        protected Guid Id
        {
            get { return _currentId; }
        }
    }
}
