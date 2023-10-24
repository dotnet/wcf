// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeDom;

namespace Microsoft.Tools.ServiceModel.Svcutil
{
    internal class NamespaceFixup : CodeDomVisitor
    {
        private static string s_microsoftXml = "Microsoft.Xml";
        private static string s_systemXml = "global::System.Xml";
        private static string s_microsoftCodeDom = "Microsoft.CodeDom";
        private static string s_systemCodeDom = "System.CodeDom";
        private static string s_runtimeSerialization = "System.Runtime.Serialization";
        private static string s_serviceModelMessageContract = "System.ServiceModel.Message";
        private static string s_globalRuntimeSerialization = "global::System.Runtime.Serialization";
        private static string s_globalServiceModelMessageContract = "global::System.ServiceModel.Message";
        private Dictionary<string, Type> _xmlTypes = new Dictionary<string, Type>();
        private Dictionary<string, Type> _codeDomTypes = new Dictionary<string, Type>();
        private Dictionary<string, Type> _runtimeSerializationTypes = new Dictionary<string, Type>();
        private Dictionary<string, Type> _smMessageContractAttrTypes = new Dictionary<string, Type>();

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

            // Microsoft.CodeDom
            var mscodedomTypes = TypeLoader.LoadTypes(typeof(Microsoft.CodeDom.CodeObject).GetTypeInfo().Assembly, Verbosity.Silent);
            
            foreach (var type in mscodedomTypes)
            {
                if (type.FullName.Contains(s_microsoftCodeDom))
                {
                    _codeDomTypes[type.FullName] = type;
                }
            }

            //find types of System.Runtime.Serialization and prefix with global:: to allow "System" be used as member of a data class.
            var runtimeSerTypes = TypeLoader.LoadTypes(typeof(System.Runtime.Serialization.DataMemberAttribute).GetTypeInfo().Assembly, Verbosity.Silent);

            foreach (var type in runtimeSerTypes)
            {
                if (type.FullName.Contains(s_runtimeSerialization))
                {
                    _runtimeSerializationTypes[type.FullName] = type;
                }
            }

            //find types of System.ServiceModel.MessageContractAttribute and prefix with global:: to allow "System" be used as member of a data class.
            var smMessageContractTypes = TypeLoader.LoadTypes(typeof(System.ServiceModel.MessageContractAttribute).GetTypeInfo().Assembly, Verbosity.Silent);
            foreach (var type in smMessageContractTypes)
            {
                if (type.FullName.Contains(s_serviceModelMessageContract))
                {
                    _smMessageContractAttrTypes[type.FullName] = type;
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
            else if (attr.Name.Contains(s_microsoftCodeDom) && _codeDomTypes.ContainsKey(attr.Name))
            {
                attr.Name = attr.Name.Replace(s_microsoftCodeDom, s_systemCodeDom);
            }
            else if (attr.Name.Contains(s_runtimeSerialization) && _runtimeSerializationTypes.ContainsKey(attr.Name))
            {
                attr.Name = attr.Name.Replace(s_runtimeSerialization, s_globalRuntimeSerialization);
            }
            else if (attr.Name.Contains(s_serviceModelMessageContract) && _smMessageContractAttrTypes.ContainsKey(attr.Name))
            {
                attr.Name = attr.Name.Replace(s_serviceModelMessageContract, s_globalServiceModelMessageContract);
            }
        }

        protected override void Visit(CodeComment comment)
        {
            base.Visit(comment);
            if (comment.Text.Contains(s_microsoftXml) && _xmlTypes.ContainsKey(comment.Text))
            {
                comment.Text = comment.Text.Replace(s_microsoftXml, s_systemXml);
            }
            if (comment.Text.Contains(s_microsoftCodeDom) && _codeDomTypes.ContainsKey(comment.Text))
            {
                comment.Text = comment.Text.Replace(s_microsoftCodeDom, s_systemCodeDom);
            }
        }

        protected override void Visit(CodeTypeReference typeref)
        {
            base.Visit(typeref);
            if (typeref.BaseType.Contains(s_microsoftXml) && _xmlTypes.ContainsKey(typeref.BaseType))
            {
                typeref.BaseType = typeref.BaseType.Replace(s_microsoftXml, s_systemXml);
            }
            else if (typeref.BaseType.Contains(s_microsoftCodeDom) && _codeDomTypes.ContainsKey(typeref.BaseType))
            {
                typeref.BaseType = typeref.BaseType.Replace(s_microsoftCodeDom, s_systemCodeDom);
            }
            else if (typeref.BaseType.Contains(s_runtimeSerialization) && _runtimeSerializationTypes.ContainsKey(typeref.BaseType))
            {
                typeref.BaseType = typeref.BaseType.Replace(s_runtimeSerialization, s_globalRuntimeSerialization);
            }
            else if (typeref.BaseType.Contains(s_serviceModelMessageContract) && _smMessageContractAttrTypes.ContainsKey(typeref.BaseType))
            {
                typeref.BaseType = typeref.BaseType.Replace(s_serviceModelMessageContract, s_globalServiceModelMessageContract);
            }
        }
    }
}
