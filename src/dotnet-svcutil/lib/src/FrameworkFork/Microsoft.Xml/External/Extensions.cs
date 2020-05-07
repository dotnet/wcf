// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Reflection;

namespace Microsoft.Xml
{
    using System;

    public static class Extensions
    {
        public static void Close(this Stream stream)
        {
            stream.Dispose();
        }

        public static MethodInfo GetMethod(this Type type, String name, BindingFlags bindingAttr, System.Reflection.Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            return type.GetMethod(name, types);
        }

        public static MethodInfo GetMethod(this Type type, String name, BindingFlags bindingAttr, System.Reflection.Binder binder, Type[] types, ParameterModifier[] modifiers)
        {
            return GetMethod(type, name, bindingAttr, binder, CallingConventions.Standard, types, modifiers);
        }
    }
}
