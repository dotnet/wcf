// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using Microsoft.CodeDom;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime;
    using System.Runtime.Serialization;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    internal class DataContractSerializerOperationGenerator : IOperationBehavior, IOperationContractGenerationExtension
    {
        private Dictionary<OperationDescription, DataContractFormatAttribute> _operationAttributes = new Dictionary<OperationDescription, DataContractFormatAttribute>();
        private OperationGenerator _operationGenerator;
        private Dictionary<MessagePartDescription, ICollection<CodeTypeReference>> _knownTypes;
        private Dictionary<MessagePartDescription, bool> _isNonNillableReferenceTypes;
        private CodeCompileUnit _codeCompileUnit;

        public DataContractSerializerOperationGenerator() : this(new CodeCompileUnit()) { }
        public DataContractSerializerOperationGenerator(CodeCompileUnit codeCompileUnit)
        {
            _codeCompileUnit = codeCompileUnit;
            _operationGenerator = new OperationGenerator();
        }

        internal void Add(MessagePartDescription part, CodeTypeReference typeReference, ICollection<CodeTypeReference> knownTypeReferences, bool isNonNillableReferenceType)
        {
            OperationGenerator.ParameterTypes.Add(part, typeReference);
            if (knownTypeReferences != null)
                KnownTypes.Add(part, knownTypeReferences);
            if (isNonNillableReferenceType)
            {
                if (_isNonNillableReferenceTypes == null)
                    _isNonNillableReferenceTypes = new Dictionary<MessagePartDescription, bool>();
                _isNonNillableReferenceTypes.Add(part, isNonNillableReferenceType);
            }
        }

        internal OperationGenerator OperationGenerator
        {
            get { return _operationGenerator; }
        }

        internal Dictionary<OperationDescription, DataContractFormatAttribute> OperationAttributes
        {
            get { return _operationAttributes; }
        }

        internal Dictionary<MessagePartDescription, ICollection<CodeTypeReference>> KnownTypes
        {
            get
            {
                if (_knownTypes == null)
                    _knownTypes = new Dictionary<MessagePartDescription, ICollection<CodeTypeReference>>();
                return _knownTypes;
            }
        }

        void IOperationBehavior.Validate(OperationDescription description)
        {
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription description, DispatchOperation dispatch) { }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription description, ClientOperation proxy) { }

        void IOperationBehavior.AddBindingParameters(OperationDescription description, BindingParameterCollection parameters) { }

        // Assumption: gets called exactly once per operation
        void IOperationContractGenerationExtension.GenerateOperation(OperationContractGenerationContext context)
        {
            DataContractSerializerOperationBehavior DataContractSerializerOperationBehavior = context.Operation.Behaviors.Find<DataContractSerializerOperationBehavior>() as DataContractSerializerOperationBehavior;
            DataContractFormatAttribute dataContractFormatAttribute = (DataContractSerializerOperationBehavior == null) ? new DataContractFormatAttribute() : DataContractSerializerOperationBehavior.DataContractFormatAttribute;
            OperationFormatStyle style = dataContractFormatAttribute.Style;
            _operationGenerator.GenerateOperation(context, ref style, false/*isEncoded*/, new WrappedBodyTypeGenerator(this, context), _knownTypes);
            dataContractFormatAttribute.Style = style;
            if (dataContractFormatAttribute.Style != TypeLoader.DefaultDataContractFormatAttribute.Style)
                context.SyncMethod.CustomAttributes.Add(OperationGenerator.GenerateAttributeDeclaration(context.Contract.ServiceContractGenerator, dataContractFormatAttribute));
            if (_knownTypes != null)
            {
                Dictionary<CodeTypeReference, object> operationKnownTypes = new Dictionary<CodeTypeReference, object>(new CodeTypeReferenceComparer());
                foreach (MessageDescription message in context.Operation.Messages)
                {
                    foreach (MessagePartDescription part in message.Body.Parts)
                        AddKnownTypesForPart(context, part, operationKnownTypes);
                    foreach (MessageHeaderDescription header in message.Headers)
                        AddKnownTypesForPart(context, header, operationKnownTypes);
                    if (OperationFormatter.IsValidReturnValue(message.Body.ReturnValue))
                        AddKnownTypesForPart(context, message.Body.ReturnValue, operationKnownTypes);
                }
            }
            UpdateTargetCompileUnit(context, _codeCompileUnit);
        }

        private void AddKnownTypesForPart(OperationContractGenerationContext context, MessagePartDescription part, Dictionary<CodeTypeReference, object> operationKnownTypes)
        {
            ICollection<CodeTypeReference> knownTypesForPart;
            if (_knownTypes.TryGetValue(part, out knownTypesForPart))
            {
                foreach (CodeTypeReference knownTypeReference in knownTypesForPart)
                {
                    object value;
                    if (!operationKnownTypes.TryGetValue(knownTypeReference, out value))
                    {
                        operationKnownTypes.Add(knownTypeReference, null);
                        CodeAttributeDeclaration knownTypeAttribute = new CodeAttributeDeclaration(typeof(ServiceKnownTypeAttribute).FullName);
                        knownTypeAttribute.Arguments.Add(new CodeAttributeArgument(new CodeTypeOfExpression(knownTypeReference)));
                        context.SyncMethod.CustomAttributes.Add(knownTypeAttribute);
                    }
                }
            }
        }

        internal static void UpdateTargetCompileUnit(OperationContractGenerationContext context, CodeCompileUnit codeCompileUnit)
        {
            CodeCompileUnit targetCompileUnit = context.ServiceContractGenerator.TargetCompileUnit;
            if (!Object.ReferenceEquals(targetCompileUnit, codeCompileUnit))
            {
                foreach (CodeNamespace codeNamespace in codeCompileUnit.Namespaces)
                    if (!targetCompileUnit.Namespaces.Contains(codeNamespace))
                        targetCompileUnit.Namespaces.Add(codeNamespace);
                foreach (string referencedAssembly in codeCompileUnit.ReferencedAssemblies)
                    if (!targetCompileUnit.ReferencedAssemblies.Contains(referencedAssembly))
                        targetCompileUnit.ReferencedAssemblies.Add(referencedAssembly);
                foreach (CodeAttributeDeclaration assemblyCustomAttribute in codeCompileUnit.AssemblyCustomAttributes)
                    if (!targetCompileUnit.AssemblyCustomAttributes.Contains(assemblyCustomAttribute))
                        targetCompileUnit.AssemblyCustomAttributes.Add(assemblyCustomAttribute);
                foreach (CodeDirective startDirective in codeCompileUnit.StartDirectives)
                    if (!targetCompileUnit.StartDirectives.Contains(startDirective))
                        targetCompileUnit.StartDirectives.Add(startDirective);
                foreach (CodeDirective endDirective in codeCompileUnit.EndDirectives)
                    if (!targetCompileUnit.EndDirectives.Contains(endDirective))
                        targetCompileUnit.EndDirectives.Add(endDirective);
                foreach (DictionaryEntry userData in codeCompileUnit.UserData)
                    targetCompileUnit.UserData[userData.Key] = userData.Value;
            }
        }

        internal class WrappedBodyTypeGenerator : IWrappedBodyTypeGenerator
        {
            private static CodeTypeReference s_dataContractAttributeTypeRef = new CodeTypeReference(typeof(DataContractAttribute));
            private int _memberCount;
            private OperationContractGenerationContext _context;
            private DataContractSerializerOperationGenerator _dataContractSerializerOperationGenerator;
            public void ValidateForParameterMode(OperationDescription operation)
            {
                if (_dataContractSerializerOperationGenerator._isNonNillableReferenceTypes == null)
                    return;
                foreach (MessageDescription messageDescription in operation.Messages)
                {
                    if (messageDescription.Body != null)
                    {
                        if (messageDescription.Body.ReturnValue != null)
                            ValidateForParameterMode(messageDescription.Body.ReturnValue);
                        foreach (MessagePartDescription bodyPart in messageDescription.Body.Parts)
                        {
                            ValidateForParameterMode(bodyPart);
                        }
                    }
                }
            }

            private void ValidateForParameterMode(MessagePartDescription part)
            {
                if (_dataContractSerializerOperationGenerator._isNonNillableReferenceTypes.ContainsKey(part))
                {
                    ParameterModeException parameterModeException = new ParameterModeException(string.Format(SRServiceModel.SFxCannotImportAsParameters_ElementIsNotNillable, part.Name, part.Namespace));
                    parameterModeException.MessageContractType = MessageContractType.BareMessageContract;
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(parameterModeException);
                }
            }

            public WrappedBodyTypeGenerator(DataContractSerializerOperationGenerator dataContractSerializerOperationGenerator, OperationContractGenerationContext context)
            {
                _context = context;
                _dataContractSerializerOperationGenerator = dataContractSerializerOperationGenerator;
            }

            public void AddMemberAttributes(XmlName messageName, MessagePartDescription part, CodeAttributeDeclarationCollection attributesImported, CodeAttributeDeclarationCollection typeAttributes, CodeAttributeDeclarationCollection fieldAttributes)
            {
                CodeAttributeDeclaration dataContractAttributeDecl = null;
                foreach (CodeAttributeDeclaration attr in typeAttributes)
                {
                    if (attr.AttributeType.BaseType == s_dataContractAttributeTypeRef.BaseType)
                    {
                        dataContractAttributeDecl = attr;
                        break;
                    }
                }

                if (dataContractAttributeDecl == null)
                {
                    Fx.Assert(String.Format(CultureInfo.InvariantCulture, "Cannot find DataContract attribute for  {0}", messageName));

                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(String.Format(CultureInfo.InvariantCulture, "Cannot find DataContract attribute for  {0}", messageName)));
                }
                bool nsAttrFound = false;
                foreach (CodeAttributeArgument attrArg in dataContractAttributeDecl.Arguments)
                {
                    if (attrArg.Name == "Namespace")
                    {
                        nsAttrFound = true;
                        string nsValue = ((CodePrimitiveExpression)attrArg.Value).Value.ToString();
                        if (nsValue != part.Namespace)
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxWrapperTypeHasMultipleNamespaces, messageName)));
                    }
                }
                if (!nsAttrFound)
                    dataContractAttributeDecl.Arguments.Add(new CodeAttributeArgument("Namespace", new CodePrimitiveExpression(part.Namespace)));

                DataMemberAttribute dataMemberAttribute = new DataMemberAttribute();
                dataMemberAttribute.Order = _memberCount++;
                dataMemberAttribute.EmitDefaultValue = !IsNonNillableReferenceType(part);
                fieldAttributes.Add(OperationGenerator.GenerateAttributeDeclaration(_context.Contract.ServiceContractGenerator, dataMemberAttribute));
            }

            private bool IsNonNillableReferenceType(MessagePartDescription part)
            {
                if (_dataContractSerializerOperationGenerator._isNonNillableReferenceTypes == null)
                    return false;
                return _dataContractSerializerOperationGenerator._isNonNillableReferenceTypes.ContainsKey(part);
            }

            public void AddTypeAttributes(string messageName, string typeNS, CodeAttributeDeclarationCollection typeAttributes, bool isEncoded)
            {
                typeAttributes.Add(OperationGenerator.GenerateAttributeDeclaration(_context.Contract.ServiceContractGenerator, new DataContractAttribute()));
                _memberCount = 0;
            }
        }

        private class CodeTypeReferenceComparer : IEqualityComparer<CodeTypeReference>
        {
            public bool Equals(CodeTypeReference x, CodeTypeReference y)
            {
                if (Object.ReferenceEquals(x, y))
                    return true;

                if (x == null || y == null || x.ArrayRank != y.ArrayRank || x.BaseType != y.BaseType)
                    return false;

                CodeTypeReferenceCollection xTypeArgs = x.TypeArguments;
                CodeTypeReferenceCollection yTypeArgs = y.TypeArguments;
                if (yTypeArgs.Count == xTypeArgs.Count)
                {
                    foreach (CodeTypeReference xTypeArg in xTypeArgs)
                    {
                        foreach (CodeTypeReference yTypeArg in yTypeArgs)
                        {
                            if (!this.Equals(xTypeArg, xTypeArg))
                                return false;
                        }
                    }
                }
                return true;
            }

            public int GetHashCode(CodeTypeReference obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
