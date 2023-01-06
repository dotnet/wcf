﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;

namespace System.Runtime
{
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

    internal class IOThreadTimer
    {
        private const int maxSkewInMillisecondsDefault = 100;
        private Action<object> callback;
        private Func<object, Task> asyncCallback;
        private object callbackState;
        private long dueTime;
        private int index;
        private long maxSkew;
        private TimerGroup timerGroup;
        private bool isAsyncCallback;

        public IOThreadTimer(Func<object, Task> asyncCallback, object callbackState, bool isTypicallyCanceledShortlyAfterBeingSet)
            : this(null, asyncCallback, callbackState, isTypicallyCanceledShortlyAfterBeingSet, maxSkewInMillisecondsDefault)
        {
            isAsyncCallback = true;
        }

        public IOThreadTimer(Action<object> callback, object callbackState, bool isTypicallyCanceledShortlyAfterBeingSet)
            : this(callback, null, callbackState, isTypicallyCanceledShortlyAfterBeingSet, maxSkewInMillisecondsDefault)
        {
        }

        public IOThreadTimer(Action<object> callback, Func<object, Task> asyncCallback, object callbackState, bool isTypicallyCanceledShortlyAfterBeingSet, int maxSkewInMilliseconds)
        {
            this.callback = callback;
            this.asyncCallback = asyncCallback;
            this.callbackState = callbackState;
            maxSkew = Ticks.FromMilliseconds(maxSkewInMilliseconds);
            timerGroup =
                (isTypicallyCanceledShortlyAfterBeingSet ? TimerManager.Value.VolatileTimerGroup : TimerManager.Value.StableTimerGroup);
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

        protected void Reinitialize(Action<object> callback, object callbackState)
        {
            this.callback = callback;
            this.isAsyncCallback = false;
            this.callbackState = callbackState;
        }

        internal static void KillTimers()
        {
            TimerManager.Value.Kill();
        }

        private class TimerManager
        {
            private const long maxTimeToWaitForMoreTimers = 1000 * TimeSpan.TicksPerMillisecond;
            private static TimerManager value = new TimerManager();
            private Action<object> onWaitCallback;
            private TimerGroup stableTimerGroup;
            private TimerGroup volatileTimerGroup;
            private WaitableTimer[] waitableTimers;
            private bool waitScheduled;

            public TimerManager()
            {
                onWaitCallback = new Action<object>(OnWaitCallback);
                stableTimerGroup = new TimerGroup();
                volatileTimerGroup = new TimerGroup();
                waitableTimers = new WaitableTimer[] { stableTimerGroup.WaitableTimer, volatileTimerGroup.WaitableTimer };
            }

            private object ThisLock
            {
                get { return this; }
            }

            public static TimerManager Value
            {
                get
                {
                    return TimerManager.value;
                }
            }

            public TimerGroup StableTimerGroup
            {
                get
                {
                    return stableTimerGroup;
                }
            }
            public TimerGroup VolatileTimerGroup
            {
                get
                {
                    return volatileTimerGroup;
                }
            }

            internal void Kill()
            {
                stableTimerGroup.WaitableTimer.Kill();
                volatileTimerGroup.WaitableTimer.Kill();
            }

            public void Set(IOThreadTimer timer, long dueTime)
            {
                long timeDiff = dueTime - timer.dueTime;
                if (timeDiff < 0)
                {
                    timeDiff = -timeDiff;
                }

                if (timeDiff > timer.maxSkew)
                {
                    lock (ThisLock)
                    {
                        TimerGroup timerGroup = timer.timerGroup;
                        TimerQueue timerQueue = timerGroup.TimerQueue;

                        if (timer.index > 0)
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
                    if (timer.index > 0)
                    {
                        TimerGroup timerGroup = timer.timerGroup;
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
                                if (thisGroupRemainingTime > maxTimeToWaitForMoreTimers &&
                                    otherGroupRemainingTime > maxTimeToWaitForMoreTimers)
                                {
                                    timerGroup.WaitableTimer.Set(Ticks.Add(now, maxTimeToWaitForMoreTimers));
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

            private void EnsureWaitScheduled()
            {
                if (!waitScheduled)
                {
                    ScheduleWait();
                }
            }

            private TimerGroup GetOtherTimerGroup(TimerGroup timerGroup)
            {
                if (object.ReferenceEquals(timerGroup, volatileTimerGroup))
                {
                    return stableTimerGroup;
                }
                else
                {
                    return volatileTimerGroup;
                }
            }

            private void OnWaitCallback(object state)
            {
                WaitableTimer.WaitAny(waitableTimers);
                long now = Ticks.Now;
                lock (ThisLock)
                {
                    waitScheduled = false;
                    ScheduleElapsedTimers(now);
                    ReactivateWaitableTimers();
                    ScheduleWaitIfAnyTimersLeft();
                }
            }

            private void ReactivateWaitableTimers()
            {
                ReactivateWaitableTimer(stableTimerGroup);
                ReactivateWaitableTimer(volatileTimerGroup);
            }

            private void ReactivateWaitableTimer(TimerGroup timerGroup)
            {
                TimerQueue timerQueue = timerGroup.TimerQueue;

                if (timerGroup.WaitableTimer.dead)
                    return;

                if (timerQueue.Count > 0)
                {
                    timerGroup.WaitableTimer.Set(timerQueue.MinTimer.dueTime);
                }
                else
                {
                    timerGroup.WaitableTimer.Set(long.MaxValue);
                }
            }

            private void ScheduleElapsedTimers(long now)
            {
                ScheduleElapsedTimers(stableTimerGroup, now);
                ScheduleElapsedTimers(volatileTimerGroup, now);
            }

            private void ScheduleElapsedTimers(TimerGroup timerGroup, long now)
            {
                TimerQueue timerQueue = timerGroup.TimerQueue;
                while (timerQueue.Count > 0)
                {
                    IOThreadTimer timer = timerQueue.MinTimer;
                    long timeDiff = timer.dueTime - now;
                    if (timeDiff <= timer.maxSkew)
                    {
                        timerQueue.DeleteMinTimer();
                        if (timer.isAsyncCallback)
                        {
                            ActionItem.Schedule(timer.asyncCallback, timer.callbackState);
                        }
                        else
                        {
                            ActionItem.Schedule(timer.callback, timer.callbackState);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            private void ScheduleWait()
            {
                ActionItem.Schedule(onWaitCallback, null);
                waitScheduled = true;
            }

            private void ScheduleWaitIfAnyTimersLeft()
            {
                if (this.stableTimerGroup.WaitableTimer.dead &&
                    this.volatileTimerGroup.WaitableTimer.dead)
                    return;

                if (this.stableTimerGroup.TimerQueue.Count > 0 ||
                    this.volatileTimerGroup.TimerQueue.Count > 0)
                {
                    ScheduleWait();
                }
            }

            private void UpdateWaitableTimer(TimerGroup timerGroup)
            {
                WaitableTimer waitableTimer = timerGroup.WaitableTimer;
                IOThreadTimer minTimer = timerGroup.TimerQueue.MinTimer;
                long timeDiff = waitableTimer.DueTime - minTimer.dueTime;
                if (timeDiff < 0)
                {
                    timeDiff = -timeDiff;
                }
                if (timeDiff > minTimer.maxSkew)
                {
                    waitableTimer.Set(minTimer.dueTime);
                }
            }
        }

        private class TimerGroup
        {
            private TimerQueue timerQueue;
            private WaitableTimer waitableTimer;

            public TimerGroup()
            {
                waitableTimer = new WaitableTimer();
                timerQueue = new TimerQueue();
            }

            public TimerQueue TimerQueue
            {
                get
                {
                    return timerQueue;
                }
            }
            public WaitableTimer WaitableTimer
            {
                get
                {
                    return waitableTimer;
                }
            }
        }

        private class TimerQueue
        {
            private int count;
            private IOThreadTimer[] timers;

            public TimerQueue()
            {
                timers = new IOThreadTimer[4];
            }

            public int Count
            {
                get { return count; }
            }

            public IOThreadTimer MinTimer
            {
                get
                {
                    Fx.Assert(count > 0, "Should have at least one timer in our queue.");
                    return timers[1];
                }
            }
            public void DeleteMinTimer()
            {
                IOThreadTimer minTimer = MinTimer;
                DeleteMinTimerCore();
                minTimer.index = 0;
                minTimer.dueTime = 0;
            }
            public void DeleteTimer(IOThreadTimer timer)
            {
                int index = timer.index;

                Fx.Assert(index > 0, "");
                Fx.Assert(index <= count, "");

                IOThreadTimer[] timers = this.timers;

                for (; ; )
                {
                    int parentIndex = index / 2;

                    if (parentIndex >= 1)
                    {
                        IOThreadTimer parentTimer = timers[parentIndex];
                        timers[index] = parentTimer;
                        parentTimer.index = index;
                    }
                    else
                    {
                        break;
                    }

                    index = parentIndex;
                }

                timer.index = 0;
                timer.dueTime = 0;
                timers[1] = null;
                DeleteMinTimerCore();
            }

            public bool InsertTimer(IOThreadTimer timer, long dueTime)
            {
                Fx.Assert(timer.index == 0, "Timer should not have an index.");

                IOThreadTimer[] timers = this.timers;

                int index = count + 1;

                if (index == timers.Length)
                {
                    timers = new IOThreadTimer[timers.Length * 2];
                    Array.Copy(this.timers, timers, this.timers.Length);
                    this.timers = timers;
                }

                count = index;

                if (index > 1)
                {
                    for (; ; )
                    {
                        int parentIndex = index / 2;

                        if (parentIndex == 0)
                        {
                            break;
                        }

                        IOThreadTimer parent = timers[parentIndex];

                        if (parent.dueTime > dueTime)
                        {
                            timers[index] = parent;
                            parent.index = index;
                            index = parentIndex;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                timers[index] = timer;
                timer.index = index;
                timer.dueTime = dueTime;
                return index == 1;
            }
            public bool UpdateTimer(IOThreadTimer timer, long dueTime)
            {
                int index = timer.index;

                IOThreadTimer[] timers = this.timers;
                int count = this.count;

                Fx.Assert(index > 0, "");
                Fx.Assert(index <= count, "");

                int parentIndex = index / 2;
                if (parentIndex == 0 ||
                    timers[parentIndex].dueTime <= dueTime)
                {
                    int leftChildIndex = index * 2;
                    if (leftChildIndex > count ||
                        timers[leftChildIndex].dueTime >= dueTime)
                    {
                        int rightChildIndex = leftChildIndex + 1;
                        if (rightChildIndex > count ||
                            timers[rightChildIndex].dueTime >= dueTime)
                        {
                            timer.dueTime = dueTime;
                            return index == 1;
                        }
                    }
                }

                DeleteTimer(timer);
                InsertTimer(timer, dueTime);
                return true;
            }

            private void DeleteMinTimerCore()
            {
                int count = this.count;

                if (count == 1)
                {
                    this.count = 0;
                    timers[1] = null;
                }
                else
                {
                    IOThreadTimer[] timers = this.timers;
                    IOThreadTimer lastTimer = timers[count];
                    this.count = --count;

                    int index = 1;
                    for (; ; )
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

                            if (rightChild.dueTime < leftChild.dueTime)
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

                        if (lastTimer.dueTime > child.dueTime)
                        {
                            timers[index] = child;
                            child.index = index;
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
                    lastTimer.index = index;
                    timers[count + 1] = null;
                }
            }
        }

        public class WaitableTimer : EventWaitHandle
        {
            private long dueTime; // Ticks
            public bool dead;

            public WaitableTimer() : base(false, EventResetMode.AutoReset)
            {
            }

            public long DueTime
            {
                get { return dueTime; }
            }

            public void Set(long dueTime)
            {
                if (dueTime < this.dueTime)
                {
                    this.dueTime = dueTime;
                    Set(); // We might be waiting on a later time so nudge it to reworkout the time
                }
                else
                {
                    this.dueTime = dueTime;
                }
            }

            public void Kill()
            {
                dead = true;
                Set();
            }

            public static int WaitAny(WaitableTimer[] waitableTimers)
            {
                do
                {
                    var earliestDueTime = waitableTimers[0].dueTime;
                    for (int i = 1; i < waitableTimers.Length; i++)
                    {
                        if (waitableTimers[i].dead)
                            return 0;
                        if (waitableTimers[i].dueTime < earliestDueTime)
                            earliestDueTime = waitableTimers[i].dueTime;
                        waitableTimers[i].Reset();
                    }

                    var waitDurationInMillis = (earliestDueTime - DateTime.UtcNow.Ticks) / TimeSpan.TicksPerMillisecond;
                    if (waitDurationInMillis < 0) // Already passed the due time
                        return 0;

                    Contract.Assert(waitDurationInMillis < int.MaxValue, "Waiting for longer than is possible");
                    WaitHandle.WaitAny(waitableTimers, (int)waitDurationInMillis);
                    // Always loop around and check wait time again as values might have changed.
                } while (true);
            }
        }
    }
}
