﻿//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using Microsoft.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class XmlDomAttributeFixer : CodeDomVisitor
    {
        // removes members of the affected type
        static readonly Type[] filteredTypes = new Type[] { typeof(Microsoft.Xml.XmlAttribute), };
        protected override void Visit(CodeTypeDeclaration type)
        {
            base.Visit(type);

            CollectionHelpers.MapList<CodeTypeMember>(type.Members, IsNonFilteredMember, AddWarningComment);
        }

        static bool IsNonFilteredMember(CodeTypeMember member)
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

            return memberType == null || !CodeDomHelpers.MatchAnyBaseType(memberType, filteredTypes);
        }

        static void AddWarningComment(CodeTypeMember member, int index)
        {
            // CONSIDER: add warning comment for filtered member
        }
    }
}