// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Xunit;

public static class DuplexClientBaseTests
{
    static TimeSpan maxTestWaitTime = TimeSpan.FromSeconds(10);

    [Fact]
    [OuterLoop]
    public static void DuplexClientBaseOfT_OverHttp_Call_Throws_InvalidOperation()
    {
        DuplexClientBase<IWcfDuplexService> duplexService = null;
        Guid guid = Guid.NewGuid();

        try
        {
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.None); 

            WcfDuplexServiceCallback callbackService = new WcfDuplexServiceCallback();
            InstanceContext context = new InstanceContext(callbackService);

            duplexService = new MyDuplexClientBase<IWcfDuplexService>(context, binding, new EndpointAddress(Endpoints.Https_DefaultBinding_Address));

            var exception = Assert.Throws<InvalidOperationException>(() => 
            { 
                IWcfDuplexService proxy = duplexService.ChannelFactory.CreateChannel();
            });

            // Can't compare the entire exception message - .NET Native doesn't necessarily output the entire message, just params
            // "Contract requires Duplex, but Binding 'BasicHttpBinding' doesn't support it or isn't configured properly to support it"
            Assert.True(exception.Message.Contains("BasicHttpBinding"));

            Assert.Throws<CommunicationObjectFaultedException>(() => 
            {
                // You can't gracefully close a Faulted CommunicationObject, so we should make sure it throws here too
                ((ICommunicationObject)duplexService).Close();
            }); 
        }
        finally
        {
            if (duplexService != null && duplexService.State != CommunicationState.Closed)
            {
                duplexService.Abort();
            }
        }
    }

    [Fact]
    [OuterLoop]
    public static void DuplexClientBaseOfT_OverNetTcp_Synchronous_Call()
    {
        DuplexClientBase<IWcfDuplexService> duplexService = null;
        Guid guid = Guid.NewGuid();

        try
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;

            WcfDuplexServiceCallback callbackService = new WcfDuplexServiceCallback();
            InstanceContext context = new InstanceContext(callbackService);

            duplexService = new MyDuplexClientBase<IWcfDuplexService>(context, binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_Callback_Address));
            IWcfDuplexService proxy = duplexService.ChannelFactory.CreateChannel();

            // Ping on another thread.
            Task.Run(() => proxy.Ping(guid));
            Guid returnedGuid = callbackService.CallbackGuid;

            Assert.True(guid == returnedGuid, 
                string.Format("The sent GUID does not match the returned GUID. Sent '{0}', Received: '{1}'", guid, returnedGuid)); 

            ((ICommunicationObject)duplexService).Close();
        }
        finally
        {
            if (duplexService != null && duplexService.State != CommunicationState.Closed)
            {
                duplexService.Abort();
            }
        }
    }

    public class WcfDuplexServiceCallback : IWcfDuplexServiceCallback
    {
        private TaskCompletionSource<Guid> _tcs;

        public WcfDuplexServiceCallback()
        {
            _tcs = new TaskCompletionSource<Guid>();
        }

        public Guid CallbackGuid
        {
            get
            {
                if (_tcs.Task.Wait(maxTestWaitTime))
                {
                    return _tcs.Task.Result;
                }
                throw new TimeoutException(string.Format("Not completed within the alloted time of {0}", maxTestWaitTime));
            }
        }

        public void OnPingCallback(Guid guid)
        {
            // Set the result in an async task with a 100ms delay to prevent a race condition
            // where the OnPingCallback hasn't sent the reply to the server before the channel is closed.
            Task.Run(async () =>
            {
                await Task.Delay(100);
                _tcs.SetResult(guid);
            });
        }
    }
}

