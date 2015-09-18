// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using WcfService1;

namespace SharedPoolsOfWCFObjects
{
    #region Actual tests
    // A full cycle of creating a pool of channel factories, using each factory to create
    // a pool of channels, using all channels once and then closing all of them
    public static class CreateAndCloseFactoryAndChannelFullCycleTest<ChannelType, TestTemplate>
        where ChannelType : class
        where TestTemplate : ITestTemplate<ChannelType>, new()
    {
        private static CallStats s_CreateChannelStats = new CallStats();
        private static CallStats s_CallChannelStats = new CallStats();
        private static CallStats s_CloseChannelStats = new CallStats();
        private static CallStats s_CloseFactoryStats = new CallStats();

        private static ITestTemplate<ChannelType> s_test;

        static CreateAndCloseFactoryAndChannelFullCycleTest()
        {
            s_test = new TestTemplate();
        }
        public static void CreateFactoriesAndChannelsUseAllOnceCloseAll()
        {
            using (var theOneTimeThing = new PoolOfThings<FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, ChannelType>>(
                    maxSize: 3, // # of pooled FactoryAndPoolOfItsObjects
                    createInstance: () => new FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, ChannelType>(
                        createFactoryInstance: () =>
                            s_test.CreateChannelFactory(),
                        destroyFactoryInstance: (chf) =>
                            s_CloseFactoryStats.CallActionAndRecordStats(() => s_test.CloseFactory(chf)),
                        maxPooledObjects: 3, // # of pooled channels within each pooled FactoryAndPoolOfItsObjects
                        createObject: (chf) =>
                            s_CreateChannelStats.CallFuncAndRecordStats(func: () => s_test.CreateChannel(chf)),
                        destroyObject: (ch) =>
                            s_CloseChannelStats.CallActionAndRecordStats(() => s_test.CloseChannel(ch))
                        ),
                    destroyInstance: (_fapoic) => _fapoic.Destroy()))
            {
                foreach (var factoryAndPoolOfItsChannels in theOneTimeThing.GetAllPooledInstances())
                {
                    foreach (var channel in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                    {
                        s_CallChannelStats.CallActionAndRecordStats(() =>
                        {
                            s_test.UseChannel()(channel);
                        });
                    }
                }
            }
        }
    }

    // One of the most common scenario: create a pool of channel factories, use each factory to create a pool of channels,
    // and then use all pooled channels for the duration of the stress run
    public class PooledFactories<ChannelType, TestTemplate>
    where ChannelType : class
    where TestTemplate : ITestTemplate<ChannelType>, new()
    {
        private static CallStats s_CreateChannelStats = new CallStats();
        private static CallStats s_CallChannelStats = new CallStats();
        private static CallStats s_CloseChannelStats = new CallStats();
        private static CallStats s_CloseFactoryStats = new CallStats();

        private static ITestTemplate<ChannelType> s_test;
        private static PoolOfThings<ChannelFactory<ChannelType>> s_pooledChannelFactories;

        static PooledFactories()
        {
            s_test = new TestTemplate();
            s_pooledChannelFactories = StaticDisposablesHelper.AddDisposable(
                new PoolOfThings<ChannelFactory<ChannelType>>(
                    maxSize: 10,
                    createInstance: () =>
                        s_test.CreateChannelFactory(),
                    destroyInstance: (chf) =>
                         s_CloseFactoryStats.CallActionAndRecordStats(() => s_test.CloseFactory(chf))));
        }
        public static void CreateUseAndCloseChannels()
        {
            foreach (var factory in s_pooledChannelFactories.GetAllPooledInstances())
            {
                ChannelType channel = null;
                s_CreateChannelStats.CallActionAndRecordStats(() =>
                    channel = s_test.CreateChannel(factory));

                if (channel != null)
                {
                    s_CallChannelStats.CallActionAndRecordStats(() =>
                        s_test.UseChannel()(channel));

                    s_CloseChannelStats.CallActionAndRecordStats(() =>
                        s_test.CloseChannel(channel));
                }
            }
        }
    }

    public class PooledFactoriesAndChannels<ChannelType, TestTemplate>
        where ChannelType : class
        where TestTemplate : ITestTemplate<ChannelType>, new()
    {
        private static CallStats s_CreateChannelStats = new CallStats();
        private static CallStats s_CallChannelStats = new CallStats();
        private static CallStats s_CloseChannelStats = new CallStats();
        private static CallStats s_CloseFactoryStats = new CallStats();
        private static ITestTemplate<ChannelType> s_test;

        private static PoolOfThings<
            FactoryAndPoolOfItsObjects<
                ChannelFactory<ChannelType>,
                ChannelType>
            > s_pooledFactoriesAndChannels;

        static PooledFactoriesAndChannels()
        {
            s_test = new TestTemplate();
            s_pooledFactoriesAndChannels = StaticDisposablesHelper.AddDisposable(
                new PoolOfThings<FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, ChannelType>>(
                    maxSize: 3, // # of pooled FactoryAndPoolOfItsObjects
                    createInstance: () => new FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, ChannelType>(
                        createFactoryInstance:
                            s_test.CreateChannelFactory,
                        destroyFactoryInstance: (chf) =>
                            s_CloseFactoryStats.CallActionAndRecordStats(() => s_test.CloseFactory(chf)),
                        maxPooledObjects: 3, // # of pooled channels within each pooled FactoryAndPoolOfItsObjects
                        createObject: (chf) =>
                            s_CreateChannelStats.CallFuncAndRecordStats(s_test.CreateChannel, chf),
                        destroyObject: (ch) =>
                            s_CloseChannelStats.CallActionAndRecordStats(() => s_test.CloseChannel(ch))
                        ),
                    destroyInstance: (_fapoic) => _fapoic.Destroy()));
        }

        public static void UseChannelsInPooledFactoriesAndChannels()
        {
            foreach (var factoryAndPoolOfItsChannels in s_pooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channel in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    s_CallChannelStats.CallActionAndRecordStats(() =>
                    {
                        s_test.UseChannel()(channel);
                    });
                }
            }
        }
    }

