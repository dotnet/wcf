// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class WcfDuplexService : IWcfDuplexService, IWcfDuplexService_DataContract, IWcfDuplexService_Xml
    {
        public static IWcfDuplexServiceCallback callback;
        public static IWcfDuplexService_DataContract_Callback dc_callback;
        public static IWcfDuplexService_Xml_Callback xml_callback;

        public void Ping(Guid guid)
        {
            callback = OperationContext.Current.GetCallbackChannel<IWcfDuplexServiceCallback>();
            // Schedule the callback on another thread to avoid reentrancy.
            Task.Run(() => callback.OnPingCallback(guid));
        }

        public void Ping_DataContract(Guid guid)
        {
            dc_callback = OperationContext.Current.GetCallbackChannel<IWcfDuplexService_DataContract_Callback>();

            ComplexCompositeTypeDuplexCallbackOnly complexCompositeType = new ComplexCompositeTypeDuplexCallbackOnly();
            complexCompositeType.GuidValue = guid;

            // Schedule the callback on another thread to avoid reentrancy.
            Task.Run(() => dc_callback.OnDataContractPingCallback(complexCompositeType));
        }

        public void Ping_Xml(Guid guid)
        {
            xml_callback = OperationContext.Current.GetCallbackChannel<IWcfDuplexService_Xml_Callback>();
            XmlCompositeTypeDuplexCallbackOnly xmlCompositeType = new XmlCompositeTypeDuplexCallbackOnly();
            xmlCompositeType.StringValue = guid.ToString();

            // Schedule the callback on another thread to avoid reentrancy.
            Task.Run(() => xml_callback.OnXmlPingCallback(xmlCompositeType));
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    public class DuplexCallbackService : IDuplexChannelService
    {
        public void Ping(Guid guid)
        {
            IDuplexChannelCallback callback = OperationContext.Current.GetCallbackChannel<IDuplexChannelCallback>();
            callback.OnPingCallback(guid);
        }
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any)]
    public class DuplexChannelCallbackReturnService : IWcfDuplexTaskReturnService
    {
        public Task<Guid> Ping(Guid guid)
        {
            IWcfDuplexTaskReturnCallback callback = OperationContext.Current.GetCallbackChannel<IWcfDuplexTaskReturnCallback>();
            return callback.ServicePingCallback(guid);
        }

        public async Task<Guid> FaultPing(Guid guid)
        {
            Guid retval;
            IWcfDuplexTaskReturnCallback callback = OperationContext.Current.GetCallbackChannel<IWcfDuplexTaskReturnCallback>();
            try
            {
                // If we just return the result of the Callback we won't catch the FaultException here.
                retval = await callback.ServicePingFaultCallback(guid);
            }
            catch (FaultException<FaultDetail> ex)
            {
                // Need to throw a new instance of FaultException so that certain fields such as the Action match what the client expects
                // Otherwise the client will just get a plain FaultException instead of the expected FaultException<FaultDetail>
                throw new FaultException<FaultDetail>(ex.Detail, ex.Message, ex.Code);
            }
            return retval;
        }
    }
}
