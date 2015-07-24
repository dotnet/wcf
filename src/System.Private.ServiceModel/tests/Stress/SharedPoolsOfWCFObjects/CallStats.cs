// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using System.Diagnostics;

namespace WCFClientStressTests
{
    /// <summary>
    /// This class provides a few Call*AndRecordStats() utility methods which will store and aggregate the timing of the delegates they invoke 
    /// 
    /// Note: all Call*AndRecordStats() methods catch all exceptions coming from the delegates
    /// However we don't want to miss the case when an unexpected exception happens for the calls that should never have exceptions
    /// </summary>
    public class CallStats
    {
        public CallTimingStatsCollector SunnyDay { get; set; }
        public CallTimingStatsCollector RainyDay { get; set; }

        public static int SunnyDaySamples = 1000000;
        public static int RainyDaySamples = 10000;

        public CallStats()
            : this(SunnyDaySamples, RainyDaySamples, CallTimingStatsCollector.CalculateSomePercentiles, CallTimingStatsCollector.CalculateSomePercentiles)
        {
        }

        public CallStats(int samples, int errorSamples)
            : this(samples, errorSamples, CallTimingStatsCollector.CalculateSomePercentiles, CallTimingStatsCollector.CalculateSomePercentiles)
        {
        }

        public CallStats(int samples, int errorSamples, Func<long[], TimingPercentileStats> statsCalculator, Func<long[], TimingPercentileStats> errorStatsCalculator)
        {
            SunnyDay = new CallTimingStatsCollector(samples, statsCalculator);
            RainyDay = new CallTimingStatsCollector(errorSamples, errorStatsCalculator);
        }

        public void CallActionAndRecordStats(Action action)
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
            }
        }

        public T CallFuncAndRecordStats<T>(Func<T> func)
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
            }
            return result;
        }

        public T CallFuncAndRecordStats<T, P>(Func<P, T> func, P param)
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
            }
            return result;
        }

        public async Task CallAsyncFuncAndRecordStatsAsync(Func<Task> func)
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
            }
        }

        public async Task CallAsyncFuncAndRecordStatsAsync<P>(Func<P, Task> func, P param)
        {
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Restart();
                await func(param);
                SunnyDay.AddCallTiming(sw.ElapsedTicks);
            }
            catch (Exception e)
            {
                RainyDay.AddCallTiming(sw.ElapsedTicks);
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
                    stats.AvgTiming[i] = timingArr.OrderBy(t1 => t1).Take(lowestXPctNum).Sum(t2 => t2) / lowestXPctNum;
                }
                else
                {
                    int topXPctNum = (timingArr.Length * stats.Centile[i]) / 100;
                    stats.AvgTiming[i] = timingArr.OrderByDescending(t1 => t1).Take(topXPctNum).Sum(t2 => t2) / topXPctNum;
                }

                //Console.WriteLine(String.Format("Percentile {0}, time {1}", stats.Centile[i], stats.AvgTiming[i]));
            }

            return stats;
        }
    }
}
