// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Reflection;

namespace System.ServiceModel.Dispatcher
{
    public class DispatchOperationRuntimeHelpers
    {
        internal static Dictionary<string, IOperationInvoker> OperationInvokers { get; } = new();

        public static void RegisterOperationInvoker(string key, IOperationInvoker invoker)
        {
            OperationInvokers[key] = invoker;
        }

        internal static string GetKey(MethodInfo method)
        {
            // TODO implement GetKey and wire it OperationInvokerBehavior
            throw new NotImplementedException();
        }
    }
}
