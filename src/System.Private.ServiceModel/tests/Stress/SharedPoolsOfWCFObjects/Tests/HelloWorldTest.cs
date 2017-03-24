// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using WcfService1;

namespace SharedPoolsOfWCFObjects
{
    public class HelloWorldTest<TestParams> : CommonTest<IService1, TestParams, BasicRequestContext<IService1>>
        where TestParams : IExceptionHandlingPolicyParameter, IPoolTestParameter, IStatsCollectingTestParameter, new()
    {
        public override int UseChannelImpl(IService1 channel)
        {
            channel.GetData(44);
            return 1;
        }
        public override async Task<int> UseAsyncChannelImpl(IService1 channel)
        {
            await channel.GetDataAsync(44);
            return 1;
        }
    }

    public class HelloWorldAPMTest<TestParams> : HelloWorldTest<TestParams>
        where TestParams : IExceptionHandlingPolicyParameter, IPoolTestParameter, IStatsCollectingTestParameter, new()
    {
        public override async Task<int> UseAsyncChannelImpl(IService1 channel)
        {
            await Task.Factory.FromAsync(channel.BeginGetData, channel.EndGetData, 44, null);
            return 1;
        }
    }
}