// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class NamespaceFixup : CodeDomVisitor
    {
        private static string s_microsoftXml = "Microsoft.Xml";
        private static string s_systemXml = "System.Xml";
        private Dictionary<string, Type> _xmlTypes = new Dictionary<string, Type>();

        public NamespaceFixup()
        {
            // Microsoft.Xml.dll
            var msxmlTypes = TypeLoader.LoadTypes(typeof(Microsoft.Xml.XmlDocument).GetTypeInfo().Assembly, Verbosity.Silent);
            foreach (var type in msxmlTypes)
            {
                if (type.FullName.Contains(s_microsoftXml))
                {
                    _xmlTypes[type.FullName] = type;
                }
            }
        }

        protected override void Visit(CodeAttributeDeclaration attr)
        {
            base.Visit(attr);
            if (attr.Name.Contains(s_microsoftXml) && _xmlTypes.ContainsKey(attr.Name))
            {
                attr.Name = attr.Name.Replace(s_microsoftXml, s_systemXml);
            }
        }

        protected override void Visit(CodeComment comment)
        {
            base.Visit(comment);
            if (comment.Text.Contains(s_microsoftXml) && _xmlTypes.ContainsKey(comment.Text))
            {
                comment.Text = comment.Text.Replace(s_microsoftXml, s_systemXml);
            }
        }

        protected override void Visit(CodeTypeReference typeref)
        {
            base.Visit(typeref);
            if (typeref.BaseType.Contains(s_microsoftXml) && _xmlTypes.ContainsKey(typeref.BaseType))
            {
                typeref.BaseType = typeref.BaseType.Replace(s_microsoftXml, s_systemXml);
            }
        }
    }
}
