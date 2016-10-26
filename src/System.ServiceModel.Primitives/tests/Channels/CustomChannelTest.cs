// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using Infrastructure.Common;
using Xunit;
using System.Threading.Tasks;

public static class CustomChannelTest
{
    [WcfFact]
    public static void CustomChannel_Sync_Open_Close_Methods_Called()
    {
        MockChannelFactory<IRequestChannel> mockChannelFactory = null;
        MockRequestChannel mockRequestChannel = null;
        List<string> channelOpenMethodsCalled = new List<string>();
        List<string> channelCloseMethodsCalled = new List<string>();
        List<string> factoryOpenMethodsCalled = new List<string>();
        List<string> factoryCloseMethodsCalled = new List<string>();
        string testMessageBody = "CustomChannelTest_Sync";
        Message inputMessage = Message.CreateMessage(MessageVersion.Default, action: "Test", body: testMessageBody);

        // *** SETUP *** \\
        // Intercept the creation of the factory so we can intercept creation of the channel
        Func<Type, BindingContext, IChannelFactory> buildFactoryAction = (Type type, BindingContext context) =>
        {
            // Create the channel factory and intercept all open and close method calls
            mockChannelFactory = new MockChannelFactory<IRequestChannel>(context, new TextMessageEncodingBindingElement().CreateMessageEncoderFactory());
            MockCommunicationObject.InterceptAllOpenMethods(mockChannelFactory, factoryOpenMethodsCalled);
            MockCommunicationObject.InterceptAllCloseMethods(mockChannelFactory, factoryCloseMethodsCalled);

            // Override the OnCreateChannel call so we get the mock channel created by the factory
            mockChannelFactory.OnCreateChannelOverride = (EndpointAddress endpoint, Uri via) =>
            {
                // Create the mock channel and intercept all its open and close method calls
                mockRequestChannel = (MockRequestChannel) mockChannelFactory.DefaultOnCreateChannel(endpoint, via);
                MockCommunicationObject.InterceptAllOpenMethods(mockRequestChannel, channelOpenMethodsCalled);
                MockCommunicationObject.InterceptAllCloseMethods(mockRequestChannel, channelCloseMethodsCalled);
                return mockRequestChannel;
            };
            return mockChannelFactory;
        };

        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        mockBindingElement.BuildChannelFactoryOverride = buildFactoryAction;

        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<ICustomChannelServiceInterface>(binding, address);

        // We rely on the implicit open of the channel to be synchronous.
        // This is true for both the full framework and this NET Core version.
        ICustomChannelServiceInterface channel = factory.CreateChannel();

        // *** EXECUTE *** \\
        Message outputMessage = channel.Process(inputMessage);

        // The mock's default behavior is just to loopback what we sent.
        var result = outputMessage.GetBody<string>();

        // Explicitly close the channel factory synchronously.
        // One of the important aspects of this test is that a synchronous
        // close of the factory also synchronously closes the channel.
        factory.Close();

        // *** VALIDATE *** \\
        Assert.True(String.Equals(testMessageBody, result), 
                    String.Format("Expected body to be '{0}' but actual was '{1}'", testMessageBody, result));

        string expectedOpens = "OnOpening,OnOpen,OnOpened";
        string expectedCloses = "OnClosing,OnClose,OnClosed";

        string actualOpens = String.Join(",", channelOpenMethodsCalled);
        Assert.True(String.Equals(expectedOpens, actualOpens, StringComparison.Ordinal),
               String.Format("Expected channel open methods to be '{0}' but actual was '{1}'.",
                             expectedOpens, actualOpens));

        string actualCloses = String.Join(",", channelCloseMethodsCalled);
        Assert.True(String.Equals(expectedCloses, actualCloses, StringComparison.Ordinal),
               String.Format("Expected channel close methods to be '{0}' but actual was '{1}'.",
                             expectedCloses, actualCloses));

        actualOpens = String.Join(",", factoryOpenMethodsCalled);
        Assert.True(String.Equals(expectedOpens, actualOpens, StringComparison.Ordinal),
               String.Format("Expected factory open methods to be '{0}' but actual was '{1}'.",
                             expectedOpens, actualOpens));

        actualCloses = String.Join(",", factoryCloseMethodsCalled);
        Assert.True(String.Equals(expectedCloses, actualCloses, StringComparison.Ordinal),
               String.Format("Expected factory close methods to be '{0}' but actual was '{1}'.",
                             expectedCloses, actualCloses));

        Assert.True(factory.State == CommunicationState.Closed,
            String.Format("Expected factory's final state to be Closed but was '{0}'", factory.State));

        Assert.True(((ICommunicationObject)channel).State == CommunicationState.Closed,
            String.Format("Expected channel's final state to be Closed but was '{0}'", ((ICommunicationObject)channel).State));

    }

