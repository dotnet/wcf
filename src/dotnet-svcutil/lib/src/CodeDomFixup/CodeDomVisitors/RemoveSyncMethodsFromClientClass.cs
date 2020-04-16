// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
