// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
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
            this.ActivityId = guid;
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

        [Fx.Tag.SecurityNote(Critical = "Critical because the CorrelationManager property has a link demand on UnmanagedCode.",
            Safe = "We do not leak security data.")]
        [SecuritySafeCritical]
        public static EventTraceActivity GetFromThreadOrCreate(bool clearIdOnThread = false)
        {
            return new EventTraceActivity(Guid.Empty);
        }

        [Fx.Tag.SecurityNote(Critical = "Critical because the CorrelationManager property has a link demand on UnmanagedCode.",
            Safe = "We do not leak security data.")]
        [SecuritySafeCritical]
        public static Guid GetActivityIdFromThread()
        {
            throw ExceptionHelper.PlatformNotSupported();
        }

        public void SetActivityId(Guid guid)
        {
            this.ActivityId = guid;
        }
        [Fx.Tag.SecurityNote(Critical = "Critical because the CorrelationManager property has a link demand on UnmanagedCode.",
                    Safe = "We do not leak security data.")]
        [SecuritySafeCritical]

        private void SetActivityIdOnThread()
        {
        }
    }
}