    [WcfFact]
    public static void CustomChannel_Async_Open_Close_Methods_Called()
    {
        MockChannelFactory<IRequestChannel> mockChannelFactory = null;
        MockRequestChannel mockRequestChannel = null;
        List<string> channelOpenMethodsCalled = new List<string>();
        List<string> channelCloseMethodsCalled = new List<string>();
        List<string> factoryOpenMethodsCalled = new List<string>();
        List<string> factoryCloseMethodsCalled = new List<string>();
        string testMessageBody = "CustomChannelTest_Async";
        Message inputMessage = Message.CreateMessage(MessageVersion.Default, action: "Test", body: testMessageBody);

        // *** SETUP *** \\
        // Intercept the creation of the factory so we can intercept creation of the channel
        Func<Type, BindingContext, IChannelFactory> buildFactoryAction = (Type type, BindingContext context) =>
        {
            // Create the channel factory and intercept all open and close method calls
            mockChannelFactory = new MockChannelFactory<IRequestChannel>(context, new TextMessageEncodingBindingElement().CreateMessageEncoderFactory());
            MockCommunicationObject.InterceptAllOpenMethods(mockChannelFactory, factoryOpenMethodsCalled);
            MockCommunicationObject.InterceptAllCloseMethods(mockChannelFactory, factoryCloseMethodsCalled);

                // Override the OnCreateChannel call so we get the mock channel created by the factory
                mockChannelFactory.OnCreateChannelOverride = (EndpointAddress endpoint, Uri via) =>
            {
                // Create the default mock channel and intercept all its open and close method calls
                mockRequestChannel = (MockRequestChannel)mockChannelFactory.DefaultOnCreateChannel(endpoint, via);
                MockCommunicationObject.InterceptAllOpenMethods(mockRequestChannel, channelOpenMethodsCalled);
                MockCommunicationObject.InterceptAllCloseMethods(mockRequestChannel, channelCloseMethodsCalled);

                return mockRequestChannel;
            };
            return mockChannelFactory;
        };

        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        mockBindingElement.BuildChannelFactoryOverride = buildFactoryAction;

        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<ICustomChannelServiceInterface>(binding, address);

        // Explicitly open factory asynchronously because the implicit open is synchronous
        IAsyncResult openResult = factory.BeginOpen(null, null);
        Task openTask = Task.Factory.FromAsync(openResult, factory.EndOpen);
        openTask.GetAwaiter().GetResult();

        ICustomChannelServiceInterface channel = factory.CreateChannel();

        // *** EXECUTE *** \\
        Task<Message> processTask = channel.ProcessAsync(inputMessage);

        // The mock's default behavior is just to loopback what we sent.
        Message outputMessage = processTask.GetAwaiter().GetResult();
        var result = outputMessage.GetBody<string>();

        // Explicitly close the channel factory asynchronously.
        // One of the important aspects of this test is that an asynchronous
        // close of the factory also asynchronously closes the channel.
        IAsyncResult asyncResult = factory.BeginClose(null, null);
        Task task = Task.Factory.FromAsync(asyncResult, factory.EndClose);
        task.GetAwaiter().GetResult();

        // *** VALIDATE *** \\
        Assert.True(String.Equals(testMessageBody, result),
                    String.Format("Expected body to be '{0}' but actual was '{1}'", testMessageBody, result));

        string expectedOpens = "OnOpening,OnBeginOpen,OnOpened";
        string expectedCloses = "OnClosing,OnBeginClose,OnClosed";

        string actualOpens = String.Join(",", channelOpenMethodsCalled);
        Assert.True(String.Equals(expectedOpens, actualOpens, StringComparison.Ordinal),
               String.Format("Expected channel open methods to be '{0}' but actual was '{1}'.",
                             expectedOpens, actualOpens));

        string actualCloses = String.Join(",", channelCloseMethodsCalled);
        Assert.True(String.Equals(expectedCloses, actualCloses, StringComparison.Ordinal),
               String.Format("Expected channel close methods to be '{0}' but actual was '{1}'.",
                             expectedCloses, actualCloses));

        actualOpens = String.Join(",", factoryOpenMethodsCalled);
        Assert.True(String.Equals(expectedOpens, actualOpens, StringComparison.Ordinal),
               String.Format("Expected factory open methods to be '{0}' but actual was '{1}'.",
                             expectedOpens, actualOpens));

        actualCloses = String.Join(",", factoryCloseMethodsCalled);
        Assert.True(String.Equals(expectedCloses, actualCloses, StringComparison.Ordinal),
               String.Format("Expected factory close methods to be '{0}' but actual was '{1}'.",
                             expectedCloses, actualCloses));

        Assert.True(factory.State == CommunicationState.Closed,
            String.Format("Expected factory's final state to be Closed but was '{0}'", factory.State));

        Assert.True(((ICommunicationObject)channel).State == CommunicationState.Closed,
            String.Format("Expected channel's final state to be Closed but was '{0}'", ((ICommunicationObject)channel).State));

    }

