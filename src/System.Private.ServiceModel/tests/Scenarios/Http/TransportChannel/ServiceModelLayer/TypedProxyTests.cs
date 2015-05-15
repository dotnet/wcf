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

public static class Http_TransportChannel_ServiceModelLayer_TypedProxyTests
{
    [Fact]
    [OuterLoop]
    public static void TypedProxy_AsyncBeginEnd_Call()
    {
        // This test verifies a typed proxy can call a service operation asynchronously using Begin/End
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            ChannelFactory<IWcfServiceBeginEndGenerated> factory = new ChannelFactory<IWcfServiceBeginEndGenerated>(customBinding, new EndpointAddress(BaseAddress.HttpBaseAddress));
            IWcfServiceBeginEndGenerated serviceProxy = factory.CreateChannel();
            string result = null;
            ManualResetEvent waitEvent = new ManualResetEvent(false);

            // The callback is optional with this Begin call, but we want to test that it works.
            // This delegate should execute when the call has completed, and that is how it gets the result of the call.
            AsyncCallback callback = (iar) =>
            {
                result = serviceProxy.EndEcho(iar);
                waitEvent.Set();
            };

            IAsyncResult ar = serviceProxy.BeginEcho("Hello", callback, null);

            // This test requires the callback to be called.
            // An actual timeout should call the callback, but we still set
            // a maximum wait time in case that does not happen.
            bool success = waitEvent.WaitOne(TestHelpers.TestTimeout);
            if (!success)
            {
                errorBuilder.AppendLine("AsyncCallback was not called.");
            }

            if (!string.Equals(result, "Hello"))
            {
                errorBuilder.AppendLine(String.Format("Expected response from Service: {0} Actual was: {1}", "Hello", result));
            }

            factory.Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: TypedProxyAsyncBeginEndCall FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void TypedProxy_AsyncBeginEnd_Call_WithNoCallback()
    {
        // This test verifies a typed proxy can call a service operation asynchronously using Begin/End
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            ChannelFactory<IWcfServiceBeginEndGenerated> factory = new ChannelFactory<IWcfServiceBeginEndGenerated>(customBinding, new EndpointAddress(BaseAddress.HttpBaseAddress));
            IWcfServiceBeginEndGenerated serviceProxy = factory.CreateChannel();
            string result = null;

            IAsyncResult ar = serviceProxy.BeginEcho("Hello", null, null);
            // An actual timeout should complete the ar, but we still set
            // a maximum wait time in case that does not happen.
            bool success = ar.AsyncWaitHandle.WaitOne(TestHelpers.TestTimeout);
            if (success)
            {
                result = serviceProxy.EndEcho(ar);
            }
            else
            {
                errorBuilder.AppendLine("AsyncCallback was not called.");
            }

            if (!string.Equals(result, "Hello"))
            {
                errorBuilder.AppendLine(String.Format("Expected response from Service: {0} Actual was: {1}", "Hello", result));
            }

            factory.Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: TypedProxyAsyncBeginEndNoCallbackCall FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void TypedProxy_AsyncBeginEnd_Call_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => Http_TransportChannel_ServiceModelLayer_TypedProxyTests.TypedProxy_AsyncBeginEnd_Call(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(TestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: TypedProxyAsyncBeginEndCallWithSingleThreadedSyncContext FAILED");
    }

    [Fact]
    [OuterLoop]
    public static void TypedProxy_AsyncTask_Call()
    {
        // This test verifies a typed proxy can call a service operation asynchronously using Task<string>
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            ChannelFactory<IWcfServiceGenerated> factory = new ChannelFactory<IWcfServiceGenerated>(customBinding, new EndpointAddress(BaseAddress.HttpBaseAddress));
            IWcfServiceGenerated serviceProxy = factory.CreateChannel();

            Task<string> task = serviceProxy.EchoAsync("Hello");
            string result = task.Result;
            if (!string.Equals(result, "Hello"))
            {
                errorBuilder.AppendLine(String.Format("Expected response from Service: {0} Actual was: {1}", "Hello", result));
            }

            factory.Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: TypedProxyAsyncTaskCall FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void TypedProxy_AsyncTask_Call_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => Http_TransportChannel_ServiceModelLayer_TypedProxyTests.TypedProxy_AsyncTask_Call(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(TestHelpers.TestTimeout);

        Assert.True(success, "Test Scenario: TypedProxyAsyncTaskCallWithSingleThreadedSyncContext FAILED");
    }

    [Fact]
    [OuterLoop]
    public static void TypedProxy_Synchronous_Call()
    {
        // This test verifies a typed proxy can call a service operation synchronously
        StringBuilder errorBuilder = new StringBuilder();
        try
        {
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            // Note the service interface used.  It was manually generated with svcutil.
            ChannelFactory<IWcfServiceGenerated> factory = new ChannelFactory<IWcfServiceGenerated>(customBinding, new EndpointAddress(BaseAddress.HttpBaseAddress));
            IWcfServiceGenerated serviceProxy = factory.CreateChannel();

            string result = serviceProxy.Echo("Hello");
            if (!string.Equals(result, "Hello"))
            {
                errorBuilder.AppendLine(String.Format("Expected response from Service: {0} Actual was: {1}", "Hello", result));
            }

            factory.Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: TypedProxySynchronousCall FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void TypedProxy_Synchronous_Call_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => Http_TransportChannel_ServiceModelLayer_TypedProxyTests.TypedProxy_Synchronous_Call(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(TestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: TypedProxySynchronousCallWithSingleThreadedSyncContext FAILED");
    }

    [Fact]
    [OuterLoop]
    public static void TypedProxy_Task_Call_WithSyncContext_ContinuesOnSameThread()
    {
        // This test verifies a task based call to a service operation continues on the same thread
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            ChannelFactory<IWcfServiceGenerated> factory = new ChannelFactory<IWcfServiceGenerated>(customBinding, new EndpointAddress(BaseAddress.HttpBaseAddress));
            IWcfServiceGenerated serviceProxy = factory.CreateChannel();
            string result = String.Empty;

            bool success = Task.Run(() =>
            {
                SingleThreadSynchronizationContext.Run(async delegate
                {
                    int startThread = Environment.CurrentManagedThreadId; //!!!! Use of System.Environment pulls in the contract "System.Runtime.Extensions", is this really needed?
                    result = await serviceProxy.EchoAsync("Hello");
                    if (startThread != Environment.CurrentManagedThreadId)
                    {
                        errorBuilder.AppendLine(String.Format("Expected continuation to happen on thread {0} but actually continued on thread {1}",
                            startThread, Environment.CurrentManagedThreadId));
                    }
                });
            }).Wait(TestHelpers.TestTimeout);

            if (!success)
            {
                errorBuilder.AppendLine(String.Format("Test didn't complete within the expected time"));
            }

            if (!string.Equals(result, "Hello"))
            {
                errorBuilder.AppendLine(String.Format("Expected response from Service: {0} Actual was: {1}", "Hello", result));
            }

            factory.Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: TaskCallWithSynchContextContinuesOnSameThread FAILED with the following errors: {0}", errorBuilder));
    }
}
