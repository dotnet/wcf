// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using WcfService1;

namespace SharedPoolsOfWCFObjects
{
    public interface IDuplexParams
    {
        int CallbacksToExpect { get; }
    }
    public class DuplexStressTestParams : CommonStressTestParams, IDuplexParams
    {
        public const int MaxCallbacksToExpect = 10;
        private int _callbacksToExpect = 0;

        // For stress we want to iterate through a wide variety of number of callbacks to exercise all possible timings
        public int CallbacksToExpect
        {
            get
            {
                return Interlocked.Increment(ref _callbacksToExpect) % MaxCallbacksToExpect;
            }
        }
    }

    public class DuplexPerfStartupTestParams : CommonPerfStartupTestParams, IDuplexParams
    {
        // For perf measurements we want a constant number of duplex callbacks
        public const int NumberOfDuplexCallbacks = 1;
        public int CallbacksToExpect { get { return NumberOfDuplexCallbacks; } }
    }

    public class DuplexPerfThroughputTestParams : CommonPerfThroughputTestParams, IDuplexParams
    {
        public const int NumberOfDuplexCallbacks = 1;
        public int CallbacksToExpect { get { return NumberOfDuplexCallbacks; } }
    }

    public class DuplexTest<TestParams> : CommonTest<WcfService1.IDuplexService, TestParams, BasicRequestContext<IDuplexService>>
        where TestParams : IExceptionHandlingPolicyParameter, IPoolTestParameter, IStatsCollectingTestParameter, IDuplexParams, new()
    {
        public override EndpointAddress CreateEndPointAddress()
        {
            return TestHelpers.CreateEndPointDuplexAddress();
        }
        public override ChannelFactory<IDuplexService> CreateChannelFactory()
        {
            var duplexCallback = new DuplexCallback();
            return TestHelpers.CreateDuplexChannelFactory<WcfService1.IDuplexService>(CreateEndPointAddress(), CreateBinding(), duplexCallback);
        }
        public override int UseChannelImpl(IDuplexService channel)
        {
            int callbacks = _params.CallbacksToExpect;
            int result = channel.GetAsyncCallbackData(1, callbacks);
            // we can't guarantee the correctness of the result in case of relaxed exception policy
            if (!RelaxedExceptionPolicy)
            {
                int expected = (1 + callbacks) * callbacks / 2;
                if (result != expected)
                {
                    TestUtils.ReportFailure("Unexpected result. Expected: " + expected + " result: " + result);
                }
            }
            return callbacks + 1;
        }
        public override async Task<int> UseAsyncChannelImpl(IDuplexService channel)
        {
            int callbacks = _params.CallbacksToExpect;
            int result = await channel.GetAsyncCallbackDataAsync(1, callbacks);

            // we can't guarantee the correctness of the result in case of relaxed exception policy
            if (!RelaxedExceptionPolicy)
            {
                int expected = (1 + callbacks) * callbacks / 2;
                if (result != expected)
                {
                    TestUtils.ReportFailure("Unexpected result. Expected: " + expected + "result: " + result);
                }
            }
            // count our call to GetAsyncCallbackDataAsync and each callback call as a separate request
            return callbacks + 1;
        }
    }

    public class DuplexCallback : IDuplexCallback
    {
        public int EchoSetData(int value)
        {
            return value;
        }

        public async Task<int> EchoGetAsyncCallbackData(int value)
        {
            await Task.Yield();
            return value;
        }
    }
}