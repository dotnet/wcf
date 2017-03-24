// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

namespace SharedPoolsOfWCFObjects
{
    /// <summary>
    /// This class provides a few Call*AndRecordStats() utility methods which will store and aggregate the timing of the delegates they invoke 
    /// 
    /// Note that there are 2 separate mechanisms to control how the exceptions are handled by these utility methods.
    /// The first one is the boolean "hideExceptions" parameter to all Call*AndRecordStats methods where 'true' means completely ignore exceptions.
    /// This is used by the scenarios where the exceptions are expected and should be ignored no matter what kind of test is running.
    /// 
    /// The second one is the Func<Exception, bool> exceptionHandler parameter which is typically controlled by the caller via generic TestParams.
    /// This allows different level of tolerance for different test runs (e.g. no unexpected exceptions for Perf and some tolerance for Stress).
    /// The boolean return value signifies if the exception is considered to be "handled" - e.g. no need to propagate the exception to the caller.
    /// </summary>
    public class CallStats
    {
        public Func<Exception, bool> _exceptionHandler;
        public CallTimingStatsCollector SunnyDay { get; set; }
        public CallTimingStatsCollector RainyDay { get; set; }

        public CallStats(int samples, int errorSamples, Func<Exception, bool> exceptionHandler)
            : this(samples, errorSamples, exceptionHandler, CallTimingStatsCollector.CalculateSomePercentiles, CallTimingStatsCollector.CalculateSomePercentiles)
        {
        }

        public CallStats(int samples, int errorSamples, Func<Exception, bool> exceptionHandler, Func<long[], TimingPercentileStats> statsCalculator, Func<long[], TimingPercentileStats> errorStatsCalculator)
        {
            SunnyDay = new CallTimingStatsCollector(samples, statsCalculator);
            RainyDay = new CallTimingStatsCollector(errorSamples, errorStatsCalculator);
            _exceptionHandler = exceptionHandler;
        }

        public void CallActionAndRecordStats(Action action, bool hideExceptions)
        {
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Restart();
                action();
                SunnyDay.AddCallTiming(sw.ElapsedTicks);
            }
            catch (Exception e)
            {
                RainyDay.AddCallTiming(sw.ElapsedTicks);
                if (!hideExceptions)
                {
                    if (!_exceptionHandler(e))
                    {
                        throw;
                    }
                }
            }
        }

        public T CallFuncAndRecordStats<T>(Func<T> func, bool hideExceptions = false)
        {
            T result = default(T);
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Restart();
                result = func();
                SunnyDay.AddCallTiming(sw.ElapsedTicks);
            }
            catch (Exception e)
            {
                RainyDay.AddCallTiming(sw.ElapsedTicks);
                if (!hideExceptions)
                {
                    if (!_exceptionHandler(e))
                    {
                        throw;
                    }
                }
            }
            return result;
        }

        public T CallFuncAndRecordStats<T, P>(Func<P, T> func, P param, bool hideExceptions = false)
        {
            T result = default(T);
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Restart();
                result = func(param);
                SunnyDay.AddCallTiming(sw.ElapsedTicks);
            }
            catch (Exception e)
            {
                RainyDay.AddCallTiming(sw.ElapsedTicks);
                if (!hideExceptions)
                {
                    if (!_exceptionHandler(e))
                    {
                        throw;
                    }
                }
            }
            return result;
        }

        public async Task CallAsyncFuncAndRecordStatsAsync(Func<Task> func, bool hideExceptions)
        {
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Restart();
                await func();
                SunnyDay.AddCallTiming(sw.ElapsedTicks);
            }
            catch (Exception e)
            {
                RainyDay.AddCallTiming(sw.ElapsedTicks);
                if (!hideExceptions)
                {
                    if (!_exceptionHandler(e))
                    {
                        throw;
                    }
                }
            }
        }

        public async Task<R> CallAsyncFuncAndRecordStatsAsync<R>(Func<Task<R>> func, bool hideExceptions)
        {
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Restart();
                var result = await func();
                SunnyDay.AddCallTiming(sw.ElapsedTicks);
                return result;
            }
            catch (Exception e)
            {
                RainyDay.AddCallTiming(sw.ElapsedTicks);
                if (!hideExceptions)
                {
                    if (!_exceptionHandler(e))
                    {
                        throw;
                    }
                }
                return default(R);
            }
        }
    }

    public class TimingPercentileStats
    {
        public DateTime Time;       // Time when the stats were calculated
        public int[] Centile;       // negative % means take bottom part, positive % means take top part
        public long[] AvgTiming;    // within this time
    }

    public class CallTimingStatsCollector
    {
        private int _samples;
        private int _numCalls;
        private long[] _timings;
        private Object _lock = new Object();
        private Func<long[], TimingPercentileStats> _statsCalculator;
        public ConcurrentQueue<TimingPercentileStats> PercentileStats { get; set; }

        public CallTimingStatsCollector(int samples, Func<long[], TimingPercentileStats> statsCalculator)
        {
            _samples = samples;
            _timings = new long[_samples * 2];
            _statsCalculator = statsCalculator;
            PercentileStats = new ConcurrentQueue<TimingPercentileStats>();
        }

        // For the most part lock-free and isn't particularly thread safe
        //
        // However here we use it without any additional synchronization 
        // since we use a large number of samples and 
        public void AddCallTiming(long ticks)
        {
            int index = Interlocked.Increment(ref _numCalls) % (_samples * 2);
            _timings[index] = ticks;

            if (index % _samples == 0)
            {
                // if we get a context switch here then the data in _timings may get overwritten by other threads calling AddCallTiming()
                // however no data will be lost if there were fewer than _samples calls while we make a copy of our half of _timings
                // the copy may land into LOH - before trying to optimize it we'll need to optimize _statsCalculator as well
                long[] copy = new long[_samples];
                int timingsIndex = index == 0 ? _samples + 1 : 1;
                // this almost useless lock will only stop the first thread that wrapped up _samples calls while we copy the data
                lock (_lock)
                {
                    for (int i = 0; i < _samples; i++)
                    {
                        int ti = (timingsIndex + i) % (_samples * 2);
                        copy[i] = _timings[ti];
                    }
                }
                //calculate percentiles
                PercentileStats.Enqueue(_statsCalculator(copy));
            }
        }

        public long[] CurrentTimings
        {
            get
            {
                long[] copy = new long[_timings.Length];
                for (int i = 0; i < _timings.Length; i++)
                {
                    copy[i] = _timings[i];
                }
                return copy;
            }
        }

        public static TimingPercentileStats CalculateSomePercentiles(long[] timingArr)
        {
            var stats = new TimingPercentileStats()
            {
                Time = DateTime.Now,
                Centile = new int[] { -50, 1 },
                AvgTiming = new long[2]
            };

            for (int i = 0; i < stats.Centile.Length; i++)
            {
                if (stats.Centile[i] < 0)
                {
                    int lowestXPctNum = (timingArr.Length * -1 * stats.Centile[i]) / 100;
                    if (lowestXPctNum != 0)
                    {
                        stats.AvgTiming[i] = timingArr.OrderBy(t1 => t1).Take(lowestXPctNum).Sum(t2 => t2) / lowestXPctNum;
                    }
                }
                else
                {
                    int topXPctNum = (timingArr.Length * stats.Centile[i]) / 100;
                    if (topXPctNum != 0)
                    {
                        stats.AvgTiming[i] = timingArr.OrderByDescending(t1 => t1).Take(topXPctNum).Sum(t2 => t2) / topXPctNum;
                    }
                }
            }

            return stats;
        }
    }
}
