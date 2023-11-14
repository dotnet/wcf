// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.ServiceModel.Description;
using System.Text;

namespace System.ServiceModel.Dispatcher
{
    internal delegate object InvokeDelegate(object target, object[] inputs, object[] outputs);

    internal sealed class InvokerUtil
    {
        private static readonly string s_useLegacyInvokeDelegateAppContextSwitchKey = "System.ServiceModel.Dispatcher.UseLegacyInvokeDelegate";

        private static readonly Lazy<bool> s_useLegacyInvokeDelegate = new Lazy<bool>(() =>
            AppContext.TryGetSwitch(s_useLegacyInvokeDelegateAppContextSwitchKey, out bool useLegacyInvokeDelegate)
                ? useLegacyInvokeDelegate
                : false
        );

        private readonly CriticalHelper _helper;

        public InvokerUtil()
        {
            _helper = new CriticalHelper();
        }

        internal InvokeDelegate GenerateInvokeDelegate(MethodInfo method, out int inputParameterCount,
            out int outputParameterCount)
        {
            if (!s_useLegacyInvokeDelegate.Value && RuntimeFeature.IsDynamicCodeSupported)
            {
                return _helper.GenerateInvokeDelegateInternalWithExpressions(method, out inputParameterCount, out outputParameterCount);
            }

            return _helper.GenerateInvokeDelegate(method, out inputParameterCount, out outputParameterCount);
        }

        private class CriticalHelper
        {
            internal InvokeDelegate GenerateInvokeDelegate(MethodInfo method, out int inputParameterCount, out int outputParameterCount)
            {
                ParameterInfo[] parameters = method.GetParameters();
                bool returnsValue = method.ReturnType != typeof(void);
                var inputCount = parameters.Length;
                inputParameterCount = inputCount;

                var outputParamPositions = new List<int>();
                for (int i = 0; i < inputParameterCount; i++)
                {
                    if (parameters[i].ParameterType.IsByRef)
                    {
                        outputParamPositions.Add(i);
                    }
                }

                var outputPos = outputParamPositions.ToArray();
                outputParameterCount = outputPos.Length;

                InvokeDelegate lambda = delegate (object target, object[] inputs, object[] outputs)
                {
                    object[] inputsLocal = null;
                    if (inputCount > 0)
                    {
                        inputsLocal = new object[inputCount];
                        for (var i = 0; i < inputCount; i++)
                        {
                            inputsLocal[i] = inputs[i];
                        }
                    }
                    object result = null;
                    try
                    {
                        if (returnsValue)
                        {
                            result = method.Invoke(target, inputsLocal);
                        }
                        else
                        {
                            method.Invoke(target, inputsLocal);
                        }
                    }
                    catch (TargetInvocationException tie)
                    {
                        Exception actualException = tie.InnerException;
                        ExceptionDispatchInfo.Capture(actualException).Throw(); // Keep original call stack
                    }
                    for (var i = 0; i < outputPos.Length; i++)
                    {
                        outputs[i] = inputs[outputPos[i]];
                    }

                    return result;
                };

                return lambda;
            }

            internal InvokeDelegate GenerateInvokeDelegateInternalWithExpressions(MethodInfo method, out int inputParameterCount, out int outputParameterCount)
            {
                inputParameterCount = 0;
                outputParameterCount = 0;
                ParameterInfo[] parameters = method.GetParameters();
                bool returnsValue = method.ReturnType != typeof(void);
                bool returnsValueType = method.ReturnType.IsValueType;

                var targetParam = Expression.Parameter(typeof(object), "target");
                var inputsParam = Expression.Parameter(typeof(object[]), "inputs");
                var outputsParam = Expression.Parameter(typeof(object[]), "outputs");

                List<ParameterExpression> variables = new();
                var result = Expression.Variable(typeof(object), "result");
                variables.Add(result);

                List<(Type ParameterType, ParameterExpression OutputExpression)> outputVariables = new();
                List<ParameterExpression> invocationParameters = new();
                List<Expression> expressions = new();

                for (int i = 0; i < parameters.Length; i++)
                {
                    Type variableType = parameters[i].ParameterType.IsByRef
                        ? parameters[i].ParameterType.GetElementType()
                        : parameters[i].ParameterType;
                    ParameterExpression variable = Expression.Variable(variableType, $"p{i}");

                    if (ServiceReflector.FlowsIn(parameters[i]))
                    {
                        expressions.Add(Expression.Assign(variable, Expression.Convert(Expression.ArrayIndex(inputsParam, Expression.Constant(inputParameterCount)), variableType)));
                        inputParameterCount++;
                    }

                    if (ServiceReflector.FlowsOut(parameters[i]))
                    {
                        outputParameterCount++;
                        outputVariables.Add((variableType, variable));
                    }

                    variables.Add(variable);
                    invocationParameters.Add(variable);
                }

                var castTargetParam = Expression.Convert(targetParam, method.DeclaringType);

                if (returnsValue)
                {
                    if (returnsValueType)
                    {
                        expressions.Add(Expression.Assign(result, Expression.Convert(Expression.Call(castTargetParam, method, invocationParameters), typeof(object))));
                    }
                    else
                    {
                        expressions.Add(Expression.Assign(result, Expression.Call(castTargetParam, method, invocationParameters)));
                    }
                }
                else
                {
                    expressions.Add(Expression.Call(castTargetParam, method, invocationParameters));
                    expressions.Add(Expression.Assign(result, Expression.Constant(null, typeof(object))));
                }

                int j = 0;
                foreach (var outputVariable in outputVariables)
                {
                    if (outputVariable.ParameterType.IsValueType)
                    {
                        expressions.Add(Expression.Assign(
                            Expression.ArrayAccess(outputsParam, Expression.Constant(j)),
                            Expression.Convert(outputVariable.OutputExpression, typeof(object))));
                    }
                    else
                    {
                        expressions.Add(Expression.Assign(
                            Expression.ArrayAccess(outputsParam, Expression.Constant(j)),
                            outputVariable.OutputExpression));
                    }
                    j++;
                }

                expressions.Add(result);

                BlockExpression finalBlock = Expression.Block(variables: variables, expressions: expressions);

                Expression<InvokeDelegate> lambda = Expression.Lambda<InvokeDelegate>(
                    finalBlock,
                    targetParam,
                    inputsParam,
                    outputsParam);

                return lambda.Compile();
            }
        }
    }
}
