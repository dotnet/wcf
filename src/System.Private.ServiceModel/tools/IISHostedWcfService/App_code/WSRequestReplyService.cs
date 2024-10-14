// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if NET
using CoreWCF;
using CoreWCF.Channels;
#else
using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
#endif
using System.Net.NetworkInformation;

namespace WcfService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class WSRequestReplyService : IWSRequestReplyService
    {
        private static string s_contentToReplace = "ContentToReplace";
        private static string s_responseReplaceThisContent = "ResponseReplaceThisContent";
        private static string s_replacedContent = "ReplacedContent";
        private static string s_remoteEndpointMessagePropertyFailure = "RemoteEndpointMessageProperty did not contain the address of this machine.";

        private List<string> _log = new List<string>();
        private Random _rand = new Random(DateTime.Now.Millisecond);
        private FlowControlledStream _localStream;

        public void UploadData(string data)
        {
            if (data.Contains(s_contentToReplace) || data.Contains(s_replacedContent) || data.Contains(s_responseReplaceThisContent))
            {
                _log.Add(string.Format("UploadData received {0}", data));
            }
            else
            {
                _log.Add(string.Format("UploadData received {0} length string.", data.Length));
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
                _log.Add(String.Format(s_remoteEndpointMessagePropertyFailure + " Expected to find: {0}", remp.Address));
            }
        }

        public string DownloadData()
        {
            string data = CreateInterestingString(_rand.Next(512, 4096));
            _log.Add(string.Format("DownloadData returning {0} length string.", data.Length));
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

            _log.Add(string.Format("UploadStream read {0} bytes from the client's stream.", bytesRead));
        }

        public Stream DownloadStream()
        {
            _log.Add("DownloadStream");
            _localStream = new FlowControlledStream();
            _localStream.ReadThrottle = TimeSpan.FromMilliseconds(500);
            _localStream.StreamDuration = TimeSpan.FromSeconds(1);

            return _localStream;
        }

        public Stream DownloadCustomizedStream(TimeSpan readThrottle, TimeSpan streamDuration)
        {
            _log.Add("DownloadStream");
            _localStream = new FlowControlledStream();
            _localStream.ReadThrottle = readThrottle;
            _localStream.StreamDuration = streamDuration;

            return _localStream;
        }

        public void ThrowingOperation(Exception exceptionToThrow)
        {
            _log.Add("ThrowingOperation");
            throw exceptionToThrow;
        }

        public string DelayOperation(TimeSpan delay)
        {
            _log.Add("DelayOperation");
            Thread.CurrentThread.Join(delay);
            return "Done with delay.";
        }

        public List<string> GetLog()
        {
            return _log;
        }

        public string CreateInterestingString(int length)
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
            char highSurrogate;
            char lowSurrogate;

            while (index < length)
            {
                highSurrogate = Convert.ToChar(_rand.Next(0xD800, 0xDC00));
                lowSurrogate = Convert.ToChar(_rand.Next(0xDC00, 0xE000));

                chars[index] = highSurrogate;
                chars[index + 1] = lowSurrogate;
                index += 2;
            }

            return new string(chars, 0, chars.Length);
        }
    }
}
