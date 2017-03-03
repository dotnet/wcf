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
        public override Func<WcfService1.IService1, int> UseChannelImpl()
        {
            return (channel) =>
            {
                channel.GetData(44);
                return 1;
            };
        }
        public override Func<WcfService1.IService1, Task<int>> UseAsyncChannelImpl()
        {
            return async (channel) =>
            {
                await channel.GetDataAsync(44);
                return 1;
            };
        }
    }

    public class HelloWorldAPMTest<TestParams> : HelloWorldTest<TestParams>
        where TestParams : IExceptionHandlingPolicyParameter, IPoolTestParameter, IStatsCollectingTestParameter, new()
    {
        public override Func<WcfService1.IService1, Task<int>> UseAsyncChannelImpl()
        {
            return async (channel) =>
            {
                await Task.Factory.FromAsync(channel.BeginGetData, channel.EndGetData, 44, null);
                return 1;
            };
        }
    }
}
