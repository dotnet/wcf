// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeDom;
using System.Collections.Generic;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class EnsureAdditionalAssemblyReference : CodeDomVisitor
    {
        private CodeCompileUnit _compileUnit;
        private Dictionary<string, string> _namespacesToMatch = new Dictionary<string, string>() { { "System.ServiceModel.XmlSerializerFormatAttribute", "System.Xml.Serialization" }, { "System.Xml.Linq", "System.Xml.Linq" }, { "System.ServiceModel.Duplex", "System.ServiceModel.Extensions" } };

        private List<string> _alreadyAdded = new List<string>();

        protected override void Visit(CodeCompileUnit cu)
        {
            base.Visit(cu);
            _compileUnit = cu;
        }
        protected override void Visit(CodeTypeReference typeref)
        {
            base.Visit(typeref);
            foreach (string ns in _namespacesToMatch.Keys)
            {
                if (!_alreadyAdded.Contains(_namespacesToMatch[ns]) && typeref.BaseType.StartsWith(ns, StringComparison.Ordinal))
                {
                    EnsureAssemblyReference(_namespacesToMatch[ns]);
                }
            }
        }

        private void EnsureAssemblyReference(string ns)
        {
            if (!_alreadyAdded.Contains(ns) && _compileUnit != null)
            {
                if (!_compileUnit.ReferencedAssemblies.Contains(ns + ".dll"))
                {
                    _compileUnit.ReferencedAssemblies.Add(ns + ".dll");
                }

                _alreadyAdded.Add(ns);
            }
        }
        protected override void FinishVisit(CodeCompileUnit cu)
        {
            base.FinishVisit(cu);
            Visit((object)cu);
        }
    }
}
