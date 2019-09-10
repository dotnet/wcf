//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

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
        static Type[] attrsToRemove = new Type[]
                    {
#if disabled
                        typeof(DesignerCategoryAttribute),
                        typeof(SerializableAttribute),
#endif
                        typeof(TransactionFlowAttribute),
                    };

        static string[] serviceContractPropsToRemove = new string[]
                    {
                        "ProtectionLevel",
                        "SessionMode",
                    };

        static string[] operationContractPropsToRemove = new string[]
                    {
                        "ProtectionLevel",
                        "IsInitiating",
                        "IsTerminating",
                    };

        static string[] faultContractPropsToRemove = new string[]
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
                propsToRemove = serviceContractPropsToRemove;
            }
            else if (CodeDomHelpers.MatchType(attr.AttributeType, typeof(OperationContractAttribute)))
            {
                propsToRemove = operationContractPropsToRemove;
            }
            else if (CodeDomHelpers.MatchType(attr.AttributeType, typeof(FaultContractAttribute)))
            {
                propsToRemove = faultContractPropsToRemove;
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
                    delegate(CodeAttributeArgument arg)
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

        static bool IsValidProperty(string[] propsToRemove, string prop)
        {
            for (int i = 0; i < propsToRemove.Length; i++)
            {
                if (propsToRemove[i] == prop)
                    return false;
            }
            return true;
        }

        static bool IsValidAttribute(CodeAttributeDeclaration attr)
        {
            for (int i = 0; i < attrsToRemove.Length; i++)
            {
                if (CodeDomHelpers.MatchType(attr.AttributeType, attrsToRemove[i]))
                    return false;
            }
            return true;
        }
    }
}