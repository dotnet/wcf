// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ServiceModel.Description;
using System.Runtime.Serialization;

namespace System.ServiceModel.Dispatcher
{
    public class FaultContractInfo
    {
        private string _action;
        private Type _detail;
        private string _elementName;
        private string _ns;
        private IList<Type> _knownTypes;
        private DataContractSerializer _serializer;

        public FaultContractInfo(string action, Type detail)
            : this(action, detail, null, null, null)
        {
        }
        internal FaultContractInfo(string action, Type detail, XmlName elementName, string ns, IList<Type> knownTypes)
        {
            if (action == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("action");
            }
            if (detail == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("detail");
            }

            _action = action;
            _detail = detail;
            if (elementName != null)
                _elementName = elementName.EncodedName;
            _ns = ns;
            _knownTypes = knownTypes;
        }

        public string Action { get { return _action; } }

        public Type Detail { get { return _detail; } }

        internal string ElementName { get { return _elementName; } }

        internal string ElementNamespace { get { return _ns; } }

        internal IList<Type> KnownTypes { get { return _knownTypes; } }

        internal DataContractSerializer Serializer
        {
            get
            {
                if (_serializer == null)
                {
                    if (_elementName == null)
                    {
                        _serializer = DataContractSerializerDefaults.CreateSerializer(_detail, _knownTypes, int.MaxValue /* maxItemsInObjectGraph */);
                    }
                    else
                    {
                        _serializer = DataContractSerializerDefaults.CreateSerializer(_detail, _knownTypes, _elementName, _ns == null ? string.Empty : _ns, int.MaxValue /* maxItemsInObjectGraph */);
                    }
                }
                return _serializer;
            }
        }
    }
}

