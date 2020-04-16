// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using Microsoft.CodeDom;
using System.Globalization;
using System.Text;
using Microsoft.Xml.Serialization;
using Microsoft.CodeDom.Compiler;
using System.Runtime.Serialization;
using System.Reflection;

namespace System.ServiceModel.Description
{
    internal class XmlSerializerOperationGenerator : IOperationBehavior, IOperationContractGenerationExtension
    {
        private OperationGenerator _operationGenerator;
        private Dictionary<MessagePartDescription, PartInfo> _partInfoTable;
        private Dictionary<OperationDescription, XmlSerializerFormatAttribute> _operationAttributes = new Dictionary<OperationDescription, XmlSerializerFormatAttribute>();
        private XmlCodeExporter _xmlExporter;
        private SoapCodeExporter _soapExporter;

        private XmlSerializerImportOptions _options;
        private CodeNamespace _codeNamespace;

        internal XmlSerializerOperationGenerator(XmlSerializerImportOptions options)
        {
            _operationGenerator = new OperationGenerator();
            _options = options;
            _codeNamespace = GetTargetCodeNamespace(options);
            _partInfoTable = new Dictionary<MessagePartDescription, PartInfo>();
        }

        private static CodeNamespace GetTargetCodeNamespace(XmlSerializerImportOptions options)
        {
            CodeNamespace targetCodeNamespace = null;
            string clrNamespace = options.ClrNamespace ?? string.Empty;
            foreach (CodeNamespace ns in options.CodeCompileUnit.Namespaces)
            {
                if (ns.Name == clrNamespace)
                {
                    targetCodeNamespace = ns;
                }
            }
            if (targetCodeNamespace == null)
            {
                targetCodeNamespace = new CodeNamespace(clrNamespace);
                options.CodeCompileUnit.Namespaces.Add(targetCodeNamespace);
            }
            return targetCodeNamespace;
        }

        internal void Add(MessagePartDescription part, XmlMemberMapping memberMapping, XmlMembersMapping membersMapping, bool isEncoded)
        {
            PartInfo partInfo = new PartInfo();
            partInfo.MemberMapping = memberMapping;
            partInfo.MembersMapping = membersMapping;
            partInfo.IsEncoded = isEncoded;
            _partInfoTable[part] = partInfo;
        }

        public XmlCodeExporter XmlExporter
        {
            get
            {
                if (_xmlExporter == null)
                {
                    _xmlExporter = new XmlCodeExporter(_codeNamespace, _options.CodeCompileUnit, _options.CodeProvider,
                        _options.WebReferenceOptions.CodeGenerationOptions, null);
                }
                return _xmlExporter;
            }
        }

        public SoapCodeExporter SoapExporter
        {
            get
            {
                if (_soapExporter == null)
                {
                    _soapExporter = new SoapCodeExporter(_codeNamespace, _options.CodeCompileUnit, _options.CodeProvider,
                        _options.WebReferenceOptions.CodeGenerationOptions, null);
                }
                return _soapExporter;
            }
        }

        private OperationGenerator OperationGenerator
        {
            get { return _operationGenerator; }
        }

        internal Dictionary<OperationDescription, XmlSerializerFormatAttribute> OperationAttributes
        {
            get { return _operationAttributes; }
        }


        void IOperationBehavior.Validate(OperationDescription description)
        {
        }

        void IOperationBehavior.AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
        {
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription description, DispatchOperation dispatch) { }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription description, ClientOperation proxy) { }

