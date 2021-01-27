// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ServiceModel.Description
{
    using System;
    using Microsoft.CodeDom;
    using Microsoft.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Net.Security;
    using System.Reflection;
    using System.Runtime;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public class ServiceContractGenerator
    {
        private CodeCompileUnit _compileUnit;
        private NamespaceHelper _namespaceManager;

        // options
        private OptionsHelper _options = new OptionsHelper(ServiceContractGenerationOptions.ChannelInterface |
                                                    ServiceContractGenerationOptions.ClientClass);

        private Dictionary<ContractDescription, Type> _referencedTypes;
        private Dictionary<ContractDescription, ServiceContractGenerationContext> _generatedTypes;
        private Dictionary<OperationDescription, OperationContractGenerationContext> _generatedOperations;
        private Dictionary<MessageDescription, CodeTypeReference> _generatedTypedMessages;

        private Collection<MetadataConversionError> _errors = new Collection<MetadataConversionError>();

        public ServiceContractGenerator()
            : this(null)
        {
        }

        public ServiceContractGenerator(CodeCompileUnit targetCompileUnit)
        {
            _compileUnit = targetCompileUnit ?? new CodeCompileUnit();
            _namespaceManager = new NamespaceHelper(_compileUnit.Namespaces);

            AddReferencedAssembly(typeof(ServiceContractGenerator).GetTypeInfo().Assembly);
            _generatedTypes = new Dictionary<ContractDescription, ServiceContractGenerationContext>();
            _generatedOperations = new Dictionary<OperationDescription, OperationContractGenerationContext>();
            _referencedTypes = new Dictionary<ContractDescription, Type>();
        }

        internal CodeTypeReference GetCodeTypeReference(Type type)
        {
            AddReferencedAssembly(type.GetTypeInfo().Assembly);
            return new CodeTypeReference(type);
        }

        internal void AddReferencedAssembly(Assembly assembly)
        {
            string assemblyName = assembly.GetName().Name;
            bool alreadyExisting = false;
            foreach (string existingName in _compileUnit.ReferencedAssemblies)
            {
                if (String.Compare(existingName, assemblyName, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    alreadyExisting = true;
                    break;
                }
            }
            if (!alreadyExisting)
                _compileUnit.ReferencedAssemblies.Add(assemblyName);
        }

        // options
        public ServiceContractGenerationOptions Options
        {
            get { return _options.Options; }
            set { _options = new OptionsHelper(value); }
        }

        internal OptionsHelper OptionsInternal
        {
            get { return _options; }
        }

        public Dictionary<ContractDescription, Type> ReferencedTypes
        {
            get { return _referencedTypes; }
        }

        public CodeCompileUnit TargetCompileUnit
        {
            get { return _compileUnit; }
        }

        public Dictionary<string, string> NamespaceMappings
        {
            get { return this.NamespaceManager.NamespaceMappings; }
        }

        public Collection<MetadataConversionError> Errors
        {
            get { return _errors; }
        }

        internal NamespaceHelper NamespaceManager
        {
            get { return _namespaceManager; }
        }

        public CodeTypeReference GenerateServiceContractType(ContractDescription contractDescription)
        {
            CodeTypeReference retVal = GenerateServiceContractTypeInternal(contractDescription);
            Microsoft.CodeDom.Compiler.CodeGenerator.ValidateIdentifiers(TargetCompileUnit);
            return retVal;
        }

        private CodeTypeReference GenerateServiceContractTypeInternal(ContractDescription contractDescription)
        {
            if (contractDescription == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contractDescription");

            Type existingType;
            if (_referencedTypes.TryGetValue(contractDescription, out existingType))
            {
                return GetCodeTypeReference(existingType);
            }

            ServiceContractGenerationContext context;
            CodeNamespace ns = this.NamespaceManager.EnsureNamespace(contractDescription.Namespace);
            if (!_generatedTypes.TryGetValue(contractDescription, out context))
            {
                context = new ContextInitializer(this, new CodeTypeFactory(this, _options.IsSet(ServiceContractGenerationOptions.InternalTypes))).CreateContext(contractDescription);

                ExtensionsHelper.CallContractExtensions(GetBeforeExtensionsBuiltInContractGenerators(), context);
                ExtensionsHelper.CallOperationExtensions(GetBeforeExtensionsBuiltInOperationGenerators(), context);

                ExtensionsHelper.CallBehaviorExtensions(context);

                ExtensionsHelper.CallContractExtensions(GetAfterExtensionsBuiltInContractGenerators(), context);
                ExtensionsHelper.CallOperationExtensions(GetAfterExtensionsBuiltInOperationGenerators(), context);

                _generatedTypes.Add(contractDescription, context);
            }
            return context.ContractTypeReference;
        }

        private IEnumerable<IServiceContractGenerationExtension> GetBeforeExtensionsBuiltInContractGenerators()
        {
            return EmptyArray<IServiceContractGenerationExtension>.Allocate(0);
        }

        private IEnumerable<IOperationContractGenerationExtension> GetBeforeExtensionsBuiltInOperationGenerators()
        {
            yield return new FaultContractAttributeGenerator();
            yield return new TransactionFlowAttributeGenerator();
        }

        private IEnumerable<IServiceContractGenerationExtension> GetAfterExtensionsBuiltInContractGenerators()
        {
            if (_options.IsSet(ServiceContractGenerationOptions.ChannelInterface))
            {
                yield return new ChannelInterfaceGenerator();
            }

            if (_options.IsSet(ServiceContractGenerationOptions.ClientClass))
            {
                // unless the caller explicitly asks for TM we try to generate a helpful overload if we end up with TM
                bool tryAddHelperMethod = !_options.IsSet(ServiceContractGenerationOptions.TypedMessages);
                bool generateEventAsyncMethods = _options.IsSet(ServiceContractGenerationOptions.EventBasedAsynchronousMethods);
                yield return new ClientClassGenerator(tryAddHelperMethod, generateEventAsyncMethods);
            }
        }

        private IEnumerable<IOperationContractGenerationExtension> GetAfterExtensionsBuiltInOperationGenerators()
        {
            return EmptyArray<IOperationContractGenerationExtension>.Allocate(0);
        }

        internal static CodeExpression GetEnumReference<EnumType>(EnumType value)
        {
            return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(EnumType)), Enum.Format(typeof(EnumType), value, "G"));
        }

        internal Dictionary<MessageDescription, CodeTypeReference> GeneratedTypedMessages
        {
            get
            {
                if (_generatedTypedMessages == null)
                    _generatedTypedMessages = new Dictionary<MessageDescription, CodeTypeReference>(MessageDescriptionComparer.Singleton);
                return _generatedTypedMessages;
            }
        }

        internal class ContextInitializer
        {
            private readonly ServiceContractGenerator _parent;
            private readonly CodeTypeFactory _typeFactory;
            private readonly bool _asyncMethods;
            private readonly bool _taskMethod;

            private ServiceContractGenerationContext _context;
            private UniqueCodeIdentifierScope _contractMemberScope;
            private UniqueCodeIdentifierScope _callbackMemberScope;

            internal ContextInitializer(ServiceContractGenerator parent, CodeTypeFactory typeFactory)
            {
                _parent = parent;
                _typeFactory = typeFactory;

                _asyncMethods = parent.OptionsInternal.IsSet(ServiceContractGenerationOptions.AsynchronousMethods);
                _taskMethod = parent.OptionsInternal.IsSet(ServiceContractGenerationOptions.TaskBasedAsynchronousMethod);
            }

            public ServiceContractGenerationContext CreateContext(ContractDescription contractDescription)
            {
                VisitContract(contractDescription);

                Fx.Assert(_context != null, "context was not initialized");
                return _context;
            }

            // this could usefully be factored into a base class for use by WSDL export and others
            private void VisitContract(ContractDescription contract)
            {
                this.Visit(contract);

                foreach (OperationDescription operation in contract.Operations)
                {
                    this.Visit(operation);

                    // not used in this case
                    //foreach (MessageDescription message in operation.Messages)
                    //{
                    //    this.Visit(message);
                    //}
                }
            }

            private void Visit(ContractDescription contractDescription)
            {
                bool isDuplex = IsDuplex(contractDescription);

                _contractMemberScope = new UniqueCodeIdentifierScope();
                _callbackMemberScope = isDuplex ? new UniqueCodeIdentifierScope() : null;

                UniqueCodeNamespaceScope codeNamespaceScope = new UniqueCodeNamespaceScope(_parent.NamespaceManager.EnsureNamespace(contractDescription.Namespace));

                CodeTypeDeclaration contract = _typeFactory.CreateInterfaceType();
                CodeTypeReference contractReference = codeNamespaceScope.AddUnique(contract, contractDescription.CodeName, Strings.DefaultContractName);

                CodeTypeDeclaration callbackContract = null;
                CodeTypeReference callbackContractReference = null;
                if (isDuplex)
                {
                    callbackContract = _typeFactory.CreateInterfaceType();
                    callbackContractReference = codeNamespaceScope.AddUnique(callbackContract, contractDescription.CodeName + Strings.CallbackTypeSuffix, Strings.DefaultContractName);
                }

                _context = new ServiceContractGenerationContext(_parent, contractDescription, contract, callbackContract);
                _context.Namespace = codeNamespaceScope.CodeNamespace;
                _context.TypeFactory = _typeFactory;
                _context.ContractTypeReference = contractReference;
                _context.DuplexCallbackTypeReference = callbackContractReference;

                AddServiceContractAttribute(_context);
            }

            private void Visit(OperationDescription operationDescription)
            {
                bool isCallback = operationDescription.IsServerInitiated();
                CodeTypeDeclaration declaringType = isCallback ? _context.DuplexCallbackType : _context.ContractType;
                UniqueCodeIdentifierScope memberScope = isCallback ? _callbackMemberScope : _contractMemberScope;

                Fx.Assert(declaringType != null, "missing callback type");

                string syncMethodName = memberScope.AddUnique(operationDescription.CodeName, Strings.DefaultOperationName);

                CodeMemberMethod syncMethod = new CodeMemberMethod();
                syncMethod.Name = syncMethodName;
                declaringType.Members.Add(syncMethod);

                OperationContractGenerationContext operationContext;
                CodeMemberMethod beginMethod = null;
                CodeMemberMethod endMethod = null;
                if (_asyncMethods)
                {
                    beginMethod = new CodeMemberMethod();
                    beginMethod.Name = ServiceReflector.BeginMethodNamePrefix + syncMethodName;
                    beginMethod.Parameters.Add(new CodeParameterDeclarationExpression(_context.ServiceContractGenerator.GetCodeTypeReference(typeof(AsyncCallback)), Strings.AsyncCallbackArgName));
                    beginMethod.Parameters.Add(new CodeParameterDeclarationExpression(_context.ServiceContractGenerator.GetCodeTypeReference(typeof(object)), Strings.AsyncStateArgName));
                    beginMethod.ReturnType = _context.ServiceContractGenerator.GetCodeTypeReference(typeof(IAsyncResult));
                    declaringType.Members.Add(beginMethod);

                    endMethod = new CodeMemberMethod();
                    endMethod.Name = ServiceReflector.EndMethodNamePrefix + syncMethodName;
                    endMethod.Parameters.Add(new CodeParameterDeclarationExpression(_context.ServiceContractGenerator.GetCodeTypeReference(typeof(IAsyncResult)), Strings.AsyncResultArgName));
                    declaringType.Members.Add(endMethod);

                    operationContext = new OperationContractGenerationContext(_parent, _context, operationDescription, declaringType, syncMethod, beginMethod, endMethod);
                }
                else
                {
                    operationContext = new OperationContractGenerationContext(_parent, _context, operationDescription, declaringType, syncMethod);
                }

                if (_taskMethod)
                {
                    if (isCallback)
                    {
                        if (beginMethod == null)
                        {
                            operationContext = new OperationContractGenerationContext(_parent, _context, operationDescription, declaringType, syncMethod);
                        }
                        else
                        {
                            operationContext = new OperationContractGenerationContext(_parent, _context, operationDescription, declaringType, syncMethod, beginMethod, endMethod);
                        }
                    }
                    else
                    {
                        CodeMemberMethod taskBasedAsyncMethod = new CodeMemberMethod { Name = syncMethodName + ServiceReflector.AsyncMethodNameSuffix };
                        declaringType.Members.Add(taskBasedAsyncMethod);
                        if (beginMethod == null)
                        {
                            operationContext = new OperationContractGenerationContext(_parent, _context, operationDescription, declaringType, syncMethod, taskBasedAsyncMethod);
                        }
                        else
                        {
                            operationContext = new OperationContractGenerationContext(_parent, _context, operationDescription, declaringType, syncMethod, beginMethod, endMethod, taskBasedAsyncMethod);
                        }
                    }
                }

                operationContext.DeclaringTypeReference = operationDescription.IsServerInitiated() ? _context.DuplexCallbackTypeReference : _context.ContractTypeReference;

                _context.Operations.Add(operationContext);

                AddOperationContractAttributes(operationContext);
            }

            private void AddServiceContractAttribute(ServiceContractGenerationContext context)
            {
                CodeAttributeDeclaration serviceContractAttr = new CodeAttributeDeclaration(context.ServiceContractGenerator.GetCodeTypeReference(typeof(ServiceContractAttribute)));

                if (context.ContractType.Name != context.Contract.CodeName)
                {
                    // make sure that decoded Contract name can be used, if not, then override name with encoded value
                    // specified in wsdl; this only works beacuse our Encoding algorithm will leave alredy encoded names untouched
                    string friendlyName = NamingHelper.XmlName(context.Contract.CodeName) == context.Contract.Name ? context.Contract.CodeName : context.Contract.Name;
                    serviceContractAttr.Arguments.Add(new CodeAttributeArgument("Name", new CodePrimitiveExpression(friendlyName)));
                }

                if (NamingHelper.DefaultNamespace != context.Contract.Namespace)
                    serviceContractAttr.Arguments.Add(new CodeAttributeArgument("Namespace", new CodePrimitiveExpression(context.Contract.Namespace)));

                serviceContractAttr.Arguments.Add(new CodeAttributeArgument("ConfigurationName", new CodePrimitiveExpression(NamespaceHelper.GetCodeTypeReference(context.Namespace, context.ContractType).BaseType)));

                if (context.Contract.HasProtectionLevel)
                {
                    serviceContractAttr.Arguments.Add(new CodeAttributeArgument("ProtectionLevel",
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(ProtectionLevel)), context.Contract.ProtectionLevel.ToString())));
                }

                if (context.DuplexCallbackType != null)
                {
                    serviceContractAttr.Arguments.Add(new CodeAttributeArgument("CallbackContract", new CodeTypeOfExpression(context.DuplexCallbackTypeReference)));
                }

                if (context.Contract.SessionMode != SessionMode.Allowed)
                {
                    serviceContractAttr.Arguments.Add(new CodeAttributeArgument("SessionMode",
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(SessionMode)), context.Contract.SessionMode.ToString())));
                }

                context.ContractType.CustomAttributes.Add(serviceContractAttr);
            }

            private void AddOperationContractAttributes(OperationContractGenerationContext context)
            {
                if (context.SyncMethod != null)
                {
                    context.SyncMethod.CustomAttributes.Add(CreateOperationContractAttributeDeclaration(context.Operation, false));
                }
                if (context.BeginMethod != null)
                {
                    context.BeginMethod.CustomAttributes.Add(CreateOperationContractAttributeDeclaration(context.Operation, true));
                }
                if (context.TaskMethod != null)
                {
                    context.TaskMethod.CustomAttributes.Add(CreateOperationContractAttributeDeclaration(context.Operation, false));
                }
            }

            private CodeAttributeDeclaration CreateOperationContractAttributeDeclaration(OperationDescription operationDescription, bool asyncPattern)
            {
                CodeAttributeDeclaration serviceOperationAttr = new CodeAttributeDeclaration(_context.ServiceContractGenerator.GetCodeTypeReference(typeof(OperationContractAttribute)));
                if (operationDescription.IsOneWay)
                {
                    serviceOperationAttr.Arguments.Add(new CodeAttributeArgument("IsOneWay", new CodePrimitiveExpression(true)));
                }
                if ((operationDescription.DeclaringContract.SessionMode == SessionMode.Required) && operationDescription.IsTerminating)
                {
                    serviceOperationAttr.Arguments.Add(new CodeAttributeArgument("IsTerminating", new CodePrimitiveExpression(true)));
                }
                if ((operationDescription.DeclaringContract.SessionMode == SessionMode.Required) && !operationDescription.IsInitiating)
                {
                    serviceOperationAttr.Arguments.Add(new CodeAttributeArgument("IsInitiating", new CodePrimitiveExpression(false)));
                }
                if (asyncPattern)
                {
                    serviceOperationAttr.Arguments.Add(new CodeAttributeArgument("AsyncPattern", new CodePrimitiveExpression(true)));
                }
                if (operationDescription.HasProtectionLevel)
                {
                    serviceOperationAttr.Arguments.Add(new CodeAttributeArgument("ProtectionLevel",
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(ProtectionLevel)), operationDescription.ProtectionLevel.ToString())));
                }
                return serviceOperationAttr;
            }

            private static bool IsDuplex(ContractDescription contract)
            {
                foreach (OperationDescription operation in contract.Operations)
                    if (operation.IsServerInitiated())
                        return true;

                return false;
            }
        }

        private class ChannelInterfaceGenerator : IServiceContractGenerationExtension
        {
            void IServiceContractGenerationExtension.GenerateContract(ServiceContractGenerationContext context)
            {
                CodeTypeDeclaration channelType = context.TypeFactory.CreateInterfaceType();
                channelType.BaseTypes.Add(context.ContractTypeReference);
                channelType.BaseTypes.Add(context.ServiceContractGenerator.GetCodeTypeReference(typeof(IClientChannel)));

                new UniqueCodeNamespaceScope(context.Namespace).AddUnique(channelType, context.ContractType.Name + Strings.ChannelTypeSuffix, Strings.ChannelTypeSuffix);
            }
        }

        internal class CodeTypeFactory
        {
            private ServiceContractGenerator _parent;
            private bool _internalTypes;
            public CodeTypeFactory(ServiceContractGenerator parent, bool internalTypes)
            {
                _parent = parent;
                _internalTypes = internalTypes;
            }

            public CodeTypeDeclaration CreateClassType()
            {
                return CreateCodeType(false);
            }

            private CodeTypeDeclaration CreateCodeType(bool isInterface)
            {
                CodeTypeDeclaration codeType = new CodeTypeDeclaration();
                codeType.IsClass = !isInterface;
                codeType.IsInterface = isInterface;

                RunDecorators(codeType);

                return codeType;
            }

            public CodeTypeDeclaration CreateInterfaceType()
            {
                return CreateCodeType(true);
            }

            private void RunDecorators(CodeTypeDeclaration codeType)
            {
                AddPartial(codeType);
                AddInternal(codeType);
                AddDebuggerStepThroughAttribute(codeType);
                AddGeneratedCodeAttribute(codeType);
            }

            #region CodeTypeDeclaration decorators

            private void AddDebuggerStepThroughAttribute(CodeTypeDeclaration codeType)
            {
                if (codeType.IsClass)
                {
                    codeType.CustomAttributes.Add(new CodeAttributeDeclaration(_parent.GetCodeTypeReference(typeof(DebuggerStepThroughAttribute))));
                }
            }

            private void AddGeneratedCodeAttribute(CodeTypeDeclaration codeType)
            {
                CodeAttributeDeclaration generatedCodeAttribute = new CodeAttributeDeclaration(_parent.GetCodeTypeReference(typeof(GeneratedCodeAttribute)));

                AssemblyName assemblyName = this.GetType().GetTypeInfo().Assembly.GetName();
                generatedCodeAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(assemblyName.Name)));
                generatedCodeAttribute.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(assemblyName.Version.ToString())));

                codeType.CustomAttributes.Add(generatedCodeAttribute);
            }

            private void AddInternal(CodeTypeDeclaration codeType)
            {
                if (_internalTypes)
                {
                    codeType.TypeAttributes &= ~TypeAttributes.Public;
                }
            }

            private void AddPartial(CodeTypeDeclaration codeType)
            {
                if (codeType.IsClass)
                {
                    codeType.IsPartial = true;
                }
            }
            #endregion
        }

        internal static class ExtensionsHelper
        {
            // calls the behavior extensions
            static internal void CallBehaviorExtensions(ServiceContractGenerationContext context)
            {
                CallContractExtensions(EnumerateBehaviorExtensions(context.Contract), context);

                foreach (OperationContractGenerationContext operationContext in context.Operations)
                {
                    CallOperationExtensions(EnumerateBehaviorExtensions(operationContext.Operation), operationContext);
                }
            }

            // calls a specific set of contract-level extensions
            static internal void CallContractExtensions(IEnumerable<IServiceContractGenerationExtension> extensions, ServiceContractGenerationContext context)
            {
                foreach (IServiceContractGenerationExtension extension in extensions)
                {
                    extension.GenerateContract(context);
                }
            }

            // calls a specific set of operation-level extensions on each operation in the contract
            static internal void CallOperationExtensions(IEnumerable<IOperationContractGenerationExtension> extensions, ServiceContractGenerationContext context)
            {
                foreach (OperationContractGenerationContext operationContext in context.Operations)
                {
                    CallOperationExtensions(extensions, operationContext);
                }
            }

            // calls a specific set of operation-level extensions
            private static void CallOperationExtensions(IEnumerable<IOperationContractGenerationExtension> extensions, OperationContractGenerationContext context)
            {
                foreach (IOperationContractGenerationExtension extension in extensions)
                {
                    extension.GenerateOperation(context);
                }
            }

            private static IEnumerable<IServiceContractGenerationExtension> EnumerateBehaviorExtensions(ContractDescription contract)
            {
                foreach (IContractBehavior behavior in contract.Behaviors)
                {
                    if (behavior is IServiceContractGenerationExtension)
                    {
                        yield return (IServiceContractGenerationExtension)behavior;
                    }
                }
            }

            private static IEnumerable<IOperationContractGenerationExtension> EnumerateBehaviorExtensions(OperationDescription operation)
            {
                foreach (IOperationBehavior behavior in operation.Behaviors)
                {
                    if (behavior is IOperationContractGenerationExtension)
                    {
                        yield return (IOperationContractGenerationExtension)behavior;
                    }
                }
            }
        }

        private class FaultContractAttributeGenerator : IOperationContractGenerationExtension
        {
            private static CodeTypeReference s_voidTypeReference = new CodeTypeReference(typeof(void));

            void IOperationContractGenerationExtension.GenerateOperation(OperationContractGenerationContext context)
            {
                CodeMemberMethod methodDecl = context.SyncMethod ?? context.BeginMethod;
                foreach (FaultDescription fault in context.Operation.Faults)
                {
                    CodeAttributeDeclaration faultAttr = CreateAttrDecl(context, fault);
                    if (faultAttr != null)
                        methodDecl.CustomAttributes.Add(faultAttr);
                }
            }

            private static CodeAttributeDeclaration CreateAttrDecl(OperationContractGenerationContext context, FaultDescription fault)
            {
                CodeTypeReference exceptionTypeReference = fault.DetailType != null ? context.Contract.ServiceContractGenerator.GetCodeTypeReference(fault.DetailType) : fault.DetailTypeReference;
                if (exceptionTypeReference == null || exceptionTypeReference == s_voidTypeReference)
                    return null;
                CodeAttributeDeclaration faultContractAttr = new CodeAttributeDeclaration(context.ServiceContractGenerator.GetCodeTypeReference(typeof(FaultContractAttribute)));
                faultContractAttr.Arguments.Add(new CodeAttributeArgument(new CodeTypeOfExpression(exceptionTypeReference)));
                if (fault.Action != null)
                    faultContractAttr.Arguments.Add(new CodeAttributeArgument("Action", new CodePrimitiveExpression(fault.Action)));
                if (fault.HasProtectionLevel)
                {
                    faultContractAttr.Arguments.Add(new CodeAttributeArgument("ProtectionLevel",
                        new CodeFieldReferenceExpression(
                            new CodeTypeReferenceExpression(typeof(ProtectionLevel)), fault.ProtectionLevel.ToString())));
                }
                // override name with encoded value specified in wsdl; this only works beacuse
                // our Encoding algorithm will leave alredy encoded names untouched
                if (!XmlName.IsNullOrEmpty(fault.ElementName))
                    faultContractAttr.Arguments.Add(new CodeAttributeArgument("Name", new CodePrimitiveExpression(fault.ElementName.EncodedName)));
                if (fault.Namespace != context.Contract.Contract.Namespace)
                    faultContractAttr.Arguments.Add(new CodeAttributeArgument("Namespace", new CodePrimitiveExpression(fault.Namespace)));
                return faultContractAttr;
            }
        }

        private class MessageDescriptionComparer : IEqualityComparer<MessageDescription>
        {
            static internal MessageDescriptionComparer Singleton = new MessageDescriptionComparer();
            private MessageDescriptionComparer() { }

            bool IEqualityComparer<MessageDescription>.Equals(MessageDescription x, MessageDescription y)
            {
                if (x.XsdTypeName != y.XsdTypeName)
                    return false;
                // compare headers
                if (x.Headers.Count != y.Headers.Count)
                    return false;

                MessageHeaderDescription[] xHeaders = new MessageHeaderDescription[x.Headers.Count];
                x.Headers.CopyTo(xHeaders, 0);
                MessageHeaderDescription[] yHeaders = new MessageHeaderDescription[y.Headers.Count];
                y.Headers.CopyTo(yHeaders, 0);
                if (x.Headers.Count > 1)
                {
                    Array.Sort((MessagePartDescription[])xHeaders, MessagePartDescriptionComparer.Singleton);
                    Array.Sort((MessagePartDescription[])yHeaders, MessagePartDescriptionComparer.Singleton);
                }

                for (int i = 0; i < xHeaders.Length; i++)
                {
                    if (MessagePartDescriptionComparer.Singleton.Compare(xHeaders[i], yHeaders[i]) != 0)
                        return false;
                }
                return true;
            }

            int IEqualityComparer<MessageDescription>.GetHashCode(MessageDescription obj)
            {
                return obj.XsdTypeName.GetHashCode();
            }

            private class MessagePartDescriptionComparer : IComparer<MessagePartDescription>
            {
                static internal MessagePartDescriptionComparer Singleton = new MessagePartDescriptionComparer();
                private MessagePartDescriptionComparer() { }

                public int Compare(MessagePartDescription p1, MessagePartDescription p2)
                {
                    if (null == p1)
                    {
                        return (null == p2) ? 0 : -1;
                    }
                    if (null == p2)
                    {
                        return 1;
                    }
                    int i = String.CompareOrdinal(p1.Namespace, p2.Namespace);
                    if (i == 0)
                    {
                        i = String.CompareOrdinal(p1.Name, p2.Name);
                    }
                    return i;
                }
            }
        }

        internal class NamespaceHelper
        {
            private static readonly object s_referenceKey = new object();

            private const string WildcardNamespaceMapping = "*";

            private readonly CodeNamespaceCollection _codeNamespaces;
            private Dictionary<string, string> _namespaceMappings;

            public NamespaceHelper(CodeNamespaceCollection namespaces)
            {
                _codeNamespaces = namespaces;
            }

            public Dictionary<string, string> NamespaceMappings
            {
                get
                {
                    if (_namespaceMappings == null)
                        _namespaceMappings = new Dictionary<string, string>();

                    return _namespaceMappings;
                }
            }

            private string DescriptionToCode(string descriptionNamespace)
            {
                string target = String.Empty;

                // use field to avoid init'ing dictionary if possible
                if (_namespaceMappings != null)
                {
                    if (!_namespaceMappings.TryGetValue(descriptionNamespace, out target))
                    {
                        // try to fall back to wildcard
                        if (!_namespaceMappings.TryGetValue(WildcardNamespaceMapping, out target))
                        {
                            return String.Empty;
                        }
                    }
                }

                return target;
            }

            public CodeNamespace EnsureNamespace(string descriptionNamespace)
            {
                string ns = DescriptionToCode(descriptionNamespace);

                CodeNamespace codeNamespace = FindNamespace(ns);
                if (codeNamespace == null)
                {
                    codeNamespace = new CodeNamespace(ns);
                    _codeNamespaces.Add(codeNamespace);
                }

                return codeNamespace;
            }

            private CodeNamespace FindNamespace(string ns)
            {
                foreach (CodeNamespace codeNamespace in _codeNamespaces)
                {
                    if (codeNamespace.Name == ns)
                        return codeNamespace;
                }

                return null;
            }

            public static CodeTypeDeclaration GetCodeType(CodeTypeReference codeTypeReference)
            {
                return codeTypeReference.UserData[s_referenceKey] as CodeTypeDeclaration;
            }

            static internal CodeTypeReference GetCodeTypeReference(CodeNamespace codeNamespace, CodeTypeDeclaration codeType)
            {
                CodeTypeReference codeTypeReference = new CodeTypeReference(String.IsNullOrEmpty(codeNamespace.Name) ? codeType.Name : codeNamespace.Name + '.' + codeType.Name);
                codeTypeReference.UserData[s_referenceKey] = codeType;
                return codeTypeReference;
            }
        }

        internal struct OptionsHelper
        {
            public readonly ServiceContractGenerationOptions Options;

            public OptionsHelper(ServiceContractGenerationOptions options)
            {
                this.Options = options;
            }

            public bool IsSet(ServiceContractGenerationOptions option)
            {
                Fx.Assert(IsSingleBit((int)option), "");

                return ((this.Options & option) != ServiceContractGenerationOptions.None);
            }

            private static bool IsSingleBit(int x)
            {
                //figures out if the mode has a single bit set ( is a power of 2)
                return (x != 0) && ((x & (x + ~0)) == 0);
            }
        }

        private static class Strings
        {
            public const string AsyncCallbackArgName = "callback";
            public const string AsyncStateArgName = "asyncState";
            public const string AsyncResultArgName = "result";

            public const string CallbackTypeSuffix = "Callback";
            public const string ChannelTypeSuffix = "Channel";

            public const string DefaultContractName = "IContract";
            public const string DefaultOperationName = "Method";

            public const string InterfaceTypePrefix = "I";
        }

        // ideally this one would appear on TransactionFlowAttribute
        private class TransactionFlowAttributeGenerator : IOperationContractGenerationExtension
        {
            void IOperationContractGenerationExtension.GenerateOperation(OperationContractGenerationContext context)
            {
                System.ServiceModel.TransactionFlowAttribute attr = context.Operation.Behaviors.Find<System.ServiceModel.TransactionFlowAttribute>();
                if (attr != null && attr.Transactions != TransactionFlowOption.NotAllowed)
                {
                    CodeMemberMethod methodDecl = context.SyncMethod ?? context.BeginMethod;
                    methodDecl.CustomAttributes.Add(CreateAttrDecl(context, attr));
                }
            }

            private static CodeAttributeDeclaration CreateAttrDecl(OperationContractGenerationContext context, TransactionFlowAttribute attr)
            {
                CodeAttributeDeclaration attrDecl = new CodeAttributeDeclaration(context.Contract.ServiceContractGenerator.GetCodeTypeReference(typeof(TransactionFlowAttribute)));
                attrDecl.Arguments.Add(new CodeAttributeArgument(ServiceContractGenerator.GetEnumReference<TransactionFlowOption>(attr.Transactions)));
                return attrDecl;
            }
        }
    }
}
