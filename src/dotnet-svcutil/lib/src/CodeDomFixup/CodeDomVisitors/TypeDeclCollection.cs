// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ServiceModel.Description;
using Microsoft.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class TypeDeclCollection
    {
        internal Dictionary<string, CodeTypeDeclaration> AllTypeDecls = new Dictionary<string, CodeTypeDeclaration>();
        internal Dictionary<string, string> TypeNamespaceMappings = new Dictionary<string, string>();

        internal void Visit(CodeCompileUnit cu)
        {
            foreach (CodeNamespace ns in cu.Namespaces)
            {
                foreach (CodeTypeDeclaration typeDeclaration in ns.Types)
                {
                    var typeNamespace = ns.Name;
                    var fullName = string.IsNullOrEmpty(typeNamespace) ? typeDeclaration.Name : typeNamespace + "." + typeDeclaration.Name;
                    AddType(fullName, typeDeclaration, typeNamespace);
                }
            }
        }

        private void AddNestedTypes(string fullName, CodeTypeDeclaration type, string typeNamespace)
        {
            foreach (CodeTypeMember member in type.Members)
            {
                CodeTypeDeclaration typeDeclaration = member as CodeTypeDeclaration;
                if (typeDeclaration != null)
                {
                    fullName = fullName + "+" + typeDeclaration.Name;
                    AddType(fullName, typeDeclaration, typeNamespace);
                }
            }
        }

        private void AddType(string fullName, CodeTypeDeclaration typeDeclaration, string typeNamespace)
        {
            System.Diagnostics.Debug.Assert(!AllTypeDecls.ContainsKey(fullName), $"Key '{fullName}' already added to dictionary!");

            AllTypeDecls[fullName] = typeDeclaration;
            TypeNamespaceMappings[fullName] = typeNamespace;

            AddNestedTypes(fullName, typeDeclaration, typeNamespace);
        }
    }
}
