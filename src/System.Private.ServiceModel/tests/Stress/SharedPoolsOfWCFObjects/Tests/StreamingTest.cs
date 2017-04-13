// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        StreamEcho = 3,
        StreamAll = 4
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

    public class StreamingRequestContext<ChannelType> : BasicRequestContext<ChannelType>
    {
        public StreamingScenarios TestScenario { get; set; }
        public int StreamSize { get; set; }
        public int BufferSize { get; set; }
        public bool VerifyStreamContent { get; set; }
        public Stream StreamIn { get; set; }
        public Stream StreamOut { get; set; }
    }

    public class StreamingTest<StreamingService, TestParams> : CommonMultiCallTest<StreamingService, TestParams, StreamingRequestContext<StreamingService>>
        where TestParams : IExceptionHandlingPolicyParameter, IPoolTestParameter, IStatsCollectingTestParameter, IStreamingTestParams, new()
        where StreamingService : class, IStreamingService
    {
        private static Func<StreamingRequestContext<StreamingService>, Task<int>>[][] s_asyncFuncs = new Func<StreamingRequestContext<StreamingService>, Task<int>>[][]
        {
            new Func<StreamingRequestContext<StreamingService>, Task<int>>[] { },                           // StreamNone = 0
            new Func<StreamingRequestContext<StreamingService>, Task<int>>[] { TestGetIntFromStreamAsync }, // StreamOut = 1
            new Func<StreamingRequestContext<StreamingService>, Task<int>>[] { TestGetStreamFromIntAsync }, // StreamIn = 2
            new Func<StreamingRequestContext<StreamingService>, Task<int>>[] { TestEchoStreamAsync },       // StreamEcho = 3
            new Func<StreamingRequestContext<StreamingService>, Task<int>>[] {
                TestGetIntFromStreamAsync, TestGetStreamFromIntAsync, TestEchoStreamAsync                   // StreamAll = 4
            }
        };
        private static Func<StreamingRequestContext<StreamingService>, int>[][] s_syncFuncs = new Func<StreamingRequestContext<StreamingService>, int>[][]
        {
            new Func<StreamingRequestContext<StreamingService>, int>[]{ },                                  // StreamNone = 0
            new Func<StreamingRequestContext<StreamingService>, int>[]{ TestGetIntFromStream },             // StreamOut = 1
            new Func<StreamingRequestContext<StreamingService>, int>[]{ TestGetStreamFromInt },             // StreamIn = 2
            new Func<StreamingRequestContext<StreamingService>, int>[]{ TestEchoStream },                   // StreamEcho = 3
            new Func<StreamingRequestContext<StreamingService>, int>[]{ TestGetStreamFromInt, TestGetIntFromStream, TestEchoStream }    // StreamAll = 4
        };

        public override Binding CreateBinding()
        {
            return TestHelpers.CreateStreamingBinding(_params.MaxStreamSize);
        }

        public override EndpointAddress CreateEndPointAddress()
        {
            return TestHelpers.CreateEndPointStreamingAddress();
        }

        public override Func<StreamingRequestContext<StreamingService>, int>[] UseChannelImplFuncs()
        {
            return s_syncFuncs[(int)StreamingPerfTestParamsBase.StreamingScenario];
        }

        public override Func<StreamingRequestContext<StreamingService>, Task<int>>[] UseChannelAsyncImplFuncs()
        {
            return s_asyncFuncs[(int)StreamingPerfTestParamsBase.StreamingScenario];
        }
        protected override StreamingRequestContext<StreamingService> CaptureRequestContext(StreamingService channel)
        {
            return new StreamingRequestContext<StreamingService>()
            {
                Channel = channel,
                // skip InitialCommunicationState, StartTime, and TestScenario - they get updated before each service call
                StreamSize = _params.CurrentStreamSize,
                BufferSize = _params.CurrentBufferSize,
                VerifyStreamContent = _params.VerifyStreamContent
            };
        }
        protected override string PrintRequestDetails(StreamingRequestContext<StreamingService> requestDetails)
        {
            return base.PrintRequestDetails(requestDetails) + 
                String.Format (", TestScenario {0}, StreamSize {1}, StreamInPosition {2}, StreamOutPosition {3}", requestDetails.TestScenario, requestDetails.StreamSize, requestDetails.StreamIn?.Position, requestDetails.StreamOut?.Position);
        }

        private static async Task<int> TestGetStreamFromIntAsync(StreamingRequestContext<StreamingService> details)
        {
            details.TestScenario = StreamingScenarios.StreamIn; // update the scenario
            using (var stream = await details.Channel.GetStreamFromIntAsync(details.StreamSize))
            {
                details.StreamIn = stream;
                var receivedStreamSize = await VerifiableStream.VerifyStreamAsync(stream, details.StreamSize, details.BufferSize, details.VerifyStreamContent);
                if (receivedStreamSize != details.StreamSize)
                {
                    VerifiableStream.ReportIncorrectStreamSize(receivedStreamSize, details.StreamSize);
                }
            }
            return 1;
        }
        private static async Task<int> TestGetIntFromStreamAsync(StreamingRequestContext<StreamingService> details)
        {
            details.TestScenario = StreamingScenarios.StreamOut;
            using (var stream = new VerifiableStream(details.StreamSize))
            {
                details.StreamOut = stream;
                int receivedSize = await details.Channel.GetIntFromStreamAsync(stream);
                if (receivedSize != details.StreamSize)
                {
                    VerifiableStream.ReportIncorrectStreamSize(receivedSize, details.StreamSize);
                }
            }
            return 1;
        }
        private static async Task<int> TestEchoStreamAsync(StreamingRequestContext<StreamingService> details)
        {
            details.TestScenario = StreamingScenarios.StreamEcho;
            using (var stream = new VerifiableStream(details.StreamSize))
            {
                details.StreamOut = stream;
                using (var receivedStream = await details.Channel.EchoStreamAsync(stream))
                {
                    details.StreamIn = receivedStream;
                    var receivedSize = await VerifiableStream.VerifyStreamAsync(receivedStream, details.StreamSize, details.BufferSize, details.VerifyStreamContent);
                    if (receivedSize != details.StreamSize)
                    {
                        VerifiableStream.ReportIncorrectStreamSize(receivedSize, details.StreamSize);
                    }
                }
            }
            return 1;
        }
        private static int TestGetStreamFromInt(StreamingRequestContext<StreamingService> details)
        {
            details.TestScenario = StreamingScenarios.StreamIn;
            bool verifyStreamContent = details.VerifyStreamContent;
            using (var stream = details.Channel.GetStreamFromInt(details.StreamSize))
            {
                details.StreamIn = stream;
                var receivedStreamSize = VerifiableStream.VerifyStream(stream, details.StreamSize, details.BufferSize, verifyStreamContent);
                if (receivedStreamSize != details.StreamSize)
                {
                    VerifiableStream.ReportIncorrectStreamSize(receivedStreamSize, details.StreamSize);
                }
            }
            return 1;
        }
        private static int TestGetIntFromStream(StreamingRequestContext<StreamingService> details)
        {
            details.TestScenario = StreamingScenarios.StreamOut;
            using (var stream = new VerifiableStream(details.StreamSize))
            {
                details.StreamOut = stream;
                int receivedSize = details.Channel.GetIntFromStream(stream);

                if (receivedSize != details.StreamSize)
                {
                    VerifiableStream.ReportIncorrectStreamSize(receivedSize, details.StreamSize);
                }
            }
            return 1;
        }
        private static int TestEchoStream(StreamingRequestContext<StreamingService> details)
        {
            details.TestScenario = StreamingScenarios.StreamEcho;
            using (var stream = new VerifiableStream(details.StreamSize))
            {
                details.StreamOut = stream;
                using (var receivedStream = details.Channel.EchoStream(stream))
                {
                    details.StreamIn = receivedStream;
                    var receivedSize = VerifiableStream.VerifyStream(receivedStream, details.StreamSize, details.BufferSize, details.VerifyStreamContent);

                    if (receivedSize != details.StreamSize)
                    {
                        VerifiableStream.ReportIncorrectStreamSize(receivedSize, details.StreamSize);
                    }
                }
            }
            return 1;
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