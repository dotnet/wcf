// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Runtime.Diagnostics;

namespace System.ServiceModel.Diagnostics
{
    internal class Activity : IDisposable
    {
        protected Guid parentId;
        private bool _mustDispose = false;

        protected Activity(Guid activityId, Guid parentId)
        {
            Id = activityId;
            this.parentId = parentId;
            _mustDispose = true;
            // Unreachable code
            //DiagnosticTraceBase.ActivityId = Id;
        }

        internal static Activity CreateActivity(Guid activityId)
        {
            // Unreachable code
            //Activity retval = null;
            //if (activityId != Guid.Empty)
            //{
            //    Guid currentActivityId = DiagnosticTraceBase.ActivityId;
            //    if (activityId != currentActivityId)
            //    {
            //        retval = new Activity(activityId, currentActivityId);
            //    }
            //}
            //return retval;
            return default;
        }

        public virtual void Dispose()
        {
            if (_mustDispose)
            {
                _mustDispose = false;
                // Unreachable code
                //DiagnosticTraceBase.ActivityId = parentId;
            }
            GC.SuppressFinalize(this);
        }

        protected Guid Id { get; }
    }
}
