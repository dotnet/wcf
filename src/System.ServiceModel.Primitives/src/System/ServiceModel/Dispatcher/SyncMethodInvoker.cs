﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Reflection;
using System.Runtime;
using System.Runtime.Diagnostics;
using System.ServiceModel.Diagnostics;
using System.Threading.Tasks;

namespace System.ServiceModel.Dispatcher
{
    public class SyncMethodInvoker : IOperationInvoker
    {
        private InvokeDelegate _invokeDelegate;
        private int _inputParameterCount;
        private int _outputParameterCount;
        private string _methodName;

        public SyncMethodInvoker(MethodInfo method)
        {
            Method = method ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException(nameof(method)));
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
            var task = result as Task<Tuple<object, object[]>>;
            if (task == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SRP.SFxInvalidCallbackIAsyncResult));
            }

            var tuple = task.Result;
            outputs = tuple.Item2;
            return tuple.Item1;
        }

        private Task<Tuple<object, object[]>> InvokeAsync(object instance, object[] inputs)
        {
            EnsureIsInitialized();

            if (instance == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.SFxNoServiceObject));
            }

            if (inputs == null)
            {
                if (_inputParameterCount > 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SFxInputParametersToServiceNull, _inputParameterCount)));
                }
            }
            else if (inputs.Length != _inputParameterCount)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRP.Format(SRP.SFxInputParametersToServiceInvalid, _inputParameterCount, inputs.Length)));
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
                        ServiceModelActivity.Start(activity, SRP.Format(SRP.ActivityExecuteMethod, Method.DeclaringType.FullName, Method.Name), ActivityType.ExecuteUserCode);
                    }
                    if (WcfEventSource.Instance.OperationInvokedIsEnabled())
                    {
                        WcfEventSource.Instance.OperationInvoked(eventTraceActivity, MethodName, TraceUtility.GetCallerInfo(OperationContext.Current));
                    }
                    returnValue = _invokeDelegate(instance, inputs, outputs);
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

            return Task.FromResult(Tuple.Create(returnValue, outputs));
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
