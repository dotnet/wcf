// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Runtime.Serialization
{
    using System;
    using System.Reflection;

    internal static class SerializationExtensionMethods
    {
        // This extension method is needed because of dotnet-svcutil's use  of the private copy of the framework.
        // This method repalce Type.IsDefined, which checks the assembly name in addition to the namespace and type name.
        // This creates a problem when checking if a type has a runtime attribute on it (e.g. DataContractAttribute), because
        // it will return false because of the assembly name. This method will return true instead as long as the namesapce and type name match.
        // This should ONLY be used when checking for attributes defined in our private copy of the framework.
        public static bool IsAttributeDefined(this TypeInfo type, Type attributeType)
        {
            if (type.IsDefined(attributeType, false))
            {
                return true;
            }

            foreach (Attribute a in type.GetCustomAttributes())
            {
                if (a.GetType().FullName == attributeType.FullName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
