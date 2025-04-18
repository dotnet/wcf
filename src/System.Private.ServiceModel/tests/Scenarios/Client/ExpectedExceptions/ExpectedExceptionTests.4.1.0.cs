// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using Infrastructure.Common;
using System;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public partial class ExpectedExceptionTests : ConditionalWcfTest
{
    [WcfFact]
    [OuterLoop]
    public static void NotExistentHost_Throws_EndpointNotFoundException()
    {
        string nonExistentHost = "http://nonexisthost/WcfService/WindowsCommunicationFoundation";
        ChannelFactory<IWcfService> factory = null;
        EndpointAddress endpointAddress = null;
        BasicHttpBinding binding = null;
        IWcfService serviceProxy = null;


        // *** VALIDATE *** \\
        EndpointNotFoundException exception = Assert.Throws<EndpointNotFoundException>(() =>
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            binding.SendTimeout = TimeSpan.FromMilliseconds(40000);
            endpointAddress = new EndpointAddress(nonExistentHost);
            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            try
            {
                serviceProxy.Echo("Hello");
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }
        });

        // *** ADDITIONAL VALIDATION FOR NET NATIVE *** \\
        // On .Net Native retail, exception message is stripped to include only parameter
        Assert.True(exception.Message.Contains(nonExistentHost), string.Format("Expected exception message to contain: '{0}'\nThe exception message was: {1}", nonExistentHost, exception.Message));
    }

    [WcfFact]
    [OuterLoop]
    public static void ServiceRestart_Throws_CommunicationException()
    {
        // This test validates that if the Service were to shut-down, re-start or otherwise die in the
        // middle of an operation the client will not just hang. It should instead receive a CommunicationException.
        string restartServiceAddress = "";
        ChannelFactory<IWcfService> setupHostFactory = null;
        IWcfService setupHostServiceProxy = null;
        ChannelFactory<IWcfRestartService> factory = null;
        IWcfRestartService serviceProxy = null;
        BasicHttpBinding binding = new BasicHttpBinding();

        // *** Step 1 *** \\
        // We need the Service to create and open a ServiceHost and then give us the endpoint address for it.
        try
        {
            setupHostFactory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            setupHostServiceProxy = setupHostFactory.CreateChannel();
            restartServiceAddress = setupHostServiceProxy.GetRestartServiceEndpoint();

            // *** CLEANUP *** \\
            ((ICommunicationObject)setupHostServiceProxy).Close();
            setupHostFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)setupHostServiceProxy, setupHostFactory);
        }

        // *** Additional Setup *** \\
        // The restartServiceAddress we got from the Service used localhost as the host name.
        // We need the actual host name for the client call to work.
        // To make it easier to parse, localhost was replaced with '[HOST]'.

        // Use Endpoints.HttpBaseAddress_Basic only for the purpose of extracting the Service host name.
        // Then update 'restartServiceAddress' with it.
        string hostName = new Uri(Endpoints.HttpBaseAddress_Basic_Text).Host;
        restartServiceAddress = restartServiceAddress.Replace("[HOST]", hostName);

        // Get the last portion of the restart service url which is a Guid and convert it back to a Guid
        // This is needed by the RestartService operation as a Dictionary key to get the ServiceHost
        string uniqueIdentifier = restartServiceAddress.Substring(restartServiceAddress.LastIndexOf("/") + 1);
        Guid guid = new Guid(uniqueIdentifier);

        // *** Step 2 *** \\
        // Simple echo call to make sure the newly created endpoint is working.
        try
        {
            factory = new ChannelFactory<IWcfRestartService>(binding, new EndpointAddress(restartServiceAddress));
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            string result = serviceProxy.NonRestartService(guid);

            Assert.True(result == "Success!", string.Format("Test Case failed, expected the returned string to be: {0}, instead it was: {1}", "Success!", result));
        }
        catch (Exception ex)
        {
            string exceptionMessage = ex.Message;
            string innerExceptionMessage = ex.InnerException?.Message;
            string testExceptionMessage = $"The ping to validate the newly created endpoint failed.\nThe endpoint pinged was: {restartServiceAddress}\nThe GUID used to extract the ServiceHost from the server side dictionary was: {guid}";
            string fullExceptionMessage = $"testExceptionMessage: {testExceptionMessage}\nexceptionMessage: {exceptionMessage}\ninnerExceptionMessage: {innerExceptionMessage}";
            
            Assert.Fail(fullExceptionMessage);
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }

        // *** Step 3 *** \\
        // The actual part of the test where the host is killed in the middle of the operation.
        // We expect the test should not hang and should receive a CommunicationException.
        CommunicationException exception = Assert.Throws<CommunicationException>(() =>
        {
            factory = new ChannelFactory<IWcfRestartService>(binding, new EndpointAddress(restartServiceAddress));
            serviceProxy = factory.CreateChannel();

            try
            {
                // *** EXECUTE *** \\
                serviceProxy.RestartService(guid);
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }
        });
    }

    [WcfFact]
    [OuterLoop]
    public static void UnknownUrl_Throws_EndpointNotFoundException()
    {
        // We need a running service host at the other end but mangle the endpoint suffix
        string notFoundUrl = Endpoints.HttpBaseAddress_Basic_Text + "not-an-endpoint";
        BasicHttpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        EndpointAddress endpointAddress = null;
        IWcfService serviceProxy = null;

        // *** VALIDATE *** \\
        EndpointNotFoundException exception = Assert.Throws<EndpointNotFoundException>(() =>
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            binding.SendTimeout = TimeSpan.FromMilliseconds(10000);
            endpointAddress = new EndpointAddress(notFoundUrl);
            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            try
            {
                serviceProxy.Echo("Hello");
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }

        });

        // *** ADDITIONAL VALIDATION FOR NET NATIVE *** \\
        // On .Net Native retail, exception message is stripped to include only parameter
        Assert.True(exception.Message.Contains(notFoundUrl), string.Format("Expected exception message to contain: '{0}'\nThe exception message was:{1}", notFoundUrl, exception.Message));
    }

    [WcfFact]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void UnknownUrl_Throws_ProtocolException()
    {
        string protocolExceptionUri = Endpoints.HttpProtocolError_Address;
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        // *** VALIDATE *** \\
        ProtocolException exception = Assert.Throws<ProtocolException>(() =>
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            binding.SendTimeout = TimeSpan.FromMilliseconds(10000);
            endpointAddress = new EndpointAddress(protocolExceptionUri);
            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            try
            {
                serviceProxy.Echo("Hello");
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }

        });

        // *** ADDITIONAL VALIDATION FOR NET NATIVE *** \\
        // On .Net Native retail, exception message is stripped to include only parameter
        Assert.True(exception.Message.Contains(protocolExceptionUri), string.Format("Expected exception message to contain: '{0}'\nThe exception was: '{1}'", protocolExceptionUri, exception.Message));
    }

    [WcfFact]
    [OuterLoop]
    public static void DuplexCallback_Throws_FaultException_DirectThrow()
    {
        DuplexChannelFactory<IWcfDuplexTaskReturnService> factory = null;
        Guid guid = Guid.NewGuid();
        EndpointAddress endpointAddress = null;
        NetTcpBinding binding = null;
        DuplexTaskReturnServiceCallback callbackService = null;
        InstanceContext context = null;
        IWcfDuplexTaskReturnService serviceProxy = null;

        // *** VALIDATE *** \\
        FaultException<FaultDetail> exception = Assert.Throws<FaultException<FaultDetail>>(() =>
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            callbackService = new DuplexTaskReturnServiceCallback(true);
            context = new InstanceContext(callbackService);
            endpointAddress = new EndpointAddress(Endpoints.Tcp_NoSecurity_TaskReturn_Address);
            factory = new DuplexChannelFactory<IWcfDuplexTaskReturnService>(context, binding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            try
            {
                Task<Guid> task = serviceProxy.FaultPing(guid);
                if ((task as IAsyncResult).AsyncWaitHandle.WaitOne(ScenarioTestHelpers.TestTimeout))
                {
                    Guid returnedGuid = task.GetAwaiter().GetResult();
                }
                else
                {
                    throw new TimeoutException(String.Format("The call to the Service did not complete within the alloted time of: {0}", ScenarioTestHelpers.TestTimeout));
                }
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }
        });

        // *** ADDITIONAL VALIDATION *** \\
        string exceptionCodeName = "ServicePingFaultCallback";
        string exceptionReason = "Reason: Testing FaultException returned from Duplex Callback";

        Assert.True(String.Equals(exceptionCodeName, exception.Code.Name), String.Format("Expected exception code name: {0}\nActual exception code name: {1}", exceptionCodeName, exception.Code.Name));
        Assert.True(String.Equals(exceptionReason, exception.Reason.GetMatchingTranslation().Text), String.Format("Expected exception reason: {0}\nActual exception reason: {1}", exceptionReason, exception.Reason.GetMatchingTranslation().Text));
    }

    [WcfFact]
    [OuterLoop]
    public static void DuplexCallback_Throws_FaultException_ReturnsFaultedTask()
    {
        DuplexChannelFactory<IWcfDuplexTaskReturnService> factory = null;
        Guid guid = Guid.NewGuid();
        NetTcpBinding binding = null;
        DuplexTaskReturnServiceCallback callbackService = null;
        InstanceContext context = null;
        EndpointAddress endpointAddress = null;
        IWcfDuplexTaskReturnService serviceProxy = null;

        // *** VALIDATE *** \\
        FaultException<FaultDetail> exception = Assert.Throws<FaultException<FaultDetail>>(() =>
        {
            // *** SETUP *** \\
            binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;
            endpointAddress = new EndpointAddress(Endpoints.Tcp_NoSecurity_TaskReturn_Address);
            callbackService = new DuplexTaskReturnServiceCallback();
            context = new InstanceContext(callbackService);
            factory = new DuplexChannelFactory<IWcfDuplexTaskReturnService>(context, binding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            try
            {
                Task<Guid> task = serviceProxy.FaultPing(guid);
                if ((task as IAsyncResult).AsyncWaitHandle.WaitOne(ScenarioTestHelpers.TestTimeout))
                {
                    Guid returnedGuid = task.GetAwaiter().GetResult();
                }
                else
                {
                    throw new TimeoutException(String.Format("The call to the Service did not complete within the alloted time of: {0}", ScenarioTestHelpers.TestTimeout));
                }
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }

        });

        // *** ADDITIONAL VALIDATION *** \\
        string exceptionCodeName = "ServicePingFaultCallback";
        string exceptionReason = "Reason: Testing FaultException returned from Duplex Callback";

        Assert.True(String.Equals(exceptionCodeName, exception.Code.Name), String.Format("Expected exception code name: {0}\nActual exception code name: {1}", exceptionCodeName, exception.Code.Name));
        Assert.True(String.Equals(exceptionReason, exception.Reason.GetMatchingTranslation().Text), String.Format("Expected exception reason: {0}\nActual exception reason: {1}", exceptionReason, exception.Reason.GetMatchingTranslation().Text));
    }

    [WcfFact]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    // Verify product throws MessageSecurityException when the Dns identity from the server does not match the expectation
    public static void TCP_ServiceCertExpired_Throw_MessageSecurityException()
    {
        string testString = "Hello";
        NetTcpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;


        // *** SETUP *** \\
        binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.Transport;
        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
        endpointAddress = new EndpointAddress(Endpoints.Tcp_ExpiredServerCertResource_Address);
        factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
        serviceProxy = factory.CreateChannel();

        // *** EXECUTE *** \\
        try
        {
            serviceProxy.Echo(testString);
            Assert.Fail("Expected: SecurityNegotiationException, Actual: no exception");
        }
        catch (CommunicationException exception)
        {
            // *** VALIDATE *** \\
            // Cannot explicitly catch a SecurityNegotiationException as it is not in the public contract.
            string exceptionType = exception.GetType().Name;
            if (exceptionType != "SecurityNegotiationException")
            {
                Assert.Fail(string.Format("Expected type SecurityNegotiationException, Actual: {0}", exceptionType));
            }
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
    // Verify product throws SecurityNegotiationException when the service cert is revoked
    public static void TCP_ServiceCertRevoked_Throw_SecurityNegotiationException()
    {
        string testString = "Hello";
        NetTcpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        // *** SETUP *** \\
        binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.Transport;
        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
        endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_RevokedServerCertResource_Address), new DnsEndpointIdentity(Endpoints.Tcp_RevokedServerCertResource_HostName));
        factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
        serviceProxy = factory.CreateChannel();

        // *** EXECUTE *** \\
        try
        {
            serviceProxy.Echo(testString);
            Assert.Fail("Expected: SecurityNegotiationException, Actual: no exception");
        }
        catch (CommunicationException exception)
        {
            // *** VALIDATION *** \\
            // Cannot explicitly catch a SecurityNegotiationException as it is not in the public contract.
            string exceptionType = exception.GetType().Name;
            if (exceptionType != "SecurityNegotiationException")
            {
                Assert.Fail(string.Format("Expected type SecurityNegotiationException, Actual: {0}", exceptionType));
            }

            Assert.True(exception.Message.Contains(Endpoints.Tcp_RevokedServerCertResource_HostName),
                                                    string.Format("Expected message contains {0}, actual message: {1}",
                                                    Endpoints.Tcp_RevokedServerCertResource_HostName,
                                                    exception.ToString()));
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
    // Verify product throws SecurityNegotiationException when the service cert only has the ClientAuth usage
    public static void TCP_ServiceCertInvalidEKU_Throw_SecurityNegotiationException()
    {
        string testString = "Hello";
        NetTcpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        // *** SETUP *** \\
        binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.Transport;
        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;
        endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_InvalidEkuServerCertResource_Address), new DnsEndpointIdentity(Endpoints.Tcp_InvalidEkuServerCertResource_HostName));
        factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
        serviceProxy = factory.CreateChannel();

        // *** EXECUTE *** \\
        try
        {
            serviceProxy.Echo(testString);
            Assert.Fail("Expected: SecurityNegotiationException, Actual: no exception");
        }
        catch (CommunicationException exception)
        {
            // *** VALIDATION *** \\
            // Cannot explicitly catch a SecurityNegotiationException as it is not in the public contract.
            string exceptionType = exception.GetType().Name;
            if (exceptionType != "SecurityNegotiationException")
            {
                Assert.Fail(string.Format("Expected type SecurityNegotiationException, Actual: {0}", exceptionType));
            }

            Assert.True(exception.Message.Contains(Endpoints.Tcp_RevokedServerCertResource_HostName),
                                                    string.Format("Expected message contains {0}, actual message: {1}",
                                                    Endpoints.Tcp_RevokedServerCertResource_HostName,
                                                    exception.ToString()));
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void Abort_During_Implicit_Open_Closes_Sync_Waiters()
    {
        // This test is a regression test of an issue with CallOnceManager.
        // When a single proxy is used to make several service calls without
        // explicitly opening it, the CallOnceManager queues up all the requests
        // that happen while it is opening the channel (or handling previously
        // queued service calls.  If the channel was closed or faulted during
        // the handling of any queued requests, it caused a pathological worst
        // case where every queued request waited for its complete SendTimeout
        // before failing.
        //
        // This test operates by making multiple concurrent synchronous service
        // calls, but stalls the Opening event to allow them to be queued before
        // any of them are allowed to proceed.  It then aborts the channel when
        // the first service operation is allowed to proceed.  This causes the
        // CallOnce manager to deal with all its queued operations and cause
        // them to complete other than by timing out.

        BasicHttpBinding binding = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        int timeoutMs = 20000;
        long operationsQueued = 0;
        int operationCount = 5;
        Task<string>[] tasks = new Task<string>[operationCount];
        Exception[] exceptions = new Exception[operationCount];
        string[] results = new string[operationCount];
        bool isClosed = false;
        DateTime endOfOpeningStall = DateTime.Now;
        int serverDelayMs = 100;
        TimeSpan serverDelayTimeSpan = TimeSpan.FromMilliseconds(serverDelayMs);
        string testMessage = "testMessage";

        try
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding(BasicHttpSecurityMode.None);
            binding.TransferMode = TransferMode.Streamed;
            // SendTimeout is the timeout used for implicit opens
            binding.SendTimeout = TimeSpan.FromMilliseconds(timeoutMs);
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text));
            serviceProxy = factory.CreateChannel();

            // Force the implicit open to stall until we have multiple concurrent calls pending.
            // This forces the CallOnceManager to have a queue of waiters it will need to notify.
            ((ICommunicationObject)serviceProxy).Opening += (s, e) =>
            {
                // Wait until we see sync calls have been queued
                DateTime startOfOpeningStall = DateTime.Now;
                while (true)
                {
                    endOfOpeningStall = DateTime.Now;

                    // Don't wait forever -- if we stall longer than the SendTimeout, it means something
                    // is wrong other than what we are testing, so just fail early.
                    if ((endOfOpeningStall - startOfOpeningStall).TotalMilliseconds > timeoutMs)
                    {
                        Assert.Fail("The Opening event timed out waiting for operations to queue, which was not expected for this test.");
                    }

                    // As soon as we have all our Tasks at least running, wait a little
                    // longer to allow them finish queuing up their waiters, then stop stalling the Opening
                    if (Interlocked.Read(ref operationsQueued) >= operationCount)
                    {
                        Task.Delay(500).Wait();
                        endOfOpeningStall = DateTime.Now;
                        return;
                    }

                    Task.Delay(100).Wait();
                }
            };

            // Each task will make a synchronous service call, which will cause all but the
            // first to be queued for the implicit open.  The first call to complete then closes
            // the channel so that it is forced to deal with queued waiters.
            Func<string> callFunc = () =>
            {
                // We increment the # ops queued before making the actual sync call, which is
                // technically a short race condition in the test.  But reversing the order would
                // timeout the implicit open and fault the channel. 
                Interlocked.Increment(ref operationsQueued);

                // The call of the operation is what creates the entry in the CallOnceManager queue.
                // So as each Task below starts, it increments the count and adds a waiter to the
                // queue.  We ask for a small delay on the server side just to introduce a small
                // stall after the sync request has been made before it can complete.  Otherwise
                // fast machines can finish all the requests before the first one finishes the Close().
                string result = serviceProxy.EchoWithTimeout("test", serverDelayTimeSpan);
                lock (tasks)
                {
                    if (!isClosed)
                    {
                        try
                        {
                            isClosed = true;
                            ((ICommunicationObject)serviceProxy).Abort();
                        }
                        catch { }
                    }
                }
                return result;
            };

            // *** EXECUTE *** \\

            DateTime startTime = DateTime.Now;
            for (int i = 0; i < operationCount; ++i)
            {
                tasks[i] = Task.Run(callFunc);
            }

            for (int i = 0; i < operationCount; ++i)
            {
                try
                {
                    results[i] = tasks[i].GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    exceptions[i] = ex;
                }
            }

            // *** VALIDATE *** \\
            double elapsedMs = (DateTime.Now - endOfOpeningStall).TotalMilliseconds;

            // Before validating that the issue was fixed, first validate that we received the exceptions or the
            // results we expected. This is to verify the fix did not introduce a behavioral change other than the
            // elimination of the long unnecessary timeouts after the channel was closed.
            int nFailures = 0;
            for (int i = 0; i < operationCount; ++i)
            {
                if (exceptions[i] == null)
                {
                    Assert.True((String.Equals("test", results[i])),
                                    String.Format("Expected operation #{0} to return '{1}' but actual was '{2}'",
                                                    i, testMessage, results[i]));
                }
                else
                {
                    ++nFailures;

                    TimeoutException toe = exceptions[i] as TimeoutException;
                    Assert.True(toe == null, String.Format("Task [{0}] should not have failed with TimeoutException", i));
                }
            }

            Assert.True(nFailures > 0,
                String.Format("Expected at least one operation to throw an exception, but none did. Elapsed time = {0} ms.",
                    elapsedMs));

            Assert.True(nFailures < operationCount,
                String.Format("Expected at least one operation to succeed but none did. Elapsed time = {0} ms.",
                    elapsedMs));

            // The original issue was that sync waiters in the CallOnceManager were not notified when
            // the channel became unusable and therefore continued to time out for the full amount.
            // Additionally, because they were executed sequentially, it was also possible for each one
            // to time out for the full amount.  Given that we closed the channel, we expect all the queued
            // waiters to have been immediately waked up and detected failure.
            int expectedElapsedMs = (operationCount * serverDelayMs) + timeoutMs / 2;
            Assert.True(elapsedMs < expectedElapsedMs,
                        String.Format("The {0} operations took {1} ms to complete which exceeds the expected {2} ms",
                                      operationCount, elapsedMs, expectedElapsedMs));

            // *** CLEANUP *** \\
            ((ICommunicationObject)serviceProxy).Close();
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
