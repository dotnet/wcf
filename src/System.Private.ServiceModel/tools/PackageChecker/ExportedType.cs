// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Text;

namespace PackageChecker
{
    internal class ExportedType
    {
        public ExportedType(Type exportedType)
        {
            TypeName = exportedType.FullName;
            IsStatic = exportedType.IsAbstract && exportedType.IsSealed;
            IsInterface = exportedType.IsInterface;
            this.Assembly = exportedType.Assembly.GetName().Name;
            var constructors = exportedType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            Constructors = new List<ExportedMethod>(constructors.Where(c => c.IsPublic | c.IsFamily).Select(c => new ExportedMethod(c)));
            var methods = exportedType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            Methods = new List<ExportedMethod>(methods.Where(m => m.IsPublic | m.IsFamily).Select(m => new ExportedMethod(m)));
            var properties = exportedType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            Properties = new List<ExportedProperty>(properties.Where(p => p.GetMethod.IsPublic | p.GetMethod.IsFamily).Select(p => new ExportedProperty(p)));
            var events = exportedType.GetEvents(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            Events = new List<ExportedEvent>(events.Where(e => e.AddMethod.IsPublic | e.AddMethod.IsFamily).Select(e => new ExportedEvent(e)));
            ImplementedInterfaces = new List<string>(exportedType.GetInterfaces().Where(i => i.IsPublic).Select(i => i.FullName));
        }

        public string TypeName { get; }
        public string Assembly { get; }
        public List<string> ImplementedInterfaces { get; }
        public bool IsStatic { get; }
        public bool IsInterface { get; }
        public List<ExportedMethod> Methods { get; }
        public List<ExportedProperty> Properties { get; }
        public List<ExportedEvent> Events { get; }
        public List<ExportedMethod> Constructors { get; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"TypeName: {TypeName}");
            sb.AppendLine($"Assembly: {Assembly}");
            sb.AppendLine($"IsStatic: {IsStatic}");
            sb.AppendLine($"IsInterface: {IsInterface}");
            sb.AppendLine("Constructors:");
            foreach (var constructor in Constructors)
            {
                sb.AppendLine($"  {constructor.Signature}");
            }
            sb.AppendLine("Methods:");
            foreach (var method in Methods)
            {
                sb.AppendLine($"  {method.Signature}");
            }
            sb.AppendLine("Properties:");
            foreach (var property in Properties)
            {
                sb.AppendLine($"  {property.Signature}");
            }
            sb.AppendLine("Implemented Interfaces:");
            foreach (var implementedInterface in ImplementedInterfaces)
            {
                sb.AppendLine($"  {implementedInterface}");
            }
            return sb.ToString();
        }

        internal bool IsMatchingImplmentation(ExportedType candidateType)
        {
            bool matches = true;
            if (TypeName != candidateType.TypeName)
                return false;

            foreach (var implementedInterface in ImplementedInterfaces)
            {
                if (!candidateType.ImplementedInterfaces.Any(i => i == implementedInterface))
                {
                    Console.Error.WriteLine($"Type {TypeName} is missing implementing interface {implementedInterface} in the implementation.");
                    matches = false;
                }
            }

            foreach (var ev in Events)
            {
                if (!candidateType.Events.Any(e => e.Signature == ev.Signature))
                {
                    Console.Error.WriteLine($"Type {TypeName} is missing event {ev.Signature} in the implementation.");
                    matches = false;
                }
            }

            foreach (var property in Properties)
            {
                if (!candidateType.Properties.Any(p => p.Signature == property.Signature))
                {
                    Console.Error.WriteLine($"Type {TypeName} is missing property {property.Signature} in the implementation.");
                    matches = false;
                }
            }

            foreach (var method in Methods)
            {
                if (!candidateType.Methods.Any(m => m.Signature == method.Signature))
                {
                    Console.Error.WriteLine($"Type {TypeName} is missing method {method.Signature} in the implementation.");
                    matches = false;
                }
            }

            foreach (var constructor in Constructors)
            {
                if (!candidateType.Constructors.Any(c => c.Signature == constructor.Signature))
                {
                    Console.Error.WriteLine($"Type {TypeName} is missing constructor {constructor.Signature} in the implementation.");
                    matches = false;
                }
            }

            return matches;
        }
    }
}
