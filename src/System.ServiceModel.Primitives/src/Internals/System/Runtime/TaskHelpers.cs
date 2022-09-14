// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace System.Runtime
{
    public static class TaskHelpers
    {
        //This replaces the Wait<TException>(this Task task) method as we want to await and not Wait()
        public static async Task AsyncWait<TException>(this Task task)
        {
            try
            {
                await task;
            }
            catch
            {
                throw Fx.Exception.AsError<TException>(task.Exception);
            }
        }

        // Helper method when implementing an APM wrapper around a Task based async method which returns a result. 
        // In the BeginMethod method, you would call use ToApm to wrap a call to MethodAsync:
        //     return MethodAsync(params).ToApm(callback, state);
        // In the EndMethod, you would use ToApmEnd<TResult> to ensure the correct exception handling
        // This will handle throwing exceptions in the correct place and ensure the IAsyncResult contains the provided
        // state object
        public static Task<TResult> ToApm<TResult>(this Task<TResult> task, AsyncCallback callback, object state)
        {
            // When using APM, the returned IAsyncResult must have the passed in state object stored in AsyncState. This
            // is so the callback can regain state. If the incoming task already holds the state object, there's no need
            // to create a TaskCompletionSource to ensure the returned (IAsyncResult)Task has the right state object.
            // This is a performance optimization for this special case.
            if (task.AsyncState == state)
            {
                if (callback != null)
                {
                    task.ContinueWith((antecedent, obj) =>
                    {
                        var callbackObj = obj as AsyncCallback;
                        callbackObj(antecedent);
                    }, callback, CancellationToken.None, TaskContinuationOptions.HideScheduler, TaskScheduler.Default);
                }
                return task;
            }

            // Need to create a TaskCompletionSource so that the returned Task object has the correct AsyncState value.
            var tcs = new TaskCompletionSource<TResult>(state);
            var continuationState = Tuple.Create(tcs, callback);
            task.ContinueWith((antecedent, obj) =>
            {
                var tuple = obj as Tuple<TaskCompletionSource<TResult>, AsyncCallback>;
                var tcsObj = tuple.Item1;
                var callbackObj = tuple.Item2;
                if (antecedent.IsFaulted)
                {
                    tcsObj.TrySetException(antecedent.Exception.InnerException);
                }
                else if (antecedent.IsCanceled)
                {
                    tcsObj.TrySetCanceled();
                }
                else
                {
                    tcsObj.TrySetResult(antecedent.Result);
                }

                if (callbackObj != null)
                {
                    callbackObj(tcsObj.Task);
                }
            }, continuationState, CancellationToken.None, TaskContinuationOptions.HideScheduler, TaskScheduler.Default);
            return tcs.Task;
        }

        // Helper method when implementing an APM wrapper around a Task based async method which returns a result. 
        // In the BeginMethod method, you would call use ToApm to wrap a call to MethodAsync:
        //     return MethodAsync(params).ToApm(callback, state);
        // In the EndMethod, you would use ToApmEnd to ensure the correct exception handling
        // This will handle throwing exceptions in the correct place and ensure the IAsyncResult contains the provided
        // state object
        public static Task ToApm(this Task task, AsyncCallback callback, object state)
        {
            // When using APM, the returned IAsyncResult must have the passed in state object stored in AsyncState. This
            // is so the callback can regain state. If the incoming task already holds the state object, there's no need
            // to create a TaskCompletionSource to ensure the returned (IAsyncResult)Task has the right state object.
            // This is a performance optimization for this special case.
            if (task.AsyncState == state)
            {
                if (callback != null)
                {
                    task.ContinueWith((antecedent, obj) =>
                    {
                        var callbackObj = obj as AsyncCallback;
                        callbackObj(antecedent);
                    }, callback, CancellationToken.None, TaskContinuationOptions.HideScheduler, TaskScheduler.Default);
                }
                return task;
            }

            // Need to create a TaskCompletionSource so that the returned Task object has the correct AsyncState value.
            // As we intend to create a task with no Result value, we don't care what result type the TCS holds as we
            // won't be using it. As Task<TResult> derives from Task, the returned Task is compatible.
            var tcs = new TaskCompletionSource<object>(state);
            var continuationState = Tuple.Create(tcs, callback);
            task.ContinueWith((antecedent, obj) =>
            {
                var tuple = obj as Tuple<TaskCompletionSource<object>, AsyncCallback>;
                var tcsObj = tuple.Item1;
                var callbackObj = tuple.Item2;
                if (antecedent.IsFaulted)
                {
                    tcsObj.TrySetException(antecedent.Exception.InnerException);
                }
                else if (antecedent.IsCanceled)
                {
                    tcsObj.TrySetCanceled();
                }
                else
                {
                    tcsObj.TrySetResult(null);
                }

                if (callbackObj != null)
                {
                    callbackObj(tcsObj.Task);
                }
            }, continuationState, CancellationToken.None, TaskContinuationOptions.HideScheduler, TaskScheduler.Default);
            return tcs.Task;
        }

        // Helper method to implement the End method of an APM method pair which is wrapping a Task based
        // async method when the Task returns a result. By using task.GetAwaiter.GetResult(), the exception
        // handling conventions are the same as when await'ing a task, i.e. this throws the first exception
        // and doesn't wrap it in an AggregateException. It also throws the right exception if the task was
        // cancelled.
        public static TResult ToApmEnd<TResult>(this IAsyncResult iar)
        {
            Task<TResult> task = iar as Task<TResult>;
            Contract.Assert(task != null, "IAsyncResult must be an instance of Task<TResult>");
            return task.GetAwaiter().GetResult();
        }

        // Helper method to implement the End method of an APM method pair which is wrapping a Task based
        // async method when the Task does not return result.
        public static void ToApmEnd(this IAsyncResult iar)
        {
            Task task = iar as Task;
            Contract.Assert(task != null, "IAsyncResult must be an instance of Task");
            task.GetAwaiter().GetResult();
        }

        // Helper method similar to TaskFactory.FromAsync but supports End methods which have an out parameter.
        public delegate TResult EndWithOutDelegate<T1, TResult>(IAsyncResult iar, out T1 arg1);
        public static Task<(TOut1, TOut2)> FromAsync<TIn, TOut1, TOut2>(Func<TIn, AsyncCallback, object, IAsyncResult> beginDelegate, EndWithOutDelegate<TOut2, TOut1> endDelegate, TIn arg1, object state)
        {
            var tcs = new TaskCompletionSource<(TOut1, TOut2)>(state);
            try
            {
                beginDelegate(arg1, iar =>
                {
                    var tuple = iar.AsyncState as Tuple<EndWithOutDelegate<TOut2, TOut1>, TaskCompletionSource<(TOut1, TOut2)>>;
                    var end = tuple.Item1;
                    var taskcs = tuple.Item2;
                    try
                    {
                        TOut2 out2;
                        TOut1 out1 = end(iar, out out2);
                        taskcs.TrySetResult((out1, out2));
                    }
                    catch (Exception e)
                    {
                        taskcs.TrySetException(e);
                    }
                }, Tuple.Create(endDelegate, tcs));
            }
            catch (Exception e)
            {
                tcs.TrySetException(e);
            }

            return tcs.Task;
        }

        public static Task CloseHelperAsync(this ICommunicationObject communicationObject, TimeSpan timeout)
        {
            if (communicationObject is IAsyncCommunicationObject)
            {
                return ((IAsyncCommunicationObject)communicationObject).CloseAsync(timeout);
            }
            else
            {
                return Task.Factory.FromAsync(communicationObject.BeginClose, communicationObject.EndClose, timeout, null);
            }
        }

        public static Task OpenHelperAsync(this ICommunicationObject communicationObject, TimeSpan timeout)
        {
            if (communicationObject is IAsyncCommunicationObject)
            {
                return ((IAsyncCommunicationObject)communicationObject).OpenAsync(timeout);
            }
            else
            {
                return Task.Factory.FromAsync(communicationObject.BeginOpen, communicationObject.EndOpen, timeout, null);
            }
        }

        // Awaitable helper to await a maximum amount of time for a task to complete. If the task doesn't
        // complete in the specified amount of time, returns false. This does not modify the state of the
        // passed in class, but instead is a mechanism to allow interrupting awaiting a task if a timeout
        // period passes.
        public static async Task<bool> AwaitWithTimeout(this Task task, TimeSpan timeout)
        {
            if (task.IsCompleted)
            {
                return true;
            }

            if (timeout == TimeSpan.MaxValue || timeout == Timeout.InfiniteTimeSpan)
            {
                await task;
                return true;
            }

            using (CancellationTokenSource cts = new CancellationTokenSource())
            {
                var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cts.Token));
                if (completedTask == task)
                {
                    cts.Cancel();
                    return true;
                }
                else
                {
                    return (task.IsCompleted);
                }
            }
        }

        // Task.GetAwaiter().GetResult() calls an internal variant of Wait() which doesn't wrap exceptions in
        // an AggregateException. It does spinwait so if it's expected that the Task isn't about to complete,
        // then use the NoSpin variant.
        public static void WaitForCompletion(this Task task)
        {
            Fx.Assert(task.IsCompleted || !IOThreadScheduler.IsRunningOnIOThread, "Waiting on an IO Thread might cause problems");
            // Waiting on an IO Thread can cause performance problems as we might block the IOThreadScheduler
            // dequeuing loop.
            task.GetAwaiter().GetResult();
        }

        // If the task is about to complete, this method will be more expensive than the regular method as it
        // always causes a WaitHandle to be allocated. If it is expected that the task will take longer than
        // the time of a spin wait, then a WaitHandle will be allocated anyway and this method avoids the CPU
        // cost of the spin wait.
        public static void WaitForCompletionNoSpin(this Task task)
        {
            if (!task.IsCompleted)
            {
                Fx.Assert(!IOThreadScheduler.IsRunningOnIOThread, "Waiting on an IO Thread might cause problems");
                // Waiting on an IO Thread can cause performance problems as we might block the IOThreadScheduler
                // dequeuing loop.
                ((IAsyncResult)task).AsyncWaitHandle.WaitOne();
            }

            // Call GetResult() to get any exceptions that were thrown
            task.GetAwaiter().GetResult();
        }

        public static TResult WaitForCompletion<TResult>(this Task<TResult> task)
        {
            Fx.Assert(task.IsCompleted || !IOThreadScheduler.IsRunningOnIOThread, "Waiting on an IO Thread might cause problems");
            // Waiting on an IO Thread can cause performance problems as we might block the IOThreadScheduler
            // dequeuing loop.
            return task.GetAwaiter().GetResult();
        }

        public static TResult WaitForCompletionNoSpin<TResult>(this Task<TResult> task)
        {
            if (!task.IsCompleted)
            {
                Fx.Assert(!IOThreadScheduler.IsRunningOnIOThread, "Waiting on an IO Thread might cause problems");
                // Waiting on an IO Thread can cause performance problems as we might block the IOThreadScheduler
                // dequeuing loop.
                ((IAsyncResult)task).AsyncWaitHandle.WaitOne();
            }

            return task.GetAwaiter().GetResult();
        }

        public static bool WaitForCompletionNoSpin(this Task task, TimeSpan timeout)
        {
            if (timeout >= TimeoutHelper.MaxWait)
            {
                task.WaitForCompletionNoSpin();
                return true;
            }

            bool completed = true;
            if (!task.IsCompleted)
            {
                Fx.Assert(!IOThreadScheduler.IsRunningOnIOThread, "Waiting on an IO Thread might cause problems");
                // Waiting on an IO Thread can cause performance problems as we might block the IOThreadScheduler
                // dequeuing loop.
                completed = ((IAsyncResult)task).AsyncWaitHandle.WaitOne(timeout);
            }

            if (completed)
            {
                // Throw any exceptions if there are any
                task.GetAwaiter().GetResult();
            }

            return completed;
        }

        // Used by WebSocketTransportDuplexSessionChannel on the sync code path.
        // TODO: Try and switch as many code paths as possible which use this to async
        public static void Wait(this Task task, TimeSpan timeout, Action<Exception, TimeSpan, string> exceptionConverter, string operationType)
        {
            bool timedOut = false;

            try
            {
                timedOut = !task.WaitForCompletionNoSpin(timeout);
            }
            catch (Exception ex)
            {
                if (Fx.IsFatal(ex) || exceptionConverter == null)
                {
                    throw;
                }

                exceptionConverter(ex, timeout, operationType);
            }

            if (timedOut)
            {
                throw Fx.Exception.AsError(new TimeoutException(InternalSR.TaskTimedOutError(timeout)));
            }
        }

        public static Task CompletedTask()
        {
            return Task.FromResult(true);
        }

        public static DefaultTaskSchedulerAwaiter EnsureDefaultTaskScheduler()
        {
            return DefaultTaskSchedulerAwaiter.Singleton;
        }

        public static Action<object> OnAsyncCompletionCallback = OnAsyncCompletion;

        // Method to act as callback for asynchronous code which uses AsyncCompletionResult as the return type when used within
        // a Task based async method. These methods require a callback which is called in the case of the IO completing asynchronously.
        // This pattern still requires an allocation, whereas the purpose of using the AsyncCompletionResult enum is to avoid allocation.
        // In the future, this pattern should be replaced with a reusable awaitable object, potentially with a global pool.
        private static void OnAsyncCompletion(object state)
        {
            var tcs = state as TaskCompletionSource<bool>;
            Contract.Assert(state != null, "Async state should be of type TaskCompletionSource<bool>");
            tcs.TrySetResult(true);
        }

        public static IDisposable RunTaskContinuationsOnOurThreads()
        {
            if (SynchronizationContext.Current == ServiceModelSynchronizationContext.Instance)
            {
                return null; // No need to save and restore state as we're already using the correct sync context
            }

            return new SyncContextScope();
        }

        // Calls the given Action asynchronously on the ThreadPool.
        public static async Task CallActionAsync<TArg>(Action<TArg> action, TArg argument)
        {
            // Make sure any async tasks started from the action have their continuation
            // execute on the IOThreadScheduler, but make sure the action itself is running
            // on the thread pool.
            if (!Thread.CurrentThread.IsThreadPoolThread)
            {
                // Switch to a thread pool thread to run passed action
                SynchronizationContext.SetSynchronizationContext(null);
                await Task.Yield();
            }

            // Now we're running on the ThreadPool, we reset the SynchronizationContext to
            // our sync context which posts to the IOThreadScheduler. We're not hopping threads
            // so any synchronous blocking will occur on the current thread pool thread.
            Fx.Assert(Thread.CurrentThread.IsThreadPoolThread, "We should be running on the thread pool");
            using (var scope = RunTaskContinuationsOnOurThreads())
            {
                action(argument);
            }
        }

        private class SyncContextScope : IDisposable
        {
            private readonly SynchronizationContext _prevContext;

            public SyncContextScope()
            {
                _prevContext = SynchronizationContext.Current;
                SynchronizationContext.SetSynchronizationContext(ServiceModelSynchronizationContext.Instance);
            }

            public void Dispose()
            {
                SynchronizationContext.SetSynchronizationContext(_prevContext);
            }
        }
    }

    // This awaiter causes an awaiting async method to continue on the same thread if using the
    // default task scheduler, otherwise it posts the continuation to the ThreadPool. While this
    // does a similar function to Task.ConfigureAwait, this code doesn't require a Task to function.
    // With Task.ConfigureAwait, you would need to call it on the first task on each potential code
    // path in a method. This could mean calling ConfigureAwait multiple times in a single method.
    // This awaiter can be awaited on at the beginning of a method a single time and isn't dependant
    // on running other awaitable code.
    public struct DefaultTaskSchedulerAwaiter : INotifyCompletion
    {
        public static DefaultTaskSchedulerAwaiter Singleton = new DefaultTaskSchedulerAwaiter();

        // If the current TaskScheduler is the default, if we aren't currently running inside a task and
        // the default SynchronizationContext isn't current, when a Task starts, it will change the TaskScheduler
        // to one based off the current SynchronizationContext. Also, any async api's that WCF consumes will
        // post back to the same SynchronizationContext as they were started in which could cause WCF to deadlock
        // on our Sync code path.
        public bool IsCompleted
        {
            get
            {
                return (TaskScheduler.Current == TaskScheduler.Default) &&
                    (SynchronizationContext.Current == null ||
                    (SynchronizationContext.Current.GetType() == typeof(SynchronizationContext)));
            }
        }

        // Only called when IsCompleted returns false, otherwise the caller will call the continuation
        // directly causing it to stay on the same thread.
        public void OnCompleted(Action continuation)
        {
            Task.Run(continuation);
        }

        // Awaiter is only used to control where subsequent awaitable's run so GetResult needs no
        // implementation. Normally any exceptions would be thrown here, but we have nothing to throw
        // as we don't run anything, only control where other code runs.
        public void GetResult() { }

        public DefaultTaskSchedulerAwaiter GetAwaiter()
        {
            return this;
        }
    }


    // Async methods can't take an out (or ref) argument. This wrapper allows passing in place of an out argument
    // and can be used to return a value via a method argument.
    public class OutWrapper<T>
    {
        public OutWrapper()
        {
            Value = default(T);
        }

        public T Value { get; set; }

        public static implicit operator T(OutWrapper<T> wrapper)
        {
            return wrapper.Value;
        }
    }
}
