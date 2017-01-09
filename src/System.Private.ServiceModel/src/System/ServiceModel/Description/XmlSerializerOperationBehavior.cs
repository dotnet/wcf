// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//-----------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime;
using System.Runtime.Serialization;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
using System.Xml.Serialization;

namespace System.ServiceModel.Description
{
    public class XmlSerializerOperationBehavior : IOperationBehavior
    {
        private readonly Reflector.OperationReflector _reflector;
        private readonly bool _builtInOperationBehavior;

        public XmlSerializerOperationBehavior(OperationDescription operation)
            : this(operation, null)
        {
        }

        public XmlSerializerOperationBehavior(OperationDescription operation, XmlSerializerFormatAttribute attribute)
        {
            if (operation == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("operation");
#pragma warning suppress 56506 // Declaring contract cannot be null
            Reflector parentReflector = new Reflector(operation.DeclaringContract.Namespace, operation.DeclaringContract.ContractType);
#pragma warning suppress 56506 // parentReflector cannot be null
            _reflector = parentReflector.ReflectOperation(operation, attribute ?? new XmlSerializerFormatAttribute());
        }

        internal XmlSerializerOperationBehavior(OperationDescription operation, XmlSerializerFormatAttribute attribute, Reflector parentReflector)
            : this(operation, attribute)
        {
            // used by System.ServiceModel.Web
            _reflector = parentReflector.ReflectOperation(operation, attribute ?? new XmlSerializerFormatAttribute());
        }

        private XmlSerializerOperationBehavior(Reflector.OperationReflector reflector, bool builtInOperationBehavior)
        {
            Fx.Assert(reflector != null, "");
            _reflector = reflector;
            _builtInOperationBehavior = builtInOperationBehavior;
        }

        internal Reflector.OperationReflector OperationReflector
        {
            get { return _reflector; }
        }

        internal bool IsBuiltInOperationBehavior
        {
            get { return _builtInOperationBehavior; }
        }

        public XmlSerializerFormatAttribute XmlSerializerFormatAttribute
        {
            get
            {
                return _reflector.Attribute;
            }
        }

        internal static XmlSerializerOperationFormatter CreateOperationFormatter(OperationDescription operation)
        {
            return new XmlSerializerOperationBehavior(operation).CreateFormatter();
        }

        internal static XmlSerializerOperationFormatter CreateOperationFormatter(OperationDescription operation, XmlSerializerFormatAttribute attr)
        {
            return new XmlSerializerOperationBehavior(operation, attr).CreateFormatter();
        }

        internal static void AddBehaviors(ContractDescription contract)
        {
            AddBehaviors(contract, false);
        }

        internal static void AddBuiltInBehaviors(ContractDescription contract)
        {
            AddBehaviors(contract, true);
        }

        private static void AddBehaviors(ContractDescription contract, bool builtInOperationBehavior)
        {
            Reflector reflector = new Reflector(contract.Namespace, contract.ContractType);

            foreach (OperationDescription operation in contract.Operations)
            {
                Reflector.OperationReflector operationReflector = reflector.ReflectOperation(operation);
                if (operationReflector != null)
                {
                    bool isInherited = operation.DeclaringContract != contract;
                    if (!isInherited)
                    {
                        operation.Behaviors.Add(new XmlSerializerOperationBehavior(operationReflector, builtInOperationBehavior));
                    }
                }
            }
        }

        internal XmlSerializerOperationFormatter CreateFormatter()
        {
            return new XmlSerializerOperationFormatter(_reflector.Operation, _reflector.Attribute, _reflector.Request, _reflector.Reply);
        }

        private XmlSerializerFaultFormatter CreateFaultFormatter(SynchronizedCollection<FaultContractInfo> faultContractInfos)
        {
            return new XmlSerializerFaultFormatter(faultContractInfos, _reflector.XmlSerializerFaultContractInfos);
        }

        void IOperationBehavior.Validate(OperationDescription description)
        {
        }

        void IOperationBehavior.AddBindingParameters(OperationDescription description, BindingParameterCollection parameters)
        {
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription description, DispatchOperation dispatch)
        {
            if (description == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("description");

            if (dispatch == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("dispatch");

            if (dispatch.Formatter == null)
            {
                dispatch.Formatter = (IDispatchMessageFormatter)CreateFormatter();
                dispatch.DeserializeRequest = _reflector.RequestRequiresSerialization;
                dispatch.SerializeReply = _reflector.ReplyRequiresSerialization;
            }

            if (_reflector.Attribute.SupportFaults && !dispatch.IsFaultFormatterSetExplicit)
            {
                dispatch.FaultFormatter = (IDispatchFaultFormatter)CreateFaultFormatter(dispatch.FaultContractInfos);
            }
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription description, ClientOperation proxy)
        {
            if (description == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("description");

            if (proxy == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("proxy");

            if (proxy.Formatter == null)
            {
                proxy.Formatter = (IClientMessageFormatter)CreateFormatter();
                proxy.SerializeRequest = _reflector.RequestRequiresSerialization;
                proxy.DeserializeReply = _reflector.ReplyRequiresSerialization;
            }

            if (_reflector.Attribute.SupportFaults && !proxy.IsFaultFormatterSetExplicit)
                proxy.FaultFormatter = (IClientFaultFormatter)CreateFaultFormatter(proxy.FaultContractInfos);
        }


        // helper for reflecting operations
        internal class Reflector
        {
            private readonly XmlSerializerImporter _importer;
            private readonly SerializerGenerationContext _generation;
            private Collection<OperationReflector> _operationReflectors = new Collection<OperationReflector>();
            private object _thisLock = new object();

            internal Reflector(string defaultNs, Type type)
            {
                _importer = new XmlSerializerImporter(defaultNs);
                _generation = new SerializerGenerationContext(type);
            }

            internal void EnsureMessageInfos()
            {
                lock (_thisLock)
                {
                    foreach (OperationReflector operationReflector in _operationReflectors)
                    {
                        operationReflector.EnsureMessageInfos();
                    }
                }
            }


            private static XmlSerializerFormatAttribute FindAttribute(OperationDescription operation)
            {
                Type contractType = operation.DeclaringContract != null ? operation.DeclaringContract.ContractType : null;
                XmlSerializerFormatAttribute contractFormatAttribute = contractType != null ? TypeLoader.GetFormattingAttribute(contractType, null) as XmlSerializerFormatAttribute : null;
                return TypeLoader.GetFormattingAttribute(operation.OperationMethod, contractFormatAttribute) as XmlSerializerFormatAttribute;
            }

            // auto-reflects the operation, returning null if no attribute was found or inherited
            internal OperationReflector ReflectOperation(OperationDescription operation)
            {
                XmlSerializerFormatAttribute attr = FindAttribute(operation);
                if (attr == null)
                    return null;

                return ReflectOperation(operation, attr);
            }

            // overrides the auto-reflection with an attribute
            internal OperationReflector ReflectOperation(OperationDescription operation, XmlSerializerFormatAttribute attrOverride)
            {
                OperationReflector operationReflector = new OperationReflector(this, operation, attrOverride, true/*reflectOnDemand*/);
                _operationReflectors.Add(operationReflector);

                return operationReflector;
            }

            internal class OperationReflector
            {
                private readonly Reflector _parent;

                internal readonly OperationDescription Operation;
                internal readonly XmlSerializerFormatAttribute Attribute;

                internal readonly bool IsRpc;
                internal readonly bool IsOneWay;
                internal readonly bool RequestRequiresSerialization;
                internal readonly bool ReplyRequiresSerialization;

                private readonly string _keyBase;

                private MessageInfo _request;
                private MessageInfo _reply;
                private SynchronizedCollection<XmlSerializerFaultContractInfo> _xmlSerializerFaultContractInfos;

                internal OperationReflector(Reflector parent, OperationDescription operation, XmlSerializerFormatAttribute attr, bool reflectOnDemand)
                {
                    Fx.Assert(parent != null, "");
                    Fx.Assert(operation != null, "");
                    Fx.Assert(attr != null, "");

                    OperationFormatter.Validate(operation, attr.Style == OperationFormatStyle.Rpc, false/*IsEncoded*/);

                    _parent = parent;

                    this.Operation = operation;
                    this.Attribute = attr;

                    this.IsRpc = (attr.Style == OperationFormatStyle.Rpc);
                    this.IsOneWay = operation.Messages.Count == 1;

                    this.RequestRequiresSerialization = !operation.Messages[0].IsUntypedMessage;
                    this.ReplyRequiresSerialization = !this.IsOneWay && !operation.Messages[1].IsUntypedMessage;

                    MethodInfo methodInfo = operation.OperationMethod;
                    if (methodInfo == null)
                    {
                        // keyBase needs to be unique within the scope of the parent reflector
                        _keyBase = string.Empty;
                        if (operation.DeclaringContract != null)
                        {
                            _keyBase = operation.DeclaringContract.Name + "," + operation.DeclaringContract.Namespace + ":";
                        }
                        _keyBase = _keyBase + operation.Name;
                    }
                    else
                        _keyBase = methodInfo.DeclaringType.FullName + ":" + methodInfo.ToString();

                    foreach (MessageDescription message in operation.Messages)
                        foreach (MessageHeaderDescription header in message.Headers)
                            SetUnknownHeaderInDescription(header);
                    if (!reflectOnDemand)
                    {
                        this.EnsureMessageInfos();
                    }
                }

                private void SetUnknownHeaderInDescription(MessageHeaderDescription header)
                {
                    if (header.AdditionalAttributesProvider != null)
                    {
                        object[] attrs = header.AdditionalAttributesProvider.GetCustomAttributes(false);
                        foreach (var attr in attrs)
                        {
                            if (attr is XmlAnyElementAttribute)
                            {
                                if (String.IsNullOrEmpty(((XmlAnyElementAttribute)attr).Name))
                                {
                                    header.IsUnknownHeaderCollection = true;
                                }
                            }
                        }
                    }
                }

                private string ContractName
                {
                    get { return this.Operation.DeclaringContract.Name; }
                }

                private string ContractNamespace
                {
                    get { return this.Operation.DeclaringContract.Namespace; }
                }

                internal MessageInfo Request
                {
                    get
                    {
                        _parent.EnsureMessageInfos();
                        return _request;
                    }
                }

                internal MessageInfo Reply
                {
                    get
                    {
                        _parent.EnsureMessageInfos();
                        return _reply;
                    }
                }

                internal SynchronizedCollection<XmlSerializerFaultContractInfo> XmlSerializerFaultContractInfos
                {
                    get
                    {
                        _parent.EnsureMessageInfos();
                        return _xmlSerializerFaultContractInfos;
                    }
                }

                internal void EnsureMessageInfos()
                {
                    if (_request == null)
                    {
                        foreach (Type knownType in Operation.KnownTypes)
                        {
                            if (knownType == null)
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SFxKnownTypeNull, Operation.Name)));
                            _parent._importer.IncludeType(knownType);
                        }
                        _request = CreateMessageInfo(this.Operation.Messages[0], ":Request");
                        // We don't do the following check at Net Native runtime because XmlMapping.XsdElementName 
                        // is not available at that time.
                        bool skipVerifyXsdElementName = false;
#if FEATURE_NETNATIVE
                        skipVerifyXsdElementName = GeneratedXmlSerializers.IsInitialized;
#endif
                        if (_request != null && this.IsRpc && this.Operation.IsValidateRpcWrapperName && !skipVerifyXsdElementName && _request.BodyMapping.XsdElementName != this.Operation.Name)
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SFxRpcMessageBodyPartNameInvalid, Operation.Name, this.Operation.Messages[0].MessageName, _request.BodyMapping.XsdElementName, this.Operation.Name)));
                        if (!this.IsOneWay)
                        {
                            _reply = CreateMessageInfo(this.Operation.Messages[1], ":Response");
                            XmlName responseName = TypeLoader.GetBodyWrapperResponseName(this.Operation.Name);
                            if (_reply != null && this.IsRpc && this.Operation.IsValidateRpcWrapperName && !skipVerifyXsdElementName && _reply.BodyMapping.XsdElementName != responseName.EncodedName)
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SFxRpcMessageBodyPartNameInvalid, Operation.Name, this.Operation.Messages[1].MessageName, _reply.BodyMapping.XsdElementName, responseName.EncodedName)));
                        }
                        if (this.Attribute.SupportFaults)
                        {
                            GenerateXmlSerializerFaultContractInfos();
                        }
                    }
                }

                private void GenerateXmlSerializerFaultContractInfos()
                {
                    SynchronizedCollection<XmlSerializerFaultContractInfo> faultInfos = new SynchronizedCollection<XmlSerializerFaultContractInfo>();
                    for (int i = 0; i < this.Operation.Faults.Count; i++)
                    {
                        FaultDescription fault = this.Operation.Faults[i];
                        FaultContractInfo faultContractInfo = new FaultContractInfo(fault.Action, fault.DetailType, fault.ElementName, fault.Namespace, this.Operation.KnownTypes);

                        XmlQualifiedName elementName;
                        XmlMembersMapping xmlMembersMapping = this.ImportFaultElement(fault, out elementName);

                        SerializerStub serializerStub = _parent._generation.AddSerializer(xmlMembersMapping);
                        faultInfos.Add(new XmlSerializerFaultContractInfo(faultContractInfo, serializerStub, elementName));
                    }
                    _xmlSerializerFaultContractInfos = faultInfos;
                }

                private MessageInfo CreateMessageInfo(MessageDescription message, string key)
                {
                    if (message.IsUntypedMessage)
                        return null;
                    MessageInfo info = new MessageInfo();
                    bool isEncoded = false;
                    if (message.IsTypedMessage)
                        key = message.MessageType.FullName + ":" + isEncoded + ":" + IsRpc;
                    XmlMembersMapping headersMapping = LoadHeadersMapping(message, key + ":Headers");
                    info.SetHeaders(_parent._generation.AddSerializer(headersMapping));
                    MessagePartDescriptionCollection rpcEncodedTypedMessgeBodyParts;
                    info.SetBody(_parent._generation.AddSerializer(LoadBodyMapping(message, key, out rpcEncodedTypedMessgeBodyParts)), rpcEncodedTypedMessgeBodyParts);
                    CreateHeaderDescriptionTable(message, info, headersMapping);
                    return info;
                }

                private void CreateHeaderDescriptionTable(MessageDescription message, MessageInfo info, XmlMembersMapping headersMapping)
                {
                    int headerNameIndex = 0;
                    OperationFormatter.MessageHeaderDescriptionTable headerDescriptionTable = new OperationFormatter.MessageHeaderDescriptionTable();
                    info.SetHeaderDescriptionTable(headerDescriptionTable);
                    foreach (MessageHeaderDescription header in message.Headers)
                    {
                        if (header.IsUnknownHeaderCollection)
                            info.SetUnknownHeaderDescription(header);
                        else if (headersMapping != null)
                        {
                            XmlMemberMapping memberMapping = headersMapping[headerNameIndex++];

                            if (GeneratedXmlSerializers.IsInitialized)
                            {
                                // If GeneratedXmlSerializers has been initialized, we would use the 
                                // mappings generated by .Net Native tools. In that case, the mappings
                                // we genrated at Runtime are just fake mapping instance which have
                                // no valid name/namespace. Therefore we cannot do the checks in the
                                // else block. Those checks should have been done during NET Native 
                                // precompilation.
                                headerDescriptionTable.Add(header.Name, header.Namespace, header);
                            }
                            else
                            {
                                string headerName, headerNs;
                                headerName = memberMapping.XsdElementName;
                                headerNs = memberMapping.Namespace;
                                if (headerName != header.Name)
                                {
                                    if (message.MessageType != null)
                                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SFxHeaderNameMismatchInMessageContract, message.MessageType, header.MemberInfo.Name, header.Name, headerName)));
                                    else
                                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SFxHeaderNameMismatchInOperation, this.Operation.Name, this.Operation.DeclaringContract.Name, this.Operation.DeclaringContract.Namespace, header.Name, headerName)));
                                }
                                if (headerNs != header.Namespace)
                                {
                                    if (message.MessageType != null)
                                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SFxHeaderNamespaceMismatchInMessageContract, message.MessageType, header.MemberInfo.Name, header.Namespace, headerNs)));
                                    else
                                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SFxHeaderNamespaceMismatchInOperation, this.Operation.Name, this.Operation.DeclaringContract.Name, this.Operation.DeclaringContract.Namespace, header.Namespace, headerNs)));
                                }

                                headerDescriptionTable.Add(headerName, headerNs, header);
                            }
                        }
                    }
                }

                private XmlMembersMapping LoadBodyMapping(MessageDescription message, string mappingKey, out MessagePartDescriptionCollection rpcEncodedTypedMessageBodyParts)
                {
                    MessagePartDescription returnPart;
                    string wrapperName, wrapperNs;
                    MessagePartDescriptionCollection bodyParts;
                    rpcEncodedTypedMessageBodyParts = null;
                    returnPart = OperationFormatter.IsValidReturnValue(message.Body.ReturnValue) ? message.Body.ReturnValue : null;
                    bodyParts = message.Body.Parts;
                    wrapperName = message.Body.WrapperName;
                    wrapperNs = message.Body.WrapperNamespace;
                    bool isWrapped = (wrapperName != null);
                    bool hasReturnValue = returnPart != null;
                    int paramCount = bodyParts.Count + (hasReturnValue ? 1 : 0);
                    if (paramCount == 0 && !isWrapped) // no need to create serializer
                    {
                        return null;
                    }

                    XmlReflectionMember[] members = new XmlReflectionMember[paramCount];
                    int paramIndex = 0;
                    if (hasReturnValue)
                        members[paramIndex++] = XmlSerializerHelper.GetXmlReflectionMember(returnPart, IsRpc, isWrapped);

                    for (int i = 0; i < bodyParts.Count; i++)
                        members[paramIndex++] = XmlSerializerHelper.GetXmlReflectionMember(bodyParts[i], IsRpc, isWrapped);

                    if (!isWrapped)
                        wrapperNs = ContractNamespace;
                    return ImportMembersMapping(wrapperName, wrapperNs, members, isWrapped, IsRpc, mappingKey);
                }

                private XmlMembersMapping LoadHeadersMapping(MessageDescription message, string mappingKey)
                {
                    int headerCount = message.Headers.Count;

                    if (headerCount == 0)
                        return null;

                    int unknownHeaderCount = 0, headerIndex = 0;
                    XmlReflectionMember[] members = new XmlReflectionMember[headerCount];
                    for (int i = 0; i < headerCount; i++)
                    {
                        MessageHeaderDescription header = message.Headers[i];
                        if (!header.IsUnknownHeaderCollection)
                        {
                            members[headerIndex++] = XmlSerializerHelper.GetXmlReflectionMember(header, false/*isRpc*/, false/*isWrapped*/);
                        }
                        else
                        {
                            unknownHeaderCount++;
                        }
                    }

                    if (unknownHeaderCount == headerCount)
                    {
                        return null;
                    }

                    if (unknownHeaderCount > 0)
                    {
                        XmlReflectionMember[] newMembers = new XmlReflectionMember[headerCount - unknownHeaderCount];
                        Array.Copy(members, newMembers, newMembers.Length);
                        members = newMembers;
                    }

                    return ImportMembersMapping(ContractName, ContractNamespace, members, false /*isWrapped*/, false /*isRpc*/, mappingKey);
                }

                internal XmlMembersMapping ImportMembersMapping(string elementName, string ns, XmlReflectionMember[] members, bool hasWrapperElement, bool rpc, string mappingKey)
                {
                    string key = mappingKey.StartsWith(":", StringComparison.Ordinal) ? _keyBase + mappingKey : mappingKey;
                    return _parent._importer.ImportMembersMapping(new XmlName(elementName, true /*isEncoded*/), ns, members, hasWrapperElement, rpc, key);
                }

                internal XmlMembersMapping ImportFaultElement(FaultDescription fault, out XmlQualifiedName elementName)
                {
                    // the number of reflection members is always 1 because there is only one fault detail type
                    XmlReflectionMember[] members = new XmlReflectionMember[1];

                    XmlName faultElementName = fault.ElementName;
                    string faultNamespace = fault.Namespace;
                    if (faultElementName == null)
                    {
                        XmlTypeMapping mapping = _parent._importer.ImportTypeMapping(fault.DetailType);
                        faultElementName = new XmlName(mapping.ElementName, false /*isEncoded*/);
                        faultNamespace = mapping.Namespace;
                        if (faultElementName == null)
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SFxFaultTypeAnonymous, this.Operation.Name, fault.DetailType.FullName)));
                    }

                    elementName = new XmlQualifiedName(faultElementName.DecodedName, faultNamespace);

                    members[0] = XmlSerializerHelper.GetXmlReflectionMember(null /*memberName*/, faultElementName, faultNamespace, fault.DetailType,
                        null /*additionalAttributesProvider*/, false /*isMultiple*/, false /*isWrapped*/);

                    string mappingKey = "fault:" + faultElementName.DecodedName + ":" + faultNamespace;
                    return ImportMembersMapping(faultElementName.EncodedName, faultNamespace, members, false /*hasWrapperElement*/, this.IsRpc, mappingKey);
                }
            }

            private class XmlSerializerImporter
            {
                private readonly string _defaultNs;
                private XmlReflectionImporter _xmlImporter;
                private Dictionary<string, XmlMembersMapping> _xmlMappings;
                private HashSet<Type> _includedTypes;

                internal XmlSerializerImporter(string defaultNs)
                {
                    _defaultNs = defaultNs;
                    _xmlImporter = null;
                }

                private XmlReflectionImporter XmlImporter
                {
                    get
                    {
                        if (_xmlImporter == null)
                        {
                            _xmlImporter = new XmlReflectionImporter(_defaultNs);
                        }
                        return _xmlImporter;
                    }
                }

                private Dictionary<string, XmlMembersMapping> XmlMappings
                {
                    get
                    {
                        if (_xmlMappings == null)
                        {
                            _xmlMappings = new Dictionary<string, XmlMembersMapping>();
                        }
                        return _xmlMappings;
                    }
                }

                private HashSet<Type> IncludedTypes
                {
                    get
                    {
                        if (_includedTypes == null)
                        {
                            _includedTypes = new HashSet<Type>();
                        }
                        return _includedTypes;
                    }
                }

                internal XmlMembersMapping ImportMembersMapping(XmlName elementName, string ns, XmlReflectionMember[] members, bool hasWrapperElement, bool rpc, string mappingKey)
                {
                    XmlMembersMapping mapping;
                    string mappingName = elementName.DecodedName;
                    if (XmlMappings.TryGetValue(mappingKey, out mapping))
                    {
                        return mapping;
                    }

                    mapping = this.XmlImporter.ImportMembersMapping(mappingName, ns, members, hasWrapperElement, rpc);
                    mapping.SetKey(mappingKey);
                    XmlMappings.Add(mappingKey, mapping);
                    return mapping;
                }

                internal XmlTypeMapping ImportTypeMapping(Type type)
                {
                    return this.XmlImporter.ImportTypeMapping(type);
                }

                internal void IncludeType(Type knownType)
                {
                    // XmlReflectionImporter.IncludeTypes calls XmlReflectionImporter.ImportTypeMapping to generate mappings
                    // for types and store those mappings.
                    // XmlReflectionImporter.ImportTypeMapping internally uses HashTables for caching imported mappings.
                    // But it's still very costly to call XmlReflectionImporter.ImportTypeMapping because XmlReflectionImporter.ImportTypeMapping
                    // method takes many params and the generated mapping can vary on all those params. XmlReflectionImporter 
                    // needs to do some work before it can use its caches.
                    //
                    // In this case, the mapping should only vary on the value of the knownType. 
                    // Including a type twice doesn't make any difference than including the type once. Therefore we use 
                    // IncludedTypes to store the types that have been included and skip them later.
                    if (IncludedTypes.Contains(knownType))
                        return;

                    this.XmlImporter.IncludeType(knownType);
                    IncludedTypes.Add(knownType);
                }
            }

            internal class SerializerGenerationContext
            {
                private List<XmlMembersMapping> _mappings = new List<XmlMembersMapping>();
                private XmlSerializer[] _serializers = null;
                private Type _type;
                private object _thisLock = new object();

                internal SerializerGenerationContext(Type type)
                {
                    _type = type;
                }

                // returns a stub to a serializer
                internal SerializerStub AddSerializer(XmlMembersMapping mapping)
                {
                    int handle = -1;
                    if (mapping != null)
                    {
                        handle = ((IList)_mappings).Add(mapping);
                    }

                    return new SerializerStub(this, mapping, handle);
                }

                internal XmlSerializer GetSerializer(int handle)
                {
                    if (handle < 0)
                    {
                        return null;
                    }

                    if (_serializers == null)
                    {
                        lock (_thisLock)
                        {
                            if (_serializers == null)
                            {
                                _serializers = GenerateSerializers();
                            }
                        }
                    }
                    return _serializers[handle];
                }

                private XmlSerializer[] GenerateSerializers()
                {
                    //this.Mappings may have duplicate mappings (for e.g. same message contract is used by more than one operation)
                    //XmlSerializer.FromMappings require unique mappings. The following code uniquifies, calls FromMappings and deuniquifies
                    List<XmlMembersMapping> uniqueMappings = new List<XmlMembersMapping>();
                    int[] uniqueIndexes = new int[_mappings.Count];
                    for (int srcIndex = 0; srcIndex < _mappings.Count; srcIndex++)
                    {
                        XmlMembersMapping mapping = _mappings[srcIndex];
                        int uniqueIndex = uniqueMappings.IndexOf(mapping);
                        if (uniqueIndex < 0)
                        {
                            uniqueMappings.Add(mapping);
                            uniqueIndex = uniqueMappings.Count - 1;
                        }
                        uniqueIndexes[srcIndex] = uniqueIndex;
                    }
                    XmlSerializer[] uniqueSerializers = CreateSerializersFromMappings(uniqueMappings.ToArray(), _type);
                    if (uniqueMappings.Count == _mappings.Count)
                        return uniqueSerializers;
                    XmlSerializer[] serializers = new XmlSerializer[_mappings.Count];
                    for (int i = 0; i < _mappings.Count; i++)
                    {
                        serializers[i] = uniqueSerializers[uniqueIndexes[i]];
                    }
                    return serializers;
                }

                [Fx.Tag.SecurityNote(Critical = "XmlSerializer.FromMappings has a LinkDemand.",
                    Safe = "LinkDemand is spurious, not protecting anything in particular.")]
                [SecuritySafeCritical]
                private XmlSerializer[] CreateSerializersFromMappings(XmlMapping[] mappings, Type type)
                {
                    return XmlSerializerHelper.FromMappings(mappings, type);
                }
            }

            internal struct SerializerStub
            {
                private readonly SerializerGenerationContext _context;

                internal readonly XmlMembersMapping Mapping;
                internal readonly int Handle;

                internal SerializerStub(SerializerGenerationContext context, XmlMembersMapping mapping, int handle)
                {
                    _context = context;
                    this.Mapping = mapping;
                    this.Handle = handle;
                }

                internal XmlSerializer GetSerializer()
                {
                    return _context.GetSerializer(Handle);
                }
            }

            internal class XmlSerializerFaultContractInfo
            {
                private FaultContractInfo _faultContractInfo;
                private SerializerStub _serializerStub;
                private XmlQualifiedName _faultContractElementName;
                private XmlSerializerObjectSerializer _serializer;

                internal XmlSerializerFaultContractInfo(FaultContractInfo faultContractInfo, SerializerStub serializerStub,
                    XmlQualifiedName faultContractElementName)
                {
                    if (faultContractInfo == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("faultContractInfo");
                    }
                    if (faultContractElementName == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("faultContractElementName");
                    }
                    _faultContractInfo = faultContractInfo;
                    _serializerStub = serializerStub;
                    _faultContractElementName = faultContractElementName;
                }

                internal FaultContractInfo FaultContractInfo
                {
                    get { return _faultContractInfo; }
                }

                internal XmlQualifiedName FaultContractElementName
                {
                    get { return _faultContractElementName; }
                }

                internal XmlSerializerObjectSerializer Serializer
                {
                    get
                    {
                        if (_serializer == null)
                            _serializer = new XmlSerializerObjectSerializer(_faultContractInfo.Detail, _faultContractElementName, _serializerStub.GetSerializer());
                        return _serializer;
                    }
                }
            }

            internal class MessageInfo : XmlSerializerOperationFormatter.MessageInfo
            {
                private SerializerStub _headers;
                private SerializerStub _body;
                private OperationFormatter.MessageHeaderDescriptionTable _headerDescriptionTable;
                private MessageHeaderDescription _unknownHeaderDescription;
                private MessagePartDescriptionCollection _rpcEncodedTypedMessageBodyParts;

                internal XmlMembersMapping BodyMapping
                {
                    get { return _body.Mapping; }
                }

                internal override XmlSerializer BodySerializer
                {
                    get { return _body.GetSerializer(); }
                }

                internal XmlMembersMapping HeadersMapping
                {
                    get { return _headers.Mapping; }
                }

                internal override XmlSerializer HeaderSerializer
                {
                    get { return _headers.GetSerializer(); }
                }

                internal override OperationFormatter.MessageHeaderDescriptionTable HeaderDescriptionTable
                {
                    get { return _headerDescriptionTable; }
                }

                internal override MessageHeaderDescription UnknownHeaderDescription
                {
                    get { return _unknownHeaderDescription; }
                }

                internal override MessagePartDescriptionCollection RpcEncodedTypedMessageBodyParts
                {
                    get { return _rpcEncodedTypedMessageBodyParts; }
                }

                internal void SetBody(SerializerStub body, MessagePartDescriptionCollection rpcEncodedTypedMessageBodyParts)
                {
                    _body = body;
                    _rpcEncodedTypedMessageBodyParts = rpcEncodedTypedMessageBodyParts;
                }

                internal void SetHeaders(SerializerStub headers)
                {
                    _headers = headers;
                }

                internal void SetHeaderDescriptionTable(OperationFormatter.MessageHeaderDescriptionTable headerDescriptionTable)
                {
                    _headerDescriptionTable = headerDescriptionTable;
                }

                internal void SetUnknownHeaderDescription(MessageHeaderDescription unknownHeaderDescription)
                {
                    _unknownHeaderDescription = unknownHeaderDescription;
                }
            }
        }
    }

    internal static class XmlSerializerHelper
    {
        static internal XmlReflectionMember GetXmlReflectionMember(MessagePartDescription part, bool isRpc, bool isWrapped)
        {
            string ns = isRpc ? null : part.Namespace;
            MemberInfo additionalAttributesProvider = null;
            if (part.AdditionalAttributesProvider.MemberInfo != null)
                additionalAttributesProvider = part.AdditionalAttributesProvider.MemberInfo;
            XmlName memberName = string.IsNullOrEmpty(part.UniquePartName) ? null : new XmlName(part.UniquePartName, true /*isEncoded*/);
            XmlName elementName = part.XmlName;
            return GetXmlReflectionMember(memberName, elementName, ns, part.Type, additionalAttributesProvider, part.Multiple, isWrapped);
        }

        static internal XmlReflectionMember GetXmlReflectionMember(XmlName memberName, XmlName elementName, string ns, Type type, MemberInfo additionalAttributesProvider, bool isMultiple, bool isWrapped)
        {
            XmlReflectionMember member = new XmlReflectionMember();
            member.MemberName = (memberName ?? elementName).DecodedName;
            member.MemberType = type;
            if (member.MemberType.IsByRef)
                member.MemberType = member.MemberType.GetElementType();
            if (isMultiple)
                member.MemberType = member.MemberType.MakeArrayType();
            if (additionalAttributesProvider != null)
            {
#if NETStandard13
                member.XmlAttributes = XmlAttributesHelper.CreateXmlAttributes(additionalAttributesProvider);
#else
                member.XmlAttributes = new XmlAttributes(additionalAttributesProvider);
#endif
            }

            if (member.XmlAttributes == null)
                member.XmlAttributes = new XmlAttributes();
            else
            {
                Type invalidAttributeType = null;
                if (member.XmlAttributes.XmlAttribute != null)
                    invalidAttributeType = typeof(XmlAttributeAttribute);
                else if (member.XmlAttributes.XmlAnyAttribute != null && !isWrapped)
                    invalidAttributeType = typeof(XmlAnyAttributeAttribute);
                else if (member.XmlAttributes.XmlChoiceIdentifier != null)
                    invalidAttributeType = typeof(XmlChoiceIdentifierAttribute);
                else if (member.XmlAttributes.XmlIgnore)
                    invalidAttributeType = typeof(XmlIgnoreAttribute);
                else if (member.XmlAttributes.Xmlns)
                    invalidAttributeType = typeof(XmlNamespaceDeclarationsAttribute);
                else if (member.XmlAttributes.XmlText != null)
                    invalidAttributeType = typeof(XmlTextAttribute);
                else if (member.XmlAttributes.XmlEnum != null)
                    invalidAttributeType = typeof(XmlEnumAttribute);
                if (invalidAttributeType != null)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(isWrapped ? SR.SFxInvalidXmlAttributeInWrapped : SR.SFxInvalidXmlAttributeInBare, invalidAttributeType, elementName.DecodedName)));
                if (member.XmlAttributes.XmlArray != null && isMultiple)
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.SFxXmlArrayNotAllowedForMultiple, elementName.DecodedName, ns)));
            }


            bool isArray = member.MemberType.IsArray;
            if ((isArray && !isMultiple && member.MemberType != typeof(byte[])) ||
                (!isArray && typeof(IEnumerable).IsAssignableFrom(member.MemberType) && member.MemberType != typeof(string) && !typeof(XmlNode).IsAssignableFrom(member.MemberType) && !typeof(IXmlSerializable).IsAssignableFrom(member.MemberType)))
            {
                if (member.XmlAttributes.XmlArray != null)
                {
                    if (member.XmlAttributes.XmlArray.ElementName == String.Empty)
                        member.XmlAttributes.XmlArray.ElementName = elementName.DecodedName;
                    if (member.XmlAttributes.XmlArray.Namespace == null)
                        member.XmlAttributes.XmlArray.Namespace = ns;
                }
                else if (HasNoXmlParameterAttributes(member.XmlAttributes))
                {
                    member.XmlAttributes.XmlArray = new XmlArrayAttribute();
                    member.XmlAttributes.XmlArray.ElementName = elementName.DecodedName;
                    member.XmlAttributes.XmlArray.Namespace = ns;
                }
            }
            else
            {
                if (member.XmlAttributes.XmlElements == null || member.XmlAttributes.XmlElements.Count == 0)
                {
                    if (HasNoXmlParameterAttributes(member.XmlAttributes))
                    {
                        XmlElementAttribute elementAttribute = new XmlElementAttribute();
                        elementAttribute.ElementName = elementName.DecodedName;
                        elementAttribute.Namespace = ns;
                        member.XmlAttributes.XmlElements.Add(elementAttribute);
                    }
                }
                else
                {
                    foreach (XmlElementAttribute elementAttribute in member.XmlAttributes.XmlElements)
                    {
                        if (elementAttribute.ElementName == String.Empty)
                            elementAttribute.ElementName = elementName.DecodedName;
                        if (elementAttribute.Namespace == null)
                            elementAttribute.Namespace = ns;
                    }
                }
            }

            return member;
        }

        private static bool HasNoXmlParameterAttributes(XmlAttributes xmlAttributes)
        {
            return xmlAttributes.XmlAnyAttribute == null &&
                (xmlAttributes.XmlAnyElements == null || xmlAttributes.XmlAnyElements.Count == 0) &&
                xmlAttributes.XmlArray == null &&
                xmlAttributes.XmlAttribute == null &&
                !xmlAttributes.XmlIgnore &&
                xmlAttributes.XmlText == null &&
                xmlAttributes.XmlChoiceIdentifier == null &&
                (xmlAttributes.XmlElements == null || xmlAttributes.XmlElements.Count == 0) &&
                !xmlAttributes.Xmlns;
        }

        public static XmlSerializer[] FromMappings(XmlMapping[] mappings, Type type)
        {
#if FEATURE_NETNATIVE
            if (GeneratedXmlSerializers.IsInitialized)
            {
                return FromMappingsViaInjection(mappings, type);
            }

#endif
            return FromMappingsViaReflection(mappings, type);
        }

        private static XmlSerializer[] FromMappingsViaReflection(XmlMapping[] mappings, Type type)
        {
            if (mappings == null || mappings.Length == 0)
            {
                return new XmlSerializer[0];
            }

#if NETStandard13
            Array mappingArray = XmlMappingTypesHelper.InitializeArray(XmlMappingTypesHelper.XmlMappingType, mappings);
            MethodInfo method = typeof(XmlSerializer).GetMethod("FromMappings", new Type[] { XmlMappingTypesHelper.XmlMappingType.MakeArrayType(), typeof(Type) });
            object result = method.Invoke(null, new object[] { mappingArray, type });

            return (XmlSerializer[])result;
#else
            return XmlSerializer.FromMappings(mappings, type);
#endif
        }
#if FEATURE_NETNATIVE

        private static XmlSerializer[] FromMappingsViaInjection(XmlMapping[] mappings, Type type)
        {
            XmlSerializer[] serializers = new XmlSerializer[mappings.Length];
            for (int i = 0; i < serializers.Length; i++)
            {
                Type t;
                GeneratedXmlSerializers.GetGeneratedSerializers().TryGetValue(mappings[i].Key, out t);
                if (t == null)
                {
                    throw new InvalidOperationException(SR.Format(SR.SFxXmlSerializerIsNotFound, type));
                }

                serializers[i] = new XmlSerializer(t);
            }

            return serializers;
        }
#endif
    }
}
