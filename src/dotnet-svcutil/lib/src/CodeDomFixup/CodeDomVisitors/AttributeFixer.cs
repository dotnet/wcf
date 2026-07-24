// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class AttributeFixer : CodeDomVisitor
    {
        private static readonly string s_excludeFromCodeCoverageAttributeFullName = typeof(ExcludeFromCodeCoverageAttribute).FullName;

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

            FixupGeneratedCodeAndEnsureExcludeFromCoverage(type.CustomAttributes, CanApplyExcludeFromCodeCoverage(type));
        }

        protected override void Visit(CodeTypeMember member)
        {
            base.Visit(member);

            CollectionHelpers.MapList<CodeAttributeDeclaration>(member.CustomAttributes, IsValidAttribute, null);

            // Runs before `CodeDomVisitor` enumerates `member.CustomAttributes`.
            FixupGeneratedCodeAndEnsureExcludeFromCoverage(member.CustomAttributes, CanApplyExcludeFromCodeCoverage(member));
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

        private static void FixupGeneratedCodeAndEnsureExcludeFromCoverage(CodeAttributeDeclarationCollection attributes, bool canAddExcludeFromCoverage)
        {
            if (attributes == null || attributes.Count == 0)
            {
                return;
            }

            bool hasGeneratedCode = false;
            bool hasExcludeFromCoverage = false;

            for (int i = 0; i < attributes.Count; i++)
            {
                CodeAttributeDeclaration attr = attributes[i];
                if (attr == null)
                {
                    continue;
                }
                if (CodeDomHelpers.MatchType(attr.AttributeType, typeof(GeneratedCodeAttribute)))
                {
                    hasGeneratedCode = true;
                    attr.Arguments.Clear();
                    attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(Tool.ToolName)));
                    attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(Tool.PackageVersion)));
                }
                else if (IsExcludeFromCodeCoverageAttribute(attr.AttributeType))
                {
                    hasExcludeFromCoverage = true;
                }

                if (hasGeneratedCode && hasExcludeFromCoverage)
                {
                    break;
                }
            }

            if (hasGeneratedCode && !hasExcludeFromCoverage)
            {
                if (canAddExcludeFromCoverage)
                {
                    attributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(s_excludeFromCodeCoverageAttributeFullName)));
                }
            }
        }

        private static bool CanApplyExcludeFromCodeCoverage(CodeTypeDeclaration type)
        {
            return type != null && (type.IsClass || type.IsStruct);
        }

        private static bool CanApplyExcludeFromCodeCoverage(CodeTypeMember member)
        {
            return member is CodeConstructor ||
                   member is CodeMemberMethod ||
                   member is CodeMemberProperty ||
                   member is CodeMemberEvent;
        }

        private static bool IsExcludeFromCodeCoverageAttribute(CodeTypeReference attributeType)
        {
            if (attributeType == null)
            {
                return false;
            }

            string baseType = attributeType.BaseType;
            if (string.IsNullOrEmpty(baseType))
            {
                return false;
            }

            return baseType == s_excludeFromCodeCoverageAttributeFullName ||
                   baseType == "System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage" ||
                   baseType == "ExcludeFromCodeCoverageAttribute" ||
                   baseType == "ExcludeFromCodeCoverage" ||
                   baseType.EndsWith(".ExcludeFromCodeCoverageAttribute", StringComparison.Ordinal) ||
                   baseType.EndsWith(".ExcludeFromCodeCoverage", StringComparison.Ordinal);
        }
    }
}
