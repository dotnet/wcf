// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using System.Diagnostics;
using WcfService1;


using System.Threading;

namespace SharedPoolsOfWCFObjects
{
    public class Program
    {
        private enum Program2Run { Stress, Perf };

        // Parameters and default values
        private Program2Run _paramProgram2Run = Program2Run.Stress;
        private int _paramStressLevel = DefaultStressLevel;
        private TimeSpan _paramStressRunDuration = TimeSpan.FromMinutes(1);
        private string _paramHostName = "localhost";
        private TestBinding _paramBinding = TestBinding.Http;
        private bool _paramUseAsync = true;
        private string _paramTestToRun = "HelloWorld";

        private const int DefaultStressLevel = 10;
        private const long DefaultStressIterations = 1024L * 1024L * 1024L * 1024L;
        // Sometimes it is beneficial to execute stress tests a certain number of iterations rather than run for a certain period of time.
        // The default number of iterations is an arbitrary large number that would leave this type of stress running for a very long time.
        private long _paramIterations = DefaultStressIterations;

        public static void Main(string[] args)
        {
            var test = new Program();
            test.ProcessRunOptions(args);
            if (test._paramProgram2Run == Program2Run.Stress)
            {
                test.RunStress();
            }
            else
            {
                test.RunPerf();
            }
        }

        private bool ProcessRunOptions(string[] args)
        {
            //var paramHostName = Environment.GetEnvironmentVariable("HostName");
            //if (!String.IsNullOrEmpty(paramHostName))
            //{
            //    _paramHostName = paramHostName;
            //    Console.WriteLine("HostName: " + _paramHostName);
            //}

            // Stress also needs control over the list of scenarios we want to run [RecycleFactories :True|False] [RecycleChannels:True|False] etc
            // For now we'll run all stress scenarios
            Console.WriteLine("[Program2Run:Stress|Perf] [StressRunDuration:minutes] [Binding:Http|NetTcp|NetHttpBinding] [Async:true|false] [Test:HelloWorld|Streaming|Duplex|DuplexStreaming]");

            foreach (string s in args)
            {
                Console.WriteLine(s);
                string[] p = s.Split(new char[] { ':' });
                if (p.Length != 2)
                {
                    continue;
                }

                switch (p[0])
                {
                    case "Program2Run":
                        if (!Enum.TryParse(p[1], out _paramProgram2Run))
                        {
                            Console.WriteLine("wrong argument: " + s);
                            return false;
                        }
                        break;
                    case "StressRunDuration":
                        int minutes = 0;
                        if (!Int32.TryParse(p[1], out minutes))
                        {
                            Console.WriteLine("wrong argument: " + s);
                            return false;
                        }
                        _paramStressRunDuration = TimeSpan.FromMinutes(minutes);
                        break;
                    case "StressLevel":
                        int stressLevel = 0;
                        if (!Int32.TryParse(p[1], out stressLevel))
                        {
                            Console.WriteLine("wrong argument: " + s);
                            return false;
                        }
                        _paramStressLevel = stressLevel;
                        break;
                    case "Binding":
                        if (!Enum.TryParse<TestBinding>(p[1], out _paramBinding))
                        {
                            Console.WriteLine("wrong argument: " + s);
                            return false;
                        }
                        break;
                    case "Async":
                        if (!Boolean.TryParse(p[1], out _paramUseAsync))
                        {
                            Console.WriteLine("wrong argument: " + s);
                            return false;
                        }
                        break;
                    case "Test":
                        _paramTestToRun = p[1];
                        break;
                    default:
                        Console.WriteLine("wrong argument: " + s);
                        return false;
                }
            }

            // Set binding once
            TestHelpers.SetHostAndProtocol(_paramBinding, hostName: _paramHostName, appName: "WcfService1");

            return true;
        }

