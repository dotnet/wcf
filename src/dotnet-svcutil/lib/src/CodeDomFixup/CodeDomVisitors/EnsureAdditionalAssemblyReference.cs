// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using Microsoft.CodeDom;
using System.Collections.Generic;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class EnsureAdditionalAssemblyReference : CodeDomVisitor
    {
        CodeCompileUnit compileUnit;
        Dictionary<string, string> namespacesToMatch = new Dictionary<string, string>() { { "System.ServiceModel.XmlSerializerFormatAttribute", "System.Xml.Serialization" }, { "System.Xml.Linq", "System.Xml.Linq" }, { "System.ServiceModel.Duplex", "System.ServiceModel.Extensions" } };

        List<string> alreadyAdded = new List<string>();

        protected override void Visit(CodeCompileUnit cu)
        {
            base.Visit(cu);
            this.compileUnit = cu;
        }
        protected override void Visit(CodeTypeReference typeref)
        {
            base.Visit(typeref);
            foreach (string ns in namespacesToMatch.Keys)
            {
                if (!alreadyAdded.Contains(namespacesToMatch[ns]) && typeref.BaseType.StartsWith(ns, StringComparison.Ordinal))
                {
                    EnsureAssemblyReference(namespacesToMatch[ns]);
                }
            }
        }

        void EnsureAssemblyReference(string ns)
        {
            if (!alreadyAdded.Contains(ns) && this.compileUnit != null)
            {
                if (!this.compileUnit.ReferencedAssemblies.Contains(ns + ".dll"))
                {
                    this.compileUnit.ReferencedAssemblies.Add(ns + ".dll");
                }

                alreadyAdded.Add(ns);
            }
        }
        protected override void FinishVisit(CodeCompileUnit cu)
        {
            base.FinishVisit(cu);
            Visit((object)cu);
        }
    }
}
