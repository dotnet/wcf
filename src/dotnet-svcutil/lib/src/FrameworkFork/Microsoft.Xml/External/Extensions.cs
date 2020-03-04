// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.
using System.IO;
using System.Reflection;

namespace Microsoft.Xml {
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


namespace Microsoft.Xml.Serialization {
				using System;
				using Microsoft.Xml;
    public static class SerializationExtensions
    {
        public static TypeCode GetTypeCode(this Type type)
        {
            if (type == null)
            {
                return TypeCode.Empty;
            }
            else if (type == typeof(Boolean))
            {
                return TypeCode.Boolean;
            }
            else if (type == typeof(Char))
            {
                return TypeCode.Char;
            }
            else if (type == typeof(SByte))
            {
                return TypeCode.SByte;
            }
            else if (type == typeof(Byte))
            {
                return TypeCode.Byte;
            }
            else if (type == typeof(Int16))
            {
                return TypeCode.Int16;
            }
            else if (type == typeof(UInt16))
            {
                return TypeCode.UInt16;
            }
            else if (type == typeof(Int32))
            {
                return TypeCode.Int32;
            }
            else if (type == typeof(UInt32))
            {
                return TypeCode.UInt32;
            }
            else if (type == typeof(Int64))
            {
                return TypeCode.Int64;
            }
            else if (type == typeof(UInt64))
            {
                return TypeCode.UInt64;
            }
            else if (type == typeof(Single))
            {
                return TypeCode.Single;
            }
            else if (type == typeof(Double))
            {
                return TypeCode.Double;
            }
            else if (type == typeof(Decimal))
            {
                return TypeCode.Decimal;
            }
            else if (type == typeof(DateTime))
            {
                return TypeCode.DateTime;
            }
            else if (type == typeof(String))
            {
                return TypeCode.String;
            }
            else
            {
                return TypeCode.Object;
            }
        }
    }
}
