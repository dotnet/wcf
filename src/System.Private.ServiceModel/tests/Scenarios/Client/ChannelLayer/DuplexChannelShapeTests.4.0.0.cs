// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Infrastructure.Common;
using Xunit;

public partial class DuplexChannelShapeTests : ConditionalWcfTest
{
    // Creating a ChannelFactory using a binding's 'BuildChannelFactory' method and providing a channel shape...
    //       returns a concrete type determined by the channel shape requested and other binding related settings.
    // The tests in this file use the IDuplexChannel shape.

    private const string action = "http://tempuri.org/IWcfService/MessageRequestReply";
    private const string clientMessage = "[client] This is my request.";

    [WcfFact]
    [OuterLoop]
    public static void IDuplexSessionChannel_Tcp_NetTcpBinding()
    {
        IChannelFactory<IDuplexSessionChannel> factory = null;
        IDuplexSessionChannel channel = null;
        Message replyMessage = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            // Create the channel factory
            factory = binding.BuildChannelFactory<IDuplexSessionChannel>(new BindingParameterCollection());
            factory.Open();

            // Create the channel.
            channel = factory.CreateChannel(new EndpointAddress(Endpoints.Tcp_NoSecurity_Address));
            channel.Open();

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));
            requestMessage.Headers.MessageId = new UniqueId(Guid.NewGuid());

            // *** EXECUTE *** \\
            // Send the Message and receive the Response.
            channel.Send(requestMessage);
            replyMessage = channel.Receive(TimeSpan.FromSeconds(5));

            // *** VALIDATE *** \\
            // If the incoming Message did not contain the same UniqueId used for the MessageId of the outgoing Message we would have received a Fault from the Service
            Assert.Equal(requestMessage.Headers.MessageId.ToString(), replyMessage.Headers.RelatesTo.ToString());

            // Validate the Response
            var replyReader = replyMessage.GetReaderAtBodyContents();
            string actualResponse = replyReader.ReadElementContentAsString();
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";
            Assert.Equal(expectedResponse, actualResponse);

            // *** CLEANUP *** \\
            replyMessage.Close();
            channel.Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects(channel, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void IDuplexSessionChannel_Async_Tcp_NetTcpBinding()
    {
        IChannelFactory<IDuplexSessionChannel> factory = null;
        IDuplexSessionChannel channel = null;
        Message replyMessage = null;

        try
        {
            // *** SETUP *** \\
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            // Create the channel factory
            factory = binding.BuildChannelFactory<IDuplexSessionChannel>(new BindingParameterCollection());
            Task.Factory.FromAsync(factory.BeginOpen, factory.EndOpen, TaskCreationOptions.None).GetAwaiter().GetResult();

            // Create the channel.
            channel = factory.CreateChannel(new EndpointAddress(Endpoints.Tcp_NoSecurity_Address));
            Task.Factory.FromAsync(channel.BeginOpen, channel.EndOpen, TaskCreationOptions.None).GetAwaiter().GetResult();

            // Create the Message object to send to the service.
            Message requestMessage = Message.CreateMessage(
                binding.MessageVersion,
                action,
                new CustomBodyWriter(clientMessage));
            requestMessage.Headers.MessageId = new UniqueId(Guid.NewGuid());

            // *** EXECUTE *** \\
            // Send the Message and receive the Response.
            Task.Factory.FromAsync((asyncCallback, o) => channel.BeginSend(requestMessage, asyncCallback, o),
                channel.EndSend,
                TaskCreationOptions.None).GetAwaiter().GetResult();
            replyMessage = Task.Factory.FromAsync(channel.BeginReceive, channel.EndReceive, TaskCreationOptions.None).GetAwaiter().GetResult();

            // *** VALIDATE *** \\
            // If the incoming Message did not contain the same UniqueId used for the MessageId of the outgoing Message we would have received a Fault from the Service
            Assert.Equal(requestMessage.Headers.MessageId.ToString(), replyMessage.Headers.RelatesTo.ToString());

            // Validate the Response
            var replyReader = replyMessage.GetReaderAtBodyContents();
            string actualResponse = replyReader.ReadElementContentAsString();
            string expectedResponse = "[client] This is my request.[service] Request received, this is my Reply.";
            Assert.Equal(expectedResponse, actualResponse);

            // *** CLEANUP *** \\
            replyMessage.Close();
            Task.Factory.FromAsync(channel.BeginClose, channel.EndClose, TaskCreationOptions.None).GetAwaiter().GetResult();
            Task.Factory.FromAsync(factory.BeginClose, factory.EndClose, TaskCreationOptions.None).GetAwaiter().GetResult();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects(channel, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static async void CallbackBehavior_ConcurrencyMode_Single_NetTcpBindingAsync()
    {
        NetTcpBinding binding;
        InstanceContext instanceContext;
        DuplexChannelFactory<IWcfDuplexService_CallbackConcurrencyMode> factory;
        IWcfDuplexService_CallbackConcurrencyMode channel;

        // *** SETUP *** \\
        binding = new NetTcpBinding(SecurityMode.None);
        var imp = new CallbackHandler_ConcurrencyMode_Single(new ManualResetEvent(false));
        instanceContext = new InstanceContext(imp);
        factory = new DuplexChannelFactory<IWcfDuplexService_CallbackConcurrencyMode>(instanceContext, binding, Endpoints.DuplexCallbackConcurrencyMode_Address);

        // *** EXECUTE *** \\
        channel = factory.CreateChannel();
        Task task = channel.DoWorkAsync();

        // *** VALIDATE *** \\
        Assert.True(imp.MyManualResetEvent.WaitOne(20000));
        Assert.Equal(1, imp.Counter);
        await task;

        // *** CLEANUP *** \\
        ((ICommunicationObject)channel).Close();
        factory.Close();
    }

    [WcfFact]
    [OuterLoop]
    public static async void CallbackBehavior_ConcurrencyMode_Multiple_NetTcpBinding()
    {
        NetTcpBinding binding;
        InstanceContext instanceContext;
        DuplexChannelFactory<IWcfDuplexService_CallbackConcurrencyMode> factory;
        IWcfDuplexService_CallbackConcurrencyMode channel;

        // *** SETUP *** \\
        binding = new NetTcpBinding(SecurityMode.None);
        var imp = new CallbackHandler_ConcurrencyMode_Multiple(new ManualResetEvent(false));
        instanceContext = new InstanceContext(imp);
        factory = new DuplexChannelFactory<IWcfDuplexService_CallbackConcurrencyMode>(instanceContext, binding, Endpoints.DuplexCallbackConcurrencyMode_Address);

        // *** EXECUTE *** \\
        channel = factory.CreateChannel();
        Task task = channel.DoWorkAsync();

        // *** VALIDATE *** \\
        Assert.True(imp.ManualResetEvent.WaitOne(20000));
        Assert.Equal(2, imp.Counter);
        await task;

        // *** CLEANUP *** \\
        ((ICommunicationObject)channel).Close();
        factory.Close();
    }

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single, UseSynchronizationContext = false)]
    internal class CallbackHandler_ConcurrencyMode_Single : IWcfDuplexService_CallbackConcurrencyMode_Callback
    {
        public int Counter = 0;
        public ManualResetEvent MyManualResetEvent;

        public CallbackHandler_ConcurrencyMode_Single(ManualResetEvent manualResetEvent)
        {
            MyManualResetEvent = manualResetEvent;
        }

        public async Task CallWithWaitAsync(int delayTime)
        {
            Interlocked.Increment(ref Counter);
            await Task.Delay(delayTime);
            MyManualResetEvent.Set();
            // Need time between setting MRE and the next call incrementing counter.
            await Task.Delay(100);
        }
    }

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    internal class CallbackHandler_ConcurrencyMode_Multiple : IWcfDuplexService_CallbackConcurrencyMode_Callback
    {
        public int Counter = 0;
        public ManualResetEvent ManualResetEvent;

        public CallbackHandler_ConcurrencyMode_Multiple(ManualResetEvent manualResetEvent)
        {
            ManualResetEvent = manualResetEvent;
        }

        public async Task CallWithWaitAsync(int delayTime)
        {
            Interlocked.Increment(ref Counter);
            await Task.Delay(delayTime);
            ManualResetEvent.Set();
            // Need time between setting MRE and the next call incrementing counter.
            await Task.Delay(100);
        }
    }
}
