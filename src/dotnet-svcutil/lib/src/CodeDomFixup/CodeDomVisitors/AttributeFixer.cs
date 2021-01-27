// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.CodeDom;
using Microsoft.CodeDom.Compiler;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class AttributeFixer : CodeDomVisitor
    {
        private static Type[] s_attrsToRemove = new Type[]
                    {
                        typeof(TransactionFlowAttribute),
                    };

        private static string[] s_serviceContractPropsToRemove = new string[]
                    {
                        "ProtectionLevel",
                        "SessionMode",
                    };

        private static string[] s_operationContractPropsToRemove = new string[]
                    {
                        "ProtectionLevel",
                        "IsInitiating",
                        "IsTerminating",
                    };

        private static string[] s_faultContractPropsToRemove = new string[]
                    {
                        "ProtectionLevel"
                    };

        public AttributeFixer(ServiceContractGenerator generator)
        {
            System.Diagnostics.Debug.Assert(generator != null);
        }

        protected override void Visit(CodeTypeDeclaration type)
        {
            base.Visit(type);

            CollectionHelpers.MapList<CodeAttributeDeclaration>(type.CustomAttributes, IsValidAttribute, null);
        }

        protected override void Visit(CodeTypeMember member)
        {
            base.Visit(member);

            CollectionHelpers.MapList<CodeAttributeDeclaration>(member.CustomAttributes, IsValidAttribute, null);
        }

        protected override void Visit(CodeAttributeDeclaration attr)
        {
            base.Visit(attr);

            string[] propsToRemove = null;
            if (CodeDomHelpers.MatchType(attr.AttributeType, typeof(ServiceContractAttribute)))
            {
                propsToRemove = s_serviceContractPropsToRemove;
            }
            else if (CodeDomHelpers.MatchType(attr.AttributeType, typeof(OperationContractAttribute)))
            {
                propsToRemove = s_operationContractPropsToRemove;
            }
            else if (CodeDomHelpers.MatchType(attr.AttributeType, typeof(FaultContractAttribute)))
            {
                propsToRemove = s_faultContractPropsToRemove;
            }
            else if (CodeDomHelpers.MatchType(attr.AttributeType, typeof(GeneratedCodeAttribute)))
            {
                attr.Arguments.Clear();
                attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(Tool.ToolName)));
                attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(Tool.PackageVersion)));
            }

            if (propsToRemove != null)
            {
                CollectionHelpers.MapList<CodeAttributeArgument>(attr.Arguments,
                    delegate (CodeAttributeArgument arg)
                    {
                        return IsValidProperty(propsToRemove, arg.Name);
                    },
                    null
                );
            }
        }

        protected override void FinishVisit(CodeCompileUnit cu)
        {
        }

        private static bool IsValidProperty(string[] propsToRemove, string prop)
        {
            for (int i = 0; i < propsToRemove.Length; i++)
            {
                if (propsToRemove[i] == prop)
                    return false;
            }
            return true;
        }

        private static bool IsValidAttribute(CodeAttributeDeclaration attr)
        {
            for (int i = 0; i < s_attrsToRemove.Length; i++)
            {
                if (CodeDomHelpers.MatchType(attr.AttributeType, s_attrsToRemove[i]))
                    return false;
            }
            return true;
        }
    }
}
