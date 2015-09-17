// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using WcfService1;

namespace SharedPoolsOfWCFObjects
{
    public class DuplexTest : ITestTemplate<WcfService1.IDuplexService>
    {
        const int MaxCallbacksToExpect = 100;
        int _callbacksToExpect = 0;

        public DuplexTest() { }
        public EndpointAddress CreateEndPointAddress()
        {
            return TestHelpers.CreateEndPointDuplexAddress();
        }

        public Binding CreateBinding()
        {
            return TestHelpers.CreateBinding();
        }

        public ChannelFactory<WcfService1.IDuplexService> CreateChannelFactory()
        {
            var duplexCallback = new DuplexCallback();
            return TestHelpers.CreateDuplexChannelFactory<WcfService1.IDuplexService>(CreateEndPointAddress(), CreateBinding(), duplexCallback);
        }
        public void CloseFactory(ChannelFactory<WcfService1.IDuplexService> factory)
        {
            TestHelpers.CloseFactory(factory);
        }

        public Task CloseFactoryAsync(ChannelFactory<WcfService1.IDuplexService> factory)
        {
            return TestHelpers.CloseFactoryAsync(factory);
        }

        public WcfService1.IDuplexService CreateChannel(ChannelFactory<WcfService1.IDuplexService> factory)
        {
            return TestHelpers.CreateChannel(factory);
        }
        public void CloseChannel(WcfService1.IDuplexService channel)
        {
            TestHelpers.CloseChannel<WcfService1.IDuplexService>(channel);
        }
        public Task CloseChannelAsync(WcfService1.IDuplexService channel)
        {
            return TestHelpers.CloseChannelAsync<WcfService1.IDuplexService>(channel);
        }

        public Action<WcfService1.IDuplexService> UseChannel()
        {
            return (channel) => {
                //int callbacks = GetCallbacksToExpect();
                //int result = channel.SetData(44, callbacks);
                //// rather than counting the number of the callabacks 
                //// we check the return value which should be incremented by the number of the callaback calls
                //if (result != 44 + callbacks)
                //{
                //    Console.WriteLine("Unexpected number of callbacks!");
                //    System.Diagnostics.Debugger.Break();
                //}


                int callbacks = GetCallbacksToExpect();
                int result = channel.GetAsyncCallbackData(1, callbacks);
                //Console.WriteLine("callbacks: " + callbacks + ", result " + result);
                if (result != (1 + callbacks) * callbacks / 2)
                {
                    Console.WriteLine("Unexpected result.");
                    System.Diagnostics.Debugger.Break();
                }
            };
        }

        public Func<WcfService1.IDuplexService, Task> UseAsyncChannel()
        {
            return async (channel) => {
                int callbacks = GetCallbacksToExpect();
                int result = await channel.SetDataAsync(1, callbacks);
                if (result != 1 + callbacks)
                {
                    Console.WriteLine("Unexpected number of callbacks!");
                    System.Diagnostics.Debugger.Break();
                }
            };
        }

        private void RunAllScenarios()
        {
        }

        private int GetCallbacksToExpect()
        {
            return Interlocked.Increment(ref _callbacksToExpect) % MaxCallbacksToExpect;
        }
    }


    public class DuplexCallback : IDuplexCallback
    {

        //public Task<int> EchoSetData(int value)
        //{
        //    System.Console.WriteLine("EchoSetData: " + value);
        //    return Task.FromResult(value + 1);
        //}

        public int EchoSetData(int value)
        {
            //System.Console.WriteLine("EchoSetData: " + value);
            return value;
        }

        public async Task<int> EchoGetAsyncCallbackData(int value)
        {
            await Task.Yield();
            //Console.WriteLine("returning "+ value);
            return value;
        }

        /*
        public async Task<int> NestedEchoCallback(int value, int nestedCallsRemaining)
        {
            // we need a channel here...
        }
        */
    }
}
