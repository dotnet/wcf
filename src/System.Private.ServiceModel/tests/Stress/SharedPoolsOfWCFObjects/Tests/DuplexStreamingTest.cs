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
    // This is essentially a StreamingTest where each streaming call is mirrored by exactly one duplex callback from the server
    // Currently we don't have any additional test parameters for the duplex part, but we keep this interface for future flexibility
    public interface IDuplexStreamingTestParams
    {
    }

    public class DuplexStreamingStressTestParams : StreamingStressTestParams, IDuplexStreamingTestParams
    {
    }
    public class DuplexStreamingPerfStartupTestParams : StreamingPerfStartupTestParams, IDuplexStreamingTestParams
    {
    }

    public class DuplexStreamingPerfThroughputTestParams : StreamingPerfThroughputTestParams, IDuplexStreamingTestParams
    {
        public override int MaxPooledChannels
        {
            get
            {
                return 8;
            }
        }
        public override int MaxPooledFactories
        {
            get
            {
                return 12;
            }
        }
    }

    internal class DuplexStreamingTest<StreamingService, TestParams> : StreamingTest<StreamingService, TestParams>
        where TestParams : IExceptionHandlingPolicyParameter, IPoolTestParameter, IStatsCollectingTestParameter, IDuplexStreamingTestParams, IStreamingTestParams, new()
        where StreamingService : class, IDuplexStreamingService
    {
        public override EndpointAddress CreateEndPointAddress()
        {
            return TestHelpers.CreateEndPointDuplexStreamingAddress();
        }
        public override Binding CreateBinding()
        {
            return TestHelpers.CreateStreamingBinding(_params.MaxStreamSize);
        }
        public override ChannelFactory<StreamingService> CreateChannelFactory()
        {
            var duplexCallback = new DuplexStreamingCallback(_params.VerifyStreamContent);
            return TestHelpers.CreateDuplexChannelFactory<StreamingService>(CreateEndPointAddress(), CreateBinding(), duplexCallback);
        }
    }

    public class DuplexStreamingCallback : IDuplexStreamingCallback
    {
        private static long s_echoStreamCallbacks = 0;
        private static long s_getIntFromStreamCallbacks = 0;
        private static long s_getStreamFromIntCallbacks = 0;
        private bool _verifyContent;

        public DuplexStreamingCallback(bool verifyStreamContent)
        {
            _verifyContent = verifyStreamContent;
        }
        public Stream EchoStream(Stream stream)
        {
            Interlocked.Increment(ref s_echoStreamCallbacks);
            return stream;
        }
        public int GetIntFromStream(Stream stream)
        {
            Interlocked.Increment(ref s_getIntFromStreamCallbacks);
            // we don't know the stream size 
            // If we care much about the buffer size it should go to IDuplexStreamingTestParams
            return VerifiableStream.VerifyStream(stream, -1, 4096, _verifyContent);
        }
        public Stream GetStreamFromInt(int bytesToStream)
        {
            Interlocked.Increment(ref s_getStreamFromIntCallbacks);
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
