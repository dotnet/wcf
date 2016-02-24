// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace SharedPoolsOfWCFObjects
{
    // Common test params are shared by all the tests which makes the results comparable between different 
    // bindings and tests. E.g. the throughput perf test running say Http&HelloWorld and NetTcp&Streaming 
    // will have the same number of channels, factories, and calls per test so it is possible to compare
    // the results.

    public class CommonStressTestParams : IPoolTestParameter, IStatsCollectingTestParameter
    {
        public virtual int MaxPooledChannels { get { return 3; } }
        public virtual int MaxPooledFactories { get { return 3; } }
        public virtual int SunnyDayMaxStatsSamples { get { return 10000; } }
        public virtual int RainyDayMaxStatsSamples { get { return 10000; } }
    }

    // For startup we're going to have 1 channel factory and 1 channel created per call.
    // Failures aren't expected in perf startup/throughput scenarios and we don't have a lot of calls 
    // so we can reduce the number of samples collected (hence we use different magic numbers).
    public class CommonPerfStartupTestParams : IPoolTestParameter, IStatsCollectingTestParameter
    {
        public int MaxPooledChannels { get { return 1; } }
        public int MaxPooledFactories { get { return 1; } }
        public int SunnyDayMaxStatsSamples { get { return 100; } }
        public int RainyDayMaxStatsSamples { get { return 10; } }
    }

    // For the throughput perf we're using several factories and several channels per factory
    // Again, the failures aren't expected in perf tests so we're reducing the number of samples
    public class CommonPerfThroughputTestParams : IPoolTestParameter, IStatsCollectingTestParameter
    {
        public int MaxPooledChannels { get { return 3; } }
        public int MaxPooledFactories { get { return 3; } }
        public int SunnyDayMaxStatsSamples { get { return 1000; } }
        public int RainyDayMaxStatsSamples { get { return 10; } }
    }
    public abstract class CommonTest<ChannelType, TestParams> : ITestTemplate<ChannelType, TestParams>, IExceptionPolicy
    where TestParams : IPoolTestParameter, IStatsCollectingTestParameter, new()
    {
        protected TestParams _params;
        protected CallStats _createChannelStats;
        protected CallStats _closeChannelStats;
        protected CallStats _closeChannelAsyncStats;
        protected CallStats _closeFactoryStats;
        protected CallStats _closeFactoryAsyncStats;
        protected CallStats _useChannelStats;
        protected CallStats _useChannelAsyncStats;

        public CommonTest()
        {
            _params = new TestParams();
            _createChannelStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples);
            _useChannelStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples);
            _useChannelAsyncStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples);
            _closeChannelStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples);
            _closeChannelAsyncStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples);
            _closeFactoryStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples);
            _closeFactoryAsyncStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples);
        }

        public TestParams TestParameters
        {
            get
            {
                return _params;
            }
        }

        public bool RelaxedExceptionPolicy { get; set; }

        public virtual EndpointAddress CreateEndPointAddress()
        {
            return TestHelpers.CreateEndPointHelloAddress();
        }

        public virtual Binding CreateBinding()
        {
            return TestHelpers.CreateBinding();
        }

        public virtual ChannelFactory<ChannelType> CreateChannelFactory()
        {
            return TestHelpers.CreateChannelFactory<ChannelType>(CreateEndPointAddress(), CreateBinding());
        }
        public void CloseFactory(ChannelFactory<ChannelType> factory)
        {
            _closeFactoryStats.CallActionAndRecordStats(() => TestHelpers.CloseFactory(factory), RelaxedExceptionPolicy);
        }
        public Task CloseFactoryAsync(ChannelFactory<ChannelType> factory)
        {
            return _closeFactoryAsyncStats.CallAsyncFuncAndRecordStatsAsync(TestHelpers.CloseFactoryAsync, factory, RelaxedExceptionPolicy);
        }

        public ChannelType CreateChannel(ChannelFactory<ChannelType> factory)
        {
            return _createChannelStats.CallFuncAndRecordStats(TestHelpers.CreateChannel, factory, RelaxedExceptionPolicy);
        }
        public void CloseChannel(ChannelType channel)
        {
            _closeChannelStats.CallActionAndRecordStats(() => TestHelpers.CloseChannel(channel), RelaxedExceptionPolicy);
        }
        public Task CloseChannelAsync(ChannelType channel)
        {
            return _closeChannelAsyncStats.CallAsyncFuncAndRecordStatsAsync(TestHelpers.CloseChannelAsync, channel, RelaxedExceptionPolicy);
        }
        public abstract Action<ChannelType> UseChannel();
        public abstract Func<ChannelType, Task> UseAsyncChannel();
    }
}
