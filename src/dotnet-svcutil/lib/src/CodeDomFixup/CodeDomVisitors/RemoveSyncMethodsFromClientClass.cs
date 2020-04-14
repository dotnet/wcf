// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    using Microsoft.CodeDom;

    internal class RemoveSyncMethodsFromClientClass : ClientClassVisitor
    {
        protected override void VisitClientClass(CodeTypeDeclaration type)
        {
            base.VisitClientClass(type);

            CollectionHelpers.MapList<CodeMemberMethod>(
                type.Members,
                delegate (CodeMemberMethod method)
                {
                    return method is CodeConstructor ||
                           CodeDomHelpers.IsBeginMethod(method) ||
                           CodeDomHelpers.IsEndMethod(method) ||
                           CodeDomHelpers.IsTaskAsyncMethod(method);
                },
                null
            );
        }
    }
}
