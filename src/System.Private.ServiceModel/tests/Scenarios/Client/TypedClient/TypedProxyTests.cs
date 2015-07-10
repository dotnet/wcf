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

public static class TypedProxyTests
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

    [Fact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncBeginEnd_Call()
    {
        CustomBinding customBinding = new CustomBinding();
        customBinding.Elements.Add(new TextMessageEncodingBindingElement());
        customBinding.Elements.Add(new HttpTransportBindingElement());
        ServiceContract_TypedProxy_AsyncBeginEnd_Call(customBinding, Endpoints.DefaultCustomHttp_Address, "ServiceContract_TypedProxy_AsyncBeginEnd_Call");
    }

    [Fact]
    [ActiveIssue(78)]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_NetTcpBinding_AsyncBeginEnd_Call()
    {
        NetTcpBinding netTcpBinding = new NetTcpBinding(SecurityMode.None);
        ServiceContract_TypedProxy_AsyncBeginEnd_Call(netTcpBinding, Endpoints.Tcp_NoSecurity_Address, "ServiceContract_TypedProxy_NetTcpBinding_AsyncBeginEnd_Call");
    }

    [Fact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncBeginEnd_Call_WithNoCallback()
    {
        CustomBinding customBinding = new CustomBinding();
        customBinding.Elements.Add(new TextMessageEncodingBindingElement());
        customBinding.Elements.Add(new HttpTransportBindingElement());

        ServiceContract_TypedProxy_AsyncBeginEnd_Call_WithNoCallback(customBinding, Endpoints.DefaultCustomHttp_Address, "ServiceContract_TypedProxy_AsyncBeginEnd_Call_WithNoCallback");
    }

    [Fact]
    [ActiveIssue(78)]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_NetTcpBinding_AsyncBeginEnd_Call_WithNoCallback()
    {
        NetTcpBinding netTcpBinding = new NetTcpBinding(SecurityMode.None);
        ServiceContract_TypedProxy_AsyncBeginEnd_Call_WithNoCallback(netTcpBinding, Endpoints.Tcp_NoSecurity_Address, "ServiceContract_TypedProxy_NetTcpBinding_AsyncBeginEnd_Call_WithNoCallback");
    }

    [Fact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncBeginEnd_Call_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => TypedProxyTests.ServiceContract_TypedProxy_AsyncBeginEnd_Call(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(TestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: TypedProxy_AsyncBeginEnd_Call_WithSingleThreadedSyncContext timed out");
    }

    [Fact]
    [ActiveIssue(78)]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_NetTcpBinding_AsyncBeginEnd_Call_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => TypedProxyTests.ServiceContract_TypedProxy_NetTcpBinding_AsyncBeginEnd_Call(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(TestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: ServiceContract_TypedProxy_NetTcpBinding_AsyncBeginEnd_Call_WithSingleThreadedSyncContext timed out");
    }

    [Fact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncTask_Call()
    {
        CustomBinding customBinding = new CustomBinding();
        customBinding.Elements.Add(new TextMessageEncodingBindingElement());
        customBinding.Elements.Add(new HttpTransportBindingElement());

        ServiceContract_TypedProxy_AsyncTask_Call(customBinding, Endpoints.DefaultCustomHttp_Address, "ServiceContract_TypedProxy_AsyncTask_Call");
    }

    [Fact]
    [ActiveIssue(78)]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_NetTcpBinding_AsyncTask_Call()
    {
        NetTcpBinding netTcpBinding = new NetTcpBinding();
        ServiceContract_TypedProxy_AsyncTask_Call(netTcpBinding, Endpoints.Tcp_NoSecurity_Address, "ServiceContract_TypedProxy_NetTcpBinding_AsyncTask_Call");
    }

    [Fact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_AsyncTask_Call_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => TypedProxyTests.ServiceContract_TypedProxy_AsyncTask_Call(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(TestHelpers.TestTimeout);

        Assert.True(success, "Test Scenario: TypedProxy_AsyncTask_Call_WithSingleThreadedSyncContext timed out");
    }

    [Fact]
    [ActiveIssue(78)]
    [OuterLoop]
    public static void ServiceContract_TypedProxy__NetTcpBinding_AsyncTask_Call_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => TypedProxyTests.ServiceContract_TypedProxy_NetTcpBinding_AsyncTask_Call(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(TestHelpers.TestTimeout);

        Assert.True(success, "Test Scenario: ServiceContract_TypedProxy__NetTcpBinding_AsyncTask_Call_WithSingleThreadedSyncContext timed out");
    }

    [Fact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_Synchronous_Call()
    {
        // This test verifies a typed proxy can call a service operation synchronously
        StringBuilder errorBuilder = new StringBuilder();
        try
        {
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            // Note the service interface used.  It was manually generated with svcutil.
            ChannelFactory<IWcfServiceGenerated> factory = new ChannelFactory<IWcfServiceGenerated>(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
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
    public static void ServiceContract_TypedProxy_Synchronous_Call_WithSingleThreadedSyncContext()
    {
        bool success = Task.Run(() =>
        {
            TestTypes.SingleThreadSynchronizationContext.Run(() =>
            {
                Task.Factory.StartNew(() => TypedProxyTests.ServiceContract_TypedProxy_Synchronous_Call(), CancellationToken.None, TaskCreationOptions.None, TaskScheduler.FromCurrentSynchronizationContext()).Wait();
            });
        }).Wait(TestHelpers.TestTimeout);
        Assert.True(success, "Test Scenario: TypedProxy_Synchronous_Call_WithSingleThreadedSyncContext timed out");
    }

    [Fact]
    [OuterLoop]
    public static void ServiceContract_TypedProxy_Task_Call_WithSyncContext_ContinuesOnSameThread()
    {
        // This test verifies a task based call to a service operation continues on the same thread
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding customBinding = new CustomBinding();
            customBinding.Elements.Add(new TextMessageEncodingBindingElement());
            customBinding.Elements.Add(new HttpTransportBindingElement());

            ChannelFactory<IWcfServiceGenerated> factory = new ChannelFactory<IWcfServiceGenerated>(customBinding, new EndpointAddress(Endpoints.DefaultCustomHttp_Address));
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

    [Fact]
    [OuterLoop]
    public static void ChannelShape_TypedProxy_InvokeIRequestChannel()
    {
        string address = Endpoints.DefaultCustomHttp_Address;

        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding binding = new CustomBinding(new BindingElement[] {
                new TextMessageEncodingBindingElement(MessageVersion.Default, Encoding.UTF8),
                new HttpTransportBindingElement() });

            EndpointAddress endpointAddress = new EndpointAddress(address);

            // Create the channel factory for the request-reply message exchange pattern.
            var factory = new ChannelFactory<IRequestChannel>(binding, endpointAddress);

            // Create the channel.
            IRequestChannel channel = factory.CreateChannel();
            channel.Open();

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));

            // Send the Message and receive the Response.
            Message replyMessage = channel.Request(requestMessage);

            string replyMessageAction = replyMessage.Headers.Action;

            if (!string.Equals(replyMessageAction, action + "Response"))
            {
                errorBuilder.AppendLine(String.Format("A response was received from the Service but it was not the expected Action, expected: {0} actual: {1}", action + "Response", replyMessageAction));
            }

            var replyReader = replyMessage.GetReaderAtBodyContents();
            string actualResponse = replyReader.ReadElementContentAsString();
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";
            if (!string.Equals(actualResponse, expectedResponse))
            {
                errorBuilder.AppendLine(String.Format("Actual MessageBodyContent from service did not match the expected MessageBodyContent, expected: {0} actual: {1}", expectedResponse, actualResponse));
            }

            replyMessage.Close();
            channel.Close();
            factory.Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: InvokeRequestChannelViaProxy FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void ChannelShape_TypedProxy_InvokeIRequestChannelTimeout()
    {
        string address = Endpoints.DefaultCustomHttp_Address;
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding binding = new CustomBinding(new BindingElement[] {
                new TextMessageEncodingBindingElement(MessageVersion.Default, Encoding.UTF8),
                new HttpTransportBindingElement() });

            EndpointAddress endpointAddress = new EndpointAddress(address);

            // Create the channel factory for the request-reply message exchange pattern.
            var factory = new ChannelFactory<IRequestChannel>(binding, endpointAddress);

            // Create the channel.
            IRequestChannel channel = factory.CreateChannel();
            channel.Open();

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));

            // Send the Message and receive the Response.
            Message replyMessage = channel.Request(requestMessage, TimeSpan.FromSeconds(60));

            string replyMessageAction = replyMessage.Headers.Action;

            if (!string.Equals(replyMessageAction, action + "Response"))
            {
                errorBuilder.AppendLine(String.Format("A response was received from the Service but it was not the expected Action, expected: {0} actual: {1}", action + "Response", replyMessageAction));
            }


            var replyReader = replyMessage.GetReaderAtBodyContents();
            string actualResponse = replyReader.ReadElementContentAsString();
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";
            if (!string.Equals(actualResponse, expectedResponse))
            {
                errorBuilder.AppendLine(String.Format("Actual MessageBodyContent from service did not match the expected MessageBodyContent, expected: {0} actual: {1}", expectedResponse, actualResponse));
            }

            replyMessage.Close();
            channel.Close();
            factory.Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: InvokeIRequestChannelViaProxyTimeout FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [OuterLoop]
    public static void ChannelShape_TypedProxy_InvokeIRequestChannelAsync()
    {
        string address = Endpoints.DefaultCustomHttp_Address;
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            CustomBinding binding = new CustomBinding(new BindingElement[] {
                new TextMessageEncodingBindingElement(MessageVersion.Default, Encoding.UTF8),
                new HttpTransportBindingElement() });

            EndpointAddress endpointAddress = new EndpointAddress(address);

            // Create the channel factory for the request-reply message exchange pattern.
            var factory = new ChannelFactory<IRequestChannel>(binding, endpointAddress);

            // Create the channel.
            IRequestChannel channel = factory.CreateChannel();
            channel.Open();

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));

            // Send the Message and receive the Response.
            IAsyncResult ar = channel.BeginRequest(requestMessage, null, null);
            Message replyMessage = channel.EndRequest(ar);

            string replyMessageAction = replyMessage.Headers.Action;

            if (!string.Equals(replyMessageAction, action + "Response"))
            {
                errorBuilder.AppendLine(String.Format("A response was received from the Service but it was not the expected Action, expected: {0} actual: {1}", action + "Response", replyMessageAction));
            }

            var replyReader = replyMessage.GetReaderAtBodyContents();
            string actualResponse = replyReader.ReadElementContentAsString();
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";
            if (!string.Equals(actualResponse, expectedResponse))
            {
                errorBuilder.AppendLine(String.Format("Actual MessageBodyContent from service did not match the expected MessageBodyContent, expected: {0} actual: {1}", expectedResponse, actualResponse));
            }

            replyMessage.Close();
            channel.Close();
            factory.Close();
        }
        catch (Exception ex)
        {
            errorBuilder.AppendLine(String.Format("Unexpected exception was caught: {0}", ex.ToString()));
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: InvokeIRequestChannelViaProxyAsync FAILED with the following errors: {0}", errorBuilder));
    }

    [Fact]
    [ActiveIssue(90)]
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

            factory = new DuplexChannelFactory<IDuplexChannelService>(context, binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_Callback_Address));
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

    private static void ServiceContract_TypedProxy_AsyncBeginEnd_Call(Binding binding, string endpoint, string testName)
    {
        // Verifies a typed proxy can call a service operation asynchronously using Begin/End
        StringBuilder errorBuilder = new StringBuilder();
        try
        {
            ChannelFactory<IWcfServiceBeginEndGenerated> factory = new ChannelFactory<IWcfServiceBeginEndGenerated>(binding, new EndpointAddress(endpoint));
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

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: {0} FAILED with the following errors: {1}", testName, errorBuilder));
    }

    private static void ServiceContract_TypedProxy_AsyncBeginEnd_Call_WithNoCallback(Binding binding, string endpoint, string testName)
    {
        // This test verifies a typed proxy can call a service operation asynchronously using Begin/End
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            ChannelFactory<IWcfServiceBeginEndGenerated> factory = new ChannelFactory<IWcfServiceBeginEndGenerated>(binding, new EndpointAddress(endpoint));
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

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: {0} FAILED with the following errors: {1}", testName, errorBuilder));
    }

    private static void ServiceContract_TypedProxy_AsyncTask_Call(Binding binding, string endpoint, string testName)
    {
        // This test verifies a typed proxy can call a service operation asynchronously using Task<string>
        StringBuilder errorBuilder = new StringBuilder();

        try
        {
            ChannelFactory<IWcfServiceGenerated> factory = new ChannelFactory<IWcfServiceGenerated>(binding, new EndpointAddress(endpoint));
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
}
