// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using TestTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public partial class TypedProxyTests : ConditionalWcfTest
{
    // ServiceContract typed proxy tests create a ChannelFactory using a provided [ServiceContract] Interface which...
    //       returns a generated proxy based on that Interface.
    // ChannelShape typed proxy tests create a ChannelFactory using a WCF understood channel shape which...
    //       returns a generated proxy based on the channel shape used, such as...
    //              IRequestChannel (for a request-reply message exchange pattern)
    //              IDuplexChannel (for a two-way duplex message exchange pattern)

    private const string action = "http://tempuri.org/IWcfService/MessageRequestReply";
    private const string clientMessage = "[client] This is my request.";
    static TimeSpan maxTestWaitTime = TimeSpan.FromSeconds(10);

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncBeginEnd_Call()
    {
        CustomBinding customBinding = new CustomBinding();
        customBinding.Elements.Add(new TextMessageEncodingBindingElement());
        customBinding.Elements.Add(new HttpTransportBindingElement());
        ServiceContract_TypedProxy_AsyncBeginEnd_Call_TestImpl(customBinding, Endpoints.DefaultCustomHttp_Address, "ServiceContract_TypedProxy_AsyncBeginEnd_Call");
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_NetTcpBinding_AsyncBeginEnd_Call()
    {
        NetTcpBinding netTcpBinding = new NetTcpBinding(SecurityMode.None);
        ServiceContract_TypedProxy_AsyncBeginEnd_Call_TestImpl(netTcpBinding, Endpoints.Tcp_NoSecurity_Address, "ServiceContract_TypedProxy_NetTcpBinding_AsyncBeginEnd_Call");
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncBeginEnd_Call_WithNoCallback()
    {
        CustomBinding customBinding = new CustomBinding();
        customBinding.Elements.Add(new TextMessageEncodingBindingElement());
        customBinding.Elements.Add(new HttpTransportBindingElement());

        ServiceContract_TypedProxy_AsyncBeginEnd_Call_WithNoCallback_TestImpl(customBinding, Endpoints.DefaultCustomHttp_Address, "ServiceContract_TypedProxy_AsyncBeginEnd_Call_WithNoCallback");
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_NetTcpBinding_AsyncBeginEnd_Call_WithNoCallback()
    {
        NetTcpBinding netTcpBinding = new NetTcpBinding(SecurityMode.None);
        ServiceContract_TypedProxy_AsyncBeginEnd_Call_WithNoCallback_TestImpl(netTcpBinding, Endpoints.Tcp_NoSecurity_Address, "ServiceContract_TypedProxy_NetTcpBinding_AsyncBeginEnd_Call_WithNoCallback");
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncBeginEnd_Call_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => TypedProxyTests.ServiceContract_TypedProxy_AsyncBeginEnd_Call(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: TypedProxy_AsyncBeginEnd_Call_WithSingleThreadedSyncContext timed out");
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncTask_Call()
    {
        CustomBinding customBinding = new CustomBinding();
        customBinding.Elements.Add(new TextMessageEncodingBindingElement());
        customBinding.Elements.Add(new HttpTransportBindingElement());

        ServiceContract_TypedProxy_AsyncTask_Call_TestImpl(customBinding, Endpoints.DefaultCustomHttp_Address, "ServiceContract_TypedProxy_AsyncTask_Call");
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_NetTcpBinding_AsyncTask_Call()
    {
        NetTcpBinding netTcpBinding = new NetTcpBinding();
        netTcpBinding.Security.Mode = SecurityMode.None;
        ServiceContract_TypedProxy_AsyncTask_Call_TestImpl(netTcpBinding, Endpoints.Tcp_NoSecurity_Address, "ServiceContract_TypedProxy_NetTcpBinding_AsyncTask_Call");
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncTask_Call_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => TypedProxyTests.ServiceContract_TypedProxy_AsyncTask_Call(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);

        Assert.True(success, "Test Scenario: TypedProxy_AsyncTask_Call_WithSingleThreadedSyncContext timed out");
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_Synchronous_Call()
    {
        // This test verifies a typed proxy can call a service operation synchronously
        CustomBinding customBinding = null;
        ChannelFactory<IWcfServiceGenerated> factory = null;
        EndpointAddress endpointAddress = null;
        IWcfServiceGenerated serviceProxy = null;
        string result = null;

        try
        {
            // *** SETUP *** \\
            customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());
            endpointAddress = new EndpointAddress(Endpoints.DefaultCustomHttp_Address);
            // Note the service interface used.  It was manually generated with svcutil.
            factory = new ChannelFactory<IWcfServiceGenerated>(customBinding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            result = serviceProxy.Echo("Hello");

            // *** VALIDATE *** \\
            Assert.True(String.Equals(result, "Hello"), String.Format("Expected response from Service: {0} Actual was: {1}", "Hello", result));

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

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_Synchronous_Call_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => TypedProxyTests.ServiceContract_TypedProxy_Synchronous_Call(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(ScenarioTestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: TypedProxy_Synchronous_Call_WithSingleThreadedSyncContext timed out");
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_Task_Call_WithSyncContext_ContinuesOnSameThread()
    {
        // This test verifies a task based call to a service operation continues on the same thread
        CustomBinding customBinding = null;
        ChannelFactory<IWcfServiceGenerated> factory = null;
        EndpointAddress endpointAddress = null;
        IWcfServiceGenerated serviceProxy = null;
        string result = null;

        try
        {
            // *** SETUP *** \\
            customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());
            endpointAddress = new EndpointAddress(Endpoints.DefaultCustomHttp_Address);
            factory = new ChannelFactory<IWcfServiceGenerated>(customBinding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            bool success = Task.Run(() =>
            {
                SingleThreadSynchronizationContext.Run(async delegate
                {
                    int startThread = Environment.CurrentManagedThreadId;
                    result = await serviceProxy.EchoAsync("Hello");

                    Assert.True(startThread == Environment.CurrentManagedThreadId, String.Format("Expected continuation to happen on thread {0} but actually continued on thread {1}",
                                                                                                    startThread, Environment.CurrentManagedThreadId));
                });
            }).Wait(ScenarioTestHelpers.TestTimeout);

            // *** VALIDATE *** \\
            Assert.True(success, String.Format("Test didn't complete within the expected time"));
            Assert.True(String.Equals(result, "Hello"), String.Format("Expected response from Service: {0} Actual was: {1}", "Hello", result));

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

    [WcfFact]
    [OuterLoop]
    public static void ChannelShape_TypedProxy_InvokeIRequestChannel()
    {
        CustomBinding customBinding = null;
        ChannelFactory<IRequestChannel> factory = null;
        EndpointAddress endpointAddress = null;
        IRequestChannel channel = null;
        Message replyMessage = null;
        string replyMessageAction = null;
        string actualResponse = null;

        try
        {
            // *** SETUP *** \\
            customBinding = new CustomBinding(new BindingElement[] {
                  new TextMessageEncodingBindingElement(MessageVersion.Default, Encoding.UTF8),
                  new HttpTransportBindingElement() });
            endpointAddress = new EndpointAddress(Endpoints.DefaultCustomHttp_Address);
            // Create the channel factory for the request-reply message exchange pattern.
            factory = new ChannelFactory<IRequestChannel>(customBinding, endpointAddress);
            // Create the channel.
            channel = factory.CreateChannel();
            channel.Open();
            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                customBinding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";

            // *** EXECUTE *** \\
            // Send the Message and receive the Response.
            replyMessage = channel.Request(requestMessage);
            replyMessageAction = replyMessage.Headers.Action;

            // *** VALIDATE *** \\
            Assert.True(String.Equals(replyMessageAction, action + "Response"),
                String.Format("A response was received from the Service but it was not the expected Action, expected: {0} actual: {1}",
                action + "Response", replyMessageAction));

            // *** EXECUTE *** \\
            var replyReader = replyMessage.GetReaderAtBodyContents();
            actualResponse = replyReader.ReadElementContentAsString();

            // *** VALIDATE *** \\
            Assert.True(String.Equals(actualResponse, expectedResponse),
                String.Format("Actual MessageBodyContent from service did not match the expected MessageBodyContent, expected: {0} actual: {1}",
                expectedResponse, actualResponse));

            // *** CLEANUP *** \\
            replyMessage.Close();
            channel.Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            replyMessage.Close();
            ScenarioTestHelpers.CloseCommunicationObjects(channel, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ChannelShape_TypedProxy_InvokeIRequestChannelTimeout()
    {
        CustomBinding customBinding = null;
        ChannelFactory<IRequestChannel> factory = null;
        EndpointAddress endpointAddress = null;
        IRequestChannel channel = null;
        Message requestMessage = null;
        Message replyMessage = null;
        string replyMessageAction = null;
        string actualResponse = null;

        try
        {
            // *** SETUP *** \\
            customBinding = new CustomBinding(new BindingElement[] {
                  new TextMessageEncodingBindingElement(MessageVersion.Default, Encoding.UTF8),
                  new HttpTransportBindingElement() });
            endpointAddress = new EndpointAddress(Endpoints.DefaultCustomHttp_Address);
            // Create the channel factory for the request-reply message exchange pattern.
            factory = new ChannelFactory<IRequestChannel>(customBinding, endpointAddress);
            // Create the channel.
            channel = factory.CreateChannel();
            channel.Open();
            // Create the Message object to send to the service.
            requestMessage = Message.CreateMessage(
                customBinding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";

            // *** EXECUTE *** \\
            // Send the Message and receive the Response.
            replyMessage = channel.Request(requestMessage, TimeSpan.FromSeconds(60));
            replyMessageAction = replyMessage.Headers.Action;

            // *** VALIDATE *** \\
            Assert.True(String.Equals(replyMessageAction, action + "Response"),
                String.Format("A response was received from the Service but it was not the expected Action, expected: {0} actual: {1}",
                action + "Response", replyMessageAction));

            // *** EXECUTE *** \\
            var replyReader = replyMessage.GetReaderAtBodyContents();
            actualResponse = replyReader.ReadElementContentAsString();

            // *** VALIDATE *** \\
            Assert.True(String.Equals(actualResponse, expectedResponse),
                String.Format("Actual MessageBodyContent from service did not match the expected MessageBodyContent, expected: {0} actual: {1}",
                expectedResponse, actualResponse));

            // *** CLEANUP *** \\
            replyMessage.Close();
            channel.Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            replyMessage.Close();
            ScenarioTestHelpers.CloseCommunicationObjects(channel, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void ChannelShape_TypedProxy_InvokeIRequestChannelAsync()
    {
        CustomBinding customBinding = null;
        ChannelFactory<IRequestChannel> factory = null;
        EndpointAddress endpointAddress = null;
        IRequestChannel channel = null;
        Message requestMessage = null;
        Message replyMessage = null;
        string replyMessageAction = null;
        string actualResponse = null;

        try
        {
            // *** SETUP *** \\
            customBinding = new CustomBinding(new BindingElement[] {
                  new TextMessageEncodingBindingElement(MessageVersion.Default, Encoding.UTF8),
                  new HttpTransportBindingElement() });
            endpointAddress = new EndpointAddress(Endpoints.DefaultCustomHttp_Address);
            // Create the channel factory for the request-reply message exchange pattern.
            factory = new ChannelFactory<IRequestChannel>(customBinding, endpointAddress);
            // Create the channel.
            channel = factory.CreateChannel();
            channel.Open();
            // Create the Message object to send to the service.
            requestMessage = Message.CreateMessage(
                customBinding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";

            // *** EXECUTE *** \\
            // Send the Message and receive the Response.
            IAsyncResult ar = channel.BeginRequest(requestMessage, null, null);
            replyMessage = channel.EndRequest(ar);
            replyMessageAction = replyMessage.Headers.Action;

            // *** VALIDATE *** \\
            Assert.True(String.Equals(replyMessageAction, action + "Response"),
                String.Format("A response was received from the Service but it was not the expected Action, expected: {0} actual: {1}",
                action + "Response", replyMessageAction));

            // *** EXECUTE *** \\
            var replyReader = replyMessage.GetReaderAtBodyContents();
            actualResponse = replyReader.ReadElementContentAsString();

            // *** VALIDATE *** \\
            Assert.True(String.Equals(actualResponse, expectedResponse),
                String.Format("Actual MessageBodyContent from service did not match the expected MessageBodyContent, expected: {0} actual: {1}",
                expectedResponse, actualResponse));

            // *** CLEANUP *** \\
            replyMessage.Close();
            channel.Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            replyMessage.Close();
            ScenarioTestHelpers.CloseCommunicationObjects(channel, factory);
        }
    }

    [CallbackBehavior(UseSynchronizationContext = false)]
    public class DuplexChannelServiceCallback : IDuplexChannelCallback
    {
        private TaskCompletionSource<Guid> _tcs;

        public DuplexChannelServiceCallback()
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
            _tcs.SetResult(guid);
        }
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

    private static void ServiceContract_TypedProxy_AsyncBeginEnd_Call_TestImpl(Binding binding, string endpoint, string testName)
    {
        // Verifies a typed proxy can call a service operation asynchronously using Begin/End
        ChannelFactory<IWcfServiceBeginEndGenerated> factory = null;
        EndpointAddress endpointAddress = null;
        IWcfServiceBeginEndGenerated serviceProxy = null;
        ManualResetEvent waitEvent = null;
        string result = null;

        try
        {
            // *** SETUP *** \\
            endpointAddress = new EndpointAddress(endpoint);
            factory = new ChannelFactory<IWcfServiceBeginEndGenerated>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();
            waitEvent = new ManualResetEvent(false);

            // *** EXECUTE *** \\
            // The callback is optional with this Begin call, but we want to test that it works.
            // This delegate should execute when the call has completed, and that is how it gets the result of the call.
            AsyncCallback callback = (iar) =>
            {
                result = serviceProxy.EndEcho(iar);
                waitEvent.Set();
            };

            IAsyncResult ar = serviceProxy.BeginEcho("Hello", callback, null);

            // *** VALIDATE *** \\
            // This test requires the callback to be called.
            // An actual timeout should call the callback, but we still set
            // a maximum wait time in case that does not happen.
            bool success = waitEvent.WaitOne(ScenarioTestHelpers.TestTimeout);
            Assert.True(success, String.Format("The AsyncCallback was not called. If the AsyncCallback had been called the waitEvent would have been set to 'True', but the value of waitEvent was: {0}", success));
            Assert.True(String.Equals(result, "Hello"), String.Format("Expected response from Service: {0} Actual was: {1}", "Hello", result));

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

    private static void ServiceContract_TypedProxy_AsyncBeginEnd_Call_WithNoCallback_TestImpl(Binding binding, string endpoint, string testName)
    {
        // This test verifies a typed proxy can call a service operation asynchronously using Begin/End
        ChannelFactory<IWcfServiceBeginEndGenerated> factory = null;
        EndpointAddress endpointAddress = null;
        IWcfServiceBeginEndGenerated serviceProxy = null;
        string result = null;

        try
        {
            // *** SETUP *** \\
            endpointAddress = new EndpointAddress(endpoint);
            factory = new ChannelFactory<IWcfServiceBeginEndGenerated>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            IAsyncResult ar = serviceProxy.BeginEcho("Hello", null, null);
            // An actual timeout should complete the ar, but we still set
            // a maximum wait time in case that does not happen.
            bool success = ar.AsyncWaitHandle.WaitOne(ScenarioTestHelpers.TestTimeout);

            // *** VALIDATE *** \\
            Assert.True(success, String.Format("The IAsyncResult was not called. If the IAsyncResult had been called the AsyncWaitHandle would have been set to 'True', but the value of AsyncWaitHandle was: {0}", success));

            // *** EXECUTE *** \\
            result = serviceProxy.EndEcho(ar);

            // *** VALIDATE *** \\
            Assert.True(String.Equals(result, "Hello"), String.Format("Expected response from Service: {0} Actual was: {1}", "Hello", result));

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

    private static void ServiceContract_TypedProxy_AsyncTask_Call_TestImpl(Binding binding, string endpoint, string testName)
    {
        // This test verifies a typed proxy can call a service operation asynchronously using Task<string>
        ChannelFactory<IWcfServiceGenerated> factory = null;
        EndpointAddress endpointAddress = null;
        IWcfServiceGenerated serviceProxy = null;
        string result = null;

        try
        {
            // *** SETUP *** \\
            endpointAddress = new EndpointAddress(endpoint);
            factory = new ChannelFactory<IWcfServiceGenerated>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            Task<string> task = serviceProxy.EchoAsync("Hello");
            result = task.Result;

            // *** VALIDATE *** \\
            Assert.True(String.Equals(result, "Hello"), String.Format("Expected response from Service: {0} Actual was: {1}", "Hello", result));

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

    [WcfFact]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static async Task ServiceContract_TypedProxy_Task_Call_AsyncLocal_NonCapture()
    {
        // This test verifies a task based call to a service operation doesn't capture any AsyncLocal's in an ExecutionContext
        // This is indirectly checking that the OperationContext won't get captured by a registered CancellationToken callback
        CustomBinding customBinding = null;
        ChannelFactory<IWcfServiceGenerated> factory = null;
        EndpointAddress endpointAddress = null;
        IWcfServiceGenerated serviceProxy = null;
        string requestString = "Hello";
        string result = null;
        WeakReference<OperationContextExtension> opContextReference = null;

        try
        {
            // *** SETUP *** \\
            customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());
            endpointAddress = new EndpointAddress(Endpoints.DefaultCustomHttp_Address);
            factory = new ChannelFactory<IWcfServiceGenerated>(customBinding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            var opExtension = new OperationContextExtension();
            opContextReference = new WeakReference<OperationContextExtension>(opExtension);
            using (new OperationContextScope((IContextChannel)serviceProxy))
            {
                OperationContext.Current.Extensions.Add(opExtension);
                result = await serviceProxy.EchoAsync(requestString);
            }

            opExtension = null;

            // The Task generated by the compiler for this method stores the current ExecutionContext when an await is hit.
            // This means even after we've nulled out the OperationContext via disposing the OperationContextScope, it's still
            // stored in the saved ExecutionContext which was captured by the Task. Forcing another async continuation via
            // Task.Yield() causes the stored ExecutionContext to be replace with the current one which doesn't reference
            // OperationContext any more. Without this, the OperationContext would be kept alive until the next await call,
            // and the weak reference would still hold a reference to the OperationContextExtension.
            await Task.Yield();

            // Force a Gen2 GC so that the WeakReference will no longer have the OperationContextExtension as a target.
            GC.Collect(2);

            // *** VALIDATE *** \\
            Assert.Equal(requestString, result);
            Assert.False(opContextReference.TryGetTarget(out OperationContextExtension opContext), "OperationContextExtension should have been collected");
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

    public class OperationContextExtension : IExtension<OperationContext>
    {
        public void Attach(OperationContext owner)
        {
            // Do nothing
        }
        public void Detach(OperationContext owner)
        {
            // Do nothing
        }
    }

}