    [WcfFact]
    public static void CustomChannel_Sync_Request_Exception_Propagates()
    {
        MockChannelFactory<IRequestChannel> mockChannelFactory = null;
        MockRequestChannel mockRequestChannel = null;
        string testMessageBody = "CustomChannelTest_Sync";
        Message inputMessage = Message.CreateMessage(MessageVersion.Default, action: "Test", body: testMessageBody);
        string expectedExceptionMessage = "Sync exception message";

        // *** SETUP *** \\
        // Intercept the creation of the factory so we can intercept creation of the channel
        Func<Type, BindingContext, IChannelFactory> buildFactoryAction = (Type type, BindingContext context) =>
        {
            // Create the channel factory so we can intercept channel creation
            mockChannelFactory = new MockChannelFactory<IRequestChannel>(context, new TextMessageEncodingBindingElement().CreateMessageEncoderFactory());

            // Override the OnCreateChannel call so we can fault inject an exception
            mockChannelFactory.OnCreateChannelOverride = (EndpointAddress endpoint, Uri via) =>
            {
                // Create the mock channel and fault inject an exception in the Request
                mockRequestChannel = (MockRequestChannel)mockChannelFactory.DefaultOnCreateChannel(endpoint, via);
                mockRequestChannel.RequestOverride = (Message m, TimeSpan t) =>
                {
                    throw new InvalidOperationException(expectedExceptionMessage);
                };
                return mockRequestChannel;
            };
            return mockChannelFactory;
        };

        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        mockBindingElement.BuildChannelFactoryOverride = buildFactoryAction;

        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<ICustomChannelServiceInterface>(binding, address);
        ICustomChannelServiceInterface channel = factory.CreateChannel();

        // *** EXECUTE *** \\
        InvalidOperationException actualException = Assert.Throws<InvalidOperationException>(() =>
        {
            channel.Process(inputMessage);
        });

        // *** VALIDATE *** \\
        Assert.True(String.Equals(expectedExceptionMessage, actualException.Message),
                    String.Format("Expected exception message to be '{0}' but actual was '{1}'", 
                                  expectedExceptionMessage, actualException.Message));
        
    }

