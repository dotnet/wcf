// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Xunit;
using Infrastructure.Common;

public class DuplexClientBaseTests : ConditionalWcfTest
{

    [ConditionalFact(nameof(Root_Certificate_Installed))]
    [OuterLoop]
    public static void DuplexClientBaseOfT_OverHttp_Call_Throws_InvalidOperation()
    {
        DuplexClientBase<IWcfDuplexService> duplexService = null;
        IWcfDuplexService proxy = null;

        try
        {
            // *** SETUP *** \\
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None); 

            WcfDuplexServiceCallback callbackService = new WcfDuplexServiceCallback();
            InstanceContext context = new InstanceContext(callbackService);

            duplexService = new MyDuplexClientBase<IWcfDuplexService>(context, binding, new EndpointAddress(Endpoints.Https_DefaultBinding_Address));

            // *** EXECUTE *** \\
            var exception = Assert.Throws<InvalidOperationException>(() => 
            { 
                proxy = duplexService.ChannelFactory.CreateChannel();
            });

            // *** VALIDATE *** \\
            // Can't compare the entire exception message - .NET Native doesn't necessarily output the entire message, just params
            // "Contract requires Duplex, but Binding 'BasicHttpBinding' doesn't support it or isn't configured properly to support it"
            Assert.True(exception.Message.Contains("BasicHttpBinding"));

            Assert.Throws<CommunicationObjectFaultedException>(() => 
            {
                // You can't gracefully close a Faulted CommunicationObject, so we should make sure it throws here too
                ((ICommunicationObject)duplexService).Close();
            });

            // *** CLEANUP *** \\
            // proxy will be null here so can't close.
            // duplexService is closed prior to this as part of an assert to verify an expected exception.
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)proxy, duplexService);
        }
    }

    [Fact]
    [OuterLoop]
    public static void DuplexClientBaseOfT_OverNetTcp_Synchronous_Call()
    {
        DuplexClientBase<IWcfDuplexService> duplexService = null;
        IWcfDuplexService proxy = null;
        Guid guid = Guid.NewGuid();

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;

            WcfDuplexServiceCallback callbackService = new WcfDuplexServiceCallback();
            InstanceContext context = new InstanceContext(callbackService);

            duplexService = new MyDuplexClientBase<IWcfDuplexService>(context, binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_Callback_Address));
            proxy = duplexService.ChannelFactory.CreateChannel();

            // *** EXECUTE *** \\
            // Ping on another thread.
            Task.Run(() => proxy.Ping(guid));
            Guid returnedGuid = callbackService.CallbackGuid;

            // *** VALIDATE *** \\
            Assert.True(guid == returnedGuid, 
                string.Format("The sent GUID does not match the returned GUID. Sent '{0}', Received: '{1}'", guid, returnedGuid));

            // *** CLEANUP *** \\
            ((ICommunicationObject)duplexService).Close();
            ((ICommunicationObject)proxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)proxy, (ICommunicationObject)duplexService);
        }
    }
}
