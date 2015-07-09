// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TestTypes;
using Xunit;

public static class TypedProxyDuplexTests
{
    // ServiceContract typed proxy tests create a ChannelFactory using a provided [ServiceContract] Interface which...
    //       returns a generated proxy based on that Interface.
    // ChannelShape typed proxy tests create a ChannelFactory using a WCF understood channel shape which...
    //       returns a generated proxy based on the channel shape used, such as...
    //              IRequestChannel (for a request-reply message exchange pattern)
    //              IDuplexChannel (for a two-way duplex message exchange pattern)

    [Fact]
    [ActiveIssue(138)]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncTask_CallbackReturn()
    {
        DuplexChannelFactory<IWcfDuplexTaskReturnService> factory = null;
        Guid guid = Guid.NewGuid();

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.None;

        DuplexChannelServiceCallback callbackService = new DuplexChannelServiceCallback();
        InstanceContext context = new InstanceContext(callbackService);

        using (factory = new DuplexChannelFactory<IWcfDuplexTaskReturnService>(context, binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_Callback_Address)))
        {
            IWcfDuplexTaskReturnService serviceProxy = factory.CreateChannel();

            Task<Guid> task = serviceProxy.Ping(guid);

            Guid returnedGuid = task.Result;

            if (guid != returnedGuid)
            {
                Assert.True(false, String.Format("The sent GUID does not match the returned GUID. Sent: {0} Received: {1}", guid, returnedGuid));
            }

            factory.Close();

            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }

    public class DuplexTaskReturnServiceCallback : IWcfDuplexTaskReturnCallback
    {
        public Task<Guid> ServicePingCallback(Guid guid)
        {
            // This returns the guid to the service which called this callback.
            // We could return Task.FromResult(guid) but that means we could execute the 
            // completion on the same thread. But if someone is using a task it means they 
            // would potentially have the completion on another thread.
            return Task.Run<Guid>(() => guid);
        }
    }
}
