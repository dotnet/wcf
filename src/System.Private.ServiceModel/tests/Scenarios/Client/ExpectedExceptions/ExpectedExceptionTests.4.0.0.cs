// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Infrastructure.Common;
using System;
using System.Diagnostics;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public partial class ExpectedExceptionTests : ConditionalWcfTest
{
    [WcfFact]
    [OuterLoop]
    public static void NonExistentAction_Throws_ActionNotSupportedException()
    {
        string exceptionMsg = "The message with Action 'http://tempuri.org/IWcfService/NotExistOnServer' cannot be processed at the receiver, due to a ContractFilter mismatch at the EndpointDispatcher. This may be because of either a contract mismatch (mismatched Actions between sender and receiver) or a binding/security mismatch between the sender and the receiver.  Check that sender and receiver have the same contract and the same binding (including security requirements, e.g. Message, Transport, None).";
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        // *** VALIDATE *** \\
        ActionNotSupportedException exception = Assert.Throws<ActionNotSupportedException>(() =>
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            endpointAddress = new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text);
            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            try
            {
                serviceProxy.NotExistOnServer();
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }
        });

        // *** ADDITIONAL VALIDATION *** \\
        Assert.True(String.Equals(exception.Message, exceptionMsg), String.Format("Expected exception message: {0}\nActual exception message: {1}", exceptionMsg, exception.Message));
    }

    // SendTimeout is set to 5 seconds, the service waits 10 seconds to respond.
    // The client should throw a TimeoutException
    [WcfFact]
    [OuterLoop]
    public static void SendTimeout_For_Long_Running_Operation_Throws_TimeoutException()
    {
        TimeSpan serviceOperationTimeout = TimeSpan.FromMilliseconds(10000);
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stopwatch watch = null;
        int lowRange = 4985;
        int highRange = 6000;

        // *** VALIDATE *** \\
        TimeoutException exception = Assert.Throws<TimeoutException>(() =>
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            binding.SendTimeout = TimeSpan.FromMilliseconds(5000);
            endpointAddress = new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text);
            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();
            watch = new Stopwatch();

            // *** EXECUTE *** \\
            try
            {
                watch = new Stopwatch();
                watch.Start();
                serviceProxy.EchoWithTimeout("Hello", serviceOperationTimeout);
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                watch.Stop();
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }
        });

        // *** ADDITIONAL VALIDATION *** \\
        // want to assert that this completed in > 5 s as an upper bound since the SendTimeout is 5 sec
        // (usual case is around 5001-5005 ms) 
        Assert.True((watch.ElapsedMilliseconds >= lowRange && watch.ElapsedMilliseconds <= highRange),
            String.Format("Expected elapsed time to be >= to {0} and <= to {1}\nActual elapsed time was: {2}", lowRange, highRange, watch.ElapsedMilliseconds));
    }

    // SendTimeout is set to 0, this should trigger a TimeoutException before even attempting to call the service.
    [WcfFact]
    [OuterLoop]
    public static void SendTimeout_Zero_Throws_TimeoutException_Immediately()
    {
        TimeSpan serviceOperationTimeout = TimeSpan.FromMilliseconds(5000);
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stopwatch watch = null;
        int lowRange = 0;
        int highRange = 2000;

        // *** VALIDATE *** \\
        TimeoutException exception = Assert.Throws<TimeoutException>(() =>
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            binding.SendTimeout = TimeSpan.FromMilliseconds(0);
            endpointAddress = new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text);
            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();
            watch = new Stopwatch();

            // *** EXECUTE *** \\
            try
            {
                watch.Start();
                serviceProxy.EchoWithTimeout("Hello", serviceOperationTimeout);
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                watch.Stop();
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }

        });

        // *** ADDITIONAL VALIDATION *** \\
        // want to assert that this completed in < 2 s as an upper bound since the SendTimeout is 0 sec
        // (usual case is around 1 - 3 ms) 
        Assert.True((watch.ElapsedMilliseconds >= lowRange && watch.ElapsedMilliseconds <= highRange),
            String.Format("Expected elapsed time to be >= to {0} and <= to {1}\nActual elapsed time was: {2}", lowRange, highRange, watch.ElapsedMilliseconds));
    }

    [WcfFact]
    [OuterLoop]
    public static void FaultException_Throws_WithFaultDetail()
    {
        string faultMsg = "Test Fault Exception";
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        // *** VALIDATE *** \\
        FaultException<FaultDetail> exception = Assert.Throws<FaultException<FaultDetail>>(() =>
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            endpointAddress = new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text);
            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            try
            {
                serviceProxy.TestFault(faultMsg);
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }
        });

        // *** ADDITIONAL VALIDATION *** \\
        Assert.True(String.Equals(exception.Detail.Message, faultMsg), String.Format("Expected fault message: {0}\nActual fault message: {1}", faultMsg, exception.Detail.Message));
    }

    [WcfFact]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static void UnexpectedException_Throws_FaultException()
    {
        string faultMsg = "This is a test fault msg";
        BasicHttpBinding binding = null;
        EndpointAddress endpointAddress = null;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;

        // *** VALIDATE *** \\
        FaultException<ExceptionDetail> exception = Assert.Throws<FaultException<ExceptionDetail>>(() =>
        {
            // *** SETUP *** \\
            binding = new BasicHttpBinding();
            endpointAddress = new EndpointAddress(Endpoints.HttpBaseAddress_Basic_Text);
            factory = new ChannelFactory<IWcfService>(binding, endpointAddress);
            serviceProxy = factory.CreateChannel();

            // *** EXECUTE *** \\
            try
            {
                serviceProxy.ThrowInvalidOperationException(faultMsg);
            }
            finally
            {
                // *** ENSURE CLEANUP *** \\
                ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
            }
        });

        // *** ADDITIONAL VALIDATION *** \\
        Assert.True(String.Equals(exception.Detail.Message, faultMsg), String.Format("Expected fault message: {0}\nActual fault message: {1}", faultMsg, exception.Detail.Message));
    }

    [WcfFact]
    [OuterLoop]
    public static void Abort_During_Implicit_Open_Closes_Async_Waiters()
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
        // This test operates by making multiple concurrent asynchronous service
        // calls, but stalls the Opening event to allow them to be queued before
        // any of them are allowed to proceed.  It then closes the channel when
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
                Task<string> t = serviceProxy.EchoWithTimeoutAsync(testMessage, serverDelayTimeSpan);
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
                return t.GetAwaiter().GetResult();
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


            // --- Here is the test of the actual bug fix ---
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

public class MyCertificateValidator : X509CertificateValidator
{
    public const string exceptionMsg = "Throwing exception from Validate method on purpose.";

    public override void Validate(X509Certificate2 certificate)
    {
        // Always throw an exception.
        // MSDN guidance also uses a simple Exception when an exception is thrown from this method.
        throw new Exception(exceptionMsg);
    }
}
