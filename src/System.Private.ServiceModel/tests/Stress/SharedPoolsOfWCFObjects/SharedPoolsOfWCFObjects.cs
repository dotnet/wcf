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
    // Consider making classes non static and inherit RecyclablePooled* from Pooled*
    // While this will increase the code reuse it will likely require additional wrappers 
    // to be able to simply call methods without prior initialization steps
    public static class CreateAndCloseFactoryAndChannelFullCycleTest<ChannelType, TestTemplate, TestParams>
        where ChannelType : class
        where TestTemplate : ITestTemplate<ChannelType, TestParams>, new()
        where TestParams : IPoolTestParameter
    {
        private static ITestTemplate<ChannelType, TestParams> s_test;
        static CreateAndCloseFactoryAndChannelFullCycleTest()
        {
            s_test = new TestTemplate();
        }

        // A full cycle of creating a pool of channel factories, using each factory to create
        // a pool of channels, using all channels once and then closing all of them
        public static void CreateFactoriesAndChannelsUseAllOnceCloseAll()
        {
            using (var theOneTimeThing = new PoolOfThings<FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, ChannelType>>(
                    maxSize: s_test.TestParameters.MaxPooledFactories, // # of pooled FactoryAndPoolOfItsObjects
                    createInstance: () => new FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, ChannelType>(
                        createFactoryInstance: () =>
                            s_test.CreateChannelFactory(),
                        destroyFactoryInstance: (chf) =>
                            s_test.CloseFactory(chf),
                        maxPooledObjects: s_test.TestParameters.MaxPooledChannels, // # of pooled channels within each pooled FactoryAndPoolOfItsObjects
                        createObject: (chf) =>
                            s_test.CreateChannel(chf),
                        destroyObject: (ch) =>
                            s_test.CloseChannel(ch)),
                    destroyInstance: (_fapoio) => _fapoio.Destroy()))
            {
                foreach (var factoryAndPoolOfItsChannels in theOneTimeThing.GetAllPooledInstances())
                {
                    foreach (var channel in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                    {
                        s_test.UseChannel()(channel);
                    }
                }
            }
        }
    }

    public static class CreateAndCloseFactoryAndChannelFullCycleTestAsync<ChannelType, TestTemplate, TestParams>
        where ChannelType : class
        where TestTemplate : ITestTemplate<ChannelType, TestParams>, new()
        where TestParams : IPoolTestParameter
    {
        private static ITestTemplate<ChannelType, TestParams> s_test;
        static CreateAndCloseFactoryAndChannelFullCycleTestAsync()
        {
            s_test = new TestTemplate();
        }
        public static async Task CreateFactoriesAndChannelsUseAllOnceCloseAllAsync()
        {
            PoolOfAsyncThings<FactoryAndPoolOfItsAsyncObjects<ChannelFactory<ChannelType>, ChannelType>> oneTimeAsyncThing = null;
            var allTasksFromOneTimeAsyncThing = new List<Task>();
            try
            {
                oneTimeAsyncThing = new PoolOfAsyncThings<FactoryAndPoolOfItsAsyncObjects<ChannelFactory<ChannelType>, ChannelType>>(
                    maxSize: s_test.TestParameters.MaxPooledFactories, // # of pooled FactoryAndPoolOfItsObjects
                    createInstance: () => new FactoryAndPoolOfItsAsyncObjects<ChannelFactory<ChannelType>, ChannelType>(
                        createFactoryInstance: () =>
                            s_test.CreateChannelFactory(),
                        destroyFactoryInstanceAsync: async (chf) =>
                            await s_test.CloseFactoryAsync(chf),
                        maxPooledObjects: s_test.TestParameters.MaxPooledChannels, // # of pooled channels within each pooled FactoryAndPoolOfItsObjects
                        createObject: (chf) =>
                            s_test.CreateChannel(chf),
                        destroyObjectAsync: async (ch) =>
                            await s_test.CloseChannelAsync(ch)
                        ),
                    destroyInstanceAsync: (_fapoiao) => _fapoiao.DestroyAsync());

                foreach (var factoryAndPoolOfItsChannels in oneTimeAsyncThing.GetAllPooledInstances())
                {
                    foreach (var channel in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                    {
                        allTasksFromOneTimeAsyncThing.Add(s_test.UseAsyncChannel()(channel));
                    }
                }
            }
            finally
            {
                if (oneTimeAsyncThing != null)
                {
                    await Task.WhenAll(allTasksFromOneTimeAsyncThing);
                    await oneTimeAsyncThing.DestoryAllPooledInstancesAsync();
                }
            }
        }
    }

    public class PooledFactories<ChannelType, TestTemplate, TestParams>
        where ChannelType : class
        where TestTemplate : ITestTemplate<ChannelType, TestParams>, new()
        where TestParams : IPoolTestParameter
    {
        private static ITestTemplate<ChannelType, TestParams> s_test;
        private static PoolOfThings<ChannelFactory<ChannelType>> s_pooledChannelFactories;

        static PooledFactories()
        {
            s_test = new TestTemplate();
            s_pooledChannelFactories = StaticDisposablesHelper.AddDisposable(
                new PoolOfThings<ChannelFactory<ChannelType>>(
                    maxSize: s_test.TestParameters.MaxPooledFactories,
                    createInstance: () => s_test.CreateChannelFactory(),
                    destroyInstance: (chf) => s_test.CloseFactory(chf)));
        }
        public static void CreateUseAndCloseChannels()
        {
            foreach (var factory in s_pooledChannelFactories.GetAllPooledInstances())
            {
                ChannelType channel = s_test.CreateChannel(factory);
                if (channel != null)
                {
                    s_test.UseChannel()(channel);
                    s_test.CloseChannel(channel);
                }
            }
        }
    }

    public class PooledFactoriesAsync<ChannelType, TestTemplate, TestParams>
      where ChannelType : class
      where TestTemplate : ITestTemplate<ChannelType, TestParams>, new()
      where TestParams : IPoolTestParameter
    {
        private static ITestTemplate<ChannelType, TestParams> s_test;
        private static PoolOfAsyncThings<ChannelFactory<ChannelType>> s_pooledChannelFactories;

        static PooledFactoriesAsync()
        {
            s_test = new TestTemplate();
            s_pooledChannelFactories = StaticDisposablesHelper.AddDisposable(
                new PoolOfAsyncThings<ChannelFactory<ChannelType>>(
                    maxSize: s_test.TestParameters.MaxPooledFactories,
                    createInstance: () => s_test.CreateChannelFactory(),
                    destroyInstanceAsync: async (chf) => await s_test.CloseFactoryAsync(chf)));
        }
        public static async Task CreateUseAndCloseChannelsAsync()
        {
            var allTasks = new List<Task>();
            foreach (var factory in s_pooledChannelFactories.GetAllPooledInstances())
            {
                ChannelType channel = s_test.CreateChannel(factory);
                if (channel != null)
                {
                    allTasks.Add(UseAndCloseChannelAsync(channel));
                }
            }
            await Task.WhenAll(allTasks);
        }

        private static async Task UseAndCloseChannelAsync(ChannelType channel)
        {
            await s_test.UseAsyncChannel()(channel);
            await s_test.CloseChannelAsync(channel);
        }
    }

    public class PooledFactoriesAndChannels<ChannelType, TestTemplate, TestParams>
        where ChannelType : class
        where TestTemplate : ITestTemplate<ChannelType, TestParams>, new()
        where TestParams : IPoolTestParameter
    {
        private static ITestTemplate<ChannelType, TestParams> s_test;

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
                    maxSize: s_test.TestParameters.MaxPooledFactories, // # of pooled FactoryAndPoolOfItsObjects
                    createInstance: () => new FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, ChannelType>(
                        createFactoryInstance:
                            s_test.CreateChannelFactory,
                        destroyFactoryInstance: (chf) =>
                            s_test.CloseFactory(chf),
                        maxPooledObjects: s_test.TestParameters.MaxPooledChannels, // # of pooled channels within each pooled FactoryAndPoolOfItsObjects
                        createObject: (chf) =>
                            s_test.CreateChannel(chf),
                        destroyObject: (ch) =>
                            s_test.CloseChannel(ch)
                        ),
                    destroyInstance: (_fapoio) => _fapoio.Destroy()));
        }

        public static void UseChannelsInPooledFactoriesAndChannels()
        {
            foreach (var factoryAndPoolOfItsChannels in s_pooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channel in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    s_test.UseChannel()(channel);
                }
            }
        }
    }

    public class PooledFactoriesAndChannelsAsync<ChannelType, TestTemplate, TestParams>
        where ChannelType : class
        where TestTemplate : ITestTemplate<ChannelType, TestParams>, new()
        where TestParams : IPoolTestParameter
    {
        private static ITestTemplate<ChannelType, TestParams> s_test;

        private static PoolOfAsyncThings<
            FactoryAndPoolOfItsAsyncObjects<
                ChannelFactory<ChannelType>,
                ChannelType>
            > s_pooledFactoriesAndChannels;

        static PooledFactoriesAndChannelsAsync()
        {
            s_test = new TestTemplate();
            s_pooledFactoriesAndChannels = StaticDisposablesHelper.AddDisposable(
                new PoolOfAsyncThings<FactoryAndPoolOfItsAsyncObjects<ChannelFactory<ChannelType>, ChannelType>>(
                    maxSize: s_test.TestParameters.MaxPooledFactories, // # of pooled FactoryAndPoolOfItsObjects
                    createInstance: () => new FactoryAndPoolOfItsAsyncObjects<ChannelFactory<ChannelType>, ChannelType>(
                        createFactoryInstance:
                            s_test.CreateChannelFactory,
                        destroyFactoryInstanceAsync: async (chf) =>
                            await s_test.CloseFactoryAsync(chf),
                        maxPooledObjects: s_test.TestParameters.MaxPooledChannels, // # of pooled channels within each pooled FactoryAndPoolOfItsObjects
                        createObject: (chf) =>
                            s_test.CreateChannel(chf),
                        destroyObjectAsync: async (ch) =>
                            await s_test.CloseChannelAsync(ch)
                        ),
                    destroyInstanceAsync: async (_fapoiao) => await _fapoiao.DestroyAsync()));
        }

        public static async Task UseChannelsInPooledFactoriesAndChannelsAsync()
        {
            var allTasks = new List<Task>();
            foreach (var factoryAndPoolOfItsChannels in s_pooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channel in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    allTasks.Add(s_test.UseAsyncChannel()(channel));
                }
            }
            await Task.WhenAll(allTasks);
        }
    }

    public class RecyclablePooledFactories<ChannelType, TestTemplate, TestParams>
        where ChannelType : class
        where TestTemplate : ITestTemplate<ChannelType, TestParams>, IExceptionPolicy, new()
        where TestParams : IPoolTestParameter
    {
        private static int s_iteration = 0;
        private static ITestTemplate<ChannelType, TestParams> s_test;
        private static PoolOfThings<ChannelFactory<ChannelType>> s_recyclablePooledChannelFactories;

        static RecyclablePooledFactories()
        {
            s_test = new TestTemplate();
            // It is expected to see various exceptions when we use factories while recycling them
            (s_test as IExceptionPolicy).RelaxedExceptionPolicy = true;

            s_recyclablePooledChannelFactories = StaticDisposablesHelper.AddDisposable(
                new PoolOfThings<ChannelFactory<ChannelType>>(
                    maxSize: s_test.TestParameters.MaxPooledFactories,
                    createInstance:
                        s_test.CreateChannelFactory,
                    destroyInstance: (chf) =>
                         s_test.CloseFactory(chf)));
        }
        public static void CreateUseAndCloseChannels()
        {
            foreach (var factory in s_recyclablePooledChannelFactories.GetAllPooledInstances())
            {
                ChannelType channel = s_test.CreateChannel(factory);

                if (channel != null)
                {
                    s_test.UseChannel()(channel);
                    s_test.CloseChannel(channel);
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

    public class RecyclablePooledFactoriesAsync<ChannelType, TestTemplate, TestParams>
    where ChannelType : class
    where TestTemplate : ITestTemplate<ChannelType, TestParams>, IExceptionPolicy, new()
    where TestParams : IPoolTestParameter
    {
        private static int s_iteration = 0;
        private static ITestTemplate<ChannelType, TestParams> s_test;
        private static PoolOfAsyncThings<ChannelFactory<ChannelType>> s_recyclablePooledChannelFactories;

        static RecyclablePooledFactoriesAsync()
        {
            s_test = new TestTemplate();
            // It is expected to see various exceptions when we use factories while recycling them
            (s_test as IExceptionPolicy).RelaxedExceptionPolicy = true;

            s_recyclablePooledChannelFactories = StaticDisposablesHelper.AddDisposable(
                new PoolOfAsyncThings<ChannelFactory<ChannelType>>(
                    maxSize: s_test.TestParameters.MaxPooledFactories,
                    createInstance: s_test.CreateChannelFactory,
                    destroyInstanceAsync: async (chf) => await s_test.CloseFactoryAsync(chf)));
        }
        public static async Task CreateUseAndCloseChannelsAsync()
        {
            var allTasks = new List<Task>();
            foreach (var factory in s_recyclablePooledChannelFactories.GetAllPooledInstances())
            {
                ChannelType channel = s_test.CreateChannel(factory);

                if (channel != null)
                {
                    allTasks.Add(UseAndCloseChannelAsync(channel));
                }
            }
            await Task.WhenAll(allTasks);
        }

        private static async Task UseAndCloseChannelAsync(ChannelType channel)
        {
            await s_test.UseAsyncChannel()(channel);
            await s_test.CloseChannelAsync(channel);
        }

        public static async Task RecycleFactoriesAsync()
        {
            await s_recyclablePooledChannelFactories.DestoryAllPooledInstancesAsync();
        }
        public static async Task RunAllScenariosWithWeightsAsync(int createUseAndCloseChannelsWeight, int recycleFactoriesWeight)
        {
            int seed = Interlocked.Increment(ref s_iteration) % (createUseAndCloseChannelsWeight + recycleFactoriesWeight);
            if (seed < createUseAndCloseChannelsWeight)
            {
                await CreateUseAndCloseChannelsAsync();
            }
            else if (seed < createUseAndCloseChannelsWeight + recycleFactoriesWeight)
            {
                await RecycleFactoriesAsync();
            }
        }
    }

    // not used - *_OpenOnce is used instead as a workaround for #108
    public static class RecyclablePooledFactoriesAndChannels<ChannelType, TestTemplate, TestParams>
        where ChannelType : class
        where TestTemplate : ITestTemplate<ChannelType, TestParams>, new()
        where TestParams : IPoolTestParameter, IExceptionPolicy
    {
        private static int s_iteration = 0;
        private static ITestTemplate<ChannelType, TestParams> s_test;

        private static PoolOfThings<FactoryAndPoolOfItsObjects<
                ChannelFactory<ChannelType>,
                ChannelType>
            > s_recyclablePooledFactoriesAndChannels;

        static RecyclablePooledFactoriesAndChannels()
        {
            s_test = new TestTemplate();
            // It is expected to see various exceptions when we use factories while recycling them
            s_test.TestParameters.RelaxedExceptionPolicy = true;

            s_recyclablePooledFactoriesAndChannels = StaticDisposablesHelper.AddDisposable(
                new PoolOfThings<FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, ChannelType>>(
                    maxSize: s_test.TestParameters.MaxPooledFactories,
                    createInstance: () => new FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, ChannelType>(
                        createFactoryInstance: () =>
                            s_test.CreateChannelFactory(),
                        destroyFactoryInstance: (chf) => s_test.CloseFactory(chf),
                        maxPooledObjects: s_test.TestParameters.MaxPooledChannels,
                        createObject: (chf) => s_test.CreateChannel(chf),
                        destroyObject: (ch) => s_test.CloseChannel(ch)),
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
                    s_test.UseChannel()(channel);
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
                    TestUtils.ReportFailure("Factory.State == CommunicationState.Closed");
                }
            }
        }
    }

    public static class RecyclablePooledFactoriesAndChannels_OpenOnce<ChannelType, TestTemplate, TestParams>
        where ChannelType : class
        where TestTemplate : ITestTemplate<ChannelType, TestParams>, IExceptionPolicy, new()
        where TestParams : IPoolTestParameter
    {
        private static int s_iteration = 0;
        private static ITestTemplate<ChannelType, TestParams> s_test;

        private static PoolOfThings<FactoryAndPoolOfItsObjects<
                ChannelFactory<ChannelType>,
                OpenOnceChannelWrapper<ChannelType>>
            > s_recyclablePooledFactoriesAndChannels;

        static RecyclablePooledFactoriesAndChannels_OpenOnce()
        {
            s_test = new TestTemplate();
            // It is expected to see various exceptions when we use factories while recycling them
            (s_test as IExceptionPolicy).RelaxedExceptionPolicy = true;

            s_recyclablePooledFactoriesAndChannels = StaticDisposablesHelper.AddDisposable(
                new PoolOfThings<FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, OpenOnceChannelWrapper<ChannelType>>>(
                    maxSize: s_test.TestParameters.MaxPooledFactories,
                    createInstance: () => new FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, OpenOnceChannelWrapper<ChannelType>>(
                        createFactoryInstance: () => s_test.CreateChannelFactory(),
                        destroyFactoryInstance: (chf) => s_test.CloseFactory(chf),
                        maxPooledObjects: s_test.TestParameters.MaxPooledChannels,
                        createObject: (chf) => new OpenOnceChannelWrapper<ChannelType>(s_test.CreateChannel(chf)),
                        destroyObject: (chWr) => s_test.CloseChannel(chWr.Channel)),
                    destroyInstance: (_fapoio) => _fapoio.Destroy()));
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
        }

        public static void UsePooledChannels()
        {
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channelWrapper in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    channelWrapper.OpenChannelOnce();
                    s_test.UseChannel()(channelWrapper.Channel);
                }
            }
        }

        public static void AbortPooledChannels()
        {
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channelWrapper in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    var co = (ICommunicationObject)channelWrapper.Channel;
                    co.Abort();
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
                    TestUtils.ReportFailure("Factory.State == CommunicationState.Closed");
                }
            }
        }
    }

    public class RecyclablePooledFactoriesAndChannelsAsync_OpenOnce<ChannelType, TestTemplate, TestParams>
        where ChannelType : class
        where TestTemplate : ITestTemplate<ChannelType, TestParams>, IExceptionPolicy, new()
        where TestParams : IPoolTestParameter
    {
        private static int s_iteration = 0;
        private static ITestTemplate<ChannelType, TestParams> s_test;
        private static PoolOfAsyncThings<
            FactoryAndPoolOfItsAsyncObjects<
                ChannelFactory<ChannelType>,
                OpenAsyncOnceChannelWrapper<ChannelType>>
            > s_recyclablePooledFactoriesAndChannels;

        static RecyclablePooledFactoriesAndChannelsAsync_OpenOnce()
        {
            s_test = new TestTemplate();
            // It is expected to see various exceptions when we use factories while recycling them
            (s_test as IExceptionPolicy).RelaxedExceptionPolicy = true;

            s_recyclablePooledFactoriesAndChannels = StaticDisposablesHelper.AddDisposable(
                new PoolOfAsyncThings<
                    FactoryAndPoolOfItsAsyncObjects<ChannelFactory<ChannelType>, OpenAsyncOnceChannelWrapper<ChannelType>>>(
                        maxSize: s_test.TestParameters.MaxPooledFactories,
                        createInstance: () => new FactoryAndPoolOfItsAsyncObjects<ChannelFactory<ChannelType>, OpenAsyncOnceChannelWrapper<ChannelType>>(
                            createFactoryInstance:
                                s_test.CreateChannelFactory,
                            destroyFactoryInstanceAsync: async (chf) =>
                                await s_test.CloseFactoryAsync(chf),
                            maxPooledObjects: s_test.TestParameters.MaxPooledChannels,
                            createObject: (chf) => new OpenAsyncOnceChannelWrapper<ChannelType>(s_test.CreateChannel(chf)),
                            destroyObjectAsync: async (chWr) => await s_test.CloseChannelAsync(chWr.Channel)),
                    destroyInstanceAsync: async (_fapoio) => await _fapoio.DestroyAsync()));
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
                    allTasks.Add(UseChannelWrapperAsync(channelWrapper));
                }
            }
            await Task.WhenAll(allTasks);
        }

        private static async Task UseChannelWrapperAsync(OpenAsyncOnceChannelWrapper<ChannelType> channelWrapper)
        {
            if (channelWrapper.Channel != null)
            {
                await channelWrapper.OpenChannelOnceAsync();
                await s_test.UseAsyncChannel()(channelWrapper.Channel);
            }
        }

        public static async Task RecyclePooledChannelsAsync()
        {
            Console.WriteLine("RecyclePooledChannelsAsync");
            var allTasks = new List<Task>();
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                allTasks.Add(factoryAndPoolOfItsChannels.ObjectsPool.DestoryAllPooledInstancesAsync());
            }
            await Task.WhenAll(allTasks);
        }

        public static async Task RecyclePooledFactoriesAsync()
        {
            Console.WriteLine("RecyclePooledFactoriesAsync");
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
