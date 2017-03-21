// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
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
        // No timeout tracking for perf tests
        virtual public bool TrackServiceCallTimeouts { get { return false; } }
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
        // All stress tests track all service call timeouts
        virtual public bool TrackServiceCallTimeouts { get { return true; } }
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

    public class BasicRequestContext<ChannelType>
    {
        public ChannelType Channel { get; set; }
        public DateTime StartTime { get; set; }
        public CommunicationState InitialCommunicationState { get; set; }
    }

    // Base parent class for the tests that make 1 service call per test iteration
    // It implements all ITestTemplate methods and takes care of additional common tasks such as:
    // - handling most common test parameters
    // - recording timings of factory/channel/service calls
    // - detecting and reporting service request timeouts
    //
    // Derived classes need to override 2 abstract methods
    // UseChannelImpl() and UseAsyncChannelImpl() where they should make the actual service calls
    public abstract class CommonTest<ChannelType, TestParams, CapturedRequestContext> : ITestTemplate<ChannelType, TestParams>, IExceptionPolicy
    where TestParams : IExceptionHandlingPolicyParameter, IPoolTestParameter, IStatsCollectingTestParameter, new()
    where CapturedRequestContext : BasicRequestContext<ChannelType>
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
        protected OperationTimeoutTracker<CapturedRequestContext> _timeoutTracker;

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

            // Figure out the service calls timeout value
            EffectiveTimeoutMs = TestHelpers.SendTimeoutMs;
            if (EffectiveTimeoutMs <= 0)
            {
                // If the timeout was not set then one alternative (to just using the usual default timeout)
                // would be to wait for a binding to be created and then getting its timeout and only then initialize _timeoutTracker. 
                // This involves some additional synchronization which we want to avoid here.
                EffectiveTimeoutMs = 60000;
            }
            if (_params.TrackServiceCallTimeouts)
            {
                _timeoutTracker = new OperationTimeoutTracker<CapturedRequestContext>(EffectiveTimeoutMs, ReportTimeout);
            }
        }

        // IExceptionPolicy:
        virtual public bool RelaxedExceptionPolicy { get; set; }

        // ITestTemplate:
        virtual public TestParams TestParameters
        {
            get
            {
                return _params;
            }
        }
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
        virtual public Func<ChannelType, int> UseChannel()
        {
            return (channel) =>
            {
                using (StartTrackingOperation(channel))
                {
                    return _useChannelStats.CallFuncAndRecordStats(
                        () => UseChannelImpl(channel), RelaxedExceptionPolicy);
                }
            };
        }
        virtual public Func<ChannelType, Task<int>> UseAsyncChannel()
        {
            return async (channel) =>
            {
                using (StartTrackingOperation(channel))
                {
                    return await _useChannelAsyncStats.CallAsyncFuncAndRecordStatsAsync(
                        () => UseAsyncChannelImpl(channel), RelaxedExceptionPolicy);
                }
            };
        }
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

        // children can capture more details about the test request and present these details their own way
        virtual protected CapturedRequestContext CaptureRequestContext(ChannelType channel)
        {
            return new BasicRequestContext<ChannelType>()
            {
                Channel = channel,
                InitialCommunicationState = (channel as ICommunicationObject).State,
                StartTime = DateTime.Now
            } as CapturedRequestContext;
        }
        virtual protected string PrintRequestDetails(CapturedRequestContext requestDetails)
        {
            return String.Format("Initial state {0}, current state {1}, execution time {2}",
                requestDetails.InitialCommunicationState,
                (requestDetails.Channel as ICommunicationObject).State,
                DateTime.Now.Subtract(requestDetails.StartTime).TotalSeconds);
        }

        // abstract methods to make the actual service calls:
        public abstract int UseChannelImpl(ChannelType c);
        public abstract Task<int> UseAsyncChannelImpl(ChannelType c);

        // timetout tracking
        private IDisposable StartTrackingOperation(ChannelType channel)
        {
            if (_params.TrackServiceCallTimeouts)
            {
                return _timeoutTracker.StartTrackingOperation(CaptureRequestContext(channel));
            }
            else
            {
                return null;
            }
        }
        private int EffectiveTimeoutMs { get; set; }
        private void ReportTimeout(CapturedRequestContext[] requestList)
        {
            if (requestList.Length > 0)
            {
                string type = typeof(ChannelType).ToString();
                Console.WriteLine();
                StringBuilder message = new StringBuilder();
                message.AppendFormat(" {0} requests of type {1} timed out.\r\n Their states are:\r\n", requestList.Length, type.ToString());
                foreach (var requestInfo in requestList)
                {
                    message.AppendLine(PrintRequestDetails(requestInfo));
                }
                // also print the longest execution times for comparison
                message.AppendLine("Top 10 async \"sunny day\" recent execution times (s):");
                AppendStats(message, _useChannelAsyncStats.SunnyDay.CurrentTimings);
                message.AppendLine("Top 10 async \"rainy day\" recent execution times (s):");
                AppendStats(message, _useChannelAsyncStats.RainyDay.CurrentTimings);
                message.AppendLine("Top 10 sync \"sunny day\" recent execution times (s):");
                AppendStats(message, _useChannelStats.SunnyDay.CurrentTimings);
                message.AppendLine("Top 10 sync \"rainy day\" recent execution times (s):");
                AppendStats(message, _useChannelStats.RainyDay.CurrentTimings);

                TestUtils.ReportFailure(message: message.ToString(), debugBreak: true);
                GC.KeepAlive(requestList);
            }
        }
        static void AppendStats(StringBuilder message, long[] timings)
        {
            foreach (long ticks in timings.OrderByDescending(t1 => t1).Take(10))
            {
                if (ticks > 0)
                {
                    message.AppendLine(((double)ticks / Stopwatch.Frequency).ToString());
                }
            }
        }
    }

    // Tests that make multiple service calls per test often require per call timeout and timing tracking
    // Unlike its parent CommonTest class (which directly calls into child-provided UseChannelImpl* methods) 
    // this class obtains a request context once and iterates through an array of child-provided funcs
    public abstract class CommonMultiCallTest<ChannelType, TestParams, CapturedRequestContext> : CommonTest<ChannelType, TestParams, CapturedRequestContext>
    where TestParams : IExceptionHandlingPolicyParameter, IPoolTestParameter, IStatsCollectingTestParameter, new()
    where CapturedRequestContext : BasicRequestContext<ChannelType>
    {
        // Override the parent class UseChannel() and UseAsyncChannel() to handle multiple calls differently
        public override Func<ChannelType, int> UseChannel()
        {
            return (channel) =>
            {
                int totalCalls = 0;

                // This mutable call context is captured once per test iteration
                // Both the funcs provided by the child class and StartTrackingOperation are free to modify it
                var crd = CaptureRequestContext (channel);

                foreach (var func in UseChannelImplFuncs())
                {
                    using (StartTrackingOperation(crd))
                    {
                        totalCalls += _useChannelStats.CallFuncAndRecordStats(
                            () => func(crd), RelaxedExceptionPolicy);
                    }
                }
                return totalCalls;
            };
        }
        public override Func<ChannelType, Task<int>> UseAsyncChannel()
        {
            return async (channel) =>
            {
                int totalCalls = 0;
                var crd = CaptureRequestContext(channel);
                foreach (var func in UseChannelAsyncImplFuncs())
                {
                    using (StartTrackingOperation(crd))
                    {
                        totalCalls += await _useChannelStats.CallAsyncFuncAndRecordStatsAsync(
                            () => func(crd), RelaxedExceptionPolicy);
                    }
                }
                return totalCalls;
            };
        }

        // Children classes need to provide a list of funcs, each func makes a signle service call request.
        // The funcs take an instance of CapturedRequestContext which is created by a virtual method CaptureRequestContext
        // Note that the funcs still return int (or Task<int>) to signify how many service calls have been made.
        // This is done to account for special cases like duplex callbacks which are considered separate calls
        public abstract Func<CapturedRequestContext, Task<int>>[] UseChannelAsyncImplFuncs();
        public abstract Func<CapturedRequestContext, int>[] UseChannelImplFuncs();

        // Override parent class abstract methods that will never get called
        public override int UseChannelImpl(ChannelType c) { throw new NotImplementedException(); }
        public override Task<int> UseAsyncChannelImpl(ChannelType c) { throw new NotImplementedException(); }

        private IDisposable StartTrackingOperation(CapturedRequestContext operationDetails)
        {
            if (_params.TrackServiceCallTimeouts)
            {
                // Update channel state and start time in CapturedRequestContext before each call
                operationDetails.InitialCommunicationState = (operationDetails.Channel as ICommunicationObject).State;
                operationDetails.StartTime = DateTime.Now;
                return _timeoutTracker.StartTrackingOperation(operationDetails);
            }
            else
            {
                return null;
            }
        }
    }
}