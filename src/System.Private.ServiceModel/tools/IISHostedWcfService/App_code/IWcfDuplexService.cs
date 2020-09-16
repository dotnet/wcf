﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceContract(CallbackContract = typeof(IWcfDuplexServiceCallback))]
    public interface IWcfDuplexService
    {
        [OperationContract]
        void Ping(Guid guid);
    }

    public interface IWcfDuplexServiceCallback
    {
        [OperationContract]
        void OnPingCallback(Guid guid);
    }

    [ServiceContract(CallbackContract = typeof(IDuplexChannelCallback))]
    public interface IDuplexChannelService
    {
        [OperationContract(IsOneWay = true)]
        void Ping(Guid guid);
    }

    public interface IDuplexChannelCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnPingCallback(Guid guid);
    }

    [ServiceContract(CallbackContract = typeof(IWcfDuplexTaskReturnCallback))]
    public interface IWcfDuplexTaskReturnService
    {
        [OperationContract]
        Task<Guid> Ping(Guid guid);

        [OperationContract]
        [FaultContract(typeof(FaultDetail), Name = "FaultDetail",
            Action = "http://tempuri.org/IWcfDuplexTaskReturnService/FaultPingFaultDetailFault")]
        Task<Guid> FaultPing(Guid guid);
    }

    public interface IWcfDuplexTaskReturnCallback
    {
        [OperationContract]
        Task<Guid> ServicePingCallback(Guid guid);

        [OperationContract]
        [FaultContract(typeof(FaultDetail), Name = "FaultDetail",
            Action = "http://tempuri.org/IWcfDuplexTaskReturnCallback/ServicePingFaultCallbackFaultDetailFault")]
        Task<Guid> ServicePingFaultCallback(Guid guid);
    }

    // ********************************************************************************

    [ServiceContract(CallbackContract = typeof(IWcfDuplexService_Xml_Callback))]
    public interface IWcfDuplexService_Xml
    {
        [OperationContract]
        void Ping_Xml(Guid guid);
    }

    public interface IWcfDuplexService_Xml_Callback
    {
        [OperationContract, XmlSerializerFormat]
        void OnXmlPingCallback(XmlCompositeTypeDuplexCallbackOnly xmlCompositeType);
    }

    // ********************************************************************************

    [ServiceContract(CallbackContract = typeof(IWcfDuplexService_DataContract_Callback))]
    public interface IWcfDuplexService_DataContract
    {
        [OperationContract]
        void Ping_DataContract(Guid guid);
    }

    public interface IWcfDuplexService_DataContract_Callback
    {
        [OperationContract]
        void OnDataContractPingCallback(ComplexCompositeTypeDuplexCallbackOnly complexCompositeType);
    }

    [ServiceContract(CallbackContract = typeof(IWcfDuplexService_CallbackConcurrencyMode_Callback))]
    public interface IWcfDuplexService_CallbackConcurrencyMode
    {
        [OperationContract]
        Task DoWorkAsync();
    }

    public interface IWcfDuplexService_CallbackConcurrencyMode_Callback
    {
        [OperationContract]
        Task CallWithWaitAsync(int delayTime);
    }
}
