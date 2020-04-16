// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    /// <summary>
    /// This class allows for running and cancelling a sync operation that is not cancellable.
    /// </summary>
    internal static class AsyncHelper
    {
        public static async Task RunAsync(Action action, CancellationToken cancellationToken)
        {
            await RunAsync(action, null, cancellationToken).ConfigureAwait(false);
        }

        public static async Task RunAsync(Action action, Action onCancellation, CancellationToken cancellationToken)
        {
            var taskCompletionSrc = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => taskCompletionSrc.SetCanceled()))
            {
                var actionTask = Task.Run(action, cancellationToken);
                await Task.WhenAny(actionTask, taskCompletionSrc.Task);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                onCancellation?.Invoke();
            }

            cancellationToken.ThrowIfCancellationRequested();
        }

        public static async Task<T> RunAsync<T>(Func<T> func, CancellationToken cancellationToken)
        {
            return await RunAsync(func, null, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<T> RunAsync<T>(Func<T> func, Action onCancellation, CancellationToken cancellationToken)
        {
            Task<T> finishedTask = null;
            var taskCompletionSrc = new TaskCompletionSource<T>();

            using (cancellationToken.Register(() => taskCompletionSrc.SetCanceled()))
            {
                finishedTask = await Task.WhenAny(Task<T>.Run(func, cancellationToken), taskCompletionSrc.Task);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                onCancellation?.Invoke();
            }

            cancellationToken.ThrowIfCancellationRequested();

            System.Diagnostics.Debug.Assert(finishedTask != taskCompletionSrc.Task, "Unexpected completion task!");
            if (finishedTask == taskCompletionSrc.Task)
            {
                // this should never happen but if we introduce a bug here better fail earlier!
                throw new InvalidOperationException();
            }

            return finishedTask.Result;
        }
    }
}
