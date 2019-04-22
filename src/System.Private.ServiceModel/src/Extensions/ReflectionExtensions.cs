// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System.ServiceModel
{
    internal static class ReflectionExtensions
    {
        #region Type
        public static Assembly Assembly(this Type type)
        {
            return type.GetTypeInfo().Assembly;
        }
        public static Type BaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }
        public static bool ContainsGenericParameters(this Type type)
        {
            return type.GetTypeInfo().ContainsGenericParameters;
        }
        public static ConstructorInfo GetConstructor(this Type type, Type[] types)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
        public static ConstructorInfo GetConstructor(this Type type, BindingFlags bindingAttr, object binder, Type[] types, object[] modifiers)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
        public static PropertyInfo GetProperty(this Type type, string name, BindingFlags bindingAttr)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
        public static Type[] GetGenericArguments(this Type type)
        {
            return type.GetTypeInfo().GenericTypeArguments;
        }
        public static Type[] GetInterfaces(this Type type)
        {
            return Enumerable.ToArray(type.GetTypeInfo().ImplementedInterfaces);
        }
        public static bool IsAbstract(this Type type)
        {
            return type.GetTypeInfo().IsAbstract;
        }
        public static bool IsAssignableFrom(this Type type, Type otherType)
        {
            return type.GetTypeInfo().IsAssignableFrom(otherType.GetTypeInfo());
        }
        public static bool IsClass(this Type type)
        {
            return type.GetTypeInfo().IsClass;
        }
        public static bool IsDefined(this Type type, Type attributeType, bool inherit)
        {
            return type.GetTypeInfo().IsDefined(attributeType, inherit);
        }
        public static bool IsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }
        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }
        public static bool IsInterface(this Type type)
        {
            return type.GetTypeInfo().IsInterface;
        }
        public static bool IsInstanceOfType(this Type type, object o)
        {
            return o == null ? false : type.GetTypeInfo().IsAssignableFrom(o.GetType().GetTypeInfo());
        }
        public static bool IsMarshalByRef(this Type type)
        {
            return type.GetTypeInfo().IsMarshalByRef;
        }
        public static bool IsNotPublic(this Type type)
        {
            return type.GetTypeInfo().IsNotPublic;
        }
        public static bool IsSealed(this Type type)
        {
            return type.GetTypeInfo().IsSealed;
        }
        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }
        public static InterfaceMapping GetInterfaceMap(this Type type, Type interfaceType)
        {
            return type.GetTypeInfo().GetRuntimeInterfaceMap(interfaceType);
        }
        public static MemberInfo[] GetMember(this Type type, string name, BindingFlags bindingAttr)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
        public static MemberInfo[] GetMembers(this Type type, BindingFlags bindingAttr)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
        public static MethodInfo GetMethod(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredMethod(name);
        }

        public static MethodInfo GetMethod(this Type type, string name, Type[] types)
        {
            return type.GetRuntimeMethod(name, types);
        }

        // TypeCode does not exist in N, but it is used by ServiceModel.
        // This extension method was copied from System.Private.PortableThunks\Internal\PortableLibraryThunks\System\TypeThunks.cs
        public static TypeCode GetTypeCode(this Type type)
        {
            if (type == null)
            {
                return TypeCode.Empty;
            }

            if (type == typeof(Boolean))
            {
                return TypeCode.Boolean;
            }

            if (type == typeof(Char))
            {
                return TypeCode.Char;
            }

            if (type == typeof(SByte))
            {
                return TypeCode.SByte;
            }

            if (type == typeof(Byte))
            {
                return TypeCode.Byte;
            }

            if (type == typeof(Int16))
            {
                return TypeCode.Int16;
            }

            if (type == typeof(UInt16))
            {
                return TypeCode.UInt16;
            }

            if (type == typeof(Int32))
            {
                return TypeCode.Int32;
            }

            if (type == typeof(UInt32))
            {
                return TypeCode.UInt32;
            }

            if (type == typeof(Int64))
            {
                return TypeCode.Int64;
            }

            if (type == typeof(UInt64))
            {
                return TypeCode.UInt64;
            }

            if (type == typeof(Single))
            {
                return TypeCode.Single;
            }

            if (type == typeof(Double))
            {
                return TypeCode.Double;
            }

            if (type == typeof(Decimal))
            {
                return TypeCode.Decimal;
            }

            if (type == typeof(DateTime))
            {
                return TypeCode.DateTime;
            }

            if (type == typeof(String))
            {
                return TypeCode.String;
            }

            if (type.GetTypeInfo().IsEnum)
            {
                return GetTypeCode(Enum.GetUnderlyingType(type));
            }

            return TypeCode.Object;
        }
        #endregion Type

        #region ConstructorInfo
        public static bool IsPublic(this ConstructorInfo ci)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
        public static object Invoke(this ConstructorInfo ci, BindingFlags invokeAttr, object binder, object[] parameters, CultureInfo culture)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
        #endregion ConstructorInfo

        #region MethodInfo, MethodBase
        public static RuntimeMethodHandle MethodHandle(this MethodBase mb)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
        public static RuntimeMethodHandle MethodHandle(this MethodInfo mi)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
        public static Type ReflectedType(this MethodInfo mi)
        {
            throw ExceptionHelper.PlatformNotSupported();
        }
        #endregion MethodInfo, MethodBase
    }
}
