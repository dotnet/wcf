// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;
using System.Xml;
using System.Runtime;

namespace System.ServiceModel.Dispatcher
{
    /*
     This class is not exposed on the contract as we only need it public for reflection purpose on .Net Native.
     */
    public class FaultFormatter : IClientFaultFormatter, IDispatchFaultFormatter
    {
        private FaultContractInfo[] _faultContractInfos;

        internal FaultFormatter(Type[] detailTypes)
        {
            List<FaultContractInfo> faultContractInfoList = new List<FaultContractInfo>();
            for (int i = 0; i < detailTypes.Length; i++)
            {
                faultContractInfoList.Add(new FaultContractInfo(MessageHeaders.WildcardAction, detailTypes[i]));
            }

            AddInfrastructureFaults(faultContractInfoList);
            _faultContractInfos = GetSortedArray(faultContractInfoList);
        }

        internal FaultFormatter(SynchronizedCollection<FaultContractInfo> faultContractInfoCollection)
        {
            List<FaultContractInfo> faultContractInfoList;
            lock (faultContractInfoCollection.SyncRoot)
            {
                faultContractInfoList = new List<FaultContractInfo>(faultContractInfoCollection);
            }
            AddInfrastructureFaults(faultContractInfoList);
            _faultContractInfos = GetSortedArray(faultContractInfoList);
        }

        public MessageFault Serialize(FaultException faultException, out string action)
        {
            XmlObjectSerializer serializer = null;
            Type detailType = null;
            string faultExceptionAction = action = faultException.Action;

            Type faultExceptionOfT = null;
            for (Type faultType = faultException.GetType(); faultType != typeof(FaultException); faultType = faultType.BaseType())
            {
                if (faultType.IsGenericType() && (faultType.GetGenericTypeDefinition() == typeof(FaultException<>)))
                {
                    faultExceptionOfT = faultType;
                    break;
                }
            }
            if (faultExceptionOfT != null)
            {
                detailType = faultExceptionOfT.GetGenericArguments()[0];
                serializer = GetSerializer(detailType, faultExceptionAction, out action);
            }
            return CreateMessageFault(serializer, faultException, detailType);
        }

        public FaultException Deserialize(MessageFault messageFault, string action)
        {
            if (!messageFault.HasDetail)
            {
                return new FaultException(messageFault, action);
            }

            return CreateFaultException(messageFault, action);
        }

        protected virtual XmlObjectSerializer GetSerializer(Type detailType, string faultExceptionAction, out string action)
        {
            action = faultExceptionAction;
            FaultContractInfo faultInfo = null;
            for (int i = 0; i < _faultContractInfos.Length; i++)
            {
                if (_faultContractInfos[i].Detail == detailType)
                {
                    faultInfo = _faultContractInfos[i];
                    break;
                }
            }
            if (faultInfo != null)
            {
                if (action == null)
                {
                    action = faultInfo.Action;
                }

                return faultInfo.Serializer;
            }
            else
            {
                return DataContractSerializerDefaults.CreateSerializer(detailType, int.MaxValue /* maxItemsInObjectGraph */ );
            }
        }

        protected virtual FaultException CreateFaultException(MessageFault messageFault, string action)
        {
            IList<FaultContractInfo> faultInfos;
            if (action != null)
            {
                faultInfos = new List<FaultContractInfo>();
                for (int i = 0; i < _faultContractInfos.Length; i++)
                {
                    if (_faultContractInfos[i].Action == action || _faultContractInfos[i].Action == MessageHeaders.WildcardAction)
                    {
                        faultInfos.Add(_faultContractInfos[i]);
                    }
                }
            }
            else
            {
                faultInfos = _faultContractInfos;
            }

            Type detailType = null;
            object detailObj = null;
            for (int i = 0; i < faultInfos.Count; i++)
            {
                FaultContractInfo faultInfo = faultInfos[i];
                XmlDictionaryReader detailReader = messageFault.GetReaderAtDetailContents();
                XmlObjectSerializer serializer = faultInfo.Serializer;
                if (serializer.IsStartObject(detailReader))
                {
                    detailType = faultInfo.Detail;
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

        protected FaultException CreateFaultException(MessageFault messageFault, string action,
            object detailObj, Type detailType, XmlDictionaryReader detailReader)
        {
            if (!detailReader.EOF)
            {
                detailReader.MoveToContent();
                if (detailReader.NodeType != XmlNodeType.EndElement && !detailReader.EOF)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new FormatException(SRP.ExtraContentIsPresentInFaultDetail));
                }
            }
            bool isDetailObjectValid = true;
            if (detailObj == null)
            {
                isDetailObjectValid = !detailType.IsValueType();
            }
            else
            {
                isDetailObjectValid = detailType.IsAssignableFrom(detailObj.GetType());
            }
            if (isDetailObjectValid)
            {
                Type knownFaultType = typeof(FaultException<>).MakeGenericType(detailType);
                return (FaultException)Activator.CreateInstance(knownFaultType,
                                                            detailObj,
                                                            messageFault.Reason,
                                                            messageFault.Code,
                                                            action);
            }
            return null;
        }

        private static FaultContractInfo[] GetSortedArray(List<FaultContractInfo> faultContractInfoList)
        {
            FaultContractInfo[] temp = faultContractInfoList.ToArray();
            Array.Sort(temp,
                delegate (FaultContractInfo x, FaultContractInfo y)
                { return string.CompareOrdinal(x.Action, y.Action); }
                );
            return temp;
        }

        private static void AddInfrastructureFaults(List<FaultContractInfo> faultContractInfos)
        {
            faultContractInfos.Add(new FaultContractInfo(FaultCodeConstants.Actions.NetDispatcher, typeof(ExceptionDetail)));
        }

        private static MessageFault CreateMessageFault(XmlObjectSerializer serializer, FaultException faultException, Type detailType)
        {
            if (detailType == null)
            {
                if (faultException.Fault != null)
                {
                    return faultException.Fault;
                }

                return MessageFault.CreateFault(faultException.Code, faultException.Reason);
            }
            Fx.Assert(serializer != null, "");

            Type operationFaultType = typeof(OperationFault<>).MakeGenericType(detailType);
            return (MessageFault)Activator.CreateInstance(operationFaultType, serializer, faultException);
        }

        /*
        This class is not exposed on the contract as we only need it public for reflection purpose on .Net Native.
        */
        public class OperationFault<T> : XmlObjectSerializerFault
        {
            public OperationFault(XmlObjectSerializer serializer, FaultException<T> faultException) :
                base(faultException.Code, faultException.Reason,
                      faultException.Detail,
                      serializer,
                      string.Empty/*actor*/,
                      string.Empty/*node*/)
            {
            }
        }
    }
}
