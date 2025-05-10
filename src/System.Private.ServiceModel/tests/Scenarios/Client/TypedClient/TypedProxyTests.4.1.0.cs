// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using TestTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public partial class TypedProxyTests
{
    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_NetTcpBinding_AsyncBeginEnd_Call_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => TypedProxyTests.ServiceContract_TypedProxy_NetTcpBinding_AsyncBeginEnd_Call(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: ServiceContract_TypedProxy_NetTcpBinding_AsyncBeginEnd_Call_WithSingleThreadedSyncContext timed out");
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy__NetTcpBinding_AsyncTask_Call_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => TypedProxyTests.ServiceContract_TypedProxy_NetTcpBinding_AsyncTask_Call(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);

        Assert.True(success, "Test Scenario: ServiceContract_TypedProxy__NetTcpBinding_AsyncTask_Call_WithSingleThreadedSyncContext timed out");
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_DuplexCallback()
    {
        NetTcpBinding binding = null;
        DuplexChannelFactory<IDuplexChannelService> factory = null;
        Guid guid = Guid.NewGuid();
        DuplexChannelServiceCallback callbackService = null;
        InstanceContext context = null;
        EndpointAddress endpointAddress = null;
        IDuplexChannelService serviceProxy = null;

        try
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            callbackService = new DuplexChannelServiceCallback();
            context = new InstanceContext(callbackService);
            endpointAddress = new EndpointAddress(Endpoints.Tcp_NoSecurity_DuplexCallback_Address);
            factory = new DuplexChannelFactory<IDuplexChannelService>(context, binding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            serviceProxy.Ping(guid);
            Guid returnedGuid = callbackService.CallbackGuid;

            // *** VALIDATE *** \\
            Assert.True(guid == returnedGuid, String.Format("The sent GUID does not match the returned GUID. Sent: {0} Received: {1}", guid, returnedGuid));

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
