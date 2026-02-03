// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeDom;

using System.Reflection;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    /// <summary>
    /// Makes generated top-level types internal when --internal is specified.
    ///
    /// The XmlSerializer import path does not have a CodeGenerationOptions flag for internal types,
    /// so we apply a post-pass over the CodeDOM to align with ServiceContractGenerator.InternalTypes
    /// and DataContract ImportOptions.GenerateInternal behavior.
    /// </summary>
    internal sealed class InternalTypeVisibilityFixer : CodeDomVisitor
    {
        private readonly bool _enabled;

        public InternalTypeVisibilityFixer(bool enabled)
        {
            _enabled = enabled;
        }

        protected override void FinishVisit(CodeCompileUnit cu)
        {
            if (!_enabled || cu == null || cu.Namespaces == null)
            {
                return;
            }

            foreach (CodeNamespace ns in cu.Namespaces)
            {
                if (ns == null || ns.Types == null)
                {
                    continue;
                }

                foreach (CodeTypeDeclaration type in ns.Types)
                {
                    MakeTypeInternal(type);
                }
            }
        }

        private static void MakeTypeInternal(CodeTypeDeclaration type)
        {
            if (type == null)
            {
                return;
            }

            // Only adjust top-level types (CodeNamespace.Types). Nested types are uncommon in svcutil output
            // and require different visibility flags (NestedAssembly, etc.).
            type.TypeAttributes = (type.TypeAttributes & ~TypeAttributes.VisibilityMask) | TypeAttributes.NotPublic;
        }
    }
}
