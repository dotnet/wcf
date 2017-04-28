// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public static class SessionTests
{
    // A basic test to verify that a session gets created and terminated
    [WcfFact]
    [OuterLoop]
    static void Test_IsInitiating_IsTerminating()
    {
        ChannelFactory<ISessionTestsDefaultService> factory = null;
        ISessionTestsDefaultService channel = null;
        try
        {
            // *** SETUP *** \\
            factory = CreateChannelFactoryHelper<ISessionTestsDefaultService>(Endpoints.Tcp_Session_Tests_Default_Service);
            channel = factory.CreateChannel();
            // *** EXECUTE *** \\
            const int A = 0xAAA;
            const int B = 0xBBB;

            var dataA = channel.MethodAInitiating(A);
            // MethodA is initiating so now we have a session and can call non initiating MethodB
            var dataB = channel.MethodBNonInitiating(B);
            var dataC = channel.MethodCTerminating();
            // *** VALIDATE *** \\
            Assert.Equal(dataC.MethodAValue, A);
            Assert.Equal(dataC.MethodBValue, B);

            // *** CLEANUP *** \\
            factory.Close();
            ((ICommunicationObject)channel).Close();
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)channel, factory);
        }
//throw new Exception("");
    }

    [WcfFact]
    [OuterLoop]
    static void Test_IsInitiating_IsTerminating_Separate_Channels()
    {
        ChannelFactory<ISessionTestsDefaultService> channelFactory = null;
        ISessionTestsDefaultService channel1 = null, channel2 = null;
        try
        {
            // *** SETUP *** \\
            channelFactory = CreateChannelFactoryHelper<ISessionTestsDefaultService>(Endpoints.Tcp_Session_Tests_Default_Service);
            channel1 = channelFactory.CreateChannel();
            channel2 = channelFactory.CreateChannel();
            const int A1 = 0xA1, B1 = 0xB1;
            const int A2 = 0xA2, B2 = 0xB2;

            // *** EXECUTE *** \\
            var dataA1 = channel1.MethodAInitiating(A1);
            var dataA2 = channel2.MethodAInitiating(A2);
            var dataB1 = channel1.MethodBNonInitiating(B1);
            var dataB2 = channel2.MethodBNonInitiating(B2);
            var sessionId1 = ((IClientChannel)channel1).SessionId;
            var sessionId2 = ((IClientChannel)channel2).SessionId;
            var dataC1 = channel1.MethodCTerminating();
            var dataC2 = channel2.MethodCTerminating();

            // *** VALIDATE *** \\
            Assert.Equal(dataC1.MethodAValue, A1);
            Assert.Equal(dataC1.MethodBValue, B1);
            Assert.Equal(dataC2.MethodAValue, A2);
            Assert.Equal(dataC2.MethodBValue, B2);

            // Methods A & B update the private A & B fields in the service  
            // and return the original values of the service field instance
            // Our implementation of IService1 has InstanceContextMode = InstanceContextMode.PerSession
            // so all dataA&B* should have the original values == 0
            Assert.Equal(dataA1 & dataB1 & dataA2 & dataB2, 0);

            // The session ids should be different for 2 different channels
            Assert.NotEqual(sessionId1, sessionId2);
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects(
                (ICommunicationObject)channel1, 
                (ICommunicationObject)channel2, 
                channelFactory);
        }
    }

    [WcfFact]
    [OuterLoop]
    static void Test_Negative_Calling_NonInitiating_Method_First()
    {
        using (var factory = CreateChannelFactoryHelper<ISessionTestsDefaultService>(Endpoints.Tcp_Session_Tests_Default_Service))
        {
            ISessionTestsDefaultService channel1 = null;
            try
            {
                channel1 = factory.CreateChannel();
                Assert.Throws<InvalidOperationException>(() =>
                {
                    channel1.MethodBNonInitiating(1);
                });
            }
            finally
            {
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)channel1);
            }
        }
    }

    [ActiveIssue(1402)]
    [WcfFact]
    [OuterLoop]
    static void Test_Negative_Calling_Initiating_After_Calling_Terminating()
    {
        using (var factory = CreateChannelFactoryHelper<ISessionTestsDefaultService>(Endpoints.Tcp_Session_Tests_Default_Service))
        {
            ISessionTestsDefaultService channel2 = null;
            try
            {
                channel2 = factory.CreateChannel();
                channel2.MethodAInitiating(0);
                channel2.MethodCTerminating();
                Assert.Throws<InvalidOperationException>(() =>
                {
                    channel2.MethodAInitiating(0); // IsInitiating=true
                });
            }
            finally
            {
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)channel2);
            }
        }
    }

    [ActiveIssue(1402)]
    [WcfFact]
    [OuterLoop]
    static void Test_Negative_Calling_Terminating_Twice()
    {
        using (var factory = CreateChannelFactoryHelper<ISessionTestsDefaultService>(Endpoints.Tcp_Session_Tests_Default_Service))
        {
            ISessionTestsDefaultService channel3 = null;
            try
            {
                channel3 = factory.CreateChannel();
                channel3.MethodAInitiating(3);
                channel3.MethodCTerminating();
                Assert.Throws<InvalidOperationException>(() =>
                {
                    channel3.MethodCTerminating();
                });
            }
            finally
            {
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)channel3);
            }
        }
    }

    [WcfFact]
    [OuterLoop]
    static void Test_Implicit_Session_Initiation_And_Termination()
    {
        using (var factory = CreateChannelFactoryHelper<ISessionTestsShortTimeoutService>(Endpoints.Tcp_Session_Tests_Short_Timeout_Service))
        {
            ISessionTestsDefaultService channel = null;
            try
            {
                channel = factory.CreateChannel();
                
                (channel as ICommunicationObject).Open();
                // The following Non Initiating method can be called without calling an Initiating method first 
                // if the session was implicitly created by explicitly calling channel.Open()
                channel.MethodBNonInitiating(0xB);

                // The service behind service2Url has the same contract and implementation as service1Url
                // But it has a different binding with a very short receiveTimeout ="00:00:02"
                // So waiting for just 5 seconds is enough to get the connection and the session implicitly closed
                Task.Delay(8080).Wait();
                Assert.Throws<System.ServiceModel.CommunicationException>(() =>
                {
                    channel.MethodCTerminating();
                });
            }
            finally
            {
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)channel);
            }
        }
    }
    static ChannelFactory<T> CreateChannelFactoryHelper<T>(string url)
    {
        var helloEndpoint = new EndpointAddress(url);
        var binding = new NetTcpBinding();
        binding.Security = new NetTcpSecurity();
        binding.Security.Mode = SecurityMode.None;
        return new ChannelFactory<T>(binding, helloEndpoint);
    }
}