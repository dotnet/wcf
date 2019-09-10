﻿//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using Microsoft.CodeDom;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using Microsoft.CSharp;
using System.IO;
using System.Linq;
#if VB_SUPPORT
    using Microsoft.VisualBasic;
#endif

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal static class CodeDomHelpers
    {
        static object referenceKey = null;

        static object ReferenceKey
        {
            get
            {
                if (referenceKey == null)
                {
                    Type namespaceHelper = typeof(ServiceContractGenerator).GetTypeInfo().Assembly.GetType(typeof(ServiceContractGenerator).FullName + "+NamespaceHelper");
                    FieldInfo referenceKeyField = namespaceHelper.GetField("referenceKey", BindingFlags.NonPublic | BindingFlags.Static);
                    referenceKey = referenceKeyField.GetValue(null);
                }
                return referenceKey;
            }
        }

        internal static CodeAttributeDeclaration FindAttribute<T>(CodeAttributeDeclarationCollection attrs)
        {
            return CollectionHelpers.Find<CodeAttributeDeclaration>(
                attrs,
                delegate(CodeAttributeDeclaration attr)
                {
                    return MatchType<T>(attr.AttributeType);
                }
            );
        }

        internal static CodeMemberMethod FindMethodByName(CodeTypeMemberCollection members, string methodName)
        {
            return CollectionHelpers.Find<CodeMemberMethod>(
                members,
                delegate(CodeMemberMethod method)
                {
                    return method.Name == methodName;
                }
            );
        }

        internal static CodeMemberMethod GetTaskAsyncMethodForMethod(CodeTypeMemberCollection members, CodeMemberMethod method)
        {
            return CodeDomHelpers.IsTaskAsyncMethod(method) ? method : CodeDomHelpers.FindMethodByName(members, CodeDomHelpers.GetMethodNameBase(method) + "Async");
        }

        internal static CodeMemberMethod GetImplementationOfMethod(CodeTypeReference ifaceType, CodeMemberMethod method)
        {
            CodeMemberMethod m = new CodeMemberMethod();
            m.Name = method.Name;
            m.ImplementationTypes.Add(ifaceType);
            m.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            foreach (CodeParameterDeclarationExpression parameter in method.Parameters)
            {
                CodeParameterDeclarationExpression newParam = new CodeParameterDeclarationExpression(parameter.Type, parameter.Name);
                newParam.Direction = parameter.Direction;
                m.Parameters.Add(newParam);
            }
            m.ReturnType = method.ReturnType;
            return m;
        }

        static string GetMethodNameBase(CodeMemberMethod method)
        {
            string methodName = method.Name;
            if (IsBeginMethod(method))
            {
                methodName = StringHelpers.StripPrefix(methodName, "On");
                methodName = StringHelpers.StripPrefix(methodName, "Begin");
            }
            else if (IsEndMethod(method))
            {
                methodName = StringHelpers.StripPrefix(methodName, "On");
            }
            return methodName;
        }

        internal static bool IsTaskAsyncMethod(CodeMemberMethod method)
        {
            return (method.Name.EndsWith("Async", StringComparison.Ordinal) &&
                (MatchType(method.ReturnType, new CodeTypeReference(typeof(Task<>)), false, true) ||
                MatchType<Task>(method.ReturnType)));
        }

        internal static bool IsBeginMethod(CodeMemberMethod method)
        {
            int paramCount = method.Parameters.Count;
            return (method.Name.StartsWith("Begin", StringComparison.Ordinal) || method.Name.StartsWith("OnBegin", StringComparison.Ordinal)) &&
                   paramCount >= 2 &&
                   MatchType<AsyncCallback>(method.Parameters[paramCount - 2].Type) &&
                   MatchType<object>(method.Parameters[paramCount - 1].Type) &&
                   MatchType<IAsyncResult>(method.ReturnType);
        }

        internal static bool IsEndMethod(CodeMemberMethod method)
        {
            int paramCount = method.Parameters.Count;
            return (method.Name.StartsWith("End", StringComparison.Ordinal) || method.Name.StartsWith("OnEnd", StringComparison.Ordinal)) &&
                   paramCount >= 1 &&
                   (MatchType<IAsyncResult>(method.Parameters[paramCount - 1].Type) ||
                   MatchType<IAsyncResult>(method.Parameters[0].Type));
        }

        internal static bool IsOnewayMethod(CodeMemberMethod method)
        {
            CodeAttributeDeclaration operationContract = FindAttribute<OperationContractAttribute>(method.CustomAttributes);
            foreach (CodeAttributeArgument arg in operationContract.Arguments)
            {
                if (arg.Name.Equals("IsOneWay"))
                    return true;
            }
            return false;
        }

        internal static bool MatchType<T>(CodeTypeReference typeRef)
        {
            return MatchType(typeRef, typeof(T));
        }

        // exact match of Type -> CodeTypeRef
        internal static bool MatchType(CodeTypeReference typeRef, Type type)
        {
            return MatchType(typeRef, new CodeTypeReference(type));
        }

        // does a deep typeref to typeref compare
        internal static bool MatchType(CodeTypeReference typeRef1, CodeTypeReference typeRef2)
        {
            return MatchType(typeRef1, typeRef2, false/*ignoreArrayness*/, false/*ignoreGenericParameters*/);
        }

        static bool MatchType(CodeTypeReference typeRef1, CodeTypeReference typeRef2, bool ignoreArrayness, bool ignoreGenericParameters)
        {
            if (typeRef1 == null && typeRef2 == null)
                return true;
            else if (typeRef1 == null || typeRef2 == null)
                return false;
            else
                return typeRef1.BaseType == typeRef2.BaseType &&
                    (ignoreArrayness || typeRef1.ArrayRank == typeRef2.ArrayRank) &&
                    (ignoreArrayness || MatchType(typeRef1.ArrayElementType, typeRef2.ArrayElementType)) &&
                    (ignoreGenericParameters || MatchAllTypes(typeRef1.TypeArguments, typeRef2.TypeArguments));
        }

        static bool MatchAllTypes(CodeTypeReferenceCollection types1, CodeTypeReferenceCollection types2)
        {
            if (types1.Count != types2.Count)
                return false;
            else
            {
                // note that this algorithm assumes that the type collections have the same sort order 
                // which is true for all the cases we currently care about (most commonly where there's one argument)
                // but may not be true for future cases so be aware of it
                for (int i = 0; i < types1.Count; i++)
                {
                    if (!MatchType(types1[i], types2[i]))
                        return false;
                }
                return true;
            }
        }

        internal static bool MatchGenericBaseType(CodeTypeReference typeRef, Type type)
        {
            return MatchType(typeRef, new CodeTypeReference(type), false/*ignoreArrayness*/, true/*ignoreGenericParameters*/);
        }

        internal static bool MatchBaseType(CodeTypeReference typeRef, Type type)
        {
            return MatchType(typeRef, new CodeTypeReference(type), true/*ignoreArrayness*/, true/*ignoreGenericParameters*/);
        }

        internal static bool MatchAnyBaseType(CodeTypeReference typeRef, Type[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (MatchBaseType(typeRef, types[i]))
                    return true;
            }
            return false;
        }

        internal static bool MatchAnyBaseType(CodeTypeReference typeRef, CodeTypeReference[] types)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (MatchType(typeRef, types[i], false/*ignoreArrayness*/, true/*ignoreGenericParameters*/))
                    return true;
            }
            return false;
        }

        internal static bool MatchSignatures(CodeParameterDeclarationExpressionCollection args1, Type[] args2)
        {
            if (args1.Count != args2.Length)
                return false;

            for (int i = 0; i < args2.Length; i++)
            {
                if (!MatchType(args1[i].Type, args2[i]))
                    return false;
            }

            return true;
        }

        internal static CodeTypeDeclaration ResolveTypeReference(CodeTypeReference type)
        {
            return type.UserData[ReferenceKey] as CodeTypeDeclaration;
        }

        // we are not sure if this is vb or csharp project. So escape both.
        internal static string EscapeName(string identifierName)
        {
#if VB_SUPPORT
            using (VBCodeProvider vbCodeProvider = new VBCodeProvider())
            {
                identifierName = vbCodeProvider.CreateEscapedIdentifier(identifierName);
            }
#endif
            using (CSharpCodeProvider cSharpCodeProvider = new CSharpCodeProvider())
            {
                identifierName = cSharpCodeProvider.CreateEscapedIdentifier(identifierName);
            }
            return identifierName;
        }

        internal static string GetValidValueTypeIdentifier(string value)
        {
            var builder = new System.Text.StringBuilder();
            if (!string.IsNullOrEmpty(value))
            {
                value = EscapeName(value);
                foreach (var ch in value)
                {
                    var charVal = char.IsLetterOrDigit(ch) ? ch : '_';
                    builder.Append(charVal);
                }
                if (char.IsDigit(builder[0]))
                {
                    builder[0] = '_';
                }
            }
            return builder.ToString();
        }

        public static bool IsValidNameSpace(string name)
        {
            return !String.IsNullOrWhiteSpace(name) &&
                name.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 &&
                name.Split('.').All(namespacePart => new CSharpCodeProvider().IsValidIdentifier(namespacePart));
        }
    }
}