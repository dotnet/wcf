// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Threading;

namespace System.Runtime
{
    internal class IOTimer<TState> : IDisposable
    {
        private readonly Action<TState> _callback;
        private readonly TState _state;
        private Timer _timer;
        private bool _enabled;
        private DateTime _dueDateTime;
        private const double MaxSkew = 100.0; // Milliseconds

        public IOTimer(Action<TState> callback, TState state)
        {
            _callback = callback;
            _state = state;
            _enabled = false;
            _dueDateTime = DateTime.MinValue;
            // Creating a closure of the instance method OnTimer. This is to avoid a cast which
            // includes checking if the cast is compatible when the timer fires.
            _timer = new Timer(OnTimer, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Cancel()
        {
            // As this is an IOTimer, it is likely that the timer will either be reused or rescheduled
            // before the timer would actually fire. By disabling the callback from running, the timer
            // is effectively cancelled without the cost of changing the timer schedule in the timer manager.
            _enabled = false;
        }

        public void ScheduleAfter(TimeSpan delay)
        {
            // In high request rate scenarios, the requested timer scheduled time will be very close in time
            // to the previous requested expiration. In those cases, the high cost of changing the timer 
            // scheduled time can be avoided.
            DateTime now = DateTime.UtcNow;
            DateTime requestedDueDateTime = now.Add(delay);
            TimeSpan diff = requestedDueDateTime.Subtract(_dueDateTime);
            double millisecondsDiff = diff.TotalMilliseconds;
            if (millisecondsDiff < 0)
                millisecondsDiff = 0 - millisecondsDiff;
            if (millisecondsDiff > MaxSkew)
            {
                _timer.Change(delay, TimeSpan.FromMilliseconds(-1));
                _dueDateTime = requestedDueDateTime;
            }

            _enabled = true;
        }

        private void OnTimer(object state)
        {
            if (_enabled)
                _callback(_state);
        }

        public void Dispose()
        {
            _timer.Dispose();
        }
    }
}