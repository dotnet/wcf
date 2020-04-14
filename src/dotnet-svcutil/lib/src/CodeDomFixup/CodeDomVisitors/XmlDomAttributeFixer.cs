// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using Microsoft.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class XmlDomAttributeFixer : CodeDomVisitor
    {
        // removes members of the affected type
        private static readonly Type[] s_filteredTypes = new Type[] { typeof(Microsoft.Xml.XmlAttribute), };
        protected override void Visit(CodeTypeDeclaration type)
        {
            base.Visit(type);

            CollectionHelpers.MapList<CodeTypeMember>(type.Members, IsNonFilteredMember, AddWarningComment);
        }

        private static bool IsNonFilteredMember(CodeTypeMember member)
        {
            CodeTypeReference memberType = null;

            CodeMemberProperty memberProp = member as CodeMemberProperty;
            if (memberProp != null)
            {
                memberType = memberProp.Type;
            }
            else
            {
                CodeMemberField memberField = member as CodeMemberField;
                if (memberField != null)
                {
                    memberType = memberField.Type;
                }
            }

            return memberType == null || !CodeDomHelpers.MatchAnyBaseType(memberType, s_filteredTypes);
        }

        private static void AddWarningComment(CodeTypeMember member, int index)
        {
            // CONSIDER: add warning comment for filtered member
        }
    }
}