    public class RecyclablePooledFactories<ChannelType, TestTemplate>
        where ChannelType : class
        where TestTemplate : ITestTemplate<ChannelType>, new()
    {
        private static int s_iteration = 0;
        private static CallStats s_CreateChannelStats = new CallStats();
        private static CallStats s_CallChannelStats = new CallStats();
        private static CallStats s_CloseChannelStats = new CallStats();
        private static CallStats s_CloseFactoryStats = new CallStats();
        private static ITestTemplate<ChannelType> s_test;
        private static PoolOfThings<ChannelFactory<ChannelType>> s_recyclablePooledChannelFactories;

        static RecyclablePooledFactories()
        {
            s_test = new TestTemplate();
            s_recyclablePooledChannelFactories = StaticDisposablesHelper.AddDisposable(
                new PoolOfThings<ChannelFactory<ChannelType>>(
                    maxSize: 10,
                    createInstance:
                        s_test.CreateChannelFactory,
                    destroyInstance: (chf) =>
                         s_CloseFactoryStats.CallActionAndRecordStats(() => s_test.CloseFactory(chf))));
        }
        public static void CreateUseAndCloseChannels()
        {
            foreach (var factory in s_recyclablePooledChannelFactories.GetAllPooledInstances())
            {
                ChannelType channel = null;
                s_CreateChannelStats.CallActionAndRecordStats(() =>
                       channel = s_test.CreateChannel(factory));

                if (channel != null)
                {
                    s_CallChannelStats.CallActionAndRecordStats(() =>
                            s_test.UseChannel()(channel));

                    s_CloseChannelStats.CallActionAndRecordStats(() =>
                    {
                        s_test.CloseChannel(channel);
                    });
                }
            }
        }

        public static void RecycleFactories()
        {
            s_recyclablePooledChannelFactories.DestoryAllPooledInstances();
        }
        public static void RunAllScenariosWithWeights(int createUseAndCloseChannelsWeight, int recycleFactoriesWeight)
        {
            int seed = Interlocked.Increment(ref s_iteration) % (createUseAndCloseChannelsWeight + recycleFactoriesWeight);
            if (seed < createUseAndCloseChannelsWeight)
            {
                CreateUseAndCloseChannels();
            }
            else if (seed < createUseAndCloseChannelsWeight + recycleFactoriesWeight)
            {
                RecycleFactories();
            }
        }
    }

