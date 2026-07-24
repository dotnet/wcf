// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.CodeDom;
using System.ServiceModel;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class RemoveAsyncMethodsFromInterface : ServiceContractVisitor
    {
        protected override void VisitAttributedType(CodeTypeDeclaration type)
        {
            base.VisitAttributedType(type);

            CollectionHelpers.MapList<CodeMemberMethod>(
                type.Members,
                delegate (CodeMemberMethod method)
                {
                    return !(CodeDomHelpers.IsBeginMethod(method) ||
                           CodeDomHelpers.IsTaskAsyncMethod(method) ||
                           CodeDomHelpers.IsEndMethod(method));
                },
                null
            );
        }
    }
}
