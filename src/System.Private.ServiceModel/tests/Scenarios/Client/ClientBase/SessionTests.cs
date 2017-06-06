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
    public static void Test_IsInitiating_IsTerminating()
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
    }

    [WcfFact]
    [OuterLoop]
    public static void Test_IsInitiating_IsTerminating_Separate_Channels()
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
            Assert.Equal(0, dataA1 | dataB1 | dataA2 | dataB2);

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
    public static void Test_Negative_Calling_NonInitiating_Method_First()
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

    [WcfFact]
    [OuterLoop]
    public static void Test_Negative_Calling_Initiating_After_Calling_Terminating()
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

    [WcfFact]
    [OuterLoop]
    public static void Test_Negative_Calling_Terminating_Twice()
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
    public static void Test_Implicit_Session_Initiation_And_Termination()
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
                // But it has a different binding with a very short receiveTimeout ="00:00:05"
                // So waiting for just 10 seconds is enough to get the connection and the session implicitly closed
                Task.Delay(10000).Wait();
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

    // Without CallbackBehaviorAttribute (and ConcurrencyMode = ConcurrencyMode.Multiple) support
    // a few duplex session tests are not going to be able to call the service from within our callback. 
    // This is tracked by issue #1959

    //[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SessionTestsDuplexCallback : ISessionTestsDuplexCallback
    {
        public bool ClosedCalled { get; set; }
        public bool ClosingCalled { get; set; }
        public bool FaultedCalled { get; set; }
        public int ClientCallback(int callsToNonTerminatingMethodToMake, int callsToTerminatingMethodToMake)
        {
            return ClientCallbackImpl(callsToNonTerminatingMethodToMake, callsToTerminatingMethodToMake);
        }

        public int TerminatingClientCallback(int callsToNonTerminatingMethodToMake, int callsToTerminatingMethodToMake)
        {
            return ClientCallbackImpl(callsToNonTerminatingMethodToMake, callsToTerminatingMethodToMake);
        }

        public int ClientSideOnlyTerminatingClientCallback(int callsToNonTerminatingMethodToMake, int callsToTerminatingMethodToMake)
        {
            return ClientCallbackImpl(callsToNonTerminatingMethodToMake, callsToTerminatingMethodToMake);
        }

        private int ClientCallbackImpl(int callsToNonTerminatingMethodToMake, int callsToTerminatingMethodToMake)
        {
            int numCalls = 0;
            var channel = OperationContext.Current.GetCallbackChannel<ISessionTestsDuplexService>();
            var c = OperationContext.Current.Channel;

            Assert.Equal(s_channel.GetHashCode(), c.GetHashCode());
            Assert.Equal(s_channel.GetHashCode(), channel.GetHashCode());

            OperationContext.Current.Channel.Closed += (sender, e) =>
            {
                ClosedCalled = true;
            };
            OperationContext.Current.Channel.Closing += (sender, e) =>
            {
                ClosingCalled = true;
            };
            OperationContext.Current.Channel.Faulted += (sender, e) =>
            {
                FaultedCalled = true;
            };

            // Rather than dealing with a large number of combinations of various exceptions arising from both sides,
            // the test is structured to return the total count of successful calls that were made both directions.

            // terminating ones go first
            for (int i = 0; i < callsToTerminatingMethodToMake; i++)
            {
                try
                {
                    numCalls += channel.TerminatingMethod();
                }
                catch
                { }
            }
            // followed by non-terminating calls
            for (int i = 0; i < callsToNonTerminatingMethodToMake; i++)
            {
                try
                {
                    numCalls += channel.NonTerminatingMethod();
                }
                catch
                { }
            }

            return numCalls + 1; // +1 for this call
        }
    }

    // Simple case with no terminating methods and no additional calls from the callback
    [WcfFact]
    [OuterLoop]
    public static void NonTerminatingMethodCallingDuplexCallbacksNoReentrantCalls()
    {
        DuplexTestSetupHelper();
        var result = s_channel.NonTerminatingMethodCallingDuplexCallbacks(      // + 1 
            callsToClientCallbackToMake: 2,                                     // + 2  
            callsToTerminatingClientCallbackToMake: 0,
            callsToClientSideOnlyTerminatingClientCallbackToMake: 0,
            callsToNonTerminatingMethodToMakeInsideClientCallback: 0,
            callsToTerminatingMethodToMakeInsideClientCallback: 0);

        Assert.Equal(result, 3);

        ResultsVerificationHelper<Exception>(canMakeServiceCall: true, cbClosedCalled: false, cbClosingCalled: false, cbFaultedCalled: false);
    }

    // Simple case with no terminating methods. 
    // Multiple calls to the service from within the callback
    [WcfFact]
    [OuterLoop]
    [Issue(1959)]
    public static void NonTerminatingMethodCallingDuplexCallbacksCallingService()
    {
        DuplexTestSetupHelper();
        var result = s_channel.NonTerminatingMethodCallingDuplexCallbacks(      // + 1 
            callsToClientCallbackToMake: 2,                                     // + 2  
            callsToTerminatingClientCallbackToMake: 0,
            callsToClientSideOnlyTerminatingClientCallbackToMake: 0,
            callsToNonTerminatingMethodToMakeInsideClientCallback: 2,           // + 2 * 2
            callsToTerminatingMethodToMakeInsideClientCallback: 0);

        Assert.Equal(result, 7);

        ResultsVerificationHelper<Exception>(canMakeServiceCall: true, cbClosedCalled: false, cbClosingCalled: false, cbFaultedCalled: false);
    }

    // Call terminating method with client callback not calling the service again
    [WcfFact]
    [OuterLoop]
    public static void TerminatingMethodCallingDuplexCallbacksNoReentrantCalls()
    {
        DuplexTestSetupHelper();
        var result = s_channel.TerminatingMethodCallingDuplexCallbacks(         // + 1
            callsToClientCallbackToMake: 2,                                     // + 2
            callsToTerminatingClientCallbackToMake: 0,
            callsToClientSideOnlyTerminatingClientCallbackToMake: 0,
            callsToNonTerminatingMethodToMakeInsideClientCallback: 0,
            callsToTerminatingMethodToMakeInsideClientCallback: 0);

        Assert.Equal(result, 3);

        // verify that after this we can not make another call
        ResultsVerificationHelper<CommunicationException>(
            canMakeServiceCall: false, 
            cbClosedCalled: false,      // Note that closed hasn't been called in this case
            cbClosingCalled: false, 
            cbFaultedCalled: false);
    }

    // Call terminating method with client callback trying (and failing) to call non-terminating service method
    [WcfFact]
    [OuterLoop]
    [Issue(1959)]
    public static void TerminatingMethodCallingDuplexCallbacksFailingToCallService()
    {
        DuplexTestSetupHelper();
        var result = s_channel.TerminatingMethodCallingDuplexCallbacks(         // + 1
            callsToClientCallbackToMake: 2,                                     // + 2 
            callsToTerminatingClientCallbackToMake: 0,
            callsToClientSideOnlyTerminatingClientCallbackToMake: 0,
            callsToNonTerminatingMethodToMakeInsideClientCallback: 2,           // + 0 as they will fail
            callsToTerminatingMethodToMakeInsideClientCallback: 0);

        Assert.Equal(result, 3);

        // verify that after this we can not make another call
        ResultsVerificationHelper<CommunicationException>(canMakeServiceCall: false, cbClosedCalled: false, cbClosingCalled: false, cbFaultedCalled: false);
    }

    // Call non-terminating service method which calls client-side-only terminating callback 
    // Verify that the client will only accept one terminating callback call
    [WcfFact]
    [OuterLoop]
    public static void NonTerminatingMethodCallingClientSideOnlyTerminatingDuplexCallbacks()
    {
        DuplexTestSetupHelper();
        var result = s_channel.NonTerminatingMethodCallingDuplexCallbacks(  // + 1 
            callsToClientCallbackToMake: 1,                                 // + 0
            callsToTerminatingClientCallbackToMake: 0,
            callsToClientSideOnlyTerminatingClientCallbackToMake: 2,        // + 1 (not 2)
            callsToNonTerminatingMethodToMakeInsideClientCallback: 0,
            callsToTerminatingMethodToMakeInsideClientCallback: 0);

        Assert.Equal(result, 2);

        // even though the server can't call any callbacks anymore we can still call the service
        ResultsVerificationHelper<Exception>(canMakeServiceCall: true, cbClosedCalled: false, cbClosingCalled: false, cbFaultedCalled: false);
    }

    // Call non-terminating service method which calls client-side-only terminating callback 
    // which calls a non-terminating service method
    [WcfFact]
    [OuterLoop]
    [Issue(1959)]
    public static void NonTerminatingMethodCallingClientSideTerminatingDuplexCallbacksCallingServer()
    {
        DuplexTestSetupHelper();
        var result = s_channel.NonTerminatingMethodCallingDuplexCallbacks(  // + 1
            callsToClientCallbackToMake: 1,                                 // + 0
            callsToTerminatingClientCallbackToMake: 0,
            callsToClientSideOnlyTerminatingClientCallbackToMake: 2,        // + 1 (not 2)
            callsToNonTerminatingMethodToMakeInsideClientCallback: 2,       // + 2 (not 2*2)
            callsToTerminatingMethodToMakeInsideClientCallback: 0);

        Assert.Equal(result, 4);

        // even though the server can't call any callbacks anymore we can still call the server
        ResultsVerificationHelper<Exception>(canMakeServiceCall: true, cbClosedCalled: false, cbClosingCalled: false, cbFaultedCalled: false);
    }

    // A non-terminating method with non-terminating client callback calling terminating service method
    [WcfFact]
    [OuterLoop]
    [Issue(1959)]
    public static void NonTerminatingMethodCallingDuplexCallbacksCallingTerminatingService()
    {
        DuplexTestSetupHelper();
        Assert.Throws<CommunicationException>(() =>
        {
            var result = s_channel.NonTerminatingMethodCallingDuplexCallbacks(
                callsToClientCallbackToMake: 2,
                callsToTerminatingClientCallbackToMake: 0,
                callsToClientSideOnlyTerminatingClientCallbackToMake: 0,
                callsToNonTerminatingMethodToMakeInsideClientCallback: 0,
                callsToTerminatingMethodToMakeInsideClientCallback: 1);         // this causes the original call to fail
        });
    }

    // Call a non-terminating service method that calls terminating client callback
    [WcfFact]
    [OuterLoop]
    public static void NonTerminatingMethodCallingTerminatingDuplexCallbacks()
    {
        DuplexTestSetupHelper();
        Assert.Throws<CommunicationException>(() =>
        {
            var result = s_channel.NonTerminatingMethodCallingDuplexCallbacks(
            callsToClientCallbackToMake: 0,
            callsToTerminatingClientCallbackToMake: 1,                      // this causes the original call to fail
            callsToClientSideOnlyTerminatingClientCallbackToMake: 0,
            callsToNonTerminatingMethodToMakeInsideClientCallback: 0,
            callsToTerminatingMethodToMakeInsideClientCallback: 0);
        });
    }

    // A couple of helpers to reuse the common code
    private static SessionTestsDuplexCallback s_duplexCallback = null;
    private static ISessionTestsDuplexService s_channel = null;

    private static void DuplexTestSetupHelper()
    {
        var duplexEndpoint = new EndpointAddress(Endpoints.Tcp_Session_Tests_Duplex_Service);
        var binding = new NetTcpBinding();
        binding.Security = new NetTcpSecurity();
        binding.Security.Mode = SecurityMode.None;

        var duplexCallback = s_duplexCallback = new SessionTestsDuplexCallback();
        var instanceContext = new InstanceContext(duplexCallback);

        var factory = new DuplexChannelFactory<ISessionTestsDuplexService>(instanceContext, binding, duplexEndpoint);
        s_channel = factory.CreateChannel();
        ((ICommunicationObject)s_channel).Open();

        var ctxChannel = (IContextChannel)s_channel;
        ctxChannel.Closed += (sender, e) =>
        {
            Console.WriteLine("Closed");
        };
        ctxChannel.Closing += (sender, e) =>
        {
            Console.WriteLine("Closing");
        };
        ctxChannel.Faulted += (sender, e) =>
        {
            Console.WriteLine("Faulted");
        };
    }

    private static void ResultsVerificationHelper<ExpectedException>(bool canMakeServiceCall, bool cbClosedCalled, bool cbClosingCalled, bool cbFaultedCalled)
    {
        if (canMakeServiceCall)
        {
            // assert does not throw
            var result2 = s_channel.NonTerminatingMethod();
            // assert equals
            if (result2 == 1)
            { }
        }
        else
        {
            // assert throw
            try
            {
                var result2 = s_channel.NonTerminatingMethod();
            }
            catch (Exception e)
            {
                Console.WriteLine("Expected: " + typeof(ExpectedException) + " actual: " + e.GetType());
            }
        }
        // assert equals
        if (cbClosedCalled != s_duplexCallback.ClosedCalled)
        {
            Console.WriteLine("cbClosedCalled != s_duplexCallback.ClosedCalled");
        }

        // assert equals
        if (cbClosingCalled != s_duplexCallback.ClosingCalled)
        {
            Console.WriteLine("cbClosingCalled != s_duplexCallback.ClosingCalled");
        }

        // assert equals
        if (cbFaultedCalled != s_duplexCallback.FaultedCalled)
        {
            Console.WriteLine("cbFaultedCalled != s_duplexCallback.FaultedCalled");
        }
    }
}