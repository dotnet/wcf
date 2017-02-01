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
using System.Linq;

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
            // we do not expect exceptions here since we're not closing anything during usage
            (s_test as IExceptionPolicy).RelaxedExceptionPolicy = false;
        }

        // A full cycle of creating a pool of channel factories, using each factory to create
        // a pool of channels, using all channels once and then closing all of them
        public static int CreateFactoriesAndChannelsUseAllOnceCloseAll()
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
                            s_test.CloseChannel(ch),
                        validateObjectInstance: s_test.ValidateChannel),
                    destroyInstance: (fapoio) => fapoio.Destroy(),
                    instanceValidator: (fapoio) => s_test.ValidateFactory(fapoio.Factory)))
            {
                int requestsMade = 0;
                foreach (var factoryAndPoolOfItsChannels in theOneTimeThing.GetAllPooledInstances())
                {
                    foreach (var channel in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                    {
                        requestsMade += s_test.UseChannel()(channel);
                    }
                }
                return requestsMade;
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
            // we do not expect exceptions here since we're not closing anything during usage
            (s_test as IExceptionPolicy).RelaxedExceptionPolicy = false;
        }
        public static async Task<int> CreateFactoriesAndChannelsUseAllOnceCloseAllAsync()
        {
            PoolOfAsyncThings<FactoryAndPoolOfItsAsyncObjects<ChannelFactory<ChannelType>, ChannelType>> oneTimeAsyncThing = null;
            var allTasksFromOneTimeAsyncThing = new List<Task<int>>();
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
                            await s_test.CloseChannelAsync(ch),
                        validateObjectInstance: s_test.ValidateChannel),
                    destroyInstanceAsync: (fapoiao) => fapoiao.DestroyAsync(),
                    instanceValidator: (fapoiao) => s_test.ValidateFactory(fapoiao.Factory));

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
            return allTasksFromOneTimeAsyncThing.Sum(t => t.Result);
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
            // we do not expect exceptions here since we're not closing anything during usage
            (s_test as IExceptionPolicy).RelaxedExceptionPolicy = false;
            s_pooledChannelFactories = StaticDisposablesHelper.AddDisposable(
                new PoolOfThings<ChannelFactory<ChannelType>>(
                    maxSize: s_test.TestParameters.MaxPooledFactories,
                    createInstance: () => s_test.CreateChannelFactory(),
                    destroyInstance: (chf) => s_test.CloseFactory(chf),
                    instanceValidator: s_test.ValidateFactory));
        }
        public static int CreateUseAndCloseChannels()
        {
            int requestsMade = 0;
            foreach (var factory in s_pooledChannelFactories.GetAllPooledInstances())
            {
                ChannelType channel = s_test.CreateChannel(factory);
                if (channel != null)
                {
                    requestsMade += s_test.UseChannel()(channel);
                    s_test.CloseChannel(channel);
                }
            }
            return requestsMade;
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
            // we do not expect exceptions here since we're not closing anything during usage
            (s_test as IExceptionPolicy).RelaxedExceptionPolicy = false;
            s_pooledChannelFactories = StaticDisposablesHelper.AddDisposable(
                new PoolOfAsyncThings<ChannelFactory<ChannelType>>(
                    maxSize: s_test.TestParameters.MaxPooledFactories,
                    createInstance: () => s_test.CreateChannelFactory(),
                    destroyInstanceAsync: async (chf) => await s_test.CloseFactoryAsync(chf),
                    instanceValidator: s_test.ValidateFactory));
        }
        public static async Task<int> CreateUseAndCloseChannelsAsync()
        {
            var allTasks = new List<Task<int>>(32);
            foreach (var factory in s_pooledChannelFactories.GetAllPooledInstances())
            {
                ChannelType channel = s_test.CreateChannel(factory);
                if (channel != null)
                {
                    allTasks.Add(UseAndCloseChannelAsync(channel));
                }
            }
            await Task.WhenAll(allTasks);
            return allTasks.Sum(t => t.Result);
        }

        private static async Task<int> UseAndCloseChannelAsync(ChannelType channel)
        {
            var requestsMade = await s_test.UseAsyncChannel()(channel);
            await s_test.CloseChannelAsync(channel);
            return requestsMade;
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
            // we do not expect exceptions here since we're not closing anything during usage
            (s_test as IExceptionPolicy).RelaxedExceptionPolicy = false;
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
                            s_test.CloseChannel(ch),
                        validateObjectInstance: s_test.ValidateChannel),
                    destroyInstance: (fapoio) => fapoio.Destroy(),
                    instanceValidator: (fapoiao) => s_test.ValidateFactory(fapoiao.Factory)));
        }

        public static int UseAllChannelsInPooledFactoriesAndChannels()
        {
            int requestsMade = 0;
            foreach (var factoryAndPoolOfItsChannels in s_pooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channel in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    requestsMade += s_test.UseChannel()(channel);
                }
            }
            return requestsMade;
        }
        public static int UseOneChannelInPooledFactoriesAndChannels(int n)
        {
            if (n > s_test.TestParameters.MaxPooledChannels * s_test.TestParameters.MaxPooledFactories)
            {
                TestUtils.ReportFailure("Not enough channels...", debugBreak: true);
                throw new Exception(
                    String.Format("Can not use {0}th channel from the pool that only has {1} channels",
                        n, s_test.TestParameters.MaxPooledChannels * s_test.TestParameters.MaxPooledFactories));
            }
            var factoryAndChannels = s_pooledFactoriesAndChannels[n / s_test.TestParameters.MaxPooledChannels];
            var channel = factoryAndChannels.ObjectsPool[n % s_test.TestParameters.MaxPooledChannels];

            return s_test.UseChannel()(channel);
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
            // we do not expect exceptions here since we're not closing anything during usage
            (s_test as IExceptionPolicy).RelaxedExceptionPolicy = false;
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
                            await s_test.CloseChannelAsync(ch),
                        validateObjectInstance: s_test.ValidateChannel
                        ),
                    destroyInstanceAsync: async (fapoiao) => await fapoiao.DestroyAsync(),
                    instanceValidator: (fapoiao) => s_test.ValidateFactory(fapoiao.Factory)));
        }

        public static async Task<int> UseAllChannelsInPooledFactoriesAndChannelsAsync()
        {
            var allTasks = new List<Task<int>>(32);
            foreach (var factoryAndPoolOfItsChannels in s_pooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channel in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    allTasks.Add(s_test.UseAsyncChannel()(channel));
                }
            }
            await Task.WhenAll(allTasks);
            return allTasks.Sum(t => t.Result);
        }

        // For a direct performance comparison between sync and async throughput we use all channels serially
        public static async Task<int> UseAllChannelsInPooledFactoriesAndChannelsSerialAsync()
        {
            int requestsMade = 0;
            foreach (var factoryAndPoolOfItsChannels in s_pooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channel in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    requestsMade += await s_test.UseAsyncChannel()(channel);
                }
            }
            return requestsMade;
        }

        // Some bindings would perform poorly when multiple threads try to use the same channel
        // This method will only use n'th pooled channel in order to obtain maximum possible throughput
        public static async Task<int> UseOneChannelInPooledFactoriesAndChannelsSerialAsync(int n)
        {
            if (n > s_test.TestParameters.MaxPooledChannels * s_test.TestParameters.MaxPooledFactories)
            {
                TestUtils.ReportFailure("Not enough channels...", debugBreak: true);
                throw new Exception(
                    String.Format("Can not use {0}th channel from the pool that only has {1} channels",
                        n, s_test.TestParameters.MaxPooledChannels * s_test.TestParameters.MaxPooledFactories));
            }

            var factoryAndChannels = s_pooledFactoriesAndChannels[n / s_test.TestParameters.MaxPooledChannels];
            var channel = factoryAndChannels.ObjectsPool[n % s_test.TestParameters.MaxPooledChannels];
            return await s_test.UseAsyncChannel()(channel);
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
                    createInstance: s_test.CreateChannelFactory,
                    destroyInstance: (chf) => s_test.CloseFactory(chf),
                    instanceValidator: s_test.ValidateFactory));
        }
        public static int CreateUseAndCloseChannels()
        {
            int requestsMade = 0;
            foreach (var factory in s_recyclablePooledChannelFactories.GetAllPooledInstances())
            {
                ChannelType channel = s_test.CreateChannel(factory);

                if (channel != null)
                {
                    requestsMade += s_test.UseChannel()(channel);
                    s_test.CloseChannel(channel);
                }
            }
            return requestsMade;
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
                    destroyInstanceAsync: async (chf) => await s_test.CloseFactoryAsync(chf),
                    instanceValidator: s_test.ValidateFactory));
        }
        public static async Task<int> CreateUseAndCloseChannelsAsync()
        {
            var allTasks = new List<Task<int>>(32);
            foreach (var factory in s_recyclablePooledChannelFactories.GetAllPooledInstances())
            {
                ChannelType channel = s_test.CreateChannel(factory);

                if (channel != null)
                {
                    allTasks.Add(UseAndCloseChannelAsync(channel));
                }
            }
            await Task.WhenAll(allTasks);
            return allTasks.Sum(t => t.Result);
        }

        private static async Task<int> UseAndCloseChannelAsync(ChannelType channel)
        {
            var requestsMade = await s_test.UseAsyncChannel()(channel);
            await s_test.CloseChannelAsync(channel);
            return requestsMade;
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
        where TestTemplate : ITestTemplate<ChannelType, TestParams>, IExceptionPolicy, new()
        where TestParams : IPoolTestParameter
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
            (s_test as IExceptionPolicy).RelaxedExceptionPolicy = true;

            s_recyclablePooledFactoriesAndChannels = StaticDisposablesHelper.AddDisposable(
                new PoolOfThings<FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, ChannelType>>(
                    maxSize: s_test.TestParameters.MaxPooledFactories,
                    createInstance: () => new FactoryAndPoolOfItsObjects<ChannelFactory<ChannelType>, ChannelType>(
                        createFactoryInstance: () =>
                            s_test.CreateChannelFactory(),
                        destroyFactoryInstance: (chf) => s_test.CloseFactory(chf),
                        maxPooledObjects: s_test.TestParameters.MaxPooledChannels,
                        createObject: (chf) => s_test.CreateChannel(chf),
                        destroyObject: (ch) => s_test.CloseChannel(ch),
                        validateObjectInstance: s_test.ValidateChannel),
                    destroyInstance: (fapoio) => fapoio.Destroy(),
                    instanceValidator: (fapoio) => s_test.ValidateFactory(fapoio.Factory)));
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

        public static int UsePooledChannels()
        {
            int requestsMade = 0;
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channel in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    requestsMade += s_test.UseChannel()(channel);
                }
            }
            return requestsMade;
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

    public static class RecyclablePooledFactoriesAndChannelsAsync<ChannelType, TestTemplate, TestParams>
      where ChannelType : class
      where TestTemplate : ITestTemplate<ChannelType, TestParams>, IExceptionPolicy, new()
      where TestParams : IPoolTestParameter
    {
        private static int s_iteration = 0;
        private static ITestTemplate<ChannelType, TestParams> s_test;

        private static PoolOfAsyncThings<FactoryAndPoolOfItsAsyncObjects<
                ChannelFactory<ChannelType>,
                ChannelType>
            > s_recyclablePooledFactoriesAndChannels;

        static RecyclablePooledFactoriesAndChannelsAsync()
        {
            s_test = new TestTemplate();
            // It is expected to see various exceptions when we use factories while recycling them
            (s_test as IExceptionPolicy).RelaxedExceptionPolicy = true;

            s_recyclablePooledFactoriesAndChannels = StaticDisposablesHelper.AddDisposable(
                new PoolOfAsyncThings<FactoryAndPoolOfItsAsyncObjects<ChannelFactory<ChannelType>, ChannelType>>(
                    maxSize: s_test.TestParameters.MaxPooledFactories,
                    createInstance: () => new FactoryAndPoolOfItsAsyncObjects<ChannelFactory<ChannelType>, ChannelType>(
                        createFactoryInstance: () =>
                            s_test.CreateChannelFactory(),
                        destroyFactoryInstanceAsync: async (chf) => await s_test.CloseFactoryAsync(chf),
                        maxPooledObjects: s_test.TestParameters.MaxPooledChannels,
                        createObject: (chf) => s_test.CreateChannel(chf),
                        destroyObjectAsync: (ch) => s_test.CloseChannelAsync(ch),
                        validateObjectInstance: s_test.ValidateChannel),
                    destroyInstanceAsync: (fapoio) => fapoio.DestroyAsync(),
                    instanceValidator: (fapoio) => s_test.ValidateFactory(fapoio.Factory)));
        }

        public static async Task RunAllScenariosWithWeightsAsync(int useWeight, int recycleChannelsWeight, int recycleFactoriesWeight)
        {
            int requestsMade = 0;
            int seed = Interlocked.Increment(ref s_iteration) % (useWeight + recycleChannelsWeight + recycleFactoriesWeight);
            if (seed < useWeight)
            {
                requestsMade = await UsePooledChannelsAsync();
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

        public static async Task<int> UsePooledChannelsAsync()
        {
            var allTasks = new List<Task<int>>(32);
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channel in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    allTasks.Add(s_test.UseAsyncChannel()(channel));
                }
            }
            await Task.WhenAll(allTasks);
            return allTasks.Sum(t => t.Result);
        }

        public static async Task RecyclePooledChannelsAsync()
        {
            Console.WriteLine("RecyclePooledChannelsAsync");
            var allTasks = new List<Task>(32);
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
                        createObject: (chf) =>
                        {
                            var channel = s_test.CreateChannel(chf);
                            // If channel creation failed and is unexpected then we'd already catch it inside s_test.CreateChannel.
                            // If the failure is expected then we will get null back.
                            // In this case we simply return null instead of wrapping it and let the pool handle it.
                            return channel != null ? new OpenOnceChannelWrapper<ChannelType>(channel) : null;
                        },
                        destroyObject: (chWr) => s_test.CloseChannel(chWr.Channel),
                        validateObjectInstance: (chWr) => s_test.ValidateChannel(chWr.Channel)),
                    destroyInstance: (fapoio) => fapoio.Destroy(),
                    instanceValidator: (fapoio) => s_test.ValidateFactory(fapoio.Factory)));
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

        public static int UsePooledChannels()
        {
            int requestsMade = 0;
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channelWrapper in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    channelWrapper.OpenChannelOnce(s_test.OpenChannel);
                    requestsMade += s_test.UseChannel()(channelWrapper.Channel);
                }
            }
            return requestsMade;
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
                            createObject: (chf) =>
                            {
                                var channel = s_test.CreateChannel(chf);
                                // If channel creation failed and is unexpected we'd already catch it inside s_test.CreateChannel.
                                // If the failure is expected then we will get null back.
                                // In this case we simply return null instead of wrapping it and let the pool handle it
                                return channel != null ? new OpenAsyncOnceChannelWrapper<ChannelType>(channel) : null;
                            },
                            destroyObjectAsync: async (chWr) => await s_test.CloseChannelAsync(chWr.Channel),
                            validateObjectInstance: (oaocw) => s_test.ValidateChannel(oaocw.Channel)),
                    destroyInstanceAsync: async (fapoio) => await fapoio.DestroyAsync(),
                    instanceValidator: (fapoio) => s_test.ValidateFactory(fapoio.Factory)));
        }

        public static async Task<int> RunAllScenariosWithWeightsAsync(int useWeight, int recycleChannelsWeight, int recycleFactoriesWeight)
        {
            int requestsMade = 0;
            int seed = new Random(Interlocked.Increment(ref s_iteration)).Next(useWeight + recycleChannelsWeight + recycleFactoriesWeight);
            //int seed = Interlocked.Increment(ref s_iteration) % (useWeight + recycleChannelsWeight + recycleFactoriesWeight);
            if (seed < useWeight)
            {
                requestsMade = await UsePooledChannelsAsync();
            }
            else if (seed < useWeight + recycleChannelsWeight)
            {
                await RecyclePooledChannelsAsync();
            }
            else if (seed < useWeight + recycleChannelsWeight + recycleFactoriesWeight)
            {
                await RecyclePooledFactoriesAsync();
            }
            return requestsMade;
        }

        public static async Task<int> UsePooledChannelsAsync()
        {
            var allTasks = new List<Task<int>>(32);
            foreach (var factoryAndPoolOfItsChannels in s_recyclablePooledFactoriesAndChannels.GetAllPooledInstances())
            {
                foreach (var channelWrapper in factoryAndPoolOfItsChannels.ObjectsPool.GetAllPooledInstances())
                {
                    allTasks.Add(UseChannelWrapperAsync(channelWrapper));
                }
            }
            await Task.WhenAll(allTasks);
            return allTasks.Sum(t => t.Result);
        }

        private static async Task<int> UseChannelWrapperAsync(OpenAsyncOnceChannelWrapper<ChannelType> channelWrapper)
        {
            int requestsMade = 0;
            if (channelWrapper.Channel != null)
            {
                await channelWrapper.OpenChannelOnceAsync(s_test.OpenChannelAsync);
                requestsMade = await s_test.UseAsyncChannel()(channelWrapper.Channel);
            }
            return requestsMade;
        }

        public static async Task RecyclePooledChannelsAsync()
        {
            Console.WriteLine("RecyclePooledChannelsAsync");
            var allTasks = new List<Task>(32);
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
    public class OpenOnceChannelWrapper<ChannelType>
    {
        private bool _openCalled = false;
        private Object _lock = new Object();

        public ChannelType Channel { get; set; }

        public OpenOnceChannelWrapper(ChannelType c)
        {
            Channel = c;
        }
        public void OpenChannelOnce(Action<ChannelType> openChannel)
        {
            lock (_lock)
            {
                if (!_openCalled)
                {
                    openChannel(Channel);
                    _openCalled = true;
                }
            }
        }
    }

    public class OpenAsyncOnceChannelWrapper<ChannelType>
    {
        private Task<Task> _openTask = null;

        public ChannelType Channel { get; set; }

        public OpenAsyncOnceChannelWrapper(ChannelType c)
        {
            Channel = c;
        }
        public async Task OpenChannelOnceAsync(Func<ChannelType, Task> openChannelAsync)
        {
            // Channel can be null if the factory is in faulted/closed state
            if (Channel != null)
            {
                var co = Channel as ICommunicationObject;
                // create a cold task
                var t = new Task<Task>(async () => await openChannelAsync(Channel));
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
            IDisposable d = null;
            while (s_disposables.TryDequeue(out d))
            {
                d.Dispose();
            }
        }
    }
}
