// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal abstract class TypeWithAttributeVisitor<T> : SpecificTypeVisitor
    where T : Attribute
    {
        protected override bool IsSpecificType(CodeTypeDeclaration type)
        {
            return CodeDomHelpers.FindAttribute<T>(type.CustomAttributes) != null;
        }

        protected override void OnEnterSpecificType()
        {
            base.OnEnterSpecificType();
            VisitAttributedType(base.CurrentType);
        }

        protected virtual void VisitAttributedType(CodeTypeDeclaration type) { }
    }
}
