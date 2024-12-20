// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
#else
using System;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
#endif

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

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WcfDuplexService_CallbackConcurrenyMode : IWcfDuplexService_CallbackConcurrencyMode
    {
        public async Task DoWorkAsync()
        {
            Task t1 = Callback.CallWithWaitAsync(4000);
            Task t2 = Callback.CallWithWaitAsync(500);
            await Task.WhenAll(t1, t2);   
        }

        public IWcfDuplexService_CallbackConcurrencyMode_Callback Callback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IWcfDuplexService_CallbackConcurrencyMode_Callback>();
            }
        }
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WcfDuplexService_CallbackDebugBehavior : IWcfDuplexService_CallbackDebugBehavior
    {
        public string Hello(string greeting, bool includeExceptionDetailInFaults)
        {
            try
            {
                Callback.ReplyThrow(greeting);
            }
            catch (Exception ex)
            {
                if(ex.Message.Equals(greeting))
                {
                    Environment.SetEnvironmentVariable("callbackexception" + includeExceptionDetailInFaults.ToString().ToLower(), "included", EnvironmentVariableTarget.Process);
                }
                else
                {
                    Environment.SetEnvironmentVariable("callbackexception" + includeExceptionDetailInFaults.ToString().ToLower(), "unincluded", EnvironmentVariableTarget.Process);
                }
            }

            return greeting;
        }

        public bool GetResult(bool includeExceptionDetailInFaults)
        {
            string envVar = "callbackexception" + includeExceptionDetailInFaults.ToString().ToLower();
            string result = Environment.GetEnvironmentVariable(envVar, EnvironmentVariableTarget.Process);
            Environment.SetEnvironmentVariable(envVar, null, EnvironmentVariableTarget.Process);
            if (includeExceptionDetailInFaults)
            {
                return result.Equals("included");
            }
            else
            {
                return result.Equals("unincluded");
            }
        }

        public IWcfDuplexService_CallbackDebugBehavior_Callback Callback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IWcfDuplexService_CallbackDebugBehavior_Callback>();
            }
        }
    }

    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WcfDuplexService_CallbackErrorHandler : IWcfDuplexService_CallbackErrorHandler
    {
        public bool Hello(string greeting)
        {
            bool result = false;
            try
            {
                Callback.ReplyThrow(greeting);
            }
            catch (FaultException<CustomMessage> fex)
            {
                if(fex.Message.Equals("custom fault reason") && fex.Code.Name.Equals("custom fault code"))
                {
                    result = true;
                }
            }
            catch (Exception)
            {  
            }

            return result;
        }

        public IWcfDuplexService_CallbackErrorHandler_Callback Callback
        {
            get
            {
                return OperationContext.Current.GetCallbackChannel<IWcfDuplexService_CallbackErrorHandler_Callback>();
            }
        }
    }
}
