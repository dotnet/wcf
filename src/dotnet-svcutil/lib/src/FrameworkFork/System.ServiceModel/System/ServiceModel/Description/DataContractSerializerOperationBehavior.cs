// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Microsoft.Xml;

namespace System.ServiceModel.Description
{
    public class DataContractSerializerOperationBehavior : IOperationBehavior
    {
        private readonly bool _builtInOperationBehavior;

        private OperationDescription _operation;
        private DataContractFormatAttribute _dataContractFormatAttribute;
        internal bool ignoreExtensionDataObject = DataContractSerializerDefaults.IgnoreExtensionDataObject;
        private bool _ignoreExtensionDataObjectSetExplicit;
        internal int maxItemsInObjectGraph = DataContractSerializerDefaults.MaxItemsInObjectGraph;
        private bool _maxItemsInObjectGraphSetExplicit;
        private DataContractResolver _dataContractResolver;

        public DataContractFormatAttribute DataContractFormatAttribute
        {
            get { return _dataContractFormatAttribute; }
        }

        public DataContractSerializerOperationBehavior(OperationDescription operation)
            : this(operation, null)
        {
        }

        public DataContractSerializerOperationBehavior(OperationDescription operation, DataContractFormatAttribute dataContractFormatAttribute)
        {
            _dataContractFormatAttribute = dataContractFormatAttribute ?? new DataContractFormatAttribute();
            _operation = operation;
        }

        internal DataContractSerializerOperationBehavior(OperationDescription operation,
            DataContractFormatAttribute dataContractFormatAttribute, bool builtInOperationBehavior)
            : this(operation, dataContractFormatAttribute)
        {
            _builtInOperationBehavior = builtInOperationBehavior;
        }

        internal bool IsBuiltInOperationBehavior
        {
            get { return _builtInOperationBehavior; }
        }

        public int MaxItemsInObjectGraph
        {
            get { return maxItemsInObjectGraph; }
            set
            {
                maxItemsInObjectGraph = value;
                _maxItemsInObjectGraphSetExplicit = true;
            }
        }

        internal bool MaxItemsInObjectGraphSetExplicit
        {
            get { return _maxItemsInObjectGraphSetExplicit; }
            set { _maxItemsInObjectGraphSetExplicit = value; }
        }

        public bool IgnoreExtensionDataObject
        {
            get { return ignoreExtensionDataObject; }
            set
            {
                ignoreExtensionDataObject = value;
                _ignoreExtensionDataObjectSetExplicit = true;
            }
        }

        internal bool IgnoreExtensionDataObjectSetExplicit
        {
            get { return _ignoreExtensionDataObjectSetExplicit; }
            set { _ignoreExtensionDataObjectSetExplicit = value; }
        }


        public DataContractResolver DataContractResolver
        {
            get { return _dataContractResolver; }
            set { _dataContractResolver = value; }
        }

        public virtual XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
        {
            return new DataContractSerializer(type, name, ns, knownTypes);
        }

        public virtual XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
        {
            return new DataContractSerializer(type, name, ns, knownTypes);
        }

        internal object GetFormatter(OperationDescription operation, out bool formatRequest, out bool formatReply, bool isProxy)
        {
            MessageDescription request = operation.Messages[0];
            MessageDescription response = null;
            if (operation.Messages.Count == 2)
                response = operation.Messages[1];

            formatRequest = (request != null) && !request.IsUntypedMessage;
            formatReply = (response != null) && !response.IsUntypedMessage;

            if (formatRequest || formatReply)
            {
                if (PrimitiveOperationFormatter.IsContractSupported(operation))
                    return new PrimitiveOperationFormatter(operation, _dataContractFormatAttribute.Style == OperationFormatStyle.Rpc);
                else
                    return new DataContractSerializerOperationFormatter(operation, _dataContractFormatAttribute, this);
            }

            return null;
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

            if (dispatch.Formatter != null)
                return;

            bool formatRequest;
            bool formatReply;
            dispatch.Formatter = (IDispatchMessageFormatter)GetFormatter(description, out formatRequest, out formatReply, false);
            dispatch.DeserializeRequest = formatRequest;
            dispatch.SerializeReply = formatReply;
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription description, ClientOperation proxy)
        {
            if (description == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("description");

            if (proxy == null)
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("proxy");

            if (proxy.Formatter != null)
                return;

            bool formatRequest;
            bool formatReply;
            proxy.Formatter = (IClientMessageFormatter)GetFormatter(description, out formatRequest, out formatReply, true);
            proxy.SerializeRequest = formatRequest;
            proxy.DeserializeReply = formatReply;
        }
    }
}
