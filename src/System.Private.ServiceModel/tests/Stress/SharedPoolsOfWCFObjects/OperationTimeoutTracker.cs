// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;

namespace SharedPoolsOfWCFObjects
{
    /// <summary>
    /// This class is designed to efficiently track operation timeouts in stress when the following conditions apply:
    /// 1. the rate of requests is high (> 100K rq/s) so creating/disposing individual cancellation tockens is expensive (CPU & memory)
    /// 2. the timeout duration is the same for all requests being tracked
    /// 3. we only care about "truly" stuck requests (e.g. didn't finish even after exceeding ~2x timeout)
    /// </summary>
    /// <typeparam name="TOperationToken"> token type used to identify the operation in progress </typeparam>
    public class OperationTimeoutTracker<TOperationToken>
        where TOperationToken : class
    {
        private int _timeoutMs;
        private Action<TOperationToken[]> _timeoutCallback;
        private TimeoutTracker _currentTimeoutTracker;

        /// <summary>
        /// All operations tracked by this instance will need to have
        ///  - the same timeout length timeoutMs
        ///  - the same timeout callback timeoutCallback
        /// </summary>
        /// <param name="timeoutCallback"> 
        /// This callback will be called when OperationTimeoutTracker detects operations that exceeded timeouts.
        /// </param> 
        public OperationTimeoutTracker(int timeoutMs, Action<TOperationToken[]> timeoutCallback)
        {
            _timeoutMs = timeoutMs;
            _timeoutCallback = timeoutCallback;
            _currentTimeoutTracker = new TimeoutTracker(_timeoutMs, _timeoutCallback);
        }

        /// <param name="t"> A token identifying an operation being tracked </param>
        /// <returns> Returns a disposable object which (when disposed) will release the token from being tracked </returns>
        public IDisposable StartTrackingOperation(TOperationToken t)
        {
            if (DateTime.Now > _currentTimeoutTracker.StopUsingDueTime)
            {
                // Create a new tracker
                var currentTimeoutTracker = _currentTimeoutTracker;
                var newTimeoutTracker = new TimeoutTracker(_timeoutMs, _timeoutCallback);
                // and make sure we only use one new tracker
                if (Interlocked.CompareExchange(ref _currentTimeoutTracker, newTimeoutTracker, currentTimeoutTracker) != currentTimeoutTracker)
                {
                    newTimeoutTracker.Dispose();
                }
                
                // Don't touch the old currentTimeoutTracker which still tracks its stuff for completion
            }
            return _currentTimeoutTracker.TrackOperationTimeout(t);
        }

        private class TimeoutTracker : IDisposable
        {
            private ITrackingList<TOperationToken> _list = new CircularArrayList<TOperationToken>();
            private Timer _timer;
            private int _disposed;
            private Action<TOperationToken[]> _timeoutCallback;

            public TimeoutTracker(int timeoutMs, Action<TOperationToken[]> timeoutCallback)
            {
                _timeoutCallback = timeoutCallback;
                // stop using this tracker in timeoutMs
                StopUsingDueTime = DateTime.Now.AddMilliseconds(timeoutMs);

                // fire the timer in 2 * timeoutMs to be sure to exceed timeouts for the most recent operations
                _timer = new Timer(NotifyTimeoutCallbackOnce, null, 2 * timeoutMs, Timeout.Infinite);
            }

            public DateTime StopUsingDueTime { get; private set; }

            public void Dispose()
            {
                if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
                {
                    _timer.Dispose();
                }
            }

            // Return a disposable token to be able to remove finished operation from the tracking list
            public IDisposable TrackOperationTimeout(TOperationToken r)
            {
                int listIndex = _list.Add(r);
                return new StopTrackingToken(_list, listIndex);
            }

            private void NotifyTimeoutCallbackOnce(Object _)
            {
                _timeoutCallback(_list.ToArray());
                Dispose();
            }
        }

        private class StopTrackingToken : IDisposable
        {
            private ITrackingList<TOperationToken> _list;
            private int _index;
            public StopTrackingToken(ITrackingList<TOperationToken> list, int index)
            {
                _list = list;
                _index = index;
            }
            public void Dispose()
            {
                _list.Remove(_index);
            }
        }

        private interface ITrackingList<T>
        {
            int Add(T t);
            void Remove(int i);
            T[] ToArray();
        }

        /// <summary>
        /// This is a specialized collection to handle fast rate of adding/removing elements.
        /// To avoid serializing threads its Add and Remove operations are lock free.
        /// </summary>
        private class CircularArrayList<T> : ITrackingList<T>
            where T : class
        {
            private const int Capacity = 1000;
            private const int MaxCapacity = 8000;
            // Small initial capacity may significantly impact its performance when going over its size.
            // To deal with this it has a self-adjusting capacity multiplier for future collection instances.
            private static int s_capacityMultiplier = 1;
            private int _sizeAdjustedOnce = 0;
            private int _currentSize = Capacity * s_capacityMultiplier;
            private T[] _list = new T[Capacity * s_capacityMultiplier];
            private long _currIdx = 0;

            public int Add(T t)
            {
                int idx = 0;
                long startIdx = _currIdx;
                do
                {
                    // There are several different strategies for choosing the initial index in the array
                    // Depending on size, NUMA architecture, and cuncurrency some may work better than others...
                    idx = (int)(Interlocked.Increment(ref _currIdx) % _currentSize);

                    // A context switch can cause a large difference between _currIdx and startIdx so this check is not reliable
                    // but we still treat it as a sign that we need to increase the array capacity next time
                    if (idx > startIdx + _currentSize)
                    {
                        if (Interlocked.CompareExchange(ref _sizeAdjustedOnce, 1, 0) == 0)
                        {
                            if (Capacity * s_capacityMultiplier < MaxCapacity)
                            {
                                s_capacityMultiplier *= 2;
                            }
                        }
                    }
                }
                while (_list[idx] != null);

                // If a thread makes a full loop ahead of the other one then both threads get the same index
                // so we use interlocked compare exchange to take care of this case
                if (Interlocked.CompareExchange(ref _list[idx], t, null) == null)
                {
                    return idx;
                }
                // This thread will have to try again
                return Add(t);
            }

            public void Remove(int index)
            {
                Debug.Assert(_list[index] != null, "Missing element");
                _list[index] = null;
            }

            public T[] ToArray()
            {
                return _list.Where(_ => _ != null).ToArray();
            }
        }
    }
}