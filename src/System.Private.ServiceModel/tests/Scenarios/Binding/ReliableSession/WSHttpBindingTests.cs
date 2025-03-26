// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public class Binding_ReliableSession_WSHttpBindingTests : ConditionalWcfTest
{
    [WcfTheory]
    [MemberData(nameof(GetTestVariations))]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static async Task EchoCall(ReliableMessagingVersion rmVersion, bool ordered, string endpointSuffix)
    {
        string testString = "Hello";
        ChannelFactory<IWcfReliableService> factory = null;
        IWcfReliableService serviceProxy = null;
        WSHttpBinding binding = null;

        try
        {
            // *** SETUP *** \\
            binding = new WSHttpBinding(SecurityMode.None, true);
            binding.ReliableSession.Ordered = ordered;
            var customBinding = new CustomBinding(binding);
            var reliableSessionBindingElement = customBinding.Elements.Find<ReliableSessionBindingElement>();
            reliableSessionBindingElement.ReliableMessagingVersion = rmVersion;
            factory = new ChannelFactory<IWcfReliableService>(customBinding, new EndpointAddress(Endpoints.ReliableSession_WSHttp + endpointSuffix));
            serviceProxy = factory.CreateChannel();
            // *** EXECUTE *** \\
            ((IClientChannel)serviceProxy).Open(); // This will establish a reliable session
            var result = await serviceProxy.EchoAsync(testString);
            // *** VALIDATE *** \\
            Assert.Equal(testString, result);

            // *** CLEANUP *** \\
            ((IClientChannel)serviceProxy).Close();
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfTheory]
    [MemberData(nameof(GetTestVariations))]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static async Task OneWayCall(ReliableMessagingVersion rmVersion, bool ordered, string endpointSuffix)
    {
        string testString = "Hello";
        ChannelFactory<IOneWayWcfReliableService> factory = null;
        IOneWayWcfReliableService serviceProxy = null;
        WSHttpBinding binding = null;

        try
        {
            // *** SETUP *** \\
            binding = new WSHttpBinding(SecurityMode.None, true);
            binding.ReliableSession.Ordered = ordered;
            var customBinding = new CustomBinding(binding);
            var reliableSessionBindingElement = customBinding.Elements.Find<ReliableSessionBindingElement>();
            reliableSessionBindingElement.ReliableMessagingVersion = rmVersion;
            factory = new ChannelFactory<IOneWayWcfReliableService>(customBinding, new EndpointAddress(Endpoints.ReliableOneWaySession_WSHttp + endpointSuffix));
            serviceProxy = factory.CreateChannel();
            // *** EXECUTE *** \\
            ((IClientChannel)serviceProxy).Open(); // This will establish a reliable session
            await serviceProxy.OneWayAsync(testString);
            // *** VALIDATE *** \\

            // *** CLEANUP *** \\
            ((IClientChannel)serviceProxy).Close();
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfTheory]
    [MemberData(nameof(GetTestVariations))]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static async Task ResendFailedRequest(ReliableMessagingVersion rmVersion, bool ordered, string endpointSuffix)
    {
        ChannelFactory<IWcfReliableService> factory = null;
        IWcfReliableService serviceProxy = null;
        WSHttpBinding binding = null;

        try
        {
            // *** SETUP *** \\
            binding = new WSHttpBinding(SecurityMode.None, true);
            binding.ReliableSession.Ordered = ordered;
            var customBinding = new CustomBinding(binding);
            var reliableSessionBindingElement = customBinding.Elements.Find<ReliableSessionBindingElement>();
            reliableSessionBindingElement.ReliableMessagingVersion = rmVersion;
            factory = new ChannelFactory<IWcfReliableService>(customBinding, new EndpointAddress(Endpoints.ReliableSession_WSHttp + endpointSuffix));
            var handlerFactoryBehavior = new HttpMessageHandlerBehavior();
            bool delayNextCall = false;
            int callCount = 0;
            TaskCompletionSource<object> tcs1 = null, tcs2 = null;
            handlerFactoryBehavior.OnSendingAsync = async (request, token) =>
            {
                Interlocked.Increment(ref callCount);
                // Once the delayNextCall latch is set, the next call will be held back until after
                // it has been retried.
                if (delayNextCall)
                {
                    delayNextCall = false;
                    tcs1 = new TaskCompletionSource<object>();
                    await tcs1.Task;
                }
                return null;
            };
            handlerFactoryBehavior.OnSentAsync = (response, token) =>
            {
                if (tcs2 != null)
                {
                    // Let the main test code know that the original held back call has returned from the service
                    tcs2.TrySetResult(null);
                }
                if (tcs1 != null)
                {
                    // This is the retry of the first service call. Release the held back initial call
                    tcs1.TrySetResult(null);
                    tcs1 = null;
                    tcs2 = new TaskCompletionSource<object>();
                }
                return Task.FromResult(response);
            };
            factory.Endpoint.Behaviors.Add(handlerFactoryBehavior);
            serviceProxy = factory.CreateChannel();
            // *** EXECUTE *** \\
            ((IClientChannel)serviceProxy).Open(); // This will establish a reliable session
            delayNextCall = true;
            // Reset call count as it would have incremented in the session open handshake
            callCount = 0;
            var result1 = await serviceProxy.GetNextNumberAsync();
            // Wait for the first attempt for first call to complete before making second call
            await tcs2.Task;
            // This check ensures that the sequence number on the retry was the same as the original. If they
            // were different, the call on the retry would have been dispatched on the service and an extra
            // increment would have happened.
            var result2 = await serviceProxy.GetNextNumberAsync();
            // *** VALIDATE *** \\
            Assert.Equal(1, result1);
            Assert.Equal(2, result2);
            // Validate that 3 http calls were made as first call should have retried.
            Assert.Equal(3, callCount);

            // *** CLEANUP *** \\
            ((IClientChannel)serviceProxy).Close();
            factory.Close();
            ((ICommunicationObject)serviceProxy).Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfTheory]
    [MemberData(nameof(GetTestVariations))]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static async Task RetryCountApplied(ReliableMessagingVersion rmVersion, bool ordered, string endpointSuffix)
    {
        ChannelFactory<IWcfReliableService> factory = null;
        IWcfReliableService serviceProxy = null;
        WSHttpBinding binding = null;

        try
        {
            // *** SETUP *** \\
            binding = new WSHttpBinding(SecurityMode.None, true);
            binding.ReliableSession.Ordered = ordered;
            var customBinding = new CustomBinding(binding);
            var reliableSessionBindingElement = customBinding.Elements.Find<ReliableSessionBindingElement>();
            reliableSessionBindingElement.MaxRetryCount = 2;
            reliableSessionBindingElement.ReliableMessagingVersion = rmVersion;
            factory = new ChannelFactory<IWcfReliableService>(customBinding, new EndpointAddress(Endpoints.ReliableSession_WSHttp + endpointSuffix));
            var handlerFactoryBehavior = new HttpMessageHandlerBehavior();
            bool delayNextCall = false;
            int httpRequestCount = 0;
            Stopwatch sw = null;
            TaskCompletionSource<object> tcs1 = null, tcs2 = new TaskCompletionSource<object>();
            handlerFactoryBehavior.OnSendingAsync = async (request, token) =>
            {
                Interlocked.Increment(ref httpRequestCount);
                // Once the delayNextCall latch is set, all subsequent calls will be on hold until tcs1 is completed.
                if (delayNextCall)
                {
                    if (tcs1 == null) // First delayed call
                    {
                        sw = Stopwatch.StartNew();
                        tcs1 = new TaskCompletionSource<object>();
                    }
                    else if (sw.IsRunning)
                    {
                        sw.Stop(); // Get time between initial call and 1st retry
                        tcs2.TrySetResult(null); // Signal main test code that stopwatch measurement taken
                    }
                    // All calls will wait on the same TCS as trying to trigger retry;
                    await tcs1.Task;
                }
                return null;
            };
            factory.Endpoint.Behaviors.Add(handlerFactoryBehavior);
            serviceProxy = factory.CreateChannel();
            // *** EXECUTE *** \\
            ((IClientChannel)serviceProxy).Open(); // This will establish a reliable session
            delayNextCall = true;
            // Reset request count as it would have incremented in the session open handshake
            httpRequestCount = 0;
            var resultTask = serviceProxy.GetNextNumberAsync();
            await tcs2.Task; // Wait for Stopwatch to be stopped

            // *** VALIDATE *** \\
            // There should only be a single retry at this point
            Assert.Equal(2, httpRequestCount);
            // ReliableSessions doubles the wait time between each retry. We know the first retry time. The second retry
            // will be 2X this, then the request will fail after another wait of 4X this delay. We need to wait at LEAST 6X
            // this initial delay for the channel to fault. 10X should be sufficient
            await Task.Delay((int)sw.ElapsedMilliseconds * 10);
            // There should now be the second retry (3 attempts to send the request) as well as the SequenceTerminated fault 
            // making a total of 4 requests
            Assert.Equal(4, httpRequestCount);
            // Release the paused Http requests
            tcs1.TrySetResult(null);
            await Assert.ThrowsAsync<CommunicationException>(() => resultTask);
            Assert.Equal(CommunicationState.Faulted, ((ICommunicationObject)serviceProxy).State);
            ((ICommunicationObject)serviceProxy).Abort(); // Remove from factory so factory doesn't throw when closed.

            // *** CLEANUP *** \\
            factory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfTheory]
    [MemberData(nameof(GetTestVariations))]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static async Task MaxTransferWindowSizeApplied(ReliableMessagingVersion rmVersion, bool ordered, string endpointSuffix)
    {
        ChannelFactory<IWcfReliableService> factory = null;
        IWcfReliableService serviceProxy = null;
        WSHttpBinding binding = null;
        string secondRequestHeaderName = "SecondRequest";

        try
        {
            // *** SETUP *** \\
            binding = new WSHttpBinding(SecurityMode.None, true);
            binding.ReliableSession.Ordered = ordered;
            var customBinding = new CustomBinding(binding);
            var reliableSessionBindingElement = customBinding.Elements.Find<ReliableSessionBindingElement>();
            reliableSessionBindingElement.MaxTransferWindowSize = 1;
            reliableSessionBindingElement.ReliableMessagingVersion = rmVersion;
            factory = new ChannelFactory<IWcfReliableService>(customBinding, new EndpointAddress(Endpoints.ReliableSession_WSHttp + endpointSuffix));
            var handlerFactoryBehavior = new HttpMessageHandlerBehavior();
            bool delayNextCall = false;
            bool secondRequestSent = false;
            TaskCompletionSource<object> tcs1 = null, tcs2 = new TaskCompletionSource<object>();
            handlerFactoryBehavior.OnSendingAsync = async (request, token) =>
            {
                if (request.Headers.Contains(secondRequestHeaderName))
                {
                    secondRequestSent = true;
                }

                // Once the delayNextCall latch is set, all subsequent calls will be on hold until tcs1 is completed.
                if (delayNextCall)
                {
                    if (tcs1 == null) // First delayed call
                    {
                        tcs1 = new TaskCompletionSource<object>();
                        tcs2.TrySetResult(null); // Signal main test code that first request has been attempted
                    }
                    // All calls will wait on the same TCS as trying to prevent requests progressing;
                    await tcs1.Task;
                }
                return null;
            };
            factory.Endpoint.Behaviors.Add(handlerFactoryBehavior);
            serviceProxy = factory.CreateChannel();
            // *** EXECUTE *** \\
            ((IClientChannel)serviceProxy).Open(); // This will establish a reliable session
            delayNextCall = true;
            Stopwatch sw = Stopwatch.StartNew();
            var resultTask1 = serviceProxy.GetNextNumberAsync();
            await tcs2.Task; // Wait for first http request to be attempted
            sw.Stop();
            Task<int> resultTask2;
            using (var scope = new OperationContextScope((IContextChannel)serviceProxy))
            {
                // Add marker to second request so we can check if it's been seen by handler
                var httpRequestMessageProperty = new HttpRequestMessageProperty();
                httpRequestMessageProperty.Headers.Add(secondRequestHeaderName, secondRequestHeaderName);
                OperationContext.Current.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, httpRequestMessageProperty);
                resultTask2 = serviceProxy.GetNextNumberAsync();
            }

            // Wait 6 times the amount of time it took for the first http request to be made to ensure we've allowed
            // enough time that the second request should have happened by now.
            await Task.Delay((int)sw.ElapsedMilliseconds * 6);
            var secondRequestBlocked = !secondRequestSent;
            tcs1.TrySetResult(null); // Release first request
            int result1 = await resultTask1;
            int result2 = await resultTask2;

            // *** VALIDATE *** \\
            Assert.Equal(1, result1);
            Assert.Equal(2, result2);
            Assert.True(secondRequestBlocked); // Captured before releasing the first request
            Assert.True(secondRequestSent); // Validate that header was seen

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

    [WcfTheory]
    [MemberData(nameof(GetTestVariations))]
    [Condition(nameof(Skip_CoreWCFService_FailedTest))]
    [OuterLoop]
    public static async Task InactivityTimeoutApplied(ReliableMessagingVersion rmVersion, bool ordered, string endpointSuffix)
    {
        ChannelFactory<IWcfReliableService> factory = null;
        IWcfReliableService serviceProxy = null;
        WSHttpBinding binding = null;
        TimeSpan keepAliveInterval = TimeSpan.FromSeconds(2);

        try
        {
            // *** SETUP *** \\
            binding = new WSHttpBinding(SecurityMode.None, true);
            binding.ReliableSession.Ordered = ordered;
            var customBinding = new CustomBinding(binding);
            var reliableSessionBindingElement = customBinding.Elements.Find<ReliableSessionBindingElement>();
            // Keepalive is sent after half the inactivity timeout duration has passed
            reliableSessionBindingElement.InactivityTimeout = keepAliveInterval * 2;
            reliableSessionBindingElement.ReliableMessagingVersion = rmVersion;
            factory = new ChannelFactory<IWcfReliableService>(customBinding, new EndpointAddress(Endpoints.ReliableSession_WSHttp + endpointSuffix));
            var handlerFactoryBehavior = new HttpMessageHandlerBehavior();
            int httpRequestCount = 0;
            handlerFactoryBehavior.OnSendingAsync = (request, token) =>
            {
                Interlocked.Increment(ref httpRequestCount);
                return Task.FromResult<HttpResponseMessage>(null);
            };
            factory.Endpoint.Behaviors.Add(handlerFactoryBehavior);
            serviceProxy = factory.CreateChannel();
            // *** EXECUTE *** \\
            ((IClientChannel)serviceProxy).Open(); // This will establish a reliable session
            var result1 = await serviceProxy.GetNextNumberAsync();
            httpRequestCount = 0;
            // Wait a little longer than (inactivity timeout / 2) to allow a keepalive request
            await Task.Delay(keepAliveInterval * 1.1);
            int httpRequestCountAfterInactivity = httpRequestCount;

            // *** VALIDATE *** \\
            Assert.Equal(1, result1);
            Assert.Equal(1, httpRequestCountAfterInactivity); // Captured before releasing the first request

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

    public static IEnumerable<object[]> GetTestVariations()
    {
        yield return new object[] { ReliableMessagingVersion.WSReliableMessaging11, true, "Ordered_" + ReliableMessagingVersion.WSReliableMessaging11.ToString() };
        yield return new object[] { ReliableMessagingVersion.WSReliableMessaging11, false, "Unordered_" + ReliableMessagingVersion.WSReliableMessaging11.ToString() };
        yield return new object[] { ReliableMessagingVersion.WSReliableMessagingFebruary2005, true, "Ordered_" + ReliableMessagingVersion.WSReliableMessagingFebruary2005.ToString() };
        yield return new object[] { ReliableMessagingVersion.WSReliableMessagingFebruary2005, false, "Unordered_" + ReliableMessagingVersion.WSReliableMessagingFebruary2005.ToString() };
    }
}
