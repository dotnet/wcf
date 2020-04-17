// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using Microsoft.CodeDom;
    using System.Collections.Generic;
    using System.Net.Security;
    using System.Reflection;
    using System.Runtime;
    using System.Runtime.Serialization;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.ComponentModel;
    using System.Threading.Tasks;

    internal enum MessageContractType { None, WrappedMessageContract, BareMessageContract }
    internal interface IWrappedBodyTypeGenerator
    {
        void ValidateForParameterMode(OperationDescription operationDescription);
        void AddMemberAttributes(XmlName messageName, MessagePartDescription part, CodeAttributeDeclarationCollection attributesImported, CodeAttributeDeclarationCollection typeAttributes, CodeAttributeDeclarationCollection fieldAttributes);
        void AddTypeAttributes(string messageName, string typeNS, CodeAttributeDeclarationCollection typeAttributes, bool isEncoded);
    }

    internal class OperationGenerator //: IOperationBehavior, IOperationContractGenerationExtension
    {
        private Dictionary<MessagePartDescription, CodeTypeReference> _parameterTypes;
        private Dictionary<MessagePartDescription, CodeAttributeDeclarationCollection> _parameterAttributes;
        private Dictionary<MessagePartDescription, string> _specialPartName;

        internal OperationGenerator()
        {
        }

        internal Dictionary<MessagePartDescription, CodeAttributeDeclarationCollection> ParameterAttributes
        {
            get
            {
                if (_parameterAttributes == null)
                    _parameterAttributes = new Dictionary<MessagePartDescription, CodeAttributeDeclarationCollection>();
                return _parameterAttributes;
            }
        }

        internal Dictionary<MessagePartDescription, CodeTypeReference> ParameterTypes
        {
            get
            {
                if (_parameterTypes == null)
                    _parameterTypes = new Dictionary<MessagePartDescription, CodeTypeReference>();
                return _parameterTypes;
            }
        }

        internal Dictionary<MessagePartDescription, string> SpecialPartName
        {
            get
            {
                if (_specialPartName == null)
                    _specialPartName = new Dictionary<MessagePartDescription, string>();
                return _specialPartName;
            }
        }

        internal void GenerateOperation(OperationContractGenerationContext context, ref OperationFormatStyle style, bool isEncoded, IWrappedBodyTypeGenerator wrappedBodyTypeGenerator, Dictionary<MessagePartDescription, ICollection<CodeTypeReference>> knownTypes)
        {
            if (context == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("context"));
            if (context.Operation == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.OperationPropertyIsRequiredForAttributeGeneration));

            MethodSignatureGenerator methodSignatureGenerator = new MethodSignatureGenerator(this, context, style, isEncoded, wrappedBodyTypeGenerator, knownTypes);
            methodSignatureGenerator.GenerateSyncSignature(ref style);

            if (context.IsTask)
            {
                methodSignatureGenerator.GenerateTaskSignature(ref style);
            }

            if (context.IsAsync)
            {
                methodSignatureGenerator.GenerateAsyncSignature(ref style);
            }
        }

        internal static CodeAttributeDeclaration GenerateAttributeDeclaration(ServiceContractGenerator generator, Attribute attribute)
        {
            return CustomAttributeHelper.GenerateAttributeDeclaration(generator, attribute);
        }

        private class MethodSignatureGenerator
        {
            private readonly OperationGenerator _parent;
            private readonly OperationContractGenerationContext _context;
            private readonly OperationFormatStyle _style;
            private readonly bool _isEncoded;
            private readonly IWrappedBodyTypeGenerator _wrappedBodyTypeGenerator;
            private readonly Dictionary<MessagePartDescription, ICollection<CodeTypeReference>> _knownTypes;

            private CodeMemberMethod _method;
            private CodeMemberMethod _endMethod;

            private readonly string _contractName;
            private string _defaultName;
            private readonly string _contractNS;
            private readonly string _defaultNS;

            private readonly bool _oneway;

            private readonly MessageDescription _request;
            private readonly MessageDescription _response;

            private bool _isNewRequest;
            private bool _isNewResponse;
            private bool _isTaskWithOutputParameters;
            private MessageContractType _messageContractType;

            private IPartCodeGenerator _beginPartCodeGenerator;
            private IPartCodeGenerator _endPartCodeGenerator;

            internal MethodSignatureGenerator(OperationGenerator parent, OperationContractGenerationContext context, OperationFormatStyle style, bool isEncoded, IWrappedBodyTypeGenerator wrappedBodyTypeGenerator, Dictionary<MessagePartDescription, ICollection<CodeTypeReference>> knownTypes)
            {
                _parent = parent;
                _context = context;
                _style = style;
                _isEncoded = isEncoded;
                _wrappedBodyTypeGenerator = wrappedBodyTypeGenerator;
                _knownTypes = knownTypes;
                _messageContractType = context.ServiceContractGenerator.OptionsInternal.IsSet(ServiceContractGenerationOptions.TypedMessages) ? MessageContractType.WrappedMessageContract : MessageContractType.None;

                _contractName = context.Contract.Contract.CodeName;
                _contractNS = context.Operation.DeclaringContract.Namespace;
                _defaultNS = (style == OperationFormatStyle.Rpc) ? string.Empty : _contractNS;
                _oneway = (context.Operation.IsOneWay);
                _request = context.Operation.Messages[0];
                _response = _oneway ? null : context.Operation.Messages[1];

                _isNewRequest = true;
                _isNewResponse = true;
                _beginPartCodeGenerator = null;
                _endPartCodeGenerator = null;
                _isTaskWithOutputParameters = context.IsTask && context.Operation.HasOutputParameters;

                Fx.Assert(_oneway == (_response == null), "OperationContractGenerationContext.Operation cannot contain a null response message when the operation is not one-way");
            }

            internal void GenerateSyncSignature(ref OperationFormatStyle style)
            {
                _method = _context.SyncMethod;
                _endMethod = _context.SyncMethod;
                _defaultName = _method.Name;
                GenerateOperationSignatures(ref style);
            }

            internal void GenerateAsyncSignature(ref OperationFormatStyle style)
            {
                _method = _context.BeginMethod;
                _endMethod = _context.EndMethod;
                _defaultName = _method.Name.Substring(5);
                GenerateOperationSignatures(ref style);
            }

            private void GenerateOperationSignatures(ref OperationFormatStyle style)
            {
                if (_messageContractType != MessageContractType.None || this.GenerateTypedMessageForTaskWithOutputParameters())
                {
                    CheckAndSetMessageContractTypeToBare();
                    this.GenerateTypedMessageOperation(false /*hideFromEditor*/, ref style);
                }
                else if (!this.TryGenerateParameterizedOperation())
                {
                    this.GenerateTypedMessageOperation(true /*hideFromEditor*/, ref style);
                }
            }

            private bool GenerateTypedMessageForTaskWithOutputParameters()
            {
                if (_isTaskWithOutputParameters)
                {
                    if (_method == _context.TaskMethod)
                    {
                        _method.Comments.Add(new CodeCommentStatement(string.Format(SRServiceModel.SFxCodeGenWarning, SRServiceModel.SFxCannotImportAsParameters_OutputParameterAndTask)));
                    }

                    return true;
                }

                return false;
            }

            private void CheckAndSetMessageContractTypeToBare()
            {
                if (_messageContractType == MessageContractType.BareMessageContract)
                    return;
                try
                {
                    _wrappedBodyTypeGenerator.ValidateForParameterMode(_context.Operation);
                }
                catch (ParameterModeException ex)
                {
                    _messageContractType = ex.MessageContractType;
                }
            }

            private bool TryGenerateParameterizedOperation()
            {
                CodeParameterDeclarationExpressionCollection methodParameters, endMethodParameters = null;
                methodParameters = new CodeParameterDeclarationExpressionCollection(_method.Parameters);
                if (_endMethod != null)
                    endMethodParameters = new CodeParameterDeclarationExpressionCollection(_endMethod.Parameters);

                try
                {
                    GenerateParameterizedOperation();
                }
                catch (ParameterModeException ex)
                {
                    _messageContractType = ex.MessageContractType;
                    CodeMemberMethod method = _method;
                    method.Comments.Add(new CodeCommentStatement(string.Format(SRServiceModel.SFxCodeGenWarning, ex.Message)));
                    method.Parameters.Clear();
                    method.Parameters.AddRange(methodParameters);
                    if (_context.IsAsync)
                    {
                        CodeMemberMethod endMethod = _endMethod;
                        endMethod.Parameters.Clear();
                        endMethod.Parameters.AddRange(endMethodParameters);
                    }
                    return false;
                }
                return true;
            }

            private void GenerateParameterizedOperation()
            {
                ParameterizedMessageHelper.ValidateProtectionLevel(this);
                CreateOrOverrideActionProperties();

                if (this.HasUntypedMessages)
                {
                    if (!this.IsCompletelyUntyped)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ParameterModeException(string.Format(SRServiceModel.SFxCannotImportAsParameters_Message, _context.Operation.CodeName)));

                    CreateUntypedMessages();
                }
                else
                {
                    ParameterizedMessageHelper.ValidateWrapperSettings(this);
                    ParameterizedMessageHelper.ValidateNoHeaders(this);
                    _wrappedBodyTypeGenerator.ValidateForParameterMode(_context.Operation);

                    ParameterizedMethodGenerator generator = new ParameterizedMethodGenerator(_method, _endMethod);
                    _beginPartCodeGenerator = generator.InputGenerator;
                    _endPartCodeGenerator = generator.OutputGenerator;

                    if (!_oneway && _response.Body.ReturnValue != null)
                    {
                        _endMethod.ReturnType = GetParameterType(_response.Body.ReturnValue);
                        ParameterizedMessageHelper.GenerateMessageParameterAttribute(_response.Body.ReturnValue, _endMethod.ReturnTypeCustomAttributes, TypeLoader.GetReturnValueName(_defaultName), _defaultNS);
                        AddAdditionalAttributes(_response.Body.ReturnValue, _endMethod.ReturnTypeCustomAttributes, _isEncoded);
                    }

                    GenerateMessageBodyParts(false /*generateTypedMessages*/);
                }
            }

            private void GenerateTypedMessageOperation(bool hideFromEditor, ref OperationFormatStyle style)
            {
                CreateOrOverrideActionProperties();

                if (this.HasUntypedMessages)
                {
                    CreateUntypedMessages();
                    if (this.IsCompletelyUntyped)
                        return;
                }

                CodeNamespace ns = _context.ServiceContractGenerator.NamespaceManager.EnsureNamespace(_contractNS);

                if (!_request.IsUntypedMessage)
                {
                    CodeTypeReference typedReqMessageRef = GenerateTypedMessageHeaderAndReturnValueParts(ns, _defaultName + "Request", _request, false /*isReply*/, hideFromEditor, ref _isNewRequest, out _beginPartCodeGenerator);
                    _method.Parameters.Insert(0, new CodeParameterDeclarationExpression(typedReqMessageRef, "request"));
                }

                if (!_oneway && !_response.IsUntypedMessage)
                {
                    CodeTypeReference typedRespMessageRef = GenerateTypedMessageHeaderAndReturnValueParts(ns, _defaultName + "Response", _response, true /*isReply*/, hideFromEditor, ref _isNewResponse, out _endPartCodeGenerator);
                    _endMethod.ReturnType = typedRespMessageRef;
                }

                GenerateMessageBodyParts(true /*generateTypedMessages*/);

                if (!_isEncoded)
                    style = OperationFormatStyle.Document;
            }

            private CodeTypeReference GenerateTypedMessageHeaderAndReturnValueParts(CodeNamespace ns, string defaultName, MessageDescription message, bool isReply, bool hideFromEditor, ref bool isNewMessage, out IPartCodeGenerator partCodeGenerator)
            {
                CodeTypeReference typedMessageRef;
                if (TypedMessageHelper.FindGeneratedTypedMessage(_context.Contract, message, out typedMessageRef))
                {
                    partCodeGenerator = null;
                    isNewMessage = false;
                }
                else
                {
                    UniqueCodeNamespaceScope namespaceScope = new UniqueCodeNamespaceScope(ns);

                    CodeTypeDeclaration typedMessageDecl = _context.Contract.TypeFactory.CreateClassType();
                    string messageName = XmlName.IsNullOrEmpty(message.MessageName) ? null : message.MessageName.DecodedName;
                    typedMessageRef = namespaceScope.AddUnique(typedMessageDecl, messageName, defaultName);

                    TypedMessageHelper.AddGeneratedTypedMessage(_context.Contract, message, typedMessageRef);

                    if (_messageContractType == MessageContractType.BareMessageContract && message.Body.WrapperName != null)
                        WrapTypedMessage(ns, typedMessageDecl.Name, message, isReply, _context.IsInherited, hideFromEditor);

                    partCodeGenerator = new TypedMessagePartCodeGenerator(typedMessageDecl);

                    if (hideFromEditor)
                    {
                        TypedMessageHelper.AddEditorBrowsableAttribute(typedMessageDecl.CustomAttributes);
                    }
                    TypedMessageHelper.GenerateWrapperAttribute(message, partCodeGenerator);
                    TypedMessageHelper.GenerateProtectionLevelAttribute(message, partCodeGenerator);

                    foreach (MessageHeaderDescription setting in message.Headers)
                        GenerateHeaderPart(setting, partCodeGenerator);

                    if (isReply && message.Body.ReturnValue != null)
                    {
                        GenerateBodyPart(0, message.Body.ReturnValue, partCodeGenerator, true, _isEncoded, _defaultNS);
                    }
                }
                return typedMessageRef;
            }


            private bool IsCompletelyUntyped
            {
                get
                {
                    bool isRequestMessage = _request != null && _request.IsUntypedMessage;
                    bool isResponseMessage = _response != null && _response.IsUntypedMessage;

                    if (isRequestMessage && isResponseMessage)
                        return true;
                    else if (isResponseMessage && _request == null || IsEmpty(_request))
                        return true;
                    else if (isRequestMessage && _response == null || IsEmpty(_response))
                        return true;
                    else
                        return false;
                }
            }

            private bool IsEmpty(MessageDescription message)
            {
                return (message.Body.Parts.Count == 0 && message.Headers.Count == 0);
            }

            private bool HasUntypedMessages
            {
                get
                {
                    bool isRequestMessage = _request != null && _request.IsUntypedMessage;
                    bool isResponseMessage = _response != null && _response.IsUntypedMessage;
                    return (isRequestMessage || isResponseMessage);
                }
            }

            private void CreateUntypedMessages()
            {
                bool isRequestMessage = _request != null && _request.IsUntypedMessage;
                bool isResponseMessage = _response != null && _response.IsUntypedMessage;

                if (isRequestMessage)
                    _method.Parameters.Insert(0, new CodeParameterDeclarationExpression(_context.ServiceContractGenerator.GetCodeTypeReference((typeof(Message))), "request"));
                if (isResponseMessage)
                    _endMethod.ReturnType = _context.ServiceContractGenerator.GetCodeTypeReference(typeof(Message));
            }

            private void CreateOrOverrideActionProperties()
            {
                if (_request != null)
                {
                    CustomAttributeHelper.CreateOrOverridePropertyDeclaration(
                        CustomAttributeHelper.FindOrCreateAttributeDeclaration<OperationContractAttribute>(_method.CustomAttributes), OperationContractAttribute.ActionPropertyName, _request.Action);
                }
                if (_response != null)
                {
                    CustomAttributeHelper.CreateOrOverridePropertyDeclaration(
                        CustomAttributeHelper.FindOrCreateAttributeDeclaration<OperationContractAttribute>(_method.CustomAttributes), OperationContractAttribute.ReplyActionPropertyName, _response.Action);
                }
            }

            private interface IPartCodeGenerator
            {
                CodeAttributeDeclarationCollection AddPart(CodeTypeReference type, ref string name);
                CodeAttributeDeclarationCollection MessageLevelAttributes { get; }
                void EndCodeGeneration();
            }

            private class ParameterizedMethodGenerator
            {
                private ParametersPartCodeGenerator _ins;
                private ParametersPartCodeGenerator _outs;
                private bool _isSync;

                internal ParameterizedMethodGenerator(CodeMemberMethod beginMethod, CodeMemberMethod endMethod)
                {
                    _ins = new ParametersPartCodeGenerator(this, beginMethod.Name, beginMethod.Parameters, beginMethod.CustomAttributes, FieldDirection.In);
                    _outs = new ParametersPartCodeGenerator(this, beginMethod.Name, endMethod.Parameters, beginMethod.CustomAttributes, FieldDirection.Out);
                    _isSync = (beginMethod == endMethod);
                }

                internal CodeParameterDeclarationExpression GetOrCreateParameter(CodeTypeReference type, string name, FieldDirection direction, ref int index, out bool createdNew)
                {
                    Fx.Assert(Microsoft.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(name), String.Format(System.Globalization.CultureInfo.InvariantCulture, "Type name '{0}' is not ValidLanguageIndependentIdentifier!", name));
                    ParametersPartCodeGenerator existingParams = direction != FieldDirection.In ? _ins : _outs;
                    int i = index;
                    CodeParameterDeclarationExpression existing = existingParams.GetParameter(name, ref i);
                    bool isRef = existing != null && existing.Type.BaseType == type.BaseType;
                    if (isRef)
                    {
                        existing.Direction = FieldDirection.Ref;
                        if (_isSync)
                        {
                            index = i + 1;
                            createdNew = false;
                            return existing;
                        }
                    }

                    CodeParameterDeclarationExpression paramDecl = new CodeParameterDeclarationExpression();
                    paramDecl.Name = name;
                    paramDecl.Type = type;
                    paramDecl.Direction = direction;
                    if (isRef)
                        paramDecl.Direction = FieldDirection.Ref;

                    createdNew = true;

                    return paramDecl;
                }

                internal IPartCodeGenerator InputGenerator
                {
                    get
                    {
                        return _ins;
                    }
                }

                internal IPartCodeGenerator OutputGenerator
                {
                    get
                    {
                        return _outs;
                    }
                }

                private class ParametersPartCodeGenerator : IPartCodeGenerator
                {
                    private ParameterizedMethodGenerator _parent;
                    private FieldDirection _direction;
                    private CodeParameterDeclarationExpressionCollection _parameters;
                    private CodeAttributeDeclarationCollection _messageAttrs;
                    private string _methodName;
                    private int _index;

                    internal ParametersPartCodeGenerator(ParameterizedMethodGenerator parent, string methodName, CodeParameterDeclarationExpressionCollection parameters, CodeAttributeDeclarationCollection messageAttrs, FieldDirection direction)
                    {
                        _parent = parent;
                        _methodName = methodName;
                        _parameters = parameters;
                        _messageAttrs = messageAttrs;
                        _direction = direction;
                        _index = 0;
                    }

                    public bool NameExists(string name)
                    {
                        if (String.Compare(name, _methodName, StringComparison.OrdinalIgnoreCase) == 0)
                            return true;
                        int index = 0;
                        return GetParameter(name, ref index) != null;
                    }

                    CodeAttributeDeclarationCollection IPartCodeGenerator.AddPart(CodeTypeReference type, ref string name)
                    {
                        bool createdNew;
                        name = UniqueCodeIdentifierScope.MakeValid(name, "param");
                        CodeParameterDeclarationExpression paramDecl = _parent.GetOrCreateParameter(type, name, _direction, ref _index, out createdNew);
                        if (createdNew)
                        {
                            paramDecl.Name = GetUniqueParameterName(paramDecl.Name, this);
                            _parameters.Insert(_index++, paramDecl);
                        }

                        name = paramDecl.Name;
                        if (!createdNew)
                            return null;
                        return paramDecl.CustomAttributes;
                    }


                    internal CodeParameterDeclarationExpression GetParameter(string name, ref int index)
                    {
                        for (int i = index; i < _parameters.Count; i++)
                        {
                            CodeParameterDeclarationExpression parameter = _parameters[i];
                            if (String.Compare(parameter.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                index = i;
                                return parameter;
                            }
                        }

                        return null;
                    }

                    CodeAttributeDeclarationCollection IPartCodeGenerator.MessageLevelAttributes
                    {
                        get
                        {
                            return _messageAttrs;
                        }
                    }

                    void IPartCodeGenerator.EndCodeGeneration()
                    {
                    }

                    private static string GetUniqueParameterName(string name, ParametersPartCodeGenerator parameters)
                    {
                        return NamingHelper.GetUniqueName(name, DoesParameterNameExist, parameters);
                    }
                    private static bool DoesParameterNameExist(string name, object parametersObject)
                    {
                        return ((ParametersPartCodeGenerator)parametersObject).NameExists(name);
                    }
                }
            }


            private class TypedMessagePartCodeGenerator : IPartCodeGenerator
            {
                private CodeTypeDeclaration _typeDecl;
                private UniqueCodeIdentifierScope _memberScope;

                internal TypedMessagePartCodeGenerator(CodeTypeDeclaration typeDecl)
                {
                    _typeDecl = typeDecl;
                    _memberScope = new UniqueCodeIdentifierScope();
                    _memberScope.AddReserved(typeDecl.Name);
                }

                CodeAttributeDeclarationCollection IPartCodeGenerator.AddPart(CodeTypeReference type, ref string name)
                {
                    CodeMemberField memberDecl = new CodeMemberField();
                    memberDecl.Name = name = _memberScope.AddUnique(name, "member");
                    memberDecl.Type = type;
                    memberDecl.Attributes = MemberAttributes.Public;
                    _typeDecl.Members.Add(memberDecl);
                    return memberDecl.CustomAttributes;
                }

                CodeAttributeDeclarationCollection IPartCodeGenerator.MessageLevelAttributes
                {
                    get
                    {
                        return _typeDecl.CustomAttributes;
                    }
                }

                void IPartCodeGenerator.EndCodeGeneration()
                {
                    TypedMessageHelper.GenerateConstructors(_typeDecl);
                }
            }

            private void WrapTypedMessage(CodeNamespace ns, string typeName, MessageDescription messageDescription, bool isReply, bool isInherited, bool hideFromEditor)
            {
                Fx.Assert(String.IsNullOrEmpty(typeName) || Microsoft.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(typeName), String.Format(System.Globalization.CultureInfo.InvariantCulture, "Type name '{0}' is not ValidLanguageIndependentIdentifier!", typeName));
                UniqueCodeNamespaceScope namespaceScope = new UniqueCodeNamespaceScope(ns);
                CodeTypeDeclaration wrapperTypeDecl = _context.Contract.TypeFactory.CreateClassType();
                CodeTypeReference wrapperTypeRef = namespaceScope.AddUnique(wrapperTypeDecl, typeName + "Body", "Body");

                if (hideFromEditor)
                {
                    TypedMessageHelper.AddEditorBrowsableAttribute(wrapperTypeDecl.CustomAttributes);
                }

                string defaultNS = GetWrapperNamespace(messageDescription);
                string messageName = XmlName.IsNullOrEmpty(messageDescription.MessageName) ? null : messageDescription.MessageName.DecodedName;
                _wrappedBodyTypeGenerator.AddTypeAttributes(messageName, defaultNS, wrapperTypeDecl.CustomAttributes, _isEncoded);

                IPartCodeGenerator partGenerator = new TypedMessagePartCodeGenerator(wrapperTypeDecl);
                System.Net.Security.ProtectionLevel protectionLevel = System.Net.Security.ProtectionLevel.None;
                bool isProtectionLevelSetExplicitly = false;

                if (messageDescription.Body.ReturnValue != null)
                {
                    AddWrapperPart(messageDescription.MessageName, _wrappedBodyTypeGenerator, partGenerator, messageDescription.Body.ReturnValue, wrapperTypeDecl.CustomAttributes);
                    protectionLevel = ProtectionLevelHelper.Max(protectionLevel, messageDescription.Body.ReturnValue.ProtectionLevel);
                    if (messageDescription.Body.ReturnValue.HasProtectionLevel)
                        isProtectionLevelSetExplicitly = true;
                }

                List<CodeTypeReference> wrapperKnownTypes = new List<CodeTypeReference>();
                foreach (MessagePartDescription part in messageDescription.Body.Parts)
                {
                    AddWrapperPart(messageDescription.MessageName, _wrappedBodyTypeGenerator, partGenerator, part, wrapperTypeDecl.CustomAttributes);
                    protectionLevel = ProtectionLevelHelper.Max(protectionLevel, part.ProtectionLevel);
                    if (part.HasProtectionLevel)
                        isProtectionLevelSetExplicitly = true;

                    ICollection<CodeTypeReference> knownTypesForPart = null;
                    if (_knownTypes != null && _knownTypes.TryGetValue(part, out knownTypesForPart))
                    {
                        foreach (CodeTypeReference typeReference in knownTypesForPart)
                            wrapperKnownTypes.Add(typeReference);
                    }
                }
                messageDescription.Body.Parts.Clear();

                MessagePartDescription wrapperPart = new MessagePartDescription(messageDescription.Body.WrapperName, messageDescription.Body.WrapperNamespace);
                if (_knownTypes != null)
                    _knownTypes.Add(wrapperPart, wrapperKnownTypes);
                if (isProtectionLevelSetExplicitly)
                    wrapperPart.ProtectionLevel = protectionLevel;
                messageDescription.Body.WrapperName = null;
                messageDescription.Body.WrapperNamespace = null;
                if (isReply)
                    messageDescription.Body.ReturnValue = wrapperPart;
                else
                    messageDescription.Body.Parts.Add(wrapperPart);
                TypedMessageHelper.GenerateConstructors(wrapperTypeDecl);
                _parent.ParameterTypes.Add(wrapperPart, wrapperTypeRef);
                _parent.SpecialPartName.Add(wrapperPart, "Body");
            }

            private string GetWrapperNamespace(MessageDescription messageDescription)
            {
                string defaultNS = _defaultNS;
                if (messageDescription.Body.ReturnValue != null)
                    defaultNS = messageDescription.Body.ReturnValue.Namespace;
                else if (messageDescription.Body.Parts.Count > 0)
                    defaultNS = messageDescription.Body.Parts[0].Namespace;
                return defaultNS;
            }

            private void GenerateMessageBodyParts(bool generateTypedMessages)
            {
                int order = 0;
                if (_isNewRequest)
                {
                    foreach (MessagePartDescription setting in _request.Body.Parts)
                        GenerateBodyPart(order++, setting, _beginPartCodeGenerator, generateTypedMessages, _isEncoded, _defaultNS);
                }

                if (!_oneway && _isNewResponse)
                {
                    order = _response.Body.ReturnValue != null ? 1 : 0;
                    foreach (MessagePartDescription setting in _response.Body.Parts)
                        GenerateBodyPart(order++, setting, _endPartCodeGenerator, generateTypedMessages, _isEncoded, _defaultNS);
                }
                if (_isNewRequest)
                {
                    if (_beginPartCodeGenerator != null)
                        _beginPartCodeGenerator.EndCodeGeneration();
                }
                if (_isNewResponse)
                {
                    if (_endPartCodeGenerator != null)
                        _endPartCodeGenerator.EndCodeGeneration();
                }
            }

            private void AddWrapperPart(XmlName messageName, IWrappedBodyTypeGenerator wrappedBodyTypeGenerator, IPartCodeGenerator partGenerator, MessagePartDescription part, CodeAttributeDeclarationCollection typeAttributes)
            {
                string fieldName = part.CodeName;
                CodeTypeReference type;
                if (part.Type == typeof(System.IO.Stream))
                    type = _context.ServiceContractGenerator.GetCodeTypeReference(typeof(byte[]));
                else
                    type = GetParameterType(part);
                CodeAttributeDeclarationCollection fieldAttributes = partGenerator.AddPart(type, ref fieldName);

                CodeAttributeDeclarationCollection importedAttributes = null;

                bool hasAttributes = _parent.ParameterAttributes.TryGetValue(part, out importedAttributes);

                wrappedBodyTypeGenerator.AddMemberAttributes(messageName, part, importedAttributes, typeAttributes, fieldAttributes);
                _parent.ParameterTypes.Remove(part);
                if (hasAttributes)
                    _parent.ParameterAttributes.Remove(part);
            }

            private void GenerateBodyPart(int order, MessagePartDescription messagePart, IPartCodeGenerator partCodeGenerator, bool generateTypedMessage, bool isEncoded, string defaultNS)
            {
                if (!generateTypedMessage) order = -1;

                string partName;
                if (!_parent.SpecialPartName.TryGetValue(messagePart, out partName))
                    partName = messagePart.CodeName;

                CodeTypeReference partType = GetParameterType(messagePart);
                CodeAttributeDeclarationCollection partAttributes = partCodeGenerator.AddPart(partType, ref partName);

                if (partAttributes == null)
                    return;

                XmlName xmlPartName = new XmlName(partName);
                if (generateTypedMessage)
                    TypedMessageHelper.GenerateMessageBodyMemberAttribute(order, messagePart, partAttributes, xmlPartName);
                else
                    ParameterizedMessageHelper.GenerateMessageParameterAttribute(messagePart, partAttributes, xmlPartName, defaultNS);

                AddAdditionalAttributes(messagePart, partAttributes, generateTypedMessage || isEncoded);
            }

            private void GenerateHeaderPart(MessageHeaderDescription setting, IPartCodeGenerator parts)
            {
                string partName;
                if (!_parent.SpecialPartName.TryGetValue(setting, out partName))
                    partName = setting.CodeName;
                CodeTypeReference partType = GetParameterType(setting);
                CodeAttributeDeclarationCollection partAttributes = parts.AddPart(partType, ref partName);
                TypedMessageHelper.GenerateMessageHeaderAttribute(setting, partAttributes, new XmlName(partName));
                AddAdditionalAttributes(setting, partAttributes, true /*isAdditionalAttributesAllowed*/);
            }

            private CodeTypeReference GetParameterType(MessagePartDescription setting)
            {
                if (setting.Type != null)
                    return _context.ServiceContractGenerator.GetCodeTypeReference(setting.Type);
                else if (_parent._parameterTypes.ContainsKey(setting))
                    return _parent._parameterTypes[setting];
                else
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SfxNoTypeSpecifiedForParameter, setting.Name)));
            }

            private void AddAdditionalAttributes(MessagePartDescription setting, CodeAttributeDeclarationCollection attributes, bool isAdditionalAttributesAllowed)
            {
                if (_parent._parameterAttributes != null && _parent._parameterAttributes.ContainsKey(setting))
                {
                    CodeAttributeDeclarationCollection localAttributes = _parent._parameterAttributes[setting];
                    if (localAttributes != null && localAttributes.Count > 0)
                    {
                        if (isAdditionalAttributesAllowed)
                            attributes.AddRange(localAttributes);
                        else
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ParameterModeException(string.Format(SRServiceModel.SfxUseTypedMessageForCustomAttributes, setting.Name, localAttributes[0].AttributeType.BaseType)));
                    }
                }
            }


            private static class TypedMessageHelper
            {
                internal static void GenerateProtectionLevelAttribute(MessageDescription message, IPartCodeGenerator partCodeGenerator)
                {
                    CodeAttributeDeclaration messageContractAttr = CustomAttributeHelper.FindOrCreateAttributeDeclaration<MessageContractAttribute>(partCodeGenerator.MessageLevelAttributes);
                    if (message.HasProtectionLevel)
                    {
                        messageContractAttr.Arguments.Add(new CodeAttributeArgument("ProtectionLevel",
                            new CodeFieldReferenceExpression(
                                new CodeTypeReferenceExpression(typeof(ProtectionLevel)), message.ProtectionLevel.ToString())));
                    }
                }

                internal static void GenerateWrapperAttribute(MessageDescription message, IPartCodeGenerator partCodeGenerator)
                {
                    CodeAttributeDeclaration messageContractAttr = CustomAttributeHelper.FindOrCreateAttributeDeclaration<MessageContractAttribute>(partCodeGenerator.MessageLevelAttributes);
                    if (message.Body.WrapperName != null)
                    {
                        // use encoded name to specify exactly what goes on the wire.
                        messageContractAttr.Arguments.Add(new CodeAttributeArgument("WrapperName",
                            new CodePrimitiveExpression(NamingHelper.CodeName(message.Body.WrapperName))));
                        messageContractAttr.Arguments.Add(new CodeAttributeArgument("WrapperNamespace",
                            new CodePrimitiveExpression(message.Body.WrapperNamespace)));
                        messageContractAttr.Arguments.Add(new CodeAttributeArgument("IsWrapped",
                            new CodePrimitiveExpression(true)));
                    }
                    else
                        messageContractAttr.Arguments.Add(new CodeAttributeArgument("IsWrapped",
                            new CodePrimitiveExpression(false)));
                }

                internal static void AddEditorBrowsableAttribute(CodeAttributeDeclarationCollection attributes)
                {
                    attributes.Add(ClientClassGenerator.CreateEditorBrowsableAttribute(EditorBrowsableState.Advanced));
                }

                internal static void AddGeneratedTypedMessage(ServiceContractGenerationContext contract, MessageDescription message, CodeTypeReference codeTypeReference)
                {
                    if (message.XsdTypeName != null && !message.XsdTypeName.IsEmpty)
                    {
                        contract.ServiceContractGenerator.GeneratedTypedMessages.Add(message, codeTypeReference);
                    }
                }

                internal static bool FindGeneratedTypedMessage(ServiceContractGenerationContext contract, MessageDescription message, out CodeTypeReference codeTypeReference)
                {
                    if (message.XsdTypeName == null || message.XsdTypeName.IsEmpty)
                    {
                        codeTypeReference = null;
                        return false;
                    }
                    return contract.ServiceContractGenerator.GeneratedTypedMessages.TryGetValue(message, out codeTypeReference);
                }

                internal static void GenerateConstructors(CodeTypeDeclaration typeDecl)
                {
                    CodeConstructor defaultCtor = new CodeConstructor();
                    defaultCtor.Attributes = MemberAttributes.Public;
                    typeDecl.Members.Add(defaultCtor);
                    CodeConstructor otherCtor = new CodeConstructor();
                    otherCtor.Attributes = MemberAttributes.Public;
                    foreach (CodeTypeMember member in typeDecl.Members)
                    {
                        CodeMemberField field = member as CodeMemberField;
                        if (field == null)
                            continue;
                        CodeParameterDeclarationExpression param = new CodeParameterDeclarationExpression(field.Type, field.Name);
                        otherCtor.Parameters.Add(param);
                        otherCtor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), field.Name), new CodeArgumentReferenceExpression(param.Name)));
                    }
                    if (otherCtor.Parameters.Count > 0)
                        typeDecl.Members.Add(otherCtor);
                }

                internal static void GenerateMessageBodyMemberAttribute(int order, MessagePartDescription setting, CodeAttributeDeclarationCollection attributes, XmlName defaultName)
                {
                    GenerateMessageContractMemberAttribute<MessageBodyMemberAttribute>(order, setting, attributes, defaultName);
                }

                internal static void GenerateMessageHeaderAttribute(MessageHeaderDescription setting, CodeAttributeDeclarationCollection attributes, XmlName defaultName)
                {
                    if (setting.Multiple)
                        GenerateMessageContractMemberAttribute<MessageHeaderArrayAttribute>(-1, setting, attributes, defaultName);
                    else
                        GenerateMessageContractMemberAttribute<MessageHeaderAttribute>(-1, setting, attributes, defaultName);
                }

                private static void GenerateMessageContractMemberAttribute<T>(int order, MessagePartDescription setting, CodeAttributeDeclarationCollection attrs, XmlName defaultName)
                    where T : Attribute
                {
                    CodeAttributeDeclaration decl = CustomAttributeHelper.FindOrCreateAttributeDeclaration<T>(attrs);

                    if (setting.Name != defaultName.EncodedName)
                        // override name with encoded value specified in wsdl; this only works beacuse
                        // our Encoding algorithm will leave alredy encoded names untouched
                        CustomAttributeHelper.CreateOrOverridePropertyDeclaration(decl, MessageContractMemberAttribute.NamePropertyName, setting.Name);

                    CustomAttributeHelper.CreateOrOverridePropertyDeclaration(decl, MessageContractMemberAttribute.NamespacePropertyName, setting.Namespace);

                    if (setting.HasProtectionLevel)
                        CustomAttributeHelper.CreateOrOverridePropertyDeclaration(decl, MessageContractMemberAttribute.ProtectionLevelPropertyName, setting.ProtectionLevel);

                    if (order >= 0)
                        CustomAttributeHelper.CreateOrOverridePropertyDeclaration(decl, MessageBodyMemberAttribute.OrderPropertyName, order);
                }
            }

            private static class ParameterizedMessageHelper
            {
                internal static void GenerateMessageParameterAttribute(MessagePartDescription setting, CodeAttributeDeclarationCollection attributes, XmlName defaultName, string defaultNS)
                {
                    if (setting.Name != defaultName.EncodedName)
                    {
                        // override name with encoded value specified in wsdl; this only works beacuse
                        // our Encoding algorithm will leave alredy encoded names untouched
                        CustomAttributeHelper.CreateOrOverridePropertyDeclaration(
                            CustomAttributeHelper.FindOrCreateAttributeDeclaration<MessageParameterAttribute>(attributes), MessageParameterAttribute.NamePropertyName, setting.Name);
                    }
                    if (setting.Namespace != defaultNS)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ParameterModeException(string.Format(SRServiceModel.SFxCannotImportAsParameters_NamespaceMismatch, setting.Namespace, defaultNS)));
                }

                internal static void ValidateProtectionLevel(MethodSignatureGenerator parent)
                {
                    if (parent._request != null && parent._request.HasProtectionLevel)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ParameterModeException(string.Format(SRServiceModel.SFxCannotImportAsParameters_MessageHasProtectionLevel, parent._request.Action == null ? "" : parent._request.Action)));
                    }
                    if (parent._response != null && parent._response.HasProtectionLevel)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ParameterModeException(string.Format(SRServiceModel.SFxCannotImportAsParameters_MessageHasProtectionLevel, parent._response.Action == null ? "" : parent._response.Action)));
                    }
                }

                internal static void ValidateWrapperSettings(MethodSignatureGenerator parent)
                {
                    if (parent._request.Body.WrapperName == null || (parent._response != null && parent._response.Body.WrapperName == null))
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ParameterModeException(string.Format(SRServiceModel.SFxCannotImportAsParameters_Bare, parent._context.Operation.CodeName)));

                    if (!StringEqualOrNull(parent._request.Body.WrapperNamespace, parent._contractNS))
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ParameterModeException(string.Format(SRServiceModel.SFxCannotImportAsParameters_DifferentWrapperNs, parent._request.MessageName, parent._request.Body.WrapperNamespace, parent._contractNS)));

                    XmlName defaultName = new XmlName(parent._defaultName);
                    if (!String.Equals(parent._request.Body.WrapperName, defaultName.EncodedName, StringComparison.Ordinal))
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ParameterModeException(string.Format(SRServiceModel.SFxCannotImportAsParameters_DifferentWrapperName, parent._request.MessageName, parent._request.Body.WrapperName, defaultName.EncodedName)));

                    if (parent._response != null)
                    {
                        if (!StringEqualOrNull(parent._response.Body.WrapperNamespace, parent._contractNS))
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ParameterModeException(string.Format(SRServiceModel.SFxCannotImportAsParameters_DifferentWrapperNs, parent._response.MessageName, parent._response.Body.WrapperNamespace, parent._contractNS)));

                        if (!String.Equals(parent._response.Body.WrapperName, TypeLoader.GetBodyWrapperResponseName(defaultName).EncodedName, StringComparison.Ordinal))
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ParameterModeException(string.Format(SRServiceModel.SFxCannotImportAsParameters_DifferentWrapperName, parent._response.MessageName, parent._response.Body.WrapperName, defaultName.EncodedName)));
                    }
                }

                internal static void ValidateNoHeaders(MethodSignatureGenerator parent)
                {
                    if (parent._request.Headers.Count > 0)
                    {
                        if (parent._isEncoded)
                        {
                            parent._context.Contract.ServiceContractGenerator.Errors.Add(new MetadataConversionError(string.Format(SRServiceModel.SFxCannotImportAsParameters_HeadersAreIgnoredInEncoded, parent._request.MessageName), true/*isWarning*/));
                        }
                        else
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ParameterModeException(string.Format(SRServiceModel.SFxCannotImportAsParameters_HeadersAreUnsupported, parent._request.MessageName)));
                    }

                    if (!parent._oneway && parent._response.Headers.Count > 0)
                    {
                        if (parent._isEncoded)
                            parent._context.Contract.ServiceContractGenerator.Errors.Add(new MetadataConversionError(string.Format(SRServiceModel.SFxCannotImportAsParameters_HeadersAreIgnoredInEncoded, parent._response.MessageName), true/*isWarning*/));
                        else
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ParameterModeException(string.Format(SRServiceModel.SFxCannotImportAsParameters_HeadersAreUnsupported, parent._response.MessageName)));
                    }
                }

                private static bool StringEqualOrNull(string overrideValue, string defaultValue)
                {
                    return overrideValue == null || String.Equals(overrideValue, defaultValue, StringComparison.Ordinal);
                }
            }

            internal void GenerateTaskSignature(ref OperationFormatStyle style)
            {
                _method = _context.TaskMethod;
                _endMethod = _context.TaskMethod;
                _defaultName = _context.SyncMethod.Name;
                GenerateOperationSignatures(ref style);

                CodeTypeReference resultType = _method.ReturnType;
                CodeTypeReference taskReturnType;
                if (resultType.BaseType == ServiceReflector.VoidType.FullName)
                {
                    taskReturnType = new CodeTypeReference(ServiceReflector.taskType);
                }
                else
                {
                    taskReturnType = new CodeTypeReference(_context.ServiceContractGenerator.GetCodeTypeReference(ServiceReflector.taskTResultType).BaseType, resultType);
                }

                _method.ReturnType = taskReturnType;
            }
        }

        private static class CustomAttributeHelper
        {
            internal static void CreateOrOverridePropertyDeclaration<V>(CodeAttributeDeclaration attribute, string propertyName, V value)
            {
                SecurityAttributeGenerationHelper.CreateOrOverridePropertyDeclaration<V>(attribute, propertyName, value);
            }

            internal static CodeAttributeDeclaration FindOrCreateAttributeDeclaration<T>(CodeAttributeDeclarationCollection attributes)
                where T : Attribute
            {
                return SecurityAttributeGenerationHelper.FindOrCreateAttributeDeclaration<T>(attributes);
            }

            internal static CodeAttributeDeclaration GenerateAttributeDeclaration(ServiceContractGenerator generator, Attribute attribute)
            {
                Type attributeType = attribute.GetType();
                Attribute defaultAttribute = (Attribute)Activator.CreateInstance(attributeType);
                MemberInfo[] publicMembers = attributeType.GetMembers(BindingFlags.Instance | BindingFlags.Public);
                Array.Sort<MemberInfo>(publicMembers,
                                       delegate (MemberInfo a, MemberInfo b)
                                       {
                                           return String.Compare(a.Name, b.Name, StringComparison.Ordinal);
                                       }
                );
                // we should create this reference through ServiceContractGenerator, which tracks referenced assemblies
                CodeAttributeDeclaration attr = new CodeAttributeDeclaration(generator.GetCodeTypeReference(attributeType));
                foreach (MemberInfo member in publicMembers)
                {
                    if (member.DeclaringType == typeof(Attribute))
                        continue;
                    FieldInfo field = member as FieldInfo;
                    if (field != null)
                    {
                        object fieldValue = field.GetValue(attribute);
                        object defaultValue = field.GetValue(defaultAttribute);

                        if (!object.Equals(fieldValue, defaultValue))
                            attr.Arguments.Add(new CodeAttributeArgument(field.Name, GetArgValue(fieldValue)));
                        continue;
                    }
                    PropertyInfo property = member as PropertyInfo;
                    if (property != null)
                    {
                        object propertyValue = property.GetValue(attribute, null);
                        object defaultValue = property.GetValue(defaultAttribute, null);
                        if (!object.Equals(propertyValue, defaultValue))
                            attr.Arguments.Add(new CodeAttributeArgument(property.Name, GetArgValue(propertyValue)));
                        continue;
                    }
                }
                return attr;
            }

            private static CodeExpression GetArgValue(object val)
            {
                Type type = val.GetType();
                TypeInfo info = type.GetTypeInfo();
                if (info.IsPrimitive || type == typeof(string))
                    return new CodePrimitiveExpression(val);
                if (info.IsEnum)
                    return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(type), Enum.Format(type, val, "G"));

                Fx.Assert(String.Format(System.Globalization.CultureInfo.InvariantCulture, "Attribute generation is not supported for argument type {0}", type));
                return null;
            }
        }
    }

    internal class ParameterModeException : Exception
    {
        private MessageContractType _messageContractType = MessageContractType.WrappedMessageContract;
        public ParameterModeException() { }
        public ParameterModeException(string message) : base(message) { }
        public MessageContractType MessageContractType
        {
            get { return _messageContractType; }
            set { _messageContractType = value; }
        }
    }
}
