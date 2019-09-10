//-----------------------------------------------------------------------------
// <copyright company="Microsoft">
//   Copyright (C) Microsoft Corporation. All Rights Reserved.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{

    internal class NamespaceFixup : CodeDomVisitor
    {
        private static string MicrosoftXml = "Microsoft.Xml";
        private static string SystemXml = "System.Xml";
        private static string MicrosoftCodeDom = "Microsoft.CodeDom";
        private static string SystemCodeDom = "System.CodeDom";
        private Dictionary<string, Type> xmlTypes = new Dictionary<string, Type>();
        private Dictionary<string, Type> codeDomTypes = new Dictionary<string, Type>();

        public NamespaceFixup()
        {
            // Microsoft.Xml.dll
            var msxmlTypes = TypeLoader.LoadTypes(typeof(Microsoft.Xml.XmlDocument).GetTypeInfo().Assembly, Verbosity.Silent);
            foreach (var type in msxmlTypes)
            {
                if (type.FullName.Contains(MicrosoftXml))
                {
                    xmlTypes[type.FullName] = type;
                }
            }
#if disabled
            // System.Runtime.Serialization.dll
            var msxmlSerializationTypes = InputModule.LoadTypes(typeof(Microsoft.Xml.XmlDictionaryReader).GetTypeInfo().Assembly, Verbosity.Silent);
            foreach (var type in msxmlSerializationTypes)
            {
                if (type.FullName.Contains(MicrosoftXml))
                {
                    xmlTypes[type.FullName] = type;
                }
            }
#endif
            // Microsoft.CodeDom
            var mscodedomTypes = TypeLoader.LoadTypes(typeof(Microsoft.CodeDom.CodeObject).GetTypeInfo().Assembly, Verbosity.Silent);
            foreach (var type in mscodedomTypes)
            {
                if (type.FullName.Contains(MicrosoftCodeDom))
                {
                    codeDomTypes[type.FullName] = type;
                }
            }
        }

        protected override void Visit(CodeAttributeDeclaration attr)
        {
            base.Visit(attr);
            if (attr.Name.Contains(MicrosoftXml) && xmlTypes.ContainsKey(attr.Name))
            {
                attr.Name = attr.Name.Replace(MicrosoftXml, SystemXml);
            }
            if (attr.Name.Contains(MicrosoftCodeDom) && codeDomTypes.ContainsKey(attr.Name))
            {
                attr.Name = attr.Name.Replace(MicrosoftCodeDom, SystemCodeDom);
            }
        }

        protected override void Visit(CodeComment comment)
        {
            base.Visit(comment);
            if (comment.Text.Contains(MicrosoftXml) && xmlTypes.ContainsKey(comment.Text))
            {
                comment.Text = comment.Text.Replace(MicrosoftXml, SystemXml);
            }
            if (comment.Text.Contains(MicrosoftCodeDom) && codeDomTypes.ContainsKey(comment.Text))
            {
                comment.Text = comment.Text.Replace(MicrosoftCodeDom, SystemCodeDom);
            }
        }

        protected override void Visit(CodeTypeReference typeref)
        {
            base.Visit(typeref);
            if (typeref.BaseType.Contains(MicrosoftXml) && xmlTypes.ContainsKey(typeref.BaseType))
            {
                typeref.BaseType = typeref.BaseType.Replace(MicrosoftXml, SystemXml);
            }
            if (typeref.BaseType.Contains(MicrosoftCodeDom) && codeDomTypes.ContainsKey(typeref.BaseType))
            {
                typeref.BaseType = typeref.BaseType.Replace(MicrosoftCodeDom, SystemCodeDom);
            }
        }
    }
}
