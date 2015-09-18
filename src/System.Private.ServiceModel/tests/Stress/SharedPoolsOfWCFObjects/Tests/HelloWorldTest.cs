// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace SharedPoolsOfWCFObjects
{
    public class HelloWorldTest : ITestTemplate<WcfService1.IService1>
    {
        public HelloWorldTest() { }
        public EndpointAddress CreateEndPointAddress()
        {
            return TestHelpers.CreateEndPointAddress();
        }

        public Binding CreateBinding()
        {
            return TestHelpers.CreateBinding();
        }

        public ChannelFactory<WcfService1.IService1> CreateChannelFactory()
        {
            return TestHelpers.CreateChannelFactory<WcfService1.IService1>(CreateEndPointAddress(), CreateBinding());
        }
        public void CloseFactory(ChannelFactory<WcfService1.IService1> factory)
        {
            TestHelpers.CloseFactory(factory);
        }
        public Task CloseFactoryAsync(ChannelFactory<WcfService1.IService1> factory)
        {
            return TestHelpers.CloseFactoryAsync(factory);
        }

        public WcfService1.IService1 CreateChannel(ChannelFactory<WcfService1.IService1> factory)
        {
            return TestHelpers.CreateChannel(factory);
        }
        public void CloseChannel(WcfService1.IService1 channel)
        {
            TestHelpers.CloseChannel(channel);
        }
        public Task CloseChannelAsync(WcfService1.IService1 channel)
        {
            return TestHelpers.CloseChannelAsync(channel);
        }

        public Action<WcfService1.IService1> UseChannel()
        {
            return (channel) => { channel.GetData(44); };
        }
        public Func<WcfService1.IService1, Task> UseAsyncChannel()
        {
            return (channel) => { return channel.GetDataAsync(44); };
        }
    }

}
