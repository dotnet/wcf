// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Security;
using System.ServiceModel;

namespace System.Runtime.Diagnostics
{
    public class EventTraceActivity
    {
        // This field is public because it needs to be passed by reference for P/Invoke
        public Guid ActivityId;
        private static EventTraceActivity s_empty;

        public EventTraceActivity(bool setOnThread = false)
            : this(Guid.NewGuid(), setOnThread)
        {
        }

        public EventTraceActivity(Guid guid, bool setOnThread = false)
        {
            ActivityId = guid;
            if (setOnThread)
            {
                SetActivityIdOnThread();
            }
        }


        public static EventTraceActivity Empty
        {
            get
            {
                if (s_empty == null)
                {
                    s_empty = new EventTraceActivity(Guid.Empty);
                }

                return s_empty;
            }
        }

        public static string Name
        {
            get { return "E2EActivity"; }
        }

        public static EventTraceActivity GetFromThreadOrCreate(bool clearIdOnThread = false)
        {
            Guid guid = Trace.CorrelationManager.ActivityId;
            if (guid == Guid.Empty)
            {
                guid = Guid.NewGuid();
            }
            else if (clearIdOnThread)
            {
                // Reset the ActivityId on the thread to avoid using the same Id again
                Trace.CorrelationManager.ActivityId = Guid.Empty;
            }

            return new EventTraceActivity(guid);
        }

        public static Guid GetActivityIdFromThread()
        {
            return EventSource.CurrentThreadActivityId;
        }

        private void SetActivityIdOnThread()
        {
            EventSource.SetCurrentThreadActivityId(ActivityId);
        }
    }
}
