// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infrastructure.Common;
using Xunit;

public static class HttpStreamingAbortTests
{
    [WcfFact]
    [OuterLoop]
    public static void HttpStreaming_Abort_During_Response_Receiving()
    {
        // This test validates that calling Abort() on an HTTP channel works correctly
        // when the channel is in the middle of receiving a streamed response.
        // If Abort() doesn't propagate correctly, the test will timeout with a TimeoutException.
        // If Abort() works correctly, a CommunicationObjectAbortedException should be thrown.

        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        CustomBinding binding = null;
        Stream responseStream = null;
        Exception caughtException = null;

        try
        {
            // *** SETUP *** \\
            // Create a binding with streamed transfer mode
            binding = new CustomBinding(
                new TextMessageEncodingBindingElement(),
                new HttpTransportBindingElement
                {
                    TransferMode = TransferMode.StreamedResponse,
                    // Set a short SendTimeout so that if Abort() doesn't work,
                    // we'll get a TimeoutException relatively quickly
                    MaxReceivedMessageSize = 1024 * 1024 // 1 MB
                });

            // Set a reasonable SendTimeout - if Abort() doesn't work, this will cause a timeout
            binding.SendTimeout = TimeSpan.FromSeconds(10);

            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.CustomTextEncoderStreamed_Address));
            serviceProxy = factory.CreateChannel();

            // Create a large string to ensure the response takes time to read
            string testString = new string('a', 500000); // 500KB of data

            // *** EXECUTE *** \\
            // Start the call to get a stream response
            responseStream = serviceProxy.GetStreamFromString(testString);

            // Start reading a small amount from the stream to ensure we're in the receiving phase
            byte[] buffer = new byte[1024];
            int bytesRead = responseStream.Read(buffer, 0, buffer.Length);

            // Verify we actually received some data
            Assert.True(bytesRead > 0, "Expected to read some data from the stream");

            // Now abort the channel while we're in the middle of receiving the response
            // This should cause the ongoing read operation to be cancelled
            ((ICommunicationObject)serviceProxy).Abort();

            // Try to continue reading from the stream
            // If Abort() works correctly, this should throw an exception
            // If Abort() doesn't work, this will hang until the SendTimeout expires
            try
            {
                while (responseStream.Read(buffer, 0, buffer.Length) > 0)
                {
                    // Keep reading
                }
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            // *** VALIDATE *** \\
            // We expect an exception to be thrown after Abort() is called
            Assert.NotNull(caughtException);

            // The exception should be related to the communication object being aborted
            // It could be CommunicationObjectAbortedException or an IOException wrapping it
            Assert.True(
                caughtException is CommunicationObjectAbortedException ||
                caughtException is IOException ||
                caughtException is CommunicationException,
                $"Expected CommunicationObjectAbortedException, IOException, or CommunicationException, but got: {caughtException.GetType().Name}");
        }
        catch (TimeoutException)
        {
            // If we get a TimeoutException, it means Abort() didn't work correctly
            Assert.Fail("Test timed out, which indicates that Abort() did not properly cancel the ongoing stream read operation.");
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            responseStream?.Dispose();
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static async Task HttpStreaming_Abort_During_Async_Response_Receiving()
    {
        // This test validates that calling Abort() on an HTTP channel works correctly
        // when the channel is in the middle of receiving a streamed response asynchronously.
        // If Abort() doesn't propagate correctly, the test will timeout with a TimeoutException.
        // If Abort() works correctly, a CommunicationObjectAbortedException should be thrown.

        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        CustomBinding binding = null;
        Stream responseStream = null;
        Exception caughtException = null;

        try
        {
            // *** SETUP *** \\
            // Create a binding with streamed transfer mode
            binding = new CustomBinding(
                new TextMessageEncodingBindingElement(),
                new HttpTransportBindingElement
                {
                    TransferMode = TransferMode.StreamedResponse,
                    MaxReceivedMessageSize = 1024 * 1024 // 1 MB
                });

            // Set a reasonable SendTimeout - if Abort() doesn't work, this will cause a timeout
            binding.SendTimeout = TimeSpan.FromSeconds(10);

            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.CustomTextEncoderStreamed_Address));
            serviceProxy = factory.CreateChannel();

            // Create a large string to ensure the response takes time to read
            string testString = new string('a', 500000); // 500KB of data

            // *** EXECUTE *** \\
            // Start the call to get a stream response
            responseStream = serviceProxy.GetStreamFromString(testString);

            // Start reading a small amount from the stream to ensure we're in the receiving phase
            byte[] buffer = new byte[1024];
            int bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);

            // Verify we actually received some data
            Assert.True(bytesRead > 0, "Expected to read some data from the stream");

            // Now abort the channel while we're in the middle of receiving the response
            ((ICommunicationObject)serviceProxy).Abort();

            // Try to continue reading from the stream asynchronously
            // If Abort() works correctly, this should throw an exception
            // If Abort() doesn't work, this will hang until the SendTimeout expires
            try
            {
                while ((await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    // Keep reading
                }
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            // *** VALIDATE *** \\
            // We expect an exception to be thrown after Abort() is called
            Assert.NotNull(caughtException);

            // The exception should be related to the communication object being aborted
            // It could be CommunicationObjectAbortedException or an IOException wrapping it
            Assert.True(
                caughtException is CommunicationObjectAbortedException ||
                caughtException is IOException ||
                caughtException is CommunicationException ||
                caughtException is OperationCanceledException,
                $"Expected CommunicationObjectAbortedException, IOException, CommunicationException, or OperationCanceledException, but got: {caughtException.GetType().Name}");
        }
        catch (TimeoutException)
        {
            // If we get a TimeoutException, it means Abort() didn't work correctly
            Assert.Fail("Test timed out, which indicates that Abort() did not properly cancel the ongoing stream read operation.");
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\
            responseStream?.Dispose();
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)serviceProxy, factory);
        }
    }
}
