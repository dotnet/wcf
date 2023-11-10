// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

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
            StringBuilder stringBuilder = new($"{method.DeclaringType.FullName}.{method.Name}(");
            stringBuilder.Append(string.Join(", ", method.GetParameters().Select(GetParameterString)));
            stringBuilder.Replace("+", ".");
            stringBuilder.Append(")");
            string result = stringBuilder.ToString();
            return result;
        }

        private static string GetParameterString(ParameterInfo p)
        {
            StringBuilder sb = new();
            Type parameterType = p.ParameterType;
            if (p.IsOut)
            {
                sb.Append("out ");
                parameterType = p.ParameterType.GetElementType();
            }
            else if (p.ParameterType.IsByRef)
            {
                sb.Append("ref ");
                parameterType = p.ParameterType.GetElementType();
            }

            if (p.IsDefined(typeof(ParamArrayAttribute)))
            {
                sb.Append("params ");
            }

            string parameterName = GetParameterFullName(parameterType);
            sb.Append(parameterName);
            return sb.ToString();
        }

        private static string GetParameterFullName(Type type)
        {
            if (type.IsGenericType)
            {
                StringBuilder sb = new();
                sb.Append(type.FullName.Substring(0, type.FullName.IndexOf('`')));
                sb.Append("<");
                sb.Append(string.Join(", ", type.GetGenericArguments().Select(GetParameterFullName)));
                sb.Append(">");
                return sb.ToString();
            }

            if (type.IsArray)
            {
                return GetParameterFullName(type.GetElementType()) + "[]";
            }

            string result;
            if (RuntimeFeature.IsDynamicCodeSupported)
            {
                result = s_runtimeIntegratedTypesMap.TryGetValue(type.TypeHandle.Value, out result)
                    ? result
                    : type.FullName;
            }
            else
            {
                result = s_integratedTypesMap.TryGetValue(type, out result)
                    ? result
                    : type.FullName;
            }

            return result;
        }

        private static readonly Dictionary<Type, string> s_integratedTypesMap = new()
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(nint), "nint" },
            { typeof(nuint), "nuint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(object), "object" },
            { typeof(string), "string" }
        };

        private static readonly Dictionary<IntPtr, string> s_runtimeIntegratedTypesMap = s_integratedTypesMap
            .ToDictionary(static kvp => kvp.Key.TypeHandle.Value, kvp => kvp.Value);
    }
}
