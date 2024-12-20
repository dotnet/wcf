// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace PackageChecker
{
    internal class ExportedMethod
    {
        public ExportedMethod(MethodBase method)
        {
            Signature = GetMethodSignature(method);
        }

        public string Signature { get; }

        private static string GetMethodSignature(MethodBase method)
        {
            var parameterTypes = method.GetParameters()
                                       .Select(p => GetTypeName(p.ParameterType))
                                       .ToArray();

            var genericArguments = method.IsGenericMethod ? $"<{string.Join(", ", method.GetGenericArguments().Select(arg => arg.Name))}>" : "";
            var returnType = GetTypeName(method is MethodInfo methodInfo ? methodInfo.ReturnType : method is ConstructorInfo ci ? ci.ReflectedType : null );
            var methodName = method.Name + genericArguments;

            return $"{returnType} {methodName}({string.Join(", ", parameterTypes)})";
        }

        private static string GetTypeName(Type type)
        {
            if (type is null)
            {
                return "__unknown__";
            }

            if (type.IsGenericParameter)
            {
                return $"T{type.GenericParameterPosition}";
            }
            else if (type.IsGenericType)
            {
                var genericTypeDefinition = type.GetGenericTypeDefinition().Name;
                if (genericTypeDefinition.IndexOf('`') != -1) // Delegate definitions don't contain a backtick
                {
                    genericTypeDefinition = genericTypeDefinition.Substring(0, genericTypeDefinition.IndexOf('`'));
                }
                var genericArguments = string.Join(", ", type.GetGenericArguments().Select(GetTypeName));
                return $"{genericTypeDefinition}<{genericArguments}>";
            }
            else
            {
                return type.Name;
            }
        }
    }
}
