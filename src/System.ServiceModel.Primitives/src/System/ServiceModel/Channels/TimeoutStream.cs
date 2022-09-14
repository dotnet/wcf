// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace System.ServiceModel.Channels
{
    // Enforces an overall timeout based on the TimeoutHelper passed in
    internal class TimeoutStream : DelegatingStream
    {
        private TimeoutHelper _timeoutHelper;
        private bool _disposed;
        private byte[] _oneByteArray = new byte[1];

        public TimeoutStream(Stream stream, TimeSpan timeout)
            : base(stream)
        {
            if (!stream.CanTimeout)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(nameof(stream), SRP.StreamDoesNotSupportTimeout);
            }

            _timeoutHelper = new TimeoutHelper(timeout);
            ReadTimeout = TimeoutHelper.ToMilliseconds(timeout);
            WriteTimeout = ReadTimeout;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var valueTask = ReadAsyncInternal(buffer, offset, count, CancellationToken.None);
            if (valueTask.IsCompletedSuccessfully)
            {
                return valueTask.Result;
            }

            return valueTask.AsTask().WaitForCompletion();
        }

        public override int ReadByte()
        {
            int r = Read(_oneByteArray, 0, 1);
            if (r == 0)
            {
                return -1;
            }

            return _oneByteArray[0];
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            // Supporting a passed in cancellationToken as well as honoring the timeout token in this class would require
            // creating a linked token source on every call which is extra allocation and needs disposal. As this is an 
            // internal classs, it's okay to add this extra constraint to usage of this method.
            Fx.Assert(!cancellationToken.CanBeCanceled, "cancellationToken shouldn't be cancellable");
            var cancelToken = await _timeoutHelper.GetCancellationTokenAsync();
            return await base.ReadAsync(buffer, offset, count, cancelToken);
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            // Supporting a passed in cancellationToken as well as honoring the timeout token in this class would require
            // creating a linked token source on every call which is extra allocation and needs disposal. As this is an 
            // internal classs, it's okay to add this extra constraint to usage of this method.
            Fx.Assert(!cancellationToken.CanBeCanceled, "cancellationToken shouldn't be cancellable");
            var cancelToken = await _timeoutHelper.GetCancellationTokenAsync();
            return await base.ReadAsync(buffer, cancelToken);
        }

        private async ValueTask<int> ReadAsyncInternal(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await TaskHelpers.EnsureDefaultTaskScheduler();
            // Using the ReadAsync overload which takes Memory<byte> as it returns a ValueTask which avoids
            // allocation when the read completes synchronously.
            return await ReadAsync(new Memory<byte>(buffer, offset, count), cancellationToken);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var valueTask = WriteAsyncInternal(buffer, offset, count, CancellationToken.None);
            if (!valueTask.IsCompletedSuccessfully)
            {
                valueTask.AsTask().WaitForCompletion();
            }
        }

        public override void WriteByte(byte value)
        {
            _oneByteArray[0] = value;
            Write(_oneByteArray, 0, 1);
        }

        public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            // Supporting a passed in cancellationToken as well as honoring the timeout token in this class would require
            // creating a linked token source on every call which is extra allocation and needs disposal. As this is an 
            // internal classs, it's okay to add this extra constraint to usage of this method.
            Fx.Assert(!cancellationToken.CanBeCanceled, "cancellationToken shouldn't be cancellable");
            var cancelToken = await _timeoutHelper.GetCancellationTokenAsync();
            await base.WriteAsync(buffer, offset, count, cancelToken);
        }

        public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            // Supporting a passed in cancellationToken as well as honoring the timeout token in this class would require
            // creating a linked token source on every call which is extra allocation and needs disposal. As this is an 
            // internal classs, it's okay to add this extra constraint to usage of this method.
            Fx.Assert(!cancellationToken.CanBeCanceled, "cancellationToken shouldn't be cancellable");
            var cancelToken = await _timeoutHelper.GetCancellationTokenAsync();
            await base.WriteAsync(buffer, cancelToken);
        }

        private async ValueTask WriteAsyncInternal(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            await TaskHelpers.EnsureDefaultTaskScheduler();
            // Using the ReadAsync overload which takes Memory<byte> as it returns a ValueTask which avoids
            // allocation when the read completes synchronously.
            await WriteAsync(new Memory<byte>(buffer, offset, count), cancellationToken);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _timeoutHelper = default(TimeoutHelper);
                }

                _disposed = true;
            }
            base.Dispose(disposing);
        }

        public override ValueTask DisposeAsync()
        {
            if (!_disposed)
            {
                _timeoutHelper = default(TimeoutHelper);
                _disposed = true;
            }

            return base.DisposeAsync();
        }
    }
}