        #region Stress
        private void RunStress()
        {
            RunStressImpl();

            Console.WriteLine("Done. Press Enter to GC.");
            Console.ReadLine();
            GC.Collect();
            Console.WriteLine("After GC");
            Console.ReadLine();
        }
        private void RunStressImpl()
        {
            switch (_paramTestToRun)
            {
                case "HelloWorld":
                    if (_paramUseAsync)
                    {
                        DoTheRunAsync(RunAllStressTestsAsync<IService1, HelloWorldTest<CommonStressTestParams>, CommonStressTestParams>);
                    }
                    else
                    {
                        DoTheRun(RunAllStressTests<IService1, HelloWorldTest<CommonStressTestParams>, CommonStressTestParams>);
                    }
                    break;
                case "Streaming":
                    if (_paramUseAsync)
                    {
                        DoTheRunAsync(RunAllStressTestsAsync<IStreamingService, StreamingTest<IStreamingService, StreamingStressTestParams>, StreamingStressTestParams>);
                    }
                    else
                    {
                        DoTheRun(RunAllStressTests<IStreamingService, StreamingTest<IStreamingService, StreamingStressTestParams>, StreamingStressTestParams>);
                    }
                    break;
                case "Duplex":
                    if (_paramUseAsync)
                    {
                        DoTheRunAsync(RunAllStressTestsAsync<IDuplexService, DuplexTest<DuplexStressTestParams>, DuplexStressTestParams>);
                    }
                    else
                    {
                        DoTheRun(RunAllStressTests<IDuplexService, DuplexTest<DuplexStressTestParams>, DuplexStressTestParams>);
                    }
                    break;
                case "DuplexStreaming":
                    if (_paramUseAsync)
                    {
                        DoTheRunAsync(RunAllStressTestsAsync<IDuplexStreamingService, DuplexStreamingTest<IDuplexStreamingService, DuplexStreamingStressTestParams>, DuplexStreamingStressTestParams>);
                    }
                    else
                    {
                        DoTheRun(RunAllStressTests<IDuplexStreamingService, DuplexStreamingTest<IDuplexStreamingService, DuplexStreamingStressTestParams>, DuplexStreamingStressTestParams>);
                    }
                    break;
                default:
                    Console.WriteLine("wrong argument: " + _paramTestToRun);
                    return;
            }
        }

        private void DoTheRun(Action test)
        {
            DoTheRunAsync(() => { test(); return Task.FromResult(true); });
        }

        private void DoTheRunAsync(Func<Task> testAsync)
        {
            var cts = new CancellationTokenSource(_paramStressRunDuration);
            Console.WriteLine("Start");

            Task[] allTasks = new Task[_paramStressLevel];
            for (int t = 0; t < allTasks.Length; t++)
            {
                int tt = t;
                allTasks[t] = Task.Run(async () =>
                {
                    var ttt = tt;
                    for (long i = 0; i < _paramIterations / allTasks.Length; i++)
                    {
                        try
                        {
                            await testAsync();
                        }
                        catch (ObjectDisposedException e)
                        {
                            TestUtils.ReportFailure(e.ToString());
                        }
                        catch (Exception e)
                        {
                            TestUtils.ReportFailure(e.ToString());
                            throw;
                        }

                        if (i % 100 == 0)
                        {
                            Console.WriteLine(DateTime.Now.ToString() + " " + ttt + " " + i);
                        }

                        if (cts.Token.IsCancellationRequested)
                        {
                            break;
                        }
                    }
                    Console.WriteLine(ttt + ": done");
                }, cts.Token);
            }
            Task.WaitAll(allTasks);

            Console.WriteLine("Dispose all");
            StaticDisposablesHelper.DisposeAll();
        }

        public static void RunAllStressTests<ChannelType, TestTemplate, TestParams>()
            where ChannelType : class
            where TestTemplate : ITestTemplate<ChannelType, TestParams>, IExceptionPolicy, new()
            where TestParams : IPoolTestParameter
        {
            CreateAndCloseFactoryAndChannelFullCycleTest<ChannelType, TestTemplate, TestParams>.CreateFactoriesAndChannelsUseAllOnceCloseAll();
            PooledFactories<ChannelType, TestTemplate, TestParams>.CreateUseAndCloseChannels();
            PooledFactoriesAndChannels<ChannelType, TestTemplate, TestParams>.UseChannelsInPooledFactoriesAndChannels();
            RecyclablePooledFactories<ChannelType, TestTemplate, TestParams>.RunAllScenariosWithWeights(100, 1);
            RecyclablePooledFactoriesAndChannels_OpenOnce<ChannelType, TestTemplate, TestParams>.RunAllScenariosWithWeights(100, 1, 1);
        }

