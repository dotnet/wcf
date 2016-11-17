// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace SharedPoolsOfWCFObjects
{
    public class HelloWorldTest<TestParams> : CommonTest<WcfService1.IService1, TestParams>
        where TestParams : IExceptionHandlingPolicyParameter, IPoolTestParameter, IStatsCollectingTestParameter, new()
    {
        public override Func<WcfService1.IService1, int> UseChannel()
        {
            return (channel) =>
            {
                _useChannelStats.CallActionAndRecordStats(() => channel.GetData(44), RelaxedExceptionPolicy);
                return 1;
            };
        }
        public override Func<WcfService1.IService1, Task<int>> UseAsyncChannel()
        {
            return async (channel) =>
            {
                await _useChannelAsyncStats.CallAsyncFuncAndRecordStatsAsync(() => channel.GetDataAsync(44), RelaxedExceptionPolicy);
                return 1;
            };
        }
    }

    public class HelloWorldAPMTest<TestParams> : HelloWorldTest<TestParams>
        where TestParams : IExceptionHandlingPolicyParameter, IPoolTestParameter, IStatsCollectingTestParameter, new()
    {
        public override Func<WcfService1.IService1, Task<int>> UseAsyncChannel()
        {
            return async (channel) =>
            {
                await _useChannelAsyncStats.CallAsyncFuncAndRecordStatsAsync(() => Task.Factory.FromAsync(channel.BeginGetData, channel.EndGetData, 44, null),
                    RelaxedExceptionPolicy);
                return 1;
            };
        }
    }
}
