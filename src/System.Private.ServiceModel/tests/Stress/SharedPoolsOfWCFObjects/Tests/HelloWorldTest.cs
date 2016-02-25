// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace SharedPoolsOfWCFObjects
{
    public class HelloWorldTest<TestParams> : CommonTest<WcfService1.IService1, TestParams>
        where TestParams : IPoolTestParameter, IStatsCollectingTestParameter, new()
    {
        public override Action<WcfService1.IService1> UseChannel()
        {
            return (channel) =>
                _useChannelStats.CallActionAndRecordStats(() => channel.GetData(44), RelaxedExceptionPolicy);
        }
        public override Func<WcfService1.IService1, Task> UseAsyncChannel()
        {
            return (channel) =>
                _useChannelAsyncStats.CallAsyncFuncAndRecordStatsAsync(() => channel.GetDataAsync(44), RelaxedExceptionPolicy);
        }
    }
}
