// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Xml;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace System.ServiceModel.Dispatcher
{
    internal class XmlSerializerFaultFormatter : FaultFormatter
    {
        private SynchronizedCollection<XmlSerializerOperationBehavior.Reflector.XmlSerializerFaultContractInfo> _xmlSerializerFaultContractInfos;

        internal XmlSerializerFaultFormatter(Type[] detailTypes,
            SynchronizedCollection<XmlSerializerOperationBehavior.Reflector.XmlSerializerFaultContractInfo> xmlSerializerFaultContractInfos)
            : base(detailTypes)
        {
            Initialize(xmlSerializerFaultContractInfos);
        }

        internal XmlSerializerFaultFormatter(SynchronizedCollection<FaultContractInfo> faultContractInfoCollection,
            SynchronizedCollection<XmlSerializerOperationBehavior.Reflector.XmlSerializerFaultContractInfo> xmlSerializerFaultContractInfos)
            : base(faultContractInfoCollection)
        {
            Initialize(xmlSerializerFaultContractInfos);
        }

        private void Initialize(SynchronizedCollection<XmlSerializerOperationBehavior.Reflector.XmlSerializerFaultContractInfo> xmlSerializerFaultContractInfos)
        {
            _xmlSerializerFaultContractInfos = xmlSerializerFaultContractInfos ?? throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull(nameof(xmlSerializerFaultContractInfos));
        }

        protected override XmlObjectSerializer GetSerializer(Type detailType, string faultExceptionAction, out string action)
        {
            action = faultExceptionAction;

            XmlSerializerOperationBehavior.Reflector.XmlSerializerFaultContractInfo faultInfo = null;
            for (int i = 0; i < _xmlSerializerFaultContractInfos.Count; i++)
            {
                if (_xmlSerializerFaultContractInfos[i].FaultContractInfo.Detail == detailType)
                {
                    faultInfo = _xmlSerializerFaultContractInfos[i];
                    break;
                }
            }
            if (faultInfo != null)
            {
                if (action == null)
                {
                    action = faultInfo.FaultContractInfo.Action;
                }

                return faultInfo.Serializer;
            }
            else
            {
                return new XmlSerializerObjectSerializer(detailType);
            }
        }

        protected override FaultException CreateFaultException(MessageFault messageFault, string action)
        {
            IList<XmlSerializerOperationBehavior.Reflector.XmlSerializerFaultContractInfo> faultInfos;
            if (action != null)
            {
                faultInfos = new List<XmlSerializerOperationBehavior.Reflector.XmlSerializerFaultContractInfo>();
                for (int i = 0; i < _xmlSerializerFaultContractInfos.Count; i++)
                {
                    if (_xmlSerializerFaultContractInfos[i].FaultContractInfo.Action == action
                        || _xmlSerializerFaultContractInfos[i].FaultContractInfo.Action == MessageHeaders.WildcardAction)
                    {
                        faultInfos.Add(_xmlSerializerFaultContractInfos[i]);
                    }
                }
            }
            else
            {
                faultInfos = _xmlSerializerFaultContractInfos;
            }

            Type detailType = null;
            object detailObj = null;
            for (int i = 0; i < faultInfos.Count; i++)
            {
                XmlSerializerOperationBehavior.Reflector.XmlSerializerFaultContractInfo faultInfo = faultInfos[i];
                XmlDictionaryReader detailReader = messageFault.GetReaderAtDetailContents();
                XmlObjectSerializer serializer = faultInfo.Serializer;

                if (serializer.IsStartObject(detailReader))
                {
                    detailType = faultInfo.FaultContractInfo.Detail;
                    try
                    {
                        detailObj = serializer.ReadObject(detailReader);
                        FaultException faultException = CreateFaultException(messageFault, action,
                            detailObj, detailType, detailReader);
                        if (faultException != null)
                        {
                            return faultException;
                        }
                    }
                    catch (SerializationException)
                    {
                    }
                }
            }
            return new FaultException(messageFault, action);
        }
    }
}
