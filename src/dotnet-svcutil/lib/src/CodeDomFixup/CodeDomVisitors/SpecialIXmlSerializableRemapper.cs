// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using Microsoft.CodeDom;
using System.Collections.Generic;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class SpecialIXmlSerializableRemapper : SimpleTypeRemapper
    {
        Dictionary<string, CodeTypeDeclaration> specialIXmlSerializableTypes = new Dictionary<string, CodeTypeDeclaration>();
        TypeDeclCollection typeDeclCollection = new TypeDeclCollection();
        const string specialTypeName = "schema";
        const string specialFieldName = "schema";
        static string currentMatchingFullTypeName = null;
        readonly ArrayOfXElementTypeHelper ArrayOfXElementTypeHelper;

        public SpecialIXmlSerializableRemapper(ArrayOfXElementTypeHelper arrayOfXElementTypeHelper)
            : base(null, ArrayOfXElementTypeHelper.ArrayOfXElementRef.BaseType)
        {
            this.ArrayOfXElementTypeHelper = arrayOfXElementTypeHelper;
        }

        protected override bool Match(CodeTypeReference typeref)
        {
            if (typeref.BaseType.Contains("."))
            {
                currentMatchingFullTypeName = typeref.BaseType;
                return specialIXmlSerializableTypes.ContainsKey(typeref.BaseType);
            }
            else
            {
                return MatchTypeName(typeref.BaseType);
            }

        }

        bool MatchTypeName(string typeName)
        {
            foreach (string name in specialIXmlSerializableTypes.Keys)
            {
                if (name.EndsWith(typeName, StringComparison.Ordinal))
                {
                    currentMatchingFullTypeName = name;
                    return true;
                }
            }
            currentMatchingFullTypeName = null;
            return false;
        }

        bool IsSpecialIXmlSerializableType(CodeTypeDeclaration type)
        {
            foreach (CodeTypeMember member in type.Members)
            {
                CodeMemberProperty memberProp = member as CodeMemberProperty;
                if (memberProp != null)
                {
                    if (memberProp.Name == specialFieldName
                        && memberProp.Type.BaseType == specialTypeName
                        && !typeDeclCollection.AllTypeDecls.ContainsKey(specialTypeName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool IsIXmlSerializableType(CodeTypeDeclaration typeDecl)
        {
            foreach (CodeTypeReference typeRef in typeDecl.BaseTypes)
            {
                if(CodeDomHelpers.MatchType<Microsoft.Xml.Serialization.IXmlSerializable>(typeRef))
                    return true;
            }
            return false;
        }

        protected override void Map(CodeTypeReference typeref)
        {
            string typeNamespace = typeDeclCollection.TypeNamespaceMappings[currentMatchingFullTypeName];
            if (!string.IsNullOrEmpty(currentMatchingFullTypeName))
            {
                ArrayOfXElementTypeHelper.CheckToAdd(typeNamespace);
            }
            typeref.BaseType = string.IsNullOrEmpty(typeNamespace) ? destType : typeDeclCollection.TypeNamespaceMappings[currentMatchingFullTypeName] + "." + destType;
        }

        protected override void Visit(CodeCompileUnit cu)
        {
            base.Visit(cu);
            typeDeclCollection.Visit(cu);
            CodeTypeDeclaration typeDecl;
            foreach (string typeName in typeDeclCollection.AllTypeDecls.Keys)
            {
                typeDecl = typeDeclCollection.AllTypeDecls[typeName];
                if (IsSpecialIXmlSerializableType(typeDecl) || IsIXmlSerializableType(typeDecl))
                {
                    System.Diagnostics.Debug.Assert(!specialIXmlSerializableTypes.ContainsKey(typeName), $"Key '{typeName}' already added to dictionary!");
                    specialIXmlSerializableTypes[typeName] = typeDeclCollection.AllTypeDecls[typeName];
                }
            }
        }
        
        protected override void FinishVisit(CodeCompileUnit cu)
        {
            base.FinishVisit(cu);
            List<string> namespaceToAdd = new List<string>();
            foreach (CodeNamespace ns in cu.Namespaces)
            {
                foreach (CodeTypeDeclaration typeDecl in specialIXmlSerializableTypes.Values)
                {
                    if (ns.Types.Contains(typeDecl))
                    {
                        ns.Types.Remove(typeDecl);
                        namespaceToAdd.Add(ns.Name);
                    }
                }
            }
            foreach (string ns in namespaceToAdd)
            {
                ArrayOfXElementTypeHelper.AddToCompileUnit(cu, ns);
            }
        }
    }
}
