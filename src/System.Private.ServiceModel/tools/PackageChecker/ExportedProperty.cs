// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace PackageChecker
{
    internal class ExportedProperty
    {
        public ExportedProperty(PropertyInfo p)
        {
            Signature = GetPropertySignature(p);
        }

        public string Signature { get; }

        private static string GetPropertySignature(PropertyInfo property)
        {
            var propertyType = GetTypeName(property.PropertyType);
            var propertyName = property.Name;

            var accessors = property.GetAccessors(true) // Include non-public to check visibility
                                    .Where(a => a.IsPublic || a.IsFamily) // Filter to include only public or protected
                                    .Select(a => a.Name.Contains("get_") ? "get" : "set")
                                    .Distinct()
                                    .ToArray();

            if (accessors.Length == 0)
            {
                // If there are no public or protected accessors, the property should not be included
                return string.Empty;
            }

            var accessorsString = string.Join("/", accessors);
            return $"{propertyType} {propertyName} {{{accessorsString}}}";
        }

        private static string GetTypeName(Type type)
        {
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
