// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ServiceModel;
using System.Collections.Generic;
using Infrastructure.Common;
using Xunit;
using System.Threading;

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
            Assert.Equal(A, dataC.MethodAValue);
            Assert.Equal(B, dataC.MethodBValue);

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
            Assert.Equal(A1, dataC1.MethodAValue);
            Assert.Equal(B1, dataC1.MethodBValue);
            Assert.Equal(A2, dataC2.MethodAValue);
            Assert.Equal(B2, dataC2.MethodBValue);

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
                Thread.Sleep(TimeSpan.FromSeconds(10));
                try
                {
                    channel.MethodCTerminating();
                    Assert.Fail("channel.MethodCTerminating() should throw, but it didn't.");
                }
                catch(CommunicationException)
                {
                    // channel.MethodCTerminating threw CommunicationException on NetCore
                }
                catch (System.IO.IOException)
                {
                    // channel.MethodCTerminating threw IOException on uap
                }
                catch (Exception e)
                {
                    Assert.Fail($"channel.MethodCTerminating() threw unexpected exception: {e}");
                }
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
        using (DuplexTestSetupHelper())
        {
            var result = s_channel.NonTerminatingMethodCallingDuplexCallbacks(      // + 1 
                callsToClientCallbackToMake: 2,                                     // + 2  
                callsToTerminatingClientCallbackToMake: 0,
                callsToClientSideOnlyTerminatingClientCallbackToMake: 0,
                callsToNonTerminatingMethodToMakeInsideClientCallback: 0,
                callsToTerminatingMethodToMakeInsideClientCallback: 0);

            Assert.Equal(3, result);

            ResultsVerificationHelper<Exception>(canMakeServiceCall: true, cbClosedCalled: false, cbClosingCalled: false, cbFaultedCalled: false);
        }
    }

    // Simple case with no terminating methods. 
    // Multiple calls to the service from within the callback
    [WcfFact]
    [OuterLoop]
    [Issue(1959)]
    public static void NonTerminatingMethodCallingDuplexCallbacksCallingService()
    {
        using (DuplexTestSetupHelper())
        {
            var result = s_channel.NonTerminatingMethodCallingDuplexCallbacks(  // + 1 
            callsToClientCallbackToMake: 2,                                     // + 2  
            callsToTerminatingClientCallbackToMake: 0,
            callsToClientSideOnlyTerminatingClientCallbackToMake: 0,
            callsToNonTerminatingMethodToMakeInsideClientCallback: 2,           // + 2 * 2
            callsToTerminatingMethodToMakeInsideClientCallback: 0);

            Assert.Equal(7, result);

            ResultsVerificationHelper<Exception>(canMakeServiceCall: true, cbClosedCalled: false, cbClosingCalled: false, cbFaultedCalled: false);
        }
    }

    // Call terminating method with client callback not calling the service again
    [WcfFact]
    [OuterLoop]
    public static void TerminatingMethodCallingDuplexCallbacksNoReentrantCalls()
    {
        using (DuplexTestSetupHelper())
        {
            var result = s_channel.TerminatingMethodCallingDuplexCallbacks(     // + 1
            callsToClientCallbackToMake: 2,                                     // + 2
            callsToTerminatingClientCallbackToMake: 0,
            callsToClientSideOnlyTerminatingClientCallbackToMake: 0,
            callsToNonTerminatingMethodToMakeInsideClientCallback: 0,
            callsToTerminatingMethodToMakeInsideClientCallback: 0);

            Assert.Equal(3, result);

            // verify that after this we can not make another call
            // note: we really expect ChannelTerminatedException but it is not exposed publicly yet
            // so we pass the base class instead
            ResultsVerificationHelper<CommunicationException>(
                canMakeServiceCall: false,
                cbClosedCalled: false,      // Note that closed hasn't been called in this case
                cbClosingCalled: false,
                cbFaultedCalled: false);
        }
    }

    // Call terminating method with client callback trying (and failing) to call non-terminating service method
    [WcfFact]
    [OuterLoop]
    [Issue(1959)]
    public static void TerminatingMethodCallingDuplexCallbacksFailingToCallService()
    {
        using (DuplexTestSetupHelper())
        {
            var result = s_channel.TerminatingMethodCallingDuplexCallbacks(     // + 1
            callsToClientCallbackToMake: 2,                                     // + 2 
            callsToTerminatingClientCallbackToMake: 0,
            callsToClientSideOnlyTerminatingClientCallbackToMake: 0,
            callsToNonTerminatingMethodToMakeInsideClientCallback: 2,           // + 0 as they will fail
            callsToTerminatingMethodToMakeInsideClientCallback: 0);

            Assert.Equal(3, result);

            // verify that after this we can not make another call
            // note: we really expect ChannelTerminatedException but it is not exposed publicly yet
            // so we pass the base class instead
            ResultsVerificationHelper<CommunicationException>(canMakeServiceCall: false, cbClosedCalled: false, cbClosingCalled: false, cbFaultedCalled: false);
        }
    }

    // Call non-terminating service method which calls client-side-only terminating callback 
    // Verify that the client will only accept one terminating callback call
    [WcfFact]
    [OuterLoop]
    public static void NonTerminatingMethodCallingClientSideOnlyTerminatingDuplexCallbacks()
    {
        using (DuplexTestSetupHelper())
        {
            var result = s_channel.NonTerminatingMethodCallingDuplexCallbacks(  // + 1 
            callsToClientCallbackToMake: 1,                                     // + 0
            callsToTerminatingClientCallbackToMake: 0,
            callsToClientSideOnlyTerminatingClientCallbackToMake: 2,            // + 1 (not 2)
            callsToNonTerminatingMethodToMakeInsideClientCallback: 0,
            callsToTerminatingMethodToMakeInsideClientCallback: 0);

            Assert.Equal(2, result);

            // even though the server can't call any callbacks anymore we can still call the service
            ResultsVerificationHelper<Exception>(canMakeServiceCall: true, cbClosedCalled: false, cbClosingCalled: false, cbFaultedCalled: false);
        }
    }

    // Call non-terminating service method which calls client-side-only terminating callback 
    // which calls a non-terminating service method
    [WcfFact]
    [OuterLoop]
    [Issue(1959)]
    public static void NonTerminatingMethodCallingClientSideTerminatingDuplexCallbacksCallingServer()
    {
        using (DuplexTestSetupHelper())
        {
            var result = s_channel.NonTerminatingMethodCallingDuplexCallbacks(  // + 1
            callsToClientCallbackToMake: 1,                                     // + 0
            callsToTerminatingClientCallbackToMake: 0,
            callsToClientSideOnlyTerminatingClientCallbackToMake: 2,            // + 1 (not 2)
            callsToNonTerminatingMethodToMakeInsideClientCallback: 2,           // + 2 (not 2*2)
            callsToTerminatingMethodToMakeInsideClientCallback: 0);

            Assert.Equal(4, result);

            // even though the server can't call any callbacks anymore we can still call the server
            ResultsVerificationHelper<Exception>(canMakeServiceCall: true, cbClosedCalled: false, cbClosingCalled: false, cbFaultedCalled: false);
        }
    }

    // A non-terminating method with non-terminating client callback calling terminating service method
    [WcfFact]
    [OuterLoop]
    [Issue(1959)]
    public static void NonTerminatingMethodCallingDuplexCallbacksCallingTerminatingService()
    {
        using (DuplexTestSetupHelper())
        {
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
    }

    // Call a non-terminating service method that calls terminating client callback
    [WcfFact]
    [OuterLoop]
    public static void NonTerminatingMethodCallingTerminatingDuplexCallbacks()
    {
        using (DuplexTestSetupHelper())
        {
            // Should be Assert.Throws<ChannelTerminatedException>(), () =>
            // but ChannelTerminatedException isn't exposed
            // and our version of Assert.Throws requires the exact exception type
            try
            {
                var result = s_channel.NonTerminatingMethodCallingDuplexCallbacks(
                callsToClientCallbackToMake: 0,
                callsToTerminatingClientCallbackToMake: 1,                      // this causes the original call to fail
                callsToClientSideOnlyTerminatingClientCallbackToMake: 0,
                callsToNonTerminatingMethodToMakeInsideClientCallback: 0,
                callsToTerminatingMethodToMakeInsideClientCallback: 0);
            }
            catch (Exception e)
            {
                Assert.True(e is CommunicationException);
            }
        }
    }

    // A few helper methods to reuse the common code
    private static SessionTestsDuplexCallback s_duplexCallback = null;
    private static ISessionTestsDuplexService s_channel = null;

    public class DuplexTestCleanupHelper : IDisposable
    {
        private List<ICommunicationObject> _objectsToClose = new List<ICommunicationObject>();

        public void AddObjectToClose(Object o)
        {
            _objectsToClose.Add((ICommunicationObject)o);
        }

        public void Dispose()
        {
            foreach (var o in _objectsToClose)
            {
                ScenarioTestHelpers.CloseCommunicationObjects(o);
            }
        }
    }

    // Returns IDisposable to unify the cleanup for all scenarios
    private static IDisposable DuplexTestSetupHelper()
    {
        var cleanupHelper = new DuplexTestCleanupHelper();
        var duplexEndpoint = new EndpointAddress(Endpoints.Tcp_Session_Tests_Duplex_Service);
        var binding = new NetTcpBinding();
        binding.Security = new NetTcpSecurity();
        binding.Security.Mode = SecurityMode.None;

        var duplexCallback = s_duplexCallback = new SessionTestsDuplexCallback();
        var instanceContext = new InstanceContext(duplexCallback);

        var factory = new DuplexChannelFactory<ISessionTestsDuplexService>(instanceContext, binding, duplexEndpoint);
        cleanupHelper.AddObjectToClose(factory);
        s_channel = factory.CreateChannel();
        cleanupHelper.AddObjectToClose(s_channel);
        ((ICommunicationObject)s_channel).Open();

        var ctxChannel = (IContextChannel)s_channel;
        ctxChannel.Closed += (sender, e) =>
        {
            Logger.LogInformation("Closed");
        };
        ctxChannel.Closing += (sender, e) =>
        {
            Logger.LogInformation("Closing");
        };
        ctxChannel.Faulted += (sender, e) =>
        {
            Logger.LogInformation("Faulted");
        };

        return cleanupHelper;
    }

    private static void ResultsVerificationHelper<ExpectedException>(bool canMakeServiceCall, bool cbClosedCalled, bool cbClosingCalled, bool cbFaultedCalled)
        where ExpectedException : Exception
    {
        if (canMakeServiceCall)
        {
            var result2 = s_channel.NonTerminatingMethod();
            Assert.Equal(1, result2);
        }
        else
        {
            // This should really be Assert.Throws(Is.Typeof<ExpectedException>(), () => {
            // but some of the exceptions that can be passed here are not exposed publicly yet
            // tracked by WCF issue #1962
            try
            {
                var result2 = s_channel.NonTerminatingMethod();
            }
            catch (Exception e)
            {
                Assert.True(e is ExpectedException);
            }
        }
        Assert.Equal(cbClosedCalled, s_duplexCallback.ClosedCalled);
        Assert.Equal(cbClosingCalled, s_duplexCallback.ClosingCalled);
        Assert.Equal(cbFaultedCalled, s_duplexCallback.FaultedCalled);
    }
}
