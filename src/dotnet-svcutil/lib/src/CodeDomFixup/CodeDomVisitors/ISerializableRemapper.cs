#if disabled // TODO: ISerializable not available, do we need it?
//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    using Microsoft.CodeDom;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    internal class ISerializableRemapper : SimpleTypeRemapper
    {
        Dictionary<string, CodeTypeDeclaration> iSerializableTypes = new Dictionary<string, CodeTypeDeclaration>();
        TypeDeclCollection typeDeclCollection = new TypeDeclCollection();
        readonly ArrayOfXElementTypeHelper ArrayOfXElementTypeHelper;

        public ISerializableRemapper(ArrayOfXElementTypeHelper arrayOfXElementTypeHelper)
            : base(typeof(ISerializable), ArrayOfXElementTypeHelper.ArrayOfXElementRef.BaseType)
        {
            this.ArrayOfXElementTypeHelper = arrayOfXElementTypeHelper;
        }

        protected override bool Match(CodeTypeReference typeref)
        {
            return iSerializableTypes.ContainsKey(typeref.BaseType);
        }

        bool IsISerializableType(CodeTypeDeclaration type)
        {
            foreach (CodeTypeReference typeRef in type.BaseTypes)
            {
                CodeTypeDeclaration baseDecl;
                if (CodeDomHelpers.MatchType(typeRef, srcType)
                    || (typeDeclCollection.AllTypeDecls.TryGetValue(typeRef.BaseType, out baseDecl) && IsISerializableType(baseDecl)))
                    return true;
            }
            return false;
        }

        protected override void Map(CodeTypeReference typeref)
        {
            string typeNamespace = typeDeclCollection.TypeNamespaceMappings[typeref.BaseType];
            ArrayOfXElementTypeHelper.CheckToAdd(typeDeclCollection.TypeNamespaceMappings[typeref.BaseType]);
            typeref.BaseType = string.IsNullOrEmpty(typeNamespace) ? destType : typeNamespace + "." + destType;
        }

        protected override void Visit(CodeCompileUnit cu)
        {
            base.Visit(cu);
            typeDeclCollection.Visit(cu);
            foreach (string typeName in typeDeclCollection.AllTypeDecls.Keys)
            {
                if (IsISerializableType(typeDeclCollection.AllTypeDecls[typeName]))
                {
                    iSerializableTypes.Add(typeName, typeDeclCollection.AllTypeDecls[typeName]);
                }
            }
        }
        
        protected override void FinishVisit(CodeCompileUnit cu)
        {
            base.FinishVisit(cu);
            List<string> namespaceToAddType = new List<string>();
            foreach (CodeNamespace ns in cu.Namespaces)
            {
                foreach (CodeTypeDeclaration typeDecl in iSerializableTypes.Values)
                {
                    if (ns.Types.Contains(typeDecl))
                    {
                        ns.Types.Remove(typeDecl);
                        namespaceToAddType.Add(ns.Name);
                    }
                }
            }
            foreach (string ns in namespaceToAddType)
            {
                ArrayOfXElementTypeHelper.AddToCompileUnit(cu, ns);
            }
        }
    }
}
#endif