        public static async Task RunAllStressTestsAsync<ChannelType, TestTemplate, TestParams>()
            where ChannelType : class
            where TestTemplate : ITestTemplate<ChannelType, TestParams>, IExceptionPolicy, new()
            where TestParams : IPoolTestParameter
        {
            await CreateAndCloseFactoryAndChannelFullCycleTestAsync<ChannelType, TestTemplate, TestParams>.CreateFactoriesAndChannelsUseAllOnceCloseAllAsync();
            await PooledFactoriesAsync<ChannelType, TestTemplate, TestParams>.CreateUseAndCloseChannelsAsync();
            await PooledFactoriesAndChannelsAsync<ChannelType, TestTemplate, TestParams>.UseChannelsInPooledFactoriesAndChannelsAsync();
            await RecyclablePooledFactoriesAsync<ChannelType, TestTemplate, TestParams>.RunAllScenariosWithWeightsAsync(100, 1);
            await RecyclablePooledFactoriesAndChannelsAsync_OpenOnce<ChannelType, TestTemplate, TestParams>.RunAllScenariosWithWeightsAsync(100, 1, 1);
        }
        #endregion

        #region Perf

        // Consider adding these as perf command line parameters as well.
        // These defaults seem to work well to produce stable results in the existing perf tests
        private const int MaxTasks = 25;
        private const int MaxIterations = 1000;
        private static readonly TimeSpan s_measurementDuration = TimeSpan.FromSeconds(10);

