// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.


using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Extensibility.MessageEncoder.Tests;
using Infrastructure.Common;
using Xunit;

public static class TextTests
{
    [WcfFact]
    [OuterLoop]
    public static void CustomTextMessageEncoder_Http_RequestReply_Buffered()
    {
        ChannelFactory<IWcfService> channelFactory = null;
        IWcfService client = null;
        string testString = "Hello";

        try
        {
            // *** SETUP *** \\
            CustomBinding binding = new CustomBinding(new CustomTextMessageBindingElement(Encoding.UTF8.WebName),
                new HttpTransportBindingElement
                {
                    MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                    MaxBufferSize = ScenarioTestHelpers.SixtyFourMB
                });

            channelFactory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.CustomTextEncoderBuffered_Address));
            client = channelFactory.CreateChannel();

            // *** EXECUTE *** \\
            string result = client.Echo(testString);

            // *** VALIDATE *** \\
            Assert.Equal(result, testString);

            // *** CLEANUP *** \\
            ((ICommunicationObject)client).Close();
            channelFactory.Close();
        }
        finally
        {
            // *** ENSURE CLEANUP *** \\  
            ScenarioTestHelpers.CloseCommunicationObjects((ICommunicationObject)client, channelFactory);
        }
    }

    [WcfFact]
    [OuterLoop]
    public static void CustomTextMessageEncoder_Http_RequestReply_Streamed()
    {
        // 84K, larger than any buffers, but won't allocate in LOH
        int streamKBytes = 84;
        int streamLength = 1024 * streamKBytes;
        int lowestPrintable = ' ';
        int printableRange = '~' - lowestPrintable;
        ChannelFactory<IWcfService> factory = null;
        IWcfService serviceProxy = null;
        Stream stream = null;

        try
        {
            // *** SETUP *** \\
            CustomBinding binding = new CustomBinding(new CustomTextMessageBindingElement(Encoding.UTF8.WebName),
                new HttpTransportBindingElement
                {
                    MaxReceivedMessageSize = ScenarioTestHelpers.SixtyFourMB,
                    MaxBufferSize = ScenarioTestHelpers.SixtyFourMB,
                    TransferMode = TransferMode.Streamed
                });

            factory = new ChannelFactory<IWcfService>(binding, new EndpointAddress(Endpoints.CustomTextEncoderStreamed_Address));
            serviceProxy = factory.CreateChannel();
            byte[] requestBytes = new byte[streamLength];
            RandomNumberGenerator rnd = RandomNumberGenerator.Create();
            int pos = 0;
            for (int i = 0; i < streamKBytes; i++)
            {
                byte[] tempBuffer = new byte[1024];
                rnd.GetBytes(tempBuffer);
                for (int j = 0; j < 1024; j++)
                {
                    byte val = tempBuffer[j];
                    if (val < ' ' || val > '~')
                    {
                        // Force the value to be between ' ' and '~'
                        int temp1 = val % printableRange;
                        val = (byte)(temp1 + lowestPrintable);
                    }

                    requestBytes[pos++] = val;
                }
            }
            stream = new MemoryStream(requestBytes);

            // *** EXECUTE *** \\
            var returnStream = serviceProxy.EchoStream(stream);

            // *** VALIDATE *** \\
            MemoryStream ms = new MemoryStream(streamLength);
            returnStream.CopyTo(ms);

            Assert.True(streamLength == ms.Length,
                        String.Format("Expected returned stream length = {0}, actual = {1}",
                                        streamLength, ms.Length));

            ArraySegment<byte> returnedByteArraySegment;
            ms.TryGetBuffer(out returnedByteArraySegment);
            Assert.True(requestBytes.SequenceEqual(returnedByteArraySegment.Array), "Returned bytes are different than sent bytes");

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
