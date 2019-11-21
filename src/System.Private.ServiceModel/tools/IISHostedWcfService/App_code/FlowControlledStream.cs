﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;

namespace WcfService
{
    public class FlowControlledStream : Stream
    {
        // Used to control when Read will return 0.
        public bool StopStreaming { get; set; }
        //bool readCalledWithStopStreaming = false;

        public TimeSpan ReadThrottle { get; set; }

        // Only set this if you don't want to manually control when 
        // the stream stops.
        // Keep it low - less than 1 second.  The server can send bytes very quickly, so
        // sending a continuous stream will easily blow the MaxReceivedMessageSize buffer.
        public TimeSpan StreamDuration { get; set; }

        private DateTime _readStartedTime;
        private long _totalBytesRead = 0;

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
            // Duration-based streaming logic: Control the "StopStreaming" flag based on a Duration
            if (StreamDuration != TimeSpan.Zero)
            {
                if (_readStartedTime == DateTime.MinValue)
                {
                    _readStartedTime = DateTime.Now;
                }
                if (DateTime.Now - _readStartedTime >= StreamDuration)
                {
                    StopStreaming = true;
                }
            }

            if (StopStreaming)
            {
                buffer[offset] = 0;
                return 0;
            }

            // Allow Read to continue as long as StopStreaming is false.
            // Just fill buffer with as many random bytes as necessary.
            int seed = DateTime.Now.Millisecond;
            Random rand = new Random(seed);
            byte[] randomBuffer = new byte[count];
            rand.NextBytes(randomBuffer);
            randomBuffer.CopyTo(buffer, offset);
            _totalBytesRead += count;

            if (ReadThrottle != TimeSpan.Zero)
            {
                Thread.CurrentThread.Join(ReadThrottle); // This API is not supported in K but is ok for Server side
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
