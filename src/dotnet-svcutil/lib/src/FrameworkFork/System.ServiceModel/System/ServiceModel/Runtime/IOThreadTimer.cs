//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace System.Runtime
{
    using System;
    using System.ComponentModel;
    //using System.Runtime.Interop;
    using System.Security;
    using System.Threading;
    using Microsoft.Win32.SafeHandles;

    // IOThreadTimer has several characterstics that are important for performance:
    // - Timers that expire benefit from being scheduled to run on IO threads using IOThreadScheduler.Schedule.
    // - The timer "waiter" thread thread is only allocated if there are set timers.
    // - The timer waiter thread itself is an IO thread, which allows it to go away if there is no need for it,
    //   and allows it to be reused for other purposes.
    // - After the timer count goes to zero, the timer waiter thread remains active for a bounded amount
    //   of time to wait for additional timers to be set.
    // - Timers are stored in an array-based priority queue to reduce the amount of time spent in updates, and
    //   to always provide O(1) access to the minimum timer (the first one that will expire).
    // - The standard textbook priority queue data structure is extended to allow efficient Delete in addition to 
    //   DeleteMin for efficient handling of canceled timers.
    // - Timers that are typically set, then immediately canceled (such as a retry timer, 
    //   or a flush timer), are tracked separately from more stable timers, to avoid having 
    //   to update the waitable timer in the typical case when a timer is canceled.  Whether 
    //   a timer instance follows this pattern is specified when the timer is constructed.
    // - Extending a timer by a configurable time delta (maxSkew) does not involve updating the
    //   waitable timer, or taking a lock.
    // - Timer instances are relatively cheap.  They share "heavy" resources like the waiter thread and 
    //   waitable timer handle.
    // - Setting or canceling a timer does not typically involve any allocations.

    class IOThreadTimer
    {
        const int MaxSkewInMillisecondsDefault = 100;
        static long s_systemTimeResolutionTicks = -1;
        Action<object> _callback;
        object _callbackState;
        long _dueTime;

        int _index;
        long _maxSkew;
        TimerGroup _timerGroup;

        public IOThreadTimer(Action<object> callback, object callbackState, bool isTypicallyCanceledShortlyAfterBeingSet)
            : this(callback, callbackState, isTypicallyCanceledShortlyAfterBeingSet, MaxSkewInMillisecondsDefault)
        {
        }

        public IOThreadTimer(Action<object> callback, object callbackState, bool isTypicallyCanceledShortlyAfterBeingSet, int maxSkewInMilliseconds)
        {
            this._callback = callback;
            this._callbackState = callbackState;
            this._maxSkew = Ticks.FromMilliseconds(maxSkewInMilliseconds);
            this._timerGroup =
                (isTypicallyCanceledShortlyAfterBeingSet ? TimerManager.Value.VolatileTimerGroup : TimerManager.Value.StableTimerGroup);
        }

        public static long SystemTimeResolutionTicks
        {
            get
            {
                if (IOThreadTimer.s_systemTimeResolutionTicks == -1)
                {
                    IOThreadTimer.s_systemTimeResolutionTicks = GetSystemTimeResolution();
                }
                return IOThreadTimer.s_systemTimeResolutionTicks;
            }
        }

        [Fx.Tag.SecurityNote(Critical = "Calls critical method GetSystemTimeAdjustment", Safe = "method is a SafeNativeMethod")]
        [SecuritySafeCritical]
        static long GetSystemTimeResolution()
        {
            //int dummyAdjustment;
            //uint increment;
            //uint dummyAdjustmentDisabled;

            //if (UnsafeNativeMethods.GetSystemTimeAdjustment(out dummyAdjustment, out increment, out dummyAdjustmentDisabled) != 0)
            //{
            //    return (long)increment;
            //}

            // Assume the default, which is around 15 milliseconds.
            return 15 * TimeSpan.TicksPerMillisecond;
        }

        public bool Cancel()
        {
            return TimerManager.Value.Cancel(this);
        }

        public void Set(TimeSpan timeFromNow)
        {
            if (timeFromNow != TimeSpan.MaxValue)
            {
                SetAt(Ticks.Add(Ticks.Now, Ticks.FromTimeSpan(timeFromNow)));
            }
        }

        public void Set(int millisecondsFromNow)
        {
            SetAt(Ticks.Add(Ticks.Now, Ticks.FromMilliseconds(millisecondsFromNow)));
        }

        public void SetAt(long dueTime)
        {
            TimerManager.Value.Set(this, dueTime);
        }

        [Fx.Tag.SynchronizationObject(Blocking = false, Scope = Fx.Tag.Strings.AppDomain)]
        class TimerManager
        {
            const long MaxTimeToWaitForMoreTimers = 1000 * TimeSpan.TicksPerMillisecond;

            [Fx.Tag.Queue(typeof(IOThreadTimer), Scope = Fx.Tag.Strings.AppDomain, StaleElementsRemovedImmediately = true)]
            static TimerManager s_value = new TimerManager();

            Action<object> _onWaitCallback;
            TimerGroup _stableTimerGroup;
            TimerGroup _volatileTimerGroup;
            [Fx.Tag.SynchronizationObject(Blocking = false)]
            WaitableTimer[] _waitableTimers;

            bool _waitScheduled;

            public TimerManager()
            {
                this._onWaitCallback = new Action<object>(OnWaitCallback);
                this._stableTimerGroup = new TimerGroup();
                this._volatileTimerGroup = new TimerGroup();
                this._waitableTimers = new WaitableTimer[] { this._stableTimerGroup.WaitableTimer, this._volatileTimerGroup.WaitableTimer };
            }

            object ThisLock
            {
                get { return this; }
            }

            public static TimerManager Value
            {
                get
                {
                    return TimerManager.s_value;
                }
            }

            public TimerGroup StableTimerGroup
            {
                get
                {
                    return this._stableTimerGroup;
                }
            }
            public TimerGroup VolatileTimerGroup
            {
                get
                {
                    return this._volatileTimerGroup;
                }
            }

            public void Set(IOThreadTimer timer, long dueTime)
            {
                long timeDiff = dueTime - timer._dueTime;
                if (timeDiff < 0)
                {
                    timeDiff = -timeDiff;
                }

                if (timeDiff > timer._maxSkew)
                {
                    lock (ThisLock)
                    {
                        TimerGroup timerGroup = timer._timerGroup;
                        TimerQueue timerQueue = timerGroup.TimerQueue;

                        if (timer._index > 0)
                        {
                            if (timerQueue.UpdateTimer(timer, dueTime))
                            {
                                UpdateWaitableTimer(timerGroup);
                            }
                        }
                        else
                        {
                            if (timerQueue.InsertTimer(timer, dueTime))
                            {
                                UpdateWaitableTimer(timerGroup);

                                if (timerQueue.Count == 1)
                                {
                                    EnsureWaitScheduled();
                                }
                            }
                        }
                    }
                }
            }

            public bool Cancel(IOThreadTimer timer)
            {
                lock (ThisLock)
                {
                    if (timer._index > 0)
                    {
                        TimerGroup timerGroup = timer._timerGroup;
                        TimerQueue timerQueue = timerGroup.TimerQueue;

                        timerQueue.DeleteTimer(timer);

                        if (timerQueue.Count > 0)
                        {
                            UpdateWaitableTimer(timerGroup);
                        }
                        else
                        {
                            TimerGroup otherTimerGroup = GetOtherTimerGroup(timerGroup);
                            if (otherTimerGroup.TimerQueue.Count == 0)
                            {
                                long now = Ticks.Now;
                                long thisGroupRemainingTime = timerGroup.WaitableTimer.DueTime - now;
                                long otherGroupRemainingTime = otherTimerGroup.WaitableTimer.DueTime - now;
                                if (thisGroupRemainingTime > MaxTimeToWaitForMoreTimers &&
                                    otherGroupRemainingTime > MaxTimeToWaitForMoreTimers)
                                {
                                    timerGroup.WaitableTimer.Set(Ticks.Add(now, MaxTimeToWaitForMoreTimers));
                                }
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            void EnsureWaitScheduled()
            {
                if (!this._waitScheduled)
                {
                    ScheduleWait();
                }
            }

            TimerGroup GetOtherTimerGroup(TimerGroup timerGroup)
            {
                if (object.ReferenceEquals(timerGroup, this._volatileTimerGroup))
                {
                    return this._stableTimerGroup;
                }
                else
                {
                    return this._volatileTimerGroup;
                }
            }

            void OnWaitCallback(object state)
            {
                WaitHandle.WaitAny(this._waitableTimers);
                long now = Ticks.Now;
                lock (ThisLock)
                {
                    this._waitScheduled = false;
                    ScheduleElapsedTimers(now);
                    ReactivateWaitableTimers();
                    ScheduleWaitIfAnyTimersLeft();
                }
            }

            void ReactivateWaitableTimers()
            {
                ReactivateWaitableTimer(this._stableTimerGroup);
                ReactivateWaitableTimer(this._volatileTimerGroup);
            }

            void ReactivateWaitableTimer(TimerGroup timerGroup)
            {
                TimerQueue timerQueue = timerGroup.TimerQueue;

                if (timerQueue.Count > 0)
                {
                    timerGroup.WaitableTimer.Set(timerQueue.MinTimer._dueTime);
                }
                else
                {
                    timerGroup.WaitableTimer.Set(long.MaxValue);
                }
            }

            void ScheduleElapsedTimers(long now)
            {
                ScheduleElapsedTimers(this._stableTimerGroup, now);
                ScheduleElapsedTimers(this._volatileTimerGroup, now);
            }

            void ScheduleElapsedTimers(TimerGroup timerGroup, long now)
            {
                TimerQueue timerQueue = timerGroup.TimerQueue;
                while (timerQueue.Count > 0)
                {
                    IOThreadTimer timer = timerQueue.MinTimer;
                    long timeDiff = timer._dueTime - now;
                    if (timeDiff <= timer._maxSkew)
                    {
                        timerQueue.DeleteMinTimer();
                        ActionItem.Schedule(timer._callback, timer._callbackState);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            void ScheduleWait()
            {
                ActionItem.Schedule(this._onWaitCallback, null);
                this._waitScheduled = true;
            }

            void ScheduleWaitIfAnyTimersLeft()
            {
                if (this._stableTimerGroup.TimerQueue.Count > 0 ||
                    this._volatileTimerGroup.TimerQueue.Count > 0)
                {
                    ScheduleWait();
                }
            }

            void UpdateWaitableTimer(TimerGroup timerGroup)
            {
                WaitableTimer waitableTimer = timerGroup.WaitableTimer;
                IOThreadTimer minTimer = timerGroup.TimerQueue.MinTimer;
                long timeDiff = waitableTimer.DueTime - minTimer._dueTime;
                if (timeDiff < 0)
                {
                    timeDiff = -timeDiff;
                }
                if (timeDiff > minTimer._maxSkew)
                {
                    waitableTimer.Set(minTimer._dueTime);
                }
            }
        }

        class TimerGroup
        {
            TimerQueue _timerQueue;
            WaitableTimer _waitableTimer;

            public TimerGroup()
            {
                this._waitableTimer = new WaitableTimer();
                this._waitableTimer.Set(long.MaxValue);
                this._timerQueue = new TimerQueue();
            }

            public TimerQueue TimerQueue
            {
                get
                {
                    return this._timerQueue;
                }
            }
            public WaitableTimer WaitableTimer
            {
                get
                {
                    return this._waitableTimer;
                }
            }
        }

        class TimerQueue
        {
            int _count;
            IOThreadTimer[] _timers;

            public TimerQueue()
            {
                this._timers = new IOThreadTimer[4];
            }

            public int Count
            {
                get { return _count; }
            }

            public IOThreadTimer MinTimer
            {
                get
                {
                    Fx.Assert(this._count > 0, "Should have at least one timer in our queue.");
                    return _timers[1];
                }
            }
            public void DeleteMinTimer()
            {
                IOThreadTimer minTimer = this.MinTimer;
                DeleteMinTimerCore();
                minTimer._index = 0;
                minTimer._dueTime = 0;
            }
            public void DeleteTimer(IOThreadTimer timer)
            {
                int index = timer._index;

                Fx.Assert(index > 0, "");
                Fx.Assert(index <= this._count, "");

                IOThreadTimer[] timers = this._timers;

                for (;;)
                {
                    int parentIndex = index / 2;

                    if (parentIndex >= 1)
                    {
                        IOThreadTimer parentTimer = timers[parentIndex];
                        timers[index] = parentTimer;
                        parentTimer._index = index;
                    }
                    else
                    {
                        break;
                    }

                    index = parentIndex;
                }

                timer._index = 0;
                timer._dueTime = 0;
                timers[1] = null;
                DeleteMinTimerCore();
            }

            public bool InsertTimer(IOThreadTimer timer, long dueTime)
            {
                Fx.Assert(timer._index == 0, "Timer should not have an index.");

                IOThreadTimer[] timers = this._timers;

                int index = this._count + 1;

                if (index == timers.Length)
                {
                    timers = new IOThreadTimer[timers.Length * 2];
                    Array.Copy(this._timers, timers, this._timers.Length);
                    this._timers = timers;
                }

                this._count = index;

                if (index > 1)
                {
                    for (;;)
                    {
                        int parentIndex = index / 2;

                        if (parentIndex == 0)
                        {
                            break;
                        }

                        IOThreadTimer parent = timers[parentIndex];

                        if (parent._dueTime > dueTime)
                        {
                            timers[index] = parent;
                            parent._index = index;
                            index = parentIndex;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                timers[index] = timer;
                timer._index = index;
                timer._dueTime = dueTime;
                return index == 1;
            }
            public bool UpdateTimer(IOThreadTimer timer, long dueTime)
            {
                int index = timer._index;

                IOThreadTimer[] timers = this._timers;
                int count = this._count;

                Fx.Assert(index > 0, "");
                Fx.Assert(index <= count, "");

                int parentIndex = index / 2;
                if (parentIndex == 0 ||
                    timers[parentIndex]._dueTime <= dueTime)
                {
                    int leftChildIndex = index * 2;
                    if (leftChildIndex > count ||
                        timers[leftChildIndex]._dueTime >= dueTime)
                    {
                        int rightChildIndex = leftChildIndex + 1;
                        if (rightChildIndex > count ||
                            timers[rightChildIndex]._dueTime >= dueTime)
                        {
                            timer._dueTime = dueTime;
                            return index == 1;
                        }
                    }
                }

                DeleteTimer(timer);
                InsertTimer(timer, dueTime);
                return true;
            }

            void DeleteMinTimerCore()
            {
                int count = this._count;

                if (count == 1)
                {
                    this._count = 0;
                    this._timers[1] = null;
                }
                else
                {
                    IOThreadTimer[] timers = this._timers;
                    IOThreadTimer lastTimer = timers[count];
                    this._count = --count;

                    int index = 1;
                    for (;;)
                    {
                        int leftChildIndex = index * 2;

                        if (leftChildIndex > count)
                        {
                            break;
                        }

                        int childIndex;
                        IOThreadTimer child;

                        if (leftChildIndex < count)
                        {
                            IOThreadTimer leftChild = timers[leftChildIndex];
                            int rightChildIndex = leftChildIndex + 1;
                            IOThreadTimer rightChild = timers[rightChildIndex];

                            if (rightChild._dueTime < leftChild._dueTime)
                            {
                                child = rightChild;
                                childIndex = rightChildIndex;
                            }
                            else
                            {
                                child = leftChild;
                                childIndex = leftChildIndex;
                            }
                        }
                        else
                        {
                            childIndex = leftChildIndex;
                            child = timers[childIndex];
                        }

                        if (lastTimer._dueTime > child._dueTime)
                        {
                            timers[index] = child;
                            child._index = index;
                        }
                        else
                        {
                            break;
                        }

                        index = childIndex;

                        if (leftChildIndex >= count)
                        {
                            break;
                        }
                    }

                    timers[index] = lastTimer;
                    lastTimer._index = index;
                    timers[count + 1] = null;
                }
            }
        }

        [Fx.Tag.SynchronizationPrimitive(Fx.Tag.BlocksUsing.NonBlocking)]
        class WaitableTimer : WaitHandle
        {

            long _dueTime;

            [Fx.Tag.SecurityNote(Critical = "Call the critical CreateWaitableTimer method in TimerHelper",
                Safe = "Doesn't leak information or resources")]
            [SecuritySafeCritical]
            public WaitableTimer()
            {
                this.SafeWaitHandle = TimerHelper.CreateWaitableTimer();
            }

            public long DueTime
            {
                get { return this._dueTime; }
            }

            [Fx.Tag.SecurityNote(Critical = "Call the critical Set method in TimerHelper",
                Safe = "Doesn't leak information or resources")]
            [SecuritySafeCritical]
            public void Set(long dueTime)
            {
                this._dueTime = TimerHelper.Set(this.SafeWaitHandle, dueTime);
            }
            [Fx.Tag.SecurityNote(Critical = "Provides a set of unsafe methods used to work with the WaitableTimer")]
            [SecurityCritical]
            static class TimerHelper
            {
                public static unsafe SafeWaitHandle CreateWaitableTimer()
                {
                    SafeWaitHandle handle = ServiceModel.Channels.UnsafeNativeMethods.CreateWaitableTimer(IntPtr.Zero, false, null);
                    if (handle.IsInvalid)
                    {
                        Exception exception = new Win32Exception();
                        handle.SetHandleAsInvalid();
                        throw Fx.Exception.AsError(exception);
                    }
                    return handle;
                }
                public static unsafe long Set(SafeWaitHandle timer, long dueTime)
                {
                    if (!ServiceModel.Channels.UnsafeNativeMethods.SetWaitableTimer(timer, ref dueTime, 0, IntPtr.Zero, IntPtr.Zero, false))
                    {
                        throw Fx.Exception.AsError(new Win32Exception());
                    }
                    return dueTime;
                }
            }
        }
    }
}


