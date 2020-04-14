// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using Microsoft.CodeDom;
using System.Runtime.Serialization;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class RemoveExtensibleDataObjectImpl : DataContractVisitor
    {
        protected override void VisitAttributedType(CodeTypeDeclaration type)
        {
            base.VisitAttributedType(type);

            // remove IExtensibleDataObject impl
            CollectionHelpers.MapList<CodeTypeReference>(
                type.BaseTypes,
                delegate (CodeTypeReference typeRef)
                {
                    return !CodeDomHelpers.MatchType<IExtensibleDataObject>(typeRef);
                },
                null
            );

            // remove ExtensionData members
            CollectionHelpers.MapList<CodeTypeMember>(
                type.Members,
                delegate (CodeTypeMember member)
                {
                    return !IsExtensionDataMember(member);
                },
                null
            );
        }

        private static bool IsExtensionDataMember(CodeTypeMember member)
        {
            CodeTypeReference memberType = null;
            switch (member.Name)
            {
                case "ExtensionData": // property
                    memberType = member is CodeMemberProperty ? ((CodeMemberProperty)member).Type : null;
                    break;
                case "extensionDataField": // field
                    memberType = member is CodeMemberField ? ((CodeMemberField)member).Type : null;
                    break;
            }
            return memberType != null && CodeDomHelpers.MatchType(memberType, typeof(ExtensionDataObject));
        }
    }
}
