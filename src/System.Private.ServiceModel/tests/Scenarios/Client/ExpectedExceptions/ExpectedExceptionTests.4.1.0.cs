// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.IdentityModel.Selectors;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Threading;
using System.IO;

using Infrastructure.Common;


public partial class ExpectedExceptionTests : ConditionalWcfTest
{
    [WcfFact]
    [OuterLoop]
    public static void NotExistentHost_Throws_EndpointNotFoundException()
    {
        string nonExistentHost = "http://nonexisthost/WcfService/WindowsCommunicationFoundation";

        BasicHttpBinding binding = new BasicHttpBinding();
        binding.SendTimeout = TimeSpan.FromMilliseconds(20000);

        EndpointNotFoundException exception = Assert.Throws<EndpointNotFoundException>(() =>
        {
            using (ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(nonExistentHost)))
            {
                IWcfService serviceProxy = factory.CreateChannel();
                string response = serviceProxy.Echo("Hello");
            }
        });

        // On .Net Native retail, exception message is stripped to include only parameter
        Assert.True(exception.Message.Contains(nonExistentHost), string.Format("Expected exception message to contain: '{0}'", nonExistentHost));
    }

    [WcfFact]
    [OuterLoop]
    [Issue(398)]
    public static void ServiceRestart_Throws_CommunicationException()
    {
        StringBuilder errorBuilder = new StringBuilder();
        string restartServiceAddress = "";

        BasicHttpBinding binding = new BasicHttpBinding();

        try
        {
            using (ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic)))
            {
                IWcfService serviceProxy = factory.CreateChannel();
                restartServiceAddress = serviceProxy.GetRestartServiceEndpoint();
            }
        }
        catch (Exception e)
        {
            string error = String.Format("Unexpected exception thrown while calling the 'GetRestartServiceEndpoint' operation. {0}", e.ToString());
            if (e.InnerException != null)
                error += String.Format("\r\nInnerException:\r\n{0}", e.InnerException.ToString());
            errorBuilder.AppendLine(error);
        }

        if (errorBuilder.Length == 0)
        {
            // Get the Service host name and replace localhost with it
            UriBuilder builder = new UriBuilder(Endpoints.HttpBaseAddress_Basic);
            string hostName = builder.Uri.Host;
            restartServiceAddress = restartServiceAddress.Replace("[HOST]", hostName);
            //On .NET Native retail, exception message is stripped to include only parameter
            string expectExceptionMsg = restartServiceAddress;

            try
            {
                using (ChannelFactory<IWcfRestartService> factory = new ChannelFactory<IWcfRestartService>(binding, new EndpointAddress(restartServiceAddress)))
                {
                    // Get the last portion of the restart service url which is a Guid and convert it back to a Guid
                    // This is needed by the RestartService operation as a Dictionary key to get the ServiceHost
                    string uniqueIdentifier = restartServiceAddress.Substring(restartServiceAddress.LastIndexOf("/") + 1);
                    Guid guid = new Guid(uniqueIdentifier);

                    IWcfRestartService serviceProxy = factory.CreateChannel();
                    serviceProxy.RestartService(guid);
                }

                errorBuilder.AppendLine("Expected CommunicationException exception, but no exception thrown.");
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(CommunicationException))
                {
                    if (e.Message.Contains(expectExceptionMsg))
                    {
                    }
                    else
                    {
                        errorBuilder.AppendLine(string.Format("Expected exception message contains: {0}, actual: {1}", expectExceptionMsg, e.Message));
                    }
                }
                else
                {
                    errorBuilder.AppendLine(string.Format("Expected exception: {0}, actual: {1}/n Exception was: {2}", "CommunicationException", e.GetType(), e.ToString()));
                }
            }
        }

        Assert.True(errorBuilder.Length == 0, string.Format("Test Scenario: ServiceRestart_Throws_CommunicationException FAILED with the following errors: {0}", errorBuilder));
    }

    [WcfFact]
    [OuterLoop]
    public static void UnknownUrl_Throws_EndpointNotFoundException()
    {
        // We need a running service host at the other end but mangle the endpoint suffix
        string notFoundUrl = Endpoints.HttpBaseAddress_Basic + "not-an-endpoint";

        BasicHttpBinding binding = new BasicHttpBinding();
        binding.SendTimeout = TimeSpan.FromMilliseconds(10000);

        EndpointNotFoundException exception = Assert.Throws<EndpointNotFoundException>(() =>
        {
            try
            {
                using (
                    ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding,
                        new EndpointAddress(notFoundUrl)))
                {
                    IWcfService serviceProxy = factory.CreateChannel();
                    string response = serviceProxy.Echo("Hello");
                }
            }
            catch (EndpointNotFoundException)
            {
                throw;
            }
            catch (CommunicationException ce)
            {
                if (ce.InnerException == null)
                    throw;

                if (ce.InnerException.GetType() == typeof(HttpRequestException))
                {
                    var httpReqExcep = ce.InnerException as HttpRequestException;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Received HttpRequestException with unknown error code ")
                        .AppendLine(ce.InnerException.HResult.ToString())
                        .AppendLine("Full details for HttpRequestException:")
                        .AppendLine(httpReqExcep.ToString());
                    throw new CommunicationException(sb.ToString());
                }

                throw;
            }
        });

        // On .Net Native retail, exception message is stripped to include only parameter
        Assert.True(exception.Message.Contains(notFoundUrl), string.Format("Expected exception message to contain: '{0}'", notFoundUrl));
    }

    [WcfFact]
    [OuterLoop]
    public static void UnknownUrl_Throws_ProtocolException()
    {
        string protocolExceptionUri = Endpoints.HttpProtocolError_Address;

        BasicHttpBinding binding = new BasicHttpBinding();
        binding.SendTimeout = TimeSpan.FromMilliseconds(10000);

        using (ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(protocolExceptionUri)))
        {
            IWcfService serviceProxy = factory.CreateChannel();
            ProtocolException exception = Assert.Throws<ProtocolException>(() =>
            {
                string response = serviceProxy.Echo("Hello");
            });

            // On .Net Native retail, exception message is stripped to include only parameter
            Assert.True(exception.Message.Contains(protocolExceptionUri), string.Format("Expected exception message to contain '{0}'", protocolExceptionUri));
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void DuplexCallback_Throws_FaultException_DirectThrow()
    {
        DuplexChannelFactory<IWcfDuplexTaskReturnService> factory = null;
        Guid guid = Guid.NewGuid();

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.None;

        DuplexTaskReturnServiceCallback callbackService = new DuplexTaskReturnServiceCallback(true);
        InstanceContext context = new InstanceContext(callbackService);

        try
        {
            var exception = Assert.Throws<FaultException<FaultDetail>>(() =>
            {
                factory = new DuplexChannelFactory<IWcfDuplexTaskReturnService>(context, binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_TaskReturn_Address));
                IWcfDuplexTaskReturnService serviceProxy = factory.CreateChannel();

                Task<Guid> task = serviceProxy.FaultPing(guid);
                if ((task as IAsyncResult).AsyncWaitHandle.WaitOne(ScenarioTestHelpers.TestTimeout))
                {
                    Guid returnedGuid = task.GetAwaiter().GetResult();
                }
                else
                {
                    throw new TimeoutException(String.Format("The call to the Service did not complete within the alloted time of: {0}", ScenarioTestHelpers.TestTimeout));
                }

                // Not closing the factory as an exception will always be thrown prior to this point.
            });

            Assert.Equal("ServicePingFaultCallback", exception.Code.Name);
            Assert.Equal("Reason: Testing FaultException returned from Duplex Callback", exception.Reason.GetMatchingTranslation().Text);
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void DuplexCallback_Throws_FaultException_ReturnsFaultedTask()
    {
        DuplexChannelFactory<IWcfDuplexTaskReturnService> factory = null;
        Guid guid = Guid.NewGuid();

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.None;

        DuplexTaskReturnServiceCallback callbackService = new DuplexTaskReturnServiceCallback();
        InstanceContext context = new InstanceContext(callbackService);

        try
        {
            var exception = Assert.Throws<FaultException<FaultDetail>>(() =>
            {
                factory = new DuplexChannelFactory<IWcfDuplexTaskReturnService>(context, binding, new EndpointAddress(Endpoints.Tcp_NoSecurity_TaskReturn_Address));
                IWcfDuplexTaskReturnService serviceProxy = factory.CreateChannel();

                Task<Guid> task = serviceProxy.FaultPing(guid);
                if ((task as IAsyncResult).AsyncWaitHandle.WaitOne(ScenarioTestHelpers.TestTimeout))
                {
                    Guid returnedGuid = task.GetAwaiter().GetResult();
                }
                else
                {
                    throw new TimeoutException(String.Format("The call to the Service did not complete within the alloted time of: {0}", ScenarioTestHelpers.TestTimeout));
                }

                // Not closing the factory as an exception will always be thrown prior to this point.
            });

            Assert.Equal("ServicePingFaultCallback", exception.Code.Name);
            Assert.Equal("Reason: Testing FaultException returned from Duplex Callback", exception.Reason.GetMatchingTranslation().Text);
        }
        finally
        {
            if (factory != null && factory.State != CommunicationState.Closed)
            {
                factory.Abort();
            }
        }
    }

    [WcfFact]
    [OuterLoop]
    // Verify product throws MessageSecurityException when the Dns identity from the server does not match the expectation
    public static void TCP_ServiceCertExpired_Throw_MessageSecurityException()
    {
        string testString = "Hello";

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.Transport;
        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

        EndpointAddress endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_ExpiredServerCertResource_Address));
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
        IWcfService serviceProxy = factory.CreateChannel();

        try
        {
            var result = serviceProxy.Echo(testString);
            Assert.True(false, "Expected: SecurityNegotiationException, Actual: no exception");
        }
        catch (CommunicationException exception)
        {
            string exceptionType = exception.GetType().Name;
            if (exceptionType != "SecurityNegotiationException")
            {
                Assert.True(false, string.Format("Expected type SecurityNegotiationException, Actual: {0}", exceptionType));
            }
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects(factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    // Verify product throws SecurityNegotiationException when the service cert is revoked
    public static void TCP_ServiceCertRevoked_Throw_SecurityNegotiationException()
    {
        string testString = "Hello";

        NetTcpBinding binding = new NetTcpBinding();
        binding.Security.Mode = SecurityMode.Transport;
        binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;

        EndpointAddress endpointAddress = new EndpointAddress(new Uri(Endpoints.Tcp_RevokedServerCertResource_Address), new DnsEndpointIdentity(Endpoints.Tcp_RevokedServerCertResource_HostName));
        ChannelFactory<IWcfService> factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
        IWcfService serviceProxy = factory.CreateChannel();

        try
        {
            var result = serviceProxy.Echo(testString);
            Assert.True(false, "Expected: SecurityNegotiationException, Actual: no exception");
        }
        catch (CommunicationException exception)
        {
            string exceptionType = exception.GetType().Name;
            if (exceptionType != "SecurityNegotiationException")
            {
                Assert.True(false, string.Format("Expected type SecurityNegotiationException, Actual: {0}", exceptionType));
            }
            string exceptionMessage = exception.Message;
            Assert.True(exceptionMessage.Contains(Endpoints.Tcp_RevokedServerCertResource_HostName), string.Format("Expected message contains {0}, actual message: {1}", Endpoints.Tcp_RevokedServerCertResource_HostName, exception.ToString()));
        }
        finally
        {
            ScenarioTestHelpers.CloseCommunicationObjects(factory);
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
            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.HttpBaseAddress_Basic));
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
                        Assert.True(false, "The Opening event timed out waiting for operations to queue, which was not expected for this test.");
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
