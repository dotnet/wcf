// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Microsoft.Xml;

namespace System.ServiceModel.Description
{
    internal class TypeLoader
    {
        private static Type[] s_messageContractMemberAttributes = {
            typeof(MessageHeaderAttribute),
            typeof(MessageBodyMemberAttribute),
        };

        private static Type[] s_formatterAttributes = {
            typeof(XmlSerializerFormatAttribute),
            typeof(DataContractFormatAttribute)
        };

        private static Type[] s_knownTypesMethodParamType = new Type[] { typeof(CustomAttributeProvider) };

        internal static DataContractFormatAttribute DefaultDataContractFormatAttribute = new DataContractFormatAttribute();
        internal static XmlSerializerFormatAttribute DefaultXmlSerializerFormatAttribute = new XmlSerializerFormatAttribute();

        private static readonly Type s_OperationContractAttributeType = typeof(OperationContractAttribute);

        internal const string ReturnSuffix = "Result";
        internal const string ResponseSuffix = "Response";
        internal const string FaultSuffix = "Fault";
        internal const BindingFlags DefaultBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public;

        private readonly object _thisLock;
        private readonly Dictionary<Type, ContractDescription> _contracts;
        private readonly Dictionary<Type, MessageDescriptionItems> _messages;

        public TypeLoader()
        {
            _thisLock = new object();
            _contracts = new Dictionary<Type, ContractDescription>();
            _messages = new Dictionary<Type, MessageDescriptionItems>();
        }
        [SuppressMessage(FxCop.Category.Usage, "CA2301:EmbeddableTypesInContainersRule", MessageId = "contracts", Justification = "No need to support type equivalence here.")]

        private ContractDescription LoadContractDescriptionHelper(Type contractType, Type serviceType, object serviceImplementation)
        {
            ContractDescription contractDescription;
            if (contractType == typeof(IOutputChannel))
            {
                contractDescription = LoadOutputChannelContractDescription();
            }
            else if (contractType == typeof(IRequestChannel))
            {
                contractDescription = LoadRequestChannelContractDescription();
            }
            else
            {
                ServiceContractAttribute actualContractAttribute;
                Type actualContractType = ServiceReflector.GetContractTypeAndAttribute(contractType, out actualContractAttribute);
                lock (_thisLock)
                {
                    if (!_contracts.TryGetValue(actualContractType, out contractDescription))
                    {
                        EnsureNoInheritanceWithContractClasses(actualContractType);
                        EnsureNoOperationContractsOnNonServiceContractTypes(actualContractType);
                        ContractReflectionInfo reflectionInfo;
                        contractDescription = CreateContractDescription(actualContractAttribute, actualContractType, serviceType, out reflectionInfo, serviceImplementation);
                        // IContractBehaviors
                        if (serviceImplementation != null && serviceImplementation is IContractBehavior)
                        {
                            contractDescription.Behaviors.Add((IContractBehavior)serviceImplementation);
                        }
                        if (serviceType != null)
                        {
                            UpdateContractDescriptionWithAttributesFromServiceType(contractDescription, serviceType);
                            foreach (ContractDescription inheritedContract in contractDescription.GetInheritedContracts())
                            {
                                UpdateContractDescriptionWithAttributesFromServiceType(inheritedContract, serviceType);
                            }
                        }
                        UpdateOperationsWithInterfaceAttributes(contractDescription, reflectionInfo);
                        AddBehaviors(contractDescription, serviceType, false, reflectionInfo);

                        _contracts.Add(actualContractType, contractDescription);
                    }
                }
            }
            return contractDescription;
        }

