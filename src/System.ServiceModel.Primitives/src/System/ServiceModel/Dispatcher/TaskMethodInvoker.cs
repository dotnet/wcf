// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Reflection;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.ServiceModel.Description;
using System.ServiceModel.Diagnostics;
using System.Threading.Tasks;

namespace System.ServiceModel.Dispatcher
{
    public class TaskMethodInvoker : IOperationInvoker
    {
        private const string ResultMethodName = "Result";
        private InvokeDelegate _invokeDelegate;
        private int _inputParameterCount;
        private int _outputParameterCount;
        private string _methodName;
        private MethodInfo _taskTResultGetMethod;
        private bool _isGenericTask;

        public TaskMethodInvoker(MethodInfo taskMethod, Type taskType)
        {
            Method = taskMethod ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(taskMethod)));

            if (taskType != ServiceReflector.VoidType)
            {
                _taskTResultGetMethod = ((PropertyInfo)taskMethod.ReturnType.GetMember(ResultMethodName)[0]).GetGetMethod();
                _isGenericTask = true;
            }
        }

        public MethodInfo Method { get; }

        public string MethodName
        {
            get
            {
                if (_methodName == null)
                {
                    _methodName = Method.Name;
                }

                return _methodName;
            }
        }

        public object[] AllocateInputs()
        {
            EnsureIsInitialized();

            return EmptyArray<object>.Allocate(_inputParameterCount);
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return InvokeAsync(instance, inputs).ToApm(callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            object returnVal;
            var invokeTask = result as Task<Tuple<object, object[]>>;
            if (invokeTask == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.SFxInvalidCallbackIAsyncResult));
            }

            AggregateException ae = null;
            Tuple<object, object[]> tuple = null;
            Task task = null;

            if (invokeTask.IsFaulted)
            {
                Fx.Assert(invokeTask.Exception != null, "Task.IsFaulted guarantees non-null exception.");
                ae = invokeTask.Exception;
            }
            else
            {
                Fx.Assert(invokeTask.IsCompleted, "Task.Result is expected to be completed");
                tuple = invokeTask.Result;
                task = tuple.Item1 as Task;
                Fx.Assert(task != null, "Returned object is expected to be a non-null Task");
                if (task.IsFaulted)
                {
                    Fx.Assert(task.Exception != null, "Task.IsFaulted guarantees non-null exception.");
                    ae = task.Exception;
                }
            }

            if (ae != null)
            {
                // If FaultException is thrown, we will get 'callFaulted' behavior below.
                // Any other exception will retain 'callFailed' behavior.
                throw FxTrace.Exception.AsError<FaultException>(ae);
            }

            // Task cancellation without an exception indicates failure but we have no
            // additional information to provide.  Accessing Task.Result will throw a
            // TaskCanceledException.   For consistency between void Tasks and Task<T>,
            // we detect and throw here.
            if (task.IsCanceled)
            {
                throw FxTrace.Exception.AsError(new TaskCanceledException(task));
            }

            outputs = tuple.Item2;

            if (_isGenericTask)
            {
                returnVal = _taskTResultGetMethod.Invoke(task, Type.EmptyTypes);
            }
            else
            {
                returnVal = null;
            }

            return returnVal;
        }

        private async Task<Tuple<object, object[]>> InvokeAsync(object instance, object[] inputs)
        {
            EnsureIsInitialized();

            if (instance == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new InvalidOperationException(SRP.SFxNoServiceObject));
            }

            if (inputs == null)
            {
                if (_inputParameterCount > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new InvalidOperationException(SRP.Format(SRP.SFxInputParametersToServiceNull, _inputParameterCount)));
                }
            }
            else if (inputs.Length != _inputParameterCount)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                    new InvalidOperationException(SRP.Format(SRP.SFxInputParametersToServiceInvalid, _inputParameterCount,
                        inputs.Length)));
            }

            var outputs = EmptyArray<object>.Allocate(_outputParameterCount);

            long beginOperation = 0;
            bool callSucceeded = false;
            bool callFaulted = false;

            EventTraceActivity eventTraceActivity = null;
            if (WcfEventSource.Instance.OperationCompletedIsEnabled() ||
                WcfEventSource.Instance.OperationFaultedIsEnabled() ||
                WcfEventSource.Instance.OperationFailedIsEnabled())
            {
                beginOperation = DateTime.UtcNow.Ticks;
                OperationContext context = OperationContext.Current;
                if (context != null && context.IncomingMessage != null)
                {
                    eventTraceActivity = EventTraceActivityHelper.TryExtractActivity(context.IncomingMessage);
                }
            }

            object returnValue;
            try
            {
                ServiceModelActivity activity = null;
                IDisposable boundActivity = null;
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    activity = ServiceModelActivity.CreateBoundedActivity(true);
                    boundActivity = activity;
                }
                else if (TraceUtility.MessageFlowTracingOnly)
                {
                    Guid activityId = TraceUtility.GetReceivedActivityId(OperationContext.Current);
                    if (activityId != Guid.Empty)
                    {
                        DiagnosticTraceBase.ActivityId = activityId;
                    }
                }
                else if (TraceUtility.ShouldPropagateActivity)
                {
                    //Message flow tracing only scenarios use a light-weight ActivityID management logic
                    Guid activityId = ActivityIdHeader.ExtractActivityId(OperationContext.Current.IncomingMessage);
                    if (activityId != Guid.Empty)
                    {
                        boundActivity = Activity.CreateActivity(activityId);
                    }
                }

                using (boundActivity)
                {
                    if (DiagnosticUtility.ShouldUseActivity)
                    {
                        ServiceModelActivity.Start(activity,
                            SRP.Format(SRP.ActivityExecuteMethod, Method.DeclaringType.FullName, Method.Name),
                            ActivityType.ExecuteUserCode);
                    }
                    if (WcfEventSource.Instance.OperationInvokedIsEnabled())
                    {
                        WcfEventSource.Instance.OperationInvoked(eventTraceActivity, MethodName,
                            TraceUtility.GetCallerInfo(OperationContext.Current));
                    }
                    returnValue = _invokeDelegate(instance, inputs, outputs);

                    if (returnValue == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("task");
                    }

                    var returnValueTask = returnValue as Task;
                    if (returnValueTask != null)
                    {
                        // Only return once the task has completed
                        await returnValueTask;
                    }

                    callSucceeded = true;
                }
            }
            catch (FaultException)
            {
                callFaulted = true;
                throw;
            }
            finally
            {
                if (beginOperation != 0)
                {
                    if (callSucceeded)
                    {
                        if (WcfEventSource.Instance.OperationCompletedIsEnabled())
                        {
                            WcfEventSource.Instance.OperationCompleted(eventTraceActivity, _methodName,
                                TraceUtility.GetUtcBasedDurationForTrace(beginOperation));
                        }
                    }
                    else if (callFaulted)
                    {
                        if (WcfEventSource.Instance.OperationFaultedIsEnabled())
                        {
                            WcfEventSource.Instance.OperationFaulted(eventTraceActivity, _methodName,
                                TraceUtility.GetUtcBasedDurationForTrace(beginOperation));
                        }
                    }
                    else
                    {
                        if (WcfEventSource.Instance.OperationFailedIsEnabled())
                        {
                            WcfEventSource.Instance.OperationFailed(eventTraceActivity, _methodName,
                                TraceUtility.GetUtcBasedDurationForTrace(beginOperation));
                        }
                    }
                }
            }

            return Tuple.Create(returnValue, outputs);
        }

        private void EnsureIsInitialized()
        {
            if (_invokeDelegate == null)
            {
                EnsureIsInitializedCore();
            }
        }

        private void EnsureIsInitializedCore()
        {
            // Only pass locals byref because InvokerUtil may store temporary results in the byref.
            // If two threads both reference this.count, temporary results may interact.
            int inputParameterCount;
            int outputParameterCount;
            var invokeDelegate = new InvokerUtil().GenerateInvokeDelegate(Method, out inputParameterCount, out outputParameterCount);
            _outputParameterCount = outputParameterCount;
            _inputParameterCount = inputParameterCount;
            _invokeDelegate = invokeDelegate;  // must set this last due to race
        }
    }
}