    [WcfFact]
    public static void CustomChannel_Async_Request_Exception_Propagates()
    {
        MockChannelFactory<IRequestChannel> mockChannelFactory = null;
        MockRequestChannel mockRequestChannel = null;
        string testMessageBody = "CustomChannelTest_Async";
        Message inputMessage = Message.CreateMessage(MessageVersion.Default, action: "Test", body: testMessageBody);
        string expectedExceptionMessage = "Async exception message";

        // *** SETUP *** \\
        // Intercept the creation of the factory so we can intercept creation of the channel
        Func<Type, BindingContext, IChannelFactory> buildFactoryAction = (Type type, BindingContext context) =>
        {
            // Create the channel factory so we can intercept channel creation
            mockChannelFactory = new MockChannelFactory<IRequestChannel>(context, new TextMessageEncodingBindingElement().CreateMessageEncoderFactory());

            // Override the OnCreateChannel call so we can fault inject an exception
            mockChannelFactory.OnCreateChannelOverride = (EndpointAddress endpoint, Uri via) =>
            {
                // Create the mock channel and fault inject an exception in the EndRequest
                mockRequestChannel = (MockRequestChannel)mockChannelFactory.DefaultOnCreateChannel(endpoint, via);
                mockRequestChannel.EndRequestOverride = (IAsyncResult ar) =>
                {
                    throw new InvalidOperationException(expectedExceptionMessage);
                };
                return mockRequestChannel;
            };
            return mockChannelFactory;
        };

        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        mockBindingElement.BuildChannelFactoryOverride = buildFactoryAction;

        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<ICustomChannelServiceInterface>(binding, address);
        ICustomChannelServiceInterface channel = factory.CreateChannel();

        // *** EXECUTE *** \\
        // The ProcessAsync should not throw -- the fault comes in the EndRequest
        Task<Message> processTask = channel.ProcessAsync(inputMessage);

        InvalidOperationException actualException = Assert.Throws<InvalidOperationException>(() =>
        {
            // awaiting the task will invoke the EndRequest
            processTask.GetAwaiter().GetResult();
        });

        // *** VALIDATE *** \\
        Assert.True(String.Equals(expectedExceptionMessage, actualException.Message),
                    String.Format("Expected exception message to be '{0}' but actual was '{1}'",
                                  expectedExceptionMessage, actualException.Message));
    }

    [WcfFact]
    public static void CustomChannel_Factory_Abort_Aborts_Channel()
    {
        MockChannelFactory<IRequestChannel> mockChannelFactory = null;
        MockRequestChannel mockRequestChannel = null;
        List<string> channelOpenMethodsCalled = new List<string>();
        List<string> channelCloseMethodsCalled = new List<string>();
        List<string> factoryOpenMethodsCalled = new List<string>();
        List<string> factoryCloseMethodsCalled = new List<string>();
        string testMessageBody = "CustomChannelTest_Sync";
        Message inputMessage = Message.CreateMessage(MessageVersion.Default, action: "Test", body: testMessageBody);

        // *** SETUP *** \\
        // Intercept the creation of the factory so we can intercept creation of the channel
        Func<Type, BindingContext, IChannelFactory> buildFactoryAction = (Type type, BindingContext context) =>
        {
            // Create the channel factory and intercept all open and close method calls
            mockChannelFactory = new MockChannelFactory<IRequestChannel>(context, new TextMessageEncodingBindingElement().CreateMessageEncoderFactory());
            MockCommunicationObject.InterceptAllOpenMethods(mockChannelFactory, factoryOpenMethodsCalled);
            MockCommunicationObject.InterceptAllCloseMethods(mockChannelFactory, factoryCloseMethodsCalled);

            // Override the OnCreateChannel call so we get the mock channel created by the factory
            mockChannelFactory.OnCreateChannelOverride = (EndpointAddress endpoint, Uri via) =>
            {
                // Create the mock channel and intercept all its open and close method calls
                mockRequestChannel = (MockRequestChannel)mockChannelFactory.DefaultOnCreateChannel(endpoint, via);
                MockCommunicationObject.InterceptAllOpenMethods(mockRequestChannel, channelOpenMethodsCalled);
                MockCommunicationObject.InterceptAllCloseMethods(mockRequestChannel, channelCloseMethodsCalled);
                return mockRequestChannel;
            };
            return mockChannelFactory;
        };

        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        mockBindingElement.BuildChannelFactoryOverride = buildFactoryAction;

        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<ICustomChannelServiceInterface>(binding, address);

        // We rely on the implicit open of the channel to be synchronous.
        // This is true for both the full framework and this NET Core version.
        ICustomChannelServiceInterface channel = factory.CreateChannel();

        // *** EXECUTE *** \\
        Message outputMessage = channel.Process(inputMessage);

        // The mock's default behavior is just to loopback what we sent.
        var result = outputMessage.GetBody<string>();

        // Abort the factory and expect both factory and channel to be aborted.
        factory.Abort();

        // *** VALIDATE *** \\
        Assert.True(String.Equals(testMessageBody, result),
                    String.Format("Expected body to be '{0}' but actual was '{1}'", testMessageBody, result));

        string expectedOpens = "OnOpening,OnOpen,OnOpened";
        string expectedCloses = "OnClosing,OnAbort,OnClosed";

        string actualOpens = String.Join(",", channelOpenMethodsCalled);
        Assert.True(String.Equals(expectedOpens, actualOpens, StringComparison.Ordinal),
               String.Format("Expected channel open methods to be '{0}' but actual was '{1}'.",
                             expectedOpens, actualOpens));

        string actualCloses = String.Join(",", channelCloseMethodsCalled);
        Assert.True(String.Equals(expectedCloses, actualCloses, StringComparison.Ordinal),
               String.Format("Expected channel close methods to be '{0}' but actual was '{1}'.",
                             expectedCloses, actualCloses));

        actualOpens = String.Join(",", factoryOpenMethodsCalled);
        Assert.True(String.Equals(expectedOpens, actualOpens, StringComparison.Ordinal),
               String.Format("Expected factory open methods to be '{0}' but actual was '{1}'.",
                             expectedOpens, actualOpens));

        actualCloses = String.Join(",", factoryCloseMethodsCalled);
        Assert.True(String.Equals(expectedCloses, actualCloses, StringComparison.Ordinal),
               String.Format("Expected factory close methods to be '{0}' but actual was '{1}'.",
                             expectedCloses, actualCloses));

        Assert.True(factory.State == CommunicationState.Closed,
                    String.Format("Expected factory's final state to be Closed but was '{0}'", factory.State));

        Assert.True(((ICommunicationObject)channel).State == CommunicationState.Closed,
            String.Format("Expected channel's final state to be Closed but was '{0}'", ((ICommunicationObject)channel).State));
    }