        public void RunPerf()
        {
            switch (_paramTestToRun)
            {
                case "HelloWorld":
                    if (_paramUseAsync)
                    {
                        RunFirstNIterationsAsync(MaxIterations, MaxTasks,
                            testAsync: RunStartupPerfTestsAsync<IService1, HelloWorldTest<CommonPerfStartupTestParams>, CommonPerfStartupTestParams>);
                        RunMaxThroughputAsync(duration: s_measurementDuration, maxTasks: MaxTasks,
                            testAsync: RunThroughputPerfTestsAsync<IService1, HelloWorldTest<CommonPerfThroughputTestParams>, CommonPerfThroughputTestParams>);
                    }
                    else
                    {
                        RunFirstNIterations(MaxIterations, MaxTasks,
                            test: RunStartupPerfTests<IService1, HelloWorldTest<CommonPerfStartupTestParams>, CommonPerfStartupTestParams>);
                        RunMaxThroughput(duration: s_measurementDuration, maxTasks: MaxTasks,
                            test: RunThroughputPerfTests<IService1, HelloWorldTest<CommonPerfThroughputTestParams>, CommonPerfThroughputTestParams>);
                    }
                    break;
                case "Streaming":
                    if (_paramUseAsync)
                    {
                        RunFirstNIterationsAsync(MaxIterations, MaxTasks,
                            testAsync: RunStartupPerfTestsAsync<IStreamingService, StreamingTest<IStreamingService, StreamingPerfStartupTestParams>, StreamingPerfStartupTestParams>);
                        RunMaxThroughputAsync(duration: s_measurementDuration, maxTasks: MaxTasks,
                            testAsync: RunThroughputPerfTestsAsync<IStreamingService, StreamingTest<IStreamingService, StreamingPerfThroughputTestParams>, StreamingPerfThroughputTestParams>);
                    }
                    else
                    {
                        RunFirstNIterations(MaxIterations, MaxTasks,
                           test: RunStartupPerfTests<IStreamingService, StreamingTest<IStreamingService, StreamingPerfStartupTestParams>, StreamingPerfStartupTestParams>);
                        RunMaxThroughput(duration: s_measurementDuration, maxTasks: MaxTasks,
                            test: RunThroughputPerfTests<IStreamingService, StreamingTest<IStreamingService, StreamingPerfThroughputTestParams>, StreamingPerfThroughputTestParams>);
                    }
                    break;
                case "Duplex":
                    if (_paramUseAsync)
                    {
                        RunFirstNIterationsAsync(MaxIterations, MaxTasks,
                            testAsync: RunStartupPerfTestsAsync<IDuplexService, DuplexTest<DuplexPerfStartupTestParams>, DuplexPerfStartupTestParams>);
                        RunMaxThroughputAsync(duration: s_measurementDuration, maxTasks: MaxTasks,
                            testAsync: RunThroughputPerfTestsAsync<IDuplexService, DuplexTest<DuplexPerfThroughputTestParams>, DuplexPerfThroughputTestParams>);
                    }
                    else
                    {
                        RunFirstNIterations(MaxIterations, MaxTasks,
                            test: RunStartupPerfTests<IDuplexService, DuplexTest<DuplexPerfStartupTestParams>, DuplexPerfStartupTestParams>);
                        RunMaxThroughput(duration: s_measurementDuration, maxTasks: MaxTasks,
                            test: RunThroughputPerfTests<IDuplexService, DuplexTest<DuplexPerfThroughputTestParams>, DuplexPerfThroughputTestParams>);
                    }
                    break;
                case "DuplexStreaming":
                    if (_paramUseAsync)
                    {
                        RunFirstNIterationsAsync(MaxIterations, MaxTasks,
                            testAsync: RunStartupPerfTestsAsync<IDuplexStreamingService, DuplexStreamingTest<IDuplexStreamingService, DuplexStreamingPerfStartupTestParams>, DuplexStreamingPerfStartupTestParams>);
                        RunMaxThroughputAsync(duration: s_measurementDuration, maxTasks: MaxTasks,
                            testAsync: RunThroughputPerfTestsAsync<IDuplexStreamingService, DuplexStreamingTest<IDuplexStreamingService, DuplexStreamingPerfThroughputTestParams>, DuplexStreamingPerfThroughputTestParams>);
                    }
                    else
                    {
                        RunFirstNIterations(MaxIterations, MaxTasks,
                            test: RunStartupPerfTests<IDuplexStreamingService, DuplexStreamingTest<IDuplexStreamingService, DuplexStreamingPerfStartupTestParams>, DuplexStreamingPerfStartupTestParams>);
                        RunMaxThroughput(duration: s_measurementDuration, maxTasks: MaxTasks,
                            test: RunThroughputPerfTests<IDuplexStreamingService, DuplexStreamingTest<IDuplexStreamingService, DuplexStreamingPerfThroughputTestParams>, DuplexStreamingPerfThroughputTestParams>);
                    }
                    break;
                default:
                    Console.WriteLine("wrong argument: " + _paramTestToRun);
                    return;
            }

            var sw = new Stopwatch();
            sw.Start();
            StaticDisposablesHelper.DisposeAll();
            Console.WriteLine("Dispose all done in " + sw.ElapsedMilliseconds);
        }

        public static void RunFirstNIterations(int iterations, int maxTasks, Action test)
        {
            RunFirstNIterationsAsyncImpl(iterations, maxTasks, () => { test(); return Task.FromResult(true); });
        }
        public static void RunFirstNIterationsAsync(int iterations, int maxTasks, Func<Task> testAsync)
        {
            RunFirstNIterationsAsyncImpl(iterations, maxTasks, testAsync);
        }
        public static void RunFirstNIterationsAsyncImpl(int iterations, int maxTasks, Func<Task> testAsync)
        {
            Task[] allTasks = new Task[maxTasks];

            Console.WriteLine("Start RunFirstNIterations with " + iterations + " iterations");
            var sw = new Stopwatch();
            sw.Start();

            for (int t = 0; t < allTasks.Length; t++)
            {
                int tt = t;
                allTasks[t] = Task.Run(async () =>
                {
                    var ttt = tt;
                    for (int i = 0; i < iterations / allTasks.Length; i++)
                    {
                        try
                        {
                            await testAsync();
                        }
                        catch (ObjectDisposedException e)
                        {
                            TestUtils.ReportFailure(e.ToString());
                        }
                        catch (Exception e)
                        {
                            TestUtils.ReportFailure(e.ToString());
                            throw;
                        }
#if DEBUG
                        if (i % 100 == 0)
                        {
                            Console.WriteLine(DateTime.Now.ToString() + " " + ttt + " " + i);
                        }
#endif
                    }
#if DEBUG
                    Console.WriteLine(ttt + ": done");
#endif
                });
            }
            Task.WaitAll(allTasks);
            sw.Stop();
            Console.WriteLine(MaxIterations + " are done in: " + sw.ElapsedMilliseconds);
        }