        private void EnsureNoInheritanceWithContractClasses(Type actualContractType)
        {
            if (actualContractType.IsClass())
            {
                // we only need to check base _classes_ here, the check for interfaces happens elsewhere
                for (Type service = actualContractType.BaseType(); service != null; service = service.BaseType())
                {
                    if (ServiceReflector.GetSingleAttribute<ServiceContractAttribute>(service) != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                            string.Format(SRServiceModel.SFxContractInheritanceRequiresInterfaces, actualContractType, service)));
                    }
                }
            }
        }

        private void EnsureNoOperationContractsOnNonServiceContractTypes(Type actualContractType)
        {
            foreach (Type t in actualContractType.GetInterfaces())
            {
                EnsureNoOperationContractsOnNonServiceContractTypes_Helper(t);
            }
            for (Type u = actualContractType.BaseType(); u != null; u = u.BaseType())
            {
                EnsureNoOperationContractsOnNonServiceContractTypes_Helper(u);
            }
        }

        private void EnsureNoOperationContractsOnNonServiceContractTypes_Helper(Type aParentType)
        {
            // if not [ServiceContract]
            if (ServiceReflector.GetSingleAttribute<ServiceContractAttribute>(aParentType) == null)
            {
                foreach (MethodInfo methodInfo in aParentType.GetRuntimeMethods().Where(m => !m.IsStatic))
                {
                    // but does have an OperationContractAttribute
                    Type operationContractProviderType = ServiceReflector.GetOperationContractProviderType(methodInfo);
                    if (operationContractProviderType != null)
                    {
                        if (operationContractProviderType == s_OperationContractAttributeType)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(
                                SRServiceModel.SFxOperationContractOnNonServiceContract, methodInfo.Name, aParentType.Name)));
                        }
                        else
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(
                                SRServiceModel.SFxOperationContractProviderOnNonServiceContract, operationContractProviderType.Name, methodInfo.Name, aParentType.Name)));
                        }
                    }
                }
            }
        }

        public ContractDescription LoadContractDescription(Type contractType)
        {
            Fx.Assert(contractType != null, "");

            return LoadContractDescriptionHelper(contractType, null, null);
        }

        public ContractDescription LoadContractDescription(Type contractType, Type serviceType)
        {
            Fx.Assert(contractType != null, "");
            Fx.Assert(serviceType != null, "");

            return LoadContractDescriptionHelper(contractType, serviceType, null);
        }

        public ContractDescription LoadContractDescription(Type contractType, Type serviceType, object serviceImplementation)
        {
            Fx.Assert(contractType != null, "");
            Fx.Assert(serviceType != null, "");
            Fx.Assert(serviceImplementation != null, "");

            return LoadContractDescriptionHelper(contractType, serviceType, serviceImplementation);
        }

        private ContractDescription LoadOutputChannelContractDescription()
        {
            Type channelType = typeof(IOutputChannel);
            XmlQualifiedName contractName = NamingHelper.GetContractName(channelType, null, NamingHelper.MSNamespace);
            ContractDescription contract = new ContractDescription(contractName.Name, contractName.Namespace);
            contract.ContractType = channelType;
            contract.ConfigurationName = channelType.FullName;
            contract.SessionMode = SessionMode.NotAllowed;
            OperationDescription operation = new OperationDescription("Send", contract);
            MessageDescription message = new MessageDescription(MessageHeaders.WildcardAction, MessageDirection.Input);
            operation.Messages.Add(message);
            contract.Operations.Add(operation);
            return contract;
        }

        private ContractDescription LoadRequestChannelContractDescription()
        {
            Type channelType = typeof(IRequestChannel);
            XmlQualifiedName contractName = NamingHelper.GetContractName(channelType, null, NamingHelper.MSNamespace);
            ContractDescription contract = new ContractDescription(contractName.Name, contractName.Namespace);
            contract.ContractType = channelType;
            contract.ConfigurationName = channelType.FullName;
            contract.SessionMode = SessionMode.NotAllowed;
            OperationDescription operation = new OperationDescription("Request", contract);
            MessageDescription request = new MessageDescription(MessageHeaders.WildcardAction, MessageDirection.Input);
            MessageDescription reply = new MessageDescription(MessageHeaders.WildcardAction, MessageDirection.Output);
            operation.Messages.Add(request);
            operation.Messages.Add(reply);
            contract.Operations.Add(operation);
            return contract;
        }

        private void AddBehaviors(ContractDescription contractDesc, Type implType, bool implIsCallback, ContractReflectionInfo reflectionInfo)
        {
            ServiceContractAttribute contractAttr = ServiceReflector.GetRequiredSingleAttribute<ServiceContractAttribute>(reflectionInfo.iface);
            for (int i = 0; i < contractDesc.Operations.Count; i++)
            {
                OperationDescription operationDescription = contractDesc.Operations[i];
                bool isInherited = operationDescription.DeclaringContract != contractDesc;
                if (!isInherited)
                {
                    operationDescription.Behaviors.Add(new OperationInvokerBehavior());
                }
            }
            contractDesc.Behaviors.Add(new OperationSelectorBehavior());

            for (int i = 0; i < contractDesc.Operations.Count; i++)
            {
                OperationDescription opDesc = contractDesc.Operations[i];
                bool isInherited = opDesc.DeclaringContract != contractDesc;
                Type targetIface = implIsCallback ? opDesc.DeclaringContract.CallbackContractType : opDesc.DeclaringContract.ContractType;

                if (implType == null && !isInherited)
                {
                    KeyedByTypeCollection<IOperationBehavior> toAdd =
                        GetIOperationBehaviorAttributesFromType(opDesc, targetIface, null);
                    for (int j = 0; j < toAdd.Count; j++)
                    {
                        opDesc.Behaviors.Add(toAdd[j]);
                    }
                }
                else
                {
                    // look for IOperationBehaviors on implementation methods in service class hierarchy
                    ApplyServiceInheritance<IOperationBehavior, KeyedByTypeCollection<IOperationBehavior>>(
                        implType, opDesc.Behaviors,
                        delegate (Type currentType, KeyedByTypeCollection<IOperationBehavior> behaviors)
                        {
                            KeyedByTypeCollection<IOperationBehavior> toAdd =
                                GetIOperationBehaviorAttributesFromType(opDesc, targetIface, currentType);
                            for (int j = 0; j < toAdd.Count; j++)
                            {
                                behaviors.Add(toAdd[j]);
                            }
                        });
                    // then look for IOperationBehaviors on interface type
                    if (!isInherited)
                    {
                        AddBehaviorsAtOneScope<IOperationBehavior, KeyedByTypeCollection<IOperationBehavior>>(
                            targetIface, opDesc.Behaviors,
                            delegate (Type currentType, KeyedByTypeCollection<IOperationBehavior> behaviors)
                            {
                                KeyedByTypeCollection<IOperationBehavior> toAdd =
                                    GetIOperationBehaviorAttributesFromType(opDesc, targetIface, null);
                                for (int j = 0; j < toAdd.Count; j++)
                                {
                                    behaviors.Add(toAdd[j]);
                                }
                            });
                    }
                }
            }

            Type targetInterface = implIsCallback ? reflectionInfo.callbackiface : reflectionInfo.iface;
            AddBehaviorsAtOneScope<IContractBehavior, KeyedByTypeCollection<IContractBehavior>>(targetInterface, contractDesc.Behaviors,
                GetIContractBehaviorsFromInterfaceType);

            bool hasXmlSerializerMethod = false;
            for (int i = 0; i < contractDesc.Operations.Count; i++)
            {
                OperationDescription operationDescription = contractDesc.Operations[i];
                bool isInherited = operationDescription.DeclaringContract != contractDesc;
                MethodInfo opMethod = operationDescription.OperationMethod;
                Attribute formattingAttribute = GetFormattingAttribute(opMethod,
                                                    GetFormattingAttribute(operationDescription.DeclaringContract.ContractType,
                                                        DefaultDataContractFormatAttribute));
                DataContractFormatAttribute dataContractFormatAttribute = formattingAttribute as DataContractFormatAttribute;
                if (dataContractFormatAttribute != null)
                {
                    if (!isInherited)
                    {
                        operationDescription.Behaviors.Add(new DataContractSerializerOperationBehavior(operationDescription, dataContractFormatAttribute, true));
                    }
                }
                else if (formattingAttribute != null && formattingAttribute is XmlSerializerFormatAttribute)
                {
                    hasXmlSerializerMethod = true;
                }
            }
            if (hasXmlSerializerMethod)
            {
                XmlSerializerOperationBehavior.AddBuiltInBehaviors(contractDesc);
            }
        }

        private void GetIContractBehaviorsFromInterfaceType(Type interfaceType, KeyedByTypeCollection<IContractBehavior> behaviors)
        {
            object[] ifaceAttributes = ServiceReflector.GetCustomAttributes(interfaceType, typeof(IContractBehavior), false);
            for (int i = 0; i < ifaceAttributes.Length; i++)
            {
                IContractBehavior behavior = (IContractBehavior)ifaceAttributes[i];
                behaviors.Add(behavior);
            }
        }

        private static void UpdateContractDescriptionWithAttributesFromServiceType(ContractDescription description, Type serviceType)
        {
            ApplyServiceInheritance<IContractBehavior, KeyedByTypeCollection<IContractBehavior>>(
                serviceType, description.Behaviors,
                delegate (Type currentType, KeyedByTypeCollection<IContractBehavior> behaviors)
                {
                    foreach (IContractBehavior iContractBehavior in ServiceReflector.GetCustomAttributes(currentType, typeof(IContractBehavior), false))
                    {
                        throw ExceptionHelper.PlatformNotSupported();
                    }
                });
        }

        private void UpdateOperationsWithInterfaceAttributes(ContractDescription contractDesc, ContractReflectionInfo reflectionInfo)
        {
            object[] customAttributes = ServiceReflector.GetCustomAttributes(reflectionInfo.iface, typeof(ServiceKnownTypeAttribute), false);
            IEnumerable<Type> knownTypes = GetKnownTypes(customAttributes, reflectionInfo.iface);
            foreach (Type knownType in knownTypes)
            {
                foreach (OperationDescription operationDescription in contractDesc.Operations)
                {
                    if (!operationDescription.IsServerInitiated())
                        operationDescription.KnownTypes.Add(knownType);
                }
            }

            if (reflectionInfo.callbackiface != null)
            {
                customAttributes = ServiceReflector.GetCustomAttributes(reflectionInfo.callbackiface, typeof(ServiceKnownTypeAttribute), false);
                knownTypes = GetKnownTypes(customAttributes, reflectionInfo.callbackiface);
                foreach (Type knownType in knownTypes)
                {
                    foreach (OperationDescription operationDescription in contractDesc.Operations)
                    {
                        if (operationDescription.IsServerInitiated())
                            operationDescription.KnownTypes.Add(knownType);
                    }
                }
            }
        }

        private IEnumerable<Type> GetKnownTypes(object[] knownTypeAttributes, CustomAttributeProvider provider)
        {
            if (knownTypeAttributes.Length == 1)
            {
                ServiceKnownTypeAttribute knownTypeAttribute = (ServiceKnownTypeAttribute)knownTypeAttributes[0];
                if (!string.IsNullOrEmpty(knownTypeAttribute.MethodName))
                {
                    Type type = knownTypeAttribute.DeclaringType;
                    if (type == null)
                    {
                        type = provider.Type;
                        if (type == null && provider.MethodInfo != null)
                            type = provider.MethodInfo.DeclaringType;
                    }
                    MethodInfo method = type.GetRuntimeMethod(knownTypeAttribute.MethodName, s_knownTypesMethodParamType);
                    if (method == null)
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxKnownTypeAttributeUnknownMethod3, provider, knownTypeAttribute.MethodName, type.FullName)));

                    if (!typeof(IEnumerable<Type>).IsAssignableFrom(method.ReturnType))
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxKnownTypeAttributeReturnType3, provider, knownTypeAttribute.MethodName, type.FullName)));

                    return (IEnumerable<Type>)method.Invoke(null, new object[] { provider });
                }
            }

            List<Type> knownTypes = new List<Type>();
            for (int i = 0; i < knownTypeAttributes.Length; ++i)
            {
                ServiceKnownTypeAttribute knownTypeAttribute = (ServiceKnownTypeAttribute)knownTypeAttributes[i];
                if (knownTypeAttribute.Type == null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxKnownTypeAttributeInvalid1, provider.ToString())));
                knownTypes.Add(knownTypeAttribute.Type);
            }
            return knownTypes;
        }

        private KeyedByTypeCollection<IOperationBehavior> GetIOperationBehaviorAttributesFromType(OperationDescription opDesc, Type targetIface, Type implType)
        {
            KeyedByTypeCollection<IOperationBehavior> result = new KeyedByTypeCollection<IOperationBehavior>();
            bool useImplAttrs = false;
            if (implType != null)
            {
                if (targetIface.IsAssignableFrom(implType) && targetIface.IsInterface())
                {
                    useImplAttrs = true;
                }
                else
                {
                    // implType does not implement any methods from the targetIface, so there is nothing to do
                    return result;
                }
            }
            MethodInfo opMethod = opDesc.OperationMethod;
            ProcessOpMethod(opMethod, true, opDesc, result, targetIface, implType, useImplAttrs);
            if (opDesc.SyncMethod != null && opDesc.BeginMethod != null)
            {
                ProcessOpMethod(opDesc.BeginMethod, false, opDesc, result, targetIface, implType, useImplAttrs);
            }
            else if (opDesc.SyncMethod != null && opDesc.TaskMethod != null)
            {
                ProcessOpMethod(opDesc.TaskMethod, false, opDesc, result, targetIface, implType, useImplAttrs);
            }
            else if (opDesc.TaskMethod != null && opDesc.BeginMethod != null)
            {
                ProcessOpMethod(opDesc.BeginMethod, false, opDesc, result, targetIface, implType, useImplAttrs);
            }
            return result;
        }

        private void ProcessOpMethod(MethodInfo opMethod, bool canHaveBehaviors,
                     OperationDescription opDesc, KeyedByTypeCollection<IOperationBehavior> result,
                     Type ifaceType, Type implType, bool useImplAttrs)
        {
            MethodInfo method = null;
            if (useImplAttrs)
            {
                MethodInfo ifaceMethod = GetCorrespondingMethodFromType(ifaceType, opMethod);
                // if opMethod doesn't exist in the interface, it means opMethod was on
                // the "other" interface (not the one implemented by implType)
                if (ifaceMethod != null)
                {
                    MethodInfo implMethod = GetCorrespondingMethodFromType(implType, opMethod);
                    if (implMethod != null)
                    {
                        method = implMethod;
                    }
                }
                if (method == null)
                {
                    return;
                }
            }
            else
            {
                method = opMethod;
            }

            object[] methodAttributes = ServiceReflector.GetCustomAttributes(method, typeof(IOperationBehavior), false);
            for (int k = 0; k < methodAttributes.Length; k++)
            {
                IOperationBehavior opBehaviorAttr = (IOperationBehavior)methodAttributes[k];
                if (canHaveBehaviors)
                {
                    result.Add(opBehaviorAttr);
                }
                else
                {
                    if (opDesc.SyncMethod != null && opDesc.BeginMethod != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(string.Format(SRServiceModel.SyncAsyncMatchConsistency_Attributes6,
                                                                       opDesc.SyncMethod.Name,
                                                                       opDesc.SyncMethod.DeclaringType,
                                                                       opDesc.BeginMethod.Name,
                                                                       opDesc.EndMethod.Name,
                                                                       opDesc.Name,
                                                                       opBehaviorAttr.GetType().FullName)));
                    }
                    else if (opDesc.SyncMethod != null && opDesc.TaskMethod != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(string.Format(SRServiceModel.SyncTaskMatchConsistency_Attributes6,
                                                                       opDesc.SyncMethod.Name,
                                                                       opDesc.SyncMethod.DeclaringType,
                                                                       opDesc.TaskMethod.Name,
                                                                       opDesc.Name,
                                                                       opBehaviorAttr.GetType().FullName)));
                    }
                    else if (opDesc.TaskMethod != null && opDesc.BeginMethod != null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(string.Format(SRServiceModel.TaskAsyncMatchConsistency_Attributes6,
                                                                       opDesc.TaskMethod.Name,
                                                                       opDesc.TaskMethod.DeclaringType,
                                                                       opDesc.BeginMethod.Name,
                                                                       opDesc.EndMethod.Name,
                                                                       opDesc.Name,
                                                                       opBehaviorAttr.GetType().FullName)));
                    }
                    Fx.Assert("Invalid state. No exception for canHaveBehaviors = false");
                }
            }
        }

        // Given a MethodInfo which could be from any type, return a MethodInfo declared by the given type 
        // that matches in name and parameter types.  Null is returned if there is no match.
        private static MethodInfo GetCorrespondingMethodFromType(Type type, MethodInfo methodInfo)
        {
            Contract.Assert(type != null);
            Contract.Assert(methodInfo != null);

            if (methodInfo.DeclaringType == type)
            {
                return methodInfo;
            }

            MethodInfo matchingMethod = type.GetTypeInfo().DeclaredMethods.SingleOrDefault(m => MethodsMatch(m, methodInfo));
            return matchingMethod;
        }

        // Returns true if the given methods match in name and parameter types
        private static bool MethodsMatch(MethodInfo method1, MethodInfo method2)
        {
            Contract.Assert(method1 != null);
            Contract.Assert(method2 != null);

            if (method1.Equals(method2))
            {
                return true;
            }

            if (method1.ReturnType != method2.ReturnType ||
                !String.Equals(method1.Name, method2.Name, StringComparison.Ordinal) ||
                !ParameterInfosMatch(method1.ReturnParameter, method2.ReturnParameter))
            {
                return false;
            }

            ParameterInfo[] parameters1 = method1.GetParameters();
            ParameterInfo[] parameters2 = method2.GetParameters();
            if (parameters1.Length != parameters2.Length)
            {
                return false;
            }

            for (int i = 0; i < parameters1.Length; ++i)
            {
                if (!ParameterInfosMatch(parameters1[i], parameters2[i]))
                {
                    return false;
                }
            }

            return true;
        }

        // Returns true if 2 ParameterInfo's match in signature with respect
        // to the MemberInfo's in which they are declared. Position is required
        // to match but name is not.
        private static bool ParameterInfosMatch(ParameterInfo parameterInfo1, ParameterInfo parameterInfo2)
        {
            // Null is possible for a ParameterInfo from MethodInfo.ReturnParameter.
            // If both are null, we have no information to compare and say they are equal.
            if (parameterInfo1 == null && parameterInfo2 == null)
            {
                return true;
            }

            if (parameterInfo1 == null || parameterInfo2 == null)
            {
                return false;
            }

            if (parameterInfo1.Equals(parameterInfo2))
            {
                return true;
            }

            return ((parameterInfo1.ParameterType == parameterInfo2.ParameterType) &&
                    (parameterInfo1.IsIn == parameterInfo2.IsIn) &&
                    (parameterInfo1.IsOut == parameterInfo2.IsOut) &&
                    (parameterInfo1.IsRetval == parameterInfo2.IsRetval) &&
                    (parameterInfo1.Position == parameterInfo2.Position));
        }

        internal void AddBehaviorsSFx(ServiceEndpoint serviceEndpoint, Type contractType)
        {
            if (serviceEndpoint.Contract.IsDuplex())
            {
                CallbackBehaviorAttribute attr = serviceEndpoint.Behaviors.Find<CallbackBehaviorAttribute>();
                if (attr == null)
                {
                    serviceEndpoint.Behaviors.Insert(0, new CallbackBehaviorAttribute());
                }
            }
        }

        internal void AddBehaviorsFromImplementationType(ServiceEndpoint serviceEndpoint, Type implementationType)
        {
            foreach (IEndpointBehavior behaviorAttribute in ServiceReflector.GetCustomAttributes(implementationType, typeof(IEndpointBehavior), false))
            {
                if(behaviorAttribute is CallbackBehaviorAttribute)
                {
                    serviceEndpoint.Behaviors.Insert(0, behaviorAttribute);
                }
                else
                {
                    serviceEndpoint.Behaviors.Add(behaviorAttribute);
                }
            }
            foreach (IContractBehavior behaviorAttribute in ServiceReflector.GetCustomAttributes(implementationType, typeof(IContractBehavior), false))
            {
                serviceEndpoint.Contract.Behaviors.Add(behaviorAttribute);
            }
            Type targetIface = serviceEndpoint.Contract.CallbackContractType;
            for (int i = 0; i < serviceEndpoint.Contract.Operations.Count; i++)
            {
                OperationDescription opDesc = serviceEndpoint.Contract.Operations[i];
                KeyedByTypeCollection<IOperationBehavior> opBehaviors = new KeyedByTypeCollection<IOperationBehavior>();
                // look for IOperationBehaviors on implementation methods in callback class hierarchy
                ApplyServiceInheritance<IOperationBehavior, KeyedByTypeCollection<IOperationBehavior>>(
                    implementationType, opBehaviors,
                    delegate (Type currentType, KeyedByTypeCollection<IOperationBehavior> behaviors)
                    {
                        KeyedByTypeCollection<IOperationBehavior> toAdd =
                            GetIOperationBehaviorAttributesFromType(opDesc, targetIface, currentType);
                        for (int j = 0; j < toAdd.Count; j++)
                        {
                            behaviors.Add(toAdd[j]);
                        }
                    });
                // a bunch of default IOperationBehaviors have already been added, which we may need to replace
                for (int k = 0; k < opBehaviors.Count; k++)
                {
                    IOperationBehavior behavior = opBehaviors[k];
                    Type t = behavior.GetType();
                    if (opDesc.Behaviors.Contains(t))
                    {
                        opDesc.Behaviors.Remove(t);
                    }
                    opDesc.Behaviors.Add(behavior);
                }
            }
        }

        internal static int CompareMessagePartDescriptions(MessagePartDescription a, MessagePartDescription b)
        {
            int posCmp = a.SerializationPosition - b.SerializationPosition;
            if (posCmp != 0)
            {
                return posCmp;
            }

            int nsCmp = string.Compare(a.Namespace, b.Namespace, StringComparison.Ordinal);
            if (nsCmp != 0)
            {
                return nsCmp;
            }

            return string.Compare(a.Name, b.Name, StringComparison.Ordinal);
        }

        internal static XmlName GetBodyWrapperResponseName(string operationName)
        {
#if DEBUG
            Fx.Assert(NamingHelper.IsValidNCName(operationName), "operationName value has to be a valid NCName.");
#endif
            return new XmlName(operationName + ResponseSuffix);
        }

        internal static XmlName GetBodyWrapperResponseName(XmlName operationName)
        {
            return new XmlName(operationName.EncodedName + ResponseSuffix, true /*isEncoded*/);
        }

        private void CreateOperationDescriptions(ContractDescription contractDescription,
                                         ContractReflectionInfo reflectionInfo,
                                         Type contractToGetMethodsFrom,
                                         ContractDescription declaringContract,
                                         MessageDirection direction
                                         )
        {
            MessageDirection otherDirection = MessageDirectionHelper.Opposite(direction);
            if (!(declaringContract.ContractType.IsAssignableFrom(contractDescription.ContractType)))
            {
                Fx.Assert("bad contract inheritance");
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    String.Format(CultureInfo.InvariantCulture, "Bad contract inheritence. Contract {0} does not implement {1}", declaringContract.ContractType.Name, contractDescription.ContractType.Name)
                    ));
            }

            foreach (MethodInfo methodInfo in contractToGetMethodsFrom.GetRuntimeMethods().Where(m => !m.IsStatic))
            {
                ServiceReflector.ValidateParameterMetadata(methodInfo);
                OperationDescription operation = CreateOperationDescription(contractDescription, methodInfo, direction, reflectionInfo, declaringContract);
                if (operation != null)
                {
                    contractDescription.Operations.Add(operation);
                }
            }
        }

        //Checks whether that the Callback contract provided on a ServiceContract follows rules
        //1. It has to be a interface
        //2. If its a class then it needs to implement MarshallByRefObject
        internal static void EnsureCallbackType(Type callbackType)
        {
            if (callbackType != null && !callbackType.IsInterface() && !callbackType.IsMarshalByRef())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(string.Format(SRServiceModel.SFxInvalidCallbackContractType, callbackType.Name)));
            }
        }

        // checks a contract for substitutability (in the Liskov Substitution Principle sense), throws on error
        internal static void EnsureSubcontract(ServiceContractAttribute svcContractAttr, Type contractType)
        {
            Type callbackType = svcContractAttr.CallbackContract;

            List<Type> types = ServiceReflector.GetInheritedContractTypes(contractType);
            for (int i = 0; i < types.Count; i++)
            {
                Type inheritedContractType = types[i];
                ServiceContractAttribute inheritedContractAttr = ServiceReflector.GetRequiredSingleAttribute<ServiceContractAttribute>(inheritedContractType);
                // we must be covariant in our callbacks
                if (inheritedContractAttr.CallbackContract != null)
                {
                    if (callbackType == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                            string.Format(SRServiceModel.InAContractInheritanceHierarchyIfParentHasCallbackChildMustToo,
                            inheritedContractType.Name, inheritedContractAttr.CallbackContract.Name, contractType.Name)));
                    }
                    if (!inheritedContractAttr.CallbackContract.IsAssignableFrom(callbackType))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                            string.Format(SRServiceModel.InAContractInheritanceHierarchyTheServiceContract3_2,
                            inheritedContractType.Name, contractType.Name)));
                    }
                }
            }
        }

        private ContractDescription CreateContractDescription(ServiceContractAttribute contractAttr, Type contractType, Type serviceType, out ContractReflectionInfo reflectionInfo, object serviceImplementation)
        {
            reflectionInfo = new ContractReflectionInfo();

            XmlQualifiedName contractName = NamingHelper.GetContractName(contractType, contractAttr.Name, contractAttr.Namespace);
            ContractDescription contractDescription = new ContractDescription(contractName.Name, contractName.Namespace);
            contractDescription.ContractType = contractType;
            if (contractAttr.HasProtectionLevel)
            {
                contractDescription.ProtectionLevel = contractAttr.ProtectionLevel;
            }

            Type callbackType = contractAttr.CallbackContract;

            EnsureCallbackType(callbackType);

            EnsureSubcontract(contractAttr, contractType);

            // reflect the methods in contractType and add OperationDescriptions to ContractDescription
            reflectionInfo.iface = contractType;
            reflectionInfo.callbackiface = callbackType;

            contractDescription.SessionMode = contractAttr.SessionMode;
            contractDescription.CallbackContractType = callbackType;
            contractDescription.ConfigurationName = contractAttr.ConfigurationName ?? contractType.FullName;

            // get inherited operations
            List<Type> types = ServiceReflector.GetInheritedContractTypes(contractType);
            List<Type> inheritedCallbackTypes = new List<Type>();
            for (int i = 0; i < types.Count; i++)
            {
                Type inheritedContractType = types[i];
                ServiceContractAttribute inheritedContractAttr = ServiceReflector.GetRequiredSingleAttribute<ServiceContractAttribute>(inheritedContractType);
                ContractDescription inheritedContractDescription = LoadContractDescriptionHelper(inheritedContractType, serviceType, serviceImplementation);
                foreach (OperationDescription op in inheritedContractDescription.Operations)
                {
                    if (!contractDescription.Operations.Contains(op)) // in a diamond hierarchy, ensure we don't add same op twice from two different parents
                    {
                        // ensure two different parents don't try to add conflicting operations
                        Collection<OperationDescription> existingOps = contractDescription.Operations.FindAll(op.Name);
                        foreach (OperationDescription existingOp in existingOps)
                        {
                            if (existingOp.Messages[0].Direction == op.Messages[0].Direction)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                                    string.Format(SRServiceModel.CannotInheritTwoOperationsWithTheSameName3,
                                        op.Name, inheritedContractDescription.Name, existingOp.DeclaringContract.Name)));
                            }
                        }
                        contractDescription.Operations.Add(op);
                    }
                }
                if (inheritedContractDescription.CallbackContractType != null)
                {
                    inheritedCallbackTypes.Add(inheritedContractDescription.CallbackContractType);
                }
            }

            // this contract 
            CreateOperationDescriptions(contractDescription, reflectionInfo, contractType, contractDescription, MessageDirection.Input);
            // CallbackContract 
            if (callbackType != null && !inheritedCallbackTypes.Contains(callbackType))
            {
                CreateOperationDescriptions(contractDescription, reflectionInfo, callbackType, contractDescription, MessageDirection.Output);
            }

            return contractDescription;
        }

        internal static Attribute GetFormattingAttribute(CustomAttributeProvider attrProvider, Attribute defaultFormatAttribute)
        {
            if (attrProvider != null)
            {
                if (attrProvider.IsDefined(typeof(XmlSerializerFormatAttribute), false))
                {
                    return ServiceReflector.GetSingleAttribute<XmlSerializerFormatAttribute>(attrProvider, s_formatterAttributes);
                }
                if (attrProvider.IsDefined(typeof(DataContractFormatAttribute), false))
                {
                    return ServiceReflector.GetSingleAttribute<DataContractFormatAttribute>(attrProvider, s_formatterAttributes);
                }
            }
            return defaultFormatAttribute;
        }

        //Sync and Async should follow the rules:
        //    1. Parameter match
        //    2. Async cannot have behaviors (verification happens later in ProcessOpMethod - behaviors haven't yet been loaded here)
        //    3. Async cannot have known types
        //    4. Async cannot have known faults
        //    5. Sync and Async have to match on OneWay status
        //    6. Sync and Async have to match Action and ReplyAction
        private void VerifyConsistency(OperationConsistencyVerifier verifier)
        {
            verifier.VerifyParameterLength();
            verifier.VerifyParameterType();
            verifier.VerifyOutParameterType();
            verifier.VerifyReturnType();
            verifier.VerifyFaultContractAttribute();
            verifier.VerifyKnownTypeAttribute();
            verifier.VerifyIsOneWayStatus();
            verifier.VerifyActionAndReplyAction();
        }

        // "direction" is the "direction of the interface" (from the perspective of the server, as usual):
        //    proxy interface on client: MessageDirection.Input
        //    callback interface on client: MessageDirection.Output
        //    service interface (or class) on server: MessageDirection.Input
        //    callback interface on server: MessageDirection.Output
        private OperationDescription CreateOperationDescription(ContractDescription contractDescription, MethodInfo methodInfo, MessageDirection direction,
                                                        ContractReflectionInfo reflectionInfo, ContractDescription declaringContract)
        {
            OperationContractAttribute opAttr = ServiceReflector.GetOperationContractAttribute(methodInfo);
            if (opAttr == null)
            {
                return null;
            }

            if (ServiceReflector.HasEndMethodShape(methodInfo))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                    string.Format(SRServiceModel.EndMethodsCannotBeDecoratedWithOperationContractAttribute,
                                 methodInfo.Name, reflectionInfo.iface)));
            }

            Type taskTResult;
            bool isTask = ServiceReflector.IsTask(methodInfo, out taskTResult);
            bool isAsync = isTask ? false : ServiceReflector.IsBegin(opAttr, methodInfo);

            XmlName operationName = NamingHelper.GetOperationName(ServiceReflector.GetLogicalName(methodInfo, isAsync, isTask), opAttr.Name);

            opAttr.EnsureInvariants(methodInfo, operationName.EncodedName);

            Collection<OperationDescription> operations = contractDescription.Operations.FindAll(operationName.EncodedName);
            for (int i = 0; i < operations.Count; i++)
            {
                OperationDescription existingOp = operations[i];
                if (existingOp.Messages[0].Direction == direction)
                {
                    // if we have already seen a task-based method with the same name, we need to throw.
                    if (existingOp.TaskMethod != null && isTask)
                    {
                        string method1Name = existingOp.OperationMethod.Name;
                        string method2Name = methodInfo.Name;
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.CannotHaveTwoOperationsWithTheSameName3, method1Name, method2Name, reflectionInfo.iface)));
                    }
                    if (isAsync && (existingOp.BeginMethod != null))
                    {
                        string method1Name = existingOp.BeginMethod.Name;
                        string method2Name = methodInfo.Name;
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.CannotHaveTwoOperationsWithTheSameName3, method1Name, method2Name, reflectionInfo.iface)));
                    }
                    if (!isAsync && !isTask && (existingOp.SyncMethod != null))
                    {
                        string method1Name = existingOp.SyncMethod.Name;
                        string method2Name = methodInfo.Name;
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.CannotHaveTwoOperationsWithTheSameName3, method1Name, method2Name, reflectionInfo.iface)));
                    }

                    contractDescription.Operations.Remove(existingOp);
                    OperationDescription newOp = CreateOperationDescription(contractDescription,
                                                                            methodInfo,
                                                                            direction,
                                                                            reflectionInfo,
                                                                            declaringContract);

                    newOp.HasNoDisposableParameters = ServiceReflector.HasNoDisposableParameters(methodInfo);

                    if (isTask)
                    {
                        existingOp.TaskMethod = newOp.TaskMethod;
                        existingOp.TaskTResult = newOp.TaskTResult;
                        if (existingOp.SyncMethod != null)
                        {
                            // Task vs. Sync 
                            VerifyConsistency(new SyncTaskOperationConsistencyVerifier(existingOp, newOp));
                        }
                        else
                        {
                            // Task vs. Async
                            VerifyConsistency(new TaskAsyncOperationConsistencyVerifier(newOp, existingOp));
                        }
                        return existingOp;
                    }
                    else if (isAsync)
                    {
                        existingOp.BeginMethod = newOp.BeginMethod;
                        existingOp.EndMethod = newOp.EndMethod;
                        if (existingOp.SyncMethod != null)
                        {
                            // Async vs. Sync
                            VerifyConsistency(new SyncAsyncOperationConsistencyVerifier(existingOp, newOp));
                        }
                        else
                        {
                            // Async vs. Task
                            VerifyConsistency(new TaskAsyncOperationConsistencyVerifier(existingOp, newOp));
                        }
                        return existingOp;
                    }
                    else
                    {
                        newOp.BeginMethod = existingOp.BeginMethod;
                        newOp.EndMethod = existingOp.EndMethod;
                        newOp.TaskMethod = existingOp.TaskMethod;
                        newOp.TaskTResult = existingOp.TaskTResult;
                        if (existingOp.TaskMethod != null)
                        {
                            // Sync vs. Task
                            VerifyConsistency(new SyncTaskOperationConsistencyVerifier(newOp, existingOp));
                        }
                        else
                        {
                            // Sync vs. Async
                            VerifyConsistency(new SyncAsyncOperationConsistencyVerifier(newOp, existingOp));
                        }
                        return newOp;
                    }
                }
            }

            OperationDescription operationDescription = new OperationDescription(operationName.EncodedName, declaringContract);
            operationDescription.IsInitiating = opAttr.IsInitiating;
            operationDescription.IsSessionOpenNotificationEnabled = opAttr.IsSessionOpenNotificationEnabled;

            operationDescription.HasNoDisposableParameters = ServiceReflector.HasNoDisposableParameters(methodInfo);

            if (opAttr.HasProtectionLevel)
            {
                throw ExceptionHelper.PlatformNotSupported("security: protectionLevel");
            }

            XmlQualifiedName contractQname = new XmlQualifiedName(declaringContract.Name, declaringContract.Namespace);

            object[] methodAttributes = ServiceReflector.GetCustomAttributes(methodInfo, typeof(FaultContractAttribute), false);

            if (opAttr.IsOneWay && methodAttributes.Length > 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.OneWayAndFaultsIncompatible2, methodInfo.DeclaringType.FullName, operationName.EncodedName)));
            }

            for (int i = 0; i < methodAttributes.Length; i++)
            {
                FaultContractAttribute knownFault = (FaultContractAttribute)methodAttributes[i];
                FaultDescription faultDescription = CreateFaultDescription(knownFault, contractQname, declaringContract.Namespace, operationDescription.XmlName);
                CheckDuplicateFaultContract(operationDescription.Faults, faultDescription, operationName.EncodedName);
                operationDescription.Faults.Add(faultDescription);
            }

            methodAttributes = ServiceReflector.GetCustomAttributes(methodInfo, typeof(ServiceKnownTypeAttribute), false);
            IEnumerable<Type> knownTypes = GetKnownTypes(methodAttributes, methodInfo);
            foreach (Type knownType in knownTypes)
                operationDescription.KnownTypes.Add(knownType);

            MessageDirection requestDirection = direction;
            MessageDirection responseDirection = MessageDirectionHelper.Opposite(direction);

            string requestAction = NamingHelper.GetMessageAction(contractQname,
                                                                   operationDescription.CodeName,
                                                                   opAttr.Action,
                                                                   false);

            string responseAction = NamingHelper.GetMessageAction(contractQname,
                                                                  operationDescription.CodeName,
                                                                  opAttr.ReplyAction,
                                                                  true);
            XmlName wrapperName = operationName;
            XmlName wrapperResponseName = GetBodyWrapperResponseName(operationName);
            string wrapperNamespace = declaringContract.Namespace;

            MessageDescription requestDescription = CreateMessageDescription(methodInfo,
                                                           isAsync,
                                                           isTask,
                                                           null,
                                                           null,
                                                           contractDescription.Namespace,
                                                           requestAction,
                                                           wrapperName,
                                                           wrapperNamespace,
                                                           requestDirection);
            MessageDescription responseDescription = null;
            operationDescription.Messages.Add(requestDescription);
            MethodInfo outputMethod = methodInfo;
            if (isTask)
            {
                operationDescription.TaskMethod = methodInfo;
                operationDescription.TaskTResult = taskTResult;
            }
            else if (!isAsync)
            {
                operationDescription.SyncMethod = methodInfo;
            }
            else
            {
                outputMethod = ServiceReflector.GetEndMethod(methodInfo);
                operationDescription.EndMethod = outputMethod;
                operationDescription.BeginMethod = methodInfo;
            }

            if (!opAttr.IsOneWay)
            {
                XmlName returnValueName = GetReturnValueName(operationName);
                responseDescription = CreateMessageDescription(outputMethod,
                                                                isAsync,
                                                                isTask,
                                                                taskTResult,
                                                                returnValueName,
                                                                contractDescription.Namespace,
                                                                responseAction,
                                                                wrapperResponseName,
                                                                wrapperNamespace,
                                                                responseDirection);
                operationDescription.Messages.Add(responseDescription);
            }
            else
            {
                if ((!isTask && outputMethod.ReturnType != ServiceReflector.VoidType) || (isTask && taskTResult != ServiceReflector.VoidType) ||
                    ServiceReflector.HasOutputParameters(outputMethod, isAsync))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SRServiceModel.ServiceOperationsMarkedWithIsOneWayTrueMust0));
                }

                if (opAttr.ReplyAction != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.OneWayOperationShouldNotSpecifyAReplyAction1, operationName)));
                }
            }

            if (!opAttr.IsOneWay)
            {
                if (responseDescription.IsVoid &&
                    (requestDescription.IsUntypedMessage || requestDescription.IsTypedMessage))
                {
                    responseDescription.Body.WrapperName = responseDescription.Body.WrapperNamespace = null;
                }
                else if (requestDescription.IsVoid &&
                    (responseDescription.IsUntypedMessage || responseDescription.IsTypedMessage))
                {
                    requestDescription.Body.WrapperName = requestDescription.Body.WrapperNamespace = null;
                }
            }
            return operationDescription;
        }

        private void CheckDuplicateFaultContract(FaultDescriptionCollection faultDescriptionCollection, FaultDescription fault, string operationName)
        {
            foreach (FaultDescription existingFault in faultDescriptionCollection)
            {
                if (XmlName.IsNullOrEmpty(existingFault.ElementName) && XmlName.IsNullOrEmpty(fault.ElementName) && existingFault.DetailType == fault.DetailType)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        string.Format(SRServiceModel.SFxFaultContractDuplicateDetailType, operationName, fault.DetailType)));
                if (!XmlName.IsNullOrEmpty(existingFault.ElementName) && !XmlName.IsNullOrEmpty(fault.ElementName) && existingFault.ElementName == fault.ElementName && existingFault.Namespace == fault.Namespace)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(
                        string.Format(SRServiceModel.SFxFaultContractDuplicateElement, operationName, fault.ElementName, fault.Namespace)));
            }
        }

        private FaultDescription CreateFaultDescription(FaultContractAttribute attr,
                                                XmlQualifiedName contractName,
                                                string contractNamespace,
                                                XmlName operationName)
        {
            XmlName faultName = new XmlName(attr.Name ?? NamingHelper.TypeName(attr.DetailType) + FaultSuffix);
            FaultDescription fault = new FaultDescription(NamingHelper.GetMessageAction(contractName, operationName.DecodedName + faultName.DecodedName, attr.Action, false/*isResponse*/));
            if (attr.Name != null)
                fault.SetNameAndElement(faultName);
            else
                fault.SetNameOnly(faultName);
            fault.Namespace = attr.Namespace ?? contractNamespace;
            fault.DetailType = attr.DetailType;
            if (attr.HasProtectionLevel)
            {
                fault.ProtectionLevel = attr.ProtectionLevel;
            }
            return fault;
        }

        private MessageDescription CreateMessageDescription(MethodInfo methodInfo,
                                                           bool isAsync,
                                                           bool isTask,
                                                           Type taskTResult,
                                                           XmlName returnValueName,
                                                           string defaultNS,
                                                           string action,
                                                           XmlName wrapperName,
                                                           string wrapperNamespace,
                                                           MessageDirection direction)
        {
            string methodName = methodInfo.Name;
            MessageDescription messageDescription;
            if (returnValueName == null)
            {
                ParameterInfo[] parameters = ServiceReflector.GetInputParameters(methodInfo, isAsync);
                if (parameters.Length == 1 && parameters[0].ParameterType.IsDefined(typeof(MessageContractAttribute), false))
                {
                    messageDescription = CreateTypedMessageDescription(parameters[0].ParameterType,
                                                                null,
                                                                null,
                                                                defaultNS,
                                                                action,
                                                                direction);
                }
                else
                {
                    messageDescription = CreateParameterMessageDescription(parameters,
                                                             null,
                                                             null,
                                                             null,
                                                             methodName,
                                                             defaultNS,
                                                             action,
                                                             wrapperName,
                                                             wrapperNamespace,
                                                             direction);
                }
            }
            else
            {
                ParameterInfo[] parameters = ServiceReflector.GetOutputParameters(methodInfo, isAsync);
                Type responseType = isTask ? taskTResult : methodInfo.ReturnType;
                if (responseType.IsDefined(typeof(MessageContractAttribute), false) && parameters.Length == 0)
                {
                    messageDescription = CreateTypedMessageDescription(responseType,
                                                         methodInfo.ReturnParameter,
                                                         returnValueName,
                                                         defaultNS,
                                                         action,
                                                         direction);
                }
                else
                {
                    messageDescription = CreateParameterMessageDescription(parameters,
                                                         responseType,
                                                         methodInfo.ReturnParameter,
                                                         returnValueName,
                                                         methodName,
                                                         defaultNS,
                                                         action,
                                                         wrapperName,
                                                         wrapperNamespace,
                                                         direction);
                }
            }

            bool hasUnknownHeaders = false;
            for (int i = 0; i < messageDescription.Headers.Count; i++)
            {
                MessageHeaderDescription header = messageDescription.Headers[i];
                if (header.IsUnknownHeaderCollection)
                {
                    if (hasUnknownHeaders)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxMultipleUnknownHeaders, methodInfo, methodInfo.DeclaringType)));
                    }
                    else
                    {
                        hasUnknownHeaders = true;
                    }
                }
            }
            return messageDescription;
        }

        private MessageDescription CreateParameterMessageDescription(ParameterInfo[] parameters,
                                                  Type returnType,
                                                  CustomAttributeProvider returnAttrProvider,
                                                  XmlName returnValueName,
                                                  string methodName,
                                                  string defaultNS,
                                                  string action,
                                                  XmlName wrapperName,
                                                  string wrapperNamespace,
                                                  MessageDirection direction)
        {
            foreach (ParameterInfo param in parameters)
            {
                if (GetParameterType(param).IsDefined(typeof(MessageContractAttribute), false/*inherit*/))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxInvalidMessageContractSignature, methodName)));
                }
            }
            if (returnType != null && returnType.IsDefined(typeof(MessageContractAttribute), false/*inherit*/))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxInvalidMessageContractSignature, methodName)));
            }

            MessageDescription messageDescription = new MessageDescription(action, direction);
            MessagePartDescriptionCollection partDescriptionCollection = messageDescription.Body.Parts;
            for (int index = 0; index < parameters.Length; index++)
            {
                MessagePartDescription partDescription = CreateParameterPartDescription(new XmlName(parameters[index].Name), defaultNS, index, parameters[index], GetParameterType(parameters[index]));
                if (partDescriptionCollection.Contains(new XmlQualifiedName(partDescription.Name, partDescription.Namespace)))
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidMessageContractException(string.Format(SRServiceModel.SFxDuplicateMessageParts, partDescription.Name, partDescription.Namespace)));
                messageDescription.Body.Parts.Add(partDescription);
            }

            if (returnType != null)
            {
                messageDescription.Body.ReturnValue = CreateParameterPartDescription(returnValueName, defaultNS, 0, returnAttrProvider, returnType);
            }
            if (messageDescription.IsUntypedMessage)
            {
                messageDescription.Body.WrapperName = null;
                messageDescription.Body.WrapperNamespace = null;
            }
            else
            {
                messageDescription.Body.WrapperName = wrapperName.EncodedName;
                messageDescription.Body.WrapperNamespace = wrapperNamespace;
            }

            return messageDescription;
        }

        private static MessagePartDescription CreateParameterPartDescription(XmlName defaultName, string defaultNS, int index, CustomAttributeProvider attrProvider, Type type)
        {
            MessagePartDescription parameterPart;
            MessageParameterAttribute paramAttr = ServiceReflector.GetSingleAttribute<MessageParameterAttribute>(attrProvider);

            XmlName name = paramAttr == null || !paramAttr.IsNameSetExplicit ? defaultName : new XmlName(paramAttr.Name);
            parameterPart = new MessagePartDescription(name.EncodedName, defaultNS);
            parameterPart.Type = type;
            parameterPart.Index = index;
            parameterPart.AdditionalAttributesProvider = attrProvider;
            return parameterPart;
        }

        [SuppressMessage(FxCop.Category.Usage, "CA2301:EmbeddableTypesInContainersRule", MessageId = "messages", Justification = "No need to support type equivalence here.")]
        internal MessageDescription CreateTypedMessageDescription(Type typedMessageType,
                                                  CustomAttributeProvider returnAttrProvider,
                                                  XmlName returnValueName,
                                                  string defaultNS,
                                                  string action,
                                                  MessageDirection direction)
        {
            MessageDescription messageDescription;
            bool messageItemsInitialized = false;
            MessageDescriptionItems messageItems;
            MessageContractAttribute messageContractAttribute = ServiceReflector.GetSingleAttribute<MessageContractAttribute>(typedMessageType);
            if (_messages.TryGetValue(typedMessageType, out messageItems))
            {
                messageDescription = new MessageDescription(action, direction, messageItems);
                messageItemsInitialized = true;
            }
            else
                messageDescription = new MessageDescription(action, direction, null);
            messageDescription.MessageType = typedMessageType;
            messageDescription.MessageName = new XmlName(NamingHelper.TypeName(typedMessageType));
            if (messageContractAttribute.IsWrapped)
            {
                messageDescription.Body.WrapperName = GetWrapperName(messageContractAttribute.WrapperName, messageDescription.MessageName).EncodedName;
                messageDescription.Body.WrapperNamespace = messageContractAttribute.WrapperNamespace ?? defaultNS;
            }
            List<MemberInfo> contractMembers = new List<MemberInfo>();

            for (Type baseType = typedMessageType; baseType != null && baseType != typeof(object) && baseType != typeof(ValueType); baseType = baseType.BaseType())
            {
                if (!baseType.IsDefined(typeof(MessageContractAttribute), false/*inherit*/))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxMessageContractBaseTypeNotValid, baseType, typedMessageType)));
                }
                if (!messageDescription.HasProtectionLevel)
                {
                    MessageContractAttribute mca = ServiceReflector.GetRequiredSingleAttribute<MessageContractAttribute>(baseType);
                    if (mca.HasProtectionLevel)
                    {
                        messageDescription.ProtectionLevel = mca.ProtectionLevel;
                    }
                }

                if (messageItemsInitialized)
                    continue;
                foreach (MemberInfo memberInfo in baseType.GetTypeInfo().DeclaredMembers)
                {
                    if (!(memberInfo is FieldInfo || memberInfo is PropertyInfo))
                    {
                        continue;
                    }
                    PropertyInfo property = memberInfo as PropertyInfo;
                    if (property != null)
                    {
                        MethodInfo getMethod = property.GetGetMethod(true);
                        if (getMethod != null && (getMethod.IsStatic || IsMethodOverriding(getMethod)))
                        {
                            continue;
                        }
                        MethodInfo setMethod = property.GetSetMethod(true);
                        if (setMethod != null && (setMethod.IsStatic || IsMethodOverriding(setMethod)))
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Contract.Assert(memberInfo is FieldInfo);
                        if (((FieldInfo)memberInfo).IsStatic)
                        {
                            continue;
                        }
                    }


                    if (memberInfo.IsDefined(typeof(MessageBodyMemberAttribute), false) ||
                        memberInfo.IsDefined(typeof(MessageHeaderAttribute), false)
                        )
                    {
                        contractMembers.Add(memberInfo);
                    }
                }
            }

            if (messageItemsInitialized)
                return messageDescription;

            List<MessagePartDescription> bodyPartDescriptionList = new List<MessagePartDescription>();
            List<MessageHeaderDescription> headerPartDescriptionList = new List<MessageHeaderDescription>();
            for (int i = 0; i < contractMembers.Count; i++)
            {
                MemberInfo memberInfo = contractMembers[i];

                Type memberType;
                if (memberInfo is PropertyInfo)
                {
                    memberType = ((PropertyInfo)memberInfo).PropertyType;
                }
                else
                {
                    memberType = ((FieldInfo)memberInfo).FieldType;
                }

                if (memberInfo.IsDefined(typeof(MessageHeaderAttribute), false))
                {
                    headerPartDescriptionList.Add(CreateMessageHeaderDescription(memberType,
                                                                              memberInfo,
                                                                              new XmlName(memberInfo.Name),
                                                                              defaultNS,
                                                                              i,
                                                                              -1));
                }
                else
                {
                    bodyPartDescriptionList.Add(CreateMessagePartDescription(memberType,
                                                                            memberInfo,
                                                                            new XmlName(memberInfo.Name),
                                                                            defaultNS,
                                                                            i,
                                                                            -1));
                }
            }

            if (returnAttrProvider != null)
            {
                messageDescription.Body.ReturnValue = CreateMessagePartDescription(typeof(void),
                                                                  returnAttrProvider,
                                                                  returnValueName,
                                                                  defaultNS,
                                                                  0,
                                                                  0);
            }

            AddSortedParts<MessagePartDescription>(bodyPartDescriptionList, messageDescription.Body.Parts);
            AddSortedParts<MessageHeaderDescription>(headerPartDescriptionList, messageDescription.Headers);
            _messages.Add(typedMessageType, messageDescription.Items);

            return messageDescription;
        }

        private static bool IsMethodOverriding(MethodInfo method)
        {
            return method.IsVirtual && ((method.Attributes & MethodAttributes.NewSlot) == 0);
        }




        private MessagePartDescription CreateMessagePartDescription(Type bodyType,
                                                         CustomAttributeProvider attrProvider,
                                                         XmlName defaultName,
                                                         string defaultNS,
                                                         int parameterIndex,
                                                         int serializationIndex)
        {
            MessagePartDescription partDescription = null;
            MessageBodyMemberAttribute bodyAttr = ServiceReflector.GetSingleAttribute<MessageBodyMemberAttribute>(attrProvider, s_messageContractMemberAttributes);

            if (bodyAttr == null)
            {
                partDescription = new MessagePartDescription(defaultName.EncodedName, defaultNS);
                partDescription.SerializationPosition = serializationIndex;
            }
            else
            {
                XmlName partName = bodyAttr.IsNameSetExplicit ? new XmlName(bodyAttr.Name) : defaultName;
                string partNs = bodyAttr.IsNamespaceSetExplicit ? bodyAttr.Namespace : defaultNS;
                partDescription = new MessagePartDescription(partName.EncodedName, partNs);
                partDescription.SerializationPosition = bodyAttr.Order < 0 ? serializationIndex : bodyAttr.Order;
                if (bodyAttr.HasProtectionLevel)
                {
                    partDescription.ProtectionLevel = bodyAttr.ProtectionLevel;
                }
            }

            if (attrProvider.MemberInfo != null)
            {
                partDescription.MemberInfo = attrProvider.MemberInfo;
            }
            partDescription.Type = bodyType;
            partDescription.Index = parameterIndex;
            return partDescription;
        }

        private MessageHeaderDescription CreateMessageHeaderDescription(Type headerParameterType,
                                                                    CustomAttributeProvider attrProvider,
                                                                    XmlName defaultName,
                                                                    string defaultNS,
                                                                    int parameterIndex,
                                                                    int serializationPosition)
        {
            MessageHeaderDescription headerDescription = null;
            MessageHeaderAttribute headerAttr = ServiceReflector.GetRequiredSingleAttribute<MessageHeaderAttribute>(attrProvider, s_messageContractMemberAttributes);
            XmlName headerName = headerAttr.IsNameSetExplicit ? new XmlName(headerAttr.Name) : defaultName;
            string headerNs = headerAttr.IsNamespaceSetExplicit ? headerAttr.Namespace : defaultNS;
            headerDescription = new MessageHeaderDescription(headerName.EncodedName, headerNs);
            headerDescription.UniquePartName = defaultName.EncodedName;

            // check on MessageHeaderArrayAttribute is omitted.

            headerDescription.Type = TypedHeaderManager.GetHeaderType(headerParameterType);
            headerDescription.TypedHeader = (headerParameterType != headerDescription.Type);
            if (headerDescription.TypedHeader)
            {
                if (headerAttr.IsMustUnderstandSet || headerAttr.IsRelaySet || headerAttr.Actor != null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(string.Format(SRServiceModel.SFxStaticMessageHeaderPropertiesNotAllowed, defaultName)));
                }
            }
            else
            {
                headerDescription.Actor = headerAttr.Actor;
                headerDescription.MustUnderstand = headerAttr.MustUnderstand;
                headerDescription.Relay = headerAttr.Relay;
            }
            headerDescription.SerializationPosition = serializationPosition;
            if (headerAttr.HasProtectionLevel)
            {
                headerDescription.ProtectionLevel = headerAttr.ProtectionLevel;
            }
            if (attrProvider.MemberInfo != null)
            {
                headerDescription.MemberInfo = attrProvider.MemberInfo;
            }

            headerDescription.Index = parameterIndex;
            return headerDescription;
        }

        private MessagePropertyDescription CreateMessagePropertyDescription(CustomAttributeProvider attrProvider,
                                                            XmlName defaultName,
                                                            int parameterIndex)
        {
            XmlName propertyName = defaultName;
            MessagePropertyDescription propertyDescription = new MessagePropertyDescription(propertyName.EncodedName);
            propertyDescription.Index = parameterIndex;

            if (attrProvider.MemberInfo != null)
            {
                propertyDescription.MemberInfo = attrProvider.MemberInfo;
            }

            return propertyDescription;
        }

        internal static XmlName GetReturnValueName(XmlName methodName)
        {
            return new XmlName(methodName.EncodedName + ReturnSuffix, true);
        }

        internal static XmlName GetReturnValueName(string methodName)
        {
            return new XmlName(methodName + ReturnSuffix);
        }

        internal static Type GetParameterType(ParameterInfo parameterInfo)
        {
            Type parameterType = parameterInfo.ParameterType;
            if (parameterType.IsByRef)
            {
                return parameterType.GetElementType();
            }
            else
            {
                return parameterType;
            }
        }

        internal static XmlName GetWrapperName(string wrapperName, XmlName defaultName)
        {
            if (string.IsNullOrEmpty(wrapperName))
                return defaultName;
            return new XmlName(wrapperName);
        }

        private void AddSortedParts<T>(List<T> partDescriptionList, KeyedCollection<XmlQualifiedName, T> partDescriptionCollection)
            where T : MessagePartDescription
        {
            MessagePartDescription[] partDescriptions = partDescriptionList.ToArray();
            if (partDescriptions.Length > 1)
            {
                Array.Sort<MessagePartDescription>(partDescriptions, CompareMessagePartDescriptions);
            }
            foreach (T partDescription in partDescriptions)
            {
                if (partDescriptionCollection.Contains(new XmlQualifiedName(partDescription.Name, partDescription.Namespace)))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidMessageContractException(string.Format(SRServiceModel.SFxDuplicateMessageParts, partDescription.Name, partDescription.Namespace)));
                }
                partDescriptionCollection.Add(partDescription);
            }
        }

        private abstract class OperationConsistencyVerifier
        {
            public virtual void VerifyParameterLength() { }
            public virtual void VerifyParameterType() { }
            public virtual void VerifyOutParameterType() { }
            public virtual void VerifyReturnType() { }
            public virtual void VerifyFaultContractAttribute() { }
            public virtual void VerifyKnownTypeAttribute() { }
            public virtual void VerifyIsOneWayStatus() { }
            public virtual void VerifyActionAndReplyAction() { }
        }

        private class SyncAsyncOperationConsistencyVerifier : OperationConsistencyVerifier
        {
            private OperationDescription _syncOperation;
            private OperationDescription _asyncOperation;
            private ParameterInfo[] _syncInputs;
            private ParameterInfo[] _asyncInputs;
            private ParameterInfo[] _syncOutputs;
            private ParameterInfo[] _asyncOutputs;

            public SyncAsyncOperationConsistencyVerifier(OperationDescription syncOperation, OperationDescription asyncOperation)
            {
                _syncOperation = syncOperation;
                _asyncOperation = asyncOperation;
                _syncInputs = ServiceReflector.GetInputParameters(_syncOperation.SyncMethod, false);
                _asyncInputs = ServiceReflector.GetInputParameters(_asyncOperation.BeginMethod, true);
                _syncOutputs = ServiceReflector.GetOutputParameters(_syncOperation.SyncMethod, false);
                _asyncOutputs = ServiceReflector.GetOutputParameters(_asyncOperation.EndMethod, true);
            }

            public override void VerifyParameterLength()
            {
                if (_syncInputs.Length != _asyncInputs.Length || _syncOutputs.Length != _asyncOutputs.Length)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new InvalidOperationException(string.Format(SRServiceModel.SyncAsyncMatchConsistency_Parameters5,
                                                                   _syncOperation.SyncMethod.Name,
                                                                   _syncOperation.SyncMethod.DeclaringType,
                                                                   _asyncOperation.BeginMethod.Name,
                                                                   _asyncOperation.EndMethod.Name,
                                                                   _syncOperation.Name)));
                }
            }

            public override void VerifyParameterType()
            {
                for (int i = 0; i < _syncInputs.Length; i++)
                {
                    if (_syncInputs[i].ParameterType != _asyncInputs[i].ParameterType)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(string.Format(SRServiceModel.SyncAsyncMatchConsistency_Parameters5,
                                                                       _syncOperation.SyncMethod.Name,
                                                                       _syncOperation.SyncMethod.DeclaringType,
                                                                       _asyncOperation.BeginMethod.Name,
                                                                       _asyncOperation.EndMethod.Name,
                                                                       _syncOperation.Name)));
                    }
                }
            }

            public override void VerifyOutParameterType()
            {
                for (int i = 0; i < _syncOutputs.Length; i++)
                {
                    if (_syncOutputs[i].ParameterType != _asyncOutputs[i].ParameterType)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(string.Format(SRServiceModel.SyncAsyncMatchConsistency_Parameters5,
                                                                       _syncOperation.SyncMethod.Name,
                                                                       _syncOperation.SyncMethod.DeclaringType,
                                                                       _asyncOperation.BeginMethod.Name,
                                                                       _asyncOperation.EndMethod.Name,
                                                                       _syncOperation.Name)));
                    }
                }
            }

            public override void VerifyReturnType()
            {
                if (_syncOperation.SyncMethod.ReturnType != _syncOperation.EndMethod.ReturnType)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new InvalidOperationException(string.Format(SRServiceModel.SyncAsyncMatchConsistency_ReturnType5,
                                                                   _syncOperation.SyncMethod.Name,
                                                                   _syncOperation.SyncMethod.DeclaringType,
                                                                   _asyncOperation.BeginMethod.Name,
                                                                   _asyncOperation.EndMethod.Name,
                                                                   _syncOperation.Name)));
                }
            }

            public override void VerifyFaultContractAttribute()
            {
                if (_asyncOperation.Faults.Count != 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new InvalidOperationException(string.Format(SRServiceModel.SyncAsyncMatchConsistency_Attributes6,
                                                                   _syncOperation.SyncMethod.Name,
                                                                   _syncOperation.SyncMethod.DeclaringType,
                                                                   _asyncOperation.BeginMethod.Name,
                                                                   _asyncOperation.EndMethod.Name,
                                                                   _syncOperation.Name,
                                                                   typeof(FaultContractAttribute).Name)));
                }
            }

            public override void VerifyKnownTypeAttribute()
            {
                if (_asyncOperation.KnownTypes.Count != 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new InvalidOperationException(string.Format(SRServiceModel.SyncAsyncMatchConsistency_Attributes6,
                                                                   _syncOperation.SyncMethod.Name,
                                                                   _syncOperation.SyncMethod.DeclaringType,
                                                                   _asyncOperation.BeginMethod.Name,
                                                                   _asyncOperation.EndMethod.Name,
                                                                   _syncOperation.Name,
                                                                   typeof(ServiceKnownTypeAttribute).Name)));
                }
            }

            public override void VerifyIsOneWayStatus()
            {
                if (_syncOperation.Messages.Count != _asyncOperation.Messages.Count)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(string.Format(SRServiceModel.SyncAsyncMatchConsistency_Property6,
                                                                       _syncOperation.SyncMethod.Name,
                                                                       _syncOperation.SyncMethod.DeclaringType,
                                                                       _asyncOperation.BeginMethod.Name,
                                                                       _asyncOperation.EndMethod.Name,
                                                                       _syncOperation.Name,
                                                                       "IsOneWay")));
                }
            }

            public override void VerifyActionAndReplyAction()
            {
                for (int index = 0; index < _syncOperation.Messages.Count; ++index)
                {
                    if (_syncOperation.Messages[index].Action != _asyncOperation.Messages[index].Action)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(string.Format(SRServiceModel.SyncAsyncMatchConsistency_Property6,
                                                                       _syncOperation.SyncMethod.Name,
                                                                       _syncOperation.SyncMethod.DeclaringType,
                                                                       _asyncOperation.BeginMethod.Name,
                                                                       _asyncOperation.EndMethod.Name,
                                                                       _syncOperation.Name,
                                                                       index == 0 ? "Action" : "ReplyAction")));
                    }
                }
            }
        }

        private class SyncTaskOperationConsistencyVerifier : OperationConsistencyVerifier
        {
            private OperationDescription _syncOperation;
            private OperationDescription _taskOperation;
            private ParameterInfo[] _syncInputs;
            private ParameterInfo[] _taskInputs;

            public SyncTaskOperationConsistencyVerifier(OperationDescription syncOperation, OperationDescription taskOperation)
            {
                _syncOperation = syncOperation;
                _taskOperation = taskOperation;
                _syncInputs = ServiceReflector.GetInputParameters(_syncOperation.SyncMethod, false);
                _taskInputs = ServiceReflector.GetInputParameters(_taskOperation.TaskMethod, false);
            }

            public override void VerifyParameterLength()
            {
                if (_syncInputs.Length != _taskInputs.Length)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new InvalidOperationException(string.Format(SRServiceModel.SyncTaskMatchConsistency_Parameters5,
                                                                   _syncOperation.SyncMethod.Name,
                                                                   _syncOperation.SyncMethod.DeclaringType,
                                                                   _taskOperation.TaskMethod.Name,
                                                                   _syncOperation.Name)));
                }
            }

            public override void VerifyParameterType()
            {
                for (int i = 0; i < _syncInputs.Length; i++)
                {
                    if (_syncInputs[i].ParameterType != _taskInputs[i].ParameterType)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(string.Format(SRServiceModel.SyncTaskMatchConsistency_Parameters5,
                                                                       _syncOperation.SyncMethod.Name,
                                                                       _syncOperation.SyncMethod.DeclaringType,
                                                                       _taskOperation.TaskMethod.Name,
                                                                       _syncOperation.Name)));
                    }
                }
            }

            public override void VerifyReturnType()
            {
                if (_syncOperation.SyncMethod.ReturnType != _syncOperation.TaskTResult)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new InvalidOperationException(string.Format(SRServiceModel.SyncTaskMatchConsistency_ReturnType5,
                                                                   _syncOperation.SyncMethod.Name,
                                                                   _syncOperation.SyncMethod.DeclaringType,
                                                                   _taskOperation.TaskMethod.Name,
                                                                   _syncOperation.Name)));
                }
            }

            public override void VerifyFaultContractAttribute()
            {
                if (_taskOperation.Faults.Count != 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new InvalidOperationException(string.Format(SRServiceModel.SyncTaskMatchConsistency_Attributes6,
                                                                   _syncOperation.SyncMethod.Name,
                                                                   _syncOperation.SyncMethod.DeclaringType,
                                                                   _taskOperation.TaskMethod.Name,
                                                                   _syncOperation.Name,
                                                                   typeof(FaultContractAttribute).Name)));
                }
            }

            public override void VerifyKnownTypeAttribute()
            {
                if (_taskOperation.KnownTypes.Count != 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new InvalidOperationException(string.Format(SRServiceModel.SyncTaskMatchConsistency_Attributes6,
                                                                   _syncOperation.SyncMethod.Name,
                                                                   _syncOperation.SyncMethod.DeclaringType,
                                                                   _taskOperation.TaskMethod.Name,
                                                                   _syncOperation.Name,
                                                                   typeof(ServiceKnownTypeAttribute).Name)));
                }
            }

            public override void VerifyIsOneWayStatus()
            {
                if (_syncOperation.Messages.Count != _taskOperation.Messages.Count)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(string.Format(SRServiceModel.SyncTaskMatchConsistency_Property6,
                                                                       _syncOperation.SyncMethod.Name,
                                                                       _syncOperation.SyncMethod.DeclaringType,
                                                                       _taskOperation.TaskMethod.Name,
                                                                       _syncOperation.Name,
                                                                       "IsOneWay")));
                }
            }

            public override void VerifyActionAndReplyAction()
            {
                for (int index = 0; index < _syncOperation.Messages.Count; ++index)
                {
                    if (_syncOperation.Messages[index].Action != _taskOperation.Messages[index].Action)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(string.Format(SRServiceModel.SyncTaskMatchConsistency_Property6,
                                                                       _syncOperation.SyncMethod.Name,
                                                                       _syncOperation.SyncMethod.DeclaringType,
                                                                       _taskOperation.TaskMethod.Name,
                                                                       _syncOperation.Name,
                                                                       index == 0 ? "Action" : "ReplyAction")));
                    }
                }
            }
        }

        private class TaskAsyncOperationConsistencyVerifier : OperationConsistencyVerifier
        {
            private OperationDescription _taskOperation;
            private OperationDescription _asyncOperation;
            private ParameterInfo[] _taskInputs;
            private ParameterInfo[] _asyncInputs;

            public TaskAsyncOperationConsistencyVerifier(OperationDescription taskOperation, OperationDescription asyncOperation)
            {
                _taskOperation = taskOperation;
                _asyncOperation = asyncOperation;
                _taskInputs = ServiceReflector.GetInputParameters(_taskOperation.TaskMethod, false);
                _asyncInputs = ServiceReflector.GetInputParameters(_asyncOperation.BeginMethod, true);
            }

            public override void VerifyParameterLength()
            {
                if (_taskInputs.Length != _asyncInputs.Length)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new InvalidOperationException(string.Format(SRServiceModel.TaskAsyncMatchConsistency_Parameters5,
                                                                   _taskOperation.TaskMethod.Name,
                                                                   _taskOperation.TaskMethod.DeclaringType,
                                                                   _asyncOperation.BeginMethod.Name,
                                                                   _asyncOperation.EndMethod.Name,
                                                                   _taskOperation.Name)));
                }
            }

            public override void VerifyParameterType()
            {
                for (int i = 0; i < _taskInputs.Length; i++)
                {
                    if (_taskInputs[i].ParameterType != _asyncInputs[i].ParameterType)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(string.Format(SRServiceModel.TaskAsyncMatchConsistency_Parameters5,
                                                                       _taskOperation.TaskMethod.Name,
                                                                       _taskOperation.TaskMethod.DeclaringType,
                                                                       _asyncOperation.BeginMethod.Name,
                                                                       _asyncOperation.EndMethod.Name,
                                                                       _taskOperation.Name)));
                    }
                }
            }

            public override void VerifyReturnType()
            {
                if (_taskOperation.TaskTResult != _asyncOperation.EndMethod.ReturnType)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new InvalidOperationException(string.Format(SRServiceModel.TaskAsyncMatchConsistency_ReturnType5,
                                                                   _taskOperation.TaskMethod.Name,
                                                                   _taskOperation.TaskMethod.DeclaringType,
                                                                   _asyncOperation.BeginMethod.Name,
                                                                   _asyncOperation.EndMethod.Name,
                                                                   _taskOperation.Name)));
                }
            }

            public override void VerifyFaultContractAttribute()
            {
                if (_asyncOperation.Faults.Count != 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new InvalidOperationException(string.Format(SRServiceModel.TaskAsyncMatchConsistency_Attributes6,
                                                                   _taskOperation.TaskMethod.Name,
                                                                   _taskOperation.TaskMethod.DeclaringType,
                                                                   _asyncOperation.BeginMethod.Name,
                                                                   _asyncOperation.EndMethod.Name,
                                                                   _taskOperation.Name,
                                                                   typeof(FaultContractAttribute).Name)));
                }
            }

            public override void VerifyKnownTypeAttribute()
            {
                if (_asyncOperation.KnownTypes.Count != 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                        new InvalidOperationException(string.Format(SRServiceModel.TaskAsyncMatchConsistency_Attributes6,
                                                                   _taskOperation.TaskMethod.Name,
                                                                   _taskOperation.TaskMethod.DeclaringType,
                                                                   _asyncOperation.BeginMethod.Name,
                                                                   _asyncOperation.EndMethod.Name,
                                                                   _taskOperation.Name,
                                                                   typeof(ServiceKnownTypeAttribute).Name)));
                }
            }

            public override void VerifyIsOneWayStatus()
            {
                if (_taskOperation.Messages.Count != _asyncOperation.Messages.Count)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(string.Format(SRServiceModel.TaskAsyncMatchConsistency_Property6,
                                                                       _taskOperation.TaskMethod.Name,
                                                                       _taskOperation.TaskMethod.DeclaringType,
                                                                       _asyncOperation.BeginMethod.Name,
                                                                       _asyncOperation.EndMethod.Name,
                                                                       _taskOperation.Name,
                                                                       "IsOneWay")));
                }
            }

            public override void VerifyActionAndReplyAction()
            {
                for (int index = 0; index < _taskOperation.Messages.Count; ++index)
                {
                    if (_taskOperation.Messages[index].Action != _asyncOperation.Messages[index].Action)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(
                            new InvalidOperationException(string.Format(SRServiceModel.TaskAsyncMatchConsistency_Property6,
                                                                       _taskOperation.TaskMethod.Name,
                                                                       _taskOperation.TaskMethod.DeclaringType,
                                                                       _asyncOperation.BeginMethod.Name,
                                                                       _asyncOperation.EndMethod.Name,
                                                                       _taskOperation.Name,
                                                                       index == 0 ? "Action" : "ReplyAction")));
                    }
                }
            }
        }

        private class ContractReflectionInfo
        {
            internal Type iface;
            internal Type callbackiface;
        }

        // This function factors out the logic of how programming model attributes interact with service inheritance.
        // See MB 37427 for details.
        //
        // To use this, just call ApplyServiceInheritance() with
        //  - the service type you want to pull behavior attributes from
        //  - the "destination" behavior collection, where all the right behavior attributes should be added to
        //  - a delegate
        // The delegate is just a function you write that behaves like this: 
        //    imagine that "currentType" was the only type (imagine there was no inheritance hierarchy)
        //    find desired behavior attributes on this type, and add them to "behaviors"
        // ApplyServiceInheritance then uses the logic you provide for getting behavior attributes from a single type, 
        // and it walks the actual type hierarchy and does the inheritance/override logic for you.
        public static void ApplyServiceInheritance<IBehavior, TBehaviorCollection>(
                     Type serviceType,
                     TBehaviorCollection descriptionBehaviors,
                     ServiceInheritanceCallback<IBehavior, TBehaviorCollection> callback)
            where IBehavior : class
            where TBehaviorCollection : KeyedByTypeCollection<IBehavior>
        {
            // work our way up the class hierarchy, looking for attributes; adding "bottom up" so that for each
            // type of attribute, we only pick up the bottom-most one (the one attached to most-derived class)
            for (Type currentType = serviceType; currentType != null; currentType = currentType.BaseType())
            {
                AddBehaviorsAtOneScope(currentType, descriptionBehaviors, callback);
            }
        }

        public delegate void ServiceInheritanceCallback<IBehavior, TBehaviorCollection>(Type currentType, KeyedByTypeCollection<IBehavior> behaviors);

        // To use this, just call AddBehaviorsAtOneScope() with
        //  - the type you want to pull behavior attributes from
        //  - the "destination" behavior collection, where all the right behavior attributes should be added to
        //  - a delegate
        // The delegate is just a function you write that behaves like this: 
        //    imagine that "currentType" was the only type (imagine there was no inheritance hierarchy)
        //    find desired behavior attributes on this type, and add them to "behaviors"
        // AddBehaviorsAtOneScope then uses the logic you provide for getting behavior attributes from a single type, 
        // and it does the override logic for you (only add the behavior if it wasn't already in the descriptionBehaviors)
        private static void AddBehaviorsAtOneScope<IBehavior, TBehaviorCollection>(
                     Type type,
                     TBehaviorCollection descriptionBehaviors,
                     ServiceInheritanceCallback<IBehavior, TBehaviorCollection> callback)
            where IBehavior : class
            where TBehaviorCollection : KeyedByTypeCollection<IBehavior>
        {
            KeyedByTypeCollection<IBehavior> toAdd = new KeyedByTypeCollection<IBehavior>();
            callback(type, toAdd);
            // toAdd now contains the set of behaviors we'd add if this type (scope) were the only source of behaviors

            for (int i = 0; i < toAdd.Count; i++)
            {
                IBehavior behavior = toAdd[i];
                if (!descriptionBehaviors.Contains(behavior.GetType()))
                {
                    // 
                    throw ExceptionHelper.PlatformNotSupported();
                }
            }
        }
    }
}
