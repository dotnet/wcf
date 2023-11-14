// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Net.Sockets;
using System.Threading.Tasks.Sources;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace System.ServiceModel.Channels
{
    // Copied and modified from https://github.com/dotnet/aspnetcore/blob/7a5d1cc1beda12eebb3fb3aa8ccb8253cf445115/src/Servers/Kestrel/Transport.Sockets/src/Internal/SocketAwaitableEventArgs.cs

    // A slimmed down version of https://github.com/dotnet/runtime/blob/82ca681cbac89d813a3ce397e0c665e6c051ed67/src/libraries/System.Net.Sockets/src/System/Net/Sockets/Socket.Tasks.cs#L798 that
    // 1. Doesn't support any custom scheduling other than the PipeScheduler (no sync context, no task scheduler)
    // 2. Doesn't do ValueTask validation using the token
    // 3. Doesn't support usage outside of async/await (doesn't try to capture and restore the execution context)
    // 4. Doesn't use cancellation tokens
    internal class SocketAwaitableEventArgs : SocketAsyncEventArgs, IValueTaskSource<int>, IValueTaskSource
    {
        private static readonly Action<object> _continuationCompleted = _ => { };

        // There are places where we read the _continuation field and then read some other state which we assume to be consistent
        // with the value we read in _continuation. Without a fence, those secondary reads could be reordered with respect to the first.
        // https://github.com/dotnet/runtime/pull/84432
        // https://github.com/dotnet/aspnetcore/issues/50623
        private volatile Action<object> _continuation;

        public SocketAwaitableEventArgs() : base(unsafeSuppressExecutionContextFlow: true) { }

        public ValueTask<int> ReceiveAsync(Socket socket, Memory<byte> buffer)
        {
            SetBuffer(buffer);

            if (socket.ReceiveAsync(this))
            {
                return new ValueTask<int>(this, 0);
            }

            var bytesTransferred = BytesTransferred;
            var error = SocketError;

            return error == SocketError.Success
                ? new ValueTask<int>(bytesTransferred)
                : new ValueTask<int>(Task.FromException<int>(CreateException(error)));
        }

        public ValueTask SendAsync(Socket socket, ReadOnlyMemory<byte> memory)
        {
            SetBuffer(MemoryMarshal.AsMemory(memory));

            if (socket.SendAsync(this))
            {
                return new ValueTask(this, 0);
            }

            var bytesTransferred = BytesTransferred;
            var error = SocketError;

            return error == SocketError.Success
                ? ValueTask.CompletedTask
                : new ValueTask(Task.FromException(CreateException(error)));
        }

        protected override void OnCompleted(SocketAsyncEventArgs _)
        {
            var c = _continuation;

            if (c != null || (c = Interlocked.CompareExchange(ref _continuation, _continuationCompleted, null)) != null)
            {
                var continuationState = UserToken;
                UserToken = null;
                _continuation = _continuationCompleted; // in case someone's polling IsCompleted

                c.Invoke(continuationState);
            }
        }

        int IValueTaskSource<int>.GetResult(short token)
        {
            _continuation = null;

            if (SocketError != SocketError.Success)
            {
                throw CreateException(SocketError);
            }

            return BytesTransferred;
        }

        void IValueTaskSource.GetResult(short token)
        {
            _continuation = null;

            if (SocketError != SocketError.Success)
            {
                throw CreateException(SocketError);
            }
        }

        protected static SocketException CreateException(SocketError e)
        {
            return new SocketException((int)e);
        }

        public ValueTaskSourceStatus GetStatus(short token)
        {
            return !ReferenceEquals(_continuation, _continuationCompleted) ? ValueTaskSourceStatus.Pending :
                    SocketError == SocketError.Success ? ValueTaskSourceStatus.Succeeded :
                    ValueTaskSourceStatus.Faulted;
        }

        public void OnCompleted(Action<object> continuation, object state, short token, ValueTaskSourceOnCompletedFlags flags)
        {
            UserToken = state;
            var prevContinuation = Interlocked.CompareExchange(ref _continuation, continuation, null);
            if (ReferenceEquals(prevContinuation, _continuationCompleted))
            {
                UserToken = null;
                // This should only get hit if the operation completes between ValueTask<int>.IsCompleted being
                // called and returning false and this method being called. In which case we will have one extra frame
                // on the call stack. This will only be a problem is calling ReceiveAsync in a loop. The only time Receive
                // will be called in a loop we are doing so because the message size is larger than the max buffer size,
                // which would mean we're receiving a very large message and the receive will be completing asynchronously.
                // We read the message size from the NetTcp frame header and try allocate a buffer large enough for entire
                // message. We then call ReceiveAsync with a buffer size up to the max buffer size.
                continuation(state);
                //ThreadPool.UnsafeQueueUserWorkItem(continuation, state, preferLocal: true);
            }
        }
    }
}
