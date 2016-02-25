// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using WcfService1;

namespace SharedPoolsOfWCFObjects
{
    // This is essentially a StreamingTest + DuplexTest - each streaming call is mirrored by a callback from the server
    public interface IDuplexStreamingTestParams : IStreamingTestParams, IDuplexParams
    {
    }

    public class DuplexStreamingStressTestParams : StreamingStressTestParams, IDuplexStreamingTestParams
    {
        private int _callbacksToExpect = 0;

        // For stress we want to iterate through a wide variety of number of callbacks to exercise all possible timings
        public int CallbacksToExpect
        {
            get
            {
                return Interlocked.Increment(ref _callbacksToExpect) % DuplexStressTestParams.MaxCallbacksToExpect;
            }
        }
    }
    public class DuplexStreamingPerfStartupTestParams : StreamingPerfStartupTestParams, IDuplexStreamingTestParams
    {
        public int CallbacksToExpect { get { return DuplexPerfStartupTestParams.NumberOfDuplexCallbacks; } }
    }
    public class DuplexStreamingPerfThroughputTestParams : StreamingPerfThroughputTestParams, IDuplexStreamingTestParams
    {
        public int CallbacksToExpect { get { return DuplexPerfThroughputTestParams.NumberOfDuplexCallbacks; } }
    }

    internal class DuplexStreamingTest<StreamingService, TestParams> : StreamingTest<StreamingService, TestParams>
        where TestParams : IPoolTestParameter, IStatsCollectingTestParameter, IDuplexStreamingTestParams, new()
        where StreamingService : IDuplexStreamingService
    {
        public override EndpointAddress CreateEndPointAddress()
        {
            return TestHelpers.CreateEndPointStreamingAddress();
        }
        public override Binding CreateBinding()
        {
            return TestHelpers.CreateStreamingBinding(_params.MaxStreamSize);
        }
        public override ChannelFactory<StreamingService> CreateChannelFactory()
        {
            var duplexCallback = new DuplexStreamingCallback();
            return TestHelpers.CreateDuplexChannelFactory<StreamingService>(CreateEndPointAddress(), CreateBinding(), duplexCallback);
        }
    }

    public class DuplexStreamingCallback : IDuplexStreamingCallback
    {
        public Stream EchoStream(Stream stream)
        {
            return stream;
        }

        public int GetIntFromStream(Stream stream)
        {
            // we don't know the stream size 
            // If we care much about the buffer size it should go to IDuplexStreamingTestParams
            return VerifiableStream.VerifyStream(stream, -1, 4096);
        }

        public Stream GetStreamFromInt(int bytesToStream)
        {
            var stream = new VerifiableStream(bytesToStream);
            OperationContext clientContext = OperationContext.Current;
            clientContext.OperationCompleted += new EventHandler(delegate (object sender, EventArgs args)
            {
                stream.Dispose();
            });
            return stream;
        }
    }
}