        private static object s_contractMarker = new object();
        // Assumption: gets called exactly once per operation
        void IOperationContractGenerationExtension.GenerateOperation(OperationContractGenerationContext context)
        {
            if (context == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("context");
            if (_partInfoTable != null && _partInfoTable.Count > 0)
            {
                Dictionary<XmlMembersMapping, XmlMembersMapping> alreadyExported = new Dictionary<XmlMembersMapping, XmlMembersMapping>();
                foreach (MessageDescription message in context.Operation.Messages)
                {
                    foreach (MessageHeaderDescription header in message.Headers)
                        GeneratePartType(alreadyExported, header, header.Namespace);


                    MessageBodyDescription body = message.Body;
                    bool isWrapped = (body.WrapperName != null);
                    if (OperationFormatter.IsValidReturnValue(body.ReturnValue))
                        GeneratePartType(alreadyExported, body.ReturnValue, isWrapped ? body.WrapperNamespace : body.ReturnValue.Namespace);

                    foreach (MessagePartDescription part in body.Parts)
                        GeneratePartType(alreadyExported, part, isWrapped ? body.WrapperNamespace : part.Namespace);
                }
            }
            XmlSerializerOperationBehavior xmlSerializerOperationBehavior = context.Operation.Behaviors.Find<XmlSerializerOperationBehavior>() as XmlSerializerOperationBehavior;
            if (xmlSerializerOperationBehavior == null)
                return;

            XmlSerializerFormatAttribute xmlSerializerFormatAttribute = (xmlSerializerOperationBehavior == null) ? new XmlSerializerFormatAttribute() : xmlSerializerOperationBehavior.XmlSerializerFormatAttribute;
            OperationFormatStyle style = xmlSerializerFormatAttribute.Style;
            _operationGenerator.GenerateOperation(context, ref style, xmlSerializerFormatAttribute.IsEncoded, new WrappedBodyTypeGenerator(context), new Dictionary<MessagePartDescription, ICollection<CodeTypeReference>>());
            context.ServiceContractGenerator.AddReferencedAssembly(typeof(Microsoft.Xml.Serialization.XmlTypeAttribute).GetTypeInfo().Assembly);
            xmlSerializerFormatAttribute.Style = style;
            context.SyncMethod.CustomAttributes.Add(OperationGenerator.GenerateAttributeDeclaration(context.Contract.ServiceContractGenerator, xmlSerializerFormatAttribute));
            AddKnownTypes(context.SyncMethod.CustomAttributes, xmlSerializerFormatAttribute.IsEncoded ? SoapExporter.IncludeMetadata : XmlExporter.IncludeMetadata);
            DataContractSerializerOperationGenerator.UpdateTargetCompileUnit(context, _options.CodeCompileUnit);
        }

        private void AddKnownTypes(CodeAttributeDeclarationCollection destination, CodeAttributeDeclarationCollection source)
        {
            foreach (CodeAttributeDeclaration attribute in source)
            {
                CodeAttributeDeclaration knownType = ToKnownType(attribute);
                if (knownType != null)
                {
                    destination.Add(knownType);
                }
            }
        }

        // Convert [XmlInclude] or [SoapInclude] attribute to [KnownType] attribute
        private CodeAttributeDeclaration ToKnownType(CodeAttributeDeclaration include)
        {
            if (include.Name == typeof(SoapIncludeAttribute).FullName || include.Name == typeof(XmlIncludeAttribute).FullName)
            {
                CodeAttributeDeclaration knownType = new CodeAttributeDeclaration(new CodeTypeReference(typeof(ServiceKnownTypeAttribute)));
                foreach (CodeAttributeArgument argument in include.Arguments)
                {
                    knownType.Arguments.Add(argument);
                }
                return knownType;
            }
            return null;
        }

        private void GeneratePartType(Dictionary<XmlMembersMapping, XmlMembersMapping> alreadyExported, MessagePartDescription part, string partNamespace)
        {
            if (!_partInfoTable.ContainsKey(part))
                return;
            PartInfo partInfo = _partInfoTable[part];
            XmlMembersMapping membersMapping = partInfo.MembersMapping;
            XmlMemberMapping memberMapping = partInfo.MemberMapping;
            if (!alreadyExported.ContainsKey(membersMapping))
            {
                if (partInfo.IsEncoded)
                    SoapExporter.ExportMembersMapping(membersMapping);
                else
                    XmlExporter.ExportMembersMapping(membersMapping);
                alreadyExported.Add(membersMapping, membersMapping);
            }
            CodeAttributeDeclarationCollection additionalAttributes = new CodeAttributeDeclarationCollection();
            if (partInfo.IsEncoded)
                SoapExporter.AddMappingMetadata(additionalAttributes, memberMapping, false/*forceUseMemberName*/);
            else
                XmlExporter.AddMappingMetadata(additionalAttributes, memberMapping, partNamespace, false/*forceUseMemberName*/);
            part.BaseType = GetTypeName(memberMapping);
            _operationGenerator.ParameterTypes.Add(part, new CodeTypeReference(part.BaseType));
            _operationGenerator.ParameterAttributes.Add(part, additionalAttributes);
        }

        internal string GetTypeName(XmlMemberMapping member)
        {
            string typeName = member.GenerateTypeName(_options.CodeProvider);
            // If it is an array type, get the array element type name instead
            string comparableTypeName = typeName.Replace("[]", null);
            if (_codeNamespace != null && !string.IsNullOrEmpty(_codeNamespace.Name))
            {
                foreach (CodeTypeDeclaration typeDecl in _codeNamespace.Types)
                {
                    if (typeDecl.Name == comparableTypeName)
                    {
                        typeName = _codeNamespace.Name + "." + typeName;
                    }
                }
            }
            return typeName;
        }

        private class PartInfo
        {
            internal XmlMemberMapping MemberMapping;
            internal XmlMembersMapping MembersMapping;
            internal bool IsEncoded;
        }

        internal class WrappedBodyTypeGenerator : IWrappedBodyTypeGenerator
        {
            private OperationContractGenerationContext _context;
            public WrappedBodyTypeGenerator(OperationContractGenerationContext context)
            {
                _context = context;
            }
            public void ValidateForParameterMode(OperationDescription operation)
            {
            }

            public void AddMemberAttributes(XmlName messageName, MessagePartDescription part, CodeAttributeDeclarationCollection importedAttributes, CodeAttributeDeclarationCollection typeAttributes, CodeAttributeDeclarationCollection fieldAttributes)
            {
                if (importedAttributes != null)
                    fieldAttributes.AddRange(importedAttributes);
            }
            public void AddTypeAttributes(string messageName, string typeNS, CodeAttributeDeclarationCollection typeAttributes, bool isEncoded)
            {
                // we do not need top-level attibutes for the encoded SOAP
                if (isEncoded)
                    return;
                XmlTypeAttribute xmlType = new XmlTypeAttribute();
                xmlType.Namespace = typeNS;
                typeAttributes.Add(OperationGenerator.GenerateAttributeDeclaration(_context.Contract.ServiceContractGenerator, xmlType));
            }
        }
    }
}
