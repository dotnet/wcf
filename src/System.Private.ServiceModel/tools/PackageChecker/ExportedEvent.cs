// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace PackageChecker
{
    internal class ExportedEvent
    {
        public ExportedEvent(EventInfo e)
        {
            Signature = GetEventSignature(e);
        }

        private string GetEventSignature(EventInfo e)
        {
            var eventType = GetTypeName(e.EventHandlerType);
            var eventName = e.Name;

            var accessors = new[] { e.GetAddMethod(), e.GetRemoveMethod() }
                .Where(a => a.IsPublic || a.IsFamily) // Filter to include only public or protected
                .Select(a => a.Name.Contains("add_") ? "add" : "remove")
                .Distinct()
                .ToArray();

            if (accessors.Length == 0)
            {
                // If there are no public or protected accessors, the event should not be included
                return string.Empty;
            }

            var accessorsString = string.Join("/", accessors);
            return $"{eventType} {eventName} {{{accessorsString}}}";
        }

        public string Signature { get; }

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
