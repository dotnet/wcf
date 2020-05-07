// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.CodeDom;
using System.Collections.Generic;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class SpecialIXmlSerializableRemapper : SimpleTypeRemapper
    {
        private Dictionary<string, CodeTypeDeclaration> _specialIXmlSerializableTypes = new Dictionary<string, CodeTypeDeclaration>();
        private TypeDeclCollection _typeDeclCollection = new TypeDeclCollection();
        private const string specialTypeName = "schema";
        private const string specialFieldName = "schema";
        private static string s_currentMatchingFullTypeName = null;
        private readonly ArrayOfXElementTypeHelper _arrayOfXElementTypeHelper;

        public SpecialIXmlSerializableRemapper(ArrayOfXElementTypeHelper arrayOfXElementTypeHelper)
            : base(null, ArrayOfXElementTypeHelper.ArrayOfXElementRef.BaseType)
        {
            _arrayOfXElementTypeHelper = arrayOfXElementTypeHelper;
        }

        protected override bool Match(CodeTypeReference typeref)
        {
            if (typeref.BaseType.Contains("."))
            {
                s_currentMatchingFullTypeName = typeref.BaseType;
                return _specialIXmlSerializableTypes.ContainsKey(typeref.BaseType);
            }
            else
            {
                return MatchTypeName(typeref.BaseType);
            }
        }

        private bool MatchTypeName(string typeName)
        {
            foreach (string name in _specialIXmlSerializableTypes.Keys)
            {
                if (name.EndsWith(typeName, StringComparison.Ordinal))
                {
                    s_currentMatchingFullTypeName = name;
                    return true;
                }
            }
            s_currentMatchingFullTypeName = null;
            return false;
        }

        private bool IsSpecialIXmlSerializableType(CodeTypeDeclaration type)
        {
            foreach (CodeTypeMember member in type.Members)
            {
                CodeMemberProperty memberProp = member as CodeMemberProperty;
                if (memberProp != null)
                {
                    if (memberProp.Name == specialFieldName
                        && memberProp.Type.BaseType == specialTypeName
                        && !_typeDeclCollection.AllTypeDecls.ContainsKey(specialTypeName))
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
                if (CodeDomHelpers.MatchType<Microsoft.Xml.Serialization.IXmlSerializable>(typeRef))
                    return true;
            }
            return false;
        }

        protected override void Map(CodeTypeReference typeref)
        {
            string typeNamespace = _typeDeclCollection.TypeNamespaceMappings[s_currentMatchingFullTypeName];
            if (!string.IsNullOrEmpty(s_currentMatchingFullTypeName))
            {
                _arrayOfXElementTypeHelper.CheckToAdd(typeNamespace);
            }
            typeref.BaseType = string.IsNullOrEmpty(typeNamespace) ? destType : _typeDeclCollection.TypeNamespaceMappings[s_currentMatchingFullTypeName] + "." + destType;
        }

        protected override void Visit(CodeCompileUnit cu)
        {
            base.Visit(cu);
            _typeDeclCollection.Visit(cu);
            CodeTypeDeclaration typeDecl;
            foreach (string typeName in _typeDeclCollection.AllTypeDecls.Keys)
            {
                typeDecl = _typeDeclCollection.AllTypeDecls[typeName];
                if (IsSpecialIXmlSerializableType(typeDecl) || IsIXmlSerializableType(typeDecl))
                {
                    System.Diagnostics.Debug.Assert(!_specialIXmlSerializableTypes.ContainsKey(typeName), $"Key '{typeName}' already added to dictionary!");
                    _specialIXmlSerializableTypes[typeName] = _typeDeclCollection.AllTypeDecls[typeName];
                }
            }
        }

        protected override void FinishVisit(CodeCompileUnit cu)
        {
            base.FinishVisit(cu);
            List<string> namespaceToAdd = new List<string>();
            foreach (CodeNamespace ns in cu.Namespaces)
            {
                foreach (CodeTypeDeclaration typeDecl in _specialIXmlSerializableTypes.Values)
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
                _arrayOfXElementTypeHelper.AddToCompileUnit(cu, ns);
            }
        }
    }
}
