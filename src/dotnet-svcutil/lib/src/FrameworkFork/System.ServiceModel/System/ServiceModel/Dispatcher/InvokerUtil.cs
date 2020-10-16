// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace System.ServiceModel.Dispatcher
{
    internal delegate object InvokeDelegate(object target, object[] inputs, object[] outputs);

    internal sealed class InvokerUtil
    {
        private readonly CriticalHelper _helper;

        public InvokerUtil()
        {
            _helper = new CriticalHelper();
        }

        internal InvokeDelegate GenerateInvokeDelegate(MethodInfo method, out int inputParameterCount,
            out int outputParameterCount)
        {
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
                    if (returnsValue)
                    {
                        result = method.Invoke(target, inputsLocal);
                    }
                    else
                    {
                        method.Invoke(target, inputsLocal);
                    }
                    for (var i = 0; i < outputPos.Length; i++)
                    {
                        outputs[i] = inputs[outputPos[i]];
                    }

                    return result;
                };

                return lambda;
            }
        }
    }
}