        private static long s_iterationsCompleted = 0;
        public static void RunMaxThroughput(TimeSpan duration, int maxTasks, Action test)
        {
            RunMaxThroughputAsyncImpl(duration, maxTasks, () => { test(); return Task.FromResult(true); });
            RunMaxThroughputReverseAsyncImpl(duration, maxTasks, () => { test(); return Task.FromResult(true); });
        }

        public static void RunMaxThroughputAsync(TimeSpan duration, int maxTasks, Func<Task> testAsync)
        {
            RunMaxThroughputAsyncImpl(duration, maxTasks, testAsync);
            RunMaxThroughputReverseAsyncImpl(duration, maxTasks, testAsync);
        }

        public static void RunMaxThroughputAsyncImpl(TimeSpan duration, int maxTasks, Func<Task> testAsync)
        {
            bool stopAllTasks = false;
            long iterationStartIterations, iterationEndIterations;
            DateTime iterationStartTime, iterationEndTime;
            double bestThroughput = 0;
            int bestThroughputTasks = 0;
            Task[] allTasks = new Task[maxTasks];

            Console.WriteLine("Determining maximum throughput. ");

            var sw = new Stopwatch();
            sw.Start();

            for (int t = 0; t < allTasks.Length; t++)
            {
                iterationStartIterations = s_iterationsCompleted;
                iterationStartTime = DateTime.Now;

                int tt = t;
                allTasks[t] = Task.Run(async () =>
                {
                    var ttt = tt;
                    for (long l = 0; l < 1000000000; l++)
                    {
                        try
                        {
                            await testAsync();
                            Interlocked.Increment(ref s_iterationsCompleted);
                        }
                        catch (ObjectDisposedException e)
                        {
                            TestUtils.ReportFailure(e.ToString());
                        }
                        catch (Exception e)
                        {
                            TestUtils.ReportFailure(e.ToString());
                            throw;
                        }
#if DEBUG
                        if (l % 1000 == 0)
                        {
                            Console.WriteLine(DateTime.Now.ToString() + " " + ttt + " " + l);
                        }
#endif
                        if (stopAllTasks)
                        {
                            break;
                        }
                    }
#if DEBUG
                    Console.WriteLine(ttt + ": done");
#endif
                });

                Task.Delay(duration).Wait();

                iterationEndTime = DateTime.Now;
                iterationEndIterations = s_iterationsCompleted;
                double throughtput = ((iterationEndIterations - iterationStartIterations) / ((iterationEndTime - iterationStartTime).TotalSeconds));
                Console.WriteLine(String.Format("!!!          Tasks {0} throughput Rq/s {1}", t + 1, throughtput));
                if (throughtput > bestThroughput)
                {
                    bestThroughput = throughtput;
                    bestThroughputTasks = t;
                }
            }

            stopAllTasks = true;
            Task.WaitAll(allTasks);
            sw.Stop();
            Console.WriteLine(s_iterationsCompleted + " are done in: " + sw.ElapsedMilliseconds);
            Console.WriteLine(String.Format("\r\n Best throughput {0} with {1} threads \r\n", bestThroughput, bestThroughputTasks + 1));
        }