    public static class RecyclablePooledFactoriesAndChannels<ChannelType, TestTemplate>
        where ChannelType : class
        where TestTemplate : ITestTemplate<ChannelType>, new()
    {
        private static int s_iteration = 0;
        private static CallStats s_CreateChannelStats = new CallStats();
        private static CallStats s_CallChannelStats = new CallStats();
        private static CallStats s_CloseChannelStats = new CallStats();
        private static CallStats s_CloseFactoryStats = new CallStats();

        private static ITestTemplate<ChannelType> s_test;

        private static PoolOfThings<FactoryAndPoolOfItsObjects<
                ChannelFactory<ChannelType>,
                ChannelType>
            > s_recyclablePooledFactoriesAndChannels;

        static RecyclablePooledFactoriesAndChannels()
        {
            s_test = new TestTemplate();

            s_recyclablePooledFactoriesAndChannels = StaticDisposablesHelper.AddDisposable(
                new PoolOfThings<FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, ChannelType>>(
                    maxSize: 3, // # of pooled FactoryAndPoolOfItsObjects
                    createInstance: () => new FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, ChannelType>(
                        createFactoryInstance: () =>
                            s_test.CreateChannelFactory(),
                        destroyFactoryInstance: (chf) =>
                            s_CloseFactoryStats.CallActionAndRecordStats(() => s_test.CloseFactory(chf)),
                        maxPooledObjects: 3, // # of pooled channels within each pooled FactoryAndPoolOfItsObjects
                        createObject: (chf) =>
                            s_CreateChannelStats.CallFuncAndRecordStats(func: s_test.CreateChannel, param: chf),
                        destroyObject: (ch) =>
                            s_CloseChannelStats.CallActionAndRecordStats(action: () => s_test.CloseChannel(ch))
                        ),
                    destroyInstance: (_fapoic) => _fapoic.Destroy()));
        }

        public static void RunAllScenariosWithWeights(int useWeight, int recycleChannelsWeight, int recycleFactoriesWeight)
        {
            int seed = Interlocked.Increment(ref s_iteration) % (useWeight + recycleChannelsWeight + recycleFactoriesWeight);
            if (seed < useWeight)
            {
                UsePooledChannels();
            }
            else if (seed < useWeight + recycleChannelsWeight)
            {
                RecyclePooledChannels();
            }
            else if (seed < useWeight + recycleChannelsWeight + recycleFactoriesWeight)
            {
                RecyclePooledFactories();
            }
        }

        public static void UsePooledChannels()
        {
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channel in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    s_CallChannelStats.CallActionAndRecordStats(() =>
                    {
                        s_test.UseChannel()(channel);
                    });
                }
            }
        }

        public static void RecyclePooledChannels()
        {
            Console.WriteLine("RecyclePooledChannels");
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                factoryAndPoolOfItsChannels.ObjectsPool.DestoryAllPooledInstances();
            }
        }

        public static void RecyclePooledFactories()
        {
            Console.WriteLine("RecyclePooledFactories");
            s_recyclablePooledFactoriesAndChannels.DestoryAllPooledInstances();
            // additional checks - move to DestoryAllPooledInstances itself
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                if (factoryAndPoolOfItsChannels.Factory.State == CommunicationState.Closed)
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
        }
    }

    public static class RecyclablePooledFactoriesAndChannels_OpenOnce<ChannelType, TestTemplate>
        where TestTemplate : ITestTemplate<ChannelType>, new()
    {
        private static int s_iteration = 0;
        private static CallStats s_CreateChannelStats;
        private static CallStats s_CallChannelStats;
        private static CallStats s_CloseChannelStats;
        private static CallStats s_CloseFactoryStats;

        private static ITestTemplate<ChannelType> s_test;

        private static PoolOfThings<FactoryAndPoolOfItsObjects<
                ChannelFactory<ChannelType>,
                OpenOnceChannelWrapper<ChannelType>>
            > s_recyclablePooledFactoriesAndChannels;

        static RecyclablePooledFactoriesAndChannels_OpenOnce()
        {
            // moving the following magic numbers to the test template interface will help each test to collect different type of stats
            s_CreateChannelStats = new CallStats(samples: 1000, errorSamples: 1000);
            s_CallChannelStats = new CallStats(samples: 1000000, errorSamples: 10000);
            s_CloseChannelStats = new CallStats(samples: 1000, errorSamples: 1000);
            s_CloseFactoryStats = new CallStats(samples: 1000, errorSamples: 1000);

            s_test = new TestTemplate();
            Console.WriteLine(s_test.GetType().ToString());

            // Perhaps a helper (extension?) method to hide new PoolOfThings would make things more readable
            s_recyclablePooledFactoriesAndChannels = StaticDisposablesHelper.AddDisposable(
                new PoolOfThings<FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, OpenOnceChannelWrapper<ChannelType>>>(
                    maxSize: 3, // # of pooled FactoryAndPoolOfItsObjects
                    createInstance: () => new FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, OpenOnceChannelWrapper<ChannelType>>(
                        createFactoryInstance: () =>
                            s_test.CreateChannelFactory(),
                        destroyFactoryInstance: (chf) =>
                            s_CloseFactoryStats.CallActionAndRecordStats(() => s_test.CloseFactory(chf)),
                        maxPooledObjects: 3, // # of pooled channels within each pooled FactoryAndPoolOfItsObjects
                        createObject: (chf) =>
                            s_CreateChannelStats.CallFuncAndRecordStats(func: () => new OpenOnceChannelWrapper<ChannelType>(s_test.CreateChannel(chf))),
                        destroyObject: (chWr) =>
                            s_CloseChannelStats.CallActionAndRecordStats(action: () => s_test.CloseChannel(chWr.Channel))
                        ),
                    destroyInstance: (_fapoic) => _fapoic.Destroy()));
        }

        public static void RunAllScenariosWithWeights(int useWeight, int recycleChannelsWeight, int recycleFactoriesWeight)
        {
            int seed = new Random(Interlocked.Increment(ref s_iteration)).Next(useWeight + recycleChannelsWeight + recycleFactoriesWeight);
            if (seed < useWeight)
            {
                UsePooledChannels();
            }
            else if (seed < useWeight + recycleChannelsWeight)
            {
                RecyclePooledChannels();
            }
            else if (seed < useWeight + recycleChannelsWeight + recycleFactoriesWeight)
            {
                RecyclePooledFactories();
            }

            // move this outside of the test - an external code should query the test for its call stats
            if (Interlocked.Increment(ref s_iteration) % 1000000 == 0)
            {
                var sb = new StringBuilder();
                sb.AppendLine("CreateChannelStats: ");
                PrintStats(s_CreateChannelStats, sb);
                sb.AppendLine();

                sb.AppendLine("CallChannelStats: ");
                PrintStats(s_CallChannelStats, sb);
                sb.AppendLine();

                sb.AppendLine("CloseChannelStats: ");
                PrintStats(s_CloseChannelStats, sb);
                sb.AppendLine();

                sb.AppendLine("CloseFactoryStats: ");
                PrintStats(s_CloseFactoryStats, sb);
                sb.AppendLine();

                Console.WriteLine(sb.ToString());
            }
        }

        // move this outside of the test - an external code should query the test for its call stats
        private static void PrintStats(CallStats stats, StringBuilder sb)
        {
            sb.AppendLine("Sunny:");
            AppendStats(stats.SunnyDay, sb);
            sb.AppendLine("Rainy:");
            AppendStats(stats.RainyDay, sb);
        }

        // move this outside of the test - an external code should query the test for its call stats
        private static void AppendStats(CallTimingStatsCollector tstats, StringBuilder sb)
        {
            foreach (var stat in tstats.PercentileStats)
            {
                sb.AppendLine("Time: " + stat.Time);
                for (int i = 0; i < stat.Centile.Length; i++)
                {
                    sb.AppendLine(stat.Centile[i] + ": " + stat.AvgTiming[i]);
                }
            }
        }

        public static void UsePooledChannels()
        {
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channelWrapper in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    s_CallChannelStats.CallActionAndRecordStats(() =>
                    {
                        channelWrapper.OpenChannelOnce();
                        s_test.UseChannel()(channelWrapper.Channel);
                    });
                }
            }
        }

        public static void AbortPooledChannels()
        {
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channelWrapper in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    s_CallChannelStats.CallActionAndRecordStats(() =>
                    {
                        var co = (ICommunicationObject)channelWrapper.Channel;
                        co.Abort();
                    });
                }
            }
        }


        public static void RecyclePooledChannels()
        {
            Console.WriteLine("RecyclePooledChannels");
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                factoryAndPoolOfItsChannels.ObjectsPool.DestoryAllPooledInstances();
            }
        }

        public static void RecyclePooledFactories()
        {
            Console.WriteLine("RecyclePooledFactories");
            s_recyclablePooledFactoriesAndChannels.DestoryAllPooledInstances();
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                if (factoryAndPoolOfItsChannels.Factory.State == CommunicationState.Closed)
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
        }
    }

    public class RecyclablePooledFactoriesAndChannelsAsync_OpenOnce<ChannelType, TestTemplate>
        where ChannelType : class
        where TestTemplate : ITestTemplate<ChannelType>, new()
    {
        private static int s_iteration = 0;
        private static CallStats s_CreateChannelStats = new CallStats();
        private static CallStats s_CallChannelStats = new CallStats();
        private static CallStats s_CloseChannelStats = new CallStats();
        private static CallStats s_CloseFactoryStats = new CallStats();

        private static ITestTemplate<ChannelType> s_test;

        private static PoolOfAsyncThings<
            FactoryAndPoolOfItsAsyncObjects<
                ChannelFactory<ChannelType>,
                OpenAsyncOnceChannelWrapper<ChannelType>>
            > s_recyclablePooledFactoriesAndChannels;

        static RecyclablePooledFactoriesAndChannelsAsync_OpenOnce()
        {
            s_test = new TestTemplate();
            s_recyclablePooledFactoriesAndChannels = StaticDisposablesHelper.AddDisposable(
                new PoolOfAsyncThings<
                    FactoryAndPoolOfItsAsyncObjects<ChannelFactory<ChannelType>, OpenAsyncOnceChannelWrapper<ChannelType>>>(
                        maxSize: 3, // # of pooled FactoryAndPoolOfItsChannels
                        createInstance: () => new FactoryAndPoolOfItsAsyncObjects<ChannelFactory<ChannelType>, OpenAsyncOnceChannelWrapper<ChannelType>>(
                            createFactoryInstance:
                                s_test.CreateChannelFactory,
                            destroyFactoryInstanceAsync: async (chf) =>
                                await s_CloseFactoryStats.CallAsyncFuncAndRecordStatsAsync(s_test.CloseFactoryAsync, chf),
                            maxPooledObjects: 3, // # of pooled channels within each pooled FactoryAndPoolOfItsAsyncObjects
                            createObject: (chf) =>
                                s_CreateChannelStats.CallFuncAndRecordStats(func: () => new OpenAsyncOnceChannelWrapper<ChannelType>(s_test.CreateChannel(chf))),
                            destroyObjectAsync: async (chWr) =>
                                await s_CloseChannelStats.CallAsyncFuncAndRecordStatsAsync(func: () => s_test.CloseChannelAsync(chWr.Channel))
                        ),
                    destroyInstanceAsync: async (_fapoic) => await _fapoic.DestroyAsync()));
        }

        public static async Task RunAllScenariosWithWeightsAsync(int useWeight, int recycleChannelsWeight, int recycleFactoriesWeight)
        {
            int seed = new Random(Interlocked.Increment(ref s_iteration)).Next(useWeight + recycleChannelsWeight + recycleFactoriesWeight);
            //int seed = Interlocked.Increment(ref s_iteration) % (useWeight + recycleChannelsWeight + recycleFactoriesWeight);
            if (seed < useWeight)
            {
                await UsePooledChannelsAsync();
            }
            else if (seed < useWeight + recycleChannelsWeight)
            {
                await RecyclePooledChannelsAsync();
            }
            else if (seed < useWeight + recycleChannelsWeight + recycleFactoriesWeight)
            {
                await RecyclePooledFactoriesAsync();
            }
        }

        public static async Task UsePooledChannelsAsync()
        {
            var allTasks = new List<Task>();
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channelWrapper in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    allTasks.Add(s_CallChannelStats.CallAsyncFuncAndRecordStatsAsync(async () =>
                    {
                        if (channelWrapper.Channel != null)
                        {
                            await channelWrapper.OpenChannelOnceAsync();
                            await s_test.UseAsyncChannel()(channelWrapper.Channel);
                            //Console.WriteLine(s);
                        }
                    }));
                }
            }
            await Task.WhenAll(allTasks);
        }

        public static async Task RecyclePooledChannelsAsync()
        {
            Console.WriteLine("RecyclePooledChannels");
            var allTasks = new List<Task>();
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                allTasks.Add(factoryAndPoolOfItsChannels.ObjectsPool.DestoryAllPooledInstancesAsync());
            }
            await Task.WhenAll(allTasks);
        }

        public static async Task RecyclePooledFactoriesAsync()
        {
            Console.WriteLine("RecyclePooledFactories");
            await s_recyclablePooledFactoriesAndChannels.DestoryAllPooledInstancesAsync();
        }
    }
    #endregion


    // Helpers
    public class OpenOnceChannelWrapper<C>
    {
        private bool _openCalled = false;
        private Object _lock = new Object();

        public C Channel { get; set; }

        public OpenOnceChannelWrapper(C c)
        {
            Channel = c;
        }
        public void OpenChannelOnce()
        {
            lock (_lock)
            {
                if (!_openCalled)
                {
                    (Channel as ICommunicationObject).Open();
                    _openCalled = true;
                }
            }
        }
    }

    public class OpenAsyncOnceChannelWrapper<C>
    {
        private Task<Task> _openTask = null;

        public C Channel { get; set; }

        public OpenAsyncOnceChannelWrapper(C c)
        {
            Channel = c;
        }
        public async Task OpenChannelOnceAsync()
        {
            // Channel can be null if the factory is in faulted/closed state
            if (Channel != null)
            {
                var co = Channel as ICommunicationObject;
                // create a cold task
                var t = new Task<Task>(async () => await Task.Factory.FromAsync(co.BeginOpen, co.EndOpen, TaskCreationOptions.None));
                // see if we can save it to _openTask
                if (Interlocked.CompareExchange(ref _openTask, t, null) == null)
                {
                    // then we start
                    t.Start();
                    // and await its async action
                    await t.Unwrap();
                }
                else
                {
                    await _openTask.Unwrap();
                }
            }
        }
    }

    public class StaticDisposablesHelper
    {
        private static ConcurrentQueue<IDisposable> s_disposables = new ConcurrentQueue<IDisposable>();
        public static DT AddDisposable<DT>(DT t) where DT : IDisposable
        {
            s_disposables.Enqueue(t);
            return t;
        }

        public static void DisposeAll()
        {
            if (s_disposables != null)
            {
                IDisposable d = null;
                while (s_disposables.TryDequeue(out d))
                {
                    d.Dispose();
                }
                s_disposables = null;
            }
        }
    }
}
