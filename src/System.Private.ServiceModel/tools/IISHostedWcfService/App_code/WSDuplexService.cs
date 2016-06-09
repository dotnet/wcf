// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace WcfService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Reentrant)]
    public class WSDuplexService : IWSDuplexService
    {
        private static string s_contentToReplace = "ContentToReplace";
        private static string s_responseReplaceThisContent = "ResponseReplaceThisContent";
        private static string s_replacedContent = "ReplacedContent";
        private static string s_lastMessage = "LastMessage";

        private string _exceptionstring = string.Empty;
        private List<string> _log = new List<string>();
        private static bool s_continuePushingData;

        private static int s_seed = DateTime.Now.Millisecond;
        private static Random s_rand = new Random(s_seed);

        private static FlowControlledStream s_localStream;

        public Stream EchoStream(Stream stream)
        {
            IPushCallback pushCallbackChannel = OperationContext.Current.GetCallbackChannel<IPushCallback>();
            return pushCallbackChannel.EchoStream(stream);
        }

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
        }

        public string DownloadData()
        {
            string data = CreateInterestingString(s_rand.Next(512, 4096));
            _log.Add(string.Format("DownloadData returning {0} length string", data.Length));
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

            _log.Add(string.Format("UploadStream read {0} bytes from client's stream", bytesRead));
        }

        // Not using the localStream because this is the request-reply operation.
        public Stream DownloadStream()
        {
            _log.Add("DownloadStream");
            FlowControlledStream stream = new FlowControlledStream();
            stream.StreamDuration = TimeSpan.FromSeconds(1);
            stream.ReadThrottle = TimeSpan.FromMilliseconds(500);
            return stream;
        }

        public void StartPushingData()
        {
            _log.Add("StartPushingData");
            s_continuePushingData = true;
            IPushCallback pushCallbackChannel = OperationContext.Current.GetCallbackChannel<IPushCallback>();
            Task.Factory.StartNew(PushData, pushCallbackChannel, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public void StopPushingData()
        {
            _log.Add("StopPushingData");
            s_continuePushingData = false;
        }

        public void StartPushingStream()
        {
            _log.Add("StartPushingStream");
            IPushCallback pushCallbackChannel = OperationContext.Current.GetCallbackChannel<IPushCallback>();
            Task.Factory.StartNew(PushStream, pushCallbackChannel, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public void StartPushingStreamLongWait()
        {
            _log.Add("StartPushingStream");
            IPushCallback pushCallbackChannel = OperationContext.Current.GetCallbackChannel<IPushCallback>();
            Task.Factory.StartNew(PushStreamLongwait, pushCallbackChannel, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        public void StopPushingStream()
        {
            _log.Add("StopPushingStream");
            s_localStream.StopStreaming = true;
        }

        private void PushData(object state)
        {
            IPushCallback pushCallbackChannel = state as IPushCallback;

            do
            {
                pushCallbackChannel.ReceiveData(CreateInterestingString(s_rand.Next(4, 256)));
            }
            while (s_continuePushingData);

            pushCallbackChannel.ReceiveData(s_lastMessage);
        }

        private void PushStream(object state)
        {
            IPushCallback pushCallbackChannel = state as IPushCallback;
            s_localStream = new FlowControlledStream();
            s_localStream.ReadThrottle = TimeSpan.FromMilliseconds(800);

            pushCallbackChannel.ReceiveStream(s_localStream);
        }

        private void PushStreamLongwait(object state)
        {
            IPushCallback pushCallbackChannel = state as IPushCallback;
            s_localStream = new FlowControlledStream();
            s_localStream.ReadThrottle = TimeSpan.FromMilliseconds(3000);
            s_localStream.StreamDuration = TimeSpan.FromSeconds(2);

            try
            {
                pushCallbackChannel.ReceiveStreamWithException(s_localStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Server got the following exception: {0}", ex));
                _exceptionstring = ex.GetType().Name;
            }
        }

        /// <summary>
        /// This method was used to pass the exception message caught in the PushStreamLongwait method to the client
        /// </summary>
        /// <returns></returns>
        public string GetExceptionString()
        {
            return _exceptionstring;
        }

        public void GetLog()
        {
            IPushCallback pushCallbackChannel = OperationContext.Current.GetCallbackChannel<IPushCallback>();
            pushCallbackChannel.ReceiveLog(_log);
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