    [WcfFact]
    public static void CustomChannel_ChannelInitializer_CreateChannel_Calls_Initialize()
    {
        string testMessageBody = "CustomChannelTest_Sync";
        Message inputMessage = Message.CreateMessage(MessageVersion.Default, action: "Test", body: testMessageBody);

        // *** SETUP *** \\
        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<ICustomChannelServiceInterface>(binding, address);

        // Create a mock channel initializer to observe whether
        // its method is invoked.
        bool initializeCalled = false;
        MockChannelInitializer mockInitializer = new MockChannelInitializer();
        mockInitializer.InitializeOverride = (chnl) =>
        {
            initializeCalled = true;
            mockInitializer.DefaultInitialize(chnl);
        };

        // Add an endpoint behavior that registers the mock initializer
        MockEndpointBehavior mockEndpointBehavior = new MockEndpointBehavior();
        mockEndpointBehavior.ApplyClientBehaviorOverride = (ServiceEndpoint serviceEndpoint, ClientRuntime clientRuntime) =>
        {
            clientRuntime.ChannelInitializers.Add(mockInitializer);
        };

        factory.Endpoint.EndpointBehaviors.Add(mockEndpointBehavior);

        // *** EXECUTE *** \\
        factory.CreateChannel();

        // *** VALIDATE *** \\
        Assert.True(initializeCalled, "Initialize was not called.");
    }

