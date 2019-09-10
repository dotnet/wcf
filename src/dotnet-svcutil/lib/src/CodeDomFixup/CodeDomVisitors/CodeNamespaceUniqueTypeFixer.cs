//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

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