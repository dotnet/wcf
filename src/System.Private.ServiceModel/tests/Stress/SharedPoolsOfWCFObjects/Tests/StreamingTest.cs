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
    public class StreamingTest : ITestTemplate<WcfService1.IStreamingService>
    {
        const int MaxStreamSize = 80000; //1671088640;
        int _curStreamSize = 1;
        const int MaxBufferSize = 8 * 1024; // keep it out of LOH 80 * 1024
        int _curBufferSize = 1;

        public StreamingTest() { }
        public EndpointAddress CreateEndPointAddress()
        {
            return TestHelpers.CreateEndPointStreamingAddress();
        }
        public Binding CreateBinding()
        {
            return TestHelpers.CreateStreamingBinding(MaxStreamSize);
        }
        public ChannelFactory<IStreamingService> CreateChannelFactory()
        {
            return TestHelpers.CreateChannelFactory<WcfService1.IStreamingService>(CreateEndPointAddress(), CreateBinding());
        }
        public void CloseFactory(ChannelFactory<WcfService1.IStreamingService> factory)
        {
            TestHelpers.CloseFactory(factory);
        }
        public Task CloseFactoryAsync(ChannelFactory<WcfService1.IStreamingService> factory)
        {
            return TestHelpers.CloseFactoryAsync(factory);
        }

        public WcfService1.IStreamingService CreateChannel(ChannelFactory<WcfService1.IStreamingService> factory)
        {
            return TestHelpers.CreateChannel(factory);
        }

        public void CloseChannel(WcfService1.IStreamingService channel)
        {
            TestHelpers.CloseChannel(channel);
        }

        public Task CloseChannelAsync(WcfService1.IStreamingService channel)
        {
            return TestHelpers.CloseChannelAsync(channel);
        }


        public Func<IStreamingService, Task> UseAsyncChannel()
        {
            return (channel) => { return Task.FromResult<object>(null); };
        }

        public Action<IStreamingService> UseChannel()
        {
            return (channel) =>
            {
                RunAllScenarios(channel);
            };
        }
        public void RunAllScenarios(IStreamingService channel)
        {
            var requestedStreamSize = (Interlocked.Increment(ref _curStreamSize) * 3 + 1) % MaxStreamSize;
            // Are variations of the buffer size important? 
            // Maybe, since just like the stream size they contribute to timing variations...
            var bufSize = Interlocked.Increment(ref _curBufferSize) % MaxBufferSize + 2;
            // Just run all 3 streaming scenarios here
            TestGetStreamFromInt(channel, requestedStreamSize, bufSize);
            TestGetIntFromStream(channel, requestedStreamSize, bufSize);
            TestEchoStream(channel, requestedStreamSize, bufSize);
        }
        private void TestGetStreamFromInt(IStreamingService channel, int requestedStreamSize, int bufSize)
        {
            using (var stream = channel.GetStreamFromInt(requestedStreamSize))
            {
                var receivedStreamSize = VerifyStream(stream, requestedStreamSize, bufSize);
                if (receivedStreamSize != requestedStreamSize)
                {
                    Console.WriteLine("Received stream is different size from the requested one. Expected size: " + requestedStreamSize + " received: " + receivedStreamSize);
                    //if (requestedStreamSize % 384 != 0)
                    System.Diagnostics.Debugger.Break();
                }
            }
        }
        private void TestGetIntFromStream(IStreamingService channel, int requestedStreamSize, int bufSize)
        {
            using (var stream = new VerifiableStream(requestedStreamSize))
            {
                int receivedSize = channel.GetIntFromStream(stream);
                if (receivedSize != requestedStreamSize)
                {
                    Console.WriteLine("We didn't receive the correct stream size. Stream size: " + requestedStreamSize + "received: " + receivedSize);
                    System.Diagnostics.Debugger.Break();
                }
            }
        }
        private void TestEchoStream(IStreamingService channel, int requestedStreamSize, int bufSize)
        {
            using (var stream = new VerifiableStream(requestedStreamSize))
            {
                using (var receivedStream = channel.EchoStream(stream))
                {
                    var receivedSize = VerifyStream(receivedStream, requestedStreamSize, bufSize);
                    if (receivedSize != requestedStreamSize)
                    {
                        Console.WriteLine("Unexpected stream size: " + receivedSize);
                        //if (requestedStreamSize % 384 != 0)
                        System.Diagnostics.Debugger.Break();
                    }
                }
            }
        }

        const int Padding = 16;
        private static int VerifyStream(Stream stream, int expectedStreamSize, int bufSize)
        {
            int bytesRead = 0;
            long totalBytesRead = 0;

            while (true)
            {
                // At this point we really want to make sure the stream implementation is correct
                // So we create a new buffer every time and have padding in it to catch buffer overruns
                var buff = new byte[bufSize + Padding];
                for (int i = 0; i < buff.Length; i++)
                {
                    buff[i] = 48; // '0'
                }

                bytesRead = stream.Read(buffer: buff, offset: 0, count: bufSize);
                //Console.WriteLine("bytes read: " + bytesRead);

                if (bytesRead == 0)
                {
                    if (totalBytesRead != expectedStreamSize)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                    break;
                }

                for (int i = 0; i < bytesRead; i++)
                {
                    //Console.Write(new string((char)buff[i], 1));
                    byte shouldRead = (byte)(65 + (totalBytesRead++ % 26));
                    if (shouldRead != buff[i])
                    {
                        Console.WriteLine("pos: " + totalBytesRead + " should read: " + new string((char)shouldRead, 1) + "actual read: " + new string((char)buff[i], 1));
                        System.Diagnostics.Debugger.Break();
                    }
                    // Catch extra data in stream early
                    if (totalBytesRead > expectedStreamSize)
                    {
                        Console.WriteLine("The stream is longer than expected!");
                        System.Diagnostics.Debugger.Break();
                    }
                }

                // Now, verify our padding
                for (int i= bytesRead; i < buff.Length; i++)
                {
                    if (buff[i] != 48)
                    {
                        Console.WriteLine("The stream.Read corrupted padding around the buffer!");
                        System.Diagnostics.Debugger.Break();
                    }
                }
            }
            return (int)totalBytesRead;
        }

        public class VerifiableStream : Stream
        {
            // Used to control when Read will return 0.
            long _bytesToStream;
            long _totalBytesRead = 0;

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
        }
    }

}
