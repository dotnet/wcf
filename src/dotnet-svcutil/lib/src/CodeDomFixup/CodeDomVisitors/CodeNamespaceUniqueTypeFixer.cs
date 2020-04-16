// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Description;
using Microsoft.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class CodeNamespaceUniqueTypeFixer : CodeDomVisitor
    {
        protected override void Visit(CodeCompileUnit cu)
        {
            foreach (CodeNamespace ns in cu.Namespaces)
            {
                var namespaceScope = new UniqueCodeIdentifierScope();

                foreach (CodeTypeDeclaration typeDeclaration in ns.Types)
                {
                    var uniqueName = namespaceScope.AddUnique(typeDeclaration.Name, string.Empty);
                    if (uniqueName != typeDeclaration.Name)
                    {
                        typeDeclaration.Name = uniqueName;
                    }
                }
            }
        }
    }
}