    [WcfFact]
    public static void CustomChannel_ChannelInitializer_Failed_Initialize_Throws()
    {
        string testMessageBody = "CustomChannelTest_Sync";
        Message inputMessage = Message.CreateMessage(MessageVersion.Default, action: "Test", body: testMessageBody);

        // *** SETUP *** \\
        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<ICustomChannelServiceInterface>(binding, address);

        // Create a mock channel initializer to fault inject an exception
        InvalidOperationException thrownException = new InvalidOperationException("test threw this");
        MockChannelInitializer mockInitializer = new MockChannelInitializer();
        mockInitializer.InitializeOverride = (chnl) =>
        {
            mockInitializer.DefaultInitialize(chnl);
            throw thrownException;
        };

        // Add an endpoint behavior that registers the mock initializer
        MockEndpointBehavior mockEndpointBehavior = new MockEndpointBehavior();
        mockEndpointBehavior.ApplyClientBehaviorOverride = (ServiceEndpoint serviceEndpoint, ClientRuntime clientRuntime) =>
        {
            clientRuntime.ChannelInitializers.Add(mockInitializer);
        };

        factory.Endpoint.EndpointBehaviors.Add(mockEndpointBehavior);

        // *** EXECUTE *** \\
        InvalidOperationException caughtException =
            Assert.Throws<InvalidOperationException>(() => factory.CreateChannel());

        // *** VALIDATE *** \\
        Assert.True(String.Equals(caughtException.Message, thrownException.Message), 
                    String.Format("Expected exception message '{0}' but actual was '{1}'",
                                    thrownException.Message, caughtException.Message));
    }
    
        [WcfFact]
    public static void CustomChannel_InteractiveInitializer_Called_Implicit_Open()
    {
        string testMessageBody = "CustomChannelTest_Sync";
        Message inputMessage = Message.CreateMessage(MessageVersion.Default, action: "Test", body: testMessageBody);

        // *** SETUP *** \\
        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<ICustomChannelServiceInterface>(binding, address);

        // Create a mock interactive channel initializer to observe whether
        // its methods are invoked.
        bool beginInitializeCalled = false;
        bool endInitializeCalled = false;
        MockInteractiveChannelInitializer mockInitializer = new MockInteractiveChannelInitializer();
        mockInitializer.BeginDisplayInitializationUIOverride = (chnl, callback, state) =>
        {
            beginInitializeCalled = true;
            return mockInitializer.DefaultBeginDisplayInitializationUI(chnl, callback, state);
        };

        mockInitializer.EndDisplayInitializationUIOverride = (ar) =>
        {
            endInitializeCalled = true;
            mockInitializer.DefaultEndDisplayInitializationUI(ar);
        };

        // Create a mock endpoint behavior that adds the mock interactive initializer
        MockEndpointBehavior mockEndpointBehavior = new MockEndpointBehavior();
        mockEndpointBehavior.ApplyClientBehaviorOverride = (ServiceEndpoint serviceEndpoint, ClientRuntime clientRuntime) =>
        {
            clientRuntime.InteractiveChannelInitializers.Add(mockInitializer);
        };

        // Attach our mock endpoint behavior to the ServiceEndpoint
        // so that our mock channel initializer is registered.
        factory.Endpoint.EndpointBehaviors.Add(mockEndpointBehavior);

        ICustomChannelServiceInterface channel = factory.CreateChannel();

        // *** EXECUTE *** \\
        // The implicit open does the call to display interactive UI.
        channel.Process(inputMessage);

        // *** VALIDATE *** \\
        Assert.True(beginInitializeCalled, "BeginDisplayInitializationUI was not called.");
        Assert.True(endInitializeCalled, "EndDisplayInitializationUI was not called.");
    }

