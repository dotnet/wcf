// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace WcfService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class WSRequestReplyService : IWSRequestReplyService
    {
        private static string ContentToReplace = "ContentToReplace";
        private static string ResponseReplaceThisContent = "ResponseReplaceThisContent";
        private static string ReplacedContent = "ReplacedContent";
        private static string RemoteEndpointMessagePropertyFailure = "RemoteEndpointMessageProperty did not contain the address of this machine.";

        static List<string> log = new List<string>();
        static int seed = DateTime.Now.Millisecond;
        static Random rand = new Random(seed);
        FlowControlledStream localStream;

        public void UploadData(string data)
        {
            if (data.Contains(ContentToReplace) || data.Contains(ReplacedContent) || data.Contains(ResponseReplaceThisContent))
            {
                log.Add(string.Format("UploadData received {0}", data));
            }
            else
            {
                log.Add(string.Format("UploadData received {0} length string.", data.Length));
            }

            // Access the RemoteEndpointMessageProperty
            RemoteEndpointMessageProperty remp = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            bool success = false;

            // Get a collection of all IP addresses on the server.
            // Getting the addresses from the Unicast IPAddress Information collection ensures that a match
            // will be found regardless of whether the RemoteEndpointMessageProperty resolves to an IPv4 or an IPv6 address.
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                UnicastIPAddressInformationCollection addressCollection = properties.UnicastAddresses;
                foreach (UnicastIPAddressInformation address in addressCollection)
                {
                    if (remp.Address == address.Address.ToString())
                    {
                        success = true;
                        break;
                    }
                }
            }

                if (!success)
            {
                log.Add(String.Format(RemoteEndpointMessagePropertyFailure + " Expected to find: {0}", remp.Address));
            }
        }

        public string DownloadData()
        {
            string data = CreateInterestingString(rand.Next(512, 4096));
            log.Add(string.Format("DownloadData returning {0} length string.", data.Length));
            return data;
        }

        public void UploadStream(Stream stream)
        {
            int readResult;
            int bytesRead = 0;
            byte[] buffer = new byte[1000];
            do
            {
                readResult = stream.Read(buffer, 0, buffer.Length);
                bytesRead += readResult;
            }
            while (readResult != 0);

            stream.Close();

            log.Add(string.Format("UploadStream read {0} bytes from the client's stream.", bytesRead));
        }

        public Stream DownloadStream()
        {
            log.Add("DownloadStream");
            localStream = new FlowControlledStream();
            localStream.ReadThrottle = TimeSpan.FromMilliseconds(500);
            localStream.StreamDuration = TimeSpan.FromSeconds(1);

            return localStream;
        }

        public Stream DownloadCustomizedStream(TimeSpan readThrottle, TimeSpan streamDuration)
        {
            log.Add("DownloadStream");
            localStream = new FlowControlledStream();
            localStream.ReadThrottle = readThrottle;
            localStream.StreamDuration = streamDuration;

            return localStream;
        }

        public void ThrowingOperation(Exception exceptionToThrow)
        {
            log.Add("ThrowingOperation");
            throw exceptionToThrow;
        }

        public string DelayOperation(TimeSpan delay)
        {
            log.Add("DelayOperation");
            Thread.CurrentThread.Join(delay);
            return "Done with delay.";
        }

        public List<string> GetLog()
        {
            return log;
        }

        public static string CreateInterestingString(int length)
        {
            char[] chars = new char[length];
            int index = 0;

            // Arrays of odd length will start with a single char.
            // The rest of the entries will be surrogate pairs.
            if (length % 2 == 1)
            {
                chars[index] = 'a';
                index++;
            }

            // Fill remaining entries with surrogate pairs
            int seed = DateTime.Now.Millisecond;
            // Log.Info("Seed for CreateInterestingCharArray = {0}", seed);
            Random rand = new Random(seed);
            char highSurrogate;
            char lowSurrogate;

            while (index < length)
            {
                highSurrogate = Convert.ToChar(rand.Next(0xD800, 0xDC00));
                lowSurrogate = Convert.ToChar(rand.Next(0xDC00, 0xE000));

                chars[index] = highSurrogate;
                ++index;
                chars[index] = lowSurrogate;
                ++index;
            }

            return new string(chars, 0, chars.Length);
        }
    }
}
