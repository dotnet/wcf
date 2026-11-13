// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.IO;
using System.Reflection;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public static class ProducerConsumerStreamTest
{
    [WcfTheory]
    [InlineData(false)]
    [InlineData(true)]
    public static async Task WriteAsync_TerminatedBeforeBufferReplacement_DoesNotWaitForAnotherReader(bool complete)
    {
        Type type = typeof(Message).Assembly.GetType("System.ServiceModel.Channels.ProducerConsumerStream", throwOnError: true);
        using var stream = (Stream)Activator.CreateInstance(type);
        using var cancellation = new CancellationTokenSource();
        var context = new PausedSynchronizationContext();
        SynchronizationContext originalContext = SynchronizationContext.Current;
        Task write;
        try
        {
            SynchronizationContext.SetSynchronizationContext(context);
            write = stream.WriteAsync(new byte[2], 0, 2, cancellation.Token);
        }
        finally
        {
            SynchronizationContext.SetSynchronizationContext(originalContext);
        }

        Task<int> read = stream.ReadAsync(new byte[1], 0, 1);
        Action resumeWriter = await context.Continuation.Task.WaitAsync(TimeSpan.FromSeconds(15));
        try
        {
            // The reader has supplied a buffer, but the writer has not replaced its source yet.
            if (complete)
            {
                type.GetMethod("Complete", BindingFlags.Instance | BindingFlags.NonPublic)
                    .Invoke(stream, new object[] { null });
            }
            else
            {
                stream.Dispose();
            }

            Assert.Equal(0, await read.WaitAsync(TimeSpan.FromSeconds(15)));
            resumeWriter();
            Assert.True(write.IsCompleted, "The writer must not wait on a new buffer source after termination.");
            await Assert.ThrowsAsync<ObjectDisposedException>(() => write);
        }
        finally
        {
            cancellation.Cancel();
            Exception cleanupException = await Record.ExceptionAsync(() => write.WaitAsync(TimeSpan.FromSeconds(15)));
            Assert.IsNotType<TimeoutException>(cleanupException);
        }
    }

    private sealed class PausedSynchronizationContext : SynchronizationContext
    {
        public TaskCompletionSource<Action> Continuation { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public override void Post(SendOrPostCallback callback, object state) =>
            Continuation.SetResult(() => callback(state));
    }
}
