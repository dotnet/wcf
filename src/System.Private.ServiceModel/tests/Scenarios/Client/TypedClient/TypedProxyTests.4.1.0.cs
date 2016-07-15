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
using Xunit;

public static partial class TypedProxyTests
{    
    [Fact]
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
    
    [Fact]
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
    
    [Fact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_DuplexCallback()
    {
        DuplexChannelFactory<IDuplexChannelService> factory = null;
        StringBuilder errorBuilder = new StringBuilder();
        Guid guid = Guid.NewGuid();

        try
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;

            DuplexChannelServiceCallback callbackService = new DuplexChannelServiceCallback();
            InstanceContext context = new InstanceContext(callbackService);

            factory = new DuplexChannelFactory<IDuplexChannelService>(context, binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_DuplexCallback_Address));
            IDuplexChannelService serviceProxy = factory.CreateChannel();

            serviceProxy.Ping(guid);
            Guid returnedGuid = callbackService.CallbackGuid;

            if (guid != returnedGuid)
            {
                errorBuilder.AppendLine(String.Format("The sent GUID does not match the returned GUID. Sent: {0} Received: {1}", guid, returnedGuid));
            }

            factory.Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
            for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
            {
                errorBuilder.AppendLine(String.Format("Inner exception: {0}", innerException.ToString()));
            }
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }

        if (errorBuilder.Length != 0)
        {
            Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: ServiceContract_TypedProxy_DuplexCallback FAILED with the following errors: {0}", errorBuilder));
        }
    }
}
