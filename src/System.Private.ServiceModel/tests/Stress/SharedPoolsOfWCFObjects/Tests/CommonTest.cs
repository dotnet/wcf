// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace SharedPoolsOfWCFObjects
{
    // The following classes are used as generic parameters to test templates which in turn are used as generic parameters to test scenarios.
    // Common test params are shared by all the tests which makes the results comparable between different
    // bindings and tests. E.g. the throughput perf test running Http&HelloWorld and NetTcp&Streaming
    // will have the same number of channels, factories, and calls per iteration
    // which allows for comparing the results.
    // These classes also define all the "magic" constants such as #stat collection samples, timeout values, etc
    public class CommonPerfExceptionPolicyParam : IExceptionHandlingPolicyParameter
    {
        virtual public Func<Exception, bool> ExceptionHandler { get { return TestUtils.ReportFailureDontEatException; } }
        // For perf we do not want to ignore/replace pooled factories and channels that did not pass ITestTemplate validation checks
        public bool ReplaceInvalidPooledFactories { get { return false; } }
        public bool ReplaceInvalidPooledChannels { get { return false; } }
    }

    // This class represents a relaxed failure criteria currently used for stress tests.
    // In its current shape it makes the tests to:
    // -    print and ignore timeout exceptions originated from our calls to Channels and Factories
    // -    replace faulted/closed Channels and Factories (otherwise the tests will continue using them)
    // Depending on the severity of stress issues we target we can tighten it to have absolutely no tolerance towards any unexpected behavior 
    // or we can implement some limited tolerance logic
    //
    // Notes for the future refactoring:
    // - Rather than being a part of *TestParams classes, the IExceptionHandlingPolicyParameter could be a generic parameter 
    //   to the Test/TestParameter class itself. This would allow the caller to explicitly control the exception policy 
    //   from the place where the tests are invoked.
    public class CommonStressExceptionPolicyParam : IExceptionHandlingPolicyParameter
    {
        virtual public Func<Exception, bool> ExceptionHandler { get { return TestUtils.ReportFailureAndBreakIfNotTimeout; } }

        // For some stress tests we might want to ignore/replace pooled factories and channels that did not pass ITestTemplate validation checks
        virtual public bool ReplaceInvalidPooledFactories { get { return true; } }
        virtual public bool ReplaceInvalidPooledChannels { get { return true; } }
    }

    public class CommonStressTestParams : CommonStressExceptionPolicyParam, IPoolTestParameter, IStatsCollectingTestParameter
    {
        virtual public int MaxPooledChannels { get { return 3; } }
        virtual public int MaxPooledFactories { get { return 3; } }
        virtual public int SunnyDayMaxStatsSamples { get { return 10000; } }
        virtual public int RainyDayMaxStatsSamples { get { return 10000; } }
        // Investigation of timeouts could be tricky when exceptions are already thrown and stacks unwound.
        // To help with the investigations we preemptively break into debuggers just before the actual timeout happens
        // so we have a chance to analyze the reasons for the upcoming WCF timeout.
        public int PreemptiveCloseChannelTimeout { get { return 50000; } }
        public int PreemptiveCloseFactoryTimeout { get { return 50000; } }
    }

    // For startup we're going to have 1 channel factory and 1 channel created per call.
    // Failures aren't expected in perf startup/throughput scenarios and we don't have a lot of calls 
    // so we can reduce the number of samples collected (hence we use different magic numbers).
    public class CommonPerfStartupTestParams : CommonPerfExceptionPolicyParam, IPoolTestParameter, IStatsCollectingTestParameter
    {
        virtual public int MaxPooledChannels { get { return 1; } }
        virtual public int MaxPooledFactories { get { return 1; } }
        virtual public int SunnyDayMaxStatsSamples { get { return 100; } }
        virtual public int RainyDayMaxStatsSamples { get { return 10; } }
        public int PreemptiveCloseChannelTimeout { get { return 0; } }
        public int PreemptiveCloseFactoryTimeout { get { return 0; } }
    }

    // For the throughput perf we're using several factories and several channels per factory
    // Again, the failures aren't expected in perf tests so we're reducing the number of samples
    public class CommonPerfThroughputTestParams : CommonPerfExceptionPolicyParam, IPoolTestParameter, IStatsCollectingTestParameter
    {
        virtual public int MaxPooledChannels { get { return 8; } }
        virtual public int MaxPooledFactories { get { return 12; } }
        virtual public int SunnyDayMaxStatsSamples { get { return 1000; } }
        virtual public int RainyDayMaxStatsSamples { get { return 10; } }
        public int PreemptiveCloseChannelTimeout { get { return 0; } }
        public int PreemptiveCloseFactoryTimeout { get { return 0; } }
    }

    public abstract class CommonTest<ChannelType, TestParams> : ITestTemplate<ChannelType, TestParams>, IExceptionPolicy
    where TestParams : IExceptionHandlingPolicyParameter, IPoolTestParameter, IStatsCollectingTestParameter, new()
    {
        protected TestParams _params;
        protected CallStats _createChannelStats;
        protected CallStats _closeChannelStats;
        protected CallStats _closeChannelAsyncStats;
        protected CallStats _closeFactoryStats;
        protected CallStats _closeFactoryAsyncStats;
        protected CallStats _useChannelStats;
        protected CallStats _useChannelAsyncStats;
        protected CallStats _openChannelStats;
        protected CallStats _openChannelAsyncStats;


        public CommonTest()
        {
            _params = new TestParams();
            _createChannelStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples, _params.ExceptionHandler);
            _useChannelStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples, _params.ExceptionHandler);
            _useChannelAsyncStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples, _params.ExceptionHandler);
            _closeChannelStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples, _params.ExceptionHandler);
            _closeChannelAsyncStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples, _params.ExceptionHandler);
            _closeFactoryStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples, _params.ExceptionHandler);
            _closeFactoryAsyncStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples, _params.ExceptionHandler);
            _openChannelStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples, _params.ExceptionHandler);
            _openChannelAsyncStats = new CallStats(_params.SunnyDayMaxStatsSamples, _params.RainyDayMaxStatsSamples, _params.ExceptionHandler);
        }

        public TestParams TestParameters
        {
            get
            {
                return _params;
            }
        }

        virtual public bool RelaxedExceptionPolicy { get; set; }

        virtual public EndpointAddress CreateEndPointAddress()
        {
            return TestHelpers.CreateEndPointHelloAddress();
        }

        virtual public Binding CreateBinding()
        {
            return TestHelpers.CreateBinding();
        }

        virtual public ChannelFactory<ChannelType> CreateChannelFactory()
        {
            return TestHelpers.CreateChannelFactory<ChannelType>(CreateEndPointAddress(), CreateBinding());
        }
        virtual public void CloseFactory(ChannelFactory<ChannelType> factory)
        {
            _closeFactoryStats.CallActionAndRecordStats(() => TestHelpers.CloseFactory(factory), RelaxedExceptionPolicy);
        }
        virtual public Task CloseFactoryAsync(ChannelFactory<ChannelType> factory)
        {
            return _params.PreemptiveCloseFactoryTimeout > 0
                ? _closeFactoryAsyncStats.CallAsyncFuncAndRecordStatsAsync(() =>
                    TestHelpers.CloseFactoryPreemptiveTimeoutAsync(factory, _params.PreemptiveCloseFactoryTimeout, failOnPreemptiveTimeout: false), RelaxedExceptionPolicy)
                : _closeFactoryAsyncStats.CallAsyncFuncAndRecordStatsAsync(() =>
                    TestHelpers.CloseFactoryAsync(factory), RelaxedExceptionPolicy);
        }

        virtual public ChannelType CreateChannel(ChannelFactory<ChannelType> factory)
        {
            return _createChannelStats.CallFuncAndRecordStats(TestHelpers.CreateChannel, factory, RelaxedExceptionPolicy);
        }
        virtual public void OpenChannel(ChannelType channel)
        {
            _openChannelStats.CallActionAndRecordStats(() => TestHelpers.OpenChannel(channel), RelaxedExceptionPolicy);
        }

        virtual public Task OpenChannelAsync(ChannelType channel)
        {
            return _openChannelAsyncStats.CallAsyncFuncAndRecordStatsAsync(() => TestHelpers.OpenChannelAsync(channel), RelaxedExceptionPolicy);
        }

        virtual public void CloseChannel(ChannelType channel)
        {
            _closeChannelStats.CallActionAndRecordStats(() => TestHelpers.CloseChannel(channel), RelaxedExceptionPolicy);
        }
        virtual public Task CloseChannelAsync(ChannelType channel)
        {
            return _params.PreemptiveCloseChannelTimeout > 0
                ? _closeChannelAsyncStats.CallAsyncFuncAndRecordStatsAsync(() =>
                    TestHelpers.CloseChannelPreemptiveTimeoutAsync(channel, _params.PreemptiveCloseChannelTimeout, failOnPreemptiveTimeout: false), RelaxedExceptionPolicy)
                : _closeChannelAsyncStats.CallAsyncFuncAndRecordStatsAsync(() =>
                    TestHelpers.CloseChannelAsync(channel), RelaxedExceptionPolicy);
        }
        public abstract Func<ChannelType, int> UseChannel();
        public abstract Func<ChannelType, Task<int>> UseAsyncChannel();

        virtual public bool ValidateChannel(ChannelType channel)
        {
            // if the scenario that uses our test may close/fault our channel (RelaxedExceptionPolicy)
            // && if this test parameters (ReplaceInvalidPooledChannels) allow us to replace the objects that went bad
            // then we validate the channel, otherwise we skip the validation
            return (RelaxedExceptionPolicy && _params.ReplaceInvalidPooledChannels) ? TestHelpers.IsCommunicationObjectUsable<ChannelType>(channel) : true;
        }

        virtual public bool ValidateFactory(ChannelFactory<ChannelType> factory)
        {
            return (RelaxedExceptionPolicy && _params.ReplaceInvalidPooledFactories) ? TestHelpers.IsCommunicationObjectUsable<ChannelType>(factory) : true;
        }
    }
}
