// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeDom;
using System.ServiceModel;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal abstract class ClientClassVisitor : SpecificTypeVisitor
    {
        protected override void OnEnterSpecificType()
        {
            base.OnEnterSpecificType();
            VisitClientClass(base.CurrentType);
        }
        protected virtual void VisitClientClass(CodeTypeDeclaration type) { }

        protected override bool IsSpecificType(CodeTypeDeclaration type)
        {
            return type.IsClass &&
                   type.BaseTypes.Count == 2 &&
                   (CodeDomHelpers.MatchGenericBaseType(type.BaseTypes[0], typeof(ClientBase<>)) ||
                    CodeDomHelpers.MatchGenericBaseType(type.BaseTypes[0], typeof(DuplexClientBase<>)));
        }
    }
}
