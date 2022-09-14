// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Xml;

namespace System.ServiceModel.Description
{
    public class DataContractSerializerOperationBehavior : IOperationBehavior
    {
        private OperationDescription _operation;
        internal bool ignoreExtensionDataObject = DataContractSerializerDefaults.IgnoreExtensionDataObject;
        internal int maxItemsInObjectGraph = DataContractSerializerDefaults.MaxItemsInObjectGraph;
        private DataContractResolver _dataContractResolver;

        public DataContractFormatAttribute DataContractFormatAttribute { get; }

        public DataContractSerializerOperationBehavior(OperationDescription operation)
            : this(operation, null)
        {
        }

        public DataContractSerializerOperationBehavior(OperationDescription operation, DataContractFormatAttribute dataContractFormatAttribute)
        {
            DataContractFormatAttribute = dataContractFormatAttribute ?? new DataContractFormatAttribute();
            _operation = operation;
        }

        internal DataContractSerializerOperationBehavior(OperationDescription operation,
            DataContractFormatAttribute dataContractFormatAttribute, bool builtInOperationBehavior)
            : this(operation, dataContractFormatAttribute)
        {
            IsBuiltInOperationBehavior = builtInOperationBehavior;
        }

        internal bool IsBuiltInOperationBehavior { get; }

        public int MaxItemsInObjectGraph
        {
            get { return maxItemsInObjectGraph; }
            set
            {
                maxItemsInObjectGraph = value;
                MaxItemsInObjectGraphSetExplicit = true;
            }
        }

        internal bool MaxItemsInObjectGraphSetExplicit { get; set; }

        public bool IgnoreExtensionDataObject
        {
            get { return ignoreExtensionDataObject; }
            set
            {
                ignoreExtensionDataObject = value;
                IgnoreExtensionDataObjectSetExplicit = true;
            }
        }

        internal bool IgnoreExtensionDataObjectSetExplicit { get; set; }

        public ISerializationSurrogateProvider SerializationSurrogateProvider { get; set; }

        public DataContractResolver DataContractResolver
        {
            get { return _dataContractResolver; }
            set { _dataContractResolver = value; }
        }

        public virtual XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
        {
            XmlDictionary dictionary = new XmlDictionary(2);
            DataContractSerializerSettings settings = new DataContractSerializerSettings();
            settings.RootName = dictionary.Add(name);
            settings.RootNamespace = dictionary.Add(ns);
            settings.KnownTypes = knownTypes;
            settings.MaxItemsInObjectGraph = MaxItemsInObjectGraph;
            settings.DataContractResolver = DataContractResolver;
            DataContractSerializer dcs = new DataContractSerializer(type, settings);
            dcs.SetSerializationSurrogateProvider(SerializationSurrogateProvider);
            return dcs;
        }

        public virtual XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
        {
            DataContractSerializerSettings settings = new DataContractSerializerSettings();
            settings.RootName = name;
            settings.RootNamespace = ns;
            settings.KnownTypes = knownTypes;
            settings.MaxItemsInObjectGraph = MaxItemsInObjectGraph;
            settings.DataContractResolver = DataContractResolver;
            DataContractSerializer dcs = new DataContractSerializer(type, settings);
            dcs.SetSerializationSurrogateProvider(SerializationSurrogateProvider);
            return dcs;
        }

        internal object GetFormatter(OperationDescription operation, out bool formatRequest, out bool formatReply, bool isProxy)
        {
            MessageDescription request = operation.Messages[0];
            MessageDescription response = null;
            if (operation.Messages.Count == 2)
            {
                response = operation.Messages[1];
            }

            formatRequest = (request != null) && !request.IsUntypedMessage;
            formatReply = (response != null) && !response.IsUntypedMessage;

            if (formatRequest || formatReply)
            {
                if (PrimitiveOperationFormatter.IsContractSupported(operation))
                {
                    return new PrimitiveOperationFormatter(operation, DataContractFormatAttribute.Style == OperationFormatStyle.Rpc);
                }
                else
                {
                    return new DataContractSerializerOperationFormatter(operation, DataContractFormatAttribute, this);
                }
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
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(description));
            }

            if (dispatch == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(dispatch));
            }

            if (dispatch.Formatter != null)
            {
                return;
            }

            bool formatRequest;
            bool formatReply;
            dispatch.Formatter = (IDispatchMessageFormatter)GetFormatter(description, out formatRequest, out formatReply, false);
            dispatch.DeserializeRequest = formatRequest;
            dispatch.SerializeReply = formatReply;
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription description, ClientOperation proxy)
        {
            if (description == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(description));
            }

            if (proxy == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(proxy));
            }

            if (proxy.Formatter != null)
            {
                return;
            }

            bool formatRequest;
            bool formatReply;
            proxy.Formatter = (IClientMessageFormatter)GetFormatter(description, out formatRequest, out formatReply, true);
            proxy.SerializeRequest = formatRequest;
            proxy.DeserializeReply = formatReply;
        }
    }
}