        public static void RunMaxThroughputReverseAsyncImpl(TimeSpan duration, int maxTasks, Func<Task> testAsync)
        {
            bool stopAllTasks = false;
            long iterationStartIterations, iterationEndIterations;
            DateTime iterationStartTime, iterationEndTime;
            double bestThroughput = 0;
            int bestThroughputTasks = 0;

            Console.WriteLine("Determining maximum throughput. ");
            Task[] allTasks = new Task[maxTasks];
            CancellationTokenSource[] allCancellations = new CancellationTokenSource[maxTasks];

            var sw = new Stopwatch();
            sw.Start();
            for (int t = 0; t < maxTasks; t++)
            {
                int tt = t;
                allCancellations[t] = new CancellationTokenSource();
                allTasks[t] = Task.Run(async () =>
                {
                    var ttt = tt;
                    for (long l = 0; l < 1000000000; l++)
                    {
                        try
                        {
                            await testAsync();
                            Interlocked.Increment(ref s_iterationsCompleted);
                        }
                        catch (ObjectDisposedException e)
                        {
                            TestUtils.ReportFailure(e.ToString());
                        }
                        catch (Exception e)
                        {
                            TestUtils.ReportFailure(e.ToString());
                            throw;
                        }
#if DEBUG
                        if (l % 1000 == 0)
                        {
                            Console.WriteLine(DateTime.Now.ToString() + " " + ttt + " " + l);
                        }
#endif
                        if (allCancellations[ttt].IsCancellationRequested)
                        {
                            break;
                        }

                        if (stopAllTasks)
                        {
                            break;
                        }
                    }
#if DEBUG
                    Console.WriteLine(ttt + ": done");
#endif
                });
            }

            for (int t = maxTasks - 1; t >= 0; t--)
            {
                iterationStartIterations = s_iterationsCompleted;
                iterationStartTime = DateTime.Now;
                Task.Delay(duration).Wait();
                iterationEndTime = DateTime.Now;
                iterationEndIterations = s_iterationsCompleted;
                double throughtput = ((iterationEndIterations - iterationStartIterations) / ((iterationEndTime - iterationStartTime).TotalSeconds));
                Console.WriteLine(String.Format("!!!          Tasks {0} throughput Rq/s {1}", t + 1, throughtput));
                if (throughtput > bestThroughput)
                {
                    bestThroughput = throughtput;
                    bestThroughputTasks = t;
                }
#if DEBUG
                Console.WriteLine("Stopping task " + (t + 1));
#endif
                allCancellations[t].Cancel();
            }

            stopAllTasks = true;
            Task.WaitAll(allTasks);
            sw.Stop();
            Console.WriteLine(s_iterationsCompleted + " are done in: " + sw.ElapsedMilliseconds);
            Console.WriteLine(String.Format("\r\n Best throughput {0} with {1} threads \r\n", bestThroughput, bestThroughputTasks + 1));
        }


        // For the startup scenario we choose to run the full cycle of creating factory, creating channel, using them, and closing all
        public static void RunStartupPerfTests<ChannelType, TestTemplate, TestParams>()
            where ChannelType : class
            where TestTemplate : ITestTemplate<ChannelType, TestParams>, new()
            where TestParams : IPoolTestParameter
        {
            CreateAndCloseFactoryAndChannelFullCycleTest<ChannelType, TestTemplate, TestParams>.CreateFactoriesAndChannelsUseAllOnceCloseAll();
        }

        // For the throughput scenario we choose to run scenario where both channel factories and channels are pooled and never recycled
        public static void RunThroughputPerfTests<ChannelType, TestTemplate, TestParams>()
            where ChannelType : class
            where TestTemplate : ITestTemplate<ChannelType, TestParams>, new()
            where TestParams : IPoolTestParameter
        {
            PooledFactoriesAndChannels<ChannelType, TestTemplate, TestParams>.UseChannelsInPooledFactoriesAndChannels();
        }

        public static async Task RunStartupPerfTestsAsync<ChannelType, TestTemplate, TestParams>()
            where ChannelType : class
            where TestTemplate : ITestTemplate<ChannelType, TestParams>, new()
            where TestParams : IPoolTestParameter
        {
            await CreateAndCloseFactoryAndChannelFullCycleTestAsync<ChannelType, TestTemplate, TestParams>.CreateFactoriesAndChannelsUseAllOnceCloseAllAsync();
        }

        public static async Task RunThroughputPerfTestsAsync<ChannelType, TestTemplate, TestParams>()
            where ChannelType : class
            where TestTemplate : ITestTemplate<ChannelType, TestParams>, new()
            where TestParams : IPoolTestParameter
        {
            await PooledFactoriesAndChannelsAsync<ChannelType, TestTemplate, TestParams>.UseChannelsInPooledFactoriesAndChannelsAsync();
        }
        #endregion
    }
}
