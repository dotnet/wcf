//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

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