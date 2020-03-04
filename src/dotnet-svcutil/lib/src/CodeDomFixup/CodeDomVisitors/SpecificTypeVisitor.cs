// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal abstract class SpecificTypeVisitor : CodeDomVisitor
    {
        CodeTypeDeclaration currentType = null;
        protected CodeTypeDeclaration CurrentType
        {
            get { return this.currentType; }
        }

        protected override void Visit(CodeTypeDeclaration type)
        {
            base.Visit(type);

            if (IsSpecificType(type))
            {
                EnterSpecificType(type);
            }
            else
            {
                ExitSpecificType();
            }
        }
        protected override void Visit(CodeCompileUnit cu)
        {
            base.Visit(cu);
            ExitSpecificType();
        }

        protected override void Visit(CodeNamespace ns)
        {
            base.Visit(ns);
            ExitSpecificType();
        }

        void EnterSpecificType(CodeTypeDeclaration type)
        {
            this.currentType = type;
            OnEnterSpecificType();
        }
        void ExitSpecificType()
        {
            OnExitSpecificType();
            this.currentType = null;
        }

        protected virtual void OnEnterSpecificType() { }
        protected virtual void OnExitSpecificType() { }

        protected abstract bool IsSpecificType(CodeTypeDeclaration type);
    }

}
