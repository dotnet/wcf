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
    public interface IStreamingTestParams
    {
        int MaxStreamSize { get; }
        int CurrentStreamSize { get; }
        int MaxBufferSize { get; }
        int CurrentBufferSize { get; }
        bool VerifyStreamContent { get; }
    }
    public enum StreamingScenarios
    {
        StreamNone = 0,
        StreamOut = 1,
        StreamIn = 2,
        StreamEcho = 4,
        StreamAll = StreamOut | StreamIn | StreamEcho
    };

    public class StreamingStressTestParams : CommonStressTestParams, IStreamingTestParams
    {
        public int MaxStreamSize { get { return 800000; } }
        public int MaxBufferSize { get { return 80 * 1024; } }
        private int _curStreamSize = 1;
        public int CurrentStreamSize
        {
            get
            {
                // Variations of the stream size are important for stress so we'll loop through the values
                return (Interlocked.Increment(ref _curStreamSize) * 3 + 1) % MaxStreamSize;
            }
        }
        private int _curBufferSize = 1;
        public int CurrentBufferSize
        {
            get
            {
                // Variations of the buffer size are important for stress so we'll loop through the values
                return Interlocked.Increment(ref _curBufferSize) % MaxBufferSize + 2;
            }
        }
        public bool VerifyStreamContent { get { return true; } }
    }

    // For perf we want to be more specific about what streaming scenario we use
    // so we have a few different parameters to select as a generic parameter to the test
    public abstract class StreamingPerfTestParamsBase : CommonPerfStartupTestParams, IStreamingTestParams
    {
        protected const int TheMaxStreamSize = 80000;
        protected const int TheMaxBufferSize = 8 * 1024;
        public int MaxStreamSize { get { return TheMaxStreamSize; } }
        public int MaxBufferSize { get { return TheMaxBufferSize; } }
        public bool VerifyStreamContent { get { return false; } }
        public abstract int CurrentStreamSize { get; }
        public abstract int CurrentBufferSize { get; }
        public static StreamingScenarios StreamingScenario { get; set; }
    }


    public class StreamingPerfStartupTestParams : StreamingPerfTestParamsBase
    {
        public override int CurrentStreamSize
        {
            get
            {
                // we use an arbitrary small number for stream size for the startup perf scenario
                return 512;
            }
        }
        public override int CurrentBufferSize
        {
            get
            {
                // we just use a fixed size buffer for the startup perf scenario
                return CurrentStreamSize / 2;
            }
        }
    }

    public class StreamingPerfThroughputTestParams : StreamingPerfTestParamsBase
    {
        // Throughput perf tests are likely to run into different kind of bottlenecks than startup perf tests
        // Therefore we loop through a predefined small set of stream sizes
        private int[] _streamSizes = { 10, TheMaxStreamSize / 10, TheMaxStreamSize };
        private int _curStreamSizeIndex = 0;
        public override int CurrentStreamSize
        {
            get
            {
                return _streamSizes[Interlocked.Increment(ref _curStreamSizeIndex) % _streamSizes.Length];
            }
        }
        public override int CurrentBufferSize { get { return MaxBufferSize; } }

        override public int MaxPooledChannels { get { return 8; } }
        override public int MaxPooledFactories { get { return 12; } }
    }


    public class StreamingTest<StreamingService, TestParams> : CommonTest<StreamingService, TestParams>
        where TestParams : IExceptionHandlingPolicyParameter, IPoolTestParameter, IStatsCollectingTestParameter, IStreamingTestParams, new()
        where StreamingService : IStreamingService
    {
        public override Binding CreateBinding()
        {
            return TestHelpers.CreateStreamingBinding(_params.MaxStreamSize);
        }

        public override EndpointAddress CreateEndPointAddress()
        {
            return TestHelpers.CreateEndPointStreamingAddress();
        }

        public override Func<StreamingService, Task<int>> UseAsyncChannel()
        {
            return async (channel) =>
                await _useChannelAsyncStats.CallAsyncFuncAndRecordStatsAsync(() => RunAllScenariosAsync(channel), RelaxedExceptionPolicy);
        }
        public async Task<int> RunAllScenariosAsync(StreamingService channel)
        {
            var requestedStreamSize = _params.CurrentStreamSize;
            var bufSize = _params.CurrentBufferSize;
            int requestsMade = 0;
            if ((StreamingPerfTestParamsBase.StreamingScenario & StreamingScenarios.StreamIn) != 0)
            {
                await TestGetStreamFromIntAsync(channel, requestedStreamSize, bufSize);
                requestsMade++;
            }
            if ((StreamingPerfTestParamsBase.StreamingScenario & StreamingScenarios.StreamOut) != 0)
            {
                await TestGetIntFromStreamAsync(channel, requestedStreamSize, bufSize);
                requestsMade++;
            }
            if ((StreamingPerfTestParamsBase.StreamingScenario & StreamingScenarios.StreamEcho) != 0)
            {
                await TestEchoStreamAsync(channel, requestedStreamSize, bufSize);
                requestsMade++;
            }

            return requestsMade;
        }
        private async Task TestGetStreamFromIntAsync(StreamingService channel, int requestedStreamSize, int bufSize)
        {
            using (var stream = await channel.GetStreamFromIntAsync(requestedStreamSize))
            {
                var receivedStreamSize = await VerifiableStream.VerifyStreamAsync(stream, requestedStreamSize, bufSize, _params.VerifyStreamContent);
                if (receivedStreamSize != requestedStreamSize)
                {
                    VerifiableStream.ReportIncorrectStreamSize(receivedStreamSize, requestedStreamSize);
                }
            }
        }
        private async Task TestGetIntFromStreamAsync(StreamingService channel, int requestedStreamSize, int bufSize)
        {
            using (var stream = new VerifiableStream(requestedStreamSize))
            {
                int receivedSize = await channel.GetIntFromStreamAsync(stream);
                if (receivedSize != requestedStreamSize)
                {
                    VerifiableStream.ReportIncorrectStreamSize(receivedSize, requestedStreamSize);
                }
            }
        }
        private async Task TestEchoStreamAsync(StreamingService channel, int requestedStreamSize, int bufSize)
        {
            using (var stream = new VerifiableStream(requestedStreamSize))
            {
                using (var receivedStream = await channel.EchoStreamAsync(stream))
                {
                    var receivedSize = await VerifiableStream.VerifyStreamAsync(receivedStream, requestedStreamSize, bufSize, _params.VerifyStreamContent);
                    if (receivedSize != requestedStreamSize)
                    {
                        VerifiableStream.ReportIncorrectStreamSize(receivedSize, requestedStreamSize);
                    }
                }
            }
        }

        public override Func<StreamingService, int> UseChannel()
        {
            return (channel) =>
            {
                return _useChannelStats.CallFuncAndRecordStats(() => RunAllScenarios(channel), RelaxedExceptionPolicy);
            };
        }
        public int RunAllScenarios(StreamingService channel)
        {
            var requestedStreamSize = _params.CurrentStreamSize;
            // Are variations of the buffer size important? 
            // Maybe, since just like the stream size they contribute to timing variations...
            var bufSize = _params.CurrentBufferSize;
            int requestsMade = 0;

            if ((StreamingPerfTestParamsBase.StreamingScenario & StreamingScenarios.StreamIn) != 0)
            {
                TestGetStreamFromInt(channel, requestedStreamSize, bufSize);
                requestsMade++;
            }
            if ((StreamingPerfTestParamsBase.StreamingScenario & StreamingScenarios.StreamOut) != 0)
            {
                TestGetIntFromStream(channel, requestedStreamSize, bufSize);
                requestsMade++;
            }
            if ((StreamingPerfTestParamsBase.StreamingScenario & StreamingScenarios.StreamEcho) != 0)
            {
                TestEchoStream(channel, requestedStreamSize, bufSize);
                requestsMade++;
            }
            return requestsMade;
        }
        private void TestGetStreamFromInt(StreamingService channel, int requestedStreamSize, int bufSize)
        {
            using (var stream = channel.GetStreamFromInt(requestedStreamSize))
            {
                var receivedStreamSize = VerifiableStream.VerifyStream(stream, requestedStreamSize, bufSize, _params.VerifyStreamContent);
                if (receivedStreamSize != requestedStreamSize)
                {
                    VerifiableStream.ReportIncorrectStreamSize(receivedStreamSize, requestedStreamSize);
                }
            }
        }
        private void TestGetIntFromStream(StreamingService channel, int requestedStreamSize, int bufSize)
        {
            using (var stream = new VerifiableStream(requestedStreamSize))
            {
                int receivedSize = channel.GetIntFromStream(stream);

                if (receivedSize != requestedStreamSize)
                {
                    VerifiableStream.ReportIncorrectStreamSize(receivedSize, requestedStreamSize);
                }
            }
        }
        private void TestEchoStream(StreamingService channel, int requestedStreamSize, int bufSize)
        {
            using (var stream = new VerifiableStream(requestedStreamSize))
            {
                using (var receivedStream = channel.EchoStream(stream))
                {
                    var receivedSize = VerifiableStream.VerifyStream(receivedStream, requestedStreamSize, bufSize, _params.VerifyStreamContent);

                    if (receivedSize != requestedStreamSize)
                    {
                        VerifiableStream.ReportIncorrectStreamSize(receivedSize, requestedStreamSize);
                    }
                }
            }
        }
    }
    public class VerifiableStream : Stream
    {
        // Used to control when Read will return 0.
        private long _bytesToStream;
        private long _totalBytesRead = 0;

        public VerifiableStream(long bytesToStream)
        {
            _bytesToStream = bytesToStream;
        }
        public bool StopStreaming { get; set; }

        public override bool CanRead
        {
            get { return !StopStreaming; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                return _totalBytesRead;
            }
            set
            {
                _totalBytesRead = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_totalBytesRead >= _bytesToStream)
            {
                StopStreaming = true;
            }
            if (StopStreaming)
            {
                buffer[offset] = 0;
                return 0;
            }

            // Adjust count if we're approaching BytesToStream limit
            if (_totalBytesRead + count > _bytesToStream)
            {
                count = (int)(_bytesToStream - _totalBytesRead);
                // This might be not necessary
                if (buffer.Length > offset + _bytesToStream - _totalBytesRead)
                {
                    buffer[offset + _bytesToStream - _totalBytesRead] = 0;
                }
            }

            for (int i = 0; i < count; i++)
            {
                buffer[offset + i] = (byte)(65 + (_totalBytesRead++ % 26));
            }
            return count;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        private const int Padding = 16;
        public static int VerifyStream(Stream stream, int expectedStreamSize, int bufSize, bool verifyContent)
        {
            int bytesRead = 0;
            int totalBytesRead = 0;

            while (true)
            {
                // At this point we really want to make sure the stream implementation is correct
                // So we create a new buffer every time and have padding in it to catch buffer overruns
                // This might be an overkill for perf - consider controlling it with StreamingTestParams
                // Depending on the size of the buffer we might also periodically skip it in stress
                var buff = new byte[bufSize + Padding];
                for (int i = 0; i < buff.Length; i++)
                {
                    buff[i] = 48; // '0'
                }

                bytesRead = stream.Read(buffer: buff, offset: 0, count: bufSize);

                if (bytesRead == 0)
                {
                    break;
                }

                if (!verifyContent)
                {
                    totalBytesRead += bytesRead;
                }
                else
                {
                    for (int i = 0; i < bytesRead; i++)
                    {
#if DIAG_TRACE
                        Console.Write((char)buff[i]);
#endif
                        byte shouldRead = (byte)(65 + (totalBytesRead++ % 26));
                        if (shouldRead != buff[i])
                        {
                            TestUtils.ReportFailure("pos: " + totalBytesRead + " should read: " + new string((char)shouldRead, 1) + "actual read: " + new string((char)buff[i], 1));
                        }
                        // Catch extra data in stream early (if we know its expected size)
                        if (expectedStreamSize >= 0 && totalBytesRead > expectedStreamSize)
                        {
                            ReportIncorrectStreamSize(totalBytesRead, expectedStreamSize);
                        }
                    }

                    // Now, verify our padding
                    for (int i = bytesRead; i < buff.Length; i++)
                    {
                        if (buff[i] != 48)
                        {
                            TestUtils.ReportFailure("The stream.Read corrupted padding around the buffer!");
                        }
                    }
                }
            }
            return (int)totalBytesRead;
        }

        public static async Task<int> VerifyStreamAsync(Stream stream, int expectedStreamSize, int bufSize, bool verifyContent)
        {
#if DIAG_TRACE
Console.WriteLine("bufSize "+bufSize);
#endif

            int bytesRead = 0;
            int totalBytesRead = 0;

            while (true)
            {
                // At this point we really want to make sure the stream implementation is correct
                // So we create a new buffer every time and have padding in it to catch buffer overruns
                // This might be an overkill for perf - consider controlling it with StreamingTestParams
                // Depending on the size of the buffer we might also periodically skip it in stress
                var buff = new byte[bufSize + Padding];
                for (int i = 0; i < buff.Length; i++)
                {
                    buff[i] = 48; // '0'
                }

                bytesRead = await stream.ReadAsync(buffer: buff, offset: 0, count: bufSize);

                if (bytesRead == 0)
                {
                    break;
                }
                if (!verifyContent)
                {
                    totalBytesRead += bytesRead;
                }
                else
                {
#if DIAG_TRACE
                    var buf = System.Text.Encoding.UTF8.GetString(buff).Substring(0, bytesRead);
                    Console.WriteLine(buf);
                    Console.WriteLine(expectedStreamSize + " " + totalBytesRead+ " " + bytesRead);
#endif
                    for (int i = 0; i < bytesRead; i++)
                    {
#if DIAG_TRACE
                        Console.Write((char)buff[i]);
#endif
                        byte shouldRead = (byte)(65 + (totalBytesRead++ % 26));
                        if (shouldRead != buff[i])
                        {
                            TestUtils.ReportFailure("pos: " + totalBytesRead + " should read: " + new string((char)shouldRead, 1) + "actual read: " + new string((char)buff[i], 1));
                        }
                        // Catch extra data in stream early (if we know its expected size)
                        if (expectedStreamSize >= 0 && totalBytesRead > expectedStreamSize)
                        {
                            ReportIncorrectStreamSize(totalBytesRead, expectedStreamSize);
                        }
                    }

                    // Now, verify our padding
                    for (int i = bytesRead; i < buff.Length; i++)
                    {
                        if (buff[i] != 48)
                        {
                            TestUtils.ReportFailure("The stream.Read corrupted padding around the buffer!");
                        }
                    }
                }
            }
            return (int)totalBytesRead;
        }

        public static void ReportIncorrectStreamSize(int incorrectStreamSize, int expectedStreamSize)
        {
            TestUtils.ReportFailure(String.Format("Unexpected stream size: {0}. Expected size: {1}", incorrectStreamSize, expectedStreamSize));
        }
    }
}