    [WcfFact]
    public static void CustomChannel_InteractiveInitializer_Called_Explicitly()
    {
        string testMessageBody = "CustomChannelTest_Sync";
        Message inputMessage = Message.CreateMessage(MessageVersion.Default, action: "Test", body: testMessageBody);

        // *** SETUP *** \\
        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<ICustomChannelServiceInterface>(binding, address);

        // Create a mock interactive channel initializer to observe whether
        // its methods are invoked.
        bool beginInitializeCalled = false;
        bool endInitializeCalled = false;
        MockInteractiveChannelInitializer mockInitializer = new MockInteractiveChannelInitializer();
        mockInitializer.BeginDisplayInitializationUIOverride = (chnl, callback, state) =>
        {
            beginInitializeCalled = true;
            return mockInitializer.DefaultBeginDisplayInitializationUI(chnl, callback, state);
        };

        mockInitializer.EndDisplayInitializationUIOverride = (ar) =>
        {
            endInitializeCalled = true;
            mockInitializer.DefaultEndDisplayInitializationUI(ar);
        };

        // Create a mock endpoint behavior that adds the mock interactive initializer
        MockEndpointBehavior mockEndpointBehavior = new MockEndpointBehavior();
        mockEndpointBehavior.ApplyClientBehaviorOverride = (ServiceEndpoint serviceEndpoint, ClientRuntime clientRuntime) =>
        {
            clientRuntime.InteractiveChannelInitializers.Add(mockInitializer);
        };

        // Attach our mock endpoint behavior to the ServiceEndpoint
        // so that our mock channel initializer is registered.
        factory.Endpoint.EndpointBehaviors.Add(mockEndpointBehavior);

        ICustomChannelServiceInterface channel = factory.CreateChannel();

        // *** EXECUTE *** \\
        // Invoke the interative UI explicitly
        ((IClientChannel)channel).DisplayInitializationUI();

        // *** VALIDATE *** \\
        Assert.True(beginInitializeCalled, "BeginDisplayInitializationUI was not called.");
        Assert.True(endInitializeCalled, "EndDisplayInitializationUI was not called.");
    }

    [WcfFact]
    public static void CustomChannel_InteractiveInitializer_Direct_Open_To_Bypass_UI_Throws()
    {
        string testMessageBody = "CustomChannelTest_Sync";
        Message inputMessage = Message.CreateMessage(MessageVersion.Default, action: "Test", body: testMessageBody);

        // *** SETUP *** \\
        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<ICustomChannelServiceInterface>(binding, address);

        // Create a mock interactive channel initializer to observe whether
        // its methods are invoked.
        bool beginInitializeCalled = false;
        bool endInitializeCalled = false;
        MockInteractiveChannelInitializer mockInitializer = new MockInteractiveChannelInitializer();
        mockInitializer.BeginDisplayInitializationUIOverride = (chnl, callback, state) =>
        {
            beginInitializeCalled = true;
            return mockInitializer.DefaultBeginDisplayInitializationUI(chnl, callback, state);
        };

        mockInitializer.EndDisplayInitializationUIOverride = (ar) =>
        {
            endInitializeCalled = true;
            mockInitializer.DefaultEndDisplayInitializationUI(ar);
        };

        // Create a mock endpoint behavior that adds the mock interactive initializer
        MockEndpointBehavior mockEndpointBehavior = new MockEndpointBehavior();
        mockEndpointBehavior.ApplyClientBehaviorOverride = (ServiceEndpoint serviceEndpoint, ClientRuntime clientRuntime) =>
        {
            clientRuntime.InteractiveChannelInitializers.Add(mockInitializer);
        };

        // Attach our mock endpoint behavior to the ServiceEndpoint
        // so that our mock channel initializer is registered.
        factory.Endpoint.EndpointBehaviors.Add(mockEndpointBehavior);

        ICustomChannelServiceInterface channel = factory.CreateChannel();

        // *** EXECUTE *** \\
        // Attempting to open a channel explicitly without first calling DisplayInteractiveUI
        // throws an exception.
        Assert.Throws<InvalidOperationException>(() => ((IClientChannel)channel).Open());

        // *** VALIDATE *** \\
        Assert.False(beginInitializeCalled, "BeginDisplayInitializationUI should not have been called.");
        Assert.False(endInitializeCalled, "EndDisplayInitializationUI should not have been  called.");
    }

