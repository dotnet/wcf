//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

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