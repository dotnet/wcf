// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
using CoreWCF.Description;
using CoreWCF.Dispatcher;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
#else
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
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

    // Service implementation for ServerInitiatedShutdownService (dotnet/wcf#5803 regression coverage).
    // On RequestServerShutdown the service gracefully closes the *current session's* output channel
    // (sends the NetFraming EndRecord) so the idle duplex client's receive pump observes end-of-stream
    // and runs DecrementActivity. This reproduces the user-reported scenario where a host shuts down
    // while a session-ful client is idle, without disturbing other sessions on the same host.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, AddressFilterMode = AddressFilterMode.Any)]
    public class ServerInitiatedShutdownService : IServerInitiatedShutdownService
    {
        public string Ping(string text)
        {
            return "Pong: " + text;
        }

        public string RequestServerShutdown()
        {
            IClientChannel channel = CaptureChannelServiceBehavior.GetCurrentChannel();
            Task.Run(async () =>
            {
                await Task.Delay(250);
                ISessionChannel<IDuplexSession> duplex = channel as ISessionChannel<IDuplexSession>;
                try
                {
                    if (duplex != null)
                    {
#if NET
                        await duplex.Session.CloseOutputSessionAsync();
#else
                        duplex.Session.CloseOutputSession();
#endif
                    }
                    else if (channel != null)
                    {
#if NET
                        await ((ICommunicationObject)channel).CloseAsync();
#else
                        ((ICommunicationObject)channel).Close();
#endif
                    }
                }
                catch
                {
                    try { if (channel != null) channel.Abort(); } catch { }
                }
            });
            return "Server shutting down";
        }
    }

    // Captures the per-session IClientChannel into OperationContext.Extensions so the service
    // operation can act on it (e.g., to gracefully close that session's output channel).
    internal sealed class CaptureChannelServiceBehavior : IServiceBehavior, IDispatchMessageInspector
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase,
            Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters) { }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase) { }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher cd in serviceHostBase.ChannelDispatchers)
            {
                foreach (EndpointDispatcher ed in cd.Endpoints)
                {
                    ed.DispatchRuntime.MessageInspectors.Add(this);
                }
            }
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            OperationContext ctx = OperationContext.Current;
            if (ctx != null && ctx.Extensions.Find<ChannelHolder>() == null)
            {
                ctx.Extensions.Add(new ChannelHolder(channel));
            }
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState) { }

        public static IClientChannel GetCurrentChannel()
        {
            OperationContext ctx = OperationContext.Current;
            if (ctx == null) return null;
            ChannelHolder holder = ctx.Extensions.Find<ChannelHolder>();
            return holder == null ? null : holder.Channel;
        }

        private sealed class ChannelHolder : IExtension<OperationContext>
        {
            private readonly IClientChannel _channel;
            public IClientChannel Channel { get { return _channel; } }
            public ChannelHolder(IClientChannel channel) { _channel = channel; }
            public void Attach(OperationContext owner) { }
            public void Detach(OperationContext owner) { }
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