    [WcfFact]
    public static void CustomChannel_InteractiveInitializer_Can_Be_Blocked()
    {
        string testMessageBody = "CustomChannelTest_Sync";
        Message inputMessage = Message.CreateMessage(MessageVersion.Default, action: "Test", body: testMessageBody);

        // *** SETUP *** \\
        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<ICustomChannelServiceInterface>(binding, address);

        // Create a mock interactive channel initializer to observe whether
        // its methods are invoked.
        bool beginInitializeCalled = false;
        bool endInitializeCalled = false;
        MockInteractiveChannelInitializer mockInitializer = new MockInteractiveChannelInitializer();
        mockInitializer.BeginDisplayInitializationUIOverride = (chnl, callback, state) =>
        {
            beginInitializeCalled = true;
            return mockInitializer.DefaultBeginDisplayInitializationUI(chnl, callback, state);
        };

        mockInitializer.EndDisplayInitializationUIOverride = (ar) =>
        {
            endInitializeCalled = true;
            mockInitializer.DefaultEndDisplayInitializationUI(ar);
        };

        // Create a mock endpoint behavior that adds the mock interactive initializer
        MockEndpointBehavior mockEndpointBehavior = new MockEndpointBehavior();
        mockEndpointBehavior.ApplyClientBehaviorOverride = (ServiceEndpoint serviceEndpoint, ClientRuntime clientRuntime) =>
        {
            clientRuntime.InteractiveChannelInitializers.Add(mockInitializer);
        };

        // Attach our mock endpoint behavior to the ServiceEndpoint
        // so that our mock channel initializer is registered.
        factory.Endpoint.EndpointBehaviors.Add(mockEndpointBehavior);

        ICustomChannelServiceInterface channel = factory.CreateChannel();

        // block attempts to use interactive UI
        ((IClientChannel)channel).AllowInitializationUI = false;

        // *** EXECUTE *** \\
        Assert.Throws<InvalidOperationException>(() => ((IClientChannel)channel).DisplayInitializationUI());

        // *** VALIDATE *** \\
        Assert.False(beginInitializeCalled, "BeginDisplayInitializationUI should not have been called.");
        Assert.False(endInitializeCalled, "EndDisplayInitializationUI should not have been called.");
    }

    [WcfFact]
    public static void CustomChannel_InteractiveInitializer_Error_Blocks_Open()
    {
        string testMessageBody = "CustomChannelTest_Sync";
        Message inputMessage = Message.CreateMessage(MessageVersion.Default, action: "Test", body: testMessageBody);

        // *** SETUP *** \\
        MockTransportBindingElement mockBindingElement = new MockTransportBindingElement();
        CustomBinding binding = new CustomBinding(mockBindingElement);
        EndpointAddress address = new EndpointAddress("myprotocol://localhost:5000");
        var factory = new ChannelFactory<ICustomChannelServiceInterface>(binding, address);

        // Create a mock interactive channel initializer to throw during initialization
        InvalidOperationException thrownException = new InvalidOperationException("ui initialization failed");
        MockInteractiveChannelInitializer mockInitializer = new MockInteractiveChannelInitializer();
        mockInitializer.EndDisplayInitializationUIOverride = (ar) =>
        {
            // Throw from the EndDisplayInitializationUI call to simulate a UI
            // action that throws an error to prevent the open.
            mockInitializer.DefaultEndDisplayInitializationUI(ar);
            throw thrownException;
        };

        // Create a mock endpoint behavior that adds the mock interactive initializer
        MockEndpointBehavior mockEndpointBehavior = new MockEndpointBehavior();
        mockEndpointBehavior.ApplyClientBehaviorOverride = (ServiceEndpoint serviceEndpoint, ClientRuntime clientRuntime) =>
        {
            clientRuntime.InteractiveChannelInitializers.Add(mockInitializer);
        };

        // Attach our mock endpoint behavior to the ServiceEndpoint
        // so that our mock channel initializer is registered.
        factory.Endpoint.EndpointBehaviors.Add(mockEndpointBehavior);

        ICustomChannelServiceInterface channel = factory.CreateChannel();

        // *** EXECUTE *** \\
        // The implicit open does the call to display interactive UI.
        InvalidOperationException caughtException = 
            Assert.Throws<InvalidOperationException>(() => channel.Process(inputMessage));

        // *** VALIDATE *** \\
        Assert.True(string.Equals(thrownException.Message, caughtException.Message),
                    String.Format("Expected exception message to be '{0}' but actual was '{1}'",
                                  thrownException.Message, caughtException.Message));
    }

    #region Helpers

    [ServiceContract]
    public interface ICustomChannelServiceInterface
    {
        [OperationContract(Action = "*", ReplyAction = "*")]
        Message Process(Message input);

        [OperationContract(Action = "*", ReplyAction = "*")]
        Task<Message> ProcessAsync(Message input);
    }

    #endregion Helpers
}
