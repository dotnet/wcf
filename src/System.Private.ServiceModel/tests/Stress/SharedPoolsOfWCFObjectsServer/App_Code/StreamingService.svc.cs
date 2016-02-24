// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace WcfService1
{
    public class StreamingService : IStreamingService
    {
        public Stream EchoStream(Stream stream)
        {
            return stream;
        }

        public int GetIntFromStream(Stream stream)
        {
            // a negative expectedStreamSize means we don't know the expected size
            // and the bufSize is an arbitrary large enough size
            return VerifiableStream.VerifyStream(stream, expectedStreamSize: -1, bufSize: 4096);
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

        public static int VerifyStream(Stream stream, int expectedStreamSize, int bufSize)
        {
            var buff = new byte[bufSize];
            int bytesRead = 0;
            long totalBytesRead = 0;
            while ((bytesRead = stream.Read(buff, 0, bufSize)) > 0)
            {
                for (int i = 0; i < bytesRead; i++)
                {
                    byte shouldRead = (byte)(65 + (totalBytesRead++ % 26));
                    if (shouldRead != buff[i])
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                    // Catch extra data in stream early (if we know its expected size)
                    if (expectedStreamSize > 0 && totalBytesRead > expectedStreamSize)
                    {
                        System.Diagnostics.Debugger.Break();
                    }
                }
            }
            return (int)totalBytesRead;
        }
    }
}
