﻿//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

using Microsoft.CodeDom;
using System.ServiceModel;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class RemoveSyncMethodsFromInterface : ServiceContractVisitor
    {
        protected override void VisitAttributedType(CodeTypeDeclaration type)
        {
            base.VisitAttributedType(type);

            CollectionHelpers.MapList<CodeMemberMethod>(
                type.Members,
                delegate(CodeMemberMethod method)
                {
                    return CodeDomHelpers.IsBeginMethod(method) ||
                           CodeDomHelpers.IsTaskAsyncMethod(method) ||
                           CodeDomHelpers.IsEndMethod(method);
                },
                this.CopyAttrsToTaskAsyncMethod
            );
        }

        void CopyAttrsToTaskAsyncMethod(CodeMemberMethod syncMethod, int index)
        {
            CodeMemberMethod taskAyncMethod = CodeDomHelpers.GetTaskAsyncMethodForMethod(CurrentType.Members, syncMethod);
            if (taskAyncMethod != null && !ReferenceEquals(taskAyncMethod, syncMethod))
            {
                foreach (CodeAttributeDeclaration attr in syncMethod.CustomAttributes)
                {
                    // skip [OperationContract] as that appears in both places and is guaranteed to be the same
                    if (!CodeDomHelpers.MatchType<OperationContractAttribute>(attr.AttributeType))
                    {
                        taskAyncMethod.CustomAttributes.Add(attr);
                    }
                }
            }
        }
    }
}